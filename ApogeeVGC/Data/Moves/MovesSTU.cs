using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesSTU()
    {
        return new Dictionary<MoveId, Move>
        {
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
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    move.Type = MoveType.Fighting;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon, "move: Struggle");
                    }
                }),
                StruggleRecoil = true,
                Secondary = null,
                Target = MoveTarget.RandomNormal,
                Type = MoveType.Normal,
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
                        };
                    }
                }
