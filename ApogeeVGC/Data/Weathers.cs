using ApogeeVGC.Sim;
using System.Collections.ObjectModel;

namespace ApogeeVGC.Data;

public record Weathers
{
    public IReadOnlyDictionary<WeatherId, Weather> WeatherData { get; }

    public Weathers()
    {
        WeatherData = new ReadOnlyDictionary<WeatherId, Weather>(_weathers);
    }

    private readonly Dictionary<WeatherId, Weather> _weathers = new()
    {
        [WeatherId.HarshSunlight] = new Weather
        {
            WeatherId = WeatherId.HarshSunlight,
            BaseDuration = 5,
            DurationExtension = 3,
            // TODO: Implement effects
        },
        [WeatherId.Rain] = new Weather
        {
            WeatherId = WeatherId.Rain,
            BaseDuration = 5,
            DurationExtension = 3,
            // TODO: Implement effects
        },
        [WeatherId.Sandstorm] = new Weather
        {
            WeatherId = WeatherId.Sandstorm,
            BaseDuration = 5,
            DurationExtension = 3,
            // TODO: Implement effects
        },
        [WeatherId.Snow] = new Weather
        {
            WeatherId = WeatherId.Snow,
            BaseDuration = 5,
            DurationExtension = 3,
            // TODO: Implement effects
        },
    };
}