using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.FieldEventMethods;

/// <summary>
/// Event handler info for OnFieldStart event (field-specific).
/// Triggered when a field condition starts.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Field, Pokemon, IEffect) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnFieldStartEventInfo : EventHandlerInfo
{
    public OnFieldStartEventInfo(
        EventHandlerDelegate contextHandler,
  int? priority = null,
bool usesSpeed = true)
    {
      Id = EventId.FieldStart;
   Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
UsesSpeed = usesSpeed;
    }
    
    /// <summary>
  /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnFieldStartEventInfo Create(
        Action<Battle, Field, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   return new OnFieldStartEventInfo(
     context =>
          {
       handler(
   context.Battle,
  context.Battle.Field,
 context.GetTargetOrSourcePokemon(),
     context.GetSourceEffect<IEffect>()
     );
      return null;
   },
        priority,
    usesSpeed
        );
    }
}
