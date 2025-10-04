namespace ApogeeVGC.Sim.Choices;

public record Choice
{
    public bool CantUndo { get; init; }
    //public required string Error { get; init; }
    public IReadOnlyList<ChosenAction> Actions { get; init; } = [];
    public int ForcedSwitchesLeft { get; init; }
    public int ForcedPassesLeft { get; init; }
    public HashSet<int> SwitchIns { get; init; } = [];
    public bool Terastallize { get; init; }
}