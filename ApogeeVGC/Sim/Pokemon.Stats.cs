using ApogeeVGC.Sim.Ui;

namespace ApogeeVGC.Sim;

public partial class Pokemon
{
    // Stat-related properties
    private StatsTable UnmodifiedStats { get; }
    public StatModifiers StatModifiers { get; private set; } = new();

    // Unmodified stat accessors
    public int UnmodifiedHp => UnmodifiedStats.Hp;
    public int UnmodifiedAtk => UnmodifiedStats.Atk;
    public int UnmodifiedDef => UnmodifiedStats.Def;
    public int UnmodifiedSpA => UnmodifiedStats.SpA;
    public int UnmodifiedSpD => UnmodifiedStats.SpD;
    public int UnmodifiedSpe => UnmodifiedStats.Spe;

    // Current stat accessors
    public int CurrentAtk => CalculateModifiedStat(StatId.Atk);
    public int CurrentDef => CalculateModifiedStat(StatId.Def);
    public int CurrentSpA => CalculateModifiedStat(StatId.SpA);
    public int CurrentSpD => CalculateModifiedStat(StatId.SpD);
    public int CurrentSpe => CalculateModifiedStat(StatId.Spe);

    // Critical hit stats
    private int CritAtk => StatModifiers.Atk < 0 ? UnmodifiedAtk : CurrentAtk;
    private int CritDef => StatModifiers.Def > 0 ? UnmodifiedDef : CurrentDef;
    private int CritSpA => StatModifiers.SpA < 0 ? UnmodifiedSpA : CurrentSpA;
    private int CritSpD => StatModifiers.SpD > 0 ? UnmodifiedSpD : CurrentSpD;

    // Type-related properties and methods
    public PokemonType[] DefensiveTypes
    {
        get
        {
            if (IsTeraUsed)
            {
                return TeraType == MoveType.Stellar ? Specie.Types.ToArray() : [TeraType.ConvertToPokemonType()];
            }
            return Specie.Types.ToArray();
        }
    }

    public (PokemonType, double)[] StabTypes
    {
        get
        {
            const double normalStab = 1.5;
            const double enhancedStab = 2.0;

            if (!IsTeraUsed)
            {
                return Specie.Types.Select(type => (type, NormalStab: normalStab)).ToArray();
            }

            var stabTypes = new List<(PokemonType, double)>();
            PokemonType teraType = TeraType.ConvertToPokemonType();

            if (Specie.Types.Contains(teraType))
            {
                stabTypes.AddRange(
                    Specie.Types.Select(type =>
                        (type, type == teraType ? enhancedStab : normalStab))
                );
            }
            else
            {
                stabTypes.Add((teraType, normalStab));
                stabTypes.AddRange(Specie.Types.Select(type => (type, NormalStab: normalStab)));
            }

            return stabTypes.ToArray();
        }
    }

    // Combat stat methods
    public int GetAttackStat(Move move, bool crit)
    {
        switch (move.Category)
        {
            case MoveCategory.Physical:
                return crit ? CritAtk : CurrentAtk;
            case MoveCategory.Special:
                return crit ? CritSpA : CurrentSpA;
            case MoveCategory.Status:
            default:
                throw new ArgumentException("Invalid move category.");
        }
    }

    public int GetDefenseStat(Move move, bool crit)
    {
        switch (move.Category)
        {
            case MoveCategory.Physical:
                return crit ? CritDef : CurrentDef;
            case MoveCategory.Special:
                return crit ? CritSpD : CurrentSpD;
            case MoveCategory.Status:
            default:
                throw new ArgumentException("Invalid move category.");
        }
    }

    // Type checking methods
    public bool IsStab(Move move)
    {
        PokemonType moveType = move.Type.ConvertToPokemonType();
        foreach ((PokemonType type, double _) in StabTypes)
        {
            if (type == moveType)
            {
                return true;
            }
        }
        return false;
    }

    public double GetStabMultiplier(Move move)
    {
        PokemonType moveType = move.Type.ConvertToPokemonType();
        foreach ((PokemonType type, double multiplier) in StabTypes)
        {
            if (type == moveType)
            {
                return multiplier;
            }
        }
        return 1.0;
    }

    public bool HasType(PokemonType type)
    {
        return Specie.Types.Contains(type);
    }

    public bool HasType(MoveType type)
    {
        return Specie.Types.Contains(type.ConvertToPokemonType());
    }

    public bool HasItem(ItemId item)
    {
        return Item != null && Item.Id == item;
    }

    public void AlterStatModifier(StatId stat, int change, BattleContext context)
    {
        switch (change)
        {
            case 0:
                throw new InvalidOperationException("Stat change cannot be zero.");
            case > 12 or < -12:
                throw new ArgumentOutOfRangeException(nameof(change), "Stat change must be between -12 and +12.");
        }

        int currentStage = stat switch
        {
            StatId.Atk => StatModifiers.Atk,
            StatId.Def => StatModifiers.Def,
            StatId.SpA => StatModifiers.SpA,
            StatId.SpD => StatModifiers.SpD,
            StatId.Spe => StatModifiers.Spe,
            StatId.Hp => throw new ArgumentException("Cannot modify HP stat stage."),
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
        };

        switch (currentStage)
        {
            case 6 when change > 0:
                {
                    // Already at max stage, cannot increase further
                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintStatModifierTooHigh(this, stat);
                    }
                    return;
                }
            case -6 when change < 0:
                {
                    // Already at min stage, cannot decrease further
                    if (context.PrintDebug)
                    {
                        UiGenerator.PrintStatModifierTooLow(this, stat);
                    }
                    return;
                }
        }


        int newStage = Math.Clamp(currentStage + change, -6, 6);

        int actualChange = newStage - currentStage;
        if (actualChange == 0)
        {
            // No change in stage, so no need to update
            return;
        }
        StatModifiers = stat switch
        {
            StatId.Atk => StatModifiers with { Atk = newStage },
            StatId.Def => StatModifiers with { Def = newStage },
            StatId.SpA => StatModifiers with { SpA = newStage },
            StatId.SpD => StatModifiers with { SpD = newStage },
            StatId.Spe => StatModifiers with { Spe = newStage },
            StatId.Hp => throw new ArgumentException("Cannot modify HP stat stage."),
            _ => throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID."),
        };

        // TODO: Trigger any on-stat-change effects from conditions, abilities, items, etc.

        if (context.PrintDebug)
        {
            UiGenerator.PrintStatModifierChange(this, stat, actualChange);
        }
    }

    public void SetStatModifier(StatId stat, int value, BattleContext context)
    {
        // TODO: Implement setting stat stage directly if needed
        // Would be used for belly drum, etc.
    }

    /// <summary>
    /// Gets the Pokemon's best stat.
    /// Used by Beast Boost, Quark Drive, and Protosynthesis.
    /// </summary>
    /// <param name="unboosted">If true, ignores stat boosts</param>
    /// <param name="unmodified">If true, uses base stats without modifications</param>
    /// <returns>The stat ID of the highest stat (excluding HP)</returns>
    public StatIdExceptHp GetBestStat(bool unboosted = false, bool unmodified = false)
    {
        var statName = StatIdExceptHp.Atk;
        int bestStat = 0;
        StatIdExceptHp[] stats = [StatIdExceptHp.Atk, StatIdExceptHp.Def, StatIdExceptHp.SpA,
            StatIdExceptHp.SpD, StatIdExceptHp.Spe];

        foreach (StatIdExceptHp stat in stats)
        {
            //int currentStat = GetStat(stat, unboosted, unmodified);
            // TODO: Implement GetStat method if needed. need to account for unboosted and unmodified flags
            int currentStat = CalculateUnmodifiedStat(stat.ConvertToStatId());
            if (currentStat <= bestStat) continue;
            statName = stat;
            bestStat = currentStat;
        }

        return statName;
    }

    //private int GetStat(StatIdExceptHp stat, bool unboosted = false, bool unmodified = false)
    //{
    //    // Implementation would depend on your specific requirements
    //    // This might involve checking the unboosted/unmodified flags
    //    // and returning either current stats, base stats, or unboosted stats
    //    throw new NotImplementedException("GetStat method needs to be implemented");
    //}

    private int CalculateModifiedStat(StatId stat)
    {
        if (stat == StatId.Hp)
        {
            return UnmodifiedStats.Hp; // HP is not modified by stat stages
        }

        // apply stat modifiers
        double statModifier;
        switch (stat)
        {
            case StatId.Atk:
                statModifier = StatModifiers.AtkMultiplier;
                break;
            case StatId.Def:
                statModifier = StatModifiers.DefMultiplier;
                break;
            case StatId.SpA:
                statModifier = StatModifiers.SpAMultiplier;
                break;
            case StatId.SpD:
                statModifier = StatModifiers.SpDMultiplier;
                break;
            case StatId.Spe:
                statModifier = StatModifiers.SpeMultiplier;
                break;
            case StatId.Hp:
            default:
                throw new ArgumentOutOfRangeException(nameof(stat), "Invalid stat ID.");
        }

        // apply condition effects here
        double conditionModifier = 1.0;
        conditionModifier = stat switch
        {
            StatId.Atk => Conditions.OrderBy(c => c.OnModifyAtkPriority ?? 0) // Sort by priority (default 0)
                .Aggregate(conditionModifier,
                    (current, condition) => current * (condition.OnModifyAtk?.Invoke(this) ?? 1.0)),
            StatId.Def => Conditions.OrderBy(c => c.OnModifyDefPriority ?? 0)
                .Aggregate(conditionModifier,
                    (current, condition) => current * (condition.OnModifyDef?.Invoke(this) ?? 1.0)),
            StatId.SpA => Conditions.OrderBy(c => c.OnModifySpAPriority ?? 0)
                .Aggregate(conditionModifier,
                    (current, condition) => current * (condition.OnModifySpA?.Invoke(this) ?? 1.0)),
            StatId.SpD => Conditions.OrderBy(c => c.OnModifySpDPriority ?? 0)
                .Aggregate(conditionModifier,
                    (current, condition) => current * (condition.OnModifySpD?.Invoke(this) ?? 1.0)),
            StatId.Spe => Conditions.OrderBy(c => c.OnModifySpePriority ?? 0)
                .Aggregate(conditionModifier,
                    (current, condition) => current * (condition.OnModifySpe?.Invoke(this) ?? 1.0)),
            _ => conditionModifier,
        };

        return (int)Math.Floor(UnmodifiedStats.GetStat(stat) * statModifier * conditionModifier);
    }

    private int CalculateUnmodifiedStat(StatId stat)
    {
        int baseStat = Specie.BaseStats.GetStat(stat);
        int iv = Ivs.GetStat(stat);
        int ev = Evs.GetStat(stat);

        if (stat == StatId.Hp)
        {

            return (int)Math.Floor((2 * baseStat + iv + Math.Floor(ev / 4.0)) * Level / 100.0) + Level + 10;
        }
        int preNature = (int)Math.Floor((2 * baseStat + iv + Math.Floor(ev / 4.0)) * Level / 100.0) + 5;
        double natureModifier = Nature.GetStatModifier(stat.ConvertToStatIdExceptId());
        return (int)Math.Floor(preNature * natureModifier);
    }

    private StatsTable CalculateUnmodifiedStats()
    {
        return new StatsTable
        {
            Hp = CalculateUnmodifiedStat(StatId.Hp),
            Atk = CalculateUnmodifiedStat(StatId.Atk),
            Def = CalculateUnmodifiedStat(StatId.Def),
            SpA = CalculateUnmodifiedStat(StatId.SpA),
            SpD = CalculateUnmodifiedStat(StatId.SpD),
            Spe = CalculateUnmodifiedStat(StatId.Spe)
        };
    }
}