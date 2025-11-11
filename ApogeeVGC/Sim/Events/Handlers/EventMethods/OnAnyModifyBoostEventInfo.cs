using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyModifyBoost event.
/// Signature: Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>
/// </summary>
public sealed record OnAnyModifyBoostEventInfo : EventHandlerInfo
{
    public OnAnyModifyBoostEventInfo(
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.ModifyBoost;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(SparseBoostsTable), typeof(Pokemon)];
        ExpectedReturnType = typeof(SparseBoostsTableVoidUnion);
    }
}