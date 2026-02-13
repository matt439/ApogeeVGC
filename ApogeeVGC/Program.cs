using ApogeeVGC.Sim.Core;
using ApogeeVGC.Tools;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--generate-learnsets-json")
        {
            GenerateLearnsetJson();
            return;
        }

        // Validate species data conversion
        if (args.Length > 0 && args[0] == "--validate-species")
        {
            ValidateSpeciesData.Run();
            return;
        }

        // Generate species data report
        if (args.Length > 0 && args[0] == "--species-report")
        {
            SpeciesDataReport.GenerateReport();
            return;
        }

        // Search for specific species
        if (args.Length > 0 && args[0] == "--search-species")
        {
            var searchTerms = args.Skip(1).ToArray();
            SpeciesSearch.Run(searchTerms);
            return;
        }

        // Identify Gigantamax forms
        if (args.Length > 0 && args[0] == "--identify-gmax")
        {
            IdentifyGigantamaxForms.Run();
            return;
        }

        //// Debug test to investigate Player 2 bias
        //DebugRandomBias.RunDebugTest();


        // SingleBattleDebug or IncrementalDebug
        var driver = new Driver();
        driver.Start(DriverMode.SingleBattleDebugVgcRegI);


        //Console.WriteLine("Press Enter key to exit...");
        //Console.ReadLine();
    }

    private static void GenerateLearnsetJson()
    {
        try
        {
            // Use absolute paths for reliability
            string solutionDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            string inputPath = Path.Combine(solutionDir, "pokemon-showdown", "data", "learnsets.ts");
            string outputPath = Path.Combine(solutionDir, "ApogeeVGC", "Data", "Learnsets", "learnsets.json");

            Console.WriteLine($"Solution directory: {solutionDir}");
            Console.WriteLine($"Input path: {inputPath}");
            Console.WriteLine($"Output path: {outputPath}");
            Console.WriteLine($"Input exists: {File.Exists(inputPath)}");
            Console.WriteLine();

            // Pass true for writeIndented to generate human-readable JSON with line breaks
            // Use false (default) for production to minimize file size
            LearnsetJsonConverter.ConvertToJson(inputPath, outputPath, writeIndented: true);

            Console.WriteLine();
            Console.WriteLine($"Output exists: {File.Exists(outputPath)}");
            if (File.Exists(outputPath))
            {
                var info = new FileInfo(outputPath);
                Console.WriteLine($"Output size: {info.Length:N0} bytes");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}