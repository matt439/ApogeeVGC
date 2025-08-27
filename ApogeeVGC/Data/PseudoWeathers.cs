using ApogeeVGC.Sim;
using System.Collections.ObjectModel;

namespace ApogeeVGC.Data;

public class PseudoWeathers
{
    public IReadOnlyDictionary<PseudoWeatherId, PseudoWeather> PseudoWeatherData { get; }

    public PseudoWeathers()
    {
        PseudoWeatherData = new ReadOnlyDictionary<PseudoWeatherId, PseudoWeather>(_pseudoWeathers);
    }

    private readonly Dictionary<PseudoWeatherId, PseudoWeather> _pseudoWeathers = new()
    {
        [PseudoWeatherId.TrickRoom] = new PseudoWeather
        {
            PseudoWeatherId = PseudoWeatherId.TrickRoom,
            BaseDuration = 5,
            DurationExtension = 0,
            // TODO: Implement effects
        },
    };
}