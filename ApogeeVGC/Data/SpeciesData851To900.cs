using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData851To900()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Grimmsnarl] = new()
            {
                Id = SpecieId.Grimmsnarl,
                Num = 861,
                Name = "Grimmsnarl",
                Types = [PokemonType.Dark, PokemonType.Fairy],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 120,
                    Def = 65,
                    SpA = 95,
                    SpD = 75,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 1.5,
                WeightKg = 61,
                Color = "Purple",
            },
            [SpecieId.CalyrexIce] = new()
            {
                Id = SpecieId.CalyrexIce,
                Num = 898,
                Name = "Calyrex-Ice",
                BaseSpecies = SpecieId.Calyrex,
                Forme = FormeId.Ice,
                Types = [PokemonType.Psychic, PokemonType.Ice],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 165,
                    Def = 150,
                    SpA = 85,
                    SpD = 130,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.AsOneGlastrier,
                },
                HeightM = 2.4,
                WeightKg = 809.1,
                Color = "White",
            },
        };
    }
}
