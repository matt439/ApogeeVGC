using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAfterSubDamage event.
/// Triggered after damage is dealt to a substitute.
/// Signature: (Battle battle, int damage, Pokemon target, Pokemon source, ActiveMove move) => void
/// </summary>
public sealed record OnAfterSubDamageEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnAfterSubDamage event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAfterSubDamageEventInfo(
   Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterSubDamage;
     Handler = handler;
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
    public OnAfterSubDamageEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterSubDamage;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAfterSubDamageEventInfo Create(
        Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAfterSubDamageEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetRelayVar<IntRelayVar>().Value,
                context.GetTargetPokemon(),
                context.GetSourcePokemon(),
                context.GetMove()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
