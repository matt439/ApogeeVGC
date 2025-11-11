using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceTakeItem event.
/// Signature: Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion>
/// </summary>
public sealed record OnSourceTakeItemEventInfo : EventHandlerInfo
{
    public OnSourceTakeItemEventInfo(
        Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.TakeItem;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}