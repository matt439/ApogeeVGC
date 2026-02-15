using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeAfterSwitchInSelf event.
/// Triggered after a foe Pokemon switches in.
/// Signature: (Battle battle, Pokemon pokemon) => void
/// </summary>
public sealed record OnFoeAfterSwitchInSelfEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeAfterSwitchInSelf event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeAfterSwitchInSelfEventInfo(
        Action<Battle, Pokemon> handler,
  int? priority = null,
   bool usesSpeed = true)
    {
        Id = EventId.AfterSwitchInSelf;
        Prefix = EventPrefix.Foe;
    Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
   ExpectedParameterTypes =
  [
      typeof(Battle),
  typeof(Pokemon),
   ];
    ExpectedReturnType = typeof(void);
        
  // Nullability: Battle (non-null), Pokemon (non-null)
  ParameterNullability = [false, false];
  ReturnTypeNullable = false; // void
      
 // Validate configuration
   ValidateConfiguration();
    }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnFoeAfterSwitchInSelfEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterSwitchInSelf;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeAfterSwitchInSelfEventInfo Create(
        Action<Battle, Pokemon> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeAfterSwitchInSelfEventInfo(
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
