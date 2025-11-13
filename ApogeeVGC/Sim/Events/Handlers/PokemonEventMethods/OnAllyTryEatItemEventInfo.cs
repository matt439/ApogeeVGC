using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;

/// <summary>
/// Event handler info for OnAllyTryEatItem event (pokemon/ally-specific).
/// Determines if an item can be consumed.
/// Signature: (Battle battle, Item item, Pokemon pokemon) => BoolVoidUnion | bool
/// </summary>
public sealed record OnAllyTryEatItemEventInfo : UnionEventHandlerInfo<OnTryEatItem>
{
    /// <summary>
    /// Creates a new OnAllyTryEatItem event handler.
    /// </summary>
    /// <param name="unionValue">The union value (delegate or bool constant)</param>
    /// <param name="priority">Execution priority (higher executes first)</param>
    /// <param name="usesSpeed">Whether this event uses speed-based ordering</param>
 public OnAllyTryEatItemEventInfo(
        OnTryEatItem unionValue,
        int? priority = null,
     bool usesSpeed = true)
    {
        Id = EventId.TryEatItem;
      Prefix = EventPrefix.Ally;
        UnionValue = unionValue;
Handler = ExtractDelegate();
        Priority = priority;
  UsesSpeed = usesSpeed;
     ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
        ExpectedReturnType = typeof(BoolVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = new[] { false, false, false };
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
  }
}