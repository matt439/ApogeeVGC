using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryHeal event.
/// Triggered when attempting to heal a Pokemon.
/// Signature: (Battle battle, int relayVar, Pokemon target, Pokemon source, IEffect effect) => IntBoolVoidUnion?
/// </summary>
public sealed record OnTryHealEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnTryHeal event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTryHealEventInfo(
        Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolVoidUnion?> handler,
int? priority = null,
  bool usesSpeed = true)
    {
        Id = EventId.TryHeal;
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
