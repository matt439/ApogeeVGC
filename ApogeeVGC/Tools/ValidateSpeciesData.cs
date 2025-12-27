using System.Text;
using System.Text.RegularExpressions;
using ApogeeVGC.Data.SpeciesData;

namespace ApogeeVGC.Tools;

/// <summary>
/// Tool to validate that all species from the TypeScript pokedex.ts file
/// have been converted to C# SpeciesData files.
/// </summary>
public static class ValidateSpeciesData
{
    public static void Run()
    {
        Console.WriteLine("=== Species Data Validation ===\n");

        var speciesData = new SpeciesData();
        var allSpecies = speciesData.SpeciesDataDictionary;

        Console.WriteLine($"Total species in C# data: {allSpecies.Count}\n");

        // Parse the TypeScript file to extract all species
        var tsSpecies = ParseTypeScriptPokedex();

        Console.WriteLine($"Total species in TypeScript file: {tsSpecies.Count}\n");

        // Find missing species
        var missingInCSharp = new List<string>();
        var extraInCSharp = new List<string>();

        foreach (var tsName in tsSpecies)
        {
            var found = allSpecies.Values.Any(s =>
                NormalizeSpeciesName(s.Name) == NormalizeSpeciesName(tsName));

            if (!found)
            {
                missingInCSharp.Add(tsName);
            }
        }

        foreach (var csSpecies in allSpecies.Values)
        {
            var normalized = NormalizeSpeciesName(csSpecies.Name);
            if (!tsSpecies.Any(ts => NormalizeSpeciesName(ts) == normalized))
            {
                extraInCSharp.Add(csSpecies.Name);
            }
        }

        // Report results
        if (missingInCSharp.Count == 0 && extraInCSharp.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("? All species validated successfully!");
            Console.WriteLine("  No missing or extra species found.");
            Console.ResetColor();
        }
        else
        {
            if (missingInCSharp.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"? Missing {missingInCSharp.Count} species in C#:");
                Console.ResetColor();
                foreach (var name in missingInCSharp.OrderBy(n => n))
                {
                    Console.WriteLine($"  - {name}");
                }
                Console.WriteLine();
            }

            if (extraInCSharp.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"? Extra {extraInCSharp.Count} species in C# (not in TypeScript):");
                Console.ResetColor();
                foreach (var name in extraInCSharp.OrderBy(n => n))
                {
                    Console.WriteLine($"  - {name}");
                }
                Console.WriteLine();
            }
        }

        // Validate ordering by num
        Console.WriteLine("\n=== Validating Species Order ===\n");
        ValidateSpeciesOrder(allSpecies);
    }

    private static List<string> ParseTypeScriptPokedex()
    {
        var tsPath = Path.Combine("..", "..", "pokemon-showdown", "data", "pokedex.ts");
        var fullPath = Path.GetFullPath(tsPath);

        if (!File.Exists(fullPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"? TypeScript pokedex.ts file not found at: {fullPath}");
            Console.ResetColor();
            return new List<string>();
        }

        var content = File.ReadAllText(fullPath);
        var species = new List<string>();

        // Find the start of the export
        var exportMatch = Regex.Match(content, @"export const Pokedex:.*?=\s*\{", RegexOptions.Singleline);
        if (!exportMatch.Success)
        {
            Console.WriteLine("Warning: Could not find 'export const Pokedex' declaration");
            return species;
        }

        // Extract content from the Pokedex object
        var startIndex = exportMatch.Index + exportMatch.Length;

        // Find matching closing brace
        int braceCount = 1;
        int endIndex = startIndex;

        for (int i = startIndex; i < content.Length && braceCount > 0; i++)
        {
            if (content[i] == '{') braceCount++;
            else if (content[i] == '}') braceCount--;
            endIndex = i;
        }

        var pokedexContent = content.Substring(startIndex, endIndex - startIndex);

        // Match top-level species entries: key: {
        // Use a pattern that looks for identifiers followed by colon and opening brace at the start of a line
        var pattern = @"^\s*(\w+):\s*\{";
        var matches = Regex.Matches(pokedexContent, pattern, RegexOptions.Multiline);

        foreach (Match match in matches)
        {
            if (match.Groups.Count > 1)
            {
                var name = match.Groups[1].Value;

                // Skip common property names that aren't species
                if (name != "genderRatio" && 
                    name != "baseStats" && 
                    name != "abilities" &&
                    name != "eggGroups" &&
                    name != "otherFormes" &&
                    name != "formeOrder" &&
                    name != "cosmeticFormes")
                {
                    species.Add(name);
                }
            }
        }

        return species;
    }

    private static string NormalizeSpeciesName(string name)
    {
        // Remove spaces, hyphens, apostrophes, and convert to lowercase for comparison
        return name.Replace(" ", "")
            .Replace("-", "")
            .Replace("'", "")
            .Replace("'", "")
            .Replace(".", "")
            .ToLowerInvariant();
    }

    private static void ValidateSpeciesOrder(IReadOnlyDictionary<Sim.SpeciesClasses.SpecieId, Sim.SpeciesClasses.Species> allSpecies)
    {
        var orderedByNum = allSpecies.Values
            .OrderBy(s => s.Num)
            .ThenBy(s => s.Name)
            .ToList();

        var issues = new List<string>();
        var previousNum = 0;

        foreach (var species in orderedByNum)
        {
            if (species.Num < 1 || species.Num > 1025)
            {
                issues.Add($"  - {species.Name} (#{species.Num}): Invalid num value");
            }

            if (species.Num < previousNum)
            {
                issues.Add($"  - {species.Name} (#{species.Num}): Out of order (previous was #{previousNum})");
            }

            previousNum = species.Num;
        }

        if (issues.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("? All species are in correct order by num field");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"? Found {issues.Count} ordering issues:");
            Console.ResetColor();
            foreach (var issue in issues)
            {
                Console.WriteLine(issue);
            }
        }

        // Show distribution across files
        Console.WriteLine("\n=== Species Distribution by Num ===\n");
        var ranges = new[]
        {
            (1, 50), (51, 100), (101, 150), (151, 200), (201, 250),
            (251, 300), (301, 350), (351, 400), (401, 450), (451, 500),
            (501, 550), (551, 600), (601, 650), (651, 700), (701, 750),
            (751, 800), (801, 850), (851, 900), (901, 950), (951, 1000),
            (1001, 1050)
        };

        foreach (var (start, end) in ranges)
        {
            var count = allSpecies.Values.Count(s => s.Num >= start && s.Num <= end);
            Console.WriteLine($"  {start,4}-{end,4}: {count,3} species");
        }
    }
}
