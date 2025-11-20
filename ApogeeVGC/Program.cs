using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        //// Debug test to investigate Player 2 bias
        //DebugRandomBias.RunDebugTest();

        var driver = new Driver();
        
        // Switch to GUI mode for testing
        driver.Start(DriverMode.GuiVsRandomDoubles);
        
        // Uncomment to test other modes:
        // driver.Start(DriverMode.GuiVsRandomSingles);
        // driver.Start(DriverMode.ConsoleVsRandomDoubles);
        // driver.Start(DriverMode.RandomVsRandomDoublesEvaluation);
        
        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
    }
}