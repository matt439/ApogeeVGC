using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyTryPrimaryHit event (pokemon/ally-specific).
/// Triggered when trying primary hit on ally.
/// Signature: Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?>
/// </summary>
public sealed record OnAllyTryPrimaryHitEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnAllyTryPrimaryHitEventInfo(
    Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryPrimaryHit;
  Prefix = EventPrefix.Ally;
  #pragma warning disable CS0618
  Handler = handler;
  #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(IntBoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
  }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyTryPrimaryHitEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.TryPrimaryHit;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyTryPrimaryHitEventInfo Create(
        Func<Battle, Pokemon, Pokemon, ActiveMove, IntBoolVoidUnion?> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyTryPrimaryHitEventInfo(
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
