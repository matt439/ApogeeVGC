using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnNegateImmunity event.
/// Negates type immunity for a move.
/// Signature: (Battle battle, Pokemon pokemon, PokemonType? type) => BoolVoidUnion | bool
/// </summary>
public sealed record OnNegateImmunityEventInfo : UnionEventHandlerInfo<OnNegateImmunity>
{
  /// <summary>
    /// Creates a new OnNegateImmunity event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnNegateImmunityEventInfo(
  OnNegateImmunity unionValue,
     int? priority = null,
        bool usesSpeed = true)
    {
 Id = EventId.NegateImmunity;
   UnionValue = unionValue;
    Handler = ExtractDelegate();
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
