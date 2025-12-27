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
        };
    }
}