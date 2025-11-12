using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyTryEatItem event.
/// Determines if an item can be consumed.
/// Signature: (Battle battle, Item item, Pokemon pokemon) => BoolVoidUnion | bool
/// </summary>
public sealed record OnAnyTryEatItemEventInfo : UnionEventHandlerInfo<OnTryEatItem>
{
    /// <summary>
    /// Creates a new OnAnyTryEatItem event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAnyTryEatItemEventInfo(
        OnTryEatItem unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryEatItem;
 Prefix = EventPrefix.Any;
        UnionValue = unionValue;
     Handler = ExtractDelegate();
        Priority = priority;
     UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
    ExpectedReturnType = typeof(BoolVoidUnion);
    }
}