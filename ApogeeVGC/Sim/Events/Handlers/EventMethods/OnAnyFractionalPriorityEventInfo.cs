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
        ExpectedReturnType = typeof(DoubleVoidUnion);
        
        // Nullability: target (4th param) is nullable since it's passed as null
        ParameterNullability = [false, false, false, true, false];
        ReturnTypeNullable = false;
    
        // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnyFractionalPriorityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.FractionalPriority;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyFractionalPriorityEventInfo Create(
        Func<Battle, int, Pokemon, Pokemon?, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyFractionalPriorityEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetRelayVar<IntRelayVar>().Value,
                context.GetSourcePokemon(),
                context.TargetPokemon,
                context.GetMove()
                );
                return result switch
                {
                    DoubleDoubleVoidUnion d => new DecimalRelayVar((decimal)d.Value),
                    VoidDoubleVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}