using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData351To400()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Castform] = new()
            {
                Id = SpecieId.Castform,
                Num = 351,
                Name = "Castform",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 70,
                    Def = 70,
                    SpA = 70,
                    SpD = 70,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Forecast,
                },
                HeightM = 0.3,
                WeightKg = 0.8,
                Color = "Gray",
                OtherFormes = [FormeId.Sunny, FormeId.Rainy, FormeId.Snowy],
            },
            [SpecieId.CastformSunny] = new()
            {
                Id = SpecieId.CastformSunny,
                Num = 351,
                Name = "Castform-Sunny",
                BaseSpecies = SpecieId.Castform,
                Forme = FormeId.Sunny,
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 70,
                    Def = 70,
                    SpA = 70,
                    SpD = 70,
                    Spe = 70,
                },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Forecast,
                    },
                    HeightM = 0.3,
                    WeightKg = 0.8,
                    Color = "Red",
                },
            [SpecieId.CastformRainy] = new()
            {
                Id = SpecieId.CastformRainy,
                Num = 351,
                Name = "Castform-Rainy",
                BaseSpecies = SpecieId.Castform,
                Forme = FormeId.Rainy,
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 70,
                    Def = 70,
                    SpA = 70,
                    SpD = 70,
                    Spe = 70,
                },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Forecast,
                    },
                    HeightM = 0.3,
                    WeightKg = 0.8,
                    Color = "Blue",
                },
            [SpecieId.CastformSnowy] = new()
            {
                Id = SpecieId.CastformSnowy,
                Num = 351,
                Name = "Castform-Snowy",
                BaseSpecies = SpecieId.Castform,
                Forme = FormeId.Snowy,
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 70,
                    Def = 70,
                    SpA = 70,
                    SpD = 70,
                    Spe = 70,
                },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Forecast,
                    },
                    HeightM = 0.3,
                    WeightKg = 0.8,
                    Color = "White",
                },
            [SpecieId.Kecleon] = new()
            {
                Id = SpecieId.Kecleon,
                Num = 352,
                Name = "Kecleon",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 90,
                    Def = 70,
                    SpA = 60,
                    SpD = 120,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ColorChange,
                    Hidden = AbilityId.Protean,
                },
                HeightM = 1,
                WeightKg = 22,
                Color = "Green",
            },
            [SpecieId.Shuppet] = new()
            {
                Id = SpecieId.Shuppet,
                Num = 353,
                Name = "Shuppet",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 44,
                    Atk = 75,
                    Def = 35,
                    SpA = 63,
                    SpD = 33,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.CursedBody,
                },
                HeightM = 0.6,
                WeightKg = 2.3,
                Color = "Black",
            },
            [SpecieId.Banette] = new()
            {
                Id = SpecieId.Banette,
                Num = 354,
                Name = "Banette",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 64,
                    Atk = 115,
                    Def = 65,
                    SpA = 83,
                    SpD = 63,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.CursedBody,
                },
                HeightM = 1.1,
                WeightKg = 12.5,
                Color = "Black",
                Prevo = SpecieId.Shuppet,
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.BanetteMega] = new()
            {
                Id = SpecieId.BanetteMega,
                Num = 354,
                Name = "Banette-Mega",
                BaseSpecies = SpecieId.Banette,
                Forme = FormeId.Mega,
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 64,
                    Atk = 165,
                    Def = 75,
                    SpA = 93,
                    SpD = 83,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                },
                HeightM = 1.2,
                WeightKg = 13,
                Color = "Black",
                RequiredItem = Sim.Items.ItemId.Banettite,
            },
            [SpecieId.Duskull] = new()
            {
                Id = SpecieId.Duskull,
                Num = 355,
                Name = "Duskull",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 20,
                    Atk = 40,
                    Def = 90,
                    SpA = 30,
                    SpD = 90,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 0.8,
                WeightKg = 15,
                Color = "Black",
            },
            [SpecieId.Dusclops] = new()
            {
                Id = SpecieId.Dusclops,
                Num = 356,
                Name = "Dusclops",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 70,
                    Def = 130,
                    SpA = 60,
                    SpD = 130,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 1.6,
                WeightKg = 30.6,
                Color = "Black",
                Prevo = SpecieId.Duskull,
            },
            [SpecieId.Tropius] = new()
            {
                Id = SpecieId.Tropius,
                Num = 357,
                Name = "Tropius",
                Types = [PokemonType.Grass, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 99,
                    Atk = 68,
                    Def = 83,
                    SpA = 72,
                    SpD = 87,
                    Spe = 51,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.SolarPower,
                    Hidden = AbilityId.Harvest,
                },
                HeightM = 2,
                WeightKg = 100,
                Color = "Green",
            },
            [SpecieId.Chimecho] = new()
            {
                Id = SpecieId.Chimecho,
                Num = 358,
                Name = "Chimecho",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 50,
                    Def = 80,
                    SpA = 95,
                    SpD = 90,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.6,
                WeightKg = 1,
                Color = "Blue",
            },
            [SpecieId.Absol] = new()
            {
                Id = SpecieId.Absol,
                Num = 359,
                Name = "Absol",
                Types = [PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 130,
                    Def = 60,
                    SpA = 75,
                    SpD = 60,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Slot1 = AbilityId.SuperLuck,
                    Hidden = AbilityId.Justified,
                },
                HeightM = 1.2,
                WeightKg = 47,
                Color = "White",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.AbsolMega] = new()
            {
                Id = SpecieId.AbsolMega,
                Num = 359,
                Name = "Absol-Mega",
                BaseSpecies = SpecieId.Absol,
                Forme = FormeId.Mega,
                Types = [PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 150,
                    Def = 60,
                    SpA = 115,
                    SpD = 60,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagicBounce,
                },
                HeightM = 1.2,
                WeightKg = 49,
                Color = "White",
                RequiredItem = Sim.Items.ItemId.Absolite,
            },
            [SpecieId.Wynaut] = new()
            {
                Id = SpecieId.Wynaut,
                Num = 360,
                Name = "Wynaut",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 23,
                    Def = 48,
                    SpA = 23,
                    SpD = 48,
                    Spe = 23,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShadowTag,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 0.6,
                WeightKg = 14,
                Color = "Blue",
            },
            [SpecieId.Snorunt] = new()
            {
                Id = SpecieId.Snorunt,
                Num = 361,
                Name = "Snorunt",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 50,
                    Def = 50,
                    SpA = 50,
                    SpD = 50,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Slot1 = AbilityId.IceBody,
                    Hidden = AbilityId.Moody,
                },
                HeightM = 0.7,
                WeightKg = 16.8,
                Color = "Gray",
            },
            [SpecieId.Glalie] = new()
            {
                Id = SpecieId.Glalie,
                Num = 362,
                Name = "Glalie",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 80,
                    Def = 80,
                    SpA = 80,
                    SpD = 80,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Slot1 = AbilityId.IceBody,
                    Hidden = AbilityId.Moody,
                },
                HeightM = 1.5,
                WeightKg = 256.5,
                Color = "Gray",
                Prevo = SpecieId.Snorunt,
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.GlalieMega] = new()
            {
                Id = SpecieId.GlalieMega,
                Num = 362,
                Name = "Glalie-Mega",
                BaseSpecies = SpecieId.Glalie,
                Forme = FormeId.Mega,
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 80,
                    SpA = 120,
                    SpD = 80,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Refrigerate,
                },
                HeightM = 2.1,
                WeightKg = 350.2,
                Color = "Gray",
                RequiredItem = Sim.Items.ItemId.Glalitite,
            },
            [SpecieId.Spheal] = new()
            {
                Id = SpecieId.Spheal,
                Num = 363,
                Name = "Spheal",
                Types = [PokemonType.Ice, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 40,
                    Def = 50,
                    SpA = 55,
                    SpD = 50,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.IceBody,
                    Hidden = AbilityId.Oblivious,
                },
                HeightM = 0.8,
                WeightKg = 39.5,
                Color = "Blue",
            },
            [SpecieId.Sealeo] = new()
            {
                Id = SpecieId.Sealeo,
                Num = 364,
                Name = "Sealeo",
                Types = [PokemonType.Ice, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 60,
                    Def = 70,
                    SpA = 75,
                    SpD = 70,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.IceBody,
                    Hidden = AbilityId.Oblivious,
                },
                HeightM = 1.1,
                WeightKg = 87.6,
                Color = "Blue",
                Prevo = SpecieId.Spheal,
            },
        };
    }
}
