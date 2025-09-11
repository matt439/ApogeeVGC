using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.FieldClasses;

public enum SideConditionId
{
    Tailwind,
    Reflect,
    LightScreen,
}

public class SideCondition : FieldElement
{
    public required SideConditionId Id { get; init; }
    public required int OnSideResidualOrder { get; init; }
    public required int OnSideResidualSubOrder { get; init; }
    /// <summary>
    /// side, context
    /// </summary>
    public Action<Side, BattleContext>? OnSideStart { get; init; }
    /// <summary>
    /// side, context
    /// </summary>
    public Action<Side, BattleContext>? OnSideEnd { get; init; }
    /// <summary>
    /// pokemon, context
    /// </summary>
    public Action<Pokemon, BattleContext>? OnSidePokemonSwitchIn { get; init; }

    public SideCondition Copy()
    {
        return new SideCondition
        {
            Id = Id,
            OnSideResidualOrder = OnSideResidualOrder,
            OnSideResidualSubOrder = OnSideResidualSubOrder,
            OnSideStart = OnSideStart,
            OnSideEnd = OnSideEnd,
            Name = Name,
            IsExtended = IsExtended,
            BaseDuration = BaseDuration,
            DurationExtension = DurationExtension,
            DurationCallback = DurationCallback,
            ElapsedTurns = ElapsedTurns,
            PrintDebug = PrintDebug, // Added missing PrintDebug
            // Note: TurnStart delegates are shared immutable references
            OnEnd = OnEnd,
            OnStart = OnStart,
            OnReapply = OnReapply,
            OnIncrementTurnCounter = OnIncrementTurnCounter,
            OnPokemonSwitchIn = OnPokemonSwitchIn,
        };
    }
}