using System.Security.Cryptography;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using Xunit.Abstractions;
using PlayerType = ApogeeVGC.Sim.Player.PlayerType;

namespace ApogeeVGC.Tests;

/// <summary>
/// Runs a fixed set of battles across multiple formats with deterministic seeds.
/// Computes a SHA-256 hash of all results — if the hash changes between runs
/// on the same commit, the engine's behavior has changed.
/// </summary>
[Collection(LibraryCollection.Name)]
public class DeterministicRegressionTests
{
    // Deterministic seed constants (must match Driver values to produce the same hash)
    private const int Team1EvalSeed = 54321;
    private const int Team2EvalSeed = 67890;
    private const int PlayerRandom1EvalSeed = 12345;
    private const int PlayerRandom2EvalSeed = 1818;
    private const int BattleEvalSeed = 9876;

    private const int BattlesPerFormat = 200;

    private readonly Library _library;
    private readonly ITestOutputHelper _output;

    public DeterministicRegressionTests(LibraryFixture fixture, ITestOutputHelper output)
    {
        _library = fixture.Library;
        _output = output;
    }

    [Fact]
    public void AllFormats_ProduceDeterministicResults_WithNoExceptions()
    {
        var formats = new[]
        {
            FormatId.Gen9VgcRegulationI,
            FormatId.Gen9VgcMega,
        };

        var resultBytes = new List<byte>();
        var totalExceptions = 0;

        foreach (FormatId formatId in formats)
        {
            string label = _library.Formats[formatId].Name;
            int p1Wins = 0, p2Wins = 0, ties = 0, exceptions = 0;

            for (var i = 0; i < BattlesPerFormat; i++)
            {
                int baseOffset = i * 5 + 1;
                int team1Seed = Team1EvalSeed + baseOffset;
                int team2Seed = Team2EvalSeed + baseOffset + 1;

                var team1 = new RandomTeamGenerator(_library, formatId, team1Seed).GenerateTeam();
                var team2 = new RandomTeamGenerator(_library, formatId, team2Seed).GenerateTeam();

                try
                {
                    SimulatorResult result = RunBattle(
                        team1, team2,
                        PlayerRandom1EvalSeed + baseOffset + 2,
                        PlayerRandom2EvalSeed + baseOffset + 3,
                        BattleEvalSeed + baseOffset + 4,
                        formatId);

                    resultBytes.Add((byte)result);

                    switch (result)
                    {
                        case SimulatorResult.Player1Win: p1Wins++; break;
                        case SimulatorResult.Player2Win: p2Wins++; break;
                        case SimulatorResult.Tie: ties++; break;
                    }
                }
                catch (Exception ex)
                {
                    exceptions++;
                    resultBytes.Add(0xFF);
                    _output.WriteLine(
                        $"Exception in {label} battle {i}: {ex.GetType().Name}: {ex.Message}");
                }
            }

            totalExceptions += exceptions;
            _output.WriteLine($"{label}: P1={p1Wins} P2={p2Wins} Tie={ties} Err={exceptions}");
        }

        byte[] sha = SHA256.HashData(resultBytes.ToArray());
        string hashHex = Convert.ToHexString(sha)[..16];

        _output.WriteLine($"Regression Hash: {hashHex}");
        _output.WriteLine($"Total Exceptions: {totalExceptions}");

        Assert.Equal(0, totalExceptions);
    }

    private SimulatorResult RunBattle(
        IReadOnlyList<PokemonSet> team1,
        IReadOnlyList<PokemonSet> team2,
        int player1Seed,
        int player2Seed,
        int battleSeed,
        FormatId formatId)
    {
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
        return simulator.Run(_library, battleOptions, printDebug: false);
    }
}
