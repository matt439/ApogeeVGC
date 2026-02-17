using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.ItemSpecific;

/// <summary>
/// Event handler info for Item.OnStart event.
/// Signature: (Battle, Pokemon) => void
/// </summary>
public sealed record OnStartEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnStart event handler for items using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnStartEventInfo(
        Action<Battle, Pokemon>? handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Start;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
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
    public OnStartEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Start;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnStartEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnStartEventInfo(
            context =>
            {
                handler(context.Battle, context.GetTargetOrSourcePokemon());
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
