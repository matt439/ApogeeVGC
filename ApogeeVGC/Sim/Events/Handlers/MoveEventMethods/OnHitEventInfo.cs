using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnHit event (move-specific).
/// Triggered when a move hits.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, ActiveMove) => BoolEmptyVoidUnion?
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnHitEventInfo : EventHandlerInfo
{
    public OnHitEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
  int? priority = null,
  bool usesSpeed = true)
    {
Id = EventId.Hit;
   Prefix = EventPrefix.None;
        Handler = handler;
Priority = priority;
 UsesSpeed = usesSpeed;
   ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(BoolEmptyVoidUnion);
        
// Nullability: All parameters non-nullable by default (adjust as needed)
   ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
     // Validate configuration
  ValidateConfiguration();
}
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, Move
    /// </summary>
  public OnHitEventInfo(
   EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
 {
        Id = EventId.Hit;
  Prefix = EventPrefix.None;
  ContextHandler = contextHandler;
 Priority = priority;
 UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnHitEventInfo Create(
  Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
  int? priority = null,
 bool usesSpeed = true)
    {
return new OnHitEventInfo(
   context =>
   {
      var result = handler(
   context.Battle,
context.GetTargetOrSourcePokemon(),
context.GetSourceOrTargetPokemon(),
 context.GetMove()
  );
   if (result == null) return null;
   return result switch
   {
    BoolBoolEmptyVoidUnion b => new BoolRelayVar(b.Value),
  EmptyBoolEmptyVoidUnion => null,
   VoidUnionBoolEmptyVoidUnion => null,
          _ => null
     };
    },
     priority,
usesSpeed
  );
  }
}