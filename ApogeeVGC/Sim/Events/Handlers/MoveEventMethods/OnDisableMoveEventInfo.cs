using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnDisableMove event (move-specific).
/// Triggered to disable a move.
/// Signature: Action&lt;Battle, Pokemon&gt;
/// </summary>
public sealed record OnDisableMoveEventInfo : EventHandlerInfo
{
    public OnDisableMoveEventInfo(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.DisableMove;
        Prefix = EventPrefix.None;
Handler = handler;
        Priority = priority;
     UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
    ExpectedReturnType = typeof(void);
    }
}
