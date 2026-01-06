using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifyWeight event.
/// Modifies a Pokemon's weight.
/// Signature: (Battle battle, int weighthg, Pokemon pokemon) => IntVoidUnion
/// </summary>
public sealed record OnModifyWeightEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifyWeight event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifyWeightEventInfo(
        Func<Battle, int, Pokemon, IntVoidUnion> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
Id = EventId.ModifyWeight;
        Handler = handler;
        Priority = priority;
     UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
        [
   typeof(Battle),
            typeof(int),
  typeof(Pokemon),
     ];
 ExpectedReturnType = typeof(IntVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
