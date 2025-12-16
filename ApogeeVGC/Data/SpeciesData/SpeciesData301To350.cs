using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData301To350()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Skitty] = new()
            {
                Id = SpecieId.Skitty,
                Num = 300,
                Name = "Skitty",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 45,
                    Def = 45,
                    SpA = 35,
                    SpD = 35,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.Normalize,
                    Hidden = AbilityId.WonderSkin,
                },
                HeightM = 0.6,
                WeightKg = 11,
                Color = "Pink",
            },
            [SpecieId.Delcatty] = new()
            {
                Id = SpecieId.Delcatty,
                Num = 301,
                Name = "Delcatty",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 65,
                    Def = 65,
                    SpA = 55,
                    SpD = 55,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.Normalize,
                    Hidden = AbilityId.WonderSkin,
                },
                HeightM = 1.1,
                WeightKg = 32.6,
                Color = "Purple",
            },
        };
    }
}
