using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeTakeItem event.
/// Signature: Func<Battle, Item, Pokemon, Pokemon, Move?, PokemonVoidUnion> | bool
/// </summary>
public sealed record OnFoeTakeItemEventInfo : UnionEventHandlerInfo<OnTakeItem>
{
    public OnFoeTakeItemEventInfo(
  OnTakeItem unionValue,
     int? priority = null,
        bool usesSpeed = true)
    {
Id = EventId.TakeItem;
  Prefix = EventPrefix.Foe;
  UnionValue = unionValue;
Handler = ExtractDelegate();
Priority = priority;
      UsesSpeed = usesSpeed;
      ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon), typeof(Pokemon), typeof(Move)];
   ExpectedReturnType = typeof(PokemonVoidUnion);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}