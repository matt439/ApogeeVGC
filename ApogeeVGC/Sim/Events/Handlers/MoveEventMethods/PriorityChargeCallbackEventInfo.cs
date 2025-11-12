using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for PriorityChargeCallback event (move-specific).
/// Callback for charging moves with priority calculation.
/// Signature: Action&lt;Battle, Pokemon&gt;
/// </summary>
public sealed record PriorityChargeCallbackEventInfo : EventHandlerInfo
{
    public PriorityChargeCallbackEventInfo(
        Action<Battle, Pokemon> handler,
      int? priority = null,
        bool usesSpeed = true)
    {
    Id = EventId.PriorityChargeCallback;
        Prefix = EventPrefix.None;
        Handler = handler;
 Priority = priority;
 UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
 ExpectedReturnType = typeof(void);
 }
}
