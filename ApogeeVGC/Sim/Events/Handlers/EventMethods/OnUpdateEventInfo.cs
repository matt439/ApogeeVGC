using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnUpdate event.
/// Triggered during Pokemon update phase.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnUpdateEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnUpdate event handler using the legacy strongly-typed pattern.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnUpdateEventInfo(
   Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
  {
  Id = EventId.Update;
Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
ExpectedParameterTypes =
    [
 typeof(Battle),
typeof(Pokemon),
 ];
        ExpectedReturnType = typeof(void);

    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;

    // Validate configuration
        ValidateConfiguration();
 }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, TargetPokemon
    /// </summary>
    public OnUpdateEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Update;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnUpdateEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnUpdateEventInfo(
            context =>
            {
                handler(
                    context.Battle,
                    context.GetTargetPokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
