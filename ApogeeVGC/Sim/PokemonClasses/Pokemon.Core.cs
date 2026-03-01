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

    private ConditionId _status;
    private Condition? _cachedStatus;

    public ConditionId Status
    {
        get => _status;
        set
        {
            _status = value;
            _cachedStatus = null;
        }
    }

    public EffectState StatusState { get; set; }

    public Dictionary<ConditionId, EffectState> Volatiles { get; set; }
    public bool? ShowCure { get; set; }

    public StatsTable BaseStoredStats { get; set; }
    public StatsExceptHpTable StoredStats { get; set; }
    public BoostsTable Boosts { get; set; }

    public AbilityId BaseAbility { get; set; }

    private AbilityId _ability;
    private Ability? _cachedAbility;

    public AbilityId Ability
    {
        get => _ability;
        set
        {
            _ability = value;
            _cachedAbility = null;
        }
    }

    public EffectState AbilityState { get; set; }

    private ItemId _item;
    private Item? _cachedItem;

    public ItemId Item
    {
        get => _item;
        set
        {
            _item = value;
            _cachedItem = null;
        }
    }

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

    // Mega Evolution
    public SpecieId? CanMegaEvo { get; set; }

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

    #region Cached End delegates for event handler discovery (avoids per-call allocations)

    private EffectDelegate? _clearStatusEndDelegate;
    private EffectDelegate? _removeVolatileEndDelegate;
    private EffectDelegate? _clearAbilityEndDelegate;
    private EffectDelegate? _clearItemEndDelegate;

    internal EffectDelegate ClearStatusEndDelegate =>
        _clearStatusEndDelegate ??=
            EffectDelegate.FromNullableDelegate(new Action<bool>(_ => ClearStatus()))!;

    internal EffectDelegate RemoveVolatileEndDelegate =>
        _removeVolatileEndDelegate ??=
            EffectDelegate.FromNullableDelegate((Func<Condition, bool>)RemoveVolatile)!;

    internal EffectDelegate ClearAbilityEndDelegate =>
        _clearAbilityEndDelegate ??=
            EffectDelegate.FromNullableDelegate(
                (Func<AbilityIdFalseUnion?>)ClearAbility)!;

    internal EffectDelegate ClearItemEndDelegate =>
        _clearItemEndDelegate ??=
            EffectDelegate.FromNullableDelegate((Func<bool>)ClearItem)!;

    internal static readonly List<object> ClearStatusEndCallArgs = [false];

    #endregion

    #region Cached EffectHolder (avoids per-conversion allocations from implicit operator)

    private PokemonEffectHolder? _cachedEffectHolder;

    internal PokemonEffectHolder CachedEffectHolder =>
        _cachedEffectHolder ??= new PokemonEffectHolder(this);

    #endregion

    /// <summary>
    /// Gets the list of move IDs for the Pokemon's current move slots.
    /// Allocates a new list on each access — prefer <see cref="HasMove"/>/<see cref="FindMoveIndex"/> for lookups.
    /// </summary>
    public List<MoveId> Moves
    {
        get
        {
            var list = new List<MoveId>(MoveSlots.Count);
            foreach (var slot in MoveSlots) list.Add(slot.Id);
            return list;
        }
    }

    /// <summary>
    /// Gets the list of move IDs for the Pokemon's base move slots (original moves before transformations).
    /// Allocates a new list on each access — prefer <see cref="HasBaseMove"/>/<see cref="FindBaseMoveIndex"/> for lookups.
    /// </summary>
    public List<MoveId> BaseMoves
    {
        get
        {
            var list = new List<MoveId>(BaseMoveSlots.Count);
            foreach (var slot in BaseMoveSlots) list.Add(slot.Id);
            return list;
        }
    }

    /// <summary>Zero-allocation check for whether a move is in the base move slots.</summary>
    public bool HasBaseMove(MoveId moveId)
    {
        foreach (var slot in BaseMoveSlots)
            if (slot.Id == moveId) return true;
        return false;
    }

    /// <summary>Zero-allocation index-of for the current move slots. Returns -1 if not found.</summary>
    public int FindMoveIndex(MoveId moveId)
    {
        for (int i = 0; i < MoveSlots.Count; i++)
            if (MoveSlots[i].Id == moveId) return i;
        return -1;
    }

    /// <summary>Zero-allocation index-of for the base move slots. Returns -1 if not found.</summary>
    public int FindBaseMoveIndex(MoveId moveId)
    {
        for (int i = 0; i < BaseMoveSlots.Count; i++)
            if (BaseMoveSlots[i].Id == moveId) return i;
        return -1;
    }

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

        CanMegaEvo = battle.Actions.CanMegaEvo(this);
        CanTerastallize = battle.RuleTable.AllowTerastallization && CanMegaEvo is null
            ? TeraType
            : null;

        // Initialize stats by calling SetSpecie
        // This must be done before ClearVolatile() and Hp = MaxHp
        SetSpecie(BaseSpecies, null, isTransform: false);

        ClearVolatile();
        Hp = MaxHp;
        Details = GetUpdatedDetails();
    }

    /// <summary>
    /// Internal copy constructor for deep copy. Creates a copy of the Pokemon
    /// with all value-type and stat fields copied. Pokemon references (Illusion,
    /// AttackedBy sources, EffectState sources) are set to null/empty and must be
    /// remapped via <see cref="RemapPokemonReferences"/> after all Pokemon are copied.
    /// </summary>
    internal Pokemon(Side newSide, Pokemon source)
    {
        // Get-only properties
        Side = newSide;
        Set = source.Set;
        BattleLevel = source.BattleLevel;

        // Move slots (get-only BaseMoveSlots, settable MoveSlots)
        BaseMoveSlots = source.BaseMoveSlots.Select(ms => ms.Copy()).ToList();
        MoveSlots = source.MoveSlots.Select(ms => ms.Copy()).ToList();

        // Position
        Position = source.Position;

        // Species (library objects, share references)
        BaseSpecies = source.BaseSpecies;
        Species = source.Species;

        // Stats (use existing Copy() methods)
        BaseStoredStats = source.BaseStoredStats.Copy();
        StoredStats = source.StoredStats.Copy();
        Boosts = source.Boosts.Copy();

        // HP
        MaxHp = source.MaxHp;
        BaseMaxHp = source.BaseMaxHp;
        Hp = source.Hp;
        Fainted = source.Fainted;
        FaintQueued = source.FaintQueued;
        SubFainted = source.SubFainted;

        // Status
        Status = source.Status;

        // Ability / Item (enum values)
        BaseAbility = source.BaseAbility;
        Ability = source.Ability;
        Item = source.Item;
        LastItem = source.LastItem;
        UsedItemThisTurn = source.UsedItemThisTurn;
        AteBerry = source.AteBerry;

        // Trapped
        Trapped = source.Trapped;
        MaybeTrapped = source.MaybeTrapped;
        MaybeDisabled = source.MaybeDisabled;
        MaybeLocked = source.MaybeLocked;

        // Transformed
        Transformed = source.Transformed;
        FormeRegression = source.FormeRegression;

        // Types (copy lists of enums)
        Types = new List<PokemonType>(source.Types);
        BaseTypes = new List<PokemonType>(source.BaseTypes);
        ApparentType = new List<PokemonType>(source.ApparentType);
        AddedType = source.AddedType;
        KnownType = source.KnownType;

        // Switch flags
        SwitchFlag = source.SwitchFlag;
        ForceSwitchFlag = source.ForceSwitchFlag;
        SkipBeforeSwitchOutEventFlag = source.SkipBeforeSwitchOutEventFlag;
        DraggedIn = source.DraggedIn;
        NewlySwitched = source.NewlySwitched;
        BeingCalledBack = source.BeingCalledBack;

        // Move history (Move refs are library objects, safe to share)
        LastMove = source.LastMove;
        LastMoveEncore = source.LastMoveEncore;
        LastMoveUsed = source.LastMoveUsed;
        LastMoveTargetLoc = source.LastMoveTargetLoc;
        MoveThisTurn = source.MoveThisTurn;
        MoveLastTurnResult = source.MoveLastTurnResult;
        MoveThisTurnResult = source.MoveThisTurnResult;
        StatsRaisedThisTurn = source.StatsRaisedThisTurn;
        StatsLoweredThisTurn = source.StatsLoweredThisTurn;

        // Damage tracking
        LastDamage = source.LastDamage;
        HurtThisTurn = source.HurtThisTurn;
        TimesAttacked = source.TimesAttacked;

        // Active state
        IsActive = source.IsActive;
        ActiveTurns = source.ActiveTurns;
        ActiveMoveActions = source.ActiveMoveActions;
        PreviouslySwitchedIn = source.PreviouslySwitchedIn;
        IsStarted = source.IsStarted;
        DuringMove = source.DuringMove;

        // Ability flags
        TruantTurn = source.TruantTurn;
        BondTriggered = source.BondTriggered;
        SwordBoost = source.SwordBoost;
        ShieldBoost = source.ShieldBoost;
        SyrupTriggered = source.SyrupTriggered;
        HeroMessageDisplayed = source.HeroMessageDisplayed;

        // Weight / Speed
        WeightKg = source.WeightKg;
        WeightHg = source.WeightHg;
        Speed = source.Speed;

        // Tera
        CanTerastallize = source.CanTerastallize;
        TeraType = source.TeraType;
        Terastallized = source.Terastallized;
        StellarBoostedTypes = new List<MoveType>(source.StellarBoostedTypes);

        // Mega Evolution
        CanMegaEvo = source.CanMegaEvo;

        // Staleness
        Staleness = source.Staleness;
        PendingStaleness = source.PendingStaleness;
        VolatileStaleness = source.VolatileStaleness;

        ShowCure = source.ShowCure;

        // Details (has mutable TeraType, so create a copy)
        Details = new PokemonDetails
        {
            Id = source.Details.Id,
            Level = source.Details.Level,
            Gender = source.Details.Gender,
            Shiny = source.Details.Shiny,
            TeraType = source.Details.TeraType,
        };

        // EffectStates — deep clone values, Pokemon refs are null (remapped in Pass 2)
        SpeciesState = source.SpeciesState.DeepClone();
        StatusState = source.StatusState.DeepClone();
        AbilityState = source.AbilityState.DeepClone();
        ItemState = source.ItemState.DeepClone();
        Volatiles = source.Volatiles.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.DeepClone());

        // Pokemon references — null/empty (remapped in Pass 2)
        Illusion = null;
        AttackedBy = [];
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
            string statusSuffix = string.Concat(" ", Status.ToString());
            secretStr = string.Concat(secretStr, statusSuffix);
            sharedStr = string.Concat(sharedStr, statusSuffix);
        }

        Secret secret = secretStr;
        Shared shared = sharedStr;

        return new SideSecretSharedResult(Side.Id, secret, shared)
        {
            HpColor = hpColor,
        };
    }
}