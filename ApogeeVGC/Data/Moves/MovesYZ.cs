using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesYz()
    {
        return new Dictionary<MoveId, Move>
        {
            // ===== Y MOVES =====

            [MoveId.Yawn] = new()
            {
                Id = MoveId.Yawn,
                Num = 281,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Yawn",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.Yawn,
                OnTryHit = OnTryHitEventInfo.Create((_, target, _, _) =>
                {
                    // Fail if target already has a status or is immune to sleep
                    if (target.Status != ConditionId.None ||
                        !target.RunStatusImmunity(ConditionId.Sleep))
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
                Condition = _library.Conditions[ConditionId.Yawn],
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },

            // ===== Z MOVES =====

            [MoveId.ZapCannon] = new()
            {
                Id = MoveId.ZapCannon,
                Num = 192,
                Accuracy = 50,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Zap Cannon",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bullet = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.ZenHeadbutt] = new()
            {
                Id = MoveId.ZenHeadbutt,
                Num = 428,
                Accuracy = 90,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Zen Headbutt",
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
                Type = MoveType.Psychic,
            },
            [MoveId.ZingZap] = new()
            {
                Id = MoveId.ZingZap,
                Num = 716,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Zing Zap",
                BasePp = 10,
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
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
        };
    }
}