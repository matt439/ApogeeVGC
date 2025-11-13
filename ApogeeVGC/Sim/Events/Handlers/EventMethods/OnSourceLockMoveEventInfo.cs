using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceLockMove event.
/// Forces a Pokémon to use a specific move.
/// Signature: (Battle battle, Pokemon pokemon) => MoveIdVoidUnion | MoveId?
/// </summary>
public sealed record OnSourceLockMoveEventInfo : UnionEventHandlerInfo<OnLockMove>
{
    /// <summary>
    /// Creates a new OnSourceLockMove event handler.
 /// </summary>
    /// <param name="unionValue">The union value (delegate or MoveId constant)</param>
  /// <param name="priority">Execution priority (higher executes first)</param>
 /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnSourceLockMoveEventInfo(
        OnLockMove unionValue,
        int? priority = null,
        bool usesSpeed = true)
    {
        Id = EventId.LockMove;
        Prefix = EventPrefix.Source;
        UnionValue = unionValue;
        Handler = ExtractDelegate();
        Priority = priority;
  UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(ActiveMove);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}