using ApogeeVGC.Player;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    /// <summary>
    /// Check if game time limit has been exceeded
    /// </summary>
    private bool HasGameTimedOut()
    {
        return DateTime.UtcNow - _gameStartTime > GameTimeLimit;
    }

    /// <summary>
    /// Check if a player has exceeded their total time limit
    /// </summary>
    private bool HasPlayerTimedOut(PlayerId playerId)
    {
        var playerTime = GetPlayerTotalTime(playerId);
        return playerTime > PlayerTotalTimeLimit;
    }

    /// <summary>
    /// Handle game timeout - call tiebreak function
    /// </summary>
    private async Task HandleGameTimeoutAsync()
    {
        if (PrintDebug)
            Console.WriteLine("Game time limit exceeded, calling tiebreak");

        var winner = PerformTiebreak();
        await EndGameAsync(winner, GameEndReason.GameTimeout);
    }

    /// <summary>
    /// Handle player total time timeout - player loses immediately
    /// </summary>
    private async Task HandlePlayerTimeoutAsync(PlayerId playerId)
    {
        if (PrintDebug)
            Console.WriteLine($"Player {playerId} exceeded total time limit, loses the game");

        var winner = playerId == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
        await EndGameAsync(winner, GameEndReason.PlayerTimeout);
    }

    /// <summary>
    /// Handle turn limit reached
    /// </summary>
    private async Task HandleTurnLimitReachedAsync()
    {
        if (PrintDebug)
            Console.WriteLine($"Turn limit ({TurnLimit}) reached, calling tiebreak");

        var winner = PerformTiebreak();
        await EndGameAsync(winner, GameEndReason.TurnLimit);
    }

    /// <summary>
    /// Perform tiebreak logic when game times out or turn limit is reached
    /// </summary>
    private PlayerId PerformTiebreak()
    {
        if (PrintDebug)
            Console.WriteLine("Performing tiebreak calculation...");

        // Tiebreak logic based on Pokemon Showdown rules:
        // 1. Player with more Pokemon remaining wins
        // 2. If tied, player with higher total HP percentage wins
        // 3. If still tied, player with higher total HP wins
        // 4. If still tied, it's a true tie (pick randomly or declare draw)

        var side1Stats = CalculateSideStats(Side1);
        var side2Stats = CalculateSideStats(Side2);

        if (PrintDebug)
        {
            Console.WriteLine($"Player 1: {side1Stats.PokemonRemaining} Pokemon, {side1Stats.TotalHpPercentage:F1}% HP, {side1Stats.TotalHp} total HP");
            Console.WriteLine($"Player 2: {side2Stats.PokemonRemaining} Pokemon, {side2Stats.TotalHpPercentage:F1}% HP, {side2Stats.TotalHp} total HP");
        }

        // Compare Pokemon remaining
        if (side1Stats.PokemonRemaining > side2Stats.PokemonRemaining)
        {
            if (PrintDebug)
                Console.WriteLine("Player 1 wins tiebreak: more Pokemon remaining");
            return PlayerId.Player1;
        }
        if (side2Stats.PokemonRemaining > side1Stats.PokemonRemaining)
        {
            if (PrintDebug)
                Console.WriteLine("Player 2 wins tiebreak: more Pokemon remaining");
            return PlayerId.Player2;
        }

        // Compare HP percentage
        if (side1Stats.TotalHpPercentage > side2Stats.TotalHpPercentage)
        {
            if (PrintDebug)
                Console.WriteLine("Player 1 wins tiebreak: higher HP percentage");
            return PlayerId.Player1;
        }
        if (side2Stats.TotalHpPercentage > side1Stats.TotalHpPercentage)
        {
            if (PrintDebug)
                Console.WriteLine("Player 2 wins tiebreak: higher HP percentage");
            return PlayerId.Player2;
        }

        // Compare total HP
        if (side1Stats.TotalHp > side2Stats.TotalHp)
        {
            if (PrintDebug)
                Console.WriteLine("Player 1 wins tiebreak: higher total HP");
            return PlayerId.Player1;
        }
        if (side2Stats.TotalHp > side1Stats.TotalHp)
        {
            if (PrintDebug)
                Console.WriteLine("Player 2 wins tiebreak: higher total HP");
            return PlayerId.Player2;
        }

        // True tie - use random determination
        var randomWinner = BattleRandom.Next(2) == 0 ? PlayerId.Player1 : PlayerId.Player2;
        
        if (PrintDebug)
            Console.WriteLine($"True tie in tiebreak, random winner: {randomWinner}");

        return randomWinner;
    }

    /// <summary>
    /// Calculate statistics for a side for tiebreak purposes
    /// </summary>
    private SideStats CalculateSideStats(Side side)
    {
        var stats = new SideStats();
        var totalMaxHp = 0;

        foreach (var pokemon in side.AllSlots)
        {
            totalMaxHp += pokemon.UnmodifiedHp;

            if (!pokemon.IsFainted)
            {
                stats.PokemonRemaining++;
                stats.TotalHp += pokemon.CurrentHp;
            }
        }

        stats.TotalHpPercentage = totalMaxHp > 0 ? (double)stats.TotalHp / totalMaxHp * 100.0 : 0.0;

        return stats;
    }

    /// <summary>
    /// Check for timeout warnings and send notifications
    /// </summary>
    private async Task CheckAndSendTimeoutWarningsAsync()
    {
        // Check game time warning (at 2 minutes remaining)
        var gameTimeRemaining = GameTimeLimit - (DateTime.UtcNow - _gameStartTime);
        if (gameTimeRemaining <= TimeSpan.FromMinutes(2) && gameTimeRemaining > TimeSpan.FromMinutes(1.9))
        {
            if (PrintDebug)
                Console.WriteLine($"Game time warning: {gameTimeRemaining.TotalMinutes:F1} minutes remaining");
        }

        // Check player time warnings (at 1 minute remaining)
        var player1TimeRemaining = PlayerTotalTimeLimit - _player1TotalTime;
        if (player1TimeRemaining <= TimeSpan.FromMinutes(1) && player1TimeRemaining > TimeSpan.FromSeconds(50))
        {
            await Player1.NotifyTimeoutWarningAsync(player1TimeRemaining);
        }

        var player2TimeRemaining = PlayerTotalTimeLimit - _player2TotalTime;
        if (player2TimeRemaining <= TimeSpan.FromMinutes(1) && player2TimeRemaining > TimeSpan.FromSeconds(50))
        {
            await Player2.NotifyTimeoutWarningAsync(player2TimeRemaining);
        }
    }

    /// <summary>
    /// Get remaining time for a player
    /// </summary>
    public TimeSpan GetPlayerRemainingTime(PlayerId playerId)
    {
        var usedTime = GetPlayerTotalTime(playerId);
        var remaining = PlayerTotalTimeLimit - usedTime;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Get remaining game time
    /// </summary>
    public TimeSpan GetGameRemainingTime()
    {
        var elapsed = DateTime.UtcNow - _gameStartTime;
        var remaining = GameTimeLimit - elapsed;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }
}

/// <summary>
/// Statistics for a side used in tiebreak calculations
/// </summary>
internal class SideStats
{
    public int PokemonRemaining { get; set; }
    public int TotalHp { get; set; }
    public double TotalHpPercentage { get; set; }
}