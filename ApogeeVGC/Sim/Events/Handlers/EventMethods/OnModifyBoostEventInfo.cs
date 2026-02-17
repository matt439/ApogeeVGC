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
    public OnModifyBoostEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyBoost;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnModifyBoostEventInfo Create(
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnModifyBoostEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetSparseBoostsTableRelayVar(),
                context.GetTargetOrSourcePokemon()
                );
                return result switch
                {
                    SparseBoostsTableSparseBoostsTableVoidUnion s => new SparseBoostsTableRelayVar(s.Table),
                    VoidSparseBoostsTableVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
