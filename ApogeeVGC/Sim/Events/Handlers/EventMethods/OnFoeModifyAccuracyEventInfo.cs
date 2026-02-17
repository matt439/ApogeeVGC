using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeModifyAccuracy event.
/// Signature: Func<Battle, int?, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>
/// Note: accuracy is nullable because moves with true accuracy (always hit) pass null/true instead of a number
/// </summary>
public sealed record OnFoeModifyAccuracyEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnFoeModifyAccuracyEventInfo(
        Func<Battle, int?, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyAccuracy;
        Prefix = EventPrefix.Foe;
   #pragma warning disable CS0618
   Handler = handler;
   #pragma warning restore CS0618
        Priority = priority;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(int?), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(DoubleVoidUnion);
        
    // Nullability: accuracy parameter is nullable (position 1)
        ParameterNullability = [false, true, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeModifyAccuracyEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyAccuracy;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeModifyAccuracyEventInfo Create(
        Func<Battle, int?, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeModifyAccuracyEventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetNullableIntRelayVar(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetMove()
                );
                return result switch
                {
                    DoubleDoubleVoidUnion d => new DecimalRelayVar((decimal)d.Value),
                    VoidDoubleVoidUnion => null,
                    _ => null
                };
            },
            priority,
            usesSpeed
        );
    }
}
