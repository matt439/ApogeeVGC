namespace ApogeeVGC;

public class Program
{
    public static void Main(string[] args)
    {
        var driver = new Driver();
        
        // Example of how to use the new MCTS timing functionality:
        
        // Option 1: Run standard MCTS vs Random evaluation with timing statistics
        // driver.Start(DriverMode.MctsVsRandomEvaluation);
        
        // Option 2: Run a single MCTS vs Random game with timing statistics
        // driver.Start(DriverMode.MctsVsRandom);
        
        // Option 3: Compare MCTS performance across different iteration counts
        driver.Start(DriverMode.MctsVsRandomEvaluation);
        
        // The default RandomVsRandomEvaluation for comparison
        // driver.Start(DriverMode.RandomVsRandomEvaluation);
    }
}