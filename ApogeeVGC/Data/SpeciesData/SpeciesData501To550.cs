using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData501To550()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Victini] = new()
            {
                Id = SpecieId.Victini,
                Num = 494,
                Name = "Victini",
                Types = [PokemonType.Psychic, PokemonType.Fire],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 100,
                    Def = 100,
                    SpA = 100,
                    SpD = 100,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VictoryStar,
                },
                HeightM = 0.4,
                WeightKg = 4,
                Color = "Yellow",
            },
            [SpecieId.Snivy] = new()
            {
                Id = SpecieId.Snivy,
                Num = 495,
                Name = "Snivy",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 45,
                    Def = 55,
                    SpA = 45,
                    SpD = 55,
                    Spe = 63,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Contrary,
                },
                HeightM = 0.6,
                WeightKg = 8.1,
                Color = "Green",
            },
            [SpecieId.Servine] = new()
            {
                Id = SpecieId.Servine,
                Num = 496,
                Name = "Servine",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 60,
                    Def = 75,
                    SpA = 60,
                    SpD = 75,
                    Spe = 83,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Contrary,
                },
                HeightM = 0.8,
                WeightKg = 16,
                Color = "Green",
                Prevo = SpecieId.Snivy,
            },
            [SpecieId.Serperior] = new()
            {
                Id = SpecieId.Serperior,
                Num = 497,
                Name = "Serperior",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 75,
                    Def = 95,
                    SpA = 75,
                    SpD = 95,
                    Spe = 113,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Contrary,
                },
                HeightM = 3.3,
                WeightKg = 63,
                Color = "Green",
                Prevo = SpecieId.Servine,
            },
            [SpecieId.Tepig] = new()
            {
                Id = SpecieId.Tepig,
                Num = 498,
                Name = "Tepig",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 63,
                    Def = 45,
                    SpA = 45,
                    SpD = 45,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 0.5,
                WeightKg = 9.9,
                Color = "Red",
            },
            [SpecieId.Pignite] = new()
            {
                Id = SpecieId.Pignite,
                Num = 499,
                Name = "Pignite",
                Types = [PokemonType.Fire, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 93,
                    Def = 55,
                    SpA = 70,
                    SpD = 55,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 1,
                WeightKg = 55.5,
                Color = "Red",
                Prevo = SpecieId.Tepig,
            },
            [SpecieId.Emboar] = new()
            {
                Id = SpecieId.Emboar,
                Num = 500,
                Name = "Emboar",
                Types = [PokemonType.Fire, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 110,
                    Atk = 123,
                    Def = 65,
                    SpA = 100,
                    SpD = 65,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Reckless,
                },
                HeightM = 1.6,
                WeightKg = 150,
                Color = "Red",
                Prevo = SpecieId.Pignite,
            },
            [SpecieId.EmboarMega] = new()
            {
                Id = SpecieId.EmboarMega,
                Num = 500,
                Name = "Emboar-Mega",
                Types = [PokemonType.Fire, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 110,
                    Atk = 163,
                    Def = 85,
                    SpA = 130,
                    SpD = 85,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Reckless,
                },
                                HeightM = 1.6,
                                WeightKg = 165,
                                Color = "Red",
                                BaseSpecies = SpecieId.Emboar,
                                Forme = FormeId.Mega,
                            },
                            [SpecieId.Oshawott] = new()
                            {
                                Id = SpecieId.Oshawott,
                                Num = 501,
                                Name = "Oshawott",
                                Types = [PokemonType.Water],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 55,
                                    Atk = 55,
                                    Def = 45,
                                    SpA = 63,
                                    SpD = 45,
                                    Spe = 45,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Torrent,
                                    Hidden = AbilityId.ShellArmor,
                                },
                                HeightM = 0.5,
                                WeightKg = 5.9,
                                Color = "Blue",
                            },
                            [SpecieId.Dewott] = new()
                            {
                                Id = SpecieId.Dewott,
                                Num = 502,
                                Name = "Dewott",
                                Types = [PokemonType.Water],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 75,
                                    Atk = 75,
                                    Def = 60,
                                    SpA = 83,
                                    SpD = 60,
                                    Spe = 60,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Torrent,
                                    Hidden = AbilityId.ShellArmor,
                                },
                                HeightM = 0.8,
                                WeightKg = 24.5,
                                Color = "Blue",
                                Prevo = SpecieId.Oshawott,
                            },
                            [SpecieId.Samurott] = new()
                            {
                                Id = SpecieId.Samurott,
                                Num = 503,
                                Name = "Samurott",
                                Types = [PokemonType.Water],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 95,
                                    Atk = 100,
                                    Def = 85,
                                    SpA = 108,
                                    SpD = 70,
                                    Spe = 70,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Torrent,
                                    Hidden = AbilityId.ShellArmor,
                                },
                                HeightM = 1.5,
                                WeightKg = 94.6,
                                Color = "Blue",
                                Prevo = SpecieId.Dewott,
                            },
                            [SpecieId.SamurottHisui] = new()
                            {
                                Id = SpecieId.SamurottHisui,
                                Num = 503,
                                Name = "Samurott-Hisui",
                                Types = [PokemonType.Water, PokemonType.Dark],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 90,
                                    Atk = 108,
                                    Def = 80,
                                    SpA = 100,
                                    SpD = 65,
                                    Spe = 85,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Torrent,
                                    Hidden = AbilityId.Sharpness,
                                },
                                HeightM = 1.5,
                                WeightKg = 58.2,
                                Color = "Blue",
                                BaseSpecies = SpecieId.Samurott,
                                Forme = FormeId.Hisui,
                                Prevo = SpecieId.Dewott,
                            },
                            [SpecieId.Patrat] = new()
                            {
                                Id = SpecieId.Patrat,
                                Num = 504,
                                Name = "Patrat",
                                Types = [PokemonType.Normal],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 45,
                                    Atk = 55,
                                    Def = 39,
                                    SpA = 35,
                                    SpD = 39,
                                    Spe = 42,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.RunAway,
                                    Slot1 = AbilityId.KeenEye,
                                    Hidden = AbilityId.Analytic,
                                },
                                HeightM = 0.5,
                                WeightKg = 11.6,
                                Color = "Brown",
                            },
                            [SpecieId.Watchog] = new()
                            {
                                Id = SpecieId.Watchog,
                                Num = 505,
                                Name = "Watchog",
                                Types = [PokemonType.Normal],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 60,
                                    Atk = 85,
                                    Def = 69,
                                    SpA = 60,
                                    SpD = 69,
                                    Spe = 77,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Illuminate,
                                    Slot1 = AbilityId.KeenEye,
                                    Hidden = AbilityId.Analytic,
                                },
                                HeightM = 1.1,
                                WeightKg = 27,
                                Color = "Brown",
                                Prevo = SpecieId.Patrat,
                            },
                            [SpecieId.Lillipup] = new()
                            {
                                Id = SpecieId.Lillipup,
                                Num = 506,
                                Name = "Lillipup",
                                Types = [PokemonType.Normal],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 45,
                                    Atk = 60,
                                    Def = 45,
                                    SpA = 25,
                                    SpD = 45,
                                    Spe = 55,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.VitalSpirit,
                                    Slot1 = AbilityId.Pickup,
                                    Hidden = AbilityId.RunAway,
                                },
                                HeightM = 0.4,
                                WeightKg = 4.1,
                                Color = "Brown",
                            },
                            [SpecieId.Herdier] = new()
                            {
                                Id = SpecieId.Herdier,
                                Num = 507,
                                Name = "Herdier",
                                Types = [PokemonType.Normal],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 65,
                                    Atk = 80,
                                    Def = 65,
                                    SpA = 35,
                                    SpD = 65,
                                    Spe = 60,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Intimidate,
                                    Slot1 = AbilityId.SandRush,
                                    Hidden = AbilityId.Scrappy,
                                },
                                HeightM = 0.9,
                                WeightKg = 14.7,
                                Color = "Gray",
                                Prevo = SpecieId.Lillipup,
                            },
                            [SpecieId.Stoutland] = new()
                            {
                                Id = SpecieId.Stoutland,
                                Num = 508,
                                Name = "Stoutland",
                                Types = [PokemonType.Normal],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 85,
                                    Atk = 110,
                                    Def = 90,
                                    SpA = 45,
                                    SpD = 90,
                                    Spe = 80,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Intimidate,
                                    Slot1 = AbilityId.SandRush,
                                    Hidden = AbilityId.Scrappy,
                                },
                                HeightM = 1.2,
                                WeightKg = 61,
                                Color = "Gray",
                                Prevo = SpecieId.Herdier,
                            },
                            [SpecieId.Purrloin] = new()
                            {
                                Id = SpecieId.Purrloin,
                                Num = 509,
                                Name = "Purrloin",
                                Types = [PokemonType.Dark],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 41,
                                    Atk = 50,
                                    Def = 37,
                                    SpA = 50,
                                    SpD = 37,
                                    Spe = 66,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Limber,
                                    Slot1 = AbilityId.Unburden,
                                    Hidden = AbilityId.Prankster,
                                },
                                HeightM = 0.4,
                                WeightKg = 10.1,
                                Color = "Purple",
                            },
                            [SpecieId.Liepard] = new()
                            {
                                Id = SpecieId.Liepard,
                                Num = 510,
                                Name = "Liepard",
                                Types = [PokemonType.Dark],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 64,
                                    Atk = 88,
                                    Def = 50,
                                    SpA = 88,
                                    SpD = 50,
                                    Spe = 106,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Limber,
                                    Slot1 = AbilityId.Unburden,
                                    Hidden = AbilityId.Prankster,
                                },
                                HeightM = 1.1,
                                WeightKg = 37.5,
                                Color = "Purple",
                                Prevo = SpecieId.Purrloin,
                            },
                            [SpecieId.Pansage] = new()
                            {
                                Id = SpecieId.Pansage,
                                Num = 511,
                                Name = "Pansage",
                                Types = [PokemonType.Grass],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 50,
                                    Atk = 53,
                                    Def = 48,
                                    SpA = 53,
                                    SpD = 48,
                                    Spe = 64,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Gluttony,
                                    Hidden = AbilityId.Overgrow,
                                },
                                HeightM = 0.6,
                                WeightKg = 10.5,
                                Color = "Green",
                            },
                            [SpecieId.Simisage] = new()
                            {
                                Id = SpecieId.Simisage,
                                Num = 512,
                                Name = "Simisage",
                                Types = [PokemonType.Grass],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 75,
                                    Atk = 98,
                                    Def = 63,
                                    SpA = 98,
                                    SpD = 63,
                                    Spe = 101,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Gluttony,
                                    Hidden = AbilityId.Overgrow,
                                },
                                HeightM = 1.1,
                                WeightKg = 30.5,
                                Color = "Green",
                                Prevo = SpecieId.Pansage,
                            },
                            [SpecieId.Pansear] = new()
                            {
                                Id = SpecieId.Pansear,
                                Num = 513,
                                Name = "Pansear",
                                Types = [PokemonType.Fire],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 50,
                                    Atk = 53,
                                    Def = 48,
                                    SpA = 53,
                                    SpD = 48,
                                    Spe = 64,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Gluttony,
                                    Hidden = AbilityId.Blaze,
                                },
                                HeightM = 0.6,
                                WeightKg = 11,
                                Color = "Red",
                            },
                            [SpecieId.Simisear] = new()
                            {
                                Id = SpecieId.Simisear,
                                Num = 514,
                                Name = "Simisear",
                                Types = [PokemonType.Fire],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 75,
                                    Atk = 98,
                                    Def = 63,
                                    SpA = 98,
                                    SpD = 63,
                                    Spe = 101,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Gluttony,
                                    Hidden = AbilityId.Blaze,
                                },
                                HeightM = 1,
                                WeightKg = 28,
                                Color = "Red",
                                Prevo = SpecieId.Pansear,
                            },
                            [SpecieId.Panpour] = new()
                            {
                                Id = SpecieId.Panpour,
                                Num = 515,
                                Name = "Panpour",
                                Types = [PokemonType.Water],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 50,
                                    Atk = 53,
                                    Def = 48,
                                    SpA = 53,
                                    SpD = 48,
                                    Spe = 64,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Gluttony,
                                    Hidden = AbilityId.Torrent,
                                },
                                HeightM = 0.6,
                                WeightKg = 13.5,
                                Color = "Blue",
                            },
                            [SpecieId.Simipour] = new()
                            {
                                Id = SpecieId.Simipour,
                                Num = 516,
                                Name = "Simipour",
                                Types = [PokemonType.Water],
                                Gender = GenderId.Empty,
                                BaseStats = new StatsTable
                                {
                                    Hp = 75,
                                    Atk = 98,
                                    Def = 63,
                                    SpA = 98,
                                    SpD = 63,
                                    Spe = 101,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Gluttony,
                                    Hidden = AbilityId.Torrent,
                                },
                                                HeightM = 1,
                                                WeightKg = 29,
                                                Color = "Blue",
                                                Prevo = SpecieId.Panpour,
                                            },
                                            [SpecieId.Munna] = new()
                                            {
                                                Id = SpecieId.Munna,
                                                Num = 517,
                                                Name = "Munna",
                                                Types = [PokemonType.Psychic],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 76,
                                                    Atk = 25,
                                                    Def = 45,
                                                    SpA = 67,
                                                    SpD = 55,
                                                    Spe = 24,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Forewarn,
                                                    Slot1 = AbilityId.Synchronize,
                                                    Hidden = AbilityId.Telepathy,
                                                },
                                                HeightM = 0.6,
                                                WeightKg = 23.3,
                                                Color = "Pink",
                                            },
                                            [SpecieId.Musharna] = new()
                                            {
                                                Id = SpecieId.Musharna,
                                                Num = 518,
                                                Name = "Musharna",
                                                Types = [PokemonType.Psychic],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 116,
                                                    Atk = 55,
                                                    Def = 85,
                                                    SpA = 107,
                                                    SpD = 95,
                                                    Spe = 29,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Forewarn,
                                                    Slot1 = AbilityId.Synchronize,
                                                    Hidden = AbilityId.Telepathy,
                                                },
                                                HeightM = 1.1,
                                                WeightKg = 60.5,
                                                Color = "Pink",
                                                Prevo = SpecieId.Munna,
                                            },
                                            [SpecieId.Pidove] = new()
                                            {
                                                Id = SpecieId.Pidove,
                                                Num = 519,
                                                Name = "Pidove",
                                                Types = [PokemonType.Normal, PokemonType.Flying],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 50,
                                                    Atk = 55,
                                                    Def = 50,
                                                    SpA = 36,
                                                    SpD = 30,
                                                    Spe = 43,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.BigPecks,
                                                    Slot1 = AbilityId.SuperLuck,
                                                    Hidden = AbilityId.Rivalry,
                                                },
                                                HeightM = 0.3,
                                                WeightKg = 2.1,
                                                Color = "Gray",
                                            },
                                            [SpecieId.Tranquill] = new()
                                            {
                                                Id = SpecieId.Tranquill,
                                                Num = 520,
                                                Name = "Tranquill",
                                                Types = [PokemonType.Normal, PokemonType.Flying],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 62,
                                                    Atk = 77,
                                                    Def = 62,
                                                    SpA = 50,
                                                    SpD = 42,
                                                    Spe = 65,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.BigPecks,
                                                    Slot1 = AbilityId.SuperLuck,
                                                    Hidden = AbilityId.Rivalry,
                                                },
                                                HeightM = 0.6,
                                                WeightKg = 15,
                                                Color = "Gray",
                                                Prevo = SpecieId.Pidove,
                                            },
                                            [SpecieId.Unfezant] = new()
                                            {
                                                Id = SpecieId.Unfezant,
                                                Num = 521,
                                                Name = "Unfezant",
                                                Types = [PokemonType.Normal, PokemonType.Flying],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 80,
                                                    Atk = 115,
                                                    Def = 80,
                                                    SpA = 65,
                                                    SpD = 55,
                                                    Spe = 93,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.BigPecks,
                                                    Slot1 = AbilityId.SuperLuck,
                                                    Hidden = AbilityId.Rivalry,
                                                },
                                                HeightM = 1.2,
                                                WeightKg = 29,
                                                Color = "Gray",
                                                Prevo = SpecieId.Tranquill,
                                            },
                                            [SpecieId.Blitzle] = new()
                                            {
                                                Id = SpecieId.Blitzle,
                                                Num = 522,
                                                Name = "Blitzle",
                                                Types = [PokemonType.Electric],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 45,
                                                    Atk = 60,
                                                    Def = 32,
                                                    SpA = 50,
                                                    SpD = 32,
                                                    Spe = 76,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.LightningRod,
                                                    Slot1 = AbilityId.MotorDrive,
                                                    Hidden = AbilityId.SapSipper,
                                                },
                                                HeightM = 0.8,
                                                WeightKg = 29.8,
                                                Color = "Black",
                                            },
                                            [SpecieId.Zebstrika] = new()
                                            {
                                                Id = SpecieId.Zebstrika,
                                                Num = 523,
                                                Name = "Zebstrika",
                                                Types = [PokemonType.Electric],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 75,
                                                    Atk = 100,
                                                    Def = 63,
                                                    SpA = 80,
                                                    SpD = 63,
                                                    Spe = 116,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.LightningRod,
                                                    Slot1 = AbilityId.MotorDrive,
                                                    Hidden = AbilityId.SapSipper,
                                                },
                                                HeightM = 1.6,
                                                WeightKg = 79.5,
                                                Color = "Black",
                                                Prevo = SpecieId.Blitzle,
                                            },
                                            [SpecieId.Roggenrola] = new()
                                            {
                                                Id = SpecieId.Roggenrola,
                                                Num = 524,
                                                Name = "Roggenrola",
                                                Types = [PokemonType.Rock],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 55,
                                                    Atk = 75,
                                                    Def = 85,
                                                    SpA = 25,
                                                    SpD = 25,
                                                    Spe = 15,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Sturdy,
                                                    Slot1 = AbilityId.WeakArmor,
                                                    Hidden = AbilityId.SandForce,
                                                },
                                                HeightM = 0.4,
                                                WeightKg = 18,
                                                Color = "Blue",
                                            },
                                            [SpecieId.Boldore] = new()
                                            {
                                                Id = SpecieId.Boldore,
                                                Num = 525,
                                                Name = "Boldore",
                                                Types = [PokemonType.Rock],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 70,
                                                    Atk = 105,
                                                    Def = 105,
                                                    SpA = 50,
                                                    SpD = 40,
                                                    Spe = 20,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Sturdy,
                                                    Slot1 = AbilityId.WeakArmor,
                                                    Hidden = AbilityId.SandForce,
                                                },
                                                HeightM = 0.9,
                                                WeightKg = 102,
                                                Color = "Blue",
                                                Prevo = SpecieId.Roggenrola,
                                            },
                                            [SpecieId.Gigalith] = new()
                                            {
                                                Id = SpecieId.Gigalith,
                                                Num = 526,
                                                Name = "Gigalith",
                                                Types = [PokemonType.Rock],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 85,
                                                    Atk = 135,
                                                    Def = 130,
                                                    SpA = 60,
                                                    SpD = 80,
                                                    Spe = 25,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Sturdy,
                                                    Slot1 = AbilityId.SandStream,
                                                    Hidden = AbilityId.SandForce,
                                                },
                                                HeightM = 1.7,
                                                WeightKg = 260,
                                                Color = "Blue",
                                                Prevo = SpecieId.Boldore,
                                            },
                                            [SpecieId.Woobat] = new()
                                            {
                                                Id = SpecieId.Woobat,
                                                Num = 527,
                                                Name = "Woobat",
                                                Types = [PokemonType.Psychic, PokemonType.Flying],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 65,
                                                    Atk = 45,
                                                    Def = 43,
                                                    SpA = 55,
                                                    SpD = 43,
                                                    Spe = 72,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Unaware,
                                                    Slot1 = AbilityId.Klutz,
                                                    Hidden = AbilityId.Simple,
                                                },
                                                HeightM = 0.4,
                                                WeightKg = 2.1,
                                                Color = "Blue",
                                            },
                                            [SpecieId.Swoobat] = new()
                                            {
                                                Id = SpecieId.Swoobat,
                                                Num = 528,
                                                Name = "Swoobat",
                                                Types = [PokemonType.Psychic, PokemonType.Flying],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 67,
                                                    Atk = 57,
                                                    Def = 55,
                                                    SpA = 77,
                                                    SpD = 55,
                                                    Spe = 114,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.Unaware,
                                                    Slot1 = AbilityId.Klutz,
                                                    Hidden = AbilityId.Simple,
                                                },
                                                HeightM = 0.9,
                                                WeightKg = 10.5,
                                                Color = "Blue",
                                                Prevo = SpecieId.Woobat,
                                            },
                                            [SpecieId.Drilbur] = new()
                                            {
                                                Id = SpecieId.Drilbur,
                                                Num = 529,
                                                Name = "Drilbur",
                                                Types = [PokemonType.Ground],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 60,
                                                    Atk = 85,
                                                    Def = 40,
                                                    SpA = 30,
                                                    SpD = 45,
                                                    Spe = 68,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.SandRush,
                                                    Slot1 = AbilityId.SandForce,
                                                    Hidden = AbilityId.MoldBreaker,
                                                },
                                                HeightM = 0.3,
                                                WeightKg = 8.5,
                                                Color = "Gray",
                                            },
                                            [SpecieId.Excadrill] = new()
                                            {
                                                Id = SpecieId.Excadrill,
                                                Num = 530,
                                                Name = "Excadrill",
                                                Types = [PokemonType.Ground, PokemonType.Steel],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 110,
                                                    Atk = 135,
                                                    Def = 60,
                                                    SpA = 50,
                                                    SpD = 65,
                                                    Spe = 88,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.SandRush,
                                                    Slot1 = AbilityId.SandForce,
                                                    Hidden = AbilityId.MoldBreaker,
                                                },
                                                HeightM = 0.7,
                                                WeightKg = 40.4,
                                                Color = "Gray",
                                                Prevo = SpecieId.Drilbur,
                                            },
                                            [SpecieId.ExcadrillMega] = new()
                                            {
                                                Id = SpecieId.ExcadrillMega,
                                                Num = 530,
                                                Name = "Excadrill-Mega",
                                                Types = [PokemonType.Ground, PokemonType.Steel],
                                                Gender = GenderId.Empty,
                                                BaseStats = new StatsTable
                                                {
                                                    Hp = 110,
                                                    Atk = 165,
                                                    Def = 100,
                                                    SpA = 65,
                                                    SpD = 65,
                                                    Spe = 103,
                                                },
                                                Abilities = new SpeciesAbility
                                                {
                                                    Slot0 = AbilityId.SandRush,
                                                    Slot1 = AbilityId.SandForce,
                                                    Hidden = AbilityId.MoldBreaker,
                                                },
                                                                HeightM = 0.9,
                                                                WeightKg = 60,
                                                                Color = "Gray",
                                                                BaseSpecies = SpecieId.Excadrill,
                                                                Forme = FormeId.Mega,
                                                            },
                                                            [SpecieId.Audino] = new()
                                                            {
                                                                Id = SpecieId.Audino,
                                                                Num = 531,
                                                                Name = "Audino",
                                                                Types = [PokemonType.Normal],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 103,
                                                                    Atk = 60,
                                                                    Def = 86,
                                                                    SpA = 60,
                                                                    SpD = 86,
                                                                    Spe = 50,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.Healer,
                                                                    Slot1 = AbilityId.Regenerator,
                                                                    Hidden = AbilityId.Klutz,
                                                                },
                                                                HeightM = 1.1,
                                                                WeightKg = 31,
                                                                Color = "Pink",
                                                            },
                                                            [SpecieId.AudinoMega] = new()
                                                            {
                                                                Id = SpecieId.AudinoMega,
                                                                Num = 531,
                                                                Name = "Audino-Mega",
                                                                Types = [PokemonType.Normal, PokemonType.Fairy],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 103,
                                                                    Atk = 60,
                                                                    Def = 126,
                                                                    SpA = 80,
                                                                    SpD = 126,
                                                                    Spe = 50,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.Healer,
                                                                },
                                                                HeightM = 1.5,
                                                                WeightKg = 32,
                                                                Color = "White",
                                                                BaseSpecies = SpecieId.Audino,
                                                                Forme = FormeId.Mega,
                                                            },
                                                            [SpecieId.Timburr] = new()
                                                            {
                                                                Id = SpecieId.Timburr,
                                                                Num = 532,
                                                                Name = "Timburr",
                                                                Types = [PokemonType.Fighting],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 75,
                                                                    Atk = 80,
                                                                    Def = 55,
                                                                    SpA = 25,
                                                                    SpD = 35,
                                                                    Spe = 35,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.Guts,
                                                                    Slot1 = AbilityId.SheerForce,
                                                                    Hidden = AbilityId.IronFist,
                                                                },
                                                                HeightM = 0.6,
                                                                WeightKg = 12.5,
                                                                Color = "Gray",
                                                            },
                                                            [SpecieId.Gurdurr] = new()
                                                            {
                                                                Id = SpecieId.Gurdurr,
                                                                Num = 533,
                                                                Name = "Gurdurr",
                                                                Types = [PokemonType.Fighting],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 85,
                                                                    Atk = 105,
                                                                    Def = 85,
                                                                    SpA = 40,
                                                                    SpD = 50,
                                                                    Spe = 40,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.Guts,
                                                                    Slot1 = AbilityId.SheerForce,
                                                                    Hidden = AbilityId.IronFist,
                                                                },
                                                                HeightM = 1.2,
                                                                WeightKg = 40,
                                                                Color = "Gray",
                                                                Prevo = SpecieId.Timburr,
                                                            },
                                                            [SpecieId.Conkeldurr] = new()
                                                            {
                                                                Id = SpecieId.Conkeldurr,
                                                                Num = 534,
                                                                Name = "Conkeldurr",
                                                                Types = [PokemonType.Fighting],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 105,
                                                                    Atk = 140,
                                                                    Def = 95,
                                                                    SpA = 55,
                                                                    SpD = 65,
                                                                    Spe = 45,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.Guts,
                                                                    Slot1 = AbilityId.SheerForce,
                                                                    Hidden = AbilityId.IronFist,
                                                                },
                                                                HeightM = 1.4,
                                                                WeightKg = 87,
                                                                Color = "Brown",
                                                                Prevo = SpecieId.Gurdurr,
                                                            },
                                                            [SpecieId.Tympole] = new()
                                                            {
                                                                Id = SpecieId.Tympole,
                                                                Num = 535,
                                                                Name = "Tympole",
                                                                Types = [PokemonType.Water],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 50,
                                                                    Atk = 50,
                                                                    Def = 40,
                                                                    SpA = 50,
                                                                    SpD = 40,
                                                                    Spe = 64,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.SwiftSwim,
                                                                    Slot1 = AbilityId.Hydration,
                                                                    Hidden = AbilityId.WaterAbsorb,
                                                                },
                                                                HeightM = 0.5,
                                                                WeightKg = 4.5,
                                                                Color = "Blue",
                                                            },
                                                            [SpecieId.Palpitoad] = new()
                                                            {
                                                                Id = SpecieId.Palpitoad,
                                                                Num = 536,
                                                                Name = "Palpitoad",
                                                                Types = [PokemonType.Water, PokemonType.Ground],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 75,
                                                                    Atk = 65,
                                                                    Def = 55,
                                                                    SpA = 65,
                                                                    SpD = 55,
                                                                    Spe = 69,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.SwiftSwim,
                                                                    Slot1 = AbilityId.Hydration,
                                                                    Hidden = AbilityId.WaterAbsorb,
                                                                },
                                                                HeightM = 0.8,
                                                                WeightKg = 17,
                                                                Color = "Blue",
                                                                Prevo = SpecieId.Tympole,
                                                            },
                                                            [SpecieId.Seismitoad] = new()
                                                            {
                                                                Id = SpecieId.Seismitoad,
                                                                Num = 537,
                                                                Name = "Seismitoad",
                                                                Types = [PokemonType.Water, PokemonType.Ground],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 105,
                                                                    Atk = 95,
                                                                    Def = 75,
                                                                    SpA = 85,
                                                                    SpD = 75,
                                                                    Spe = 74,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.SwiftSwim,
                                                                    Slot1 = AbilityId.PoisonTouch,
                                                                    Hidden = AbilityId.WaterAbsorb,
                                                                },
                                                                HeightM = 1.5,
                                                                WeightKg = 62,
                                                                Color = "Blue",
                                                                Prevo = SpecieId.Palpitoad,
                                                            },
                                                            [SpecieId.Throh] = new()
                                                            {
                                                                Id = SpecieId.Throh,
                                                                Num = 538,
                                                                Name = "Throh",
                                                                Types = [PokemonType.Fighting],
                                                                Gender = GenderId.M,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 120,
                                                                    Atk = 100,
                                                                    Def = 85,
                                                                    SpA = 30,
                                                                    SpD = 85,
                                                                    Spe = 45,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.Guts,
                                                                    Slot1 = AbilityId.InnerFocus,
                                                                    Hidden = AbilityId.MoldBreaker,
                                                                },
                                                                HeightM = 1.3,
                                                                WeightKg = 55.5,
                                                                Color = "Red",
                                                            },
                                                            [SpecieId.Sawk] = new()
                                                            {
                                                                Id = SpecieId.Sawk,
                                                                Num = 539,
                                                                Name = "Sawk",
                                                                Types = [PokemonType.Fighting],
                                                                Gender = GenderId.M,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 75,
                                                                    Atk = 125,
                                                                    Def = 75,
                                                                    SpA = 30,
                                                                    SpD = 75,
                                                                    Spe = 85,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.Sturdy,
                                                                    Slot1 = AbilityId.InnerFocus,
                                                                    Hidden = AbilityId.MoldBreaker,
                                                                },
                                                                HeightM = 1.4,
                                                                WeightKg = 51,
                                                                Color = "Blue",
                                                            },
                                                            [SpecieId.Sewaddle] = new()
                                                            {
                                                                Id = SpecieId.Sewaddle,
                                                                Num = 540,
                                                                Name = "Sewaddle",
                                                                Types = [PokemonType.Bug, PokemonType.Grass],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 45,
                                                                    Atk = 53,
                                                                    Def = 70,
                                                                    SpA = 40,
                                                                    SpD = 60,
                                                                    Spe = 42,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.Swarm,
                                                                    Slot1 = AbilityId.Chlorophyll,
                                                                    Hidden = AbilityId.Overcoat,
                                                                },
                                                                HeightM = 0.3,
                                                                WeightKg = 2.5,
                                                                Color = "Yellow",
                                                            },
                                                            [SpecieId.Swadloon] = new()
                                                            {
                                                                Id = SpecieId.Swadloon,
                                                                Num = 541,
                                                                Name = "Swadloon",
                                                                Types = [PokemonType.Bug, PokemonType.Grass],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 55,
                                                                    Atk = 63,
                                                                    Def = 90,
                                                                    SpA = 50,
                                                                    SpD = 80,
                                                                    Spe = 42,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.LeafGuard,
                                                                    Slot1 = AbilityId.Chlorophyll,
                                                                    Hidden = AbilityId.Overcoat,
                                                                },
                                                                HeightM = 0.5,
                                                                WeightKg = 7.3,
                                                                Color = "Green",
                                                                Prevo = SpecieId.Sewaddle,
                                                            },
                                                            [SpecieId.Leavanny] = new()
                                                            {
                                                                Id = SpecieId.Leavanny,
                                                                Num = 542,
                                                                Name = "Leavanny",
                                                                Types = [PokemonType.Bug, PokemonType.Grass],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 75,
                                                                    Atk = 103,
                                                                    Def = 80,
                                                                    SpA = 70,
                                                                    SpD = 80,
                                                                    Spe = 92,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.Swarm,
                                                                    Slot1 = AbilityId.Chlorophyll,
                                                                    Hidden = AbilityId.Overcoat,
                                                                },
                                                                HeightM = 1.2,
                                                                WeightKg = 20.5,
                                                                Color = "Yellow",
                                                                Prevo = SpecieId.Swadloon,
                                                            },
                                                            [SpecieId.Venipede] = new()
                                                            {
                                                                Id = SpecieId.Venipede,
                                                                Num = 543,
                                                                Name = "Venipede",
                                                                Types = [PokemonType.Bug, PokemonType.Poison],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 30,
                                                                    Atk = 45,
                                                                    Def = 59,
                                                                    SpA = 30,
                                                                    SpD = 39,
                                                                    Spe = 57,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.PoisonPoint,
                                                                    Slot1 = AbilityId.Swarm,
                                                                    Hidden = AbilityId.SpeedBoost,
                                                                },
                                                                HeightM = 0.4,
                                                                WeightKg = 5.3,
                                                                Color = "Red",
                                                            },
                                                            [SpecieId.Whirlipede] = new()
                                                            {
                                                                Id = SpecieId.Whirlipede,
                                                                Num = 544,
                                                                Name = "Whirlipede",
                                                                Types = [PokemonType.Bug, PokemonType.Poison],
                                                                Gender = GenderId.Empty,
                                                                BaseStats = new StatsTable
                                                                {
                                                                    Hp = 40,
                                                                    Atk = 55,
                                                                    Def = 99,
                                                                    SpA = 40,
                                                                    SpD = 79,
                                                                    Spe = 47,
                                                                },
                                                                Abilities = new SpeciesAbility
                                                                {
                                                                    Slot0 = AbilityId.PoisonPoint,
                                                                    Slot1 = AbilityId.Swarm,
                                                                    Hidden = AbilityId.SpeedBoost,
                                                                },
                                                                HeightM = 1.2,
                                                                WeightKg = 58.5,
                                                                Color = "Gray",
                                                                Prevo = SpecieId.Venipede,
                                                            },
                                                        };
                                                    }
                                                }
