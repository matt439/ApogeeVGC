using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;

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
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.TrickRoom] = new()
            {
                Id = MoveId.TrickRoom,
                Num = 433,
                Accuracy = 100,
                AlwaysHit = true,
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
                Target = MoveTarget.Field,
                Type = MoveType.Psychic,
            },
            [MoveId.Protect] = new()
            {
                Id = MoveId.Protect,
                Num = 182,
                Accuracy = 100,
                AlwaysHit = true,
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
                Condition = _library.Conditions[ConditionId.Protect],
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
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.Tailwind] = new()
            {
                Id = MoveId.Tailwind,
                Num = 366,
                Accuracy = 100,
                AlwaysHit = true,
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
                Condition = _library.Conditions[ConditionId.Paralysis],
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.Reflect] = new()
            {
                Id = MoveId.Reflect,
                Num = 115,
                Accuracy = 100,
                AlwaysHit = true,
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
                Target = MoveTarget.AllySide,
                Type = MoveType.Psychic,
            },
            [MoveId.LightScreen] = new()
            {
                Id = MoveId.LightScreen,
                Num = 113,
                Accuracy = 100,
                AlwaysHit = true,
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
                Condition = _library.Conditions[ConditionId.Flinch],
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.HeavySlam] = new()
            {
                Id = MoveId.HeavySlam,
                Num = 484,
                Accuracy = 100,
                BasePower = 0,
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
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.LowKick] = new()
            {
                Id = MoveId.LowKick,
                Num = 67,
                Accuracy = 100,
                BasePower = 0,
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
                Recoil = 1.0 / 4.0,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },




            // Struggle
            [MoveId.Struggle] = new()
            {
                Id = MoveId.Struggle,
                Num = 10000,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Struggle",
                BasePp = 1,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },





            //// Custom moves
            //[MoveId.NormalBasic] = new Move
            //{
            //    Id = MoveId.NormalBasic,
            //    Num = 10001,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Normal Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Normal,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.IceBasic] = new Move
            //{
            //    Id = MoveId.IceBasic,
            //    Num = 10002,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Ice Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Ice,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.DragonBasic] = new Move
            //{
            //    Id = MoveId.DragonBasic,
            //    Num = 10003,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Dragon Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Dragon,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.FireBasic] = new Move
            //{
            //    Id = MoveId.FireBasic,
            //    Num = 10004,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Fire Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Fire,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.BugBasic] = new Move
            //{
            //    Id = MoveId.BugBasic,
            //    Num = 10005,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Bug Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Bug,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.FairyBasic] = new Move
            //{
            //    Id = MoveId.FairyBasic,
            //    Num = 10006,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Fairy Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Fairy,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.GroundBasic] = new Move
            //{
            //    Id = MoveId.GroundBasic,
            //    Num = 10007,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Ground Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Ground,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.FightingBasic] = new Move
            //{
            //    Id = MoveId.FightingBasic,
            //    Num = 10008,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Fighting Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Fighting,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.SteelBasic] = new Move
            //{
            //    Id = MoveId.SteelBasic,
            //    Num = 10009,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Steel Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Steel,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.DarkBasic] = new Move
            //{
            //    Id = MoveId.DarkBasic,
            //    Num = 10010,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Dark Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Dark,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.ElectricBasic] = new Move
            //{
            //    Id = MoveId.ElectricBasic,
            //    Num = 10011,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Electric Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Electric,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.GrassBasic] = new Move
            //{
            //    Id = MoveId.GrassBasic,
            //    Num = 10012,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Grass Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Grass,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.PsychicBasic] = new Move
            //{
            //    Id = MoveId.PsychicBasic,
            //    Num = 10013,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Psychic Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Psychic,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.PoisonBasic] = new Move
            //{
            //    Id = MoveId.PoisonBasic,
            //    Num = 10014,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Poison Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Poison,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.FlyingBasic] = new Move
            //{
            //    Id = MoveId.FlyingBasic,
            //    Num = 10015,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Flying Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Flying,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.RockBasic] = new Move
            //{
            //    Id = MoveId.RockBasic,
            //    Num = 10016,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Physical,
            //    Name = "Rock Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Rock,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.WaterBasic] = new Move
            //{
            //    Id = MoveId.WaterBasic,
            //    Num = 10017,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Water Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Water,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
            //[MoveId.GhostBasic] = new Move
            //{
            //    Id = MoveId.GhostBasic,
            //    Num = 10018,
            //    Accuracy = 90,
            //    BasePower = 70,
            //    Category = MoveCategory.Special,
            //    Name = "Ghost Basic",
            //    BasePp = 10,
            //    Priority = 0,
            //    Target = MoveTarget.Normal,
            //    Type = MoveType.Ghost,
            //    Flags = new MoveFlags
            //    {
            //        Protect = true,
            //    },
            //},
        };
    }
}