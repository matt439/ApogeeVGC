using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTakeItem event.
/// Triggered when an item is taken from a Pokemon.
/// Signature: (Battle, Item, Pokemon, Pokemon, Move?) => PokemonVoidUnion | bool
/// </summary>
public sealed record OnTakeItemEventInfo : UnionEventHandlerInfo<OnTakeItem>
{
    /// <summary>
    /// Creates a new OnTakeItem event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTakeItemEventInfo(
        OnTakeItem unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TakeItem;
        UnionValue = unionValue;
        Handler = ExtractDelegate();
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
            typeof(Item),
            typeof(Pokemon),
            typeof(Pokemon),
            typeof(Move),
        ];
        ExpectedReturnType = typeof(PokemonVoidUnion);

        // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;

        // Validate configuration
        ValidateConfiguration();
    }
}