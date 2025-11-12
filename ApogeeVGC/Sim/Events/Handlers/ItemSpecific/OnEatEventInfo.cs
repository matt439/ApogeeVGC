using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.ItemSpecific;

/// <summary>
/// Event handler info for Item.OnEat event.
/// Union type: Action&lt;Battle, Pokemon&gt; | false
/// </summary>
public sealed record OnEatEventInfo : UnionEventHandlerInfo<OnItemEatUse>
{
    public OnEatEventInfo(OnItemEatUse? handler)
    {
        Id = EventId.Eat;
        UnionValue = handler;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
    }
}
