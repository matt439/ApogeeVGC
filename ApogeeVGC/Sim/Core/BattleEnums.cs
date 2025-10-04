namespace ApogeeVGC.Sim.Core;

public enum SideId
{
    Side1,
    Side2,
}

public enum SlotId
{
    Slot1 = 1,
    Slot2 = 2,
    Slot3 = 3,
    Slot4 = 4,

    // Only for singles
    Slot5 = 5,
    Slot6 = 6,
}

public enum GameType
{
    Singles,
    Doubles,
}

public enum BattleRequestState
{
    RequestingPlayer1Input,
    RequestingPlayer2Input,
    RequestingBothPlayersInput,
    Player1Win,
    Player2Win,
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

