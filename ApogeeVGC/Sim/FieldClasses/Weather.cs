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
            IsExtended = IsExtended,
            BaseDuration = BaseDuration,
            DurationExtension = DurationExtension,
            DurationCallback = DurationCallback,
            ElapsedTurns = ElapsedTurns,
            PrintDebug = PrintDebug, // Added missing PrintDebug
            // Note: Action delegates (OnEnd, OnStart, etc.) are shared immutable references
            // since they don't contain mutable state - they're function pointers
            OnEnd = OnEnd,
            OnStart = OnStart,
            OnReapply = OnReapply,
            OnIncrementTurnCounter = OnIncrementTurnCounter,
            OnPokemonSwitchIn = OnPokemonSwitchIn,
        };
    }
}