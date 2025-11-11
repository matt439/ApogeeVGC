using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyTryHeal event.
/// Signature: Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?>
/// </summary>
public sealed record OnAnyTryHealEventInfo : EventHandlerInfo
{
    public OnAnyTryHealEventInfo(
        Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.TryHeal;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(IntBoolVoidUnion);
    }
}