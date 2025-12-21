using System.Text;
using System.Text.RegularExpressions;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Abilities;

namespace ApogeeVGC.Tools;

/// <summary>
/// Converts Pokemon Showdown's learnsets.ts TypeScript file to C# Learnsets partial class files.
/// Splits output into multiple files to avoid compiler performance issues.
/// </summary>
public static partial class LearnsetConverter
{
    private const int SpeciesPerFile = 150; // ~150 species per file to keep file sizes manageable

    // Lookup dictionaries built from actual enum values (case-insensitive)
    private static readonly Dictionary<string, string> MoveIdLookup = BuildEnumLookup<MoveId>();
    private static readonly Dictionary<string, string> SpecieIdLookup = BuildEnumLookup<SpecieId>();
    private static readonly Dictionary<string, string> AbilityIdLookup = BuildEnumLookup<AbilityId>();

    /// <summary>
    /// Builds a case-insensitive lookup dictionary from enum values.
    /// Key: lowercase enum name, Value: actual enum name
    /// </summary>
    private static Dictionary<string, string> BuildEnumLookup<T>() where T : struct, Enum
    {
        var lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in Enum.GetNames<T>())
        {
            lookup[name.ToLowerInvariant()] = name;
        }
        return lookup;
    }

    /// <summary>
    /// Converts the TypeScript learnsets file to multiple C# partial class files.
    /// </summary>
    /// <param name="inputPath">Path to the learnsets.ts file</param>
    /// <param name="outputDirectory">Directory where the generated C# files will be written</param>
    public static void ConvertToMultipleFiles(string inputPath, string outputDirectory)
    {
        Console.WriteLine("LearnsetConverter - Converting TypeScript learnsets to C# (split files)");
        Console.WriteLine($"Input:  {Path.GetFullPath(inputPath)}");
        Console.WriteLine($"Output: {Path.GetFullPath(outputDirectory)}");

        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"ERROR: Input file not found at {inputPath}");
            return;
        }

        var content = File.ReadAllText(inputPath);
        Console.WriteLine($"Read {content.Length:N0} characters from input file");

        var speciesMatches = SpeciesEntryRegex().Matches(content);
        Console.WriteLine($"Found {speciesMatches.Count} species entries");

        // Parse all species entries
        var speciesEntries = new List<(string speciesId, string entryContent)>();
        foreach (Match match in speciesMatches)
        {
            var speciesId = match.Groups["species"].Value;
            var entryContent = match.Groups["content"].Value;
            var specieIdEnum = ConvertToSpecieIdEnum(speciesId);
            if (specieIdEnum != null)
            {
                speciesEntries.Add((specieIdEnum, entryContent));
            }
        }

        Console.WriteLine($"Valid species entries: {speciesEntries.Count}");

        // Calculate number of files needed
        int totalFiles = (speciesEntries.Count + SpeciesPerFile - 1) / SpeciesPerFile;
        Console.WriteLine($"Splitting into {totalFiles} files (~{SpeciesPerFile} species each)");

        // Ensure output directory exists
        Directory.CreateDirectory(outputDirectory);

        // Generate main file with constructor and property
        GenerateMainFile(outputDirectory, totalFiles);

        // Generate partial files with data
        for (int fileIndex = 0; fileIndex < totalFiles; fileIndex++)
        {
            int startIndex = fileIndex * SpeciesPerFile;
            int endIndex = Math.Min(startIndex + SpeciesPerFile, speciesEntries.Count);
            var chunk = speciesEntries.Skip(startIndex).Take(endIndex - startIndex).ToList();

            GeneratePartialFile(outputDirectory, fileIndex + 1, chunk);
            Console.WriteLine($"  Generated file {fileIndex + 1}/{totalFiles} ({chunk.Count} species)");
        }

        Console.WriteLine("Done!");
    }

    private static void GenerateMainFile(string outputDirectory, int totalFiles)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System.Collections.ObjectModel;");
        sb.AppendLine("using ApogeeVGC.Sim.Abilities;");
        sb.AppendLine("using ApogeeVGC.Sim.Events;");
        sb.AppendLine("using ApogeeVGC.Sim.FormatClasses;");
        sb.AppendLine("using ApogeeVGC.Sim.Moves;");
        sb.AppendLine("using ApogeeVGC.Sim.SpeciesClasses;");
        sb.AppendLine("using ApogeeVGC.Sim.Stats;");
        sb.AppendLine();
        sb.AppendLine("namespace ApogeeVGC.Data;");
        sb.AppendLine();
        sb.AppendLine("// Auto-generated from pokemon-showdown/data/learnsets.ts");
        sb.AppendLine("// Do not edit manually - use LearnsetConverter.ConvertToMultipleFiles() to regenerate");
        sb.AppendLine($"// Split into {totalFiles} partial files for compiler performance");
        sb.AppendLine("public partial record Learnsets");
        sb.AppendLine("{");
        sb.AppendLine("    public IReadOnlyDictionary<SpecieId, Learnset> LearnsetsData { get; }");
        sb.AppendLine();
        sb.AppendLine("    public Learnsets()");
        sb.AppendLine("    {");
        sb.AppendLine("        var learnsets = new Dictionary<SpecieId, Learnset>();");

        // Call each partial initializer
        for (int i = 1; i <= totalFiles; i++)
        {
            sb.AppendLine($"        InitializeLearnsets{i}(learnsets);");
        }

        sb.AppendLine("        LearnsetsData = new ReadOnlyDictionary<SpecieId, Learnset>(learnsets);");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        var outputPath = Path.Combine(outputDirectory, "Learnsets.cs");
        File.WriteAllText(outputPath, sb.ToString());
    }

    private static void GeneratePartialFile(string outputDirectory, int fileNumber, List<(string speciesId, string entryContent)> entries)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using ApogeeVGC.Sim.Abilities;");
        sb.AppendLine("using ApogeeVGC.Sim.Events;");
        sb.AppendLine("using ApogeeVGC.Sim.FormatClasses;");
        sb.AppendLine("using ApogeeVGC.Sim.Moves;");
        sb.AppendLine("using ApogeeVGC.Sim.SpeciesClasses;");
        sb.AppendLine("using ApogeeVGC.Sim.Stats;");
        sb.AppendLine();
        sb.AppendLine("namespace ApogeeVGC.Data;");
        sb.AppendLine();
        sb.AppendLine($"// Auto-generated - Part {fileNumber}");
        sb.AppendLine("public partial record Learnsets");
        sb.AppendLine("{");
        sb.AppendLine($"    private static void InitializeLearnsets{fileNumber}(Dictionary<SpecieId, Learnset> learnsets)");
        sb.AppendLine("    {");

        foreach (var (speciesId, entryContent) in entries)
        {
            sb.AppendLine($"        learnsets[SpecieId.{speciesId}] = new Learnset");
            sb.AppendLine("        {");

            // Parse learnset
            var learnsetMatch = LearnsetRegex().Match(entryContent);
            if (learnsetMatch.Success)
            {
                ParseAndWriteLearnset(sb, learnsetMatch.Groups["moves"].Value);
            }

            // Parse eventData
            var eventDataMatch = EventDataRegex().Match(entryContent);
            if (eventDataMatch.Success)
            {
                ParseAndWriteEventData(sb, eventDataMatch.Groups["events"].Value);
            }

            // Parse eventOnly
            if (entryContent.Contains("eventOnly: true"))
            {
                sb.AppendLine("            EventOnly = true,");
            }

            // Parse encounters
            var encountersMatch = EncountersRegex().Match(entryContent);
            if (encountersMatch.Success)
            {
                ParseAndWriteEncounters(sb, encountersMatch.Groups["encounters"].Value);
            }

            sb.AppendLine("        };");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        var outputPath = Path.Combine(outputDirectory, $"Learnsets.Part{fileNumber}.cs");
        File.WriteAllText(outputPath, sb.ToString());
    }

    /// <summary>
    /// Legacy single-file conversion (may cause compiler issues with large files).
    /// </summary>
    public static void Convert(string inputPath, string outputPath)
    {
        var content = File.ReadAllText(inputPath);
        var result = ParseLearnsets(content);
        File.WriteAllText(outputPath, result);
    }

    /// <summary>
    /// Parses the TypeScript content and returns C# code as string (single file).
    /// </summary>
    public static string ParseLearnsets(string tsContent)
    {
        var sb = new StringBuilder();
        WriteHeader(sb);
        
        // Parse each species entry
        var speciesMatches = SpeciesEntryRegex().Matches(tsContent);
        
        foreach (Match match in speciesMatches)
        {
            var speciesId = match.Groups["species"].Value;
            var entryContent = match.Groups["content"].Value;
            
            var specieIdEnum = ConvertToSpecieIdEnum(speciesId);
            if (specieIdEnum == null) continue; // Skip unknown species
            
            sb.AppendLine($"        [SpecieId.{specieIdEnum}] = new Learnset");
            sb.AppendLine("        {");
            
            // Parse learnset
            var learnsetMatch = LearnsetRegex().Match(entryContent);
            if (learnsetMatch.Success)
            {
                ParseAndWriteLearnset(sb, learnsetMatch.Groups["moves"].Value);
            }
            
            // Parse eventData
            var eventDataMatch = EventDataRegex().Match(entryContent);
            if (eventDataMatch.Success)
            {
                ParseAndWriteEventData(sb, eventDataMatch.Groups["events"].Value);
            }
            
            // Parse eventOnly
            if (entryContent.Contains("eventOnly: true"))
            {
                sb.AppendLine("            EventOnly = true,");
            }
            
            // Parse encounters
            var encountersMatch = EncountersRegex().Match(entryContent);
                    if (encountersMatch.Success)
                    {
                        ParseAndWriteEncounters(sb, encountersMatch.Groups["encounters"].Value);
                    }

                    sb.AppendLine("        },");
                }

                WriteFooter(sb);
                return sb.ToString();
            }

            private static void WriteHeader(StringBuilder sb)
            {
                sb.AppendLine("using System.Collections.ObjectModel;");
                sb.AppendLine("using ApogeeVGC.Sim.Abilities;");
                sb.AppendLine("using ApogeeVGC.Sim.Events;");
                sb.AppendLine("using ApogeeVGC.Sim.FormatClasses;");
                sb.AppendLine("using ApogeeVGC.Sim.Moves;");
                sb.AppendLine("using ApogeeVGC.Sim.SpeciesClasses;");
                sb.AppendLine("using ApogeeVGC.Sim.Stats;");
                sb.AppendLine();
                sb.AppendLine("namespace ApogeeVGC.Data;");
                sb.AppendLine();
                sb.AppendLine("// Auto-generated from pokemon-showdown/data/learnsets.ts");
                sb.AppendLine("// Do not edit manually - use LearnsetConverter.Convert() to regenerate");
                sb.AppendLine("public record Learnsets");
                sb.AppendLine("{");
                sb.AppendLine("    public IReadOnlyDictionary<SpecieId, Learnset> LearnsetsData { get; }");
                sb.AppendLine();
                sb.AppendLine("    public Learnsets()");
                sb.AppendLine("    {");
                sb.AppendLine("        LearnsetsData = new ReadOnlyDictionary<SpecieId, Learnset>(_learnsets);");
                sb.AppendLine("    }");
                sb.AppendLine();
                sb.AppendLine("    private readonly Dictionary<SpecieId, Learnset> _learnsets = new()");
                sb.AppendLine("    {");
            }

            private static void WriteFooter(StringBuilder sb)
            {
                sb.AppendLine("    };");
                sb.AppendLine("}");
            }

            private static void ParseAndWriteLearnset(StringBuilder sb, string movesContent)
            {
                sb.AppendLine("            LearnsetData = new Dictionary<MoveId, List<MoveSource>>");
        sb.AppendLine("            {");
        
        var moveMatches = MoveEntryRegex().Matches(movesContent);
        foreach (Match moveMatch in moveMatches)
        {
            var moveId = moveMatch.Groups["move"].Value;
            var sources = moveMatch.Groups["sources"].Value;
            
            var moveIdEnum = ConvertToMoveIdEnum(moveId);
            if (moveIdEnum == null) continue; // Skip unknown moves
            
            var sourcesList = ParseMoveSources(sources);
            if (sourcesList.Count == 0) continue;
            
            sb.Append($"                [MoveId.{moveIdEnum}] = [");
            sb.Append(string.Join(", ", sourcesList.Select(s => $"MoveSource.Parse(\"{s}\")")));
            sb.AppendLine("],");
        }
        
        sb.AppendLine("            },");
    }

    private static List<string> ParseMoveSources(string sourcesContent)
    {
        var result = new List<string>();
        var matches = SourceCodeRegex().Matches(sourcesContent);
        foreach (Match match in matches)
        {
            result.Add(match.Groups["code"].Value);
        }
        return result;
    }

    private static void ParseAndWriteEventData(StringBuilder sb, string eventsContent)
    {
        sb.AppendLine("            EventData =");
        sb.AppendLine("            [");
        
        var eventMatches = SingleEventRegex().Matches(eventsContent);
        foreach (Match eventMatch in eventMatches)
        {
            var eventContent = eventMatch.Groups["event"].Value;
            sb.AppendLine("                new EventInfo");
            sb.AppendLine("                {");
            
            // Parse generation
            var genMatch = GenerationRegex().Match(eventContent);
            if (genMatch.Success)
            {
                sb.AppendLine($"                    Generation = {genMatch.Groups["gen"].Value},");
            }
            
            // Parse level
            var levelMatch = LevelRegex().Match(eventContent);
            if (levelMatch.Success)
            {
                sb.AppendLine($"                    Level = {levelMatch.Groups["level"].Value},");
            }
            
            // Parse gender
            var genderMatch = GenderRegex().Match(eventContent);
            if (genderMatch.Success)
            {
                var gender = genderMatch.Groups["gender"].Value;
                var genderId = gender switch
                {
                    "M" => "GenderId.M",
                    "F" => "GenderId.F",
                    "N" => "GenderId.N",
                    _ => null
                };
                if (genderId != null)
                {
                    sb.AppendLine($"                    Gender = {genderId},");
                }
            }
            
            // Parse nature
            var natureMatch = NatureRegex().Match(eventContent);
            if (natureMatch.Success)
            {
                sb.AppendLine($"                    Nature = \"{natureMatch.Groups["nature"].Value}\",");
            }
            
            // Parse shiny
            if (eventContent.Contains("shiny: true"))
            {
                sb.AppendLine("                    Shiny = true,");
            }
            else if (eventContent.Contains("shiny: 1"))
            {
                sb.AppendLine("                    ShinySometimes = 1,");
            }
            
            // Parse isHidden
            if (eventContent.Contains("isHidden: true"))
            {
                sb.AppendLine("                    IsHidden = true,");
            }
            
            // Parse perfectIvs
            var perfectIvsMatch = PerfectIvsRegex().Match(eventContent);
            if (perfectIvsMatch.Success)
            {
                sb.AppendLine($"                    PerfectIvs = {perfectIvsMatch.Groups["count"].Value},");
            }
            
            // Parse IVs
            var ivsMatch = IvsRegex().Match(eventContent);
            if (ivsMatch.Success)
            {
                sb.AppendLine($"                    Ivs = new Dictionary<StatId, int> {{ {ParseIvs(ivsMatch.Groups["ivs"].Value)} }},");
            }
            
            // Parse abilities
            var abilitiesMatch = AbilitiesRegex().Match(eventContent);
            if (abilitiesMatch.Success)
            {
                var abilities = ParseAbilities(abilitiesMatch.Groups["abilities"].Value);
                if (abilities.Count > 0)
                {
                    sb.AppendLine($"                    Abilities = [{string.Join(", ", abilities.Select(a => $"AbilityId.{a}"))}],");
                }
            }
            
            // Parse moves
            var movesMatch = EventMovesRegex().Match(eventContent);
            if (movesMatch.Success)
            {
                var moves = ParseEventMoves(movesMatch.Groups["moves"].Value);
                sb.AppendLine($"                    Moves = [{string.Join(", ", moves.Select(m => $"\"{m}\""))}],");
            }
            
            // Parse pokeball
            var pokeballMatch = PokeballRegex().Match(eventContent);
            if (pokeballMatch.Success)
            {
                sb.AppendLine($"                    Pokeball = \"{pokeballMatch.Groups["ball"].Value}\",");
            }
            
            sb.AppendLine("                },");
        }
        
        sb.AppendLine("            ],");
    }

    private static void ParseAndWriteEncounters(StringBuilder sb, string encountersContent)
    {
        sb.AppendLine("            Encounters =");
        sb.AppendLine("            [");
        
        var encounterMatches = SingleEventRegex().Matches(encountersContent);
        foreach (Match encounterMatch in encounterMatches)
        {
            var eventContent = encounterMatch.Groups["event"].Value;
            sb.AppendLine("                new EventInfo");
            sb.AppendLine("                {");
            
            // Parse generation
            var genMatch = GenerationRegex().Match(eventContent);
            if (genMatch.Success)
            {
                sb.AppendLine($"                    Generation = {genMatch.Groups["gen"].Value},");
            }
            
            // Parse level
            var levelMatch = LevelRegex().Match(eventContent);
            if (levelMatch.Success)
            {
                sb.AppendLine($"                    Level = {levelMatch.Groups["level"].Value},");
            }
            
            sb.AppendLine("                },");
        }
        
        sb.AppendLine("            ],");
    }

    private static string ParseIvs(string ivsContent)
    {
        var parts = new List<string>();
        
        var hpMatch = Regex.Match(ivsContent, @"hp:\s*(\d+)");
        if (hpMatch.Success) parts.Add($"[StatId.Hp] = {hpMatch.Groups[1].Value}");
        
        var atkMatch = Regex.Match(ivsContent, @"atk:\s*(\d+)");
        if (atkMatch.Success) parts.Add($"[StatId.Atk] = {atkMatch.Groups[1].Value}");
        
        var defMatch = Regex.Match(ivsContent, @"def:\s*(\d+)");
        if (defMatch.Success) parts.Add($"[StatId.Def] = {defMatch.Groups[1].Value}");
        
        var spaMatch = Regex.Match(ivsContent, @"spa:\s*(\d+)");
        if (spaMatch.Success) parts.Add($"[StatId.SpA] = {spaMatch.Groups[1].Value}");
        
        var spdMatch = Regex.Match(ivsContent, @"spd:\s*(\d+)");
        if (spdMatch.Success) parts.Add($"[StatId.SpD] = {spdMatch.Groups[1].Value}");
        
        var speMatch = Regex.Match(ivsContent, @"spe:\s*(\d+)");
        if (speMatch.Success) parts.Add($"[StatId.Spe] = {speMatch.Groups[1].Value}");
        
        return string.Join(", ", parts);
    }

    private static List<string> ParseAbilities(string abilitiesContent)
    {
        var result = new List<string>();
        var matches = Regex.Matches(abilitiesContent, @"""([^""]+)""");
        foreach (Match match in matches)
        {
            var ability = ConvertToAbilityIdEnum(match.Groups[1].Value);
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
        var matches = Regex.Matches(movesContent, @"""([^""]+)""");
            foreach (Match match in matches)
            {
                result.Add(match.Groups[1].Value);
            }
            return result;
        }

        /// <summary>
        /// Converts a TypeScript species ID (lowercase, no separators) to C# SpecieId enum name.
        /// Uses reflection-based lookup for exact enum matching.
        /// </summary>
        private static string? ConvertToSpecieIdEnum(string tsId)
        {
            if (tsId == "missingno") return null; // Skip MissingNo

            // Try direct lookup first (handles most cases)
            if (SpecieIdLookup.TryGetValue(tsId, out var enumName))
            {
                return enumName;
            }

            // Return null for unknown species (will be skipped)
            return null;
        }

        /// <summary>
        /// Converts a TypeScript move ID (lowercase, no separators) to C# MoveId enum name.
        /// Uses reflection-based lookup for exact enum matching.
        /// </summary>
        private static string? ConvertToMoveIdEnum(string tsId)
        {
            // Try direct lookup first (handles most cases)
            if (MoveIdLookup.TryGetValue(tsId, out var enumName))
            {
                return enumName;
            }

            // Return null for unknown moves (will be skipped)
            return null;
        }

        /// <summary>
        /// Converts a TypeScript ability ID to C# AbilityId enum name.
        /// Uses reflection-based lookup for exact enum matching.
        /// </summary>
        private static string? ConvertToAbilityIdEnum(string tsId)
        {
            if (AbilityIdLookup.TryGetValue(tsId, out var enumName))
            {
                return enumName;
            }
            return null;
        }

        /// <summary>
        /// Converts a lowercase identifier to PascalCase.
        /// </summary>
        private static string ConvertToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // Simple conversion: capitalize first letter
            // For compound names, we rely on the special cases above
        var result = new StringBuilder();
        bool capitalizeNext = true;

        foreach (char c in input)
        {
            if (capitalizeNext && char.IsLetter(c))
            {
                result.Append(char.ToUpper(c));
                capitalizeNext = false;
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    // Regex patterns using GeneratedRegex for performance
    // Match species entry: starts with tab + name, captures everything until tab + },
    [GeneratedRegex(@"\t(?<species>[a-z0-9]+):\s*\{(?<content>[\s\S]*?)\n\t\}", RegexOptions.None)]
    private static partial Regex SpeciesEntryRegex();

    [GeneratedRegex(@"learnset:\s*\{(?<moves>[\s\S]*?)\n\t\t\}")]
    private static partial Regex LearnsetRegex();

    [GeneratedRegex(@"(?<move>[a-z0-9]+):\s*\[(?<sources>[^\]]+)\]")]
    private static partial Regex MoveEntryRegex();

    [GeneratedRegex(@"""(?<code>[0-9][A-Z][^""]*?)""")]
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

    [GeneratedRegex(@"gender:\s*""(?<gender>[MFN])""")]
    private static partial Regex GenderRegex();

    [GeneratedRegex(@"nature:\s*""(?<nature>[^""]+)""")]
    private static partial Regex NatureRegex();

    [GeneratedRegex(@"perfectIvs:\s*(?<count>\d+)")]
    private static partial Regex PerfectIvsRegex();

    [GeneratedRegex(@"ivs:\s*\{(?<ivs>[^}]+)\}")]
    private static partial Regex IvsRegex();

    [GeneratedRegex(@"abilities:\s*\[(?<abilities>[^\]]+)\]")]
    private static partial Regex AbilitiesRegex();

    [GeneratedRegex(@"moves:\s*\[(?<moves>[^\]]+)\]")]
    private static partial Regex EventMovesRegex();

    [GeneratedRegex(@"pokeball:\s*""(?<ball>[^""]+)""")]
    private static partial Regex PokeballRegex();
}
