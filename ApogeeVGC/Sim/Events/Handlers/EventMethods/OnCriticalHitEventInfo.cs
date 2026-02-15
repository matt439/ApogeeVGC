using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnCriticalHit event.
/// Determines if a move should critically hit.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => BoolVoidUnion | bool
/// </summary>
public sealed record OnCriticalHitEventInfo : UnionEventHandlerInfo<OnCriticalHit>
{
    /// <summary>
    /// Creates a new OnCriticalHit event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnCriticalHitEventInfo(
        OnCriticalHit unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
    Id = EventId.CriticalHit;
        UnionValue = unionValue;
        Handler = ExtractDelegate();
        Priority = priority;
     UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
  [
       typeof(Battle),
     typeof(Pokemon),
   typeof(Pokemon),
        typeof(ActiveMove),
        ];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: Battle (non-null), target (non-null), source (nullable - can be null from RunEvent), move (non-null)
     ParameterNullability = [false, false, true, false];
      ReturnTypeNullable = false; // BoolVoidUnion is a struct

        // Validate configuration
   ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnCriticalHitEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.CriticalHit;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnCriticalHitEventInfo Create(
        Func<Battle, Pokemon, Pokemon?, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnCriticalHitEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetPokemon(),
                context.SourcePokemon,
                context.GetMove()
                );
                return result switch
                {
                    BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
                    VoidBoolVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
