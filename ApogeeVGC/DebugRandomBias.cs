using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC;

/// <summary>
/// Debug helper to investigate why Player 2 always wins
/// </summary>
public class DebugRandomBias
{
    public static void RunDebugTest()
    {
        Console.WriteLine("=== DEBUG: Player 2 Bias Investigation ===\n");
        
        var library = new Library();
        const int numTests = 10;
        int player1Wins = 0;
        int player2Wins = 0;
        int ties = 0;
        
        for (int i = 0; i < numTests; i++)
        {
            Console.WriteLine($"--- Battle {i + 1} ---");
            
            // Use INDEPENDENT seeds (not consecutive)
            PlayerOptions player1Options = new()
            {
                Type = Sim.Player.PlayerType.Random,
                Name = "Player 1",
                Team = TeamGenerator.GenerateTestTeam(library),
                Seed = new PrngSeed(10000 + i * 1000), // Independent, widely spaced
                PrintDebug = false,
            };

            PlayerOptions player2Options = new()
            {
                Type = Sim.Player.PlayerType.Random,
                Name = "Player 2",
                Team = TeamGenerator.GenerateTestTeam(library),
                Seed = new PrngSeed(50000 + i * 1000), // Independent, widely spaced
                PrintDebug = false,
            };

            BattleOptions battleOptions = new()
            {
                Id = FormatId.CustomSingles,
                Player1Options = player1Options,
                Player2Options = player2Options,
                Debug = false, // DISABLE DEBUG for cleaner output
                Sync = true,
                Seed = new PrngSeed(99000 + i * 1000), // Independent battle seed
                MaxTurns = 1000,
            };

            var simulator = new SyncSimulator();
            SimulatorResult result = simulator.Run(library, battleOptions, printDebug: false);
            
            switch (result)
            {
                case SimulatorResult.Player1Win:
                    player1Wins++;
                    Console.WriteLine("Winner: Player 1");
                    break;
                case SimulatorResult.Player2Win:
                    player2Wins++;
                    Console.WriteLine("Winner: Player 2");
                    break;
                case SimulatorResult.Tie:
                    ties++;
                    Console.WriteLine("Winner: Tie");
                    break;
            }
            
            Console.WriteLine();
        }
        
        Console.WriteLine("=== RESULTS ===");
        Console.WriteLine($"Player 1 Wins: {player1Wins}");
        Console.WriteLine($"Player 2 Wins: {player2Wins}");
        Console.WriteLine($"Ties: {ties}");
        Console.WriteLine($"Player 1 Win Rate: {(double)player1Wins / numTests:P1}");
        Console.WriteLine($"Player 2 Win Rate: {(double)player2Wins / numTests:P1}");
    }
}
