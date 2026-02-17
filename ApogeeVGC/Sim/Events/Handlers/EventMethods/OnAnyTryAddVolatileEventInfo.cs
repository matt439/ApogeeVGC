using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyTryAddVolatile event.
/// Signature: Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?>
/// </summary>
public sealed record OnAnyTryAddVolatileEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnAnyTryAddVolatileEventInfo(
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.TryAddVolatile;
Prefix = EventPrefix.Any;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Condition), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnyTryAddVolatileEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryAddVolatile;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyTryAddVolatileEventInfo Create(
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyTryAddVolatileEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetEffectParam<Condition>(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<IEffect>()
                );
                return result switch
                {
                    BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidBoolVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
