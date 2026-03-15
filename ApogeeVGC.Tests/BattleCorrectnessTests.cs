using System.Collections.Concurrent;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.Utils;
using Xunit.Abstractions;
using PlayerType = ApogeeVGC.Sim.Player.PlayerType;

namespace ApogeeVGC.Tests;

/// <summary>
/// Runs many parallel random-team battles and asserts zero exceptions.
/// This is the correctness aspect of RunRandomTeamEvaluation — the performance
/// measurement (throughput, JIT warm-up analysis) stays in Driver.
/// </summary>
[Collection(LibraryCollection.Name)]
public class BattleCorrectnessTests(LibraryFixture fixture, ITestOutputHelper output)
{
    private const int Team1EvalSeed = 54321;
    private const int Team2EvalSeed = 67890;
    private const int PlayerRandom1EvalSeed = 12345;
    private const int PlayerRandom2EvalSeed = 1818;
    private const int BattleEvalSeed = 9876;

    private readonly Library _library = fixture.Library;

    [Theory]
    [InlineData(FormatId.Gen9VgcRegulationI, 1000)]
    //[InlineData(FormatId.Gen9VgcMega, 1000)]
    public void ParallelBattles_CompleteWithoutExceptions(FormatId formatId, int numBattles)
    {
        string formatLabel = _library.Formats[formatId].Name;
        output.WriteLine($"Running {numBattles} parallel battles for {formatLabel}");

        var exceptions = new ConcurrentBag<(int Index, string Seeds, Exception Exception)>();

        Parallel.For(0, numBattles, i =>
        {
            int baseOffset = i * 5 + 1;
            int team1Seed = Team1EvalSeed + baseOffset;
            int team2Seed = Team2EvalSeed + baseOffset + 1;
            int player1Seed = PlayerRandom1EvalSeed + baseOffset + 2;
            int player2Seed = PlayerRandom2EvalSeed + baseOffset + 3;
            int battleSeed = BattleEvalSeed + baseOffset + 4;

            try
            {
                var team1 = new RandomTeamGenerator(_library, formatId, team1Seed).GenerateTeam();
                var team2 = new RandomTeamGenerator(_library, formatId, team2Seed).GenerateTeam();

                var battleOptions = new BattleOptions
                {
                    Id = formatId,
                    Player1Options = new PlayerOptions
                    {
                        Type = PlayerType.Random,
                        Name = "Random 1",
                        Team = team1,
                        Seed = new PrngSeed(player1Seed),
                        PrintDebug = false,
                    },
                    Player2Options = new PlayerOptions
                    {
                        Type = PlayerType.Random,
                        Name = "Random 2",
                        Team = team2,
                        Seed = new PrngSeed(player2Seed),
                        PrintDebug = false,
                    },
                    Debug = false,
                    Sync = true,
                    Seed = new PrngSeed(battleSeed),
                    MaxTurns = 5000,
                };

                var simulator = new SimulatorSync();
                simulator.Run(_library, battleOptions, printDebug: false);
            }
            catch (Exception ex)
            {
                var seeds = $"T1={team1Seed} T2={team2Seed} P1={player1Seed} P2={player2Seed} B={battleSeed}";
                exceptions.Add((i, seeds, ex));
            }
        });

        if (!exceptions.IsEmpty)
        {
            foreach ((int index, string seeds, Exception ex) in exceptions.OrderBy(e => e.Index))
            {
                output.WriteLine($"Battle {index} failed: {ex.GetType().Name}: {ex.Message}");
                output.WriteLine($"  Seeds: {seeds}");
            }
        }

        output.WriteLine($"Completed {numBattles} battles, {exceptions.Count} failures");
        Assert.Empty(exceptions);
    }
}
