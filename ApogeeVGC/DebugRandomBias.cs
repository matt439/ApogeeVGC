using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.Utils;
using System.Text;

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
        const int numTests = 100;  // Run many battles to verify fix
        int player1Wins = 0;
        int player2Wins = 0;
        int ties = 0;
        
        for (int i = 0; i < numTests; i++)
        {
            Console.WriteLine($"\n??????????????????????????????????????????");
            Console.WriteLine($"?        Battle {i + 1,-2} / {numTests}                  ?");
            Console.WriteLine($"??????????????????????????????????????????\n");
            
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
                Debug = false, // Disable debug for cleaner output
                Sync = true,
                Seed = new PrngSeed(99000 + i * 1000), // Independent battle seed
                MaxTurns = 1000,
            };

            var simulator = new SyncSimulator();
            SimulatorResult result = simulator.Run(library, battleOptions, printDebug: false);
            
            // Log battle setup after battle is created
            if (simulator.Battle != null)
            {
                LogBattleSetup(simulator.Battle);
            }
            
            switch (result)
            {
                case SimulatorResult.Player1Win:
                    player1Wins++;
                    Console.WriteLine("\n??? Winner: Player 1 ???");
                    break;
                case SimulatorResult.Player2Win:
                    player2Wins++;
                    Console.WriteLine("\n??? Winner: Player 2 ???");
                    break;
                case SimulatorResult.Tie:
                    ties++;
                    Console.WriteLine("\n??? Result: Tie ???");
                    break;
            }
        }
        
        Console.WriteLine("\n\n??????????????????????????????????????????");
        Console.WriteLine("?          FINAL RESULTS                 ?");
        Console.WriteLine("??????????????????????????????????????????");
        Console.WriteLine($"Player 1 Wins: {player1Wins} ({(double)player1Wins / numTests:P1})");
        Console.WriteLine($"Player 2 Wins: {player2Wins} ({(double)player2Wins / numTests:P1})");
        Console.WriteLine($"Ties:          {ties} ({(double)ties / numTests:P1})");
    }
    
    private static void LogBattleSetup(Battle battle)
    {
        Console.WriteLine("??? Battle Setup ???");
        Console.WriteLine($"Battle PRNG Seed: {battle.PrngSeed.Seed}");
        Console.WriteLine($"\nPlayer 1 Team:");
        foreach (var pokemon in battle.P1.Pokemon)
        {
            Console.WriteLine($"  - {pokemon.Name} (HP: {pokemon.MaxHp}, Atk: {pokemon.BaseStoredStats.Atk}, Def: {pokemon.BaseStoredStats.Def}, SpA: {pokemon.BaseStoredStats.SpA}, SpD: {pokemon.BaseStoredStats.SpD}, Spe: {pokemon.BaseStoredStats.Spe})");
        }
        
        Console.WriteLine($"\nPlayer 2 Team:");
        foreach (var pokemon in battle.P2.Pokemon)
        {
            Console.WriteLine($"  - {pokemon.Name} (HP: {pokemon.MaxHp}, Atk: {pokemon.BaseStoredStats.Atk}, Def: {pokemon.BaseStoredStats.Def}, SpA: {pokemon.BaseStoredStats.SpA}, SpD: {pokemon.BaseStoredStats.SpD}, Spe: {pokemon.BaseStoredStats.Spe})");
        }
        Console.WriteLine();
    }
}
