using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Ui;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        //// Test the DecodeDoublesChoice fix
        //ChoiceGenerator.TestDecodeDoublesChoice();

        //// Test a scenario that might cause empty doubles choices
        //TestDoublesChoiceGeneration();

        //// Generate team preview choices file
        //ChoiceGenerator.WriteTeamPreviewChoicesToFile();

        var driver = new Driver();
        driver.Start(DriverMode.RandomVsRandomEvaluation);
    }
    
    private static void TestDoublesChoiceGeneration()
    {
        Console.WriteLine("\n=== Testing Doubles Choice Generation ===");
        
        // Test with some common single choices
        Choice[] slot1Choices = [Choice.Move1NormalFoe1, Choice.Switch1];
        Choice[] slot2Choices = [Choice.Move2NormalFoe2, Choice.Switch2];
        
        Console.WriteLine("Testing with sample choices...");
        var result = ChoiceGenerator.GenerateValidDoublesChoices(slot1Choices, slot2Choices);
        
        Console.WriteLine($"Generated {result.Length} doubles choices:");
        foreach (var choice in result)
        {
            Console.WriteLine($"  {choice}");
        }
        
        if (result.Length == 0)
        {
            Console.WriteLine("No choices generated - running diagnostics:");
            ChoiceGenerator.DiagnoseDoublesChoiceGeneration(slot1Choices, slot2Choices);
        }
        
        Console.WriteLine();
    }
}