using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData0201to0250()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Unown] = new()
            {
                Id = SpecieId.Unown,
                Num = 201,
                Name = "Unown",
                Types = [PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 48,
                    Atk = 72,
                    Def = 48,
                    SpA = 72,
                    SpD = 48,
                    Spe = 48,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.5,
                WeightKg = 5,
                Color = "Black",
            },
            [SpecieId.Wobbuffet] = new()
            {
                Id = SpecieId.Wobbuffet,
                Num = 202,
                Name = "Wobbuffet",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 190,
                    Atk = 33,
                    Def = 58,
                    SpA = 33,
                    SpD = 58,
                    Spe = 33,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShadowTag,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 1.3,
                WeightKg = 28.5,
                Color = "Blue",
            },
            [SpecieId.Girafarig] = new()
            {
                Id = SpecieId.Girafarig,
                Num = 203,
                Name = "Girafarig",
                Types = [PokemonType.Normal, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 80,
                    Def = 65,
                    SpA = 90,
                    SpD = 65,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Slot1 = AbilityId.EarlyBird,
                    Hidden = AbilityId.SapSipper,
                },
                HeightM = 1.5,
                WeightKg = 41.5,
                Color = "Yellow",
            },
            [SpecieId.Pineco] = new()
            {
                Id = SpecieId.Pineco,
                Num = 204,
                Name = "Pineco",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 90,
                    SpA = 35,
                    SpD = 35,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Hidden = AbilityId.Overcoat,
                },
                HeightM = 0.6,
                WeightKg = 7.2,
                Color = "Gray",
            },
            [SpecieId.Forretress] = new()
            {
                Id = SpecieId.Forretress,
                Num = 205,
                Name = "Forretress",
                Types = [PokemonType.Bug, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 90,
                    Def = 140,
                    SpA = 60,
                    SpD = 60,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Hidden = AbilityId.Overcoat,
                },
                HeightM = 1.2,
                WeightKg = 125.8,
                Color = "Purple",
            },
            [SpecieId.Dunsparce] = new()
            {
                Id = SpecieId.Dunsparce,
                Num = 206,
                Name = "Dunsparce",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 70,
                    Def = 70,
                    SpA = 65,
                    SpD = 65,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SereneGrace,
                    Slot1 = AbilityId.RunAway,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 1.5,
                WeightKg = 14,
                Color = "Yellow",
            },
            [SpecieId.Gligar] = new()
            {
                Id = SpecieId.Gligar,
                Num = 207,
                Name = "Gligar",
                Types = [PokemonType.Ground, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 75,
                    Def = 105,
                    SpA = 35,
                    SpD = 65,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.SandVeil,
                    Hidden = AbilityId.Immunity,
                },
                HeightM = 1.1,
                WeightKg = 64.8,
                Color = "Purple",
            },
            [SpecieId.Steelix] = new()
            {
                Id = SpecieId.Steelix,
                Num = 208,
                Name = "Steelix",
                Types = [PokemonType.Steel, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 85,
                    Def = 200,
                    SpA = 55,
                    SpD = 65,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 9.2,
                WeightKg = 400,
                Color = "Gray",
            },
            [SpecieId.SteelixMega] = new()
            {
                Id = SpecieId.SteelixMega,
                Num = 208,
                Name = "Steelix-Mega",
                BaseSpecies = SpecieId.Steelix,
                Forme = FormeId.Mega,
                Types = [PokemonType.Steel, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 125,
                    Def = 230,
                    SpA = 55,
                    SpD = 95,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandForce,
                },
                HeightM = 10.5,
                WeightKg = 740,
                Color = "Gray",
            },
            [SpecieId.Snubbull] = new()
            {
                Id = SpecieId.Snubbull,
                Num = 209,
                Name = "Snubbull",
                Types = [PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 80,
                    Def = 50,
                    SpA = 40,
                    SpD = 40,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.RunAway,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 0.6,
                WeightKg = 7.8,
                Color = "Pink",
            },
            [SpecieId.Granbull] = new()
            {
                Id = SpecieId.Granbull,
                Num = 210,
                Name = "Granbull",
                Types = [PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 120,
                    Def = 75,
                    SpA = 60,
                    SpD = 60,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.QuickFeet,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 1.4,
                WeightKg = 48.7,
                Color = "Purple",
            },
            [SpecieId.Qwilfish] = new()
            {
                Id = SpecieId.Qwilfish,
                Num = 211,
                Name = "Qwilfish",
                Types = [PokemonType.Water, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 95,
                    Def = 85,
                    SpA = 55,
                    SpD = 55,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.SwiftSwim,
                    Hidden = AbilityId.Intimidate,
                },
                HeightM = 0.5,
                WeightKg = 3.9,
                Color = "Gray",
            },
            [SpecieId.QwilfishHisui] = new()
            {
                Id = SpecieId.QwilfishHisui,
                Num = 211,
                Name = "Qwilfish-Hisui",
                BaseSpecies = SpecieId.Qwilfish,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Dark, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 95,
                    Def = 85,
                    SpA = 55,
                    SpD = 55,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.SwiftSwim,
                    Hidden = AbilityId.Intimidate,
                },
                HeightM = 0.5,
                WeightKg = 3.9,
                Color = "Black",
            },
            [SpecieId.Scizor] = new()
            {
                Id = SpecieId.Scizor,
                Num = 212,
                Name = "Scizor",
                Types = [PokemonType.Bug, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 130,
                    Def = 100,
                    SpA = 55,
                    SpD = 80,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Slot1 = AbilityId.Technician,
                    Hidden = AbilityId.LightMetal,
                },
                HeightM = 1.8,
                WeightKg = 118,
                Color = "Red",
            },
            [SpecieId.ScizorMega] = new()
            {
                Id = SpecieId.ScizorMega,
                Num = 212,
                Name = "Scizor-Mega",
                BaseSpecies = SpecieId.Scizor,
                Forme = FormeId.Mega,
                Types = [PokemonType.Bug, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 150,
                    Def = 140,
                    SpA = 65,
                    SpD = 100,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Technician,
                },
                HeightM = 2.0,
                WeightKg = 125,
                Color = "Red",
            },
            [SpecieId.Shuckle] = new()
            {
                Id = SpecieId.Shuckle,
                Num = 213,
                Name = "Shuckle",
                Types = [PokemonType.Bug, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 20,
                    Atk = 10,
                    Def = 230,
                    SpA = 10,
                    SpD = 230,
                    Spe = 5,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.Contrary,
                },
                HeightM = 0.6,
                WeightKg = 20.5,
                Color = "Yellow",
            },
            [SpecieId.Heracross] = new()
            {
                Id = SpecieId.Heracross,
                Num = 214,
                Name = "Heracross",
                Types = [PokemonType.Bug, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 125,
                    Def = 75,
                    SpA = 40,
                    SpD = 95,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Slot1 = AbilityId.Guts,
                    Hidden = AbilityId.Moxie,
                },
                HeightM = 1.5,
                WeightKg = 54,
                Color = "Blue",
            },
            [SpecieId.HeracrossMega] = new()
            {
                Id = SpecieId.HeracrossMega,
                Num = 214,
                Name = "Heracross-Mega",
                BaseSpecies = SpecieId.Heracross,
                Forme = FormeId.Mega,
                Types = [PokemonType.Bug, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 185,
                    Def = 115,
                    SpA = 40,
                    SpD = 105,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SkillLink,
                },
                HeightM = 1.7,
                WeightKg = 62.5,
                Color = "Blue",
            },
            [SpecieId.Sneasel] = new()
            {
                Id = SpecieId.Sneasel,
                Num = 215,
                Name = "Sneasel",
                Types = [PokemonType.Dark, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 95,
                    Def = 55,
                    SpA = 35,
                    SpD = 75,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Slot1 = AbilityId.KeenEye,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 0.9,
                WeightKg = 28,
                Color = "Black",
            },
            [SpecieId.SneaselHisui] = new()
            {
                Id = SpecieId.SneaselHisui,
                Num = 215,
                Name = "Sneasel-Hisui",
                BaseSpecies = SpecieId.Sneasel,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Fighting, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 95,
                    Def = 55,
                    SpA = 35,
                    SpD = 75,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Slot1 = AbilityId.KeenEye,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 0.9,
                WeightKg = 27,
                Color = "Gray",
            },
            [SpecieId.Teddiursa] = new()
            {
                Id = SpecieId.Teddiursa,
                Num = 216,
                Name = "Teddiursa",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 80,
                    Def = 50,
                    SpA = 50,
                    SpD = 50,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.QuickFeet,
                    Hidden = AbilityId.HoneyGather,
                },
                HeightM = 0.6,
                WeightKg = 8.8,
                Color = "Brown",
            },
            [SpecieId.Ursaring] = new()
            {
                Id = SpecieId.Ursaring,
                Num = 217,
                Name = "Ursaring",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 130,
                    Def = 75,
                    SpA = 75,
                    SpD = 75,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Guts,
                    Slot1 = AbilityId.QuickFeet,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 1.8,
                WeightKg = 125.8,
                Color = "Brown",
            },
            [SpecieId.Slugma] = new()
            {
                Id = SpecieId.Slugma,
                Num = 218,
                Name = "Slugma",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 40,
                    Def = 40,
                    SpA = 70,
                    SpD = 40,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagmaArmor,
                    Slot1 = AbilityId.FlameBody,
                    Hidden = AbilityId.WeakArmor,
                },
                HeightM = 0.7,
                WeightKg = 35,
                Color = "Red",
            },
            [SpecieId.Magcargo] = new()
            {
                Id = SpecieId.Magcargo,
                Num = 219,
                Name = "Magcargo",
                Types = [PokemonType.Fire, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 50,
                    Def = 120,
                    SpA = 90,
                    SpD = 80,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagmaArmor,
                    Slot1 = AbilityId.FlameBody,
                    Hidden = AbilityId.WeakArmor,
                },
                HeightM = 0.8,
                WeightKg = 55,
                Color = "Red",
            },
            [SpecieId.Swinub] = new()
            {
                Id = SpecieId.Swinub,
                Num = 220,
                Name = "Swinub",
                Types = [PokemonType.Ice, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 50,
                    Def = 40,
                    SpA = 30,
                    SpD = 30,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.SnowCloak,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 0.4,
                WeightKg = 6.5,
                Color = "Brown",
            },
            [SpecieId.Piloswine] = new()
            {
                Id = SpecieId.Piloswine,
                Num = 221,
                Name = "Piloswine",
                Types = [PokemonType.Ice, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 100,
                    Def = 80,
                    SpA = 60,
                    SpD = 60,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.SnowCloak,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 1.1,
                WeightKg = 55.8,
                Color = "Brown",
            },
            [SpecieId.Corsola] = new()
            {
                Id = SpecieId.Corsola,
                Num = 222,
                Name = "Corsola",
                Types = [PokemonType.Water, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 55,
                    Def = 95,
                    SpA = 65,
                    SpD = 95,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Hustle,
                    Slot1 = AbilityId.NaturalCure,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 0.6,
                WeightKg = 5,
                Color = "Pink",
            },
            [SpecieId.CorsolaGalar] = new()
            {
                Id = SpecieId.CorsolaGalar,
                Num = 222,
                Name = "Corsola-Galar",
                BaseSpecies = SpecieId.Corsola,
                Forme = FormeId.Galar,
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 55,
                    Def = 100,
                    SpA = 65,
                    SpD = 100,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WeakArmor,
                    Hidden = AbilityId.CursedBody,
                },
                HeightM = 0.6,
                WeightKg = 0.5,
                Color = "White",
            },
            [SpecieId.Remoraid] = new()
            {
                Id = SpecieId.Remoraid,
                Num = 223,
                Name = "Remoraid",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 65,
                    Def = 35,
                    SpA = 65,
                    SpD = 35,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Hustle,
                    Slot1 = AbilityId.Sniper,
                    Hidden = AbilityId.Moody,
                },
                HeightM = 0.6,
                WeightKg = 12,
                Color = "Gray",
            },
            [SpecieId.Octillery] = new()
            {
                Id = SpecieId.Octillery,
                Num = 224,
                Name = "Octillery",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 105,
                    Def = 75,
                    SpA = 105,
                    SpD = 75,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SuctionCups,
                    Slot1 = AbilityId.Sniper,
                    Hidden = AbilityId.Moody,
                },
                HeightM = 0.9,
                WeightKg = 28.5,
                Color = "Red",
            },
            [SpecieId.Delibird] = new()
            {
                Id = SpecieId.Delibird,
                Num = 225,
                Name = "Delibird",
                Types = [PokemonType.Ice, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 55,
                    Def = 45,
                    SpA = 65,
                    SpD = 45,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VitalSpirit,
                    Slot1 = AbilityId.Hustle,
                    Hidden = AbilityId.Insomnia,
                },
                HeightM = 0.9,
                WeightKg = 16,
                Color = "Red",
            },
            [SpecieId.Mantine] = new()
            {
                Id = SpecieId.Mantine,
                Num = 226,
                Name = "Mantine",
                Types = [PokemonType.Water, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 40,
                    Def = 70,
                    SpA = 80,
                    SpD = 140,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.WaterAbsorb,
                    Hidden = AbilityId.WaterVeil,
                },
                HeightM = 2.1,
                WeightKg = 220,
                Color = "Purple",
            },
            [SpecieId.Skarmory] = new()
            {
                Id = SpecieId.Skarmory,
                Num = 227,
                Name = "Skarmory",
                Types = [PokemonType.Steel, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 80,
                    Def = 140,
                    SpA = 40,
                    SpD = 70,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.WeakArmor,
                },
                HeightM = 1.7,
                WeightKg = 50.5,
                Color = "Gray",
            },
            [SpecieId.SkarmoryMega] = new()
            {
                Id = SpecieId.SkarmoryMega,
                Num = 227,
                Name = "Skarmory-Mega",
                BaseSpecies = SpecieId.Skarmory,
                Forme = FormeId.Mega,
                Types = [PokemonType.Steel, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 140,
                    Def = 110,
                    SpA = 40,
                    SpD = 100,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.WeakArmor,
                },
                HeightM = 1.7,
                WeightKg = 40.4,
                Color = "Gray",
            },
            [SpecieId.Houndour] = new()
            {
                Id = SpecieId.Houndour,
                Num = 228,
                Name = "Houndour",
                Types = [PokemonType.Dark, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 60,
                    Def = 30,
                    SpA = 80,
                    SpD = 50,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EarlyBird,
                    Slot1 = AbilityId.FlashFire,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 0.6,
                WeightKg = 10.8,
                Color = "Black",
            },
            [SpecieId.Houndoom] = new()
            {
                Id = SpecieId.Houndoom,
                Num = 229,
                Name = "Houndoom",
                Types = [PokemonType.Dark, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 90,
                    Def = 50,
                    SpA = 110,
                    SpD = 80,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EarlyBird,
                    Slot1 = AbilityId.FlashFire,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 1.4,
                WeightKg = 35,
                Color = "Black",
            },
            [SpecieId.HoundoomMega] = new()
            {
                Id = SpecieId.HoundoomMega,
                Num = 229,
                Name = "Houndoom-Mega",
                BaseSpecies = SpecieId.Houndoom,
                Forme = FormeId.Mega,
                Types = [PokemonType.Dark, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 90,
                    Def = 90,
                    SpA = 140,
                    SpD = 90,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SolarPower,
                },
                HeightM = 1.9,
                WeightKg = 49.5,
                Color = "Black",
            },
            [SpecieId.Kingdra] = new()
            {
                Id = SpecieId.Kingdra,
                Num = 230,
                Name = "Kingdra",
                Types = [PokemonType.Water, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.Sniper,
                    Hidden = AbilityId.Damp,
                },
                HeightM = 1.8,
                WeightKg = 152,
                Color = "Blue",
            },
            [SpecieId.Phanpy] = new()
            {
                Id = SpecieId.Phanpy,
                Num = 231,
                Name = "Phanpy",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 60,
                    Def = 60,
                    SpA = 40,
                    SpD = 40,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 0.5,
                WeightKg = 33.5,
                Color = "Blue",
            },
            [SpecieId.Donphan] = new()
            {
                Id = SpecieId.Donphan,
                Num = 232,
                Name = "Donphan",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 120,
                    Def = 120,
                    SpA = 60,
                    SpD = 60,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 1.1,
                WeightKg = 120,
                Color = "Gray",
            },
            [SpecieId.Porygon2] = new()
            {
                Id = SpecieId.Porygon2,
                Num = 233,
                Name = "Porygon2",
                Types = [PokemonType.Normal],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 80,
                    Def = 90,
                    SpA = 105,
                    SpD = 95,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Trace,
                    Slot1 = AbilityId.Download,
                    Hidden = AbilityId.Analytic,
                },
                HeightM = 0.6,
                WeightKg = 32.5,
                Color = "Red",
            },
            [SpecieId.Stantler] = new()
            {
                Id = SpecieId.Stantler,
                Num = 234,
                Name = "Stantler",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 73,
                    Atk = 95,
                    Def = 62,
                    SpA = 85,
                    SpD = 65,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.SapSipper,
                },
                HeightM = 1.4,
                WeightKg = 71.2,
                Color = "Brown",
            },
            [SpecieId.Smeargle] = new()
            {
                Id = SpecieId.Smeargle,
                Num = 235,
                Name = "Smeargle",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 20,
                    Def = 35,
                    SpA = 20,
                    SpD = 45,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.OwnTempo,
                    Slot1 = AbilityId.Technician,
                    Hidden = AbilityId.Moody,
                },
                HeightM = 1.2,
                WeightKg = 58,
                Color = "White",
            },
            [SpecieId.Tyrogue] = new()
            {
                Id = SpecieId.Tyrogue,
                Num = 236,
                Name = "Tyrogue",
                Types = [PokemonType.Fighting],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 35,
                    Def = 35,
                    SpA = 35,
                    SpD = 35,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Guts,
                    Slot1 = AbilityId.Steadfast,
                    Hidden = AbilityId.VitalSpirit,
                },
                HeightM = 0.7,
                WeightKg = 21,
                Color = "Purple",
            },
            [SpecieId.Hitmontop] = new()
            {
                Id = SpecieId.Hitmontop,
                Num = 237,
                Name = "Hitmontop",
                Types = [PokemonType.Fighting],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 95,
                    Def = 95,
                    SpA = 35,
                    SpD = 110,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.Technician,
                    Hidden = AbilityId.Steadfast,
                },
                HeightM = 1.4,
                WeightKg = 48,
                Color = "Brown",
            },
            [SpecieId.Smoochum] = new()
            {
                Id = SpecieId.Smoochum,
                Num = 238,
                Name = "Smoochum",
                Types = [PokemonType.Ice, PokemonType.Psychic],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 30,
                    Def = 15,
                    SpA = 85,
                    SpD = 65,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.Forewarn,
                    Hidden = AbilityId.Hydration,
                },
                HeightM = 0.4,
                WeightKg = 6,
                Color = "Pink",
            },
            [SpecieId.Elekid] = new()
            {
                Id = SpecieId.Elekid,
                Num = 239,
                Name = "Elekid",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 63,
                    Def = 37,
                    SpA = 65,
                    SpD = 55,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.VitalSpirit,
                },
                HeightM = 0.6,
                WeightKg = 23.5,
                Color = "Yellow",
            },
            [SpecieId.Magby] = new()
            {
                Id = SpecieId.Magby,
                Num = 240,
                Name = "Magby",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 75,
                    Def = 37,
                    SpA = 70,
                    SpD = 55,
                    Spe = 83,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlameBody,
                    Hidden = AbilityId.VitalSpirit,
                },
                HeightM = 0.7,
                WeightKg = 21.4,
                Color = "Red",
            },
            [SpecieId.Miltank] = new()
            {
                Id = SpecieId.Miltank,
                Num = 241,
                Name = "Miltank",
                Types = [PokemonType.Normal],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 80,
                    Def = 105,
                    SpA = 40,
                    SpD = 70,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.Scrappy,
                    Hidden = AbilityId.SapSipper,
                },
                HeightM = 1.2,
                WeightKg = 75.5,
                Color = "Pink",
            },
            [SpecieId.Blissey] = new()
            {
                Id = SpecieId.Blissey,
                Num = 242,
                Name = "Blissey",
                Types = [PokemonType.Normal],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 255,
                    Atk = 10,
                    Def = 10,
                    SpA = 75,
                    SpD = 135,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Slot1 = AbilityId.SereneGrace,
                    Hidden = AbilityId.Healer,
                },
                HeightM = 1.5,
                WeightKg = 46.8,
                Color = "Pink",
            },
            [SpecieId.Raikou] = new()
            {
                Id = SpecieId.Raikou,
                Num = 243,
                Name = "Raikou",
                Types = [PokemonType.Electric],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 85,
                    Def = 75,
                    SpA = 115,
                    SpD = 100,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 1.9,
                WeightKg = 178,
                Color = "Yellow",
            },
            [SpecieId.Entei] = new()
            {
                Id = SpecieId.Entei,
                Num = 244,
                Name = "Entei",
                Types = [PokemonType.Fire],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 115,
                    Atk = 115,
                    Def = 85,
                    SpA = 90,
                    SpD = 75,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 2.1,
                WeightKg = 198,
                Color = "Brown",
            },
            [SpecieId.Suicune] = new()
            {
                Id = SpecieId.Suicune,
                Num = 245,
                Name = "Suicune",
                Types = [PokemonType.Water],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 75,
                    Def = 115,
                    SpA = 90,
                    SpD = 115,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 2.0,
                WeightKg = 187,
                Color = "Blue",
            },
            [SpecieId.Larvitar] = new()
            {
                Id = SpecieId.Larvitar,
                Num = 246,
                Name = "Larvitar",
                Types = [PokemonType.Rock, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 64,
                    Def = 50,
                    SpA = 45,
                    SpD = 50,
                    Spe = 41,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Guts,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 0.6,
                WeightKg = 72,
                Color = "Green",
            },
            [SpecieId.Pupitar] = new()
            {
                Id = SpecieId.Pupitar,
                Num = 247,
                Name = "Pupitar",
                Types = [PokemonType.Rock, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 84,
                    Def = 70,
                    SpA = 65,
                    SpD = 70,
                    Spe = 51,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                },
                HeightM = 1.2,
                WeightKg = 152,
                Color = "Gray",
            },
            [SpecieId.Tyranitar] = new()
            {
                Id = SpecieId.Tyranitar,
                Num = 248,
                Name = "Tyranitar",
                Types = [PokemonType.Rock, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 134,
                    Def = 110,
                    SpA = 95,
                    SpD = 100,
                    Spe = 61,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandStream,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 2.0,
                WeightKg = 202,
                Color = "Green",
            },
            [SpecieId.TyranitarMega] = new()
            {
                Id = SpecieId.TyranitarMega,
                Num = 248,
                Name = "Tyranitar-Mega",
                BaseSpecies = SpecieId.Tyranitar,
                Forme = FormeId.Mega,
                Types = [PokemonType.Rock, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 164,
                    Def = 150,
                    SpA = 95,
                    SpD = 120,
                    Spe = 71,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandStream,
                },
                HeightM = 2.5,
                WeightKg = 255,
                Color = "Green",
            },
            [SpecieId.Lugia] = new()
            {
                Id = SpecieId.Lugia,
                Num = 249,
                Name = "Lugia",
                Types = [PokemonType.Psychic, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 106,
                    Atk = 90,
                    Def = 130,
                    SpA = 90,
                    SpD = 154,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.Multiscale,
                },
                HeightM = 5.2,
                WeightKg = 216,
                Color = "White",
            },
            [SpecieId.HoOh] = new()
            {
                Id = SpecieId.HoOh,
                Num = 250,
                Name = "Ho-Oh",
                Types = [PokemonType.Fire, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 106,
                    Atk = 130,
                    Def = 90,
                    SpA = 110,
                    SpD = 154,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 3.8,
                WeightKg = 199,
                Color = "Red",
            },
        };
    }
}