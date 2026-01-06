using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Modern interface for ability-specific event methods using strongly-typed EventHandlerInfo records.
/// This replaces IAbilityEventMethods with a type-safe approach that validates delegate signatures at compile-time.
/// Each EventHandlerInfo record contains its own Priority, Order, and SubOrder properties.
/// </summary>
public interface IAbilityEventMethodsV2
{
    /// <summary>
    /// Triggered to check if an ability should be shown/revealed.
    /// </summary>
    OnCheckShowEventInfo? OnCheckShow { get; }

    /// <summary>
  /// Triggered when an ability effect ends.
    /// </summary>
    OnEndEventInfo? OnEnd { get; }

    /// <summary>
    /// Triggered when an ability effect starts/activates.
    /// </summary>
    OnStartEventInfo? OnStart { get; }
}