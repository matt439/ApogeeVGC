using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using System.Text.Json.Nodes;

namespace ApogeeVGC.Sim.Utils;

public static partial class State
{
    public static JsonObject SerializeField(Field field)
    {
        // Field has special skip list - we skip 'id' and 'battle'
        var skip = new List<string> { "Id", "Battle" };
        JsonObject state = Serialize(field, skip, field.Battle);

        // Weather and Terrain are just IDs (enums), they serialize fine
        // WeatherState and TerrainState are EffectState objects that need to be serialized
        // PseudoWeather is a Dictionary<ConditionId, EffectState> that needs special handling

        // The generic Serialize should handle most of this, but we need to ensure
        // PseudoWeather dictionary is properly converted
        if (field.PseudoWeather.Count > 0)
        {
            var pseudoWeatherJson = new JsonObject();
            foreach (var kvp in field.PseudoWeather)
            {
                pseudoWeatherJson[kvp.Key.ToString()] = SerializeWithRefs(kvp.Value, field.Battle) as JsonNode;
            }
            state["pseudoWeather"] = pseudoWeatherJson;
        }

        return state;
    }

    public static void DeserializeField(JsonObject state, out Field field)
    {
        throw new NotImplementedException("DeserializeField requires a battle context - use the IBattle parameter version");
    }

    public static void DeserializeField(JsonObject state, Field field, IBattle battle)
    {
        // Field deserialization is simpler - we just update the existing field
        var skip = new List<string> { "Id", "Battle", "PseudoWeather" };
        Deserialize(state, field, skip, battle);

        // Handle PseudoWeather dictionary specially
        if (state.ContainsKey("pseudoWeather") && state["pseudoWeather"] is JsonObject pseudoWeatherJson)
        {
            field.PseudoWeather.Clear();
            foreach (var kvp in pseudoWeatherJson)
            {
                var conditionId = Enum.Parse<ConditionId>(kvp.Key);
                if (DeserializeWithRefs(kvp.Value, battle) is EffectState effectState)
                {
                    field.PseudoWeather[conditionId] = effectState;
                }
            }
        }
    }
}