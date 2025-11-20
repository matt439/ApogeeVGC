using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    //[STAThread]
    public static void Main(string[] args)
    {
        var driver = new Driver();
        driver.Start(DriverMode.RandomVsRandomSingles);
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}