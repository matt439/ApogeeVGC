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
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsJkl()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.IceBall] = new()
            {
                Id = ConditionId.IceBall,
                Name = "Ice Ball",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.IceBall,
                // Ice Ball uses LockedMove for the locking behavior and RolloutStorage for damage scaling
                // This is just a marker condition
            },
            [ConditionId.LeechSeed] = new()
            {
                Id = ConditionId.LeechSeed,
                Name = "Leech Seed",
                EffectType = EffectType.Condition,
                ImmuneTypes = [PokemonType.Grass],
                AssociatedMove = MoveId.LeechSeed,
                OnStart = OnStartEventInfo.Create((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "move: Leech Seed");
                    }

                    return new VoidReturn();
                }),
                //OnResidualOrder = 8,
                OnResidual = OnResidualEventInfo.Create((battle, pokemon, _, _) =>
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

                        if (damage is IntIntFalseUndefined { Value: > 0 } d)
                        {
                            // Pass LeechSeed condition as effect so LiquidOoze can detect it
                            var leechSeedCondition = battle.Library.Conditions[ConditionId.LeechSeed];
                            battle.Heal(d.Value, target, pokemon,
                                BattleHealEffect.FromIEffect(leechSeedCondition));
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
                DurationCallback = DurationCallbackEventInfo.Create((_, _, source, _) =>
                    source?.HasItem(ItemId.LightClay) == true ? 8 : 5),
                OnAnyModifyDamage =
                    OnAnyModifyDamageEventInfo.Create((battle, damage, source, target, move) =>
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

                                if (battle.ActivePerHalf > 1)
                                {
                                    battle.ChainModify([2732, 4096]);
                                }
                                else
                                {
                                    battle.ChainModify(0.5);
                                }

                                return battle.FinalModify(damage);
                            }
                        }

                        return new VoidReturn();
                    }),
                OnSideStart = OnSideStartEventInfo.Create((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "move: Light Screen");
                    }
                }),
                //OnSideResidualOrder = 26,
                //OnSideResidualSubOrder = 2,
                OnSideResidual = OnSideResidualEventInfo.Create((_, _, _, _) => { }) with
                {
                    Order = 26,
                    SubOrder = 2,
                },
                OnSideEnd = OnSideEndEventInfo.Create((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "move: Light Screen");
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
                OnResidual = OnResidualEventInfo.Create((battle, target, _, _) =>
                {
                    if (target.Status == ConditionId.Sleep)
                    {
                        // Don't lock, and bypass confusion for calming
                        target.DeleteVolatile(ConditionId.LockedMove);
                    }

                    // Decrement trueDuration unconditionally (matches TS: this.effectState.trueDuration--)
                    battle.EffectState.TrueDuration = (battle.EffectState.TrueDuration ?? 0) - 1;
                }),
                OnStart = OnStartEventInfo.Create((battle, _, _, effect) =>
                {
                    battle.EffectState.TrueDuration = battle.Random(2, 4);
                    battle.EffectState.Move = effect is Move move ? move.Id : null;
                    return new VoidReturn();
                }),
                OnRestart = OnRestartEventInfo.Create((battle, _, _, _) =>
                {
                    if ((battle.EffectState.TrueDuration ?? 0) >= 2)
                    {
                        battle.EffectState.Duration = 2;
                    }

                    return null;
                }),
                OnAfterMove = OnAfterMoveEventInfo.Create((battle, source, _, _) =>
                {
                    if ((battle.EffectState.Duration ?? 0) == 1)
                    {
                        source.RemoveVolatile(_library.Conditions[ConditionId.LockedMove]);
                    }
                }),
                OnEnd = OnEndEventInfo.Create((battle, target) =>
                {
                    if ((battle.EffectState.TrueDuration ?? 0) > 1) return;
                    target.AddVolatile(ConditionId.Confusion);
                }),
                OnLockMove = OnLockMoveEventInfo.Create(
                    (Func<Battle, Pokemon, MoveIdVoidUnion>)((battle, _) =>
                    {
                        // TS: return this.effectState.move
                        if (battle.EffectState.Move.HasValue)
                        {
                            return battle.EffectState.Move.Value;
                        }

                        return MoveIdVoidUnion.FromVoid();
                    })),
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
                OnSourceInvulnerability = OnSourceInvulnerabilityEventInfo.Create(
                    (battle, target, source, move) =>
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
                OnSourceAccuracy =
                    OnSourceAccuracyEventInfo.Create((battle, _, target, source, move) =>
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
                OnSwitchIn = OnSwitchInEventInfo.Create((battle, target) =>
                {
                    battle.SingleEvent(EventId.Swap,
                        battle.Library.Conditions[ConditionId.LunarDance],
                        battle.EffectState, new PokemonSingleEventTarget(target));
                }),
                OnSwap = OnSwapEventInfo.Create((battle, target, _) =>
                {
                    // Check if target needs healing, status cure, or PP restoration
                    bool needsPpRestore = target.MoveSlots.Any(ms => ms.Pp < ms.MaxPp);
                    if (!target.Fainted &&
                        (target.Hp < target.MaxHp || target.Status != ConditionId.None ||
                         needsPpRestore))
                    {
                        target.Heal(target.MaxHp);
                        target.ClearStatus();
                        // Restore PP for all moves
                        foreach (MoveSlot moveSlot in target.MoveSlots)
                        {
                            moveSlot.Pp = moveSlot.MaxPp;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Add("-heal", target, target.GetHealth,
                                "[from] move: Lunar Dance");
                        }

                        target.Side.RemoveSlotCondition(target,
                            _library.Conditions[ConditionId.LunarDance]);
                    }
                }),
            },
        };
    }
}