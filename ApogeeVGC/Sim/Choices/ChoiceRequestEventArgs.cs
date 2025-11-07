namespace ApogeeVGC.Sim.Choices;

public class ChoiceRequestEventArgs : EventArgs
{
    public required IChoiceRequest Choice { get; init; }
    public required TimeSpan TimeLimit { get; init; }
    public required DateTime RequestTime { get; init; }
    public int ActionIndex { get; init; } = 0;
}