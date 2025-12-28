using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
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
                // TODO: OnModifyCritRatio - guarantee critical hit for next move
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
                // TODO: onSourceInvulnerabilityPriority = 1
                // TODO: onSourceInvulnerability - if move is from target to source, return 0
                // TODO: onSourceAccuracy - if move is from target to source, return true
            },
            [ConditionId.LunarDance] = new()
            {
                Id = ConditionId.LunarDance,
                Name = "Lunar Dance",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.LunarDance,
                // TODO: onSwitchIn - trigger Swap event
                // TODO: onSwap - heal HP to max, cure status, restore PP if needed
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
                        // TODO: Check for gmaxoneblow, gmaxrapidflow
                        // TODO: Check if move.isZ or move.isMax and set zBrokeProtect
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }
                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Protect");
                    }
                    // TODO: Check for lockedmove volatile and reset Outrage counter
                    // TODO: Check if move makes contact and lower Attack
                    // if (this.checkMoveMakesContact(move, source, target)) {
                    //     this.boost({ atk: -1 }, source, target, this.dex.getActiveMove("King's Shield"));
                    // }
                    return BoolIntEmptyVoidUnion.FromBool(false);
                }, 3),
                // TODO: OnHit - if move is Z or Max powered and makes contact, lower Attack
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
                // TODO: OnSourceAccuracy - if not OHKO move, multiply accuracy by 1.2 (4915/4096)
                // Remove volatile after modifying accuracy
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
                        // TODO: Check for gmaxoneblow, gmaxrapidflow
                        // TODO: Check if move.isZ or move.isMax and set zBrokeProtect
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }
                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Protect");
                    }
                    // TODO: Check for lockedmove volatile and reset Outrage counter
                    // TODO: Check if move makes contact and lower Defense by 2
                    // if (this.checkMoveMakesContact(move, source, target)) {
                    //     this.boost({ def: -2 }, source, target, this.dex.getActiveMove("Obstruct"));
                    // }
                    return BoolIntEmptyVoidUnion.FromBool(false);
                }, 3),
                // TODO: OnHit - if move is Z or Max powered and makes contact, lower Defense by 2
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
