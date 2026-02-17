using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceModifyBoost event.
/// Signature: Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>
/// </summary>
public sealed record OnSourceModifyBoostEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceModifyBoostEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyBoost;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceModifyBoostEventInfo Create(
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceModifyBoostEventInfo(
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
