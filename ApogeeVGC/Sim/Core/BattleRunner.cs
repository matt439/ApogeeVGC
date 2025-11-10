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
    private readonly Simulator _simulator;
    private Task<SimulatorResult>? _battleTask;
    private CancellationTokenSource? _cancellationTokenSource;

    public Battle? Battle => _simulator.Battle;
    public bool IsRunning => _battleTask != null && !_battleTask.IsCompleted;
    public bool IsCompleted => _battleTask?.IsCompleted ?? false;
    public SimulatorResult? Result { get; private set; }

    public BattleRunner(Library library, BattleOptions battleOptions, Simulator simulator)
    {
        _library = library;
        _battleOptions = battleOptions;
        _simulator = simulator;
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

        // Run the battle on a background thread using Simulator.RunAsync()
        _battleTask = Task.Run(async () =>
        {
            try
            {
                // Use Simulator.RunAsync() which creates players and starts the battle
                SimulatorResult result = await _simulator.RunAsync(_library, _battleOptions, printDebug: true);
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
}
