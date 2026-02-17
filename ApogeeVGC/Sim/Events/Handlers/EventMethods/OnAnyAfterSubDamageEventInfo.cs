using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyAfterSubDamage event.
/// Signature: Action<Battle, int, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnAnyAfterSubDamageEventInfo : EventHandlerInfo
{
 [Obsolete("Use Create factory method instead.")]
 public OnAnyAfterSubDamageEventInfo(
      Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.AfterSubDamage;
   Prefix = EventPrefix.Any;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
    Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
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
    public OnAnyAfterSubDamageEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterSubDamage;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyAfterSubDamageEventInfo Create(
        Action<Battle, int, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyAfterSubDamageEventInfo(
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
