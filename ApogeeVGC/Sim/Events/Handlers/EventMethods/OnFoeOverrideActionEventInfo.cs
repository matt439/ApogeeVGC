using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeOverrideAction event.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>
/// </summary>
public sealed record OnFoeOverrideActionEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeOverrideActionEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.OverrideAction;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeOverrideActionEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeOverrideActionEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
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
