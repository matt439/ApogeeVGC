using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesGhi()
    {
        return new Dictionary<MoveId, Move>
        {
            [MoveId.GastroAcid] = new()
            {
                Id = MoveId.GastroAcid,
                Num = 380,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Gastro Acid",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.GastroAcid,
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
                OnTryHit = new OnTryHitEventInfo((battle, target, _, _) =>
                {
                    // Check if ability can't be suppressed
                    if (target.GetAbility().Flags.CantSuppress == true)
                    {
                        return false;
                    }

                    // Check for Ability Shield
                    if (target.HasItem(ItemId.AbilityShield))
                    {
                        battle.Add("-block", target, "item: Ability Shield");
                        return null;
                    }

                    return new VoidReturn();
                }),
            },
            [MoveId.GigaDrain] = new()
            {
                Id = MoveId.GigaDrain,
                Num = 202,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Special,
                Name = "Giga Drain",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Heal = true,
                    Metronome = true,
                },
                Drain = (1, 2),
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.GigaImpact] = new()
            {
                Id = MoveId.GigaImpact,
                Num = 416,
                Accuracy = 90,
                BasePower = 150,
                Category = MoveCategory.Physical,
                Name = "Giga Impact",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Recharge = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.MustRecharge,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.GigatonHammer] = new()
            {
                Id = MoveId.GigatonHammer,
                Num = 893,
                Accuracy = 100,
                BasePower = 160,
                Category = MoveCategory.Physical,
                Name = "Gigaton Hammer",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    CantUseTwice = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.GlacialLance] = new()
            {
                Id = MoveId.GlacialLance,
                Num = 824,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Glacial Lance",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Ice,
            },
            [MoveId.Glaciate] = new()
            {
                Id = MoveId.Glaciate,
                Num = 549,
                Accuracy = 95,
                BasePower = 65,
                Category = MoveCategory.Special,
                Name = "Glaciate",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable
                    {
                        Spe = -1,
                    },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Ice,
            },
            [MoveId.GlaiveRush] = new()
            {
                Id = MoveId.GlaiveRush,
                Num = 862,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Glaive Rush",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.GlaiveRush,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.Glare] = new()
            {
                Id = MoveId.Glare,
                Num = 137,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Glare",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Status = ConditionId.Paralysis,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.GrassKnot] = new()
            {
                Id = MoveId.GrassKnot,
                Num = 447,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, target, _) =>
                {
                    var targetWeight = target.GetWeight();
                    int bp;
                    if (targetWeight >= 2000)
                    {
                        bp = 120;
                    }
                    else if (targetWeight >= 1000)
                    {
                        bp = 100;
                    }
                    else if (targetWeight >= 500)
                    {
                        bp = 80;
                    }
                    else if (targetWeight >= 250)
                    {
                        bp = 60;
                    }
                    else if (targetWeight >= 100)
                    {
                        bp = 40;
                    }
                    else
                    {
                        bp = 20;
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Debug($"BP: {bp}");
                    }

                    return bp;
                }),
                Category = MoveCategory.Special,
                Name = "Grass Knot",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    NonSky = true,
                    Metronome = true,
                },
                // onTryHit only applies to dynamax
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.GrassPledge] = new()
            {
                Id = MoveId.GrassPledge,
                Num = 520,
                Accuracy = 100,
                BasePower = 80,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, _, move) =>
                {
                    // Check if this is being called as part of a pledge combo
                    if (move.SourceEffect is MoveEffectStateId
                        {
                            MoveId: MoveId.WaterPledge or MoveId.FirePledge
                        })
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-combine");
                        }

                        return 150;
                    }

                    return move.BasePower;
                }),
                Category = MoveCategory.Special,
                Name = "Grass Pledge",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    NonSky = true,
                    Metronome = true,
                    PledgeCombo = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
                OnPrepareHit = new OnPrepareHitEventInfo((battle, _, source, move) =>
                {
                    // Check the battle queue for ally Pokémon using Water Pledge or Fire Pledge
                    if (battle.Queue.List != null)
                    {
                        foreach (var action in battle.Queue.List)
                        {
                            if (action is not MoveAction moveAction ||
                                moveAction.Move == null ||
                                moveAction.Pokemon?.IsActive != true ||
                                moveAction.Pokemon.Fainted)
                            {
                                continue;
                            }

                            // Check if it's an ally using Water Pledge or Fire Pledge
                            if (moveAction.Pokemon.IsAlly(source) &&
                                moveAction.Move.Id is MoveId.WaterPledge or MoveId.FirePledge)
                            {
                                battle.Queue.PrioritizeAction(moveAction, move);
                                if (battle.DisplayUi)
                                {
                                    battle.Add("-waiting", source, moveAction.Pokemon);
                                }

                                return null;
                            }
                        }
                    }

                    return new VoidReturn();
                }),
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    // Check if this move is being modified by a pledge combo
                    if (move.SourceEffect is MoveEffectStateId { MoveId: MoveId.WaterPledge })
                    {
                        // Water Pledge + Grass Pledge = Grass-type move with Grass Pledge side condition (swamp)
                        // Swamp applies to target's side (slows their speed to 25%)
                        move.Type = MoveType.Grass;
                        move.ForceStab = true;
                        move.SideCondition = ConditionId.GrassPledge; // Applies to target side
                    }
                    else if (move.SourceEffect is MoveEffectStateId { MoveId: MoveId.FirePledge })
                    {
                        // Fire Pledge + Grass Pledge = Fire-type move with Fire Pledge side condition (sea of fire)
                        // Sea of fire applies to target's side (damages non-Fire types)
                        move.Type = MoveType.Fire;
                        move.ForceStab = true;
                        move.SideCondition = ConditionId.FirePledge; // Applies to target side
                    }
                }),
            },
            [MoveId.GrassyGlide] = new()
            {
                Id = MoveId.GrassyGlide,
                Num = 803,
                Accuracy = 100,
                BasePower = 55,
                Category = MoveCategory.Physical,
                Name = "Grassy Glide",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
                OnModifyPriority = new OnModifyPriorityEventInfo((battle, priority, source, _, _) =>
                {
                    if (battle.Field.IsTerrain(ConditionId.GrassyTerrain, source) &&
                        (source.IsGrounded() ?? false))
                    {
                        return priority + 1;
                    }

                    return new VoidReturn();
                }),
            },
            [MoveId.GrassyTerrain] = new()
            {
                Id = MoveId.GrassyTerrain,
                Num = 580,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Grassy Terrain",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    NonSky = true,
                    Metronome = true,
                },
                Terrain = ConditionId.GrassyTerrain,
                Condition = _library.Conditions[ConditionId.GrassyTerrain],
                Target = MoveTarget.All,
                Type = MoveType.Grass,
            },
            [MoveId.GravApple] = new()
            {
                Id = MoveId.GravApple,
                Num = 788,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Grav Apple",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable
                    {
                        Def = -1,
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, _) =>
                {
                    if (battle.Field.PseudoWeather.ContainsKey(ConditionId.Gravity))
                    {
                        battle.ChainModify(3, 2); // 1.5x = 3/2
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }),
            },
            [MoveId.Gravity] = new()
            {
                Id = MoveId.Gravity,
                Num = 356,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Gravity",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    NonSky = true,
                    Metronome = true,
                },
                PseudoWeather = ConditionId.Gravity,
                Target = MoveTarget.All,
                Type = MoveType.Psychic,
            },
            [MoveId.Growl] = new()
            {
                Id = MoveId.Growl,
                Num = 45,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Growl",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Sound = true,
                    BypassSub = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable
                {
                    Atk = -1,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Normal,
            },
            [MoveId.Growth] = new()
            {
                Id = MoveId.Growth,
                Num = 74,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Growth",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable
                {
                    Atk = 1,
                    SpA = 1,
                },
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
                OnModifyMove = new OnModifyMoveEventInfo((_, move, pokemon, _) =>
                {
                    // In sun, boost to +2/+2 instead of +1/+1
                    var weather = pokemon.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        move.Boosts = new SparseBoostsTable { Atk = 2, SpA = 2 };
                    }
                }),
            },
            [MoveId.GuardSplit] = new()
            {
                Id = MoveId.GuardSplit,
                Num = 470,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Guard Split",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    var newDef = (target.StoredStats.Def + source.StoredStats.Def) / 2;
                    target.StoredStats.Def = newDef;
                    source.StoredStats.Def = newDef;
                    var newSpD = (target.StoredStats.SpD + source.StoredStats.SpD) / 2;
                    target.StoredStats.SpD = newSpD;
                    source.StoredStats.SpD = newSpD;
                    battle.Add("-activate", source, "move: Guard Split", $"[of] {target}");
                    return new VoidReturn();
                }),
            },
            [MoveId.GuardSwap] = new()
            {
                Id = MoveId.GuardSwap,
                Num = 385,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Guard Swap",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    BypassSub = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    // Swap def and spd boosts
                    var targetDef = target.Boosts.Def;
                    var targetSpD = target.Boosts.SpD;
                    var sourceDef = source.Boosts.Def;
                    var sourceSpD = source.Boosts.SpD;

                    source.SetBoost(new SparseBoostsTable { Def = targetDef, SpD = targetSpD });
                    target.SetBoost(new SparseBoostsTable { Def = sourceDef, SpD = sourceSpD });

                    battle.Add("-swapboost", source, target, "def, spd", "[from] move: Guard Swap");
                    return new VoidReturn();
                }),
            },
            [MoveId.Guillotine] = new()
            {
                Id = MoveId.Guillotine,
                Num = 12,
                Accuracy = 30,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Guillotine",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Ohko = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.GunkShot] = new()
            {
                Id = MoveId.GunkShot,
                Num = 441,
                Accuracy = 80,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Gunk Shot",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Poison,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.Gust] = new()
            {
                Id = MoveId.Gust,
                Num = 16,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Gust",
                BasePp = 35,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Distance = true,
                    Metronome = true,
                    Wind = true,
                },
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.GyroBall] = new()
            {
                Id = MoveId.GyroBall,
                Num = 360,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, pokemon, target, _) =>
                {
                    var targetSpeed = target.GetStat(StatIdExceptHp.Spe);
                    var userSpeed = pokemon.GetStat(StatIdExceptHp.Spe);
                    var power = userSpeed > 0 ? 25 * targetSpeed / userSpeed + 1 : 1;
                    if (power > 150) power = 150;
                    if (battle.DisplayUi)
                    {
                        battle.Debug($"BP: {power}");
                    }

                    return power;
                }),
                Category = MoveCategory.Physical,
                Name = "Gyro Ball",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bullet = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.HappyHour] = new()
            {
                Id = MoveId.HappyHour,
                Num = 603,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Happy Hour",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Metronome = true },
                OnTryHit = new OnTryHitEventInfo((battle, target, _, _) =>
                {
                    battle.Add("-activate", target, "move: Happy Hour");
                    return new VoidReturn();
                }),
                Target = MoveTarget.AllySide,
                Type = MoveType.Normal,
            },
            [MoveId.Harden] = new()
            {
                Id = MoveId.Harden,
                Num = 106,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Harden",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                Boosts = new SparseBoostsTable { Def = 1 },
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Haze] = new()
            {
                Id = MoveId.Haze,
                Num = 114,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Haze",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { BypassSub = true, Metronome = true },
                Target = MoveTarget.All,
                Type = MoveType.Ice,
                OnHitField = new OnHitFieldEventInfo((battle, _, _, _) =>
                {
                    battle.Add("-clearallboost");
                    foreach (var pokemon in battle.GetAllActive())
                    {
                        pokemon.ClearBoosts();
                    }

                    return new VoidReturn();
                }),
            },
            [MoveId.Headbutt] = new()
            {
                Id = MoveId.Headbutt,
                Num = 29,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Headbutt",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.HeadSmash] = new()
            {
                Id = MoveId.HeadSmash,
                Num = 457,
                Accuracy = 80,
                BasePower = 150,
                Category = MoveCategory.Physical,
                Name = "Head Smash",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Recoil = (1, 2),
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.HealBell] = new()
            {
                Id = MoveId.HealBell,
                Num = 215,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Heal Bell",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true, Distance = true, Sound = true, BypassSub = true,
                    Metronome = true,
                },
                Target = MoveTarget.AllyTeam,
                Type = MoveType.Normal,
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    battle.Add("-activate", source, "move: Heal Bell");
                    var success = false;

                    // Note: In standard 2v2 VGC, all team Pokemon are on the same Side.Pokemon list
                    // AllySide is only relevant for multi battles (4 player) which are not supported
                    List<Pokemon> allies = [..target.Side.Pokemon];

                    foreach (var ally in allies)
                    {
                        // Check if ally has ability immunity (except if source)
                        if (ally != source && !battle.SuppressingAbility(ally))
                        {
                            if (ally.HasAbility(AbilityId.Soundproof))
                            {
                                battle.Add("-immune", ally, "[from] ability: Soundproof");
                                continue;
                            }

                            if (ally.HasAbility(AbilityId.GoodAsGold))
                            {
                                battle.Add("-immune", ally, "[from] ability: Good as Gold");
                                continue;
                            }
                        }

                        if (ally.CureStatus()) success = true;
                    }

                    if (success)
                    {
                        return new VoidReturn();
                    }

                    return false;
                }),
            },
            [MoveId.HealingWish] = new()
            {
                Id = MoveId.HealingWish,
                Num = 361,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Healing Wish",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Heal = true, Metronome = true },
                SelfDestruct = MoveSelfDestruct.FromIfHit(),
                SlotCondition = ConditionId.HealingWish,
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
                OnTryHit = new OnTryHitEventInfo((battle, _, source, _) =>
                {
                    if (battle.CanSwitch(source.Side) == 0)
                    {
                        battle.AttrLastMove("[still]");
                        battle.Add("-fail", source);
                        return null; // NOT_FAIL equivalent
                    }

                    return new VoidReturn();
                }),
            },
            [MoveId.HeartSwap] = new()
            {
                Id = MoveId.HeartSwap,
                Num = 391,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Heart Swap",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Mirror = true, BypassSub = true, AllyAnim = true,
                    Metronome = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    // Store both Pokemon's boosts
                    SparseBoostsTable targetBoosts = new()
                    {
                        Atk = target.Boosts.Atk,
                        Def = target.Boosts.Def,
                        SpA = target.Boosts.SpA,
                        SpD = target.Boosts.SpD,
                        Spe = target.Boosts.Spe,
                        Accuracy = target.Boosts.Accuracy,
                        Evasion = target.Boosts.Evasion,
                    };
                    SparseBoostsTable sourceBoosts = new()
                    {
                        Atk = source.Boosts.Atk,
                        Def = source.Boosts.Def,
                        SpA = source.Boosts.SpA,
                        SpD = source.Boosts.SpD,
                        Spe = source.Boosts.Spe,
                        Accuracy = source.Boosts.Accuracy,
                        Evasion = source.Boosts.Evasion,
                    };

                    target.SetBoost(sourceBoosts);
                    source.SetBoost(targetBoosts);

                    battle.Add("-swapboost", source, target, "[from] move: Heart Swap");
                    return new VoidReturn();
                }),
            },
            [MoveId.HeatCrash] = new()
            {
                Id = MoveId.HeatCrash,
                Num = 535,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, pokemon, target, _) =>
                {
                    var targetWeight = target.GetWeight();
                    var pokemonWeight = pokemon.GetWeight();
                    int bp;
                    if (pokemonWeight >= targetWeight * 5)
                    {
                        bp = 120;
                    }
                    else if (pokemonWeight >= targetWeight * 4)
                    {
                        bp = 100;
                    }
                    else if (pokemonWeight >= targetWeight * 3)
                    {
                        bp = 80;
                    }
                    else if (pokemonWeight >= targetWeight * 2)
                    {
                        bp = 60;
                    }
                    else
                    {
                        bp = 40;
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Debug($"BP: {bp}");
                    }

                    return bp;
                }),
                Category = MoveCategory.Physical,
                Name = "Heat Crash",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, NonSky = true, Metronome = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.HeadlongRush] = new()
            {
                Id = MoveId.HeadlongRush,
                Num = 838,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Headlong Rush",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Punch = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable
                    {
                        Def = -1,
                        SpD = -1,
                    },
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
            },
            [MoveId.HeavySlam] = new()
            {
                Id = MoveId.HeavySlam,
                Num = 484,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, pokemon, target, _) =>
                {
                    var targetWeight = target.GetWeight();
                    var pokemonWeight = pokemon.GetWeight();
                    int bp;
                    if (pokemonWeight >= targetWeight * 5)
                    {
                        bp = 120;
                    }
                    else if (pokemonWeight >= targetWeight * 4)
                    {
                        bp = 100;
                    }
                    else if (pokemonWeight >= targetWeight * 3)
                    {
                        bp = 80;
                    }
                    else if (pokemonWeight >= targetWeight * 2)
                    {
                        bp = 60;
                    }
                    else
                    {
                        bp = 40;
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Debug($"BP: {bp}");
                    }

                    return bp;
                }),
                Category = MoveCategory.Physical,
                Name = "Heavy Slam",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    NonSky = true,
                    Metronome = true,
                },
                // OnTryHit only applies to dynamax
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.HammerArm] = new()
            {
                Id = MoveId.HammerArm,
                Num = 359,
                Accuracy = 90,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Hammer Arm",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Punch = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable
                    {
                        Spe = -1,
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.HardPress] = new()
            {
                Id = MoveId.HardPress,
                Num = 912,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, target, _) =>
                {
                    var hp = target.Hp;
                    var maxHp = target.MaxHp;
                    // Use 4096-based rounding to match game mechanics
                    // TypeScript: Math.floor(Math.floor((100 * (100 * Math.floor(hp * 4096 / maxHP)) + 2048 - 1) / 4096) / 100) || 1
                    var step1 = (int)Math.Floor((double)hp * 4096 / maxHp);
                    var step2 = 100 * (100 * step1) + 2048 - 1; // 10000 * step1 + 2047
                    var step3 = (int)Math.Floor((double)step2 / 4096);
                    var bp = (int)Math.Floor((double)step3 / 100);
                    if (bp == 0) bp = 1;
                    if (battle.DisplayUi)
                    {
                        battle.Debug($"BP for {hp}/{maxHp} HP: {bp}");
                    }

                    return bp;
                }),
                Category = MoveCategory.Physical,
                Name = "Hard Press",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.HealPulse] = new()
            {
                Id = MoveId.HealPulse,
                Num = 505,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Heal Pulse",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Distance = true,
                    Heal = true,
                    AllyAnim = true,
                    Pulse = true,
                    Metronome = true,
                },
                Target = MoveTarget.Any,
                Type = MoveType.Psychic,
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    int healAmount;
                    if (source.HasAbility(AbilityId.MegaLauncher))
                    {
                        healAmount = battle.Modify(target.BaseMaxHp, 3, 4); // 75%
                    }
                    else
                    {
                        healAmount = (int)Math.Ceiling(target.BaseMaxHp * 0.5);
                    }

                    var healResult = battle.Heal(healAmount, target);
                    var success = healResult is not FalseIntFalseUnion;

                    if (success && !target.IsAlly(source))
                    {
                        target.Staleness = StalenessId.External;
                    }

                    if (!success)
                    {
                        battle.Add("-fail", target, "heal");
                        return null; // NOT_FAIL equivalent
                    }

                    return new VoidReturn();
                }),
            },
            [MoveId.Heatwave] = new()
            {
                Id = MoveId.Heatwave,
                Num = 257,
                Accuracy = 90,
                BasePower = 95,
                Category = MoveCategory.Special,
                Name = "Heat Wave",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Wind = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Fire,
            },
            [MoveId.HelpingHand] = new()
            {
                Id = MoveId.HelpingHand,
                Num = 270,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Helping Hand",
                BasePp = 20,
                Priority = 5,
                Flags = new MoveFlags { BypassSub = true, NoAssist = true, FailCopycat = true },
                VolatileStatus = ConditionId.HelpingHand,
                Target = MoveTarget.AdjacentAlly,
                Type = MoveType.Normal,
                OnTryHit = new OnTryHitEventInfo((battle, target, _, _) =>
                {
                    // Fails if target already moved or won't move this turn
                    if (!target.NewlySwitched && battle.Queue.WillMove(target) == null)
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
            },
            [MoveId.Hex] = new()
            {
                Id = MoveId.Hex,
                Num = 506,
                Accuracy = 100,
                BasePower = 65,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, target, move) =>
                {
                    if (target.Status != ConditionId.None || target.HasAbility(AbilityId.Comatose))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Debug("BP doubled from status condition");
                        }

                        return move.BasePower * 2;
                    }

                    return move.BasePower;
                }),
                Category = MoveCategory.Special,
                Name = "Hex",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.HighJumpKick] = new()
            {
                Id = MoveId.HighJumpKick,
                Num = 136,
                Accuracy = 90,
                BasePower = 130,
                Category = MoveCategory.Physical,
                Name = "High Jump Kick",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Gravity = true, Metronome = true,
                },
                HasCrashDamage = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
                OnMoveFail = new OnMoveFailEventInfo((battle, _, source, move) =>
                {
                    battle.Damage(source.BaseMaxHp / 2, source, source,
                        BattleDamageEffect.FromIEffect(move));
                }),
            },
            [MoveId.HighHorsepower] = new()
            {
                Id = MoveId.HighHorsepower,
                Num = 667,
                Accuracy = 95,
                BasePower = 95,
                Category = MoveCategory.Physical,
                Name = "High Horsepower",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
            },
            [MoveId.HoneClaws] = new()
            {
                Id = MoveId.HoneClaws,
                Num = 468,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Hone Claws",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                Boosts = new SparseBoostsTable { Atk = 1, Accuracy = 1 },
                Target = MoveTarget.Self,
                Type = MoveType.Dark,
            },
            [MoveId.HornAttack] = new()
            {
                Id = MoveId.HornAttack,
                Num = 30,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Physical,
                Name = "Horn Attack",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.HornDrill] = new()
            {
                Id = MoveId.HornDrill,
                Num = 32,
                Accuracy = 30,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Horn Drill",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Ohko = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.HornLeech] = new()
            {
                Id = MoveId.HornLeech,
                Num = 532,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Horn Leech",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Heal = true, Metronome = true,
                },
                Drain = (1, 2),
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.Howl] = new()
            {
                Id = MoveId.Howl,
                Num = 336,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Howl",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Sound = true, Metronome = true },
                Boosts = new SparseBoostsTable { Atk = 1 },
                Target = MoveTarget.Allies,
                Type = MoveType.Normal,
            },
            [MoveId.Hurricane] = new()
            {
                Id = MoveId.Hurricane,
                Num = 542,
                Accuracy = 70,
                BasePower = 110,
                Category = MoveCategory.Special,
                Name = "Hurricane",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Distance = true,
                    Wind = true,
                    Metronome = true,
                },
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, target) =>
                {
                    if (target != null)
                    {
                        var effectiveWeather = target.EffectiveWeather();
                        switch (effectiveWeather)
                        {
                            case ConditionId.RainDance:
                            case ConditionId.PrimordialSea:
                                move.Accuracy = IntTrueUnion.FromTrue();
                                break;
                            case ConditionId.SunnyDay:
                            case ConditionId.DesolateLand:
                                move.Accuracy = 50;
                                break;
                        }
                    }
                }),
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Confusion,
                },
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.HydroCannon] = new()
            {
                Id = MoveId.HydroCannon,
                Num = 308,
                Accuracy = 90,
                BasePower = 150,
                Category = MoveCategory.Special,
                Name = "Hydro Cannon",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Recharge = true, Protect = true, Mirror = true, Metronome = true },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.MustRecharge,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.HydroPump] = new()
            {
                Id = MoveId.HydroPump,
                Num = 56,
                Accuracy = 80,
                BasePower = 110,
                Category = MoveCategory.Special,
                Name = "Hydro Pump",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.HydroSteam] = new()
            {
                Id = MoveId.HydroSteam,
                Num = 876,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Hydro Steam",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Mirror = true, Defrost = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.HyperDrill] = new()
            {
                Id = MoveId.HyperDrill,
                Num = 887,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Hyper Drill",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Mirror = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.HyperBeam] = new()
            {
                Id = MoveId.HyperBeam,
                Num = 63,
                Accuracy = 90,
                BasePower = 150,
                Category = MoveCategory.Special,
                Name = "Hyper Beam",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Recharge = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.MustRecharge,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.HyperVoice] = new()
            {
                Id = MoveId.HyperVoice,
                Num = 304,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Hyper Voice",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Sound = true,
                    BypassSub = true,
                    Metronome = true,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Normal,
            },
            [MoveId.HyperspaceFury] = new()
            {
                Id = MoveId.HyperspaceFury,
                Num = 621,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Hyperspace Fury",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Mirror = true, BypassSub = true, NoSketch = true },
                BreaksProtect = true,
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { Def = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
                OnTry = new OnTryEventInfo((battle, _, source, _) =>
                {
                    if (source.Species.Name == "Hoopa-Unbound")
                    {
                        return new VoidReturn();
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Hint(
                            "Only a Pokemon whose form is Hoopa Unbound can use this move.");
                        if (source.Species.Name == "Hoopa")
                        {
                            battle.AttrLastMove("[still]");
                            battle.Add("-fail", source, "move: Hyperspace Fury", "[forme]");
                            return null;
                        }

                        battle.AttrLastMove("[still]");
                        battle.Add("-fail", source, "move: Hyperspace Fury");
                    }

                    return null;
                }),
            },
            [MoveId.HyperspaceHole] = new()
            {
                Id = MoveId.HyperspaceHole,
                Num = 593,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Hyperspace Hole",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Mirror = true, Metronome = true, BypassSub = true },
                BreaksProtect = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.Hypnosis] = new()
            {
                Id = MoveId.Hypnosis,
                Num = 95,
                Accuracy = 60,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Hypnosis",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Reflectable = true, Mirror = true, Metronome = true },
                Status = ConditionId.Sleep,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.IceBeam] = new()
            {
                Id = MoveId.IceBeam,
                Num = 58,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Ice Beam",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Freeze,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.IceBurn] = new()
            {
                Id = MoveId.IceBurn,
                Num = 554,
                Accuracy = 90,
                BasePower = 140,
                Category = MoveCategory.Special,
                Name = "Ice Burn",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Charge = true, Protect = true, Mirror = true, NoSleepTalk = true,
                    FailInstruct = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
                OnTryMove = new OnTryMoveEventInfo((battle, attacker, defender, move) =>
                {
                    // If already charged (volatile exists), remove it and execute the attack
                    if (attacker.RemoveVolatile(battle.Library.Conditions[ConditionId.TwoTurnMove]))
                    {
                        return new VoidReturn();
                    }

                    // Starting the charge turn - show prepare message
                    battle.Add("-prepare", attacker, move.Name);
                    // Run ChargeMove event (for Power Herb, etc.)
                    var chargeResult =
                        battle.RunEvent(EventId.ChargeMove, attacker, defender, move);
                    if (chargeResult is BoolRelayVar { Value: false })
                    {
                        return new VoidReturn();
                    }

                    // Add the volatile for two-turn move state
                    attacker.AddVolatile(ConditionId.TwoTurnMove, defender);
                    return null; // Return null to skip the attack this turn
                }),
            },
            [MoveId.IceFang] = new()
            {
                Id = MoveId.IceFang,
                Num = 423,
                Accuracy = 95,
                BasePower = 65,
                Category = MoveCategory.Physical,
                Name = "Ice Fang",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Metronome = true, Bite = true,
                },
                Secondaries =
                [
                    new SecondaryEffect { Chance = 10, Status = ConditionId.Freeze },
                    new SecondaryEffect { Chance = 10, VolatileStatus = ConditionId.Flinch },
                ],
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.IceHammer] = new()
            {
                Id = MoveId.IceHammer,
                Num = 665,
                Accuracy = 90,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Ice Hammer",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Punch = true, Metronome = true },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { Spe = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.IcePunch] = new()
            {
                Id = MoveId.IcePunch,
                Num = 8,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Ice Punch",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Punch = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Freeze,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.IceShard] = new()
            {
                Id = MoveId.IceShard,
                Num = 420,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Ice Shard",
                BasePp = 30,
                Priority = 1,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.IceSpinner] = new()
            {
                Id = MoveId.IceSpinner,
                Num = 861,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Ice Spinner",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
                OnAfterHit = new OnAfterHitEventInfo((battle, _, source, _) =>
                {
                    if (source.Hp > 0)
                    {
                        battle.Field.ClearTerrain();
                    }

                    return new VoidReturn();
                }),
                OnAfterSubDamage = new OnAfterSubDamageEventInfo((battle, _, _, source, _) =>
                {
                    if (source.Hp > 0)
                    {
                        battle.Field.ClearTerrain();
                    }
                }),
            },
            [MoveId.IcicleCrash] = new()
            {
                Id = MoveId.IcicleCrash,
                Num = 556,
                Accuracy = 90,
                BasePower = 85,
                Category = MoveCategory.Physical,
                Name = "Icicle Crash",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.IcicleSpear] = new()
            {
                Id = MoveId.IcicleSpear,
                Num = 333,
                Accuracy = 100,
                BasePower = 25,
                Category = MoveCategory.Physical,
                Name = "Icicle Spear",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                MultiHit = new[] { 2, 5 },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.IcyWind] = new()
            {
                Id = MoveId.IcyWind,
                Num = 196,
                Accuracy = 95,
                BasePower = 55,
                Category = MoveCategory.Special,
                Name = "Icy Wind",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true, Wind = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Spe = -1 },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Ice,
            },
            [MoveId.Incinerate] = new()
            {
                Id = MoveId.Incinerate,
                Num = 510,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Incinerate",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Fire,
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    var item = target.GetItem();
                    if ((item.IsBerry || item.IsGem) &&
                        target.TakeItem(source) is ItemItemFalseUnion)
                    {
                        battle.Add("-enditem", target, item.Name, "[from] move: Incinerate");
                    }

                    return new VoidReturn();
                }),
            },
            [MoveId.InfernalParade] = new()
            {
                Id = MoveId.InfernalParade,
                Num = 844,
                Accuracy = 100,
                BasePower = 60,
                BasePowerCallback = new BasePowerCallbackEventInfo((_, _, target, move) =>
                {
                    if (target.Status != ConditionId.None || target.HasAbility(AbilityId.Comatose))
                    {
                        return move.BasePower * 2;
                    }

                    return move.BasePower;
                }),
                Category = MoveCategory.Special,
                Name = "Infernal Parade",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.Imprison] = new()
            {
                Id = MoveId.Imprison,
                Num = 286,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Imprison",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    BypassSub = true,
                    Metronome = true,
                    MustPressure = true,
                },
                VolatileStatus = ConditionId.Imprison,
                Condition = _library.Conditions[ConditionId.Imprison],
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.Inferno] = new()
            {
                Id = MoveId.Inferno,
                Num = 517,
                Accuracy = 50,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Inferno",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.Infestation] = new()
            {
                Id = MoveId.Infestation,
                Num = 611,
                Accuracy = 100,
                BasePower = 20,
                Category = MoveCategory.Special,
                Name = "Infestation",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                VolatileStatus = ConditionId.PartiallyTrapped,
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.Ingrain] = new()
            {
                Id = MoveId.Ingrain,
                Num = 275,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Ingrain",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, NonSky = true, Metronome = true },
                VolatileStatus = ConditionId.Ingrain,
                Condition = _library.Conditions[ConditionId.Ingrain],
                Target = MoveTarget.Self,
                Type = MoveType.Grass,
            },
            [MoveId.Instruct] = new()
            {
                Id = MoveId.Instruct,
                Num = 689,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Instruct",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, BypassSub = true, AllyAnim = true, Metronome = true,
                    FailInstruct = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    // Check if target has a last move
                    if (target.LastMove == null)
                    {
                        return false;
                    }

                    var lastMove = target.LastMove;
                    var moveSlot = target.GetMoveData(lastMove.Id);


                    // Check various fail conditions
                    if ((lastMove.Flags.FailInstruct ?? false) ||
                        (lastMove.Flags.Charge ?? false) ||
                        (lastMove.Flags.Recharge ?? false) ||
                        target.Volatiles.ContainsKey(ConditionId.FocusPunch) ||
                        moveSlot is { Pp: <= 0 })
                    {
                        return false;
                    }

                    battle.Add("-singleturn", target, "move: Instruct", $"[of] {source}");

                    // Create a move action for the target to use their last move
                    MoveAction instructedAction = new()
                    {
                        Choice = ActionId.Move,
                        Pokemon = target,
                        Move = lastMove,
                        TargetLoc = target.LastMoveTargetLoc ?? 0,
                        Order = 200,
                        OriginalTarget = target,
                    };

                    var resolvedActions = battle.Queue.ResolveAction(instructedAction);
                    if (resolvedActions.Count > 0 && resolvedActions[0] is MoveAction ma)
                    {
                        battle.Queue.PrioritizeAction(ma,
                            _library.Moves[MoveId.Instruct].ToActiveMove());
                    }

                    return new VoidReturn();
                }),
            },
            [MoveId.IronDefense] = new()
            {
                Id = MoveId.IronDefense,
                Num = 334,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Iron Defense",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                Boosts = new SparseBoostsTable { Def = 2 },
                Target = MoveTarget.Self,
                Type = MoveType.Steel,
            },
            [MoveId.IronHead] = new()
            {
                Id = MoveId.IronHead,
                Num = 442,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Iron Head",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.IronTail] = new()
            {
                Id = MoveId.IronTail,
                Num = 231,
                Accuracy = 75,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Iron Tail",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Boosts = new SparseBoostsTable { Def = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.IvyCudgel] = new()
            {
                Id = MoveId.IvyCudgel,
                Num = 904,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Ivy Cudgel",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                CritRatio = 2,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
                OnModifyType = new OnModifyTypeEventInfo((_, move, pokemon, _) =>
                {
                    switch (pokemon.Species.Name)
                    {
                        case "Ogerpon-Wellspring":
                        case "Ogerpon-Wellspring-Tera":
                            move.Type = MoveType.Water;
                            break;
                        case "Ogerpon-Hearthflame":
                        case "Ogerpon-Hearthflame-Tera":
                            move.Type = MoveType.Fire;
                            break;
                        case "Ogerpon-Cornerstone":
                        case "Ogerpon-Cornerstone-Tera":
                            move.Type = MoveType.Rock;
                            break;
                    }
                }),
            },
        };
    }
}