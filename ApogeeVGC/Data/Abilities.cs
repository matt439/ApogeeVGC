using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils;
using System.Collections.ObjectModel;
using ApogeeVGC.Sim.PokemonClasses;

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
                OnStart = (battle, pokemon) =>
                {
                    if (battle.EffectState.Unnerved is true) return;
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintAbilityEvent(pokemon, "As One");
                        UiGenerator.PrintAbilityEvent(pokemon, _library.Abilities[AbilityId.Unnerve]);
                    }
                    battle.EffectState.Unnerved = true;
                },
                OnEnd = (battle, _) =>
                {
                    battle.EffectState.Unnerved = false;
                },
                OnFoeTryEatItem = (Func<IBattle, Item, Pokemon, BoolVoidUnion>)((battle, _, _) =>
                    BoolVoidUnion.FromBool(!(battle.EffectState.Unnerved ?? false))),
                OnSourceAfterFaint = (battle, length, _, source, effect) =>
                {
                    if (effect.EffectType != EffectType.Move) return;

                    battle.Boost(new SparseBoostsTable { Atk = length }, source, source,
                        _library.Abilities[AbilityId.ChillingNeigh]);
                },
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                },
            },
            [AbilityId.HadronEngine] = new()
            {
                Id = AbilityId.HadronEngine,
                Name = "Hadron Engine",
                Num = 289,
                Rating = 4.5,
                OnStart = (battle, pokemon) =>
                {
                    if (!battle.Field.SetTerrain(_library.Conditions[ConditionId.ElectricTerrain]) &&
                        battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        if (battle.PrintDebug)
                        {
                            UiGenerator.PrintActivateEvent(pokemon, _library.Abilities[pokemon.Ability]);
                        }
                    }
                },
                OnModifySpAPriority = 5,
                OnModifySpA = (battle, _, _, _, _) =>
                {
                    if (battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        return battle.ChainModify([5461, 4096]);
                    }
                    return new VoidReturn();
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
                    if (pokemon.Status is not ConditionId.None)
                    {
                        return battle.ChainModify(1.5);
                    }
                    return new VoidReturn();
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
                        source.TrySetStatus(ConditionId.Burn, target);
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
                    if (move.Category != MoveCategory.Status) return new VoidReturn();

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
                Condition = ConditionId.QuarkDrive,
                OnSwitchInPriority = -2,
                OnStart = (battle, pokemon) =>
                {
                    battle.SingleEvent(EventId.TerrainChange, battle.Effect, battle.EffectState, pokemon);
                },
                OnTerrainChange = (battle, pokemon, _, _) =>
                {
                    Condition quarkDrive = _library.Conditions[ConditionId.QuarkDrive];

                    if (battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        pokemon.AddVolatile(quarkDrive.Id);
                    }
                    else if (!(pokemon.GetVolatile(ConditionId.QuarkDrive)?.FromBooster ?? false))
                    {
                        pokemon.RemoveVolatile(quarkDrive);
                    }
                },
                OnEnd = (_, pokemon) =>
                {
                    if (pokemon is not PokemonSideFieldPokemon pok)
                    {
                        throw new ArgumentException("Expecting a Pokemon here.");
                    }
                    pok.Pokemon.DeleteVolatile(ConditionId.QuarkDrive);
                    UiGenerator.PrintEndEvent(pok.Pokemon, _library.Abilities[pok.Pokemon.Ability]);
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