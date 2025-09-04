using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;

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
        return new Dictionary<AbilityId, Ability>
        {
            [AbilityId.AsOneGlastrier] = new()
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
                OnSourceAfterFaint = (battle, length, _, source, effect) =>
                {
                    if (effect.EffectType != EffectType.Move) return;
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintChillingNeighActivation(source);
                    }
                    source.AlterStatModifier(StatId.Atk, length, battle.Context);
                },
            },
            [AbilityId.HadronEngine] = new()
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
            },
            [AbilityId.Guts] = new()
            {
                Id = AbilityId.Guts,
                Name = "Guts",
                Num = 62,
                Rating = 3.5,
                OnStart = (pokemon, _, _, _, context) =>
                {
                    pokemon.AddCondition(context.Library.Conditions[ConditionId.Guts], context);
                },
            },
            [AbilityId.FlameBody] = new()
            {
                Id = AbilityId.FlameBody,
                Name = "Flame Body",
                Num = 49,
                Rating = 2.0,
                OnStart = (pokemon, _, _, _, context) =>
                {
                    pokemon.AddCondition(context.Library.Conditions[ConditionId.FlameBody], context);
                },
            },
            [AbilityId.Prankster] = new()
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
            },
            [AbilityId.QuarkDrive] = new()
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
            },
        };
    }
}