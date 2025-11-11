using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyBoost event.
/// Modifies stat boosts before they are applied.
/// Signature: (Battle battle, SparseBoostsTable boosts, Pokemon pokemon) => SparseBoostsTableVoidUnion
/// </summary>
public sealed record OnModifyBoostEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifyBoost event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyBoostEventInfo(
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyBoost;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
   [
        typeof(Battle),
     typeof(SparseBoostsTable),
            typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(SparseBoostsTableVoidUnion);
    }
}
