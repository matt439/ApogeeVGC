using BenchmarkDotNet.Running;

namespace ApogeeVGC.Benchmarks;

internal class Program
{
    static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(BattleSimulationBenchmark).Assembly).Run(args);
    }
}
