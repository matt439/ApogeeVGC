using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
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
            Duration = Duration,
            DurationCallback = DurationCallback,
            ElapsedTurns = ElapsedTurns,
            PrintDebug = PrintDebug,

            //TODO: add delegates
        };
    }

    public override ConditionEffectType ConditionEffectType => ConditionEffectType.SideCondition;
}