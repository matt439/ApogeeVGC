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
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
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
