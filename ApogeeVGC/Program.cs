using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        Driver driver = new();
        // Parse --mode argument
        DriverMode mode = DriverMode.RndVsRndEvaluation;
        string? format = null;
        // First bare argument is the mode name
        if (args.Length > 0 && Enum.TryParse(args[0], true, out DriverMode parsedMode))
        {
            mode = parsedMode;
        }

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--mode" && Enum.TryParse(args[i + 1], true, out DriverMode parsed))
            {
                mode = parsed;
            }
            else if (args[i] == "--format")
            {
                format = args[i + 1];
            }
        }

        driver.Start(mode, format);
    }
}