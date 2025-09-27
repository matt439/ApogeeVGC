using ApogeeVGC.Player;
using ApogeeVGC.Sim.Turns;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    /// <summary>
    /// Main turn processing dispatcher
    /// </summary>
    public async Task ProcessTurnAsync(CancellationToken cancellationToken = default)
    {
        ValidateTurnState();

        try
        {
            LogBattleState("Before Turn Processing");

            await ProcessCurrentTurnAsync(cancellationToken);

            LogBattleState("After Turn Processing");
        }
        catch (Exception ex)
        {
            if (PrintDebug)
                Console.WriteLine($"Turn processing error: {ex.Message}");

            // Try to recover from the error
            bool recovered = await TryRecoverFromTurnErrorAsync(ex);
            if (!recovered)
            {
                // If recovery failed, rethrow the exception
                throw;
            }
        }
    }

    /// <summary>
    /// Get turn history for replay or analysis
    /// </summary>
    public IReadOnlyList<Turn> GetTurnHistory()
    {
        return Turns.AsReadOnly();
    }

    /// <summary>
    /// Get specific turn by index
    /// </summary>
    public Turn GetTurn(int turnIndex)
    {
        if (turnIndex < 0 || turnIndex >= Turns.Count)
            throw new ArgumentOutOfRangeException(nameof(turnIndex));

        return Turns[turnIndex];
    }

    /// <summary>
    /// Get the last completed turn (excludes current in-progress turn)
    /// </summary>
    public Turn? GetLastCompletedTurn()
    {
        // Find the last turn that has end states set
        for (int i = Turns.Count - 1; i >= 0; i--)
        {
            var turn = Turns[i];
            if (turn.Side1End != null && turn.Side2End != null && turn.FieldEnd != null)
                return turn;
        }

        return null;
    }

    /// <summary>
    /// Check if current turn is complete
    /// </summary>
    public bool IsCurrentTurnComplete()
    {
        if (Turns.Count == 0)
            return false;

        var currentTurn = CurrentTurn;
        return currentTurn.Side1End != null &&
               currentTurn.Side2End != null &&
               currentTurn.FieldEnd != null &&
               currentTurn.TurnEndTime != null;
    }

    /// <summary>
    /// Force complete the current turn (for error recovery)
    /// </summary>
    public void ForceCompleteTurn()
    {
        if (IsCurrentTurnComplete())
            return;

        if (PrintDebug)
            Console.WriteLine("Force completing current turn");

        CompleteTurnWithEndStates();
    }

    /// <summary>
    /// Get battle statistics
    /// </summary>
    public BattleStatistics GetBattleStatistics()
    {
        return new BattleStatistics
        {
            TotalTurns = TurnCounter,
            GameDuration = DateTime.UtcNow - _gameStartTime,
            Player1TotalTime = _player1TotalTime,
            Player2TotalTime = _player2TotalTime,
            IsComplete = IsGameComplete,
            TurnsCompleted = Turns.Count(t => t.TurnEndTime != null),
            GameStartTime = _gameStartTime
        };
    }

    /// <summary>
    /// Calculate turn duration
    /// </summary>
    public TimeSpan? GetTurnDuration(int turnIndex)
    {
        var turn = GetTurn(turnIndex);

        if (turn.TurnEndTime == null)
            return null;

        return turn.TurnEndTime.Value - turn.TurnStartTime;
    }

    /// <summary>
    /// Get average turn duration
    /// </summary>
    public TimeSpan GetAverageTurnDuration()
    {
        var completedTurns = Turns.Where(t => t.TurnEndTime != null).ToList();

        if (completedTurns.Count == 0)
            return TimeSpan.Zero;

        var totalDuration = completedTurns
            .Sum(t => (t.TurnEndTime!.Value - t.TurnStartTime).Ticks);

        return new TimeSpan(totalDuration / completedTurns.Count);
    }

    /// <summary>
    /// Check if battle is currently waiting for player input
    /// </summary>
    public bool IsWaitingForInput()
    {
        // Battle is waiting if it's not complete and current turn is not complete
        return !IsGameComplete && !IsCurrentTurnComplete();
    }

    /// <summary>
    /// Get which players are currently expected to provide input
    /// </summary>
    public List<PlayerId> GetPlayersWaitingForInput()
    {
        var waitingPlayers = new List<PlayerId>();

        if (!IsWaitingForInput())
            return waitingPlayers;

        // This would depend on your turn structure and what actions are pending
        // For now, return both players as potentially waiting
        switch (CurrentTurn)
        {
            case TeamPreviewTurn:
                // Both players need to submit team preview
                waitingPlayers.Add(PlayerId.Player1);
                waitingPlayers.Add(PlayerId.Player2);
                break;

            case GameplayTurn:
                // Would need to check which specific actions are pending
                // For now, assume both players might need to submit
                waitingPlayers.Add(PlayerId.Player1);
                waitingPlayers.Add(PlayerId.Player2);
                break;
        }

        return waitingPlayers;
    }

    /// <summary>
    /// Pause the battle (for external control)
    /// </summary>
    public void PauseBattle()
    {
        if (PrintDebug)
            Console.WriteLine("Battle paused");

        // Pause timers by not updating them
        // This would require more sophisticated timer management
    }

    /// <summary>
    /// Resume the battle (for external control)
    /// </summary>
    public void ResumeBattle()
    {
        if (PrintDebug)
            Console.WriteLine("Battle resumed");

        // Resume timers
        // This would require more sophisticated timer management
    }
}

/// <summary>
/// Battle statistics for monitoring and analysis
/// </summary>
public class BattleStatistics
{
    public required int TotalTurns { get; init; }
    public required TimeSpan GameDuration { get; init; }
    public required TimeSpan Player1TotalTime { get; init; }
    public required TimeSpan Player2TotalTime { get; init; }
    public required bool IsComplete { get; init; }
    public required int TurnsCompleted { get; init; }
    public required DateTime GameStartTime { get; init; }

    public double TurnsPerMinute => GameDuration.TotalMinutes > 0 ? TotalTurns / GameDuration.TotalMinutes : 0;
    public TimeSpan AveragePlayerTime => TimeSpan.FromTicks((Player1TotalTime.Ticks + Player2TotalTime.Ticks) / 2);
}