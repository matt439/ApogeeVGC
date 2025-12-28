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
using ApogeeVGC.Sim.SpeciesClasses;
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
                    if (type == ConditionId.Sandstorm || type == ConditionId.Hail)
                    {
                        return false;
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnInvulnerability = new OnInvulnerabilityEventInfo((battle, target, source, move) =>
                {
                    // Phantom Force can only be hit by certain moves
                    return false;
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
                    if (type == ConditionId.Sandstorm || type == ConditionId.Hail)
                    {
                        return false;
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnInvulnerability = new OnInvulnerabilityEventInfo((battle, target, source, move) =>
                {
                    // Shadow Force can only be hit by certain moves
                    return false;
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
                        (pokemon.Ability != AbilityId.RKSSystem && battle.Gen >= 8))
                    {
                        return types;
                    }

                    PokemonType type = PokemonType.Normal;
                    if (pokemon.Ability == AbilityId.RKSSystem)
                    {
                        var item = pokemon.GetItem();
                        // TODO: Get type from item.OnMemory property when implemented
                        // For now, default to Normal type
                        type = PokemonType.Normal;
                    }

                    return new PokemonType[] { type };
                }, 1),
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
                    if (effect?.Id == EffectId.ShedTail)
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
                        return BoolVoidUnion.FromVoid();
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

                    return BoolVoidUnion.FromVoid();
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
                    battle.EffectState.Move = effect?.Id;

                    // Add the move-specific volatile (e.g., 'fly', 'dig', 'dive')
                    if (effect?.Id != null)
                    {
                        attacker.AddVolatile(_library.Conditions[(ConditionId)effect.Id]);
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
                        target.RemoveVolatile(_library.Conditions[(ConditionId)battle.EffectState.Move]);
                    }
                }),
                OnLockMove = new OnLockMoveEventInfo((battle, pokemon) =>
                {
                    return battle.EffectState.Move;
                }),
                OnMoveAborted = new OnMoveAbortedEventInfo((battle, pokemon, _, _) =>
                {
                    pokemon.RemoveVolatile(_library.Conditions[ConditionId.TwoTurnMove]);
                }),
            },
        };
    }
}