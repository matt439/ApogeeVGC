using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnDisableMove event.
/// Triggered when moves are disabled.
/// 
/// Supports two handler patterns:
/// 1. Legacy strongly-typed: (Battle, Pokemon) => void
/// 2. Context-based: (EventContext) => RelayVar?
/// </summary>
public sealed record OnDisableMoveEventInfo : EventHandlerInfo
{
    public OnDisableMoveEventInfo(
        EventHandlerDelegate contextHandler,
    int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.DisableMove;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
 /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnDisableMoveEventInfo Create(
  Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnDisableMoveEventInfo(
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
