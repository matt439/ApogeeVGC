using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeDamage event.
/// Modifies or prevents damage to a foe Pokemon.
/// Signature: (Battle battle, int damage, Pokemon target, Pokemon source, IEffect effect) => IntBoolVoidUnion?
/// </summary>
public sealed record OnFoeDamageEventInfo : EventHandlerInfo
{
/// <summary>
    /// Creates a new OnFoeDamage event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeDamageEventInfo(
 Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?> handler,
int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.Damage;
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
     ExpectedReturnType = typeof(IntBoolVoidUnion);
  }
}
