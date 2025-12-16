using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data;

public record SpeciesData
{
    public IReadOnlyDictionary<SpecieId, Species> SpeciesDataDictionary { get; }

    public SpeciesData()
    {
        SpeciesDataDictionary = new ReadOnlyDictionary<SpecieId, Species>(_species);
    }

    private readonly Dictionary<SpecieId, Species> _species = new()
    {
        [SpecieId.Bulbasaur] = new Species
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
        [SpecieId.Ivysaur] = new Species
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
        [SpecieId.Venusaur] = new Species
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
        [SpecieId.VenusaurMega] = new Species
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
        [SpecieId.Charmander] = new Species
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
        [SpecieId.Charmeleon] = new Species
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
        [SpecieId.Charizard] = new Species
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
        [SpecieId.CharizardMegaX] = new Species
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
        [SpecieId.CharizardMegaY] = new Species
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
        [SpecieId.CharizardGmax] = new Species
        {
            Id = SpecieId.CharizardGmax,
            Num = 6,
            Name = "Charizard-Gmax",
            BaseSpecies = SpecieId.Charizard,
            Forme = FormeId.Gmax,
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
            HeightM = 28,
            WeightKg = 0,
            Color = "Red",
        },
        [SpecieId.Squirtle] = new Species
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
        [SpecieId.Wartortle] = new Species
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
        [SpecieId.Blastoise] = new Species
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
        [SpecieId.BlastoiseMega] = new Species
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
        [SpecieId.Caterpie] = new Species
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
        [SpecieId.Metapod] = new Species
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
        [SpecieId.Butterfree] = new Species
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
        [SpecieId.Weedle] = new Species
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
        [SpecieId.Kakuna] = new Species
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
        [SpecieId.Beedrill] = new Species
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
        [SpecieId.BeedrillMega] = new Species
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
            [SpecieId.Pidgey] = new Species
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
            [SpecieId.Pidgeotto] = new Species
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
            [SpecieId.Pidgeot] = new Species
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
            [SpecieId.PidgeotMega] = new Species
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
            [SpecieId.Rattata] = new Species
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
            [SpecieId.RattataAlola] = new Species
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
            [SpecieId.Raticate] = new Species
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
            [SpecieId.RaticateAlola] = new Species
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
            [SpecieId.RaticateAlolaTotem] = new Species
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
            [SpecieId.Spearow] = new Species
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
            [SpecieId.Fearow] = new Species
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
            [SpecieId.Ekans] = new Species
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
            [SpecieId.Arbok] = new Species
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
            [SpecieId.CalyrexIce] = new Species
            {
                Id = SpecieId.CalyrexIce,
                Num = 898,
                Name = "Calyrex-Ice",
            BaseSpecies = SpecieId.Calyrex,
            Forme = FormeId.Ice,
            Types = [PokemonType.Psychic, PokemonType.Ice],
            Gender = GenderId.N,
            BaseStats = new StatsTable()
            {
                Hp = 100,
                Atk = 165,
                Def = 150,
                SpA = 85,
                SpD = 130,
                Spe = 50,
            },
            Abilities = new SpeciesAbility { Slot0 = AbilityId.AsOneGlastrier },
            HeightM = 2.4,
            WeightKg = 809.1,
            Color = "White",
        },
        [SpecieId.Miraidon] = new Species
        {
            Id = SpecieId.Miraidon,
            Num = 1008,
            Name = "Miraidon",
            Types = [PokemonType.Electric, PokemonType.Dragon],
            Gender = GenderId.N,
            BaseStats = new StatsTable
            {
                Hp = 100,
                Atk = 85,
                Def = 100,
                SpA = 135,
                SpD = 115,
                Spe = 135,
            },
            Abilities = new SpeciesAbility { Slot0 = AbilityId.HadronEngine },
            HeightM = 3.5,
            WeightKg = 240,
            Color = "Purple",
        },
        [SpecieId.Ursaluna] = new Species
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
        [SpecieId.Volcarona] = new Species
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
        [SpecieId.Grimmsnarl] = new Species
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
        [SpecieId.IronHands] = new Species
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