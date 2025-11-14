using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifySpA event.
/// Modifies the Special Attack stat.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, int, Pokemon, Pokemon, ActiveMove) => DoubleVoidUnion
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnModifySpAEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifySpA event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifySpAEventInfo(
 Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
      int? priority = null,
     bool usesSpeed = true)
    {
        Id = EventId.ModifySpA;
        Handler = handler;
      Priority = priority;
        UsesSpeed = usesSpeed;
     ExpectedParameterTypes =
        [
         typeof(Battle),
         typeof(int),
      typeof(Pokemon),
     typeof(Pokemon),
     typeof(ActiveMove),
   ];
      ExpectedReturnType = typeof(DoubleVoidUnion);
        
        // Nullability: All parameters non-nullable by default (adjust as needed)
   ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
   // Validate configuration
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, RelayVar (int value), SourcePokemon, TargetPokemon, Move
  /// </summary>
  public OnModifySpAEventInfo(
        EventHandlerDelegate contextHandler,
   int? priority = null,
  bool usesSpeed = true)
    {
     Id = EventId.ModifySpA;
    ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
  /// Creates strongly-typed context-based handler.
  /// Best of both worlds: strongly-typed parameters + context performance.
  /// </summary>
    public static OnModifySpAEventInfo Create(
    Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   return new OnModifySpAEventInfo(
     context =>
   {
    var result = handler(
      context.Battle,
   context.GetRelayVar<IntRelayVar>().Value,
   context.GetSourcePokemon(),
 context.GetTargetPokemon(),
   context.GetMove()
       );
      return result switch
    {
          DoubleDoubleVoidUnion d => new DecimalRelayVar((decimal)d.Value),
     VoidDoubleVoidUnion => null,
           _ => null
       };
         },
 priority,
   usesSpeed
        );
    }
}
