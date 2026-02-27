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
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

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
                OnStart = OnStartEventInfo.Create((battle, pokemon, _, _) =>
                {
                    // Check if pokemon has Ability Shield
                    if (pokemon.HasItem(ItemId.AbilityShield))
                    {
                        return new BoolRelayVar(false);
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Add("-endability", pokemon);
                    }

                    // End the pokemon's current ability
                    Ability ability = pokemon.GetAbility();
                    battle.SingleEvent(EventId.End, ability, pokemon.AbilityState,
                        (SingleEventTarget)pokemon,
                        new PokemonSingleEventSource(pokemon),
                        _library.Conditions[ConditionId.GastroAcid]);
                    return null;
                }),
                OnCopy = OnCopyEventInfo.Create((_, pokemon) =>
                {
                    // Remove Gastro Acid if the pokemon has an ability with cantsuppress flag
                    Ability ability = pokemon.GetAbility();
                    if (ability.Flags.CantSuppress ?? false)
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.GastroAcid]);
                    }
                }),
            },
            [ConditionId.Gem] = new()
            {
                Id = ConditionId.Gem,
                Name = "Gem",
                EffectType = EffectType.Condition,
                Duration = 1,
                AffectsFainted = true,
                //OnBasePowerPriority = 14,
                OnBasePower = OnBasePowerEventInfo.Create((battle, _, _, _, _) =>
                    {
                        battle.Debug("Gem Boost");
                        return battle.ChainModify([5325, 4096]);
                    },
                    14),
            },
            // CantUseTwice volatile condition placeholder - tracks that Gigaton Hammer was used last turn
            [ConditionId.GigatonHammer] = new()
            {
                Id = ConditionId.GigatonHammer,
                Name = "Gigaton Hammer",
                AssociatedMove = MoveId.GigatonHammer,
                EffectType = EffectType.Condition,
                // No handlers needed - this is just a volatile marker for cantUseTwice tracking
            },
            [ConditionId.GlaiveRush] = new()
            {
                Id = ConditionId.GlaiveRush,
                Name = "Glaive Rush",
                AssociatedMove = MoveId.GlaiveRush,
                EffectType = EffectType.Condition,
                NoCopy = true,
                OnStart = OnStartEventInfo.Create((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singlemove", pokemon, "Glaive Rush", "[silent]");
                    }

                    return new VoidReturn();
                }),
                // Moves always hit the Glaive Rush user
                OnAccuracy =
                    OnAccuracyEventInfo.Create((_, _, _, _, _) => IntBoolVoidUnion.FromBool(true)),
                // Damage is doubled against the Glaive Rush user
                OnSourceModifyDamage =
                    OnSourceModifyDamageEventInfo.Create((battle, _, _, _, _) =>
                        battle.ChainModify(2)),
                // Remove the Glaive Rush drawback before the pokemon attacks
                OnBeforeMove = OnBeforeMoveEventInfo.Create((battle, pokemon, _, _) =>
                {
                    battle.Debug("removing Glaive Rush drawback before attack");
                    pokemon.RemoveVolatile(_library.Conditions[ConditionId.GlaiveRush]);
                    return null;
                }, 100),
            },
            [ConditionId.GrassPledge] = new()
            {
                Id = ConditionId.GrassPledge,
                Name = "Grass Pledge",
                AssociatedMove = MoveId.GrassPledge,
                EffectType = EffectType.Condition,
                Duration = 4,
                OnSideStart = OnSideStartEventInfo.Create((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "Grass Pledge");
                    }
                }),
                // TS: return this.chainModify(0.25) - reduces Speed to 25%
                OnModifySpe = OnModifySpeEventInfo.Create((battle, spe, _) =>
                {
                    battle.ChainModify(0.25);
                    return new VoidReturn();
                }),
                OnSideResidual = OnSideResidualEventInfo.Create((_, _, _, _) =>
                {
                    // Duration handled automatically
                }, 26, 9),
                OnSideEnd = OnSideEndEventInfo.Create((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "Grass Pledge");
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
                DurationCallback = DurationCallbackEventInfo.Create((_, source, _, _) =>
                {
                    if (source != null && source.HasItem(ItemId.TerrainExtender))
                    {
                        return 8;
                    }

                    return 5;
                }),
                OnBasePower = OnBasePowerEventInfo.Create((battle, _, attacker, defender, move) =>
                {
                    // Weaken Earthquake, Bulldoze if defender is grounded
                    MoveId[] weakenedMoves = [MoveId.Earthquake, MoveId.Bulldoze];
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
                OnFieldStart = OnFieldStartEventInfo.Create((battle, _, source, effect) =>
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
                OnResidual = OnResidualEventInfo.Create((battle, pokemon, _, _) =>
                {
                    if (pokemon.IsGrounded() == true && !pokemon.IsSemiInvulnerable())
                    {
                        battle.Heal(pokemon.BaseMaxHp / 16, pokemon, pokemon);
                    }
                    else
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug(
                                "Pokemon semi-invuln or not grounded; Grassy Terrain skipped");
                        }
                    }
                }, 5) with
                {
                    SubOrder = 2,
                },
                OnFieldResidual = OnFieldResidualEventInfo.Create((_, _, _, _) => { }) with
                {
                    Order = 27,
                    SubOrder = 7,
                },
                OnFieldEnd = OnFieldEndEventInfo.Create((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Grassy Terrain");
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
                OnFieldStart = OnFieldStartEventInfo.Create((battle, _, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldstart", "move: Gravity");
                    }

                    // Remove airborne-related volatiles from all active Pokemon
                    foreach (Pokemon pokemon in battle.GetAllActive())
                    {
                        bool applies = false;
                        if (pokemon.RemoveVolatile(_library.Conditions[ConditionId.Bounce]) ||
                            pokemon.RemoveVolatile(_library.Conditions[ConditionId.Fly]))
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

                        if (applies && battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "move: Gravity");
                        }
                    }
                }),
                OnModifyAccuracy = OnModifyAccuracyEventInfo.Create((battle, accuracy, _, _, _) =>
                {
                    // Only modify accuracy for moves with numeric accuracy (not always-hit moves)
                    if (accuracy == null) return new VoidReturn();
                    return battle.ChainModify([6840, 4096]);
                }),
                OnDisableMove = OnDisableMoveEventInfo.Create((_, pokemon) =>
                {
                    foreach (MoveSlot moveSlot in pokemon.MoveSlots)
                    {
                        Move move = _library.Moves[moveSlot.Id];
                        if (move.Flags.Gravity ?? false)
                        {
                            pokemon.DisableMove(moveSlot.Id);
                        }
                    }
                }),
                // Groundedness is implemented in Pokemon.IsGrounded()
                OnBeforeMove = OnBeforeMoveEventInfo.Create((battle, pokemon, _, move) =>
                {
                    if (move.Flags.Gravity ?? false)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "move: Gravity", move.Name);
                        }

                        return new BoolRelayVar(false);
                    }

                    return null;
                }, 6),
                OnModifyMove = OnModifyMoveEventInfo.Create((battle, move, pokemon, _) =>
                {
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
                OnFieldResidual = OnFieldResidualEventInfo.Create((_, _, _, _) =>
                {
                    // Duration handled automatically
                }) with
                {
                    Order = 27,
                    SubOrder = 2,
                },
                OnFieldEnd = OnFieldEndEventInfo.Create((battle, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldend", "move: Gravity");
                    }
                }),
            },
            [ConditionId.GuardSplit] = new()
            {
                Id = ConditionId.GuardSplit,
                Name = "Guard Split",
                EffectType = EffectType.Condition,
                // This is handled in the move's onHit, not as a persistent condition
            },
            [ConditionId.HealingWish] = new()
            {
                // This is a slot condition for Healing Wish
                Id = ConditionId.HealingWish,
                Name = "Healing Wish",
                AssociatedMove = MoveId.HealingWish,
                EffectType = EffectType.Condition,
                OnSwitchIn = OnSwitchInEventInfo.Create((battle, target) =>
                {
                    // Trigger the onSwap event
                    battle.SingleEvent(EventId.Swap, _library.Conditions[ConditionId.HealingWish],
                        battle.EffectState, target);
                }),
                OnSwap = OnSwapEventInfo.Create((battle, target, _) =>
                {
                    // Only heal if the Pokemon needs healing (HP not full or has status)
                    if (!target.Fainted &&
                        (target.Hp < target.MaxHp || target.Status != ConditionId.None))
                    {
                        target.Heal(target.MaxHp);
                        target.ClearStatus();
                        if (battle.DisplayUi)
                        {
                            battle.Add("-heal", target, target.GetHealth,
                                "[from] move: Healing Wish");
                        }

                        target.Side.RemoveSlotCondition(target,
                            _library.Conditions[ConditionId.HealingWish]);
                    }
                }),
            },
            [ConditionId.HealReplacement] = new()
            {
                // This is a slot condition
                Id = ConditionId.HealReplacement,
                Name = "HealReplacement",
                EffectType = EffectType.Condition,
                OnStart = OnStartEventInfo.Create((battle, _, source, sourceEffect) =>
                {
                    battle.EffectState.SourceEffect = sourceEffect;
                    battle.Add("-activate", source, "healreplacement");
                    return new VoidReturn();
                }),
                OnSwitchIn = OnSwitchInEventInfo.Create((battle, target) =>
                {
                    if (!target.Fainted)
                    {
                        target.Heal(target.MaxHp);
                        if (battle.DisplayUi)
                        {
                            battle.Add("-heal", target, target.GetHealth,
                                "[from] move: " + battle.EffectState.SourceEffect?.Name, "[zeffect]");
                        }

                        target.Side.RemoveSlotCondition(target,
                            _library.Conditions[ConditionId.HealReplacement]);
                    }
                }),
            },
            [ConditionId.HelpingHand] = new()
            {
                Id = ConditionId.HelpingHand,
                Name = "Helping Hand",
                AssociatedMove = MoveId.HelpingHand,
                EffectType = EffectType.Condition,
                Duration = 1,
                OnStart = OnStartEventInfo.Create((battle, target, source, _) =>
                {
                    battle.EffectState.DoubleMultiplier = 1.5;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "Helping Hand", $"[of] {source}");
                    }

                    return null;
                }),
                OnRestart = OnRestartEventInfo.Create((battle, target, source, _) =>
                {
                    battle.EffectState.DoubleMultiplier = (battle.EffectState.DoubleMultiplier ?? 1.0) * 1.5;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "Helping Hand", $"[of] {source}");
                    }

                    return null;
                }),
                OnBasePower = OnBasePowerEventInfo.Create((battle, _, _, _, _) =>
                {
                    battle.Debug($"Boosting from Helping Hand: {battle.EffectState.DoubleMultiplier}");
                    return battle.ChainModify(battle.EffectState.DoubleMultiplier ?? 1.0);
                }, 10),
            },
            [ConditionId.Imprison] = new()
            {
                Id = ConditionId.Imprison,
                Name = "Imprison",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Imprison,
                NoCopy = true,
                OnStart = OnStartEventInfo.Create((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "move: Imprison");
                    }

                    return null;
                }),
                OnFoeDisableMove = OnFoeDisableMoveEventInfo.Create((battle, pokemon) =>
                {
                    // Get the source Pokemon from battle.EffectState
                    Pokemon? source = battle.EffectState.Source;
                    if (source == null) return;

                    // Disable all moves that the source Pokemon also knows (except Struggle)
                    foreach (MoveSlot moveSlot in source.MoveSlots)
                    {
                        if (moveSlot.Id == MoveId.Struggle) continue;
                        pokemon.DisableMove(moveSlot.Id, true);
                    }

                    pokemon.MaybeDisabled = true;
                }),
                OnFoeBeforeMove = OnFoeBeforeMoveEventInfo.Create((battle, attacker, _, move) =>
                {
                    // Get the source Pokemon from battle.EffectState
                    Pokemon? source = battle.EffectState.Source;
                    if (source == null) return null;

                    if (move.Id == MoveId.Struggle)
                    {
                        return null;
                    }

                    // Check if the source Pokemon also knows this move
                    if (source.HasMove(move.Id))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", attacker, "move: Imprison", move.Name);
                        }

                        return BoolVoidUnion.FromBool(false);
                    }

                    return null;
                }, 4),
            },
            [ConditionId.Ingrain] = new()
            {
                Id = ConditionId.Ingrain,
                Name = "Ingrain",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Ingrain,
                OnStart = OnStartEventInfo.Create((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "move: Ingrain");
                    }

                    return null;
                }),
                OnTrapPokemon = OnTrapPokemonEventInfo.Create((_, pokemon) => { pokemon.TryTrap(); }),
                OnResidual = OnResidualEventInfo.Create(
                    (battle, pokemon, _, _) => { battle.Heal(pokemon.BaseMaxHp / 16, pokemon, pokemon); }, 7),
                // groundedness implemented in Pokemon.IsGrounded()
                OnDragOut = OnDragOutEventInfo.Create((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon, "move: Ingrain");
                    }

                    return null; // Prevent drag-out silently
                }),
            },
        };
    }
}