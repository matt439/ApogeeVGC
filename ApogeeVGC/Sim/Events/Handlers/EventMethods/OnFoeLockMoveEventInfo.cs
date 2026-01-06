using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeLockMove event.
/// Signature: Func<Battle, Pokemon, MoveIdVoidUnion> | MoveId
/// </summary>
public sealed record OnFoeLockMoveEventInfo : UnionEventHandlerInfo<OnLockMove>
{
    public OnFoeLockMoveEventInfo(
OnLockMove unionValue,
  int? priority = null,
     bool usesSpeed = true)
  {
   Id = EventId.LockMove;
     Prefix = EventPrefix.Foe;
   UnionValue = unionValue;
  Handler = ExtractDelegate();
    Priority = priority;
  UsesSpeed = usesSpeed;
   ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
ExpectedReturnType = typeof(MoveIdVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}