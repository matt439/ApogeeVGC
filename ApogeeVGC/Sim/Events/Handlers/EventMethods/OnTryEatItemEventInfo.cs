using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryEatItem event.
/// Triggered when a Pokemon tries to eat an item (like a berry).
/// Signature: (Battle battle, Item item, Pokemon pokemon) => BoolVoidUnion
/// </summary>
public sealed record OnTryEatItemEventInfo : EventHandlerInfo
{
/// <summary>
    /// Creates a new OnTryEatItem event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTryEatItemEventInfo(
        Func<Battle, Item, Pokemon, BoolVoidUnion> handler,
   int? priority = null,
      bool usesSpeed = true)
  {
        Id = EventId.TryEatItem;
     Handler = handler;
  Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
  [
            typeof(Battle),
  typeof(Item),
            typeof(Pokemon),
        ];
  ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
