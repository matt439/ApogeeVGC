namespace ApogeeVGC.Sim.Core;

public enum SideId
{
    P1,
    P2,
}

public enum PositionLetter
{
    A,
    B,
    C,
    D,
    E,
    F,
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

public enum MoveAction
{
    None,
    SwitchAttackerOut,
}

