using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        var driver = new Driver();
        // Parse --mode argument
        var mode = DriverMode.EquivalenceBatchTest;
        string? format = null;
        // First bare argument is the mode name
        if (args.Length > 0 && Enum.TryParse<DriverMode>(args[0], true, out var parsedMode))
        {
            mode = parsedMode;
        }
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--mode" && Enum.TryParse<DriverMode>(args[i + 1], true, out var parsed))
                mode = parsed;
            else if (args[i] == "--format")
                format = args[i + 1];
        }
        driver.Start(mode, format);
    }
}