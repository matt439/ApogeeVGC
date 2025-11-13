using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnModifySpe event.
/// Modifies the Speed stat.
/// Signature: (Battle battle, int spe, Pokemon pokemon) => IntVoidUnion
/// </summary>
public sealed record OnModifySpeEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnModifySpe event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnModifySpeEventInfo(
        Func<Battle, int, Pokemon, IntVoidUnion> handler,
        int? priority = null,
 bool usesSpeed = true)
    {
        Id = EventId.ModifySpe;
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
