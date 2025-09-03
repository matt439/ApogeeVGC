using ApogeeVGC.Sim;
using ApogeeVGC.Sim.Ui;
using System.Collections.ObjectModel;

namespace ApogeeVGC.Data;

public class PseudoWeathers
{
    public IReadOnlyDictionary<PseudoWeatherId, PseudoWeather> PseudoWeatherData { get; }
    private readonly Library _library;

    public PseudoWeathers(Library library)
    {
        _library = library;
        PseudoWeatherData = new ReadOnlyDictionary<PseudoWeatherId, PseudoWeather>(CreatePseudoWeathers());
    }

    private Dictionary<PseudoWeatherId, PseudoWeather> CreatePseudoWeathers()
    {
        return new Dictionary<PseudoWeatherId, PseudoWeather>
        {
            [PseudoWeatherId.TrickRoom] = new()
            {
                Id = PseudoWeatherId.TrickRoom,
                Name = "Trick Room",
                BaseDuration = 5,
                DurationExtension = 0,
                OnStart = (pokemon, context) =>
                {
                    // For each pokemon on the field, add the trick room condition
                    foreach (Pokemon p in pokemon)
                    {
                        p.AddCondition(context.Library.Conditions[ConditionId.TrickRoom], context);
                    }

                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintTrickRoomStart();
                    }
                },
                OnEnd = (pokemon, context) =>
                {
                    // For each pokemon on the field, remove the trick room condition
                    foreach (Pokemon p in pokemon)
                    {
                        if (!p.RemoveCondition(ConditionId.TrickRoom))
                        {
                            throw new InvalidOperationException($"Failed to remove Trick Room condition from {p.Specie.Name}");
                        }
                    }
                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintTrickRoomEnd();
                    }
                },
                OnReapply = (field, pokemon, context) =>
                {
                    // In case of trick room, applying trick room while it is already active
                    // removes it from the field and from all pokemons
                    field.RemovePseudoWeather( PseudoWeatherId.TrickRoom, pokemon, context);

                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintTrickRoomRestart();
                    }
                },
                //OnIncrementTurnCounter = (_, element, context) =>
                //{
                //    if (context.PrintDebug)
                //    {
                //        UiGenerator.PrintFieldElementCounter(element);
                //    }
                //},
                OnPokemonSwitchIn = (pokemon, context) =>
                {
                    pokemon.AddCondition(context.Library.Conditions[ConditionId.TrickRoom], context);
                },
            },
        };
    }
}