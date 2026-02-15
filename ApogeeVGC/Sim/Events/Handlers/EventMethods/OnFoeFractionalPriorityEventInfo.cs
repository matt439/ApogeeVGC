using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeFractionalPriority event.
/// Signature: (Battle, int, Pokemon, Pokemon?, ActiveMove) => double | decimal constant
/// </summary>
public sealed record OnFoeFractionalPriorityEventInfo : UnionEventHandlerInfo<OnFractionalPriority>
{
    public OnFoeFractionalPriorityEventInfo(
        OnFractionalPriority unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.FractionalPriority;
        Prefix = EventPrefix.Foe;
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
    public OnFoeFractionalPriorityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.FractionalPriority;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeFractionalPriorityEventInfo Create(
        Func<Battle, int, Pokemon, Pokemon?, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeFractionalPriorityEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetRelayVar<IntRelayVar>().Value,
                context.GetTargetPokemon(),
                context.SourcePokemon,
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