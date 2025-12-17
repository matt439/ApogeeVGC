using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData451To500()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Skorupi] = new()
            {
                Id = SpecieId.Skorupi,
                Num = 451,
                Name = "Skorupi",
                Types = [PokemonType.Poison, PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 50,
                    Def = 90,
                    SpA = 30,
                    SpD = 55,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                    Slot1 = AbilityId.Sniper,
                    Hidden = AbilityId.KeenEye,
                },
                HeightM = 0.8,
                WeightKg = 12,
                Color = "Purple",
            },
            [SpecieId.Drapion] = new()
            {
                Id = SpecieId.Drapion,
                Num = 452,
                Name = "Drapion",
                Types = [PokemonType.Poison, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 90,
                    Def = 110,
                    SpA = 60,
                    SpD = 75,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                    Slot1 = AbilityId.Sniper,
                    Hidden = AbilityId.KeenEye,
                },
                HeightM = 1.3,
                WeightKg = 61.5,
                Color = "Purple",
                Prevo = SpecieId.Skorupi,
            },
            [SpecieId.Croagunk] = new()
            {
                Id = SpecieId.Croagunk,
                Num = 453,
                Name = "Croagunk",
                Types = [PokemonType.Poison, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 48,
                    Atk = 61,
                    Def = 40,
                    SpA = 61,
                    SpD = 40,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Anticipation,
                    Slot1 = AbilityId.DrySkin,
                    Hidden = AbilityId.PoisonTouch,
                },
                HeightM = 0.7,
                WeightKg = 23,
                Color = "Blue",
            },
            [SpecieId.Toxicroak] = new()
            {
                Id = SpecieId.Toxicroak,
                Num = 454,
                Name = "Toxicroak",
                Types = [PokemonType.Poison, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 83,
                    Atk = 106,
                    Def = 65,
                    SpA = 86,
                    SpD = 65,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Anticipation,
                    Slot1 = AbilityId.DrySkin,
                    Hidden = AbilityId.PoisonTouch,
                },
                HeightM = 1.3,
                WeightKg = 44.4,
                Color = "Blue",
                Prevo = SpecieId.Croagunk,
            },
            [SpecieId.Carnivine] = new()
            {
                Id = SpecieId.Carnivine,
                Num = 455,
                Name = "Carnivine",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 74,
                    Atk = 100,
                    Def = 72,
                    SpA = 90,
                    SpD = 72,
                    Spe = 46,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 1.4,
                WeightKg = 27,
                Color = "Green",
            },
            [SpecieId.Finneon] = new()
            {
                Id = SpecieId.Finneon,
                Num = 456,
                Name = "Finneon",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 49,
                    Atk = 49,
                    Def = 56,
                    SpA = 49,
                    SpD = 61,
                    Spe = 66,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.StormDrain,
                    Hidden = AbilityId.WaterVeil,
                },
                HeightM = 0.4,
                WeightKg = 7,
                Color = "Blue",
            },
            [SpecieId.Lumineon] = new()
            {
                Id = SpecieId.Lumineon,
                Num = 457,
                Name = "Lumineon",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 69,
                    Atk = 69,
                    Def = 76,
                    SpA = 69,
                    SpD = 86,
                    Spe = 91,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.StormDrain,
                    Hidden = AbilityId.WaterVeil,
                },
                HeightM = 1.2,
                WeightKg = 24,
                Color = "Blue",
                Prevo = SpecieId.Finneon,
            },
            [SpecieId.Mantyke] = new()
            {
                Id = SpecieId.Mantyke,
                Num = 458,
                Name = "Mantyke",
                Types = [PokemonType.Water, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 20,
                    Def = 50,
                    SpA = 60,
                    SpD = 120,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.WaterAbsorb,
                    Hidden = AbilityId.WaterVeil,
                },
                HeightM = 1,
                WeightKg = 65,
                Color = "Blue",
            },
            [SpecieId.Snover] = new()
            {
                Id = SpecieId.Snover,
                Num = 459,
                Name = "Snover",
                Types = [PokemonType.Grass, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 62,
                    Def = 50,
                    SpA = 62,
                    SpD = 60,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowWarning,
                    Hidden = AbilityId.Soundproof,
                },
                HeightM = 1,
                WeightKg = 50.5,
                Color = "White",
            },
            [SpecieId.Abomasnow] = new()
            {
                Id = SpecieId.Abomasnow,
                Num = 460,
                Name = "Abomasnow",
                Types = [PokemonType.Grass, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 92,
                    Def = 75,
                    SpA = 92,
                    SpD = 85,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowWarning,
                    Hidden = AbilityId.Soundproof,
                },
                HeightM = 2.2,
                WeightKg = 135.5,
                Color = "White",
                Prevo = SpecieId.Snover,
            },
            [SpecieId.AbomasnowMega] = new()
            {
                Id = SpecieId.AbomasnowMega,
                Num = 460,
                Name = "Abomasnow-Mega",
                Types = [PokemonType.Grass, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 132,
                    Def = 105,
                    SpA = 132,
                    SpD = 105,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowWarning,
                },
                HeightM = 2.7,
                WeightKg = 185,
                Color = "White",
                BaseSpecies = SpecieId.Abomasnow,
                Forme = FormeId.Mega,
            },
            [SpecieId.Weavile] = new()
            {
                Id = SpecieId.Weavile,
                Num = 461,
                Name = "Weavile",
                Types = [PokemonType.Dark, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 120,
                    Def = 65,
                    SpA = 45,
                    SpD = 85,
                    Spe = 125,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 1.1,
                WeightKg = 34,
                Color = "Black",
                Prevo = SpecieId.Sneasel,
            },
            [SpecieId.Magnezone] = new()
            {
                Id = SpecieId.Magnezone,
                Num = 462,
                Name = "Magnezone",
                Types = [PokemonType.Electric, PokemonType.Steel],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 70,
                    Def = 115,
                    SpA = 130,
                    SpD = 90,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagnetPull,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.Analytic,
                },
                HeightM = 1.2,
                WeightKg = 180,
                Color = "Gray",
                Prevo = SpecieId.Magneton,
            },
            [SpecieId.Lickilicky] = new()
            {
                Id = SpecieId.Lickilicky,
                Num = 463,
                Name = "Lickilicky",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 110,
                    Atk = 85,
                    Def = 95,
                    SpA = 80,
                    SpD = 95,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.OwnTempo,
                    Slot1 = AbilityId.Oblivious,
                    Hidden = AbilityId.CloudNine,
                },
                HeightM = 1.7,
                WeightKg = 140,
                Color = "Pink",
                Prevo = SpecieId.Lickitung,
            },
            [SpecieId.Rhyperior] = new()
            {
                Id = SpecieId.Rhyperior,
                Num = 464,
                Name = "Rhyperior",
                Types = [PokemonType.Ground, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 115,
                    Atk = 140,
                    Def = 130,
                    SpA = 55,
                    SpD = 55,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LightningRod,
                    Slot1 = AbilityId.SolidRock,
                    Hidden = AbilityId.Reckless,
                },
                HeightM = 2.4,
                WeightKg = 282.8,
                Color = "Gray",
                Prevo = SpecieId.Rhydon,
            },
            [SpecieId.Tangrowth] = new()
            {
                Id = SpecieId.Tangrowth,
                Num = 465,
                Name = "Tangrowth",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 100,
                    Def = 125,
                    SpA = 110,
                    SpD = 50,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.LeafGuard,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 2,
                WeightKg = 128.6,
                Color = "Blue",
                Prevo = SpecieId.Tangela,
            },
            [SpecieId.Electivire] = new()
            {
                Id = SpecieId.Electivire,
                Num = 466,
                Name = "Electivire",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 123,
                    Def = 67,
                    SpA = 95,
                    SpD = 85,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MotorDrive,
                    Hidden = AbilityId.VitalSpirit,
                },
                HeightM = 1.8,
                WeightKg = 138.6,
                Color = "Yellow",
                Prevo = SpecieId.Electabuzz,
            },
            [SpecieId.Magmortar] = new()
            {
                Id = SpecieId.Magmortar,
                Num = 467,
                Name = "Magmortar",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 95,
                    Def = 67,
                    SpA = 125,
                    SpD = 95,
                    Spe = 83,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlameBody,
                    Hidden = AbilityId.VitalSpirit,
                },
                HeightM = 1.6,
                WeightKg = 68,
                Color = "Red",
                Prevo = SpecieId.Magmar,
            },
            [SpecieId.Togekiss] = new()
            {
                Id = SpecieId.Togekiss,
                Num = 468,
                Name = "Togekiss",
                Types = [PokemonType.Fairy, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 50,
                    Def = 95,
                    SpA = 120,
                    SpD = 115,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Hustle,
                    Slot1 = AbilityId.SereneGrace,
                    Hidden = AbilityId.SuperLuck,
                },
                HeightM = 1.5,
                WeightKg = 38,
                Color = "White",
                Prevo = SpecieId.Togetic,
            },
            [SpecieId.Yanmega] = new()
            {
                Id = SpecieId.Yanmega,
                Num = 469,
                Name = "Yanmega",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 86,
                    Atk = 76,
                    Def = 86,
                    SpA = 116,
                    SpD = 56,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SpeedBoost,
                    Slot1 = AbilityId.TintedLens,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 1.9,
                WeightKg = 51.5,
                Color = "Green",
                Prevo = SpecieId.Yanma,
            },
            [SpecieId.Leafeon] = new()
            {
                Id = SpecieId.Leafeon,
                Num = 470,
                Name = "Leafeon",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 110,
                    Def = 130,
                    SpA = 60,
                    SpD = 65,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LeafGuard,
                    Hidden = AbilityId.Chlorophyll,
                },
                HeightM = 1,
                WeightKg = 25.5,
                Color = "Green",
                Prevo = SpecieId.Eevee,
            },
            [SpecieId.Glaceon] = new()
            {
                Id = SpecieId.Glaceon,
                Num = 471,
                Name = "Glaceon",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 60,
                    Def = 110,
                    SpA = 130,
                    SpD = 95,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowCloak,
                    Hidden = AbilityId.IceBody,
                },
                HeightM = 0.8,
                WeightKg = 25.9,
                Color = "Blue",
                Prevo = SpecieId.Eevee,
            },
            [SpecieId.Gliscor] = new()
            {
                Id = SpecieId.Gliscor,
                Num = 472,
                Name = "Gliscor",
                Types = [PokemonType.Ground, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 95,
                    Def = 125,
                    SpA = 45,
                    SpD = 75,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.SandVeil,
                    Hidden = AbilityId.PoisonHeal,
                },
                HeightM = 2,
                WeightKg = 42.5,
                Color = "Purple",
                Prevo = SpecieId.Gligar,
            },
            [SpecieId.Mamoswine] = new()
            {
                Id = SpecieId.Mamoswine,
                Num = 473,
                Name = "Mamoswine",
                Types = [PokemonType.Ice, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 110,
                    Atk = 130,
                    Def = 80,
                    SpA = 70,
                    SpD = 60,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.SnowCloak,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 2.5,
                WeightKg = 291,
                Color = "Brown",
                Prevo = SpecieId.Piloswine,
            },
            [SpecieId.PorygonZ] = new()
            {
                Id = SpecieId.PorygonZ,
                Num = 474,
                Name = "Porygon-Z",
                Types = [PokemonType.Normal],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 80,
                    Def = 70,
                    SpA = 135,
                    SpD = 75,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Adaptability,
                    Slot1 = AbilityId.Download,
                    Hidden = AbilityId.Analytic,
                },
                HeightM = 0.9,
                WeightKg = 34,
                Color = "Red",
                Prevo = SpecieId.Porygon2,
            },
            [SpecieId.Gallade] = new()
            {
                Id = SpecieId.Gallade,
                Num = 475,
                Name = "Gallade",
                Types = [PokemonType.Psychic, PokemonType.Fighting],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 125,
                    Def = 65,
                    SpA = 65,
                    SpD = 115,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Steadfast,
                    Slot1 = AbilityId.Sharpness,
                    Hidden = AbilityId.Justified,
                },
                HeightM = 1.6,
                WeightKg = 52,
                Color = "White",
                Prevo = SpecieId.Kirlia,
            },
            [SpecieId.GalladeMega] = new()
            {
                Id = SpecieId.GalladeMega,
                Num = 475,
                Name = "Gallade-Mega",
                Types = [PokemonType.Psychic, PokemonType.Fighting],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 165,
                    Def = 95,
                    SpA = 65,
                    SpD = 115,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                },
                HeightM = 1.6,
                WeightKg = 56.4,
                Color = "White",
                BaseSpecies = SpecieId.Gallade,
                Forme = FormeId.Mega,
            },
            [SpecieId.Probopass] = new()
            {
                Id = SpecieId.Probopass,
                Num = 476,
                Name = "Probopass",
                Types = [PokemonType.Rock, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 55,
                    Def = 145,
                    SpA = 75,
                    SpD = 150,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Slot1 = AbilityId.MagnetPull,
                    Hidden = AbilityId.SandForce,
                },
                HeightM = 1.4,
                WeightKg = 340,
                Color = "Gray",
                Prevo = SpecieId.Nosepass,
            },
            [SpecieId.Dusknoir] = new()
            {
                Id = SpecieId.Dusknoir,
                Num = 477,
                Name = "Dusknoir",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 100,
                    Def = 135,
                    SpA = 65,
                    SpD = 135,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 2.2,
                WeightKg = 106.6,
                Color = "Black",
                Prevo = SpecieId.Dusclops,
            },
            [SpecieId.Froslass] = new()
            {
                Id = SpecieId.Froslass,
                Num = 478,
                Name = "Froslass",
                Types = [PokemonType.Ice, PokemonType.Ghost],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 80,
                    Def = 70,
                    SpA = 80,
                    SpD = 70,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowCloak,
                    Hidden = AbilityId.CursedBody,
                },
                HeightM = 1.3,
                WeightKg = 26.6,
                Color = "White",
                Prevo = SpecieId.Snorunt,
            },
            [SpecieId.FroslassMega] = new()
            {
                Id = SpecieId.FroslassMega,
                Num = 478,
                Name = "Froslass-Mega",
                Types = [PokemonType.Ice, PokemonType.Ghost],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 80,
                    Def = 70,
                    SpA = 140,
                    SpD = 100,
                    Spe = 120,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowCloak,
                    Hidden = AbilityId.CursedBody,
                },
                HeightM = 2.6,
                WeightKg = 29.6,
                Color = "White",
                BaseSpecies = SpecieId.Froslass,
                Forme = FormeId.Mega,
            },
            [SpecieId.Rotom] = new()
            {
                Id = SpecieId.Rotom,
                Num = 479,
                Name = "Rotom",
                Types = [PokemonType.Electric, PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 50,
                    Def = 77,
                    SpA = 95,
                    SpD = 77,
                    Spe = 91,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.3,
                WeightKg = 0.3,
                Color = "Red",
            },
            [SpecieId.RotomHeat] = new()
            {
                Id = SpecieId.RotomHeat,
                Num = 479,
                Name = "Rotom-Heat",
                Types = [PokemonType.Electric, PokemonType.Fire],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 107,
                    SpA = 105,
                    SpD = 107,
                    Spe = 86,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.3,
                WeightKg = 0.3,
                Color = "Red",
                BaseSpecies = SpecieId.Rotom,
                Forme = FormeId.Heat,
            },
            [SpecieId.RotomWash] = new()
            {
                Id = SpecieId.RotomWash,
                Num = 479,
                Name = "Rotom-Wash",
                Types = [PokemonType.Electric, PokemonType.Water],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 107,
                    SpA = 105,
                    SpD = 107,
                    Spe = 86,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.3,
                WeightKg = 0.3,
                Color = "Red",
                BaseSpecies = SpecieId.Rotom,
                Forme = FormeId.Wash,
            },
            [SpecieId.RotomFrost] = new()
            {
                Id = SpecieId.RotomFrost,
                Num = 479,
                Name = "Rotom-Frost",
                Types = [PokemonType.Electric, PokemonType.Ice],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 107,
                    SpA = 105,
                    SpD = 107,
                    Spe = 86,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.3,
                WeightKg = 0.3,
                Color = "Red",
                BaseSpecies = SpecieId.Rotom,
                Forme = FormeId.Frost,
            },
            [SpecieId.RotomFan] = new()
            {
                Id = SpecieId.RotomFan,
                Num = 479,
                Name = "Rotom-Fan",
                Types = [PokemonType.Electric, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 107,
                    SpA = 105,
                    SpD = 107,
                    Spe = 86,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.3,
                WeightKg = 0.3,
                Color = "Red",
                BaseSpecies = SpecieId.Rotom,
                Forme = FormeId.Fan,
            },
            [SpecieId.RotomMow] = new()
            {
                Id = SpecieId.RotomMow,
                Num = 479,
                Name = "Rotom-Mow",
                Types = [PokemonType.Electric, PokemonType.Grass],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 107,
                    SpA = 105,
                    SpD = 107,
                    Spe = 86,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.3,
                WeightKg = 0.3,
                Color = "Red",
                BaseSpecies = SpecieId.Rotom,
                Forme = FormeId.Mow,
            },
            [SpecieId.Uxie] = new()
            {
                Id = SpecieId.Uxie,
                Num = 480,
                Name = "Uxie",
                Types = [PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 75,
                    Def = 130,
                    SpA = 75,
                    SpD = 130,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.3,
                WeightKg = 0.3,
                Color = "Yellow",
            },
        };
    }
}
