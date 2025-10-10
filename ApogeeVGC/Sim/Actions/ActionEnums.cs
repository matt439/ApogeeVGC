namespace ApogeeVGC.Sim.Actions;

public enum ActionId
{
    // MoveAction
    Move,
    BeforeTurnMove,
    PriorityChargeMove,

    // SwitchAction
    Switch,
    InstaSwitch,
    RevivalBlessing,

    // TeamAction
    Team,

    // FieldAction
    Start,
    Residual,
    Pass,
    BeforeTurn,

    // PokemonAction
    Shift,
    RunSwitch,
    Event,
    Terastallize,
}

public enum ActionOrder
{
    S3 = 3,
    S6 = 6,
    S5 = 5,
    S200 = 200,
    S201 = 201,
    S199 = 199,
    S106 = 106,
    S103 = 103,
}