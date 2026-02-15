using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyBeforeSwitchOut event (pokemon/ally-specific).
/// Triggered before ally switches out.
/// Signature: Action<Battle, Pokemon>
/// </summary>
public sealed record OnAllyBeforeSwitchOutEventInfo : EventHandlerInfo
{
    public OnAllyBeforeSwitchOutEventInfo(
      Action<Battle, Pokemon> handler,
 int? priority = null,
 bool usesSpeed = true)
    {
   Id = EventId.BeforeSwitchOut;
     Prefix = EventPrefix.Ally;
  Handler = handler;
 Priority = priority;
  UsesSpeed = usesSpeed;
 ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
 ExpectedReturnType = typeof(void);
   
      // Nullability: Battle (non-null), Pokemon (non-null)
   ParameterNullability = new[] { false, false };
  ReturnTypeNullable = false; // void
        
 // Validate configuration
   ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyBeforeSwitchOutEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeSwitchOut;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyBeforeSwitchOutEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyBeforeSwitchOutEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetPokemon()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}