using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Core;

/// <summary>
/// Synchronous battle simulator for AI training (MCTS).
/// No async/threading complexity - perfect for fast rollouts.
/// </summary>
public class SyncSimulator : IBattleController
{
    public Battle? Battle { get; set; }
    public IPlayer? Player1 { get; set; }
    public IPlayer? Player2 { get; set; }
    public bool PrintDebug { get; set; }

    public BattlePerspective GetCurrentPerspective(SideId sideId)
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        return Battle.GetPerspectiveForSide(sideId);
    }

    public void ClearMessages()
    {
        // No-op for sync simulator
    }

    public SimulatorResult Run(Library library, BattleOptions battleOptions, bool printDebug = true)
    {
        Battle = new Battle(battleOptions, library);
        Player1 = CreatePlayer(SideId.P1, battleOptions.Player1Options);
        Player2 = CreatePlayer(SideId.P2, battleOptions.Player2Options);
        PrintDebug = printDebug;

        // Subscribe to Battle events
        Battle.ChoiceRequested += OnChoiceRequested;
        Battle.UpdateRequested += OnUpdateRequested;
        Battle.BattleEnded += OnBattleEnded;

        if (PrintDebug)
        {
            Console.WriteLine("Starting synchronous battle simulation...");
        }

        try
        {
            // Start the battle - this is completely synchronous
            Battle.Start();

            if (PrintDebug)
            {
                Console.WriteLine("Battle has ended.");
            }

            return DetermineWinner();
        }
        catch (Exception ex)
        {
            if (PrintDebug)
            {
                Console.WriteLine($"Battle error: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine(
                        $"Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                }

                Console.WriteLine($"Stack trace (last 10 frames):");
                var frames = ex.StackTrace?.Split('\n').Take(10);
                if (frames != null)
                {
                    foreach (var frame in frames)
                    {
                        Console.WriteLine($"  {frame.Trim()}");
                    }
                }
            }

            throw;
        }
        finally
        {
            // Unsubscribe from events
            if (Battle != null)
            {
                Battle.ChoiceRequested -= OnChoiceRequested;
                Battle.UpdateRequested -= OnUpdateRequested;
                Battle.BattleEnded -= OnBattleEnded;
            }
        }
    }

    /// <summary>
    /// Event handler for when Battle requests a choice from a player.
    /// Synchronously gets the choice and submits it to the battle.
    /// </summary>
    private void OnChoiceRequested(object? sender, BattleChoiceRequestEventArgs e)
    {
        Console.WriteLine($"[SyncSimulator.OnChoiceRequested] Choice requested for {e.SideId}");

        IPlayer player = GetPlayer(e.SideId);
        Side side = Battle!.Sides.First(s => s.Id == e.SideId);

        // Get choice synchronously
        Choice choice = player.GetChoiceSync(e.Request, e.RequestType, e.Perspective);

        Console.WriteLine(
            $"[SyncSimulator.OnChoiceRequested] Received choice for {e.SideId}: {choice.Actions.Count} actions");

        // If the choice is empty, use AutoChoose
        if (choice.Actions.Count == 0)
        {
            Console.WriteLine(
                $"[SyncSimulator.OnChoiceRequested] Empty choice, using AutoChoose for {e.SideId}");
            side.AutoChoose();
            choice = side.GetChoice();
            Console.WriteLine(
                $"[SyncSimulator.OnChoiceRequested] After AutoChoose: {choice.Actions.Count} actions");
        }

        // Submit the choice immediately (synchronous)
        bool success = Battle.Choose(e.SideId, choice);

        if (!success)
        {
            Console.WriteLine($"[SyncSimulator.OnChoiceRequested] Invalid choice for {e.SideId}");
        }
    }

    /// <summary>
    /// Event handler for UI updates from the battle.
    /// </summary>
    private void OnUpdateRequested(object? sender, BattleUpdateEventArgs e)
    {
        IPlayer player = GetPlayer(e.SideId);
        player.UpdateUi(e.Perspective);
        player.UpdateMessages(e.Messages);
    }

    /// <summary>
    /// Event handler for battle end notification.
    /// </summary>
    private void OnBattleEnded(object? sender, BattleEndedEventArgs e)
    {
        Console.WriteLine(
            $"[SyncSimulator.OnBattleEnded] Battle ended, winner: {e.Winner ?? "tie"}");
    }

    private SimulatorResult DetermineWinner()
    {
        // Check if we have a winner
        if (!string.IsNullOrEmpty(Battle!.Winner))
        {
            // Winner is stored as side ID string ("p1" or "p2")
            bool isP1Winner = Battle.Winner.Equals("p1", StringComparison.OrdinalIgnoreCase);

            if (PrintDebug)
            {
                string winnerName = isP1Winner ? "Player 1" : "Player 2";
                Console.WriteLine($"Winner: {winnerName}");
            }

            return isP1Winner ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
        }

        // No clear winner - use tiebreak
        if (PrintDebug)
        {
            Console.WriteLine("Battle ended without a clear winner");
        }

        return DetermineTiebreakWinner();
    }

    private SimulatorResult DetermineTiebreakWinner()
    {
        if (PrintDebug)
        {
            Console.WriteLine("Executing tiebreaker...");
        }

        // Use Battle's tiebreak logic which checks:
        // 1. Remaining Pokemon count
        // 2. HP percentage
        // 3. Total HP
        Battle!.Tiebreak();

        // After tiebreak, check the winner
        return DetermineWinner();
    }

    private IPlayer CreatePlayer(SideId sideId, PlayerOptions options)
    {
        return options.Type switch
        {
            Player.PlayerType.Random => new PlayerRandom(sideId, options, this),
            Player.PlayerType.Console => throw new NotSupportedException(
                "Console player cannot be used in synchronous mode"),
            Player.PlayerType.Gui => throw new NotSupportedException(
                "GUI player cannot be used in synchronous mode"),
            Player.PlayerType.Mcts => throw new NotImplementedException(
                "MCTS player not implemented yet"),
            _ => throw new ArgumentOutOfRangeException($"Unknown player type: {options.Type}"),
        };
    }

    private IPlayer GetPlayer(SideId sideId)
    {
        return sideId switch
        {
            SideId.P1 => Player1 ??
                         throw new InvalidOperationException("Player 1 is not initialized"),
            SideId.P2 => Player2 ??
                         throw new InvalidOperationException("Player 2 is not initialized"),
            _ => throw new ArgumentOutOfRangeException(nameof(sideId), $"Invalid SideId: {sideId}"),
        };
    }

    // Force win/lose/tie methods for testing purposes

    /// <summary>
    /// Forces the specified side to win the battle.
    /// Useful for testing and debugging scenarios.
    /// </summary>
    public void ForceWin(SideId sideId)
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        Side side = Battle.Sides.First(s => s.Id == sideId);
        Battle.Win(side);
    }

    /// <summary>
    /// Forces a tie in the battle.
    /// Useful for testing and debugging scenarios.
    /// </summary>
    public void ForceTie()
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        Battle.Win((Side?)null);
    }

    /// <summary>
    /// Forces the specified side to lose the battle.
    /// Useful for testing and debugging scenarios.
    /// </summary>
    public void ForceLose(SideId sideId)
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        Side side = Battle.Sides.First(s => s.Id == sideId);
        Battle.Lose(side);
    }
}