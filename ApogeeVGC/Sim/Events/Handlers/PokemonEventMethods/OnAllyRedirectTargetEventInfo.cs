using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyRedirectTarget event (pokemon/ally-specific).
/// Triggered to redirect target from ally.
/// Signature: Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion>
/// </summary>
public sealed record OnAllyRedirectTargetEventInfo : EventHandlerInfo
{
    public OnAllyRedirectTargetEventInfo(
    Func<Battle, Pokemon, Pokemon, IEffect, ActiveMove, PokemonVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.RedirectTarget;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(IEffect), typeof(ActiveMove)];
        ExpectedReturnType = typeof(PokemonVoidUnion);
  }
}