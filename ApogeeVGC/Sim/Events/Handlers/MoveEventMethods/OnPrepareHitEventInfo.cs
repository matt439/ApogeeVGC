using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for OnPrepareHit event (move-specific).
/// Triggered to prepare a hit.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, ActiveMove) => BoolEmptyVoidUnion?
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnPrepareHitEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon, SourcePokemon, Move
    /// </summary>
    public OnPrepareHitEventInfo(
    EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.PrepareHit;
  Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
     Priority = priority;
   UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnPrepareHitEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolEmptyVoidUnion?> handler,
int? priority = null,
bool usesSpeed = true)
    {
        return new OnPrepareHitEventInfo(
context =>
   {
     var result = handler(
    context.Battle,
       context.GetTargetOrSourcePokemon(),
   context.GetSourceOrTargetPokemon(),
    context.GetMove()
      );
   // null = TS null (silent fail), VoidReturn = TS undefined (passthrough)
   if (result == null) return new NullRelayVar();
       return result switch
     {
     BoolBoolEmptyVoidUnion b => new BoolRelayVar(b.Value),
     EmptyBoolEmptyVoidUnion => new NullRelayVar(),
     VoidUnionBoolEmptyVoidUnion => null,
     _ => null
        };
      },
 priority,
        usesSpeed
  );
  }
}
