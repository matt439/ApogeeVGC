using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyTakeItem event (pokemon/ally-specific).
/// Triggered when an item is taken from a Pokémon.
/// Signature: (Battle battle, Item item, Pokemon source, Pokemon target) => PokemonVoidUnion | Pokemon?
/// </summary>
public sealed record OnAllyTakeItemEventInfo : UnionEventHandlerInfo<OnTakeItem>
{
    public OnAllyTakeItemEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TakeItem;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyTakeItemEventInfo Create(
        Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyTakeItemEventInfo(
            context =>
            {
                var result = handler(
                    context.Battle,
                    context.GetEffectParam<Item>(),
                    context.GetSourceOrTargetPokemon(),
                    context.GetTargetOrSourcePokemon()
                );
                return result switch
                {
                    BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidBoolVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
