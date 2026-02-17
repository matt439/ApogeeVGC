using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyModifySecondaries event (pokemon/ally-specific).
/// Triggered to modify ally's move secondary effects.
/// Signature: Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove>
/// </summary>
public sealed record OnAllyModifySecondariesEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnAllyModifySecondariesEventInfo(
    Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifySecondaries;
  Prefix = EventPrefix.Ally;
  #pragma warning disable CS0618
  Handler = handler;
  #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(List<SecondaryEffect>), typeof(Pokemon), typeof(Pokemon), typeof(ActiveMove)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
  }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyModifySecondariesEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifySecondaries;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyModifySecondariesEventInfo Create(
        Action<Battle, List<SecondaryEffect>, Pokemon, Pokemon, ActiveMove> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyModifySecondariesEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                new List<SecondaryEffect>(context.GetRelayVar<SecondaryEffectArrayRelayVar>().Effects),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetMove()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
