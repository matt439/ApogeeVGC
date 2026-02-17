using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnChangeBoost event.
/// Triggered when stat boosts are about to be changed.
/// Signature: (Battle battle, SparseBoostsTable boost, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnChangeBoostEventInfo : EventHandlerInfo
{
    /// <summary>
 /// Creates a new OnChangeBoost event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnChangeBoostEventInfo(
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ChangeBoost;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
     [
            typeof(Battle),
            typeof(SparseBoostsTable),
            typeof(Pokemon),
     typeof(Pokemon),
            typeof(IEffect),
  ];
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
    public OnChangeBoostEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ChangeBoost;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnChangeBoostEventInfo Create(
        Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnChangeBoostEventInfo(
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
