namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Exception thrown when a battle exceeds the maximum turn limit.
/// This typically indicates an infinite loop or stalemate condition.
/// </summary>
public class BattleTurnLimitException : Exception
{
    public int Turn { get; }
    public int MaxTurns { get; }
    public int Player1Seed { get; }
    public int Player2Seed { get; }
    public int BattleSeed { get; }

    public BattleTurnLimitException(int turn, int maxTurns)
        : base($"Battle exceeded turn limit of {maxTurns} turns (current turn: {turn}). This likely indicates an infinite loop or stalemate.")
    {
        Turn = turn;
        MaxTurns = maxTurns;
    }

    public BattleTurnLimitException(int turn, int maxTurns, int player1Seed, int player2Seed, int battleSeed)
        : base($"Battle exceeded turn limit of {maxTurns} turns (current turn: {turn}). " +
               $"Seeds - Player1: {player1Seed}, Player2: {player2Seed}, Battle: {battleSeed}")
    {
        Turn = turn;
        MaxTurns = maxTurns;
        Player1Seed = player1Seed;
        Player2Seed = player2Seed;
        BattleSeed = battleSeed;
    }
}
