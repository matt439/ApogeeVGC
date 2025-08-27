namespace ApogeeVGC.Sim;

public record Format : IEffect
{
    public EffectType EffectType => EffectType.Format;
}

public enum Tier
{
    // Singles
    Ag, Uber, UberAlt, Ou, OuAlt, Uubl, Uu, Rubl, Ru, Nubl, Nu, NuAlt, Publ, Pu, PuAlt, Zubl, Zu, Nfe, Lc,
    // Doubles
    DUber, DUberAlt, Dou, DouAlt, Dbl, Duu, DuuAlt,
    // Other
    Unreleased, Illegal, Cap, CapNfe, CapLc
}

// Helper class for tier checks
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

public record SpeciesFormat
{
    public Tier? DoublesTier
    {
        get;
        init // TierTypes.Doubles | TierTypes.Other
        {
            if (value == null || TierTools.IsDoublesOrOtherTier(value))
            {
                field = value;
            }
        }
    }
    public bool? GmaxUnreleased { get; init; }
    public Nonstandard? IsNonstandard { get; init; }
    public Tier? NatDexTier
    {
        get;
        init // TierTypes.Singles | TierTypes.Other
        {
            if (value == null || TierTools.IsSinglesOrOtherTier(value))
            {
                field = value;
            }
        }
    }

    public Tier? Tier
    {
        get;
        init // TierTypes.Singles | TierTypes.Other
        {
            if (value == null || TierTools.IsSinglesOrOtherTier(value))
            {
                field = value;
            }
        }
    }
}