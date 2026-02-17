using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceType event.
/// Signature: Func<Battle, PokemonType[], Pokemon, TypesVoidUnion>
/// </summary>
public sealed record OnSourceTypeEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnSourceTypeEventInfo(
        Func<Battle, PokemonType[], Pokemon, TypesVoidUnion> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.Type;
        Prefix = EventPrefix.Source;
 #pragma warning disable CS0618
 Handler = handler;
 #pragma warning restore CS0618
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(PokemonType[]), typeof(Pokemon)];
        ExpectedReturnType = typeof(TypesVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
    ParameterNullability = [false, false, false];
      ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSourceTypeEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Type;
        Prefix = EventPrefix.Source;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSourceTypeEventInfo Create(
        Func<Battle, PokemonType[], Pokemon, TypesVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSourceTypeEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetRelayVar<TypesRelayVar>().Types.ToArray(),
                context.GetSourceOrTargetPokemon()
                );
                return result switch
                {
                    TypesTypesVoidUnion t => new TypesRelayVar(t.Types.ToList()),
                    VoidTypesVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
