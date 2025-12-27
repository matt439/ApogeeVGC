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
            [SpecieId.Orbeetle] = new()
            {
                Id = SpecieId.Orbeetle,
                Num = 826,
                Name = "Orbeetle",
                Types = [PokemonType.Bug, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 45,
                    Def = 110,
                    SpA = 80,
                    SpD = 120,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 0.4,
                WeightKg = 40.8,
                Color = "Red",
            },
            [SpecieId.Nickit] = new()
            {
                Id = SpecieId.Nickit,
                Num = 827,
                Name = "Nickit",
                Types = [PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 28,
                    Def = 28,
                    SpA = 47,
                    SpD = 52,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.Unburden,
                    Hidden = AbilityId.Stakeout,
                },
                HeightM = 0.6,
                WeightKg = 8.9,
                Color = "Brown",
            },
            [SpecieId.Thievul] = new()
            {
                Id = SpecieId.Thievul,
                Num = 828,
                Name = "Thievul",
                Types = [PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 58,
                    Def = 58,
                    SpA = 87,
                    SpD = 92,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.Unburden,
                    Hidden = AbilityId.Stakeout,
                },
                HeightM = 1.2,
                WeightKg = 19.9,
                Color = "Brown",
            },
            [SpecieId.Gossifleur] = new()
            {
                Id = SpecieId.Gossifleur,
                Num = 829,
                Name = "Gossifleur",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 40,
                    Def = 60,
                    SpA = 40,
                    SpD = 60,
                    Spe = 10,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CottonDown,
                    Slot1 = AbilityId.Regenerator,
                    Hidden = AbilityId.EffectSpore,
                },
                HeightM = 0.4,
                WeightKg = 2.2,
                Color = "Green",
            },
            [SpecieId.Eldegoss] = new()
            {
                Id = SpecieId.Eldegoss,
                Num = 830,
                Name = "Eldegoss",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 50,
                    Def = 90,
                    SpA = 80,
                    SpD = 120,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CottonDown,
                    Slot1 = AbilityId.Regenerator,
                    Hidden = AbilityId.EffectSpore,
                },
                HeightM = 0.5,
                WeightKg = 2.5,
                Color = "Green",
            },
            [SpecieId.Wooloo] = new()
            {
                Id = SpecieId.Wooloo,
                Num = 831,
                Name = "Wooloo",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 42,
                    Atk = 40,
                    Def = 55,
                    SpA = 40,
                    SpD = 45,
                    Spe = 48,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Fluffy,
                    Slot1 = AbilityId.RunAway,
                    Hidden = AbilityId.Bulletproof,
                },
                HeightM = 0.6,
                WeightKg = 6,
                Color = "White",
            },
            [SpecieId.Dubwool] = new()
            {
                Id = SpecieId.Dubwool,
                Num = 832,
                Name = "Dubwool",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 72,
                    Atk = 80,
                    Def = 100,
                    SpA = 60,
                    SpD = 90,
                    Spe = 88,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Fluffy,
                    Slot1 = AbilityId.Steadfast,
                    Hidden = AbilityId.Bulletproof,
                },
                HeightM = 1.3,
                WeightKg = 43,
                Color = "White",
            },
            [SpecieId.Chewtle] = new()
            {
                Id = SpecieId.Chewtle,
                Num = 833,
                Name = "Chewtle",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 64,
                    Def = 50,
                    SpA = 38,
                    SpD = 38,
                    Spe = 44,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.StrongJaw,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 0.3,
                WeightKg = 8.5,
                Color = "Green",
            },
            [SpecieId.Drednaw] = new()
            {
                Id = SpecieId.Drednaw,
                Num = 834,
                Name = "Drednaw",
                Types = [PokemonType.Water, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 115,
                    Def = 90,
                    SpA = 48,
                    SpD = 68,
                    Spe = 74,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.StrongJaw,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 1.0,
                WeightKg = 115.5,
                Color = "Green",
            },
            [SpecieId.Yamper] = new()
            {
                Id = SpecieId.Yamper,
                Num = 835,
                Name = "Yamper",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 59,
                    Atk = 45,
                    Def = 50,
                    SpA = 40,
                    SpD = 50,
                    Spe = 26,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BallFetch,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 0.3,
                WeightKg = 13.5,
                Color = "Yellow",
            },
            [SpecieId.Boltund] = new()
            {
                Id = SpecieId.Boltund,
                Num = 836,
                Name = "Boltund",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 69,
                    Atk = 90,
                    Def = 60,
                    SpA = 90,
                    SpD = 60,
                    Spe = 121,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.StrongJaw,
                    Hidden = AbilityId.Competitive,
                },
                HeightM = 1.0,
                WeightKg = 34,
                Color = "Yellow",
            },
            [SpecieId.Rolycoly] = new()
            {
                Id = SpecieId.Rolycoly,
                Num = 837,
                Name = "Rolycoly",
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 40,
                    Def = 50,
                    SpA = 40,
                    SpD = 50,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SteamEngine,
                    Slot1 = AbilityId.Heatproof,
                    Hidden = AbilityId.FlashFire,
                },
                HeightM = 0.3,
                WeightKg = 12,
                Color = "Black",
            },
            [SpecieId.Carkol] = new()
            {
                Id = SpecieId.Carkol,
                Num = 838,
                Name = "Carkol",
                Types = [PokemonType.Rock, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 60,
                    Def = 90,
                    SpA = 60,
                    SpD = 70,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SteamEngine,
                    Slot1 = AbilityId.FlameBody,
                    Hidden = AbilityId.FlashFire,
                },
                HeightM = 1.1,
                WeightKg = 78,
                Color = "Black",
            },
            [SpecieId.Coalossal] = new()
            {
                Id = SpecieId.Coalossal,
                Num = 839,
                Name = "Coalossal",
                Types = [PokemonType.Rock, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 110,
                    Atk = 80,
                    Def = 120,
                    SpA = 80,
                    SpD = 90,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SteamEngine,
                    Slot1 = AbilityId.FlameBody,
                    Hidden = AbilityId.FlashFire,
                },
                HeightM = 2.8,
                WeightKg = 310.5,
                Color = "Black",
            },
            [SpecieId.Applin] = new()
            {
                Id = SpecieId.Applin,
                Num = 840,
                Name = "Applin",
                Types = [PokemonType.Grass, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 40,
                    Def = 80,
                    SpA = 40,
                    SpD = 40,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Ripen,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.Bulletproof,
                },
                HeightM = 0.2,
                WeightKg = 0.5,
                Color = "Green",
            },
            [SpecieId.Flapple] = new()
            {
                Id = SpecieId.Flapple,
                Num = 841,
                Name = "Flapple",
                Types = [PokemonType.Grass, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 110,
                    Def = 80,
                    SpA = 95,
                    SpD = 60,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Ripen,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.Hustle,
                },
                HeightM = 0.3,
                WeightKg = 1,
                Color = "Green",
            },
            [SpecieId.Appletun] = new()
            {
                Id = SpecieId.Appletun,
                Num = 842,
                Name = "Appletun",
                Types = [PokemonType.Grass, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 110,
                    Atk = 85,
                    Def = 80,
                    SpA = 100,
                    SpD = 80,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Ripen,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 0.4,
                WeightKg = 13,
                Color = "Green",
            },
            [SpecieId.Silicobra] = new()
            {
                Id = SpecieId.Silicobra,
                Num = 843,
                Name = "Silicobra",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 52,
                    Atk = 57,
                    Def = 75,
                    SpA = 35,
                    SpD = 50,
                    Spe = 46,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandSpit,
                    Slot1 = AbilityId.ShedSkin,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 2.2,
                WeightKg = 7.6,
                Color = "Green",
            },
            [SpecieId.Sandaconda] = new()
            {
                Id = SpecieId.Sandaconda,
                Num = 844,
                Name = "Sandaconda",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 72,
                    Atk = 107,
                    Def = 125,
                    SpA = 65,
                    SpD = 70,
                    Spe = 71,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandSpit,
                    Slot1 = AbilityId.ShedSkin,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 3.8,
                WeightKg = 65.5,
                Color = "Green",
            },
            [SpecieId.Cramorant] = new()
            {
                Id = SpecieId.Cramorant,
                Num = 845,
                Name = "Cramorant",
                Types = [PokemonType.Flying, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 85,
                    Def = 55,
                    SpA = 85,
                    SpD = 95,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.GulpMissile,
                },
                HeightM = 0.8,
                WeightKg = 18,
                Color = "Blue",
            },
            [SpecieId.CramorantGulping] = new()
            {
                Id = SpecieId.CramorantGulping,
                Num = 845,
                Name = "Cramorant-Gulping",
                BaseSpecies = SpecieId.Cramorant,
                Forme = FormeId.Gulping,
                Types = [PokemonType.Flying, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 85,
                    Def = 55,
                    SpA = 85,
                    SpD = 95,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.GulpMissile,
                },
                HeightM = 0.8,
                WeightKg = 18,
                Color = "Blue",
            },
            [SpecieId.CramorantGorging] = new()
            {
                Id = SpecieId.CramorantGorging,
                Num = 845,
                Name = "Cramorant-Gorging",
                BaseSpecies = SpecieId.Cramorant,
                Forme = FormeId.Gorging,
                Types = [PokemonType.Flying, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 85,
                    Def = 55,
                    SpA = 85,
                    SpD = 95,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.GulpMissile,
                },
                HeightM = 0.8,
                WeightKg = 18,
                Color = "Blue",
            },
            [SpecieId.Arrokuda] = new()
            {
                Id = SpecieId.Arrokuda,
                Num = 846,
                Name = "Arrokuda",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 41,
                    Atk = 63,
                    Def = 40,
                    SpA = 40,
                    SpD = 30,
                    Spe = 66,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Hidden = AbilityId.PropellerTail,
                },
                HeightM = 0.5,
                WeightKg = 1,
                Color = "Brown",
            },
            [SpecieId.Barraskewda] = new()
            {
                Id = SpecieId.Barraskewda,
                Num = 847,
                Name = "Barraskewda",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 61,
                    Atk = 123,
                    Def = 60,
                    SpA = 60,
                    SpD = 50,
                    Spe = 136,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Hidden = AbilityId.PropellerTail,
                },
                HeightM = 1.3,
                WeightKg = 30,
                Color = "Brown",
            },
            [SpecieId.Toxel] = new()
            {
                Id = SpecieId.Toxel,
                Num = 848,
                Name = "Toxel",
                Types = [PokemonType.Electric, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 38,
                    Def = 35,
                    SpA = 54,
                    SpD = 35,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rattled,
                    Slot1 = AbilityId.Static,
                    Hidden = AbilityId.Klutz,
                },
                HeightM = 0.4,
                WeightKg = 11,
                Color = "Purple",
            },
            [SpecieId.Toxtricity] = new()
            {
                Id = SpecieId.Toxtricity,
                Num = 849,
                Name = "Toxtricity",
                BaseForme = FormeId.Amped,
                Types = [PokemonType.Electric, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 98,
                    Def = 70,
                    SpA = 114,
                    SpD = 70,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PunkRock,
                    Slot1 = AbilityId.Plus,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 1.6,
                WeightKg = 40,
                Color = "Purple",
            },
            [SpecieId.ToxtricityLowKey] = new()
            {
                Id = SpecieId.ToxtricityLowKey,
                Num = 849,
                Name = "Toxtricity-Low-Key",
                BaseSpecies = SpecieId.Toxtricity,
                Forme = FormeId.LowKey,
                Types = [PokemonType.Electric, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 98,
                    Def = 70,
                    SpA = 114,
                    SpD = 70,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PunkRock,
                    Slot1 = AbilityId.Minus,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 1.6,
                WeightKg = 40,
                Color = "Purple",
            },
            [SpecieId.Sizzlipede] = new()
            {
                Id = SpecieId.Sizzlipede,
                Num = 850,
                Name = "Sizzlipede",
                Types = [PokemonType.Fire, PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 45,
                    SpA = 50,
                    SpD = 50,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlashFire,
                    Slot1 = AbilityId.WhiteSmoke,
                    Hidden = AbilityId.FlameBody,
                },
                HeightM = 0.7,
                WeightKg = 1,
                Color = "Red",
            },
        };
    }
}