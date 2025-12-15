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
using ApogeeVGC.Sim.Utils.Extensions;

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
                        if (effect is null || effect.EffectType != EffectType.Move) return;

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
                    battle.Debug($"[HadronEngine.OnStart] HANDLER EXECUTING for {pokemon.Name}");
                    
                    bool terrainSet = battle.Field.SetTerrain(
                        _library.Conditions[ConditionId.ElectricTerrain]);
                    
                    battle.Debug($"[HadronEngine.OnStart] SetTerrain returned: {terrainSet}");
                    battle.Debug($"[HadronEngine.OnStart] Current terrain: {battle.Field.Terrain}");
                    
                    if (!terrainSet &&
                        battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        battle.Debug($"[HadronEngine.OnStart] Terrain already Electric, showing activate");
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "ability: Hadron Engine");
                        }
                    }
                }),
                //OnModifySpAPriority = 5,
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, _) =>
                    {
                        if (!battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                            return spa;
                        battle.Debug("Hadron Engine boost");
                        battle.ChainModify([5461, 4096]);
                        return battle.FinalModify(spa);
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
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
                    {
                        if (pokemon.Status is not ConditionId.None)
                        {
                            battle.ChainModify(1.5);
                            return battle.FinalModify(atk);
                        }

                        return atk;
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
            [AbilityId.Bulletproof] = new()
            {
                Id = AbilityId.Bulletproof,
                Name = "Bulletproof",
                Num = 171,
                Rating = 3.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = new OnTryHitEventInfo((battle, pokemon, _, move) =>
                {
                    if (move.Flags.Bullet == true)
                    {
                        battle.Add("-immune", pokemon, "[from] ability: Bulletproof");
                        return null;
                    }
                    return new VoidReturn();
                }),
            },
            [AbilityId.Unnerve] = new()
            {
                Id = AbilityId.Unnerve,
                Name = "Unnerve",
                Rating = 1.0,
                Num = 127,
                OnSwitchIn = new OnSwitchInEventInfo((_, _) => { }, 1),
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (battle.EffectState.Unnerved ?? false) return;
                    battle.Add("-ability", pokemon, "Unnerve");
                    battle.EffectState.Unnerved = true;
                }),
                OnEnd = new OnEndEventInfo((battle, _) => { battle.EffectState.Unnerved = false; }),
                OnFoeTryEatItem = new OnFoeTryEatItemEventInfo(OnTryEatItem.FromFunc((battle, _, _) =>
                    !(battle.EffectState.Unnerved ?? false))),
            },
                        [AbilityId.NaturalCure] = new()
                        {
                            Id = AbilityId.NaturalCure,
                            Name = "Natural Cure",
                            Rating = 2.5,
                            Num = 30,
                            // TODO: Implement OnSwitchOut and OnCheckShow
                        },
                        [AbilityId.Adaptability] = new()
                        {
                            Id = AbilityId.Adaptability,
                            Name = "Adaptability",
                            Num = 91,
                            Rating = 4.0,
                            OnModifyStab = new OnModifyStabEventInfo((battle, stab, source, target, move) =>
                            {
                                if ((move.ForceStab ?? false) || source.HasType(move.Type.ConvertToPokemonType()))
                                {
                                    if (stab == 2)
                                    {
                                        return 2.25;
                                    }
                                    return 2.0;
                                }
                                return new VoidReturn();
                            }),
                        },
                        [AbilityId.Aerilate] = new()
                        {
                            Id = AbilityId.Aerilate,
                            Name = "Aerilate",
                            Num = 184,
                            Rating = 4.0,
                            //OnModifyTypePriority = -1,
                            OnModifyType = new OnModifyTypeEventInfo((battle, move, pokemon, _) =>
                                {
                                    // Change Normal-type moves to Flying
                                    // TODO: Add checks for specific moves like Judgment, Multi-Attack, etc.
                                    if (move.Type == MoveType.Normal && move.Category != MoveCategory.Status)
                                    {
                                        move.Type = MoveType.Flying;
                                        move.TypeChangerBoosted = battle.Effect;
                                    }
                                },
                                -1),
                            //OnBasePowerPriority = 23,
                            OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                                {
                                    if (move.TypeChangerBoosted == battle.Effect)
                                    {
                                        battle.ChainModify([4915, 4096]);
                                        return battle.FinalModify(basePower);
                                    }
                                    return basePower;
                                },
                                23),
                        },
                                    [AbilityId.Aftermath] = new()
                                    {
                                        Id = AbilityId.Aftermath,
                                        Name = "Aftermath",
                                        Num = 106,
                                        Rating = 2.0,
                                        //OnDamagingHitOrder = 1,
                                        OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                                            {
                                                if (target.Hp == 0 && battle.CheckMoveMakesContact(move, source, target, true))
                                                {
                                                    battle.Damage(source.BaseMaxHp / 4, source, target);
                                                }
                                            },
                                            1),
                                    },
                                    [AbilityId.AirLock] = new()
                                    {
                                        Id = AbilityId.AirLock,
                                        Name = "Air Lock",
                                        Num = 76,
                                        Rating = 1.5,
                                        OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                                        {
                                            // Air Lock does not activate when Skill Swapped or when Neutralizing Gas leaves the field
                                            battle.Add("-ability", pokemon, "Air Lock");
                                            // Call onStart
                                            battle.SingleEvent(EventId.Start, battle.Effect, battle.EffectState, pokemon);
                                        }),
                                        OnStart = new OnStartEventInfo((battle, pokemon) =>
                                        {
                                            pokemon.AbilityState.Ending = false; // Clear the ending flag
                                            battle.EachEvent(EventId.WeatherChange, battle.Effect);
                                        }),
                                        OnEnd = new OnEndEventInfo((battle, pokemonUnion) =>
                                        {
                                            if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                                            psfp.Pokemon.AbilityState.Ending = true;
                                            battle.EachEvent(EventId.WeatherChange, battle.Effect);
                                        }),
                                        SuppressWeather = true,
                                    },
                                    [AbilityId.Analytic] = new()
                                    {
                                        Id = AbilityId.Analytic,
                                        Name = "Analytic",
                                        Num = 148,
                                        Rating = 2.5,
                                        //OnBasePowerPriority = 21,
                                        OnBasePower = new OnBasePowerEventInfo((battle, basePower, pokemon, _, _) =>
                                            {
                                                bool boosted = true;
                                                foreach (var target in battle.GetAllActive())
                                                {
                                                    if (target == pokemon) continue;
                                                    if (battle.Queue.WillMove(target) != null)
                                                    {
                                                        boosted = false;
                                                        break;
                                                    }
                                                }

                                                if (boosted)
                                                {
                                                    battle.Debug("Analytic boost");
                                                    battle.ChainModify([5325, 4096]);
                                                    return battle.FinalModify(basePower);
                                                }

                                                return basePower;
                                            },
                                            21),
                                    },
                                    [AbilityId.AngerPoint] = new()
                                    {
                                        Id = AbilityId.AngerPoint,
                                        Name = "Anger Point",
                                        Num = 83,
                                        Rating = 1.0,
                                        OnHit = new OnHitEventInfo((battle, target, source, move) =>
                                        {
                                            if (target.Hp == 0) return BoolEmptyVoidUnion.FromEmpty();
                                            if (move?.EffectType == EffectType.Move && target.GetMoveHitData(move).Crit)
                                            {
                                                battle.Boost(new SparseBoostsTable { Atk = 12 }, target, target);
                                            }
                                            return BoolEmptyVoidUnion.FromEmpty();
                                        }),
                                    },
                                };
                            }
                        }