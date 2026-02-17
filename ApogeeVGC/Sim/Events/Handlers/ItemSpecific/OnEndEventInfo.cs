using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.ItemSpecific;

/// <summary>
/// Event handler info for Item.OnEnd event.
/// Signature: (Battle, Pokemon) => void
/// </summary>
public sealed record OnEndEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnEndEventInfo(Action<Battle, Pokemon>? handler)
    {
        Id = EventId.End;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
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
    public OnEndEventInfo(EventHandlerDelegate contextHandler)
    {
        Id = EventId.End;
        ContextHandler = contextHandler;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnEndEventInfo Create(Action<Battle, Pokemon> handler)
    {
        return new OnEndEventInfo(
            context =>
            {
                handler(context.Battle, context.GetTargetOrSourcePokemon());
                return null;
            }
        );
    }
}
