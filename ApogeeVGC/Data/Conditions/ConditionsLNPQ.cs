using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
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
    private partial Dictionary<ConditionId, Condition> CreateConditionsLNPQ()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.LeechSeed] = new()
            {
                Id = ConditionId.LeechSeed,
                Name = "Leech Seed",
                EffectType = EffectType.Condition,
                ImmuneTypes = [PokemonType.Grass],
                AssociatedMove = MoveId.LeechSeed,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "move: Leech Seed");
                    }

                    return new VoidReturn();
                }),
                //OnResidualOrder = 8,
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                    {
                        Pokemon? target =
                            battle.GetAtSlot(pokemon.Volatiles[ConditionId.LeechSeed].SourceSlot);
                        if (target is null || target.Fainted || target.Hp <= 0)
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Debug("Nothing to leech into");
                            }

                            return;
                        }

                        IntFalseUndefinedUnion damage =
                            battle.Damage(pokemon.BaseMaxHp / 8, pokemon, target);

                        if (damage is IntIntFalseUndefined d)
                        {
                            battle.Heal(d.Value, target, pokemon);
                        }
                    },
                    8),
            },
            [ConditionId.LightScreen] = new()
            {
                Id = ConditionId.LightScreen,
                Name = "Light Screen",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.LightScreen,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, _, source, _) =>
                    source.HasItem(ItemId.LightClay) ? 8 : 5),
                OnAnyModifyDamage =
                    new OnAnyModifyDamageEventInfo((battle, _, source, target, move) =>
                    {
                        if (target != source &&
                            battle.EffectState.Target is SideEffectStateTarget side &&
                            side.Side.HasAlly(target) &&
                            battle.GetCategory(move) == MoveCategory.Special)
                        {
                            if (!target.GetMoveHitData(move).Crit && !(move.Infiltrates ?? false))
                            {
                                if (battle.DisplayUi)
                                {
                                    battle.Debug("Light Screen weaken");
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
                        battle.Add("-sidestart", side, "move: Light Screen");
                    }
                }),
                //OnSideResidualOrder = 26,
                //OnSideResidualSubOrder = 2,
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) => { })
                {
                    Order = 26,
                    SubOrder = 2,
                },
                OnSideEnd = new OnSideEndEventInfo((battle, side) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-sideend", side, "move: Light Screen");
                    }
                }),
            },
            [ConditionId.None] = new()
            {
                Id = ConditionId.None,
                Name = "None",
                EffectType = EffectType.Condition,
            },
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
        };
    }
}
