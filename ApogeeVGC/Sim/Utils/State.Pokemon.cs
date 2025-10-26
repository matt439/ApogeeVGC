using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using System.Reflection;
using System.Text.Json.Nodes;

namespace ApogeeVGC.Sim.Utils;

public static partial class State
{
    public static JsonObject SerializePokemon(Pokemon pokemon)
    {
        // Pokemon skip list from TypeScript: side, battle, set, name, fullname, id, 
        // happiness, level, pokeball, baseMoveSlots
        var skip = new List<string>
        {
            "Side", "Battle", "Set", "Name", "FullName", "Fullname", "Id",
            "Happiness", "Level", "Pokeball", "BaseMoveSlots",
        };
        JsonObject state = Serialize(pokemon, skip, pokemon.Battle);

        // Manually add the set
        state["set"] = SerializeWithRefs(pokemon.Set, pokemon.Battle) as JsonNode;

        // Only serialize baseMoveSlots if they differ from moveSlots
        if (pokemon.BaseMoveSlots.Count != pokemon.MoveSlots.Count ||
            !pokemon.BaseMoveSlots.Select((ms, i) => (ms, i))
                .All(pair => ReferenceEquals(pair.ms, pokemon.MoveSlots[pair.i])))
        {
            state["baseMoveSlots"] = SerializeWithRefs(pokemon.BaseMoveSlots, pokemon.Battle) as JsonNode;
        }

        return state;
    }

    public static void DeserializePokemon(JsonObject state, out Pokemon pokemon)
    {
        throw new NotImplementedException("DeserializePokemon requires a battle context - use the IBattle parameter version");
    }

    public static void DeserializePokemon(JsonObject state, Pokemon pokemon, IBattle battle)
    {
        // Pokemon skip list (same as serialization plus baseMoveSlots which needs special handling)
        var skip = new List<string>
        {
            "Side", "Battle", "Set", "Name", "FullName", "Fullname", "Id",
            "Happiness", "Level", "Pokeball", "BaseMoveSlots"
        };
        Deserialize(state, pokemon, skip, battle);

        // Set is readonly, so we skip it during deserialization
        // baseMoveSlots and moveSlots need to point to the same objects (identity, not equality)
        if (state.ContainsKey("baseMoveSlots"))
        {
            if (DeserializeWithRefs(state["baseMoveSlots"], battle) is List<object?> baseMoveSlots)
            {
                var typedBaseMoveSlots = new List<MoveSlot>();
                for (int i = 0; i < baseMoveSlots.Count; i++)
                {
                    if (baseMoveSlots[i] is MoveSlot moveSlot)
                    {
                        // If this matches a moveSlot, use the moveSlot reference instead
                        if (i < pokemon.MoveSlots.Count)
                        {
                            MoveSlot currentMoveSlot = pokemon.MoveSlots[i];
                            // Check if IDs match and it's not virtual
                            if (currentMoveSlot.Id == moveSlot.Id && (currentMoveSlot.Virtual != true))
                            {
                                typedBaseMoveSlots.Add(currentMoveSlot);
                                continue;
                            }
                        }
                        typedBaseMoveSlots.Add(moveSlot);
                    }
                }

                // Update BaseMoveSlots using reflection since it might be readonly
                PropertyInfo? baseMoveSlotsProp = typeof(Pokemon).GetProperty("BaseMoveSlots");
                if (baseMoveSlotsProp?.CanWrite == true)
                {
                    baseMoveSlotsProp.SetValue(pokemon, typedBaseMoveSlots);
                }
            }
        }
        else
        {
            // baseMoveSlots = moveSlots.slice()
            PropertyInfo? baseMoveSlotsProp = typeof(Pokemon).GetProperty("BaseMoveSlots");
            if (baseMoveSlotsProp?.CanWrite == true)
            {
                baseMoveSlotsProp.SetValue(pokemon, pokemon.MoveSlots.ToList());
            }
        }

        // Handle showCure special case - if undefined in state, set to undefined
        if (!state.ContainsKey("showCure"))
        {
            PropertyInfo? showCureProp = typeof(Pokemon).GetProperty("ShowCure");
            if (showCureProp?.CanWrite == true)
            {
                showCureProp.SetValue(pokemon, null);
            }
        }
    }

    /// <summary>
    /// Deserializes a PokemonSet from a JsonObject.
    /// </summary>
    private static PokemonSet DeserializePokemonSet(JsonObject setObj)
    {
        // Extract required fields
        string name = setObj["name"]?.GetValue<string>() ?? throw new InvalidOperationException("PokemonSet missing name");
        string speciesStr = setObj["species"]?.GetValue<string>() ?? throw new InvalidOperationException("PokemonSet missing species");

        // Parse species
        SpecieId species = Enum.TryParse(speciesStr, true, out SpecieId parsedSpecies)
            ? parsedSpecies
            : throw new InvalidOperationException($"Invalid species: {speciesStr}");

        // Extract optional fields
        ItemId item = ItemId.None;
        if (setObj.ContainsKey("item") && setObj["item"] is JsonValue itemValue)
        {
            string itemStr = itemValue.GetValue<string>();
            if (!string.IsNullOrEmpty(itemStr) && Enum.TryParse(itemStr, true, out ItemId parsedItem))
            {
                item = parsedItem;
            }
        }

        AbilityId ability = AbilityId.None;
        if (setObj.ContainsKey("ability") && setObj["ability"] is JsonValue abilityValue)
        {
            string abilityStr = abilityValue.GetValue<string>();
            if (!string.IsNullOrEmpty(abilityStr) && Enum.TryParse(abilityStr, true, out AbilityId parsedAbility))
            {
                ability = parsedAbility;
            }
        }

        // Extract moves
        var moves = new List<MoveId>();
        if (setObj.ContainsKey("moves") && setObj["moves"] is JsonArray movesArray)
        {
            foreach (JsonNode? moveNode in movesArray)
            {
                if (moveNode != null)
                {
                    string moveStr = moveNode.GetValue<string>();
                    if (Enum.TryParse(moveStr, true, out MoveId moveId))
                    {
                        moves.Add(moveId);
                    }
                }
            }
        }

        // Extract nature
        Nature? nature = null;
        if (setObj.ContainsKey("nature") && setObj["nature"] is JsonValue natureValue)
        {
            string natureStr = natureValue.GetValue<string>();
            if (!string.IsNullOrEmpty(natureStr) && Enum.TryParse(natureStr, true, out NatureId natureId))
            {
                nature = new Nature { Id = natureId };
            }
        }

        // Extract gender
        GenderId gender = GenderId.N;
        if (setObj.ContainsKey("gender") && setObj["gender"] is JsonValue genderValue)
        {
            string genderStr = genderValue.GetValue<string>();
            if (!string.IsNullOrEmpty(genderStr) && Enum.TryParse(genderStr, true, out GenderId parsedGender))
            {
                gender = parsedGender;
            }
        }

        // Extract EVs and IVs
        var evs = new StatsTable();
        if (setObj.ContainsKey("evs") && setObj["evs"] is JsonObject evsObj)
        {
            DeserializeStatsTable(evsObj, evs);
        }

        var ivs = new StatsTable();
        if (setObj.ContainsKey("ivs") && setObj["ivs"] is JsonObject ivsObj)
        {
            DeserializeStatsTable(ivsObj, ivs);
        }

        // Extract level
        int level = 100;
        if (setObj.ContainsKey("level") && setObj["level"] is JsonValue levelValue)
        {
            level = levelValue.GetValue<int>();
        }

        // Extract shiny
        bool shiny = false;
        if (setObj.ContainsKey("shiny") && setObj["shiny"] is JsonValue shinyValue)
        {
            shiny = shinyValue.GetValue<bool>();
        }

        // Extract happiness
        int happiness = 255;
        if (setObj.ContainsKey("happiness") && setObj["happiness"] is JsonValue happinessValue)
        {
            happiness = happinessValue.GetValue<int>();
        }

        // Extract pokeball
        PokeballId pokeball = PokeballId.Pokeball;
        if (setObj.ContainsKey("pokeball") && setObj["pokeball"] is JsonValue pokeballValue)
        {
            string pokeballStr = pokeballValue.GetValue<string>();
            if (!string.IsNullOrEmpty(pokeballStr) && Enum.TryParse(pokeballStr, true, out PokeballId parsedPokeball))
            {
                pokeball = parsedPokeball;
            }
        }

        // Create and return the PokemonSet
        return new PokemonSet
        {
            Name = name,
            Species = species,
            Item = item,
            Ability = ability,
            Moves = moves,
            Nature = nature ?? new Nature { Id = NatureId.Serious }, // Default to Serious if not specified
            Gender = gender,
            Evs = evs,
            Ivs = ivs,
            Level = level,
            Shiny = shiny,
            Happiness = happiness,
            Pokeball = pokeball,
        };
    }

    /// <summary>
    /// Deserializes stats from a JsonObject into a StatsTable.
    /// </summary>
    private static void DeserializeStatsTable(JsonObject statsObj, StatsTable stats)
    {
        if (statsObj.ContainsKey("hp") && statsObj["hp"] is JsonValue hpValue)
        {
            stats.Hp = hpValue.GetValue<int>();
        }
        if (statsObj.ContainsKey("atk") && statsObj["atk"] is JsonValue atkValue)
        {
            stats.Atk = atkValue.GetValue<int>();
        }
        if (statsObj.ContainsKey("def") && statsObj["def"] is JsonValue defValue)
        {
            stats.Def = defValue.GetValue<int>();
        }
        if (statsObj.ContainsKey("spa") && statsObj["spa"] is JsonValue spaValue)
        {
            stats.SpA = spaValue.GetValue<int>();
        }
        if (statsObj.ContainsKey("spd") && statsObj["spd"] is JsonValue spdValue)
        {
            stats.SpD = spdValue.GetValue<int>();
        }
        if (statsObj.ContainsKey("spe") && statsObj["spe"] is JsonValue speValue)
        {
            stats.Spe = speValue.GetValue<int>();
        }
    }
}