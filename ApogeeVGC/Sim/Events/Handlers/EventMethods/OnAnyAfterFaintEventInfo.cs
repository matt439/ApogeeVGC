using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyAfterFaint event.
/// Signature: Action<Battle, int, Pokemon, Pokemon, IEffect>
/// </summary>
public sealed record OnAnyAfterFaintEventInfo : EventHandlerInfo
{
    public OnAnyAfterFaintEventInfo(
        Action<Battle, int, Pokemon, Pokemon, IEffect> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.AfterFaint;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}