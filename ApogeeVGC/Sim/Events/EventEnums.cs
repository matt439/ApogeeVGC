namespace ApogeeVGC.Sim.Events;

public enum EventId
{
    // From CommonHandlers
    ModifierEffectHandler,
    ModifierMoveHandler,
    ResultMoveHandler,
    ExtResultMoveHandler,
    VoidEffectHandler,
    VoidMoveHandler,
    ModifierSourceEffectHandler,
    ModifierSourceMoveHandler,
    ResultSourceMoveHandler,
    ExtResultSourceMoveHandler,
    VoidSourceEffectHandler,
    VoidSourceMoveHandler,
    
    // Move-specific event handlers (not covered by CommonHandlers)
    BasePowerCallback,
    BeforeMoveCallback,
    BeforeTurnCallback,
    DamageCallback,
    PriorityChargeCallback,
    OnDisableMove,
    OnAfterSubDamage,
    OnDamage,
    OnEffectiveness,
    OnHitSide,
    OnModifyMove,
    OnModifyType,
    OnModifyTarget,
    OnTryHitSide,
    OnMoveFail,
    OnUseMoveMessage,
    
    // Additional handlers that don't match CommonHandlers exactly
    OnDamagePriority, // This is an int property, not a delegate
    OnTryHitField,
    OnTryImmunity,
    OnTry,
    OnTryHit,
    OnPrepareHit,
    OnTryMove,
    
    // Note: Many handlers in the interface comments map to CommonHandlers:
    // OnAfterHit -> VoidSourceMoveHandler
    // OnAfterMoveSecondarySelf -> VoidSourceMoveHandler  
    // OnAfterMoveSecondary -> VoidMoveHandler
    // OnAfterMove -> VoidSourceMoveHandler
    // OnBasePower -> ModifierSourceMoveHandler
    // OnHit -> ResultMoveHandler
    // OnHitField -> ResultMoveHandler
    // OnModifyPriority -> ModifierSourceMoveHandler
    // OnPrepareHit -> ResultMoveHandler
    // OnTry -> ResultSourceMoveHandler
    // OnTryHit -> ExtResultSourceMoveHandler
    // OnTryHitField -> ResultMoveHandler
    // OnTryImmunity -> ResultMoveHandler
    // OnTryMove -> ResultSourceMoveHandler
    // OnUseMoveMessage -> VoidSourceMoveHandler
}