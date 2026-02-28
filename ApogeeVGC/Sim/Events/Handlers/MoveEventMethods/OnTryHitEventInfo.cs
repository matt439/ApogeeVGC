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
  if (result == null) return new NullRelayVar();
  return result switch
 {
       BoolBoolIntEmptyVoidUnion b => (b.Value ? BoolRelayVar.True : BoolRelayVar.False),
    IntBoolIntEmptyVoidUnion i => IntRelayVar.Get(i.Value),
     EmptyBoolIntEmptyVoidUnion => new NullRelayVar(),
 VoidUnionBoolIntEmptyVoidUnion => new VoidReturnRelayVar(),
    _ => new VoidReturnRelayVar()
   };
   },
   priority,
     usesSpeed
  );
    }
}
