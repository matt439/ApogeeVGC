using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeAfterSubDamage event.
/// Triggered after damage is dealt to a foe's substitute.
/// Signature: (Battle battle, int damage, Pokemon target, Pokemon source, ActiveMove move) => void
/// </summary>
public sealed record OnFoeAfterSubDamageEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeAfterSubDamage event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnFoeAfterSubDamageEventInfo(
        Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.AfterSubDamage;
  Prefix = EventPrefix.Foe;
     #pragma warning disable CS0618
     Handler = handler;
     #pragma warning restore CS0618
  Priority = priority;
    UsesSpeed = usesSpeed;
 ExpectedParameterTypes =
        [
  typeof(Battle),
    typeof(int),
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
    public OnFoeAfterSubDamageEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterSubDamage;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeAfterSubDamageEventInfo Create(
        Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeAfterSubDamageEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetIntRelayVar(),
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
