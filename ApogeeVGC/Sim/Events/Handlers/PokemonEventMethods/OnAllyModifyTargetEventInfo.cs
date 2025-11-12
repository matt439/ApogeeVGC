using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyModifyTarget event (pokemon/ally-specific).
/// Triggered to modify ally's move target.
/// Signature: Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnAllyModifyTargetEventInfo : EventHandlerInfo
{
    public OnAllyModifyTargetEventInfo(
    Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyTarget;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
  }
}