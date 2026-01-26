using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon : IPriorityComparison
{
    public Side Side { get; }
    public Battle Battle => Side.Battle;
    public PokemonSet Set { get; }
    public string Name => Set.Name[..Math.Min(20, Set.Name.Length)];
    public string FullName => $"{Side.Id.ToString()}: {Name}";

    public string Fullname => $"{Side.Id.GetSideIdName()}: {Name}";

    /// <summary>
    /// The Pokemon's original level from the team set (1-100).
    /// </summary>
    public int Level => Set.Level;

    /// <summary>
    /// The Pokemon's effective level for battle, adjusted by format rules.
    /// For VGC formats with AdjustLevelDown = 50, Pokemon above level 50 are scaled down.
    /// </summary>
    public int BattleLevel { get; }

    public GenderId Gender => Set.Gender;
    public int Happiness => Set.Happiness;
    public PokeballId Pokeball => Set.Pokeball;

    public List<MoveSlot> BaseMoveSlots { get; }
    public StatsTable Evs => Set.Evs;
    public StatsTable Ivs => Set.Ivs;
    public List<MoveSlot> MoveSlots { get; set; }

    public int Position { get; set; }


    public Species BaseSpecies { get; set; }
    public Species Species { get; set; }

    public EffectState SpeciesState { get; set; }

    public ConditionId Status { get; set; }
    public EffectState StatusState { get; set; }

    public Dictionary<ConditionId, EffectState> Volatiles { get; set; }
    public bool? ShowCure { get; set; }

    public StatsTable BaseStoredStats { get; set; }
    public StatsExceptHpTable StoredStats { get; set; }
    public BoostsTable Boosts { get; set; }

    public AbilityId BaseAbility { get; set; }
    public AbilityId Ability { get; set; }
    public EffectState AbilityState { get; set; }

    public ItemId Item { get; set; }
    public EffectState ItemState { get; set; }
    public ItemId LastItem { get; set; }
    public bool UsedItemThisTurn { get; set; }

    public bool AteBerry { get; set; }
    public PokemonTrapped Trapped { get; set; }
    public bool MaybeTrapped { get; set; }
    public bool MaybeDisabled { get; set; }
    public bool? MaybeLocked { get; set; }

    public Pokemon? Illusion { get; set; }
    public bool Transformed { get; set; }

    public int MaxHp { get; set; }
    public int BaseMaxHp { get; set; }
    public int Hp { get; set; }
    public bool Fainted { get; set; }
    public bool FaintQueued { get; set; }
    public bool? SubFainted { get; set; }

    public bool FormeRegression { get; set; }

    public List<PokemonType> Types { get; set; }
    public PokemonType? AddedType { get; set; }
    public bool KnownType { get; set; }
    public List<PokemonType> ApparentType { get; set; }

    public MoveIdBoolUnion SwitchFlag { get; set; }
    public bool ForceSwitchFlag { get; set; }
    public bool SkipBeforeSwitchOutEventFlag { get; set; }
    public int? DraggedIn { get; set; }
    public bool NewlySwitched { get; set; }
    public bool BeingCalledBack { get; set; }

    public Move? LastMove { get; set; }
    public Move? LastMoveEncore { get; set; } // Gen 2 only
    public Move? LastMoveUsed { get; set; }
    public int? LastMoveTargetLoc { get; set; }
    public MoveIdBoolUnion MoveThisTurn { get; set; }
    public bool StatsRaisedThisTurn { get; set; }
    public bool StatsLoweredThisTurn { get; set; }
    public BoolUndefinedUnion? MoveLastTurnResult { get; set; }
    public BoolUndefinedUnion? MoveThisTurnResult { get; set; }
    public int? HurtThisTurn { get; set; }
    public int LastDamage { get; set; }
    public List<Attacker> AttackedBy { get; set; }
    public int TimesAttacked { get; set; }

    public bool IsActive { get; set; }
    public int ActiveTurns { get; set; }
    public int ActiveMoveActions { get; set; }
    public int PreviouslySwitchedIn { get; set; }
    public bool TruantTurn { get; set; }
    public bool BondTriggered { get; set; }
    public bool SwordBoost { get; set; } // Gen 9 only
    public bool ShieldBoost { get; set; } // Gen 9 only
    public bool SyrupTriggered { get; set; } // Gen 9 only
    public List<MoveType> StellarBoostedTypes { get; set; } // Gen 9 only
    public bool HeroMessageDisplayed { get; set; } // Gen 9 only - for Zero to Hero ability

    public bool IsStarted { get; set; }
    public bool DuringMove { get; set; }

    public double WeightKg { get; set; }
    public int WeightHg { get; set; }
    public IntFalseUnion Order => int.MaxValue;
    public int Priority => 0;
    public int Speed { get; set; }
    public int SubOrder => 0;
    public int EffectOrder => 0;

    public MoveTypeFalseUnion? CanTerastallize { get; set; }
    public MoveType TeraType { get; set; }
    public List<PokemonType> BaseTypes { get; set; }
    public MoveType? Terastallized { get; set; }

    public StalenessId? Staleness { get; set; }
    public StalenessId? PendingStaleness { get; set; }

    public StalenessId? VolatileStaleness
    {
        get;
        set // must be 'external' or null
        {
            if (value is StalenessId.External or null)
            {
                field = value;
            }
            else
            {
                throw new ArgumentException("VolatileStaleness must be 'external' or null");
            }
        }
    }

    public PokemonDetails Details { get; set; }

    /// <summary>
    /// Gets the list of move IDs for the Pokemon's current move slots.
    /// </summary>
    public List<MoveId> Moves => MoveSlots.Select(moveSlot => moveSlot.Id).ToList();

    /// <summary>
    /// Gets the list of move IDs for the Pokemon's base move slots (original moves before transformations).
    /// </summary>
    public List<MoveId> BaseMoves => BaseMoveSlots.Select(moveSlot => moveSlot.Id).ToList();

    private const int TrickRoomSpeedOffset = 10000;

    public Pokemon(Battle battle, PokemonSet set, Side side)
    {
        Side = side;
        Set = set;
        BaseSpecies = battle.Library.Species[set.Species];
        Species = BaseSpecies;

        // Calculate battle level based on format rules
        // AdjustLevelDown scales Pokemon above the threshold down to that level
        // AdjustLevel forces all Pokemon to a specific level
        var ruleTable = battle.RuleTable;
        if (ruleTable.AdjustLevel.HasValue)
        {
            // Force all Pokemon to specific level
            BattleLevel = ruleTable.AdjustLevel.Value;
        }
        else if (ruleTable.AdjustLevelDown.HasValue && set.Level > ruleTable.AdjustLevelDown.Value)
        {
            // Scale down Pokemon above the threshold
            BattleLevel = ruleTable.AdjustLevelDown.Value;
        }
        else
        {
            // Use original level
            BattleLevel = set.Level;
        }

        SpeciesState = battle.InitEffectState(Species.Id);

        if (set.Moves.Count == 0)
        {
            throw new InvalidOperationException($"Set {Name} has no moves");
        }


        MoveSlots = [];
        BaseMoveSlots = [];

        List<MoveSlot> baseMoveSlots = [];

        baseMoveSlots.AddRange(from moveId in Set.Moves
            let move = battle.Library.Moves[moveId]
            let basePp = move.NoPpBoosts ? move.BasePp : move.BasePp * 8 / 5
            select new MoveSlot
            {
                Id = moveId,
                Move = moveId,
                Pp = basePp,
                MaxPp = basePp,
                Target = move.Target,
                Disabled = false,
                DisabledSource = null,
                Used = false,
            }
        );
        BaseMoveSlots = baseMoveSlots;

        // Position = 0;
        StatusState = battle.InitEffectState();
        Volatiles = [];

        BaseStoredStats = new StatsTable(); // Will be initialized in SetSpecies
        StoredStats = new StatsExceptHpTable { Atk = 0, Def = 0, SpA = 0, SpD = 0, Spe = 0 };
        Boosts = new BoostsTable
        {
            Atk = 0,
            Def = 0,
            SpA = 0,
            SpD = 0,
            Spe = 0,
            Accuracy = 0,
            Evasion = 0,
        };

        BaseAbility = set.Ability;
        Ability = BaseAbility;
        AbilityState = battle.InitEffectState(Ability, null, this);

        Item = set.Item;
        ItemState = battle.InitEffectState(Item, null, this);
        LastItem = ItemId.None;

        Trapped = PokemonTrapped.False;

        Types = BaseSpecies.Types.ToList();
        BaseTypes = Types;
        KnownType = true;
        ApparentType = BaseTypes;
        TeraType = Set.TeraType;

        SwitchFlag = false;

        LastMoveUsed = null;
        MoveThisTurn = false;
        AttackedBy = [];

        StellarBoostedTypes = [];

        WeightKg = 1.0;

        CanTerastallize = TeraType;

        // Initialize stats by calling SetSpecie
        // This must be done before ClearVolatile() and Hp = MaxHp
        SetSpecie(BaseSpecies, null, isTransform: false);

        ClearVolatile();
        Hp = MaxHp;
        Details = GetUpdatedDetails();
    }

    /// <summary>
    /// Gets the full slot identifier combining side ID and position letter.
    /// Simplified version for standard battle formats.
    /// </summary>
    public PokemonSlot GetSlot()
    {
        var positionOffset = (int)Math.Floor(Side.N / 2.0) * Side.Active.Count;
        var slotPosition = Position + positionOffset;
        return new PokemonSlot(Side.Id, slotPosition);
    }

    /// <summary>
    /// Returns a string representation of this Pokemon.
    /// If the Pokemon has an Illusion, shows the disguised Pokemon's name.
    /// For active Pokemon, includes slot identifier; otherwise just shows full name.
    /// </summary>
    /// <returns>String representation combining slot (if active) and Pokemon name</returns>
    public override string ToString()
    {
        // Determine the full name to display (real or illusion)
        var fullname = Illusion != null ? Illusion.Fullname : FullName;

        // If active, combine slot identifier with name (skip first 2 chars of fullname)
        // Otherwise just return the full name
        return IsActive ? GetSlot() + fullname[2..] : fullname;
    }

    public PokemonDetails GetUpdatedDetails(int? level = null)
    {
        var id = Species.Id;

        // Handle special forms that should use base species name
        if (id is SpecieId.GreninjaBond or SpecieId.RockruffDusk)
        {
            id = Species.BaseSpecies;
        }

        // Use provided level or fall back to Pokemon's battle level (adjusted for format rules)
        var displayLevel = level ?? BattleLevel;

        var details = new PokemonDetails
        {
            Id = id,
            Level = displayLevel,
            Gender = Gender,
            Shiny = Set.Shiny,
        };

        return details;
    }

    public Func<SideSecretSharedResult> GetFullDetails => GetFullDetailsData;

    private SideSecretSharedResult GetFullDetailsData()
    {
        var health = GetHealth();
        var details = Details;
        if (Illusion is not null)
        {
            details = Illusion.GetUpdatedDetails(Level);
        }

        if (Terastallized is not null)
        {
            details.TeraType = Terastallized;
        }

        // Combine details string with health string
        var detailsStr = details.ToString();
        Secret secretFullDetails = $"{detailsStr}|{health.Secret}";
        Shared sharedFullDetails = $"{detailsStr}|{health.Shared}";

        return health with { Secret = secretFullDetails, Shared = sharedFullDetails };
    }

    public int GetUndynamaxedHp(int? amount = null)
    {
        var hp = amount ?? Hp;
        return hp;
    }

    public Nature GetNature()
    {
        return Set.Nature;
    }

    public Func<SideSecretSharedResult> GetHealth => GetHealthData;

    private SideSecretSharedResult GetHealthData()
    {
        // If Pokemon is fainted, return fainted status
        if (Hp <= 0)
        {
            Secret faintedSecret = "0 fnt";
            Shared faintedShared = "0 fnt";
            return new SideSecretSharedResult(Side.Id, faintedSecret, faintedShared);
        }

        // Build secret HP string (always exact)
        var secretStr = $"{Hp}/{MaxHp}";
        string sharedStr;
        HpColor? hpColor = null;

        if (Battle.ReportExactHp)
        {
            // Report exact HP
            sharedStr = secretStr;
        }
        else if (Battle.ReportPercentages || Battle.Gen >= 7)
        {
            // HP Percentage Mod mechanics
            var percentage = (int)Math.Ceiling(100.0 * Hp / MaxHp);
            if (percentage == 100 && Hp < MaxHp)
            {
                percentage = 99;
            }

            sharedStr = $"{percentage}/100";

            // Calculate HP color based on percentage
            hpColor = percentage switch
            {
                > 50 => HpColor.Green,
                > 20 => HpColor.Yellow,
                _ => HpColor.Red
            };
        }
        else
        {
            // In-game accurate pixel health mechanics
            // PS doesn't use pixels after Gen 6, but for reference:
            // - [Gen 7] SM uses 99 pixels
            // - [Gen 7] USUM uses 86 pixels
            var pixels = (int)Math.Floor(48.0 * Hp / MaxHp);
            if (pixels == 0) pixels = 1; // Equivalent to: || 1

            sharedStr = $"{pixels}/48";

            if (Battle.Gen >= 5)
            {
                if (pixels == 9)
                {
                    var colorSuffix = Hp * 5 > MaxHp ? "y" : "r";
                    sharedStr += colorSuffix;
                    hpColor = colorSuffix == "y" ? HpColor.Yellow : HpColor.Red;
                }
                else if (pixels == 24)
                {
                    var colorSuffix = Hp * 2 > MaxHp ? "g" : "y";
                    sharedStr += colorSuffix;
                    hpColor = colorSuffix == "g" ? HpColor.Green : HpColor.Yellow;
                }
            }

            // For other pixel values, calculate color based on pixel ratio
            if (!hpColor.HasValue)
            {
                var hpRatio = (double)Hp / MaxHp;
                hpColor = hpRatio switch
                {
                    > 0.5 => HpColor.Green,
                    > 0.2 => HpColor.Yellow,
                    _ => HpColor.Red,
                };
            }
        }

        // Append status condition if present
        if (Status != ConditionId.None)
        {
            secretStr += $" {Status}";
            sharedStr += $" {Status}";
        }

        Secret secret = secretStr;
        Shared shared = sharedStr;

        return new SideSecretSharedResult(Side.Id, secret, shared)
        {
            HpColor = hpColor,
        };
    }
}