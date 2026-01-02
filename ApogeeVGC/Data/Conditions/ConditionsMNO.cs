using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;
using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

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
                DurationCallback = new DurationCallbackEventInfo((_, _, source, _) =>
                {
                    // TODO: Check for Persistent ability
                    return 5;
                }),
                // TODO: onFieldStart - trigger item End events for all active pokemon
                // TODO: onFieldRestart - remove Magic Room if used again
                // TODO: onFieldResidualOrder = 27
                // TODO: onFieldResidualSubOrder = 6
                // TODO: onFieldEnd - display end message
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
                // TODO: onImmunity - if type is Ground, return false
                // TODO: onResidualOrder = 18
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
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.Volatiles.TryGetValue(ConditionId.Metronome, out var state))
                    {
                        state.LastMove = "";
                        state.NumConsecutive = 0;
                    }
                    return new VoidReturn();
                }),
                OnTryMove = new OnTryMoveEventInfo((battle, source, target, move) =>
                {
                    if (!source.Volatiles.TryGetValue(ConditionId.Metronome, out var effectState))
                        return null;

                    // Remove volatile if no longer holding the item
                    if (!source.HasItem(ItemId.Metronome))
                    {
                        source.RemoveVolatile(_library.Conditions[ConditionId.Metronome]);
                        return null;
                    }
                    // Don't track moves that call other moves
                    if (move.CallsMove == true) return null;

                    // Track consecutive move usage
                    var lastMoveMatches = effectState.LastMove == move.Id.ToString();
                    var moveSucceededLastTurn = source.MoveLastTurnResult?.IsTruthy() == true;

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
                    return null;
                }, -2),
                OnModifyDamage = new OnModifyDamageEventInfo((battle, damage, source, target, move) =>
                {
                    if (!source.Volatiles.TryGetValue(ConditionId.Metronome, out var effectState))
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
                OnSourceAccuracy = new OnSourceAccuracyEventInfo((battle, accuracy, target, source, move) =>
                {
                    // OHKO moves bypass Micle Berry boost
                    if (move.Ohko != null) return null;

                    if (battle.DisplayUi)
                    {
                        battle.Add("-enditem", source, "Micle Berry");
                    }
                    source.RemoveVolatile(_library.Conditions[ConditionId.MicleBerry]);

                    // Boost accuracy by 1.2x (4915/4096 ≈ 1.2)
                    int modifiedAccuracy = (int)Math.Floor(accuracy * 4915.0 / 4096.0);
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
                // TODO: onRestart - return null
                // TODO: onSourceModifyDamage - double damage from stomping moves
                // TODO: onAccuracy - guarantee hit from stomping moves
            },
            [ConditionId.MirrorCoat] = new()
            {
                Id = ConditionId.MirrorCoat,
                Name = "Mirror Coat",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.MirrorCoat,
                Duration = 1,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    battle.EffectState.Slot = null;
                    battle.EffectState.TotalDamage = 0;
                    return BoolVoidUnion.FromVoid();
                }),
                OnRedirectTarget = new OnRedirectTargetEventInfo((battle, target, source, _, move) =>
                {
                    if (move.Id != MoveId.MirrorCoat) return PokemonVoidUnion.FromVoid();

                    var effectState = source.Volatiles.TryGetValue(ConditionId.MirrorCoat, out var state) ? state : null;
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

                    if (!target.Volatiles.TryGetValue(ConditionId.MirrorCoat, out var effectState)) return;

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
                // TODO: onTryBoost - prevent negative stat changes from opponents
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
                OnSetStatus = new OnSetStatusEventInfo((battle, status, target, _, _) =>
                {
                    if ((target.IsGrounded() ?? false) && !target.IsSemiInvulnerable())
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Misty Terrain");
                        }
                        return false;
                    }
                    return new VoidReturn();
                }),
                //OnBasePowerPriority = 6,
                OnBasePower = new OnBasePowerEventInfo((battle, _, attacker, defender, move) =>
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
                //OnFieldResidualOrder = 27,
                //OnFieldResidualSubOrder = 7,
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
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
            [ConditionId.Nightmare] = new()
            {
                Id = ConditionId.Nightmare,
                Name = "Nightmare",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Nightmare,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.Status != ConditionId.Sleep && !pokemon.HasAbility(AbilityId.Comatose))
                    {
                        return BoolVoidUnion.FromBool(false);
                    }
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Nightmare");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Damage(pokemon.BaseMaxHp / 4);
                }, 11),
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
                // TODO: onTrapPokemon - trap the pokemon
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
