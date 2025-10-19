namespace ApogeeVGC.Sim.Choices;

public record Choice
{
    public bool CantUndo { get; init; }
    public string Error { get; init; } = string.Empty;
    public IReadOnlyList<ChosenAction> Actions { get; init; } = [];
    public int ForcedSwitchesLeft { get; init; }
    public int ForcedPassesLeft { get; init; }
    public HashSet<int> SwitchIns { get; init; } = [];
    public bool Terastallize { get; init; }
}