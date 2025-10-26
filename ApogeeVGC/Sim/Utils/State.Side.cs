using System.Text.Json.Nodes;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Utils;

public static partial class State
{
    public static JsonObject SerializeSide(Side side)
    {
        // Side skip list from TypeScript: battle, team, pokemon, choice, activeRequest
        var skip = new List<string> { "Battle", "Team", "Pokemon", "Choice", "ActiveRequest" };
        JsonObject state = Serialize(side, skip, side.Battle);

        // Manually serialize pokemon array
        state["pokemon"] = new JsonArray(
            side.Pokemon.Select(SerializePokemon).ToArray()
        );

        // Encode team as position string (e.g., "1,2,3" or "123")
        // This represents the original team order mapping to current pokemon array positions
        var team = side.Pokemon.Select(t =>
            side.Team.IndexOf(t.Set)).Select(teamIndex => teamIndex + 1).ToList();
        state["team"] = team.Count > 9
            ? string.Join(",", team)
            : string.Join("", team);

        // Serialize choice (special handling for SwitchIns set)
        JsonObject choiceState = Serialize(side.Choice, ["SwitchIns"], side.Battle);
        choiceState["switchIns"] = new JsonArray(
            side.Choice.SwitchIns.Select(i => JsonValue.Create(i)).ToArray()
        );
        state["choice"] = choiceState;

        // ActiveRequest: If null, encode as tombstone to prevent recomputation
        if (side.ActiveRequest == null)
        {
            state["activeRequest"] = JsonValue.Create((string?)null);
        }
        // Otherwise, skip it (will be recomputed during deserialization)

        return state;
    }

    public static void DeserializeSide(JsonObject state, out Side side)
    {
        throw new NotImplementedException("DeserializeSide requires a battle context - use the IBattle parameter version");
    }

    public static void DeserializeSide(JsonObject state, Side side, IBattle battle)
    {
        // Side skip list (same as serialization)
        var skip = new List<string> { "Battle", "Team", "Pokemon", "Choice", "ActiveRequest" };
        Deserialize(state, side, skip, battle);

        // Deserialize pokemon array
        if (state.ContainsKey("pokemon") && state["pokemon"] is JsonArray pokemonArray)
        {
            for (int i = 0; i < pokemonArray.Count && i < side.Pokemon.Count; i++)
            {
                if (pokemonArray[i] is JsonObject pokemonState)
                {
                    DeserializePokemon(pokemonState, side.Pokemon[i], battle);
                }
            }
        }

        // Deserialize choice
        if (state.ContainsKey("choice") && state["choice"] is JsonObject choiceState)
        {
            var choiceSkip = new List<string> { "SwitchIns" };
            Deserialize(choiceState, side.Choice, choiceSkip, battle);

            // Deserialize SwitchIns set
            if (choiceState.ContainsKey("switchIns") && choiceState["switchIns"] is JsonArray switchInsArray)
            {
                side.Choice.SwitchIns.Clear();
                foreach (JsonNode? item in switchInsArray)
                {
                    if (item != null)
                    {
                        side.Choice.SwitchIns.Add(item.GetValue<int>());
                    }
                }
            }
        }

        // Note: team string is not deserialized back - it's only used during
        // battle reconstruction to reorder pokemon correctly
    }
}