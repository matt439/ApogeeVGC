using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData151To200()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Mew] = new()
            {
                Id = SpecieId.Mew,
                Num = 151,
                Name = "Mew",
                Types = [PokemonType.Psychic],
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
                    Slot0 = AbilityId.Synchronize,
                },
                HeightM = 0.4,
                WeightKg = 4,
                Color = "Pink",
            },
            [SpecieId.Chikorita] = new()
            {
                Id = SpecieId.Chikorita,
                Num = 152,
                Name = "Chikorita",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 49,
                    Def = 65,
                    SpA = 49,
                    SpD = 65,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 0.9,
                WeightKg = 6.4,
                Color = "Green",
            },
            [SpecieId.Bayleef] = new()
            {
                Id = SpecieId.Bayleef,
                Num = 153,
                Name = "Bayleef",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 62,
                    Def = 80,
                    SpA = 63,
                    SpD = 80,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 1.2,
                WeightKg = 15.8,
                Color = "Green",
            },
            [SpecieId.Meganium] = new()
            {
                Id = SpecieId.Meganium,
                Num = 154,
                Name = "Meganium",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 82,
                    Def = 100,
                    SpA = 83,
                    SpD = 100,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 1.8,
                WeightKg = 100.5,
                Color = "Green",
            },
            [SpecieId.MeganiummMega] = new()
            {
                Id = SpecieId.MeganiummMega,
                Num = 154,
                Name = "Meganium-Mega",
                BaseSpecies = SpecieId.Meganium,
                Forme = FormeId.Mega,
                Types = [PokemonType.Grass, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 92,
                    Def = 115,
                    SpA = 143,
                    SpD = 115,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 2.4,
                WeightKg = 201,
                Color = "Green",
            },
        };
    }
}
