namespace ApogeeVGC.Sim.Choices;

/// <summary>
/// One single turn's choice for one single player.
/// </summary>
public class Choice
{
    public bool CantUndo { get; set; }
    public string Error { get; set; } = string.Empty;
    public IReadOnlyList<ChosenAction> Actions { get; set; } = [];
    public int ForcedSwitchesLeft { get; set; }
    public int ForcedPassesLeft { get; set; }
    public HashSet<int> SwitchIns { get; init; } = [];
    public bool Terastallize { get; set; }
    public bool Mega { get; set; }
}