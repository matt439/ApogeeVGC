using System.Collections.ObjectModel;
using ApogeeVGC.Sim;

namespace ApogeeVGC.Data;

public record SpeciesFormats
{
    public IReadOnlyDictionary<SpecieId, SpeciesFormat> SpeciesFormatsData { get; }

    public SpeciesFormats()
    {
        SpeciesFormatsData = new ReadOnlyDictionary<SpecieId, SpeciesFormat>(_speciesFormatsData);

    }

    private readonly Dictionary<SpecieId, SpeciesFormat> _speciesFormatsData = new()
    {
        [SpecieId.CalyrexIce] = new SpeciesFormat()
        {
            Tier = Tier.Uber,
            DoublesTier = Tier.DUber,
            NatDexTier = Tier.Uber,
        },
        [SpecieId.Miraidon] = new SpeciesFormat
        {
            Tier = Tier.Ag,
            DoublesTier = Tier.DUber,
            NatDexTier = Tier.Ag,
        },
        [SpecieId.Ursaluna] = new SpeciesFormat
        {
            Tier = Tier.Uubl,
            DoublesTier = Tier.Dou,
            NatDexTier = Tier.Uu,
        },
        [SpecieId.Volcarona] = new SpeciesFormat
        {
            Tier = Tier.Uber,
            DoublesTier = Tier.DuuAlt,
            NatDexTier = Tier.Ou,
        },
        [SpecieId.Grimmsnarl] = new SpeciesFormat
        {
            Tier = Tier.Pu,
            DoublesTier = Tier.Dou,
            NatDexTier = Tier.Ru,
        },
        [SpecieId.IronHands] = new SpeciesFormat
        {
            Tier = Tier.Uubl,
            DoublesTier = Tier.Dou,
            NatDexTier = Tier.Uubl,
        },
    };
}