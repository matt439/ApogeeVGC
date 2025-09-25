# Memory Profiling Guide for MCTS Evaluation

This guide explains how to use the enhanced memory profiling capabilities that have been added to the MCTS vs Random evaluation system.

## Basic Memory Tracking (Always Enabled)

The basic memory tracking is always enabled and provides:

- **Initial Memory**: Memory usage at the start of evaluation
- **Final Memory**: Memory usage at the end of evaluation  
- **Peak Memory**: Maximum memory usage observed during evaluation
- **Memory Delta**: Net change in memory from start to finish
- **Memory Statistics**: Mean, standard deviation, median, min/max memory usage throughout the evaluation
- **Real-time Progress**: Memory usage reported every 10 simulations

### Example Output:
```
Memory Usage Statistics:
Initial Memory: 45.23 MB
Final Memory: 47.86 MB
Memory Delta: +2.63 MB
Peak Memory: 52.14 MB
Mean Memory Usage: 48.75 MB
Standard Deviation of Memory: 1.45 MB
Median Memory Usage: 48.52 MB
Minimum Memory Usage: 45.23 MB
Maximum Memory Usage: 52.14 MB
Memory Samples Collected: 4
```

## Detailed Memory Profiling (Optional)

For advanced memory analysis, you can enable detailed profiling by modifying the constants in `Driver.cs`:

```csharp
// Memory profiling options
private const bool EnableDetailedMemoryProfiling = true;  // Enable detailed analysis
private const bool SaveMemoryProfileToCsv = true;         // Save CSV logs
```

### Detailed Features Include:

1. **Garbage Collection Tracking**: Monitor GC collections across generations
2. **Working Set Monitoring**: Track process working set in addition to managed memory
3. **Granular Snapshots**: Memory snapshots at key points:
   - Evaluation start/end
   - Before/after each battle setup
   - Before/after battle execution
   - After battle cleanup
   - At progress checkpoints
   - Before/after garbage collection cycles

3. **CSV Export**: Detailed memory timeline saved to CSV for external analysis
4. **Comprehensive Report**: Extended memory analysis including GC statistics

### Example Detailed Output:
```
=== DETAILED MEMORY PROFILE ===
=== Memory Profile Report: MCTS_vs_Random_Singles_100_battles ===
Duration: 45,250 ms
Snapshots: 15

Managed Memory (GC):
  Initial: 45.23 MB
  Final: 47.86 MB
  Peak: 52.14 MB
  Min: 45.23 MB
  Mean: 48.75 MB
  Median: 48.52 MB
  Std Dev: 1.45 MB
  Delta: +2.63 MB

Working Set:
  Initial: 85.45 MB
  Final: 89.12 MB
  Peak: 95.23 MB
  Delta: +3.67 MB

Garbage Collections:
  Gen 0: 25
  Gen 1: 8  
  Gen 2: 2

Detailed Snapshots:
      0ms |    45.23 MB | Evaluation_Start
   1250ms |    46.15 MB | Before_Battle_1_Setup
   1280ms |    46.89 MB | Before_Battle_1_Execution
   ...
```

## How Memory Tracking Works

### Sample Collection
- **Basic Mode**: Collects memory samples before and after each battle
- **Detailed Mode**: Collects samples at 8+ points per battle plus checkpoints

### Memory Measurements
- **Managed Memory**: Memory managed by .NET garbage collector (`GC.GetTotalMemory()`)
- **Working Set**: Total physical memory used by the process (`Environment.WorkingSet`)
- **Forced GC**: Periodic garbage collection to prevent memory buildup every 50 battles

### Statistics Calculated
All standard statistical measures are computed:
- Mean, median, standard deviation
- Min/max values  
- Total memory delta
- Peak usage tracking

## Using Memory Data for Analysis

### Performance Optimization
1. **Memory Leaks**: Watch for continuously increasing memory delta
2. **Peak Usage**: Identify memory-intensive operations
3. **GC Pressure**: Monitor garbage collection frequency
4. **Memory Efficiency**: Compare memory usage across different MCTS parameters

### MCTS Parameter Tuning
Use memory statistics to understand the relationship between:
- **Max Iterations** ? Memory usage (more iterations = more tree nodes)
- **Battle Complexity** ? Memory requirements
- **Garbage Collection** ? Performance impact

### CSV Analysis
When CSV export is enabled, you get a timestamped log suitable for:
- Plotting memory usage over time
- Correlating memory spikes with specific operations
- External statistical analysis
- Memory leak detection

## Example Usage Scenarios

### 1. Comparing MCTS Iteration Counts
```csharp
// Test with different iteration counts and compare memory usage
private const int MctsMaxIterations = 50;   // vs 100, 200, 500
private const bool EnableDetailedMemoryProfiling = true;
```

### 2. Memory Leak Detection
```csharp
private const int MctsEvaluationNumTest = 1000;  // Longer run
private const bool EnableDetailedMemoryProfiling = true;
// Watch for continuously increasing memory delta
```

### 3. Performance Profiling
```csharp
private const bool SaveMemoryProfileToCsv = true;
// Analyze CSV data for memory patterns and bottlenecks
```

## Configuration Options

| Setting | Default | Description |
|---------|---------|-------------|
| `EnableDetailedMemoryProfiling` | `false` | Enable comprehensive memory analysis |
| `SaveMemoryProfileToCsv` | `false` | Export detailed timeline to CSV |
| GC Every 50 Battles | `true` | Automatic garbage collection to prevent buildup |

## Memory Impact

The memory profiling itself has minimal overhead:
- **Basic tracking**: ~1KB per battle (2 long values)
- **Detailed profiling**: ~200 bytes per snapshot (~1.6KB per battle)
- **CSV export**: Additional I/O overhead at end of evaluation

This overhead is negligible compared to the MCTS algorithm's memory usage.