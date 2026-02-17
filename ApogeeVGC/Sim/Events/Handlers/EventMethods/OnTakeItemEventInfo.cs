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
