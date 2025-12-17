using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData601To650()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Klinklang] = new()
            {
                Id = SpecieId.Klinklang,
                Num = 601,
                Name = "Klinklang",
                Types = [PokemonType.Steel],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 100,
                    Def = 115,
                    SpA = 70,
                    SpD = 85,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Plus,
                    Slot1 = AbilityId.Minus,
                    Hidden = AbilityId.ClearBody,
                },
                HeightM = 0.6,
                WeightKg = 81,
                Color = "Gray",
                Prevo = SpecieId.Klang,
            },
            [SpecieId.Tynamo] = new()
            {
                Id = SpecieId.Tynamo,
                Num = 602,
                Name = "Tynamo",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 45,
                    SpD = 40,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.2,
                WeightKg = 0.3,
                Color = "White",
            },
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
