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
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsGhi()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.GastroAcid] = new()
            {
                Id = ConditionId.GastroAcid,
                Name = "Gastro Acid",
                AssociatedMove = MoveId.GastroAcid,
                EffectType = EffectType.Condition,
                // Ability suppression is implemented in Pokemon.IgnoringAbility()
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    // Check if pokemon has Ability Shield
                    if (pokemon.HasItem(ItemId.AbilityShield))
                    {
                        return BoolVoidUnion.FromBool(false);
                    }
                    if (battle.DisplayUi)
                    {
                        battle.Add("-endability", pokemon);
                    }
                    // End the pokemon's current ability
                    var ability = pokemon.GetAbility();
                    battle.SingleEvent(EventId.End, ability, pokemon.AbilityState,
                        new PokemonSingleEventTarget(pokemon), new PokemonSingleEventSource(pokemon),
                        _library.Conditions[ConditionId.GastroAcid]);
                    return BoolVoidUnion.FromVoid();
                }),
                OnCopy = new OnCopyEventInfo((battle, pokemon) =>
                {
                    // Remove Gastro Acid if the pokemon has an ability with cantsuppress flag
                    var ability = pokemon.GetAbility();
                    if (ability.Flags.CantSuppress ?? false)
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.GastroAcid]);
                    }
                }),
            },
            [ConditionId.IceBall] = new()
            {
                Id = ConditionId.IceBall,
                Name = "Ice Ball",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.IceBall,
                // Ice Ball uses LockedMove for the locking behavior and RolloutStorage for damage scaling
                // This is just a marker condition
            },
            [ConditionId.Gem] = new()
            {
                Id = ConditionId.Gem,
                Name = "Gem",
                EffectType = EffectType.Condition,
                Duration = 1,
                AffectsFainted = true,
                //OnBasePowerPriority = 14,
                OnBasePower = new OnBasePowerEventInfo((battle, _, _, _, _) =>
                    {
                        battle.Debug("Gem Boost");
                        return battle.ChainModify([5325, 4096]);
                    },
                    14),
            },
            [ConditionId.GlaiveRush] = new()
            {
                Id = ConditionId.GlaiveRush,
                Name = "Glaive Rush",
                AssociatedMove = MoveId.GlaiveRush,
                EffectType = EffectType.Condition,
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singlemove", pokemon, "Glaive Rush", "[silent]");
                    }

                    return new VoidReturn();
                }),
                // Moves always hit the Glaive Rush user
                OnAccuracy = new OnAccuracyEventInfo((_, _, _, _, _) =>
                {
                    return IntBoolVoidUnion.FromBool(true);
                }),
                // Damage is doubled against the Glaive Rush user
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, _, _, _, _) =>
                {
                    return battle.ChainModify(2);
                }),
                // Remove the Glaive Rush drawback before the pokemon attacks
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Debug("removing Glaive Rush drawback before attack");
                    pokemon.RemoveVolatile(_library.Conditions[ConditionId.GlaiveRush]);
                    return BoolVoidUnion.FromVoid();
                }, 100),
            },
            [ConditionId.GrassPledge] = new()
            {
                Id = ConditionId.GrassPledge,
                Name = "Grass Pledge",
                AssociatedMove = MoveId.GrassPledge,
                EffectType = EffectType.Condition,
                Duration = 4,
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "Grass Pledge");
                    }
                }),
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, _) =>
                {
                    // Quarter speed (0.25x)
                    return (int)Math.Floor(spe * 0.25);
                }),
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) =>
                {
                    // Duration handled automatically
                }, 26, 9),
                OnSideEnd = new OnSideEndEventInfo((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "Grass Pledge");
                    }
                }),
            },
            [ConditionId.Gravity] = new()
            {
                Id = ConditionId.Gravity,
                Name = "Gravity",
                AssociatedMove = MoveId.Gravity,
                EffectType = EffectType.Condition,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((battle, source, _, _) =>
                {
                    if (source != null && source.HasAbility(AbilityId.Persistent))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", source, "ability: Persistent", "[move] Gravity");
                        }
                        return 7;
                    }
                    return 5;
                }),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, _) =>
                {
                    if (source != null && source.HasAbility(AbilityId.Persistent))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-fieldstart", "move: Gravity", "[persistent]");
                        }
                    }
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-fieldstart", "move: Gravity");
                    }

                    // Remove airborne-related volatiles from all active Pokemon
                    foreach (var pokemon in battle.GetAllActive())
                    {
                        bool applies = false;
                        if (pokemon.RemoveVolatile(_library.Conditions[ConditionId.Bounce]) ||
                            pokemon.RemoveVolatile(_library.Conditions[ConditionId.Fly]))
                        {
                            applies = true;
                            battle.Queue.CancelMove(pokemon);
                            pokemon.RemoveVolatile(_library.Conditions[ConditionId.TwoTurnMove]);
                        }
                        // TODO: Handle SkyDrop volatile when implemented
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
                        if (applies && battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "move: Gravity");
                        }
                    }
                }),
                OnModifyAccuracy = new OnModifyAccuracyEventInfo((battle, accuracy, _, _, _) =>
                {
                    // Accuracy boost: 6840/4096 ? 1.67x
                    return battle.ChainModify([6840, 4096]);
                }),
                OnDisableMove = new OnDisableMoveEventInfo((battle, pokemon) =>
                {
                    foreach (var moveSlot in pokemon.MoveSlots)
                    {
                        var move = _library.Moves[moveSlot.Id];
                        if (move.Flags.Gravity ?? false)
                        {
                            pokemon.DisableMove(moveSlot.Id);
                        }
                    }
                }),
                // Groundedness is implemented in Pokemon.IsGrounded()
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, move) =>
                {
                    // TODO: Check if move is Z-move (move.IsZ)
                    if (move.Flags.Gravity ?? false)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "move: Gravity", move.Name);
                        }
                        return BoolVoidUnion.FromBool(false);
                    }
                    return BoolVoidUnion.FromVoid();
                }, 6),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    // TODO: Check if move is Z-move (move.IsZ)
                    if (move.Flags.Gravity ?? false)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "move: Gravity", move.Name);
                        }
                        // Note: In TypeScript this returns false to prevent the move
                        // In C# we can't return from OnModifyMove to prevent the move
                        // The OnBeforeMove handler above handles the prevention
                    }
                }),
                OnFieldResidual = new OnFieldResidualEventInfo((_, _, _, _) =>
                {
                    // Duration handled automatically
                })
                {
                    Order = 27,
                    SubOrder = 2,
                },
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Gravity");
                    }
                }),
            },
            [ConditionId.Grudge] = new()
            {
                Id = ConditionId.Grudge,
                Name = "Grudge",
                EffectType = EffectType.Condition,
                // Note: Grudge move is marked as isNonstandard: "Past" in TypeScript
                // AssociatedMove not set since MoveId.Grudge doesn't exist in gen 9 VGC
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singlemove", pokemon, "Grudge");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnFaint = new OnFaintEventInfo((battle, target, source, effect) =>
                {
                    if (source == null || source.Fainted || effect == null) return;
                    // Check if fainted by a move (not a future move)
                    if (effect.EffectType == EffectType.Move &&
                        !(effect is Move { Flags.FutureMove: true }) &&
                        source.LastMove != null)
                    {
                        var move = source.LastMove;
                        // TODO: Handle Max moves - if (move.IsMax && move.BaseMove != null) get base move

                        foreach (var moveSlot in source.MoveSlots)
                        {
                            if (moveSlot.Id == move.Id)
                            {
                                moveSlot.Pp = 0;
                                if (battle.DisplayUi)
                                {
                                    battle.Add("-activate", source, "move: Grudge", move.Name);
                                }
                            }
                        }
                    }
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Debug("removing Grudge before attack");
                    pokemon.RemoveVolatile(_library.Conditions[ConditionId.Grudge]);
                    return BoolVoidUnion.FromVoid();
                }, 100),
            },
            [ConditionId.GuardSplit] = new()
            {
                Id = ConditionId.GuardSplit,
                Name = "Guard Split",
                EffectType = EffectType.Condition,
                // This is handled in the move's onHit, not as a persistent condition
            },
            [ConditionId.HealBlock] = new()
            {
                Id = ConditionId.HealBlock,
                Name = "Heal Block",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.HealBlock,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((battle, _, source, effect) =>
                {
                    if (effect?.Name == "Psychic Noise")
                    {
                        return 2;
                    }
                    if (source != null && source.HasAbility(AbilityId.Persistent))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", source, "ability: Persistent", "[move] Heal Block");
                        }
                        return 7;
                    }
                    return 5;
                }),
                OnStart = new OnStartEventInfo((battle, pokemon, source, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "move: Heal Block");
                    }
                    // TODO: source.moveThisTurnResult = true;
                    return BoolVoidUnion.FromVoid();
                }),
                OnDisableMove = new OnDisableMoveEventInfo((battle, pokemon) =>
                {
                    foreach (var moveSlot in pokemon.MoveSlots)
                    {
                        var move = _library.Moves[moveSlot.Id];
                        if (move.Flags.Heal ?? false)
                        {
                            pokemon.DisableMove(moveSlot.Id);
                        }
                    }
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, move) =>
                {
                    // TODO: Check if move is Z-move or Max move
                    if (move.Flags.Heal ?? false)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "move: Heal Block", move.Name);
                        }
                        return false;
                    }
                    return BoolVoidUnion.FromVoid();
                }, 6),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    // TODO: Check if move is Z-move or Max move
                    if (move.Flags.Heal ?? false)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "move: Heal Block", move.Name);
                        }
                    }
                }),
                OnResidual = new OnResidualEventInfo((_, _, _, _) =>
                {
                    // Duration handled automatically
                }, 20),
                        OnEnd = new OnEndEventInfo((battle, pokemon) =>
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Add("-end", pokemon, "move: Heal Block");
                            }
                        }),
                        OnTryHeal = new OnTryHealEventInfo(
                            (Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>)((battle, damage, target, source, effect) =>
                            {
                                // Z-power healing bypasses Heal Block
                                // TODO: Check for effect.IsZ when implemented
                                // if (effect != null && (effect.Id == "zpower" || effect is Move { IsZ: true })) return damage;

                                // Pollen Puff healing is blocked with a special message
                                if (source != null && target != source && target.Hp != target.MaxHp &&
                                    effect is Move { Id: MoveId.PollenPuff })
                                {
                                    battle.AttrLastMove("[still]");
                                    if (battle.DisplayUi)
                                    {
                                        // FIXME: Wrong error message in TypeScript, but following the same pattern
                                        battle.Add("cant", source, "move: Heal Block", effect.Name);
                                    }
                                    return null;
                                }
                                // Block all other healing
                                return IntBoolUnion.FromBool(false);
                            })),
                        // OnRestart is not needed - Psychic Noise duration is handled by DurationCallback
                    },
            [ConditionId.Hail] = new()
            {
                Id = ConditionId.Hail,
                Name = "Hail",
                EffectType = EffectType.Weather,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                    source.HasItem(ItemId.IcyRock) ? 8 : 5),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (!battle.DisplayUi) return;

                    if (effect is Ability)
                    {
                        if (battle.Gen <= 5) battle.EffectState.Duration = 0;
                        battle.Add("-weather", "Hail", "[from] ability: " + effect.Name,
                            $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-weather", "Hail");
                    }
                }),
                //OnFieldResidualOrder = 1,
                OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "Hail", "[upkeep]");
                        }

                        if (battle.Field.IsWeather(ConditionId.Hail))
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
            [ConditionId.HealReplacement] = new()
            {
                // This is a slot condition
                Id = ConditionId.HealReplacement,
                Name = "HealReplacement",
                EffectType = EffectType.Condition,
                OnStart = new OnStartEventInfo((battle, _, source, sourceEffect) =>
                {
                    battle.EffectState.SourceEffect = sourceEffect;
                    battle.Add("-activate", source, "healreplacement");
                    return new VoidReturn();
                }),
                OnSwitchIn = new OnSwitchInEventInfo((battle, target) =>
                {
                    if (!target.Fainted)
                    {
                        target.Heal(target.MaxHp);
                        if (battle.DisplayUi)
                        {
                            battle.Add("-heal", target, target.GetHealth,
                                "[from] move: " + battle.EffectState.SourceEffect?.Name);
                        }

                        target.Side.RemoveSlotCondition(target,
                            _library.Conditions[ConditionId.HealReplacement]);
                    }
                }),
            },
            [ConditionId.GrassyTerrain] = new()
            {
                Id = ConditionId.GrassyTerrain,
                Name = "Grassy Terrain",
                EffectType = EffectType.Terrain,
                AssociatedMove = MoveId.GrassyTerrain,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                {
                    if (source != null && source.HasItem(ItemId.TerrainExtender))
                    {
                        return 8;
                    }
                    return 5;
                }),
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, attacker, defender, move) =>
                {
                    // Weaken Earthquake, Bulldoze, Magnitude if defender is grounded
                    MoveId[] weakenedMoves = [MoveId.Earthquake, MoveId.Bulldoze, MoveId.Magnitude];
                    if (weakenedMoves.Contains(move.Id) &&
                        defender != null &&
                        (defender.IsGrounded() ?? false) &&
                        !defender.IsSemiInvulnerable())
                    {
                        battle.Debug("move weakened by grassy terrain");
                        return battle.ChainModify(0.5);
                    }
                    // Boost Grass moves if attacker is grounded
                    if (move.Type == MoveType.Grass && (attacker.IsGrounded() ?? false))
                    {
                        battle.Debug("grassy terrain boost");
                        return battle.ChainModify([5325, 4096]);
                    }
                    return new VoidReturn();
                }, 6),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (battle.DisplayUi)
                    {
                        if (effect is Ability)
                        {
                            battle.Add("-fieldstart", "move: Grassy Terrain",
                                $"[from] ability: {effect.Name}", $"[of] {source}");
                        }
                        else
                        {
                            battle.Add("-fieldstart", "move: Grassy Terrain");
                        }
                    }
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.IsGrounded() == true && !pokemon.IsSemiInvulnerable())
                    {
                        battle.Heal(pokemon.BaseMaxHp / 16, pokemon, pokemon);
                    }
                    else
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("Pokemon semi-invuln or not grounded; Grassy Terrain skipped");
                        }
                    }
                }, 5)
                {
                    SubOrder = 2,
                },
                OnFieldResidual = new OnFieldResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 27,
                    SubOrder = 7,
                },
                OnFieldEnd = new OnFieldEndEventInfo((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Grassy Terrain");
                    }
                }),
            },
        };
    }
}