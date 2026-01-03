using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesPqr()
    {
        return new Dictionary<AbilityId, Ability>
        {
            // ==================== 'P' Abilities ====================
            [AbilityId.ParentalBond] = new()
            {
                Id = AbilityId.ParentalBond,
                Name = "Parental Bond",
                Num = 185,
                Rating = 4.5,
                OnPrepareHit = new OnPrepareHitEventInfo((_, _, _, move) =>
                {
                    if (move.Category == MoveCategory.Status || move.MultiHit != null ||
                        move.Flags.NoParentalBond == true || move.Flags.Charge == true ||
                        move.Flags.FutureMove == true || move.SpreadHit == true)
                    {
                        return new VoidReturn();
                    }

                    move.MultiHit = 2;
                    move.MultiHitType = MoveMultiHitType.ParentBond;
                    return new VoidReturn();
                }),
                // Note: Damage modifier for second hit implemented in BattleActions.ModifyDamage()
                OnSourceModifySecondaries =
                    new OnSourceModifySecondariesEventInfo((_, secondaries, _, _,
                        move) =>
                    {
                        // Hack to prevent accidentally suppressing King's Rock/Razor Fang
                        if (move is
                            {
                                MultiHitType: MoveMultiHitType.ParentBond, Id: MoveId.SecretPower,
                                Hit: < 2
                            })
                        {
                            // Keep only flinch effects (filter returns matching items)
                            secondaries.RemoveAll(effect =>
                                effect.VolatileStatus != ConditionId.Flinch);
                        }
                    }),
            },
            [AbilityId.PastelVeil] = new()
            {
                Id = AbilityId.PastelVeil,
                Name = "Pastel Veil",
                Num = 257,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    foreach (Pokemon ally in pokemon.AlliesAndSelf())
                    {
                        if (ally.Status is ConditionId.Poison or ConditionId.Toxic)
                        {
                            battle.Add("-activate", pokemon, "ability: Pastel Veil");
                            ally.CureStatus();
                        }
                    }
                }),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status is ConditionId.Poison or ConditionId.Toxic)
                    {
                        battle.Add("-activate", pokemon, "ability: Pastel Veil");
                        pokemon.CureStatus();
                    }
                }),
                OnAnySwitchIn = new OnAnySwitchInEventInfo((battle, _) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var target,
                        })
                        return;
                    foreach (Pokemon ally in target.AlliesAndSelf())
                    {
                        if (ally.Status is ConditionId.Poison or ConditionId.Toxic)
                        {
                            battle.Add("-activate", target, "ability: Pastel Veil");
                            ally.CureStatus();
                        }
                    }
                }),
                OnSetStatus = new OnSetStatusEventInfo((battle, status, target, _, effect) =>
                {
                    if (status.Id is not (ConditionId.Poison or ConditionId.Toxic))
                        return new VoidReturn();
                    if (effect is ActiveMove { Status: not ConditionId.None })
                    {
                        battle.Add("-immune", target, "[from] ability: Pastel Veil");
                    }

                    return false;
                }),
                OnAllySetStatus =
                    new OnAllySetStatusEventInfo((battle, status, target, _, effect) =>
                    {
                        if (status.Id is not (ConditionId.Poison or ConditionId.Toxic))
                            return PokemonVoidUnion.FromVoid();
                        if (effect is ActiveMove { Status: not ConditionId.None })
                        {
                            if (battle.EffectState.Target is PokemonEffectStateTarget
                                {
                                    Pokemon: var effectHolder,
                                })
                            {
                                battle.Add("-block", target, "ability: Pastel Veil",
                                    $"[of] {effectHolder}");
                            }
                        }

                        return null;
                    }),
            },
            [AbilityId.PerishBody] = new()
            {
                Id = AbilityId.PerishBody,
                Name = "Perish Body",
                Num = 253,
                Rating = 1.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    if (!battle.CheckMoveMakesContact(move, source, target) ||
                        source.Volatiles.ContainsKey(ConditionId.PerishSong))
                    {
                        return;
                    }

                    battle.Add("-ability", target, "ability: Perish Body");
                    source.AddVolatile(ConditionId.PerishSong);
                    target.AddVolatile(ConditionId.PerishSong);
                }),
            },
            [AbilityId.Pickpocket] = new()
            {
                Id = AbilityId.Pickpocket,
                Name = "Pickpocket",
                Num = 124,
                Rating = 1.0,
                OnAfterMoveSecondary =
                    new OnAfterMoveSecondaryEventInfo((battle, target, source, move) =>
                    {
                        if (source == null || source == target || move.Flags.Contact != true)
                            return;
                        if (target.Item != ItemId.None || target.SwitchFlag == true ||
                            target.ForceSwitchFlag ||
                            source.SwitchFlag == true)
                            return;

                        ItemFalseUnion yourItemResult = source.TakeItem(target);
                        if (yourItemResult is not ItemItemFalseUnion { Item: var yourItem })
                            return;
                        if (!target.SetItem(yourItem.Id))
                        {
                            source.Item = yourItem.Id;
                            return;
                        }

                        battle.Add("-enditem", source, yourItem.Name, "[silent]",
                            "[from] ability: Pickpocket", $"[of] {source}");
                        battle.Add("-item", target, yourItem.Name, "[from] ability: Pickpocket",
                            $"[of] {source}");
                    }),
            },
            [AbilityId.Pickup] = new()
            {
                Id = AbilityId.Pickup,
                Name = "Pickup",
                Num = 53,
                Rating = 0.5,
                // OnResidualOrder = 28, OnResidualSubOrder = 2
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.Item != ItemId.None) return;

                    var pickupTargets = battle.GetAllActive()
                        .Where(target => target.LastItem != ItemId.None &&
                                         target.UsedItemThisTurn &&
                                         pokemon.IsAdjacent(target))
                        .ToList();

                    if (pickupTargets.Count == 0) return;

                    Pokemon randomTarget = battle.Sample(pickupTargets);
                    ItemId item = randomTarget.LastItem;
                    randomTarget.LastItem = ItemId.None;

                    if (battle.Library.Items.TryGetValue(item, out Item? itemData))
                    {
                        battle.Add("-item", pokemon, itemData.Name, "[from] ability: Pickup");
                    }

                    pokemon.SetItem(item);
                }, order: 28, subOrder: 2),
            },
            [AbilityId.Pixilate] = new()
            {
                Id = AbilityId.Pixilate,
                Name = "Pixilate",
                Num = 182,
                Rating = 4.0,
                // OnModifyTypePriority = -1
                OnModifyType = new OnModifyTypeEventInfo((battle, move, pokemon, _) =>
                {
                    // Non-Gen9 moves excluded: MultiAttack, NaturalGift, Technoblast
                    MoveId[] noModifyType =
                    [
                        MoveId.Judgment, MoveId.RevelationDance, MoveId.TerrainPulse,
                        MoveId.WeatherBall,
                    ];
                    if (move.Type == MoveType.Normal &&
                        !noModifyType.Contains(move.Id) &&
                        !(move.Id == MoveId.TeraBlast && pokemon.Terastallized != null))
                    {
                        move.Type = MoveType.Fairy;
                        move.TypeChangerBoosted = battle.Effect;
                    }
                }, -1),
                // OnBasePowerPriority = 23
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.TypeChangerBoosted == battle.Effect)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 23),
            },
            [AbilityId.Plus] = new()
            {
                Id = AbilityId.Plus,
                Name = "Plus",
                Num = 57,
                Rating = 0.0,
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, pokemon, _, _) =>
                {
                    foreach (Pokemon allyActive in pokemon.Allies())
                    {
                        if (allyActive.HasAbility(AbilityId.Minus) ||
                            allyActive.HasAbility(AbilityId.Plus))
                        {
                            battle.ChainModify(1.5);
                            return battle.FinalModify(spa);
                        }
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.PoisonHeal] = new()
            {
                Id = AbilityId.PoisonHeal,
                Name = "Poison Heal",
                Num = 90,
                Rating = 4.0,
                // OnDamagePriority = 1
                OnDamage = new OnDamageEventInfo((battle, _, target, _, effect) =>
                {
                    if (effect is Condition { Id: ConditionId.Poison or ConditionId.Toxic })
                    {
                        battle.Heal(target.BaseMaxHp / 8);
                        return false;
                    }

                    return new VoidReturn();
                }, 1),
            },
            [AbilityId.PoisonPoint] = new()
            {
                Id = AbilityId.PoisonPoint,
                Name = "Poison Point",
                Num = 38,
                Rating = 1.5,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        if (battle.RandomChance(3, 10))
                        {
                            source.TrySetStatus(ConditionId.Poison, target);
                        }
                    }
                }),
            },
            [AbilityId.PoisonPuppeteer] = new()
            {
                Id = AbilityId.PoisonPuppeteer,
                Name = "Poison Puppeteer",
                Num = 310,
                Rating = 3.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                },
                OnAnyAfterSetStatus =
                    new OnAnyAfterSetStatusEventInfo((battle, status, target, source, effect) =>
                    {
                        if (source.BaseSpecies.Id != SpecieId.Pecharunt) return;
                        if (battle.EffectState.Target is not PokemonEffectStateTarget
                            {
                                Pokemon: var effectHolder,
                            })
                            return;
                        if (source != effectHolder || target == source || effect is not ActiveMove)
                            return;
                        if (status.Id is ConditionId.Poison or ConditionId.Toxic)
                        {
                            target.AddVolatile(ConditionId.Confusion);
                        }
                    }),
            },
            [AbilityId.PoisonTouch] = new()
            {
                Id = AbilityId.PoisonTouch,
                Name = "Poison Touch",
                Num = 143,
                Rating = 2.0,
                OnSourceDamagingHit =
                    new OnSourceDamagingHitEventInfo((battle, _, target, source, move) =>
                    {
                        // Despite not being a secondary, Shield Dust / Covert Cloak block Poison Touch's effect
                        if (target.HasAbility(AbilityId.ShieldDust) ||
                            target.HasItem(ItemId.CovertCloak)) return;
                        if (battle.CheckMoveMakesContact(move, target, source))
                        {
                            if (battle.RandomChance(3, 10))
                            {
                                target.TrySetStatus(ConditionId.Poison, source);
                            }
                        }
                    }),
            },
            [AbilityId.PowerConstruct] = new()
            {
                Id = AbilityId.PowerConstruct,
                Name = "Power Construct",
                Num = 211,
                Rating = 5.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                },
                // OnResidualOrder = 29
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.BaseSpecies.BaseSpecies != SpecieId.Zygarde ||
                        pokemon.Transformed ||
                        pokemon.Hp == 0) return;
                    if (pokemon.Species.Id == SpecieId.ZygardeComplete ||
                        pokemon.Hp > pokemon.MaxHp / 2) return;

                    battle.Add("-activate", pokemon, "ability: Power Construct");
                    pokemon.FormeChange(SpecieId.ZygardeComplete, battle.Effect, true);
                    // Note: canMegaEvo and formeRegression handled differently in C#
                }, order: 29),
            },
            [AbilityId.PowerOfAlchemy] = new()
            {
                Id = AbilityId.PowerOfAlchemy,
                Name = "Power of Alchemy",
                Num = 223,
                Rating = 0.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                },
                OnAllyFaint = new OnAllyFaintEventInfo((_, target, pokemon, _) =>
                {
                    if (pokemon.Hp == 0) return;
                    Ability ability = target.GetAbility();
                    if (ability.Flags.NoReceiver == true || ability.Id == AbilityId.None) return;
                    pokemon.SetAbility(ability.Id, target);
                }),
            },
            [AbilityId.PowerSpot] = new()
            {
                Id = AbilityId.PowerSpot,
                Name = "Power Spot",
                Num = 249,
                Rating = 0.0,
                // OnAllyBasePowerPriority = 22
                OnAllyBasePower = new OnAllyBasePowerEventInfo(
                    (battle, basePower, attacker, _, _) =>
                    {
                        // Boost if attacker is not the PowerSpot holder (effectState.target)
                        if (battle.EffectState.Target is PokemonEffectStateTarget
                            {
                                Pokemon: var effectHolder,
                            } &&
                            attacker != effectHolder)
                        {
                            battle.Debug("Power Spot boost");
                            battle.ChainModify([5325, 4096]);
                            return battle.FinalModify(basePower);
                        }

                        return basePower;
                    }, 22),
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
                    move.PranksterBoosted = true;
                    return priority + 1;
                }),
            },
            [AbilityId.Pressure] = new()
            {
                Id = AbilityId.Pressure,
                Name = "Pressure",
                Num = 46,
                Rating = 2.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    battle.Add("-ability", pokemon, "Pressure");
                }),
                OnDeductPp = new OnDeductPpEventInfo((_, target, source) =>
                {
                    if (target.IsAlly(source)) return 0;
                    return 1;
                }),
            },
            [AbilityId.PrimordialSea] = new()
            {
                Id = AbilityId.PrimordialSea,
                Name = "Primordial Sea",
                Num = 189,
                Rating = 4.5,
                OnStart = new OnStartEventInfo((battle, _) =>
                {
                    battle.Field.SetWeather(_library.Conditions[ConditionId.PrimordialSea]);
                }),
                OnAnySetWeather = new OnAnySetWeatherEventInfo((battle, _, _, weather) =>
                {
                    ConditionId[] strongWeathers =
                    [
                        ConditionId.DesolateLand, ConditionId.PrimordialSea,
                        ConditionId.DeltaStream,
                    ];
                    if (battle.Field.GetWeather().Id == ConditionId.PrimordialSea &&
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
                    if (battle.Field.WeatherState.Source != pokemon) return;
                    foreach (Pokemon target in battle.GetAllActive())
                    {
                        if (target == pokemon) continue;
                        if (target.HasAbility(AbilityId.PrimordialSea))
                        {
                            battle.Field.WeatherState.Source = target;
                            return;
                        }
                    }

                    battle.Field.ClearWeather();
                }),
            },
            [AbilityId.PrismArmor] = new()
            {
                Id = AbilityId.PrismArmor,
                Name = "Prism Armor",
                Num = 232,
                Rating = 3.0,
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, _, target, move) =>
                    {
                        if (target.GetMoveHitData(move).TypeMod > 0)
                        {
                            battle.Debug("Prism Armor neutralize");
                            battle.ChainModify(0.75);
                            return battle.FinalModify(damage);
                        }

                        return damage;
                    }),
            },
            [AbilityId.PropellerTail] = new()
            {
                Id = AbilityId.PropellerTail,
                Name = "Propeller Tail",
                Num = 239,
                Rating = 0.0,
                // OnModifyMovePriority = 1
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    // Most of the implementation is in Battle.GetTarget
                    move.TracksTarget = move.Target != MoveTarget.Scripted;
                }, 1),
            },
            [AbilityId.Protean] = new()
            {
                Id = AbilityId.Protean,
                Name = "Protean",
                Num = 168,
                Rating = 4.0,
                OnPrepareHit = new OnPrepareHitEventInfo((battle, source, _, move) =>
                {
                    if (battle.EffectState.Protean == true) return new VoidReturn();
                    if (move.HasBounced == true || move.Flags.FutureMove == true ||
                        move.CallsMove == true)
                        return new VoidReturn();

                    MoveType type = move.Type;
                    if (type != MoveType.Unknown)
                    {
                        PokemonType pokemonType = type.ConvertToPokemonType();
                        // TS: source.getTypes().join() !== type - compares joined type string to move type
                        PokemonType[] currentTypes = source.GetTypes();
                        // Only change type if pokemon doesn't already have exactly this single type
                        if (currentTypes.Length != 1 || currentTypes[0] != pokemonType)
                        {
                            if (!source.SetType([pokemonType])) return new VoidReturn();
                            battle.EffectState.Protean = true;
                            battle.Add("-start", source, "typechange", type.ToString(),
                                "[from] ability: Protean");
                        }
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Protosynthesis] = new()
            {
                Id = AbilityId.Protosynthesis,
                Name = "Protosynthesis",
                Num = 281,
                Rating = 3.0,
                Condition = ConditionId.Protosynthesis,
                // OnSwitchInPriority = -2
                OnSwitchIn = new OnSwitchInEventInfo((_, _) => { }, -2),
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    battle.SingleEvent(EventId.WeatherChange, battle.Effect, battle.EffectState,
                        pokemon);
                }),
                OnWeatherChange = new OnWeatherChangeEventInfo((battle, pokemon, _, _) =>
                {
                    // Protosynthesis is not affected by Utility Umbrella
                    if (battle.Field.IsWeather(ConditionId.SunnyDay))
                    {
                        pokemon.AddVolatile(ConditionId.Protosynthesis);
                    }
                    else if (!(pokemon.GetVolatile(ConditionId.Protosynthesis)?.FromBooster ??
                               false) &&
                             !battle.Field.IsWeather(ConditionId.SunnyDay))
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Protosynthesis]);
                    }
                }),
                OnEnd = new OnEndEventInfo((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                    psfp.Pokemon.DeleteVolatile(ConditionId.Protosynthesis);
                    battle.Add("-end", psfp.Pokemon, "Protosynthesis", "[silent]");
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
            [AbilityId.PsychicSurge] = new()
            {
                Id = AbilityId.PsychicSurge,
                Name = "Psychic Surge",
                Num = 227,
                Rating = 4.0,
                OnStart = new OnStartEventInfo((battle, _) =>
                {
                    battle.Field.SetTerrain(_library.Conditions[ConditionId.PsychicTerrain]);
                }),
            },
            [AbilityId.PunkRock] = new()
            {
                Id = AbilityId.PunkRock,
                Name = "Punk Rock",
                Num = 244,
                Rating = 3.5,
                Flags = new AbilityFlags { Breakable = true },
                // OnBasePowerPriority = 7
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Flags.Sound == true)
                    {
                        battle.Debug("Punk Rock boost");
                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 7),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, _, _, move) =>
                    {
                        if (move.Flags.Sound == true)
                        {
                            battle.Debug("Punk Rock weaken");
                            battle.ChainModify(0.5);
                            return battle.FinalModify(damage);
                        }

                        return damage;
                    }),
            },
            [AbilityId.PurePower] = new()
            {
                Id = AbilityId.PurePower,
                Name = "Pure Power",
                Num = 74,
                Rating = 5.0,
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, _, _, _) =>
                {
                    battle.ChainModify(2);
                    return battle.FinalModify(atk);
                }, 5),
            },
            [AbilityId.PurifyingSalt] = new()
            {
                Id = AbilityId.PurifyingSalt,
                Name = "Purifying Salt",
                Num = 272,
                Rating = 4.0,
                Flags = new AbilityFlags { Breakable = true },
                OnSetStatus = new OnSetStatusEventInfo((battle, _, target, _, effect) =>
                {
                    if (effect is ActiveMove { Status: not ConditionId.None })
                    {
                        battle.Add("-immune", target, "[from] ability: Purifying Salt");
                    }

                    return false;
                }),
                OnTryAddVolatile = new OnTryAddVolatileEventInfo((battle, status, target, _, _) =>
                {
                    if (status.Id == ConditionId.Yawn)
                    {
                        battle.Add("-immune", target, "[from] ability: Purifying Salt");
                        return null;
                    }

                    return new VoidReturn();
                }),
                // OnSourceModifyAtkPriority = 6
                OnSourceModifyAtk = new OnSourceModifyAtkEventInfo((battle, atk, _, _, move) =>
                {
                    if (move.Type == MoveType.Ghost)
                    {
                        battle.Debug("Purifying Salt weaken");
                        battle.ChainModify(0.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 6),
                // OnSourceModifySpAPriority = 5
                OnSourceModifySpA = new OnSourceModifySpAEventInfo((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Ghost)
                    {
                        battle.Debug("Purifying Salt weaken");
                        battle.ChainModify(0.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
            },

            // ==================== 'Q' Abilities ====================
            [AbilityId.QuarkDrive] = new()
            {
                Id = AbilityId.QuarkDrive,
                Name = "Quark Drive",
                Num = 282,
                Rating = 3.0,
                Condition = ConditionId.QuarkDrive,
                // OnSwitchInPriority = -2
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
            [AbilityId.QueenlyMajesty] = new()
            {
                Id = AbilityId.QueenlyMajesty,
                Name = "Queenly Majesty",
                Num = 214,
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
                            Pokemon: var dazzlingHolder,
                        })
                        return new VoidReturn();

                    if ((source.IsAlly(dazzlingHolder) || move.Target == MoveTarget.All) &&
                        move.Priority > 0.1)
                    {
                        battle.AttrLastMove("[still]");
                        battle.Add("cant", dazzlingHolder, "ability: Queenly Majesty", move.Name,
                            $"[of] {target}");
                        return false;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.QuickDraw] = new()
            {
                Id = AbilityId.QuickDraw,
                Name = "Quick Draw",
                Num = 259,
                Rating = 2.5,
                // OnFractionalPriorityPriority = -1
                OnFractionalPriority = new OnFractionalPriorityEventInfo(
                    (ModifierSourceMoveHandler)((battle, _, pokemon, _, move) =>
                    {
                        if (move.Category != MoveCategory.Status && battle.RandomChance(3, 10))
                        {
                            battle.Add("-activate", pokemon, "ability: Quick Draw");
                            return 0.1;
                        }

                        return new VoidReturn();
                    }), -1),
            },
            [AbilityId.QuickFeet] = new()
            {
                Id = AbilityId.QuickFeet,
                Name = "Quick Feet",
                Num = 95,
                Rating = 2.5,
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    if (pokemon.Status != ConditionId.None)
                    {
                        battle.ChainModify(1.5);
                        return battle.FinalModify(spe);
                    }

                    return spe;
                }),
            },

            // ==================== 'R' Abilities ====================
            [AbilityId.RainDish] = new()
            {
                Id = AbilityId.RainDish,
                Name = "Rain Dish",
                Num = 44,
                Rating = 1.5,
                OnWeather = new OnWeatherEventInfo((battle, target, _, effect) =>
                {
                    if (target.HasItem(ItemId.UtilityUmbrella)) return;
                    if (effect.Id is ConditionId.RainDance or ConditionId.PrimordialSea)
                    {
                        battle.Heal(target.BaseMaxHp / 16);
                    }
                }),
            },
            [AbilityId.Rattled] = new()
            {
                Id = AbilityId.Rattled,
                Name = "Rattled",
                Num = 155,
                Rating = 1.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, _, _, move) =>
                {
                    if (move.Type is MoveType.Dark or MoveType.Bug or MoveType.Ghost)
                    {
                        battle.Boost(new SparseBoostsTable { Spe = 1 });
                    }
                }),
                OnAfterBoost = new OnAfterBoostEventInfo((battle, boost, _, _, effect) =>
                {
                    // TS checks effect?.name === 'Intimidate' - using Id enum instead
                    if (effect is Ability { Id: AbilityId.Intimidate } && boost.Atk != null)
                    {
                        battle.Boost(new SparseBoostsTable { Spe = 1 });
                    }
                }),
            },
            [AbilityId.Receiver] = new()
            {
                Id = AbilityId.Receiver,
                Name = "Receiver",
                Num = 222,
                Rating = 0.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                },
                OnAllyFaint = new OnAllyFaintEventInfo((_, target, pokemon, _) =>
                {
                    if (pokemon.Hp == 0) return;
                    Ability ability = target.GetAbility();
                    if (ability.Flags.NoReceiver == true || ability.Id == AbilityId.None) return;
                    pokemon.SetAbility(ability.Id, target);
                }),
            },
            [AbilityId.Reckless] = new()
            {
                Id = AbilityId.Reckless,
                Name = "Reckless",
                Num = 120,
                Rating = 3.0,
                // OnBasePowerPriority = 23
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Recoil != null || move.HasCrashDamage == true)
                    {
                        battle.Debug("Reckless boost");
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 23),
            },
            [AbilityId.Refrigerate] = new()
            {
                Id = AbilityId.Refrigerate,
                Name = "Refrigerate",
                Num = 174,
                Rating = 4.0,
                // OnModifyTypePriority = -1
                OnModifyType = new OnModifyTypeEventInfo((battle, move, pokemon, _) =>
                {
                    // Non-Gen9 moves excluded: MultiAttack, NaturalGift, Technoblast
                    MoveId[] noModifyType =
                    [
                        MoveId.Judgment, MoveId.RevelationDance, MoveId.TerrainPulse,
                        MoveId.WeatherBall,
                    ];
                    if (move.Type == MoveType.Normal &&
                        !noModifyType.Contains(move.Id) &&
                        !(move.Id == MoveId.TeraBlast && pokemon.Terastallized != null))
                    {
                        move.Type = MoveType.Ice;
                        move.TypeChangerBoosted = battle.Effect;
                    }
                }, -1),
                // OnBasePowerPriority = 23
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.TypeChangerBoosted == battle.Effect)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 23),
            },
            [AbilityId.Regenerator] = new()
            {
                Id = AbilityId.Regenerator,
                Name = "Regenerator",
                Num = 144,
                Rating = 4.5,
                OnSwitchOut = new OnSwitchOutEventInfo((_, pokemon) =>
                {
                    pokemon.Heal(pokemon.BaseMaxHp / 3);
                }),
            },
            [AbilityId.Ripen] = new()
            {
                Id = AbilityId.Ripen,
                Name = "Ripen",
                Num = 247,
                Rating = 2.0,
                OnTryHeal = new OnTryHealEventInfo(
                    (Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>)((battle, damage,
                        target, _, effect) =>
                    {
                        if (effect.EffectStateId == ItemId.BerryJuice ||
                            effect.EffectStateId == ItemId.Leftovers)
                        {
                            battle.Add("-activate", target, "ability: Ripen");
                        }

                        if (effect is Item { IsBerry: true })
                        {
                            battle.ChainModify(2);
                            return IntBoolUnion.FromInt(battle.FinalModify(damage));
                        }

                        return IntBoolUnion.FromInt(damage);
                    })),
                OnChangeBoost = new OnChangeBoostEventInfo((_, boost, _, _, effect) =>
                {
                    if (effect is Item { IsBerry: true })
                    {
                        if (boost.Atk != null) boost.Atk *= 2;
                        if (boost.Def != null) boost.Def *= 2;
                        if (boost.SpA != null) boost.SpA *= 2;
                        if (boost.SpD != null) boost.SpD *= 2;
                        if (boost.Spe != null) boost.Spe *= 2;
                        if (boost.Accuracy != null) boost.Accuracy *= 2;
                        if (boost.Evasion != null) boost.Evasion *= 2;
                    }
                }),
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo(
                    (battle, damage, _, target, _) =>
                    {
                        if (target.AbilityState.BerryWeaken == true)
                        {
                            target.AbilityState.BerryWeaken = false;
                            battle.ChainModify(0.5);
                            return DoubleVoidUnion.FromDouble(battle.FinalModify(damage));
                        }

                        return DoubleVoidUnion.FromVoid();
                    }, -1),
                OnTryEatItem = new OnTryEatItemEventInfo(
                    OnTryEatItem.FromFunc((battle, _, pokemon) =>
                    {
                        battle.Add("-activate", pokemon, "ability: Ripen");
                        return BoolVoidUnion.FromVoid();
                    }), -1),
                OnEatItem = new OnEatItemEventInfo((_, item, pokemon, _, _) =>
                {
                    ItemId[] weakenBerries =
                    [
                        ItemId.BabiriBerry, ItemId.ChartiBerry, ItemId.ChilanBerry,
                        ItemId.ChopleBerry, ItemId.CobaBerry, ItemId.ColburBerry,
                        ItemId.HabanBerry, ItemId.KasibBerry, ItemId.KebiaBerry,
                        ItemId.OccaBerry, ItemId.PasshoBerry, ItemId.PayapaBerry,
                        ItemId.RindoBerry, ItemId.RoseliBerry, ItemId.ShucaBerry,
                        ItemId.TangaBerry, ItemId.WacanBerry, ItemId.YacheBerry,
                    ];
                    // Record if the pokemon ate a berry to resist the attack
                    pokemon.AbilityState.BerryWeaken =
                        Array.Exists(weakenBerries, id => id == item.Id);
                }),
            },
            [AbilityId.Rivalry] = new()
            {
                Id = AbilityId.Rivalry,
                Name = "Rivalry",
                Num = 79,
                Rating = 0.0,
                // OnBasePowerPriority = 24
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, attacker, defender, _) =>
                {
                    if (attacker.Gender != GenderId.N && defender.Gender != GenderId.N)
                    {
                        if (attacker.Gender == defender.Gender)
                        {
                            battle.Debug("Rivalry boost");
                            battle.ChainModify(1.25);
                            return battle.FinalModify(basePower);
                        }
                        else
                        {
                            battle.Debug("Rivalry weaken");
                            battle.ChainModify(0.75);
                            return battle.FinalModify(basePower);
                        }
                    }

                    return basePower;
                }, 24),
            },
            [AbilityId.RksSystem] = new()
            {
                Id = AbilityId.RksSystem,
                Name = "RKS System",
                Num = 225,
                Rating = 4.0,
                // RKS System's type-changing itself is implemented in statuses
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
            [AbilityId.RockHead] = new()
            {
                Id = AbilityId.RockHead,
                Name = "Rock Head",
                Num = 69,
                Rating = 3.0,
                OnDamage = new OnDamageEventInfo((battle, _, _, _, effect) =>
                {
                    if (effect is Condition { Id: ConditionId.Recoil })
                    {
                        if (battle.ActiveMove == null)
                            throw new InvalidOperationException("Battle.ActiveMove is null");
                        if (battle.ActiveMove.Id != MoveId.Struggle) return null;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.RockyPayload] = new()
            {
                Id = AbilityId.RockyPayload,
                Name = "Rocky Payload",
                Num = 276,
                Rating = 3.5,
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, _, _, move) =>
                {
                    if (move.Type == MoveType.Rock)
                    {
                        battle.Debug("Rocky Payload boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Rock)
                    {
                        battle.Debug("Rocky Payload boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.RoughSkin] = new()
            {
                Id = AbilityId.RoughSkin,
                Name = "Rough Skin",
                Num = 24,
                Rating = 2.5,
                // OnDamagingHitOrder = 1
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target, true))
                    {
                        battle.Damage(source.BaseMaxHp / 8, source, target);
                    }
                }, 1),
            },
            [AbilityId.RunAway] = new()
            {
                Id = AbilityId.RunAway,
                Name = "Run Away",
                Num = 50,
                Rating = 0.0,
                // No battle effect
            },
        };
    }
}