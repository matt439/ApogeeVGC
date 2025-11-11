using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFlinch event.
/// Triggered when a Pokemon flinches.
/// Signature: (Battle battle, Pokemon pokemon) => BoolVoidUnion
/// </summary>
public sealed record OnFlinchEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnFlinch event handler.
    /// </summary>
/// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnFlinchEventInfo(
        Func<Battle, Pokemon, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
  Id = EventId.Flinch;
  Handler = handler;
  Priority = priority;
    UsesSpeed = usesSpeed;
  ExpectedParameterTypes =
 [
  typeof(Battle),
     typeof(Pokemon),
        ];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
