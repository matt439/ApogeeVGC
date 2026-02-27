using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for BeforeMoveCallback event (move-specific).
/// Callback executed before a move is used.
/// Signature: Func&lt;Battle, Pokemon, Pokemon?, ActiveMove, BoolVoidUnion&gt;
/// </summary>
public sealed record BeforeMoveCallbackEventInfo : EventHandlerInfo
{
    public BeforeMoveCallbackEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeMoveCallback;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static BeforeMoveCallbackEventInfo Create(
        Func<Battle, Pokemon, Pokemon?, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new BeforeMoveCallbackEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetTargetOrSourcePokemon(),
                    context.SourcePokemon,
                    context.GetMove()
                );
                return result switch
                {
                    BoolBoolVoidUnion b => (b.Value ? BoolRelayVar.True : BoolRelayVar.False),
                    VoidBoolVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
