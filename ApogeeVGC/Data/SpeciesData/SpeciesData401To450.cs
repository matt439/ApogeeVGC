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
                        };
                    }
                }
