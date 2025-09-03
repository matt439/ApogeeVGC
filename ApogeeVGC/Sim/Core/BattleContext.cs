using ApogeeVGC.Data;

namespace ApogeeVGC.Sim.Core;

public record BattleContext
{
    public required Library Library { get; init; }
    public required Random Random { get; init; }
    public bool PrintDebug { get; init; }
}