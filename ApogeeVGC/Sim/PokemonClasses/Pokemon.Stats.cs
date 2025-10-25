using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.PokemonClasses;

public partial class Pokemon
{
    public void UpdateSpeed()
    {
        Speed = GetActionSpeed();
    }

    /// <summary>
    /// Calculates a Pokemon's stat value with boosts and modifiers applied.
    /// This is used during battle for damage calculations and speed comparisons.
    /// </summary>
    /// <param name="statName">The stat to calculate (Atk, Def, SpA, SpD, Spe)</param>
    /// <param name="boost">Boost stage from -6 to +6</param>
    /// <param name="modifier">Optional modifier to apply (e.g., 1.5 for 50% increase)</param>
    /// <param name="statUser">Pokemon whose stats are being calculated (defaults to this)</param>
    /// <returns>The calculated stat value</returns>
    public int CalculateStat(StatIdExceptHp statName, int boost, int? modifier = null, Pokemon? statUser = null)
    {
        // Get base stat from stored stats
        int stat = StoredStats[statName];

        // Wonder Room swaps Defense and Special Defense BEFORE any other calculations
        if (Battle.Field.PseudoWeather.ContainsKey(ConditionId.WonderRoom))
        {
            stat = statName switch
            {
                StatIdExceptHp.Def => StoredStats[StatIdExceptHp.SpD],
                StatIdExceptHp.SpD => StoredStats[StatIdExceptHp.Def],
                _ => stat,
            };
        }

        // Create sparse boosts table with only the stat we're calculating
        var boosts = new SparseBoostsTable();
        BoostId boostName = statName.ConvertToBoostId();
        boosts.SetBoost(boostName, boost);

        // Run ModifyBoost event to allow abilities/items/conditions to modify boosts
        // Example: Simple ability doubles boost stages
        RelayVar? boostEvent = Battle.RunEvent(EventId.ModifyBoost, statUser ?? this, null,
            null, boosts);

        if (boostEvent is SparseBoostsTableRelayVar brv)
        {
            boosts = brv.Table;
            boost = boosts.GetBoost(boostName) ?? 0;
        }

        // Clamp boost to valid range
        boost = BoostsTable.ClampBoost(boost);

        // Apply boost multiplier
        if (boost >= 0)
        {
            // Positive boosts multiply the stat
            stat = (int)Math.Floor(stat * BoostsTable.CalculateRegularStatMultiplier(boost));
        }
        else
        {
            // Negative boosts divide the stat
            stat = (int)Math.Floor(stat / BoostsTable.CalculateRegularStatMultiplier(-boost));
        }

        // Apply modifier using battle's modify function
        return Battle.Modify(stat, modifier ?? 1);
    }

    public int GetStat(StatIdExceptHp statName, bool unboosted = false, bool unmodified = false)
    {
        int stat = StoredStats[statName];

        // Wonder Room swaps Def and SpD
        if (unmodified && Battle.Field.PseudoWeather.ContainsKey(ConditionId.WonderRoom))
        {
            statName = statName switch
            {
                StatIdExceptHp.Def => StatIdExceptHp.SpD,
                StatIdExceptHp.SpD => StatIdExceptHp.Def,
                _ => statName,
            };
        }

        // Stat boosts
        if (!unboosted)
        {
            BoostsTable boosts = Boosts;
            if (!unmodified)
            {
                RelayVar? relayVar = Battle.RunEvent(EventId.ModifyBoost, this, null,
                    null, boosts);

                if (relayVar is BoostsTableRelayVar brv)
                {
                    boosts = brv.Table;
                }
                else
                {
                    throw new InvalidOperationException("boosts must be a BoostsTableRelayVar");
                }
            }
            stat = (int)Math.Floor(stat * boosts.GetBoostMultiplier(statName.ConvertToBoostId()));
        }

        // Stat modifier effects
        if (!unmodified)
        {
            EventId eventId = statName switch
            {
                StatIdExceptHp.Atk => EventId.ModifyAtk,
                StatIdExceptHp.Def => EventId.ModifyDef,
                StatIdExceptHp.SpA => EventId.ModifySpA,
                StatIdExceptHp.SpD => EventId.ModifySpD,
                StatIdExceptHp.Spe => EventId.ModifySpe,
                _ => throw new ArgumentOutOfRangeException(nameof(statName), "Invalid stat name."),
            };
            RelayVar? relayVar = Battle.RunEvent(eventId, this, null, null, stat);
            if (relayVar is IntRelayVar irv)
            {
                stat = irv.Value;
            }
            else
            {
                throw new InvalidOperationException("stat must be an IntRelayVar");
            }
        }

        if (statName == StatIdExceptHp.Spe && stat > 10000) stat = 10000;
        return stat;
    }

    public int GetActionSpeed()
    {
        int speed = GetStat(StatIdExceptHp.Spe);
        if (Battle.Field.GetPseudoWeather(ConditionId.TrickRoom) is not null)
        {
            speed = TrickRoomSpeedOffset - speed;
        }
        return Battle.Trunc(speed, 13);
    }

    public StatIdExceptHp GetBestStat(bool unboosted = false, bool unmodified = false)
    {
        int bestStatValue = int.MinValue;
        var bestStatName = StatIdExceptHp.Atk;

        // Iterate through all stat types except HP
        foreach (StatIdExceptHp statId in Enum.GetValues<StatIdExceptHp>())
        {
            int currentStatValue = GetStat(statId, unboosted, unmodified);
            if (currentStatValue <= bestStatValue) continue;
            bestStatValue = currentStatValue;
            bestStatName = statId;
        }
        return bestStatName;
    }

    /// <summary>
    /// Gets the Pokemon's current weight in hectograms, accounting for modifying effects.
    /// Weight is used for moves like Heavy Slam, Grass Knot, and Low Kick.
    /// Minimum weight is 1 hectogram (0.1 kg).
    /// </summary>
    /// <returns>Weight in hectograms (1 hg = 0.1 kg)</returns>
    public int GetWeight()
    {
        // Run ModifyWeight event to allow abilities/items/conditions to modify weight
        RelayVar? weightEvent = Battle.RunEvent(EventId.ModifyWeight, this, null, null,
            WeightHg);

        int modifiedWeight;
        if (weightEvent is IntRelayVar irv)
        {
            modifiedWeight = irv.Value;
        }
        else
        {
            // Fallback to base weight if event doesn't return an integer
            modifiedWeight = WeightHg;
        }

        // Ensure minimum weight of 1 hectogram
        return Math.Max(1, modifiedWeight);
    }

    public void UpdateMaxHp()
    {
        int newBaseMaxHp = Battle.StatModify(Species.BaseStats, Set, StatId.Hp);
        if (newBaseMaxHp == BaseMaxHp) return;
        BaseMaxHp = newBaseMaxHp;
        int newMaxHp = BaseMaxHp;
        Hp = Hp <= 0 ? 0 : Math.Max(1, newMaxHp - (MaxHp - Hp));
        MaxHp = newMaxHp;
        if (Hp > 0)
        {
            //UiGenerator.PrintHealEvent(this, Hp.ToString());
        }
    }

    /// <summary>
    /// Calculates the sum of all positive stat boosts on this Pokemon.
    /// Used for moves like Punishment, Stored Power, and Power Trip.
    /// </summary>
    /// <returns>The total of all positive boost stages</returns>
    public int PositiveBoosts()
    {
        // Iterate through all boost types
        return Enum.GetValues<BoostId>().Select(boostId => Boosts.GetBoost(boostId)).
            Where(boostValue => boostValue > 0).Sum();
    }

    public SparseBoostsTable GetCappedBoost(SparseBoostsTable boosts)
    {
        SparseBoostsTable cappedBoost = new();
        foreach ((BoostId boostId, int value) in boosts.GetNonNullBoosts())
        {
            // Get current boost value for this stat
            int currentBoost = Boosts.GetBoost(boostId);

            // Calculate capped boost: clamp(current + incoming) - current
            // This gives us the actual amount we can boost (respecting -6 to +6 limits)
            int cappedValue = Battle.ClampIntRange(currentBoost + value, -6, 6) - currentBoost;
            cappedBoost.SetBoost(boostId, cappedValue);
        }
        return cappedBoost;
    }

    /// <summary>
    /// Applies stat boosts to this Pokemon, respecting the -6 to +6 boost limits.
    /// Returns the delta (change amount) of the last boost that was applied.
    /// </summary>
    /// <param name="boosts">The boosts to apply (will be capped to valid ranges)</param>
    /// <returns>The change amount of the last stat that was boosted (0 if no boosts were applied)</returns>
    public int BoostBy(SparseBoostsTable boosts)
    {
        // Cap all boosts to respect -6 to +6 limits
        boosts = GetCappedBoost(boosts);

        int delta = 0;

        // Apply each boost to the Pokemon's current boosts
        foreach ((BoostId boostId, int value) in boosts.GetNonNullBoosts())
        {
            delta = value;
            Boosts.SetBoost(boostId, Boosts.GetBoost(boostId) + delta);
        }

        // Return the last delta (change amount of the last stat modified)
        return delta;
    }

    public void ClearBoosts()
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
    }

    public void SetBoost(SparseBoostsTable boosts)
    {
        // Iterate through all non-null boosts in the sparse table and set them
        foreach ((BoostId boostId, int value) in boosts.GetNonNullBoosts())
        {
            Boosts.SetBoost(boostId, value);
        }
    }
}