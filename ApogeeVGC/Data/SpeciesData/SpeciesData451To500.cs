using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData451To500()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Skorupi] = new()
            {
                Id = SpecieId.Skorupi,
                Num = 451,
                Name = "Skorupi",
                Types = [PokemonType.Poison, PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 50,
                    Def = 90,
                    SpA = 30,
                    SpD = 55,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                    Slot1 = AbilityId.Sniper,
                    Hidden = AbilityId.KeenEye,
                },
                HeightM = 0.8,
                WeightKg = 12,
                Color = "Purple",
            },
            [SpecieId.Drapion] = new()
            {
                Id = SpecieId.Drapion,
                Num = 452,
                Name = "Drapion",
                Types = [PokemonType.Poison, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 90,
                    Def = 110,
                    SpA = 60,
                    SpD = 75,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                    Slot1 = AbilityId.Sniper,
                    Hidden = AbilityId.KeenEye,
                },
                HeightM = 1.3,
                WeightKg = 61.5,
                Color = "Purple",
                Prevo = SpecieId.Skorupi,
            },
            [SpecieId.Croagunk] = new()
            {
                Id = SpecieId.Croagunk,
                Num = 453,
                Name = "Croagunk",
                Types = [PokemonType.Poison, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 48,
                    Atk = 61,
                    Def = 40,
                    SpA = 61,
                    SpD = 40,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Anticipation,
                    Slot1 = AbilityId.DrySkin,
                    Hidden = AbilityId.PoisonTouch,
                },
                HeightM = 0.7,
                WeightKg = 23,
                Color = "Blue",
            },
            [SpecieId.Toxicroak] = new()
            {
                Id = SpecieId.Toxicroak,
                Num = 454,
                Name = "Toxicroak",
                Types = [PokemonType.Poison, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 83,
                    Atk = 106,
                    Def = 65,
                    SpA = 86,
                    SpD = 65,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Anticipation,
                    Slot1 = AbilityId.DrySkin,
                    Hidden = AbilityId.PoisonTouch,
                },
                HeightM = 1.3,
                WeightKg = 44.4,
                Color = "Blue",
                Prevo = SpecieId.Croagunk,
            },
            [SpecieId.Carnivine] = new()
            {
                Id = SpecieId.Carnivine,
                Num = 455,
                Name = "Carnivine",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 74,
                    Atk = 100,
                    Def = 72,
                    SpA = 90,
                    SpD = 72,
                    Spe = 46,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 1.4,
                WeightKg = 27,
                Color = "Green",
            },
            [SpecieId.Finneon] = new()
            {
                Id = SpecieId.Finneon,
                Num = 456,
                Name = "Finneon",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 49,
                    Atk = 49,
                    Def = 56,
                    SpA = 49,
                    SpD = 61,
                    Spe = 66,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.StormDrain,
                    Hidden = AbilityId.WaterVeil,
                },
                HeightM = 0.4,
                WeightKg = 7,
                Color = "Blue",
            },
            [SpecieId.Lumineon] = new()
            {
                Id = SpecieId.Lumineon,
                Num = 457,
                Name = "Lumineon",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 69,
                    Atk = 69,
                    Def = 76,
                    SpA = 69,
                    SpD = 86,
                    Spe = 91,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.StormDrain,
                    Hidden = AbilityId.WaterVeil,
                },
                HeightM = 1.2,
                WeightKg = 24,
                Color = "Blue",
                Prevo = SpecieId.Finneon,
            },
            [SpecieId.Mantyke] = new()
            {
                Id = SpecieId.Mantyke,
                Num = 458,
                Name = "Mantyke",
                Types = [PokemonType.Water, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 20,
                    Def = 50,
                    SpA = 60,
                    SpD = 120,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.WaterAbsorb,
                    Hidden = AbilityId.WaterVeil,
                },
                HeightM = 1,
                WeightKg = 65,
                Color = "Blue",
            },
            [SpecieId.Snover] = new()
            {
                Id = SpecieId.Snover,
                Num = 459,
                Name = "Snover",
                Types = [PokemonType.Grass, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 62,
                    Def = 50,
                    SpA = 62,
                    SpD = 60,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowWarning,
                    Hidden = AbilityId.Soundproof,
                },
                HeightM = 1,
                WeightKg = 50.5,
                Color = "White",
            },
            [SpecieId.Abomasnow] = new()
            {
                Id = SpecieId.Abomasnow,
                Num = 460,
                Name = "Abomasnow",
                Types = [PokemonType.Grass, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 92,
                    Def = 75,
                    SpA = 92,
                    SpD = 85,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowWarning,
                    Hidden = AbilityId.Soundproof,
                },
                HeightM = 2.2,
                WeightKg = 135.5,
                Color = "White",
                Prevo = SpecieId.Snover,
            },
            [SpecieId.AbomasnowMega] = new()
            {
                Id = SpecieId.AbomasnowMega,
                Num = 460,
                Name = "Abomasnow-Mega",
                Types = [PokemonType.Grass, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 132,
                    Def = 105,
                    SpA = 132,
                    SpD = 105,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowWarning,
                },
                HeightM = 2.7,
                WeightKg = 185,
                Color = "White",
                BaseSpecies = SpecieId.Abomasnow,
                Forme = FormeId.Mega,
            },
            [SpecieId.Weavile] = new()
            {
                Id = SpecieId.Weavile,
                Num = 461,
                Name = "Weavile",
                Types = [PokemonType.Dark, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 120,
                    Def = 65,
                    SpA = 45,
                    SpD = 85,
                    Spe = 125,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 1.1,
                WeightKg = 34,
                Color = "Black",
                Prevo = SpecieId.Sneasel,
            },
        };
    }
}
