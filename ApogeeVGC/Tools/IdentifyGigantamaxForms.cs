using ApogeeVGC.Data.SpeciesData;

namespace ApogeeVGC.Tools;

/// <summary>
/// Tool to identify all Gigantamax forms in the species data.
/// </summary>
public static class IdentifyGigantamaxForms
{
    public static void Run()
    {
        Console.WriteLine("=== Gigantamax Forms Identification ===\n");

        var speciesData = new SpeciesData();
        var allSpecies = speciesData.SpeciesDataDictionary;

        var gmaxForms = allSpecies.Values
            .Where(s => s.Name.Contains("-Gmax", StringComparison.OrdinalIgnoreCase))
            .OrderBy(s => s.Num)
            .ToList();

        Console.WriteLine($"Total Gigantamax forms found: {gmaxForms.Count}\n");

        Console.WriteLine("List of Gigantamax forms to remove:\n");
        Console.WriteLine("=== SpecieId Enum Entries ===");
        foreach (var species in gmaxForms)
        {
            Console.WriteLine($"    {species.Id},  // #{species.Num}: {species.Name}");
        }

        Console.WriteLine("\n=== Species Data Files ===");
        var groupedByNum = gmaxForms.GroupBy(s => s.Num / 50 * 50);
        
        foreach (var group in groupedByNum.OrderBy(g => g.Key))
        {
            var start = group.Key + 1;
            var end = Math.Min(group.Key + 50, 1050);
            Console.WriteLine($"\nSpeciesData{start}To{end}.cs:");
            foreach (var species in group.OrderBy(s => s.Num))
            {
                Console.WriteLine($"  - {species.Name} (#{species.Num})");
            }
        }

        Console.WriteLine("\n=== Summary by Base Species ===");
        var byBaseName = gmaxForms
            .Select(s => new { 
                Name = s.Name, 
                BaseName = s.Name.Replace("-Gmax", ""),
                Num = s.Num 
            })
            .OrderBy(s => s.BaseName);

        foreach (var species in byBaseName)
        {
            Console.WriteLine($"  {species.BaseName,-20} -> {species.Name,-30} (#{species.Num:000})");
        }

        Console.WriteLine($"\n=== Total: {gmaxForms.Count} Gigantamax forms ===");
    }
}
