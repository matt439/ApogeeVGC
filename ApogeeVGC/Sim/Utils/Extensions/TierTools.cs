using ApogeeVGC.Sim.GameObjects;

namespace ApogeeVGC.Sim.Utils.Extensions;

public static class TierTools
{
    public static bool IsSinglesTier(Tier? tier)
    {
        return tier is >= Tier.Ag and <= Tier.Lc;
    }
    public static bool IsDoublesTier(Tier? tier)
    {
        return tier is >= Tier.DUber and <= Tier.DuuAlt;
    }
    public static bool IsOtherTier(Tier? tier)
    {
        return tier is >= Tier.Unreleased and <= Tier.CapLc;
    }
    public static bool IsDoublesOrOtherTier(Tier? tier)
    {
        return IsDoublesTier(tier) || IsOtherTier(tier);
    }
    public static bool IsSinglesOrOtherTier(Tier? tier)
    {
        return IsSinglesTier(tier) || IsOtherTier(tier);
    }
}