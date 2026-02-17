using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnOverrideAction event.
/// Overrides the action a Pokemon will take.
/// Signature: (Battle battle, Pokemon pokemon, Pokemon target, ActiveMove move) => DelegateVoidUnion
/// </summary>
public sealed record OnOverrideActionEventInfo : EventHandlerInfo
{
    public OnOverrideActionEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.OverrideAction;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnOverrideActionEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnOverrideActionEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetSourceOrTargetPokemon(),
                context.GetTargetOrSourcePokemon(),
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
