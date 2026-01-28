using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;
using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsMno()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.MagicRoom] = new()
            {
                Id = ConditionId.MagicRoom,
                Name = "Magic Room",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MagicRoom,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, _, _, _) => 5),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldstart", "move: Magic Room", $"[of] {source}");
                    }

                    // Trigger item End events for all active Pokemon
                    foreach (Pokemon pokemon in battle.GetAllActive())
                    {
                        battle.SingleEvent(EventId.End, pokemon.GetItem(), pokemon.ItemState,
                            pokemon);
                    }
                }),
                OnFieldRestart = new OnFieldRestartEventInfo((battle, _, _, _) =>
                {
                    // Using Magic Room again removes it
                    battle.Field.RemovePseudoWeather(ConditionId.MagicRoom);
                }),
                OnFieldResidual = new OnFieldResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 27,
                    SubOrder = 6,
                },
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Magic Room",
                            "[of] " + battle.EffectState.Source);
                    }
                }),
            },
            [ConditionId.MagmaStorm] = new()
            {
                Id = ConditionId.MagmaStorm,
                Name = "Magma Storm",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MagmaStorm,
                // Uses PartiallyTrapped condition with MagmaStorm as source
            },
            [ConditionId.MagnetRise] = new()
            {
                Id = ConditionId.MagnetRise,
                Name = "Magnet Rise",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MagnetRise,
                Duration = 5,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "Magnet Rise");
                    }

                    return new VoidReturn();
                }),
                OnImmunity = new OnImmunityEventInfo((_, type, _) =>
                {
                    // Grant immunity to Ground-type moves
                    if (type is { IsPokemonType: true, AsPokemonType: PokemonType.Ground })
                    {
                        return BoolVoidUnion.FromBool(false);
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 18,
                },
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "Magnet Rise");
                    }
                }),
            },
            [ConditionId.MeanLook] = new()
            {
                Id = ConditionId.MeanLook,
                Name = "Mean Look",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MeanLook,
                // Uses Trapped condition
            },
            [ConditionId.Metronome] = new()
            {
                Id = ConditionId.Metronome,
                Name = "Metronome",
                EffectType = EffectType.Condition,
                AssociatedItem = ItemId.Metronome,
                OnStart = new OnStartEventInfo((_, pokemon, _, _) =>
                {
                    if (pokemon.Volatiles.TryGetValue(ConditionId.Metronome,
                            out EffectState? state))
                    {
                        state.LastMove = "";
                        state.NumConsecutive = 0;
                    }

                    return new VoidReturn();
                }),
                OnTryMove = new OnTryMoveEventInfo((_, source, _, move) =>
                {
                    if (!source.Volatiles.TryGetValue(ConditionId.Metronome,
                            out EffectState? effectState))
                        return BoolEmptyVoidUnion.FromVoid();

                    // Remove volatile if no longer holding the item
                    // TS: return; (undefined - ignore handler result)
                    if (!source.HasItem(ItemId.Metronome))
                    {
                        source.RemoveVolatile(_library.Conditions[ConditionId.Metronome]);
                        return BoolEmptyVoidUnion.FromVoid();
                    }

                    // Don't track moves that call other moves
                    // TS: return; (undefined - ignore handler result)
                    if (move.CallsMove == true) return BoolEmptyVoidUnion.FromVoid();

                    // Track consecutive move usage
                    bool lastMoveMatches = effectState.LastMove == move.Id.ToString();
                    bool moveSucceededLastTurn = source.MoveLastTurnResult?.IsTruthy() == true;

                    if (lastMoveMatches && moveSucceededLastTurn)
                    {
                        effectState.NumConsecutive = (effectState.NumConsecutive ?? 0) + 1;
                    }
                    else if (source.Volatiles.ContainsKey(ConditionId.TwoTurnMove))
                    {
                        if (!lastMoveMatches)
                        {
                            effectState.NumConsecutive = 1;
                        }
                        else
                        {
                            effectState.NumConsecutive = (effectState.NumConsecutive ?? 0) + 1;
                        }
                    }
                    else
                    {
                        effectState.NumConsecutive = 0;
                    }

                    effectState.LastMove = move.Id.ToString();
                    // TS: implicit return undefined
                    return BoolEmptyVoidUnion.FromVoid();
                }, -2),
                OnModifyDamage = new OnModifyDamageEventInfo((battle, _, source, _, _) =>
                {
                    if (!source.Volatiles.TryGetValue(ConditionId.Metronome,
                            out EffectState? effectState))
                        return DoubleVoidUnion.FromVoid();

                    int[] dmgMod = [4096, 4915, 5734, 6553, 7372, 8192];
                    int numConsecutive = Math.Min(effectState.NumConsecutive ?? 0, 5);
                    return battle.ChainModify([dmgMod[numConsecutive], 4096]);
                }),
            },
            [ConditionId.MicleBerry] = new()
            {
                Id = ConditionId.MicleBerry,
                Name = "Micle Berry",
                EffectType = EffectType.Condition,
                AssociatedItem = ItemId.MicleBerry,
                Duration = 2,
                OnSourceAccuracy =
                    new OnSourceAccuracyEventInfo((battle, accuracy, _, source, move) =>
                    {
                        // OHKO moves bypass Micle Berry boost entirely
                        if (move.Ohko != null) return null;

                        // For non-OHKO moves, always consume the berry and show message
                        if (battle.DisplayUi)
                        {
                            battle.Add("-enditem", source, "Micle Berry");
                        }

                        source.RemoveVolatile(_library.Conditions[ConditionId.MicleBerry]);

                        // Only boost if accuracy is numeric (always-hit moves pass through unchanged)
                        if (!accuracy.HasValue) return null;

                        // Boost accuracy by 1.2x (4915/4096 ≈ 1.2)
                        int modifiedAccuracy = (int)Math.Floor(accuracy.Value * 4915.0 / 4096.0);
                        return IntBoolVoidUnion.FromInt(modifiedAccuracy);
                    }),
            },
            [ConditionId.Mimic] = new()
            {
                Id = ConditionId.Mimic,
                Name = "Mimic",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Mimic,
                // Mimic doesn't create a persistent condition - it modifies move slots directly
            },
            [ConditionId.Minimize] = new()
            {
                Id = ConditionId.Minimize,
                Name = "Minimize",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Minimize,
                NoCopy = true,
                OnRestart = new OnRestartEventInfo((_, _, _, _) => null),
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, _, _, _, move) =>
                {
                    // Moves that deal double damage to Minimized targets
                    // Note: Steamroller and MaliciousMoonsault are not in Gen 9 VGC
                    MoveId[] stompingMoves =
                    [
                        MoveId.Stomp, MoveId.BodySlam, MoveId.FlyingPress,
                        MoveId.DragonRush, MoveId.HeatCrash, MoveId.HeavySlam,
                        MoveId.SupercellSlam,
                    ];
                    if (stompingMoves.Contains(move.Id))
                    {
                        return battle.ChainModify(2);
                    }

                    return DoubleVoidUnion.FromVoid();
                }),
                OnAccuracy = new OnAccuracyEventInfo((_, accuracy, _, _, move) =>
                {
                    // Moves that bypass accuracy check against Minimized targets
                    // Note: Steamroller and MaliciousMoonsault are not in Gen 9 VGC
                    MoveId[] stompingMoves =
                    [
                        MoveId.Stomp, MoveId.BodySlam, MoveId.FlyingPress,
                        MoveId.DragonRush, MoveId.HeatCrash, MoveId.HeavySlam,
                        MoveId.SupercellSlam,
                    ];
                    if (stompingMoves.Contains(move.Id))
                    {
                        return IntBoolVoidUnion.FromBool(true); // Always hit
                    }

                    // Pass through accuracy unchanged
                    // TS: return accuracy; (which is number | true)
                    return accuracy.HasValue 
                        ? IntBoolVoidUnion.FromInt(accuracy.Value) 
                        : IntBoolVoidUnion.FromBool(true); // Always-hit moves have accuracy = true
                }),
            },
            [ConditionId.MirrorCoat] = new()
            {
                Id = ConditionId.MirrorCoat,
                Name = "Mirror Coat",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MirrorCoat,
                Duration = 1,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, _, _, _) =>
                {
                    battle.EffectState.Slot = null;
                    battle.EffectState.TotalDamage = 0;
                    return BoolVoidUnion.FromVoid();
                }),
                OnRedirectTarget = new OnRedirectTargetEventInfo(
                    (battle, target, source, _, move) =>
                    {
                        if (move.Id != MoveId.MirrorCoat) return PokemonVoidUnion.FromVoid();

                        EffectState? effectState =
                            source.Volatiles.GetValueOrDefault(ConditionId.MirrorCoat);
                        if (source != target || effectState?.Slot == null)
                        {
                            return PokemonVoidUnion.FromVoid();
                        }

                        Pokemon? redirectTarget = battle.GetAtSlot(effectState.Slot);
                        if (redirectTarget != null)
                        {
                            return redirectTarget;
                        }

                        return PokemonVoidUnion.FromVoid();
                    }, -1),
                OnDamagingHit = new OnDamagingHitEventInfo((battle, damage, target, source, move) =>
                {
                    if (source.IsAlly(target)) return;
                    if (battle.GetCategory(move) != MoveCategory.Special) return;

                    if (!target.Volatiles.TryGetValue(ConditionId.MirrorCoat,
                            out EffectState? effectState)) return;

                    effectState.Slot = source.GetSlot();
                    effectState.TotalDamage = 2 * damage;
                }),
            },
            [ConditionId.Mist] = new()
            {
                Id = ConditionId.Mist,
                Name = "Mist",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Mist,
                Duration = 5,
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, source, effect) =>
                {
                    // Allow infiltrating moves to bypass Mist
                    if (effect is ActiveMove move && (move.Infiltrates ?? false) &&
                        !target.IsAlly(source))
                    {
                        return;
                    }

                    // Only block stat drops from opponents
                    if (source != null && target != source)
                    {
                        bool showMsg = false;
                        foreach ((BoostId boostId, int value) in boost.GetNonNullBoosts().ToList())
                        {
                            if (value < 0)
                            {
                                boost.ClearBoost(boostId);
                                showMsg = true;
                            }
                        }

                        // Show message if stat drops were blocked and move has no secondaries
                        if (showMsg && effect is ActiveMove { Secondaries: null })
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Add("-activate", target, "move: Mist");
                            }
                        }
                    }
                }),
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "Mist");
                    }
                }),
                //OnSideResidualOrder = 26,
                //OnSideResidualSubOrder = 4,
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 4,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "Mist");
                    }
                }),
            },
            [ConditionId.MistyTerrain] = new()
            {
                Id = ConditionId.MistyTerrain,
                Name = "Misty Terrain",
                AssociatedMove = MoveId.MistyTerrain,
                EffectType = EffectType.Terrain,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                    source.HasItem(ItemId.TerrainExtender) ? 8 : 5),
                // Block status conditions for grounded Pokemon
                // TS: if (!target.isGrounded() || target.isSemiInvulnerable()) return; (allows status)
                //     return false; (blocks status)
                OnSetStatus = new OnSetStatusEventInfo((battle, _, target, _, effect) =>
                {
                    // Allow status if target is NOT grounded OR IS semi-invulnerable
                    if (!(target.IsGrounded() ?? false) || target.IsSemiInvulnerable())
                    {
                        return new VoidReturn();
                    }

                    // Show message if this is from a move's status effect or Yawn
                    if (effect is ActiveMove move &&
                        (move.Status != null || move.Id == MoveId.Yawn))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Misty Terrain");
                        }
                    }
                    else if (effect is Condition { Id: ConditionId.Yawn })
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Misty Terrain");
                        }
                    }

                    // Block the status
                    return false;
                }),
                // Block confusion for grounded Pokemon
                OnTryAddVolatile =
                    new OnTryAddVolatileEventInfo((battle, status, target, _, effect) =>
                    {
                        // Allow volatile if target is NOT grounded OR IS semi-invulnerable
                        if (!(target.IsGrounded() ?? false) || target.IsSemiInvulnerable())
                        {
                            return new VoidReturn();
                        }

                        // Block confusion
                        if (status.Id == ConditionId.Confusion)
                        {
                            // Show message if from a move without secondaries
                            if (effect is ActiveMove { Secondaries: null })
                            {
                                if (battle.DisplayUi)
                                {
                                    battle.Add("-activate", target, "move: Misty Terrain");
                                }
                            }

                            // Return null to block the volatile
                            return null;
                        }

                        // Allow other volatiles
                        return new VoidReturn();
                    }),

                //OnBasePowerPriority = 6,
                OnBasePower = new OnBasePowerEventInfo((battle, _, _, defender, move) =>
                    {
                        if (move.Type == MoveType.Dragon &&
                            (defender.IsGrounded() ?? false) &&
                            !defender.IsSemiInvulnerable())
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Debug("misty terrain weaken");
                            }

                            return battle.ChainModify([2048, 4096]);
                        }

                        return new VoidReturn();
                    },
                    6),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (!battle.DisplayUi) return;
                    if (effect is Ability)
                    {
                        battle.Add("-fieldstart", "move: Misty Terrain", "[from] ability: " +
                                                                         effect.Name, $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-fieldstart", "move: Misty Terrain");
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
                        battle.Add("-fieldend", "move: Misty Terrain");
                    }
                }),
            },
            [ConditionId.MustRecharge] = new()
            {
                Id = ConditionId.MustRecharge,
                Name = "MustRecharge",
                EffectType = EffectType.Condition,
                Duration = 2,
                //OnBeforeMovePriority = 11,
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "recharge");
                        }

                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.MustRecharge]);
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Truant]);
                        return null;
                    },
                    11),
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-mustrecharge", pokemon);
                    }

                    return new VoidReturn();
                }),
                OnLockMove = new OnLockMoveEventInfo(MoveId.Recharge),
            },
            [ConditionId.NoRetreat] = new()
            {
                Id = ConditionId.NoRetreat,
                Name = "No Retreat",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.NoRetreat,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "move: No Retreat");
                    }

                    return new VoidReturn();
                }),
                OnTrapPokemon = new OnTrapPokemonEventInfo((_, pokemon) => { pokemon.TryTrap(); }),
            },
            [ConditionId.None] = new()
            {
                Id = ConditionId.None,
                Name = "None",
                EffectType = EffectType.Condition,
            },
        };
    }
}