using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnTryImmunity event (move-specific).
/// Triggered to check immunity.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, ActiveMove) => BoolEmptyVoidUnion?
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnTryImmunityEventInfo : EventHandlerInfo
{
    /// <summary>
  /// Creates event handler using context-based pattern.
  /// Context provides: Battle, TargetPokemon, SourcePokemon, Move
 /// </summary>
    public OnTryImmunityEventInfo(
EventHandlerDelegate contextHandler,
    int? priority = null,
        bool usesSpeed = true)
  {
   Id = EventId.TryImmunity;
  Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
  /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
 /// </summary>
    public static OnTryImmunityEventInfo Create(
  Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
        int? priority = null,
 bool usesSpeed = true)
    {
    return new OnTryImmunityEventInfo(
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
