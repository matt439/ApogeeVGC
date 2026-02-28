using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    public bool HasType(PokemonType type)
    {
        return GetTypes().Contains(type);
    }

    public bool HasType(PokemonType[] types)
    {
        var myTypes = GetTypes();
        foreach (var t in types)
        {
            if (myTypes.AsSpan().Contains(t)) return true;
        }
        return false;
    }

    /// <summary>
    /// Sets a type (except on Arceus/Silvally, who resist type changes)
    /// </summary>
    public bool SetType(PokemonType type, bool enforce = false)
    {
        return SetType([type], enforce);
    }

    /// <summary>
    /// Sets types (except on Arceus/Silvally, who resist type changes)
    /// </summary>
    public bool SetType(PokemonType[] types, bool enforce = false)
    {
        if (!enforce)
        {
            // First type of Arceus, Silvally cannot be normally changed
            if ((Battle.Gen >= 5 && Species.Num is 493 or 773) ||
                (Battle.Gen == 4 && HasAbility(AbilityId.Multitype)))
            {
                return false;
            }

            // Terastallized Pokemon cannot have their base type changed except via forme change
            if (Terastallized != null)
            {
                return false;
            }
        }

        // Validate that types array is not empty
        if (types.Length == 0)
        {
            throw new ArgumentException("Must pass type to SetType");
        }

        // Set the new types
        Types = new List<PokemonType>(types);

        // Clear any added type
        AddedType = null;

        // Mark type as known
        KnownType = true;

        // Update apparent type for display (join types with '/')
        ApparentType = new List<PokemonType>(Types);

        return true;
    }

    public bool AddType(PokemonType newType)
    {
        if (Terastallized is not null) return false;
        AddedType = newType;
        return true;
    }

    public PokemonType[] GetTypes(bool? excludeAdded = null, bool? preterastallized = null)
    {
        // Handle Terastallization - match TypeScript's !preterastallized logic
        if (preterastallized != true && Terastallized is not null && Terastallized != MoveType.Stellar)
        {
            return [Terastallized.Value.ConvertToPokemonType()];
        }

        // Run Type event to allow abilities/items/conditions to modify types
        RelayVar? rv = Battle.RunEvent(EventId.Type, this, null, null, Types);

        IList<PokemonType> sourceTypes = rv is TypesRelayVar typesRelayVar
            ? typesRelayVar.Types
            : Types;

        bool needsFallback = sourceTypes.Count == 0;
        bool hasAddedType = excludeAdded != true && AddedType is not null;

        // Compute final size and build result array directly (no intermediate List)
        int count = needsFallback ? 1 : sourceTypes.Count;
        if (hasAddedType) count++;

        var result = new PokemonType[count];
        int idx = 0;

        if (needsFallback)
        {
            result[idx++] = Battle.Gen >= 5 ? PokemonType.Normal : PokemonType.Unknown;
        }
        else
        {
            for (int i = 0; i < sourceTypes.Count; i++)
            {
                result[idx++] = sourceTypes[i];
            }
        }

        if (hasAddedType)
        {
            result[idx] = AddedType!.Value;
        }

        return result;
    }


    /// <summary>
    /// Calculates the type effectiveness for a move against this Pokemon.
    /// Returns a MoveEffectiveness enum value representing the combined effectiveness.
    /// </summary>
    /// <param name="move">The move to check effectiveness for</param>
    /// <returns>MoveEffectiveness enum value (Normal, SuperEffective2X, NotVeryEffective05X, etc.)</returns>
    public MoveEffectiveness RunEffectiveness(ActiveMove move)
    {
        int totalTypeMod = 0;

        // Special case: Stellar-type moves against Terastallized Pokemon are always neutral
        if (Terastallized != null && move.Type == MoveType.Stellar)
        {
            totalTypeMod = 1;
        }
        else
        {
            // Calculate effectiveness against each of the Pokemon's types
            foreach (PokemonType type in GetTypes())
            {
                // Get base effectiveness from ModdedDex (returns MoveEffectiveness enum)
                MoveEffectiveness effectiveness = Battle.Dex.GetEffectiveness(move.Type, type);

                // Convert MoveEffectiveness enum to integer modifier for event system
                int typeMod = effectiveness.ToModifier();

                // Allow SingleEvent to modify effectiveness (e.g., Scrappy ability)
                RelayVar? singleEventResult = Battle.SingleEvent(
                    EventId.Effectiveness,
                    move,
                    null,
                    this,
                    type,
                    move,
                    typeMod
                );

                if (singleEventResult is IntRelayVar singleIrv)
                {
                    typeMod = singleIrv.Value;
                }

                // Allow RunEvent to further modify effectiveness
                RelayVar? runEventResult = Battle.RunEvent(
                    EventId.Effectiveness,
                    this,
                    type,
                    move,
                    typeMod
                );

                if (runEventResult is IntRelayVar runIrv)
                {
                    totalTypeMod += runIrv.Value;
                }
                else
                {
                    totalTypeMod += typeMod;
                }
            }
        }

        // Special handling for Terapagos-Terastal with Tera Shell ability
        if (Species.Id == SpecieId.TerapagosTerastal && HasAbility(AbilityId.TeraShell) &&
            !Battle.SuppressingAbility(this))
        {
            // If ability already triggered, keep effectiveness at not very effective
            if (AbilityState.Resisted == true)
            {
                return MoveEffectiveness.NotVeryEffective05X; // All hits of multi-hit move should be not very effective
            }

            // Tera Shell only activates for damaging moves at full HP
            // It doesn't activate if:
            // - Move is Status category
            // - Move is Struggle
            // - Pokemon is immune to the move
            // - Effectiveness is already negative (resisted)
            // - Pokemon is not at full HP
            if (move.Category == MoveCategory.Status ||
                move.Id == MoveId.Struggle ||
                !RunImmunity(move) ||
                totalTypeMod < 0 ||
                Hp < MaxHp)
            {
                return totalTypeMod.ToMoveEffectiveness();
            }

            // Activate Tera Shell - make move not very effective
            if (Battle.DisplayUi)
            {
                Battle.Add("-activate", this, PartFuncUnion.FromIEffect(Battle.Library.Abilities[AbilityId.TeraShell]));
            }
            AbilityState.Resisted = true;
            return MoveEffectiveness.NotVeryEffective05X;
        }

        // Convert accumulated integer modifier back to MoveEffectiveness enum
        return totalTypeMod.ToMoveEffectiveness();
    }
}