using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesVwx()
    {
        return new Dictionary<MoveId, Move>
        {
            // ===== U MOVES =====

            [MoveId.UpperHand] = new()
            {
                Id = MoveId.UpperHand,
                Num = 918,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Physical,
                Name = "Upper Hand",
                BasePp = 15,
                Priority = 3,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnTry = new OnTryEventInfo((battle, _, target, _) =>
                {
                    // Check if target will move with a priority move that isn't Status
                    var action = battle.Queue.WillMove(target);
                    var move = action?.Move;
                    if (move == null || move.Priority <= 0 || move.Category == MoveCategory.Status)
                    {
                        return false;
                    }
                    return new VoidReturn();
                }),
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.Uproar] = new()
            {
                Id = MoveId.Uproar,
                Num = 253,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Uproar",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Sound = true,
                    BypassSub = true,
                    Metronome = true,
                    NoSleepTalk = true,
                    FailInstruct = true,
                },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.Uproar,
                },
                Condition = _library.Conditions[ConditionId.Uproar],
                OnTryHit = new OnTryHitEventInfo((_, target, _, _) =>
                {
                    // Wake up all sleeping Pokemon on both sides
                    foreach (var pokemon in target.Side.Active)
                    {
                        if (pokemon?.Status == ConditionId.Sleep)
                        {
                            pokemon.CureStatus();
                        }
                    }
                    foreach (var pokemon in target.Side.Foe.Active)
                    {
                        if (pokemon?.Status == ConditionId.Sleep)
                        {
                            pokemon.CureStatus();
                        }
                    }
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.RandomNormal,
                Type = MoveType.Normal,
            },
            [MoveId.UTurn] = new()
            {
                Id = MoveId.UTurn,
                Num = 369,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "U-turn",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                SelfSwitch = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },

            // ===== V MOVES =====

            [MoveId.VacuumWave] = new()
            {
                Id = MoveId.VacuumWave,
                Num = 410,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Vacuum Wave",
                BasePp = 30,
                Priority = 1,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.Venoshock] = new()
            {
                Id = MoveId.Venoshock,
                Num = 474,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Special,
                Name = "Venoshock",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, target, _) =>
                {
                    // Double power if target is poisoned or badly poisoned
                    if (target.Status == ConditionId.Poison || target.Status == ConditionId.Toxic)
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(basePower);
                    }
                    return basePower;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.VictoryDance] = new()
            {
                Id = MoveId.VictoryDance,
                Num = 837,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Victory Dance",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Dance = true,
                    Metronome = true,
                },
                SelfBoost = new SparseBoostsTable
                {
                    Atk = 1,
                    Def = 1,
                    Spe = 1,
                },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Fighting,
            },
            [MoveId.VineWhip] = new()
            {
                Id = MoveId.VineWhip,
                Num = 22,
                Accuracy = 100,
                BasePower = 45,
                Category = MoveCategory.Physical,
                Name = "Vine Whip",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.ViseGrip] = new()
            {
                Id = MoveId.ViseGrip,
                Num = 11,
                Accuracy = 100,
                BasePower = 55,
                Category = MoveCategory.Physical,
                Name = "Vise Grip",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.VoltSwitch] = new()
            {
                Id = MoveId.VoltSwitch,
                Num = 521,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Volt Switch",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                SelfSwitch = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.VoltTackle] = new()
            {
                Id = MoveId.VoltTackle,
                Num = 344,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Volt Tackle",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Recoil = (33, 100),
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },

            // ===== W MOVES =====

            [MoveId.Waterfall] = new()
            {
                Id = MoveId.Waterfall,
                Num = 127,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Waterfall",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.WaterGun] = new()
            {
                Id = MoveId.WaterGun,
                Num = 55,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Water Gun",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.WaterPledge] = new()
            {
                Id = MoveId.WaterPledge,
                Num = 518,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Water Pledge",
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
                // Note: Pledge combo mechanics (combining with Fire/Grass Pledge) are not implemented
                // as they require complex queue manipulation and are rarely used in VGC
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.WaterPulse] = new()
            {
                Id = MoveId.WaterPulse,
                Num = 352,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Water Pulse",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Distance = true,
                    Metronome = true,
                    Pulse = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    VolatileStatus = ConditionId.Confusion,
                },
                Target = MoveTarget.Any,
                Type = MoveType.Water,
            },
            [MoveId.WaterSpout] = new()
            {
                Id = MoveId.WaterSpout,
                Num = 323,
                Accuracy = 100,
                BasePower = 150,
                Category = MoveCategory.Special,
                Name = "Water Spout",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, source, _, move) =>
                {
                    // Base power scales with user's HP percentage
                    var bp = move.BasePower * source.Hp / source.MaxHp;
                    battle.Debug($"[Water Spout] BP: {bp}");
                    return (int)bp;
                }),
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Water,
            },
            [MoveId.WaterShuriken] = new()
            {
                Id = MoveId.WaterShuriken,
                Num = 594,
                Accuracy = 100,
                BasePower = 15,
                Category = MoveCategory.Special,
                Name = "Water Shuriken",
                BasePp = 20,
                Priority = 1,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                MultiHit = new int[] { 2, 5 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.WaveCrash] = new()
            {
                Id = MoveId.WaveCrash,
                Num = 834,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Wave Crash",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Recoil = (33, 100),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.WeatherBall] = new()
            {
                Id = MoveId.WeatherBall,
                Num = 311,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Weather Ball",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bullet = true,
                },
                OnModifyType = new OnModifyTypeEventInfo((battle, move, source, _) =>
                {
                    var weather = battle.Field.EffectiveWeather();
                    move.Type = weather switch
                    {
                        ConditionId.SunnyDay or ConditionId.DesolateLand => MoveType.Fire,
                        ConditionId.RainDance or ConditionId.PrimordialSea => MoveType.Water,
                        ConditionId.Sandstorm => MoveType.Rock,
                        ConditionId.Snowscape or ConditionId.Hail => MoveType.Ice,
                        _ => move.Type,
                    };
                }),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, source, _) =>
                {
                    var weather = battle.Field.EffectiveWeather();
                    if (weather != ConditionId.None)
                    {
                        move.BasePower *= 2;
                    }
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Whirlpool] = new()
            {
                Id = MoveId.Whirlpool,
                Num = 250,
                Accuracy = 85,
                BasePower = 35,
                Category = MoveCategory.Special,
                Name = "Whirlpool",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.PartiallyTrapped,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.Whirlwind] = new()
            {
                Id = MoveId.Whirlwind,
                Num = 18,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Whirlwind",
                BasePp = 20,
                Priority = -6,
                Flags = new MoveFlags
                {
                    Reflectable = true,
                    Mirror = true,
                    BypassSub = true,
                    AllyAnim = true,
                    Metronome = true,
                    NoAssist = true,
                    FailCopycat = true,
                    Wind = true,
                },
                ForceSwitch = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.WickedBlow] = new()
            {
                Id = MoveId.WickedBlow,
                Num = 817,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Wicked Blow",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Punch = true,
                },
                WillCrit = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.WideGuard] = new()
            {
                Id = MoveId.WideGuard,
                Num = 469,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Wide Guard",
                BasePp = 10,
                Priority = 3,
                Flags = new MoveFlags
                {
                    Snatch = true,
                },
                SideCondition = ConditionId.WideGuard,
                Condition = _library.Conditions[ConditionId.WideGuard],
                OnTry = new OnTryEventInfo((battle, _, _, _) =>
                {
                    // Wide Guard fails if there are no upcoming actions in the queue
                    return battle.Queue.WillAct() != null ? new VoidReturn() : (BoolEmptyVoidUnion?)false;
                }),
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Rock,
            },
            [MoveId.WildboltStorm] = new()
            {
                Id = MoveId.WildboltStorm,
                Num = 847,
                Accuracy = 80,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Wildbolt Storm",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Wind = true,
                },
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, source, _) =>
                {
                    var weather = battle.Field.EffectiveWeather();
                    if (weather is ConditionId.RainDance or ConditionId.PrimordialSea)
                    {
                        move.Accuracy = IntTrueUnion.FromTrue();
                    }
                }),
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Electric,
            },
            [MoveId.WildCharge] = new()
            {
                Id = MoveId.WildCharge,
                Num = 528,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Wild Charge",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Recoil = (1, 4),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.WillOWisp] = new()
            {
                Id = MoveId.WillOWisp,
                Num = 261,
                Accuracy = 85,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Will-O-Wisp",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Status = ConditionId.Burn,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.WingAttack] = new()
            {
                Id = MoveId.WingAttack,
                Num = 17,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Wing Attack",
                BasePp = 35,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Distance = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.Wish] = new()
            {
                Id = MoveId.Wish,
                Num = 273,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Wish",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Heal = true,
                    Metronome = true,
                },
                // Note: Wish applies a slot condition that heals after 1 turn - not implemented
                // as slot conditions require special infrastructure not present in current architecture
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Withdraw] = new()
            {
                Id = MoveId.Withdraw,
                Num = 110,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Withdraw",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                SelfBoost = new SparseBoostsTable
                {
                    Def = 1,
                },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Water,
            },
            [MoveId.WonderRoom] = new()
            {
                Id = MoveId.WonderRoom,
                Num = 472,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Wonder Room",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Mirror = true,
                    Metronome = true,
                },
                PseudoWeather = ConditionId.WonderRoom,
                // Note: WonderRoom condition (swapping Def/SpD) should be implemented in Conditions
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Psychic,
            },
            [MoveId.WoodHammer] = new()
            {
                Id = MoveId.WoodHammer,
                Num = 452,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Wood Hammer",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Recoil = (33, 100),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.WorkUp] = new()
            {
                Id = MoveId.WorkUp,
                Num = 526,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Work Up",
                BasePp = 30,
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
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.WorrySeed] = new()
            {
                Id = MoveId.WorrySeed,
                Num = 388,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Worry Seed",
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
                OnTryImmunity = new OnTryImmunityEventInfo((_, target, _, _) =>
                {
                    // Fails against Truant or Insomnia - can't replace those
                    if (target.Ability is AbilityId.Truant or AbilityId.Insomnia)
                    {
                        return false;
                    }
                    return new VoidReturn();
                }),
                OnTryHit = new OnTryHitEventInfo((_, target, _, _) =>
                {
                    // Fails if target's ability can't be suppressed
                    var targetAbility = target.GetAbility();
                    if (targetAbility.Flags.CantSuppress == true)
                    {
                        return false;
                    }
                    return new VoidReturn();
                }),
                OnHit = new OnHitEventInfo((_, target, source, _) =>
                {
                    // Set target's ability to Insomnia
                    var oldAbility = target.SetAbility(AbilityId.Insomnia, source);
                    if (oldAbility != null)
                    {
                        // Cure sleep if the target was asleep
                        if (target.Status == ConditionId.Sleep)
                        {
                            target.CureStatus();
                        }
                        return new VoidReturn();
                    }
                    return false;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.Wrap] = new()
            {
                Id = MoveId.Wrap,
                Num = 35,
                Accuracy = 90,
                BasePower = 15,
                Category = MoveCategory.Physical,
                Name = "Wrap",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.PartiallyTrapped,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },

            // ===== X MOVES =====

            [MoveId.XScissor] = new()
            {
                Id = MoveId.XScissor,
                Num = 404,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "X-Scissor",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Slicing = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
        };
    }
}
