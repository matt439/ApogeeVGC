//using ApogeeVGC.Data;
//using ApogeeVGC.Player;
//using ApogeeVGC.Sim.Choices;
//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.FieldClasses;
//using ApogeeVGC.Sim.PokemonClasses;
//using ApogeeVGC.Sim.Turns;

//namespace ApogeeVGC.Sim.BattleClasses;

//public partial class BattleAsync : IBattle
//{
//    public required Library Library { get; init; }
//    public required Field Field { get; init; }
//    public required Side Side1 { get; init; }
//    public required Side Side2 { get; init; }
//    public bool PrintDebug { get; set; }
//    public int? BattleSeed { get; set; }

//    public required GameType Format { get; init; }

//    // Random number generation
//    private Random? _battleRandom;
//    private Random BattleRandom => _battleRandom ??= BattleSeed.HasValue ?
//        new Random(BattleSeed.Value) : new Random();


//    public List<Turn> Turns { get; private set; } = [];
//    public Turn CurrentTurn => Turns.Last();
//    public int TurnCounter { get; private set; } // Starts at 0 for team preview turn
//    public bool IsGameComplete => Turns.Count > 0 && CurrentTurn is PostGameTurn;
//    public BattleContext Context => new()
//    {
//        Library = Library,
//        Random = BattleRandom,
//        PrintDebug = PrintDebug,
//    };

//    public IEnumerable<Pokemon> AllActivePokemon
//    {
//        get
//        {
//            foreach (Pokemon pokemon in Side1.ActivePokemon)
//                yield return pokemon;
//            foreach (Pokemon pokemon in Side2.ActivePokemon)
//                yield return pokemon;
//        }
//    }

//    public Pokemon[] AllActivePokemonArray => AllActivePokemon.ToArray();

//    public IEnumerable<Pokemon> AllAliveActivePokemon
//    {
//        get
//        {
//            foreach (Pokemon pokemon in Side1.AliveActivePokemon)
//                yield return pokemon;
//            foreach (Pokemon pokemon in Side2.AliveActivePokemon)
//                yield return pokemon;
//        }
//    }


//    // Player management
//    public required IPlayerNew Player1 { get; init; }
//    public required IPlayerNew Player2 { get; init; }
//    public required CancellationTokenSource Player1CancellationTokenSource { get; init; }
//    public required CancellationTokenSource Player2CancellationTokenSource { get; init; }

//    private readonly SemaphoreSlim _choiceSubmissionLock = new(1, 1);

//    // Game-level timers
//    private DateTime _gameStartTime;
//    private TimeSpan _player1TotalTime = TimeSpan.Zero;
//    private TimeSpan _player2TotalTime = TimeSpan.Zero;
//    private DateTime? _currentPlayerTurnStart;

//    // Timer limits
//    public static readonly TimeSpan PlayerTotalTimeLimit = TimeSpan.FromMinutes(PlayerTotalTimeLimitMinutes);
//    public static readonly TimeSpan GameTimeLimit = TimeSpan.FromMinutes(GameTotalTimeLimitMinutes);

//    // Events for external observers
//    public event EventHandler<Turn>? TurnCompleted;
//    public event EventHandler<GameEndEventArgs>? GameEnded;


//    public void Start()
//    {
//        // Initialize game state
//        Turns.Clear();
//        TurnCounter = 0;
//        Turns.Add(new TeamPreviewTurn
//        {
//            Side1Start = Side1.Copy(),
//            Side2Start = Side2.Copy(),
//            FieldStart = Field.Copy(),
//            TurnCounter = TurnCounter,
//        });
//        _gameStartTime = DateTime.UtcNow;
//    }

//    /// <summary>
//    /// Main async battle loop - implements the core game flow
//    /// </summary>
//    public async Task RunBattleAsync(CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            if (PrintDebug)
//                Console.WriteLine("Starting battle...");

//            while (!IsGameComplete && !cancellationToken.IsCancellationRequested)
//            {
//                // Check for game-level timeouts before processing turn
//                if (HasGameTimedOut())
//                {
//                    await HandleGameTimeoutAsync();
//                    break;
//                }

//                // Process the current turn
//                await ProcessCurrentTurnAsync(cancellationToken);

//                // Check for game end conditions after turn processing using BattleCore
//                if (BattleCore.CheckForGameEndConditions(Side1, Side2))
//                {
//                    await HandleNormalGameEndAsync();
//                    break;
//                }

//                // Check turn limit using BattleCore
//                if (BattleCore.HasExceededTurnLimit(TurnCounter))
//                {
//                    await HandleTurnLimitReachedAsync();
//                    break;
//                }
//            }

//            if (PrintDebug)
//                Console.WriteLine("Battle completed.");
//        }
//        catch (OperationCanceledException)
//        {
//            if (PrintDebug)
//                Console.WriteLine("Battle was cancelled.");
//            await HandleBattleCancellationAsync();
//        }
//        catch (Exception ex)
//        {
//            if (PrintDebug)
//                Console.WriteLine($"Battle error: {ex.Message}");
//            await HandleBattleErrorAsync(ex);
//            throw;
//        }
//        finally
//        {
//            await CleanupBattleResourcesAsync();
//        }
//    }

//    /// <summary>
//    /// Turn processing dispatch based on turn type
//    /// </summary>
//    private async Task ProcessCurrentTurnAsync(CancellationToken cancellationToken)
//    {
//        if (PrintDebug)
//            Console.WriteLine($"Processing turn {TurnCounter} of type {CurrentTurn.GetType().Name}");

//        switch (CurrentTurn)
//        {
//            case TeamPreviewTurn teamPreviewTurn:
//                await ProcessTeamPreviewTurnAsync(teamPreviewTurn, cancellationToken);
//                break;

//            case GameplayTurn gameplayTurn:
//                await ProcessGameplayTurnAsync(gameplayTurn, cancellationToken);
//                break;

//            case PostGameTurn:
//                // Game is complete, no processing needed
//                if (PrintDebug)
//                    Console.WriteLine("Game completed - PostGameTurn reached");
//                break;

//            default:
//                throw new InvalidOperationException($"Unknown turn type: {CurrentTurn.GetType().Name}");
//        }
//    }

//    /// <summary>
//    /// Get the current side for a player
//    /// </summary>
//    public Side GetCurrentSide(PlayerId playerId)
//    {
//        return playerId == PlayerId.Player1 ? Side1 : Side2;
//    }

//    public Side GetSide(SideId sideId)
//    {
//        return sideId == SideId.Side1 ? Side1 : Side2;
//    }

//    public Side GetSide(PlayerId playerId)
//    {
//        return GetCurrentSide(playerId);
//    }

//    /// <summary>
//    /// Get the opponent's side for a player
//    /// </summary>
//    public Side GetOpponentSide(PlayerId playerId)
//    {
//        return playerId == PlayerId.Player1 ? Side2 : Side1;
//    }

//    public Side GetOpponentSide(Side side)
//    {
//        PlayerId playerId = side.PlayerId;
//        return GetOpponentSide(playerId);
//    }

//    /// <summary>
//    /// Get the player for a given PlayerId
//    /// </summary>
//    public IPlayerNew GetPlayer(PlayerId playerId)
//    {
//        return playerId == PlayerId.Player1 ? Player1 : Player2;
//    }

//    /// <summary>
//    /// Get cancellation token for a player
//    /// </summary>
//    public CancellationTokenSource GetPlayerCancellationTokenSource(PlayerId playerId)
//    {
//        return playerId == PlayerId.Player1 ? Player1CancellationTokenSource : Player2CancellationTokenSource;
//    }

//    /// <summary>
//    /// Update player total time tracking
//    /// </summary>
//    private void UpdatePlayerTime(PlayerId playerId, TimeSpan duration)
//    {
//        if (playerId == PlayerId.Player1)
//            _player1TotalTime += duration;
//        else
//            _player2TotalTime += duration;

//        if (PrintDebug)
//            Console.WriteLine($"Player {playerId} time updated: +{duration.TotalSeconds:F1}s" +
//                              $"(Total: {GetPlayerTotalTime(playerId).TotalSeconds:F1}s)");
//    }

//    /// <summary>
//    /// Get total time used by a player
//    /// </summary>
//    public TimeSpan GetPlayerTotalTime(PlayerId playerId)
//    {
//        return playerId == PlayerId.Player1 ? _player1TotalTime : _player2TotalTime;
//    }

//    /// <summary>
//    /// Create the next turn after completing the current one
//    /// </summary>
//    private async Task CreateNextTurnAsync()
//    {
//        TurnCounter++;

//        var nextTurn = new GameplayTurn
//        {
//            Side1Start = Side1.Copy(),
//            Side2Start = Side2.Copy(),
//            FieldStart = Field.Copy(),
//            TurnCounter = TurnCounter,
//        };

//        Turns.Add(nextTurn);

//        if (PrintDebug)
//            Console.WriteLine($"Created next turn: {TurnCounter}");

//        await Task.CompletedTask;
//    }

//    /// <summary>
//    /// Complete the current turn by setting end states
//    /// </summary>
//    private void CompleteTurnWithEndStates()
//    {
//        Turn currentTurn = Turns[^1];
        
//        Turn completedTurn;
//        switch (currentTurn)
//        {
//            case TeamPreviewTurn tp:
//                completedTurn = tp with
//                {
//                    Side1End = Side1.Copy(),
//                    Side2End = Side2.Copy(),
//                    FieldEnd = Field.Copy(),
//                    TurnEndTime = DateTime.UtcNow,
//                };
//                break;
                
//            case GameplayTurn gp:
//                completedTurn = gp with
//                {
//                    Side1End = Side1.Copy(),
//                    Side2End = Side2.Copy(),
//                    FieldEnd = Field.Copy(),
//                    TurnEndTime = DateTime.UtcNow,
//                };
//                break;
                
//            default:
//                throw new InvalidOperationException($"Cannot complete turn of type: {currentTurn.GetType().Name}");
//        }

//        Turns[^1] = completedTurn;
//        TurnCompleted?.Invoke(this, completedTurn);

//        if (PrintDebug)
//            Console.WriteLine($"Turn {TurnCounter} completed");
//    }

//    private Pokemon[] GetAllFaintedActivePokemon()
//    {

//        List<Pokemon> faintedActive = [];
//        faintedActive.AddRange(Side1.FaintedActivePokemon);
//        faintedActive.AddRange(Side2.FaintedActivePokemon);
//        return faintedActive.ToArray();
//    }

//    /// <summary>
//    /// Clear all turn history. This can only be done after game completion.
//    /// Reduces memory usage for long-running simulations.
//    /// </summary>
//    public void ClearTurns()
//    {
//        if (CurrentTurn is not PostGameTurn)
//        {
//            throw new InvalidOperationException("Can only clear turns after game completion.");
//        }
//        Turns.Clear();
//    }

//    /// <summary>
//    /// Dispose of battle resources
//    /// </summary>
//    public void Dispose()
//    {
//        _choiceSubmissionLock.Dispose();
//        Player1CancellationTokenSource.Dispose();
//        Player2CancellationTokenSource.Dispose();
//    }

//    #region Error Handling (formerly BattleAsync.ErrorHandling.cs)

//    /// <summary>
//    /// Handle battle cancellation
//    /// </summary>
//    private async Task HandleBattleCancellationAsync()
//    {
//        if (PrintDebug)
//            Console.WriteLine("Battle was cancelled");

//        await CleanupActiveChoicesAsync();
        
//        // Don't create a PostGameTurn for cancellation - leave battle in current state
//        await Task.CompletedTask;
//    }

//    /// <summary>
//    /// Handle unexpected battle errors
//    /// </summary>
//    private async Task HandleBattleErrorAsync(Exception exception)
//    {
//        if (PrintDebug)
//            Console.WriteLine($"Battle error occurred: {exception.Message}");

//        try
//        {
//            await CleanupActiveChoicesAsync();
            
//            // End game with error reason - pick a winner randomly or based on current state
//            var winner = DetermineWinnerFromCurrentState();
//            await EndGameAsync(winner, GameEndReason.Error);
//        }
//        catch (Exception cleanupEx)
//        {
//            if (PrintDebug)
//                Console.WriteLine($"Error during error cleanup: {cleanupEx.Message}");
//        }
//    }

//    /// <summary>
//    /// Cleanup battle resources
//    /// </summary>
//    private async Task CleanupBattleResourcesAsync()
//    {
//        if (PrintDebug)
//            Console.WriteLine("Cleaning up battle resources...");

//        try
//        {
//            // Release semaphore if held
//            if (_choiceSubmissionLock.CurrentCount == 0)
//            {
//                _choiceSubmissionLock.Release();
//            }

//            // Additional cleanup as needed
//            await Task.CompletedTask;
//        }
//        catch (Exception ex)
//        {
//            if (PrintDebug)
//                Console.WriteLine($"Error during resource cleanup: {ex.Message}");
//        }
//    }

//    /// <summary>
//    /// Determine winner from current battle state (for error scenarios)
//    /// </summary>
//    private PlayerId DetermineWinnerFromCurrentState()
//    {
//        // Try to determine winner based on current Pokemon status
//        var side1Remaining = Side1.AllSlots.Count(p => !p.IsFainted);
//        var side2Remaining = Side2.AllSlots.Count(p => !p.IsFainted);

//        if (side1Remaining > side2Remaining)
//            return PlayerId.Player1;
//        if (side2Remaining > side1Remaining)
//            return PlayerId.Player2;

//        // If tied, use tiebreak logic
//        return PerformTiebreak();
//    }

//    /// <summary>
//    /// Validate battle state and throw if invalid
//    /// </summary>
//    private void ValidateBattleState()
//    {
//        if (!IsValidBattleState())
//        {
//            throw new InvalidOperationException("Battle is in an invalid state");
//        }
//    }

//    /// <summary>
//    /// Handle unexpected choice submission errors
//    /// </summary>
//    private async Task HandleChoiceErrorAsync(PlayerId playerId, Exception exception)
//    {
//        if (PrintDebug)
//            Console.WriteLine($"Choice error for {playerId}: {exception.Message}");

//        try
//        {
//            var player = GetPlayer(playerId);
//            await player.NotifyChoiceTimeoutAsync();
//        }
//        catch (Exception notifyEx)
//        {
//            if (PrintDebug)
//                Console.WriteLine($"Error notifying player of choice error: {notifyEx.Message}");
//        }
//    }

//    /// <summary>
//    /// Validate turn state before processing
//    /// </summary>
//    private void ValidateTurnState()
//    {
//        if (IsGameComplete)
//            throw new InvalidOperationException("Cannot process turn - game is complete");

//        if (TurnCounter < 0)
//            throw new InvalidOperationException("Invalid turn counter");

//        if (CurrentTurn == null)
//            throw new InvalidOperationException("No current turn");
//    }

//    /// <summary>
//    /// Handle recoverable errors during turn processing
//    /// </summary>
//    private async Task<bool> TryRecoverFromTurnErrorAsync(Exception exception)
//    {
//        if (PrintDebug)
//            Console.WriteLine($"Attempting to recover from turn error: {exception.Message}");

//        try
//        {
//            // For certain types of errors, we might be able to continue
//            // For example, if a single action failed, we could skip it and continue
            
//            // For now, return false to indicate recovery failed
//            await Task.CompletedTask;
//            return false;
//        }
//        catch
//        {
//            return false;
//        }
//    }

//    /// <summary>
//    /// Log battle state for debugging
//    /// </summary>
//    private void LogBattleState(string context)
//    {
//        if (!PrintDebug)
//            return;

//        Console.WriteLine($"=== Battle State ({context}) ===");
//        Console.WriteLine($"Turn: {TurnCounter}");
//        Console.WriteLine($"Current Turn Type: {CurrentTurn?.GetType().Name}");
//        Console.WriteLine($"Game Complete: {IsGameComplete}");
//        Console.WriteLine($"Game Time: {DateTime.UtcNow - _gameStartTime:mm\\:ss}");
//        Console.WriteLine($"Player 1 Time: {_player1TotalTime:mm\\:ss}");
//        Console.WriteLine($"Player 2 Time: {_player2TotalTime:mm\\:ss}");
        
//        // Log Pokemon status
//        Console.WriteLine("Player 1 Pokemon:");
//        foreach (var pokemon in Side1.AllSlots)
//        {
//            Console.WriteLine($"  {pokemon.Specie.Name}: {pokemon.CurrentHp}/{pokemon.UnmodifiedHp} HP {(pokemon.IsFainted ? "(Fainted)" : "")}");
//        }
        
//        Console.WriteLine("Player 2 Pokemon:");
//        foreach (var pokemon in Side2.AllSlots)
//        {
//            Console.WriteLine($"  {pokemon.Specie.Name}: {pokemon.CurrentHp}/{pokemon.UnmodifiedHp} HP {(pokemon.IsFainted ? "(Fainted)" : "")}");
//        }
        
//        Console.WriteLine("===========================");
//    }

//    #endregion

//    #region Post Game (formerly BattleAsync.PostGame.cs)

//    /// <summary>
//    /// Handle post-game operations
//    /// </summary>
//    public async Task ProcessPostGameTurnAsync(PostGameTurn turn, CancellationToken cancellationToken)
//    {
//        if (PrintDebug)
//            Console.WriteLine($"Processing post-game turn - Winner: {turn.Winner}");

//        // Post-game turn doesn't require player input, just finalization
//        await FinalizeGameAsync(turn);
//    }

//    /// <summary>
//    /// Finalize the game after it has ended
//    /// </summary>
//    private async Task FinalizeGameAsync(PostGameTurn postGameTurn)
//    {
//        if (PrintDebug)
//        {
//            Console.WriteLine("=== GAME COMPLETED ===");
//            Console.WriteLine($"Winner: {postGameTurn.Winner}");
//            Console.WriteLine($"Total Turns: {TurnCounter}");
//            Console.WriteLine($"Game Duration: {DateTime.UtcNow - _gameStartTime:mm\\:ss}");
//            Console.WriteLine($"Player 1 Total Time: {_player1TotalTime:mm\\:ss}");
//            Console.WriteLine($"Player 2 Total Time: {_player2TotalTime:mm\\:ss}");
            
//            // Show final Pokemon status
//            Console.WriteLine("\nFinal Pokemon Status:");
//            ShowFinalPokemonStatus(PlayerId.Player1, Side1);
//            ShowFinalPokemonStatus(PlayerId.Player2, Side2);
            
//            Console.WriteLine("=====================");
//        }

//        // Perform any final cleanup or notifications
//        await NotifyPlayersOfGameEndAsync(postGameTurn);
        
//        // Update any persistent game records
//        await UpdateGameRecordsAsync(postGameTurn);
//    }

//    /// <summary>
//    /// Show final Pokemon status for debugging
//    /// </summary>
//    private void ShowFinalPokemonStatus(PlayerId playerId, Side side)
//    {
//        Console.WriteLine($"{playerId} Pokemon:");
//        foreach (var pokemon in side.AllSlots)
//        {
//            var status = pokemon.IsFainted ? "Fainted" : $"{pokemon.CurrentHp}/{pokemon.UnmodifiedHp} HP";
//            Console.WriteLine($"  {pokemon.Specie.Name}: {status}");
//        }
//    }

//    /// <summary>
//    /// Notify players that the game has ended
//    /// </summary>
//    private async Task NotifyPlayersOfGameEndAsync(PostGameTurn postGameTurn)
//    {
//        try
//        {
//            // Create game end notification
//            var gameEndInfo = new GameEndNotification
//            {
//                Winner = postGameTurn.Winner,
//                GameDuration = DateTime.UtcNow - _gameStartTime,
//                TotalTurns = TurnCounter,
//                FinalScore = CalculateFinalScore()
//            };

//            // Notify both players
//            await NotifyPlayerOfGameEndAsync(Player1, gameEndInfo);
//            await NotifyPlayerOfGameEndAsync(Player2, gameEndInfo);
//        }
//        catch (Exception ex)
//        {
//            if (PrintDebug)
//                Console.WriteLine($"Error notifying players of game end: {ex.Message}");
//        }
//    }

//    /// <summary>
//    /// Notify a single player of game end
//    /// </summary>
//    private async Task NotifyPlayerOfGameEndAsync(IPlayerNew player, GameEndNotification notification)
//    {
//        try
//        {
//            // For now, just log - in a real implementation you might have a notification method
//            if (PrintDebug)
//                Console.WriteLine($"Notifying {player.PlayerId} of game end");
            
//            await Task.CompletedTask;
//        }
//        catch (Exception ex)
//        {
//            if (PrintDebug)
//                Console.WriteLine($"Error notifying {player.PlayerId}: {ex.Message}");
//        }
//    }

//    /// <summary>
//    /// Calculate final score for the game
//    /// </summary>
//    private GameScore CalculateFinalScore()
//    {
//        var side1Remaining = Side1.AllSlots.Count(p => !p.IsFainted);
//        var side2Remaining = Side2.AllSlots.Count(p => !p.IsFainted);
        
//        var side1HpPercentage = CalculateHpPercentage(Side1);
//        var side2HpPercentage = CalculateHpPercentage(Side2);

//        return new GameScore
//        {
//            Player1PokemonRemaining = side1Remaining,
//            Player2PokemonRemaining = side2Remaining,
//            Player1HpPercentage = side1HpPercentage,
//            Player2HpPercentage = side2HpPercentage,
//            Player1TotalTime = _player1TotalTime,
//            Player2TotalTime = _player2TotalTime
//        };
//    }

//    /// <summary>
//    /// Calculate HP percentage for a side
//    /// </summary>
//    private double CalculateHpPercentage(Side side)
//    {
//        int totalMaxHp = side.AllSlots.Sum(p => p.UnmodifiedAtk);
//        int totalCurrentHp = side.AllSlots.Sum(p => p.CurrentHp);
        
//        return totalMaxHp > 0 ? (double)totalCurrentHp / totalMaxHp * 100.0 : 0.0;
//    }

//    /// <summary>
//    /// Update persistent game records
//    /// </summary>
//    private async Task UpdateGameRecordsAsync(PostGameTurn postGameTurn)
//    {
//        try
//        {
//            // In a real implementation, this might:
//            // - Update player rankings/ELO
//            // - Save battle replay data
//            // - Update statistics
//            // - Log to database
            
//            if (PrintDebug)
//                Console.WriteLine("Updating game records...");
            
//            await Task.CompletedTask;
//        }
//        catch (Exception ex)
//        {
//            if (PrintDebug)
//                Console.WriteLine($"Error updating game records: {ex.Message}");
//        }
//    }

//    /// <summary>
//    /// Get detailed game results
//    /// </summary>
//    public GameResults GetGameResults()
//    {
//        if (!IsGameComplete)
//            throw new InvalidOperationException("Game is not complete");

//        var postGameTurn = (PostGameTurn)CurrentTurn;
//        var summary = GetGameSummary();
//        var score = CalculateFinalScore();
        
//        return new GameResults
//        {
//            Winner = postGameTurn.Winner,
//            Loser = postGameTurn.Winner == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1,
//            GameSummary = summary,
//            FinalScore = score,
//            TurnHistory = GetTurnHistory(),
//            BattleStatistics = GetBattleStatistics()
//        };
//    }

//    /// <summary>
//    /// Generate battle replay data
//    /// </summary>
//    public BattleReplayData GenerateReplayData()
//    {
//        return new BattleReplayData
//        {
//            BattleId = Guid.NewGuid(), // In real system, this would be assigned earlier
//            Seed = BattleSeed,
//            Format = Format,
//            Players = new Dictionary<PlayerId, string>
//            {
//                [PlayerId.Player1] = Player1.PlayerId.ToString(),
//                [PlayerId.Player2] = Player2.PlayerId.ToString()
//            },
//            TurnHistory = GetTurnHistory(),
//            GameResults = IsGameComplete ? GetGameResults() : null,
//            CreatedAt = DateTime.UtcNow
//        };
//    }

//    /// <summary>
//    /// Check if battle can be replayed
//    /// </summary>
//    public bool CanBeReplayed()
//    {
//        return IsGameComplete && 
//               BattleSeed.HasValue && 
//               Turns.Count > 0;
//    }

//    #endregion

//    #region Perspective (formerly BattleAsync.Perspective.cs)

//    private BattlePerspective GetPerspective(PlayerId playerId)
//    {
//        // TODO: Hide information based on game rules
//        return playerId switch
//        {
//            PlayerId.Player1 => BattlePerspective.CreateSafe(Side1, Side2, Field, TurnCounter, this),
//            PlayerId.Player2 => BattlePerspective.CreateSafe(Side2, Side1, Field, TurnCounter, this),
//            _ => throw new ArgumentOutOfRangeException(nameof(playerId), "Invalid player ID"),
//        };
//    }

//    #endregion

//    #region Turn Processing (formerly BattleAsync.TurnProcessing.cs)

//    /// <summary>
//    /// Main turn processing dispatcher
//    /// </summary>
//    public async Task ProcessTurnAsync(CancellationToken cancellationToken = default)
//    {
//        ValidateTurnState();

//        try
//        {
//            LogBattleState("Before Turn Processing");

//            await ProcessCurrentTurnAsync(cancellationToken);

//            LogBattleState("After Turn Processing");
//        }
//        catch (Exception ex)
//        {
//            if (PrintDebug)
//                Console.WriteLine($"Turn processing error: {ex.Message}");

//            // Try to recover from the error
//            bool recovered = await TryRecoverFromTurnErrorAsync(ex);
//            if (!recovered)
//            {
//                // If recovery failed, rethrow the exception
//                throw;
//            }
//        }
//    }

//    /// <summary>
//    /// Get turn history for replay or analysis
//    /// </summary>
//    public IReadOnlyList<Turn> GetTurnHistory()
//    {
//        return Turns.AsReadOnly();
//    }

//    /// <summary>
//    /// Get specific turn by index
//    /// </summary>
//    public Turn GetTurn(int turnIndex)
//    {
//        if (turnIndex < 0 || turnIndex >= Turns.Count)
//            throw new ArgumentOutOfRangeException(nameof(turnIndex));

//        return Turns[turnIndex];
//    }

//    /// <summary>
//    /// Get the last completed turn (excludes current in-progress turn)
//    /// </summary>
//    public Turn? GetLastCompletedTurn()
//    {
//        // Find the last turn that has end states set
//        for (int i = Turns.Count - 1; i >= 0; i--)
//        {
//            var turn = Turns[i];
//            if (turn.Side1End != null && turn.Side2End != null && turn.FieldEnd != null)
//                return turn;
//        }

//        return null;
//    }

//    /// <summary>
//    /// Check if current turn is complete
//    /// </summary>
//    public bool IsCurrentTurnComplete()
//    {
//        if (Turns.Count == 0)
//            return false;

//        var currentTurn = CurrentTurn;
//        return currentTurn.Side1End != null &&
//               currentTurn.Side2End != null &&
//               currentTurn.FieldEnd != null &&
//               currentTurn.TurnEndTime != null;
//    }

//    /// <summary>
//    /// Force complete the current turn (for error recovery)
//    /// </summary>
//    public void ForceCompleteTurn()
//    {
//        if (IsCurrentTurnComplete())
//            return;

//        if (PrintDebug)
//            Console.WriteLine("Force completing current turn");

//        CompleteTurnWithEndStates();
//    }

//    /// <summary>
//    /// Get battle statistics
//    /// </summary>
//    public BattleStatistics GetBattleStatistics()
//    {
//        return new BattleStatistics
//        {
//            TotalTurns = TurnCounter,
//            GameDuration = DateTime.UtcNow - _gameStartTime,
//            Player1TotalTime = _player1TotalTime,
//            Player2TotalTime = _player2TotalTime,
//            IsComplete = IsGameComplete,
//            TurnsCompleted = Turns.Count(t => t.TurnEndTime != null),
//            GameStartTime = _gameStartTime
//        };
//    }

//    /// <summary>
//    /// Calculate turn duration
//    /// </summary>
//    public TimeSpan? GetTurnDuration(int turnIndex)
//    {
//        var turn = GetTurn(turnIndex);

//        if (turn.TurnEndTime == null)
//            return null;

//        return turn.TurnEndTime.Value - turn.TurnStartTime;
//    }

//    /// <summary>
//    /// Get average turn duration
//    /// </summary>
//    public TimeSpan GetAverageTurnDuration()
//    {
//        var completedTurns = Turns.Where(t => t.TurnEndTime != null).ToList();

//        if (completedTurns.Count == 0)
//            return TimeSpan.Zero;

//        var totalDuration = completedTurns
//            .Sum(t => (t.TurnEndTime!.Value - t.TurnStartTime).Ticks);

//        return new TimeSpan(totalDuration / completedTurns.Count);
//    }

//    /// <summary>
//    /// Check if battle is currently waiting for player input
//    /// </summary>
//    public bool IsWaitingForInput()
//    {
//        // Battle is waiting if it's not complete and current turn is not complete
//        return !IsGameComplete && !IsCurrentTurnComplete();
//    }

//    /// <summary>
//    /// Get which players are currently expected to provide input
//    /// </summary>
//    public List<PlayerId> GetPlayersWaitingForInput()
//    {
//        var waitingPlayers = new List<PlayerId>();

//        if (!IsWaitingForInput())
//            return waitingPlayers;

//        // This would depend on your turn structure and what actions are pending
//        // For now, return both players as potentially waiting
//        switch (CurrentTurn)
//        {
//            case TeamPreviewTurn:
//                // Both players need to submit team preview
//                waitingPlayers.Add(PlayerId.Player1);
//                waitingPlayers.Add(PlayerId.Player2);
//                break;

//            case GameplayTurn:
//                // Would need to check which specific actions are pending
//                // For now, assume both players might need to submit
//                waitingPlayers.Add(PlayerId.Player1);
//                waitingPlayers.Add(PlayerId.Player2);
//                break;
//        }

//        return waitingPlayers;
//    }

//    /// <summary>
//    /// Pause the battle (for external control)
//    /// </summary>
//    public void PauseBattle()
//    {
//        if (PrintDebug)
//            Console.WriteLine("Battle paused");

//        // Pause timers by not updating them
//        // This would require more sophisticated timer management
//    }

//    /// <summary>
//    /// Resume the battle (for external control)
//    /// </summary>
//    public void ResumeBattle()
//    {
//        if (PrintDebug)
//            Console.WriteLine("Battle resumed");

//        // Resume timers
//        // This would require more sophisticated timer management
//    }

//    #endregion
//}

//// Supporting classes
//public class PendingAction
//{
//    public required PlayerId PlayerId { get; init; }
//    public required int ActionIndex { get; init; }
//    public required DateTime RequestTime { get; init; }
//    public BattleChoice? Choice { get; init; }
//}

//public class ActionWithChoice
//{
//    public required PlayerId PlayerId { get; init; }
//    public required BattleChoice Choice { get; init; }
//    public required int SpeedPriority { get; init; }
//    public required int ActionOrder { get; init; }
//    public required Pokemon Executor { get; init; }
//}

//public class GameEndEventArgs : EventArgs
//{
//    public required PlayerId Winner { get; init; }
//    public required GameEndReason Reason { get; init; }
//}

//public enum GameEndReason
//{
//    Normal,
//    PlayerTimeout,
//    GameTimeout,
//    Forfeit,
//    TurnLimit,
//    Error,
//}

//public class ChoiceRequestEventArgs : EventArgs
//{
//    public required BattleChoice[] AvailableChoices { get; init; }
//    public required TimeSpan TimeLimit { get; init; }
//    public required DateTime RequestTime { get; init; }
//    public int ActionIndex { get; init; } = 0;
//}

//#region Post Game Classes

///// <summary>
///// Game end notification for players
///// </summary>
//public class GameEndNotification
//{
//    public required PlayerId Winner { get; init; }
//    public required TimeSpan GameDuration { get; init; }
//    public required int TotalTurns { get; init; }
//    public required GameScore FinalScore { get; init; }
//}

///// <summary>
///// Final game score
///// </summary>
//public class GameScore
//{
//    public required int Player1PokemonRemaining { get; init; }
//    public required int Player2PokemonRemaining { get; init; }
//    public required double Player1HpPercentage { get; init; }
//    public required double Player2HpPercentage { get; init; }
//    public required TimeSpan Player1TotalTime { get; init; }
//    public required TimeSpan Player2TotalTime { get; init; }
//}

///// <summary>
///// Complete game results
///// </summary>
//public class GameResults
//{
//    public required PlayerId Winner { get; init; }
//    public required PlayerId Loser { get; init; }
//    public required GameSummary GameSummary { get; init; }
//    public required GameScore FinalScore { get; init; }
//    public required IReadOnlyList<Turn> TurnHistory { get; init; }
//    public required BattleStatistics BattleStatistics { get; init; }
//}

///// <summary>
///// Battle replay data for saving/loading battles
///// </summary>
//public class BattleReplayData
//{
//    public required Guid BattleId { get; init; }
//    public required int? Seed { get; init; }
//    public required GameType Format { get; init; }
//    public required Dictionary<PlayerId, string> Players { get; init; }
//    public required IReadOnlyList<Turn> TurnHistory { get; init; }
//    public required GameResults? GameResults { get; init; }
//    public required DateTime CreatedAt { get; init; }
//}

///// <summary>
///// Battle statistics for monitoring and analysis
///// </summary>
//public class BattleStatistics
//{
//    public required int TotalTurns { get; init; }
//    public required TimeSpan GameDuration { get; init; }
//    public required TimeSpan Player1TotalTime { get; init; }
//    public required TimeSpan Player2TotalTime { get; init; }
//    public required bool IsComplete { get; init; }
//    public required int TurnsCompleted { get; init; }
//    public required DateTime GameStartTime { get; init; }

//    public double TurnsPerMinute => GameDuration.TotalMinutes > 0 ? TotalTurns / GameDuration.TotalMinutes : 0;
//    public TimeSpan AveragePlayerTime => TimeSpan.FromTicks((Player1TotalTime.Ticks + Player2TotalTime.Ticks) / 2);
//}

//#endregion