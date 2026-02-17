using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.SideEventMethods;

/// <summary>
/// Event handler info for OnSideResidual event (side-specific).
/// Triggered for residual side condition effects (each turn).
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Side, Pokemon, IEffect) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnSideResidualEventInfo : EventHandlerInfo
{
    public OnSideResidualEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        int? order = null,
 int? subOrder = null,
   bool usesSpeed = true)
    {
        Id = EventId.SideResidual;
  Prefix = EventPrefix.None;
 ContextHandler = contextHandler;
     Priority = priority;
        Order = order;
        SubOrder = subOrder;
        UsesSpeed = usesSpeed;
  }
    
    public static OnSideResidualEventInfo Create(
        Action<Battle, Side, Pokemon, IEffect> handler,
        int? priority = null,
  int? order = null,
   int? subOrder = null,
     bool usesSpeed = true)
    {
  return new OnSideResidualEventInfo(
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
   order,
   subOrder,
            usesSpeed
   );
    }
}
