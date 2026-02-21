using BenchmarkDotNet.Running;
using ApogeeVGC.Benchmarks;

namespace BenchmarkSuite1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(BattleSimulationBenchmark).Assembly).Run(args);
        }
    }
}
