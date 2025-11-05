using ApogeeVGC.Sim.Core;
using ApogeeVGC.Gui;

namespace ApogeeVGC;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Check if GUI mode is requested
        bool useGui = args.Length == 0 || args.Contains("--gui");
        
        if (useGui)
        {
            // Launch MonoGame GUI
            using var game = new BattleGame();
            game.Run();
        }
        else
        {
            // Run headless test/simulation
            var driver = new Driver();
            await driver.StartTest();
        }
    }
}