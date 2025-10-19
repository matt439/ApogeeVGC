namespace ApogeeVGC.Sim.Choices;

public class Choice
{
    public bool CantUndo { get; set; }
    public string Error { get; init; } = string.Empty;
    public IReadOnlyList<ChosenAction> Actions { get; init; } = [];
    public int ForcedSwitchesLeft { get; init; }
    public int ForcedPassesLeft { get; init; }
    public HashSet<int> SwitchIns { get; init; } = [];
    public bool Terastallize { get; init; }
}