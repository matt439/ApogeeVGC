namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    private static int RoundedDownAtHalf(double value)
    {
        return (int)(value + 0.5 - double.Epsilon);
    }

    private static int RoundedUpAtHald(double value)
    {
        return (int)(value + 0.5 + double.Epsilon);
    }

    /// <summary>
    /// Deep copy the battle state for MCTS simulation
    /// </summary>
    public BattleNew Copy()
    {
        // Create new cancellation token sources for the copy
        var newPlayer1CancellationTokenSource = new CancellationTokenSource();
        var newPlayer2CancellationTokenSource = new CancellationTokenSource();

        return new BattleNew
        {
            // Required init properties - these are immutable references
            Library = Library, // Shared reference - Library is immutable
            Field = Field.Copy(), // Deep copy the field state
            Side1 = Side1.Copy(), // Deep copy side 1
            Side2 = Side2.Copy(), // Deep copy side 2
            Format = Format, // Shared reference - BattleFormat is immutable
            Player1 = Player1, // Shared reference - players are external
            Player2 = Player2, // Shared reference - players are external
            Player1CancellationTokenSource = newPlayer1CancellationTokenSource,
            Player2CancellationTokenSource = newPlayer2CancellationTokenSource,

            // Mutable properties
            PrintDebug = PrintDebug,
            BattleSeed = BattleSeed,

            // Note: _battleRandom is private and will be recreated lazily using BattleSeed
            // Note: Turns list is not copied - MCTS simulations typically start from current state
            // Note: TurnCounter is not directly settable - it's managed by the battle system
            // Note: Events are not copied - MCTS simulations don't need event notifications
            // Note: Timing state (_gameStartTime, player times) are not copied for simulations
            // Note: _choiceSubmissionLock is not copied - each copy gets its own synchronization
        };
    }
}