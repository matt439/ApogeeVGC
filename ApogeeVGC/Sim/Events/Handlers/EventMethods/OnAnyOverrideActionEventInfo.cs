using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyOverrideAction event.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>
/// </summary>
public sealed record OnAnyOverrideActionEventInfo : EventHandlerInfo
{
    public OnAnyOverrideActionEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.OverrideAction;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(DelegateVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnyOverrideActionEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.OverrideAction;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyOverrideActionEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyOverrideActionEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetPokemon(),
                context.GetSourcePokemon(),
                context.GetMove()
                );
                return result switch
                {
                    DelegateDelegateVoidUnion d => throw new NotImplementedException("DelegateVoidUnion delegate return not supported in Create pattern"),
                    VoidDelegateVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}