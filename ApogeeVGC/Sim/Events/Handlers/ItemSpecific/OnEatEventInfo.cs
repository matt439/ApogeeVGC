using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.ItemSpecific;

/// <summary>
/// Event handler info for Item.OnEat event.
/// Union type: Action&lt;Battle, Pokemon&gt; | false
/// </summary>
public sealed record OnEatEventInfo : UnionEventHandlerInfo<OnItemEatUse>
{
    public OnEatEventInfo(OnItemEatUse? handler)
    {
        Id = EventId.Eat;
        UnionValue = handler;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);

    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false };
        ReturnTypeNullable = false;

    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon
    /// </summary>
    public OnEatEventInfo(EventHandlerDelegate contextHandler)
    {
        Id = EventId.Eat;
        ContextHandler = contextHandler;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnEatEventInfo Create(Action<Battle, Pokemon> handler)
    {
        return new OnEatEventInfo(
            context =>
            {
                handler(context.Battle, context.GetTargetOrSourcePokemon());
                return null;
            }
        );
    }
}
