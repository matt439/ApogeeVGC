using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Modern interface for move-specific event methods using strongly-typed EventHandlerInfo records.
/// This replaces IMoveEventMethods with a type-safe approach that validates delegate signatures at compile-time.
/// Each EventHandlerInfo record contains its own Priority, Order, and SubOrder properties.
/// </summary>
public interface IMoveEventMethodsV2
{
    // Callback Events
    /// <summary>
    /// Callback for calculating base power dynamically.
    /// </summary>
    BasePowerCallbackEventInfo? BasePowerCallback { get; }

    /// <summary>
    /// Callback executed before a move is used.
    /// </summary>
    BeforeMoveCallbackEventInfo? BeforeMoveCallback { get; }

    /// <summary>
 /// Callback executed before a turn begins.
    /// </summary>
    BeforeTurnCallbackEventInfo? BeforeTurnCallback { get; }

    /// <summary>
    /// Callback for calculating damage dynamically.
    /// </summary>
    DamageCallbackEventInfo? DamageCallback { get; }

    /// <summary>
    /// Callback for charging moves with priority calculation.
    /// </summary>
    PriorityChargeCallbackEventInfo? PriorityChargeCallback { get; }

    // Standard Move Events
    /// <summary>
    /// Triggered to disable a move.
    /// </summary>
    OnDisableMoveEventInfo? OnDisableMove { get; }

    /// <summary>
    /// Triggered after a move hits.
    /// </summary>
    OnAfterHitEventInfo? OnAfterHit { get; }

    /// <summary>
    /// Triggered after substitute damage.
    /// </summary>
    OnAfterSubDamageEventInfo? OnAfterSubDamage { get; }

    /// <summary>
/// Triggered after move secondary effects on self.
    /// </summary>
    OnAfterMoveSecondarySelfEventInfo? OnAfterMoveSecondarySelf { get; }

    /// <summary>
    /// Triggered after move secondary effects.
    /// </summary>
    OnAfterMoveSecondaryEventInfo? OnAfterMoveSecondary { get; }

 /// <summary>
    /// Triggered after a move completes.
    /// </summary>
    OnAfterMoveEventInfo? OnAfterMove { get; }

    /// <summary>
    /// Triggered to modify damage.
    /// </summary>
    OnDamageEventInfo? OnDamage { get; }

    /// <summary>
    /// Triggered to modify base power.
    /// </summary>
    OnBasePowerEventInfo? OnBasePower { get; }

    /// <summary>
    /// Triggered to modify type effectiveness.
    /// </summary>
    OnEffectivenessEventInfo? OnEffectiveness { get; }

    /// <summary>
    /// Triggered when a move hits.
    /// </summary>
    OnHitEventInfo? OnHit { get; }

    /// <summary>
    /// Triggered when a move hits the field.
    /// </summary>
    OnHitFieldEventInfo? OnHitField { get; }

    /// <summary>
    /// Triggered when a move hits a side.
    /// </summary>
    OnHitSideEventInfo? OnHitSide { get; }

    /// <summary>
    /// Triggered to modify a move.
/// </summary>
  OnModifyMoveEventInfo? OnModifyMove { get; }

    /// <summary>
    /// Triggered to modify move priority.
    /// </summary>
    OnModifyPriorityEventInfo? OnModifyPriority { get; }

    /// <summary>
    /// Triggered when a move fails.
    /// </summary>
    OnMoveFailEventInfo? OnMoveFail { get; }

    /// <summary>
    /// Triggered to modify move type.
    /// </summary>
    OnModifyTypeEventInfo? OnModifyType { get; }

    /// <summary>
    /// Triggered to modify move target.
    /// </summary>
 OnModifyTargetEventInfo? OnModifyTarget { get; }

    /// <summary>
    /// Triggered to prepare a hit.
    /// </summary>
    OnPrepareHitEventInfo? OnPrepareHit { get; }

    /// <summary>
    /// Triggered to try executing a move.
 /// </summary>
    OnTryEventInfo? OnTry { get; }

    /// <summary>
    /// Triggered to try hitting with a move.
    /// </summary>
    OnTryHitEventInfo? OnTryHit { get; }

    /// <summary>
    /// Triggered to try hitting the field.
    /// </summary>
    OnTryHitFieldEventInfo? OnTryHitField { get; }

    /// <summary>
    /// Triggered to try hitting a side.
    /// </summary>
    OnTryHitSideEventInfo? OnTryHitSide { get; }

    /// <summary>
    /// Triggered to check immunity.
  /// </summary>
    OnTryImmunityEventInfo? OnTryImmunity { get; }

    /// <summary>
    /// Triggered to try using a move.
    /// </summary>
    OnTryMoveEventInfo? OnTryMove { get; }

    /// <summary>
    /// Triggered to display move use message.
  /// </summary>
    OnUseMoveMessageEventInfo? OnUseMoveMessage { get; }
}
