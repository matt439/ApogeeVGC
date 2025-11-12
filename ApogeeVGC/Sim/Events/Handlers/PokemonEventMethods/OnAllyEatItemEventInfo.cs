using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyEatItem event (pokemon/ally-specific).
/// Triggered when ally eats item.
/// Signature: Action<Battle, Item, Pokemon>
/// </summary>
public sealed record OnAllyEatItemEventInfo : EventHandlerInfo
{
    public OnAllyEatItemEventInfo(
    Action<Battle, Item, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.EatItem;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
  }
}