using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceDamage event.
/// Modifies or prevents damage when this Pokemon is the source.
/// Signature: (Battle battle, int damage, Pokemon target, Pokemon source, IEffect effect) => IntBoolVoidUnion?
/// </summary>
public sealed record OnSourceDamageEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnSourceDamage event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
 /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSourceDamageEventInfo(
  Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?> handler,
 int? priority = null,
      bool usesSpeed = true)
    {
    Id = EventId.Damage;
 Prefix = EventPrefix.Source;
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
  ExpectedReturnType = typeof(IntBoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
