using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.FieldClasses;

public class Field
{
    public Condition? Weather { get; private set; }
    public EffectState? WeatherState { get; private set; }
    public Condition? Terrain { get; private set; }
    public EffectState? TerrainState { get; private set; }
    public bool HasAnyWeather => Weather != null;
    public bool HasAnyTerrain => Terrain != null;
    public Dictionary<ConditionId, EffectState> PseudoWeather { get; init; } = new();
    public bool HasAnyPseudoWeather => PseudoWeather.Count > 0;

    public bool SetTerrain(IBattle battle, IEffect status, Pokemon? source = null, IEffect? sourceEffect = null)
    {
        throw new NotImplementedException();
    }

    public bool IsTerrain(IBattle battle, ConditionId terrain, PokemonSideBattleUnion? target)
    {
        throw new NotImplementedException();
    }

    public bool RemovePseudoWeather(Condition status)
    {
        throw new NotImplementedException();
    }

    public bool SuppressingWeather()
    {
        throw new NotImplementedException();
    }

    public Field Copy()
    {
        return new Field
        {
            Weather = Weather?.Copy(),
            Terrain = Terrain?.Copy(),

            // TODO: Add all other fields
        };
    }
}

