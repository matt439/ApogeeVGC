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
    /// <summary>
    /// Creates a new OnAllyTakeItem event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or Pokemon constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnAllyTakeItemEventInfo(
        OnTakeItem unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TakeItem;
        Prefix = EventPrefix.Ally;
        UnionValue = unionValue;
        #pragma warning disable CS0618
        Handler = ExtractDelegate();
        #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
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
