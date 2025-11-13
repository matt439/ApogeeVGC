using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnBasePower event.
/// Modifies the base power of a move.
/// Signature: (Battle battle, int relayVar, Pokemon source, Pokemon target, ActiveMove move) => DoubleVoidUnion
/// </summary>
public sealed record OnBasePowerEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnBasePower event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate (ModifierSourceMoveHandler)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnBasePowerEventInfo(
    ModifierSourceMoveHandler handler,
  int? priority = null,
  bool usesSpeed = true)
    {
     Id = EventId.BasePower;
  Handler = handler;
        Priority = priority;
     UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
            typeof(Battle), 
  typeof(int), // relayVar
    typeof(Pokemon), // source
       typeof(Pokemon), // target
     typeof(ActiveMove),
        ];
 ExpectedReturnType = typeof(DoubleVoidUnion);
      
        // Nullability: All parameters are non-nullable
  ParameterNullability = [false, false, false, false, false];
     ReturnTypeNullable = false; // DoubleVoidUnion is a struct
        
   // Validate configuration
  ValidateConfiguration();
    }
}
