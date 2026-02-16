using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceTryEatItem event.
/// Determines if an item can be consumed.
/// Signature: (Battle battle, Item item, Pokemon pokemon) => BoolVoidUnion | bool
/// </summary>
public sealed record OnSourceTryEatItemEventInfo : UnionEventHandlerInfo<OnTryEatItem>
{
 /// <summary>
    /// Creates a new OnSourceTryEatItem event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSourceTryEatItemEventInfo(
        OnTryEatItem unionValue,
        int? priority = null,
        bool usesSpeed = true)
  {
        Id = EventId.TryEatItem;
   Prefix = EventPrefix.Source;
  UnionValue = unionValue;
        Handler = ExtractDelegate();
      Priority = priority;
   UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
  ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceTryEatItemEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryEatItem;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceTryEatItemEventInfo Create(
        Func<Battle, Item, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceTryEatItemEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetEffectParam<Item>(),
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