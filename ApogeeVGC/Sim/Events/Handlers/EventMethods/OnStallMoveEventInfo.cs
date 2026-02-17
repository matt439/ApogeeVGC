using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnStallMove event.
/// Triggered to check if a stall move (Protect, Detect, etc.) succeeds.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon) => BoolVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnStallMoveEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnStallMove event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnStallMoveEventInfo(
        Func<Battle, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.StallMove;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
   typeof(Battle),
         typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
 /// Context provides: Battle, TargetPokemon
    /// </summary>
    public OnStallMoveEventInfo(
        EventHandlerDelegate contextHandler,
 int? priority = null,
  bool usesSpeed = true)
    {
        Id = EventId.StallMove;
        ContextHandler = contextHandler;
   Priority = priority;
  UsesSpeed = usesSpeed;
    }
    
  /// <summary>
    /// Creates strongly-typed context-based handler.
/// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnStallMoveEventInfo Create(
Func<Battle, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
  return new OnStallMoveEventInfo(
  context =>
  {
 var result = handler(
  context.Battle,
    context.GetTargetOrSourcePokemon()
     );
 return result switch
 {
   BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
 VoidBoolVoidUnion => null,
     _ => null
 };
   },
   priority,
  usesSpeed
   );
    }
}
