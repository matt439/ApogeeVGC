using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyAfterEachBoost event (pokemon/ally-specific).
/// Triggered after each boost to an ally.
/// Signature: Action<Battle, SparseBoostsTable, Pokemon, Pokemon>
/// </summary>
public sealed record OnAllyAfterEachBoostEventInfo : EventHandlerInfo
{
    public OnAllyAfterEachBoostEventInfo(
    Action<Battle, SparseBoostsTable, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterEachBoost;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(SparseBoostsTable), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
  }
}