using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.SideEventMethods;

/// <summary>
/// Event handler info for OnSideStart event (side-specific).
/// Triggered when a side condition starts.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Side, Pokemon, IEffect) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnSideStartEventInfo : EventHandlerInfo
{
    public OnSideStartEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.SideStart;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnSideStartEventInfo Create(
   Action<Battle, Side, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
      return new OnSideStartEventInfo(
        context =>
    {
      handler(
        context.Battle,
   context.GetTargetSide(),
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
