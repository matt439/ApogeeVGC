using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        var driver = new Driver();
        // Parse --mode argument
        var mode = DriverMode.EquivalenceBatchTest;
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--mode" && Enum.TryParse<DriverMode>(args[i + 1], true, out var parsed))
            {
                mode = parsed;
                break;
            }
        }
        driver.Start(mode);
    }
}