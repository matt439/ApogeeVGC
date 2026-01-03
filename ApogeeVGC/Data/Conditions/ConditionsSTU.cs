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
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
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
                    [ConditionId.ShadowForce] = new()
            {
                Id = ConditionId.ShadowForce,
                Name = "Shadow Force",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.ShadowForce,
                Duration = 2,
                OnInvulnerability = new OnInvulnerabilityEventInfo((_, _, _, _) =>
                    BoolIntEmptyVoidUnion.FromBool(false)),
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

                    var type = PokemonType.Normal;
                    if (pokemon.Ability == AbilityId.RksSystem)
                    {
                        Item item = pokemon.GetItem();
                        type = item.OnMemory?.ConvertToPokemonType() ?? PokemonType.Normal;
                    }

                    return new[] { type };
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
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    if (move.SmartTarget == true)
                    {
                        move.SmartTarget = false;
                    }
                    else
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Protect");
                        }
                    }

                    // Check for lockedmove volatile and reset Outrage counter
                    if (source.Volatiles.TryGetValue(ConditionId.LockedMove,
                            out EffectState? lockedMove))
                    {
                        // Outrage counter is reset
                        if (lockedMove.Duration == 2)
                        {
                            source.DeleteVolatile(ConditionId.LockedMove);
                        }
                    }

                    // Check if move makes contact and lower Speed
                    if (move.Flags.Contact == true)
                    {
                        battle.Boost(new SparseBoostsTable { Spe = -1 }, source, target,
                            (IEffect?)_library.Moves[MoveId.SilkTrap]);
                    }

                    return BoolIntEmptyVoidUnion.FromBool(false);
                }, 3),
            },
            [ConditionId.Snatch] = new()
            {
                Id = ConditionId.Snatch,
                AssociatedMove = MoveId.Snatch,
            },
            [ConditionId.SupercellSlam] = new()
            {
                Id = ConditionId.SupercellSlam,
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

                    if (pokemon.Volatiles.TryGetValue(ConditionId.Stall,
                            out EffectState? stallState))
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
                    if (battle.EffectState.Source is { IsActive: false })
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
                OnStart = new OnStartEventInfo((battle, target, _, effect) =>
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
                    if (target.Volatiles.TryGetValue(ConditionId.PartiallyTrapped,
                            out EffectState? @volatile))
                    {
                        IEffect? sourceEffect =
                            @volatile.SourceEffect;
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
                    // Bypass substitute if hitting self, move bypasses substitute, or move infiltrates
                    if (target == source || (move.Flags.BypassSub ?? false) ||
                        (move.Infiltrates ?? false))
                    {
                        return IntBoolVoidUnion.FromVoid();
                    }

                    // Calculate damage using the battle's GetDamage method
                    IntUndefinedFalseUnion damageResult =
                        battle.Actions.GetDamage(source, target, move);

                    // If damage calculation failed or returned false, fail the move
                    if (damageResult is FalseIntUndefinedFalseUnion)
                    {
                        return IntBoolVoidUnion.FromVoid();
                    }

                    // Get the actual damage value
                    int damage = damageResult is IntIntUndefinedFalseUnion intDmg
                        ? intDmg.Value
                        : 0;

                    // If no damage, let the move continue (might still have secondary effects)
                    if (damage == 0)
                    {
                        return IntBoolVoidUnion.FromVoid();
                    }

                    // Cap damage at substitute's remaining HP
                    int subHp = battle.EffectState.Hp ?? 0;
                    damage = Math.Min(damage, subHp);

                    // Subtract damage from substitute's HP
                    battle.EffectState.Hp = subHp - damage;

                    // Check if substitute broke
                    if ((battle.EffectState.Hp ?? 0) <= 0)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-end", target, "Substitute");
                        }

                        target.RemoveVolatile(_library.Conditions[ConditionId.Substitute]);
                    }
                    else
                    {
                        // Substitute still active, show activate message
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Substitute",
                                "[damage]");
                        }
                    }

                    // Handle recoil damage if move has recoil
                    if (move.Recoil != null)
                    {
                        int recoilDamage = battle.Actions.CalcRecoilDamage(damage, move, source);
                        battle.Damage(recoilDamage, source, source,
                            BattleDamageEffect.FromIEffect(
                                _library.Conditions[ConditionId.Recoil]));
                    }

                    // Handle drain/heal if move has drain
                    if (move.Drain != null)
                    {
                        (int numerator, int denominator) = move.Drain.Value;
                        int drainAmount =
                            battle.ClampIntRange(
                                (int)Math.Ceiling(damage * numerator / (double)denominator), 1,
                                null);
                        battle.Heal(drainAmount, source, target,
                            BattleHealEffect.FromDrain());
                    }

                    // Trigger OnAfterSubDamage event for the move
                    if (move.OnAfterSubDamage != null)
                    {
                        var handler =
                            (Action<Battle, int, Pokemon, Pokemon, ActiveMove>)move
                                .OnAfterSubDamage.GetDelegateOrThrow();
                        handler(battle, damage, target, source, move);
                    }

                    // Trigger AfterSubDamage events for target's ability, item, and volatiles
                    battle.RunEvent(EventId.AfterSubDamage,
                        target, // Implicitly converts to RunEventTarget via operator
                        source, // Implicitly converts to RunEventSource via operator
                        move,
                        new IntRelayVar(damage));

                    // Return HIT_SUBSTITUTE constant (represented as int value 0)
                    // This tells the battle engine that the substitute took the hit
                    return IntBoolVoidUnion.FromInt(0);
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
                    string[] immuneSpecies =
                        ["Diglett", "Dugtrio", "Palossand", "Sandygast", "Gengar-Mega"];
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
                OnAccuracy = new OnAccuracyEventInfo((_, _, _, _, move) =>
                {
                    // Telekinesis grants perfect accuracy on non-OHKO moves
                    if (move.Ohko == null)
                    {
                        return IntBoolVoidUnion.FromBool(true);
                    }

                    return IntBoolVoidUnion.FromVoid();
                }, -1),
                OnImmunity = new OnImmunityEventInfo((_, _, _) => new VoidReturn()),
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
                    // If target has already taken its turn and won't move again this turn, increase duration by 1
                    // This matches PS implementation: if (!this.queue.willMove(target) && target.activeTurns) this.effectState.duration++;
                    if (target.ActiveTurns > 0 && battle.Queue.WillMove(target) == null)
                    {
                        battle.EffectState.Duration = 4;
                    }

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
                OnDisableMove = new OnDisableMoveEventInfo((_, pokemon) =>
                {
                    foreach (MoveSlot moveSlot in from moveSlot in pokemon.MoveSlots
                             let move = _library.Moves[moveSlot.Id]
                             where move.Category == MoveCategory.Status && move.Id != MoveId.MeFirst
                             select moveSlot)
                    {
                        pokemon.DisableMove(moveSlot.Id);
                    }
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, attacker, _, move) =>
                {
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
                AssociatedMove = MoveId.SparklingAria,
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
                    bool applies = (pokemon.HasType(PokemonType.Flying) ||
                                    pokemon.HasAbility(AbilityId.Levitate));

                    if (pokemon.HasItem(ItemId.IronBall) ||
                        pokemon.Volatiles.ContainsKey(ConditionId.Ingrain) ||
                        battle.Field.PseudoWeather.ContainsKey(ConditionId.Gravity))
                    {
                        applies = false;
                    }

                    if (pokemon.RemoveVolatile(_library.Conditions[ConditionId.Fly]) ||
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Bounce]))
                    {
                        applies = true;
                        battle.Queue.CancelMove(pokemon);
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
                        battle.Queue.CancelMove(pokemon);
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
                OnDisableMove = new OnDisableMoveEventInfo((_, pokemon) =>
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
                            battle.Add("-sideend", pokemon.Side, "move: Toxic Spikes",
                                $"[of] {pokemon}");
                        }

                        pokemon.Side.RemoveSideCondition(ConditionId.ToxicSpikes);
                    }
                    else if (pokemon.HasType(PokemonType.Steel) ||
                             pokemon.HasItem(ItemId.HeavyDutyBoots))
                    {
                        // do nothing
                    }
                    else if ((battle.EffectState.Layers ?? 0) >= 2)
                    {
                        // Get the first active opponent Pokemon as the source
                        Pokemon? source = pokemon.Side.Foe.Active.FirstOrDefault(p => p != null);
                        pokemon.TrySetStatus(ConditionId.Toxic, source ?? pokemon);
                    }
                    else
                    {
                        // Get the first active opponent Pokemon as the source
                        Pokemon? source = pokemon.Side.Foe.Active.FirstOrDefault(p => p != null);
                        pokemon.TrySetStatus(ConditionId.Poison, source ?? pokemon);
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
                AssociatedMove = MoveId.TrickRoom,
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
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    if (pokemon.Item == ItemId.None && !pokemon.IgnoringAbility())
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spe);
                    }

                    return spe;
                }),
            },
            [ConditionId.Uproar] = new()
            {
                Id = ConditionId.Uproar,
                Name = "Uproar",
                AssociatedMove = MoveId.Uproar,
                EffectType = EffectType.Condition,
                Duration = 3,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Uproar");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, target, _, _) =>
                {
                    // Check if pokemon has ThroatChop volatile - if so, remove uproar
                    if (target.Volatiles.ContainsKey(ConditionId.ThroatChop))
                    {
                        target.RemoveVolatile(_library.Conditions[ConditionId.Uproar]);
                        return;
                    }

                    // Check if last move was Struggle - if so, don't lock (end the volatile)
                    if (target.LastMove is { Id: MoveId.Struggle })
                    {
                        target.DeleteVolatile(ConditionId.Uproar);
                        return;
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "Uproar", "[upkeep]");
                    }
                }, 28, 1),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Uproar");
                    }
                }),
                // Lock the Pokemon into using Uproar
                OnLockMove =
                    new OnLockMoveEventInfo(
                        (Func<Battle, Pokemon, MoveIdVoidUnion>)((_, _) => MoveId.Uproar)),
                // Prevent sleep on all Pokemon while Uproar is active
                OnAnySetStatus = new OnAnySetStatusEventInfo((battle, status, pokemon, _, _) =>
                {
                    if (status.Id == ConditionId.Sleep)
                    {
                        if (battle.DisplayUi)
                        {
                            if (pokemon == battle.EffectState.Target)
                            {
                                battle.Add("-fail", pokemon, "slp", "[from] Uproar", "[msg]");
                            }
                            else
                            {
                                battle.Add("-fail", pokemon, "slp", "[from] Uproar");
                            }
                        }

                        return BoolVoidUnion.FromBool(false);
                    }

                    return BoolVoidUnion.FromVoid();
                }),
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
                    int divisor = (pokemon.HasType(PokemonType.Water) ||
                                   pokemon.HasType(PokemonType.Steel))
                        ? 4
                        : 8;
                    battle.Damage(pokemon.BaseMaxHp / divisor, pokemon);
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
                    // Tar Shot fails if target is Terastallized
                    if (pokemon.Terastallized != null)
                    {
                        return BoolVoidUnion.FromBool(false);
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Tar Shot");
                    }

                    return new VoidReturn();
                }),
                OnEffectiveness = new OnEffectivenessEventInfo(
                    (_, typeMod, target, type, move) =>
                    {
                        // Increase Fire-type move effectiveness by 1 stage (2x multiplier)
                        if (move.Type != MoveType.Fire)
                        {
                            return IntVoidUnion.FromVoid();
                        }

                        if (target == null)
                        {
                            return IntVoidUnion.FromVoid();
                        }

                        // Only apply to the first type of the target
                        var targetTypes = target.GetTypes();
                        if (targetTypes.Length == 0 || type != targetTypes[0])
                        {
                            return IntVoidUnion.FromVoid();
                        }

                        return IntVoidUnion.FromInt(typeMod + 1);
                    }, -2),
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
                OnDisableMove = new OnDisableMoveEventInfo((_, pokemon) =>
                {
                    foreach (MoveSlot moveSlot in pokemon.MoveSlots)
                    {
                        Move move = _library.Moves[moveSlot.Id];
                        if (move.Flags.Sound == true)
                        {
                            pokemon.DisableMove(moveSlot.Id);
                        }
                    }
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, move) =>
                {
                    // Z-moves and Max moves are not affected by Throat Chop
                    // Note: isZ and isMax properties would need to be checked if available
                    if (move.Flags.Sound == true)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "move: Throat Chop");
                        }

                        return false;
                    }

                    return BoolVoidUnion.FromVoid();
                }, 6),
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
                        // Try to find a condition with matching name
                        var moveConditionId =
                            Enum.TryParse(move.Id.ToString(), out ConditionId cid)
                                ? cid
                                : (ConditionId?)null;
                        if (moveConditionId.HasValue &&
                            _library.Conditions.ContainsKey(moveConditionId.Value))
                        {
                            attacker.AddVolatile(moveConditionId.Value);

                            // Handle target location tracking
                            // lastMoveTargetLoc is the location of the originally targeted slot before any redirection
                            // note that this is not updated for moves called by other moves (e.g., Metronome calling Dig)
                            PokemonSlot? moveTargetLoc = attacker.LastMoveTargetLoc != null
                                ? new PokemonSlot(defender.Side.Id,
                                    attacker.LastMoveTargetLoc.Value)
                                : null;

                            if (effect is ActiveMove { SourceEffect: not null } &&
                                _library.Moves.TryGetValue(move.Id, out Move? moveData) &&
                                moveData.Target != MoveTarget.Self)
                            {
                                // This move was called by another move such as Metronome
                                // and needs a random target to be determined this turn
                                // it will already have one by now if there is any valid target
                                // but if there isn't one we need to choose a random slot now
                                Pokemon targetPokemon = defender;
                                if (defender.Fainted)
                                {
                                    // Get opponent Pokemon for random selection
                                    var foes = attacker.Side.Foe.Active
                                        .Where(p => p is { Fainted: false })
                                        .ToList();

                                    if (foes.Count == 0)
                                    {
                                        throw new InvalidOperationException(
                                            $"TwoTurnMove: Cannot find valid target for {move.Name} - all opponents are fainted");
                                    }

                                    targetPokemon = battle.Sample(foes) ??
                                                    throw new InvalidOperationException();
                                }

                                moveTargetLoc = targetPokemon.GetSlot();
                            }

                            // Store target location in the move-specific volatile state
                            if (moveTargetLoc != null &&
                                attacker.Volatiles.TryGetValue(moveConditionId.Value,
                                    out EffectState? volatileState))
                            {
                                volatileState.TargetSlot = moveTargetLoc;
                            }
                        }
                    }

                    // Note: AttrLastMove("[still]") from TS is not implemented yet
                    // This would add the [still] attribute to suppress move animation

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
                OnLockMove = new OnLockMoveEventInfo(
                    (Func<Battle, Pokemon, MoveIdVoidUnion>)((battle, _) =>
                    {
                        if (battle.EffectState.Move.HasValue)
                        {
                            return battle.EffectState.Move.Value;
                        }

                        return MoveIdVoidUnion.FromVoid();
                    })),
                OnMoveAborted = new OnMoveAbortedEventInfo((_, pokemon, _, _) =>
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
                    if (pokemon.IsGrounded() != true || pokemon.HasItem(ItemId.HeavyDutyBoots))
                        return;
                    int[] damageAmounts = [0, 3, 4, 6]; // 1/8, 1/6, 1/4
                    int layers = battle.EffectState.Layers ?? 0;
                    battle.Damage(damageAmounts[layers] * pokemon.MaxHp / 24, pokemon);
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
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    if (move.SmartTarget == true)
                    {
                        move.SmartTarget = false;
                    }
                    else
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Protect");
                        }
                    }

                    // Check for lockedmove volatile and reset Outrage counter
                    if (source.Volatiles.TryGetValue(ConditionId.LockedMove,
                            out EffectState? lockedMove))
                    {
                        // Outrage counter is reset
                        if (lockedMove.Duration == 2)
                        {
                            source.DeleteVolatile(ConditionId.LockedMove);
                        }
                    }

                    // Check if move makes contact and damage source
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        battle.Damage(source.BaseMaxHp / 8, source, target);
                    }

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

                    // Calculate type effectiveness for Rock-type move (Stealth Rock)
                    MoveEffectiveness effectiveness =
                        battle.Dex.GetEffectiveness(MoveType.Rock, pokemon);
                    int typeMod = effectiveness.ToModifier();

                    // Clamp the type modifier to a reasonable range (-6 to 6)
                    typeMod = battle.ClampIntRange(typeMod, -6, 6);

                    // Calculate damage: maxhp * (2 ** typeMod) / 8
                    // 2 ** typeMod represents the damage multiplier:
                    // -2 = 1/4x, -1 = 1/2x, 0 = 1x, 1 = 2x, 2 = 4x
                    int damage = (int)(pokemon.MaxHp * Math.Pow(2, typeMod) / 8);

                    battle.Damage(damage, pokemon);
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
                    if (pokemon.IsGrounded() != true || pokemon.HasItem(ItemId.HeavyDutyBoots))
                        return;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon, "move: Sticky Web");
                    }

                    // Get the first active opponent Pokemon as the source
                    Pokemon? source = pokemon.Side.Foe.Active.FirstOrDefault(p => p != null);
                    battle.Boost(new SparseBoostsTable { Spe = -1 }, pokemon, source,
                        (IEffect?)_library.Moves[MoveId.StickyWeb]);
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

                    int curDef = target.Boosts.GetBoost(BoostId.Def);
                    int curSpD = target.Boosts.GetBoost(BoostId.SpD);
                    battle.Boost(new SparseBoostsTable
                    {
                        Def = 1,
                        SpD = 1,
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

                    int curDef = target.Boosts.GetBoost(BoostId.Def);
                    int curSpD = target.Boosts.GetBoost(BoostId.SpD);
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