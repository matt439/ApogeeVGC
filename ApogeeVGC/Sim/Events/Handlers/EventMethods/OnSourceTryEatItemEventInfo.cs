using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceTryEatItem event.
/// Signature: Func<Battle, Item, Pokemon, BoolVoidUnion>
/// </summary>
public sealed record OnSourceTryEatItemEventInfo : EventHandlerInfo
{
    public OnSourceTryEatItemEventInfo(
        Func<Battle, Item, Pokemon, BoolVoidUnion> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.TryEatItem;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}