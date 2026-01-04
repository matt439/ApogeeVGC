using System.Collections.ObjectModel;
using System.Text.Json;
using ApogeeVGC.Data.Json;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using EventInfo = ApogeeVGC.Sim.Events.EventInfo;

namespace ApogeeVGC.Data;

/// <summary>
/// Provides access to Pokemon learnset data loaded from an external JSON file.
/// This approach reduces compilation time compared to compiled C# data.
/// </summary>
public record Learnsets
{
    private const string LearnsetJsonFileName = "learnsets.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IReadOnlyDictionary<SpecieId, Learnset> LearnsetsData { get; }

    public Learnsets()
    {
        LearnsetsData = LoadFromJson();
    }

    private static IReadOnlyDictionary<SpecieId, Learnset> LoadFromJson()
    {
        var jsonPath = GetJsonFilePath();

        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException(
                $"Learnset data file not found at '{jsonPath}'. " +
                "Run LearnsetJsonConverter.ConvertToJson() to generate it.",
                jsonPath);
        }

        var json = File.ReadAllText(jsonPath);
        var root = JsonSerializer.Deserialize<LearnsetsJsonRoot>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize learnsets JSON.");

        var result = new Dictionary<SpecieId, Learnset>();

        foreach (var kvp in root.Learnsets)
        {
            if (!Enum.TryParse<SpecieId>(kvp.Key, ignoreCase: true, out var specieId))
            {
                continue; // Skip unknown species
            }

            var learnset = ConvertToLearnset(kvp.Value);
            result[specieId] = learnset;
        }

        return new ReadOnlyDictionary<SpecieId, Learnset>(result);
    }

    private static string GetJsonFilePath()
    {
        // Try to find JSON file relative to the executable
        var exeDir = AppContext.BaseDirectory;
        var jsonPath = Path.Combine(exeDir, "Data", "Learnsets", LearnsetJsonFileName);

        if (File.Exists(jsonPath))
        {
            return jsonPath;
        }

        // Fallback: check current directory structure (for development)
        var devPath = Path.Combine("Data", "Learnsets", LearnsetJsonFileName);
        if (File.Exists(devPath))
        {
            return devPath;
        }

        // Fallback: check one level up (common in some project structures)
        var parentPath = Path.Combine("..", "Data", "Learnsets", LearnsetJsonFileName);
        if (File.Exists(parentPath))
        {
            return parentPath;
        }

        // Return the expected path for error message
        return jsonPath;
    }

    private static Learnset ConvertToLearnset(LearnsetJsonModel model)
    {
        Dictionary<MoveId, List<MoveSource>>? learnsetData = null;
        if (model.Learnset is { Count: > 0 })
        {
            learnsetData = new Dictionary<MoveId, List<MoveSource>>();
            foreach (var kvp in model.Learnset)
            {
                if (!Enum.TryParse<MoveId>(kvp.Key, ignoreCase: true, out var moveId))
                {
                    continue; // Skip unknown moves
                }

                var sourceList = kvp.Value
                    .Select(MoveSource.Parse)
                    .ToList();

                if (sourceList.Count > 0)
                {
                    learnsetData[moveId] = sourceList;
                }
            }
        }

        List<EventInfo>? eventData = null;
        if (model.EventData is { Count: > 0 })
        {
            eventData = model.EventData.Select(ConvertToEventInfo).ToList();
        }

        List<EventInfo>? encounters = null;
        if (model.Encounters is { Count: > 0 })
        {
            encounters = model.Encounters.Select(ConvertToEventInfo).ToList();
        }

        return new Learnset
        {
            LearnsetData = learnsetData,
            EventData = eventData,
            EventOnly = model.EventOnly,
            Encounters = encounters
        };
    }

    private static EventInfo ConvertToEventInfo(EventInfoJsonModel model)
    {
        Dictionary<StatId, int>? ivs = null;
        if (model.Ivs is { Count: > 0 })
        {
            ivs = new Dictionary<StatId, int>();
            foreach (var kvp in model.Ivs)
            {
                var statId = kvp.Key.ToLowerInvariant() switch
                {
                    "hp" => StatId.Hp,
                    "atk" => StatId.Atk,
                    "def" => StatId.Def,
                    "spa" => StatId.SpA,
                    "spd" => StatId.SpD,
                    "spe" => StatId.Spe,
                    _ => (StatId?)null
                };

                if (statId.HasValue)
                {
                    ivs[statId.Value] = kvp.Value;
                }
            }
        }

        List<AbilityId>? abilities = null;
        if (model.Abilities is { Count: > 0 })
        {
            abilities = new List<AbilityId>();
            foreach (var abilityString in model.Abilities)
            {
                if (Enum.TryParse<AbilityId>(abilityString, ignoreCase: true, out var abilityId))
                {
                    abilities.Add(abilityId);
                }
            }
        }

        GenderId? gender = model.Gender switch
        {
            "M" => GenderId.M,
            "F" => GenderId.F,
            "N" => GenderId.N,
            _ => null
        };

        return new EventInfo
        {
            Generation = model.Generation,
            Level = model.Level,
            Shiny = model.Shiny,
            ShinySometimes = model.ShinySometimes,
            Gender = gender,
            Nature = model.Nature,
            Ivs = ivs,
            PerfectIvs = model.PerfectIvs,
            IsHidden = model.IsHidden,
            Abilities = abilities,
            MaxEggMoves = model.MaxEggMoves,
            Moves = model.Moves,
            Pokeball = model.Pokeball,
            From = model.From,
            Japan = model.Japan,
            EmeraldEventEgg = model.EmeraldEventEgg
        };
    }
}
