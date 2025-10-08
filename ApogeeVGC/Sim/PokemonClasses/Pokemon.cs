using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.PokemonClasses;

public class Pokemon
{
    public Side Side { get; }
    public IBattle Battle => Side.Battle;
    public PokemonSet Set { get; }
    public string Name => Set.Name[..20];

    public string Fullname => $"{Side.Id.GetSideIdName()}: {Name}";
    public int Level => Set.Level;
    public GenderId Gender => Set.Gender;
    public int Happiness => Set.Happiness;
    public PokeballId Pokeball => Set.Pokeball;

    public IReadOnlyList<MoveSlot> BaseMoveSlots { get; }
    public StatsTable Evs => Set.Evs;
    public StatsTable Ivs => Set.Ivs;
    public List<MoveSlot> MoveSlots { get; set; }

    public PokemonSlotId Position { get; set; }

    public Species BaseSpecies { get; set; }
    public Species Species { get; set; }

    public EffectState SpeciesState { get; set; }

    public ConditionId? Status { get; set; }
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
    public bool? MoveLastTurnResult { get; set; }
    public bool? MoveThisTurnResult { get; set; }
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
    public bool ShieldBoost { get; init; } // Gen 9 only
    public bool SyrupTriggered { get; set; } // Gen 9 only
    public List<MoveType> StellarBoostedTypes { get; set; } // Gen 9 only

    public bool IsStarted { get; set; }
    public bool DuringMove { get; set; }

    public double WeightKg { get; set; }
    public int WeightHg { get; set; }
    public int Speed { get; set; }

    public MoveTypeFalseUnion? CanTerastallize { get; set; }
    public MoveType TeraType { get; set; }
    public List<PokemonType> BaseTypes { get; set; }
    public MoveType? Terastallized { get; set; }

    public Staleness? Staleness { get; set; }
    public Staleness? PendingStaleness { get; set; }

    public Staleness? VolatileStaleness
    {
        get;
        set // must be 'external' or null
        {
            if (value is PokemonClasses.Staleness.External or null)
            {
                field = value;
            }
            else
            {
                throw new ArgumentException("VolatileStaleness must be 'external' or null");
            }
        }
    }

    public Pokemon(IBattle battle, PokemonSet set, Side side)
    {
        Side = side;
        Set = set;
        BaseSpecies = battle.Library.Species[set.Species];
        Species = BaseSpecies;

        SpeciesState = battle.InitEffectState(Species.Id, null, null);

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

        Position = PokemonSlotId.Slot1;
        StatusState = battle.InitEffectState(null, null, null);
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

        CanTerastallize = BattleActions.CanTerastallize(battle, this);

        ClearVolatile();
        Hp = MaxHp;
    }


    public void ClearVolatile(bool includeSwitchFlags = true)
    {
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

        MoveSlots = BaseMoveSlots.ToList();

        Transformed = false;
        Ability = BaseAbility;
        if (CanTerastallize is FalseMoveTypeFalseUnion)
        {
            CanTerastallize = TeraType;
        }

        var volatileKeys = Volatiles.Keys.ToList();
        foreach (ConditionId conditionId in volatileKeys)
        {
            if (Volatiles.TryGetValue(conditionId, out EffectState? effectState) &&
                effectState.LinkedStatus is not null)
            {
                RemoveLinkedVolatiles(effectState.LinkedStatus, effectState.LinkedPokemon ?? []);
            }
        }

        Volatiles.Clear();

        if (includeSwitchFlags)
        {
            SwitchFlag = false;
            ForceSwitchFlag = false;
        }

        LastMove = null;
        LastMoveUsed = null;
        MoveThisTurn = MoveId.None;
        MoveLastTurnResult = null;
        MoveThisTurnResult = null;

        LastDamage = 0;
        AttackedBy.Clear();
        HurtThisTurn = null;
        NewlySwitched = true;
        BeingCalledBack = false;

        VolatileStaleness = null;

        AbilityState.Started = null;
        ItemState.Started = null;

        SetSpecie(BaseSpecies, Battle.Effect);
    }

    public void RemoveLinkedVolatiles(IEffect linkedStatus, Pokemon[] linkedPokemon)
    {
        throw new NotImplementedException();
    }

    public bool TrySetStatus(Condition status, Pokemon? source, IEffect? sourceEffect)
    {
        throw new NotImplementedException();
    }

    public RelayVar AddVolatile(IBattle battle, Condition status, Pokemon? source = null,
        IEffect? sourceEffect = null, Condition? linkedStatus = null)
    {
        throw new NotImplementedException();
    }

    public EffectState? GetVolatile(ConditionId volatileId)
    {
        return Volatiles.GetValueOrDefault(volatileId);
    }

    public bool RemoveVolatile(IBattle battle, Condition status)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Deletes a volatile condition without running the extra logic from RemoveVolatile
    /// </summary>
    public bool DeleteVolatile(ConditionId volatileId)
    {
        return Volatiles.Remove(volatileId);
    }

    public bool IgnoringAbility(IBattle battle)
    {
        throw new NotImplementedException();
    }

    public StatIdExceptHp GetBestStat(bool? unboosted, bool? unmodified)
    {
        throw new NotImplementedException();
    }

    public bool HasAbility(AbilityId ability)
    {
        throw new NotImplementedException();
    }

    public bool HasAbility(AbilityId[] abilities)
    {
        throw new NotImplementedException();
    }

    public bool CureStatus(bool silent = false)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Unlike CureStatus, this does not give any cure messages.
    /// </summary>
    public bool ClearStatus()
    {
        throw new NotImplementedException();
    }

    public bool FormeChange(SpecieId specieId, IEffect? source, bool? isPermanent, int abilitySlot = 0,
        string? message = null)
    {
        throw new NotImplementedException();
    }

    public bool HasType(PokemonType type)
    {
        throw new NotImplementedException();
    }

    public bool HasType(PokemonType[] types)
    {
        throw new NotImplementedException();
    }

    public bool SetType(PokemonType type, bool enforce = false)
    {
        throw new NotImplementedException();
    }

    public bool SetType(PokemonType[] types, bool enforce = false)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Default is Battle.Effect for source.
    /// </summary>
    public Species? SetSpecie(Species rawSpecies, IEffect? source, bool isTransform = false)
    {
        RelayVar? rv = Battle.RunEvent(EventId.ModifySpecie, this, null, source, rawSpecies);
        if (rv is null) return null;

        if (rv is SpecieRelayVar srv)
        {
            Species = srv.Species;
        }
        else
        {
            throw new InvalidOperationException("species must be a SpecieRelayVar" );
        }
        Species species = srv.Species;

        SetType(species.Types.ToArray(), true);
        ApparentType = rawSpecies.Types.ToList();
        AddedType = species.AddedType;
        KnownType = true;
        WeightHg = species.WeightHg;

        StatsTable stats = Battle.SpreadModify(Species.BaseStats, Set);
        if (Species.MaxHp is not null)
        {
            stats.Hp = Species.MaxHp.Value;
        }

        if (MaxHp != 0)
        {
            BaseMaxHp = stats.Hp;
            MaxHp = stats.Hp;
            Hp = stats.Hp;
        }

        if (!isTransform) BaseStoredStats = stats;
        foreach (var statName in StoredStats)
        {
            StoredStats[statName.Key] = stats[statName.Key.ConvertToStatId()];
        }

        Speed = StoredStats.Spe;
        return species;
    }

    public Item? GetItem()
    {
        throw new NotImplementedException();
    }

    public bool IgnoringItem(bool isFling = false)
    {
        throw new NotImplementedException();
    }

    public bool HasMove(MoveId move)
    {
        throw new NotImplementedException();
    }

    public void DisableMove(MoveId moveId, bool? isHidden = null, IEffect? sourceEffect = null)
    {
        throw new NotImplementedException();
    }

    public bool HasItem(ItemId item)
    {
        throw new NotImplementedException();
    }

    public MoveHitData GetMoveHitData(ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public bool IsGrounded(bool negateImmunity = false)
    {
        throw new NotImplementedException();
    }

    public bool IsSemiInvulnerable()
    {
        throw new NotImplementedException();
    }

    public double RunEffectiveness(ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public int GetWeight()
    {
        throw new NotImplementedException();
    }

    public Pokemon Copy()
    {
        throw new NotImplementedException();
    }
}


public record MoveHitData
{
    public bool Crit { get; init; }
    public double TypeMod { get; init; }
}

//public class Pokemon
//{
//    // Core properties
//    public Species Species { get; init; }
//    public required IReadOnlyList<Move> Moves
//    {
//        get;
//        init
//        {
//            if (value == null || value.Count == 0)
//            {
//                throw new ArgumentException("Pokemon must have at least one move.");
//            }
//            if (value.Count > 4)
//            {
//                throw new ArgumentException("Pokemon cannot have more than 4 moves.");
//            }
//            field = value;
//        }
//    }
//    public Item? Item { get; init; }
//    public required Ability Ability { get; set; }
//    //public Ability BaseAbility { get; }
//    //public EffectState 
//    public StatsTable Evs { get; init; }
//    public Nature Nature { get; init; }
//    public StatsTable Ivs { get; init; }
//    public required string Name { get; init; }
//    public int Level
//    {
//        get;
//        init
//        {
//            if (value is < 1 or > 100)
//            {
//                throw new ArgumentOutOfRangeException(nameof(value), "Level must be between 1 and 100.");
//            }
//            field = value;
//        }
//    }
//    public bool Shiny { get; init; }
//    public MoveType TeraType { get; init; }
//    public GenderId Gender { get; init; }
//    public bool PrintDebug { get; init; }
//    public Trainer Trainer { get; init; }
//    public SideId SideId { get; init; }
//    public PokemonSlotId PokemonSlotId { get; set; }

//    // Stat-related properties
//    private StatsTable UnmodifiedStats { get; }
//    public StatModifiers StatModifiers { get; private set; } = new();

//    // Battle state properties
//    public Dictionary<ConditionId, EffectState> Volatiles { get; } = [];
//    public bool IsTeraUsed { get; private set; }
//    public bool IgnoringItem => false;
//    public Move? LastMoveUsed { get; set; }
//    public StatIdExceptHp? BestStat { get; set; }
//    public int ActiveMoveActions { get; set; }

//    // HP and health management
//    public int CurrentHp { get; private set; }
//    public double CurrentHpRatio => (double)CurrentHp / UnmodifiedHp;
//    public int CurrentHpPercentage => (int)Math.Ceiling(CurrentHpRatio * 100);
//    public bool IsFainted => CurrentHp <= 0;

//    public bool HasStatus => throw new NotImplementedException();

//    // Unmodified stat accessors
//    public int UnmodifiedHp => UnmodifiedStats.Hp;
//    public int UnmodifiedAtk => UnmodifiedStats.Atk;
//    public int UnmodifiedDef => UnmodifiedStats.Def;
//    public int UnmodifiedSpA => UnmodifiedStats.SpA;
//    public int UnmodifiedSpD => UnmodifiedStats.SpD;
//    public int UnmodifiedSpe => UnmodifiedStats.Spe;

//    // Current stat accessors
//    public int CurrentAtk => CalculateModifiedStat(StatId.Atk);
//    public int CurrentDef => CalculateModifiedStat(StatId.Def);
//    public int CurrentSpA => CalculateModifiedStat(StatId.SpA);
//    public int CurrentSpD => CalculateModifiedStat(StatId.SpD);
//    public int CurrentSpe => CalculateModifiedStat(StatId.Spe);

//    // Critical hit stats
//    private int CritAtk => StatModifiers.Atk < 0 ? UnmodifiedAtk : CurrentAtk;
//    private int CritDef => StatModifiers.Def > 0 ? UnmodifiedDef : CurrentDef;
//    private int CritSpA => StatModifiers.SpA < 0 ? UnmodifiedSpA : CurrentSpA;
//    private int CritSpD => StatModifiers.SpD > 0 ? UnmodifiedSpD : CurrentSpD;

//    // Type-related properties and methods
//    public PokemonType[] DefensiveTypes
//    {
//        get
//        {
//            if (IsTeraUsed)
//            {
//                return TeraType == MoveType.Stellar ? Species.Types.ToArray() : [TeraType.ConvertToPokemonType()];
//            }
//            return Species.Types.ToArray();
//        }
//    }

//    public (PokemonType, double)[] StabTypes
//    {
//        get
//        {
//            const double normalStab = 1.5;
//            const double enhancedStab = 2.0;

//            if (!IsTeraUsed)
//            {
//                return Species.Types.Select(type => (type, NormalStab: normalStab)).ToArray();
//            }

//            var stabTypes = new List<(PokemonType, double)>();
//            PokemonType teraType = TeraType.ConvertToPokemonType();

//            if (Species.Types.Contains(teraType))
//            {
//                stabTypes.AddRange(
//                    Species.Types.Select(type =>
//                        (type, type == teraType ? enhancedStab : normalStab))
//                );
//            }
//            else
//            {
//                stabTypes.Add((teraType, normalStab));
//                stabTypes.AddRange(Species.Types.Select(type => (type, NormalStab: normalStab)));
//            }

//            return stabTypes.ToArray();
//        }
//    }

//    // Core constructor
//    public Pokemon(Species species, StatsTable evs, StatsTable ivs, Nature nature, int level, Trainer trainer,
//        SideId sideId)
//    {
//        Species = species;
//        Evs = evs;
//        Ivs = ivs;
//        Nature = nature;
//        Level = level;
//        UnmodifiedStats = CalculateUnmodifiedStats();
//        CurrentHp = UnmodifiedStats.Hp;
//        Trainer = trainer;
//        SideId = sideId;
//    }

//    // HP modification methods
//    public int Heal(int amount)
//    {
//        int previousHp = CurrentHp;
//        CurrentHp = Math.Min(CurrentHp + amount, UnmodifiedStats.Hp);
//        return CurrentHp - previousHp;
//    }

//    public int Damage(int amount)
//    {
//        int previousHp = CurrentHp;
//        CurrentHp = Math.Max(CurrentHp - amount, 0);
//        return previousHp - CurrentHp;
//    }

//    // Combat stat methods
//    public int GetAttackStat(Move move, bool crit)
//    {
//        switch (move.Category)
//        {
//            case MoveCategory.Physical:
//                return crit ? CritAtk : CurrentAtk;
//            case MoveCategory.Special:
//                return crit ? CritSpA : CurrentSpA;
//            case MoveCategory.Status:
//            default:
//                throw new ArgumentException("Invalid move category.");
//        }
//    }

//    public int GetDefenseStat(Move move, bool crit)
//    {
//        switch (move.Category)
//        {
//            case MoveCategory.Physical:
//                return crit ? CritDef : CurrentDef;
//            case MoveCategory.Special:
//                return crit ? CritSpD : CurrentSpD;
//            case MoveCategory.Status:
//            default:
//                throw new ArgumentException("Invalid move category.");
//        }
//    }

//    // Type checking methods
//    public bool IsStab(Move move)
//    {
//        PokemonType moveType = move.Type.ConvertToPokemonType();
//        foreach ((PokemonType type, double _) in StabTypes)
//        {
//            if (type == moveType)
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    public double GetStabMultiplier(Move move)
//    {
//        PokemonType moveType = move.Type.ConvertToPokemonType();
//        foreach ((PokemonType type, double multiplier) in StabTypes)
//        {
//            if (type == moveType)
//            {
//                return multiplier;
//            }
//        }
//        return 1.0;
//    }

//    public bool HasType(PokemonType type)
//    {
//        return Species.Types.Contains(type);
//    }

//    public bool HasType(MoveType type)
//    {
//        return Species.Types.Contains(type.ConvertToPokemonType());
//    }

//    public bool HasItem(ItemId item)
//    {
//        return Item != null && Item.Id == item;
//    }

//    public void AlterStatModifier(StatId stat, int change, IBattle context)
//    {
//        switch (change)
//        {
//            case 0:
//                throw new InvalidOperationException("Stat change cannot be zero.");
//            case > 12 or < -12:
//                throw new ArgumentOutOfRangeException(nameof(change), "Stat change must be between -12 and +12.");
//        }

//        int currentStage = stat switch
//        {
//            StatId.Atk => StatModifiers.Atk,
//            StatId.Def => StatModifiers.Def,
//            StatId.SpA => StatModifiers.SpA,
//            StatId.SpD => StatModifiers.SpD,
//            StatId.Spe => StatModifiers.Spe,
//            StatId.Hp => throw new ArgumentException("Cannot modify HP stat stage."),
//            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
//        };

//        switch (currentStage)
//        {
//            case 6 when change > 0:
//                {
//                    // Already at max stage, cannot increase further
//                    if (context.PrintDebug)
//                    {
//                        UiGenerator.PrintStatModifierTooHigh(this, stat);
//                    }
//                    return;
//                }
//            case -6 when change < 0:
//                {
//                    // Already at min stage, cannot decrease further
//                    if (context.PrintDebug)
//                    {
//                        UiGenerator.PrintStatModifierTooLow(this, stat);
//                    }
//                    return;
//                }
//        }


//        int newStage = Math.Clamp(currentStage + change, -6, 6);

//        int actualChange = newStage - currentStage;
//        if (actualChange == 0)
//        {
//            // No change in stage, so no need to update
//            return;
//        }

//        switch (stat)
//        {
//            case StatId.Atk:
//                StatModifiers.Atk = newStage;
//                break;
//            case StatId.Def:
//                StatModifiers.Def = newStage;
//                break;
//            case StatId.SpA:
//                StatModifiers.SpA = newStage;
//                break;
//            case StatId.SpD:
//                StatModifiers.SpD = newStage;
//                break;
//            case StatId.Spe:
//                StatModifiers.Spe = newStage;
//                break;
//            case StatId.Hp:
//                throw new ArgumentException("Cannot modify HP stat stage.");
//            default:
//                throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.");
//        }

//        // TODO: Trigger any on-stat-change effects from conditions, abilities, items, etc.

//        if (context.PrintDebug)
//        {
//            UiGenerator.PrintStatModifierChange(this, stat, actualChange);
//        }
//    }

//    /// <summary>
//    /// Gets the Pokemon's best stat.
//    /// Used by Beast Boost, Quark Drive, and Protosynthesis.
//    /// </summary>
//    /// <param name="unboosted">If true, ignores stat boosts</param>
//    /// <param name="unmodified">If true, uses base stats without modifications</param>
//    /// <returns>The stat ID of the highest stat (excluding HP)</returns>
//    public StatIdExceptHp GetBestStat(bool unboosted = false, bool unmodified = false)
//    {
//        var statName = StatIdExceptHp.Atk;
//        int bestStat = 0;
//        StatIdExceptHp[] stats = [StatIdExceptHp.Atk, StatIdExceptHp.Def, StatIdExceptHp.SpA,
//            StatIdExceptHp.SpD, StatIdExceptHp.Spe];

//        foreach (StatIdExceptHp stat in stats)
//        {
//            //int currentStat = GetStat(stat, unboosted, unmodified);
//            // TODO: Implement GetStat method if needed. need to account for unboosted and unmodified flags
//            int currentStat = CalculateUnmodifiedStat(stat.ConvertToStatId());
//            if (currentStat <= bestStat) continue;
//            statName = stat;
//            bestStat = currentStat;
//        }

//        return statName;
//    }

//    //private int GetStat(StatIdExceptHp stat, bool unboosted = false, bool unmodified = false)
//    //{
//    //    // Implementation would depend on your specific requirements
//    //    // This might involve checking the unboosted/unmodified flags
//    //    // and returning either current stats, base stats, or unboosted stats
//    //    throw new NotImplementedException("GetStat method needs to be implemented");
//    //}

//    // Condition management
//    public bool TrySetStatus(Condition status, Pokemon? source, IEffect? sourceEffect)
//    {
//        throw new NotImplementedException();
//    }

//    public RelayVar AddVolatile(IBattle battle, Condition status, Pokemon? source = null,
//        IEffect? sourceEffect = null, Condition? linkedStatus = null)
//    {
//        throw new NotImplementedException();
//    }

//    public EffectState? GetVolatile(ConditionId volatileId)
//    {
//        return Volatiles.GetValueOrDefault(volatileId);
//    }

//    public bool RemoveVolatile(IBattle battle, IEffect status)
//    {
//        throw new NotImplementedException();
//    }

//    /// <summary>
//    /// Deletes a volatile condition without running the extra logic from RemoveVolatile
//    /// </summary>
//    public bool DeleteVolatile(ConditionId volatileId)
//    {
//        return Volatiles.Remove(volatileId);
//    }

//    public bool IgnoringAbility(IBattle battle)
//    {
//        throw new NotImplementedException();
//    }

//    public Pokemon Copy()
//    {
//        Pokemon copy = new(Species, Evs, Ivs, Nature, Level, Trainer, SideId)
//        {
//            Moves = Moves.Select(m => m.Copy()).ToArray(),
//            Item = Item,
//            Ability = Ability,
//            Name = Name,
//            Shiny = Shiny,
//            TeraType = TeraType,
//            Gender = Gender,
//            PrintDebug = PrintDebug,
//            PokemonSlotId = PokemonSlotId,
//        };

//        int hpDifference = UnmodifiedHp - CurrentHp;
//        if (hpDifference > 0)
//        {
//            copy.Damage(hpDifference);
//        }

//        copy.StatModifiers = StatModifiers.Copy();
//        copy.LastMoveUsed = LastMoveUsed;
//        copy.ActiveMoveActions = ActiveMoveActions;
//        copy.IsTeraUsed = IsTeraUsed;
//        copy.BestStat = BestStat;

//        return copy;
//    }

//    private int CalculateModifiedStat(StatId stat)
//    {
//        if (stat == StatId.Hp)
//        {
//            return UnmodifiedStats.Hp; // HP is not modified by stat stages
//        }

//        // apply stat modifiers
//        double statModifier;
//        switch (stat)
//        {
//            case StatId.Atk:
//                statModifier = StatModifiers.AtkMultiplier;
//                break;
//            case StatId.Def:
//                statModifier = StatModifiers.DefMultiplier;
//                break;
//            case StatId.SpA:
//                statModifier = StatModifiers.SpAMultiplier;
//                break;
//            case StatId.SpD:
//                statModifier = StatModifiers.SpDMultiplier;
//                break;
//            case StatId.Spe:
//                statModifier = StatModifiers.SpeMultiplier;
//                break;
//            case StatId.Hp:
//            default:
//                throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.");
//        }

//        // apply condition effects here
//        double conditionModifier = 1.0;

//        return (int)Math.Floor(UnmodifiedStats.GetStat(stat) * statModifier * conditionModifier);
//    }

//    private int CalculateUnmodifiedStat(StatId stat)
//    {
//        int baseStat = Species.BaseStats.GetStat(stat);
//        int iv = Ivs.GetStat(stat);
//        int ev = Evs.GetStat(stat);

//        if (stat == StatId.Hp)
//        {

//            return (int)Math.Floor((2 * baseStat + iv + Math.Floor(ev / 4.0)) * Level / 100.0) + Level + 10;
//        }
//        int preNature = (int)Math.Floor((2 * baseStat + iv + Math.Floor(ev / 4.0)) * Level / 100.0) + 5;
//        double natureModifier = Nature.GetStatModifier(stat.ConvertToStatIdExceptId());
//        return (int)Math.Floor(preNature * natureModifier);
//    }

//    private StatsTable CalculateUnmodifiedStats()
//    {
//        return new StatsTable
//        {
//            Hp = CalculateUnmodifiedStat(StatId.Hp),
//            Atk = CalculateUnmodifiedStat(StatId.Atk),
//            Def = CalculateUnmodifiedStat(StatId.Def),
//            SpA = CalculateUnmodifiedStat(StatId.SpA),
//            SpD = CalculateUnmodifiedStat(StatId.SpD),
//            Spe = CalculateUnmodifiedStat(StatId.Spe)
//        };
//    }

//    //public override string ToString()
//    //{
//    //    string line1 = $"{Name} ({Species.Name}) - Lv. {Level}";
//    //    string line2 = $"{CurrentHp}/{UnmodifiedStats.Hp} HP";
//    //    string line3 = $"Ability: {Ability.Name} - Item: {Item.Name}";
//    //    string movesLine = string.Join(", ", Moves.Select(m => m.Name));
//    //    return $"{line1}\n{line2}\n{line3}\nMoves: {movesLine}\n";
//    //}
//}