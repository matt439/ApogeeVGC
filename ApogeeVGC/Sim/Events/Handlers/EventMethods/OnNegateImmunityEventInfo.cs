using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnNegateImmunity event.
/// Negates type immunity for a move.
/// Signature: (Battle battle, Pokemon pokemon, PokemonType? type) => BoolVoidUnion
/// </summary>
public sealed record OnNegateImmunityEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnNegateImmunity event handler.
 /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnNegateImmunityEventInfo(
    Func<Battle, Pokemon, PokemonType?, BoolVoidUnion> handler,
        int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.NegateImmunity;
  Handler = handler;
    Priority = priority;
        UsesSpeed = usesSpeed;
 ExpectedParameterTypes =
  [
   typeof(Battle),
  typeof(Pokemon),
    typeof(PokemonType),
        ];
     ExpectedReturnType = typeof(BoolVoidUnion);
    }
}
