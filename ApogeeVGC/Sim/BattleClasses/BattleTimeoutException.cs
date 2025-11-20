namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Exception thrown when a battle simulation exceeds the allowed timeout duration.
/// This typically indicates an infinite loop or deadlock in the battle logic.
/// </summary>
public class BattleTimeoutException : Exception
{
    /// <summary>
    /// The seed used for Player 1's RNG.
    /// </summary>
    public int Player1Seed { get; }

    /// <summary>
    /// The seed used for Player 2's RNG.
    /// </summary>
    public int Player2Seed { get; }

    /// <summary>
    /// The seed used for the battle's RNG.
    /// </summary>
    public int BattleSeed { get; }

    /// <summary>
    /// The timeout duration in milliseconds.
    /// </summary>
    public int TimeoutMilliseconds { get; }

    public BattleTimeoutException(
        int player1Seed,
        int player2Seed,
        int battleSeed,
        int timeoutMilliseconds)
        : base($"Battle simulation exceeded timeout of {timeoutMilliseconds}ms. " +
               $"Player1Seed={player1Seed}, Player2Seed={player2Seed}, BattleSeed={battleSeed}")
    {
        Player1Seed = player1Seed;
        Player2Seed = player2Seed;
        BattleSeed = battleSeed;
        TimeoutMilliseconds = timeoutMilliseconds;
    }

    public BattleTimeoutException(
        int player1Seed,
        int player2Seed,
        int battleSeed,
        int timeoutMilliseconds,
        string message)
        : base(message)
    {
        Player1Seed = player1Seed;
        Player2Seed = player2Seed;
        BattleSeed = battleSeed;
        TimeoutMilliseconds = timeoutMilliseconds;
    }

    public BattleTimeoutException(
        int player1Seed,
        int player2Seed,
        int battleSeed,
        int timeoutMilliseconds,
        string message,
        Exception innerException)
        : base(message, innerException)
    {
        Player1Seed = player1Seed;
        Player2Seed = player2Seed;
        BattleSeed = battleSeed;
        TimeoutMilliseconds = timeoutMilliseconds;
    }
}
