using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;

/// <summary>
/// Event handler info for PriorityChargeCallback event (move-specific).
/// Callback for charging moves with priority calculation.
/// Signature: Action&lt;Battle, Pokemon&gt;
/// </summary>
public sealed record PriorityChargeCallbackEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public PriorityChargeCallbackEventInfo(
        Action<Battle, Pokemon> handler,
      int? priority = null,
        bool usesSpeed = true)
    {
    Id = EventId.PriorityChargeCallback;
        Prefix = EventPrefix.None;
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

    public PriorityChargeCallbackEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.PriorityChargeCallback;
        Prefix = EventPrefix.None;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    public static PriorityChargeCallbackEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new PriorityChargeCallbackEventInfo(
            context =>
            {
                handler(
                    context.Battle,
                    context.GetTargetOrSourcePokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
