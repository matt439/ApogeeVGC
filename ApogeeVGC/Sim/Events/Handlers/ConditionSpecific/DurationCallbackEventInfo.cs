using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;

public sealed record DurationCallbackEventInfo : EventHandlerInfo
{
    public DurationCallbackEventInfo(
        Func<Battle, Pokemon, Pokemon, IEffect?, int> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.DurationCallback;
        Prefix = EventPrefix.None;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(int);
    }
}