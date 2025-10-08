namespace ApogeeVGC.Sim.Effects;

public enum ConditionId
{
    Burn,
    Paralysis,
    Sleep,
    Freeze,
    Poison,
    Toxic,
    Confusion,
    Flinch,
    ChoiceLock,
    LeechSeed,
    TrickRoom,
    Stall,
    Protect,
    Tailwind,
    Reflect,
    LightScreen,
    ElectricTerrain,
    QuarkDrive,

    Nightmare,
    LockedMove,
    Yawn,

    None,
}

public enum ConditionEffectType
{
    Condition,
    Weather,
    Status,
    Terrain,
}