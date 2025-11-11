using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnBeforeFaint event.
/// Triggered before a Pokemon faints.
/// Signature: (Battle battle, Pokemon pokemon, IEffect effect) => void
/// </summary>
public sealed record OnBeforeFaintEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnBeforeFaint event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
  /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnBeforeFaintEventInfo(
      Action<Battle, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeFaint;
        Handler = handler;
    Priority = priority;
   UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
   typeof(Pokemon),
    typeof(IEffect),
        ];
        ExpectedReturnType = typeof(void);
    }
}
