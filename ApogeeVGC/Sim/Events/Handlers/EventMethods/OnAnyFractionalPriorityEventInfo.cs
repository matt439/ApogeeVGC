using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyFractionalPriority event.
/// Modifies move priority with fractional adjustment.
/// Signature: (Battle battle, int priority, Pokemon source, Pokemon? target, ActiveMove move) => double
/// </summary>
public sealed record OnAnyFractionalPriorityEventInfo : UnionEventHandlerInfo<OnFractionalPriority>
{
    /// <summary>
    /// Creates a new OnAnyFractionalPriority event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or double constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAnyFractionalPriorityEventInfo(
        OnFractionalPriority unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.FractionalPriority;
        Prefix = EventPrefix.Any;
        UnionValue = unionValue;
        Handler = ExtractDelegate();
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(double);
        
        // Nullability: target (4th param) is nullable since it's passed as null
        ParameterNullability = [false, false, false, true, false];
        ReturnTypeNullable = false;
    
        // Validate configuration
        ValidateConfiguration();
    }
}