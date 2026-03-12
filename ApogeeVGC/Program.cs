using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        var driver = new Driver();
        // Parse --mode argument
        var mode = DriverMode.EquivalenceBatchTest;
        // First bare argument is the mode name
        if (args.Length > 0 && Enum.TryParse<DriverMode>(args[0], true, out var parsedMode))
        {
            mode = parsedMode;
        }
        else
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "--mode" && Enum.TryParse<DriverMode>(args[i + 1], true, out var parsed))
                {
                    mode = parsed;
                    break;
                }
            }
        }
        driver.Start(mode);
    }
}