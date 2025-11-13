using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnTryAddVolatile event.
/// Triggered when attempting to add a volatile condition.
/// Signature: (Battle battle, Condition status, Pokemon target, Pokemon source, IEffect sourceEffect) => BoolVoidUnion?
/// </summary>
public sealed record OnTryAddVolatileEventInfo : EventHandlerInfo
{
    /// <summary>
    /// Creates a new OnTryAddVolatile event handler.
    /// </summary>
    /// <param name="handler">The event handler delegate</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
    public OnTryAddVolatileEventInfo(
        Func<Battle, Condition, Pokemon, Pokemon, IEffect, BoolVoidUnion?> handler,
 int? priority = null,
  bool usesSpeed = true)
    {
Id = EventId.TryAddVolatile;
        Handler = handler;
   Priority = priority;
    UsesSpeed = usesSpeed;
        ExpectedParameterTypes =
[
    typeof(Battle),
     typeof(Condition),
typeof(Pokemon),
    typeof(Pokemon),
      typeof(IEffect),
        ];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}
