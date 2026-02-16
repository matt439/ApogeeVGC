using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyTakeItem event.
/// Triggered when an item is taken from a Pokémon.
/// Signature: (Battle battle, Item item, Pokemon source, Pokemon target) => PokemonVoidUnion | Pokemon?
/// </summary>
public sealed record OnAnyTakeItemEventInfo : UnionEventHandlerInfo<OnTakeItem>
{
    /// <summary>
    /// Creates a new OnAnyTakeItem event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or Pokemon constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAnyTakeItemEventInfo(
        OnTakeItem unionValue,
        int? priority = null,
    bool usesSpeed = true)
    {
Id = EventId.TakeItem;
        Prefix = EventPrefix.Any;
        UnionValue = unionValue;
 Handler = ExtractDelegate();
        Priority = priority;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnyTakeItemEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TakeItem;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyTakeItemEventInfo Create(
        Func<Battle, Item, Pokemon, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyTakeItemEventInfo(
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