//using System.Reflection;

//namespace ApogeeVGC.Sim.BattleClasses;

//public partial class BattleAsync
//{
//    /// <summary>
//    /// Deep copy the battle state for MCTS simulation
//    /// </summary>
//    public BattleAsync Copy()
//    {
//        // Create new cancellation token sources for the copy
//        var newPlayer1CancellationTokenSource = new CancellationTokenSource();
//        var newPlayer2CancellationTokenSource = new CancellationTokenSource();

//        var copy = new BattleAsync
//        {
//            // Required init properties - these are immutable references
//            Library = Library, // Shared reference - Library is immutable
//            Field = Field.Copy(), // Deep copy the field state
//            Side1 = Side1.Copy(), // Deep copy side 1
//            Side2 = Side2.Copy(), // Deep copy side 2
//            Format = Format, // Shared reference - BattleFormat is immutable
//            Player1 = Player1, // Shared reference - players are external
//            Player2 = Player2, // Shared reference - players are external
//            Player1CancellationTokenSource = newPlayer1CancellationTokenSource,
//            Player2CancellationTokenSource = newPlayer2CancellationTokenSource,

//            // Mutable properties
//            PrintDebug = PrintDebug,
//            BattleSeed = BattleSeed,

//            // Note: _battleRandom is private and will be recreated lazily using BattleSeed
//            // Note: Events are not copied - MCTS simulations don't need event notifications
//            // Note: Timing state (_gameStartTime, player times) are not copied for simulations
//            // Note: _choiceSubmissionLock is not copied - each copy gets its own synchronization
//        };

//        // Copy the Turns list - this is essential for MCTS to work correctly
//        copy.Turns.AddRange(Turns.Select(turn => turn.Copy()));
        
//        // Copy other battle state properties needed for choice generation
//        copy.ExecutionStage = ExecutionStage;
//        copy.ForceSwitcher = ForceSwitcher;
        
//        // Set the TurnCounter using reflection since it has a private setter
//        var turnCounterProperty = typeof(BattleAsync).GetProperty(nameof(TurnCounter));
//        var turnCounterSetter = turnCounterProperty?.GetSetMethod(true);
//        turnCounterSetter?.Invoke(copy, new object[] { TurnCounter });

//        return copy;
//    }
//}