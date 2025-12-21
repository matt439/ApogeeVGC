using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData951To1000()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Capsakid] = new()
            {
                Id = SpecieId.Capsakid,
                Num = 951,
                Name = "Capsakid",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 62,
                    Def = 40,
                    SpA = 62,
                    SpD = 40,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.Insomnia,
                    Hidden = AbilityId.Klutz,
                },
                HeightM = 0.3,
                WeightKg = 3,
                Color = "Green",
            },
            [SpecieId.Scovillain] = new()
            {
                Id = SpecieId.Scovillain,
                Num = 952,
                Name = "Scovillain",
                Types = [PokemonType.Grass, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 108,
                    Def = 65,
                    SpA = 108,
                    SpD = 65,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.Insomnia,
                    Hidden = AbilityId.Moody,
                },
                HeightM = 0.9,
                WeightKg = 15,
                Color = "Green",
                Prevo = SpecieId.Capsakid,
            },
            [SpecieId.Rellor] = new()
            {
                Id = SpecieId.Rellor,
                Num = 953,
                Name = "Rellor",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 41,
                    Atk = 50,
                    Def = 60,
                    SpA = 31,
                    SpD = 58,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CompoundEyes,
                    Hidden = AbilityId.ShedSkin,
                },
                HeightM = 0.2,
                WeightKg = 1,
                Color = "Brown",
            },
            [SpecieId.Rabsca] = new()
            {
                Id = SpecieId.Rabsca,
                Num = 954,
                Name = "Rabsca",
                Types = [PokemonType.Bug, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 50,
                    Def = 85,
                    SpA = 115,
                    SpD = 100,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 0.3,
                WeightKg = 3.5,
                Color = "Green",
                Prevo = SpecieId.Rellor,
            },
            [SpecieId.Flittle] = new()
            {
                Id = SpecieId.Flittle,
                Num = 955,
                Name = "Flittle",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 35,
                    Def = 30,
                    SpA = 55,
                    SpD = 30,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Anticipation,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.SpeedBoost,
                },
                HeightM = 0.2,
                WeightKg = 1.5,
                Color = "Yellow",
            },
            [SpecieId.Espathra] = new()
            {
                Id = SpecieId.Espathra,
                Num = 956,
                Name = "Espathra",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 60,
                    Def = 60,
                    SpA = 101,
                    SpD = 60,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Opportunist,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.SpeedBoost,
                },
                HeightM = 1.9,
                WeightKg = 90,
                Color = "Yellow",
                Prevo = SpecieId.Flittle,
                EvoLevel = 35,
            },
            [SpecieId.Tinkatink] = new()
            {
                Id = SpecieId.Tinkatink,
                Num = 957,
                Name = "Tinkatink",
                Types = [PokemonType.Fairy, PokemonType.Steel],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 45,
                    Def = 45,
                    SpA = 35,
                    SpD = 64,
                    Spe = 58,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MoldBreaker,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 0.4,
                WeightKg = 8.9,
                Color = "Pink",
            },
            [SpecieId.Tinkatuff] = new()
            {
                Id = SpecieId.Tinkatuff,
                Num = 958,
                Name = "Tinkatuff",
                Types = [PokemonType.Fairy, PokemonType.Steel],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 55,
                    Def = 55,
                    SpA = 45,
                    SpD = 82,
                    Spe = 78,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MoldBreaker,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 0.7,
                WeightKg = 59.1,
                Color = "Pink",
                Prevo = SpecieId.Tinkatink,
                EvoLevel = 24,
            },
            [SpecieId.Tinkaton] = new()
            {
                Id = SpecieId.Tinkaton,
                Num = 959,
                Name = "Tinkaton",
                Types = [PokemonType.Fairy, PokemonType.Steel],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 75,
                    Def = 77,
                    SpA = 70,
                    SpD = 105,
                    Spe = 94,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MoldBreaker,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 0.7,
                WeightKg = 112.8,
                Color = "Pink",
                Prevo = SpecieId.Tinkatuff,
                EvoLevel = 38,
            },
            [SpecieId.Wiglett] = new()
            {
                Id = SpecieId.Wiglett,
                Num = 960,
                Name = "Wiglett",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 10,
                    Atk = 55,
                    Def = 25,
                    SpA = 35,
                    SpD = 25,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Gooey,
                    Slot1 = AbilityId.Rattled,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 1.2,
                WeightKg = 1.8,
                Color = "White",
            },
            [SpecieId.Wugtrio] = new()
            {
                Id = SpecieId.Wugtrio,
                Num = 961,
                Name = "Wugtrio",
                Types = [PokemonType.Water],
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
                    Slot0 = AbilityId.Gooey,
                    Slot1 = AbilityId.Rattled,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 1.2,
                WeightKg = 5.4,
                Color = "Red",
                Prevo = SpecieId.Wiglett,
                EvoLevel = 26,
            },
            [SpecieId.Bombirdier] = new()
            {
                Id = SpecieId.Bombirdier,
                Num = 962,
                Name = "Bombirdier",
                Types = [PokemonType.Flying, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 103,
                    Def = 85,
                    SpA = 60,
                    SpD = 85,
                    Spe = 82,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BigPecks,
                    Slot1 = AbilityId.KeenEye,
                    Hidden = AbilityId.RockyPayload,
                },
                HeightM = 1.5,
                WeightKg = 42.9,
                Color = "White",
            },
            [SpecieId.Finizen] = new()
            {
                Id = SpecieId.Finizen,
                Num = 963,
                Name = "Finizen",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 45,
                    Def = 40,
                    SpA = 45,
                    SpD = 40,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterVeil,
                },
                HeightM = 1.3,
                WeightKg = 60.2,
                Color = "Blue",
            },
            [SpecieId.Palafin] = new()
            {
                Id = SpecieId.Palafin,
                Num = 964,
                Name = "Palafin",
                BaseForme = FormeId.Zero,
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 70,
                    Def = 72,
                    SpA = 53,
                    SpD = 62,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ZeroToHero,
                },
                HeightM = 1.3,
                WeightKg = 60.2,
                Color = "Blue",
                Prevo = SpecieId.Finizen,
                EvoLevel = 38,
                OtherFormes = [FormeId.Hero],
                FormeOrder = [FormeId.Zero, FormeId.Hero],
            },
            [SpecieId.PalafinHero] = new()
            {
                Id = SpecieId.PalafinHero,
                Num = 964,
                Name = "Palafin-Hero",
                BaseSpecies = SpecieId.Palafin,
                Forme = FormeId.Hero,
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 160,
                    Def = 97,
                    SpA = 106,
                    SpD = 87,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ZeroToHero,
                },
                HeightM = 1.8,
                WeightKg = 97.4,
                Color = "Blue",
                RequiredAbility = AbilityId.ZeroToHero,
                BattleOnly = FormeId.Zero,
            },
            [SpecieId.Varoom] = new()
            {
                Id = SpecieId.Varoom,
                Num = 965,
                Name = "Varoom",
                Types = [PokemonType.Steel, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 70,
                    Def = 63,
                    SpA = 30,
                    SpD = 45,
                    Spe = 47,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overcoat,
                    Hidden = AbilityId.SlowStart,
                },
                HeightM = 1,
                WeightKg = 35,
                Color = "Gray",
            },
            [SpecieId.Revavroom] = new()
            {
                Id = SpecieId.Revavroom,
                Num = 966,
                Name = "Revavroom",
                Types = [PokemonType.Steel, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 119,
                    Def = 90,
                    SpA = 54,
                    SpD = 67,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overcoat,
                    Hidden = AbilityId.Filter,
                },
                HeightM = 1.8,
                WeightKg = 120,
                Color = "Gray",
                Prevo = SpecieId.Varoom,
                EvoLevel = 40,
            },
            [SpecieId.Cyclizar] = new()
            {
                Id = SpecieId.Cyclizar,
                Num = 967,
                Name = "Cyclizar",
                Types = [PokemonType.Dragon, PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 95,
                    Def = 65,
                    SpA = 85,
                    SpD = 65,
                    Spe = 121,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 1.6,
                WeightKg = 63,
                Color = "Green",
            },
            [SpecieId.Orthworm] = new()
            {
                Id = SpecieId.Orthworm,
                Num = 968,
                Name = "Orthworm",
                Types = [PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 85,
                    Def = 145,
                    SpA = 60,
                    SpD = 55,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EarthEater,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 2.5,
                WeightKg = 310,
                Color = "Pink",
            },
            [SpecieId.Glimmet] = new()
            {
                Id = SpecieId.Glimmet,
                Num = 969,
                Name = "Glimmet",
                Types = [PokemonType.Rock, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 48,
                    Atk = 35,
                    Def = 42,
                    SpA = 105,
                    SpD = 60,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ToxicDebris,
                    Hidden = AbilityId.Corrosion,
                },
                HeightM = 0.7,
                WeightKg = 8,
                Color = "Blue",
            },
            [SpecieId.Glimmora] = new()
            {
                Id = SpecieId.Glimmora,
                Num = 970,
                Name = "Glimmora",
                Types = [PokemonType.Rock, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 83,
                    Atk = 55,
                    Def = 90,
                    SpA = 130,
                    SpD = 81,
                    Spe = 86,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ToxicDebris,
                    Hidden = AbilityId.Corrosion,
                },
                HeightM = 1.5,
                WeightKg = 45,
                Color = "Blue",
                Prevo = SpecieId.Glimmet,
                EvoLevel = 35,
            },
            [SpecieId.Greavard] = new()
            {
                Id = SpecieId.Greavard,
                Num = 971,
                Name = "Greavard",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 61,
                    Def = 60,
                    SpA = 30,
                    SpD = 55,
                    Spe = 34,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Hidden = AbilityId.Fluffy,
                },
                HeightM = 0.6,
                WeightKg = 35,
                Color = "White",
            },
            [SpecieId.Houndstone] = new()
            {
                Id = SpecieId.Houndstone,
                Num = 972,
                Name = "Houndstone",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 72,
                    Atk = 101,
                    Def = 100,
                    SpA = 50,
                    SpD = 97,
                    Spe = 68,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandRush,
                    Hidden = AbilityId.Fluffy,
                },
                HeightM = 2,
                WeightKg = 15,
                Color = "White",
                Prevo = SpecieId.Greavard,
                EvoLevel = 30,
            },
            [SpecieId.Flamigo] = new()
            {
                Id = SpecieId.Flamigo,
                Num = 973,
                Name = "Flamigo",
                Types = [PokemonType.Flying, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 82,
                    Atk = 115,
                    Def = 74,
                    SpA = 75,
                    SpD = 64,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Scrappy,
                    Slot1 = AbilityId.TangledFeet,
                    Hidden = AbilityId.Costar,
                },
                HeightM = 1.6,
                WeightKg = 37,
                Color = "Pink",
            },
            [SpecieId.Cetoddle] = new()
            {
                Id = SpecieId.Cetoddle,
                Num = 974,
                Name = "Cetoddle",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 108,
                    Atk = 68,
                    Def = 45,
                    SpA = 30,
                    SpD = 40,
                    Spe = 43,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.SnowCloak,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 1.2,
                WeightKg = 45,
                Color = "White",
            },
            [SpecieId.Cetitan] = new()
            {
                Id = SpecieId.Cetitan,
                Num = 975,
                Name = "Cetitan",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 170,
                    Atk = 113,
                    Def = 65,
                    SpA = 45,
                    SpD = 55,
                    Spe = 73,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.SlushRush,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 4.5,
                WeightKg = 700,
                Color = "White",
                Prevo = SpecieId.Cetoddle,
            },
            [SpecieId.IronHands] = new()
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
}
