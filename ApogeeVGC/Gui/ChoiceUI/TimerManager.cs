using Microsoft.Xna.Framework;

namespace ApogeeVGC.Gui.ChoiceUI;

/// <summary>
/// Manages three timers for the battle: battle timer, player time, and move time
/// </summary>
public class TimerManager
{
    /// <summary>
    /// Total time elapsed in the battle
    /// </summary>
 public TimeSpan BattleTime { get; private set; }

    /// <summary>
    /// Total time the current player has spent making decisions
    /// </summary>
    public TimeSpan PlayerTime { get; private set; }

    /// <summary>
    /// Time spent on the current move/decision
    /// </summary>
    public TimeSpan MoveTime { get; private set; }

  private bool _isMoveTimerRunning;
    private bool _isPlayerTimerRunning;
    private bool _isBattleTimerRunning;

    private double _accumulatedBattleTime;
    private double _accumulatedPlayerTime;
    private double _accumulatedMoveTime;

    public TimerManager()
    {
        BattleTime = TimeSpan.Zero;
        PlayerTime = TimeSpan.Zero;
        MoveTime = TimeSpan.Zero;
    }

 /// <summary>
    /// Start the battle timer (called once at battle start)
    /// </summary>
    public void StartBattleTimer()
    {
        _isBattleTimerRunning = true;
    }

    /// <summary>
    /// Stop the battle timer (called at battle end)
    /// </summary>
    public void StopBattleTimer()
    {
        _isBattleTimerRunning = false;
    }

    /// <summary>
    /// Start the player timer (called when player starts making a decision)
    /// </summary>
    public void StartPlayerTimer()
    {
    _isPlayerTimerRunning = true;
    }

    /// <summary>
    /// Pause the player timer (called when decision is submitted or paused)
    /// </summary>
  public void PausePlayerTimer()
    {
   _isPlayerTimerRunning = false;
    }

    /// <summary>
    /// Start/reset the move timer (called when starting a new move selection)
    /// </summary>
    public void StartMoveTimer()
    {
     _isMoveTimerRunning = true;
        _accumulatedMoveTime = 0;
        MoveTime = TimeSpan.Zero;
    }

/// <summary>
    /// Stop the move timer (called when move is selected)
    /// </summary>
    public void StopMoveTimer()
    {
        _isMoveTimerRunning = false;
  }

    /// <summary>
    /// Reset the move timer without starting it
    /// </summary>
    public void ResetMoveTimer()
    {
        _accumulatedMoveTime = 0;
        MoveTime = TimeSpan.Zero;
        _isMoveTimerRunning = false;
    }

    /// <summary>
    /// Update all active timers
    /// </summary>
    public void Update(GameTime gameTime)
    {
        double deltaSeconds = gameTime.ElapsedGameTime.TotalSeconds;

        if (_isBattleTimerRunning)
        {
     _accumulatedBattleTime += deltaSeconds;
      BattleTime = TimeSpan.FromSeconds(_accumulatedBattleTime);
        }

        if (_isPlayerTimerRunning)
    {
            _accumulatedPlayerTime += deltaSeconds;
            PlayerTime = TimeSpan.FromSeconds(_accumulatedPlayerTime);
        }

        if (_isMoveTimerRunning)
    {
            _accumulatedMoveTime += deltaSeconds;
            MoveTime = TimeSpan.FromSeconds(_accumulatedMoveTime);
        }
    }

    /// <summary>
 /// Format a TimeSpan as MM:SS.mmm
 /// </summary>
public static string FormatTime(TimeSpan time)
    {
        return $"{(int)time.TotalMinutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";
  }

    /// <summary>
    /// Get formatted battle time string
    /// </summary>
    public string GetBattleTimeString() => FormatTime(BattleTime);

    /// <summary>
    /// Get formatted player time string
    /// </summary>
    public string GetPlayerTimeString() => FormatTime(PlayerTime);

    /// <summary>
    /// Get formatted move time string
    /// </summary>
    public string GetMoveTimeString() => FormatTime(MoveTime);
}
