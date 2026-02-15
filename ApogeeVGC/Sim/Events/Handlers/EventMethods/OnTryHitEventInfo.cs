using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryHit event.
/// Triggered when attempting to hit a target with a move.
/// Signature: (Battle battle, Pokemon source, Pokemon target, ActiveMove move) => BoolIntEmptyVoidUnion?
/// </summary>
public sealed record OnTryHitEventInfo : EventHandlerInfo
{
    /// <summary>
  /// Creates a new OnTryHit event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTryHitEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryHit;
        Handler = handler;
  Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes =
[
   typeof(Battle),
     typeof(Pokemon),
   typeof(Pokemon),
    typeof(ActiveMove),
        ];
        ExpectedReturnType = typeof(BoolIntEmptyVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnTryHitEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryHit;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnTryHitEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnTryHitEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetSourcePokemon(),
                context.GetTargetPokemon(),
                context.GetMove()
                );
                return result switch
                {
                    BoolBoolIntEmptyVoidUnion b => new BoolRelayVar(b.Value),
                    IntBoolIntEmptyVoidUnion i => new IntRelayVar(i.Value),
                    EmptyBoolIntEmptyVoidUnion => new BoolRelayVar(false),
                    VoidUnionBoolIntEmptyVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
