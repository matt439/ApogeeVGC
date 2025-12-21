using ApogeeVGC.Sim.Moves;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesVWX()
    {
        return new Dictionary<MoveId, Move>
        {
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
