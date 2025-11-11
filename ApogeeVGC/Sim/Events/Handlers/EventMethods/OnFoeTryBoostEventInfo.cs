using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeTryBoost event.
/// Triggered when a stat boost is attempted on a foe.
/// Signature: (Battle battle, SparseBoostsTable boost, Pokemon target, Pokemon source, IEffect effect) => void
/// </summary>
public sealed record OnFoeTryBoostEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFoeTryBoost event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeTryBoostEventInfo(
  Action<Battle, SparseBoostsTable, Pokemon, Pokemon, IEffect> handler,
        int? priority = null,
        bool usesSpeed = true)
  {
        Id = EventId.TryBoost;
    Prefix = EventPrefix.Foe;
        Handler = handler;
        Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
      typeof(Battle),
  typeof(SparseBoostsTable),
   typeof(Pokemon),
  typeof(Pokemon),
    typeof(IEffect),
        ];
     ExpectedReturnType = typeof(void);
    }
}
