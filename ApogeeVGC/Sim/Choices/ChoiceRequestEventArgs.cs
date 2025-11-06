namespace ApogeeVGC.Sim.Choices;

public class ChoiceRequestEventArgs : EventArgs
{
    public required List<IChoiceRequest> AvailableChoices { get; init; }
    public required TimeSpan TimeLimit { get; init; }
    public required DateTime RequestTime { get; init; }
    public int ActionIndex { get; init; } = 0;
}