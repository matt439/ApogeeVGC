using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyDeductPp event (pokemon/ally-specific).
/// Triggered when deducting ally's PP.
/// Signature: Func<Battle, Pokemon, Pokemon, IntVoidUnion>
/// </summary>
public sealed record OnAllyDeductPpEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyDeductPpEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DeductPp;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyDeductPpEventInfo Create(
        Func<Battle, Pokemon, Pokemon, IntVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyDeductPpEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetTargetOrSourcePokemon(),
                    context.GetSourceOrTargetPokemon()
                );
                return result switch
                {
                    IntIntVoidUnion i => new IntRelayVar(i.Value),
                    VoidIntVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
