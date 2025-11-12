using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnLockMove event.
/// Returns which move a Pokemon is locked into.
/// Signature: (Battle, Pokemon) => MoveIdVoidUnion | MoveId constant
/// </summary>
public sealed record OnLockMoveEventInfo : UnionEventHandlerInfo<OnLockMove>
{
    /// <summary>
    /// Creates a new OnLockMove event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or MoveId constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnLockMoveEventInfo(
        OnLockMove unionValue,
        int? priority = null,
bool usesSpeed = true)
    {
        Id = EventId.LockMove;
   UnionValue = unionValue;
     Handler = ExtractDelegate();
  Priority = priority;
   UsesSpeed = usesSpeed;
 ExpectedParameterTypes =
   [
    typeof(Battle),
        typeof(Pokemon),
  ];
    ExpectedReturnType = typeof(MoveIdVoidUnion);
    }
}
