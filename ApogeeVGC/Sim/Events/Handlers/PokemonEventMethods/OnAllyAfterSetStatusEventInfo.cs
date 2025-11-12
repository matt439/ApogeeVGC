using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyAfterSetStatus event (pokemon/ally-specific).
/// Triggered after status is set on ally.
/// Signature: Action<Battle, Condition, Pokemon, Pokemon, IEffect>
/// </summary>
public sealed record OnAllyAfterSetStatusEventInfo : EventHandlerInfo
{
    public OnAllyAfterSetStatusEventInfo(
    Action<Battle, Condition, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterSetStatus;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Condition), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);
  }
}