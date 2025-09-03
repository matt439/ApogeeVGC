using System.Collections.ObjectModel;
using ApogeeVGC.Sim.GameObjects;

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