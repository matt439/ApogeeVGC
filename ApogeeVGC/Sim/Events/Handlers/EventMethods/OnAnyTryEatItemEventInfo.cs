using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyTryEatItem event.
/// Signature: Func<Battle, Item, Pokemon, BoolVoidUnion>
/// </summary>
public sealed record OnAnyTryEatItemEventInfo : EventHandlerInfo
{
    public OnAnyTryEatItemEventInfo(
        Func<Battle, Item, Pokemon, BoolVoidUnion> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.TryEatItem;
Prefix = EventPrefix.Any;
        Handler = handler;
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}