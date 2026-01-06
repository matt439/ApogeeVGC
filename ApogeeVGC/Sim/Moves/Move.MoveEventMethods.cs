using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

namespace ApogeeVGC.Sim.Moves;

public partial record Move : IMoveEventMethods
{
    public BasePowerCallbackEventInfo? BasePowerCallback { get; init; }
    public BeforeMoveCallbackEventInfo? BeforeMoveCallback { get; init; }
    public BeforeTurnCallbackEventInfo? BeforeTurnCallback { get; init; }
    public DamageCallbackEventInfo? DamageCallback { get; init; }
    public PriorityChargeCallbackEventInfo? PriorityChargeCallback { get; init; }
    public OnDisableMoveEventInfo? OnDisableMove { get; init; }
    public OnAfterHitEventInfo? OnAfterHit { get; init; }
    public OnAfterSubDamageEventInfo? OnAfterSubDamage { get; init; }
    public OnAfterMoveSecondarySelfEventInfo? OnAfterMoveSecondarySelf { get; init; }
    public OnAfterMoveSecondaryEventInfo? OnAfterMoveSecondary { get; init; }
    public OnAfterMoveEventInfo? OnAfterMove { get; init; }
    public OnDamageEventInfo? OnDamage { get; init; }
    public OnBasePowerEventInfo? OnBasePower { get; init; }
    public OnEffectivenessEventInfo? OnEffectiveness { get; init; }
    public OnHitEventInfo? OnHit { get; init; }
    public OnHitFieldEventInfo? OnHitField { get; init; }
    public OnHitSideEventInfo? OnHitSide { get; init; }
    public OnModifyMoveEventInfo? OnModifyMove { get; init; }
    public OnModifyPriorityEventInfo? OnModifyPriority { get; init; }
    public OnMoveFailEventInfo? OnMoveFail { get; init; }
    public OnModifyTypeEventInfo? OnModifyType { get; init; }
    public OnModifyTargetEventInfo? OnModifyTarget { get; init; }
    public OnPrepareHitEventInfo? OnPrepareHit { get; init; }
    public OnTryEventInfo? OnTry { get; init; }
    public OnTryHitEventInfo? OnTryHit { get; init; }
    public OnTryHitFieldEventInfo? OnTryHitField { get; init; }
    public OnTryHitSideEventInfo? OnTryHitSide { get; init; }
    public OnTryImmunityEventInfo? OnTryImmunity { get; init; }
    public OnTryMoveEventInfo? OnTryMove { get; init; }
    public OnUseMoveMessageEventInfo? OnUseMoveMessage { get; init; }
}