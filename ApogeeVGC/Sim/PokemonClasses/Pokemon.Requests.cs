using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    public bool IsLastActive()
    {
        // If this Pokémon isn't active, it can't be the last active
        if (!IsActive) return false;

        // Get all active Pokémon on this side
        var allyActive = Side.Active;

        // Check all positions after this Pokémon
        for (int i = Position + 1; i < allyActive.Count; i++)
        {
            Pokemon? pokemon = allyActive[i];
            if (pokemon is null) continue;

            // If there's a living Pokémon at a later position, this isn't the last
            if (!pokemon.Fainted)
            {
                return false;
            }
        }

        // No living Pokémon found after this position - this is the last active
        return true;
    }

    /// <summary>
    /// Generates switch request data for this Pokemon, containing all information
    /// needed for a player to make switching decisions (stats, moves, ability, item, etc.)
    /// </summary>
    /// <param name="forAlly">If true, returns base moves instead of current moves (for ally info)</param>
    /// <returns>Pokemon switch request data object</returns>
    public PokemonSwitchRequestData GetSwitchRequestData(bool forAlly = false)
    {
        // Build stats dictionary WITHOUT HP (as per Showdown protocol)
        // HP is tracked in the "condition" field, not in stats
        var stats = new Dictionary<string, int>
        {
            ["atk"] = BaseStoredStats[StatId.Atk],
            ["def"] = BaseStoredStats[StatId.Def],
            ["spa"] = BaseStoredStats[StatId.SpA],
            ["spd"] = BaseStoredStats[StatId.SpD],
            ["spe"] = BaseStoredStats[StatId.Spe],
        };

        // Get move list - either base moves (for allies) or current moves
        var moveSource = forAlly ? BaseMoveSlots : MoveSlots;

        // Convert move slots to Move objects
        var moves = moveSource.Select(moveSlot => Battle.Library.Moves[moveSlot.Id]).ToList();

        // Format the condition string according to Pokemon Showdown format
        string condition;
        if (Fainted || Hp <= 0)
        {
            condition = "0 fnt";
        }
        else
        {
            // Format: "HP/MaxHP" or "HP/MaxHP status"
            condition = $"{Hp}/{BaseMaxHp}";

            // Add status condition if present
            if (Status != ConditionId.None)
            {
                Condition statusCondition = Battle.Library.Conditions[Status];
                condition += $" {statusCondition.Name}";
            }
        }

        // Generate ident (e.g., "p1: Calyrex-Ice" or "p1a: Calyrex-Ice" for active)
        string ident;
        if (IsActive)
        {
            // Active Pokemon include position letter (a, b, c, d)
            char positionLetter = (char)('a' + Position);
            ident = $"{Side.Id.ToString().ToLower()}{positionLetter}: {Name}";
        }
        else
        {
            // Inactive Pokemon don't include position
            ident = $"{Side.Id.ToString().ToLower()}: {Name}";
        }

        // Generate details string (e.g., "Calyrex-Ice, L50, M")
        string details = Details.ToString();

        // Create the base entry - NOTE: Stats will need to be handled specially in serialization
        var entry = new PokemonSwitchRequestData
        {
            Ident = ident,
            Details = details,
            Condition = condition,
            Active = IsActive,
            Stats = stats, // Dictionary without HP
            Moves = moves,
            BaseAbility = Battle.Library.Abilities[BaseAbility],
            Item = Battle.Library.Items[Item],
            Pokeball = Pokeball,
            Ability = Battle.Library.Abilities[Ability],
            Commanding = false,
            Reviving = false,
            TeraType = TeraType,
            Terastallized = false,
        };

        // Gen 7+ includes current ability
        if (Battle.Gen > 6)
        {
            entry = entry with { Ability = Battle.Library.Abilities[Ability] };
        }

        // Gen 9+ includes commanding and reviving status
        if (Battle.Gen >= 9)
        {
            // Commanding: Pokemon has the Commanding volatile and is not fainted
            bool commanding = Volatiles.ContainsKey(ConditionId.Commanding) && !Fainted;

            // Reviving: Pokemon is active and has Revival Blessing slot condition at its position
            bool reviving = IsActive &&
                            Position < Side.SlotConditions.Count &&
                            Side.SlotConditions[Position].ContainsKey(ConditionId.RevivalBlessing);

            entry = entry with
            {
                Commanding = commanding,
                Reviving = reviving,
            };
        }

        // Gen 9 includes Tera type and Terastallized status
        if (Battle.Gen == 9)
        {
            entry = entry with
            {
                TeraType = TeraType,
                Terastallized = Terastallized != null,
            };
        }

        return entry;
    }

    /// <summary>
    /// Generates move request data for this Pokemon, containing information about
    /// available moves, restrictions, and special mechanics for the current turn.
    /// </summary>
    public PokemonMoveRequestData GetMoveRequestData()
    {
        // Get locked move if Pokemon is not maybe-locked
        var lockedMove = MaybeLocked == true ? null : GetLockedMove();

        // Information should be restricted for the last active Pokemon
        bool isLastActive = IsLastActive();
        int canSwitchIn = Battle.CanSwitch(Side);
        var moves = GetMoves(lockedMove, isLastActive);

        // If no moves available, default to Struggle
        if (moves.Count == 0)
        {
            moves =
            [
                new PokemonMoveData
                {
                    Move = Battle.Library.Moves[MoveId.Struggle],
                    Target = null,
                    Disabled = null,
                    DisabledSource = null,
                }
            ];
            lockedMove = MoveId.Struggle;
        }

        // Create base request data
        var data = new PokemonMoveRequestData
        {
            Moves = moves,
        };

        if (isLastActive)
        {
            // Update maybe-disabled/maybe-locked state for last active Pokemon
            MaybeDisabled = MaybeDisabled && lockedMove == null;
            MaybeLocked = MaybeLocked ?? MaybeDisabled;

            if (MaybeDisabled)
            {
                data = data with { MaybeDisabled = true };
            }

            if (MaybeLocked == true)
            {
                data = data with { MaybeLocked = true };
            }

            if (canSwitchIn > 0)
            {
                if (Trapped == PokemonTrapped.True)
                {
                    data = data with { Trapped = true };
                }
                else if (MaybeTrapped)
                {
                    data = data with { MaybeTrapped = true };
                }
            }
        }
        else
        {
            // Reset maybe-disabled/maybe-locked for non-last active Pokemon
            MaybeDisabled = false;
            MaybeLocked = null;

            if (canSwitchIn > 0)
            {
                // Discovered by selecting a valid Pokemon as a switch target and cancelling
                if (Trapped == PokemonTrapped.True)
                {
                    data = data with { Trapped = true };
                }
            }

            MaybeTrapped = false;
        }

        // Handle Terastallization if not locked into a move
        if (lockedMove == null)
        {
            if (CanTerastallize is not null and not FalseMoveTypeFalseUnion)
            {
                data = data with { CanTerastallize = CanTerastallize };
            }
        }

        return data;
    }
}