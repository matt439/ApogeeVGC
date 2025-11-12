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
        Func<Battle, Pokemon, Pokemon?, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeMoveCallback;
        Prefix = EventPrefix.None;
        Handler = handler;
    Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
