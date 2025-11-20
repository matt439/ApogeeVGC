using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        //// Debug test to investigate Player 2 bias
        //DebugRandomBias.RunDebugTest();

        var driver = new Driver();
        driver.Start(DriverMode.RandomVsRandomDoubles);
        Console.WriteLine("Press Enter key to exit...");
        Console.ReadLine();
    }
}