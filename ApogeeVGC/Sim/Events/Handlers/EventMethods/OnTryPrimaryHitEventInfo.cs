using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryPrimaryHit event.
/// Triggered when attempting a primary hit.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => IntBoolVoidUnion?
/// </summary>
public sealed record OnTryPrimaryHitEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnTryPrimaryHit event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
/// <param name="priority">Execution priority (higher executes first)</param>
  /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnTryPrimaryHitEventInfo(
    Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler,
        int? priority = null,
      bool usesSpeed = true)
    {
        Id = EventId.TryPrimaryHit;
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
     typeof(ActiveMove),
  ];
        ExpectedReturnType = typeof(IntBoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnTryPrimaryHitEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryPrimaryHit;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnTryPrimaryHitEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnTryPrimaryHitEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetMove()
                );
                return result switch
                {
                    IntIntBoolVoidUnion i => new IntRelayVar(i.Value),
                    BoolIntBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidIntBoolVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
