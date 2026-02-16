using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeAfterFaint event.
/// Triggered after a foe Pokemon faints.
/// Signature: (Battle battle, int length, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnFoeAfterFaintEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeAfterFaint event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeAfterFaintEventInfo(
    Action<Battle, int, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
      Id = EventId.AfterFaint;
      Prefix = EventPrefix.Foe;
Handler = handler;
        Priority = priority;
UsesSpeed = usesSpeed;
   ExpectedParameterTypes =
        [
  typeof(Battle),
          typeof(int),
      typeof(Pokemon),
   typeof(Pokemon),
    typeof(IEffect),
 ];
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
    public OnFoeAfterFaintEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.AfterFaint;
        Prefix = EventPrefix.Foe;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnFoeAfterFaintEventInfo Create(
        Action<Battle, int, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeAfterFaintEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetIntRelayVar(),
                context.GetTargetOrSourcePokemon(),
                context.GetSourceOrTargetPokemon(),
                context.GetSourceEffect<IEffect>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
