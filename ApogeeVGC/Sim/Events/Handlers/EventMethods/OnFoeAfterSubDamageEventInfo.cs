using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeAfterSubDamage event.
/// Triggered after damage is dealt to a foe's substitute.
/// Signature: (Battle battle, int damage, Pokemon target, Pokemon source, ActiveMove move) => void
/// </summary>
public sealed record OnFoeAfterSubDamageEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeAfterSubDamage event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeAfterSubDamageEventInfo(
        Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.AfterSubDamage;
  Prefix = EventPrefix.Foe;
     Handler = handler;
  Priority = priority;
    UsesSpeed = usesSpeed;
 ExpectedParameterTypes =
        [
  typeof(Battle),
    typeof(int),
       typeof(Pokemon),
       typeof(Pokemon),
  typeof(ActiveMove),
  ];
 ExpectedReturnType = typeof(void);
    }
}
