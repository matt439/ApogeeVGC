using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyOverrideAction event (pokemon/ally-specific).
/// Triggered to override ally action.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion>
/// </summary>
public sealed record OnAllyOverrideActionEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyOverrideActionEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.OverrideAction;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyOverrideActionEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyOverrideActionEventInfo(
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
