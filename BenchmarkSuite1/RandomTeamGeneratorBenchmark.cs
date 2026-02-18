using BenchmarkDotNet.Attributes;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Generators;
using ApogeeVGC.Sim.PokemonClasses;
using Microsoft.VSDiagnostics;
using System.Collections.Generic;

namespace ApogeeVGC.Benchmarks;
[CPUUsageDiagnoser]
public class RandomTeamGeneratorBenchmark
{
    private Library _library = null!;
    private int _seed;
    [GlobalSetup]
    public void Setup()
    {
        _library = new Library();
        _seed = 54321;
    }

    [Benchmark]
    public List<PokemonSet> CreateGeneratorAndGenerateTeam()
    {
        var generator = new RandomTeamGenerator(_library, FormatId.Gen9VgcRegulationI, _seed);
        return generator.GenerateTeam();
    }
}