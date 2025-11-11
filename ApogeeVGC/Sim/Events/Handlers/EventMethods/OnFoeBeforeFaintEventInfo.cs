using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeBeforeFaint event.
/// Triggered before a foe Pokemon faints.
/// Signature: (Battle battle, Pokemon pokemon) => void  
/// </summary>
public sealed record OnFoeBeforeFaintEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeBeforeFaint event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
  public OnFoeBeforeFaintEventInfo(
Action<Battle, Pokemon> handler,
        int? priority = null,
 bool usesSpeed = true)
    {
        Id = EventId.BeforeFaint;
        Prefix = EventPrefix.Foe;
        Handler = handler;
     Priority = priority;
  UsesSpeed = usesSpeed;
   ExpectedParameterTypes =
        [
      typeof(Battle),
      typeof(Pokemon),
   ];
   ExpectedReturnType = typeof(void);
    }
}
