using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeModifySecondaries event.
/// Signature: Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnFoeModifySecondariesEventInfo : EventHandlerInfo
{
    public OnFoeModifySecondariesEventInfo(
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifySecondaries;
        Prefix = EventPrefix.Foe;
   Handler = handler;
        Priority = priority;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(List<SecondaryEffect>), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeModifySecondariesEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifySecondaries;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeModifySecondariesEventInfo Create(
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeModifySecondariesEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                new List<SecondaryEffect>(context.GetRelayVar<SecondaryEffectArrayRelayVar>().Effects),
                context.GetTargetPokemon(),
                context.GetSourcePokemon(),
                context.GetMove()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}