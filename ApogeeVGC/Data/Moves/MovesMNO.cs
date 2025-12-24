using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesMno()
    {
        return new Dictionary<MoveId, Move>
        {
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
        };
    }
}
