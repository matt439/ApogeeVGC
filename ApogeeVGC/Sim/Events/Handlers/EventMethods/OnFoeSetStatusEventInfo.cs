using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeSetStatus event.
/// Triggered when attempting to set a status on a foe Pokemon.
/// Signature: (Battle battle, Condition status, Pokemon target, Pokemon source, IEffect effect) => BoolVoidUnion?
/// </summary>
public sealed record OnFoeSetStatusEventInfo : EventHandlerInfo
{
    /// <summary>
 /// Creates a new OnFoeSetStatus event handler.
    /// </summary>
  /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFoeSetStatusEventInfo(
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
  int? priority = null,
        bool usesSpeed = true)
  {
    Id = EventId.SetStatus;
  Prefix = EventPrefix.Foe;
        Handler = handler;
        Priority = priority;
  UsesSpeed = usesSpeed;
     ExpectedParameterTypes =
    [
      typeof(Battle),
 typeof(Condition),
   typeof(Pokemon),
   typeof(Pokemon),
      typeof(IEffect),
        ];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
