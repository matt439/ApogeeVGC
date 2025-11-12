using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnCriticalHit event.
/// Determines if a move should critically hit.
/// Signature: (Battle battle, Pokemon target, Pokemon source, ActiveMove move) => BoolVoidUnion | bool
/// </summary>
public sealed record OnCriticalHitEventInfo : UnionEventHandlerInfo<OnCriticalHit>
{
    /// <summary>
    /// Creates a new OnCriticalHit event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnCriticalHitEventInfo(
        OnCriticalHit unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
    Id = EventId.CriticalHit;
        UnionValue = unionValue;
        Handler = ExtractDelegate();
        Priority = priority;
     UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
  [
       typeof(Battle),
     typeof(Pokemon),
   typeof(Pokemon),
        typeof(ActiveMove),
        ];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: Battle (non-null), target (non-null), source (non-null), move (non-null)
     ParameterNullability = new[] { false, false, false, false };
      ReturnTypeNullable = false; // BoolVoidUnion is a struct

        // Validate configuration
   ValidateConfiguration();
    }
}
