using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeRedirectTarget event.
/// Redirects move target to a different Pokemon when targeting foes.
/// Signature: (Battle battle, Pokemon target, Pokemon source, IEffect effect, ActiveMove move) => PokemonVoidUnion
/// </summary>
public sealed record OnFoeRedirectTargetEventInfo : EventHandlerInfo
{
    public OnFoeRedirectTargetEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.RedirectTarget;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeRedirectTargetEventInfo Create(
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeRedirectTargetEventInfo(
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
