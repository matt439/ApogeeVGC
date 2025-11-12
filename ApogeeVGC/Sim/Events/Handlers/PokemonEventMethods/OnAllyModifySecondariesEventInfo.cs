using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyModifySecondaries event (pokemon/ally-specific).
/// Triggered to modify ally's move secondary effects.
/// Signature: Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnAllyModifySecondariesEventInfo : EventHandlerInfo
{
    public OnAllyModifySecondariesEventInfo(
    Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifySecondaries;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(List<SecondaryEffect>), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
  }
}