using ApogeeVGC.Sim.Effects;

namespace ApogeeVGC.Sim.FieldClasses;

public enum TerrainId
{
    Electric,
    Grassy,
    Misty,
    Psychic,
}

public class Terrain : FieldElement
{
    public required TerrainId Id { get; init; }

    public Terrain Copy()
    {
        return new Terrain
        {
            Id = Id,
            Name = Name,
            Duration = Duration,
            DurationCallback = DurationCallback,
            ElapsedTurns = ElapsedTurns,
            PrintDebug = PrintDebug,

            //TODO: add delegate properties here
        };
    }

    public override ConditionEffectType ConditionEffectType => ConditionEffectType.Terrain;
}