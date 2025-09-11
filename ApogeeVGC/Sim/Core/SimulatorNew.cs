using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Ui;

namespace ApogeeVGC.Sim.Core;

public class SimulatorNew
{
    public required BattleNew Battle { get; init; }
    public required IPlayerNew Player1 { get; init; }
    public required IPlayerNew Player2 { get; init; }
    public bool PrintDebug { get; set; }

    public async Task<SimulatorResult> Run()
    {
        // Initialize the battle
        Battle.Start();

        if (PrintDebug)
        {
            Console.WriteLine("Starting battle simulation...");
        }

        try
        {
            // Run the async battle loop
            using var cancellationTokenSource = new CancellationTokenSource();

            // Set a reasonable timeout for the entire battle
            cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(30));

            await Battle.RunBattleAsync(cancellationTokenSource.Token);

            // Determine the winner
            if (Battle.IsGameComplete)
            {
                GameResults gameResults = Battle.GetGameResults();
                PlayerId winner = gameResults.Winner;

                if (!PrintDebug)
                    return winner == PlayerId.Player1 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;

                string winnerName = winner == PlayerId.Player1
                    ? Battle.Side1.Team.Trainer.Name
                    : Battle.Side2.Team.Trainer.Name;
                UiGenerator.PrintBattleEnd(winnerName);

                return winner == PlayerId.Player1 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
            }
            else
            {
                // Battle was cancelled or timed out
                if (PrintDebug)
                {
                    Console.WriteLine("Battle ended without a clear winner");
                }

                // Use tiebreak to determine winner
                return DetermineTiebreakWinner();
            }
        }
        catch (OperationCanceledException)
        {
            if (PrintDebug)
            {
                Console.WriteLine("Battle was cancelled due to timeout");
            }

            return DetermineTiebreakWinner();
        }
        catch (Exception ex)
        {
            if (PrintDebug)
            {
                Console.WriteLine($"Battle error: {ex.Message}");
            }

            // In case of error, try to determine winner from current state
            return DetermineTiebreakWinner();
        }
    }

    public SimulatorResult RunSync()
    {
        return Run().GetAwaiter().GetResult();
    }

    private SimulatorResult DetermineTiebreakWinner()
    {
        // Count remaining Pokemon
        int side1Remaining = Battle.Side1.AllSlots.Count(p => !p.IsFainted);
        int side2Remaining = Battle.Side2.AllSlots.Count(p => !p.IsFainted);

        if (side1Remaining > side2Remaining)
            return SimulatorResult.Player1Win;
        if (side2Remaining > side1Remaining)
            return SimulatorResult.Player2Win;

        // If tied, calculate HP percentages
        double side1HpPercent = CalculateHpPercentage(Battle.Side1);
        double side2HpPercent = CalculateHpPercentage(Battle.Side2);

        if (side1HpPercent > side2HpPercent)
            return SimulatorResult.Player1Win;
        return side2HpPercent > side1HpPercent ? SimulatorResult.Player2Win :
            // True tie - could return random or draw
            SimulatorResult.Player1Win; // Default fallback
    }

    private double CalculateHpPercentage(Side side)
    {
        int totalMaxHp = side.AllSlots.Sum(p => p.UnmodifiedHp);
        int totalCurrentHp = side.AllSlots.Sum(p => p.CurrentHp);

        return totalMaxHp > 0 ? (double)totalCurrentHp / totalMaxHp * 100.0 : 0.0;
    }
}