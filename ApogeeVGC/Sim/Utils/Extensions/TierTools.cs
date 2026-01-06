using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class TierTools
{
    // Original nullable versions
    public static bool IsSinglesTier(this Tier? tier)
    {
        return tier is >= Tier.Ag and <= Tier.Lc;
    }

    public static bool IsDoublesTier(this Tier? tier)
    {
        return tier is >= Tier.DUber and <= Tier.DuuAlt;
    }

    public static bool IsOtherTier(this Tier? tier)
    {
        return tier is >= Tier.Unreleased and <= Tier.CapLc;
    }

    public static bool IsDoublesOrOtherTier(this Tier? tier)
    {
        return IsDoublesTier(tier) || IsOtherTier(tier);
    }

    public static bool IsSinglesOrOtherTier(this Tier? tier)
    {
        return IsSinglesTier(tier) || IsOtherTier(tier);
    }

    // New non-nullable overloads
    public static bool IsSinglesTier(this Tier tier)
    {
        return tier is >= Tier.Ag and <= Tier.Lc;
    }

    public static bool IsDoublesTier(this Tier tier)
    {
        return tier is >= Tier.DUber and <= Tier.DuuAlt;
    }

    public static bool IsOtherTier(this Tier tier)
    {
        return tier is >= Tier.Unreleased and <= Tier.CapLc;
    }

    public static bool IsDoublesOrOtherTier(this Tier tier)
    {
        return IsDoublesTier(tier) || IsOtherTier(tier);
    }

    public static bool IsSinglesOrOtherTier(this Tier tier)
    {
        return IsSinglesTier(tier) || IsOtherTier(tier);
    }
}