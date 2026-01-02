using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsJkl()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.LaserFocus] = new()
            {
                Id = ConditionId.LaserFocus,
                Name = "Laser Focus",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.LaserFocus,
                Duration = 2,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "move: Laser Focus");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                // Guarantees critical hit for next move by returning max crit ratio
                OnModifyCritRatio = new OnModifyCritRatioEventInfo((_, _, _, _, _) => 5),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "move: Laser Focus", "[silent]");
                    }
                }),
            },
            [ConditionId.LeechSeed] = new()
            {
                Id = ConditionId.LeechSeed,
                Name = "Leech Seed",
                EffectType = EffectType.Condition,
                ImmuneTypes = [PokemonType.Grass],
                AssociatedMove = MoveId.LeechSeed,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "move: Leech Seed");
                    }

                    return new VoidReturn();
                }),
                //OnResidualOrder = 8,
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                    {
                        Pokemon? target =
                            battle.GetAtSlot(pokemon.Volatiles[ConditionId.LeechSeed].SourceSlot);
                        if (target is null || target.Fainted || target.Hp <= 0)
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Debug("Nothing to leech into");
                            }

                            return;
                        }

                        IntFalseUndefinedUnion damage =
                            battle.Damage(pokemon.BaseMaxHp / 8, pokemon, target);

                        if (damage is IntIntFalseUndefined d)
                        {
                            battle.Heal(d.Value, target, pokemon);
                        }
                    },
                    8),
            },
            [ConditionId.LightScreen] = new()
            {
                Id = ConditionId.LightScreen,
                Name = "Light Screen",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.LightScreen,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, _, source, _) =>
                    source.HasItem(ItemId.LightClay) ? 8 : 5),
                OnAnyModifyDamage =
                    new OnAnyModifyDamageEventInfo((battle, _, source, target, move) =>
                    {
                        if (target != source &&
                            battle.EffectState.Target is SideEffectStateTarget side &&
                            side.Side.HasAlly(target) &&
                            battle.GetCategory(move) == MoveCategory.Special)
                        {
                            if (!target.GetMoveHitData(move).Crit && !(move.Infiltrates ?? false))
                            {
                                if (battle.DisplayUi)
                                {
                                    battle.Debug("Light Screen weaken");
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
                        battle.Add("-sidestart", side, "move: Light Screen");
                    }
                }),
                //OnSideResidualOrder = 26,
                //OnSideResidualSubOrder = 2,
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 2,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "move: Light Screen");
                    }
                }),
            },
            [ConditionId.LockOn] = new()
            {
                Id = ConditionId.LockOn,
                Name = "Lock-On",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.LockOn,
                NoCopy = true, // doesn't get copied by Baton Pass
                Duration = 2,
                // If the move user is the Lock-On user and target is the locked Pokemon, 
                // bypass invulnerability (return 0 means not invulnerable)
                OnSourceInvulnerability = new OnSourceInvulnerabilityEventInfo((battle, target, source, move) =>
                {
                    // effectState.Target is the Lock-On user, effectState.Source is the locked Pokemon
                    if (move != null && 
                        source == battle.EffectState.Target && 
                        target == battle.EffectState.Source)
                    {
                        return 0;
                    }
                    return BoolIntEmptyVoidUnion.FromVoid();
                }, 1),
                // If the move user is the Lock-On user and target is the locked Pokemon,
                // bypass accuracy checks (return true)
                OnSourceAccuracy = new OnSourceAccuracyEventInfo((battle, _, target, source, move) =>
                {
                    // effectState.Target is the Lock-On user, effectState.Source is the locked Pokemon
                    if (move != null && 
                        source == battle.EffectState.Target && 
                        target == battle.EffectState.Source)
                    {
                        return true;
                    }
                    return IntBoolVoidUnion.FromVoid();
                }),
            },
            [ConditionId.LunarDance] = new()
            {
                Id = ConditionId.LunarDance,
                Name = "Lunar Dance",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.LunarDance,
                OnSwitchIn = new OnSwitchInEventInfo((battle, target) =>
                {
                    battle.SingleEvent(EventId.Swap, battle.Library.Conditions[ConditionId.LunarDance],
                        battle.EffectState, new PokemonSingleEventTarget(target));
                }),
                OnSwap = new OnSwapEventInfo((battle, target, _) =>
                {
                    if (!target.Fainted && (target.Hp < target.MaxHp || target.Status != ConditionId.None))
                    {
                        target.Heal(target.MaxHp);
                        target.CureStatus();
                        if (battle.DisplayUi)
                        {
                            battle.Add("-heal", target, target.GetHealth, "[from] move: Lunar Dance");
                        }
                        target.Side.RemoveSlotCondition(target, _library.Conditions[ConditionId.LunarDance]);
                    }
                }),
            },
            [ConditionId.LockedMove] = new()
            {
                Id = ConditionId.LockedMove,
                Name = "Locked Move",
                EffectType = EffectType.Condition,
                // Outrage, Thrash, Petal Dance - moves that lock the user for 2-3 turns
                Duration = 2,
                OnResidual = new OnResidualEventInfo((battle, target, _, _) =>
                {
                    if (target.Status == ConditionId.Sleep)
                    {
                        // Don't lock, and bypass confusion for calming
                        target.DeleteVolatile(ConditionId.LockedMove);
                    }
                    if (target.Volatiles.TryGetValue(ConditionId.LockedMove, out var state))
                    {
                        state.TrueDuration = (state.TrueDuration ?? 0) - 1;
                    }
                }),
                OnStart = new OnStartEventInfo((battle, target, source, effect) =>
                {
                    battle.EffectState.TrueDuration = battle.Random(2, 4);
                    battle.EffectState.Move = effect is Move move ? move.Id : null;
                    return BoolVoidUnion.FromVoid();
                }),
                OnRestart = new OnRestartEventInfo((battle, pokemon, _, _) =>
                {
                    if ((battle.EffectState.TrueDuration ?? 0) >= 2)
                    {
                        battle.EffectState.Duration = 2;
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnAfterMove = new OnAfterMoveEventInfo((battle, source, target, move) =>
                {
                    if ((battle.EffectState.Duration ?? 0) == 1)
                    {
                        source.RemoveVolatile(_library.Conditions[ConditionId.LockedMove]);
                    }
                }),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if ((battle.EffectState.TrueDuration ?? 0) > 1) return;
                    target.AddVolatile(ConditionId.Confusion);
                }),
                OnLockMove = new OnLockMoveEventInfo((Func<Battle, Pokemon, MoveIdVoidUnion>)((battle, pokemon) =>
                {
                    // TODO: Check for Dynamax volatile - if present, don't lock
                    if (pokemon.Volatiles.TryGetValue(ConditionId.LockedMove, out var state) && state.Move.HasValue)
                    {
                        return state.Move.Value;
                    }
                    return MoveIdVoidUnion.FromVoid();
                })),
            },
            [ConditionId.KingsShield] = new()
            {
                Id = ConditionId.KingsShield,
                Name = "King's Shield",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.KingsShield,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "Protect");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (!(move.Flags.Protect ?? false) || move.Category == MoveCategory.Status)
                    {
                        // G-Max moves not in Gen 9 VGC, omit check
                        // Z/Max moves not in Gen 9 VGC, omit zBrokeProtect
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }
                    if (move.SmartTarget ?? false)
                    {
                        move.SmartTarget = false;
                    }
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Protect");
                    }

                    // Check for lockedmove volatile and reset Outrage counter
                    if (source.Volatiles.TryGetValue(ConditionId.LockedMove, out var lockedMove))
                    {
                        if (lockedMove.Duration == 2)
                        {
                            source.DeleteVolatile(ConditionId.LockedMove);
                        }
                    }

                    // If move makes contact, lower attacker's Attack by 1
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        battle.Boost(new SparseBoostsTable { Atk = -1 }, source, target, 
                            _library.Conditions[ConditionId.KingsShield]);
                    }

                    return new Empty(); // NOT_FAIL equivalent
                }, 3),
                // Note: Z/Max move OnHit handler omitted as not applicable to Gen 9 VGC
            },
            [ConditionId.MaxGuard] = new()
            {
                Id = ConditionId.MaxGuard,
                Name = "Max Guard",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MaxGuard,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "Max Guard");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                    OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                    {
                        // TODO: Check for moves that bypass Max Guard
                        // bypassesMaxGuard = ['acupressure', 'afteryou', 'allyswitch', 'aromatherapy', ...many more]
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Max Guard");
                        }
                        // TODO: Check for lockedmove volatile and reset Outrage counter
                        return BoolIntEmptyVoidUnion.FromBool(false);
                    }, 3),
                },
                [ConditionId.Metronome] = new()
            {
                Id = ConditionId.Metronome,
                Name = "Metronome",
                EffectType = EffectType.Condition,
                // Marker condition for Metronome item boost tracking
                // Actual logic is in the item handler
            },
            [ConditionId.MicleBerry] = new()
            {
                Id = ConditionId.MicleBerry,
                Name = "Micle Berry",
                EffectType = EffectType.Condition,
                Duration = 2,
                OnSourceAccuracy = new OnSourceAccuracyEventInfo((battle, accuracy, _, source, move) =>
                {
                    // Don't modify accuracy for OHKO moves
                    if (move.Ohko is not null)
                    {
                        return IntBoolVoidUnion.FromVoid();
                    }

                    // Announce the berry was consumed
                    if (battle.DisplayUi)
                    {
                        battle.Add("-enditem", source, "Micle Berry");
                    }

                    // Remove the volatile from the source (the attacker)
                    source.RemoveVolatile(_library.Conditions[ConditionId.MicleBerry]);

                    // Multiply accuracy by 1.2 (4915/4096)
                    battle.ChainModify([4915, 4096]);
                    return battle.FinalModify(accuracy);
                }),
            },
            [ConditionId.Obstruct] = new()
            {
                Id = ConditionId.Obstruct,
                Name = "Obstruct",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Obstruct,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "Protect");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (!(move.Flags.Protect ?? false) || move.Category == MoveCategory.Status)
                    {
                        // G-Max moves not in Gen 9 VGC, omit check
                        // Z/Max moves not in Gen 9 VGC, omit zBrokeProtect
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }
                    if (move.SmartTarget ?? false)
                    {
                        move.SmartTarget = false;
                    }
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Protect");
                    }

                    // Check for lockedmove volatile and reset Outrage counter
                    if (source.Volatiles.TryGetValue(ConditionId.LockedMove, out var lockedMove))
                    {
                        if (lockedMove.Duration == 2)
                        {
                            source.DeleteVolatile(ConditionId.LockedMove);
                        }
                    }

                    // If move makes contact, lower attacker's Defense by 2
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        battle.Boost(new SparseBoostsTable { Def = -2 }, source, target, 
                            _library.Conditions[ConditionId.Obstruct]);
                    }

                    return new Empty(); // NOT_FAIL equivalent
                }, 3),
                // Note: Z/Max move OnHit handler omitted as not applicable to Gen 9 VGC
            },
            [ConditionId.Octolock] = new()
            {
                Id = ConditionId.Octolock,
                Name = "Octolock",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Octolock,
                // TODO: OnTryImmunity - check trap immunity
                OnStart = new OnStartEventInfo((battle, pokemon, source, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "move: Octolock", $"[of] {source}");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    var source = battle.EffectState.Source;
                    if (source == null || !source.IsActive || source.Hp <= 0 || source.ActiveTurns <= 0)
                    {
                        pokemon.DeleteVolatile(ConditionId.Octolock);
                        if (battle.DisplayUi)
                        {
                            battle.Add("-end", pokemon, "Octolock", "[partiallytrapped]", "[silent]");
                        }
                        return;
                    }
                        battle.Boost(new SparseBoostsTable
                        {
                            Def = -1,
                            SpD = -1
                        }, pokemon, source);
                    }, 14),
                OnTrapPokemon = new OnTrapPokemonEventInfo((battle, pokemon) =>
                {
                    if (battle.EffectState.Source?.IsActive ?? false)
                    {
                        pokemon.TryTrap();
                    }
                }),
            },
        };
    }
}
