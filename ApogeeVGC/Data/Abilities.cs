using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;
using System.Collections.ObjectModel;

namespace ApogeeVGC.Data;

public record Abilities
{
    public IReadOnlyDictionary<AbilityId, Ability> AbilitiesData { get; }
    private readonly Library _library;

    public Abilities(Library library)
    {
        _library = library;
        AbilitiesData = new ReadOnlyDictionary<AbilityId, Ability>(CreateAbilities());
    }

    private Dictionary<AbilityId, Ability> CreateAbilities()
    {
        Dictionary<AbilityId, Ability> abilities = [];

        // As One (Glastrier)
        Ability asOneGlastrier = new()
        {
            Id = AbilityId.AsOneGlastrier,
            Name = "As One (Glastrier)",
            Num = 266,
            Rating = 3.5,
            OnSwitchInPriority = 1,
            Flags = new AbilityFlags
            {
                FailRolePlay = true,
                NoReceiver = true,
                NoEntrain = true,
                NoTrace = true,
                FailSkillSwap = true,
                CantSuppress = true,
            },
            OnSourceAfterFaint = (length, _, source, effect, context) =>
            {
                if (effect.EffectType != EffectType.Move) return;
                if (context.PrintDebug)
                {
                    UiGenerator.PrintChillingNeighActivation(source);
                }

                source.AlterStatModifier(StatId.Atk, length, context);
            },
        };
        abilities[AbilityId.AsOneGlastrier] = asOneGlastrier;

        // Hadron Engine
        Ability hadronEngine = new()
        {
            Id = AbilityId.HadronEngine,
            Name = "Hadron Engine",
            Num = 289,
            Rating = 4.5,
            OnStart = (pokemon, field, pokemons, effect, context) =>
            {
                field.AddTerrain(context.Library.Terrains[TerrainId.Electric], pokemon, effect, pokemons, context);
                pokemon.AddCondition(context.Library.Conditions[ConditionId.HadronEngine], context);
            },
        };
        abilities[AbilityId.HadronEngine] = hadronEngine;

        // Guts
        Ability guts = new()
        {
            Id = AbilityId.Guts,
            Name = "Guts",
            Num = 62,
            Rating = 3.5,
            OnStart = (pokemon, _, _, _, context) =>
            {
                pokemon.AddCondition(context.Library.Conditions[ConditionId.Guts], context);
            },
        };
        abilities[AbilityId.Guts] = guts;

        // Flame Body
        Ability flameBody = new()
        {
            Id = AbilityId.FlameBody,
            Name = "Flame Body",
            Num = 49,
            Rating = 2.0,
            OnStart = (pokemon, _, _, _, context) =>
            {
                pokemon.AddCondition(context.Library.Conditions[ConditionId.FlameBody], context);
            },
        };
        abilities[AbilityId.FlameBody] = flameBody;

        // Prankster
        Ability prankster = new()
        {
            Id = AbilityId.Prankster,
            Name = "Prankster",
            Num = 158,
            Rating = 4.0,
            OnModifyPriority = (priority, move) =>
            {
                if (move.Category != MoveCategory.Status) return priority;
                return priority + 1;
            },
        };
        abilities[AbilityId.Prankster] = prankster;

        // Quark Drive
        Ability quarkDrive = new()
        {
            Id = AbilityId.QuarkDrive,
            Name = "Quark Drive",
            Num = 282,
            Rating = 3.0,
            OnSwitchInPriority = -2,
            OnStart = (pokemon, field, _, _, context) =>
            {
                if (field.HasTerrain(TerrainId.Electric))
                {
                    pokemon.AddCondition(context.Library.Conditions[ConditionId.QuarkDrive], context);
                    //if (context.PrintDebug)
                    //{
                    //    UiGenerator.PrintQuarkDriveStart(pokemon);
                    //}
                }
                else
                {
                    if (pokemon.RemoveCondition(ConditionId.QuarkDrive) && context.PrintDebug)
                    {
                        UiGenerator.PrintQuarkDriveEnd(pokemon);
                    }
                }
            },
            OnTerrainChange = (pokemon, field, context) =>
            {
                if (field.HasTerrain(TerrainId.Electric))
                {
                    pokemon.AddCondition(context.Library.Conditions[ConditionId.QuarkDrive], context);
                    //if (context.PrintDebug)
                    //{
                    //    UiGenerator.PrintQuarkDriveStart(pokemon);
                    //}
                }
                else
                {
                    if (pokemon.RemoveCondition(ConditionId.QuarkDrive) && context.PrintDebug)
                    {
                        UiGenerator.PrintQuarkDriveEnd(pokemon);
                    }
                }
            },
            Flags = new AbilityFlags
            {
                FailRolePlay = true,
                NoReceiver = true,
                NoEntrain = true,
                NoTrace = true,
                FailSkillSwap = true,
                NoTransform = true,
            },
        };
        abilities[AbilityId.QuarkDrive] = quarkDrive;

        return abilities;
    }
}