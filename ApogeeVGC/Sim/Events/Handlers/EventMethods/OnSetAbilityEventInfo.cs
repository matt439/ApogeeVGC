using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSetAbility event.
/// Triggered when an ability is set on a Pokemon.
/// Signature: (Battle battle, Ability ability, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnSetAbilityEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSetAbility event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
  /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSetAbilityEventInfo(
  Action<Battle, Ability, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
   bool usesSpeed = true)
    {
        Id = EventId.SetAbility;
        Handler = handler;
   Priority = priority;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle),
        typeof(Ability),
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
}
