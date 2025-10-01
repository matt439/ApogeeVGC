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

                    battle.Boost(new SparseBoostsTable { Atk = length }, source, source,
                        _library.Abilities[AbilityId.ChillingNeigh]);
                },
            },
            [AbilityId.HadronEngine] = new()
            {
                Id = AbilityId.HadronEngine,
                Name = "Hadron Engine",
                Num = 289,
                Rating = 4.5,
                OnStart = (battle, _) =>
                {
                    if (!battle.Field.SetTerrain(battle, _library.Terrains[TerrainId.Electric]) &&
                        battle.Field.HasTerrain(TerrainId.Electric) && battle.PrintDebug)
                    {
                        UiGenerator.PrintAbilityActivation("Hadron Engine");
                    }
                },
                OnModifySpAPriority = 5,
                OnModifySpA = (battle, _, _, _, _) =>
                {
                    if (battle.Field.HasTerrain(TerrainId.Electric))
                    {
                        return battle.ChainModify([5461, 4096]);
                    }
                    return null;
                },
            },
            [AbilityId.Guts] = new()
            {
                Id = AbilityId.Guts,
                Name = "Guts",
                Num = 62,
                Rating = 3.5,
                OnModifyAtkPriority = 5,
                OnModifyAtk = (battle, _, pokemon, _, _) =>
                {
                    if (pokemon.HasStatus)
                    {
                        return battle.ChainModify(1.5);
                    }
                    return null;
                },
            },
            [AbilityId.FlameBody] = new()
            {
                Id = AbilityId.FlameBody,
                Name = "Flame Body",
                Num = 49,
                Rating = 2.0,
                OnDamagingHit = (battle, _, target, source, move) =>
                {
                    if (!battle.CheckMoveMakesContact(move, source, target)) return;

                    if (battle.RandomChance(3, 10))
                    {
                        source.TrySetStatus(_library.Conditions[ConditionId.Burn], target, null);
                    }
                },
            },
            [AbilityId.Prankster] = new()
            {
                Id = AbilityId.Prankster,
                Name = "Prankster",
                Num = 158,
                Rating = 4.0,
                OnModifyPriority = (_, priority, _, _, move) =>
                {
                    if (move.Category != MoveCategory.Status) return null;

                    move.PranksterBooster = true;
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