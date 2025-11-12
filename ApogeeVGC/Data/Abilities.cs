using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Utils.Unions;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;

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
                //OnSwitchInPriority = 1,
                OnSwitchIn = new OnSwitchInEventInfo((_, _) => { },
                    1
                ),
                OnStart = new OnStartEventInfo(
                    (battle, pokemon) =>
                    {
                        if (battle.EffectState.Unnerved is true) return;
                        if (battle.DisplayUi)
                        {
                            //UiGenerator.PrintAbilityEvent(pokemon, "As One");
                            //UiGenerator.PrintAbilityEvent(pokemon, _library.Abilities[AbilityId.Unnerve]);

                            battle.Add("-ability", pokemon, "As One");
                            battle.Add("-ability", pokemon, "Unnerve");
                        }

                        battle.EffectState.Unnerved = true;
                    },
                    1
                ),
                OnEnd = new OnEndEventInfo((battle, _) => { battle.EffectState.Unnerved = false; }),
                OnFoeTryEatItem = new OnFoeTryEatItemEventInfo( OnTryEatItem.FromFunc((battle, _, _) =>
                        BoolVoidUnion.FromBool(!(battle.EffectState.Unnerved ?? false)))),
                OnSourceAfterFaint =
                    new OnSourceAfterFaintEventInfo((battle, length, _, source, effect) =>
                    {
                        if (effect.EffectType != EffectType.Move) return;

                        battle.Boost(new SparseBoostsTable { Atk = length }, source, source,
                            _library.Abilities[AbilityId.ChillingNeigh]);
                    }),
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
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (!battle.Field.SetTerrain(
                            _library.Conditions[ConditionId.ElectricTerrain]) &&
                        battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        if (battle.DisplayUi)
                        {
                            //UiGenerator.PrintActivateEvent(pokemon, _library.Abilities[pokemon.Ability]);

                            battle.Add("-activate", pokemon, "ability: Hadron Engine");
                        }
                    }
                }),
                //OnModifySpAPriority = 5,
                OnModifySpA = new OnModifySpAEventInfo((battle, _, _, _, _) =>
                    {
                        if (!battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                            return new VoidReturn();
                        battle.Debug("Hadron Engine boost");
                        return battle.ChainModify([5461, 4096]);
                    },
                    5),
            },
            [AbilityId.Guts] = new()
            {
                Id = AbilityId.Guts,
                Name = "Guts",
                Num = 62,
                Rating = 3.5,
                //OnModifyAtkPriority = 5,
                OnModifyAtk = new OnModifyAtkEventInfo((battle, _, pokemon, _, _) =>
                    {
                        if (pokemon.Status is not ConditionId.None)
                        {
                            return battle.ChainModify(1.5);
                        }

                        return new VoidReturn();
                    },
                    5),
            },
            [AbilityId.FlameBody] = new()
            {
                Id = AbilityId.FlameBody,
                Name = "Flame Body",
                Num = 49,
                Rating = 2.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    if (!battle.CheckMoveMakesContact(move, source, target)) return;

                    if (battle.RandomChance(3, 10))
                    {
                        source.TrySetStatus(ConditionId.Burn, target);
                    }
                }),
            },
            [AbilityId.Prankster] = new()
            {
                Id = AbilityId.Prankster,
                Name = "Prankster",
                Num = 158,
                Rating = 4.0,
                OnModifyPriority = new OnModifyPriorityEventInfo((_, priority, _, _, move) =>
                {
                    if (move.Category != MoveCategory.Status) return new VoidReturn();

                    move.PranksterBooster = true;
                    return priority + 1;
                }),
            },
            [AbilityId.QuarkDrive] = new()
            {
                Id = AbilityId.QuarkDrive,
                Name = "Quark Drive",
                Num = 282,
                Rating = 3.0,
                Condition = ConditionId.QuarkDrive,
                //OnSwitchInPriority = -2,
                OnSwitchIn = new OnSwitchInEventInfo((_, _) => { }, -2),

                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    battle.SingleEvent(EventId.TerrainChange, battle.Effect, battle.EffectState,
                        pokemon);
                }),
                OnTerrainChange = new OnTerrainChangeEventInfo((battle, pokemon, _, _) =>
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
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (pokemon is not PokemonSideFieldPokemon pok)
                    {
                        throw new ArgumentException("Expecting a Pokemon here.");
                    }

                    pok.Pokemon.DeleteVolatile(ConditionId.QuarkDrive);

                    if (battle.DisplayUi)
                    {
                        //UiGenerator.PrintEndEvent(pok.Pokemon, _library.Abilities[pok.Pokemon.Ability]);

                        battle.Add("-end", pok.Pokemon, "Quark Drive", "[silent]");
                    }
                }),
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