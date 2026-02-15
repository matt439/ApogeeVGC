using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyAfterEachBoost event.
/// Signature: Action<Battle, SparseBoostsTable, Pokemon, Pokemon>
/// </summary>
public sealed record OnAnyAfterEachBoostEventInfo : EventHandlerInfo
{
 public OnAnyAfterEachBoostEventInfo(
      Action<Battle, SparseBoostsTable, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.AfterEachBoost;
   Prefix = EventPrefix.Any;
        Handler = handler;
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(SparseBoostsTable), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnyAfterEachBoostEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterEachBoost;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyAfterEachBoostEventInfo Create(
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyAfterEachBoostEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetRelayVar<SparseBoostsTableRelayVar>().Table,
                context.GetTargetPokemon(),
                context.GetSourcePokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}