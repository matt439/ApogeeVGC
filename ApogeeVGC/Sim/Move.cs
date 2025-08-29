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
    //RagePoweder,
    Tailwind,
    SpiritBreak,
    ThunderWave,
    Reflect,
    LightScreen,
    FakeOut,
    HeavySlam,
    LowKick,
    WildCharge,

    Struggle,

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
 * allies - The move affects all active Pokémon on the user's set.
 * allySide - The move adds a side condition on the user's side.
 * allyTeam - The move affects all unfainted Pokémon on the user's set.
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
    Field, // Added for Trick Room
}

public record Move : IEffect
{
    public EffectType EffectType => EffectType.Move;
    public required MoveId Id { get; init; }
    public required int Num
    {
        get;
        init
        {
            if (Num < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Num), "Move number must be non-negative.");
            }
            field = value;
        }
    }
    public string Name { get; init; } = string.Empty;
    public required int Accuracy
    {
        get;
        init
        {
            if (value is < 1 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(Accuracy), "Accuracy must be between 1 and 100.");
            }
            field = value;
        }
    }
    public int BasePower
    {
        get;
        init
        {
            if (value < 0 )
            {
                throw new ArgumentOutOfRangeException(nameof(BasePower), "Base power must be non-negative.");
            }
            field = value;
        }
    }
    public required MoveCategory Category { get; init; }
    public required int BasePp
    {
        get;
        init
        {
            if (!(value == 1 || value % 5 == 0))
            {
                throw new ArgumentOutOfRangeException(nameof(BasePp), "PP must be 1 or a multiple of 5.");
            }
            if (value > 40)
            {
                throw new ArgumentOutOfRangeException(nameof(BasePp), "PP cannot exceed 40.");
            }
            field = value;
        }
    }
    public int PpUp
    {
        get;
        set
        {
            if (value is < 0 or > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(PpUp), "PP Ups must be between 0 and 3.");
            }

            field = value;
        }
    } = 0;
    public int MaxPp => BasePp + (int)(0.2 * BasePp * PpUp);
    public int UsedPp
    {
        get;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(UsedPp), "Used PP cannot be negative.");
            }
            field = value;
        }
    } = 0;
    public int Pp
    {
        get
        {
            int pp = MaxPp - UsedPp;
            return pp > 0 ? pp : 0;
        }
    }
    public int Priority
    {
        get;
        init
        {
            if (value is > 5 or < -7)
            {
                throw new ArgumentOutOfRangeException(nameof(Priority), "Priority must be between -7 and 5.");
            }
            field = value;
        }
    } = 0;
    public MoveFlags Flags { get; init; } = new();
    public MoveTarget Target { get; init; }
    public MoveType Type { get; init; }
    public SecondaryEffect? Secondary { get; init; }
    public Condition? Condition { get; init; }
    public bool AlwaysHit { get; init; }
    public bool StallingMove { get; init; }

    public Func<Pokemon, bool>? OnTryImmunity { get; init; }
    /// <summary>
    /// target, source, move, context
    /// </summary>
    public Func<Pokemon, Pokemon, Move, BattleContext, bool>? OnPrepareHit { get; init; }
    /// <summary>
    /// target, source, move, context
    /// </summary>
    public Func<Pokemon, Pokemon?, Move?, BattleContext, bool>? OnHit { get; init; }
    /// <summary>
    /// source, target, move, context
    /// </summary>
    public Func<Pokemon, Pokemon, Move, BattleContext, int>? OnBasePower { get; init; }
    public Weather? Weather { get; init; }
    public Terrain? Terrain { get; init; }
    public PseudoWeather? PseudoWeather { get; init; }
    public SideCondition? SideCondition { get; init; }
    public bool SelfSwitch { get; init; }
    public bool Infiltrates { get; init; }

    /// <summary>
    /// Creates a deep copy of this Move for simulation purposes.
    /// This method creates an independent copy with the same state while sharing immutable references.
    /// </summary>
    /// <returns>A new Move instance with copied state</returns>
    public Move Copy()
    {
        return this with 
        { 
            // Records have built-in copy semantics with 'with' expression
            // This creates a shallow copy which is appropriate since most properties
            // are either value types, immutable references, or function delegates
            // The mutable properties (PpUp, UsedPp) are copied correctly
        };
    }

    //// Add copy constructor for explicit copying when needed
    //public Move(Move original)
    //{
    //    Id = original.Id;
    //    Num = original.Num;
    //    Name = original.Name;
    //    Accuracy = original.Accuracy;
    //    BasePower = original.BasePower;
    //    Category = original.Category;
    //    BasePp = original.BasePp;
    //    PpUp = original.PpUp;
    //    UsedPp = original.UsedPp;
    //    Priority = original.Priority;
    //    Flags = original.Flags;
    //    Target = original.Target;
    //    Type = original.Type;
    //    Secondary = original.Secondary;
    //    Condition = original.Condition;
    //    AlwaysHit = original.AlwaysHit;
    //    StallingMove = original.StallingMove;
    //    OnTryImmunity = original.OnTryImmunity;
    //    OnPrepareHit = original.OnPrepareHit;
    //    OnHit = original.OnHit;
    //    Weather = original.Weather;
    //    Terrain = original.Terrain;
    //    PseudoWeather = original.PseudoWeather;
    //}
}