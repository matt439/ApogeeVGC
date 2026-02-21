using System.Collections.Frozen;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Data;

public record SpeciesFormats
{
    public FrozenDictionary<SpecieId, SpeciesFormat> SpeciesFormatsData { get; }

    public SpeciesFormats()
    {
        SpeciesFormatsData = _speciesFormatsData.ToFrozenDictionary();

    }

    private readonly Dictionary<SpecieId, SpeciesFormat> _speciesFormatsData = new()
    {
        [SpecieId.CalyrexIce] = new SpeciesFormat()
        {
            Id = SpecieId.CalyrexIce,
            Tier = Tier.Uber,
            DoublesTier = Tier.DUber,
            NatDexTier = Tier.Uber,
        },
        [SpecieId.Miraidon] = new SpeciesFormat
        {
            Id = SpecieId.Miraidon,
            Tier = Tier.Ag,
            DoublesTier = Tier.DUber,
            NatDexTier = Tier.Ag,
        },
        [SpecieId.Ursaluna] = new SpeciesFormat
        {
            Id = SpecieId.Ursaluna,
            Tier = Tier.Uubl,
            DoublesTier = Tier.Dou,
            NatDexTier = Tier.Uu,
        },
        [SpecieId.Volcarona] = new SpeciesFormat
        {
            Id = SpecieId.Volcarona,
            Tier = Tier.Uber,
            DoublesTier = Tier.DuuAlt,
            NatDexTier = Tier.Ou,
        },
        [SpecieId.Grimmsnarl] = new SpeciesFormat
        {
            Id = SpecieId.Grimmsnarl,
            Tier = Tier.Pu,
            DoublesTier = Tier.Dou,
            NatDexTier = Tier.Ru,
        },
        [SpecieId.IronHands] = new SpeciesFormat
        {
            Id = SpecieId.IronHands,
            Tier = Tier.Uubl,
            DoublesTier = Tier.Dou,
            NatDexTier = Tier.Uubl,
        },
    };
}