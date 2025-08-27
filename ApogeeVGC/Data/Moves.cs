using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using ApogeeVGC.Sim;

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
                Num = 824,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Glacial Lance",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags() { Protect = true, Mirror = true },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Ice,
            },
            [MoveId.LeechSeed] = new()
            {
                Num = 73,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Leech Seed",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags()
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Condition = _library.Conditions[ConditionId.LeechSeed],
                OnTryImmunity = (target) => target.HasType(MoveType.Grass),
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.TrickRoom] = new()
            {
                Num = 433,
                Accuracy = 100,
                AlwaysHit = true,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Trick Room",
                BasePp = 5,
                Priority = -7,
                Flags = new MoveFlags()
                {
                    Mirror = true,
                    Metronome = true,
                },
                Condition = _library.Conditions[ConditionId.TrickRoom],
                Target = MoveTarget.All,
                Type = MoveType.Psychic,
            },
            [MoveId.Protect] = new()
            {
                Num = 182,
                Accuracy = 100,
                AlwaysHit = true,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Protect",
                BasePp = 10,
                Priority = 4,
                Flags = new MoveFlags()
                {
                    NoAssist = true,
                    FailCopycat = true,
                },
                StallingMove = true,
                // OnPrepareHit
                OnHit = (target, source, move) =>
                {
                    target.AddCondition(_library.Conditions[ConditionId.Stall]);
                    return true;
                },
                Condition = _library.Conditions[ConditionId.Protect],
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },





            // Struggle
            [MoveId.Struggle] = new()
            {
                Num = 10000,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Struggle",
                BasePp = 1,
                Priority = 0,
                Flags = new MoveFlags()
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },

            // Custom moves
            [MoveId.NormalBasic] = new Move
            {
                Num = 10001,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Normal Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.IceBasic] = new Move
            {
                Num = 10002,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Ice Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.DragonBasic] = new Move
            {
                Num = 10003,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Dragon Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.FireBasic] = new Move
            {
                Num = 10004,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Fire Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.BugBasic] = new Move
            {
                Num = 10005,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Bug Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.FairyBasic] = new Move
            {
                Num = 10006,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Fairy Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.GroundBasic] = new Move
            {
                Num = 10007,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Ground Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
            },
            [MoveId.FightingBasic] = new Move
            {
                Num = 10008,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Fighting Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.SteelBasic] = new Move
            {
                Num = 10009,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Steel Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.DarkBasic] = new Move
            {
                Num = 10010,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Dark Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.ElectricBasic] = new Move
            {
                Num = 10011,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Electric Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.GrassBasic] = new Move
            {
                Num = 10012,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Grass Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.PsychicBasic] = new Move
            {
                Num = 10013,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Psychic Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PoisonBasic] = new Move
            {
                Num = 10014,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Poison Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.FlyingBasic] = new Move
            {
                Num = 10015,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Flying Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Flying,
            },
            [MoveId.RockBasic] = new Move
            {
                Num = 10016,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Rock Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.WaterBasic] = new Move
            {
                Num = 10017,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Water Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.GhostBasic] = new Move
            {
                Num = 10018,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Ghost Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
        };
    }
}