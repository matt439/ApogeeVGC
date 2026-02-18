using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesVwx()
    {
        return new Dictionary<AbilityId, Ability>
        {
            // ==================== 'V' Abilities ====================
            [AbilityId.VesselOfRuin] = new()
            {
                Id = AbilityId.VesselOfRuin,
                Name = "Vessel of Ruin",
                Num = 284,
                Rating = 4.5,
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (battle.SuppressingAbility(pokemon)) return;
                    battle.Add("-ability", pokemon, "Vessel of Ruin");
                }),
                OnAnyModifySpA = OnAnyModifySpAEventInfo.Create((battle, spa, source, _, move) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder,
                        } || source.HasAbility(AbilityId.VesselOfRuin))
                        return spa;
                    move.RuinedSpA ??= abilityHolder;
                    if (move.RuinedSpA != abilityHolder) return spa;
                    battle.Debug("Vessel of Ruin SpA drop");
                    battle.ChainModify(0.75);
                    return battle.FinalModify(spa);
                }),
            },
            [AbilityId.VictoryStar] = new()
            {
                Id = AbilityId.VictoryStar,
                Name = "Victory Star",
                Num = 162,
                Rating = 2.0,
                // OnAnyModifyAccuracyPriority = -1
                OnAnyModifyAccuracy = OnAnyModifyAccuracyEventInfo.Create(
                    (battle, accuracy, _, source, _) =>
                    {
                        if (battle.EffectState.Target is not PokemonEffectStateTarget
                            {
                                Pokemon: var abilityHolder,
                            })
                        {
                            return new VoidReturn();
                        }

                        // Only modify numeric accuracy
                        if (accuracy.HasValue && source.IsAlly(abilityHolder))
                        {
                            return battle.ChainModify([4506, 4096]);
                        }

                        return new VoidReturn();
                    }, -1),
            },
            [AbilityId.VitalSpirit] = new()
            {
                Id = AbilityId.VitalSpirit,
                Name = "Vital Spirit",
                Num = 72,
                Rating = 1.5,
                Flags = new AbilityFlags { Breakable = true },
                OnUpdate = OnUpdateEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Sleep)
                    {
                        battle.Add("-activate", pokemon, "ability: Vital Spirit");
                        pokemon.CureStatus();
                    }
                }),
                OnSetStatus = OnSetStatusEventInfo.Create((battle, status, target, _, effect) =>
                {
                    if (status.Id != ConditionId.Sleep) return new VoidReturn();
                    if (effect is ActiveMove { Status: not ConditionId.None })
                    {
                        battle.Add("-immune", target, "[from] ability: Vital Spirit");
                    }

                    return false;
                }),
                OnTryAddVolatile = OnTryAddVolatileEventInfo.Create((battle, status, target, _, _) =>
                {
                    if (status.Id == ConditionId.Yawn)
                    {
                        battle.Add("-immune", target, "[from] ability: Vital Spirit");
                        return null;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.VoltAbsorb] = new()
            {
                Id = AbilityId.VoltAbsorb,
                Name = "Volt Absorb",
                Num = 10,
                Rating = 3.5,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = OnTryHitEventInfo.Create((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Electric)
                    {
                        if (battle.Heal(target.BaseMaxHp / 4) is FalseIntFalseUnion)
                        {
                            battle.Add("-immune", target, "[from] ability: Volt Absorb");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }, 1),
            },

            // ==================== 'W' Abilities ====================
            [AbilityId.WanderingSpirit] = new()
            {
                Id = AbilityId.WanderingSpirit,
                Name = "Wandering Spirit",
                Num = 254,
                Rating = 2.5,
                OnDamagingHit = OnDamagingHitEventInfo.Create((battle, _, target, source, move) =>
                {
                    var sourceAbility = source.GetAbility();
                    if (sourceAbility.Flags.FailSkillSwap == true) return;

                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        var targetCanBeSet =
                            battle.RunEvent(EventId.SetAbility, target, source, battle.Effect,
                                sourceAbility);
                        if (targetCanBeSet is BoolRelayVar { Value: false } or null) return;

                        var oldAbilityResult =
                            source.SetAbility(AbilityId.WanderingSpirit, target);
                        if (oldAbilityResult is null or FalseAbilityIdFalseUnion) return;
                        var oldAbility =
                            ((AbilityIdAbilityIdFalseUnion)oldAbilityResult).AbilityId;

                        if (target.IsAlly(source))
                        {
                            battle.Add("-activate", target, "Skill Swap", "", "", $"[of] {source}");
                        }
                        else
                        {
                            var oldAbilityData =
                                battle.Library.Abilities.GetValueOrDefault(oldAbility);
                            battle.Add("-activate", target, "ability: Wandering Spirit",
                                oldAbilityData?.Name ?? oldAbility.ToString(),
                                "Wandering Spirit", $"[of] {source}");
                        }

                        target.SetAbility(oldAbility);
                    }
                }),
            },
            [AbilityId.WaterAbsorb] = new()
            {
                Id = AbilityId.WaterAbsorb,
                Name = "Water Absorb",
                Num = 11,
                Rating = 3.5,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = OnTryHitEventInfo.Create((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Water)
                    {
                        if (battle.Heal(target.BaseMaxHp / 4) is FalseIntFalseUnion or IntIntFalseUnion { Value: 0 })
                        {
                            battle.Add("-immune", target, "[from] ability: Water Absorb");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }, 1),
            },
            [AbilityId.WaterBubble] = new()
            {
                Id = AbilityId.WaterBubble,
                Name = "Water Bubble",
                Num = 199,
                Rating = 4.5,
                Flags = new AbilityFlags { Breakable = true },
                // OnSourceModifyAtkPriority = 5
                OnSourceModifyAtk = OnSourceModifyAtkEventInfo.Create((battle, atk, _, _, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        battle.ChainModify(0.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                // OnSourceModifySpAPriority = 5
                OnSourceModifySpA = OnSourceModifySpAEventInfo.Create((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        battle.ChainModify(0.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
                OnModifyAtk = OnModifyAtkEventInfo.Create((battle, atk, _, _, move) =>
                {
                    if (move.Type == MoveType.Water)
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }),
                OnModifySpA = OnModifySpAEventInfo.Create((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Water)
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }),
                OnUpdate = OnUpdateEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Burn)
                    {
                        battle.Add("-activate", pokemon, "ability: Water Bubble");
                        pokemon.CureStatus();
                    }
                }),
                OnSetStatus = OnSetStatusEventInfo.Create((battle, status, target, _, effect) =>
                {
                    if (status.Id != ConditionId.Burn) return new VoidReturn();
                    if (effect is ActiveMove { Status: not ConditionId.None })
                    {
                        battle.Add("-immune", target, "[from] ability: Water Bubble");
                    }

                    return false;
                }),
            },
            [AbilityId.WaterCompaction] = new()
            {
                Id = AbilityId.WaterCompaction,
                Name = "Water Compaction",
                Num = 195,
                Rating = 1.5,
                OnDamagingHit = OnDamagingHitEventInfo.Create((battle, _, _, _, move) =>
                {
                    if (move.Type == MoveType.Water)
                    {
                        battle.Boost(new SparseBoostsTable { Def = 2 });
                    }
                }),
            },
            [AbilityId.WaterVeil] = new()
            {
                Id = AbilityId.WaterVeil,
                Name = "Water Veil",
                Num = 41,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                OnUpdate = OnUpdateEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Burn)
                    {
                        battle.Add("-activate", pokemon, "ability: Water Veil");
                        pokemon.CureStatus();
                    }
                }),
                OnSetStatus = OnSetStatusEventInfo.Create((battle, status, target, _, effect) =>
                {
                    if (status.Id != ConditionId.Burn) return new VoidReturn();
                    if (effect is ActiveMove { Status: not ConditionId.None })
                    {
                        battle.Add("-immune", target, "[from] ability: Water Veil");
                    }

                    return false;
                }),
            },
            [AbilityId.WeakArmor] = new()
            {
                Id = AbilityId.WeakArmor,
                Name = "Weak Armor",
                Num = 133,
                Rating = 1.0,
                OnDamagingHit = OnDamagingHitEventInfo.Create((battle, _, target, _, move) =>
                {
                    if (move.Category == MoveCategory.Physical)
                    {
                        battle.Boost(new SparseBoostsTable { Def = -1, Spe = 2 }, target, target);
                    }
                }),
            },
            [AbilityId.WellBakedBody] = new()
            {
                Id = AbilityId.WellBakedBody,
                Name = "Well-Baked Body",
                Num = 273,
                Rating = 3.5,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = OnTryHitEventInfo.Create((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Fire)
                    {
                        if (!(battle.Boost(new SparseBoostsTable { Def = 2 })?.IsTruthy() ?? false))
                        {
                            battle.Add("-immune", target, "[from] ability: Well-Baked Body");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }, 1),
            },
            [AbilityId.WhiteSmoke] = new()
            {
                Id = AbilityId.WhiteSmoke,
                Name = "White Smoke",
                Num = 73,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryBoost = OnTryBoostEventInfo.Create((battle, boost, target, source, effect) =>
                {
                    if (source != null && target == source) return;
                    var showMsg = false;
                    if (boost.Atk is < 0)
                    {
                        boost.Atk = null;
                        showMsg = true;
                    }

                    if (boost.Def is < 0)
                    {
                        boost.Def = null;
                        showMsg = true;
                    }

                    if (boost.SpA is < 0)
                    {
                        boost.SpA = null;
                        showMsg = true;
                    }

                    if (boost.SpD is < 0)
                    {
                        boost.SpD = null;
                        showMsg = true;
                    }

                    if (boost.Spe is < 0)
                    {
                        boost.Spe = null;
                        showMsg = true;
                    }

                    if (boost.Accuracy is < 0)
                    {
                        boost.Accuracy = null;
                        showMsg = true;
                    }

                    if (boost.Evasion is < 0)
                    {
                        boost.Evasion = null;
                        showMsg = true;
                    }

                    // Note: Octolock check omitted - isNonstandard: "Past" in Gen 9
                    if (showMsg && effect is not ActiveMove { Secondaries: not null })
                    {
                        battle.Add("-fail", target, "unboost", "[from] ability: White Smoke",
                            $"[of] {target}");
                    }
                }),
            },
            [AbilityId.WimpOut] = new()
            {
                Id = AbilityId.WimpOut,
                Name = "Wimp Out",
                Num = 193,
                Rating = 1.0,
                OnEmergencyExit = OnEmergencyExitEventInfo.Create((battle, target) =>
                {
                    if (battle.CanSwitch(target.Side) == 0 || target.ForceSwitchFlag ||
                        target.SwitchFlag == true)
                        return;
                    foreach (var active in battle.Sides.SelectMany(side =>
                                 side.Active.OfType<Pokemon>()))
                    {
                        active.SwitchFlag = false;
                    }

                    target.SwitchFlag = true;
                    battle.Add("-activate", target, "ability: Wimp Out");
                }),
            },
            [AbilityId.WindPower] = new()
            {
                Id = AbilityId.WindPower,
                Name = "Wind Power",
                Num = 277,
                Rating = 1.0,
                // OnDamagingHitOrder = 1
                OnDamagingHit = OnDamagingHitEventInfo.Create((_, _, target, _, move) =>
                {
                    if (move.Flags.Wind == true)
                    {
                        target.AddVolatile(ConditionId.Charge);
                    }
                }, 1),
                OnSideConditionStart =
                    OnSideConditionStartEventInfo.Create((battle, _, _, sideCondition) =>
                    {
                        if (battle.EffectState.Target is not PokemonEffectStateTarget
                            {
                                Pokemon: var pokemon,
                            })
                            return;
                        if (sideCondition.Id == ConditionId.Tailwind)
                        {
                            pokemon.AddVolatile(ConditionId.Charge);
                        }
                    }),
            },
            [AbilityId.WindRider] = new()
            {
                Id = AbilityId.WindRider,
                Name = "Wind Rider",
                Num = 274,
                Rating = 3.5,
                Flags = new AbilityFlags { Breakable = true },
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.Side.GetSideCondition(ConditionId.Tailwind) != null)
                    {
                        battle.Boost(new SparseBoostsTable { Atk = 1 }, pokemon, pokemon);
                    }
                }),
                OnTryHit = OnTryHitEventInfo.Create((battle, target, source, move) =>
                {
                    if (target != source && move.Flags.Wind == true)
                    {
                        if (!(battle.Boost(new SparseBoostsTable { Atk = 1 }, target, target)
                                ?.IsTruthy() ?? false))
                        {
                            battle.Add("-immune", target, "[from] ability: Wind Rider");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }, 1),
                OnSideConditionStart =
                    OnSideConditionStartEventInfo.Create((battle, _, _, condition) =>
                    {
                        if (battle.EffectState.Target is not PokemonEffectStateTarget
                            {
                                Pokemon: var pokemon,
                            })
                            return;
                        if (condition.Id == ConditionId.Tailwind)
                        {
                            battle.Boost(new SparseBoostsTable { Atk = 1 }, pokemon, pokemon);
                        }
                    }),
            },
            [AbilityId.WonderGuard] = new()
            {
                Id = AbilityId.WonderGuard,
                Name = "Wonder Guard",
                Num = 25,
                Rating = 5.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    FailSkillSwap = true,
                    Breakable = true,
                },
                OnTryHit = OnTryHitEventInfo.Create((battle, target, source, move) =>
                {
                    if (target == source || move.Category == MoveCategory.Status ||
                        move.Id == MoveId.Struggle)
                        return new VoidReturn();
                    // SkyDrop check would go here if we implemented that move

                    battle.Debug("Wonder Guard immunity: " + move.Id);
                    var effectiveness = target.RunEffectiveness(move);
                    if (effectiveness.ToModifier() <= 0 || target.RunImmunity(move) != true)
                    {
                        if (move.SmartTarget == true)
                        {
                            move.SmartTarget = false;
                        }
                        else
                        {
                            battle.Add("-immune", target, "[from] ability: Wonder Guard");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }, 1),
            },
            [AbilityId.WonderSkin] = new()
            {
                Id = AbilityId.WonderSkin,
                Name = "Wonder Skin",
                Num = 147,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                // OnModifyAccuracyPriority = 10
                OnModifyAccuracy = OnModifyAccuracyEventInfo.Create((battle, accuracy, _, _, move) =>
                {
                    if (move.Category == MoveCategory.Status && accuracy.HasValue)
                    {
                        battle.Debug("Wonder Skin - setting accuracy to 50");
                        return 50;
                    }

                    return new VoidReturn();
                }, 10),
            },

            // ==================== 'X' Abilities ====================
            // No abilities start with 'X' in the standard Pok�mon games
        };
    }
}