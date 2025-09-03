namespace ApogeeVGC.Sim
{
    public enum PseudoWeatherId
    {
        TrickRoom,
        //Gravity,
        //MudSport,
        //WaterSport,
        // TODO: Add more pseudo-weathers as needed
    }

    public class PseudoWeather : FieldElement
    {
        public required PseudoWeatherId Id { get; init; }
        public PseudoWeather Copy()
        {
            return new PseudoWeather
            {
                Id = Id,
                Name = Name,
                IsExtended = IsExtended,
                BaseDuration = BaseDuration,
                DurationExtension = DurationExtension,
                DurationCallback = DurationCallback,
                ElapsedTurns = ElapsedTurns,
                PrintDebug = PrintDebug, // Added missing PrintDebug
                // Note: Action delegates are shared immutable references
                OnEnd = OnEnd,
                OnStart = OnStart,
                OnReapply = OnReapply,
                OnIncrementTurnCounter = OnIncrementTurnCounter,
                OnPokemonSwitchIn = OnPokemonSwitchIn,
            };
        }
    }
}
