using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData501To550()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Victini] = new()
            {
                Id = SpecieId.Victini,
                Num = 494,
                Name = "Victini",
                Types = [PokemonType.Psychic, PokemonType.Fire],
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
                    Slot0 = AbilityId.VictoryStar,
                },
                HeightM = 0.4,
                WeightKg = 4,
                Color = "Yellow",
            },
            [SpecieId.Snivy] = new()
            {
                Id = SpecieId.Snivy,
                Num = 495,
                Name = "Snivy",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 45,
                    Def = 55,
                    SpA = 45,
                    SpD = 55,
                    Spe = 63,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Contrary,
                },
                HeightM = 0.6,
                WeightKg = 8.1,
                Color = "Green",
            },
            [SpecieId.Servine] = new()
            {
                Id = SpecieId.Servine,
                Num = 496,
                Name = "Servine",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 60,
                    Def = 75,
                    SpA = 60,
                    SpD = 75,
                    Spe = 83,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Contrary,
                },
                HeightM = 0.8,
                WeightKg = 16,
                Color = "Green",
                Prevo = SpecieId.Snivy,
            },
            [SpecieId.Serperior] = new()
            {
                Id = SpecieId.Serperior,
                Num = 497,
                Name = "Serperior",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 75,
                    Def = 95,
                    SpA = 75,
                    SpD = 95,
                    Spe = 113,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Contrary,
                },
                HeightM = 3.3,
                WeightKg = 63,
                Color = "Green",
                Prevo = SpecieId.Servine,
            },
            [SpecieId.Tepig] = new()
            {
                Id = SpecieId.Tepig,
                Num = 498,
                Name = "Tepig",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 63,
                    Def = 45,
                    SpA = 45,
                    SpD = 45,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 0.5,
                WeightKg = 9.9,
                Color = "Red",
            },
            [SpecieId.Pignite] = new()
            {
                Id = SpecieId.Pignite,
                Num = 499,
                Name = "Pignite",
                Types = [PokemonType.Fire, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 93,
                    Def = 55,
                    SpA = 70,
                    SpD = 55,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 1,
                WeightKg = 55.5,
                Color = "Red",
                Prevo = SpecieId.Tepig,
            },
            [SpecieId.Emboar] = new()
            {
                Id = SpecieId.Emboar,
                Num = 500,
                Name = "Emboar",
                Types = [PokemonType.Fire, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 110,
                    Atk = 123,
                    Def = 65,
                    SpA = 100,
                    SpD = 65,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Reckless,
                },
                HeightM = 1.6,
                WeightKg = 150,
                Color = "Red",
                Prevo = SpecieId.Pignite,
            },
            [SpecieId.EmboarMega] = new()
            {
                Id = SpecieId.EmboarMega,
                Num = 500,
                Name = "Emboar-Mega",
                Types = [PokemonType.Fire, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 110,
                    Atk = 163,
                    Def = 85,
                    SpA = 130,
                    SpD = 85,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Reckless,
                },
                HeightM = 1.6,
                WeightKg = 165,
                Color = "Red",
                BaseSpecies = SpecieId.Emboar,
                Forme = FormeId.Mega,
            },
        };
    }
}
