using System.Text;
using System.Text.RegularExpressions;

namespace ApogeeVGC.Tools;

/// <summary>
/// Tool to remove all Gigantamax form entries from species data files.
/// </summary>
public static class RemoveGigantamaxData
{
    public static void Run()
    {
        Console.WriteLine("=== Removing Gigantamax Data Entries ===\n");

        var filesToProcess = new[]
        {
            "ApogeeVGC/Data/SpeciesData/SpeciesData51To100.cs",
            "ApogeeVGC/Data/SpeciesData/SpeciesData101To150.cs",
            "ApogeeVGC/Data/SpeciesData/SpeciesData551To600.cs",
            "ApogeeVGC/Data/SpeciesData/SpeciesData801To850.cs",
            "ApogeeVGC/Data/SpeciesData/SpeciesData851To900.cs"
        };

        foreach (var filePath in filesToProcess)
        {
            ProcessFile(filePath);
        }

        Console.WriteLine("\n? All Gigantamax data entries removed successfully!");
    }

    private static void ProcessFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"? File not found: {filePath}");
            return;
        }

        var content = File.ReadAllText(filePath);
        var originalContent = content;

        // Pattern to match a full Gmax species entry
        // Matches from [SpecieId.NameGmax] = new() { ... },
        var pattern = @"\s*\[SpecieId\.\w+Gmax\]\s*=\s*new\(\)\s*\{[^\}]*\},\s*";
        
        var matches = Regex.Matches(content, pattern);
        
        if (matches.Count == 0)
        {
            Console.WriteLine($"  {Path.GetFileName(filePath)}: No Gmax entries found");
            return;
        }

        Console.WriteLine($"  {Path.GetFileName(filePath)}: Removing {matches.Count} Gmax entry(ies)");

        // Remove all matches
        content = Regex.Replace(content, pattern, "");

        // Write back to file
        File.WriteAllText(filePath, content);

        Console.WriteLine($"    ? Removed from {Path.GetFileName(filePath)}");
    }
}
