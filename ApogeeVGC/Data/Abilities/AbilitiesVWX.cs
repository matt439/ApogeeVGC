using ApogeeVGC.Data;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

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
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (battle.SuppressingAbility(pokemon)) return;
                    battle.Add("-ability", pokemon, "Vessel of Ruin");
                }),
                OnAnyModifySpA = new OnAnyModifySpAEventInfo((battle, spa, source, _, move) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder
                        })
                        return spa;
                    if (source.HasAbility(AbilityId.VesselOfRuin)) return spa;
                    if (move.RuinedSpA == null)
                        move.RuinedSpA = abilityHolder;
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
                OnAnyModifyAccuracy = new OnAnyModifyAccuracyEventInfo(
                    (battle, accuracy, _, source, _) =>
                    {
                        if (battle.EffectState.Target is not PokemonEffectStateTarget
                            {
                                Pokemon: var abilityHolder
                            })
                            return accuracy;
                        if (source.IsAlly(abilityHolder) && accuracy is int acc)
                        {
                            battle.ChainModify([4506, 4096]);
                            return battle.FinalModify(acc);
                        }

                        return accuracy;
                    }, -1),
            },
            [AbilityId.VitalSpirit] = new()
            {
                Id = AbilityId.VitalSpirit,
                Name = "Vital Spirit",
                Num = 72,
                Rating = 1.5,
                Flags = new AbilityFlags { Breakable = true },
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Sleep)
                    {
                        battle.Add("-activate", pokemon, "ability: Vital Spirit");
                        pokemon.CureStatus();
                    }
                }),
                OnSetStatus = new OnSetStatusEventInfo((battle, status, target, _, effect) =>
                {
                    if (status.Id != ConditionId.Sleep) return new VoidReturn();
                    if (effect is ActiveMove { Status: not ConditionId.None })
                    {
                        battle.Add("-immune", target, "[from] ability: Vital Spirit");
                    }

                    return false;
                }),
                OnTryAddVolatile = new OnTryAddVolatileEventInfo((battle, status, target, _, _) =>
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
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
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
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    Ability sourceAbility = source.GetAbility();
                    if (sourceAbility.Flags.FailSkillSwap == true) return;

                    if (battle.CheckMoveMakesContact(move, source, target, !source.IsAlly(target)))
                    {
                        var targetCanBeSet =
                            battle.RunEvent(EventId.SetAbility, target, source, battle.Effect,
                                sourceAbility);
                        if (targetCanBeSet is BoolRelayVar { Value: false }) return;

                        AbilityIdFalseUnion oldAbilityResult =
                            source.SetAbility(AbilityId.WanderingSpirit, target);
                        if (oldAbilityResult is FalseAbilityIdFalseUnion) return;
                        AbilityId oldAbility =
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
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Water)
                    {
                        if (battle.Heal(target.BaseMaxHp / 4) is FalseIntFalseUnion)
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
                OnSourceModifyAtk = new OnSourceModifyAtkEventInfo((battle, atk, _, _, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        battle.ChainModify(0.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                // OnSourceModifySpAPriority = 5
                OnSourceModifySpA = new OnSourceModifySpAEventInfo((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        battle.ChainModify(0.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, _, _, move) =>
                {
                    if (move.Type == MoveType.Water)
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }),
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Water)
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Burn)
                    {
                        battle.Add("-activate", pokemon, "ability: Water Bubble");
                        pokemon.CureStatus();
                    }
                }),
                OnSetStatus = new OnSetStatusEventInfo((battle, status, target, _, effect) =>
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
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, _, _, move) =>
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
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Burn)
                    {
                        battle.Add("-activate", pokemon, "ability: Water Veil");
                        pokemon.CureStatus();
                    }
                }),
                OnSetStatus = new OnSetStatusEventInfo((battle, status, target, _, effect) =>
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
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, _, move) =>
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
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Fire)
                    {
                        if (!battle.Boost(new SparseBoostsTable { Def = 2 }).IsTruthy())
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
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, source, effect) =>
                {
                    if (source != null && target == source) return;
                    bool showMsg = false;
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

                    if (showMsg && effect is not ActiveMove { Secondaries: not null } &&
                        effect is not Condition { Id: ConditionId.Octolock })
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
                OnEmergencyExit = new OnEmergencyExitEventInfo((battle, target) =>
                {
                    if (battle.CanSwitch(target.Side) == 0 || target.ForceSwitchFlag ||
                        target.SwitchFlag == true)
                        return;
                    foreach (Side side in battle.Sides)
                    {
                        foreach (Pokemon active in side.Active)
                        {
                            if (active != null)
                            {
                                active.SwitchFlag = false;
                            }
                        }
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
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, _, move) =>
                {
                    if (move.Flags.Wind == true)
                    {
                        target.AddVolatile(ConditionId.Charge);
                    }
                }, 1),
                OnSideConditionStart =
                    new OnSideConditionStartEventInfo((battle, _, _, sideCondition) =>
                    {
                        if (battle.EffectState.Target is not PokemonEffectStateTarget
                            {
                                Pokemon: var pokemon
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
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Side.GetSideCondition(ConditionId.Tailwind) != null)
                    {
                        battle.Boost(new SparseBoostsTable { Atk = 1 }, pokemon, pokemon);
                    }
                }),
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (target != source && move.Flags.Wind == true)
                    {
                        if (!battle.Boost(new SparseBoostsTable { Atk = 1 }, target, target)
                                .IsTruthy())
                        {
                            battle.Add("-immune", target, "[from] ability: Wind Rider");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }, 1),
                OnSideConditionStart =
                    new OnSideConditionStartEventInfo((battle, _, _, sideCondition) =>
                    {
                        if (battle.EffectState.Target is not PokemonEffectStateTarget
                            {
                                Pokemon: var pokemon
                            })
                            return;
                        if (sideCondition.Id == ConditionId.Tailwind)
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
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (target == source || move.Category == MoveCategory.Status ||
                        move.Id == MoveId.Struggle)
                        return new VoidReturn();
                    // SkyDrop check would go here if we implemented that move

                    battle.Debug("Wonder Guard immunity: " + move.Id);
                    MoveEffectiveness effectiveness = target.RunEffectiveness(move);
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
                OnModifyAccuracy = new OnModifyAccuracyEventInfo((battle, accuracy, _, _, move) =>
                {
                    if (move.Category == MoveCategory.Status && accuracy is int)
                    {
                        battle.Debug("Wonder Skin - setting accuracy to 50");
                        return 50;
                    }

                    return accuracy;
                }, 10),
            },

            // ==================== 'X' Abilities ====================
            // No abilities start with 'X' in the standard Pok�mon games
        };
    }
}