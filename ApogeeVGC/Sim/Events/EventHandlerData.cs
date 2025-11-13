using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events;

/// <summary>
/// Represents a dynamically registered event handler with its metadata.
/// Corresponds to the TypeScript event handler structure where each handler has:
/// - target (the effect)
/// - callback (the function)
/// - priority, order, subOrder (for sorting)
/// </summary>
public record EventHandlerData
{
    /// <summary>
    /// The effect that owns this handler
    /// </summary>
    public required IEffect Target { get; init; }

    /// <summary>
    /// The delegate to invoke when the event fires
    /// </summary>
    public required EffectDelegate Callback { get; init; }

    /// <summary>
    /// Priority for event ordering (higher = earlier execution)
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// Order for event execution
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Sub-order for fine-grained event ordering
    /// </summary>
    public int SubOrder { get; init; }
}
