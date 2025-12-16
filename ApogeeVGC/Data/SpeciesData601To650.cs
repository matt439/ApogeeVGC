using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData601To650()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Volcarona] = new()
            {
                Id = SpecieId.Volcarona,
                Num = 637,
                Name = "Volcarona",
                Types = [PokemonType.Bug, PokemonType.Fire],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 60,
                    Def = 65,
                    SpA = 135,
                    SpD = 105,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlameBody,
                    Hidden = AbilityId.Swarm,
                },
                HeightM = 1.6,
                WeightKg = 46,
                Color = "White",
            },
        };
    }
}
