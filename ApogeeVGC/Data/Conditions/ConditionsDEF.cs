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

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsDef()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.DragonCheer] = new()
            {
                Id = ConditionId.DragonCheer,
                Name = "Dragon Cheer",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.DragonCheer,
                OnStart = new OnStartEventInfo((battle, target, _, effect) =>
                {
                    // Check for FocusEnergy to prevent stacking
                    if (target.Volatiles.ContainsKey(ConditionId.FocusEnergy))
                    {
                        return BoolVoidUnion.FromBool(false);
                    }

                    // Check if this is from copying abilities (Costar, Imposter, Psych Up, Transform)
                    // These are moves/abilities that copy stat changes, so we show silently
                    bool isCopied =
                        effect is Ability { Id: AbilityId.Costar or AbilityId.Imposter } ||
                        effect is Move { Name: "Psych Up" or "Transform" };
                    if (isCopied)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-start", target, "move: Dragon Cheer", "[silent]");
                        }
                    }
                    else
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-start", target, "move: Dragon Cheer");
                        }
                    }

                    // Store whether the target has Dragon type at start
                    // This doesn't change if Pokemon Terastallizes
                    battle.EffectState.HasDragonType = target.HasType(PokemonType.Dragon);
                    return BoolVoidUnion.FromVoid();
                }),
                OnModifyCritRatio = new OnModifyCritRatioEventInfo((battle, critRatio, _, _, _) =>
                {
                    // +2 crit ratio if Dragon type, +1 otherwise
                    bool hasDragonType = battle.EffectState.HasDragonType ?? false;
                    return critRatio + (hasDragonType ? 2 : 1);
                }),
            },
            [ConditionId.ElectricTerrain] = new()
            {
                Id = ConditionId.ElectricTerrain,
                Name = "Electric Terrain",
                AssociatedMove = MoveId.ElectricTerrain,
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
                AssociatedMove = MoveId.DestinyBond,
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
                    if (effect.EffectType == EffectType.Move &&
                        effect is not Move { Flags.FutureMove: true })
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
                    // The target hasn't taken its turn, or Cursed Body activated and the move
                    // was not used through Dancer or Instruct
                    if (battle.Queue.WillMove(pokemon) != null ||
                        (pokemon == battle.ActivePokemon && battle.ActiveMove != null &&
                         !(battle.ActiveMove.IsExternal ?? false)))
                    {
                        pokemon.Volatiles[ConditionId.Disable].Duration--;
                    }

                    if (pokemon.LastMove == null)
                    {
                        battle.Debug("Pokemon hasn't moved yet");
                        return BoolVoidUnion.FromBool(false);
                    }

                    // Check if the last move has PP
                    if (pokemon.MoveSlots.Where(moveSlot => moveSlot.Id == pokemon.LastMove.Id)
                        .Any(moveSlot => moveSlot.Pp <= 0))
                    {
                        battle.Debug("Move out of PP");
                        return BoolVoidUnion.FromBool(false);
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
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, attacker, _, move) =>
                {
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
                    foreach (MoveSlot moveSlot in pokemon.MoveSlots)
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
                OnImmunity = new OnImmunityEventInfo((_, _, _) => new VoidReturn()),
                OnInvulnerability = new OnInvulnerabilityEventInfo((_, _, _, move) =>
                {
                    if (move.Id is MoveId.Earthquake or MoveId.Magnitude)
                    {
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    return BoolIntEmptyVoidUnion.FromBool(false);
                }),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, _, _, move) =>
                    {
                        if (move.Id is MoveId.Earthquake or MoveId.Magnitude)
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
                OnImmunity = new OnImmunityEventInfo((_, _, _) => new VoidReturn()),
                OnInvulnerability = new OnInvulnerabilityEventInfo((_, _, _, move) =>
                {
                    if (move.Id is MoveId.Surf or MoveId.Whirlpool)
                    {
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    return BoolIntEmptyVoidUnion.FromBool(false);
                }),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, _, _, move) =>
                    {
                        if (move.Id is MoveId.Surf or MoveId.Whirlpool)
                        {
                            return battle.ChainModify(2);
                        }

                        return damage;
                    }),
            },
            [ConditionId.ElectroShot] = new()
            {
                Id = ConditionId.ElectroShot,
                Name = "Electro Shot",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.ElectroShot,
                Duration = 2,
                // ElectroShot is a charging move without invulnerability
                // The SpA boost is handled by the move itself
            },
            [ConditionId.FreezeShock] = new()
            {
                Id = ConditionId.FreezeShock,
                Name = "Freeze Shock",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.FreezeShock,
                Duration = 2,
                // FreezeShock is a charging move without invulnerability
            },
            [ConditionId.Fly] = new()
            {
                Id = ConditionId.Fly,
                Name = "Fly",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Fly,
                Duration = 2,
                OnImmunity = new OnImmunityEventInfo((_, _, _) => new VoidReturn()),
                OnInvulnerability = new OnInvulnerabilityEventInfo((_, _, _, move) =>
                {
                    if (move.Id is MoveId.Gust or MoveId.Twister or MoveId.SkyUppercut
                        or MoveId.Thunder or MoveId.Hurricane or MoveId.SmackDown
                        or MoveId.ThousandArrows)
                    {
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    return BoolIntEmptyVoidUnion.FromBool(false);
                }),
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, _, _, move) =>
                    {
                        if (move.Id is MoveId.Gust or MoveId.Twister)
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
                OnStart = new OnStartEventInfo((battle, pokemon, _, effect) =>
                {
                    // Check for DragonCheer to prevent stacking
                    if (pokemon.Volatiles.ContainsKey(ConditionId.DragonCheer))
                    {
                        return BoolVoidUnion.FromBool(false);
                    }

                    // Check if this is from copying abilities (Costar, Imposter, Psych Up, Transform)
                    bool isCopied =
                        effect is Ability { Id: AbilityId.Costar or AbilityId.Imposter } ||
                        effect is Move { Name: "Psych Up" or "Transform" };
                    if (battle.DisplayUi)
                    {
                        if (isCopied)
                        {
                            battle.Add("-start", pokemon, "move: Focus Energy", "[silent]");
                        }
                        else
                        {
                            battle.Add("-start", pokemon, "move: Focus Energy");
                        }
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnModifyCritRatio =
                    new OnModifyCritRatioEventInfo((_, critRatio, _, _, _) => critRatio + 2),
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
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.Volatiles.TryGetValue(ConditionId.FocusPunch,
                            out EffectState? state) &&
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
                OnDamagingHit = new OnDamagingHitEventInfo((_, _, target, _, _) =>
                {
                    if (target.Volatiles.TryGetValue(ConditionId.FocusPunch,
                            out EffectState? @volatile))
                    {
                        @volatile.LostFocus = true;
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
                    Item item = pokemon.GetItem();
                    pokemon.SetItem(ItemId.None);
                    pokemon.LastItem = item.Id;
                    pokemon.UsedItemThisTurn = true;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-enditem", pokemon, item.Name, "[from] move: Fling");
                    }

                    battle.RunEvent(EventId.AfterUseItem, pokemon, null, null, item);
                    pokemon.RemoveVolatile(_library.Conditions[ConditionId.Fling]);
                }),
            },
            [ConditionId.FairyLock] = new()
            {
                Id = ConditionId.FairyLock,
                Name = "Fairy Lock",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.FairyLock,
                Duration = 2,
                OnFieldStart = new OnFieldStartEventInfo((battle, _, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldactivate", "move: Fairy Lock");
                    }
                }),
                OnTrapPokemon = new OnTrapPokemonEventInfo((_, pokemon) => { pokemon.TryTrap(); }),
            },
            [ConditionId.Fainted] = new()
            {
                Id = ConditionId.Fainted,
                Name = "Fainted",
                EffectType = EffectType.Condition,
                // Marker condition for fainted Pokemon
                // The actual fainted state is tracked by Pokemon.Fainted property
            },
            [ConditionId.FollowMe] = new()
            {
                Id = ConditionId.FollowMe,
                Name = "Follow Me",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.FollowMe,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "move: Follow Me");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnFoeRedirectTarget = new OnFoeRedirectTargetEventInfo(
                    (battle, _, source, _, move) =>
                    {
                        // Get the Pokemon that used Follow Me from the effect state
                        EffectStateTarget? effectTarget = battle.EffectState.Target;
                        Pokemon? followMeTarget =
                            effectTarget is PokemonEffectStateTarget pokemonTarget
                                ? pokemonTarget.Pokemon
                                : null;

                        if (followMeTarget == null) return PokemonVoidUnion.FromVoid();

                        // Check if the Follow Me user is a valid target for this move
                        if (battle.ValidTarget(followMeTarget, source, move.Target))
                        {
                            if (move.SmartTarget ?? false)
                            {
                                move.SmartTarget = false;
                            }

                            if (battle.DisplayUi)
                            {
                                battle.Debug("Follow Me redirected target of move");
                            }

                            return followMeTarget; // Uses implicit conversion
                        }

                        return PokemonVoidUnion.FromVoid();
                    }, 1),
            },
            [ConditionId.FlashFire] = new()
            {
                Id = ConditionId.FlashFire,
                Name = "Flash Fire",
                EffectType = EffectType.Condition,
                NoCopy = true,
                AssociatedAbility = AbilityId.FlashFire,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "ability: Flash Fire");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Fire && attacker.HasAbility(AbilityId.FlashFire))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("Flash Fire boost");
                        }

                        battle.ChainModify(1.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Fire && attacker.HasAbility(AbilityId.FlashFire))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("Flash Fire boost");
                        }

                        battle.ChainModify(1.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "ability: Flash Fire", "[silent]");
                    }
                }),
            },
            [ConditionId.FirePledge] = new()
            {
                Id = ConditionId.FirePledge,
                Name = "Fire Pledge",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.FirePledge,
                Duration = 4,
                OnSideStart = new OnSideStartEventInfo((battle, targetSide, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", targetSide, "Fire Pledge");
                    }
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (!pokemon.HasType(PokemonType.Fire))
                    {
                        battle.Damage(pokemon.BaseMaxHp / 8, pokemon, pokemon);
                    }
                }, 5, 1),
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 8,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, targetSide) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", targetSide, "Fire Pledge");
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
                OnStart = new OnStartEventInfo((battle, _, _, _) =>
                {
                    battle.EffectState.Multiplier = 1;
                    return BoolVoidUnion.FromVoid();
                }),
                OnRestart = new OnRestartEventInfo((battle, _, _, _) =>
                {
                    if ((battle.EffectState.Multiplier ?? 1) < 4)
                    {
                        // Double the multiplier (1 -> 2 -> 4)
                        battle.EffectState.Multiplier = (battle.EffectState.Multiplier ?? 1) * 2;
                    }

                    battle.EffectState.Duration = 2;
                    return BoolVoidUnion.FromVoid();
                }),
                // Note: Base power scaling is handled in the move's BasePowerCallback
                // The condition just tracks the multiplier (1, 2, 4)
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
            [ConditionId.Encore] = new()
            {
                Id = ConditionId.Encore,
                Name = "Encore",
                AssociatedMove = MoveId.Encore,
                EffectType = EffectType.Condition,
                Duration = 3,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    // Check if target has a valid last move for Encore
                    if (target.LastMove == null)
                    {
                        battle.Debug("Encore failed: no last move");
                        return BoolVoidUnion.FromBool(false);
                    }

                    // Check if the last move has PP remaining
                    MoveSlot? moveSlot = null;
                    foreach (MoveSlot slot in target.MoveSlots)
                    {
                        if (slot.Id == target.LastMove.Id)
                        {
                            moveSlot = slot;
                            break;
                        }
                    }

                    if (moveSlot == null || moveSlot.Pp <= 0)
                    {
                        battle.Debug("Encore failed: move not found or no PP");
                        return BoolVoidUnion.FromBool(false);
                    }

                    // Store the encored move
                    battle.EffectState.Move = target.LastMove.Id;

                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "Encore");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnLockMove = new OnLockMoveEventInfo(
                    new OnLockMoveFunc((battle, _) =>
                    {
                        // Return the encored move ID to force the Pokemon to use it
                        var encoredMove = battle.EffectState.Move;
                        if (encoredMove.HasValue)
                        {
                            // Use implicit conversion from MoveId to MoveIdVoidUnion
                            return encoredMove.Value;
                        }

                        return MoveIdVoidUnion.FromVoid();
                    })),
                OnDisableMove = new OnDisableMoveEventInfo((battle, pokemon) =>
                {
                    var encoredMove = battle.EffectState.Move;
                    if (encoredMove == null) return;

                    foreach (MoveSlot moveSlot in pokemon.MoveSlots)
                    {
                        if (moveSlot.Id != encoredMove)
                        {
                            pokemon.DisableMove(moveSlot.Id);
                        }
                    }
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    // Check if encored move has PP remaining
                    var encoredMove = battle.EffectState.Move;
                    if (encoredMove == null) return;

                    foreach (MoveSlot moveSlot in pokemon.MoveSlots)
                    {
                        if (moveSlot.Id == encoredMove && moveSlot.Pp <= 0)
                        {
                            // End Encore if the encored move has no PP
                            pokemon.RemoveVolatile(_library.Conditions[ConditionId.Encore]);
                            return;
                        }
                    }
                }, 14),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "Encore");
                    }
                }),
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
                OnFieldRestart = new OnFieldRestartEventInfo((battle, _, _, _) =>
                {
                    if (battle.EffectState.Duration != 2)
                    {
                        battle.EffectState.Duration = 2;
                        if ((battle.EffectState.Multiplier ?? 1) < 5)
                        {
                            battle.EffectState.Multiplier =
                                (battle.EffectState.Multiplier ?? 1) + 1;
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
                AssociatedMove = MoveId.Endure,
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
                OnDamage = new OnDamageEventInfo((battle, damage, target, _, effect) =>
                {
                    if (effect?.EffectType == EffectType.Move && damage >= target.Hp)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Endure");
                        }

                        return IntBoolVoidUnion.FromInt(target.Hp - 1);
                    }

                    return IntBoolVoidUnion.FromVoid();
                }, -10),
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
                        battle.Hint(
                            "In Gen 8+, Future attacks will never resolve when used on the 255th turn or later.");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, target, _, _) =>
                {
                    if (battle.GetOverflowedTurnCount() < battle.EffectState.EndingTurn) return;
                    Pokemon? slotTarget = battle.GetAtSlot(battle.EffectState.TargetSlot);
                    if (slotTarget != null)
                    {
                        target.Side.RemoveSlotCondition(slotTarget,
                            _library.Conditions[ConditionId.FutureMove]);
                    }
                }, 3),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    // Get move data from effectState
                    Move? moveData = battle.EffectState.MoveData;
                    Pokemon? source = battle.EffectState.Source;

                    if (moveData == null || source == null)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Hint(
                                $"{moveData?.Name ?? "Future Move"} did not hit because move data or source is missing.");
                        }

                        return;
                    }

                    // Check if target is fainted or is the source
                    if (target.Fainted || target == source)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Hint(
                                $"{moveData.Name} did not hit because the target is {(target.Fainted ? "fainted" : "the user")}.");
                        }

                        return;
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, $"move: {moveData.Name}");
                    }

                    // Remove Protect/Endure volatiles
                    target.RemoveVolatile(_library.Conditions[ConditionId.Protect]);
                    target.RemoveVolatile(_library.Conditions[ConditionId.Endure]);

                    // Convert to ActiveMove - need to create a dummy MoveSlot
                    // Future moves don't actually use a slot since they're delayed
                    MoveSlot dummySlot = new()
                    {
                        Id = moveData.Id,
                        Pp = 1,
                        MaxPp = 1,
                        Target = moveData.Target,
                        Disabled = false,
                        DisabledSource = null,
                        Used = false,
                        Virtual = true,
                    };

                    ActiveMove hitMove = moveData.ToActiveMove() with
                    {
                        MoveSlot = dummySlot,
                        Flags = moveData.Flags with { FutureMove = true },
                    };

                    // Apply Infiltrator ability if source has it (Gen 6+)
                    if (source.HasAbility(AbilityId.Infiltrator))
                    {
                        hitMove.Infiltrates = true;
                    }

                    // Apply Normalize ability if source has it (Gen 6+)
                    if (source.HasAbility(AbilityId.Normalize))
                    {
                        hitMove.Type = MoveType.Normal;
                    }

                    // Execute the move with TrySpreadMoveHit
                    battle.Actions.TrySpreadMoveHit([target], source, hitMove, true);

                    // Trigger Life Orb recoil if applicable (Gen 5+)
                    if (source.IsActive && source.HasItem(ItemId.LifeOrb))
                    {
                        battle.RunEvent(EventId.AfterMoveSecondarySelf, source,
                            RunEventSource.FromNullablePokemon(target), source.GetItem());
                    }

                    battle.ActiveMove = null;

                    battle.CheckWin();
                }),
            },
        };
    }
}