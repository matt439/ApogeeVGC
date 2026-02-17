using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyBeforeFaint event (pokemon/ally-specific).
/// Triggered before ally faints.
/// Signature: Action<Battle, Pokemon, IEffect>
/// </summary>
public sealed record OnAllyBeforeFaintEventInfo : EventHandlerInfo
{
    [Obsolete("Use Create factory method instead.")]
    public OnAllyBeforeFaintEventInfo(
    Action<Battle, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeFaint;
  Prefix = EventPrefix.Ally;
  #pragma warning disable CS0618
  Handler = handler;
  #pragma warning restore CS0618
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon), typeof(IEffect)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
  }

    /// <summary>
    /// Creates event handler using context-based pattern.
    /// </summary>
    public OnAllyBeforeFaintEventInfo(
        EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.BeforeFaint;
        Prefix = EventPrefix.Ally;
        ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// </summary>
    public static OnAllyBeforeFaintEventInfo Create(
        Action<Battle, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
        return new OnAllyBeforeFaintEventInfo(
                        context =>
            {
                handler(
                    context.Battle,
                context.GetTargetOrSourcePokemon(),
                context.GetSourceEffect<IEffect>()
                );
                return null;
            },
            priority,
            usesSpeed
        );
    }
}
