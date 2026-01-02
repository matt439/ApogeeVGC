using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;
using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsPqr()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.Paralysis] = new()
            {
                Id = ConditionId.Paralysis,
                Name = "Paralysis",
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Electric],
                OnStart = new OnStartEventInfo((battle, target, source, sourceEffect) =>
                {
                    if (!battle.DisplayUi) return new VoidReturn();

                    if (sourceEffect is Ability)
                    {
                        battle.Add("-status", target, "par", "[from] ability: " + sourceEffect.Name,
                            $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "par");
                    }

                    return new VoidReturn();
                }),
                //OnModifySpePriority = -101,
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                    {
                        spe = battle.FinalModify(spe);
                        if (!pokemon.HasAbility(AbilityId.QuickFeet))
                        {
                            spe = (int)Math.Floor(spe * 50.0 / 100);
                        }

                        return spe;
                    },
                    -101),
                //OnBeforeMovePriority = 1,
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                    {
                        if (!battle.RandomChance(1, 4)) return new VoidReturn();
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "par");
                        }

                        return false;
                    },
                    1),
            },
            [ConditionId.Poison] = new()
            {
                Id = ConditionId.Poison,
                Name = "Poison",
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Poison, PokemonType.Steel],
                OnStart = new OnStartEventInfo((battle, target, source, sourceEffect) =>
                {
                    if (!battle.DisplayUi) return new VoidReturn();

                    if (sourceEffect.EffectType == EffectType.Ability)
                    {
                        battle.Add("-status", target, "psn", "[from] ability: " +
                                                             sourceEffect.Name, $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "psn");
                    }

                    return new VoidReturn();
                }),
            },
            [ConditionId.Protect] = new()
            {
                Id = ConditionId.Protect,
                Name = "Protect",
                Duration = 1,
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Protect,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    battle.Debug($"[Protect.OnStart] Adding Protect volatile to {target.Name}");
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "Protect");
                    }

                    return new VoidReturn();
                }),
                //OnTryHitPriority = 3,
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                    {
                        battle.Debug(
                            $"[Protect.OnTryHit] CALLED! Target={target.Name}, Source={source.Name}, Move={move.Name}, HasProtectFlag={move.Flags.Protect ?? false}");

                        if (!(move.Flags.Protect ?? false))
                        {
                            return new VoidReturn();
                        }

                        if (move.SmartTarget ?? false)
                        {
                            move.SmartTarget = false;
                        }
                        else if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Protect");
                        }

                        EffectState? lockedMove = source.GetVolatile(ConditionId.LockedMove);
                        if (lockedMove is not null &&
                            source.Volatiles[ConditionId.LockedMove].Duration == 2)
                        {
                            source.RemoveVolatile(_library.Conditions[ConditionId.LockedMove]);
                        }

                        battle.Debug("[Protect.OnTryHit] Returning Empty (block move)");
                        return new Empty(); // in place of Battle.NOT_FAIL ("")
                    },
                    3),
            },
            [ConditionId.QuarkDrive] = new()
            {
                Id = ConditionId.QuarkDrive,
                Name = "Quark Drive",
                EffectType = EffectType.Condition,
                NoCopy = true,
                AssociatedAbility = AbilityId.QuarkDrive,
                OnStart = new OnStartEventInfo((battle, pokemon, _, effect) =>
                {
                    if (effect is Item item)
                    {
                        if (item.Id == ItemId.BoosterEnergy)
                        {
                            battle.EffectState.FromBooster = true;
                            if (battle.DisplayUi)
                            {
                                battle.Add("-activate", pokemon, "ability: Quark Drive",
                                    "[fromitem]");
                            }
                        }
                    }
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon, "ability: Quark Drive");
                    }

                    battle.EffectState.BestStat = pokemon.GetBestStat(false, true);
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "quarkdrive" + battle.EffectState.BestStat);
                    }

                    return new VoidReturn();
                }),
                //OnModifyAtkPriority = 5,
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.Atk ||
                            pokemon.IgnoringAbility())
                        {
                            return atk;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Quark Drive atk boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(atk);
                    },
                    5),
                //OnModifyDefPriority = 6,
                OnModifyDef = new OnModifyDefEventInfo((battle, def, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.Def ||
                            pokemon.IgnoringAbility())
                        {
                            return def;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Quark Drive def boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(def);
                    },
                    6),
                //OnModifySpAPriority = 5,
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.SpA ||
                            pokemon.IgnoringAbility())
                        {
                            return spa;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Quark Drive spa boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(spa);
                    },
                    5),
                //OnModifySpDPriority = 6,
                OnModifySpD = new OnModifySpDEventInfo((battle, spd, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.SpD ||
                            pokemon.IgnoringAbility())
                        {
                            return spd;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Quark Drive spd boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(spd);
                    },
                    6),
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.Spe ||
                        pokemon.IgnoringAbility())
                    {
                        return spe;
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Debug("Quark Drive spe boost");
                    }

                    battle.ChainModify(1.5);
                    return battle.FinalModify(spe);
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Quark Drive");
                    }
                }),
            },
            [ConditionId.Recharge] = new()
            {
                Id = ConditionId.Recharge,
                Name = "Recharge",
                EffectType = EffectType.Condition,
                // Recharge is handled by MustRecharge condition
                // This is just a marker condition
            },
            [ConditionId.Rest] = new()
            {
                Id = ConditionId.Rest,
                Name = "Rest",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Rest,
                // Rest doesn't create a volatile condition
                // It sets sleep status with statusState.time = 3
                // This is just a marker condition
            },
            [ConditionId.Recoil] = new()
            {
                Id = ConditionId.Recoil,
                Name = "Recoil",
                EffectType = EffectType.Condition,
            },
            [ConditionId.Reflect] = new()
            {
                Id = ConditionId.Reflect,
                Name = "Reflect",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Reflect,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, _, source, _) =>
                    source.HasItem(ItemId.LightClay) ? 8 : 5),
                OnAnyModifyDamage =
                    new OnAnyModifyDamageEventInfo((battle, _, source, target, move) =>
                    {
                        if (target != source &&
                            battle.EffectState.Target is SideEffectStateTarget side &&
                            side.Side.HasAlly(target) &&
                            battle.GetCategory(move) == MoveCategory.Physical)
                        {
                            if (!target.GetMoveHitData(move).Crit && !(move.Infiltrates ?? false))
                            {
                                if (battle.DisplayUi)
                                {
                                    battle.Debug("Reflect weaken");
                                }

                                return battle.ActivePerHalf > 1
                                    ? battle.ChainModify([2732, 4096])
                                    : battle.ChainModify(0.5);
                            }
                        }

                        return new VoidReturn();
                    }),
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "Reflect");
                    }
                }),
                //OnSideResidualOrder = 26,
                //OnSideResidualSubOrder = 1,
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 1,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "Reflect");
                    }
                }),
            },
            [ConditionId.RevivalBlessing] = new()
            {
                Id = ConditionId.RevivalBlessing,
                Name = "Revival Blessing",
                EffectType = EffectType.Condition,
                Duration = 1,
                AssociatedMove = MoveId.RevivalBlessing,
                // Note: Revival Blessing's effect is handled in Side
            },
            [ConditionId.PrimordialSea] = new()
            {
                Id = ConditionId.PrimordialSea,
                Name = "PrimordialSea",
                EffectType = EffectType.Weather,
                Duration = 0,
                //OnTryMovePriority = 1,
                OnTryMove = new OnTryMoveEventInfo((battle, attacker, _, move) =>
                    {
                        if (move.Type == MoveType.Fire && move.Category != MoveCategory.Status)
                        {
                            battle.Debug("Primordial Sea fire suppress");
                            if (battle.DisplayUi)
                            {
                                battle.Add("-fail", attacker, move, "[from] Primordial Sea");
                                battle.AttrLastMove("[still]");
                            }

                            return null;
                        }

                        return new VoidReturn();
                    },
                    1),
                OnWeatherModifyDamage =
                    new OnWeatherModifyDamageEventInfo((battle, _, _, defender, move) =>
                    {
                        if (defender.HasItem(ItemId.UtilityUmbrella)) return new VoidReturn();
                        if (move.Type == MoveType.Water)
                        {
                            battle.Debug("Rain water boost");
                            return battle.ChainModify(1.5);
                        }

                        return new VoidReturn();
                    }),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-weather", "PrimordialSea", "[from] ability: " + effect?.Name,
                            $"[of] {source}");
                    }
                }),
                //OnFieldResidualOrder = 1,
                OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "PrimordialSea", "[upkeep]");
                        }

                        battle.EachEvent(EventId.Weather);
                    },
                    1),
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-weather", "none");
                    }
                }),
            },
            [ConditionId.RainDance] = new()
            {
                Id = ConditionId.RainDance,
                Name = "RainDance",
                EffectType = EffectType.Weather,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                    source.HasItem(ItemId.DampRock) ? 8 : 5),
                OnWeatherModifyDamage =
                    new OnWeatherModifyDamageEventInfo((battle, _, _, defender, move) =>
                    {
                        if (defender.HasItem(ItemId.UtilityUmbrella)) return new VoidReturn();
                        if (move.Type == MoveType.Water)
                        {
                            battle.Debug("rain water boost");
                            return battle.ChainModify(1.5);
                        }

                        if (move.Type == MoveType.Fire)
                        {
                            battle.Debug("rain fire suppress");
                            return battle.ChainModify(0.5);
                        }

                        return new VoidReturn();
                    }),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (!battle.DisplayUi) return;
                    if (effect is Ability)
                    {
                        if (battle.Gen <= 5) battle.EffectState.Duration = 0;
                        battle.Add("-weather", "RainDance", "[from] ability: " + effect.Name,
                            $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-weather", "RainDance");
                    }
                }),
                //OnFieldResidualOrder = 1,
                OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "RainDance", "[upkeep]");
                        }

                        battle.EachEvent(EventId.Weather);
                    },
                    1),
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-weather", "none");
                    }
                }),
            },
            [ConditionId.Roost] = new()
            {
                Id = ConditionId.Roost,
                Name = "Roost",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Roost,
                Duration = 1,
                OnResidual = new OnResidualEventInfo((_, _, _, _) =>
                {
                    // Duration handled automatically
                }, 25),
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    // Check if pokemon is terastallized - if so and has Flying type, Roost's type suppression fails
                    if (target.Terastallized is not null)
                    {
                        if (target.HasType(PokemonType.Flying))
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Hint(
                                    "Roost's type suppression effect does not activate when a Terastallized Pokémon has the Flying type.");
                            }

                            return BoolVoidUnion.FromBool(false);
                        }
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "move: Roost");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnType = new OnTypeEventInfo((battle, types, _) =>
                {
                    battle.EffectState.TypeWas = types.FirstOrDefault();
                    // Filter out Flying type
                    return types.Where(t => t != PokemonType.Flying).ToArray();
                }, -1),
            },
            [ConditionId.Rollout] = new()
            {
                Id = ConditionId.Rollout,
                Name = "Rollout",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Rollout,
                // Rollout uses LockedMove for the locking behavior and RolloutStorage for damage scaling
                // This is just a marker condition
            },
            [ConditionId.RolloutStorage] = new()
            {
                Id = ConditionId.RolloutStorage,
                Name = "RolloutStorage",
                EffectType = EffectType.Condition,
                Duration = 2,
                OnBasePower = new OnBasePowerEventInfo((_, _, source, _, move) =>
                {
                    int bp = Math.Max(1, move.BasePower);
                    source.Volatiles.TryGetValue(ConditionId.RolloutStorage,
                        out EffectState? rolloutState);
                    int hitCount = rolloutState?.ContactHitCount ?? 0;
                    bp *= (int)Math.Pow(2, hitCount);
                    if (source.Volatiles.ContainsKey(ConditionId.DefenseCurl))
                    {
                        bp *= 2;
                    }

                    source.RemoveVolatile(_library.Conditions[ConditionId.RolloutStorage]);
                    return bp;
                }),
            },

            // New P and Q conditions
            [ConditionId.PerishSong] = new()
            {
                Id = ConditionId.PerishSong,
                Name = "Perish Song",
                EffectType = EffectType.Condition,
                Duration = 4,
                AssociatedMove = MoveId.PerishSong,
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "perish0");
                    }

                    target.Faint();
                }),
                // OnResidualOrder = 24
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (!pokemon.Volatiles.TryGetValue(ConditionId.PerishSong,
                            out EffectState? state)) return;
                    int duration = state.Duration ?? 0;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, $"perish{duration}");
                    }
                }, 24),
            },
            [ConditionId.Poltergeist] = new()
            {
                Id = ConditionId.Poltergeist,
                Name = "Poltergeist",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Poltergeist,
                // Note: Poltergeist's item check is handled in the move's OnTry/OnTryHit handlers,
                // not as a volatile condition. This is just a marker condition.
            },
            [ConditionId.Powder] = new()
            {
                Id = ConditionId.Powder,
                Name = "Powder",
                EffectType = EffectType.Condition,
                Duration = 1,
                // Note: Powder is marked as isNonstandard: "Past" in TypeScript, meaning it's not in Gen 9.
                // Implementing for completeness in case it's ever needed.
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "Powder");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                // OnTryMovePriority = -1
                OnTryMove = new OnTryMoveEventInfo((battle, pokemon, _, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "move: Powder");
                        }

                        // Damage 1/4 of max HP, minimum 1
                        int damage = Math.Max(1, (int)Math.Round(pokemon.MaxHp / 4.0));
                        battle.Damage(damage);
                        battle.AttrLastMove("[still]");
                        return BoolEmptyVoidUnion.FromBool(false);
                    }

                    return BoolEmptyVoidUnion.FromVoid();
                }, -1),
            },
            [ConditionId.PowerTrick] = new()
            {
                Id = ConditionId.PowerTrick,
                Name = "Power Trick",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.PowerTrick,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Power Trick");
                    }

                    // Swap Attack and Defense stored stats
                    int newAtk = pokemon.StoredStats[StatIdExceptHp.Def];
                    int newDef = pokemon.StoredStats[StatIdExceptHp.Atk];
                    pokemon.StoredStats[StatIdExceptHp.Atk] = newAtk;
                    pokemon.StoredStats[StatIdExceptHp.Def] = newDef;
                    return BoolVoidUnion.FromVoid();
                }),
                OnCopy = new OnCopyEventInfo((_, pokemon) =>
                {
                    // Re-swap when copying (e.g., Baton Pass) to maintain the swapped state
                    int newAtk = pokemon.StoredStats[StatIdExceptHp.Def];
                    int newDef = pokemon.StoredStats[StatIdExceptHp.Atk];
                    pokemon.StoredStats[StatIdExceptHp.Atk] = newAtk;
                    pokemon.StoredStats[StatIdExceptHp.Def] = newDef;
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Power Trick");
                    }

                    // Swap back Attack and Defense stored stats
                    int newAtk = pokemon.StoredStats[StatIdExceptHp.Def];
                    int newDef = pokemon.StoredStats[StatIdExceptHp.Atk];
                    pokemon.StoredStats[StatIdExceptHp.Atk] = newAtk;
                    pokemon.StoredStats[StatIdExceptHp.Def] = newDef;
                }),
                OnRestart = new OnRestartEventInfo((_, pokemon, _, _) =>
                {
                    // Using Power Trick again removes the volatile (toggles off)
                    pokemon.RemoveVolatile(_library.Conditions[ConditionId.PowerTrick]);
                    return BoolVoidUnion.FromVoid();
                }),
            },
            [ConditionId.PsychicNoise] = new()
            {
                Id = ConditionId.PsychicNoise,
                Name = "Psychic Noise",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.PsychicNoise,
                // Note: Psychic Noise applies the HealBlock volatile as a secondary effect.
                // The move's secondary { chance: 100, volatileStatus: 'healblock' } handles this.
                // HealBlock.DurationCallback checks for Psychic Noise name to set duration to 2.
                // This condition exists only as a marker for the move association.
            },
            [ConditionId.QuickGuard] = new()
            {
                Id = ConditionId.QuickGuard,
                Name = "Quick Guard",
                EffectType = EffectType.Condition,
                Duration = 1,
                AssociatedMove = MoveId.QuickGuard,
                OnSideStart = new OnSideStartEventInfo((battle, _, source, _) =>
                {
                    if (battle.DisplayUi && source != null)
                    {
                        battle.Add("-singleturn", source, "Quick Guard");
                    }
                }),
                // OnTryHitPriority = 4
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    // Quick Guard blocks moves with positive priority (> 0.1 to account for fractional priority)
                    // It blocks 0 priority moves boosted by Prankster or Gale Wings
                    // Quick Claw/Custap Berry priority boosts do NOT count
                    if (move.Priority <= 0.1) return BoolIntEmptyVoidUnion.FromVoid();

                    // Check if move has protect flag
                    if (!(move.Flags.Protect ?? false))
                    {
                        // G-Max One Blow and G-Max Rapid Flow bypass protection
                        // (Not relevant for Gen 9 VGC but included for completeness)
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Quick Guard");
                    }

                    // Reset Outrage counter if source has lockedmove volatile
                    if (source.Volatiles.TryGetValue(ConditionId.LockedMove,
                            out EffectState? lockedMoveState))
                    {
                        if (lockedMoveState.Duration == 2)
                        {
                            source.RemoveVolatile(_library.Conditions[ConditionId.LockedMove]);
                        }
                    }

                    return new Empty(); // Equivalent to Battle.NOT_FAIL
                }, 4),
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) =>
                {
                    // Duration handled automatically
                })
                {
                    Order = 26,
                    SubOrder = 2,
                },
                OnSideEnd = new OnSideEndEventInfo((_, _) =>
                {
                    // Silent end - Quick Guard doesn't announce when it ends
                }),
            },
            [ConditionId.PartiallyTrapped] = new()
            {
                Id = ConditionId.PartiallyTrapped,
                Name = "Partially Trapped",
                EffectType = EffectType.Condition,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((battle, _, source, _) =>
                {
                    if (source != null && source.HasItem(ItemId.GripClaw))
                    {
                        return 8;
                    }

                    return battle.Random(5, 7);
                }),
                OnStart = new OnStartEventInfo((battle, pokemon, source, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon,
                            $"move: {battle.EffectState.SourceEffect?.Name ?? "unknown"}",
                            $"[of] {source}");
                    }

                    battle.EffectState.BoundDivisor =
                        (source != null && source.HasItem(ItemId.BindingBand)) ? 6 : 8;
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    Pokemon? source = battle.EffectState.Source;
                    // G-Max Centiferno and G-Max Sandblast continue even after the user leaves the field
                    IEffect? sourceEffect = battle.EffectState.SourceEffect;
                    bool gmaxEffect = sourceEffect is
                        { Name: "G-Max Centiferno" or "G-Max Sandblast" };

                    if (source != null &&
                        (!source.IsActive || source.Hp <= 0 || source.ActiveTurns <= 0) &&
                        !gmaxEffect)
                    {
                        pokemon.DeleteVolatile(ConditionId.PartiallyTrapped);
                        if (battle.DisplayUi)
                        {
                            battle.Add("-end", pokemon, sourceEffect?.Name ?? "Partially Trapped",
                                "[partiallytrapped]", "[silent]");
                        }

                        return;
                    }

                    battle.Damage(pokemon.BaseMaxHp / (battle.EffectState.BoundDivisor ?? 8));
                }, 13),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon,
                            battle.EffectState.SourceEffect?.Name ?? "Partially Trapped",
                            "[partiallytrapped]");
                    }
                }),
                OnTrapPokemon = new OnTrapPokemonEventInfo((battle, pokemon) =>
                {
                    IEffect? sourceEffect = battle.EffectState.SourceEffect;
                    bool gmaxEffect = sourceEffect is
                        { Name: "G-Max Centiferno" or "G-Max Sandblast" };
                    Pokemon? source = battle.EffectState.Source;
                    if (source is { IsActive: true } || gmaxEffect)
                    {
                        pokemon.TryTrap();
                    }
                }),
            },
            [ConditionId.PartialTrappingLock] = new()
            {
                Id = ConditionId.PartialTrappingLock,
                Name = "Partial Trapping Lock",
                EffectType = EffectType.Condition,
                // This is a marker condition for moves that create partial trapping
                // The actual trapping is handled by PartiallyTrapped condition
            },
            [ConditionId.PartiallyTrappedFireSpin] = new()
            {
                Id = ConditionId.PartiallyTrappedFireSpin,
                Name = "Fire Spin",
                EffectType = EffectType.Condition,
                // Alias/variant of PartiallyTrapped for Fire Spin
                // Uses the same mechanics as PartiallyTrapped
            },
            [ConditionId.PetalDance] = new()
            {
                Id = ConditionId.PetalDance,
                Name = "Petal Dance",
                EffectType = EffectType.Condition,
                // Petal Dance uses the LockedMove condition mechanics
                // This is a marker for the specific move
            },
            [ConditionId.Safeguard] = new()
            {
                Id = ConditionId.Safeguard,
                Name = "Safeguard",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Safeguard,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((battle, _, source, _) =>
                {
                    if (source != null && source.HasAbility(AbilityId.Persistent))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", source, "ability: Persistent",
                                "[move] Safeguard");
                        }

                        return 7;
                    }

                    return 5;
                }),
                OnSetStatus = new OnSetStatusEventInfo((battle, _, target, source, effect) =>
                {
                    if (effect == null || source == null || effect is Condition
                        {
                            Id: ConditionId.Yawn
                        }) return BoolVoidUnion.FromVoid();
                    // Check if move has Infiltrates and target is not ally of source
                    if (effect is ActiveMove { Infiltrates: true } && !target.IsAlly(source))
                        return BoolVoidUnion.FromVoid();
                    if (target != source)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("interrupting setStatus");
                            // Show activation message for Synchronize ability or moves without secondaries
                            if (effect is Ability { Name: "Synchronize" } or ActiveMove
                                {
                                    Secondaries: null
                                })
                            {
                                battle.Add("-activate", target, "move: Safeguard");
                            }
                        }

                        return BoolVoidUnion.FromBool(false);
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnTryAddVolatile =
                    new OnTryAddVolatileEventInfo((battle, status, target, source, effect) =>
                    {
                        if (effect == null || source == null) return BoolVoidUnion.FromVoid();
                        // Check if move has Infiltrates and target is not ally of source
                        if (effect is ActiveMove { Infiltrates: true } && !target.IsAlly(source))
                            return BoolVoidUnion.FromVoid();
                        if (status?.Id is ConditionId.Confusion or ConditionId.Yawn &&
                            target != source)
                        {
                            if (battle.DisplayUi)
                            {
                                // Show activation message only for moves without secondaries
                                if (effect is ActiveMove { Secondaries: null })
                                {
                                    battle.Add("-activate", target, "move: Safeguard");
                                }
                            }

                            return BoolVoidUnion.FromBool(false);
                        }

                        return BoolVoidUnion.FromVoid();
                    }),
                OnSideStart = new OnSideStartEventInfo((battle, side, source, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        if (source != null && source.HasAbility(AbilityId.Persistent))
                        {
                            battle.Add("-sidestart", side, "Safeguard", "[persistent]");
                        }
                        else
                        {
                            battle.Add("-sidestart", side, "Safeguard");
                        }
                    }
                }),
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 3,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "Safeguard");
                    }
                }),
            },
            [ConditionId.PsychicTerrain] = new()
            {
                Id = ConditionId.PsychicTerrain,
                Name = "Psychic Terrain",
                EffectType = EffectType.Terrain,
                AssociatedMove = MoveId.PsychicTerrain,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                {
                    if (source != null && source.HasItem(ItemId.TerrainExtender))
                    {
                        return 8;
                    }

                    return 5;
                }),
                // OnTryHitPriority = 4
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    // Psychic Terrain blocks priority moves targeting grounded Pokemon
                    // Excludes moves targeting self or with priority <= 0.1
                    if (move.Priority <= 0.1 || move.Target == MoveTarget.Self)
                        return BoolIntEmptyVoidUnion.FromVoid();

                    // Target must be grounded and not semi-invulnerable
                    bool? isGrounded = target.IsGrounded();
                    bool isSemiInvulnerable = target.IsSemiInvulnerable();
                    if (!(isGrounded ?? false) || isSemiInvulnerable)
                        return BoolIntEmptyVoidUnion.FromVoid();

                    // Don't block if target is ally of source
                    if (target.IsAlly(source))
                        return BoolIntEmptyVoidUnion.FromVoid();

                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Psychic Terrain");
                    }

                    return new Empty(); // Block the move
                }, 4),
                // OnBasePowerPriority = 6
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, attacker, _, move) =>
                {
                    // Boost Psychic-type moves by 1.3x (5325/4096) when attacker is grounded
                    bool? attackerGrounded = attacker.IsGrounded();
                    bool attackerSemiInvuln = attacker.IsSemiInvulnerable();
                    if (move.Type == MoveType.Psychic && (attackerGrounded ?? false) &&
                        !attackerSemiInvuln)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("psychic terrain boost");
                        }

                        return battle.ChainModify([5325, 4096]);
                    }

                    return basePower;
                }, 6),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (battle.DisplayUi)
                    {
                        if (effect is Ability)
                        {
                            battle.Add("-fieldstart", "move: Psychic Terrain",
                                $"[from] ability: {effect.Name}", $"[of] {source}");
                        }
                        else
                        {
                            battle.Add("-fieldstart", "move: Psychic Terrain");
                        }
                    }
                }),
                OnFieldResidual = new OnFieldResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 27,
                    SubOrder = 7,
                },
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Psychic Terrain");
                    }
                }),
            },
            [ConditionId.Protosynthesis] = new()
            {
                Id = ConditionId.Protosynthesis,
                Name = "Protosynthesis",
                EffectType = EffectType.Condition,
                NoCopy = true,
                AssociatedAbility = AbilityId.Protosynthesis,
                OnStart = new OnStartEventInfo((battle, pokemon, _, effect) =>
                {
                    if (effect is Item { Id: ItemId.BoosterEnergy })
                    {
                        battle.EffectState.FromBooster = true;
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "ability: Protosynthesis",
                                "[fromitem]");
                        }
                    }
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon, "ability: Protosynthesis");
                    }

                    battle.EffectState.BestStat = pokemon.GetBestStat(false, true);
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon,
                            "protosynthesis" + battle.EffectState.BestStat);
                    }

                    return new VoidReturn();
                }),
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.Atk ||
                            pokemon.IgnoringAbility())
                        {
                            return atk;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Protosynthesis atk boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(atk);
                    },
                    5),
                // OnModifyDefPriority = 6
                OnModifyDef = new OnModifyDefEventInfo((battle, def, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.Def ||
                            pokemon.IgnoringAbility())
                        {
                            return def;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Protosynthesis def boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(def);
                    },
                    6),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.SpA ||
                            pokemon.IgnoringAbility())
                        {
                            return spa;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Protosynthesis spa boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(spa);
                    },
                    5),
                // OnModifySpDPriority = 6
                OnModifySpD = new OnModifySpDEventInfo((battle, spd, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.SpD ||
                            pokemon.IgnoringAbility())
                        {
                            return spd;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Protosynthesis spd boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(spd);
                    },
                    6),
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.Spe ||
                        pokemon.IgnoringAbility())
                    {
                        return spe;
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Debug("Protosynthesis spe boost");
                    }

                    battle.ChainModify(1.5);
                    return battle.FinalModify(spe);
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Protosynthesis");
                    }
                }),
            },
            [ConditionId.RagePowder] = new()
            {
                Id = ConditionId.RagePowder,
                Name = "Rage Powder",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.RagePowder,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", pokemon, "move: Rage Powder");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: OnFoeRedirectTarget - redirect attacks to this Pokemon (priority 1)
                // Check if source has powder immunity, check if ragePowderUser is sky dropped
                // Check if ragePowderUser is valid target, then redirect
            },
        };
    }
}