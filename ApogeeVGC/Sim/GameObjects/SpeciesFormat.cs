using ApogeeVGC.Sim.Types;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.GameObjects;

public record SpeciesFormat
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
}