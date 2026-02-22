using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnTry event (move-specific).
/// Triggered to try executing a move.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, ActiveMove) => BoolEmptyVoidUnion?
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnTryEventInfo : EventHandlerInfo
{
    /// <summary>
  /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, Move
    /// </summary>
    public OnTryEventInfo(
  EventHandlerDelegate contextHandler,
  int? priority = null,
        bool usesSpeed = true)
    {
     Id = EventId.Try;
     Prefix = EventPrefix.None;
     ContextHandler = contextHandler;
Priority = priority;
      UsesSpeed = usesSpeed;
    }
    
 /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnTryEventInfo Create(
  Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
    int? priority = null,
  bool usesSpeed = true)
    {
    return new OnTryEventInfo(
  context =>
     {
  var result = handler(
       context.Battle,
  context.GetTargetOrSourcePokemon(),
   context.GetSourceOrTargetPokemon(),
    context.GetMove()
  );
    // Handler null = TS null ("failed silently") → NullRelayVar so SingleEvent
    // returns it as-is instead of falling back to relayVar (truthy).
    if (result == null) return new NullRelayVar();
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
