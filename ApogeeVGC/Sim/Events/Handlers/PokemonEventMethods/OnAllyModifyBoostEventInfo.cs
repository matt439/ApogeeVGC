using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyModifyBoost event (pokemon/ally-specific).
/// Triggered to modify ally's boosts.
/// Signature: Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>
/// </summary>
public sealed record OnAllyModifyBoostEventInfo : EventHandlerInfo
{
    public OnAllyModifyBoostEventInfo(
    Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyBoost;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(SparseBoostsTable), typeof(Pokemon)];
        ExpectedReturnType = typeof(SparseBoostsTableVoidUnion);
  }
}