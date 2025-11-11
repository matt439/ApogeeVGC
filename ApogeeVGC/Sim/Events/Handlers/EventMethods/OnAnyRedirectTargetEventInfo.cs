using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyRedirectTarget event.
/// Signature: Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>
/// </summary>
public sealed record OnAnyRedirectTargetEventInfo : EventHandlerInfo
{
    public OnAnyRedirectTargetEventInfo(
        Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.RedirectTarget;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(IEffect), typeof(ActiveMove)];
        ExpectedReturnType = typeof(PokemonVoidUnion);
    }
}