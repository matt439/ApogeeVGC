using ApogeeVGC_CS.data;
using System.Drawing;

namespace ApogeeVGC_CS.sim
{
    public class Field
    {
        public Battle Battle { get; }
        public Id Id { get; }
        public Id Weather { get; init; }
        public EffectState WeatherState { get; init; }
        public Id Terrain { get; init; }
        public EffectState TerrainState { get; init; }
        public Dictionary<string, EffectState> PseudoWeather { get; init; } = [];

        public Field(Battle battle)
        {
            Battle = battle;
            // Optionally copy field scripts from format/dex if needed
            Id = Id.Empty;
            Weather = Id.Empty;
            WeatherState = Battle.InitEffectState(new EffectState { Id = Id.Empty, EffectOrder = 0 });
            Terrain = Id.Empty;
            TerrainState = Battle.InitEffectState(new EffectState { Id = Id.Empty, EffectOrder = 0 });
        }

        public object ToJson()
        {
            throw new NotImplementedException("ToJson method is not implemented yet.");
        }

        public bool? SetWeather(string status, Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("SetWeather method is not implemented yet.");
        }

        public bool? SetWeather(Condition status, Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("SetWeather method is not implemented yet.");
        }

        public bool? SetWeather(string status, bool debug, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("SetWeather method is not implemented yet.");
        }

        public bool? SetWeather(Condition status, bool debug, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("SetWeather method is not implemented yet.");
        }

        public bool ClearWeather()
        {
            throw new NotImplementedException("ClearWeather method is not implemented yet.");
        }

        public Id EffectiveWeather()
        {
            throw new NotImplementedException("EffectiveWeather method is not implemented yet.");
        }

        public bool SuppressingWeather()
        {
            // Iterate through each side of the battle.
            foreach (Side side in Battle.Sides)
            {
                // Iterate through each active Pokémon on the side.
                foreach (Pokemon pokemon in side.Active)
                {
                    // Ensure the Pokémon exists and is not fainted.
                    if (pokemon.Fainted)
                    {
                        continue;
                    }

                    // Check if the Pokémon's ability is currently active (not being ignored).
                    if (pokemon.IgnoringAbility())
                    {
                        continue;
                    }

                    Ability ability = pokemon.GetAbility();

                    // Check if the ability has the suppressWeather flag.
                    // Also check that the ability's effect is not in the process of ending.
                    bool isEnding = pokemon.AbilityState.ExtraData.TryGetValue("ending", out object? ending) &&
                                    (bool)ending;

                    if (ability.SuppressWeather && !isEnding)
                    {
                        // If we find any Pokémon suppressing weather, we can return true immediately.
                        return true;
                    }
                }
            }

            // If no Pokémon is suppressing weather, return false.
            return false;
        }

        public bool IsWeather(string weather)
        { 
            throw new NotImplementedException("IsWeather method is not implemented yet.");
        }

        public bool IsWeather(List<string> weather)
        {
            throw new NotImplementedException("IsWeather method is not implemented yet.");
        }

        public Condition GetWeather()
        {
            throw new NotImplementedException("GetWeather method is not implemented yet.");
        }

        public bool SetTerrain(string status, Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("SetTerrain method is not implemented yet.");
        }

        public bool SetTerrain(IEffect status, Pokemon? source = null, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("SetTerrain method is not implemented yet.");
        }

        public bool SetTerrain(string status, bool debug, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("SetTerrain method is not implemented yet.");
        }

        public bool SetTerrain(IEffect status, bool debug, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("SetTerrain method is not implemented yet.");
        }

        public bool ClearTerrain()
        {
            throw new NotImplementedException("ClearTerrain method is not implemented yet.");
        }

        public bool EffectiveTerrain(Pokemon target)
        {
            throw new NotImplementedException("EffectiveTerrain method is not implemented yet.");
        }

        public bool EffectiveTerrain(Side target)
        {
            throw new NotImplementedException("EffectiveTerrain method is not implemented yet.");
        }

        public bool EffectiveTerrain(Battle target)
        {
            throw new NotImplementedException("EffectiveTerrain method is not implemented yet.");
        }

        public bool EffectiveTerrain()
        {
            throw new NotImplementedException("EffectiveTerrain method is not implemented yet.");
        }

        public bool IsTerrain(string terrain)
        {
            throw new NotImplementedException("IsTerrain method is not implemented yet.");
        }

        public bool IsTerrain(List<string> terrain)
        {
            throw new NotImplementedException("IsTerrain method is not implemented yet.");
        }

        public bool IsTerrain(string terrain, Pokemon target)
        {
            throw new NotImplementedException("IsTerrain method is not implemented yet.");
        }

        public bool IsTerrain(List<string> terrain, Pokemon target)
        {
            throw new NotImplementedException("IsTerrain method is not implemented yet.");
        }

        public bool IsTerrain(string terrain, Side target)
        {
            throw new NotImplementedException("IsTerrain method is not implemented yet.");
        }

        public bool IsTerrain(List<string> terrain, Side target)
        {
            throw new NotImplementedException("IsTerrain method is not implemented yet.");
        }

        public bool IsTerrain(string terrain, Battle target)
        {
            throw new NotImplementedException("IsTerrain method is not implemented yet.");
        }

        public bool IsTerrain(List<string> terrai, Battle target)
        {
            throw new NotImplementedException("IsTerrain method is not implemented yet.");
        }

        public Condition GetTerrain()
        {
            throw new NotImplementedException("GetTerrain method is not implemented yet.");
        }

        public bool AddPseudoWeather(string status, Pokemon source, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddPseudoWeather method is not implemented yet.");
        }

        public bool AddPseudoWeather(Condition status, Pokemon source, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddPseudoWeather method is not implemented yet.");
        }

        public bool AddPseudoWeather(string status, bool debug, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddPseudoWeather method is not implemented yet.");
        }

        public bool AddPseudoWeather(Condition status, bool debug, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddPseudoWeather method is not implemented yet.");
        }

        public bool AddPseudoWeather(string status, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddPseudoWeather method is not implemented yet.");
        }

        public bool AddPseudoWeather(Condition status, IEffect? sourceEffect = null)
        {
            throw new NotImplementedException("AddPseudoWeather method is not implemented yet.");
        }


        public Condition? GetPseudoWeather(string status)
        {
            throw new NotImplementedException("GetPseudoWeather method is not implemented yet.");
        }

        public Condition? GetPseudoWeather(IEffect status)
        {
            throw new NotImplementedException("GetPseudoWeather method is not implemented yet.");
        }

        public bool RemovePseudoWeather(string status)
        {
            throw new NotImplementedException("GetPseudoWeather method is not implemented yet.");
        }

        public bool RemovePseudoWeather(IEffect status)
        {
            throw new NotImplementedException("GetPseudoWeather method is not implemented yet.");
        }

        public void Destroy()
        {
            throw new NotImplementedException("Destroy method is not implemented yet.");
        }
    }
}