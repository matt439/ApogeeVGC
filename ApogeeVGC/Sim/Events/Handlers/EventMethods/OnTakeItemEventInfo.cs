using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTakeItem event.
/// Triggered when an item is taken from a Pokemon.
/// Signature: (Battle battle, Item item, Pokemon pokemon, Pokemon source) => BoolVoidUnion
/// </summary>
public sealed record OnTakeItemEventInfo : EventHandlerInfo
{
    /// <summary>
  /// Creates a new OnTakeItem event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTakeItemEventInfo(
  Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion> handler,
 int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.TakeItem;
Handler = handler;
     Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
   [
    typeof(Battle),
  typeof(Item),
  typeof(Pokemon),
    typeof(Pokemon),
  ];
ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
