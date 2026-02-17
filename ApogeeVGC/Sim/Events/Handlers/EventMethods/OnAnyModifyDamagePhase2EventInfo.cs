using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyModifyDamagePhase2 event.
/// Signature: Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>
/// </summary>
public sealed record OnAnyModifyDamagePhase2EventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnAnyModifyDamagePhase2EventInfo(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.ModifyDamagePhase2;
Prefix = EventPrefix.Any;
        #pragma warning disable CS0618
        Handler = handler;
        #pragma warning restore CS0618
Priority = priority;
        UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(int), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(DoubleVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAnyModifyDamagePhase2EventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyDamagePhase2;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyModifyDamagePhase2EventInfo Create(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyModifyDamagePhase2EventInfo(
                        context =>
            {
                var result = handler(
                    context.Battle,
                context.GetIntRelayVar(),
                context.GetSourceOrTargetPokemon(),
                context.GetTargetOrSourcePokemon(),
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
