using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Turns;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    public required Library Library { get; init; }
    public required Field Field { get; init; }
    public required Side Side1 { get; init; }
    public required Side Side2 { get; init; }
    public bool PrintDebug { get; set; }
    public int? BattleSeed { get; set; }

    public required BattleFormat Format { get; init; }

    // Constants
    private const int TurnLimit = 1000;
    private const double Epsilon = 1e-10;

    // Random number generation
    private Random? _battleRandom;
    private Random BattleRandom => _battleRandom ??= BattleSeed.HasValue ?
        new Random(BattleSeed.Value) : new Random();


    public List<Turn> Turns { get; private set; } = [];
    public Turn CurrentTurn => Turns.Last();
    public int TurnCounter { get; private set; } // Starts at 0 for team preview turn
    public bool IsGameComplete => CurrentTurn is PostGameTurn;


    // Player management
    public required IPlayerNew Player1 { get; init; }
    public required IPlayerNew Player2 { get; init; }
    public required CancellationTokenSource Player1CancellationTokenSource { get; init; }
    public required CancellationTokenSource Player2CancellationTokenSource { get; init; }

    private readonly SemaphoreSlim _choiceSubmissionLock = new(1, 1);

    // Game-level timers
    private DateTime _gameStartTime;
    private TimeSpan _player1TotalTime = TimeSpan.Zero;
    private TimeSpan _player2TotalTime = TimeSpan.Zero;
    private DateTime? _currentPlayerTurnStart;

    // Timer limits
    public static readonly TimeSpan PlayerTotalTimeLimit = TimeSpan.FromMinutes(7);
    public static readonly TimeSpan GameTimeLimit = TimeSpan.FromMinutes(20);

    // Events for external observers
    public event EventHandler<Turn>? TurnCompleted;
    public event EventHandler<GameEndEventArgs>? GameEnded;


    public void Start()
    {
        // Initialize game state
        Turns.Clear();
        TurnCounter = 0;
        Turns.Add(new TeamPreviewTurn
        {
            Side1Start = Side1.Copy(),
            Side2Start = Side2.Copy(),
            FieldStart = Field.Copy(),
            TurnCounter = TurnCounter,
        });
        _gameStartTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Main async battle loop - implements the core game flow
    /// </summary>
    public async Task RunBattleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (PrintDebug)
                Console.WriteLine("Starting battle...");

            while (!IsGameComplete && !cancellationToken.IsCancellationRequested)
            {
                // Check for game-level timeouts before processing turn
                if (HasGameTimedOut())
                {
                    await HandleGameTimeoutAsync();
                    break;
                }

                // Process the current turn
                await ProcessCurrentTurnAsync(cancellationToken);

                // Check for game end conditions after turn processing
                if (CheckForGameEndConditions())
                {
                    await HandleNormalGameEndAsync();
                    break;
                }

                // Check turn limit
                if (TurnCounter >= TurnLimit)
                {
                    await HandleTurnLimitReachedAsync();
                    break;
                }
            }

            if (PrintDebug)
                Console.WriteLine("Battle completed.");
        }
        catch (OperationCanceledException)
        {
            if (PrintDebug)
                Console.WriteLine("Battle was cancelled.");
            await HandleBattleCancellationAsync();
        }
        catch (Exception ex)
        {
            if (PrintDebug)
                Console.WriteLine($"Battle error: {ex.Message}");
            await HandleBattleErrorAsync(ex);
        }
        finally
        {
            await CleanupBattleResourcesAsync();
        }
    }

    /// <summary>
    /// Turn processing dispatch based on turn type
    /// </summary>
    private async Task ProcessCurrentTurnAsync(CancellationToken cancellationToken)
    {
        if (PrintDebug)
            Console.WriteLine($"Processing turn {TurnCounter} of type {CurrentTurn.GetType().Name}");

        switch (CurrentTurn)
        {
            case TeamPreviewTurn teamPreviewTurn:
                await ProcessTeamPreviewTurnAsync(teamPreviewTurn, cancellationToken);
                break;

            case GameplayTurn gameplayTurn:
                await ProcessGameplayTurnAsync(gameplayTurn, cancellationToken);
                break;

            case PostGameTurn:
                // Game is complete, no processing needed
                if (PrintDebug)
                    Console.WriteLine("Game completed - PostGameTurn reached");
                break;

            default:
                throw new InvalidOperationException($"Unknown turn type: {CurrentTurn.GetType().Name}");
        }
    }

    /// <summary>
    /// Get the current side for a player
    /// </summary>
    public Side GetSide(PlayerId playerId)
    {
        return playerId == PlayerId.Player1 ? Side1 : Side2;
    }

    /// <summary>
    /// Get the opponent's side for a player
    /// </summary>
    public Side GetOpponentSide(PlayerId playerId)
    {
        return playerId == PlayerId.Player1 ? Side2 : Side1;
    }

    /// <summary>
    /// Get the player for a given PlayerId
    /// </summary>
    public IPlayerNew GetPlayer(PlayerId playerId)
    {
        return playerId == PlayerId.Player1 ? Player1 : Player2;
    }

    /// <summary>
    /// Get cancellation token for a player
    /// </summary>
    public CancellationTokenSource GetPlayerCancellationTokenSource(PlayerId playerId)
    {
        return playerId == PlayerId.Player1 ? Player1CancellationTokenSource : Player2CancellationTokenSource;
    }

    /// <summary>
    /// Update player total time tracking
    /// </summary>
    private void UpdatePlayerTime(PlayerId playerId, TimeSpan duration)
    {
        if (playerId == PlayerId.Player1)
            _player1TotalTime += duration;
        else
            _player2TotalTime += duration;

        if (PrintDebug)
            Console.WriteLine($"Player {playerId} time updated: +{duration.TotalSeconds:F1}s (Total: {GetPlayerTotalTime(playerId).TotalSeconds:F1}s)");
    }

    /// <summary>
    /// Get total time used by a player
    /// </summary>
    public TimeSpan GetPlayerTotalTime(PlayerId playerId)
    {
        return playerId == PlayerId.Player1 ? _player1TotalTime : _player2TotalTime;
    }

    /// <summary>
    /// Create the next turn after completing the current one
    /// </summary>
    private async Task CreateNextTurnAsync()
    {
        TurnCounter++;

        var nextTurn = new GameplayTurn
        {
            Side1Start = Side1.Copy(),
            Side2Start = Side2.Copy(),
            FieldStart = Field.Copy(),
            TurnCounter = TurnCounter,
        };

        Turns.Add(nextTurn);

        if (PrintDebug)
            Console.WriteLine($"Created next turn: {TurnCounter}");

        await Task.CompletedTask;
    }

    /// <summary>
    /// Complete the current turn by setting end states
    /// </summary>
    private void CompleteTurnWithEndStates()
    {
        var currentTurn = Turns[^1];
        
        Turn completedTurn;
        switch (currentTurn)
        {
            case TeamPreviewTurn tp:
                completedTurn = tp with
                {
                    Side1End = Side1.Copy(),
                    Side2End = Side2.Copy(),
                    FieldEnd = Field.Copy(),
                    TurnEndTime = DateTime.UtcNow
                };
                break;
                
            case GameplayTurn gp:
                completedTurn = gp with
                {
                    Side1End = Side1.Copy(),
                    Side2End = Side2.Copy(),
                    FieldEnd = Field.Copy(),
                    TurnEndTime = DateTime.UtcNow
                };
                break;
                
            default:
                throw new InvalidOperationException($"Cannot complete turn of type: {currentTurn.GetType().Name}");
        }

        Turns[^1] = completedTurn;
        TurnCompleted?.Invoke(this, completedTurn);

        if (PrintDebug)
            Console.WriteLine($"Turn {TurnCounter} completed");
    }

    /// <summary>
    /// Dispose of battle resources
    /// </summary>
    public void Dispose()
    {
        _choiceSubmissionLock.Dispose();
        Player1CancellationTokenSource.Dispose();
        Player2CancellationTokenSource.Dispose();
    }
}

// Supporting classes
public class PendingAction
{
    public required PlayerId PlayerId { get; init; }
    public required int ActionIndex { get; init; }
    public required DateTime RequestTime { get; init; }
    public BattleChoice? Choice { get; init; }
}

public class ActionWithChoice
{
    public required PlayerId PlayerId { get; init; }
    public required BattleChoice Choice { get; init; }
    public required int SpeedPriority { get; init; }
    public required int ActionOrder { get; init; }
    public required Pokemon Executor { get; init; }
}

public class GameEndEventArgs : EventArgs
{
    public required PlayerId Winner { get; init; }
    public required GameEndReason Reason { get; init; }
}

public enum GameEndReason
{
    Normal,
    PlayerTimeout,
    GameTimeout,
    Forfeit,
    TurnLimit,
    Error
}

public class ChoiceRequestEventArgs : EventArgs
{
    public required BattleChoice[] AvailableChoices { get; init; }
    public required TimeSpan TimeLimit { get; init; }
    public required DateTime RequestTime { get; init; }
    public int ActionIndex { get; init; } = 0;
}