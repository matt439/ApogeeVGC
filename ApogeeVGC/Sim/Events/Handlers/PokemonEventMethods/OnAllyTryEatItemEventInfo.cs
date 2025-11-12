using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyTryEatItem event (pokemon/ally-specific).
/// Triggered when ally tries to eat item.
/// Signature: Func<Battle, Item, Pokemon, BoolVoidUnion>
/// </summary>
public sealed record OnAllyTryEatItemEventInfo : EventHandlerInfo
{
    public OnAllyTryEatItemEventInfo(
    Func<Battle, Item, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryEatItem;
  Prefix = EventPrefix.Ally;
  Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
        ExpectedReturnType = typeof(BoolVoidUnion);
  }
}