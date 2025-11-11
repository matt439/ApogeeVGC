namespace ApogeeVGC.Sim.Core;

public enum SideId
{
    P1 = 0,
    P2 = 1,
}

public enum PositionLetter
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    E = 4,
    F = 5,
}

public enum SimulatorResult
{
    Player1Win,
    Player2Win,
    Tie,
}

public enum DriverMode
{
    GuiVsRandomSingles,
    GuiVsRandomDoubles,
    RandomVsRandomSingles,
}

public enum PlayerUiType
{
    None,
    Gui,
    Console,
}

public enum PlayerType
{
    Random,
    Gui,
    Console,
    Mcts,
}
