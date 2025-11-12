using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyTakeItem event (pokemon/ally-specific).
/// Triggered when taking ally's item.
/// Signature: Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion>
/// </summary>
public sealed record OnAllyTakeItemEventInfo : EventHandlerInfo
{
    public OnAllyTakeItemEventInfo(
    Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TakeItem;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(BoolVoidUnion);
  }
}