using ApogeeVGC.Sim.Effects;

namespace ApogeeVGC.Sim.FieldClasses;

public enum WeatherId
{
    HarshSunlight,
    Rain,
    Sandstorm,
    Snow,
}

public class Weather : FieldElement
{
    public required WeatherId Id { get; init; }

    public Weather Copy()
    {
        return new Weather
        {
            Id = Id,
            Name = Name,
            Duration = Duration,
            DurationCallback = DurationCallback,
            ElapsedTurns = ElapsedTurns,
            PrintDebug = PrintDebug,

            // TODO: add delegate properties here
        };
    }

    public override ConditionEffectType ConditionEffectType => ConditionEffectType.Weather;
}