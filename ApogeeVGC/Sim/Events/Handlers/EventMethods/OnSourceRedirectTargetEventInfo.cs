using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceRedirectTarget event.
/// Signature: Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>
/// </summary>
public sealed record OnSourceRedirectTargetEventInfo : EventHandlerInfo
{
    public OnSourceRedirectTargetEventInfo(
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.RedirectTarget;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(IEffect), typeof(ActiveMove)];
        ExpectedReturnType = typeof(PokemonVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceRedirectTargetEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.RedirectTarget;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceRedirectTargetEventInfo Create(
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceRedirectTargetEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetPokemon(),
                context.GetSourcePokemon(),
                context.GetSourceEffect<IEffect>(),
                context.GetMove()
                );
                return result switch
                {
                    PokemonPokemonVoidUnion p => new PokemonRelayVar(p.Pokemon),
                    VoidPokemonVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}