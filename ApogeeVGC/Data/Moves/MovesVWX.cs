using ApogeeVGC.Sim.Conditions;
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
                // TODO: onTry - fail if target's move is not priority or is Status category
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
                VolatileStatus = ConditionId.Uproar,
                Condition = _library.Conditions[ConditionId.Uproar],
                // TODO: onTryHit - wake up all sleeping Pokemon on both sides
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
                // TODO: onBasePower - double power if target is poisoned
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
        };
    }
}
