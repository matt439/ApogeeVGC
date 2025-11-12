using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.ItemSpecific;

/// <summary>
/// Event handler info for Item.OnUse event.
/// Union type: Action&lt;Battle, Pokemon&gt; | false
/// </summary>
public sealed record OnUseEventInfo : UnionEventHandlerInfo<OnItemEatUse>
{
    public OnUseEventInfo(OnItemEatUse? handler)
    {
        Id = EventId.Use;
        UnionValue = handler;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(void);
    }
}