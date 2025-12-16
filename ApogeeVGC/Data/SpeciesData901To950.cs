using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData901To950()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Ursaluna] = new()
            {
                Id = SpecieId.Ursaluna,
                Num = 901,
                Name = "Ursaluna",
                Types = [PokemonType.Ground, PokemonType.Normal],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 130,
                    Atk = 140,
                    Def = 105,
                    SpA = 45,
                    SpD = 80,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Guts,
                    Slot1 = AbilityId.Bulletproof,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 2.4,
                WeightKg = 290,
                Color = "Brown",
            },
        };
    }
}
