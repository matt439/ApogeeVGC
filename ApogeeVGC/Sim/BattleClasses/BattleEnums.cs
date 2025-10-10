namespace ApogeeVGC.Sim.BattleClasses;

public enum BattleRequestType
{
    /// <summary>
    /// The request is for the start of the turn, before any actions have been taken.
    /// The choices will can include moves or switches.
    /// </summary>
    TurnStart,

    /// <summary>
    /// The request is for a forced switch, such as from a move like Roar or U-Turn.
    /// The choices will only include switches.
    /// </summary>
    ForceSwitch,

    /// <summary>
    /// The request is for a fainted Pokemon that needs to be replaced.
    /// The choices will only include switches.
    /// </summary>
    FaintSwitch,

    /// <summary>
    /// The request is for the team preview phase at the start of the battle.
    /// The choices will include selecting lead Pokemon and the order of the team.
    /// </summary>
    TeamPreview,
}

public enum GameplayExecutionStage
{
    TurnStart,
    ForceSwitch,
    FaintedSwitch,
}

public enum BattleId
{
    Default,
}

public enum SendType
{
    Message,
}