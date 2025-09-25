using System.Diagnostics;
using System.Text;

namespace ApogeeVGC.Sim.Utils;

/// <summary>
/// Provides detailed memory profiling capabilities for performance analysis
/// </summary>
public class MemoryProfiler
{
    private readonly List<MemorySnapshot> _snapshots = [];
    private readonly string _profileName;
    private readonly Stopwatch _stopwatch;

    public MemoryProfiler(string profileName)
    {
        _profileName = profileName;
        _stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Take a memory snapshot at the current time
    /// </summary>
    public void TakeSnapshot(string label)
    {
        // Force garbage collection for more accurate readings
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var snapshot = new MemorySnapshot
        {
            Label = label,
            TimestampMs = _stopwatch.ElapsedMilliseconds,
            TotalMemoryBytes = GC.GetTotalMemory(false),
            Gen0Collections = GC.CollectionCount(0),
            Gen1Collections = GC.CollectionCount(1),
            Gen2Collections = GC.CollectionCount(2),
            WorkingSetBytes = Environment.WorkingSet
        };

        _snapshots.Add(snapshot);
    }

    /// <summary>
    /// Get memory usage statistics
    /// </summary>
    public MemoryStatistics GetStatistics()
    {
        if (_snapshots.Count == 0)
            return new MemoryStatistics();

        var memoryValues = _snapshots.Select(s => s.TotalMemoryBytes / (1024.0 * 1024.0)).ToList();
        var workingSetValues = _snapshots.Select(s => s.WorkingSetBytes / (1024.0 * 1024.0)).ToList();

        return new MemoryStatistics
        {
            ProfileName = _profileName,
            SnapshotCount = _snapshots.Count,
            InitialMemoryMB = memoryValues.First(),
            FinalMemoryMB = memoryValues.Last(),
            PeakMemoryMB = memoryValues.Max(),
            MinMemoryMB = memoryValues.Min(),
            MeanMemoryMB = memoryValues.Average(),
            MedianMemoryMB = memoryValues.Median(),
            StdDevMemoryMB = memoryValues.StandardDeviation(),
            InitialWorkingSetMB = workingSetValues.First(),
            FinalWorkingSetMB = workingSetValues.Last(),
            PeakWorkingSetMB = workingSetValues.Max(),
            TotalGen0Collections = _snapshots.Last().Gen0Collections - _snapshots.First().Gen0Collections,
            TotalGen1Collections = _snapshots.Last().Gen1Collections - _snapshots.First().Gen1Collections,
            TotalGen2Collections = _snapshots.Last().Gen2Collections - _snapshots.First().Gen2Collections,
            DurationMs = _stopwatch.ElapsedMilliseconds
        };
    }

    /// <summary>
    /// Generate a detailed report of memory usage
    /// </summary>
    public string GenerateReport()
    {
        var stats = GetStatistics();
        var sb = new StringBuilder();

        sb.AppendLine($"=== Memory Profile Report: {stats.ProfileName} ===");
        sb.AppendLine($"Duration: {stats.DurationMs:N0} ms");
        sb.AppendLine($"Snapshots: {stats.SnapshotCount}");
        sb.AppendLine();
        
        sb.AppendLine("Managed Memory (GC):");
        sb.AppendLine($"  Initial: {stats.InitialMemoryMB:F2} MB");
        sb.AppendLine($"  Final: {stats.FinalMemoryMB:F2} MB");
        sb.AppendLine($"  Peak: {stats.PeakMemoryMB:F2} MB");
        sb.AppendLine($"  Min: {stats.MinMemoryMB:F2} MB");
        sb.AppendLine($"  Mean: {stats.MeanMemoryMB:F2} MB");
        sb.AppendLine($"  Median: {stats.MedianMemoryMB:F2} MB");
        sb.AppendLine($"  Std Dev: {stats.StdDevMemoryMB:F2} MB");
        sb.AppendLine($"  Delta: {stats.FinalMemoryMB - stats.InitialMemoryMB:+F2;-F2;0} MB");
        sb.AppendLine();
        
        sb.AppendLine("Working Set:");
        sb.AppendLine($"  Initial: {stats.InitialWorkingSetMB:F2} MB");
        sb.AppendLine($"  Final: {stats.FinalWorkingSetMB:F2} MB");
        sb.AppendLine($"  Peak: {stats.PeakWorkingSetMB:F2} MB");
        sb.AppendLine($"  Delta: {stats.FinalWorkingSetMB - stats.InitialWorkingSetMB:+F2;-F2;0} MB");
        sb.AppendLine();
        
        sb.AppendLine("Garbage Collections:");
        sb.AppendLine($"  Gen 0: {stats.TotalGen0Collections}");
        sb.AppendLine($"  Gen 1: {stats.TotalGen1Collections}");
        sb.AppendLine($"  Gen 2: {stats.TotalGen2Collections}");
        sb.AppendLine();

        if (_snapshots.Count > 0)
        {
            sb.AppendLine("Detailed Snapshots:");
            foreach (var snapshot in _snapshots)
            {
                sb.AppendLine($"  {snapshot.TimestampMs,8:N0}ms | {snapshot.TotalMemoryBytes / (1024.0 * 1024.0),8:F2} MB | {snapshot.Label}");
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Save memory profile to a CSV file for analysis
    /// </summary>
    public void SaveToCsv(string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("Timestamp(ms),Label,TotalMemory(MB),WorkingSet(MB),Gen0Collections,Gen1Collections,Gen2Collections");
        
        foreach (var snapshot in _snapshots)
        {
            writer.WriteLine($"{snapshot.TimestampMs}," +
                           $"\"{snapshot.Label}\"," +
                           $"{snapshot.TotalMemoryBytes / (1024.0 * 1024.0):F2}," +
                           $"{snapshot.WorkingSetBytes / (1024.0 * 1024.0):F2}," +
                           $"{snapshot.Gen0Collections}," +
                           $"{snapshot.Gen1Collections}," +
                           $"{snapshot.Gen2Collections}");
        }
    }

    private struct MemorySnapshot
    {
        public string Label;
        public long TimestampMs;
        public long TotalMemoryBytes;
        public long WorkingSetBytes;
        public int Gen0Collections;
        public int Gen1Collections;
        public int Gen2Collections;
    }
}

public struct MemoryStatistics
{
    public string ProfileName;
    public int SnapshotCount;
    public double InitialMemoryMB;
    public double FinalMemoryMB;
    public double PeakMemoryMB;
    public double MinMemoryMB;
    public double MeanMemoryMB;
    public double MedianMemoryMB;
    public double StdDevMemoryMB;
    public double InitialWorkingSetMB;
    public double FinalWorkingSetMB;
    public double PeakWorkingSetMB;
    public int TotalGen0Collections;
    public int TotalGen1Collections;
    public int TotalGen2Collections;
    public long DurationMs;
}