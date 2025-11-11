using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyEatItem event.
/// Signature: Action<Battle, Item, Pokemon>
/// </summary>
public sealed record OnAnyEatItemEventInfo : EventHandlerInfo
{
    public OnAnyEatItemEventInfo(
        Action<Battle, Item, Pokemon> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.EatItem;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
    }
}