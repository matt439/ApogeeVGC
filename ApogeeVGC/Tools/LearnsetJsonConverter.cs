using System.Text.Json;
using System.Text.RegularExpressions;
using ApogeeVGC.Data.Json;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Tools;

/// <summary>
/// Converts Pokemon Showdown's learnsets.ts TypeScript file to a JSON file.
/// This replaces the C# code generation approach to reduce compilation time.
/// </summary>
public static partial class LearnsetJsonConverter
{
    // Lookup dictionaries built from actual enum values (case-insensitive)
    private static readonly Dictionary<string, string> MoveIdLookup = BuildEnumLookup<MoveId>();
    private static readonly Dictionary<string, string> SpecieIdLookup = BuildEnumLookup<SpecieId>();

    private static readonly Dictionary<string, string> AbilityIdLookup =
        BuildEnumLookup<AbilityId>();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>
    /// Builds a case-insensitive lookup dictionary from enum values.
    /// Key: lowercase enum name, Value: actual enum name
    /// </summary>
    private static Dictionary<string, string> BuildEnumLookup<T>() where T : struct, Enum
    {
        var lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string name in Enum.GetNames<T>())
        {
            lookup[name.ToLowerInvariant()] = name;
        }

        return lookup;
    }

    /// <summary>
    /// Converts the TypeScript learnsets file to a JSON file.
    /// </summary>
    /// <param name="inputPath">Path to the learnsets.ts file</param>
    /// <param name="outputPath">Path where the JSON file will be written</param>
    public static void ConvertToJson(string inputPath, string outputPath)
    {
        Console.WriteLine("LearnsetJsonConverter - Converting TypeScript learnsets to JSON");
        Console.WriteLine($"Input:  {Path.GetFullPath(inputPath)}");
        Console.WriteLine($"Output: {Path.GetFullPath(outputPath)}");

        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"ERROR: Input file not found at {inputPath}");
            return;
        }

        string content = File.ReadAllText(inputPath);
        Console.WriteLine($"Read {content.Length:N0} characters from input file");

        MatchCollection speciesMatches = SpeciesEntryRegex().Matches(content);
        Console.WriteLine($"Found {speciesMatches.Count} species entries");

        var root = new LearnsetsJsonRoot();
        int validCount = 0;
        int skippedCount = 0;

        foreach (Match match in speciesMatches)
        {
            string speciesId = match.Groups["species"].Value;
            string entryContent = match.Groups["content"].Value;

            string? specieIdEnum = ConvertToSpecieIdEnum(speciesId);
            if (specieIdEnum == null)
            {
                skippedCount++;
                continue;
            }

            LearnsetJsonModel? learnsetModel = ParseLearnsetEntry(entryContent);
            if (learnsetModel != null)
            {
                root.Learnsets[specieIdEnum] = learnsetModel;
                validCount++;
            }
        }

        Console.WriteLine($"Valid species entries: {validCount}");
        Console.WriteLine($"Skipped species entries: {skippedCount}");

        // Ensure output directory exists
        string? outputDir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        string json = JsonSerializer.Serialize(root, JsonOptions);
        File.WriteAllText(outputPath, json);

        var fileInfo = new FileInfo(outputPath);
        Console.WriteLine($"Written {fileInfo.Length:N0} bytes to {outputPath}");
        Console.WriteLine("Done!");
    }

    private static LearnsetJsonModel? ParseLearnsetEntry(string entryContent)
    {
        var model = new LearnsetJsonModel();
        bool hasData = false;

        // Parse learnset moves
        Match learnsetMatch = LearnsetRegex().Match(entryContent);
        if (learnsetMatch.Success)
        {
            var learnset = ParseMoves(learnsetMatch.Groups["moves"].Value);
            if (learnset.Count > 0)
            {
                model.Learnset = learnset;
                hasData = true;
            }
        }

        // Parse eventData
        Match eventDataMatch = EventDataRegex().Match(entryContent);
        if (eventDataMatch.Success)
        {
            var events = ParseEvents(eventDataMatch.Groups["events"].Value);
            if (events.Count > 0)
            {
                model.EventData = events;
                hasData = true;
            }
        }

        // Parse eventOnly
        if (entryContent.Contains("eventOnly: true"))
        {
            model.EventOnly = true;
            hasData = true;
        }

        // Parse encounters
        Match encountersMatch = EncountersRegex().Match(entryContent);
        if (encountersMatch.Success)
        {
            var encounters = ParseEvents(encountersMatch.Groups["encounters"].Value);
            if (encounters.Count > 0)
            {
                model.Encounters = encounters;
                hasData = true;
            }
        }

        return hasData ? model : null;
    }

    private static Dictionary<string, string[]> ParseMoves(string movesContent)
    {
        var result = new Dictionary<string, string[]>();

        MatchCollection moveMatches = MoveEntryRegex().Matches(movesContent);
        foreach (Match moveMatch in moveMatches)
        {
            string moveId = moveMatch.Groups["move"].Value;
            string sources = moveMatch.Groups["sources"].Value;

            string? moveIdEnum = ConvertToMoveIdEnum(moveId);
            if (moveIdEnum == null) continue; // Skip unknown moves

            string[] sourcesList = ParseMoveSources(sources);
            if (sourcesList.Length > 0)
            {
                result[moveIdEnum] = sourcesList;
            }
        }

        return result;
    }

    private static string[] ParseMoveSources(string sourcesContent)
    {
        MatchCollection matches = SourceCodeRegex().Matches(sourcesContent);
        string[] result = new string[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            result[i] = matches[i].Groups["code"].Value;
        }

        return result;
    }

    private static List<EventInfoJsonModel> ParseEvents(string eventsContent)
    {
        var result = new List<EventInfoJsonModel>();

        MatchCollection eventMatches = SingleEventRegex().Matches(eventsContent);
        foreach (Match eventMatch in eventMatches)
        {
            string eventContent = eventMatch.Groups["event"].Value;
            var eventInfo = new EventInfoJsonModel();
            bool hasData = false;

            // Parse generation
            Match genMatch = GenerationRegex().Match(eventContent);
            if (genMatch.Success)
            {
                eventInfo.Generation = int.Parse(genMatch.Groups["gen"].Value);
                hasData = true;
            }

            // Parse level
            Match levelMatch = LevelRegex().Match(eventContent);
            if (levelMatch.Success)
            {
                eventInfo.Level = int.Parse(levelMatch.Groups["level"].Value);
            }

            // Parse gender
            Match genderMatch = GenderRegex().Match(eventContent);
            if (genderMatch.Success)
            {
                eventInfo.Gender = genderMatch.Groups["gender"].Value;
            }

            // Parse nature
            Match natureMatch = NatureRegex().Match(eventContent);
            if (natureMatch.Success)
            {
                eventInfo.Nature = natureMatch.Groups["nature"].Value;
            }

            // Parse shiny
            if (eventContent.Contains("shiny: true"))
            {
                eventInfo.Shiny = true;
            }
            else if (eventContent.Contains("shiny: 1"))
            {
                eventInfo.ShinySometimes = 1;
            }

            // Parse isHidden
            if (eventContent.Contains("isHidden: true"))
            {
                eventInfo.IsHidden = true;
            }

            // Parse perfectIvs
            Match perfectIvsMatch = PerfectIvsRegex().Match(eventContent);
            if (perfectIvsMatch.Success)
            {
                eventInfo.PerfectIvs = int.Parse(perfectIvsMatch.Groups["count"].Value);
            }

            // Parse IVs
            Match ivsMatch = IvsRegex().Match(eventContent);
            if (ivsMatch.Success)
            {
                eventInfo.Ivs = ParseIvs(ivsMatch.Groups["ivs"].Value);
            }

            // Parse abilities
            Match abilitiesMatch = AbilitiesRegex().Match(eventContent);
            if (abilitiesMatch.Success)
            {
                var abilities = ParseAbilities(abilitiesMatch.Groups["abilities"].Value);
                if (abilities.Count > 0)
                {
                    eventInfo.Abilities = abilities;
                }
            }

            // Parse moves
            Match movesMatch = EventMovesRegex().Match(eventContent);
            if (movesMatch.Success)
            {
                eventInfo.Moves = ParseEventMoves(movesMatch.Groups["moves"].Value);
            }

            // Parse pokeball
            Match pokeballMatch = PokeballRegex().Match(eventContent);
            if (pokeballMatch.Success)
            {
                eventInfo.Pokeball = pokeballMatch.Groups["ball"].Value;
            }

            if (hasData)
            {
                result.Add(eventInfo);
            }
        }

        return result;
    }

    private static Dictionary<string, int> ParseIvs(string ivsContent)
    {
        var result = new Dictionary<string, int>();

        Match hpMatch = Regex.Match(ivsContent, @"hp:\s*(\d+)");
        if (hpMatch.Success) result["hp"] = int.Parse(hpMatch.Groups[1].Value);

        Match atkMatch = Regex.Match(ivsContent, @"atk:\s*(\d+)");
        if (atkMatch.Success) result["atk"] = int.Parse(atkMatch.Groups[1].Value);

        Match defMatch = Regex.Match(ivsContent, @"def:\s*(\d+)");
        if (defMatch.Success) result["def"] = int.Parse(defMatch.Groups[1].Value);

        Match spaMatch = Regex.Match(ivsContent, @"spa:\s*(\d+)");
        if (spaMatch.Success) result["spa"] = int.Parse(spaMatch.Groups[1].Value);

        Match spdMatch = Regex.Match(ivsContent, @"spd:\s*(\d+)");
        if (spdMatch.Success) result["spd"] = int.Parse(spdMatch.Groups[1].Value);

        Match speMatch = Regex.Match(ivsContent, @"spe:\s*(\d+)");
        if (speMatch.Success) result["spe"] = int.Parse(speMatch.Groups[1].Value);

        return result;
    }

    private static List<string> ParseAbilities(string abilitiesContent)
    {
        var result = new List<string>();
        MatchCollection matches = Regex.Matches(abilitiesContent, """
            "([^"]+)"
            """);
        foreach (Match match in matches)
        {
            string? ability = ConvertToAbilityIdEnum(match.Groups[1].Value);
            if (ability != null)
            {
                result.Add(ability);
            }
        }

        return result;
    }

    private static List<string> ParseEventMoves(string movesContent)
    {
        var result = new List<string>();
        MatchCollection matches = Regex.Matches(movesContent, """
                                                              "([^"]+)"
                                                              """);
        foreach (Match match in matches)
        {
            result.Add(match.Groups[1].Value);
        }

        return result;
    }

    private static string? ConvertToSpecieIdEnum(string tsId)
    {
        if (tsId == "missingno") return null; // Skip MissingNo

        return SpecieIdLookup.GetValueOrDefault(tsId);
    }

    private static string? ConvertToMoveIdEnum(string tsId)
    {
        return MoveIdLookup.GetValueOrDefault(tsId);
    }

    private static string? ConvertToAbilityIdEnum(string tsId)
    {
        return AbilityIdLookup.GetValueOrDefault(tsId);
    }

    // Regex patterns using GeneratedRegex for performance
    [GeneratedRegex(@"\t(?<species>[a-z0-9]+):\s*\{(?<content>[\s\S]*?)\n\t\}", RegexOptions.None)]
    private static partial Regex SpeciesEntryRegex();

    [GeneratedRegex(@"learnset:\s*\{(?<moves>[\s\S]*?)\n\t\t\}")]
    private static partial Regex LearnsetRegex();

    [GeneratedRegex(@"(?<move>[a-z0-9]+):\s*\[(?<sources>[^\]]+)\]")]
    private static partial Regex MoveEntryRegex();

    [GeneratedRegex("""
                    "(?<code>[0-9][A-Z][^"]*?)"
                    """)]
    private static partial Regex SourceCodeRegex();

    [GeneratedRegex(@"eventData:\s*\[(?<events>[\s\S]*?)\n\t\t\]")]
    private static partial Regex EventDataRegex();

    [GeneratedRegex(@"encounters:\s*\[(?<encounters>[\s\S]*?)\n\t\t\]")]
    private static partial Regex EncountersRegex();

    [GeneratedRegex(@"\{(?<event>[^}]+)\}")]
    private static partial Regex SingleEventRegex();

    [GeneratedRegex(@"generation:\s*(?<gen>\d+)")]
    private static partial Regex GenerationRegex();

    [GeneratedRegex(@"level:\s*(?<level>\d+)")]
    private static partial Regex LevelRegex();

    [GeneratedRegex("""
                    gender:\s*"(?<gender>[MFN])"
                    """)]
    private static partial Regex GenderRegex();

    [GeneratedRegex("""
                    nature:\s*"(?<nature>[^"]+)"
                    """)]
    private static partial Regex NatureRegex();

    [GeneratedRegex(@"perfectIvs:\s*(?<count>\d+)")]
    private static partial Regex PerfectIvsRegex();

    [GeneratedRegex(@"ivs:\s*\{(?<ivs>[^}]+)\}")]
    private static partial Regex IvsRegex();

    [GeneratedRegex(@"abilities:\s*\[(?<abilities>[^\]]+)\]")]
    private static partial Regex AbilitiesRegex();

    [GeneratedRegex(@"moves:\s*\[(?<moves>[^\]]+)\]")]
    private static partial Regex EventMovesRegex();

    [GeneratedRegex("""
                    pokeball:\s*"(?<ball>[^"]+)"
                    """)]
    private static partial Regex PokeballRegex();
}