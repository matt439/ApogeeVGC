using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyRedirectTarget event.
/// Signature: Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>
/// </summary>
public sealed record OnAnyRedirectTargetEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnyRedirectTargetEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.RedirectTarget;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyRedirectTargetEventInfo Create(
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyRedirectTargetEventInfo(
                        context =>
            {
                // In Showdown: callback(relayVar, target, source, sourceEffect)
                // relayVar = current move target Pokemon
                // target = event target = attacker
                // source = event source = attacker
                // For onAnyRedirectTarget, the first param ('target') is the relayVar
                var moveTarget = context.TryGetRelayVar<PokemonRelayVar>()?.Pokemon
                                 ?? context.GetTargetOrSourcePokemon();
                var result = handler(
                    context.Battle,
                    moveTarget,
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
