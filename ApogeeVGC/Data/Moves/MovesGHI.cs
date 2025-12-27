using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

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
                // TODO: onTryHit - check for Ability Shield and cantsuppress flag
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
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
                // TODO: cantusetwice flag - cannot be used twice in a row
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
                    int targetWeight = target.GetWeight();
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
                // TODO: basePowerCallback - 150 when combined with water/fire pledge
                // TODO: onPrepareHit - pledge combination logic
                // TODO: onModifyMove - pledge combination effects
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
                // TODO: onModifyPriority - +1 priority if Grassy Terrain and user is grounded
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
                // TODO: Set terrain to GrassyTerrain
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
                // TODO: onBasePower - 1.5x if Gravity is active
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
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable
                    {
                        Atk = -1,
                    },
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
                SelfBoost = new SparseBoostsTable
                {
                    Atk = 1,
                    SpA = 1,
                },
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
                // TODO: onModifyMove - boost becomes +2/+2 in sun
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
                // TODO: onHit - average def/spd stats between user and target
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
                // TODO: onHit - swap def/spd boosts
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
                    int targetSpeed = target.GetStat(StatIdExceptHp.Spe);
                    int userSpeed = pokemon.GetStat(StatIdExceptHp.Spe);
                    int power = 25 * targetSpeed / userSpeed + 1;
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
            [MoveId.Hail] = new()
            {
                Id = MoveId.Hail,
                Num = 258,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Hail",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Metronome = true },
                Target = MoveTarget.All,
                Type = MoveType.Ice,
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
                Flags = new MoveFlags { },
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
                SelfBoost = new SparseBoostsTable { Def = 1 },
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
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
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
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
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
                Flags = new MoveFlags { Snatch = true, Distance = true, Sound = true, BypassSub = true, Metronome = true },
                Target = MoveTarget.AllyTeam,
                Type = MoveType.Normal,
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
                SelfSwitch = true,
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
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
                Flags = new MoveFlags { Protect = true, Mirror = true, BypassSub = true, AllyAnim = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.HeatCrash] = new()
            {
                Id = MoveId.HeatCrash,
                Num = 535,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, pokemon, target, _) =>
                {
                    int targetWeight = target.GetWeight();
                    int pokemonWeight = pokemon.GetWeight();
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
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, NonSky = true, Metronome = true },
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
                                int targetWeight = target.GetWeight();
                                int pokemonWeight = pokemon.GetWeight();
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
                            Num = 876,
                            Accuracy = 100,
                            BasePower = 0,
                            BasePowerCallback = new BasePowerCallbackEventInfo((battle, pokemon, target, _) =>
                            {
                                int power = Math.Max((int)Math.Floor(100.0 * target.Hp / target.MaxHp) + 1, 1);
                                if (battle.DisplayUi)
                                {
                                    battle.Debug($"BP: {power}");
                                }
                                return power;
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
                            // TODO: onHit - heal target by 50% of its max HP, or 75% if user has Mega Launcher
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
                Target = MoveTarget.AdjacentAlly,
                Type = MoveType.Normal,
            },
            [MoveId.Hex] = new()
            {
                Id = MoveId.Hex,
                Num = 506,
                Accuracy = 100,
                BasePower = 65,
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
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Gravity = true, Metronome = true },
                HasCrashDamage = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
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
                SelfBoost = new SparseBoostsTable { Atk = 1, Accuracy = 1 },
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
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
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
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
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
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Heal = true, Metronome = true },
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
                Target = MoveTarget.AllySide,
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
                Flags = new MoveFlags { Recharge = true, Protect = true, Mirror = true, Metronome = true },
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
                Num = 875,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Hydro Steam",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Defrost = true, Metronome = true },
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
                Flags = new MoveFlags { Mirror = true, BypassSub = true },
                BreaksProtect = true,
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { Def = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
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
                Flags = new MoveFlags { Protect = true, Reflectable = true, Mirror = true, Metronome = true },
                Status = ConditionId.Sleep,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
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
                Flags = new MoveFlags { Charge = true, Protect = true, Mirror = true, NoSleepTalk = true, FailInstruct = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
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
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true, Bite = true },
                Secondaries = new[]
                {
                    new SecondaryEffect { Chance = 10, Status = ConditionId.Freeze },
                    new SecondaryEffect { Chance = 10, VolatileStatus = ConditionId.Flinch },
                },
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
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Punch = true },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { Spe = -1 },
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
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.InfernalParade] = new()
            {
                Id = MoveId.InfernalParade,
                Num = 844,
                Accuracy = 100,
                BasePower = 60,
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
                Flags = new MoveFlags { Protect = true, BypassSub = true, AllyAnim = true, Metronome = true, FailInstruct = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
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
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
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
                Flags = new MoveFlags { Protect = true, Mirror = true },
                CritRatio = 2,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
                    };
                }
            }
