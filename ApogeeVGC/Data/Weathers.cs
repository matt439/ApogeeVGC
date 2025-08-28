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
            [WeatherId.HarshSunlight] = new()
            {
                Id = WeatherId.HarshSunlight,
                Name = "Harsh Sunlight",
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [WeatherId.Rain] = new()
            {
                Id = WeatherId.Rain,
                Name = "Rain",
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [WeatherId.Sandstorm] = new()
            {
                Id = WeatherId.Sandstorm,
                Name = "Sandstorm",
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
            [WeatherId.Snow] = new()
            {
                Id = WeatherId.Snow,
                Name = "Snow",
                BaseDuration = 5,
                DurationExtension = 3,
                // TODO: Implement effects
            },
        };
    }
}