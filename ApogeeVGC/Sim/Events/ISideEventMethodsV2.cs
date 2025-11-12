using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Modern interface for side-specific event methods using strongly-typed EventHandlerInfo records.
/// This replaces ISideEventMethods with a type-safe approach that validates delegate signatures at compile-time.
/// Each EventHandlerInfo record contains its own Priority, Order, and SubOrder properties.
/// </summary>
public interface ISideEventMethodsV2 : IEventMethodsV2
{
    /// <summary>
    /// Triggered when a side condition starts.
    /// </summary>
    OnSideStartEventInfo? OnSideStart { get; }

    /// <summary>
 /// Triggered when a side condition restarts/reactivates.
    /// </summary>
    OnSideRestartEventInfo? OnSideRestart { get; }

  /// <summary>
    /// Triggered for residual side condition effects (each turn).
    /// </summary>
    OnSideResidualEventInfo? OnSideResidual { get; }

    /// <summary>
    /// Triggered when a side condition ends.
    /// </summary>
    OnSideEndEventInfo? OnSideEnd { get; }
}
