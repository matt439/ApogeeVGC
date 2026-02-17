using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using Microsoft.VSDiagnostics;

namespace ApogeeVGC.Benchmarks;
[CPUUsageDiagnoser]
public class BattleSimulationBenchmark
{
    private Library _library = null!;
    private List<PokemonSet> _team1 = null!;
    private List<PokemonSet> _team2 = null!;
    [GlobalSetup]
    public void Setup()
    {
        _library = new Library();
        // Pre-generate teams so the benchmark focuses on battle execution
        var team1Generator = new RandomTeamGenerator(_library, FormatId.Gen9VgcRegulationI, seed: 54321);
        var team2Generator = new RandomTeamGenerator(_library, FormatId.Gen9VgcRegulationI, seed: 67890);
        _team1 = team1Generator.GenerateTeam();
        _team2 = team2Generator.GenerateTeam();
    }

    [Benchmark]
    public SimulatorResult RunSingleBattle()
    {
        var player1Options = new PlayerOptions
        {
            Type = ApogeeVGC.Sim.Player.PlayerType.Random,
            Name = "Random 1",
            Team = _team1,
            Seed = new PrngSeed(12345),
            PrintDebug = false,
        };
        var player2Options = new PlayerOptions
        {
            Type = ApogeeVGC.Sim.Player.PlayerType.Random,
            Name = "Random 2",
            Team = _team2,
            Seed = new PrngSeed(1818),
            PrintDebug = false,
        };
        var battleOptions = new BattleOptions
        {
            Id = FormatId.Gen9VgcRegulationI,
            Player1Options = player1Options,
            Player2Options = player2Options,
            Debug = false,
            Sync = true,
            Seed = new PrngSeed(9876),
            MaxTurns = 1000,
        };
        var simulator = new SyncSimulator();
        return simulator.Run(_library, battleOptions, printDebug: false);
    }
}