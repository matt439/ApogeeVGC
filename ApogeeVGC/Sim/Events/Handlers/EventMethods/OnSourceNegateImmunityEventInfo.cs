using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceNegateImmunity event.
/// Determines if immunity should be negated for a type.
/// Signature: (Battle battle, Pokemon pokemon, PokemonType? type) => BoolVoidUnion | bool
/// </summary>
public sealed record OnSourceNegateImmunityEventInfo : UnionEventHandlerInfo<OnNegateImmunity>
{
    /// <summary>
    /// Creates a new OnSourceNegateImmunity event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnSourceNegateImmunityEventInfo(
        OnNegateImmunity unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.NegateImmunity;
        Prefix = EventPrefix.Source;
        UnionValue = unionValue;
        #pragma warning disable CS0618
        Handler = ExtractDelegate();
        #pragma warning restore CS0618
      Priority = priority;
   UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(PokemonType)];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceNegateImmunityEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.NegateImmunity;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceNegateImmunityEventInfo Create(
        Func<Battle, Pokemon, PokemonType, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceNegateImmunityEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.SourceType!.Value
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
