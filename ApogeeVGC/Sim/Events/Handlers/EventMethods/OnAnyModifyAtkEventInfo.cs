using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyModifyAtk event.
/// Signature: Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion>
/// </summary>
public sealed record OnAnyModifyAtkEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnAnyModifyAtkEventInfo(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.ModifyAtk;
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
    public OnAnyModifyAtkEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyAtk;
        Prefix = EventPrefix.Any;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAnyModifyAtkEventInfo Create(
        Func<Battle, int, Pokemon, Pokemon, ActiveMove, DoubleVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAnyModifyAtkEventInfo(
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
