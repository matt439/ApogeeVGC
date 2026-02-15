using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Conditions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSideConditionStart event.
/// Triggered when a side condition starts.
/// Signature: (Battle battle, Side target, Pokemon source, Condition sideCondition) => void
/// </summary>
public sealed record OnSideConditionStartEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSideConditionStart event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSideConditionStartEventInfo(
     Action<Battle, Side, Pokemon, Condition> handler,
        int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.SideConditionStart;
        Handler = handler;
     Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
[
            typeof(Battle),
     typeof(Side),
 typeof(Pokemon),
  typeof(Condition),
        ];
   ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnSideConditionStartEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.SideConditionStart;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnSideConditionStartEventInfo Create(
        Action<Battle, Side, Pokemon, Condition> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnSideConditionStartEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetSide(),
                context.GetTargetPokemon(),
                context.GetSourceEffect<Condition>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
