using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData1To50()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Bulbasaur] = new()
            {
                Id = SpecieId.Bulbasaur,
                Num = 1,
                Name = "Bulbasaur",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 49,
                    Def = 49,
                    SpA = 65,
                    SpD = 65,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Chlorophyll,
                },
                HeightM = 0.7,
                WeightKg = 6.9,
                Color = "Green",
            },
            [SpecieId.Ivysaur] = new()
            {
                Id = SpecieId.Ivysaur,
                Num = 2,
                Name = "Ivysaur",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 62,
                    Def = 63,
                    SpA = 80,
                    SpD = 80,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Chlorophyll,
                },
                HeightM = 1,
                WeightKg = 13,
                Color = "Green",
            },
            [SpecieId.Venusaur] = new()
            {
                Id = SpecieId.Venusaur,
                Num = 3,
                Name = "Venusaur",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 82,
                    Def = 83,
                    SpA = 100,
                    SpD = 100,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Chlorophyll,
                },
                HeightM = 2,
                WeightKg = 100,
                Color = "Green",
            },
            [SpecieId.VenusaurMega] = new()
            {
                Id = SpecieId.VenusaurMega,
                Num = 3,
                Name = "Venusaur-Mega",
                BaseSpecies = SpecieId.Venusaur,
                Forme = FormeId.Mega,
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 100,
                    Def = 123,
                    SpA = 122,
                    SpD = 120,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                },
                HeightM = 2.4,
                WeightKg = 155.5,
                Color = "Green",
            },
            [SpecieId.Charmander] = new()
            {
                Id = SpecieId.Charmander,
                Num = 4,
                Name = "Charmander",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 39,
                    Atk = 52,
                    Def = 43,
                    SpA = 60,
                    SpD = 50,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.SolarPower,
                },
                HeightM = 0.6,
                WeightKg = 8.5,
                Color = "Red",
            },
            [SpecieId.Charmeleon] = new()
            {
                Id = SpecieId.Charmeleon,
                Num = 5,
                Name = "Charmeleon",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 58,
                    Atk = 64,
                    Def = 58,
                    SpA = 80,
                    SpD = 65,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.SolarPower,
                },
                HeightM = 1.1,
                WeightKg = 19,
                Color = "Red",
            },
            [SpecieId.Charizard] = new()
            {
                Id = SpecieId.Charizard,
                Num = 6,
                Name = "Charizard",
                Types = [PokemonType.Fire, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 84,
                    Def = 78,
                    SpA = 109,
                    SpD = 85,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.SolarPower,
                },
                HeightM = 1.7,
                WeightKg = 90.5,
                Color = "Red",
            },
            [SpecieId.CharizardMegaX] = new()
            {
                Id = SpecieId.CharizardMegaX,
                Num = 6,
                Name = "Charizard-Mega-X",
                BaseSpecies = SpecieId.Charizard,
                Forme = FormeId.MegaX,
                Types = [PokemonType.Fire, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 130,
                    Def = 111,
                    SpA = 130,
                    SpD = 85,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ToughClaws,
                },
                HeightM = 1.7,
                WeightKg = 110.5,
                Color = "Black",
            },
            [SpecieId.CharizardMegaY] = new()
            {
                Id = SpecieId.CharizardMegaY,
                Num = 6,
                Name = "Charizard-Mega-Y",
                BaseSpecies = SpecieId.Charizard,
                Forme = FormeId.MegaY,
                Types = [PokemonType.Fire, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 104,
                    Def = 78,
                    SpA = 159,
                    SpD = 115,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Drought,
                },
                HeightM = 1.7,
                WeightKg = 100.5,
                Color = "Red",
            },
            [SpecieId.Squirtle] = new()
            {
                Id = SpecieId.Squirtle,
                Num = 7,
                Name = "Squirtle",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 44,
                    Atk = 48,
                    Def = 65,
                    SpA = 50,
                    SpD = 64,
                    Spe = 43,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.RainDish,
                },
                HeightM = 0.5,
                WeightKg = 9,
                Color = "Blue",
            },
            [SpecieId.Wartortle] = new()
            {
                Id = SpecieId.Wartortle,
                Num = 8,
                Name = "Wartortle",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 59,
                    Atk = 63,
                    Def = 80,
                    SpA = 65,
                    SpD = 80,
                    Spe = 58,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.RainDish,
                },
                HeightM = 1,
                WeightKg = 22.5,
                Color = "Blue",
            },
            [SpecieId.Blastoise] = new()
            {
                Id = SpecieId.Blastoise,
                Num = 9,
                Name = "Blastoise",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 79,
                    Atk = 83,
                    Def = 100,
                    SpA = 85,
                    SpD = 105,
                    Spe = 78,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.RainDish,
                },
                HeightM = 1.6,
                WeightKg = 85.5,
                Color = "Blue",
            },
            [SpecieId.BlastoiseMega] = new()
            {
                Id = SpecieId.BlastoiseMega,
                Num = 9,
                Name = "Blastoise-Mega",
                BaseSpecies = SpecieId.Blastoise,
                Forme = FormeId.Mega,
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 79,
                    Atk = 103,
                    Def = 120,
                    SpA = 135,
                    SpD = 115,
                    Spe = 78,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MegaLauncher,
                },
                HeightM = 1.6,
                WeightKg = 101.1,
                Color = "Blue",
            },
            [SpecieId.Caterpie] = new()
            {
                Id = SpecieId.Caterpie,
                Num = 10,
                Name = "Caterpie",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 30,
                    Def = 35,
                    SpA = 20,
                    SpD = 20,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldDust,
                    Hidden = AbilityId.RunAway,
                },
                HeightM = 0.3,
                WeightKg = 2.9,
                Color = "Green",
            },
            [SpecieId.Metapod] = new()
            {
                Id = SpecieId.Metapod,
                Num = 11,
                Name = "Metapod",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 20,
                    Def = 55,
                    SpA = 25,
                    SpD = 25,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                },
                HeightM = 0.7,
                WeightKg = 9.9,
                Color = "Green",
            },
            [SpecieId.Butterfree] = new()
            {
                Id = SpecieId.Butterfree,
                Num = 12,
                Name = "Butterfree",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 45,
                    Def = 50,
                    SpA = 90,
                    SpD = 80,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CompoundEyes,
                    Hidden = AbilityId.TintedLens,
                },
                HeightM = 1.1,
                WeightKg = 32,
                Color = "White",
            },
            [SpecieId.Weedle] = new()
            {
                Id = SpecieId.Weedle,
                Num = 13,
                Name = "Weedle",
                Types = [PokemonType.Bug, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 35,
                    Def = 30,
                    SpA = 20,
                    SpD = 20,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldDust,
                    Hidden = AbilityId.RunAway,
                },
                HeightM = 0.3,
                WeightKg = 3.2,
                Color = "Brown",
            },
            [SpecieId.Kakuna] = new()
            {
                Id = SpecieId.Kakuna,
                Num = 14,
                Name = "Kakuna",
                Types = [PokemonType.Bug, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 25,
                    Def = 50,
                    SpA = 25,
                    SpD = 25,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                },
                HeightM = 0.6,
                WeightKg = 10,
                Color = "Yellow",
            },
            [SpecieId.Beedrill] = new()
            {
                Id = SpecieId.Beedrill,
                Num = 15,
                Name = "Beedrill",
                Types = [PokemonType.Bug, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 90,
                    Def = 40,
                    SpA = 45,
                    SpD = 80,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Hidden = AbilityId.Sniper,
                },
                HeightM = 1,
                WeightKg = 29.5,
                Color = "Yellow",
            },
            [SpecieId.BeedrillMega] = new()
            {
                Id = SpecieId.BeedrillMega,
                Num = 15,
                Name = "Beedrill-Mega",
                BaseSpecies = SpecieId.Beedrill,
                Forme = FormeId.Mega,
                Types = [PokemonType.Bug, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 150,
                    Def = 40,
                    SpA = 15,
                    SpD = 80,
                    Spe = 145,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Adaptability,
                },
                HeightM = 1.4,
                WeightKg = 40.5,
                Color = "Yellow",
            },
            [SpecieId.Pidgey] = new()
            {
                Id = SpecieId.Pidgey,
                Num = 16,
                Name = "Pidgey",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 45,
                    Def = 40,
                    SpA = 35,
                    SpD = 35,
                    Spe = 56,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.TangledFeet,
                    Hidden = AbilityId.BigPecks,
                },
                HeightM = 0.3,
                WeightKg = 1.8,
                Color = "Brown",
            },
            [SpecieId.Pidgeotto] = new()
            {
                Id = SpecieId.Pidgeotto,
                Num = 17,
                Name = "Pidgeotto",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 63,
                    Atk = 60,
                    Def = 55,
                    SpA = 50,
                    SpD = 50,
                    Spe = 71,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.TangledFeet,
                    Hidden = AbilityId.BigPecks,
                },
                HeightM = 1.1,
                WeightKg = 30,
                Color = "Brown",
            },
            [SpecieId.Pidgeot] = new()
            {
                Id = SpecieId.Pidgeot,
                Num = 18,
                Name = "Pidgeot",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 83,
                    Atk = 80,
                    Def = 75,
                    SpA = 70,
                    SpD = 70,
                    Spe = 101,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.TangledFeet,
                    Hidden = AbilityId.BigPecks,
                },
                HeightM = 1.5,
                WeightKg = 39.5,
                Color = "Brown",
            },
            [SpecieId.PidgeotMega] = new()
            {
                Id = SpecieId.PidgeotMega,
                Num = 18,
                Name = "Pidgeot-Mega",
                BaseSpecies = SpecieId.Pidgeot,
                Forme = FormeId.Mega,
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 83,
                    Atk = 80,
                    Def = 80,
                    SpA = 135,
                    SpD = 80,
                    Spe = 121,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NoGuard,
                },
                HeightM = 2.2,
                WeightKg = 50.5,
                Color = "Brown",
            },
            [SpecieId.Rattata] = new()
            {
                Id = SpecieId.Rattata,
                Num = 19,
                Name = "Rattata",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 56,
                    Def = 35,
                    SpA = 25,
                    SpD = 35,
                    Spe = 72,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.Guts,
                    Hidden = AbilityId.Hustle,
                },
                HeightM = 0.3,
                WeightKg = 3.5,
                Color = "Purple",
            },
            [SpecieId.RattataAlola] = new()
            {
                Id = SpecieId.RattataAlola,
                Num = 19,
                Name = "Rattata-Alola",
                BaseSpecies = SpecieId.Rattata,
                Forme = FormeId.Alola,
                Types = [PokemonType.Dark, PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 56,
                    Def = 35,
                    SpA = 25,
                    SpD = 35,
                    Spe = 72,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Gluttony,
                    Slot1 = AbilityId.Hustle,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 0.3,
                WeightKg = 3.8,
                Color = "Black",
            },
            [SpecieId.Raticate] = new()
            {
                Id = SpecieId.Raticate,
                Num = 20,
                Name = "Raticate",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 81,
                    Def = 60,
                    SpA = 50,
                    SpD = 70,
                    Spe = 97,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.Guts,
                    Hidden = AbilityId.Hustle,
                },
                HeightM = 0.7,
                WeightKg = 18.5,
                Color = "Brown",
            },
            [SpecieId.RaticateAlola] = new()
            {
                Id = SpecieId.RaticateAlola,
                Num = 20,
                Name = "Raticate-Alola",
                BaseSpecies = SpecieId.Raticate,
                Forme = FormeId.Alola,
                Types = [PokemonType.Dark, PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 71,
                    Def = 70,
                    SpA = 40,
                    SpD = 80,
                    Spe = 77,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Gluttony,
                    Slot1 = AbilityId.Hustle,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 0.7,
                WeightKg = 25.5,
                Color = "Black",
            },
            [SpecieId.RaticateAlolaTotem] = new()
            {
                Id = SpecieId.RaticateAlolaTotem,
                Num = 20,
                Name = "Raticate-Alola-Totem",
                BaseSpecies = SpecieId.Raticate,
                Forme = FormeId.AlolaTotem,
                Types = [PokemonType.Dark, PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 71,
                    Def = 70,
                    SpA = 40,
                    SpD = 80,
                    Spe = 77,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                },
                HeightM = 1.4,
                WeightKg = 105,
                Color = "Black",
            },
            [SpecieId.Spearow] = new()
            {
                Id = SpecieId.Spearow,
                Num = 21,
                Name = "Spearow",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 60,
                    Def = 30,
                    SpA = 31,
                    SpD = 31,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Hidden = AbilityId.Sniper,
                },
                HeightM = 0.3,
                WeightKg = 2,
                Color = "Brown",
            },
            [SpecieId.Fearow] = new()
            {
                Id = SpecieId.Fearow,
                Num = 22,
                Name = "Fearow",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 90,
                    Def = 65,
                    SpA = 61,
                    SpD = 61,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Hidden = AbilityId.Sniper,
                },
                HeightM = 1.2,
                WeightKg = 38,
                Color = "Brown",
            },
            [SpecieId.Ekans] = new()
            {
                Id = SpecieId.Ekans,
                Num = 23,
                Name = "Ekans",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 60,
                    Def = 44,
                    SpA = 40,
                    SpD = 54,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.ShedSkin,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 2,
                WeightKg = 6.9,
                Color = "Purple",
            },
            [SpecieId.Arbok] = new()
            {
                Id = SpecieId.Arbok,
                Num = 24,
                Name = "Arbok",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 95,
                    Def = 69,
                    SpA = 65,
                    SpD = 79,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.ShedSkin,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 3.5,
                WeightKg = 65,
                Color = "Purple",
            },
            [SpecieId.Pikachu] = new()
            {
                Id = SpecieId.Pikachu,
                Num = 25,
                Name = "Pikachu",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuCosplay] = new()
            {
                Id = SpecieId.PikachuCosplay,
                Num = 25,
                Name = "Pikachu-Cosplay",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.Cosplay,
                Types = [PokemonType.Electric],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuRockStar] = new()
            {
                Id = SpecieId.PikachuRockStar,
                Num = 25,
                Name = "Pikachu-Rock-Star",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.RockStar,
                Types = [PokemonType.Electric],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuBelle] = new()
            {
                Id = SpecieId.PikachuBelle,
                Num = 25,
                Name = "Pikachu-Belle",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.Belle,
                Types = [PokemonType.Electric],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuPopStar] = new()
            {
                Id = SpecieId.PikachuPopStar,
                Num = 25,
                Name = "Pikachu-Pop-Star",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.PopStar,
                Types = [PokemonType.Electric],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuPhD] = new()
            {
                Id = SpecieId.PikachuPhD,
                Num = 25,
                Name = "Pikachu-PhD",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.PhD,
                Types = [PokemonType.Electric],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuLibre] = new()
            {
                Id = SpecieId.PikachuLibre,
                Num = 25,
                Name = "Pikachu-Libre",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.Libre,
                Types = [PokemonType.Electric],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuOriginal] = new()
            {
                Id = SpecieId.PikachuOriginal,
                Num = 25,
                Name = "Pikachu-Original",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.Original,
                Types = [PokemonType.Electric],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuHoenn] = new()
            {
                Id = SpecieId.PikachuHoenn,
                Num = 25,
                Name = "Pikachu-Hoenn",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.Hoenn,
                Types = [PokemonType.Electric],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuSinnoh] = new()
            {
                Id = SpecieId.PikachuSinnoh,
                Num = 25,
                Name = "Pikachu-Sinnoh",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.Sinnoh,
                Types = [PokemonType.Electric],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuUnova] = new()
            {
                Id = SpecieId.PikachuUnova,
                Num = 25,
                Name = "Pikachu-Unova",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.Unova,
                Types = [PokemonType.Electric],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuKalos] = new()
            {
                Id = SpecieId.PikachuKalos,
                Num = 25,
                Name = "Pikachu-Kalos",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.Kalos,
                Types = [PokemonType.Electric],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuAlola] = new()
            {
                Id = SpecieId.PikachuAlola,
                Num = 25,
                Name = "Pikachu-Alola",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.Alola,
                Types = [PokemonType.Electric],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuPartner] = new()
            {
                Id = SpecieId.PikachuPartner,
                Num = 25,
                Name = "Pikachu-Partner",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.Partner,
                Types = [PokemonType.Electric],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuStarter] = new()
            {
                Id = SpecieId.PikachuStarter,
                Num = 25,
                Name = "Pikachu-Starter",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.Starter,
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 80,
                    Def = 50,
                    SpA = 75,
                    SpD = 60,
                    Spe = 120,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.PikachuWorld] = new()
            {
                Id = SpecieId.PikachuWorld,
                Num = 25,
                Name = "Pikachu-World",
                BaseSpecies = SpecieId.Pikachu,
                Forme = FormeId.World,
                Types = [PokemonType.Electric],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 50,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Yellow",
            },
            [SpecieId.Raichu] = new()
            {
                Id = SpecieId.Raichu,
                Num = 26,
                Name = "Raichu",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 90,
                    Def = 55,
                    SpA = 90,
                    SpD = 80,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.8,
                WeightKg = 30,
                Color = "Yellow",
            },
            [SpecieId.RaichuAlola] = new()
            {
                Id = SpecieId.RaichuAlola,
                Num = 26,
                Name = "Raichu-Alola",
                BaseSpecies = SpecieId.Raichu,
                Forme = FormeId.Alola,
                Types = [PokemonType.Electric, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 85,
                    Def = 50,
                    SpA = 95,
                    SpD = 85,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SurgeSurfer,
                },
                HeightM = 0.7,
                WeightKg = 21,
                Color = "Brown",
            },
            [SpecieId.Sandshrew] = new()
            {
                Id = SpecieId.Sandshrew,
                Num = 27,
                Name = "Sandshrew",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 75,
                    Def = 85,
                    SpA = 20,
                    SpD = 30,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandVeil,
                    Hidden = AbilityId.SandRush,
                },
                HeightM = 0.6,
                WeightKg = 12,
                Color = "Yellow",
            },
            [SpecieId.SandshrewAlola] = new()
            {
                Id = SpecieId.SandshrewAlola,
                Num = 27,
                Name = "Sandshrew-Alola",
                BaseSpecies = SpecieId.Sandshrew,
                Forme = FormeId.Alola,
                Types = [PokemonType.Ice, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 75,
                    Def = 90,
                    SpA = 10,
                    SpD = 35,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowCloak,
                    Hidden = AbilityId.SlushRush,
                },
                HeightM = 0.7,
                WeightKg = 40,
                Color = "White",
            },
            [SpecieId.Sandslash] = new()
            {
                Id = SpecieId.Sandslash,
                Num = 28,
                Name = "Sandslash",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 100,
                    Def = 110,
                    SpA = 45,
                    SpD = 55,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandVeil,
                    Hidden = AbilityId.SandRush,
                },
                HeightM = 1,
                WeightKg = 29.5,
                Color = "Yellow",
            },
            [SpecieId.SandslashAlola] = new()
            {
                Id = SpecieId.SandslashAlola,
                Num = 28,
                Name = "Sandslash-Alola",
                BaseSpecies = SpecieId.Sandslash,
                Forme = FormeId.Alola,
                Types = [PokemonType.Ice, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 100,
                    Def = 120,
                    SpA = 25,
                    SpD = 65,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowCloak,
                    Hidden = AbilityId.SlushRush,
                },
                HeightM = 1.2,
                WeightKg = 55,
                Color = "Blue",
            },
            [SpecieId.NidoranF] = new()
            {
                Id = SpecieId.NidoranF,
                Num = 29,
                Name = "Nidoran-F",
                Types = [PokemonType.Poison],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 47,
                    Def = 52,
                    SpA = 40,
                    SpD = 40,
                    Spe = 41,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.Rivalry,
                    Hidden = AbilityId.Hustle,
                },
                HeightM = 0.4,
                WeightKg = 7,
                Color = "Blue",
            },
            [SpecieId.Nidorina] = new()
            {
                Id = SpecieId.Nidorina,
                Num = 30,
                Name = "Nidorina",
                Types = [PokemonType.Poison],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 62,
                    Def = 67,
                    SpA = 55,
                    SpD = 55,
                    Spe = 56,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.Rivalry,
                    Hidden = AbilityId.Hustle,
                },
                HeightM = 0.8,
                WeightKg = 20,
                Color = "Blue",
            },
            [SpecieId.Nidoqueen] = new()
            {
                Id = SpecieId.Nidoqueen,
                Num = 31,
                Name = "Nidoqueen",
                Types = [PokemonType.Poison, PokemonType.Ground],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 92,
                    Def = 87,
                    SpA = 75,
                    SpD = 85,
                    Spe = 76,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.Rivalry,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 1.3,
                WeightKg = 60,
                Color = "Blue",
            },
            [SpecieId.NidoranM] = new()
            {
                Id = SpecieId.NidoranM,
                Num = 32,
                Name = "Nidoran-M",
                Types = [PokemonType.Poison],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 46,
                    Atk = 57,
                    Def = 40,
                    SpA = 40,
                    SpD = 40,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.Rivalry,
                    Hidden = AbilityId.Hustle,
                },
                HeightM = 0.5,
                WeightKg = 9,
                Color = "Purple",
            },
            [SpecieId.Nidorino] = new()
            {
                Id = SpecieId.Nidorino,
                Num = 33,
                Name = "Nidorino",
                Types = [PokemonType.Poison],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 61,
                    Atk = 72,
                    Def = 57,
                    SpA = 55,
                    SpD = 55,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.Rivalry,
                    Hidden = AbilityId.Hustle,
                },
                HeightM = 0.9,
                WeightKg = 19.5,
                Color = "Purple",
            },
            [SpecieId.Nidoking] = new()
            {
                Id = SpecieId.Nidoking,
                Num = 34,
                Name = "Nidoking",
                Types = [PokemonType.Poison, PokemonType.Ground],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 81,
                    Atk = 102,
                    Def = 77,
                    SpA = 85,
                    SpD = 75,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.Rivalry,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 1.4,
                WeightKg = 62,
                Color = "Purple",
            },
            [SpecieId.Clefairy] = new()
            {
                Id = SpecieId.Clefairy,
                Num = 35,
                Name = "Clefairy",
                Types = [PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 45,
                    Def = 48,
                    SpA = 60,
                    SpD = 65,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.MagicGuard,
                    Hidden = AbilityId.FriendGuard,
                },
                HeightM = 0.6,
                WeightKg = 7.5,
                Color = "Pink",
            },
            [SpecieId.Clefable] = new()
            {
                Id = SpecieId.Clefable,
                Num = 36,
                Name = "Clefable",
                Types = [PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 70,
                    Def = 73,
                    SpA = 95,
                    SpD = 90,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.MagicGuard,
                    Hidden = AbilityId.Unaware,
                },
                HeightM = 1.3,
                WeightKg = 40,
                Color = "Pink",
            },
            [SpecieId.ClefableMega] = new()
            {
                Id = SpecieId.ClefableMega,
                Num = 36,
                Name = "Clefable-Mega",
                BaseSpecies = SpecieId.Clefable,
                Forme = FormeId.Mega,
                Types = [PokemonType.Fairy, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 80,
                    Def = 93,
                    SpA = 135,
                    SpD = 110,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.MagicGuard,
                    Hidden = AbilityId.Unaware,
                },
                HeightM = 1.7,
                WeightKg = 42.3,
                Color = "Pink",
            },
            [SpecieId.Vulpix] = new()
            {
                Id = SpecieId.Vulpix,
                Num = 37,
                Name = "Vulpix",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 38,
                    Atk = 41,
                    Def = 40,
                    SpA = 50,
                    SpD = 65,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlashFire,
                    Hidden = AbilityId.Drought,
                },
                HeightM = 0.6,
                WeightKg = 9.9,
                Color = "Brown",
            },
            [SpecieId.VulpixAlola] = new()
            {
                Id = SpecieId.VulpixAlola,
                Num = 37,
                Name = "Vulpix-Alola",
                BaseSpecies = SpecieId.Vulpix,
                Forme = FormeId.Alola,
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 38,
                    Atk = 41,
                    Def = 40,
                    SpA = 50,
                    SpD = 65,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowCloak,
                    Hidden = AbilityId.SnowWarning,
                },
                HeightM = 0.6,
                WeightKg = 9.9,
                Color = "White",
            },
            [SpecieId.Ninetales] = new()
            {
                Id = SpecieId.Ninetales,
                Num = 38,
                Name = "Ninetales",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 73,
                    Atk = 76,
                    Def = 75,
                    SpA = 81,
                    SpD = 100,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlashFire,
                    Hidden = AbilityId.Drought,
                },
                HeightM = 1.1,
                WeightKg = 19.9,
                Color = "Yellow",
            },
            [SpecieId.NinetalesAlola] = new()
            {
                Id = SpecieId.NinetalesAlola,
                Num = 38,
                Name = "Ninetales-Alola",
                BaseSpecies = SpecieId.Ninetales,
                Forme = FormeId.Alola,
                Types = [PokemonType.Ice, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 73,
                    Atk = 67,
                    Def = 75,
                    SpA = 81,
                    SpD = 100,
                    Spe = 109,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowCloak,
                    Hidden = AbilityId.SnowWarning,
                },
                HeightM = 1.1,
                WeightKg = 19.9,
                Color = "Blue",
            },
            [SpecieId.Jigglypuff] = new()
            {
                Id = SpecieId.Jigglypuff,
                Num = 39,
                Name = "Jigglypuff",
                Types = [PokemonType.Normal, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 115,
                    Atk = 45,
                    Def = 20,
                    SpA = 45,
                    SpD = 25,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.Competitive,
                    Hidden = AbilityId.FriendGuard,
                },
                HeightM = 0.5,
                WeightKg = 5.5,
                Color = "Pink",
            },
            [SpecieId.Wigglytuff] = new()
            {
                Id = SpecieId.Wigglytuff,
                Num = 40,
                Name = "Wigglytuff",
                Types = [PokemonType.Normal, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 140,
                    Atk = 70,
                    Def = 45,
                    SpA = 85,
                    SpD = 50,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.Competitive,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 1,
                WeightKg = 12,
                Color = "Pink",
            },
            [SpecieId.Zubat] = new()
            {
                Id = SpecieId.Zubat,
                Num = 41,
                Name = "Zubat",
                Types = [PokemonType.Poison, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 45,
                    Def = 35,
                    SpA = 30,
                    SpD = 40,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 0.8,
                WeightKg = 7.5,
                Color = "Purple",
            },
            [SpecieId.Golbat] = new()
            {
                Id = SpecieId.Golbat,
                Num = 42,
                Name = "Golbat",
                Types = [PokemonType.Poison, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 80,
                    Def = 70,
                    SpA = 65,
                    SpD = 75,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 1.6,
                WeightKg = 55,
                Color = "Purple",
            },
            [SpecieId.Oddish] = new()
            {
                Id = SpecieId.Oddish,
                Num = 43,
                Name = "Oddish",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 50,
                    Def = 55,
                    SpA = 75,
                    SpD = 65,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Hidden = AbilityId.RunAway,
                },
                HeightM = 0.5,
                WeightKg = 5.4,
                Color = "Blue",
            },
            [SpecieId.Gloom] = new()
            {
                Id = SpecieId.Gloom,
                Num = 44,
                Name = "Gloom",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 65,
                    Def = 70,
                    SpA = 85,
                    SpD = 75,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Hidden = AbilityId.Stench,
                },
                HeightM = 0.8,
                WeightKg = 8.6,
                Color = "Blue",
            },
            [SpecieId.Vileplume] = new()
            {
                Id = SpecieId.Vileplume,
                Num = 45,
                Name = "Vileplume",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 80,
                    Def = 85,
                    SpA = 110,
                    SpD = 90,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Hidden = AbilityId.EffectSpore,
                },
                HeightM = 1.2,
                WeightKg = 18.6,
                Color = "Red",
            },
            [SpecieId.Paras] = new()
            {
                Id = SpecieId.Paras,
                Num = 46,
                Name = "Paras",
                Types = [PokemonType.Bug, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 70,
                    Def = 55,
                    SpA = 45,
                    SpD = 55,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EffectSpore,
                    Slot1 = AbilityId.DrySkin,
                    Hidden = AbilityId.Damp,
                },
                HeightM = 0.3,
                WeightKg = 5.4,
                Color = "Red",
            },
            [SpecieId.Parasect] = new()
            {
                Id = SpecieId.Parasect,
                Num = 47,
                Name = "Parasect",
                Types = [PokemonType.Bug, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 95,
                    Def = 80,
                    SpA = 60,
                    SpD = 80,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EffectSpore,
                    Slot1 = AbilityId.DrySkin,
                    Hidden = AbilityId.Damp,
                },
                HeightM = 1,
                WeightKg = 29.5,
                Color = "Red",
            },
            [SpecieId.Venonat] = new()
            {
                Id = SpecieId.Venonat,
                Num = 48,
                Name = "Venonat",
                Types = [PokemonType.Bug, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 55,
                    Def = 50,
                    SpA = 40,
                    SpD = 55,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CompoundEyes,
                    Slot1 = AbilityId.TintedLens,
                    Hidden = AbilityId.RunAway,
                },
                HeightM = 1,
                WeightKg = 30,
                Color = "Purple",
            },
            [SpecieId.Venomoth] = new()
            {
                Id = SpecieId.Venomoth,
                Num = 49,
                Name = "Venomoth",
                Types = [PokemonType.Bug, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 65,
                    Def = 60,
                    SpA = 90,
                    SpD = 75,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldDust,
                    Slot1 = AbilityId.TintedLens,
                    Hidden = AbilityId.WonderSkin,
                },
                HeightM = 1.5,
                WeightKg = 12.5,
                Color = "Purple",
            },
            [SpecieId.Diglett] = new()
            {
                Id = SpecieId.Diglett,
                Num = 50,
                Name = "Diglett",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 10,
                    Atk = 55,
                    Def = 25,
                    SpA = 35,
                    SpD = 45,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandVeil,
                    Slot1 = AbilityId.ArenaTrap,
                    Hidden = AbilityId.SandForce,
                },
                HeightM = 0.2,
                WeightKg = 0.8,
                Color = "Brown",
            },
            [SpecieId.DiglettAlola] = new()
            {
                Id = SpecieId.DiglettAlola,
                Num = 50,
                Name = "Diglett-Alola",
                BaseSpecies = SpecieId.Diglett,
                Forme = FormeId.Alola,
                Types = [PokemonType.Ground, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 10,
                    Atk = 55,
                    Def = 30,
                    SpA = 35,
                    SpD = 45,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandVeil,
                    Slot1 = AbilityId.TanglingHair,
                    Hidden = AbilityId.SandForce,
                },
                HeightM = 0.2,
                WeightKg = 1,
                Color = "Brown",
            },
        };
    }
}