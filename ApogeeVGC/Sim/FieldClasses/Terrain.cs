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