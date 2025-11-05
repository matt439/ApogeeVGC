using ApogeeVGC.Sim.Core;

namespace ApogeeVGC;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Check if we should run in server mode
        if (args.Length > 0 && args[0].Equals("server", StringComparison.OrdinalIgnoreCase))
        {
            // Remove "server" from args before passing to ASP.NET
            string[] serverArgs = args.Skip(1).ToArray();
            await ServerProgram.RunServerAsync(serverArgs);
            return;
        }

        // Default CLI mode: run test battle
        string format = args.Length > 0 ? args[0] : "doubles";
      
        Console.WriteLine($"Starting {format} battle...\n");
        
        var driver = new Driver();
        await driver.StartTest(format);
    }
}