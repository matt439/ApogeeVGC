using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyModifyType event (pokemon/ally-specific).
/// Triggered to modify ally's move type.
/// Signature: Action<Battle, ActiveMove, Pokemon, Pokemon>
/// </summary>
public sealed record OnAllyModifyTypeEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnAllyModifyTypeEventInfo(
    Action<Battle, ActiveMove, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyType;
  Prefix = EventPrefix.Ally;
  #pragma warning disable CS0618
  Handler = handler;
  #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(ActiveMove), typeof(Pokemon), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
  }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyModifyTypeEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.ModifyType;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyModifyTypeEventInfo Create(
        Action<Battle, ActiveMove, Pokemon, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyModifyTypeEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetMove(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
