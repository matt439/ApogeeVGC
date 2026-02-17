using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSwitchIn event.
/// Triggered when a Pokemon switches in.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnSwitchInEventInfo : EventHandlerInfo
{
    public OnSwitchInEventInfo(
     EventHandlerDelegate contextHandler,
    int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SwitchIn;
        ContextHandler = contextHandler;
        Priority = priority;
    UsesSpeed = usesSpeed;
    }
 
    /// <summary>
  /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
  public static OnSwitchInEventInfo Create(
        Action<Battle, Pokemon> handler,
  int? priority = null,
     bool usesSpeed = true)
    {
        return new OnSwitchInEventInfo(
            context =>
    {
     handler(
context.Battle,
         context.GetTargetOrSourcePokemon()
  );
         return null;
            },
  priority,
      usesSpeed
        );
    }
}
