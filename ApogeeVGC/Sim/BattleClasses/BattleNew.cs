using ApogeeVGC.Player;
using ApogeeVGC.Sim.Turns;

namespace ApogeeVGC.Sim.BattleClasses;

public class BattleNew
{
    public List<Turn> Turns { get; private set; } = [];
    public Turn CurrentTurn => Turns.Last();
    public bool IsGameComplete => CurrentTurn is PostGameTurn;

    // Game-level timers
    private readonly DateTime _gameStartTime;
    private TimeSpan _player1TotalTime = TimeSpan.Zero;
    private TimeSpan _player2TotalTime = TimeSpan.Zero;
    private DateTime? _currentPlayerTurnStart;

    // Timer limits
    public static readonly TimeSpan PlayerTotalTimeLimit = TimeSpan.FromMinutes(7);
    public static readonly TimeSpan GameTimeLimit = TimeSpan.FromMinutes(20);

    public BattleNew(TeamPreviewTurn initialTurn)
    {
        _gameStartTime = DateTime.UtcNow;
        Turns.Add(initialTurn);
        _currentPlayerTurnStart = DateTime.UtcNow;
    }

    public void ProcessTurn(Turn turn)
    {
        // Check for turn timeout first
        if (turn.HasTurnTimedOut)
        {
            HandleTurnTimeout(turn);
            return;
        }

        // Check game-level timeouts
        if (HasGameTimedOut())
        {
            HandleGameTimeout();
            return;
        }

        var currentPlayerId = GetCurrentPlayerId(turn);
        if (HasPlayerTimedOut(currentPlayerId))
        {
            HandlePlayerTimeout(currentPlayerId);
            return;
        }

        switch (turn)
        {
            case GameplayTurn gameplayTurn:
                ProcessGameplayTurn(gameplayTurn);
                break;
            case TeamPreviewTurn teamPreviewTurn:
                ProcessTeamPreviewTurn(teamPreviewTurn);
                break;
            case PostGameTurn postGameTurn:
                ProcessPostGameTurn(postGameTurn);
                break;
        }

        // Update player time tracking
        UpdatePlayerTime(currentPlayerId);
    }

    private void ProcessPostGameTurn(PostGameTurn postGameTurn)
    {
        throw new NotImplementedException();
    }

    private void ProcessTeamPreviewTurn(TeamPreviewTurn teamPreviewTurn)
    {
        throw new NotImplementedException();
    }

    private void ProcessGameplayTurn(GameplayTurn gameplayTurn)
    {
        throw new NotImplementedException();
    }

    private void HandleTurnTimeout(Turn turn)
    {
        // Select index 0 choice and continue
        switch (turn)
        {
            case TeamPreviewTurn:
                SelectDefaultTeamPreview();
                break;
            case GameplayTurn:
                SelectDefaultMove();
                break;
        }
    }

    private void SelectDefaultMove()
    {
        throw new NotImplementedException();
    }

    private void SelectDefaultTeamPreview()
    {
        throw new NotImplementedException();
    }

    private void HandlePlayerTimeout(PlayerId playerId)
    {
        // Player loses
        var winner = playerId == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
        EndGame(winner, GameEndReason.PlayerTimeout);
    }

    private void HandleGameTimeout()
    {
        // Call tiebreak function
        var winner = PerformTiebreak();
        EndGame(winner, GameEndReason.GameTimeout);
    }

    private bool HasGameTimedOut()
    {
        return DateTime.UtcNow - _gameStartTime > GameTimeLimit;
    }

    private bool HasPlayerTimedOut(PlayerId playerId)
    {
        var playerTime = playerId == PlayerId.Player1 ? _player1TotalTime : _player2TotalTime;
        return playerTime > PlayerTotalTimeLimit;
    }

    private void UpdatePlayerTime(PlayerId playerId)
    {
        if (_currentPlayerTurnStart.HasValue)
        {
            var turnDuration = DateTime.UtcNow - _currentPlayerTurnStart.Value;

            if (playerId == PlayerId.Player1)
                _player1TotalTime += turnDuration;
            else
                _player2TotalTime += turnDuration;
        }

        _currentPlayerTurnStart = DateTime.UtcNow;
    }

    private PlayerId GetCurrentPlayerId(Turn turn)
    {
        // Logic to determine which player's turn it is
        // This depends on your game rules
        return PlayerId.Player1; // Placeholder
    }

    private PlayerId PerformTiebreak()
    {
        // Implement tiebreak logic (e.g., remaining HP, Pokemon count, etc.)
        return PlayerId.Player1; // Placeholder
    }

    private void EndGame(PlayerId winner, GameEndReason reason)
    {
        var postGameTurn = new PostGameTurn
        {
            Winner = winner,
            Side1Start = CurrentTurn.Side1End,
            Side2Start = CurrentTurn.Side2End,
            Side1End = CurrentTurn.Side1End, // No changes in post-game
            Side2End = CurrentTurn.Side2End,
            Field = CurrentTurn.Field
        };

        Turns.Add(postGameTurn);
    }
}

public enum GameEndReason
{
    Normal,
    PlayerTimeout,
    GameTimeout,
    Forfeit
}