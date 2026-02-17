using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyTryHitField event (pokemon/ally-specific).
/// Triggered when trying to hit field affecting ally.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?>
/// </summary>
public sealed record OnAllyTryHitFieldEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnAllyTryHitFieldEventInfo(
    Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryHitField;
  Prefix = EventPrefix.Ally;
  #pragma warning disable CS0618
  Handler = handler;
  #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(BoolIntEmptyVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
  }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyTryHitFieldEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryHitField;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }

    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyTryHitFieldEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, BoolIntEmptyVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyTryHitFieldEventInfo(
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
                    BoolBoolIntEmptyVoidUnion b => new BoolRelayVar(b.Value),
                    IntBoolIntEmptyVoidUnion i => new IntRelayVar(i.Value),
                    EmptyBoolIntEmptyVoidUnion => new BoolRelayVar(false),
                    VoidUnionBoolIntEmptyVoidUnion => null,
                    null => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
