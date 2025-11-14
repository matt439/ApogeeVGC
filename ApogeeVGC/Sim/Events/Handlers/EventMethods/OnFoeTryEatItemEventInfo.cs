using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnFoeTryEatItem event.
// / </summary>
public sealed record OnFoeTryEatItemEventInfo : UnionEventHandlerInfo<OnTryEatItem>
{
  public OnFoeTryEatItemEventInfo(
        OnTryEatItem unionValue,
        int? priority = null,
     bool usesSpeed = true)
    {
 Id = EventId.TryEatItem;
  Prefix = EventPrefix.Foe;
  UnionValue = unionValue;
        Handler = ExtractDelegate();
        Priority = priority;
        UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
        ExpectedReturnType = typeof(BoolVoidUnion);
    
        // Nullability: All parameters non-nullable by default (adjust as needed)
   ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
     // Validate configuration
        ValidateConfiguration();
    }
    
    /// <summary>
    /// Creates event handler using context-based pattern.
    /// Context provides: Battle, Item, TargetPokemon
    /// </summary>
    public OnFoeTryEatItemEventInfo(
      EventHandlerDelegate contextHandler,
        int? priority = null,
        bool usesSpeed = true)
    {
 Id = EventId.TryEatItem;
      Prefix = EventPrefix.Foe;
      ContextHandler = contextHandler;
        Priority = priority;
        UsesSpeed = usesSpeed;
    }
    
    /// <summary>
    /// Creates strongly-typed context-based handler.
    /// Best of both worlds: strongly-typed parameters + context performance.
    /// </summary>
    public static OnFoeTryEatItemEventInfo Create(
        Func<Battle, Item, Pokemon, BoolVoidUnion> handler,
  int? priority = null,
        bool usesSpeed = true)
    {
        return new OnFoeTryEatItemEventInfo(
   context =>
 {
     var result = handler(
          context.Battle,
  context.GetSourceEffect<Item>(),
   context.GetTargetPokemon()
             );
     return result switch
   {
    BoolBoolVoidUnion b => new BoolRelayVar(b.Value),
           VoidBoolVoidUnion => null,
 _ => null
       };
 },
   priority,
     usesSpeed
      );
    }
}