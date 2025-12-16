using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData951To1000()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.IronHands] = new()
            {
                Id = SpecieId.IronHands,
                Num = 992,
                Name = "Iron Hands",
                Types = [PokemonType.Fighting, PokemonType.Electric],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 154,
                    Atk = 140,
                    Def = 108,
                    SpA = 50,
                    SpD = 68,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.QuarkDrive,
                },
                HeightM = 1.8,
                WeightKg = 380.7,
                Color = "Gray",
            },
        };
    }
}
