using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Types;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.SpeciesClasses;

public record SpeciesFormat : ICopyable<SpeciesFormat>
{
    public required SpecieId Id { get; init; }
    public Tier? DoublesTier
    {
        get;
        init // TierTypes.Doubles | TierTypes.Other
        {
            if (value == null || value.IsDoublesOrOtherTier())
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
            if (value == null || value.IsSinglesOrOtherTier())
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
            if (value == null || value.IsSinglesOrOtherTier())
            {
                field = value;
            }
        }
    }

    public SpeciesFormat Copy()
    {
        return new SpeciesFormat
        {
            Id = Id,
            DoublesTier = DoublesTier,
            GmaxUnreleased = GmaxUnreleased,
            IsNonstandard = IsNonstandard,
            NatDexTier = NatDexTier,
            Tier = Tier,
        };
    }
}