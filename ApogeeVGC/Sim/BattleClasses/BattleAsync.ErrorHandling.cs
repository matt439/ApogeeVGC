using ApogeeVGC.Player;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.FieldClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    /// <summary>
    /// Handle battle cancellation
    /// </summary>
    private async Task HandleBattleCancellationAsync()
    {
        if (PrintDebug)
            Console.WriteLine("Battle was cancelled");

        await CleanupActiveChoicesAsync();
        
        // Don't create a PostGameTurn for cancellation - leave battle in current state
        await Task.CompletedTask;
    }

    /// <summary>
    /// Handle unexpected battle errors
    /// </summary>
    private async Task HandleBattleErrorAsync(Exception exception)
    {
        if (PrintDebug)
            Console.WriteLine($"Battle error occurred: {exception.Message}");

        try
        {
            await CleanupActiveChoicesAsync();
            
            // End game with error reason - pick a winner randomly or based on current state
            var winner = DetermineWinnerFromCurrentState();
            await EndGameAsync(winner, GameEndReason.Error);
        }
        catch (Exception cleanupEx)
        {
            if (PrintDebug)
                Console.WriteLine($"Error during error cleanup: {cleanupEx.Message}");
        }
    }

    /// <summary>
    /// Cleanup battle resources
    /// </summary>
    private async Task CleanupBattleResourcesAsync()
    {
        if (PrintDebug)
            Console.WriteLine("Cleaning up battle resources...");

        try
        {
            // Release semaphore if held
            if (_choiceSubmissionLock.CurrentCount == 0)
            {
                _choiceSubmissionLock.Release();
            }

            // Additional cleanup as needed
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            if (PrintDebug)
                Console.WriteLine($"Error during resource cleanup: {ex.Message}");
        }
    }

    /// <summary>
    /// Determine winner from current battle state (for error scenarios)
    /// </summary>
    private PlayerId DetermineWinnerFromCurrentState()
    {
        // Try to determine winner based on current Pokemon status
        var side1Remaining = Side1.AllSlots.Count(p => !p.IsFainted);
        var side2Remaining = Side2.AllSlots.Count(p => !p.IsFainted);

        if (side1Remaining > side2Remaining)
            return PlayerId.Player1;
        if (side2Remaining > side1Remaining)
            return PlayerId.Player2;

        // If tied, use tiebreak logic
        return PerformTiebreak();
    }

    /// <summary>
    /// Validate battle state and throw if invalid
    /// </summary>
    private void ValidateBattleState()
    {
        if (!IsValidBattleState())
        {
            throw new InvalidOperationException("Battle is in an invalid state");
        }
    }

    /// <summary>
    /// Handle unexpected choice submission errors
    /// </summary>
    private async Task HandleChoiceErrorAsync(PlayerId playerId, Exception exception)
    {
        if (PrintDebug)
            Console.WriteLine($"Choice error for {playerId}: {exception.Message}");

        try
        {
            var player = GetPlayer(playerId);
            await player.NotifyChoiceTimeoutAsync();
        }
        catch (Exception notifyEx)
        {
            if (PrintDebug)
                Console.WriteLine($"Error notifying player of choice error: {notifyEx.Message}");
        }
    }

    /// <summary>
    /// Validate turn state before processing
    /// </summary>
    private void ValidateTurnState()
    {
        if (IsGameComplete)
            throw new InvalidOperationException("Cannot process turn - game is complete");

        if (TurnCounter < 0)
            throw new InvalidOperationException("Invalid turn counter");

        if (CurrentTurn == null)
            throw new InvalidOperationException("No current turn");
    }

    /// <summary>
    /// Handle recoverable errors during turn processing
    /// </summary>
    private async Task<bool> TryRecoverFromTurnErrorAsync(Exception exception)
    {
        if (PrintDebug)
            Console.WriteLine($"Attempting to recover from turn error: {exception.Message}");

        try
        {
            // For certain types of errors, we might be able to continue
            // For example, if a single action failed, we could skip it and continue
            
            // For now, return false to indicate recovery failed
            await Task.CompletedTask;
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Log battle state for debugging
    /// </summary>
    private void LogBattleState(string context)
    {
        if (!PrintDebug)
            return;

        Console.WriteLine($"=== Battle State ({context}) ===");
        Console.WriteLine($"Turn: {TurnCounter}");
        Console.WriteLine($"Current Turn Type: {CurrentTurn?.GetType().Name}");
        Console.WriteLine($"Game Complete: {IsGameComplete}");
        Console.WriteLine($"Game Time: {DateTime.UtcNow - _gameStartTime:mm\\:ss}");
        Console.WriteLine($"Player 1 Time: {_player1TotalTime:mm\\:ss}");
        Console.WriteLine($"Player 2 Time: {_player2TotalTime:mm\\:ss}");
        
        // Log Pokemon status
        Console.WriteLine("Player 1 Pokemon:");
        foreach (var pokemon in Side1.AllSlots)
        {
            Console.WriteLine($"  {pokemon.Specie.Name}: {pokemon.CurrentHp}/{pokemon.UnmodifiedHp} HP {(pokemon.IsFainted ? "(Fainted)" : "")}");
        }
        
        Console.WriteLine("Player 2 Pokemon:");
        foreach (var pokemon in Side2.AllSlots)
        {
            Console.WriteLine($"  {pokemon.Specie.Name}: {pokemon.CurrentHp}/{pokemon.UnmodifiedHp} HP {(pokemon.IsFainted ? "(Fainted)" : "")}");
        }
        
        Console.WriteLine("===========================");
    }
}