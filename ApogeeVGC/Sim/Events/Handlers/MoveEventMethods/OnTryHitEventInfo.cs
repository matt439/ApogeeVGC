using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnTryHit event (move-specific).
/// Triggered to try hitting with a move.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, ActiveMove) => BoolIntEmptyVoidUnion?
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnTryHitEventInfo : EventHandlerInfo
{
    public OnTryHitEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
    Id = EventId.TryHit;
        Prefix = EventPrefix.None;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
  ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
  ExpectedReturnType = typeof(BoolIntEmptyVoidUnion);
        
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
    public OnTryHitEventInfo(
  EventHandlerDelegate contextHandler,
        int? priority = null,
bool usesSpeed = true)
  {
        Id = EventId.TryHit;
        Prefix = EventPrefix.None;
ContextHandler = contextHandler;
Priority = priority;
 UsesSpeed = usesSpeed;
    }
    
    /// <summary>
  /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnTryHitEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?> handler,
int? priority = null,
bool usesSpeed = true)
    {
        return new OnTryHitEventInfo(
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
       BoolBoolIntEmptyVoidUnion b => new BoolRelayVar(b.Value),
    IntBoolIntEmptyVoidUnion i => new IntRelayVar(i.Value),
     EmptyBoolIntEmptyVoidUnion => null,
 VoidUnionBoolIntEmptyVoidUnion => null,
    _ => null
   };
   },
   priority,
     usesSpeed
  );
    }
}