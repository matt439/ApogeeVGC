using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnType event.
/// Triggered to modify a Pokemon's types.
/// Signature: (Battle battle, PokemonType[] types, Pokemon pokemon) => TypesVoidUnion
/// </summary>
public sealed record OnTypeEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnType event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTypeEventInfo(
        Func<Battle, PokemonType[], Pokemon, TypesVoidUnion> handler,
   int? priority = null,
   bool usesSpeed = true)
    {
      Id = EventId.Type;
     Handler = handler;
   Priority = priority;
  UsesSpeed = usesSpeed;
     ExpectedParameterTypes =
        [
      typeof(Battle),
     typeof(PokemonType[]),
typeof(Pokemon),
 ];
     ExpectedReturnType = typeof(TypesVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
