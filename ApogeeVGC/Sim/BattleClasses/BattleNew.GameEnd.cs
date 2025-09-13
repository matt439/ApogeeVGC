using ApogeeVGC.Player;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Turns;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    /// <summary>
    /// Check for game end conditions
    /// </summary>
    private bool CheckForGameEndConditions()
    {
        return HasNoViablePokemon(Side1) || HasNoViablePokemon(Side2);
    }

    private static bool HasNoViablePokemon(Side side)
    {
        return side.IsDefeated || side is { AliveActivePokemonCount: 0, SwitchOptionsCount: 0 };
    }

    /// <summary>
    /// Handle normal game end (all Pokemon fainted on one side)
    /// </summary>
    private async Task HandleNormalGameEndAsync()
    {
        PlayerId winner;

        if (Side1.IsDefeated)
        {
            winner = PlayerId.Player2;
        }
        else if (Side2.IsDefeated)
        {
            winner = PlayerId.Player1;
        }
        else
        {
            // Should not happen if CheckForGameEndConditions returned true
            throw new InvalidOperationException("Normal game end called but no clear winner");
        }

        if (PrintDebug)
            Console.WriteLine($"Normal game end: {winner} wins");

        await EndGameAsync(winner, GameEndReason.Normal);
    }

    /// <summary>
    /// End the game with specified winner and reason
    /// </summary>
    private async Task EndGameAsync(PlayerId winner, GameEndReason reason)
    {
        if (PrintDebug)
            Console.WriteLine($"Ending game: {winner} wins by {reason}");

        // Cancel any active choice requests
        await CleanupActiveChoicesAsync();

        // Create post-game turn
        var postGameTurn = new PostGameTurn
        {
            Winner = winner,
            Side1Start = CurrentTurn.Side1End ?? Side1.Copy(),
            Side2Start = CurrentTurn.Side2End ?? Side2.Copy(),
            FieldStart = CurrentTurn.FieldEnd ?? Field.Copy(),
            Side1End = Side1.Copy(),
            Side2End = Side2.Copy(),
            FieldEnd = Field.Copy(),
            TurnCounter = TurnCounter + 1,
            TurnEndTime = DateTime.UtcNow,
        };

        Turns.Add(postGameTurn);

        // Fire game ended event
        var eventArgs = new GameEndEventArgs
        {
            Winner = winner,
            Reason = reason
        };

        GameEnded?.Invoke(this, eventArgs);

        if (PrintDebug)
        {
            Console.WriteLine($"Game completed after {TurnCounter} turns");
            Console.WriteLine($"Final game time: {DateTime.UtcNow - _gameStartTime:mm\\:ss}");
            Console.WriteLine($"Player 1 total time: {_player1TotalTime:mm\\:ss}");
            Console.WriteLine($"Player 2 total time: {_player2TotalTime:mm\\:ss}");
        }
    }

    /// <summary>
    /// Determine winner when forfeit occurs
    /// </summary>
    public async Task HandlePlayerForfeitAsync(PlayerId forfeitingPlayer)
    {
        var winner = forfeitingPlayer == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
        
        if (PrintDebug)
            Console.WriteLine($"Player {forfeitingPlayer} forfeited, {winner} wins");

        await EndGameAsync(winner, GameEndReason.Forfeit);
    }

    /// <summary>
    /// Get game summary for external systems
    /// </summary>
    public GameSummary GetGameSummary()
    {
        if (!IsGameComplete)
            throw new InvalidOperationException("Game is not complete");

        var postGameTurn = (PostGameTurn)CurrentTurn;
        
        return new GameSummary
        {
            Winner = postGameTurn.Winner,
            Reason = GetGameEndReason(),
            TotalTurns = TurnCounter,
            GameDuration = DateTime.UtcNow - _gameStartTime,
            Player1TotalTime = _player1TotalTime,
            Player2TotalTime = _player2TotalTime,
            FinalSide1State = postGameTurn.Side1End!,
            FinalSide2State = postGameTurn.Side2End!,
            FinalFieldState = postGameTurn.FieldEnd!
        };
    }

    /// <summary>
    /// Get the reason the game ended
    /// </summary>
    private GameEndReason GetGameEndReason()
    {
        if (!IsGameComplete)
            return GameEndReason.Normal; // Game not ended yet

        // Look for the most recent GameEnded event to get the reason
        // For now, determine from battle state
        if (HasGameTimedOut())
            return GameEndReason.GameTimeout;
        
        if (HasPlayerTimedOut(PlayerId.Player1) || HasPlayerTimedOut(PlayerId.Player2))
            return GameEndReason.PlayerTimeout;
            
        if (TurnCounter >= TurnLimit)
            return GameEndReason.TurnLimit;

        return GameEndReason.Normal;
    }

    /// <summary>
    /// Cancel any active choice requests and cleanup
    /// </summary>
    private async Task CleanupActiveChoicesAsync()
    {
        if (PrintDebug)
            Console.WriteLine("Cleaning up active choices...");

        // Cancel player-specific tokens
        Player1CancellationTokenSource.Cancel();
        Player2CancellationTokenSource.Cancel();

        // Notify players that the game has ended
        try
        {
            await Player1.NotifyChoiceTimeoutAsync();
            await Player2.NotifyChoiceTimeoutAsync();
        }
        catch (Exception ex)
        {
            if (PrintDebug)
                Console.WriteLine($"Error notifying players of game end: {ex.Message}");
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Check if the battle is in a valid state
    /// </summary>
    public bool IsValidBattleState()
    {
        try
        {
            // Check that both sides have valid Pokemon
            if (Side1.AllSlots.Count() == 0 || Side2.AllSlots.Count() == 0)
                return false;

            // Check that field is valid
            if (Field == null)
                return false;

            // Check that turns list is not empty
            if (Turns.Count == 0)
                return false;

            // Check that current turn is valid
            if (CurrentTurn == null)
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Summary of a completed game
/// </summary>
public class GameSummary
{
    public required PlayerId Winner { get; init; }
    public required GameEndReason Reason { get; init; }
    public required int TotalTurns { get; init; }
    public required TimeSpan GameDuration { get; init; }
    public required TimeSpan Player1TotalTime { get; init; }
    public required TimeSpan Player2TotalTime { get; init; }
    public required Side FinalSide1State { get; init; }
    public required Side FinalSide2State { get; init; }
    public required Field FinalFieldState { get; init; }
}