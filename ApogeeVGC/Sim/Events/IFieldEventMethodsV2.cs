using ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Modern interface for field-specific event methods using strongly-typed EventHandlerInfo records.
/// This replaces IFieldEventMethods with a type-safe approach that validates delegate signatures at compile-time.
/// Each EventHandlerInfo record contains its own Priority, Order, and SubOrder properties.
/// </summary>
public interface IFieldEventMethodsV2 : IEventMethodsV2
{
    /// <summary>
    /// Triggered when a field condition starts.
    /// </summary>
    OnFieldStartEventInfo? OnFieldStart { get; }

    /// <summary>
    /// Triggered when a field condition restarts/reactivates.
    /// </summary>
    OnFieldRestartEventInfo? OnFieldRestart { get; }

    /// <summary>
    /// Triggered for residual field condition effects (each turn).
    /// </summary>
    OnFieldResidualEventInfo? OnFieldResidual { get; }

    /// <summary>
    /// Triggered when a field condition ends.
    /// </summary>
    OnFieldEndEventInfo? OnFieldEnd { get; }
}
