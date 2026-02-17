using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceTryBoost event.
/// Signature: Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect>
/// </summary>
public sealed record OnSourceTryBoostEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnSourceTryBoostEventInfo(
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.TryBoost;
        Prefix = EventPrefix.Source;
 #pragma warning disable CS0618
 Handler = handler;
 #pragma warning restore CS0618
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(SparseBoostsTable), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceTryBoostEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryBoost;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceTryBoostEventInfo Create(
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceTryBoostEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetRelayVar<SparseBoostsTableRelayVar>().Table,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<IEffect>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
