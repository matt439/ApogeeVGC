using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnOverrideAction event.
/// Overrides the action a Pokemon will take.
/// Signature: (Battle battle, Pokemon pokemon, Pokemon target, ActiveMove move) => DelegateVoidUnion
/// </summary>
public sealed record OnOverrideActionEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnOverrideAction event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    [Obsolete("Use Create factory method instead.")]
    public OnOverrideActionEventInfo(
        Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.OverrideAction;
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
    ExpectedReturnType = typeof(DelegateVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnOverrideActionEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.OverrideAction;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnOverrideActionEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, DelegateVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnOverrideActionEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetSourceOrTargetPokemon(),
                context.GetTargetOrSourcePokemon(),
                context.GetMove()
                );
                return result switch
                {
                    DelegateDelegateVoidUnion d => throw new NotImplementedException("DelegateVoidUnion delegate return not supported in Create pattern"),
                    VoidDelegateVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
