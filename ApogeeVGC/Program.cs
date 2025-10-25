using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    public static async Task Main(string[] args)
    {
        var driver = new Driver();
        await driver.StartTest();
    }
}