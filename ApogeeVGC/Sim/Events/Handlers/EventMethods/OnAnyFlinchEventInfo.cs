using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnAnyFlinch event.
/// Determines if a Pokémon should flinch.
/// Signature: (Battle battle, Pokemon pokemon) => BoolVoidUnion | bool
/// </summary>
public sealed record OnAnyFlinchEventInfo : UnionEventHandlerInfo<OnFlinch>
{
    /// <summary>
    /// Creates a new OnAnyFlinch event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnAnyFlinchEventInfo(
        OnFlinch unionValue,
   int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.Flinch;
    Prefix = EventPrefix.Any;
   UnionValue = unionValue;
   Handler = ExtractDelegate();
   Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(BoolVoidUnion);
    }
}