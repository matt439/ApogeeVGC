using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyCriticalHit event (pokemon/ally-specific).
/// Determines if a move should critically hit.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => BoolVoidUnion | bool
/// </summary>
public sealed record OnAllyCriticalHitEventInfo : UnionEventHandlerInfo<OnCriticalHit>
{
    /// <summary>
    /// Creates a new OnAllyCriticalHit event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnAllyCriticalHitEventInfo(
        OnCriticalHit unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.CriticalHit;
        Prefix = EventPrefix.Ally;
        UnionValue = unionValue;
        #pragma warning disable CS0618
        Handler = ExtractDelegate();
        #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(BoolVoidUnion);

        // Nullability: Battle (non-null), target (non-null), source (non-null), move (non-null)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false; // BoolVoidUnion is a struct

        // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyCriticalHitEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.CriticalHit;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyCriticalHitEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyCriticalHitEventInfo(
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
