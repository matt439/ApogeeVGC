using ApogeeVGC.Player;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Turns;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    /// <summary>
    /// Handle post-game operations
    /// </summary>
    public async Task ProcessPostGameTurnAsync(PostGameTurn turn, CancellationToken cancellationToken)
    {
        if (PrintDebug)
            Console.WriteLine($"Processing post-game turn - Winner: {turn.Winner}");

        // Post-game turn doesn't require player input, just finalization
        await FinalizeGameAsync(turn);
    }

    /// <summary>
    /// Finalize the game after it has ended
    /// </summary>
    private async Task FinalizeGameAsync(PostGameTurn postGameTurn)
    {
        if (PrintDebug)
        {
            Console.WriteLine("=== GAME COMPLETED ===");
            Console.WriteLine($"Winner: {postGameTurn.Winner}");
            Console.WriteLine($"Total Turns: {TurnCounter}");
            Console.WriteLine($"Game Duration: {DateTime.UtcNow - _gameStartTime:mm\\:ss}");
            Console.WriteLine($"Player 1 Total Time: {_player1TotalTime:mm\\:ss}");
            Console.WriteLine($"Player 2 Total Time: {_player2TotalTime:mm\\:ss}");
            
            // Show final Pokemon status
            Console.WriteLine("\nFinal Pokemon Status:");
            ShowFinalPokemonStatus(PlayerId.Player1, Side1);
            ShowFinalPokemonStatus(PlayerId.Player2, Side2);
            
            Console.WriteLine("=====================");
        }

        // Perform any final cleanup or notifications
        await NotifyPlayersOfGameEndAsync(postGameTurn);
        
        // Update any persistent game records
        await UpdateGameRecordsAsync(postGameTurn);
    }

    /// <summary>
    /// Show final Pokemon status for debugging
    /// </summary>
    private void ShowFinalPokemonStatus(PlayerId playerId, Side side)
    {
        Console.WriteLine($"{playerId} Pokemon:");
        foreach (var pokemon in side.AllSlots)
        {
            var status = pokemon.IsFainted ? "Fainted" : $"{pokemon.CurrentHp}/{pokemon.UnmodifiedHp} HP";
            Console.WriteLine($"  {pokemon.Specie.Name}: {status}");
        }
    }

    /// <summary>
    /// Notify players that the game has ended
    /// </summary>
    private async Task NotifyPlayersOfGameEndAsync(PostGameTurn postGameTurn)
    {
        try
        {
            // Create game end notification
            var gameEndInfo = new GameEndNotification
            {
                Winner = postGameTurn.Winner,
                GameDuration = DateTime.UtcNow - _gameStartTime,
                TotalTurns = TurnCounter,
                FinalScore = CalculateFinalScore()
            };

            // Notify both players
            await NotifyPlayerOfGameEndAsync(Player1, gameEndInfo);
            await NotifyPlayerOfGameEndAsync(Player2, gameEndInfo);
        }
        catch (Exception ex)
        {
            if (PrintDebug)
                Console.WriteLine($"Error notifying players of game end: {ex.Message}");
        }
    }

    /// <summary>
    /// Notify a single player of game end
    /// </summary>
    private async Task NotifyPlayerOfGameEndAsync(IPlayerNew player, GameEndNotification notification)
    {
        try
        {
            // For now, just log - in a real implementation you might have a notification method
            if (PrintDebug)
                Console.WriteLine($"Notifying {player.PlayerId} of game end");
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            if (PrintDebug)
                Console.WriteLine($"Error notifying {player.PlayerId}: {ex.Message}");
        }
    }

    /// <summary>
    /// Calculate final score for the game
    /// </summary>
    private GameScore CalculateFinalScore()
    {
        var side1Remaining = Side1.AllSlots.Count(p => !p.IsFainted);
        var side2Remaining = Side2.AllSlots.Count(p => !p.IsFainted);
        
        var side1HpPercentage = CalculateHpPercentage(Side1);
        var side2HpPercentage = CalculateHpPercentage(Side2);

        return new GameScore
        {
            Player1PokemonRemaining = side1Remaining,
            Player2PokemonRemaining = side2Remaining,
            Player1HpPercentage = side1HpPercentage,
            Player2HpPercentage = side2HpPercentage,
            Player1TotalTime = _player1TotalTime,
            Player2TotalTime = _player2TotalTime
        };
    }

    /// <summary>
    /// Calculate HP percentage for a side
    /// </summary>
    private double CalculateHpPercentage(Side side)
    {
        int totalMaxHp = side.AllSlots.Sum(p => p.UnmodifiedAtk);
        int totalCurrentHp = side.AllSlots.Sum(p => p.CurrentHp);
        
        return totalMaxHp > 0 ? (double)totalCurrentHp / totalMaxHp * 100.0 : 0.0;
    }

    /// <summary>
    /// Update persistent game records
    /// </summary>
    private async Task UpdateGameRecordsAsync(PostGameTurn postGameTurn)
    {
        try
        {
            // In a real implementation, this might:
            // - Update player rankings/ELO
            // - Save battle replay data
            // - Update statistics
            // - Log to database
            
            if (PrintDebug)
                Console.WriteLine("Updating game records...");
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            if (PrintDebug)
                Console.WriteLine($"Error updating game records: {ex.Message}");
        }
    }

    /// <summary>
    /// Get detailed game results
    /// </summary>
    public GameResults GetGameResults()
    {
        if (!IsGameComplete)
            throw new InvalidOperationException("Game is not complete");

        var postGameTurn = (PostGameTurn)CurrentTurn;
        var summary = GetGameSummary();
        var score = CalculateFinalScore();
        
        return new GameResults
        {
            Winner = postGameTurn.Winner,
            Loser = postGameTurn.Winner == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1,
            GameSummary = summary,
            FinalScore = score,
            TurnHistory = GetTurnHistory(),
            BattleStatistics = GetBattleStatistics()
        };
    }

    /// <summary>
    /// Generate battle replay data
    /// </summary>
    public BattleReplayData GenerateReplayData()
    {
        return new BattleReplayData
        {
            BattleId = Guid.NewGuid(), // In real system, this would be assigned earlier
            Seed = BattleSeed,
            Format = Format,
            Players = new Dictionary<PlayerId, string>
            {
                [PlayerId.Player1] = Player1.PlayerId.ToString(),
                [PlayerId.Player2] = Player2.PlayerId.ToString()
            },
            TurnHistory = GetTurnHistory(),
            GameResults = IsGameComplete ? GetGameResults() : null,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Check if battle can be replayed
    /// </summary>
    public bool CanBeReplayed()
    {
        return IsGameComplete && 
               BattleSeed.HasValue && 
               Turns.Count > 0;
    }
}

/// <summary>
/// Game end notification for players
/// </summary>
public class GameEndNotification
{
    public required PlayerId Winner { get; init; }
    public required TimeSpan GameDuration { get; init; }
    public required int TotalTurns { get; init; }
    public required GameScore FinalScore { get; init; }
}

/// <summary>
/// Final game score
/// </summary>
public class GameScore
{
    public required int Player1PokemonRemaining { get; init; }
    public required int Player2PokemonRemaining { get; init; }
    public required double Player1HpPercentage { get; init; }
    public required double Player2HpPercentage { get; init; }
    public required TimeSpan Player1TotalTime { get; init; }
    public required TimeSpan Player2TotalTime { get; init; }
}

/// <summary>
/// Complete game results
/// </summary>
public class GameResults
{
    public required PlayerId Winner { get; init; }
    public required PlayerId Loser { get; init; }
    public required GameSummary GameSummary { get; init; }
    public required GameScore FinalScore { get; init; }
    public required IReadOnlyList<Turn> TurnHistory { get; init; }
    public required BattleStatistics BattleStatistics { get; init; }
}

/// <summary>
/// Battle replay data for saving/loading battles
/// </summary>
public class BattleReplayData
{
    public required Guid BattleId { get; init; }
    public required int? Seed { get; init; }
    public required BattleFormat Format { get; init; }
    public required Dictionary<PlayerId, string> Players { get; init; }
    public required IReadOnlyList<Turn> TurnHistory { get; init; }
    public required GameResults? GameResults { get; init; }
    public required DateTime CreatedAt { get; init; }
}