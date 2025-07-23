using System.Collections.Generic;

namespace ApogeeVGC_CS.sim
{
    public class Field
    {
        public Battle Battle { get; }
        public string Id { get; set; } = string.Empty;

        public string Weather { get; set; } = string.Empty;
        public EffectState WeatherState { get; set; }
        public string Terrain { get; set; } = string.Empty;
        public EffectState TerrainState { get; set; }
        public Dictionary<string, EffectState> PseudoWeather { get; set; } = new();

        public Field(Battle battle)
        {
            Battle = battle;
            // Optionally copy field scripts from format/dex if needed
            Id = string.Empty;
            Weather = string.Empty;
            WeatherState = Battle.InitEffectState(new EffectState { Id = string.Empty });
            Terrain = string.Empty;
            TerrainState = Battle.InitEffectState(new EffectState { Id = string.Empty });
            PseudoWeather = new Dictionary<string, EffectState>();
        }
    }
}