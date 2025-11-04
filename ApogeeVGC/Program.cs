using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    public static async Task Main(string[] args)
    {
// Default to doubles if no argument provided
     string format = args.Length > 0 ? args[0] : "doubles";
      
      Console.WriteLine($"Starting {format} battle...\n");
        
        var driver = new Driver();
        await driver.StartTest(format);
        // Console.ReadLine();
    }
}