using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeModifyBoost event.
/// Signature: Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion>
/// </summary>
public sealed record OnFoeModifyBoostEventInfo : EventHandlerInfo
{
    public OnFoeModifyBoostEventInfo(
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyBoost;
        Prefix = EventPrefix.Foe;
   Handler = handler;
        Priority = priority;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(SparseBoostsTable), typeof(Pokemon)];
        ExpectedReturnType = typeof(SparseBoostsTableVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeModifyBoostEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyBoost;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeModifyBoostEventInfo Create(
        Func<Battle, SparseBoostsTable, Pokemon, SparseBoostsTableVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeModifyBoostEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetRelayVar<SparseBoostsTableRelayVar>().Table,
                context.GetTargetPokemon()
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