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
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsPqr()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.Paralysis] = new()
            {
                Id = ConditionId.Paralysis,
                Name = "Paralysis",
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Electric],
                OnStart = new OnStartEventInfo((battle, target, source, sourceEffect) =>
                {
                    if (!battle.DisplayUi) return new VoidReturn();

                    if (sourceEffect?.EffectType == EffectType.Ability)
                    {
                        battle.Add("-status", target, "par", "[from] ability: " + sourceEffect.Name,
                            $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "par");
                    }

                    return new VoidReturn();
                }),
                //OnModifySpePriority = -101,
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                    {
                        spe = battle.FinalModify(spe);
                        if (!pokemon.HasAbility(AbilityId.QuickFeet))
                        {
                            spe = (int)Math.Floor(spe * 50.0 / 100);
                        }

                        return spe;
                    },
                    -101),
                //OnBeforeMovePriority = 1,
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                    {
                        if (!battle.RandomChance(1, 4)) return new VoidReturn();
                        if (battle.DisplayUi)
                        {
                            battle.Add("cant", pokemon, "par");
                        }

                        return false;
                    },
                    1),
            },
            [ConditionId.PartiallyTrapped] = new()
            {
                Id = ConditionId.PartiallyTrapped,
                Name = "Partially Trapped",
                EffectType = EffectType.Condition,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((battle, _, source, _) =>
                {
                    if (source != null && source.HasItem(ItemId.GripClaw))
                    {
                        return 8;
                    }

                    return battle.Random(5, 7);
                }),
                OnStart = new OnStartEventInfo((battle, pokemon, source, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon,
                            $"move: {battle.EffectState.SourceEffect?.Name ?? "unknown"}",
                            $"[of] {source}");
                    }

                    battle.EffectState.BoundDivisor =
                        (source != null && source.HasItem(ItemId.BindingBand)) ? 6 : 8;
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    var source = battle.EffectState.Source;
                    var sourceEffect = battle.EffectState.SourceEffect;

                    if (source != null &&
                        (!source.IsActive || source.Hp <= 0 || source.ActiveTurns <= 0))
                    {
                        pokemon.DeleteVolatile(ConditionId.PartiallyTrapped);
                        if (battle.DisplayUi)
                        {
                            battle.Add("-end", pokemon, sourceEffect?.Name ?? "Partially Trapped",
                                "[partiallytrapped]", "[silent]");
                        }

                        return;
                    }

                    battle.Damage(pokemon.BaseMaxHp / (battle.EffectState.BoundDivisor ?? 8));
                }, 13),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon,
                            battle.EffectState.SourceEffect?.Name ?? "Partially Trapped",
                            "[partiallytrapped]");
                    }
                }),
                OnTrapPokemon = new OnTrapPokemonEventInfo((battle, pokemon) =>
                {
                    var source = battle.EffectState.Source;
                    if (source is { IsActive: true })
                    {
                        pokemon.TryTrap();
                    }
                }),
            },
            [ConditionId.PartiallyTrappedFireSpin] = new()
            {
                Id = ConditionId.PartiallyTrappedFireSpin,
                Name = "Fire Spin",
                EffectType = EffectType.Condition,
                // Alias/variant of PartiallyTrapped for Fire Spin
                // Uses the same mechanics as PartiallyTrapped
            },
            [ConditionId.PartialTrappingLock] = new()
            {
                Id = ConditionId.PartialTrappingLock,
                Name = "Partial Trapping Lock",
                EffectType = EffectType.Condition,
                // This is a marker condition for moves that create partial trapping
                // The actual trapping is handled by PartiallyTrapped condition
            },
            [ConditionId.PerishSong] = new()
            {
                Id = ConditionId.PerishSong,
                Name = "Perish Song",
                EffectType = EffectType.Condition,
                Duration = 4,
                AssociatedMove = MoveId.PerishSong,
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "perish0");
                    }

                    target.Faint();
                }),
                // OnResidualOrder = 24
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (!pokemon.Volatiles.TryGetValue(ConditionId.PerishSong,
                            out var state)) return;
                    var duration = state.Duration ?? 0;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, $"perish{duration}");
                    }
                }, 24),
            },
            [ConditionId.PetalDance] = new()
            {
                Id = ConditionId.PetalDance,
                Name = "Petal Dance",
                EffectType = EffectType.Condition,
                // Petal Dance uses the LockedMove condition mechanics
                // This is a marker for the specific move
            },
            [ConditionId.PhantomForce] = new()
            {
                Id = ConditionId.PhantomForce,
                Name = "Phantom Force",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.PhantomForce,
                Duration = 2,
                OnInvulnerability = new OnInvulnerabilityEventInfo((_, _, _, _) =>
                    BoolIntEmptyVoidUnion.FromBool(false)),
            },
            [ConditionId.Poison] = new()
            {
                Id = ConditionId.Poison,
                Name = "Poison",
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Poison, PokemonType.Steel],
                OnStart = new OnStartEventInfo((battle, target, source, sourceEffect) =>
                {
                    if (!battle.DisplayUi) return new VoidReturn();

                    if (sourceEffect?.EffectType == EffectType.Ability)
                    {
                        battle.Add("-status", target, "psn", "[from] ability: " +
                                                             sourceEffect.Name, $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "psn");
                    }

                    return new VoidReturn();
                }),
                // OnResidualOrder = 9
                OnResidual = new OnResidualEventInfo(
                    (battle, pokemon, _, _) => { battle.Damage(pokemon.BaseMaxHp / 8); },
                    9),
            },
            [ConditionId.Poltergeist] = new()
            {
                Id = ConditionId.Poltergeist,
                Name = "Poltergeist",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Poltergeist,
                // Note: Poltergeist's item check is handled in the move's OnTry/OnTryHit handlers,
                // not as a volatile condition. This is just a marker condition.
            },
            [ConditionId.Powder] = new()
            {
                Id = ConditionId.Powder,
                Name = "Powder",
                EffectType = EffectType.Condition,
                Duration = 1,
                // Note: Powder is marked as isNonstandard: "Past" in TypeScript, meaning it's not in Gen 9.
                // Implementing for completeness in case it's ever needed.
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "Powder");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                // OnTryMovePriority = -1
                OnTryMove = new OnTryMoveEventInfo((battle, pokemon, _, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "move: Powder");
                        }

                        // Damage 1/4 of max HP, minimum 1
                        var damage = Math.Max(1, (int)Math.Round(pokemon.MaxHp / 4.0));
                        battle.Damage(damage);
                        battle.AttrLastMove("[still]");
                        return BoolEmptyVoidUnion.FromBool(false);
                    }

                    return BoolEmptyVoidUnion.FromVoid();
                }, -1),
            },
            [ConditionId.PowerTrick] = new()
            {
                Id = ConditionId.PowerTrick,
                Name = "Power Trick",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.PowerTrick,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "Power Trick");
                    }

                    // Swap Attack and Defense stored stats
                    var newAtk = pokemon.StoredStats[StatIdExceptHp.Def];
                    var newDef = pokemon.StoredStats[StatIdExceptHp.Atk];
                    pokemon.StoredStats[StatIdExceptHp.Atk] = newAtk;
                    pokemon.StoredStats[StatIdExceptHp.Def] = newDef;
                    return BoolVoidUnion.FromVoid();
                }),
                OnCopy = new OnCopyEventInfo((_, pokemon) =>
                {
                    // Re-swap when copying (e.g., Baton Pass) to maintain the swapped state
                    var newAtk = pokemon.StoredStats[StatIdExceptHp.Def];
                    var newDef = pokemon.StoredStats[StatIdExceptHp.Atk];
                    pokemon.StoredStats[StatIdExceptHp.Atk] = newAtk;
                    pokemon.StoredStats[StatIdExceptHp.Def] = newDef;
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Power Trick");
                    }

                    // Swap back Attack and Defense stored stats
                    var newAtk = pokemon.StoredStats[StatIdExceptHp.Def];
                    var newDef = pokemon.StoredStats[StatIdExceptHp.Atk];
                    pokemon.StoredStats[StatIdExceptHp.Atk] = newAtk;
                    pokemon.StoredStats[StatIdExceptHp.Def] = newDef;
                }),
                OnRestart = new OnRestartEventInfo((_, pokemon, _, _) =>
                {
                    // Using Power Trick again removes the volatile (toggles off)
                    pokemon.RemoveVolatile(_library.Conditions[ConditionId.PowerTrick]);
                    return BoolVoidUnion.FromVoid();
                }),
            },
            [ConditionId.PrimordialSea] = new()
            {
                Id = ConditionId.PrimordialSea,
                Name = "PrimordialSea",
                EffectType = EffectType.Weather,
                Duration = 0,
                //OnTryMovePriority = 1,
                OnTryMove = new OnTryMoveEventInfo((battle, attacker, _, move) =>
                    {
                        if (move.Type == MoveType.Fire && move.Category != MoveCategory.Status)
                        {
                            battle.Debug("Primordial Sea fire suppress");
                            if (battle.DisplayUi)
                            {
                                battle.Add("-fail", attacker, move, "[from] Primordial Sea");
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
                        if (move.Type == MoveType.Water)
                        {
                            battle.Debug("Rain water boost");
                            return battle.ChainModify(1.5);
                        }

                        return new VoidReturn();
                    }),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-weather", "PrimordialSea", "[from] ability: " + effect?.Name,
                            $"[of] {source}");
                    }
                }),
                //OnFieldResidualOrder = 1,
                OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "PrimordialSea", "[upkeep]");
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
            [ConditionId.Protect] = new()
            {
                Id = ConditionId.Protect,
                Name = "Protect",
                Duration = 1,
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Protect,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    battle.Debug($"[Protect.OnStart] Adding Protect volatile to {target.Name}");
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "Protect");
                    }

                    return new VoidReturn();
                }),
                //OnTryHitPriority = 3,
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                    {
                        battle.Debug(
                            $"[Protect.OnTryHit] CALLED! Target={target.Name}, Source={source.Name}, Move={move.Name}, HasProtectFlag={move.Flags.Protect ?? false}");

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

                        var lockedMove = source.GetVolatile(ConditionId.LockedMove);
                        if (lockedMove is not null &&
                            source.Volatiles[ConditionId.LockedMove].Duration == 2)
                        {
                            source.RemoveVolatile(_library.Conditions[ConditionId.LockedMove]);
                        }

                        battle.Debug("[Protect.OnTryHit] Returning Empty (block move)");
                        return new Empty(); // in place of Battle.NOT_FAIL ("")
                    },
                    3),
            },
            [ConditionId.Protosynthesis] = new()
            {
                Id = ConditionId.Protosynthesis,
                Name = "Protosynthesis",
                EffectType = EffectType.Condition,
                NoCopy = true,
                AssociatedAbility = AbilityId.Protosynthesis,
                OnStart = new OnStartEventInfo((battle, pokemon, _, effect) =>
                {
                    if (effect is Item { Id: ItemId.BoosterEnergy })
                    {
                        battle.EffectState.FromBooster = true;
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "ability: Protosynthesis",
                                "[fromitem]");
                        }
                    }
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon, "ability: Protosynthesis");
                    }

                    battle.EffectState.BestStat = pokemon.GetBestStat(false, true);
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon,
                            "protosynthesis" + battle.EffectState.BestStat);
                    }

                    return new VoidReturn();
                }),
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.Atk ||
                            pokemon.IgnoringAbility())
                        {
                            return atk;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Protosynthesis atk boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(atk);
                    },
                    5),
                // OnModifyDefPriority = 6
                OnModifyDef = new OnModifyDefEventInfo((battle, def, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.Def ||
                            pokemon.IgnoringAbility())
                        {
                            return def;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Protosynthesis def boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(def);
                    },
                    6),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.SpA ||
                            pokemon.IgnoringAbility())
                        {
                            return spa;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Protosynthesis spa boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(spa);
                    },
                    5),
                // OnModifySpDPriority = 6
                OnModifySpD = new OnModifySpDEventInfo((battle, spd, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.SpD ||
                            pokemon.IgnoringAbility())
                        {
                            return spd;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Protosynthesis spd boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(spd);
                    },
                    6),
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.Spe ||
                        pokemon.IgnoringAbility())
                    {
                        return spe;
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Debug("Protosynthesis spe boost");
                    }

                    battle.ChainModify(1.5);
                    return battle.FinalModify(spe);
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Protosynthesis");
                    }
                }),
            },
            [ConditionId.PsychicNoise] = new()
            {
                Id = ConditionId.PsychicNoise,
                Name = "Psychic Noise",
                EffectType = EffectType.Condition,
                Duration = 2, // Psychic Noise blocks healing for 2 turns
                AssociatedMove = MoveId.PsychicNoise,
                // Note: In Gen 9, Psychic Noise prevents the target from healing for 2 turns.
                // TypeScript uses the 'healblock' volatile with duration 2 when applied via Psychic Noise.
                // The move's secondary applies this condition, and related code checks for it when blocking heals.
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "move: Heal Block");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "move: Heal Block");
                    }
                }),
                OnTryHeal = new OnTryHealEventInfo(false),
            },
            [ConditionId.PsychicTerrain] = new()
            {
                Id = ConditionId.PsychicTerrain,
                Name = "Psychic Terrain",
                EffectType = EffectType.Terrain,
                AssociatedMove = MoveId.PsychicTerrain,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, _, source, _) =>
                {
                    if (source != null && source.HasItem(ItemId.TerrainExtender))
                    {
                        return 8;
                    }

                    return 5;
                }),
                // OnTryHitPriority = 4
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    // Psychic Terrain blocks priority moves targeting grounded Pokemon
                    // Excludes moves targeting self or with priority <= 0.1
                    if (move.Priority <= 0.1 || move.Target == MoveTarget.Self)
                        return BoolIntEmptyVoidUnion.FromVoid();

                    // Check semi-invulnerable and ally before grounded check
                    if (target.IsSemiInvulnerable() || target.IsAlly(source))
                        return BoolIntEmptyVoidUnion.FromVoid();

                    // Check if target is grounded - if not, show hint for priority moves
                    var isGrounded = target.IsGrounded();
                    if (!(isGrounded ?? false))
                    {
                        // Use base move priority (not active move priority which may include Prankster etc.)
                        Move baseMove = _library.Moves[move.Id];
                        if (baseMove.Priority > 0 && battle.DisplayUi)
                        {
                            battle.Hint("Psychic Terrain doesn't affect Pokémon immune to Ground.");
                        }

                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Psychic Terrain");
                    }

                    return null; // Silent failure - block the move
                }, 4),
                // OnBasePowerPriority = 6
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, attacker, _, move) =>
                {
                    // Boost Psychic-type moves by 1.3x (5325/4096) when attacker is grounded
                    var attackerGrounded = attacker.IsGrounded();
                    var attackerSemiInvuln = attacker.IsSemiInvulnerable();
                    if (move.Type == MoveType.Psychic && (attackerGrounded ?? false) &&
                        !attackerSemiInvuln)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("psychic terrain boost");
                        }

                        return battle.ChainModify([5325, 4096]);
                    }

                    return basePower;
                }, 6),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (battle.DisplayUi)
                    {
                        if (effect is Ability)
                        {
                            battle.Add("-fieldstart", "move: Psychic Terrain",
                                $"[from] ability: {effect.Name}", $"[of] {source}");
                        }
                        else
                        {
                            battle.Add("-fieldstart", "move: Psychic Terrain");
                        }
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
                        battle.Add("-fieldend", "move: Psychic Terrain");
                    }
                }),
            },
            [ConditionId.QuarkDrive] = new()
            {
                Id = ConditionId.QuarkDrive,
                Name = "Quark Drive",
                EffectType = EffectType.Condition,
                NoCopy = true,
                AssociatedAbility = AbilityId.QuarkDrive,
                OnStart = new OnStartEventInfo((battle, pokemon, _, effect) =>
                {
                    if (effect is Item { Id: ItemId.BoosterEnergy })
                    {
                        battle.EffectState.FromBooster = true;
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "ability: Quark Drive",
                                "[fromitem]");
                        }
                    }
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon, "ability: Quark Drive");
                    }

                    battle.EffectState.BestStat = pokemon.GetBestStat(false, true);
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", pokemon, "quarkdrive" + battle.EffectState.BestStat);
                    }

                    return new VoidReturn();
                }),
                //OnModifyAtkPriority = 5,
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.Atk ||
                            pokemon.IgnoringAbility())
                        {
                            return atk;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Quark Drive atk boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(atk);
                    },
                    5),
                //OnModifyDefPriority = 6,
                OnModifyDef = new OnModifyDefEventInfo((battle, def, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.Def ||
                            pokemon.IgnoringAbility())
                        {
                            return def;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Quark Drive def boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(def);
                    },
                    6),
                //OnModifySpAPriority = 5,
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.SpA ||
                            pokemon.IgnoringAbility())
                        {
                            return spa;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Quark Drive spa boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(spa);
                    },
                    5),
                //OnModifySpDPriority = 6,
                OnModifySpD = new OnModifySpDEventInfo((battle, spd, pokemon, _, _) =>
                    {
                        if (battle.EffectState.BestStat != StatIdExceptHp.SpD ||
                            pokemon.IgnoringAbility())
                        {
                            return spd;
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Debug("Quark Drive spd boost");
                        }

                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(spd);
                    },
                    6),
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    if (battle.EffectState.BestStat != StatIdExceptHp.Spe ||
                        pokemon.IgnoringAbility())
                    {
                        return spe;
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Debug("Quark Drive spe boost");
                    }

                    battle.ChainModify(1.5);
                    return battle.FinalModify(spe);
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", pokemon, "Quark Drive");
                    }
                }),
            },
            [ConditionId.QuickGuard] = new()
            {
                Id = ConditionId.QuickGuard,
                Name = "Quick Guard",
                EffectType = EffectType.Condition,
                Duration = 1,
                AssociatedMove = MoveId.QuickGuard,
                OnSideStart = new OnSideStartEventInfo((battle, _, source, _) =>
                {
                    if (battle.DisplayUi && source != null)
                    {
                        battle.Add("-singleturn", source, "Quick Guard");
                    }
                }),
                // OnTryHitPriority = 4
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    // Quick Guard blocks moves with positive priority (> 0.1 to account for fractional priority)
                    // It blocks 0 priority moves boosted by Prankster or Gale Wings
                    // Quick Claw/Custap Berry priority boosts do NOT count
                    if (move.Priority <= 0.1) return BoolIntEmptyVoidUnion.FromVoid();

                    // Check if move has protect flag
                    if (!(move.Flags.Protect ?? false))
                    {
                        // G-Max One Blow and G-Max Rapid Flow bypass protection
                        // (Not relevant for Gen 9 VGC but included for completeness)
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Quick Guard");
                    }

                    // Reset Outrage counter if source has lockedmove volatile
                    if (source.Volatiles.TryGetValue(ConditionId.LockedMove,
                            out var lockedMoveState))
                    {
                        if (lockedMoveState.Duration == 2)
                        {
                            source.RemoveVolatile(_library.Conditions[ConditionId.LockedMove]);
                        }
                    }

                    return new Empty(); // Equivalent to Battle.NOT_FAIL
                }, 4),
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) =>
                {
                    // Duration handled automatically
                })
                {
                    Order = 26,
                    SubOrder = 2,
                },
                OnSideEnd = new OnSideEndEventInfo((_, _) =>
                {
                    // Silent end - Quick Guard doesn't announce when it ends
                }),
            },
            [ConditionId.RagePowder] = new()
            {
                Id = ConditionId.RagePowder,
                Name = "Rage Powder",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.RagePowder,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", pokemon, "move: Rage Powder");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnFoeRedirectTarget = new OnFoeRedirectTargetEventInfo(
                    (battle, _, source, _, move) =>
                    {
                        // Get the Pokemon that used Rage Powder from the effect state
                        var effectTarget = battle.EffectState.Target;
                        var ragePowderUser =
                            effectTarget is PokemonEffectStateTarget pokemonTarget
                                ? pokemonTarget.Pokemon
                                : null;

                        if (ragePowderUser == null) return PokemonVoidUnion.FromVoid();

                        // Note: TypeScript checks isSkyDropped() here, but Sky Drop is isNonstandard: "Past"
                        // and not available in Gen 9 VGC, so we skip this check.

                        // Check if source has powder immunity
                        // Grass-types, Overcoat ability, and Safety Goggles are immune to powder moves
                        // TypeScript uses source.runStatusImmunity('powder') which returns true if NOT immune
                        var hasPowderImmunity = source.HasType(PokemonType.Grass) ||
                                                source.HasAbility(AbilityId.Overcoat) ||
                                                source.HasItem(ItemId.SafetyGoggles);

                        if (hasPowderImmunity)
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Debug(
                                    $"{source.Name} is immune to Rage Powder (powder immunity)");
                            }

                            return PokemonVoidUnion.FromVoid();
                        }

                        // Check if the Rage Powder user is a valid target for this move
                        if (battle.ValidTarget(ragePowderUser, source, move.Target))
                        {
                            if (move.SmartTarget ?? false)
                            {
                                move.SmartTarget = false;
                            }

                            if (battle.DisplayUi)
                            {
                                battle.Debug("Rage Powder redirected target of move");
                            }

                            return ragePowderUser; // Uses implicit conversion
                        }

                        return PokemonVoidUnion.FromVoid();
                    }, 1),
            },
            [ConditionId.RainDance] = new()
            {
                Id = ConditionId.RainDance,
                Name = "RainDance",
                EffectType = EffectType.Weather,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, _, source, _) =>
                    source?.HasItem(ItemId.DampRock) == true ? 8 : 5),
                OnWeatherModifyDamage =
                    new OnWeatherModifyDamageEventInfo((battle, _, _, defender, move) =>
                    {
                        if (defender.HasItem(ItemId.UtilityUmbrella)) return new VoidReturn();
                        if (move.Type == MoveType.Water)
                        {
                            battle.Debug("rain water boost");
                            return battle.ChainModify(1.5);
                        }

                        if (move.Type == MoveType.Fire)
                        {
                            battle.Debug("rain fire suppress");
                            return battle.ChainModify(0.5);
                        }

                        return new VoidReturn();
                    }),
                OnFieldStart = new OnFieldStartEventInfo((battle, _, source, effect) =>
                {
                    if (effect is Ability)
                    {
                        // Gen <= 5: ability-induced weather lasts indefinitely
                        if (battle.Gen <= 5) battle.EffectState.Duration = 0;
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "RainDance", "[from] ability: " + effect.Name,
                                $"[of] {source}");
                        }
                    }
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-weather", "RainDance");
                    }
                }),
                //OnFieldResidualOrder = 1,
                OnFieldResidual = new OnFieldResidualEventInfo((battle, _, _, _) =>
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-weather", "RainDance", "[upkeep]");
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
            [ConditionId.Recharge] = new()
            {
                Id = ConditionId.Recharge,
                Name = "Recharge",
                EffectType = EffectType.Condition,
                // Recharge is handled by MustRecharge condition
                // This is just a marker condition
            },
            [ConditionId.Recoil] = new()
            {
                Id = ConditionId.Recoil,
                Name = "Recoil",
                EffectType = EffectType.Condition,
            },
            [ConditionId.Reflect] = new()
            {
                Id = ConditionId.Reflect,
                Name = "Reflect",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Reflect,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, _, source, _) =>
                    source.HasItem(ItemId.LightClay) ? 8 : 5),
                OnAnyModifyDamage =
                    new OnAnyModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (target != source &&
                            battle.EffectState.Target is SideEffectStateTarget side &&
                            side.Side.HasAlly(target) &&
                            battle.GetCategory(move) == MoveCategory.Physical)
                        {
                            if (!target.GetMoveHitData(move).Crit && !(move.Infiltrates ?? false))
                            {
                                if (battle.DisplayUi)
                                {
                                    battle.Debug("Reflect weaken");
                                }

                                if (battle.ActivePerHalf > 1)
                                {
                                    battle.ChainModify([2732, 4096]);
                                }
                                else
                                {
                                    battle.ChainModify(0.5);
                                }

                                return battle.FinalModify(damage);
                            }
                        }

                        return new VoidReturn();
                    }),
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sidestart", side, "Reflect");
                    }
                }),
                //OnSideResidualOrder = 26,
                //OnSideResidualSubOrder = 1,
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 1,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "Reflect");
                    }
                }),
            },
            [ConditionId.Rest] = new()
            {
                Id = ConditionId.Rest,
                Name = "Rest",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Rest,
                // Rest doesn't create a volatile condition
                // It sets sleep status with statusState.time = 3
                // This is just a marker condition
            },
            [ConditionId.RevivalBlessing] = new()
            {
                Id = ConditionId.RevivalBlessing,
                Name = "Revival Blessing",
                EffectType = EffectType.Condition,
                Duration = 1,
                AssociatedMove = MoveId.RevivalBlessing,
                // Note: Revival Blessing's effect is handled in Side
            },
            [ConditionId.Rollout] = new()
            {
                Id = ConditionId.Rollout,
                Name = "Rollout",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Rollout,
                Duration = 1,
                OnLockMove = new OnLockMoveEventInfo(MoveId.Rollout),
                OnStart = new OnStartEventInfo((battle, _, _, _) =>
                {
                    battle.EffectState.HitCount = 0;
                    battle.EffectState.ContactHitCount = 0;
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((_, target, _, _) =>
                {
                    // If the pokemon used Struggle, don't lock
                    if (target.LastMove?.Id == MoveId.Struggle)
                    {
                        target.DeleteVolatile(ConditionId.Rollout);
                    }
                }),
            },
            [ConditionId.RolloutStorage] = new()
            {
                Id = ConditionId.RolloutStorage,
                Name = "RolloutStorage",
                EffectType = EffectType.Condition,
                Duration = 2,
                OnBasePower = new OnBasePowerEventInfo((_, _, source, _, move) =>
                {
                    var bp = Math.Max(1, move.BasePower);
                    source.Volatiles.TryGetValue(ConditionId.RolloutStorage,
                        out var rolloutState);
                    var hitCount = rolloutState?.ContactHitCount ?? 0;
                    bp *= (int)Math.Pow(2, hitCount);
                    if (source.Volatiles.ContainsKey(ConditionId.DefenseCurl))
                    {
                        bp *= 2;
                    }

                    source.RemoveVolatile(_library.Conditions[ConditionId.RolloutStorage]);
                    return bp;
                }),
            },
            [ConditionId.Roost] = new()
            {
                Id = ConditionId.Roost,
                Name = "Roost",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Roost,
                Duration = 1,
                OnResidual = new OnResidualEventInfo((_, _, _, _) =>
                {
                    // Duration handled automatically
                }, 25),
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    // TS: if (target.terastallized) { ... return false; }
                    // If Pokemon is Terastallized, Roost's type suppression doesn't activate
                    if (target.Terastallized is not null)
                    {
                        // Only show the hint if the Pokemon actually has Flying type
                        if (target.HasType(PokemonType.Flying) && battle.DisplayUi)
                        {
                            battle.Hint(
                                "If a Terastallized Pokemon uses Roost, it remains Flying-type.");
                        }

                        // Return false for ALL Terastallized Pokemon (not just Flying types)
                        return BoolVoidUnion.FromBool(false);
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Add("-singleturn", target, "move: Roost");
                    }


                    return BoolVoidUnion.FromVoid();
                }),
                OnType = new OnTypeEventInfo((battle, types, _) =>
                {
                    // Store the whole types array before filtering out Flying type
                    battle.EffectState.TypeWas = types;
                    // Filter out Flying type
                    return types.Where(t => t != PokemonType.Flying).ToArray();
                }, -1),
            },
        };
    }
}
