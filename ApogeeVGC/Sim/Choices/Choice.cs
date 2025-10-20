namespace ApogeeVGC.Sim.Choices;

public class Choice
{
    public bool CantUndo { get; set; }
    public string Error { get; set; } = string.Empty;
    public IReadOnlyList<ChosenAction> Actions { get; set; } = [];
    public int ForcedSwitchesLeft { get; set; }
    public int ForcedPassesLeft { get; init; }
    public HashSet<int> SwitchIns { get; init; } = [];
    public bool Terastallize { get; set; }
}