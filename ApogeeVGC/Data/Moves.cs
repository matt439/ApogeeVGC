using System.Collections.ObjectModel;
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
                Condition = _library.Conditions[ConditionId.LeechSeed].Copy(),
                OnTryImmunity = (target) => target.HasType(MoveType.Grass),
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
                //Condition = _library.Conditions[ConditionId.TrickRoom],
                PseudoWeather = _library.PseudoWeathers[PseudoWeatherId.TrickRoom].Copy(),
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
                // TODO: check if protect can be used
                OnPrepareHit = (_, _, _, _) => true,
                OnHit = (_, source, _, context) =>
                {
                    if (source is null)
                    {
                        throw new ArgumentNullException(nameof(source), "Source cannot be null in Protect move.");
                    }
                    source.AddCondition(_library.Conditions[ConditionId.Stall].Copy(), context);
                    return true;
                },
                Condition = _library.Conditions[ConditionId.Protect].Copy(),
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
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





            // Custom moves
            [MoveId.NormalBasic] = new Move
            {
                Id = MoveId.NormalBasic,
                Num = 10001,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Normal Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.IceBasic] = new Move
            {
                Id = MoveId.IceBasic,
                Num = 10002,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Ice Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.DragonBasic] = new Move
            {
                Id = MoveId.DragonBasic,
                Num = 10003,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Dragon Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.FireBasic] = new Move
            {
                Id = MoveId.FireBasic,
                Num = 10004,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Fire Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.BugBasic] = new Move
            {
                Id = MoveId.BugBasic,
                Num = 10005,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Bug Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.FairyBasic] = new Move
            {
                Id = MoveId.FairyBasic,
                Num = 10006,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Fairy Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.GroundBasic] = new Move
            {
                Id = MoveId.GroundBasic,
                Num = 10007,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Ground Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.FightingBasic] = new Move
            {
                Id = MoveId.FightingBasic,
                Num = 10008,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Fighting Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.SteelBasic] = new Move
            {
                Id = MoveId.SteelBasic,
                Num = 10009,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Steel Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.DarkBasic] = new Move
            {
                Id = MoveId.DarkBasic,
                Num = 10010,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Dark Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.ElectricBasic] = new Move
            {
                Id = MoveId.ElectricBasic,
                Num = 10011,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Electric Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.GrassBasic] = new Move
            {
                Id = MoveId.GrassBasic,
                Num = 10012,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Grass Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.PsychicBasic] = new Move
            {
                Id = MoveId.PsychicBasic,
                Num = 10013,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Psychic Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.PoisonBasic] = new Move
            {
                Id = MoveId.PoisonBasic,
                Num = 10014,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Poison Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.FlyingBasic] = new Move
            {
                Id = MoveId.FlyingBasic,
                Num = 10015,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Flying Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Flying,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.RockBasic] = new Move
            {
                Id = MoveId.RockBasic,
                Num = 10016,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Rock Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.WaterBasic] = new Move
            {
                Id = MoveId.WaterBasic,
                Num = 10017,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Water Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
            [MoveId.GhostBasic] = new Move
            {
                Id = MoveId.GhostBasic,
                Num = 10018,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Ghost Basic",
                BasePp = 10,
                Priority = 0,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
                Flags = new MoveFlags
                {
                    Protect = true,
                },
            },
        };
    }
}