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
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsDef()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.ElectricTerrain] = new()
            {
                Id = ConditionId.ElectricTerrain,
                Name = "Electric Terrain",
                EffectType = EffectType.Terrain,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                    source.HasItem(ItemId.TerrainExtender) ? 8 : 5),
                OnSetStatus = new OnSetStatusEventInfo((battle, status, target, _, effect) =>
                {
                    if (status.Id == ConditionId.Sleep &&
                        (target.IsGrounded() ?? false) &&
                        !target.IsSemiInvulnerable())
                    {
                        if (battle.DisplayUi && effect is Condition { Id: ConditionId.Yawn } or
                                Move { Id: MoveId.Yawn } or
                                ActiveMove { Secondaries: not null })
                        {
                            battle.Add("-activate", target, "move: Electric Terrain");
                        }

                        return false;
                    }

                    return new VoidReturn();
                }),
                OnTryAddVolatile = new OnTryAddVolatileEventInfo((battle, status, target, _, _) =>
                {
                    if (!(target.IsGrounded() ?? false) || target.IsSemiInvulnerable())
                        return new VoidReturn();
                    if (status.Id == ConditionId.Yawn)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Electric Terrain");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }),
                //OnBasePowerPriority = 6,
                OnBasePower = new OnBasePowerEventInfo((battle, _, attacker, _, move) =>
                    {
                        if (move.Type == MoveType.Electric &&
                            (attacker.IsGrounded() ?? false) &&
                            !attacker.IsSemiInvulnerable())
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Debug("electric terrain boost");
                            }

                            return battle.ChainModify([5325, 4096]);
                        }

                        return new VoidReturn();
                    },
                    6),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (!battle.DisplayUi) return;

                    if (effect is Ability)
                    {
                        battle.Add("-fieldstart", "move: Electric Terrain", "[from] ability: " +
                            effect.Name, $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-fieldstart", "move: Electric Terrain");
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
                        battle.Add("-fieldend", "move: Electric Terrain");
                    }
                }),
            },
            [ConditionId.DefenseCurl] = new()
            {
                Id = ConditionId.DefenseCurl,
                Name = "Defense Curl",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.DefenseCurl,
                NoCopy = true,
                // This is a marker condition used by Rollout and Ice Ball to double their damage
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "move: Defense Curl");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
            },
            [ConditionId.Detect] = new()
            {
                Id = ConditionId.Detect,
                Name = "Detect",
                Duration = 1,
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Detect,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "Protect");
                    }
                    return new VoidReturn();
                }),
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
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

                    return new Empty();
                }, 3),
            },
            [ConditionId.DestinyBond] = new()
            {
                Id = ConditionId.DestinyBond,
                Name = "Destiny Bond",
                EffectType = EffectType.Condition,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singlemove", pokemon, "Destiny Bond");
                    }
                    return new VoidReturn();
                }),
                OnFaint = new OnFaintEventInfo((battle, target, source, effect) =>
                {
                    if (source == null || effect == null || target.IsAlly(source)) return;
                    if (effect.EffectType == EffectType.Move && !(effect is Move { Flags.FutureMove: true }))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Destiny Bond");
                        }
                        source.Faint();
                    }
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, move) =>
                {
                    if (move.Id == MoveId.DestinyBond) return new VoidReturn();
                    if (battle.DisplayUi)
                    {
                        battle.Debug("removing Destiny Bond before attack");
                    }
                    pokemon.RemoveVolatile(_library.Conditions[ConditionId.DestinyBond]);
                    return new VoidReturn();
                }, -1),
                OnMoveAborted = new OnMoveAbortedEventInfo((_, pokemon, _, _) =>
                {
                    pokemon.RemoveVolatile(_library.Conditions[ConditionId.DestinyBond]);
                }),
            },
            [ConditionId.Disable] = new()
            {
                Id = ConditionId.Disable,
                Name = "Disable",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Disable,
                Duration = 5,
                NoCopy = true, // doesn't get copied by Baton Pass
                OnStart = new OnStartEventInfo((battle, pokemon, source, effect) =>
                {
                    // TODO: Check if target will move this turn or is active pokemon
                    // If so, decrease duration by 1

                    if (pokemon.LastMove == null)
                    {
                        battle.Debug("Pokemon hasn't moved yet");
                        return BoolVoidUnion.FromBool(false);
                    }

                    // Check if the last move has PP
                    foreach (var moveSlot in pokemon.MoveSlots)
                    {
                        if (moveSlot.Id == pokemon.LastMove.Id)
                        {
                            if (moveSlot.Pp <= 0)
                            {
                                battle.Debug("Move out of PP");
                                return BoolVoidUnion.FromBool(false);
                            }
                        }
                    }

                    if (battle.DisplayUi)
                    {
                        if (effect?.EffectType == EffectType.Ability)
                        {
                            battle.Add("-start", pokemon, "Disable", pokemon.LastMove.Name,
                                $"[from] ability: {effect.Name}", $"[of] {source}");
                        }
                        else
                        {
                            battle.Add("-start", pokemon, "Disable", pokemon.LastMove.Name);
                        }
                    }

                    battle.EffectState.Move = pokemon.LastMove.Id;
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((_, _, _, _) =>
                {
                    // Duration handled automatically
                }, 17),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Disable");
                    }
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, attacker, defender, move) =>
                {
                    // TODO: Check if move is Z-move
                    if (move.Id == battle.EffectState.Move)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", attacker, "Disable", move.Name);
                        }
                        return false;
                    }
                    return BoolVoidUnion.FromVoid();
                }, 7),
                OnDisableMove = new OnDisableMoveEventInfo((battle, pokemon) =>
                {
                    foreach (var moveSlot in pokemon.MoveSlots)
                    {
                        if (moveSlot.Id == battle.EffectState.Move)
                        {
                            pokemon.DisableMove(moveSlot.Id);
                        }
                    }
                }),
            },
            [ConditionId.Dig] = new()
            {
                Id = ConditionId.Dig,
                Name = "Dig",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Dig,
                Duration = 2,
                OnImmunity = new OnImmunityEventInfo((battle, type, pokemon) =>
                {
                    if (type == ConditionId.Sandstorm || type == ConditionId.Hail)
                    {
                        return false;
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnInvulnerability = new OnInvulnerabilityEventInfo((battle, target, source, move) =>
                {
                    if (move.Id == MoveId.Earthquake || move.Id == MoveId.Magnitude)
                    {
                        return BoolVoidUnion.FromVoid();
                    }
                    return false;
                }),
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                {
                    if (move.Id == MoveId.Earthquake || move.Id == MoveId.Magnitude)
                    {
                        return battle.ChainModify(2);
                    }
                    return damage;
                }),
            },
            [ConditionId.Dive] = new()
            {
                Id = ConditionId.Dive,
                Name = "Dive",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Dive,
                Duration = 2,
                OnImmunity = new OnImmunityEventInfo((battle, type, pokemon) =>
                {
                    if (type == ConditionId.Sandstorm || type == ConditionId.Hail)
                    {
                        return false;
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnInvulnerability = new OnInvulnerabilityEventInfo((battle, target, source, move) =>
                {
                    if (move.Id == MoveId.Surf || move.Id == MoveId.Whirlpool)
                    {
                        return BoolVoidUnion.FromVoid();
                    }
                    return false;
                }),
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                {
                    if (move.Id == MoveId.Surf || move.Id == MoveId.Whirlpool)
                    {
                        return battle.ChainModify(2);
                    }
                    return damage;
                }),
            },
            [ConditionId.Fly] = new()
            {
                Id = ConditionId.Fly,
                Name = "Fly",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Fly,
                Duration = 2,
                OnImmunity = new OnImmunityEventInfo((battle, type, pokemon) =>
                {
                    if (type == ConditionId.Sandstorm || type == ConditionId.Hail)
                    {
                        return false;
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnInvulnerability = new OnInvulnerabilityEventInfo((battle, target, source, move) =>
                {
                    if (move.Id == MoveId.Gust || move.Id == MoveId.Twister ||
                        move.Id == MoveId.SkyUppercut || move.Id == MoveId.Thunder ||
                        move.Id == MoveId.Hurricane || move.Id == MoveId.SmackDown ||
                        move.Id == MoveId.ThousandArrows)
                    {
                        return BoolVoidUnion.FromVoid();
                    }
                    return false;
                }),
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                {
                    if (move.Id == MoveId.Gust || move.Id == MoveId.Twister)
                    {
                        return battle.ChainModify(2);
                    }
                    return damage;
                }),
            },
            [ConditionId.FocusEnergy] = new()
            {
                Id = ConditionId.FocusEnergy,
                Name = "Focus Energy",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.FocusEnergy,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "move: Focus Energy");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: OnModifyCritRatio - increase crit ratio by 2 stages
            },
            [ConditionId.FocusPunch] = new()
            {
                Id = ConditionId.FocusPunch,
                Name = "Focus Punch",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.FocusPunch,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", pokemon, "move: Focus Punch");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, move) =>
                {
                    if (pokemon.Volatiles.TryGetValue(ConditionId.FocusPunch, out var state) &&
                        state.LostFocus == true)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "Focus Punch", "Focus Punch");
                        }
                        return false;
                    }
                    return BoolVoidUnion.FromVoid();
                }, 8),
                OnDamagingHit = new OnDamagingHitEventInfo((battle, damage, target, source, move) =>
                {
                    if (target.Volatiles.ContainsKey(ConditionId.FocusPunch))
                    {
                        target.Volatiles[ConditionId.FocusPunch].LostFocus = true;
                    }
                }),
            },
            [ConditionId.Fling] = new()
            {
                Id = ConditionId.Fling,
                Name = "Fling",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Fling,
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    var item = pokemon.GetItem();
                    pokemon.SetItem(null);
                    pokemon.LastItem = item;
                    // TODO: pokemon.usedItemThisTurn = true;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-enditem", pokemon, item.Name, "[from] move: Fling");
                    }
                    // TODO: battle.runEvent('AfterUseItem', pokemon, null, null, item);
                    pokemon.RemoveVolatile(_library.Conditions[ConditionId.Fling]);
                }),
            },
            [ConditionId.FollowMe] = new()
            {
                Id = ConditionId.FollowMe,
                Name = "Follow Me",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.FollowMe,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, target, _, effect) =>
                {
                    if (battle.DisplayUi)
                    {
                        if (effect?.Id == EffectId.ZPower)
                        {
                            battle.Add("-singleturn", target, "move: Follow Me", "[zeffect]");
                        }
                        else
                        {
                            battle.Add("-singleturn", target, "move: Follow Me");
                        }
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: OnFoeRedirectTarget - redirect attacks to this Pokemon
                // Priority 1, check if target is valid and not sky dropped
            },
            [ConditionId.FlashFire] = new()
            {
                Id = ConditionId.FlashFire,
                Name = "Flash Fire",
                EffectType = EffectType.Condition,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "ability: Flash Fire");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: OnModifyAtk (priority 5) - boost Fire moves by 1.5x if attacker has Flash Fire
                // TODO: OnModifySpA (priority 5) - boost Fire moves by 1.5x if attacker has Flash Fire
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "ability: Flash Fire", "[silent]");
                    }
                }),
            },
            [ConditionId.FuryCutter] = new()
            {
                Id = ConditionId.FuryCutter,
                Name = "Fury Cutter",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.FuryCutter,
                Duration = 2,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    battle.EffectState.HitCount = 1;
                    return BoolVoidUnion.FromVoid();
                }),
                OnRestart = new OnRestartEventInfo((battle, pokemon, _, _) =>
                {
                    if ((battle.EffectState.HitCount ?? 0) < 4)
                    {
                        battle.EffectState.HitCount = (battle.EffectState.HitCount ?? 0) + 1;
                    }
                    battle.EffectState.Duration = 2;
                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: OnBasePower - multiply base power by 2^(hitCount-1)
            },
            [ConditionId.Flinch] = new()
            {
                Id = ConditionId.Flinch,
                Name = "Flinch",
                EffectType = EffectType.Condition,
                Duration = 1,
                //OnBeforeMovePriority = 8,
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "flinch");
                        }

                        battle.RunEvent(EventId.Flinch, pokemon);
                        return false;
                    },
                    8),
            },
            [ConditionId.Freeze] = new()
            {
                Id = ConditionId.Freeze,
                Name = "Freeze",
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Ice],
                OnStart = new OnStartEventInfo((battle, target, source, sourceEffect) =>
                {
                    if (battle.DisplayUi)
                    {
                        switch (sourceEffect)
                        {
                            case Ability:
                                battle.Add("-status", target, "frz", "[from] ability: " +
                                    sourceEffect.Name, $"[of] {source}");
                                break;
                            default:
                                battle.Add("-status", target, "frz");
                                break;
                        }
                    }

                    if (target.Species.Id == SpecieId.ShayminSky &&
                        target.BaseSpecies.BaseSpecies == SpecieId.Shaymin)
                    {
                        target.FormeChange(SpecieId.Shaymin, battle.Effect, true);
                    }

                    return new VoidReturn();
                }),
                //OnBeforeMovePriority = 10,
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, move) =>
                    {
                        if ((move.Flags.Defrost ?? false) &&
                            !(move.Id == MoveId.BurnUp && !pokemon.HasType(PokemonType.Fire)))
                        {
                            return new VoidReturn();
                        }

                        if (battle.RandomChance(1, 5))
                        {
                            pokemon.CureStatus();
                            return new VoidReturn();
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "frz");
                        }

                        return false;
                    },
                    10),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    if (!(move.Flags.Defrost ?? false)) return;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-curestatus", pokemon, "frz", $"[from] move: {move}");
                    }

                    pokemon.ClearStatus();
                }),
                OnAfterMoveSecondary = new OnAfterMoveSecondaryEventInfo((_, target, _, move) =>
                {
                    if (move.ThawsTarget ?? false)
                    {
                        target.CureStatus();
                    }
                }),
                OnDamagingHit = new OnDamagingHitEventInfo((_, _, target, _, move) =>
                {
                    if (move.Type == MoveType.Fire && move.Category != MoveCategory.Status &&
                        move.Id != MoveId.PolarFlare)
                    {
                        target.CureStatus();
                    }
                }),
            },
            [ConditionId.DesolateLand] = new()
            {
                Id = ConditionId.DesolateLand,
                Name = "DesolateLand",
                EffectType = EffectType.Weather,
                Duration = 0,
                //OnTryMovePriority = 1,
                OnTryMove = new OnTryMoveEventInfo((battle, attacker, _, move) =>
                    {
                        if (move.Type == MoveType.Water && move.Category != MoveCategory.Status)
                        {
                            battle.Debug("Desolate Land water suppress");
                            if (battle.DisplayUi)
                            {
                                battle.Add("-fail", attacker, move, "[from] Desolate Land");
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
                        if (move.Type == MoveType.Fire)
                        {
                            battle.Debug("Sunny Day fire boost");
                            return battle.ChainModify(1.5);
                        }

                        return new VoidReturn();
                    }),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-weather", "DesolateLand", "[from] ability: " + effect?.Name,
                            $"[of] {source}");
                    }
                }),
                // Note: Freeze immunity handled elsewhere
                //OnFieldResidualOrder = 1,
                OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "DesolateLand", "[upkeep]");
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
            [ConditionId.DeltaStream] = new()
            {
                Id = ConditionId.DeltaStream,
                Name = "DeltaStream",
                EffectType = EffectType.Weather,
                Duration = 0,
                //OnEffectivenessPriority = -1,
                OnEffectiveness = new OnEffectivenessEventInfo((battle, typeMod, _, type, move) =>
                    {
                        if (move is not null && move.EffectType == EffectType.Move &&
                            move.Category != MoveCategory.Status &&
                            type == PokemonType.Flying && typeMod > 0)
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Add("-fieldactivate", "Delta Stream");
                            }

                            return 0;
                        }

                        return new VoidReturn();
                    },
                    -1),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-weather", "DeltaStream", "[from] ability: " + effect?.Name,
                            $"[of] {source}");
                    }
                }),
                //OnFieldResidualOrder = 1,
                OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "DeltaStream", "[upkeep]");
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
            //[ConditionId.Dynamax] = new()
            //{
            //    Id = ConditionId.Dynamax,
            //    Name = "Dynamax",
            //    EffectType = EffectType.Condition,
            //    NoCopy = true,
            //    OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
            //    {
            //        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Substitute]);

            //        if (pokemon.Volatiles.TryGetValue(ConditionId.Torment, out _))
            //        {
            //            pokemon.DeleteVolatile(ConditionId.Torment);
            //            if (battle.DisplayUi)
            //            {
            //                battle.Add("-end", pokemon, "Torment", "[silent]");
            //            }
            //        }

            //        // Handle Cramorant formes
            //        if ((pokemon.Species.Id == SpecieId.CramorantGulping ||
            //             pokemon.Species.Id == SpecieId.CramorantGorging) && !pokemon.Transformed)
            //        {
            //            pokemon.FormeChange(SpecieId.Cramorant);
            //        }

            //        if (battle.DisplayUi)
            //        {
            //            battle.Add("-start", pokemon, "Dynamax");
            //        }

            //        if (pokemon.BaseSpecies.Name == "Shedinja") return new VoidReturn();

            //        // Default dynamax HP multiplier
            //        const double ratio = 2.0;
            //        pokemon.MaxHp = (int)Math.Floor(pokemon.MaxHp * ratio);
            //        pokemon.Hp = (int)Math.Floor(pokemon.Hp * ratio);
            //        if (battle.DisplayUi)
            //        {
            //            battle.Add("-heal", pokemon, pokemon.GetHealth, "[silent]");
            //        }

            //        return new VoidReturn();
            //    }),
            //    OnTryAddVolatile = new OnTryAddVolatileEventInfo((_, status, _, _, _) =>
            //    {
            //        if (status.Id == ConditionId.Flinch) return null;
            //        return new VoidReturn();
            //    }),
            //    //OnBeforeSwitchOutPriority = -1,
            //    OnBeforeSwitchOut = new OnBeforeSwitchOutEventInfo(
            //        (_, pokemon) =>
            //        {
            //            pokemon.RemoveVolatile(_library.Conditions[ConditionId.Dynamax]);
            //        },
            //        -1),
            //    OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, _, _, _, move) =>
            //    {
            //        if (move.Id == MoveId.BehemothBash || move.Id == MoveId.BehemothBlade ||
            //            move.Id == MoveId.DynamaxCannon)
            //        {
            //            return battle.ChainModify(2);
            //        }

            //        return new VoidReturn();
            //    }),
            //    //OnDragOutPriority = 2,
            //    OnDragOut = new OnDragOutEventInfo((battle, pokemon, _, _) =>
            //        {
            //            if (battle.DisplayUi)
            //            {
            //                battle.Add("-block", pokemon, "Dynamax");
            //            }
            //            // Prevent drag out - handled by returning false in TryHit or similar
            //        },
            //        2),
            //    OnEnd = new OnEndEventInfo((battle, pokemon) =>
            //    {
            //        if (battle.DisplayUi)
            //        {
            //            battle.Add("-end", pokemon, "Dynamax");
            //        }

            //        if (pokemon.BaseSpecies.Name == "Shedinja") return;
            //        // Restore HP proportionally
            //        pokemon.Hp = pokemon.Hp / 2;
            //        pokemon.MaxHp = pokemon.BaseMaxHp;
            //        if (battle.DisplayUi)
            //        {
            //            battle.Add("-heal", pokemon, pokemon.GetHealth, "[silent]");
            //        }
            //    }),
            //},
            [ConditionId.Embargo] = new()
            {
                Id = ConditionId.Embargo,
                Name = "Embargo",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Embargo,
                Duration = 5,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Embargo");
                    }
                    // TODO: singleEvent('End', pokemon.getItem(), pokemon.itemState, pokemon)
                    // to trigger item end effects
                    return BoolVoidUnion.FromVoid();
                }),
                // Item suppression implemented in Pokemon.IgnoringItem()
                OnResidual = new OnResidualEventInfo((_, _, _, _) =>
                {
                    // Duration handled automatically
                }, 21),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Embargo");
                    }
                }),
            },
            [ConditionId.Encore] = new()
            {
                Id = ConditionId.Encore,
                Name = "Encore",
                EffectType = EffectType.Condition,
                Duration = 3,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    // TODO: Check if target's last move is valid for Encore
                    // For now, just add the start message
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "Encore");
                    }
                    return new VoidReturn();
                }),
                // TODO: OnOverrideAction - force encored move
                // TODO: OnResidual - check if PP is depleted
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "Encore");
                    }
                }),
                // TODO: OnDisableMove - disable all moves except encored move
            },
            [ConditionId.EchoedVoice] = new()
            {
                Id = ConditionId.EchoedVoice,
                Name = "Echoed Voice",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.EchoedVoice,
                Duration = 2,
                OnFieldStart = new OnFieldStartEventInfo((battle, _, _, _) =>
                {
                    battle.EffectState.Multiplier = 1;
                }),
                OnFieldRestart = new OnFieldRestartEventInfo((battle, _, _) =>
                {
                    if (battle.EffectState.Duration != 2)
                    {
                        battle.EffectState.Duration = 2;
                        if ((battle.EffectState.Multiplier ?? 1) < 5)
                        {
                            battle.EffectState.Multiplier = (battle.EffectState.Multiplier ?? 1) + 1;
                        }
                    }
                }),
                // This is a field pseudo-weather condition
                // BasePowerCallback handled in the move itself
            },
            [ConditionId.Drain] = new()
            {
                Id = ConditionId.Drain,
                Name = "Drain",
                EffectType = EffectType.Condition,
                // Marker condition for drain moves
            },
            [ConditionId.Endure] = new()
            {
                Id = ConditionId.Endure,
                Name = "Endure",
                EffectType = EffectType.Condition,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "move: Endure");
                    }
                    return new VoidReturn();
                }),
                // TODO: OnDamage (priority -10) - prevent fainting, leave at 1 HP
            },
            [ConditionId.FutureMove] = new()
            {
                Id = ConditionId.FutureMove,
                Name = "Future Move",
                EffectType = EffectType.Condition,
                // This is a slot condition
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    battle.EffectState.TargetSlot = target.GetSlot();
                    battle.EffectState.EndingTurn = (battle.Turn - 1) + 2;
                    if (battle.EffectState.EndingTurn >= 254)
                    {
                        // In Gen 8+, Future attacks will never resolve when used on the 255th turn or later
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, target, _, _) =>
                {
                    // TODO: Implement GetOverflowedTurnCount if needed
                    if (battle.Turn < battle.EffectState.EndingTurn) return;
                    var slotTarget = battle.GetAtSlot(battle.EffectState.TargetSlot);
                    if (slotTarget != null)
                    {
                        target.Side.RemoveSlotCondition(slotTarget, _library.Conditions[ConditionId.FutureMove]);
                    }
                }, 3),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    // TODO: Implement full Future Move logic:
                    // 1. Get move data from effectState
                    // 2. Check if target is fainted or is the source
                    // 3. Remove Protect/Endure volatiles
                    // 4. Apply Infiltrator ability if source has it (Gen 6+)
                    // 5. Apply Normalize ability if source has it (Gen 6+)
                    // 6. Execute the move with trySpreadMoveHit
                    // 7. Trigger Life Orb recoil if applicable (Gen 5+)
                    // 8. Check for win condition
                }),
            },
        };
    }
}