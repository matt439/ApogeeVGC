using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.Events.Handlers.ItemSpecific;

/// <summary>
/// Event handler info for Item.OnUse event.
/// Union type: Func&lt;Battle, Pokemon, BoolVoidUnion&gt; | Action&lt;Battle, Pokemon&gt; | false
/// Returns false to block item use, true/void to allow.
/// </summary>
public sealed record OnUseEventInfo : UnionEventHandlerInfo<OnItemUse>
{
    public OnUseEventInfo(OnItemUse? handler)
    {
        Id = EventId.Use;
        UnionValue = handler;
        ExpectedParameterTypes = [typeof(Battle), typeof(Pokemon)];
        ExpectedReturnType = typeof(BoolVoidUnion);

        // Nullability: All parameters non-nullable by default (adjust as needed)
        ParameterNullability = [false, false];
        ReturnTypeNullable = false;

        // Validate configuration
        ValidateConfiguration();
    }
}