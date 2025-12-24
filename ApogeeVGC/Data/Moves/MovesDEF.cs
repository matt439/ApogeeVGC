using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesDef()
    {
        return new Dictionary<MoveId, Move>
        {
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
                                Self = new SecondaryEffect { Boosts = new SparseBoostsTable { SpA = -2, }, },
                                Secondary = null,
                                Target = MoveTarget.Normal,
                                Type = MoveType.Dragon,
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
                                OnBasePower = new OnBasePowerEventInfo((battle, _, _, target, move) =>
                                {
                                    if (target.RunEffectiveness(move) <= 0.0) return new VoidReturn();
                                    if (battle.DisplayUi)
                                    {
                                        battle.Debug("electro drift super effective buff");
                                    }

                                    battle.ChainModify([5461, 4096]);
                                    return new VoidReturn();
                                }),
                                                Secondary = null,
                                                Target = MoveTarget.Normal,
                                                Type = MoveType.Electric,
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
                                                OnBasePower = new OnBasePowerEventInfo((battle, _, pokemon, _, _) =>
                                                {
                                                    if (pokemon.Status is not ConditionId.None &&
                                                        pokemon.Status != ConditionId.Sleep)
                                                    {
                                                        battle.Debug("[Facade.OnBasePower] Facade is increasing move damage.");
                                                        battle.ChainModify(2);
                                                    }

                                                    return new VoidReturn();
                                                }),
                                                Secondary = null,
                                                Target = MoveTarget.Normal,
                                                Type = MoveType.Normal,
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
                                                OnTry = new OnTryEventInfo((battle, source, _, _) =>
                                                {
                                                    if (source.ActiveMoveActions <= 1) return new VoidReturn();
                                                    if (battle.DisplayUi)
                                                    {
                                                        battle.Hint("Fake out only works on your first turn out.");
                                                    }

                                                    return false;
                                                }),
                                                Secondary = new SecondaryEffect
                                                {
                                                    Chance = 100,
                                                    VolatileStatus = ConditionId.Flinch,
                                                },
                                                Target = MoveTarget.Normal,
                                                Type = MoveType.Normal,
                                            },
                                        };
                                    }
                                }
