using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesGhi()
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
        };
    }
}
