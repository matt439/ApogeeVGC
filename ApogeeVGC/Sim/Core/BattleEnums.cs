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

public enum GameType
{
    Singles,
    Doubles,
}

public enum RequestState
{
    TeamPreview,
    Move,
    SwitchIn,
    Switch,
    None,
}

public enum PlayerState
{
    TeamPreviewSelect,
    TeamPreviewLocked,
    MoveSwitchSelect,
    MoveSwitchLocked,
    FaintedSelect,
    FaintedLocked,
    ForceSwitchSelect,
    ForceSwitchLocked,
    //TagetSelect,
    //TargetLocked,
    // TODO: Implement these above states
    Idle,
}

//public enum MoveAction
//{
//    None,
//    SwitchAttackerOut,
//}

