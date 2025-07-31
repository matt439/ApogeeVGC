using ApogeeVGC_CS.sim;

namespace ApogeeVGC_CS.data
{
    public static class FormatsData
    {
        public static SpeciesFormatsDataTable SpeciesFormatsData { get; } = new()
        {
            [new IdEntry("calyrexice")] = new SpeciesFormatsData
            {
                Tier = Tier.Uber,
                DoublesTier = Tier.DUber,
                NatDexTier = Tier.Uber,
            },
            [new IdEntry("miraidon")] = new SpeciesFormatsData
            {
                Tier = Tier.Ag,
                DoublesTier = Tier.DUber,
                NatDexTier = Tier.Ag,
            },
            [new IdEntry("ursaluna")] = new SpeciesFormatsData
            {
                Tier = Tier.Uubl,
                DoublesTier = Tier.Dou,
                NatDexTier = Tier.Uu,
            },
            [new IdEntry("volcarona")] = new SpeciesFormatsData
            {
                Tier = Tier.Uber,
                DoublesTier = Tier.DuuAlt,
                NatDexTier = Tier.Ou,
            },
            [new IdEntry("grimmsnarl")] = new SpeciesFormatsData
            {
                Tier = Tier.Pu,
                DoublesTier = Tier.Dou,
                NatDexTier = Tier.Ru,
            },
            [new IdEntry("ironhands")] = new SpeciesFormatsData
            {
                Tier = Tier.Uubl,
                DoublesTier = Tier.Dou,
                NatDexTier = Tier.Uubl,
            },
        };
    }
}
