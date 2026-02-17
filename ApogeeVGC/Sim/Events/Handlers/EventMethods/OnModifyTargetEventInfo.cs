using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyTarget event.
/// Modifies the target of a move.
/// Signature: (Battle battle, Pokemon relayTarget, Pokemon pokemon, Pokemon target, ActiveMove move) => void
/// </summary>
public sealed record OnModifyTargetEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifyTarget event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnModifyTargetEventInfo(
        Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove> handler,
  int? priority = null,
bool usesSpeed = true)
    {
      Id = EventId.ModifyTarget;
  #pragma warning disable CS0618
  Handler = handler;
  #pragma warning restore CS0618
        Priority = priority;
      UsesSpeed = usesSpeed;
ExpectedParameterTypes =
 [
        typeof(Battle),
            typeof(Pokemon),
      typeof(Pokemon),
    typeof(Pokemon),
     typeof(ActiveMove),
        ];
     ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
 }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnModifyTargetEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyTarget;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnModifyTargetEventInfo Create(
        Action<Battle, Pokemon, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnModifyTargetEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetSourceOrTargetPokemon(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetMove()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
