using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Events.Handlers.EventMethods;

/// <summary>
/// Event handler info for OnSourceEatItem event.
/// Signature: Action<Battle, Item, Pokemon>
/// </summary>
public sealed record OnSourceEatItemEventInfo : EventHandlerInfo
{
    public OnSourceEatItemEventInfo(
        Action<Battle, Item, Pokemon> handler,
    int? priority = null,
        bool usesSpeed = true)
    {
   Id = EventId.EatItem;
        Prefix = EventPrefix.Source;
 Handler = handler;
 Priority = priority;
UsesSpeed = usesSpeed;
        ExpectedParameterTypes = [typeof(Battle), typeof(Item), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
        
    // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false, false];
        ReturnTypeNullable = false;
    
    // Validate configuration
        ValidateConfiguration();
    }
}