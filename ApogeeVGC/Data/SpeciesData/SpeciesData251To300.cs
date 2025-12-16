using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData251To300()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Celebi] = new()
            {
                Id = SpecieId.Celebi,
                Num = 251,
                Name = "Celebi",
                Types = [PokemonType.Psychic, PokemonType.Grass],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 100,
                    Def = 100,
                    SpA = 100,
                    SpD = 100,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                },
                HeightM = 0.6,
                WeightKg = 5,
                Color = "Green",
            },
        };
    }
}
