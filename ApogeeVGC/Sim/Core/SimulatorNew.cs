using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Ui;

namespace ApogeeVGC.Sim.Core;

public enum SimulatorResult
{
    Player1Win,
    Player2Win,
}

public class SimulatorNew
{
    public required BattleAsync Battle { get; init; }
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

            // Battle was cancelled or timed out
            if (PrintDebug)
            {
                Console.WriteLine("Battle ended without a clear winner");
            }

            // Use tiebreak to determine winner
            return DetermineTiebreakWinner();
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

    public SimulatorResult RunSync()
    {
        return Run().GetAwaiter().GetResult();
    }

    private SimulatorResult DetermineTiebreakWinner()
    {
        int side1TeamHealthTotal = Battle.Side1.HealthTeamTotal;
        int side2TeamHealthTotal = Battle.Side2.HealthTeamTotal;

        if (side1TeamHealthTotal > side2TeamHealthTotal)
            return Battle.Side1.PlayerId == PlayerId.Player1 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
        if (side2TeamHealthTotal > side1TeamHealthTotal)
            return Battle.Side2.PlayerId == PlayerId.Player1 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;

        // Pick randomly if still tied. Use battle's random for consistency
        var rand = new Random(Battle.BattleSeed ?? 1);
        return rand.Next(2) == 0 ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
    }
}