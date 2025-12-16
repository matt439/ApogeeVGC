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
                [SpecieId.Pikachu] = new Species
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
                [SpecieId.PikachuCosplay] = new Species
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
                [SpecieId.PikachuRockStar] = new Species
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
                [SpecieId.PikachuBelle] = new Species
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
                [SpecieId.PikachuPopStar] = new Species
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
                [SpecieId.PikachuPhD] = new Species
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
                [SpecieId.PikachuLibre] = new Species
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
                [SpecieId.PikachuOriginal] = new Species
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
                [SpecieId.PikachuHoenn] = new Species
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
                [SpecieId.PikachuSinnoh] = new Species
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
                [SpecieId.PikachuUnova] = new Species
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
                [SpecieId.PikachuKalos] = new Species
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
                [SpecieId.PikachuAlola] = new Species
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
                [SpecieId.PikachuPartner] = new Species
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
                [SpecieId.PikachuStarter] = new Species
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
                [SpecieId.PikachuGmax] = new Species
                {
                    Id = SpecieId.PikachuGmax,
                    Num = 25,
                    Name = "Pikachu-Gmax",
                    BaseSpecies = SpecieId.Pikachu,
                    Forme = FormeId.Gmax,
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
                    HeightM = 21,
                    WeightKg = 0,
                    Color = "Yellow",
                },
                [SpecieId.PikachuWorld] = new Species
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
                [SpecieId.Raichu] = new Species
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
                [SpecieId.RaichuAlola] = new Species
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
                    [SpecieId.Sandshrew] = new Species
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
                    [SpecieId.SandshrewAlola] = new Species
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
                    [SpecieId.Sandslash] = new Species
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
                    [SpecieId.SandslashAlola] = new Species
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
                    [SpecieId.NidoranF] = new Species
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
                    [SpecieId.Nidorina] = new Species
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
                    [SpecieId.Nidoqueen] = new Species
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
                    [SpecieId.NidoranM] = new Species
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
                    [SpecieId.Nidorino] = new Species
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
                    [SpecieId.Nidoking] = new Species
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
                    [SpecieId.Clefairy] = new Species
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
                    [SpecieId.Clefable] = new Species
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
                    [SpecieId.ClefableMega] = new Species
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
                                [SpecieId.Vulpix] = new Species
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
                                [SpecieId.VulpixAlola] = new Species
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
                                [SpecieId.Ninetales] = new Species
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
                                [SpecieId.NinetalesAlola] = new Species
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
                                [SpecieId.Jigglypuff] = new Species
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
                                [SpecieId.Wigglytuff] = new Species
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
                                [SpecieId.Zubat] = new Species
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
                                [SpecieId.Golbat] = new Species
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
                                [SpecieId.Oddish] = new Species
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
                                [SpecieId.Gloom] = new Species
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
                                [SpecieId.Vileplume] = new Species
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
                                [SpecieId.Paras] = new Species
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
                                [SpecieId.Parasect] = new Species
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
                                [SpecieId.Venonat] = new Species
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
                                [SpecieId.Venomoth] = new Species
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
                                            [SpecieId.Diglett] = new Species
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
                                            [SpecieId.DiglettAlola] = new Species
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
                                            [SpecieId.Dugtrio] = new Species
                                            {
                                                Id = SpecieId.Dugtrio,
                                                Num = 51,
                                                Name = "Dugtrio",
                                                Types = [PokemonType.Ground],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 35,
                                                    Atk = 100,
                                                    Def = 50,
                                                    SpA = 50,
                                                    SpD = 70,
                                                    Spe = 120,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.SandVeil,
                                                    Slot1 = AbilityId.ArenaTrap,
                                                    Hidden = AbilityId.SandForce,
                                                },
                                                HeightM = 0.7,
                                                WeightKg = 33.3,
                                                Color = "Brown",
                                            },
                                            [SpecieId.DugtrioAlola] = new Species
                                            {
                                                Id = SpecieId.DugtrioAlola,
                                                Num = 51,
                                                Name = "Dugtrio-Alola",
                                                BaseSpecies = SpecieId.Dugtrio,
                                                Forme = FormeId.Alola,
                                                Types = [PokemonType.Ground, PokemonType.Steel],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 35,
                                                    Atk = 100,
                                                    Def = 60,
                                                    SpA = 50,
                                                    SpD = 70,
                                                    Spe = 110,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.SandVeil,
                                                    Slot1 = AbilityId.TanglingHair,
                                                    Hidden = AbilityId.SandForce,
                                                },
                                                HeightM = 0.7,
                                                WeightKg = 66.6,
                                                Color = "Brown",
                                            },
                                            [SpecieId.Meowth] = new Species
                                            {
                                                Id = SpecieId.Meowth,
                                                Num = 52,
                                                Name = "Meowth",
                                                Types = [PokemonType.Normal],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 40,
                                                    Atk = 45,
                                                    Def = 35,
                                                    SpA = 40,
                                                    SpD = 40,
                                                    Spe = 90,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Pickup,
                                                    Slot1 = AbilityId.Technician,
                                                    Hidden = AbilityId.Unnerve,
                                                },
                                                HeightM = 0.4,
                                                WeightKg = 4.2,
                                                Color = "Yellow",
                                            },
                                            [SpecieId.MeowthAlola] = new Species
                                            {
                                                Id = SpecieId.MeowthAlola,
                                                Num = 52,
                                                Name = "Meowth-Alola",
                                                BaseSpecies = SpecieId.Meowth,
                                                Forme = FormeId.Alola,
                                                Types = [PokemonType.Dark],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 40,
                                                    Atk = 35,
                                                    Def = 35,
                                                    SpA = 50,
                                                    SpD = 40,
                                                    Spe = 90,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Pickup,
                                                    Slot1 = AbilityId.Technician,
                                                    Hidden = AbilityId.Rattled,
                                                },
                                                HeightM = 0.4,
                                                WeightKg = 4.2,
                                                Color = "Blue",
                                            },
                                            [SpecieId.MeowthGalar] = new Species
                                            {
                                                Id = SpecieId.MeowthGalar,
                                                Num = 52,
                                                Name = "Meowth-Galar",
                                                BaseSpecies = SpecieId.Meowth,
                                                Forme = FormeId.Galar,
                                                Types = [PokemonType.Steel],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 50,
                                                    Atk = 65,
                                                    Def = 55,
                                                    SpA = 40,
                                                    SpD = 40,
                                                    Spe = 40,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Pickup,
                                                    Slot1 = AbilityId.ToughClaws,
                                                    Hidden = AbilityId.Unnerve,
                                                },
                                                HeightM = 0.4,
                                                WeightKg = 7.5,
                                                Color = "Brown",
                                            },
                                            [SpecieId.MeowthGmax] = new Species
                                            {
                                                Id = SpecieId.MeowthGmax,
                                                Num = 52,
                                                Name = "Meowth-Gmax",
                                                BaseSpecies = SpecieId.Meowth,
                                                Forme = FormeId.Gmax,
                                                Types = [PokemonType.Normal],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 40,
                                                    Atk = 45,
                                                    Def = 35,
                                                    SpA = 40,
                                                    SpD = 40,
                                                    Spe = 90,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Pickup,
                                                    Slot1 = AbilityId.Technician,
                                                    Hidden = AbilityId.Unnerve,
                                                },
                                                HeightM = 33,
                                                WeightKg = 0,
                                                Color = "Yellow",
                                            },
                                            [SpecieId.Persian] = new Species
                                            {
                                                Id = SpecieId.Persian,
                                                Num = 53,
                                                Name = "Persian",
                                                Types = [PokemonType.Normal],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 65,
                                                    Atk = 70,
                                                    Def = 60,
                                                    SpA = 65,
                                                    SpD = 65,
                                                    Spe = 115,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Limber,
                                                    Slot1 = AbilityId.Technician,
                                                    Hidden = AbilityId.Unnerve,
                                                },
                                                HeightM = 1,
                                                WeightKg = 32,
                                                Color = "Yellow",
                                            },
                                            [SpecieId.PersianAlola] = new Species
                                            {
                                                Id = SpecieId.PersianAlola,
                                                Num = 53,
                                                Name = "Persian-Alola",
                                                BaseSpecies = SpecieId.Persian,
                                                Forme = FormeId.Alola,
                                                Types = [PokemonType.Dark],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 65,
                                                    Atk = 60,
                                                    Def = 60,
                                                    SpA = 75,
                                                    SpD = 65,
                                                    Spe = 115,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.FurCoat,
                                                    Slot1 = AbilityId.Technician,
                                                    Hidden = AbilityId.Rattled,
                                                },
                                                HeightM = 1.1,
                                                WeightKg = 33,
                                                Color = "Blue",
                                            },
                                            [SpecieId.Psyduck] = new Species
                                            {
                                                Id = SpecieId.Psyduck,
                                                Num = 54,
                                                Name = "Psyduck",
                                                Types = [PokemonType.Water],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 50,
                                                    Atk = 52,
                                                    Def = 48,
                                                    SpA = 65,
                                                    SpD = 50,
                                                    Spe = 55,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Damp,
                                                    Slot1 = AbilityId.CloudNine,
                                                    Hidden = AbilityId.SwiftSwim,
                                                },
                                                HeightM = 0.8,
                                                WeightKg = 19.6,
                                                Color = "Yellow",
                                            },
                                            [SpecieId.Golduck] = new Species
                                            {
                                                Id = SpecieId.Golduck,
                                                Num = 55,
                                                Name = "Golduck",
                                                Types = [PokemonType.Water],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 80,
                                                    Atk = 82,
                                                    Def = 78,
                                                    SpA = 95,
                                                    SpD = 80,
                                                    Spe = 85,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Damp,
                                                    Slot1 = AbilityId.CloudNine,
                                                    Hidden = AbilityId.SwiftSwim,
                                                },
                                                HeightM = 1.7,
                                                WeightKg = 76.6,
                                                Color = "Blue",
                                            },
                                            [SpecieId.Mankey] = new Species
                                            {
                                                Id = SpecieId.Mankey,
                                                Num = 56,
                                                Name = "Mankey",
                                                Types = [PokemonType.Fighting],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 40,
                                                    Atk = 80,
                                                    Def = 35,
                                                    SpA = 35,
                                                    SpD = 45,
                                                    Spe = 70,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.VitalSpirit,
                                                    Slot1 = AbilityId.AngerPoint,
                                                    Hidden = AbilityId.Defiant,
                                                },
                                                HeightM = 0.5,
                                                WeightKg = 28,
                                                Color = "Brown",
                                            },
                                            [SpecieId.Primeape] = new Species
                                            {
                                                Id = SpecieId.Primeape,
                                                Num = 57,
                                                Name = "Primeape",
                                                Types = [PokemonType.Fighting],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 65,
                                                    Atk = 105,
                                                    Def = 60,
                                                    SpA = 60,
                                                    SpD = 70,
                                                    Spe = 95,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.VitalSpirit,
                                                    Slot1 = AbilityId.AngerPoint,
                                                    Hidden = AbilityId.Defiant,
                                                },
                                                HeightM = 1,
                                                WeightKg = 32,
                                                Color = "Brown",
                                            },
                                            [SpecieId.Growlithe] = new Species
                                            {
                                                Id = SpecieId.Growlithe,
                                                Num = 58,
                                                Name = "Growlithe",
                                                Types = [PokemonType.Fire],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 55,
                                                    Atk = 70,
                                                    Def = 45,
                                                    SpA = 70,
                                                    SpD = 50,
                                                    Spe = 60,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Intimidate,
                                                    Slot1 = AbilityId.FlashFire,
                                                    Hidden = AbilityId.Justified,
                                                },
                                                HeightM = 0.7,
                                                WeightKg = 19,
                                                Color = "Brown",
                                            },
                                            [SpecieId.GrowlitheHisui] = new Species
                                            {
                                                Id = SpecieId.GrowlitheHisui,
                                                Num = 58,
                                                Name = "Growlithe-Hisui",
                                                BaseSpecies = SpecieId.Growlithe,
                                                Forme = FormeId.Hisui,
                                                Types = [PokemonType.Fire, PokemonType.Rock],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 60,
                                                    Atk = 75,
                                                    Def = 45,
                                                    SpA = 65,
                                                    SpD = 50,
                                                    Spe = 55,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Intimidate,
                                                    Slot1 = AbilityId.FlashFire,
                                                    Hidden = AbilityId.RockHead,
                                                },
                                                HeightM = 0.8,
                                                WeightKg = 22.7,
                                                Color = "Brown",
                                            },
                                            [SpecieId.Arcanine] = new Species
                                            {
                                                Id = SpecieId.Arcanine,
                                                Num = 59,
                                                Name = "Arcanine",
                                                Types = [PokemonType.Fire],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 90,
                                                    Atk = 110,
                                                    Def = 80,
                                                    SpA = 100,
                                                    SpD = 80,
                                                    Spe = 95,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Intimidate,
                                                    Slot1 = AbilityId.FlashFire,
                                                    Hidden = AbilityId.Justified,
                                                },
                                                HeightM = 1.9,
                                                WeightKg = 155,
                                                Color = "Brown",
                                            },
                                            [SpecieId.ArcanineHisui] = new Species
                                            {
                                                Id = SpecieId.ArcanineHisui,
                                                Num = 59,
                                                Name = "Arcanine-Hisui",
                                                BaseSpecies = SpecieId.Arcanine,
                                                Forme = FormeId.Hisui,
                                                Types = [PokemonType.Fire, PokemonType.Rock],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 95,
                                                    Atk = 115,
                                                    Def = 80,
                                                    SpA = 95,
                                                    SpD = 80,
                                                    Spe = 90,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Intimidate,
                                                    Slot1 = AbilityId.FlashFire,
                                                    Hidden = AbilityId.RockHead,
                                                },
                                                    HeightM = 2,
                                                    WeightKg = 168,
                                                    Color = "Brown",
                                                },
                                                [SpecieId.Poliwag] = new Species
                                                {
                                                    Id = SpecieId.Poliwag,
                                                    Num = 60,
                                                    Name = "Poliwag",
                                                    Types = [PokemonType.Water],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 40,
                                                        Atk = 50,
                                                        Def = 40,
                                                        SpA = 40,
                                                        SpD = 40,
                                                        Spe = 90,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.WaterAbsorb,
                                                        Slot1 = AbilityId.Damp,
                                                        Hidden = AbilityId.SwiftSwim,
                                                    },
                                                    HeightM = 0.6,
                                                    WeightKg = 12.4,
                                                    Color = "Blue",
                                                },
                                                [SpecieId.Poliwhirl] = new Species
                                                {
                                                    Id = SpecieId.Poliwhirl,
                                                    Num = 61,
                                                    Name = "Poliwhirl",
                                                    Types = [PokemonType.Water],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 65,
                                                        Atk = 65,
                                                        Def = 65,
                                                        SpA = 50,
                                                        SpD = 50,
                                                        Spe = 90,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.WaterAbsorb,
                                                        Slot1 = AbilityId.Damp,
                                                        Hidden = AbilityId.SwiftSwim,
                                                    },
                                                    HeightM = 1,
                                                    WeightKg = 20,
                                                    Color = "Blue",
                                                },
                                                [SpecieId.Poliwrath] = new Species
                                                {
                                                    Id = SpecieId.Poliwrath,
                                                    Num = 62,
                                                    Name = "Poliwrath",
                                                    Types = [PokemonType.Water, PokemonType.Fighting],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 90,
                                                        Atk = 95,
                                                        Def = 95,
                                                        SpA = 70,
                                                        SpD = 90,
                                                        Spe = 70,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.WaterAbsorb,
                                                        Slot1 = AbilityId.Damp,
                                                        Hidden = AbilityId.SwiftSwim,
                                                    },
                                                    HeightM = 1.3,
                                                    WeightKg = 54,
                                                    Color = "Blue",
                                                },
                                                [SpecieId.Abra] = new Species
                                                {
                                                    Id = SpecieId.Abra,
                                                    Num = 63,
                                                    Name = "Abra",
                                                    Types = [PokemonType.Psychic],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 25,
                                                        Atk = 20,
                                                        Def = 15,
                                                        SpA = 105,
                                                        SpD = 55,
                                                        Spe = 90,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Synchronize,
                                                        Slot1 = AbilityId.InnerFocus,
                                                        Hidden = AbilityId.MagicGuard,
                                                    },
                                                    HeightM = 0.9,
                                                    WeightKg = 19.5,
                                                    Color = "Brown",
                                                },
                                                [SpecieId.Kadabra] = new Species
                                                {
                                                    Id = SpecieId.Kadabra,
                                                    Num = 64,
                                                    Name = "Kadabra",
                                                    Types = [PokemonType.Psychic],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 40,
                                                        Atk = 35,
                                                        Def = 30,
                                                        SpA = 120,
                                                        SpD = 70,
                                                        Spe = 105,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Synchronize,
                                                        Slot1 = AbilityId.InnerFocus,
                                                        Hidden = AbilityId.MagicGuard,
                                                    },
                                                    HeightM = 1.3,
                                                    WeightKg = 56.5,
                                                    Color = "Brown",
                                                },
                                                [SpecieId.Alakazam] = new Species
                                                {
                                                    Id = SpecieId.Alakazam,
                                                    Num = 65,
                                                    Name = "Alakazam",
                                                    Types = [PokemonType.Psychic],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 55,
                                                        Atk = 50,
                                                        Def = 45,
                                                        SpA = 135,
                                                        SpD = 95,
                                                        Spe = 120,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Synchronize,
                                                        Slot1 = AbilityId.InnerFocus,
                                                        Hidden = AbilityId.MagicGuard,
                                                    },
                                                    HeightM = 1.5,
                                                    WeightKg = 48,
                                                    Color = "Brown",
                                                },
                                                [SpecieId.AlakazamMega] = new Species
                                                {
                                                    Id = SpecieId.AlakazamMega,
                                                    Num = 65,
                                                    Name = "Alakazam-Mega",
                                                    BaseSpecies = SpecieId.Alakazam,
                                                    Forme = FormeId.Mega,
                                                    Types = [PokemonType.Psychic],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 55,
                                                        Atk = 50,
                                                        Def = 65,
                                                        SpA = 175,
                                                        SpD = 105,
                                                        Spe = 150,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Trace,
                                                    },
                                                    HeightM = 1.2,
                                                    WeightKg = 48,
                                                    Color = "Brown",
                                                },
                                                [SpecieId.Machop] = new Species
                                                {
                                                    Id = SpecieId.Machop,
                                                    Num = 66,
                                                    Name = "Machop",
                                                    Types = [PokemonType.Fighting],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 70,
                                                        Atk = 80,
                                                        Def = 50,
                                                        SpA = 35,
                                                        SpD = 35,
                                                        Spe = 35,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Guts,
                                                        Slot1 = AbilityId.NoGuard,
                                                        Hidden = AbilityId.Steadfast,
                                                    },
                                                    HeightM = 0.8,
                                                    WeightKg = 19.5,
                                                    Color = "Gray",
                                                },
                                                [SpecieId.Machoke] = new Species
                                                {
                                                    Id = SpecieId.Machoke,
                                                    Num = 67,
                                                    Name = "Machoke",
                                                    Types = [PokemonType.Fighting],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 80,
                                                        Atk = 100,
                                                        Def = 70,
                                                        SpA = 50,
                                                        SpD = 60,
                                                        Spe = 45,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Guts,
                                                        Slot1 = AbilityId.NoGuard,
                                                        Hidden = AbilityId.Steadfast,
                                                    },
                                                    HeightM = 1.5,
                                                    WeightKg = 70.5,
                                                    Color = "Gray",
                                                },
                                                [SpecieId.Machamp] = new Species
                                                {
                                                    Id = SpecieId.Machamp,
                                                    Num = 68,
                                                    Name = "Machamp",
                                                    Types = [PokemonType.Fighting],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 90,
                                                        Atk = 130,
                                                        Def = 80,
                                                        SpA = 65,
                                                        SpD = 85,
                                                        Spe = 55,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Guts,
                                                        Slot1 = AbilityId.NoGuard,
                                                        Hidden = AbilityId.Steadfast,
                                                    },
                                                    HeightM = 1.6,
                                                    WeightKg = 130,
                                                    Color = "Gray",
                                                },
                                                [SpecieId.MachampGmax] = new Species
                                                {
                                                    Id = SpecieId.MachampGmax,
                                                    Num = 68,
                                                    Name = "Machamp-Gmax",
                                                    BaseSpecies = SpecieId.Machamp,
                                                    Forme = FormeId.Gmax,
                                                    Types = [PokemonType.Fighting],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 90,
                                                        Atk = 130,
                                                        Def = 80,
                                                        SpA = 65,
                                                        SpD = 85,
                                                        Spe = 55,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Guts,
                                                        Slot1 = AbilityId.NoGuard,
                                                        Hidden = AbilityId.Steadfast,
                                                    },
                                                    HeightM = 25,
                                                    WeightKg = 0,
                                                    Color = "Gray",
                                                },
                                                [SpecieId.Bellsprout] = new Species
                                                {
                                                    Id = SpecieId.Bellsprout,
                                                    Num = 69,
                                                    Name = "Bellsprout",
                                                    Types = [PokemonType.Grass, PokemonType.Poison],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 50,
                                                        Atk = 75,
                                                        Def = 35,
                                                        SpA = 70,
                                                        SpD = 30,
                                                        Spe = 40,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Chlorophyll,
                                                        Hidden = AbilityId.Gluttony,
                                                    },
                                                    HeightM = 0.7,
                                                    WeightKg = 4,
                                                    Color = "Green",
                                                },
                                                [SpecieId.Weepinbell] = new Species
                                                {
                                                    Id = SpecieId.Weepinbell,
                                                    Num = 70,
                                                    Name = "Weepinbell",
                                                    Types = [PokemonType.Grass, PokemonType.Poison],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 65,
                                                        Atk = 90,
                                                        Def = 50,
                                                        SpA = 85,
                                                        SpD = 45,
                                                        Spe = 55,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Chlorophyll,
                                                        Hidden = AbilityId.Gluttony,
                                                    },
                                                    HeightM = 1,
                                                    WeightKg = 6.4,
                                                    Color = "Green",
                                                },
                                                [SpecieId.Victreebel] = new Species
                                                {
                                                    Id = SpecieId.Victreebel,
                                                    Num = 71,
                                                    Name = "Victreebel",
                                                    Types = [PokemonType.Grass, PokemonType.Poison],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 80,
                                                        Atk = 105,
                                                        Def = 65,
                                                        SpA = 100,
                                                        SpD = 70,
                                                        Spe = 70,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Chlorophyll,
                                                        Hidden = AbilityId.Gluttony,
                                                    },
                                                    HeightM = 1.7,
                                                    WeightKg = 15.5,
                                                    Color = "Green",
                                                },
                                                [SpecieId.VictreebelMega] = new Species
                                                {
                                                    Id = SpecieId.VictreebelMega,
                                                    Num = 71,
                                                    Name = "Victreebel-Mega",
                                                    BaseSpecies = SpecieId.Victreebel,
                                                    Forme = FormeId.Mega,
                                                    Types = [PokemonType.Grass, PokemonType.Poison],
                                                    Gender = GenderId.Empty,
                                                    BaseStats = new StatsTable
                                                    {
                                                        Hp = 80,
                                                        Atk = 125,
                                                        Def = 85,
                                                        SpA = 135,
                                                        SpD = 95,
                                                        Spe = 70,
                                                    },
                                                    Abilities = new SpeciesAbility
                                                    {
                                                        Slot0 = AbilityId.Chlorophyll,
                                                        Hidden = AbilityId.Gluttony,
                                                    },
                                                        HeightM = 4.5,
                                                        WeightKg = 125.5,
                                                        Color = "Green",
                                                    },
                                                    [SpecieId.Tentacool] = new Species
                                                    {
                                                        Id = SpecieId.Tentacool,
                                                        Num = 72,
                                                        Name = "Tentacool",
                                                        Types = [PokemonType.Water, PokemonType.Poison],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 40,
                                                            Atk = 40,
                                                            Def = 35,
                                                            SpA = 50,
                                                            SpD = 100,
                                                            Spe = 70,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.ClearBody,
                                                            Slot1 = AbilityId.LiquidOoze,
                                                            Hidden = AbilityId.RainDish,
                                                        },
                                                        HeightM = 0.9,
                                                        WeightKg = 45.5,
                                                        Color = "Blue",
                                                    },
                                                    [SpecieId.Tentacruel] = new Species
                                                    {
                                                        Id = SpecieId.Tentacruel,
                                                        Num = 73,
                                                        Name = "Tentacruel",
                                                        Types = [PokemonType.Water, PokemonType.Poison],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 80,
                                                            Atk = 70,
                                                            Def = 65,
                                                            SpA = 80,
                                                            SpD = 120,
                                                            Spe = 100,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.ClearBody,
                                                            Slot1 = AbilityId.LiquidOoze,
                                                            Hidden = AbilityId.RainDish,
                                                        },
                                                        HeightM = 1.6,
                                                        WeightKg = 55,
                                                        Color = "Blue",
                                                    },
                                                    [SpecieId.Geodude] = new Species
                                                    {
                                                        Id = SpecieId.Geodude,
                                                        Num = 74,
                                                        Name = "Geodude",
                                                        Types = [PokemonType.Rock, PokemonType.Ground],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 40,
                                                            Atk = 80,
                                                            Def = 100,
                                                            SpA = 30,
                                                            SpD = 30,
                                                            Spe = 20,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.RockHead,
                                                            Slot1 = AbilityId.Sturdy,
                                                            Hidden = AbilityId.SandVeil,
                                                        },
                                                        HeightM = 0.4,
                                                        WeightKg = 20,
                                                        Color = "Brown",
                                                    },
                                                    [SpecieId.Geodudealola] = new Species
                                                    {
                                                        Id = SpecieId.Geodudealola,
                                                        Num = 74,
                                                        Name = "Geodude-Alola",
                                                        BaseSpecies = SpecieId.Geodude,
                                                        Forme = FormeId.Alola,
                                                        Types = [PokemonType.Rock, PokemonType.Electric],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 40,
                                                            Atk = 80,
                                                            Def = 100,
                                                            SpA = 30,
                                                            SpD = 30,
                                                            Spe = 20,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.MagnetPull,
                                                            Slot1 = AbilityId.Sturdy,
                                                            Hidden = AbilityId.Galvanize,
                                                        },
                                                        HeightM = 0.4,
                                                        WeightKg = 20.3,
                                                        Color = "Gray",
                                                    },
                                                    [SpecieId.Graveler] = new Species
                                                    {
                                                        Id = SpecieId.Graveler,
                                                        Num = 75,
                                                        Name = "Graveler",
                                                        Types = [PokemonType.Rock, PokemonType.Ground],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 55,
                                                            Atk = 95,
                                                            Def = 115,
                                                            SpA = 45,
                                                            SpD = 45,
                                                            Spe = 35,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.RockHead,
                                                            Slot1 = AbilityId.Sturdy,
                                                            Hidden = AbilityId.SandVeil,
                                                        },
                                                        HeightM = 1,
                                                        WeightKg = 105,
                                                        Color = "Brown",
                                                    },
                                                    [SpecieId.GravelerAlola] = new Species
                                                    {
                                                        Id = SpecieId.GravelerAlola,
                                                        Num = 75,
                                                        Name = "Graveler-Alola",
                                                        BaseSpecies = SpecieId.Graveler,
                                                        Forme = FormeId.Alola,
                                                        Types = [PokemonType.Rock, PokemonType.Electric],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 55,
                                                            Atk = 95,
                                                            Def = 115,
                                                            SpA = 45,
                                                            SpD = 45,
                                                            Spe = 35,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.MagnetPull,
                                                            Slot1 = AbilityId.Sturdy,
                                                            Hidden = AbilityId.Galvanize,
                                                        },
                                                        HeightM = 1,
                                                        WeightKg = 110,
                                                        Color = "Gray",
                                                    },
                                                    [SpecieId.Golem] = new Species
                                                    {
                                                        Id = SpecieId.Golem,
                                                        Num = 76,
                                                        Name = "Golem",
                                                        Types = [PokemonType.Rock, PokemonType.Ground],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 80,
                                                            Atk = 120,
                                                            Def = 130,
                                                            SpA = 55,
                                                            SpD = 65,
                                                            Spe = 45,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.RockHead,
                                                            Slot1 = AbilityId.Sturdy,
                                                            Hidden = AbilityId.SandVeil,
                                                        },
                                                        HeightM = 1.4,
                                                        WeightKg = 300,
                                                        Color = "Brown",
                                                    },
                                                    [SpecieId.GolemAlola] = new Species
                                                    {
                                                        Id = SpecieId.GolemAlola,
                                                        Num = 76,
                                                        Name = "Golem-Alola",
                                                        BaseSpecies = SpecieId.Golem,
                                                        Forme = FormeId.Alola,
                                                        Types = [PokemonType.Rock, PokemonType.Electric],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 80,
                                                            Atk = 120,
                                                            Def = 130,
                                                            SpA = 55,
                                                            SpD = 65,
                                                            Spe = 45,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.MagnetPull,
                                                            Slot1 = AbilityId.Sturdy,
                                                            Hidden = AbilityId.Galvanize,
                                                        },
                                                        HeightM = 1.7,
                                                        WeightKg = 316,
                                                        Color = "Gray",
                                                    },
                                                    [SpecieId.Ponyta] = new Species
                                                    {
                                                        Id = SpecieId.Ponyta,
                                                        Num = 77,
                                                        Name = "Ponyta",
                                                        Types = [PokemonType.Fire],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 50,
                                                            Atk = 85,
                                                            Def = 55,
                                                            SpA = 65,
                                                            SpD = 65,
                                                            Spe = 90,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.RunAway,
                                                            Slot1 = AbilityId.FlashFire,
                                                            Hidden = AbilityId.FlameBody,
                                                        },
                                                        HeightM = 1,
                                                        WeightKg = 30,
                                                        Color = "Yellow",
                                                    },
                                                    [SpecieId.PonytaGalar] = new Species
                                                    {
                                                        Id = SpecieId.PonytaGalar,
                                                        Num = 77,
                                                        Name = "Ponyta-Galar",
                                                        BaseSpecies = SpecieId.Ponyta,
                                                        Forme = FormeId.Galar,
                                                        Types = [PokemonType.Psychic],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 50,
                                                            Atk = 85,
                                                            Def = 55,
                                                            SpA = 65,
                                                            SpD = 65,
                                                            Spe = 90,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.RunAway,
                                                            Slot1 = AbilityId.PastelVeil,
                                                            Hidden = AbilityId.Anticipation,
                                                        },
                                                        HeightM = 0.8,
                                                        WeightKg = 24,
                                                        Color = "White",
                                                    },
                                                    [SpecieId.Rapidash] = new Species
                                                    {
                                                        Id = SpecieId.Rapidash,
                                                        Num = 78,
                                                        Name = "Rapidash",
                                                        Types = [PokemonType.Fire],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 65,
                                                            Atk = 100,
                                                            Def = 70,
                                                            SpA = 80,
                                                            SpD = 80,
                                                            Spe = 105,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.RunAway,
                                                            Slot1 = AbilityId.FlashFire,
                                                            Hidden = AbilityId.FlameBody,
                                                        },
                                                        HeightM = 1.7,
                                                        WeightKg = 95,
                                                        Color = "Yellow",
                                                    },
                                                    [SpecieId.RapidashGalar] = new Species
                                                    {
                                                        Id = SpecieId.RapidashGalar,
                                                        Num = 78,
                                                        Name = "Rapidash-Galar",
                                                        BaseSpecies = SpecieId.Rapidash,
                                                        Forme = FormeId.Galar,
                                                        Types = [PokemonType.Psychic, PokemonType.Fairy],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 65,
                                                            Atk = 100,
                                                            Def = 70,
                                                            SpA = 80,
                                                            SpD = 80,
                                                            Spe = 105,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.RunAway,
                                                            Slot1 = AbilityId.PastelVeil,
                                                            Hidden = AbilityId.Anticipation,
                                                        },
                                                        HeightM = 1.7,
                                                        WeightKg = 80,
                                                        Color = "White",
                                                    },
                                                    [SpecieId.Slowpoke] = new Species
                                                    {
                                                        Id = SpecieId.Slowpoke,
                                                        Num = 79,
                                                        Name = "Slowpoke",
                                                        Types = [PokemonType.Water, PokemonType.Psychic],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 90,
                                                            Atk = 65,
                                                            Def = 65,
                                                            SpA = 40,
                                                            SpD = 40,
                                                            Spe = 15,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.Oblivious,
                                                            Slot1 = AbilityId.OwnTempo,
                                                            Hidden = AbilityId.Regenerator,
                                                        },
                                                        HeightM = 1.2,
                                                        WeightKg = 36,
                                                        Color = "Pink",
                                                    },
                                                    [SpecieId.SlowpokeGalar] = new Species
                                                    {
                                                        Id = SpecieId.SlowpokeGalar,
                                                        Num = 79,
                                                        Name = "Slowpoke-Galar",
                                                        BaseSpecies = SpecieId.Slowpoke,
                                                        Forme = FormeId.Galar,
                                                        Types = [PokemonType.Psychic],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 90,
                                                            Atk = 65,
                                                            Def = 65,
                                                            SpA = 40,
                                                            SpD = 40,
                                                            Spe = 15,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.Gluttony,
                                                            Slot1 = AbilityId.OwnTempo,
                                                            Hidden = AbilityId.Regenerator,
                                                        },
                                                        HeightM = 1.2,
                                                        WeightKg = 36,
                                                        Color = "Pink",
                                                    },
                                                    [SpecieId.Slowbro] = new Species
                                                    {
                                                        Id = SpecieId.Slowbro,
                                                        Num = 80,
                                                        Name = "Slowbro",
                                                        Types = [PokemonType.Water, PokemonType.Psychic],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 95,
                                                            Atk = 75,
                                                            Def = 110,
                                                            SpA = 100,
                                                            SpD = 80,
                                                            Spe = 30,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.Oblivious,
                                                            Slot1 = AbilityId.OwnTempo,
                                                            Hidden = AbilityId.Regenerator,
                                                        },
                                                        HeightM = 1.6,
                                                        WeightKg = 78.5,
                                                        Color = "Pink",
                                                    },
                                                    [SpecieId.SlowbroMega] = new Species
                                                    {
                                                        Id = SpecieId.SlowbroMega,
                                                        Num = 80,
                                                        Name = "Slowbro-Mega",
                                                        BaseSpecies = SpecieId.Slowbro,
                                                        Forme = FormeId.Mega,
                                                        Types = [PokemonType.Water, PokemonType.Psychic],
                                                        Gender = GenderId.Empty,
                                                        BaseStats = new StatsTable
                                                        {
                                                            Hp = 95,
                                                            Atk = 75,
                                                            Def = 180,
                                                            SpA = 130,
                                                            SpD = 80,
                                                            Spe = 30,
                                                        },
                                                        Abilities = new SpeciesAbility
                                                        {
                                                            Slot0 = AbilityId.ShellArmor,
                                                        },
                                                        HeightM = 2,
                                                        WeightKg = 120,
                                                        Color = "Pink",
                                                    },
                                                    [SpecieId.SlowbroGalar] = new Species
                                                                                    {
                                                                                        Id = SpecieId.SlowbroGalar,
                                                                                        Num = 80,
                                                                                        Name = "Slowbro-Galar",
                                                                                        BaseSpecies = SpecieId.Slowbro,
                                                                                        Forme = FormeId.Galar,
                                                                                        Types = [PokemonType.Poison, PokemonType.Psychic],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 95,
                                                                                            Atk = 100,
                                                                                            Def = 95,
                                                                                            SpA = 100,
                                                                                            SpD = 70,
                                                                                            Spe = 30,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.QuickDraw,
                                                                                            Slot1 = AbilityId.OwnTempo,
                                                                                            Hidden = AbilityId.Regenerator,
                                                                                        },
                                                                                        HeightM = 1.6,
                                                                                        WeightKg = 70.5,
                                                                                        Color = "Pink",
                                                                                    },
                                                                                    [SpecieId.Magnemite] = new Species
                                                                                    {
                                                                                        Id = SpecieId.Magnemite,
                                                                                        Num = 81,
                                                                                        Name = "Magnemite",
                                                                                        Types = [PokemonType.Electric, PokemonType.Steel],
                                                                                        Gender = GenderId.N,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 25,
                                                                                            Atk = 35,
                                                                                            Def = 70,
                                                                                            SpA = 95,
                                                                                            SpD = 55,
                                                                                            Spe = 45,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.MagnetPull,
                                                                                            Slot1 = AbilityId.Sturdy,
                                                                                            Hidden = AbilityId.Analytic,
                                                                                        },
                                                                                        HeightM = 0.3,
                                                                                        WeightKg = 6,
                                                                                        Color = "Gray",
                                                                                    },
                                                                                    [SpecieId.Magneton] = new Species
                                                                                    {
                                                                                        Id = SpecieId.Magneton,
                                                                                        Num = 82,
                                                                                        Name = "Magneton",
                                                                                        Types = [PokemonType.Electric, PokemonType.Steel],
                                                                                        Gender = GenderId.N,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 50,
                                                                                            Atk = 60,
                                                                                            Def = 95,
                                                                                            SpA = 120,
                                                                                            SpD = 70,
                                                                                            Spe = 70,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.MagnetPull,
                                                                                            Slot1 = AbilityId.Sturdy,
                                                                                            Hidden = AbilityId.Analytic,
                                                                                        },
                                                                                        HeightM = 1,
                                                                                        WeightKg = 60,
                                                                                        Color = "Gray",
                                                                                    },
                                                                                    [SpecieId.Farfetchd] = new Species
                                                                                    {
                                                                                        Id = SpecieId.Farfetchd,
                                                                                        Num = 83,
                                                                                        Name = "Farfetch\u2019d",
                                                                                        Types = [PokemonType.Normal, PokemonType.Flying],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 52,
                                                                                            Atk = 90,
                                                                                            Def = 55,
                                                                                            SpA = 58,
                                                                                            SpD = 62,
                                                                                            Spe = 60,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.KeenEye,
                                                                                            Slot1 = AbilityId.InnerFocus,
                                                                                            Hidden = AbilityId.Defiant,
                                                                                        },
                                                                                        HeightM = 0.8,
                                                                                        WeightKg = 15,
                                                                                        Color = "Brown",
                                                                                    },
                                                                                    [SpecieId.FarfetchdGalar] = new Species
                                                                                    {
                                                                                        Id = SpecieId.FarfetchdGalar,
                                                                                        Num = 83,
                                                                                        Name = "Farfetch\u2019d-Galar",
                                                                                        BaseSpecies = SpecieId.Farfetchd,
                                                                                        Forme = FormeId.Galar,
                                                                                        Types = [PokemonType.Fighting],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 52,
                                                                                            Atk = 95,
                                                                                            Def = 55,
                                                                                            SpA = 58,
                                                                                            SpD = 62,
                                                                                            Spe = 55,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.Steadfast,
                                                                                            Hidden = AbilityId.Scrappy,
                                                                                        },
                                                                                        HeightM = 0.8,
                                                                                        WeightKg = 42,
                                                                                        Color = "Brown",
                                                                                    },
                                                                                    [SpecieId.Doduo] = new Species
                                                                                    {
                                                                                        Id = SpecieId.Doduo,
                                                                                        Num = 84,
                                                                                        Name = "Doduo",
                                                                                        Types = [PokemonType.Normal, PokemonType.Flying],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 35,
                                                                                            Atk = 85,
                                                                                            Def = 45,
                                                                                            SpA = 35,
                                                                                            SpD = 35,
                                                                                            Spe = 75,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.RunAway,
                                                                                            Slot1 = AbilityId.EarlyBird,
                                                                                            Hidden = AbilityId.TangledFeet,
                                                                                        },
                                                                                        HeightM = 1.4,
                                                                                        WeightKg = 39.2,
                                                                                        Color = "Brown",
                                                                                    },
                                                                                    [SpecieId.Dodrio] = new Species
                                                                                    {
                                                                                        Id = SpecieId.Dodrio,
                                                                                        Num = 85,
                                                                                        Name = "Dodrio",
                                                                                        Types = [PokemonType.Normal, PokemonType.Flying],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 60,
                                                                                            Atk = 110,
                                                                                            Def = 70,
                                                                                            SpA = 60,
                                                                                            SpD = 60,
                                                                                            Spe = 110,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.RunAway,
                                                                                            Slot1 = AbilityId.EarlyBird,
                                                                                            Hidden = AbilityId.TangledFeet,
                                                                                        },
                                                                                        HeightM = 1.8,
                                                                                        WeightKg = 85.2,
                                                                                        Color = "Brown",
                                                                                    },
                                                                                    [SpecieId.Seel] = new Species
                                                                                    {
                                                                                        Id = SpecieId.Seel,
                                                                                        Num = 86,
                                                                                        Name = "Seel",
                                                                                        Types = [PokemonType.Water],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 65,
                                                                                            Atk = 45,
                                                                                            Def = 55,
                                                                                            SpA = 45,
                                                                                            SpD = 70,
                                                                                            Spe = 45,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.ThickFat,
                                                                                            Slot1 = AbilityId.Hydration,
                                                                                            Hidden = AbilityId.IceBody,
                                                                                        },
                                                                                        HeightM = 1.1,
                                                                                        WeightKg = 90,
                                                                                        Color = "White",
                                                                                    },
                                                                                    [SpecieId.Dewgong] = new Species
                                                                                    {
                                                                                        Id = SpecieId.Dewgong,
                                                                                        Num = 87,
                                                                                        Name = "Dewgong",
                                                                                        Types = [PokemonType.Water, PokemonType.Ice],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 90,
                                                                                            Atk = 70,
                                                                                            Def = 80,
                                                                                            SpA = 70,
                                                                                            SpD = 95,
                                                                                            Spe = 70,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.ThickFat,
                                                                                            Slot1 = AbilityId.Hydration,
                                                                                            Hidden = AbilityId.IceBody,
                                                                                        },
                                                                                        HeightM = 1.7,
                                                                                        WeightKg = 120,
                                                                                        Color = "White",
                                                                                    },
                                                                                    [SpecieId.Grimer] = new Species
                                                                                    {
                                                                                        Id = SpecieId.Grimer,
                                                                                        Num = 88,
                                                                                        Name = "Grimer",
                                                                                        Types = [PokemonType.Poison],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 80,
                                                                                            Atk = 80,
                                                                                            Def = 50,
                                                                                            SpA = 40,
                                                                                            SpD = 50,
                                                                                            Spe = 25,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.Stench,
                                                                                            Slot1 = AbilityId.StickyHold,
                                                                                            Hidden = AbilityId.PoisonTouch,
                                                                                        },
                                                                                        HeightM = 0.9,
                                                                                        WeightKg = 30,
                                                                                        Color = "Purple",
                                                                                    },
                                                                                    [SpecieId.GrimerAlola] = new Species
                                                                                    {
                                                                                        Id = SpecieId.GrimerAlola,
                                                                                        Num = 88,
                                                                                        Name = "Grimer-Alola",
                                                                                        BaseSpecies = SpecieId.Grimer,
                                                                                        Forme = FormeId.Alola,
                                                                                        Types = [PokemonType.Poison, PokemonType.Dark],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 80,
                                                                                            Atk = 80,
                                                                                            Def = 50,
                                                                                            SpA = 40,
                                                                                            SpD = 50,
                                                                                            Spe = 25,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.PoisonTouch,
                                                                                            Slot1 = AbilityId.Gluttony,
                                                                                            Hidden = AbilityId.PowerOfAlchemy,
                                                                                        },
                                                                                        HeightM = 0.7,
                                                                                        WeightKg = 42,
                                                                                        Color = "Green",
                                                                                    },
                                                                                    [SpecieId.Muk] = new Species
                                                                                    {
                                                                                        Id = SpecieId.Muk,
                                                                                        Num = 89,
                                                                                        Name = "Muk",
                                                                                        Types = [PokemonType.Poison],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 105,
                                                                                            Atk = 105,
                                                                                            Def = 75,
                                                                                            SpA = 65,
                                                                                            SpD = 100,
                                                                                            Spe = 50,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.Stench,
                                                                                            Slot1 = AbilityId.StickyHold,
                                                                                            Hidden = AbilityId.PoisonTouch,
                                                                                        },
                                                                                        HeightM = 1.2,
                                                                                        WeightKg = 30,
                                                                                        Color = "Purple",
                                                                                    },
                                                                                    [SpecieId.MukAlola] = new Species
                                                                                    {
                                                                                        Id = SpecieId.MukAlola,
                                                                                        Num = 89,
                                                                                        Name = "Muk-Alola",
                                                                                        BaseSpecies = SpecieId.Muk,
                                                                                        Forme = FormeId.Alola,
                                                                                        Types = [PokemonType.Poison, PokemonType.Dark],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 105,
                                                                                            Atk = 105,
                                                                                            Def = 75,
                                                                                            SpA = 65,
                                                                                            SpD = 100,
                                                                                            Spe = 50,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.PoisonTouch,
                                                                                            Slot1 = AbilityId.Gluttony,
                                                                                            Hidden = AbilityId.PowerOfAlchemy,
                                                                                        },
                                                                                        HeightM = 1,
                                                                                        WeightKg = 52,
                                                                                        Color = "Green",
                                                                                    },
                                                                                    [SpecieId.Shellder] = new Species
                                                                                    {
                                                                                        Id = SpecieId.Shellder,
                                                                                        Num = 90,
                                                                                        Name = "Shellder",
                                                                                        Types = [PokemonType.Water],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 30,
                                                                                            Atk = 65,
                                                                                            Def = 100,
                                                                                            SpA = 45,
                                                                                            SpD = 25,
                                                                                            Spe = 40,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.ShellArmor,
                                                                                            Slot1 = AbilityId.SkillLink,
                                                                                            Hidden = AbilityId.Overcoat,
                                                                                        },
                                                                                        HeightM = 0.3,
                                                                                        WeightKg = 4,
                                                                                        Color = "Purple",
                                                                                    },
                                                                                    [SpecieId.Cloyster] = new Species
                                                                                    {
                                                                                        Id = SpecieId.Cloyster,
                                                                                        Num = 91,
                                                                                        Name = "Cloyster",
                                                                                        Types = [PokemonType.Water, PokemonType.Ice],
                                                                                        Gender = GenderId.Empty,
                                                                                        BaseStats = new StatsTable
                                                                                        {
                                                                                            Hp = 50,
                                                                                            Atk = 95,
                                                                                            Def = 180,
                                                                                            SpA = 85,
                                                                                            SpD = 45,
                                                                                            Spe = 70,
                                                                                        },
                                                                                        Abilities = new SpeciesAbility
                                                                                        {
                                                                                            Slot0 = AbilityId.ShellArmor,
                                                                                            Slot1 = AbilityId.SkillLink,
                                                                                            Hidden = AbilityId.Overcoat,
                                                                                        },
                                                                                        HeightM = 1.5,
                                                                                        WeightKg = 132.5,
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