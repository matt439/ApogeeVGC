using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnEatItem event.
/// Triggered when a Pokemon eats an item (like a berry).
/// Signature: (Battle battle, Item item, Pokemon pokemon) => void
/// </summary>
public sealed record OnEatItemEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnEatItem event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnEatItemEventInfo(
 Action<Battle, Item, Pokemon> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
Id = EventId.EatItem;
  Handler = handler;
    Priority = priority;
        UsesSpeed = usesSpeed;
 ExpectedParameterTypes =
    [
   typeof(Battle),
   typeof(Item),
            typeof(Pokemon),
     ];
     ExpectedReturnType = typeof(void);
  }
}
