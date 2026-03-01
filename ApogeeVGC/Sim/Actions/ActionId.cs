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
    MegaEvo,
}