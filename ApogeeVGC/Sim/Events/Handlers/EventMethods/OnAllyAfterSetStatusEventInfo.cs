using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

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
