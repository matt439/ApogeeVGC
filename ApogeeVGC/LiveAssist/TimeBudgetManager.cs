using System.Text.RegularExpressions;

namespace ApogeeVGC.LiveAssist;

/// <summary>
/// Manages time budget for MCTS search based on Showdown timer state.
/// Allocates more time to early turns (more strategic decisions) and
/// conserves time for later turns.
///
/// Showdown VGC timer:
///   - Per-turn timer: ~55 seconds
///   - Total timer: ~420 seconds (7 minutes per player)
///   - Grace period: 90 seconds (first turn only)
/// </summary>
public sealed class TimeBudgetManager
{
    private int _turnTimerSec = 55;
    private int _totalTimerSec = 420;
    private int _currentTurn;
    private const int SafetyMarginSec = 3;
    private const int MinBudgetSec = 3;
    private const int MaxBudgetSec = 30;
    private const int EstimatedTotalTurns = 12;

    /// <summary>
    /// Update timer state from a Showdown |inactive| message.
    /// Format: "Time left: 55 sec this turn | 420 sec total"
    /// </summary>
    public void UpdateFromTimerMessage(string timerMessage)
    {
        // Parse turn timer: "Time left: XX sec this turn"
        Match turnMatch = Regex.Match(timerMessage, @"Time left:\s*(\d+)\s*sec this turn");
        if (turnMatch.Success)
            _turnTimerSec = int.Parse(turnMatch.Groups[1].Value);

        // Parse total timer: "XXX sec total"
        Match totalMatch = Regex.Match(timerMessage, @"(\d+)\s*sec total");
        if (totalMatch.Success)
            _totalTimerSec = int.Parse(totalMatch.Groups[1].Value);
    }

    /// <summary>
    /// Notify the manager that a new turn has started.
    /// </summary>
    public void OnTurnStart(int turnNumber)
    {
        _currentTurn = turnNumber;
    }

    /// <summary>
    /// Calculate the time budget in seconds for the current MCTS search.
    ///
    /// Strategy:
    ///   - Early turns (1-3): spend more time — team preview just happened,
    ///     positioning decisions matter most
    ///   - Mid turns (4-8): moderate time
    ///   - Late turns (9+): conserve — game state is simpler, less branching
    ///
    /// Budget = min(turnTimer, totalShare) - safetyMargin
    /// where totalShare = totalRemaining / estimatedTurnsRemaining
    /// </summary>
    public int GetBudgetSeconds()
    {
        int turnsRemaining = Math.Max(EstimatedTotalTurns - _currentTurn, 2);

        // Share of total timer for this turn
        int totalShare = _totalTimerSec / turnsRemaining;

        // Early turn bonus: spend more on turns 1-3
        float turnMultiplier = _currentTurn switch
        {
            <= 1 => 1.5f,
            2 => 1.3f,
            3 => 1.2f,
            >= 10 => 0.7f,
            _ => 1.0f,
        };

        int budget = (int)(totalShare * turnMultiplier);

        // Clamp to per-turn timer (can't exceed what Showdown allows)
        budget = Math.Min(budget, _turnTimerSec - SafetyMarginSec);

        // Clamp to absolute bounds
        budget = Math.Clamp(budget, MinBudgetSec, MaxBudgetSec);

        return budget;
    }

    /// <summary>
    /// Get a diagnostic string showing the current timer state and budget.
    /// </summary>
    public string GetDiagnostics()
    {
        int budget = GetBudgetSeconds();
        return $"Turn {_currentTurn} | Budget: {budget}s | Turn timer: {_turnTimerSec}s | Total: {_totalTimerSec}s";
    }
}
