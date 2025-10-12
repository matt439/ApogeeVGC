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
    ChangeBoost,
    TryBoost,
    AfterEachBoost,
    AfterBoost,
    FieldStart,
    TryTerrain,
    FieldEnd,
}

public enum EventType
{
    Terastallize,
    None,
}