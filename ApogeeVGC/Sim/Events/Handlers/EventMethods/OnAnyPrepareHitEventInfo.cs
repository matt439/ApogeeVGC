using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyPrepareHit event.
/// Triggered before any move hits in battle.
/// Signature: (Battle battle, Pokemon source, Pokemon target, ActiveMove move) => BoolEmptyVoidUnion?
/// </summary>
public sealed record OnAnyPrepareHitEventInfo : EventHandlerInfo
{
    public OnAnyPrepareHitEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.PrepareHit;
        Prefix = EventPrefix.Any;
        Handler = handler;
    Priority = priority;
        UsesSpeed = usesSpeed;
  ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
   ExpectedReturnType = typeof(BoolEmptyVoidUnion);
    }
}
