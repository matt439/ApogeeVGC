using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;
using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsVwx()
    {
        return new Dictionary<ConditionId, Condition>
        {
            // ===== V CONDITIONS =====
            // No conditions needed for V moves

            // ===== W CONDITIONS =====

            [ConditionId.WaterPledge] = new()
            {
                Id = ConditionId.WaterPledge,
                Name = "Water Pledge",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.WaterPledge,
                Duration = 4,
                OnSideStart = new OnSideStartEventInfo((battle, targetSide, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", targetSide, "Water Pledge");
                    }
                }),
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 7,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, targetSide) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", targetSide, "Water Pledge");
                    }
                }),
                // Double secondary effect chances for moves (except Secret Power)
                // Skip Serene Grace + Flinch interaction
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    if (move.Secondaries != null && move.Id != MoveId.SecretPower)
                    {
                        battle.Debug("doubling secondary chance");
                        foreach (var secondary in move.Secondaries)
                        {
                            // Skip Serene Grace + Flinch interaction
                            if (pokemon.HasAbility(AbilityId.SereneGrace) &&
                                secondary.VolatileStatus == ConditionId.Flinch)
                            {
                                continue;
                            }

                            if (secondary.Chance.HasValue)
                            {
                                secondary.Chance *= 2;
                            }
                        }

                        // Also double Self chance if present
                        if (move.Self?.Chance != null)
                        {
                            move.Self.Chance *= 2;
                        }
                    }
                }),
            },
            [ConditionId.WideGuard] = new()
            {
                Id = ConditionId.WideGuard,
                Name = "Wide Guard",
                EffectType = EffectType.Condition,
                Duration = 1,
                AssociatedMove = MoveId.WideGuard,
                OnSideStart = new OnSideStartEventInfo((battle, _, source, _) =>
                {
                    if (battle.DisplayUi && source != null)
                    {
                        battle.Add("-singleturn", source, "Wide Guard");
                    }
                }),
                // OnTryHitPriority = 4
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    // Wide Guard blocks all spread moves (allAdjacent, allAdjacentFoes)
                    if (move.Target != MoveTarget.AllAdjacent &&
                        move.Target != MoveTarget.AllAdjacentFoes)
                    {
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    // Check if move has protect flag
                    if (!(move.Flags.Protect ?? false))
                    {
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Wide Guard");
                    }

                    // Reset Outrage counter if source has lockedmove volatile
                    if (source.Volatiles.TryGetValue(ConditionId.LockedMove,
                            out var lockedMoveState))
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
                    SubOrder = 3,
                },
                OnSideEnd = new OnSideEndEventInfo((_, _) =>
                {
                    // Silent end - Wide Guard doesn't announce when it ends
                }),
            },
            [ConditionId.WonderRoom] = new()
            {
                Id = ConditionId.WonderRoom,
                Name = "Wonder Room",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.WonderRoom,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((battle, _, source, _) =>
                {
                    if (source != null && source.HasAbility(AbilityId.Persistent))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", source, "ability: Persistent",
                                "[move] Wonder Room");
                        }

                        return 7;
                    }

                    return 5;
                }),
                // Wonder Room's defensive stat swapping is implemented in Pokemon.CalculateStat():
                // When Wonder Room is active, Defense and Special Defense stat VALUES are swapped
                // BEFORE any calculations. This affects all damage calculations.
                // 
                // Note: The TypeScript version also has an OnModifyMove handler that swaps
                // move.overrideOffensiveStat for moves like Body Press. In C#, since Move properties
                // are init-only, this swap would need to be handled in the damage calculation code
                // where OverrideOffensiveStat is used. The stat swapping in CalculateStat handles
                // most cases correctly.
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        if (source != null && source.HasAbility(AbilityId.Persistent))
                        {
                            battle.Add("-fieldstart", "move: Wonder Room", $"[of] {source}",
                                "[persistent]");
                        }
                        else
                        {
                            battle.Add("-fieldstart", "move: Wonder Room", $"[of] {source}");
                        }
                    }
                }),
                OnFieldRestart = new OnFieldRestartEventInfo((battle, _, _, _) =>
                {
                    battle.Field.RemovePseudoWeather(ConditionId.WonderRoom);
                }),
                // Swapping defenses partially implemented in sim/pokemon.js:Pokemon#calculateStat and Pokemon#getStat
                OnFieldResidual = new OnFieldResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 27,
                    SubOrder = 5,
                },
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Wonder Room");
                    }
                }),
            },

            // ===== X CONDITIONS =====
            // No conditions needed for X moves
        };
    }
}