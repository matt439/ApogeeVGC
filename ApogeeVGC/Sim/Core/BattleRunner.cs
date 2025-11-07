using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Sim.Core;

/// <summary>
/// Runs a battle simulation on a background thread, allowing the GUI to remain responsive.
/// The battle sim runs async/await logic independently of the MonoGame update loop.
/// </summary>
public class BattleRunner
{
    private readonly Library _library;
    private readonly BattleOptions _battleOptions;
  private readonly IPlayerController _playerController;
    private Task<SimulatorResult>? _battleTask;
    private CancellationTokenSource? _cancellationTokenSource;

    public Battle? Battle { get; private set; }
    public bool IsRunning => _battleTask != null && !_battleTask.IsCompleted;
    public bool IsCompleted => _battleTask?.IsCompleted ?? false;
    public SimulatorResult? Result { get; private set; }

    public BattleRunner(Library library, BattleOptions battleOptions, IPlayerController playerController)
    {
     _library = library;
        _battleOptions = battleOptions;
  _playerController = playerController;
    }

    /// <summary>
    /// Starts the battle on a background thread.
  /// The battle will run independently and request choices asynchronously.
    /// </summary>
    public void StartBattle()
    {
    if (IsRunning)
        {
         throw new InvalidOperationException("Battle is already running");
        }

        _cancellationTokenSource = new CancellationTokenSource();
        Battle = new Battle(_battleOptions, _library, _playerController);

        // Run the battle on a background thread
    _battleTask = Task.Run(async () =>
        {
   try
     {
  // Start the battle asynchronously
        await Battle.StartAsync(_cancellationTokenSource.Token);

        // Determine the winner
     SimulatorResult result = DetermineWinner(Battle);
          Result = result;
      return result;
      }
         catch (OperationCanceledException)
 {
          Console.WriteLine("Battle was cancelled");
      Result = SimulatorResult.Player1Win; // Default for cancelled battles
        return SimulatorResult.Player1Win;
}
          catch (Exception ex)
            {
      Console.WriteLine($"Battle error: {ex.Message}");
 throw;
       }
        }, _cancellationTokenSource.Token);
    }

    /// <summary>
    /// Cancels the running battle.
  /// </summary>
    public void CancelBattle()
    {
        _cancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// Waits for the battle to complete (for testing/non-GUI scenarios).
    /// </summary>
    public async Task<SimulatorResult> WaitForCompletionAsync()
    {
        if (_battleTask == null)
        {
   throw new InvalidOperationException("Battle has not been started");
        }

        return await _battleTask;
    }

    private static SimulatorResult DetermineWinner(Battle battle)
    {
        // Check if we have a winner
if (!string.IsNullOrEmpty(battle.Winner))
        {
     bool isP1Winner = battle.Winner.Equals("p1", StringComparison.OrdinalIgnoreCase);
            return isP1Winner ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
     }

        // No clear winner - default to player 1
        return SimulatorResult.Player1Win;
    }
}
