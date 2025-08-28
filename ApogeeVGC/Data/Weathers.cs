using ApogeeVGC.Sim;
using System.Collections.ObjectModel;

namespace ApogeeVGC.Data;

public record Weathers
{
    public IReadOnlyDictionary<WeatherId, Weather> WeatherData { get; }
    private readonly Library _library;

    public Weathers(Library library)
    {
        _library = library;
        WeatherData = new ReadOnlyDictionary<WeatherId, Weather>(CreateWeathers());
    }

    private Dictionary<WeatherId, Weather> CreateWeathers()
    {
        return new Dictionary<WeatherId, Weather>
        {
            [WeatherId.HarshSunlight] = new Weather
            {
                Id = WeatherId.HarshSunlight,
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [WeatherId.Rain] = new Weather
            {
                Id = WeatherId.Rain,
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [WeatherId.Sandstorm] = new Weather
            {
                Id = WeatherId.Sandstorm,
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [WeatherId.Snow] = new Weather
            {
                Id = WeatherId.Snow,
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
        };
    }
}