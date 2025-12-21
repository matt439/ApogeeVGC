using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesJKL()
    {
        return new Dictionary<MoveId, Move>
        {
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
                VolatileStatus = ConditionId.LeechSeed,
                Condition = _library.Conditions[ConditionId.LeechSeed],
                OnTryImmunity = new OnTryImmunityEventInfo((_, target, _, _) =>
                    !target.HasType(PokemonType.Grass)),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
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
            [MoveId.LowKick] = new()
            {
                Id = MoveId.LowKick,
                Num = 67,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, target, _) =>
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
                    if (battle.DisplayUi)
                    {
                        battle.Debug($"BP: {bp}");
                    }

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
                // OnTryHit only applies to dynamax
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
        };
    }
}
