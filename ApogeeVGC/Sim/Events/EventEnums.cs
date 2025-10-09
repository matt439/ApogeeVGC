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
    SetAbility,
    Type,
    ModifyWeight,
    Effectiveness,
    NegateImmunity,
}

public enum EventType
{
    Terastallize,
    None,
}