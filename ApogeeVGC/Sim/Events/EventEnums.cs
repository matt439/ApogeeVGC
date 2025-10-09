namespace ApogeeVGC.Sim.Events;

public enum EventId
{
    TerrainChange,
    Flinch,
    StallMove,
    ModifySpecie,
    End,
    SetStatus,
    Start,
    AfterSetStatus,
    Immunity,
    TryAddVolatile,
    Restart,
    ModifyBoost,
    ModifyAtk,
    ModifyDef,
    ModifySpA,
    ModifySpD,
    ModifySpe,
}

public enum EventType
{
    Terastallize,
    None,
}