using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeAfterFaint event.
/// Triggered after a foe Pokemon faints.
/// Signature: (Battle battle, int length, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnFoeAfterFaintEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeAfterFaint event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeAfterFaintEventInfo(
    Action<Battle, int, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
      Id = EventId.AfterFaint;
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
