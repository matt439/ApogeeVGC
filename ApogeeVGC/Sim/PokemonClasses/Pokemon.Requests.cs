using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
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
        // Build stats dictionary from base stored stats
        var stats = new StatsTable
        {
            [StatId.Atk] = BaseStoredStats[StatId.Atk],
            [StatId.Def] = BaseStoredStats[StatId.Def],
            [StatId.SpA] = BaseStoredStats[StatId.SpA],
            [StatId.SpD] = BaseStoredStats[StatId.SpD],
            [StatId.Spe] = BaseStoredStats[StatId.Spe],
        };

        // Get move list - either base moves (for allies) or current moves
        var moveSource = forAlly ? BaseMoveSlots : MoveSlots;

        // Convert move slots to Move objects
        var moves = moveSource.Select(moveSlot => Battle.Library.Moves[moveSlot.Id]).ToList();

        if (GetHealth().Secret is not SecretConditionId secretCondition)
        {
            secretCondition = new SecretConditionId(ConditionId.None);
        }

        // Create the base entry
        var entry = new PokemonSwitchRequestData
        {
            Ident = Fullname,
            Details = Details.ToString(),
            Condition = secretCondition.Value,
            Active = IsActive, // Use IsActive property instead of position check
            Stats = stats,
            Moves = moves,
            BaseAbility = Battle.Library.Abilities[BaseAbility],
            Item = Battle.Library.Items[Item],
            Pokeball = Pokeball,
            // Default values for Gen 9+ fields
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
        Battle.Debug($"[GetMoveRequestData] {Name}: Has ChoiceLock={Volatiles.ContainsKey(ConditionId.ChoiceLock)}, Item={Item}");

        // Clear disabled states before evaluating them for this turn
        // This ensures disabled states are re-evaluated fresh each turn
        foreach (var moveSlot in MoveSlots)
        {
            moveSlot.Disabled = false;
            moveSlot.DisabledSource = null;
        }

        // Trigger DisableMove event to re-apply disabled states
        // This allows conditions like ChoiceLock and items like Assault Vest
        // to disable appropriate moves
        Battle.RunEvent(EventId.DisableMove, this);

        Battle.Debug($"[GetMoveRequestData] {Name}: After DisableMove event, move states:");
        foreach (var moveSlot in MoveSlots)
        {
            Battle.Debug($"  - {Battle.Library.Moves[moveSlot.Id].Name}: Disabled={moveSlot.Disabled}");
        }

        // Get locked move if Pokemon is not maybe-locked
        var lockedMove = MaybeLocked == true ? null : GetLockedMove();

        // Information should be restricted for the last active Pokemon
        bool isLastActive = IsLastActive();
        int canSwitchIn = Battle.CanSwitch(Side);
        var moves = GetMoves(lockedMove, isLastActive);

        // If no moves available, default to Struggle
        if (moves.Count == 0)
        {
            Move struggleMove = Battle.Library.Moves[MoveId.Struggle];
            moves =
            [
                new PokemonMoveData
                {
                    Move = struggleMove,
                    Target = null,
                    Disabled = null,
                    DisabledSource = null,
                    Pp = struggleMove.BasePp,
                    MaxPp = struggleMove.BasePp,
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