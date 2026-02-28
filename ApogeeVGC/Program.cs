using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        var driver = new Driver();
        driver.Start(DriverMode.SingleBattleDebugVgcRegI);

        //Console.WriteLine("Press Enter key to exit...");
        //Console.ReadLine();
    }
}