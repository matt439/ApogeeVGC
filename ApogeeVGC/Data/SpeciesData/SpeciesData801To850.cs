using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData801To850()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Magearna] = new()
            {
                Id = SpecieId.Magearna,
                Num = 801,
                Name = "Magearna",
                Types = [PokemonType.Steel, PokemonType.Fairy],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 95,
                    Def = 115,
                    SpA = 130,
                    SpD = 115,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SoulHeart,
                },
                HeightM = 1.0,
                WeightKg = 80.5,
                Color = "Gray",
            },
            [SpecieId.MagearnaOriginal] = new()
            {
                Id = SpecieId.MagearnaOriginal,
                Num = 801,
                Name = "Magearna-Original",
                BaseSpecies = SpecieId.Magearna,
                Forme = FormeId.Original,
                Types = [PokemonType.Steel, PokemonType.Fairy],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 95,
                    Def = 115,
                    SpA = 130,
                    SpD = 115,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SoulHeart,
                },
                HeightM = 1.0,
                WeightKg = 80.5,
                Color = "Red",
            },
            [SpecieId.Marshadow] = new()
            {
                Id = SpecieId.Marshadow,
                Num = 802,
                Name = "Marshadow",
                Types = [PokemonType.Fighting, PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 125,
                    Def = 80,
                    SpA = 90,
                    SpD = 90,
                    Spe = 125,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Technician,
                },
                HeightM = 0.7,
                WeightKg = 22.2,
                Color = "Gray",
            },
            [SpecieId.Poipole] = new()
            {
                Id = SpecieId.Poipole,
                Num = 803,
                Name = "Poipole",
                Types = [PokemonType.Poison],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 67,
                    Atk = 73,
                    Def = 67,
                    SpA = 73,
                    SpD = 67,
                    Spe = 73,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BeastBoost,
                },
                HeightM = 0.6,
                WeightKg = 1.8,
                Color = "Purple",
            },
            [SpecieId.Naganadel] = new()
            {
                Id = SpecieId.Naganadel,
                Num = 804,
                Name = "Naganadel",
                Types = [PokemonType.Poison, PokemonType.Dragon],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 73,
                    Atk = 73,
                    Def = 73,
                    SpA = 127,
                    SpD = 73,
                    Spe = 121,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BeastBoost,
                },
                HeightM = 3.6,
                WeightKg = 150,
                Color = "Purple",
            },
            [SpecieId.Stakataka] = new()
            {
                Id = SpecieId.Stakataka,
                Num = 805,
                Name = "Stakataka",
                Types = [PokemonType.Rock, PokemonType.Steel],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 61,
                    Atk = 131,
                    Def = 211,
                    SpA = 53,
                    SpD = 101,
                    Spe = 13,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BeastBoost,
                },
                HeightM = 5.5,
                WeightKg = 820,
                Color = "Gray",
            },
            [SpecieId.Blacephalon] = new()
            {
                Id = SpecieId.Blacephalon,
                Num = 806,
                Name = "Blacephalon",
                Types = [PokemonType.Fire, PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 53,
                    Atk = 127,
                    Def = 53,
                    SpA = 151,
                    SpD = 79,
                    Spe = 107,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BeastBoost,
                },
                HeightM = 1.8,
                WeightKg = 13,
                Color = "White",
            },
            [SpecieId.Zeraora] = new()
            {
                Id = SpecieId.Zeraora,
                Num = 807,
                Name = "Zeraora",
                Types = [PokemonType.Electric],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 88,
                    Atk = 112,
                    Def = 75,
                    SpA = 102,
                    SpD = 80,
                    Spe = 143,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VoltAbsorb,
                },
                HeightM = 1.5,
                WeightKg = 44.5,
                Color = "Yellow",
            },
            [SpecieId.Meltan] = new()
            {
                Id = SpecieId.Meltan,
                Num = 808,
                Name = "Meltan",
                Types = [PokemonType.Steel],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 46,
                    Atk = 65,
                    Def = 65,
                    SpA = 55,
                    SpD = 35,
                    Spe = 34,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagnetPull,
                },
                HeightM = 0.2,
                WeightKg = 8,
                Color = "Gray",
            },
            [SpecieId.Melmetal] = new()
            {
                Id = SpecieId.Melmetal,
                Num = 809,
                Name = "Melmetal",
                Types = [PokemonType.Steel],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 135,
                    Atk = 143,
                    Def = 143,
                    SpA = 80,
                    SpD = 65,
                    Spe = 34,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.IronFist,
                },
                HeightM = 2.5,
                WeightKg = 800,
                Color = "Gray",
            },
            [SpecieId.MelmetalGmax] = new()
            {
                Id = SpecieId.MelmetalGmax,
                Num = 809,
                Name = "Melmetal-Gmax",
                BaseSpecies = SpecieId.Melmetal,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Steel],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 135,
                    Atk = 143,
                    Def = 143,
                    SpA = 80,
                    SpD = 65,
                    Spe = 34,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.IronFist,
                },
                HeightM = 25,
                WeightKg = 0,
                Color = "Gray",
            },
            [SpecieId.Grookey] = new()
            {
                Id = SpecieId.Grookey,
                Num = 810,
                Name = "Grookey",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 50,
                    SpA = 40,
                    SpD = 40,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.GrassySurge,
                },
                HeightM = 0.3,
                WeightKg = 5,
                Color = "Green",
            },
            [SpecieId.Thwackey] = new()
            {
                Id = SpecieId.Thwackey,
                Num = 811,
                Name = "Thwackey",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 85,
                    Def = 70,
                    SpA = 55,
                    SpD = 60,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.GrassySurge,
                },
                HeightM = 0.7,
                WeightKg = 14,
                Color = "Green",
            },
            [SpecieId.Rillaboom] = new()
            {
                Id = SpecieId.Rillaboom,
                Num = 812,
                Name = "Rillaboom",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 125,
                    Def = 90,
                    SpA = 60,
                    SpD = 70,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.GrassySurge,
                },
                HeightM = 2.1,
                WeightKg = 90,
                Color = "Green",
            },
            [SpecieId.RillaboomGmax] = new()
            {
                Id = SpecieId.RillaboomGmax,
                Num = 812,
                Name = "Rillaboom-Gmax",
                BaseSpecies = SpecieId.Rillaboom,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 125,
                    Def = 90,
                    SpA = 60,
                    SpD = 70,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.GrassySurge,
                },
                HeightM = 28,
                WeightKg = 0,
                Color = "Green",
            },
            [SpecieId.Scorbunny] = new()
            {
                Id = SpecieId.Scorbunny,
                Num = 813,
                Name = "Scorbunny",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 71,
                    Def = 40,
                    SpA = 40,
                    SpD = 40,
                    Spe = 69,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Libero,
                },
                HeightM = 0.3,
                WeightKg = 4.5,
                Color = "White",
            },
            [SpecieId.Raboot] = new()
            {
                Id = SpecieId.Raboot,
                Num = 814,
                Name = "Raboot",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 86,
                    Def = 60,
                    SpA = 55,
                    SpD = 60,
                    Spe = 94,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Libero,
                },
                HeightM = 0.6,
                WeightKg = 9,
                Color = "Gray",
            },
            [SpecieId.Cinderace] = new()
            {
                Id = SpecieId.Cinderace,
                Num = 815,
                Name = "Cinderace",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 116,
                    Def = 75,
                    SpA = 65,
                    SpD = 75,
                    Spe = 119,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Libero,
                },
                HeightM = 1.4,
                WeightKg = 33,
                Color = "White",
            },
            [SpecieId.CinderaceGmax] = new()
            {
                Id = SpecieId.CinderaceGmax,
                Num = 815,
                Name = "Cinderace-Gmax",
                BaseSpecies = SpecieId.Cinderace,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 116,
                    Def = 75,
                    SpA = 65,
                    SpD = 75,
                    Spe = 119,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Libero,
                },
                HeightM = 27,
                WeightKg = 0,
                Color = "White",
            },
            [SpecieId.Sobble] = new()
            {
                Id = SpecieId.Sobble,
                Num = 816,
                Name = "Sobble",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 40,
                    Def = 40,
                    SpA = 70,
                    SpD = 40,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Sniper,
                },
                HeightM = 0.3,
                WeightKg = 4,
                Color = "Blue",
            },
            [SpecieId.Drizzile] = new()
            {
                Id = SpecieId.Drizzile,
                Num = 817,
                Name = "Drizzile",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 60,
                    Def = 55,
                    SpA = 95,
                    SpD = 55,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Sniper,
                },
                HeightM = 0.7,
                WeightKg = 11.5,
                Color = "Blue",
            },
            [SpecieId.Inteleon] = new()
            {
                Id = SpecieId.Inteleon,
                Num = 818,
                Name = "Inteleon",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 85,
                    Def = 65,
                    SpA = 125,
                    SpD = 65,
                    Spe = 120,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Sniper,
                },
                HeightM = 1.9,
                WeightKg = 45.2,
                Color = "Blue",
            },
            [SpecieId.InteleonGmax] = new()
            {
                Id = SpecieId.InteleonGmax,
                Num = 818,
                Name = "Inteleon-Gmax",
                BaseSpecies = SpecieId.Inteleon,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 85,
                    Def = 65,
                    SpA = 125,
                    SpD = 65,
                    Spe = 120,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Sniper,
                },
                HeightM = 40,
                WeightKg = 0,
                Color = "Blue",
            },
            [SpecieId.Skwovet] = new()
            {
                Id = SpecieId.Skwovet,
                Num = 819,
                Name = "Skwovet",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 55,
                    Def = 55,
                    SpA = 35,
                    SpD = 35,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CheekPouch,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 0.3,
                WeightKg = 2.5,
                Color = "Brown",
            },
            [SpecieId.Greedent] = new()
            {
                Id = SpecieId.Greedent,
                Num = 820,
                Name = "Greedent",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 120,
                    Atk = 95,
                    Def = 95,
                    SpA = 55,
                    SpD = 75,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CheekPouch,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 0.6,
                WeightKg = 6,
                Color = "Brown",
            },
            [SpecieId.Rookidee] = new()
            {
                Id = SpecieId.Rookidee,
                Num = 821,
                Name = "Rookidee",
                Types = [PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 38,
                    Atk = 47,
                    Def = 35,
                    SpA = 33,
                    SpD = 35,
                    Spe = 57,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.Unnerve,
                    Hidden = AbilityId.BigPecks,
                },
                HeightM = 0.2,
                WeightKg = 1.8,
                Color = "Blue",
            },
            [SpecieId.Corvisquire] = new()
            {
                Id = SpecieId.Corvisquire,
                Num = 822,
                Name = "Corvisquire",
                Types = [PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 67,
                    Def = 55,
                    SpA = 43,
                    SpD = 55,
                    Spe = 77,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.Unnerve,
                    Hidden = AbilityId.BigPecks,
                },
                HeightM = 0.8,
                WeightKg = 16,
                Color = "Blue",
            },
            [SpecieId.Corviknight] = new()
            {
                Id = SpecieId.Corviknight,
                Num = 823,
                Name = "Corviknight",
                Types = [PokemonType.Flying, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 98,
                    Atk = 87,
                    Def = 105,
                    SpA = 53,
                    SpD = 85,
                    Spe = 67,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Slot1 = AbilityId.Unnerve,
                    Hidden = AbilityId.MirrorArmor,
                },
                HeightM = 2.2,
                WeightKg = 75,
                Color = "Purple",
            },
            [SpecieId.CorviknightGmax] = new()
            {
                Id = SpecieId.CorviknightGmax,
                Num = 823,
                Name = "Corviknight-Gmax",
                BaseSpecies = SpecieId.Corviknight,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Flying, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 98,
                    Atk = 87,
                    Def = 105,
                    SpA = 53,
                    SpD = 85,
                    Spe = 67,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Slot1 = AbilityId.Unnerve,
                    Hidden = AbilityId.MirrorArmor,
                },
                HeightM = 14,
                WeightKg = 0,
                Color = "Purple",
            },
            [SpecieId.Blipbug] = new()
            {
                Id = SpecieId.Blipbug,
                Num = 824,
                Name = "Blipbug",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 25,
                    Atk = 20,
                    Def = 20,
                    SpA = 25,
                    SpD = 45,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Slot1 = AbilityId.CompoundEyes,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 0.4,
                WeightKg = 8,
                Color = "Blue",
            },
            [SpecieId.Dottler] = new()
            {
                Id = SpecieId.Dottler,
                Num = 825,
                Name = "Dottler",
                Types = [PokemonType.Bug, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 35,
                    Def = 80,
                    SpA = 50,
                    SpD = 90,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Slot1 = AbilityId.CompoundEyes,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 0.4,
                WeightKg = 19.5,
                Color = "Yellow",
            },
        };
    }
}
