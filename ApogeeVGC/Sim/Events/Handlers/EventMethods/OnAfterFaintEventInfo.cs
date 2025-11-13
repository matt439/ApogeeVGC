using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAfterFaint event.
/// Triggered after a Pokemon faints.
/// Signature: (Battle battle, int length, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnAfterFaintEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAfterFaint event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAfterFaintEventInfo(
   Action<Battle, int, Pokemon, Pokemon, IEffect> handler,
     int? priority = null,
  bool usesSpeed = true)
  {
   Id = EventId.AfterFaint;
        Handler = handler;
   Priority = priority;
        UsesSpeed = usesSpeed;
   ExpectedParameterTypes =
  [
   typeof(Battle),
       typeof(int),
  typeof(Pokemon),
            typeof(Pokemon),
            typeof(IEffect),
  ];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
