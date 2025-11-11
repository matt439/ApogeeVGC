using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyBeforeFaint event.
/// Signature: Action<Battle, Pokemon, IEffect>
/// </summary>
public sealed record OnAnyBeforeFaintEventInfo : EventHandlerInfo
{
    public OnAnyBeforeFaintEventInfo(
        Action<Battle, Pokemon, IEffect> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.BeforeFaint;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);
    }
}