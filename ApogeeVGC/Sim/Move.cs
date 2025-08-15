namespace ApogeeVGC.Sim;


public enum MoveId
{
    GlacialLance,
    LeechSeed,
    TrickRoom,
    Protect,
    VoltSwitch,
    DazzlingGleam,
    ElectroDrift,
    DracoMeteor,
    Facade,
    Crunch,
    HeadlongRush,
    StruggleBug,
    Overheat,
    RagePoweder,
    Tailwind,
    SpiritBreak,
    ThunderWave,
    Reflect,
    LightScreen,
    FakeOut,
    HeavySlam,
    LowKick,
    WildCharge,

    // Custom moves
    NormalBasic,
    FireBasic,
    WaterBasic,
    ElectricBasic,
    GrassBasic,
    IceBasic,
    FightingBasic,
    PoisonBasic,
    GroundBasic,
    FlyingBasic,
    PsychicBasic,
    BugBasic,
    RockBasic,
    GhostBasic,
    DragonBasic,
    DarkBasic,
    SteelBasic,
    FairyBasic,
}

public enum MoveCategory
{
    Physical,
    Special,
    Status,
}

public record MoveFlags
{
    public bool? AllyAnim { get; init; }
    public bool? BypassSub { get; init; }
    public bool? Bite { get; init; }
    public bool? Bullet { get; init; }
    public bool? CantUinitwice { get; init; }
    public bool? Charge { get; init; }
    public bool? Contact { get; init; }
    public bool? Dance { get; init; }
    public bool? Defrost { get; init; }
    public bool? Distance { get; init; }
    public bool? FailCopycat { get; init; }
    public bool? FailEncore { get; init; }
    public bool? FailInstruct { get; init; }
    public bool? FailMeFirst { get; init; }
    public bool? FailMimic { get; init; }
    public bool? FutureMove { get; init; }
    public bool? Gravity { get; init; }
    public bool? Heal { get; init; }
    public bool? Metronome { get; init; }
    public bool? Mirror { get; init; }
    public bool? MustPressure { get; init; }
    public bool? NoAssist { get; init; }
    public bool? NonSky { get; init; }
    public bool? NoParentalBond { get; init; }
    public bool? NoSketch { get; init; }
    public bool? NoSleepTalk { get; init; }
    public bool? PledgeCombo { get; init; }
    public bool? Powder { get; init; }
    public bool? Protect { get; init; }
    public bool? Pulse { get; init; }
    public bool? Punch { get; init; }
    public bool? Recharge { get; init; }
    public bool? Reflectable { get; init; }
    public bool? Slicing { get; init; }
    public bool? Snatch { get; init; }
    public bool? Sound { get; init; }
    public bool? Wind { get; init; }
}

public record SecondaryEffect
{
    //public Func<Battle, Pokemon, Pokemon, ActiveMove, bool?>? OnHit { get; init; }
    //public SparseBoostsTable? Boosts { get; init; }
    //public Id? Status { get; init; }
    //public Id? VolatileStatus { get; init; }
    //public string? SideCondition { get; init; }
    //public string? SlotCondition { get; init; }
    //public string? PseudoWeather { get; init; }
    //public string? Terrain { get; init; }
    //public Id? Weather { get; init; }
    //public int? Chance { get; init; }
    //public Ability? Ability { get; init; }
    //public bool? KingsRock { get; init; }
    //public IHitEffect? Self { get; init; }
}

/**
 * Describes the acceptable target(s) of a move.
 * adjacentAlly - Only relevant to Doubles or Triples, the move only targets an ally of the user.
 * adjacentAllyOrSelf - The move can target the user or its ally.
 * adjacentFoe - The move can target a foe, but not (in Triples) a distant foe.
 * all - The move targets the field or all Pokémon at once.
 * allAdjacent - The move is a spread move that also hits the user's ally.
 * allAdjacentFoes - The move is a spread move.
 * allies - The move affects all active Pokémon on the user's team.
 * allySide - The move adds a side condition on the user's side.
 * allyTeam - The move affects all unfainted Pokémon on the user's team.
 * any - The move can hit any other active Pokémon, not just those adjacent.
 * foeSide - The move adds a side condition on the foe's side.
 * normal - The move can hit one adjacent Pokémon of your choice.
 * randomNormal - The move targets an adjacent foe at random.
 * scripted - The move targets the foe that damaged the user.
 * self - The move affects the user of the move.
 */
public enum MoveTarget
{
    AdjacentAlly,
    AdjacentAllyOrSelf,
    AdjacentFoe,
    All,
    AllAdjacent,
    AllAdjacentFoes,
    Allies,
    AllySide,
    AllyTeam,
    Any,
    FoeSide,
    Normal,
    RandomNormal,
    Scripted,
    Self,
    None, // Added for DataMove.SelfSwitch
}

public record Move
{
    public int Num { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Accuracy { get; init; }
    public int BasePower { get; init; }
    public MoveCategory Category { get; init; }
    public int Pp { get; init; }
    public int Priority { get; init; }
    public MoveFlags Flags { get; init; } = new();
    public MoveTarget Target { get; init; }
    public MoveType Type { get; init; }
    public SecondaryEffect? Secondary { get; init; }
}