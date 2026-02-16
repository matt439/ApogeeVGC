using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTakeItem event.
/// Triggered when an item is taken from a Pokemon.
/// Signature: (Battle, Item, Pokemon, Pokemon, Move?) => BoolVoidUnion | bool
/// Returns false to prevent item removal, true to allow it.
/// </summary>
public sealed record OnTakeItemEventInfo : UnionEventHandlerInfo<OnTakeItem>
{
    /// <summary>
    /// Creates a new OnTakeItem event handler using the legacy union pattern.
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
        ExpectedReturnType = typeof(BoolVoidUnion);

        // Nullability: All parameters non-nullable except Move which is optional
        ParameterNullability = [false, false, false, false, true];
        ReturnTypeNullable = false;

        // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, Effect (Item), TargetPokemon, SourcePokemon, Move
    /// </summary>
    public OnTakeItemEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TakeItem;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnTakeItemEventInfo Create(
        Func<Battle, Item, Pokemon, Pokemon, Move?, RelayVar?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnTakeItemEventInfo(
            context => handler(
                context.Battle,
                context.GetEffectParam<Item>(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.Move
            ),
            priority,
            usesSpeed
        );
    }
}
