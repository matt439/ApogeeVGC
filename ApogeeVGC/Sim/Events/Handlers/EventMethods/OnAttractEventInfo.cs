using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAttract event.
/// Triggered when a Pokemon becomes attracted.
/// Signature: (Battle battle, Pokemon target, Pokemon source) => void
/// </summary>
public sealed record OnAttractEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAttract event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAttractEventInfo(
        Action<Battle, Pokemon, Pokemon> handler,
  int? priority = null,
     bool usesSpeed = true)
  {
        Id = EventId.Attract;
   Handler = handler;
     Priority = priority;
        UsesSpeed = usesSpeed;
    ExpectedParameterTypes =
  [
  typeof(Battle),
            typeof(Pokemon),
    typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(void);
    }
}
