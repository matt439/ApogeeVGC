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

                    if (sourceEffect is Ability)
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
            [ConditionId.Poison] = new()
            {
                Id = ConditionId.Poison,
                Name = "Poison",
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Poison, PokemonType.Steel],
                OnStart = new OnStartEventInfo((battle, target, source, sourceEffect) =>
                {
                    if (!battle.DisplayUi) return new VoidReturn();

                    if (sourceEffect.EffectType == EffectType.Ability)
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

                        EffectState? lockedMove = source.GetVolatile(ConditionId.LockedMove);
                        if (lockedMove is not null &&
                            source.Volatiles[ConditionId.LockedMove].Duration == 2)
                        {
                            source.RemoveVolatile(_library.Conditions[ConditionId.LockedMove]);
                        }

                        battle.Debug($"[Protect.OnTryHit] Returning Empty (block move)");
                        return new Empty(); // in place of Battle.NOT_FAIL ("")
                    },
                    3),
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
                    if (effect is Item item)
                    {
                        if (item.Id == ItemId.BoosterEnergy)
                        {
                            battle.EffectState.FromBooster = true;
                            if (battle.DisplayUi)
                            {
                                battle.Add("-activate", pokemon, "ability: Quark Drive",
                                    "[fromitem]");
                            }
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
                    new OnAnyModifyDamageEventInfo((battle, _, source, target, move) =>
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

                                return battle.ActivePerHalf > 1
                                    ? battle.ChainModify([2732, 4096])
                                    : battle.ChainModify(0.5);
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
            [ConditionId.RevivalBlessing] = new()
            {
                Id = ConditionId.RevivalBlessing,
                Name = "Revival Blessing",
                EffectType = EffectType.Condition,
                Duration = 1,
                AssociatedMove = MoveId.RevivalBlessing,
                // Note: Revival Blessing's effect is handled in Side
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
                    new OnWeatherModifyDamageEventInfo((battle, _, attacker, defender, move) =>
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
            [ConditionId.RainDance] = new()
            {
                Id = ConditionId.RainDance,
                Name = "RainDance",
                EffectType = EffectType.Weather,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, source, _, _) =>
                    source.HasItem(ItemId.DampRock) ? 8 : 5),
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
                    if (!battle.DisplayUi) return;
                    if (effect is Ability)
                    {
                        if (battle.Gen <= 5) battle.EffectState.Duration = 0;
                        battle.Add("-weather", "RainDance", "[from] ability: " + effect.Name,
                            $"[of] {source}");
                    }
                    else
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
            [ConditionId.RolloutStorage] = new()
            {
                Id = ConditionId.RolloutStorage,
                Name = "RolloutStorage",
                EffectType = EffectType.Condition,
                Duration = 2,
                OnBasePower = new OnBasePowerEventInfo((battle, _, source, _, move) =>
                {
                    int bp = Math.Max(1, move.BasePower);
                    source.Volatiles.TryGetValue(ConditionId.RolloutStorage, out EffectState? rolloutState);
                    int hitCount = rolloutState?.ContactHitCount ?? 0;
                    bp *= (int)Math.Pow(2, hitCount);
                    if (source.Volatiles.ContainsKey(ConditionId.DefenseCurl))
                    {
                        bp *= 2;
                    }

                    source.RemoveVolatile(_library.Conditions[ConditionId.RolloutStorage]);
                    return bp;
                }),
            },
        };
    }
}