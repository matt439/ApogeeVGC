using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
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
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsStu()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.Sleep] = new()
            {
                Id = ConditionId.Sleep,
                Name = "Sleep",
                EffectType = EffectType.Status,
                OnStart = new OnStartEventInfo((battle, target, source, sourceEffect) =>
                {
                    if (battle.DisplayUi)
                    {
                        switch (sourceEffect)
                        {
                            case Ability:
                                battle.Add("-status", target, "slp", "[from] ability: " +
                                    sourceEffect.Name, $"[of] {source}");
                                break;
                            case ActiveMove:
                                battle.Add("-status", target, "slp",
                                    $"[from] move: {sourceEffect.Name}");
                                break;
                            default:
                                battle.Add("-status", target, "slp");
                                break;
                        }
                    }

                    battle.EffectState.StartTime = battle.Random(2, 5);
                    battle.EffectState.Time = battle.EffectState.StartTime;

                    Condition nightmare = _library.Conditions[ConditionId.Nightmare];

                    if (!target.RemoveVolatile(nightmare)) return new VoidReturn();

                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "Nightmare", "[silent]");
                    }

                    return new VoidReturn();
                }),
                //OnBeforeMovePriority = 10,
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, move) =>
                    {
                        if (pokemon.HasAbility(AbilityId.EarlyBird))
                        {
                            pokemon.StatusState.Time--;
                        }

                        pokemon.StatusState.Time--;

                        if (pokemon.StatusState.Time <= 0)
                        {
                            pokemon.CureStatus();
                            return new VoidReturn();
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "slp");
                        }

                        if (move.SleepUsable ?? false)
                        {
                            return new VoidReturn();
                        }

                        return false;
                    },
                    10),
            },
            [ConditionId.PhantomForce] = new()
            {
                Id = ConditionId.PhantomForce,
                Name = "Phantom Force",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.PhantomForce,
                Duration = 2,
                OnImmunity = new OnImmunityEventInfo((battle, type, pokemon) =>
                {
                    // Weather immunity is handled elsewhere; this is a void handler
                    return new VoidReturn();
                }),
                OnInvulnerability = new OnInvulnerabilityEventInfo((battle, target, source, move) =>
                {
                    // Phantom Force can only be hit by certain moves
                    return BoolIntEmptyVoidUnion.FromBool(false);
                }),
                // TODO: OnTryHit - ignore Protect/Detect on the attack turn
            },
            [ConditionId.ShadowForce] = new()
            {
                Id = ConditionId.ShadowForce,
                Name = "Shadow Force",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.ShadowForce,
                Duration = 2,
                OnImmunity = new OnImmunityEventInfo((battle, type, pokemon) =>
                {
                    // Weather immunity is handled elsewhere; this is a void handler
                    return new VoidReturn();
                }),
                OnInvulnerability = new OnInvulnerabilityEventInfo((battle, target, source, move) =>
                {
                    // Shadow Force can only be hit by certain moves
                    return BoolIntEmptyVoidUnion.FromBool(false);
                }),
                // TODO: OnTryHit - ignore Protect/Detect on the attack turn
            },
            [ConditionId.Silvally] = new()
            {
                Id = ConditionId.Silvally,
                Name = "Silvally",
                EffectType = EffectType.Condition,
                OnType = new OnTypeEventInfo((battle, types, pokemon) =>
                {
                    if (pokemon.Transformed ||
                        (pokemon.Ability != AbilityId.RksSystem && battle.Gen >= 8))
                    {
                        return types;
                    }

                    PokemonType type = PokemonType.Normal;
                    if (pokemon.Ability == AbilityId.RksSystem)
                    {
                        var item = pokemon.GetItem();
                        // TODO: Get type from item.OnMemory property when implemented
                        // For now, default to Normal type
                        type = PokemonType.Normal;
                    }

                    return new PokemonType[] { type };
                }, 1),
            },
            [ConditionId.SilkTrap] = new()
            {
                Id = ConditionId.SilkTrap,
                Name = "Silk Trap",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.SilkTrap,
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
                        // TODO: Check if move.isZ or move.isMax and set zBrokeProtect
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }
                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Protect");
                    }
                    // TODO: Check for lockedmove volatile and reset Outrage counter
                    // TODO: Check if move makes contact and lower Speed
                    // if (this.checkMoveMakesContact(move, source, target)) {
                    //     this.boost({ spe: -1 }, source, target, this.dex.getActiveMove("Silk Trap"));
                    // }
                    return BoolIntEmptyVoidUnion.FromBool(false);
                }, 3),
                // TODO: OnHit - if move is Z or Max powered and makes contact, lower Speed
            },
            [ConditionId.SkyDrop] = new()
            {
                Id = ConditionId.SkyDrop,
                Name = "Sky Drop",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.SkyDrop,
                Duration = 2,
                OnImmunity = new OnImmunityEventInfo((battle, type, pokemon) =>
                {
                    // Weather immunity is handled elsewhere; this is a void handler
                    return new VoidReturn();
                }),
                OnInvulnerability = new OnInvulnerabilityEventInfo((battle, target, source, move) =>
                {
                    if (move.Id == MoveId.Gust || move.Id == MoveId.Twister ||
                        move.Id == MoveId.SkyUppercut || move.Id == MoveId.Thunder ||
                        move.Id == MoveId.Hurricane || move.Id == MoveId.SmackDown ||
                        move.Id == MoveId.ThousandArrows)
                    {
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }
                    return BoolIntEmptyVoidUnion.FromBool(false);
                }),
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                {
                    if (move.Id == MoveId.Gust || move.Id == MoveId.Twister)
                    {
                        return battle.ChainModify(2);
                    }
                    return damage;
                }),
                // TODO: OnRedirectTarget - Sky Drop carries the target into the air with the user
                // TODO: OnDragOut - release the target if user is forced out
                // TODO: OnFaint - release the target if user faints
            },
            [ConditionId.Stall] = new()
            {
                // Protect, Detect, Endure counter
                Id = ConditionId.Stall,
                Name = "Stall",
                Duration = 2,
                CounterMax = 729,
                EffectType = EffectType.Condition,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    // During OnStart, battle.EffectState IS the volatile's state being created
                    battle.EffectState.Counter = 3;
                    battle.Debug($"[Stall.OnStart] {pokemon.Name}: Initialized counter to 3");
                    return new VoidReturn();
                }),
                OnStallMove = new OnStallMoveEventInfo((battle, pokemon) =>
                {
                    // Get the counter from the Pokemon's Stall volatile state
                    int counter = 1; // Default for first use

                    if (pokemon.Volatiles.TryGetValue(ConditionId.Stall, out var stallState))
                    {
                        counter = stallState.Counter ?? 1;
                    }

                    battle.Debug(
                        $"[Stall.OnStallMove] {pokemon.Name}: Checking with counter={counter}, Success chance: {Math.Round(100.0 / counter, 2)}%");

                    bool success = battle.RandomChance(1, counter);

                    if (!success)
                    {
                        battle.Debug(
                            $"[Stall.OnStallMove] {pokemon.Name}: FAILED! Deleting Stall volatile");
                        pokemon.DeleteVolatile(ConditionId.Stall);
                    }
                    else
                    {
                        battle.Debug($"[Stall.OnStallMove] {pokemon.Name}: SUCCESS!");
                    }

                    return success;
                }),
                OnRestart = new OnRestartEventInfo((battle, pokemon, _, _) =>
                {
                    // During OnRestart, battle.EffectState IS the volatile's state being restarted
                    int oldCounter = battle.EffectState.Counter ?? 1;

                    // Update the counter in the volatile's state
                    if (battle.EffectState.Counter < 729) // CounterMax
                    {
                        battle.EffectState.Counter *= 3;
                    }

                    battle.EffectState.Duration = 2;

                    battle.Debug(
                        $"[Stall.OnRestart] {pokemon.Name}: Counter increased from {oldCounter} to {battle.EffectState.Counter}, Duration reset to 2");

                    return new VoidReturn();
                }),
            },
            [ConditionId.SyrupBomb] = new()
            {
                Id = ConditionId.SyrupBomb,
                Name = "Syrup Bomb",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.SyrupBomb,
                NoCopy = true,
                Duration = 4,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Syrup Bomb");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (battle.EffectState.Source != null && !battle.EffectState.Source.IsActive)
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.SyrupBomb]);
                    }
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Boost(new SparseBoostsTable { Spe = -1 },
                        pokemon, battle.EffectState.Source);
                }, 14),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Syrup Bomb", "[silent]");
                    }
                }),
            },
            [ConditionId.StruggleRecoil] = new()
            {
                Id = ConditionId.StruggleRecoil,
            },
            [ConditionId.Substitute] = new()
            {
                Id = ConditionId.Substitute,
                Name = "Substitute",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Substitute,
                OnStart = new OnStartEventInfo((battle, target, source, effect) =>
                {
                    if (effect is Move { Id: MoveId.ShedTail })
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-start", target, "Substitute", "[from] move: Shed Tail");
                        }
                    }
                    else
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-start", target, "Substitute");
                        }
                    }
                    battle.EffectState.Hp = (int)Math.Floor(target.MaxHp / 4.0);

                    // Remove partially trapped condition when substitute is created
                    if (target.Volatiles.ContainsKey(ConditionId.PartiallyTrapped))
                    {
                        var sourceEffect = target.Volatiles[ConditionId.PartiallyTrapped].SourceEffect;
                        if (battle.DisplayUi)
                        {
                            battle.Add("-end", target, sourceEffect?.Name ?? "Partially Trapped",
                                "[partiallytrapped]", "[silent]");
                        }
                        target.DeleteVolatile(ConditionId.PartiallyTrapped);
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnTryPrimaryHit = new OnTryPrimaryHitEventInfo((battle, target, source, move) =>
                {
                    if (target == source || (move.Flags.BypassSub ?? false) || (move.Infiltrates ?? false))
                    {
                        return IntBoolVoidUnion.FromVoid();
                    }

                    // TODO: Implement full Substitute damage blocking logic:
                    // 1. Calculate damage with actions.getDamage
                    // 2. If no damage, fail the move
                    // 3. Cap damage at substitute HP
                    // 4. Subtract from substitute HP
                    // 5. If substitute breaks, remove it
                    // 6. Otherwise show activate message
                    // 7. Handle recoil and drain
                    // 8. Trigger AfterSubDamage events
                    // 9. Return HIT_SUBSTITUTE

                    return IntBoolVoidUnion.FromVoid();
                }, -1),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "Substitute");
                    }
                }),
            },
            [ConditionId.Tailwind] = new()
            {
                Id = ConditionId.Tailwind,
                Name = "Tailwind",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Tailwind,
                Duration = 4,
                DurationCallback = new DurationCallbackEventInfo((_, _, _, _) => 4),
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "move: Tailwind");
                    }
                }),
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, _) =>
                {
                    // Tailwind doubles speed using chain modification
                    battle.ChainModify(2);
                    // Apply the accumulated modifier to the speed value
                    return battle.FinalModify(spe);
                }),
                //OnSideResidualOrder = 26,
                //OnSideResidualSubOrder = 5,
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 5,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "move: Tailwind");
                    }
                }),
            },
            [ConditionId.Telekinesis] = new()
            {
                Id = ConditionId.Telekinesis,
                Name = "Telekinesis",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Telekinesis,
                Duration = 3,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    // Check for immune species
                    string[] immuneSpecies = { "Diglett", "Dugtrio", "Palossand", "Sandygast", "Gengar-Mega" };
                    if (immuneSpecies.Contains(target.BaseSpecies.Name))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-immune", target);
                        }
                        return BoolVoidUnion.FromBool(false);
                    }
                    if (target.Volatiles.ContainsKey(ConditionId.SmackDown) ||
                        target.Volatiles.ContainsKey(ConditionId.Ingrain))
                    {
                        return BoolVoidUnion.FromBool(false);
                    }
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "Telekinesis");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: OnAccuracy - if move is not OHKO, return true (perfect accuracy)
                OnImmunity = new OnImmunityEventInfo((battle, type, pokemon) =>
                {
                    // Ground immunity is handled by Pokemon.IsGrounded() for Telekinesis
                    return new VoidReturn();
                }),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.BaseSpecies.Name == "Gengar-Mega")
                    {
                        pokemon.DeleteVolatile(ConditionId.Telekinesis);
                        if (battle.DisplayUi)
                        {
                            battle.Add("-end", pokemon, "Telekinesis", "[silent]");
                        }
                    }
                }),
                OnResidual = new OnResidualEventInfo((_, _, _, _) =>
                {
                    // Duration handled automatically
                }, 19),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "Telekinesis");
                    }
                }),
            },
            [ConditionId.Taunt] = new()
            {
                Id = ConditionId.Taunt,
                Name = "Taunt",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Taunt,
                Duration = 3,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    // TODO: If target has already taken its turn and won't move again this turn, increase duration by 1
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "move: Taunt");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((_, _, _, _) =>
                {
                    // Duration handled automatically
                }, 15),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "move: Taunt");
                    }
                }),
                OnDisableMove = new OnDisableMoveEventInfo((battle, pokemon) =>
                {
                    foreach (var moveSlot in pokemon.MoveSlots)
                    {
                        var move = _library.Moves[moveSlot.Id];
                        if (move.Category == MoveCategory.Status && move.Id != MoveId.MeFirst)
                        {
                            pokemon.DisableMove(moveSlot.Id);
                        }
                    }
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, attacker, defender, move) =>
                {
                    // TODO: Check if move is Z-move
                    if (move.Category == MoveCategory.Status && move.Id != MoveId.MeFirst)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", attacker, "move: Taunt", move.Name);
                        }
                        return false;
                    }
                    return BoolVoidUnion.FromVoid();
                }, 5),
            },
            [ConditionId.ShedTail] = new()
            {
                Id = ConditionId.ShedTail,
                Name = "Shed Tail",
                EffectType = EffectType.Condition,
                // Shed Tail creates a Substitute volatile and then switches out
                // The condition itself may not need separate handling beyond Substitute
            },
            [ConditionId.SimpleBeam] = new()
            {
                Id = ConditionId.SimpleBeam,
                Name = "Simple Beam",
                EffectType = EffectType.Condition,
                // Simple Beam doesn't create a volatile, it changes the ability directly
                // This is a placeholder condition that may not be used
            },
            [ConditionId.SparklingAria] = new()
            {
                Id = ConditionId.SparklingAria,
                Name = "Sparkling Aria",
                EffectType = EffectType.Condition,
                // Sparkling Aria applies a volatile to mark targets that will be cured of burn
                // The volatile is cleared after processing all targets
            },
            [ConditionId.SmackDown] = new()
            {
                Id = ConditionId.SmackDown,
                Name = "Smack Down",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.SmackDown,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    bool applies = false;
                    if (pokemon.HasType(PokemonType.Flying) || pokemon.HasAbility(AbilityId.Levitate))
                    {
                        applies = true;
                    }
                    if (pokemon.HasItem(ItemId.IronBall) || pokemon.Volatiles.ContainsKey(ConditionId.Ingrain) ||
                        battle.Field.PseudoWeather.ContainsKey(ConditionId.Gravity))
                    {
                        applies = false;
                    }
                    if (pokemon.RemoveVolatile(_library.Conditions[ConditionId.Fly]) ||
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Bounce]))
                    {
                        applies = true;
                        // TODO: battle.queue.cancelMove(pokemon);
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.TwoTurnMove]);
                    }
                    if (pokemon.Volatiles.ContainsKey(ConditionId.MagnetRise))
                    {
                        applies = true;
                        pokemon.DeleteVolatile(ConditionId.MagnetRise);
                    }
                    if (pokemon.Volatiles.ContainsKey(ConditionId.Telekinesis))
                    {
                        applies = true;
                        pokemon.DeleteVolatile(ConditionId.Telekinesis);
                    }
                    if (!applies) return BoolVoidUnion.FromBool(false);
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Smack Down");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnRestart = new OnRestartEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.RemoveVolatile(_library.Conditions[ConditionId.Fly]) ||
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Bounce]))
                    {
                        // TODO: battle.queue.cancelMove(pokemon);
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.TwoTurnMove]);
                        if (battle.DisplayUi)
                        {
                            battle.Add("-start", pokemon, "Smack Down");
                        }
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                // Groundedness implemented in Pokemon.IsGrounded()
            },
            [ConditionId.Torment] = new()
            {
                Id = ConditionId.Torment,
                Name = "Torment",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Torment,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, _, effect) =>
                {
                    // TODO: Check for Dynamax volatile
                    if (effect is Move { Id: MoveId.GMaxMeltdown })
                    {
                        battle.EffectState.Duration = 3;
                    }
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Torment");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Torment");
                    }
                }),
                OnDisableMove = new OnDisableMoveEventInfo((battle, pokemon) =>
                {
                    if (pokemon.LastMove != null && pokemon.LastMove.Id != MoveId.Struggle)
                    {
                        pokemon.DisableMove(pokemon.LastMove.Id);
                    }
                }),
            },
            [ConditionId.Toxic] = new()
            {
                Id = ConditionId.Toxic,
                Name = "Toxic",
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Poison, PokemonType.Steel],
                OnStart = new OnStartEventInfo((battle, target, source, sourceEffect) =>
                {
                    battle.EffectState.Stage = 0;
                    if (!battle.DisplayUi) return new VoidReturn();

                    switch (sourceEffect)
                    {
                        case Item { Id: ItemId.ToxicOrb }:
                            battle.Add("-status", target, "tox", "[from] item: Toxic Orb");
                            break;
                        case Ability:
                            battle.Add("-status", target, "tox", "[from] ability: " +
                                                                 sourceEffect.Name,
                                $"[of] {source}");
                            break;
                        default:
                            battle.Add("-status", target, "tox");
                            break;
                    }

                    return new VoidReturn();
                }),
                OnSwitchIn = new OnSwitchInEventInfo((battle, _) =>
                {
                    battle.EffectState.Stage = 0;
                }),
                //OnResidualOrder = 9,
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                    {
                        if (battle.EffectState.Stage < 15)
                        {
                            battle.EffectState.Stage++;
                        }

                        battle.Damage(battle.ClampIntRange(pokemon.BaseMaxHp / 16, 1, null) *
                                      (battle.EffectState.Stage ?? 0));
                    },
                    9),
            },
            [ConditionId.ToxicSpikes] = new()
            {
                Id = ConditionId.ToxicSpikes,
                Name = "Toxic Spikes",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.ToxicSpikes,
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "move: Toxic Spikes");
                    }
                    battle.EffectState.Layers = 1;
                }),
                OnSideRestart = new OnSideRestartEventInfo((battle, side, _, _) =>
                {
                    if ((battle.EffectState.Layers ?? 0) >= 2) return;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "move: Toxic Spikes");
                    }
                    battle.EffectState.Layers = (battle.EffectState.Layers ?? 0) + 1;
                }),
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    if (pokemon.IsGrounded() != true) return;
                    if (pokemon.HasType(PokemonType.Poison))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-sideend", pokemon.Side, "move: Toxic Spikes", $"[of] {pokemon}");
                        }
                        pokemon.Side.RemoveSideCondition(ConditionId.ToxicSpikes);
                    }
                    else if (pokemon.HasType(PokemonType.Steel) || pokemon.HasItem(ItemId.HeavyDutyBoots))
                    {
                        // do nothing
                    }
                    else if ((battle.EffectState.Layers ?? 0) >= 2)
                    {
                        // TODO: Get opponent Pokemon for source parameter (pokemon.Side.Foe.Active[0])
                        pokemon.TrySetStatus(ConditionId.Toxic, pokemon);
                    }
                    else
                    {
                        // TODO: Get opponent Pokemon for source parameter (pokemon.Side.Foe.Active[0])
                        pokemon.TrySetStatus(ConditionId.Poison, pokemon);
                    }
                }),
            },
            [ConditionId.Trapped] = new()
            {
                Id = ConditionId.Trapped,
                NoCopy = true,
                OnTrapPokemon = new OnTrapPokemonEventInfo((_, pokemon) => { pokemon.TryTrap(); }),
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    battle.Add("-activate", target, "trapped");
                    return new VoidReturn();
                }),
            },
            [ConditionId.TrickRoom] = new()
            {
                Id = ConditionId.TrickRoom,
                Name = "Trick Room",
                EffectType = EffectType.Condition,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, _, _, _) => 5),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldstart", "move: Trick Room", $"[of] {source}");
                    }
                }),
                OnFieldRestart = new OnFieldRestartEventInfo((battle, _, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Debug("[TrickRoom.OnFieldRestart] Handler called!");
                    }

                    // When Trick Room is used while already active, it should end instead of restart
                    battle.Field.RemovePseudoWeather(ConditionId.TrickRoom);

                    if (battle.DisplayUi)
                    {
                        battle.Debug("[TrickRoom.OnFieldRestart] After RemovePseudoWeather call");
                    }
                }),
                //OnFieldResidualOrder = 27,
                //OnFieldResidualSubOrder = 1,
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Trick Room");
                    }
                }),
            },
            [ConditionId.Truant] = new()
            {
                Id = ConditionId.Truant,
                Name = "Truant",
                EffectType = EffectType.Condition,
                // Truant is an ability volatile condition - empty condition marker
                // The actual logic is in the ability handler
            },
            [ConditionId.Unburden] = new()
            {
                Id = ConditionId.Unburden,
                Name = "Unburden",
                EffectType = EffectType.Condition,
                // TODO: OnModifySpe - double speed if Pokemon has no item and not ignoring ability
            },
            [ConditionId.SaltCure] = new()
            {
                Id = ConditionId.SaltCure,
                Name = "Salt Cure",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.SaltCure,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Salt Cure");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    int divisor = (pokemon.HasType(PokemonType.Water) || pokemon.HasType(PokemonType.Steel)) ? 4 : 8;
                    battle.Damage(pokemon.BaseMaxHp / divisor, pokemon, null);
                }, 13),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Salt Cure");
                    }
                }),
            },
            [ConditionId.Sandstorm] = new()
            {
                Id = ConditionId.Sandstorm,
                Name = "Sandstorm",
                EffectType = EffectType.Weather,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                    source.HasItem(ItemId.SmoothRock) ? 8 : 5),
                // This should be applied directly to the stat before any of the other modifiers are chained
                // So we give it increased priority.
                //OnModifySpDPriority = 10,
                OnModifySpD = new OnModifySpDEventInfo((battle, spd, pokemon, _, _) =>
                    {
                        if (pokemon.HasType(PokemonType.Rock) &&
                            battle.Field.IsWeather(ConditionId.Sandstorm))
                        {
                            return battle.Modify(spd, 1.5);
                        }

                        return spd;
                    },
                    10),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (!battle.DisplayUi) return;
                    if (effect is Ability)
                    {
                        if (battle.Gen <= 5) battle.EffectState.Duration = 0;
                        battle.Add("-weather", "Sandstorm", "[from] ability: " + effect.Name,
                            $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-weather", "Sandstorm");
                    }
                }),
                //OnFieldResidualOrder = 1,
                OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "Sandstorm", "[upkeep]");
                        }

                        if (battle.Field.IsWeather(ConditionId.Sandstorm))
                        {
                            battle.EachEvent(EventId.Weather);
                        }
                    },
                    1),
                OnWeather = new OnWeatherEventInfo((battle, target, _, _) =>
                {
                    battle.Damage(target.BaseMaxHp / 16);
                }),
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-weather", "none");
                    }
                }),
            },
            [ConditionId.Snowscape] = new()
            {
                Id = ConditionId.Snowscape,
                Name = "Snowscape",
                EffectType = EffectType.Weather,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                    source.HasItem(ItemId.IcyRock) ? 8 : 5),
                //OnModifyDefPriority = 10,
                OnModifyDef = new OnModifyDefEventInfo((battle, def, pokemon, _, _) =>
                    {
                        if (pokemon.HasType(PokemonType.Ice) &&
                            battle.Field.IsWeather(ConditionId.Snowscape))
                        {
                            return battle.Modify(def, 1.5);
                        }

                        return def;
                    },
                    10),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (!battle.DisplayUi) return;
                    if (effect is Ability)
                    {
                        if (battle.Gen <= 5) battle.EffectState.Duration = 0;
                        battle.Add("-weather", "Snowscape", "[from] ability: " + effect.Name,
                            $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-weather", "Snowscape");
                    }
                }),
                //OnFieldResidualOrder = 1,
                OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "Snowscape", "[upkeep]");
                        }

                        if (battle.Field.IsWeather(ConditionId.Snowscape))
                        {
                            battle.EachEvent(EventId.Weather);
                        }
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
            [ConditionId.SunnyDay] = new()
            {
                Id = ConditionId.SunnyDay,
                Name = "SunnyDay",
                EffectType = EffectType.Weather,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                    source.HasItem(ItemId.HeatRock) ? 8 : 5),
                OnWeatherModifyDamage =
                    new OnWeatherModifyDamageEventInfo((battle, _, attacker, defender, move) =>
                    {
                        if (move.Id == MoveId.HydroSteam &&
                            !attacker.HasItem(ItemId.UtilityUmbrella))
                        {
                            battle.Debug("Sunny Day Hydro Steam boost");
                            return battle.ChainModify(1.5);
                        }

                        if (defender.HasItem(ItemId.UtilityUmbrella)) return new VoidReturn();
                        if (move.Type == MoveType.Fire)
                        {
                            battle.Debug("Sunny Day fire boost");
                            return battle.ChainModify(1.5);
                        }

                        if (move.Type == MoveType.Water)
                        {
                            battle.Debug("Sunny Day water suppress");
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
                        battle.Add("-weather", "SunnyDay", "[from] ability: " + effect.Name,
                            $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-weather", "SunnyDay");
                    }
                }),
                //OnFieldResidualOrder = 1,
                OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "SunnyDay", "[upkeep]");
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
            [ConditionId.TarShot] = new()
            {
                Id = ConditionId.TarShot,
                Name = "Tar Shot",
                EffectType = EffectType.Condition,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    // TODO: check if terastallized, return false if so
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Tar Shot");
                    }
                    return new VoidReturn();
                }),
                // TODO: onEffectiveness - increase Fire-type effectiveness
            },
            [ConditionId.ThroatChop] = new()
            {
                Id = ConditionId.ThroatChop,
                Name = "Throat Chop",
                EffectType = EffectType.Condition,
                Duration = 2,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "Throat Chop", "[silent]");
                    }
                    return new VoidReturn();
                }),
                OnDisableMove = new OnDisableMoveEventInfo((battle, pokemon) =>
                {
                    // TODO: disable sound moves
                    foreach (var moveSlot in pokemon.MoveSlots)
                    {
                        var move = _library.Moves[moveSlot.Id];
                        if (move.Flags.Sound == true)
                        {
                            pokemon.DisableMove(moveSlot.Id);
                        }
                    }
                }),
                // TODO: onBeforeMove - prevent sound moves
                //OnResidualOrder = 22,
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "Throat Chop", "[silent]");
                    }
                }),
            },
            [ConditionId.Trapper] = new()
            {
                Id = ConditionId.Trapper,
                Name = "Trapper",
                EffectType = EffectType.Condition,
                NoCopy = true,
                // This condition has no event handlers - it's just a marker
            },
            [ConditionId.TwoTurnMove] = new()
            {
                Id = ConditionId.TwoTurnMove,
                Name = "Two Turn Move",
                EffectType = EffectType.Condition,
                // Skull Bash, SolarBeam, Sky Drop, etc.
                Duration = 2,
                OnStart = new OnStartEventInfo((battle, attacker, defender, effect) =>
                {
                    // attacker is the Pokemon using the two turn move and the Pokemon this condition is being applied to
                    if (effect is Move move)
                    {
                        battle.EffectState.Move = move.Id;

                        // Add the move-specific volatile (e.g., 'fly', 'dig', 'dive')
                        // TODO: Need proper mapping from MoveId to ConditionId for two-turn moves
                        // For now, try to find a condition with matching name
                        var moveConditionId = Enum.TryParse<ConditionId>(move.Id.ToString(), out var cid) ? cid : (ConditionId?)null;
                        if (moveConditionId.HasValue && _library.Conditions.ContainsKey(moveConditionId.Value))
                        {
                            attacker.AddVolatile(moveConditionId.Value);
                        }
                    }

                    // TODO: Handle lastMoveTargetLoc for targeting
                    // lastMoveTargetLoc is the location of the originally targeted slot before any redirection
                    // note that this is not updated for moves called by other moves (e.g., Metronome calling Dig)

                    // TODO: If move was called by another move (like Metronome), determine random target
                    // and store in volatiles[effect.id].targetLoc

                    battle.AttrLastMove("[still]");

                    // Run side-effects normally associated with hitting (e.g., Protean, Libero)
                    battle.RunEvent(EventId.PrepareHit, attacker, defender, effect);

                    return BoolVoidUnion.FromVoid();
                }),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.EffectState.Move != null)
                    {
                        target.DeleteVolatile((ConditionId)battle.EffectState.Move);
                    }
                }),
                OnLockMove = new OnLockMoveEventInfo((Func<Battle, Pokemon, MoveIdVoidUnion>)((battle, pokemon) =>
                {
                    if (battle.EffectState.Move.HasValue)
                    {
                        return battle.EffectState.Move.Value;
                    }
                    return MoveIdVoidUnion.FromVoid();
                })),
                OnMoveAborted = new OnMoveAbortedEventInfo((battle, pokemon, _, _) =>
                {
                    pokemon.DeleteVolatile(ConditionId.TwoTurnMove);
                }),
            },
            [ConditionId.Spikes] = new()
            {
                Id = ConditionId.Spikes,
                Name = "Spikes",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Spikes,
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "Spikes");
                    }
                    battle.EffectState.Layers = 1;
                }),
                OnSideRestart = new OnSideRestartEventInfo((battle, side, _, _) =>
                {
                    if ((battle.EffectState.Layers ?? 0) >= 3) return;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "Spikes");
                    }
                    battle.EffectState.Layers = (battle.EffectState.Layers ?? 0) + 1;
                }),
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    if (pokemon.IsGrounded() != true || pokemon.HasItem(ItemId.HeavyDutyBoots)) return;
                    int[] damageAmounts = { 0, 3, 4, 6 }; // 1/8, 1/6, 1/4
                    int layers = battle.EffectState.Layers ?? 0;
                    battle.Damage(damageAmounts[layers] * pokemon.MaxHp / 24, pokemon, null);
                }),
            },
            [ConditionId.SpikyShield] = new()
            {
                Id = ConditionId.SpikyShield,
                Name = "Spiky Shield",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.SpikyShield,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "move: Protect");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                    OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                    {
                        if (!(move.Flags.Protect ?? false))
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
                        // TODO: Check if move makes contact and damage source
                        // if (this.checkMoveMakesContact(move, source, target)) {
                        //     this.damage(source.baseMaxhp / 8, source, target);
                        // }
                        // return this.NOT_FAIL;
                        return BoolIntEmptyVoidUnion.FromBool(false);
                    }, 3),
                },
            [ConditionId.StealthRock] = new()
            {
                Id = ConditionId.StealthRock,
                Name = "Stealth Rock",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.StealthRock,
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "move: Stealth Rock");
                    }
                }),
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    if (pokemon.HasItem(ItemId.HeavyDutyBoots)) return;
                    // TODO: Calculate type effectiveness for Stealth Rock
                    // const typeMod = this.clampIntRange(pokemon.runEffectiveness(this.dex.getActiveMove('stealthrock')), -6, 6);
                    // this.damage(pokemon.maxhp * (2 ** typeMod) / 8);

                    // For now, just apply 1/8 damage (neutral effectiveness)
                    battle.Damage(pokemon.MaxHp / 8, pokemon, null);
                }),
            },
            [ConditionId.StickyWeb] = new()
            {
                Id = ConditionId.StickyWeb,
                Name = "Sticky Web",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.StickyWeb,
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "move: Sticky Web");
                    }
                }),
                    OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                    {
                        if (pokemon.IsGrounded() != true || pokemon.HasItem(ItemId.HeavyDutyBoots)) return;
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "move: Sticky Web");
                        }
                        // TODO: Get opponent Pokemon for source parameter
                        battle.Boost(new SparseBoostsTable { Spe = -1 }, pokemon, pokemon);
                    }),
                },
            [ConditionId.StockpileStorage] = new()
            {
                Id = ConditionId.StockpileStorage,
                Name = "Stockpile",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Stockpile,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    battle.EffectState.Layers = 1;
                    battle.EffectState.Def = 0;
                    battle.EffectState.Spd = 0;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, $"stockpile{battle.EffectState.Layers}");
                    }
                    var curDef = target.Boosts.GetBoost(BoostId.Def);
                    var curSpD = target.Boosts.GetBoost(BoostId.SpD);
                    battle.Boost(new SparseBoostsTable
                    {
                        Def = 1,
                        SpD = 1
                    }, target, target);
                    if (curDef != target.Boosts.GetBoost(BoostId.Def))
                        battle.EffectState.Def = (battle.EffectState.Def ?? 0) - 1;
                    if (curSpD != target.Boosts.GetBoost(BoostId.SpD))
                        battle.EffectState.Spd = (battle.EffectState.Spd ?? 0) - 1;
                    return BoolVoidUnion.FromVoid();
                }),
                OnRestart = new OnRestartEventInfo((battle, target, _, _) =>
                {
                    if ((battle.EffectState.Layers ?? 0) >= 3) return BoolVoidUnion.FromBool(false);
                    battle.EffectState.Layers = (battle.EffectState.Layers ?? 0) + 1;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, $"stockpile{battle.EffectState.Layers}");
                    }
                    var curDef = target.Boosts.GetBoost(BoostId.Def);
                    var curSpD = target.Boosts.GetBoost(BoostId.SpD);
                    battle.Boost(new SparseBoostsTable
                    {
                        Def = 1,
                        SpD = 1
                    }, target, target);
                    if (curDef != target.Boosts.GetBoost(BoostId.Def))
                        battle.EffectState.Def = (battle.EffectState.Def ?? 0) - 1;
                    if (curSpD != target.Boosts.GetBoost(BoostId.SpD))
                        battle.EffectState.Spd = (battle.EffectState.Spd ?? 0) - 1;
                    return BoolVoidUnion.FromVoid();
                }),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if ((battle.EffectState.Def ?? 0) != 0 || (battle.EffectState.Spd ?? 0) != 0)
                    {
                        var boosts = new SparseBoostsTable();
                        if ((battle.EffectState.Def ?? 0) != 0)
                            boosts.Def = battle.EffectState.Def ?? 0;
                        if ((battle.EffectState.Spd ?? 0) != 0)
                            boosts.SpD = battle.EffectState.Spd ?? 0;
                        battle.Boost(boosts, target, target);
                    }
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "Stockpile");
                    }
                }),
            },
        };
    }
}