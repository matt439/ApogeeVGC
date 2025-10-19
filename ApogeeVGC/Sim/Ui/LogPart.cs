using ApogeeVGC.Sim.Core;

namespace ApogeeVGC.Sim.Ui;

public record StringLogPart(string Value) : ILogPart
{
    public override string ToString() => Value;
}

public record FunctionLogPart(Func<(SideId, string, string)> Generator) : ILogPart;