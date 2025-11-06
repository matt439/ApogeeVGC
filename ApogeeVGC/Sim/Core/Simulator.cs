using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;

namespace ApogeeVGC.Sim.Core;

public enum SimulatorResult
{
    Player1Win,
    Player2Win,
}

public class Simulator
{
    public required IBattle Battle { get; init; }
    public bool PrintDebug { get; set; }

    public async Task<SimulatorResult> Run()
    {
        if (PrintDebug)
        {
            Console.WriteLine("Starting battle simulation...");
        }

        try
        {
            // Set a reasonable timeout for the entire battle
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(30));

            // Wait until the battle ends or we timeout
            while (!Battle.Ended)
            {
                // Check for cancellation
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                // Small delay to avoid busy waiting
                await Task.Delay(100, cancellationTokenSource.Token);
            }

            // Battle ended - determine the winner
            if (PrintDebug)
            {
                Console.WriteLine("Battle has ended.");
            }

            return DetermineWinner();
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
            throw;
        }
    }

    //public SimulatorResult RunSync()
    //{
    //    return Run().GetAwaiter().GetResult();
    //}

    private SimulatorResult DetermineWinner()
    {
        // Check if we have a winner
        if (!string.IsNullOrEmpty(Battle.Winner))
        {
            // Winner is stored as side ID string ("p1" or "p2")
            bool isP1Winner = Battle.Winner.Equals("p1", StringComparison.OrdinalIgnoreCase);

            if (PrintDebug)
            {
                string winnerName = isP1Winner ? "Player 1" : "Player 2";
                Console.WriteLine($"Winner: {winnerName}");
            }

            return isP1Winner ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
        }

        // No clear winner - use tiebreak
        if (PrintDebug)
        {
            Console.WriteLine("Battle ended without a clear winner");
        }

        return DetermineTiebreakWinner();
    }

    private SimulatorResult DetermineTiebreakWinner()
    {
        //int side1TeamHealthTotal = Battle.Side1.HealthTeamTotal;
        //int side2TeamHealthTotal = Battle.Side2.HealthTeamTotal;

        //if (side1TeamHealthTotal > side2TeamHealthTotal)
        //    return Battle.Side1.PlayerId == PlayerId.Player1 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
        //if (side2TeamHealthTotal > side1TeamHealthTotal)
        //    return Battle.Side2.PlayerId == PlayerId.Player1 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;

        //// Pick randomly if still tied. Use battle's random for consistency
        //var rand = new Random(Battle.BattleSeed ?? 1);
        //return rand.Next(2) == 0 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;

        // For now, always return Player 1 as winner in tiebreak
        Console.WriteLine("Tiebreaker applied: Player 1 declared winner by default.");
        return SimulatorResult.Player1Win;
    }
}