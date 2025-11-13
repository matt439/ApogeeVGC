using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyFaint event.
/// Triggered when any Pokemon faints.
/// Signature: (Battle battle, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnAnyFaintEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAnyFaint event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAnyFaintEventInfo(
   Action<Battle, Pokemon, Pokemon, IEffect> handler,
  int? priority = null,
  bool usesSpeed = true)
{
  Id = EventId.Faint;
      Prefix = EventPrefix.Any;
 Handler = handler;
  Priority = priority;
        UsesSpeed = usesSpeed;
  ExpectedParameterTypes =
 [
        typeof(Battle),
  typeof(Pokemon),
  typeof(Pokemon),
 typeof(IEffect),
 ];
  ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
