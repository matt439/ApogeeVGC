using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesDef()
    {
        return new Dictionary<AbilityId, Ability>
        {
            [AbilityId.Damp] = new()
            {
                Id = AbilityId.Damp,
                Name = "Damp",
                Num = 6,
                Rating = 0.5,
                Flags = new AbilityFlags { Breakable = true },
                OnAnyTryMove = new OnAnyTryMoveEventInfo((battle, target, _, move) =>
                {
                    MoveId[] blockedMoves =
                    [
                        MoveId.Explosion, MoveId.MindBlown, MoveId.MistyExplosion, MoveId.SelfDestruct
                    ];
                    if (blockedMoves.Contains(move.Id))
                    {
                        battle.AttrLastMove("[still]");
                        if (battle.EffectState.Target is PokemonEffectStateTarget
                            {
                                Pokemon: var dampHolder
                            })
                        {
                            battle.Add("cant", dampHolder, "ability: Damp", move.Name, $"[of] {target}");
                        }
                        return false;
                    }
                    return new VoidReturn();
                }),
                OnAnyDamage = new OnAnyDamageEventInfo((_, _, _, _, effect) =>
                {
                    // Block Aftermath damage
                    if (effect is Ability { Name: "Aftermath" })
                    {
                        return false;
                    }
                    return new VoidReturn();
                }),
            },
            [AbilityId.Dancer] = new()
            {
                Id = AbilityId.Dancer,
                Name = "Dancer",
                Num = 216,
                Rating = 1.5,
                // Implemented in runMove in scripts.js / Battle.RunMove
                // TODO: Implement Dancer logic in Battle.RunMove
            },
            [AbilityId.DarkAura] = new()
            {
                Id = AbilityId.DarkAura,
                Name = "Dark Aura",
                Num = 186,
                Rating = 3.0,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (battle.SuppressingAbility(pokemon)) return;
                    battle.Add("-ability", pokemon, "Dark Aura");
                }),
                OnAnyBasePower = new OnAnyBasePowerEventInfo((battle, basePower, source, target, move) =>
                {
                    if (target == source || move.Category == MoveCategory.Status || move.Type != MoveType.Dark)
                        return basePower;

                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder
                        })
                        return basePower;

                    if (move.AuraBooster?.HasAbility(AbilityId.DarkAura) != true)
                        move.AuraBooster = abilityHolder;
                    if (move.AuraBooster != abilityHolder) return basePower;

                    battle.ChainModify(move.HasAuraBreak == true ? [3072, 4096] : [5448, 4096]);
                    return battle.FinalModify(basePower);
                }, 20),
            },
            [AbilityId.DauntlessShield] = new()
            {
                Id = AbilityId.DauntlessShield,
                Name = "Dauntless Shield",
                Num = 235,
                Rating = 3.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (pokemon.ShieldBoost) return;
                    pokemon.ShieldBoost = true;
                    battle.Boost(new SparseBoostsTable { Def = 1 }, pokemon);
                }),
            },
            [AbilityId.Dazzling] = new()
            {
                Id = AbilityId.Dazzling,
                Name = "Dazzling",
                Num = 219,
                Rating = 2.5,
                Flags = new AbilityFlags { Breakable = true },
                OnFoeTryMove = new OnFoeTryMoveEventInfo((battle, target, source, move) =>
                {
                    string[] targetAllExceptions = ["perishsong", "flowershield", "rototiller"];
                    if (move.Target == MoveTarget.FoeSide ||
                        (move.Target == MoveTarget.All &&
                         !targetAllExceptions.Contains(move.Id.ToString().ToLower())))
                    {
                        return new VoidReturn();
                    }

                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var dazzlingHolder
                        })
                        return new VoidReturn();

                    if ((source.IsAlly(dazzlingHolder) || move.Target == MoveTarget.All) &&
                        move.Priority > 0.1)
                    {
                        battle.AttrLastMove("[still]");
                        battle.Add("cant", dazzlingHolder, "ability: Dazzling", move.Name,
                            $"[of] {target}");
                        return false;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Defeatist] = new()
            {
                Id = AbilityId.Defeatist,
                Name = "Defeatist",
                Num = 129,
                Rating = -1.0,
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
                {
                    if (pokemon.Hp <= pokemon.MaxHp / 2)
                    {
                        battle.ChainModify(0.5);
                        return battle.FinalModify(atk);
                    }
                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, pokemon, _, _) =>
                {
                    if (pokemon.Hp <= pokemon.MaxHp / 2)
                    {
                        battle.ChainModify(0.5);
                        return battle.FinalModify(spa);
                    }
                    return spa;
                }, 5),
            },
            [AbilityId.Defiant] = new()
            {
                Id = AbilityId.Defiant,
                Name = "Defiant",
                Num = 128,
                Rating = 3.0,
                OnAfterEachBoost =
                    new OnAfterEachBoostEventInfo((battle, boost, target, source, _) =>
                    {
                        if (source == null || target.IsAlly(source)) return;

                        bool statsLowered = boost.Atk is < 0 || boost.Def is < 0 || boost.SpA is < 0
                                            || boost.SpD is < 0 || boost.Spe is < 0 || boost.Accuracy is < 0
                                            || boost.Evasion is < 0;

                        if (statsLowered)
                        {
                            battle.Boost(new SparseBoostsTable { Atk = 2 }, target, target, null,
                                false, true);
                        }
                    }),
            },
            [AbilityId.DeltaStream] = new()
            {
                Id = AbilityId.DeltaStream,
                Name = "Delta Stream",
                Num = 191,
                Rating = 4.0,
                OnStart = new OnStartEventInfo((battle, _) =>
                {
                    battle.Field.SetWeather(ConditionId.DeltaStream);
                }),
                OnAnySetWeather = new OnAnySetWeatherEventInfo((battle, _, _, weather) =>
                {
                    ConditionId[] strongWeathers =
                        [ConditionId.DesolateLand, ConditionId.PrimordialSea, ConditionId.DeltaStream];
                    if (battle.Field.GetWeather()?.Id == ConditionId.DeltaStream &&
                        !strongWeathers.Contains(weather.Id))
                    {
                        return false;
                    }
                    return new VoidReturn();
                }),
                OnEnd = new OnEndEventInfo((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                    Pokemon pokemon = psfp.Pokemon;
                    if (battle.Field.WeatherState?.Source != pokemon) return;
                    foreach (Pokemon target in battle.GetAllActive())
                    {
                        if (target == pokemon) continue;
                        if (target.HasAbility(AbilityId.DeltaStream))
                        {
                            battle.Field.WeatherState.Source = target;
                            return;
                        }
                    }
                    battle.Field.ClearWeather();
                }),
            },
            [AbilityId.DesolateLand] = new()
            {
                Id = AbilityId.DesolateLand,
                Name = "Desolate Land",
                Num = 190,
                Rating = 4.5,
                OnStart = new OnStartEventInfo((battle, _) =>
                {
                    battle.Field.SetWeather(ConditionId.DesolateLand);
                }),
                OnAnySetWeather = new OnAnySetWeatherEventInfo((battle, _, _, weather) =>
                {
                    ConditionId[] strongWeathers =
                        [ConditionId.DesolateLand, ConditionId.PrimordialSea, ConditionId.DeltaStream];
                    if (battle.Field.GetWeather()?.Id == ConditionId.DesolateLand &&
                        !strongWeathers.Contains(weather.Id))
                    {
                        return false;
                    }
                    return new VoidReturn();
                }),
                OnEnd = new OnEndEventInfo((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                    Pokemon pokemon = psfp.Pokemon;
                    if (battle.Field.WeatherState?.Source != pokemon) return;
                    foreach (Pokemon target in battle.GetAllActive())
                    {
                        if (target == pokemon) continue;
                        if (target.HasAbility(AbilityId.DesolateLand))
                        {
                            battle.Field.WeatherState.Source = target;
                            return;
                        }
                    }
                    battle.Field.ClearWeather();
                }),
            },
            [AbilityId.Disguise] = new()
            {
                Id = AbilityId.Disguise,
                Name = "Disguise",
                Num = 209,
                Rating = 3.5,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                    Breakable = true,
                    NoTransform = true,
                },
                // TODO: Implement Disguise - requires forme change logic for Mimikyu
                // OnDamagePriority = 1
                // OnCriticalHit
                // OnEffectiveness
                // OnUpdate
            },
            [AbilityId.Download] = new()
            {
                Id = AbilityId.Download,
                Name = "Download",
                Num = 88,
                Rating = 3.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    int totalDef = 0;
                    int totalSpd = 0;
                    foreach (Pokemon target in pokemon.Foes())
                    {
                        totalDef += target.GetStat(StatIdExceptHp.Def, false, true);
                        totalSpd += target.GetStat(StatIdExceptHp.SpD, false, true);
                    }
                    if (totalDef > 0 && totalDef >= totalSpd)
                    {
                        battle.Boost(new SparseBoostsTable { SpA = 1 });
                    }
                    else if (totalSpd > 0)
                    {
                        battle.Boost(new SparseBoostsTable { Atk = 1 });
                    }
                }),
            },
            [AbilityId.DragonsMaw] = new()
            {
                Id = AbilityId.DragonsMaw,
                Name = "Dragon's Maw",
                Num = 263,
                Rating = 3.5,
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, _, _, move) =>
                {
                    if (move.Type == MoveType.Dragon)
                    {
                        battle.Debug("Dragon's Maw boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(atk);
                    }
                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Dragon)
                    {
                        battle.Debug("Dragon's Maw boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(spa);
                    }
                    return spa;
                }, 5),
            },
            [AbilityId.Drizzle] = new()
            {
                Id = AbilityId.Drizzle,
                Name = "Drizzle",
                Num = 2,
                Rating = 4.0,
                OnStart = new OnStartEventInfo((battle, _) =>
                {
                    battle.Field.SetWeather(ConditionId.RainDance);
                }),
            },
            [AbilityId.Drought] = new()
            {
                    Id = AbilityId.Drought,
                    Name = "Drought",
                    Num = 70,
                    Rating = 4.0,
                    OnStart = new OnStartEventInfo((battle, _) =>
                    {
                        battle.Field.SetWeather(ConditionId.SunnyDay);
                    }),
                },
                [AbilityId.DrySkin] = new()
                {
                    Id = AbilityId.DrySkin,
                    Name = "Dry Skin",
                    Num = 87,
                    Rating = 3.0,
                    Flags = new AbilityFlags { Breakable = true },
                    OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                                    {
                                        if (target != source && move.Type == MoveType.Water)
                                        {
                                            var healResult = battle.Heal(target.BaseMaxHp / 4, target);
                                            if (healResult is FalseIntFalseUnion)
                                            {
                                                battle.Add("-immune", target, "[from] ability: Dry Skin");
                                            }
                                            return null;
                                        }
                                        return new VoidReturn();
                                    }),
                    // OnSourceBasePowerPriority = 17
                    OnSourceBasePower = new OnSourceBasePowerEventInfo((battle, basePower, _, _, move) =>
                    {
                        if (move.Type == MoveType.Fire)
                        {
                            battle.ChainModify(1.25);
                            return battle.FinalModify(basePower);
                        }
                        return basePower;
                    }, 17),
                    OnWeather = new OnWeatherEventInfo((battle, target, _, effect) =>
                    {
                        if (target.HasItem(ItemId.UtilityUmbrella)) return;
                    if (effect.Id == ConditionId.RainDance || effect.Id == ConditionId.PrimordialSea)
                    {
                        battle.Heal(target.BaseMaxHp / 8, target);
                    }
                    else if (effect.Id == ConditionId.SunnyDay || effect.Id == ConditionId.DesolateLand)
                    {
                        battle.Damage(target.BaseMaxHp / 8, target, target);
                    }
                                }),
                            },
                            [AbilityId.EarlyBird] = new()
                            {
                                Id = AbilityId.EarlyBird,
                                Name = "Early Bird",
                                Num = 48,
                                Rating = 1.5,
                                // Implementation is in statuses.ts (Sleep condition)
                                // Early Bird causes sleep to count down twice as fast
                            },
                            [AbilityId.EarthEater] = new()
                            {
                                Id = AbilityId.EarthEater,
                                Name = "Earth Eater",
                                Num = 297,
                                Rating = 3.5,
                                Flags = new AbilityFlags { Breakable = true },
                                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                                {
                                    if (target != source && move.Type == MoveType.Ground)
                                    {
                                        var healResult = battle.Heal(target.BaseMaxHp / 4, target);
                                        if (healResult is FalseIntFalseUnion)
                                        {
                                            battle.Add("-immune", target, "[from] ability: Earth Eater");
                                        }
                                        return null;
                                    }
                                    return new VoidReturn();
                                }),
                                },
                                [AbilityId.EffectSpore] = new()
                                {
                                    Id = AbilityId.EffectSpore,
                                    Name = "Effect Spore",
                                    Num = 27,
                                    Rating = 2.0,
                                    OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                                    {
                                        if (battle.CheckMoveMakesContact(move, source, target) &&
                                            source.Status == ConditionId.None &&
                                            source.RunStatusImmunity(ConditionId.Powder))
                                        {
                                            int r = battle.Random(100);
                                            if (r < 11)
                                            {
                                                source.SetStatus(ConditionId.Sleep, target);
                                            }
                                            else if (r < 21)
                                            {
                                                source.SetStatus(ConditionId.Paralysis, target);
                                            }
                                            else if (r < 30)
                                            {
                                                source.SetStatus(ConditionId.Poison, target);
                                            }
                                        }
                                    }),
                                },
                                [AbilityId.ElectricSurge] = new()
                            {
                                Id = AbilityId.ElectricSurge,
                                Name = "Electric Surge",
                                Num = 226,
                                Rating = 4.0,
                                OnStart = new OnStartEventInfo((battle, _) =>
                                {
                                    battle.Field.SetTerrain(battle.Library.Conditions[ConditionId.ElectricTerrain]);
                                }),
                            },
                            [AbilityId.Electromorphosis] = new()
                            {
                                Id = AbilityId.Electromorphosis,
                                Name = "Electromorphosis",
                                Num = 280,
                                Rating = 3.0,
                                // OnDamagingHitOrder = 1
                                OnDamagingHit = new OnDamagingHitEventInfo((_, _, target, _, _) =>
                                {
                                    target.AddVolatile(ConditionId.Charge);
                                }, 1),
                            },
                            [AbilityId.EmbodyAspectCornerstone] = new()
                            {
                                Id = AbilityId.EmbodyAspectCornerstone,
                                Name = "Embody Aspect (Cornerstone)",
                                Num = 304,
                                Rating = 3.5,
                                Flags = new AbilityFlags
                                {
                                    FailRolePlay = true,
                                    NoReceiver = true,
                                    NoEntrain = true,
                                    NoTrace = true,
                                    FailSkillSwap = true,
                                    NoTransform = true,
                                },
                                    OnStart = new OnStartEventInfo((battle, pokemon) =>
                                    {
                                        if (pokemon.BaseSpecies.Name == "Ogerpon-Cornerstone-Tera" &&
                                            pokemon.Terastallized != null &&
                                            !(battle.EffectState.Embodied ?? false))
                                        {
                                            battle.EffectState.Embodied = true;
                                            battle.Boost(new SparseBoostsTable { Def = 1 }, pokemon);
                                        }
                                    }),
                                },
                                [AbilityId.EmbodyAspectHearthflame] = new()
                            {
                                Id = AbilityId.EmbodyAspectHearthflame,
                                Name = "Embody Aspect (Hearthflame)",
                                Num = 303,
                                Rating = 3.5,
                                Flags = new AbilityFlags
                                {
                                    FailRolePlay = true,
                                    NoReceiver = true,
                                    NoEntrain = true,
                                    NoTrace = true,
                                    FailSkillSwap = true,
                                    NoTransform = true,
                                },
                                    OnStart = new OnStartEventInfo((battle, pokemon) =>
                                    {
                                        if (pokemon.BaseSpecies.Name == "Ogerpon-Hearthflame-Tera" &&
                                            pokemon.Terastallized != null &&
                                            !(battle.EffectState.Embodied ?? false))
                                        {
                                            battle.EffectState.Embodied = true;
                                            battle.Boost(new SparseBoostsTable { Atk = 1 }, pokemon);
                                        }
                                    }),
                                },
                                [AbilityId.EmbodyAspectTeal] = new()
                            {
                                Id = AbilityId.EmbodyAspectTeal,
                                Name = "Embody Aspect (Teal)",
                                Num = 301,
                                Rating = 3.5,
                                Flags = new AbilityFlags
                                {
                                    FailRolePlay = true,
                                    NoReceiver = true,
                                    NoEntrain = true,
                                    NoTrace = true,
                                    FailSkillSwap = true,
                                    NoTransform = true,
                                },
                                    OnStart = new OnStartEventInfo((battle, pokemon) =>
                                    {
                                        if (pokemon.BaseSpecies.Name == "Ogerpon-Teal-Tera" &&
                                            pokemon.Terastallized != null &&
                                            !(battle.EffectState.Embodied ?? false))
                                        {
                                            battle.EffectState.Embodied = true;
                                            battle.Boost(new SparseBoostsTable { Spe = 1 }, pokemon);
                                        }
                                    }),
                                },
                                [AbilityId.EmbodyAspectWellspring] = new()
                            {
                                Id = AbilityId.EmbodyAspectWellspring,
                                Name = "Embody Aspect (Wellspring)",
                                Num = 302,
                                Rating = 3.5,
                                Flags = new AbilityFlags
                                {
                                    FailRolePlay = true,
                                    NoReceiver = true,
                                    NoEntrain = true,
                                    NoTrace = true,
                                    FailSkillSwap = true,
                                    NoTransform = true,
                                },
                                    OnStart = new OnStartEventInfo((battle, pokemon) =>
                                    {
                                        if (pokemon.BaseSpecies.Name == "Ogerpon-Wellspring-Tera" &&
                                            pokemon.Terastallized != null &&
                                            !(battle.EffectState.Embodied ?? false))
                                        {
                                            battle.EffectState.Embodied = true;
                                            battle.Boost(new SparseBoostsTable { SpD = 1 }, pokemon);
                                        }
                                    }),
                                },
                                        [AbilityId.EmergencyExit] = new()
                                    {
                                        Id = AbilityId.EmergencyExit,
                                        Name = "Emergency Exit",
                                        Num = 194,
                                        Rating = 1.0,
                                        OnEmergencyExit = new OnEmergencyExitEventInfo((battle, target) =>
                                        {
                                            if (battle.CanSwitch(target.Side) == 0 || target.ForceSwitchFlag || target.SwitchFlag.IsTrue())
                                                return;

                                            // Clear all switch flags
                                            foreach (var side in battle.Sides)
                                            {
                                                foreach (var active in side.Active)
                                                {
                                                    if (active != null)
                                                        active.SwitchFlag = false;
                                                }
                                            }

                                            target.SwitchFlag = true;
                                            battle.Add("-activate", target, "ability: Emergency Exit");
                                        }),
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
                        };
                    }
                }
