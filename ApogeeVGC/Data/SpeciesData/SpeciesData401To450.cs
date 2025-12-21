using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData401To450()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Kricketot] = new()
            {
                Id = SpecieId.Kricketot,
                Num = 401,
                Name = "Kricketot",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 37,
                    Atk = 25,
                    Def = 41,
                    SpA = 25,
                    SpD = 41,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                    Hidden = AbilityId.RunAway,
                },
                HeightM = 0.3,
                WeightKg = 2.2,
                Color = "Red",
            },
            [SpecieId.Kricketune] = new()
            {
                Id = SpecieId.Kricketune,
                Num = 402,
                Name = "Kricketune",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 77,
                    Atk = 85,
                    Def = 51,
                    SpA = 55,
                    SpD = 51,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 1,
                WeightKg = 25.5,
                Color = "Red",
                Prevo = SpecieId.Kricketot,
            },
            [SpecieId.Shinx] = new()
            {
                Id = SpecieId.Shinx,
                Num = 403,
                Name = "Shinx",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 65,
                    Def = 34,
                    SpA = 40,
                    SpD = 34,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.Intimidate,
                    Hidden = AbilityId.Guts,
                },
                HeightM = 0.5,
                WeightKg = 9.5,
                Color = "Blue",
            },
            [SpecieId.Luxio] = new()
            {
                Id = SpecieId.Luxio,
                Num = 404,
                Name = "Luxio",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 85,
                    Def = 49,
                    SpA = 60,
                    SpD = 49,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.Intimidate,
                    Hidden = AbilityId.Guts,
                },
                HeightM = 0.9,
                WeightKg = 30.5,
                Color = "Blue",
                Prevo = SpecieId.Shinx,
            },
            [SpecieId.Luxray] = new()
            {
                Id = SpecieId.Luxray,
                Num = 405,
                Name = "Luxray",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 79,
                    SpA = 95,
                    SpD = 79,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.Intimidate,
                    Hidden = AbilityId.Guts,
                },
                HeightM = 1.4,
                WeightKg = 42,
                Color = "Blue",
                Prevo = SpecieId.Luxio,
            },
            [SpecieId.Budew] = new()
            {
                Id = SpecieId.Budew,
                Num = 406,
                Name = "Budew",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 30,
                    Def = 35,
                    SpA = 50,
                    SpD = 70,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Slot1 = AbilityId.PoisonPoint,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 0.2,
                WeightKg = 1.2,
                Color = "Green",
            },
            [SpecieId.Roserade] = new()
            {
                Id = SpecieId.Roserade,
                Num = 407,
                Name = "Roserade",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 70,
                    Def = 65,
                    SpA = 125,
                    SpD = 105,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Slot1 = AbilityId.PoisonPoint,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 0.9,
                WeightKg = 14.5,
                Color = "Green",
            },
            [SpecieId.Cranidos] = new()
            {
                Id = SpecieId.Cranidos,
                Num = 408,
                Name = "Cranidos",
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 67,
                    Atk = 125,
                    Def = 40,
                    SpA = 30,
                    SpD = 30,
                    Spe = 58,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MoldBreaker,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 0.9,
                WeightKg = 31.5,
                Color = "Blue",
            },
            [SpecieId.Rampardos] = new()
            {
                Id = SpecieId.Rampardos,
                Num = 409,
                Name = "Rampardos",
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 97,
                    Atk = 165,
                    Def = 60,
                    SpA = 65,
                    SpD = 50,
                    Spe = 58,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MoldBreaker,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 1.6,
                WeightKg = 102.5,
                Color = "Blue",
                Prevo = SpecieId.Cranidos,
            },
            [SpecieId.Shieldon] = new()
            {
                Id = SpecieId.Shieldon,
                Num = 410,
                Name = "Shieldon",
                Types = [PokemonType.Rock, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 42,
                    Def = 118,
                    SpA = 42,
                    SpD = 88,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Hidden = AbilityId.Soundproof,
                },
                HeightM = 0.5,
                WeightKg = 57,
                Color = "Gray",
            },
            [SpecieId.Bastiodon] = new()
            {
                Id = SpecieId.Bastiodon,
                Num = 411,
                Name = "Bastiodon",
                Types = [PokemonType.Rock, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 52,
                    Def = 168,
                    SpA = 47,
                    SpD = 138,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Hidden = AbilityId.Soundproof,
                },
                HeightM = 1.3,
                WeightKg = 149.5,
                Color = "Gray",
                Prevo = SpecieId.Shieldon,
            },
            [SpecieId.Burmy] = new()
            {
                Id = SpecieId.Burmy,
                Num = 412,
                Name = "Burmy",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 29,
                    Def = 45,
                    SpA = 29,
                    SpD = 45,
                    Spe = 36,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                    Hidden = AbilityId.Overcoat,
                },
                HeightM = 0.2,
                WeightKg = 3.4,
                Color = "Green",
            },
            [SpecieId.Wormadam] = new()
            {
                Id = SpecieId.Wormadam,
                Num = 413,
                Name = "Wormadam",
                Types = [PokemonType.Bug, PokemonType.Grass],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 59,
                    Def = 85,
                    SpA = 79,
                    SpD = 105,
                    Spe = 36,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Anticipation,
                    Hidden = AbilityId.Overcoat,
                },
                HeightM = 0.5,
                WeightKg = 6.5,
                Color = "Green",
                Prevo = SpecieId.Burmy,
            },
            [SpecieId.WormadamSandy] = new()
            {
                Id = SpecieId.WormadamSandy,
                Num = 413,
                Name = "Wormadam-Sandy",
                Types = [PokemonType.Bug, PokemonType.Ground],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 79,
                    Def = 105,
                    SpA = 59,
                    SpD = 85,
                    Spe = 36,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Anticipation,
                    Hidden = AbilityId.Overcoat,
                },
                HeightM = 0.5,
                WeightKg = 6.5,
                Color = "Brown",
                Prevo = SpecieId.Burmy,
                BaseSpecies = SpecieId.Wormadam,
                Forme = FormeId.Sandy,
            },
            [SpecieId.WormadamTrash] = new()
            {
                Id = SpecieId.WormadamTrash,
                Num = 413,
                Name = "Wormadam-Trash",
                Types = [PokemonType.Bug, PokemonType.Steel],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 69,
                    Def = 95,
                    SpA = 69,
                    SpD = 95,
                    Spe = 36,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Anticipation,
                    Hidden = AbilityId.Overcoat,
                },
                HeightM = 0.5,
                WeightKg = 6.5,
                Color = "Red",
                Prevo = SpecieId.Burmy,
                BaseSpecies = SpecieId.Wormadam,
                Forme = FormeId.Trash,
            },
            [SpecieId.Mothim] = new()
            {
                Id = SpecieId.Mothim,
                Num = 414,
                Name = "Mothim",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 94,
                    Def = 50,
                    SpA = 94,
                    SpD = 50,
                    Spe = 66,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Hidden = AbilityId.TintedLens,
                },
                HeightM = 0.9,
                WeightKg = 23.3,
                Color = "Yellow",
                Prevo = SpecieId.Burmy,
            },
            [SpecieId.Combee] = new()
            {
                Id = SpecieId.Combee,
                Num = 415,
                Name = "Combee",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 30,
                    Def = 42,
                    SpA = 30,
                    SpD = 42,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HoneyGather,
                    Hidden = AbilityId.Hustle,
                },
                HeightM = 0.3,
                WeightKg = 5.5,
                Color = "Yellow",
            },
            [SpecieId.Vespiquen] = new()
            {
                Id = SpecieId.Vespiquen,
                Num = 416,
                Name = "Vespiquen",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 80,
                    Def = 102,
                    SpA = 80,
                    SpD = 102,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 1.2,
                WeightKg = 38.5,
                Color = "Yellow",
                Prevo = SpecieId.Combee,
            },
            [SpecieId.Pachirisu] = new()
            {
                Id = SpecieId.Pachirisu,
                Num = 417,
                Name = "Pachirisu",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 45,
                    Def = 70,
                    SpA = 45,
                    SpD = 90,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.Pickup,
                    Hidden = AbilityId.VoltAbsorb,
                },
                HeightM = 0.4,
                WeightKg = 3.9,
                Color = "White",
            },
            [SpecieId.Buizel] = new()
            {
                Id = SpecieId.Buizel,
                Num = 418,
                Name = "Buizel",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 65,
                    Def = 35,
                    SpA = 60,
                    SpD = 30,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Hidden = AbilityId.WaterVeil,
                },
                HeightM = 0.7,
                WeightKg = 29.5,
                Color = "Brown",
            },
            [SpecieId.Floatzel] = new()
            {
                Id = SpecieId.Floatzel,
                Num = 419,
                Name = "Floatzel",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 105,
                    Def = 55,
                    SpA = 85,
                    SpD = 50,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Hidden = AbilityId.WaterVeil,
                },
                HeightM = 1.1,
                WeightKg = 33.5,
                Color = "Brown",
                Prevo = SpecieId.Buizel,
            },
            [SpecieId.Cherubi] = new()
            {
                Id = SpecieId.Cherubi,
                Num = 420,
                Name = "Cherubi",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 35,
                    Def = 45,
                    SpA = 62,
                    SpD = 53,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                },
                HeightM = 0.4,
                WeightKg = 3.3,
                Color = "Pink",
            },
            [SpecieId.Cherrim] = new()
            {
                Id = SpecieId.Cherrim,
                Num = 421,
                Name = "Cherrim",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 60,
                    Def = 70,
                    SpA = 87,
                    SpD = 78,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlowerGift,
                },
                HeightM = 0.5,
                WeightKg = 9.3,
                Color = "Purple",
                Prevo = SpecieId.Cherubi,
            },
            [SpecieId.CherrimSunshine] = new()
            {
                Id = SpecieId.CherrimSunshine,
                Num = 421,
                Name = "Cherrim-Sunshine",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 60,
                    Def = 70,
                    SpA = 87,
                    SpD = 78,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlowerGift,
                },
                HeightM = 0.5,
                WeightKg = 9.3,
                Color = "Pink",
                BaseSpecies = SpecieId.Cherrim,
                Forme = FormeId.Sunshine,
                BattleOnly = FormeId.Sunshine,
            },
            [SpecieId.Shellos] = new()
            {
                Id = SpecieId.Shellos,
                Num = 422,
                Name = "Shellos",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 76,
                    Atk = 48,
                    Def = 48,
                    SpA = 57,
                    SpD = 62,
                    Spe = 34,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.StickyHold,
                    Slot1 = AbilityId.StormDrain,
                    Hidden = AbilityId.SandForce,
                },
                HeightM = 0.3,
                WeightKg = 6.3,
                Color = "Purple",
            },
            [SpecieId.Gastrodon] = new()
            {
                Id = SpecieId.Gastrodon,
                Num = 423,
                Name = "Gastrodon",
                Types = [PokemonType.Water, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 111,
                    Atk = 83,
                    Def = 68,
                    SpA = 92,
                    SpD = 82,
                    Spe = 39,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.StickyHold,
                    Slot1 = AbilityId.StormDrain,
                    Hidden = AbilityId.SandForce,
                },
                HeightM = 0.9,
                WeightKg = 29.9,
                Color = "Purple",
                Prevo = SpecieId.Shellos,
            },
            [SpecieId.Ambipom] = new()
            {
                Id = SpecieId.Ambipom,
                Num = 424,
                Name = "Ambipom",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 100,
                    Def = 66,
                    SpA = 60,
                    SpD = 66,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Technician,
                    Slot1 = AbilityId.Pickup,
                    Hidden = AbilityId.SkillLink,
                },
                HeightM = 1.2,
                WeightKg = 20.3,
                Color = "Purple",
            },
            [SpecieId.Drifloon] = new()
            {
                Id = SpecieId.Drifloon,
                Num = 425,
                Name = "Drifloon",
                Types = [PokemonType.Ghost, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 50,
                    Def = 34,
                    SpA = 60,
                    SpD = 44,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Aftermath,
                    Slot1 = AbilityId.Unburden,
                    Hidden = AbilityId.FlareBoost,
                },
                HeightM = 0.4,
                WeightKg = 1.2,
                Color = "Purple",
            },
            [SpecieId.Drifblim] = new()
            {
                Id = SpecieId.Drifblim,
                Num = 426,
                Name = "Drifblim",
                Types = [PokemonType.Ghost, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 150,
                    Atk = 80,
                    Def = 44,
                    SpA = 90,
                    SpD = 54,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Aftermath,
                    Slot1 = AbilityId.Unburden,
                    Hidden = AbilityId.FlareBoost,
                },
                HeightM = 1.2,
                WeightKg = 15,
                Color = "Purple",
                Prevo = SpecieId.Drifloon,
            },
            [SpecieId.Buneary] = new()
            {
                Id = SpecieId.Buneary,
                Num = 427,
                Name = "Buneary",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 66,
                    Def = 44,
                    SpA = 44,
                    SpD = 56,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.Klutz,
                    Hidden = AbilityId.Limber,
                },
                HeightM = 0.4,
                WeightKg = 5.5,
                Color = "Brown",
            },
            [SpecieId.Lopunny] = new()
            {
                Id = SpecieId.Lopunny,
                Num = 428,
                Name = "Lopunny",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 76,
                    Def = 84,
                    SpA = 54,
                    SpD = 96,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.Klutz,
                    Hidden = AbilityId.Limber,
                },
                HeightM = 1.2,
                WeightKg = 33.3,
                Color = "Brown",
                Prevo = SpecieId.Buneary,
            },
            [SpecieId.LopunnyMega] = new()
            {
                Id = SpecieId.LopunnyMega,
                Num = 428,
                Name = "Lopunny-Mega",
                Types = [PokemonType.Normal, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 136,
                    Def = 94,
                    SpA = 54,
                    SpD = 96,
                    Spe = 135,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Scrappy,
                },
                HeightM = 1.3,
                WeightKg = 28.3,
                Color = "Brown",
                BaseSpecies = SpecieId.Lopunny,
                Forme = FormeId.Mega,
            },
            [SpecieId.Mismagius] = new()
            {
                Id = SpecieId.Mismagius,
                Num = 429,
                Name = "Mismagius",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 60,
                    Def = 60,
                    SpA = 105,
                    SpD = 105,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.9,
                WeightKg = 4.4,
                Color = "Purple",
            },
            [SpecieId.Honchkrow] = new()
            {
                Id = SpecieId.Honchkrow,
                Num = 430,
                Name = "Honchkrow",
                Types = [PokemonType.Dark, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 125,
                    Def = 52,
                    SpA = 105,
                    SpD = 52,
                    Spe = 71,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Slot1 = AbilityId.SuperLuck,
                    Hidden = AbilityId.Moxie,
                },
                HeightM = 0.9,
                WeightKg = 27.3,
                Color = "Black",
            },
            [SpecieId.Glameow] = new()
            {
                Id = SpecieId.Glameow,
                Num = 431,
                Name = "Glameow",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 49,
                    Atk = 55,
                    Def = 42,
                    SpA = 42,
                    SpD = 37,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Limber,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.KeenEye,
                },
                HeightM = 0.5,
                WeightKg = 3.9,
                Color = "Gray",
            },
            [SpecieId.Purugly] = new()
            {
                Id = SpecieId.Purugly,
                Num = 432,
                Name = "Purugly",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 71,
                    Atk = 82,
                    Def = 64,
                    SpA = 64,
                    SpD = 59,
                    Spe = 112,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Defiant,
                },
                HeightM = 1,
                WeightKg = 43.8,
                Color = "Gray",
                Prevo = SpecieId.Glameow,
            },
            [SpecieId.Chingling] = new()
            {
                Id = SpecieId.Chingling,
                Num = 433,
                Name = "Chingling",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 30,
                    Def = 50,
                    SpA = 65,
                    SpD = 50,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.2,
                WeightKg = 0.6,
                Color = "Yellow",
            },
            [SpecieId.Stunky] = new()
            {
                Id = SpecieId.Stunky,
                Num = 434,
                Name = "Stunky",
                Types = [PokemonType.Poison, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 63,
                    Atk = 63,
                    Def = 47,
                    SpA = 41,
                    SpD = 41,
                    Spe = 74,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Stench,
                    Slot1 = AbilityId.Aftermath,
                    Hidden = AbilityId.KeenEye,
                },
                HeightM = 0.4,
                WeightKg = 19.2,
                Color = "Purple",
            },
            [SpecieId.Skuntank] = new()
            {
                Id = SpecieId.Skuntank,
                Num = 435,
                Name = "Skuntank",
                Types = [PokemonType.Poison, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 103,
                    Atk = 93,
                    Def = 67,
                    SpA = 71,
                    SpD = 61,
                    Spe = 84,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Stench,
                    Slot1 = AbilityId.Aftermath,
                    Hidden = AbilityId.KeenEye,
                },
                HeightM = 1,
                WeightKg = 38,
                Color = "Purple",
                Prevo = SpecieId.Stunky,
            },
            [SpecieId.Bronzor] = new()
            {
                Id = SpecieId.Bronzor,
                Num = 436,
                Name = "Bronzor",
                Types = [PokemonType.Steel, PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 57,
                    Atk = 24,
                    Def = 86,
                    SpA = 24,
                    SpD = 86,
                    Spe = 23,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                    Slot1 = AbilityId.Heatproof,
                    Hidden = AbilityId.HeavyMetal,
                },
                HeightM = 0.5,
                WeightKg = 60.5,
                Color = "Green",
            },
            [SpecieId.Bronzong] = new()
            {
                Id = SpecieId.Bronzong,
                Num = 437,
                Name = "Bronzong",
                Types = [PokemonType.Steel, PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 67,
                    Atk = 89,
                    Def = 116,
                    SpA = 79,
                    SpD = 116,
                    Spe = 33,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                    Slot1 = AbilityId.Heatproof,
                    Hidden = AbilityId.HeavyMetal,
                },
                HeightM = 1.3,
                WeightKg = 187,
                Color = "Green",
                Prevo = SpecieId.Bronzor,
            },
            [SpecieId.Bonsly] = new()
            {
                Id = SpecieId.Bonsly,
                Num = 438,
                Name = "Bonsly",
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 80,
                    Def = 95,
                    SpA = 10,
                    SpD = 45,
                    Spe = 10,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Slot1 = AbilityId.RockHead,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 0.5,
                WeightKg = 15,
                Color = "Brown",
            },
            [SpecieId.MimeJr] = new()
            {
                Id = SpecieId.MimeJr,
                Num = 439,
                Name = "Mime Jr.",
                Types = [PokemonType.Psychic, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 20,
                    Atk = 25,
                    Def = 45,
                    SpA = 70,
                    SpD = 90,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Soundproof,
                    Slot1 = AbilityId.Filter,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 0.6,
                WeightKg = 13,
                Color = "Pink",
            },
            [SpecieId.Happiny] = new()
            {
                Id = SpecieId.Happiny,
                Num = 440,
                Name = "Happiny",
                Types = [PokemonType.Normal],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 5,
                    Def = 5,
                    SpA = 15,
                    SpD = 65,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Slot1 = AbilityId.SereneGrace,
                    Hidden = AbilityId.FriendGuard,
                },
                HeightM = 0.6,
                WeightKg = 24.4,
                Color = "Pink",
            },
            [SpecieId.Chatot] = new()
            {
                Id = SpecieId.Chatot,
                Num = 441,
                Name = "Chatot",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 76,
                    Atk = 65,
                    Def = 45,
                    SpA = 92,
                    SpD = 42,
                    Spe = 91,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.TangledFeet,
                    Hidden = AbilityId.BigPecks,
                },
                HeightM = 0.5,
                WeightKg = 1.9,
                Color = "Black",
            },
            [SpecieId.Spiritomb] = new()
            {
                Id = SpecieId.Spiritomb,
                Num = 442,
                Name = "Spiritomb",
                Types = [PokemonType.Ghost, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 92,
                    Def = 108,
                    SpA = 92,
                    SpD = 108,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 1,
                WeightKg = 108,
                Color = "Purple",
            },
            [SpecieId.Gible] = new()
            {
                Id = SpecieId.Gible,
                Num = 443,
                Name = "Gible",
                Types = [PokemonType.Dragon, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 58,
                    Atk = 70,
                    Def = 45,
                    SpA = 40,
                    SpD = 45,
                    Spe = 42,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandVeil,
                    Hidden = AbilityId.RoughSkin,
                },
                HeightM = 0.7,
                WeightKg = 20.5,
                Color = "Blue",
            },
            [SpecieId.Gabite] = new()
            {
                Id = SpecieId.Gabite,
                Num = 444,
                Name = "Gabite",
                Types = [PokemonType.Dragon, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 90,
                    Def = 65,
                    SpA = 50,
                    SpD = 55,
                    Spe = 82,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandVeil,
                    Hidden = AbilityId.RoughSkin,
                },
                HeightM = 1.4,
                WeightKg = 56,
                Color = "Blue",
                Prevo = SpecieId.Gible,
            },
            [SpecieId.Garchomp] = new()
            {
                Id = SpecieId.Garchomp,
                Num = 445,
                Name = "Garchomp",
                Types = [PokemonType.Dragon, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 108,
                    Atk = 130,
                    Def = 95,
                    SpA = 80,
                    SpD = 85,
                    Spe = 102,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandVeil,
                    Hidden = AbilityId.RoughSkin,
                },
                HeightM = 1.9,
                WeightKg = 95,
                Color = "Blue",
                Prevo = SpecieId.Gabite,
            },
            [SpecieId.GarchompMega] = new()
            {
                Id = SpecieId.GarchompMega,
                Num = 445,
                Name = "Garchomp-Mega",
                Types = [PokemonType.Dragon, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 108,
                    Atk = 170,
                    Def = 115,
                    SpA = 120,
                    SpD = 95,
                    Spe = 92,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandForce,
                },
                HeightM = 1.9,
                WeightKg = 95,
                Color = "Blue",
                BaseSpecies = SpecieId.Garchomp,
                Forme = FormeId.Mega,
            },
            [SpecieId.Munchlax] = new()
            {
                Id = SpecieId.Munchlax,
                Num = 446,
                Name = "Munchlax",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 135,
                    Atk = 85,
                    Def = 40,
                    SpA = 40,
                    SpD = 85,
                    Spe = 5,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.ThickFat,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 0.6,
                WeightKg = 105,
                Color = "Black",
            },
            [SpecieId.Riolu] = new()
            {
                Id = SpecieId.Riolu,
                Num = 447,
                Name = "Riolu",
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 70,
                    Def = 40,
                    SpA = 35,
                    SpD = 40,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Steadfast,
                    Slot1 = AbilityId.InnerFocus,
                    Hidden = AbilityId.Prankster,
                },
                HeightM = 0.7,
                WeightKg = 20.2,
                Color = "Blue",
            },
            [SpecieId.Lucario] = new()
            {
                Id = SpecieId.Lucario,
                Num = 448,
                Name = "Lucario",
                Types = [PokemonType.Fighting, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 110,
                    Def = 70,
                    SpA = 115,
                    SpD = 70,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Steadfast,
                    Slot1 = AbilityId.InnerFocus,
                    Hidden = AbilityId.Justified,
                },
                HeightM = 1.2,
                WeightKg = 54,
                Color = "Blue",
                Prevo = SpecieId.Riolu,
            },
            [SpecieId.LucarioMega] = new()
            {
                Id = SpecieId.LucarioMega,
                Num = 448,
                Name = "Lucario-Mega",
                Types = [PokemonType.Fighting, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 145,
                    Def = 88,
                    SpA = 140,
                    SpD = 70,
                    Spe = 112,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Adaptability,
                },
                HeightM = 1.3,
                WeightKg = 57.5,
                Color = "Blue",
                BaseSpecies = SpecieId.Lucario,
                Forme = FormeId.Mega,
            },
            [SpecieId.Hippopotas] = new()
            {
                Id = SpecieId.Hippopotas,
                Num = 449,
                Name = "Hippopotas",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 72,
                    Def = 78,
                    SpA = 38,
                    SpD = 42,
                    Spe = 32,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandStream,
                    Hidden = AbilityId.SandForce,
                },
                HeightM = 0.8,
                WeightKg = 49.5,
                Color = "Brown",
            },
            [SpecieId.Hippowdon] = new()
            {
                Id = SpecieId.Hippowdon,
                Num = 450,
                Name = "Hippowdon",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 108,
                    Atk = 112,
                    Def = 118,
                    SpA = 68,
                    SpD = 72,
                    Spe = 47,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandStream,
                    Hidden = AbilityId.SandForce,
                },
                HeightM = 2,
                WeightKg = 300,
                Color = "Brown",
                Prevo = SpecieId.Hippopotas,
            },
        };
    }
}