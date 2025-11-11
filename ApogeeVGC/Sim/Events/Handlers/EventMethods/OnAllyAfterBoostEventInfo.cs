using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

public sealed record OnAllyAfterBoostEventInfo : EventHandlerInfo
{
    public OnAllyAfterBoostEventInfo(
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterBoost;
   Prefix = EventPrefix.Ally;
        Handler = handler;
 Priority = priority;
        UsesSpeed = usesSpeed;
 ExpectedParameterTypes = [typeof(Battle), typeof(SparseBoostsTable), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
  ExpectedReturnType = typeof(void);
  }
}
