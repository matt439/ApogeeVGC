using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesPQR()
    {
        return new Dictionary<MoveId, Move>
        {
            [MoveId.Protect] = new()
            {
                Id = MoveId.Protect,
                Num = 182,
                Accuracy = IntTrueUnion.FromTrue(),
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
                VolatileStatus = ConditionId.Protect,

                OnPrepareHit = new OnPrepareHitEventInfo((battle, _, source, _) =>
                {
                    // source is the Pokemon using Protect
                    // Always run both checks, let Stall condition handle the logic
                    bool willAct = battle.Queue.WillAct() is not null;
                    RelayVar? stallResult = battle.RunEvent(EventId.StallMove, source);
                    bool stallSuccess = stallResult is BoolRelayVar { Value: true };
                    bool result = willAct && stallSuccess;

                    // Return BoolEmptyVoidUnion explicitly
                    return result ? true : (BoolEmptyVoidUnion)false;
                }),

                OnHit = new OnHitEventInfo((battle, _, source, _) =>
                {
                    // source is the Pokemon using Protect
                    battle.Debug(
                        $"[Protect.OnHit] BEFORE AddVolatile: {source.Name} has Stall volatile = {source.Volatiles.ContainsKey(ConditionId.Stall)}");

                    source.AddVolatile(ConditionId.Stall);

                    battle.Debug(
                        $"[Protect.OnHit] AFTER AddVolatile: {source.Name} has Stall volatile = {source.Volatiles.ContainsKey(ConditionId.Stall)}");
                    if (source.Volatiles.TryGetValue(ConditionId.Stall, out var stallState))
                    {
                        battle.Debug(
                            $"[Protect.OnHit] Stall volatile state: Counter={stallState.Counter}, Duration={stallState.Duration}");
                    }

                    return new VoidReturn();
                }),
                Condition = _library.Conditions[ConditionId.Protect],
                                Secondary = null,
                                Target = MoveTarget.Self,
                                Type = MoveType.Normal,
                            },
                            [MoveId.Reflect] = new()
                            {
                                Id = MoveId.Reflect,
                                Num = 115,
                                Accuracy = IntTrueUnion.FromTrue(),
                                BasePower = 0,
                                Category = MoveCategory.Status,
                                Name = "Reflect",
                                BasePp = 20,
                                Priority = 0,
                                Flags = new MoveFlags
                                {
                                    Snatch = true,
                                    Metronome = true,
                                },
                                SideCondition = ConditionId.Reflect,
                                Condition = _library.Conditions[ConditionId.Reflect],
                                Secondary = null,
                                Target = MoveTarget.AllySide,
                                Type = MoveType.Psychic,
                            },
                        };
                    }
                }
