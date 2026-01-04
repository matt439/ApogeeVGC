using ApogeeVGC.Sim.Core;
using ApogeeVGC.Tools;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        LearnsetJsonConverter.ConvertToJson(
            "pokemon-showdown/data/learnsets.ts",
            "Data/Learnsets/learnsets.json");

        return;

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

        var driver = new Driver();
        driver.Start(DriverMode.RandomVsRandomDoublesEvaluation);

        //Console.WriteLine("Press Enter key to exit...");
        //Console.ReadLine();
    }
}