using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Data;

public record Moves
{
    public IReadOnlyDictionary<MoveId, Move> MovesData { get; }
    private readonly Library _library;

    public Moves(Library library)
    {
        _library = library;
        MovesData = new ReadOnlyDictionary<MoveId, Move>(CreateMoves());
    }

    private Dictionary<MoveId, Move> CreateMoves()
    {
        return new Dictionary<MoveId, Move>
        {
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
            [MoveId.LeechSeed] = new()
            {
                Id = MoveId.LeechSeed,
                Num = 73,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Leech Seed",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Condition = _library.Conditions[ConditionId.LeechSeed],
                OnTryImmunity = (ResultMoveHandler)((_, target, _, _) =>
                    !target.HasType(PokemonType.Grass)),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.TrickRoom] = new()
            {
                Id = MoveId.TrickRoom,
                Num = 433,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Trick Room",
                BasePp = 5,
                Priority = -7,
                Flags = new MoveFlags
                {
                    Mirror = true,
                    Metronome = true,
                },
                PseudoWeather = ConditionId.TrickRoom,
                Condition = _library.Conditions[ConditionId.TrickRoom],
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Psychic,
            },
            [MoveId.Protect] = new()
            {
                Id = MoveId.Protect,
                Num = 182,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Protect",
                BasePp = 10,
                Priority = 4,
                Flags = new MoveFlags
                {
                    NoAssist = true,
                    FailCopycat = true,
                },
                StallingMove = true,
                VolatileStatus = ConditionId.Protect,
                OnPrepareHit = (ResultMoveHandler)((battle, pokemon, _, _) => battle.Queue.WillAct() is null &&
                                                          battle.RunEvent(EventId.StallMove, pokemon) is not null),
                OnHit = (ResultMoveHandler)((_, pokemon, _, _) =>
                {
                    pokemon.AddVolatile(ConditionId.Stall);
                    return new VoidReturn();
                }),
                Condition = _library.Conditions[ConditionId.Protect],
                Secondary = null,
                Target = MoveTarget.Self,
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
            [MoveId.DazzlingGleam] = new()
            {
                Id = MoveId.DazzlingGleam,
                Num = 605,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Dazzling Gleam",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Fairy,
            },
            [MoveId.ElectroDrift] = new()
            {
                Id = MoveId.ElectroDrift,
                Num = 879,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Electro Drift",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags()
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                },
                OnBasePower = (ModifierSourceMoveHandler)((battle, _, _, target, move) =>
                {
                    if (target.RunEffectiveness(move) > 0.0)
                    {
                        return battle.ChainModify([5461, 4096]);
                    }
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.DracoMeteor] = new()
            {
                Id = MoveId.DracoMeteor,
                Num = 434,
                Accuracy = 90,
                BasePower = 130,
                Category = MoveCategory.Special,
                Name = "Draco Meteor",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect { Boosts = new SparseBoostsTable { SpA = -2, }, },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.Facade] = new()
            {
                Id = MoveId.Facade,
                Num = 263,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Facade",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnBasePower = (ModifierSourceMoveHandler)((battle, _, pokemon, _, _) =>
                {
                    if (pokemon.Status is not ConditionId.None && pokemon.Status != ConditionId.Sleep)
                    {
                        return battle.ChainModify(2);
                    }
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Crunch] = new()
            {
                Id = MoveId.Crunch,
                Num = 242,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Crunch",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bite = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    Boosts = new SparseBoostsTable { Def = -1, },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
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
            [MoveId.StruggleBug] = new()
            {
                Id = MoveId.StruggleBug,
                Num = 522,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Struggle Bug",
                BasePp = 20,
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
                    Boosts = new SparseBoostsTable { SpA = -1 },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Bug,
            },
            [MoveId.Overheat] = new()
            {
                Id = MoveId.Overheat,
                Num = 315,
                Accuracy = 90,
                BasePower = 130,
                Category = MoveCategory.Special,
                Name = "Overheat",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { SpA = -2, },
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.Tailwind] = new()
            {
                Id = MoveId.Tailwind,
                Num = 366,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Tailwind",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                    Wind = true,
                },
                SideCondition = ConditionId.Tailwind,
                Condition = _library.Conditions[ConditionId.Tailwind],
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Flying,
            },
            [MoveId.SpiritBreak] = new()
            {
                Id = MoveId.SpiritBreak,
                Num = 789,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Spirit Break",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { SpA = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.ThunderWave] = new()
            {
                Id = MoveId.ThunderWave,
                Num = 87,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Thunder Wave",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Status = ConditionId.Paralysis,
                IgnoreImmunity = false,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.Reflect] = new()
            {
                Id = MoveId.Reflect,
                Num = 115,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Reflect",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                SideCondition = ConditionId.Reflect,
                Condition = _library.Conditions[ConditionId.Reflect],
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Psychic,
            },
            [MoveId.LightScreen] = new()
            {
                Id = MoveId.LightScreen,
                Num = 113,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Light Screen",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                SideCondition = ConditionId.LightScreen,
                Condition = _library.Conditions[ConditionId.LightScreen],
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Psychic,
            },
            [MoveId.FakeOut] = new()
            {
                Id = MoveId.FakeOut,
                Num = 252,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Fake Out",
                BasePp = 10,
                Priority = 3,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnTry = (ResultSourceMoveHandler)((_, source, _, _) =>
                {
                    if (source.ActiveMoveActions > 1)
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
                Type = MoveType.Normal,
            },
            [MoveId.HeavySlam] = new()
            {
                Id = MoveId.HeavySlam,
                Num = 484,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = (BasePowerCallbackHandler)((_, pokemon, target, _) =>
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
                // OnTryHit
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.LowKick] = new()
            {
                Id = MoveId.LowKick,
                Num = 67,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = (BasePowerCallbackHandler)((_, _, target, _) =>
                {
                    int targetWeight = target.GetWeight();
                    int bp = targetWeight switch
                    {
                        >= 2000 => 120,
                        >= 1000 => 100,
                        >= 500 => 80,
                        >= 250 => 60,
                        >= 100 => 40,
                        _ => 20,
                    };
                    return bp;
                }),
                Category = MoveCategory.Physical,
                Name = "Low Kick",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // OnTryHit
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
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
                Recoil = (1,  4),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },




            // Struggle
            [MoveId.Struggle] = new()
            {
                Id = MoveId.Struggle,
                Num = 165,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Struggle",
                BasePp = 1,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    FailEncore = true,
                    FailMeFirst = true,
                    NoSleepTalk = true,
                    NoAssist = true,
                    FailCopycat = true,
                    FailMimic = true,
                    FailInstruct = true,
                    NoSketch = true,
                },
                OnModifyMove = (OnModifyMoveHandler)((battle, move, pokemon, _) =>
                {
                    move.Type = MoveType.Fighting;
                    if (battle.PrintDebug)
                    {
                        UiGenerator.PrintActivateEvent(pokemon,
                            _library.Moves[MoveId.Struggle].ToActiveMove());
                    }
                }),
                StruggleRecoil = true,
                Secondary = null,
                Target = MoveTarget.RandomNormal,
                Type = MoveType.Normal,
            },
            //[MoveId.Confused] = new()
            //{
            //    Choice = MoveId.Confused,
            //    Num = 10019,
            //    Accuracy = IntTrueUnion.FromTrue(),
            //    Name = "Confused",
            //    Id = MoveType.Normal,
            //},





            //// Custom moves
            //[MoveId.NormalBasic] = new Move
            //{
            //    Choice = MoveId.NormalBasic,
            //    Num = 10001,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Normal Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Normal,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.IceBasic] = new Move
            //{
            //    Choice = MoveId.IceBasic,
            //    Num = 10002,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Ice Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Ice,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.DragonBasic] = new Move
            //{
            //    Choice = MoveId.DragonBasic,
            //    Num = 10003,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Dragon Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Dragon,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.FireBasic] = new Move
            //{
            //    Choice = MoveId.FireBasic,
            //    Num = 10004,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Fire Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Fire,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.BugBasic] = new Move
            //{
            //    Choice = MoveId.BugBasic,
            //    Num = 10005,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Bug Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Bug,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.FairyBasic] = new Move
            //{
            //    Choice = MoveId.FairyBasic,
            //    Num = 10006,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Fairy Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Fairy,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.GroundBasic] = new Move
            //{
            //    Choice = MoveId.GroundBasic,
            //    Num = 10007,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Ground Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Ground,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.FightingBasic] = new Move
            //{
            //    Choice = MoveId.FightingBasic,
            //    Num = 10008,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Fighting Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Fighting,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.SteelBasic] = new Move
            //{
            //    Choice = MoveId.SteelBasic,
            //    Num = 10009,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Steel Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Steel,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.DarkBasic] = new Move
            //{
            //    Choice = MoveId.DarkBasic,
            //    Num = 10010,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Dark Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Dark,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.ElectricBasic] = new Move
            //{
            //    Choice = MoveId.ElectricBasic,
            //    Num = 10011,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Electric Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Electric,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.GrassBasic] = new Move
            //{
            //    Choice = MoveId.GrassBasic,
            //    Num = 10012,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Grass Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Grass,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.PsychicBasic] = new Move
            //{
            //    Choice = MoveId.PsychicBasic,
            //    Num = 10013,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Psychic Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Psychic,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.PoisonBasic] = new Move
            //{
            //    Choice = MoveId.PoisonBasic,
            //    Num = 10014,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Poison Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Poison,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.FlyingBasic] = new Move
            //{
            //    Choice = MoveId.FlyingBasic,
            //    Num = 10015,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Flying Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Flying,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.RockBasic] = new Move
            //{
            //    Choice = MoveId.RockBasic,
            //    Num = 10016,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Rock Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Rock,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.WaterBasic] = new Move
            //{
            //    Choice = MoveId.WaterBasic,
            //    Num = 10017,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Water Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Water,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.GhostBasic] = new Move
            //{
            //    Choice = MoveId.GhostBasic,
            //    Num = 10018,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Ghost Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Id = MoveType.Ghost,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
        };
    }
}