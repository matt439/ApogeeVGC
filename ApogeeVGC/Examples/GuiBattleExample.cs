using ApogeeVGC.Data;
using ApogeeVGC.Gui;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Examples;

/// <summary>
/// Example of how to run a battle with GUI players.
/// Demonstrates the proper threading model where the battle sim runs on a background thread
/// and the GUI remains responsive on the main thread.
/// </summary>
public class GuiBattleExample
{
    public static void RunGuiBattle()
    {
        // Load the library
        var library = new Library();

        // Create the MonoGame window
        using var game = new BattleGame();
  
        // Create battle options
        var battleOptions = new BattleOptions
        {
            Player1Options = new PlayerOptions
            {
                Name = "Player 1",
                Type = PlayerType.Gui,
                Team = GenerateSampleTeam(),
                GuiWindow = game, // Pass the game window to the player
            },
            Player2Options = new PlayerOptions
            {
                Name = "Player 2",
                Type = PlayerType.Random, // Or PlayerType.Gui for 2-player
                Team = GenerateSampleTeam(),
            },
            // Format = library.Formats[...], // TODO: Set format
            Seed = null, // Random seed
        };

        // Create the player controller (the Simulator in your case)
        var simulator = new Simulator();
        
        // Start the battle on a background thread
        game.StartBattle(library, battleOptions, simulator);
  
        // Run the MonoGame loop (this blocks until the window closes)
        game.Run();
    }

    private static List<PokemonSet> GenerateSampleTeam()
    {
        // TODO: Generate or load a sample team
        return new List<PokemonSet>();
    }
}
