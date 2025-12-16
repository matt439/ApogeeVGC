using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData1001To1050()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Miraidon] = new()
            {
                Id = SpecieId.Miraidon,
                Num = 1008,
                Name = "Miraidon",
                Types = [PokemonType.Electric, PokemonType.Dragon],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 85,
                    Def = 100,
                    SpA = 135,
                    SpD = 115,
                    Spe = 135,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HadronEngine,
                },
                HeightM = 3.5,
                WeightKg = 240,
                Color = "Purple",
            },
        };
    }
}
