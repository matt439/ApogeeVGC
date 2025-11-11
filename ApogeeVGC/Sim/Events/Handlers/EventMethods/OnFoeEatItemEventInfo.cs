using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeEatItem event.
/// Signature: Action<Battle, Item, Pokemon>
/// </summary>
public sealed record OnFoeEatItemEventInfo : EventHandlerInfo
{
    public OnFoeEatItemEventInfo(
        Action<Battle, Item, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.EatItem;
        Prefix = EventPrefix.Foe;
   Handler = handler;
        Priority = priority;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
    }
}