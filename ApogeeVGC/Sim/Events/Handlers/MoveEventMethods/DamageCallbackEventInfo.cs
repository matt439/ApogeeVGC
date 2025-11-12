using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for DamageCallback event (move-specific).
/// Callback for calculating damage dynamically.
/// Signature: Func&lt;Battle, Pokemon, Pokemon, ActiveMove, IntFalseUnion&gt;
/// </summary>
public sealed record DamageCallbackEventInfo : EventHandlerInfo
{
    public DamageCallbackEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, IntFalseUnion> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
        Id = EventId.DamageCallback;
   Prefix = EventPrefix.None;
        Handler = handler;
   Priority = priority;
     UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(IntFalseUnion);
    }
}
