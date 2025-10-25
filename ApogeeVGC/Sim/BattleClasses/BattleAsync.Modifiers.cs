using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    /// <summary>
    /// Chains two damage modifiers together using fixed-point arithmetic.
    /// This is used for combining multiple modifiers (STAB, type effectiveness, abilities, etc.)
    /// in the damage calculation formula.
    /// </summary>
    /// <param name="previousMod">Previous modifier (number or [numerator, denominator])</param>
    /// <param name="nextMod">Next modifier to chain (number or [numerator, denominator])</param>
    /// <returns>The combined modifier as a decimal value</returns>
    public double Chain(double previousMod, double nextMod)
    {
        // Convert to fixed-point (4096-based)
        int prevFixed = Trunc((int)(previousMod * 4096));
        int nextFixed = Trunc((int)(nextMod * 4096));

        // Apply chaining formula: M'' = ((M * M') + 0x800) >> 12
        // The 0x800 (2048) is added for proper rounding
        return ((prevFixed * nextFixed + 2048) >> 12) / 4096.0;
    }

    public double Chain(int[] previousMod, double nextMod)
    {
        if (previousMod.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]",
                nameof(previousMod));
        }

        // Convert fraction to fixed-point
        int prevFixed = Trunc(previousMod[0] * 4096 / previousMod[1]);
        int nextFixed = Trunc((int)(nextMod * 4096));

        return ((prevFixed * nextFixed + 2048) >> 12) / 4096.0;
    }

    public double Chain(double previousMod, int[] nextMod)
    {
        if (nextMod.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]",
                nameof(nextMod));
        }

        int prevFixed = Trunc((int)(previousMod * 4096));
        int nextFixed = Trunc(nextMod[0] * 4096 / nextMod[1]);

        return ((prevFixed * nextFixed + 2048) >> 12) / 4096.0;
    }

    public double Chain(int[] previousMod, int[] nextMod)
    {
        if (previousMod.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]",
                nameof(previousMod));
        }
        if (nextMod.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]",
                nameof(nextMod));
        }

        int prevFixed = Trunc(previousMod[0] * 4096 / previousMod[1]);
        int nextFixed = Trunc(nextMod[0] * 4096 / nextMod[1]);

        return ((prevFixed * nextFixed + 2048) >> 12) / 4096.0;
    }

    public double ChainModify(int numerator, int denominator = 1)
    {
        // Get the current modifier from the event state as fixed-point
        // Default to 1.0 (4096 in fixed-point) if null
        int previousMod = Trunc((int)((Event.Modifier ?? 1.0) * 4096));

        // Convert the new modifier to fixed-point format
        int nextMod = Trunc(numerator * 4096 / denominator);

        // Chain the modifiers together and store back in the event
        // The >> 12 is a right shift by 12 bits (equivalent to dividing by 4096)
        // Add 2048 for proper rounding before the shift
        Event.Modifier = ((previousMod * nextMod + 2048) >> 12) / 4096.0;
        return nextMod;
    }

    public double ChainModify(int[] fraction)
    {
        if (fraction.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]", nameof(fraction));
        }

        return ChainModify(fraction[0], fraction[1]);
    }

    public double ChainModify(double fraction)
    {
        if (fraction <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(fraction), "Fraction must be greater than 0.");
        }
        // Convert the double fraction to a fixed-point representation
        int fixedPointFraction = Trunc((int)(fraction * 4096));
        // Chain the fixed-point modification
        int previousMod = Trunc((int)((Event.Modifier ?? 1.0) * 4096));
        Event.Modifier = ((previousMod * fixedPointFraction + 2048) >> 12) / 4096.0;
        return fixedPointFraction;
    }

    public int Modify(int value, int numerator, int denominator = 1)
    {
        // Calculate the 4096-based fixed-point modifier
        int modifier = Trunc(numerator * 4096 / denominator);

        // Apply the modifier with proper rounding
        return Trunc((Trunc(value * modifier) + 2048 - 1) / 4096);
    }

    public int Modify(int value, int[] fraction)
    {
        if (fraction.Length != 2)
        {
            throw new ArgumentException("Fraction array must have exactly 2 elements [numerator, denominator]",
                nameof(fraction));
        }

        return Modify(value, fraction[0], fraction[1]);
    }

    public int Modify(int value, double fraction)
    {
        if (fraction <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(fraction), "Fraction must be greater than 0.");
        }
        // Convert the double fraction to a fixed-point representation
        int fixedPointFraction = Trunc((int)(fraction * 4096));
        // Apply the fixed-point modification
        return Trunc((Trunc(value * fixedPointFraction) + 2048 - 1) / 4096);
    }

    public StatsTable SpreadModify(StatsTable baseStats, PokemonSet set)
    {
        StatsTable modStats = new();

        // iterate through all stats in baseStats
        foreach (StatId statName in baseStats.Keys)
        {
            modStats[statName] = StatModify(baseStats, set, statName);
        }
        return modStats;
    }

    /// <summary>
    /// Calculate a single stat value using Pokemon's official stat calculation formula.
    /// HP uses: floor(floor(2 * base + IV + floor(EV/4) + 100) * level / 100 + 10)
    /// Other stats use: floor(floor(2 * base + IV + floor(EV/4)) * level / 100 + 5)
    /// Then nature modifiers are applied with 16-bit truncation.
    /// </summary>
    public int StatModify(StatsTable baseStats, PokemonSet set, StatId statName)
    {
        int stat = baseStats.GetStat(statName);
        int iv = set.Ivs.GetStat(statName);
        int ev = set.Evs.GetStat(statName);

        // HP calculation uses a different formula
        if (statName == StatId.Hp)
        {
            // HP = floor(floor(2 * base + IV + floor(EV/4) + 100) * level / 100 + 10)
            return Trunc(Trunc(2 * stat + iv + Trunc(ev / 4) + 100) * set.Level / 100 + 10);
        }

        // Other stats: floor(floor(2 * base + IV + floor(EV/4)) * level / 100 + 5)
        stat = Trunc(Trunc(2 * stat + iv + Trunc(ev / 4)) * set.Level / 100 + 5);

        // Apply nature modifiers
        Nature nature = set.Nature;

        // Natures are calculated with 16-bit truncation
        // This only affects Eternatus-Eternamax in Pure Hackmons
        if (nature.Plus == statName.ConvertToStatIdExceptId())
        {
            // Positive nature: multiply by 1.1 (110/100)
            // Overflow protection: cap at 595 if rule is enabled
            stat = RuleTable.Has(RuleId.OverflowStatMod) ? Math.Min(stat, 595) : stat;
            stat = Trunc(Trunc(stat * 110, 16) / 100);
        }
        else if (nature.Minus == statName.ConvertToStatIdExceptId())
        {
            // Negative nature: multiply by 0.9 (90/100)
            // Overflow protection: cap at 728 if rule is enabled
            stat = RuleTable.Has(RuleId.OverflowStatMod) ? Math.Min(stat, 728) : stat;
            stat = Trunc(Trunc(stat * 90, 16) / 100);
        }
        return stat;
    }

    public int FinalModify(int relayVar)
    {
        relayVar = Modify(relayVar, Event.Modifier ?? 1.0);
        Event.Modifier = 1.0;
        return relayVar;
    }

    /// <summary>
    /// Truncate a number to an unsigned 32-bit integer.
    /// If bits is specified, the number is scaled, truncated, then unscaled.
    /// This is used for precise damage calculations in Pokemon battles.
    /// </summary>
    public int Trunc(int num, int bits = 0)
    {
        if (bits == 0)
        {
            // Simple case: just return the integer as-is
            return num;
        }

        // For 16-bit truncation (used in nature calculations):
        // Truncate to 16 bits by masking with 0xFFFF (65535)
        // This matches the game's behavior for overflow prevention
        if (bits == 16)
        {
            return num & 0xFFFF;
        }

        // For other bit counts, scale up by 2^bits, truncate, then scale back down
        // This effectively performs: Math.Floor(num / (2^bits)) * (2^bits)
        int divisor = 1 << bits; // 2^bits
        return (num / divisor) * divisor;
    }

    public int Trunc(double num, int bits = 0)
    {
        return Trunc((int)Math.Floor(num), bits);
    }

    public int ClampIntRange(int num, int? min, int? max)
    {
        if (num < min)
        {
            return min.Value;
        }
        return num > max ? max.Value : num;
    }
}