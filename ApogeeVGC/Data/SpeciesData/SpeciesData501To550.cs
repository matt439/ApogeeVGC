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
                        };
                    }
                }
