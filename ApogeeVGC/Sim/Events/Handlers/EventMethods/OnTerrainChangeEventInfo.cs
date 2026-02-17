using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTerrainChange event.
/// Triggered when terrain changes.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon, Pokemon, IEffect) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnTerrainChangeEventInfo : EventHandlerInfo
{
    public OnTerrainChangeEventInfo(
        EventHandlerDelegate contextHandler,
int? priority = null,
        bool usesSpeed = true)
    {
Id = EventId.TerrainChange;
        ContextHandler = contextHandler;
     Priority = priority;
   UsesSpeed = usesSpeed;
    }
    
    /// <summary>
/// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnTerrainChangeEventInfo Create(
   Action<Battle, Pokemon, Pokemon, IEffect> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
        return new OnTerrainChangeEventInfo(
  context =>
  {
   handler(
context.Battle,
 context.GetTargetOrSourcePokemon(),
      context.GetSourceOrTargetPokemon(),
  context.GetSourceEffect<IEffect>()
  );
          return null;
            },
     priority,
   usesSpeed
     );
    }
}
