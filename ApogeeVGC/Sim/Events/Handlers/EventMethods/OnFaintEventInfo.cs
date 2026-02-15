using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFaint event.
/// Triggered when a Pokemon faints.
/// Signature: (Battle battle, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnFaintEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFaint event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
  /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFaintEventInfo(
        Action<Battle, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
    Id = EventId.Faint;
        Handler = handler;
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
  [
      typeof(Battle),
 typeof(Pokemon),
  typeof(Pokemon),
  typeof(IEffect),
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
    public OnFaintEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Faint;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFaintEventInfo Create(
        Action<Battle, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFaintEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetPokemon(),
                context.GetSourcePokemon(),
                context.GetSourceEffect<IEffect>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
