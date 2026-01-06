using ApogeeVGC.Data.SpeciesData;

namespace ApogeeVGC.Tools;

/// <summary>
/// Interactive tool to search for specific species in the data.
/// </summary>
public static class SpeciesSearch
{
    public static void Run(string[] searchTerms)
    {
        var speciesData = new SpeciesData();
        var allSpecies = speciesData.SpeciesDataDictionary;

        if (searchTerms.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run --search-species <name1> <name2> ...");
            Console.WriteLine("\nExample: dotnet run --search-species pikachu charizard \"farfetch'd\"");
            return;
        }

        Console.WriteLine("=== Species Search Results ===\n");

        foreach (var term in searchTerms)
        {
            Console.WriteLine($"Searching for: {term}");
            
            var matches = allSpecies.Values
                .Where(s => s.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.Num)
                .ToList();

            if (matches.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ? Not found");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ? Found {matches.Count} match(es):");
                Console.ResetColor();
                
                foreach (var species in matches)
                {
                    var forme = species.Forme == default || string.IsNullOrEmpty(species.Forme.ToString()) 
                        ? "" 
                        : $" ({species.Forme})";
                    Console.WriteLine($"    - #{species.Num:000}: {species.Name}{forme}");
                    Console.WriteLine($"      Types: {string.Join(", ", species.Types)}");
                    Console.WriteLine($"      Abilities: {species.Abilities.Slot0}" +
                        (species.Abilities.Slot1 != default ? $" / {species.Abilities.Slot1}" : "") +
                        (species.Abilities.Hidden != default ? $" / {species.Abilities.Hidden} (H)" : ""));
                }
            }
            Console.WriteLine();
        }
    }
}
