using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.ItemSpecific;

/// <summary>
/// Event handler info for Item.OnStart event.
/// Signature: (Battle, Pokemon) => void
/// </summary>
public sealed record OnStartEventInfo : EventHandlerInfo
{
    public OnStartEventInfo(Action<Battle, Pokemon>? handler)
    {
        Id = EventId.Start;
        Handler = handler;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
    }
}