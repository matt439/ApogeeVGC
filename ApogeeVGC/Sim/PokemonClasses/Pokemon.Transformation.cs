using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    public bool TransformInto(Pokemon pokemon, IEffect? effect = null)
    {
        var species = pokemon.Species;

        // Validation checks
        if (pokemon.Fainted || Illusion != null || pokemon.Illusion != null ||
            (pokemon.Volatiles.ContainsKey(ConditionId.Substitute)) ||
            pokemon.Transformed || Transformed ||
            species.Id == SpecieId.EternatusEternamax ||
            (species.BaseSpecies is SpecieId.Ogerpon or SpecieId.Terapagos &&
             (Terastallized != null || pokemon.Terastallized != null)) ||
            Terastallized == MoveType.Stellar)
        {
            return false;
        }

        // Set species
        if (SetSpecie(species, effect, isTransform: true) == null)
            return false;

        // Mark as transformed and copy weight
        Transformed = true;
        WeightHg = pokemon.WeightHg;

        // Copy types
        var types = pokemon.GetTypes(excludeAdded: true, preterastallized: true);
        if (pokemon.Volatiles.TryGetValue(ConditionId.Roost, out var roostState) && roostState.TypeWas is not null)
        {
            // Use the type stored in Roost volatile
            SetType(roostState.TypeWas, enforce: true);
        }
        else
        {
            SetType(types, enforce: true);
        }

        AddedType = pokemon.AddedType;
        KnownType = IsAlly(pokemon) && pokemon.KnownType;
        ApparentType = [..pokemon.ApparentType];

        // Copy stats (except HP)
        foreach (var stat in Enum.GetValues<StatIdExceptHp>())
        {
            StoredStats[stat] = pokemon.StoredStats[stat];
        }

        // Copy timesAttacked
        TimesAttacked = pokemon.TimesAttacked;

        // Copy moves
        MoveSlots.Clear();
        foreach (var moveSlot in pokemon.MoveSlots)
        {
            var pp = moveSlot.MaxPp == 1 ? 1 : 5;
            var maxPp = moveSlot.MaxPp == 1 ? 1 : 5;

            MoveSlots.Add(new MoveSlot
            {
                Id = moveSlot.Id,
                Move = moveSlot.Move,
                Pp = pp,
                MaxPp = maxPp,
                Target = moveSlot.Target,
                Disabled = false,
                DisabledSource = null,
                Used = false,
                Virtual = true,
            });
        }

        // Copy boosts
        foreach (var boost in Enum.GetValues<BoostId>())
        {
            Boosts.SetBoost(boost, pokemon.Boosts.GetBoost(boost));
        }

        // Copy critical hit volatiles (Gen 6+, so applies to Gen 9)
        ConditionId[] critVolatiles =
        [
            ConditionId.DragonCheer,
            ConditionId.FocusEnergy,
        ];

        // Remove overlapping volatiles first
        foreach (var volatileId in critVolatiles)
        {
            RemoveVolatile(Battle.Library.Conditions[volatileId]);
        }

        // Add them from target
        foreach (var volatileId in critVolatiles)
        {
            if (!pokemon.Volatiles.TryGetValue(volatileId, out var volatileState))
                continue;
            AddVolatile(volatileId);

            if (volatileId == ConditionId.DragonCheer)
            {
                Volatiles[volatileId].HasDragonType = volatileState.HasDragonType;
            }
        }

        // Add battle message
        if (Battle.DisplayUi)
        {
            if (effect != null)
            {
                Battle.Add("-transform", this, pokemon, "[from]",
                    PartFuncUnion.FromIEffect(effect));
            }
            else
            {
                Battle.Add("-transform", this, pokemon);
            }
        }

        // Handle Terastallization display
        if (Terastallized != null)
        {
            KnownType = true;
            ApparentType = [Terastallized.Value.ConvertToPokemonType()];
        }

        // Copy ability
        SetAbility(pokemon.Ability, this, isFromFormeChange: true, isTransform: true);

        // Handle Ogerpon/Terapagos Terastallization restriction
        if (Species.BaseSpecies is SpecieId.Ogerpon or SpecieId.Terapagos &&
            CanTerastallize is not FalseMoveTypeFalseUnion)
        {
            CanTerastallize = MoveTypeFalseUnion.FromFalse();
        }

        return true;
    }

    /// <summary>
    /// Default is Battle.Effect for source.
    /// </summary>
    public Species? SetSpecie(Species rawSpecies, IEffect? source, bool isTransform = false)
    {
        var rv = Battle.RunEvent(EventId.ModifySpecie, this, null, source, rawSpecies);
        if (rv is null) return null;

        if (rv is SpecieRelayVar srv)
        {
            Species = srv.Species;
        }
        else
        {
            throw new InvalidOperationException("species must be a SpecieRelayVar");
        }

        var species = srv.Species;

        SetType(species.Types.ToArray(), true);
        ApparentType = new List<PokemonType>(rawSpecies.Types);
        AddedType = species.AddedType;
        KnownType = true;
        WeightHg = species.WeightHg;

        // Use BattleLevel for stat calculations (respects AdjustLevelDown for VGC)
        var stats = Battle.SpreadModify(Species.BaseStats, Set, BattleLevel);
        if (Species.MaxHp is not null)
        {
            stats.Hp = Species.MaxHp.Value;
        }

        // Always set HP stats during initial setup (MaxHp == 0) or during transformation
        // During transformation, preserve current HP
        if (MaxHp == 0)
        {
            // Initial setup - set all HP values
            BaseMaxHp = stats.Hp;
            MaxHp = stats.Hp;
            Hp = stats.Hp;
        }
        else if (!isTransform)
        {
            // Not a transform - update HP values but preserve HP ratio
            BaseMaxHp = stats.Hp;
            MaxHp = stats.Hp;
            Hp = stats.Hp;
        }
        // else: isTransform == true, don't change HP values

        if (!isTransform) BaseStoredStats = stats;
        foreach (var stat in StatsExceptHpTable.AllStatIds)
        {
            StoredStats[stat] = stats[stat.ConvertToStatId()];
        }

        Speed = StoredStats.Spe;
        return species;
    }

    /// <summary>
    /// Changes this Pokemon's forme to match the given speciesId (or species).
    /// This function handles all changes to stats, ability, type, species, etc.
    /// as well as sending all relevant messages sent to the client.
    /// </summary>
    public bool FormeChange(SpecieId specieId, IEffect? source = null, bool? isPermanent = null,
        SpeciesAbilityType abilitySlot = SpeciesAbilityType.Slot0, string? message = null)
    {
        // Default source to battle effect if not provided
        source ??= Battle.Effect;

        // Get the raw species from the battle library
        var rawSpecies = Battle.Library.Species[specieId];

        // Attempt to set the species
        var species = SetSpecie(rawSpecies, source);
        if (species == null) return false;

        // Determine the species the opponent sees (accounting for Illusion)
        var apparentSpecies = Illusion?.Species.Name ?? species.BaseSpecies.ToString();

        if (isPermanent == true)
        {
            // Update base species for permanent changes
            BaseSpecies = rawSpecies;

            // Update details and send to client
            Details = GetUpdatedDetails();
            var details = (Illusion ?? this).GetUpdatedDetails().ToString();

            // Add Tera type to details if Terastallized
            if (Terastallized != null)
            {
                details += $", tera:{Terastallized.Value}";
            }

            if (Battle.DisplayUi)
            {
                Battle.Add("detailschange", this, details);
            }

            // Update max HP based on new species
            UpdateMaxHp();

            // Handle different source types for permanent changes
            if (source == null)
            {
                // Tera forme (Ogerpon/Terapagos text would go here if needed)
                FormeRegression = true;
            }
            else if (source.EffectType == EffectType.Condition)
            {
                // Status-based forme change (e.g., Shaymin-Sky -> Shaymin)
                if (Battle.DisplayUi)
                {
                    if (message is null)
                    {
                        Battle.Add("-formechange", this, species.Name);
                    }
                    else
                    {
                        Battle.Add("-formechange", this, species.Name, message);
                    }
                }
            }
        }
        else
        {
            // Handle temporary forme changes
            if (Battle.DisplayUi)
            {
                if (source.EffectType == EffectType.Ability)
                {
                    if (message is null)
                    {
                        Battle.Add("-formechange", this, species.Name,
                            $"[from] ability: {source.Name}");
                    }
                    else
                    {
                        Battle.Add("-formechange", this, species.Name, message,
                            $"[from] ability: {source.Name}");
                    }
                }
                else
                {
                    if (message is null)
                    {
                        Battle.Add("-formechange", this, Illusion?.Species.Name ?? species.Name);
                    }
                    else
                    {
                        Battle.Add("-formechange", this, Illusion?.Species.Name ?? species.Name,
                            message);
                    }
                }
            }
        }

        // Handle ability changes for permanent forme changes
        if (isPermanent == true &&
            source is Ability ability && ability.Id != AbilityId.Disguise &&
            ability.Id != AbilityId.IceFace)
        {
            // Break Illusion for certain Tera forme changes
            if (Illusion != null)
            {
                // Tera forme by Ogerpon or Terapagos breaks the Illusion
                Ability = AbilityId.None; // Don't allow Illusion to wear off
            }

            // Get the new ability from the species
            var newAbility =
                species.Abilities.GetAbility(abilitySlot) ?? species.Abilities.Slot0;

            // Ogerpon's forme change doesn't override permanent abilities
            if (source != null || !(GetAbility().Flags.CantSuppress ?? false))
            {
                SetAbility(newAbility, isFromFormeChange: true);
            }

            // Reset base ability (ability resets upon switching out)
            BaseAbility = newAbility;
        }

        // Update type visibility for Terastallized Pokemon
        if (Terastallized != null)
        {
            KnownType = true;
            ApparentType = [Terastallized.Value.ConvertToPokemonType()];
        }

        return true;
    }
}