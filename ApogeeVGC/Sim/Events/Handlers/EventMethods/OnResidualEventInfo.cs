using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnResidual event.
/// Triggered at the end of each turn for residual effects.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, IEffect) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnResidualEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using legacy strongly-typed pattern.
    /// </summary>
    [Obsolete("Use Create factory method instead.")]
    public OnResidualEventInfo(
        Action<Battle, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        int? order = null,
        int? subOrder = null,
        bool usesSpeed = true)
    {
        Id = EventId.Residual;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
        Order = order;
        SubOrder = subOrder;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(Pokemon),
            typeof(Pokemon),
            typeof(IEffect),
        ];
        ExpectedReturnType = typeof(void);

        // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;

        // Validate configuration
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, SourceEffect
    /// </summary>
    public OnResidualEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        int? order = null,
        int? subOrder = null,
        bool usesSpeed = true)
    {
      Id = EventId.Residual;
        ContextHandler = contextHandler;
        Priority = priority;
        Order = order;
        SubOrder = subOrder;
        UsesSpeed = usesSpeed;
    }
  
    /// <summary>
    /// Creates strongly-typed context-based handler.
 /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnResidualEventInfo Create(
   Action<Battle, Pokemon, Pokemon, IEffect> handler,
 int? priority = null,
        int? order = null,
        int? subOrder = null,
      bool usesSpeed = true)
    {
     return new OnResidualEventInfo(
context =>
            {
           handler(
       context.Battle,
       context.GetTargetOrSourcePokemon(),
context.GetSourceOrTargetPokemon(),
      context.GetSourceEffect<IEffect>()
    );
       return null; // void return
         },
        priority,
            order,
            subOrder,
            usesSpeed
        );
    }
}
