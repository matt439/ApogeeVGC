using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData901To950()
    {
        return new Dictionary<SpecieId, Species>
        {
            // 901 - Ursaluna
            [SpecieId.Ursaluna] = new()
            {
                Id = SpecieId.Ursaluna,
                Num = 901,
                Name = "Ursaluna",
                Types = [PokemonType.Ground, PokemonType.Normal],
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
            [SpecieId.UrsalunaBloodmoon] = new()
            {
                Id = SpecieId.UrsalunaBloodmoon,
                Num = 901,
                Name = "Ursaluna-Bloodmoon",
                BaseSpecies = SpecieId.Ursaluna,
                Forme = FormeId.Bloodmoon,
                Types = [PokemonType.Ground, PokemonType.Normal],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 113,
                    Atk = 70,
                    Def = 120,
                    SpA = 135,
                    SpD = 65,
                    Spe = 52,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MindsEye,
                },
                HeightM = 2.7,
                WeightKg = 333,
                Color = "Brown",
            },
            // 902 - Basculegion
            [SpecieId.Basculegion] = new()
            {
                Id = SpecieId.Basculegion,
                Num = 902,
                Name = "Basculegion",
                Types = [PokemonType.Water, PokemonType.Ghost],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 120,
                    Atk = 112,
                    Def = 65,
                    SpA = 80,
                    SpD = 75,
                    Spe = 78,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.Adaptability,
                    Hidden = AbilityId.MoldBreaker,
                },
                HeightM = 3,
                WeightKg = 110,
                Color = "Green",
            },
            [SpecieId.BasculegionF] = new()
            {
                Id = SpecieId.BasculegionF,
                Num = 902,
                Name = "Basculegion-F",
                BaseSpecies = SpecieId.Basculegion,
                Forme = FormeId.F,
                Types = [PokemonType.Water, PokemonType.Ghost],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 120,
                    Atk = 92,
                    Def = 65,
                    SpA = 100,
                    SpD = 75,
                    Spe = 78,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.Adaptability,
                    Hidden = AbilityId.MoldBreaker,
                },
                HeightM = 3,
                WeightKg = 110,
                Color = "Green",
            },
            // 903 - Sneasler
            [SpecieId.Sneasler] = new()
            {
                Id = SpecieId.Sneasler,
                Num = 903,
                Name = "Sneasler",
                Types = [PokemonType.Fighting, PokemonType.Poison],
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 130,
                    Def = 60,
                    SpA = 40,
                    SpD = 80,
                    Spe = 120,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Slot1 = AbilityId.Unburden,
                    Hidden = AbilityId.PoisonTouch,
                },
                HeightM = 1.3,
                WeightKg = 43,
                Color = "Blue",
            },
            // 904 - Overqwil
            [SpecieId.Overqwil] = new()
            {
                Id = SpecieId.Overqwil,
                Num = 904,
                Name = "Overqwil",
                Types = [PokemonType.Dark, PokemonType.Poison],
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 115,
                    Def = 95,
                    SpA = 65,
                    SpD = 65,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.SwiftSwim,
                    Hidden = AbilityId.Intimidate,
                },
                HeightM = 2.5,
                WeightKg = 60.5,
                Color = "Black",
            },
            // 905 - Enamorus
            [SpecieId.Enamorus] = new()
            {
                Id = SpecieId.Enamorus,
                Num = 905,
                Name = "Enamorus",
                Types = [PokemonType.Fairy, PokemonType.Flying],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 74,
                    Atk = 115,
                    Def = 70,
                    SpA = 135,
                    SpD = 80,
                    Spe = 106,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Hidden = AbilityId.Contrary,
                },
                HeightM = 1.6,
                WeightKg = 48,
                Color = "Pink",
            },
            [SpecieId.EnamorusTherian] = new()
            {
                Id = SpecieId.EnamorusTherian,
                Num = 905,
                Name = "Enamorus-Therian",
                BaseSpecies = SpecieId.Enamorus,
                Forme = FormeId.Therian,
                Types = [PokemonType.Fairy, PokemonType.Flying],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 74,
                    Atk = 115,
                    Def = 110,
                    SpA = 135,
                    SpD = 100,
                    Spe = 46,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overcoat,
                },
                HeightM = 1.6,
                WeightKg = 48,
                Color = "Pink",
            },
            // 906 - Sprigatito
            [SpecieId.Sprigatito] = new()
            {
                Id = SpecieId.Sprigatito,
                Num = 906,
                Name = "Sprigatito",
                Types = [PokemonType.Grass],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 61,
                    Def = 54,
                    SpA = 45,
                    SpD = 45,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Protean,
                },
                HeightM = 0.4,
                WeightKg = 4.1,
                Color = "Green",
            },
            // 907 - Floragato
            [SpecieId.Floragato] = new()
            {
                Id = SpecieId.Floragato,
                Num = 907,
                Name = "Floragato",
                Types = [PokemonType.Grass],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 61,
                    Atk = 80,
                    Def = 63,
                    SpA = 60,
                    SpD = 63,
                    Spe = 83,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Protean,
                },
                HeightM = 0.9,
                WeightKg = 12.2,
                Color = "Green",
            },
            // 908 - Meowscarada
            [SpecieId.Meowscarada] = new()
            {
                Id = SpecieId.Meowscarada,
                Num = 908,
                Name = "Meowscarada",
                Types = [PokemonType.Grass, PokemonType.Dark],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 76,
                    Atk = 110,
                    Def = 70,
                    SpA = 81,
                    SpD = 70,
                    Spe = 123,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Protean,
                },
                HeightM = 1.5,
                WeightKg = 31.2,
                Color = "Green",
            },
            // 909 - Fuecoco
            [SpecieId.Fuecoco] = new()
            {
                Id = SpecieId.Fuecoco,
                Num = 909,
                Name = "Fuecoco",
                Types = [PokemonType.Fire],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 67,
                    Atk = 45,
                    Def = 59,
                    SpA = 63,
                    SpD = 40,
                    Spe = 36,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Unaware,
                },
                HeightM = 0.4,
                WeightKg = 9.8,
                Color = "Red",
            },
            // 910 - Crocalor
            [SpecieId.Crocalor] = new()
            {
                Id = SpecieId.Crocalor,
                Num = 910,
                Name = "Crocalor",
                Types = [PokemonType.Fire],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 81,
                    Atk = 55,
                    Def = 78,
                    SpA = 90,
                    SpD = 58,
                    Spe = 49,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Unaware,
                },
                HeightM = 1,
                WeightKg = 30.7,
                Color = "Red",
            },
            // 911 - Skeledirge
            [SpecieId.Skeledirge] = new()
            {
                Id = SpecieId.Skeledirge,
                Num = 911,
                Name = "Skeledirge",
                Types = [PokemonType.Fire, PokemonType.Ghost],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 104,
                    Atk = 75,
                    Def = 100,
                    SpA = 110,
                    SpD = 75,
                    Spe = 66,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Unaware,
                },
                HeightM = 1.6,
                WeightKg = 326.5,
                Color = "Red",
            },
            // 912 - Quaxly
            [SpecieId.Quaxly] = new()
            {
                Id = SpecieId.Quaxly,
                Num = 912,
                Name = "Quaxly",
                Types = [PokemonType.Water],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 65,
                    Def = 45,
                    SpA = 50,
                    SpD = 45,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Moxie,
                },
                HeightM = 0.5,
                WeightKg = 6.1,
                Color = "Blue",
            },
            // 913 - Quaxwell
            [SpecieId.Quaxwell] = new()
            {
                Id = SpecieId.Quaxwell,
                Num = 913,
                Name = "Quaxwell",
                Types = [PokemonType.Water],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 85,
                    Def = 65,
                    SpA = 65,
                    SpD = 60,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Moxie,
                },
                HeightM = 1.2,
                WeightKg = 21.5,
                Color = "Blue",
            },
            // 914 - Quaquaval
            [SpecieId.Quaquaval] = new()
            {
                Id = SpecieId.Quaquaval,
                Num = 914,
                Name = "Quaquaval",
                Types = [PokemonType.Water, PokemonType.Fighting],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 120,
                    Def = 80,
                    SpA = 85,
                    SpD = 75,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Moxie,
                },
                HeightM = 1.8,
                WeightKg = 61.9,
                Color = "Blue",
            },
            // 915 - Lechonk
            [SpecieId.Lechonk] = new()
            {
                Id = SpecieId.Lechonk,
                Num = 915,
                Name = "Lechonk",
                Types = [PokemonType.Normal],
                BaseStats = new StatsTable
                {
                    Hp = 54,
                    Atk = 45,
                    Def = 40,
                    SpA = 35,
                    SpD = 45,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.AromaVeil,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 0.5,
                WeightKg = 10.2,
                Color = "Gray",
            },
            // 916 - Oinkologne
            [SpecieId.Oinkologne] = new()
            {
                Id = SpecieId.Oinkologne,
                Num = 916,
                Name = "Oinkologne",
                Types = [PokemonType.Normal],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 110,
                    Atk = 100,
                    Def = 75,
                    SpA = 59,
                    SpD = 80,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LingeringAroma,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 1,
                WeightKg = 120,
                Color = "Gray",
            },
            [SpecieId.OinkologneF] = new()
            {
                Id = SpecieId.OinkologneF,
                Num = 916,
                Name = "Oinkologne-F",
                BaseSpecies = SpecieId.Oinkologne,
                Forme = FormeId.F,
                Types = [PokemonType.Normal],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 115,
                    Atk = 90,
                    Def = 70,
                    SpA = 59,
                    SpD = 90,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.AromaVeil,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.ThickFat,
                },
                HeightM = 1,
                WeightKg = 120,
                Color = "Brown",
            },
            // 917 - Tarountula
            [SpecieId.Tarountula] = new()
            {
                Id = SpecieId.Tarountula,
                Num = 917,
                Name = "Tarountula",
                Types = [PokemonType.Bug],
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 41,
                    Def = 45,
                    SpA = 29,
                    SpD = 40,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Hidden = AbilityId.Stakeout,
                },
                HeightM = 0.3,
                WeightKg = 4,
                Color = "White",
            },
            // 918 - Spidops
            [SpecieId.Spidops] = new()
            {
                Id = SpecieId.Spidops,
                Num = 918,
                Name = "Spidops",
                Types = [PokemonType.Bug],
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 79,
                    Def = 92,
                    SpA = 52,
                    SpD = 86,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Hidden = AbilityId.Stakeout,
                },
                HeightM = 1,
                WeightKg = 16.5,
                Color = "Green",
            },
            // 919 - Nymble
            [SpecieId.Nymble] = new()
            {
                Id = SpecieId.Nymble,
                Num = 919,
                Name = "Nymble",
                Types = [PokemonType.Bug],
                BaseStats = new StatsTable
                {
                    Hp = 33,
                    Atk = 46,
                    Def = 40,
                    SpA = 21,
                    SpD = 25,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Hidden = AbilityId.TintedLens,
                },
                HeightM = 0.2,
                WeightKg = 1,
                Color = "Gray",
            },
            // 920 - Lokix
            [SpecieId.Lokix] = new()
            {
                Id = SpecieId.Lokix,
                Num = 920,
                Name = "Lokix",
                Types = [PokemonType.Bug, PokemonType.Dark],
                BaseStats = new StatsTable
                {
                    Hp = 71,
                    Atk = 102,
                    Def = 78,
                    SpA = 52,
                    SpD = 55,
                    Spe = 92,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Hidden = AbilityId.TintedLens,
                },
                HeightM = 1,
                WeightKg = 17.5,
                Color = "Gray",
            },
            // 921 - Pawmi
            [SpecieId.Pawmi] = new()
            {
                Id = SpecieId.Pawmi,
                Num = 921,
                Name = "Pawmi",
                Types = [PokemonType.Electric],
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 50,
                    Def = 20,
                    SpA = 40,
                    SpD = 25,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Slot1 = AbilityId.NaturalCure,
                    Hidden = AbilityId.IronFist,
                },
                HeightM = 0.3,
                WeightKg = 2.5,
                Color = "Yellow",
            },
            // 922 - Pawmo
            [SpecieId.Pawmo] = new()
            {
                Id = SpecieId.Pawmo,
                Num = 922,
                Name = "Pawmo",
                Types = [PokemonType.Electric, PokemonType.Fighting],
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 75,
                    Def = 40,
                    SpA = 50,
                    SpD = 40,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VoltAbsorb,
                    Slot1 = AbilityId.NaturalCure,
                    Hidden = AbilityId.IronFist,
                },
                HeightM = 0.4,
                WeightKg = 6.5,
                Color = "Yellow",
            },
            // 923 - Pawmot
            [SpecieId.Pawmot] = new()
            {
                Id = SpecieId.Pawmot,
                Num = 923,
                Name = "Pawmot",
                Types = [PokemonType.Electric, PokemonType.Fighting],
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 115,
                    Def = 70,
                    SpA = 70,
                    SpD = 60,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VoltAbsorb,
                    Slot1 = AbilityId.NaturalCure,
                    Hidden = AbilityId.IronFist,
                },
                HeightM = 0.9,
                WeightKg = 41,
                Color = "Yellow",
            },
            // 924 - Tandemaus
            [SpecieId.Tandemaus] = new()
            {
                Id = SpecieId.Tandemaus,
                Num = 924,
                Name = "Tandemaus",
                Types = [PokemonType.Normal],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 50,
                    Def = 45,
                    SpA = 40,
                    SpD = 45,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.Pickup,
                    Hidden = AbilityId.OwnTempo,
                },
                HeightM = 0.3,
                WeightKg = 1.8,
                Color = "White",
            },
            // 925 - Maushold
            [SpecieId.Maushold] = new()
            {
                Id = SpecieId.Maushold,
                Num = 925,
                Name = "Maushold",
                Types = [PokemonType.Normal],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 74,
                    Atk = 75,
                    Def = 70,
                    SpA = 65,
                    SpD = 75,
                    Spe = 111,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FriendGuard,
                    Slot1 = AbilityId.CheekPouch,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 0.3,
                WeightKg = 2.3,
                Color = "White",
            },
                        [SpecieId.MausholdFour] = new()
                        {
                            Id = SpecieId.MausholdFour,
                            Num = 925,
                            Name = "Maushold-Four",
                            BaseSpecies = SpecieId.Maushold,
                            Forme = FormeId.Four,
                            Types = [PokemonType.Normal],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 74,
                                Atk = 75,
                                Def = 70,
                                SpA = 65,
                                SpD = 75,
                                Spe = 111,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.FriendGuard,
                                Slot1 = AbilityId.CheekPouch,
                                Hidden = AbilityId.Technician,
                            },
                            HeightM = 0.3,
                            WeightKg = 2.8,
                            Color = "White",
                        },
                        // 926 - Fidough
                        [SpecieId.Fidough] = new()
                        {
                            Id = SpecieId.Fidough,
                            Num = 926,
                            Name = "Fidough",
                            Types = [PokemonType.Fairy],
                            BaseStats = new StatsTable
                            {
                                Hp = 37,
                                Atk = 55,
                                Def = 70,
                                SpA = 30,
                                SpD = 55,
                                Spe = 65,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.OwnTempo,
                                Hidden = AbilityId.Klutz,
                            },
                            HeightM = 0.3,
                            WeightKg = 10.9,
                            Color = "Yellow",
                        },
                        // 927 - Dachsbun
                        [SpecieId.Dachsbun] = new()
                        {
                            Id = SpecieId.Dachsbun,
                            Num = 927,
                            Name = "Dachsbun",
                            Types = [PokemonType.Fairy],
                            BaseStats = new StatsTable
                            {
                                Hp = 57,
                                Atk = 80,
                                Def = 115,
                                SpA = 50,
                                SpD = 80,
                                Spe = 95,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.WellBakedBody,
                                Hidden = AbilityId.AromaVeil,
                            },
                            HeightM = 0.5,
                            WeightKg = 14.9,
                            Color = "Brown",
                        },
                        // 928 - Smoliv
                        [SpecieId.Smoliv] = new()
                        {
                            Id = SpecieId.Smoliv,
                            Num = 928,
                            Name = "Smoliv",
                            Types = [PokemonType.Grass, PokemonType.Normal],
                            BaseStats = new StatsTable
                            {
                                Hp = 41,
                                Atk = 35,
                                Def = 45,
                                SpA = 58,
                                SpD = 51,
                                Spe = 30,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.EarlyBird,
                                Hidden = AbilityId.Harvest,
                            },
                            HeightM = 0.3,
                            WeightKg = 6.5,
                            Color = "Green",
                        },
                        // 929 - Dolliv
                        [SpecieId.Dolliv] = new()
                        {
                            Id = SpecieId.Dolliv,
                            Num = 929,
                            Name = "Dolliv",
                            Types = [PokemonType.Grass, PokemonType.Normal],
                            BaseStats = new StatsTable
                            {
                                Hp = 52,
                                Atk = 53,
                                Def = 60,
                                SpA = 78,
                                SpD = 78,
                                Spe = 33,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.EarlyBird,
                                Hidden = AbilityId.Harvest,
                            },
                            HeightM = 0.6,
                            WeightKg = 11.9,
                            Color = "Green",
                        },
                        // 930 - Arboliva
                        [SpecieId.Arboliva] = new()
                        {
                            Id = SpecieId.Arboliva,
                            Num = 930,
                            Name = "Arboliva",
                            Types = [PokemonType.Grass, PokemonType.Normal],
                            BaseStats = new StatsTable
                            {
                                Hp = 78,
                                Atk = 69,
                                Def = 90,
                                SpA = 125,
                                SpD = 109,
                                Spe = 39,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.SeedSower,
                                Hidden = AbilityId.Harvest,
                            },
                            HeightM = 1.4,
                            WeightKg = 48.2,
                            Color = "Green",
                        },
                        // 931 - Squawkabilly
                        [SpecieId.Squawkabilly] = new()
                        {
                            Id = SpecieId.Squawkabilly,
                            Num = 931,
                            Name = "Squawkabilly",
                            Types = [PokemonType.Normal, PokemonType.Flying],
                            BaseStats = new StatsTable
                            {
                                Hp = 82,
                                Atk = 96,
                                Def = 51,
                                SpA = 45,
                                SpD = 51,
                                Spe = 92,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Intimidate,
                                Slot1 = AbilityId.Hustle,
                                Hidden = AbilityId.Guts,
                            },
                            HeightM = 0.6,
                            WeightKg = 2.4,
                            Color = "Green",
                        },
                        [SpecieId.SquawkabillyBlue] = new()
                        {
                            Id = SpecieId.SquawkabillyBlue,
                            Num = 931,
                            Name = "Squawkabilly-Blue",
                            BaseSpecies = SpecieId.Squawkabilly,
                            Forme = FormeId.Blue2,
                            Types = [PokemonType.Normal, PokemonType.Flying],
                            BaseStats = new StatsTable
                            {
                                Hp = 82,
                                Atk = 96,
                                Def = 51,
                                SpA = 45,
                                SpD = 51,
                                Spe = 92,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Intimidate,
                                Slot1 = AbilityId.Hustle,
                                Hidden = AbilityId.Guts,
                            },
                            HeightM = 0.6,
                            WeightKg = 2.4,
                            Color = "Blue",
                        },
                        [SpecieId.SquawkabillyYellow] = new()
                        {
                            Id = SpecieId.SquawkabillyYellow,
                            Num = 931,
                            Name = "Squawkabilly-Yellow",
                            BaseSpecies = SpecieId.Squawkabilly,
                            Forme = FormeId.Yellow2,
                            Types = [PokemonType.Normal, PokemonType.Flying],
                            BaseStats = new StatsTable
                            {
                                Hp = 82,
                                Atk = 96,
                                Def = 51,
                                SpA = 45,
                                SpD = 51,
                                Spe = 92,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Intimidate,
                                Slot1 = AbilityId.Hustle,
                                Hidden = AbilityId.SheerForce,
                            },
                            HeightM = 0.6,
                            WeightKg = 2.4,
                            Color = "Yellow",
                        },
                        [SpecieId.SquawkabillyWhite] = new()
                        {
                            Id = SpecieId.SquawkabillyWhite,
                            Num = 931,
                            Name = "Squawkabilly-White",
                            BaseSpecies = SpecieId.Squawkabilly,
                            Forme = FormeId.White2,
                            Types = [PokemonType.Normal, PokemonType.Flying],
                            BaseStats = new StatsTable
                            {
                                Hp = 82,
                                Atk = 96,
                                Def = 51,
                                SpA = 45,
                                SpD = 51,
                                Spe = 92,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Intimidate,
                                Slot1 = AbilityId.Hustle,
                                Hidden = AbilityId.SheerForce,
                            },
                            HeightM = 0.6,
                            WeightKg = 2.4,
                            Color = "White",
                        },
                        // 932 - Nacli
                        [SpecieId.Nacli] = new()
                        {
                            Id = SpecieId.Nacli,
                            Num = 932,
                            Name = "Nacli",
                            Types = [PokemonType.Rock],
                            BaseStats = new StatsTable
                            {
                                Hp = 55,
                                Atk = 55,
                                Def = 75,
                                SpA = 35,
                                SpD = 35,
                                Spe = 25,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.PurifyingSalt,
                                Slot1 = AbilityId.Sturdy,
                                Hidden = AbilityId.ClearBody,
                            },
                            HeightM = 0.4,
                            WeightKg = 16,
                            Color = "Brown",
                        },
                        // 933 - Naclstack
                        [SpecieId.Naclstack] = new()
                        {
                            Id = SpecieId.Naclstack,
                            Num = 933,
                            Name = "Naclstack",
                            Types = [PokemonType.Rock],
                            BaseStats = new StatsTable
                            {
                                Hp = 60,
                                Atk = 60,
                                Def = 100,
                                SpA = 35,
                                SpD = 65,
                                Spe = 35,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.PurifyingSalt,
                                Slot1 = AbilityId.Sturdy,
                                Hidden = AbilityId.ClearBody,
                            },
                            HeightM = 0.6,
                            WeightKg = 105,
                            Color = "Brown",
                        },
                        // 934 - Garganacl
                        [SpecieId.Garganacl] = new()
                        {
                            Id = SpecieId.Garganacl,
                            Num = 934,
                            Name = "Garganacl",
                            Types = [PokemonType.Rock],
                            BaseStats = new StatsTable
                            {
                                Hp = 100,
                                Atk = 100,
                                Def = 130,
                                SpA = 45,
                                SpD = 90,
                                Spe = 35,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.PurifyingSalt,
                                Slot1 = AbilityId.Sturdy,
                                Hidden = AbilityId.ClearBody,
                            },
                            HeightM = 2.3,
                            WeightKg = 240,
                            Color = "Brown",
                        },
                        // 935 - Charcadet
                        [SpecieId.Charcadet] = new()
                        {
                            Id = SpecieId.Charcadet,
                            Num = 935,
                            Name = "Charcadet",
                            Types = [PokemonType.Fire],
                            BaseStats = new StatsTable
                            {
                                Hp = 40,
                                Atk = 50,
                                Def = 40,
                                SpA = 50,
                                SpD = 40,
                                Spe = 35,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.FlashFire,
                                Hidden = AbilityId.FlameBody,
                            },
                            HeightM = 0.6,
                            WeightKg = 10.5,
                            Color = "Red",
                        },
                        // 936 - Armarouge
                        [SpecieId.Armarouge] = new()
                        {
                            Id = SpecieId.Armarouge,
                            Num = 936,
                            Name = "Armarouge",
                            Types = [PokemonType.Fire, PokemonType.Psychic],
                            BaseStats = new StatsTable
                            {
                                Hp = 85,
                                Atk = 60,
                                Def = 100,
                                SpA = 125,
                                SpD = 80,
                                Spe = 75,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.FlashFire,
                                Hidden = AbilityId.WeakArmor,
                            },
                            HeightM = 1.5,
                            WeightKg = 85,
                            Color = "Red",
                        },
                        // 937 - Ceruledge
                        [SpecieId.Ceruledge] = new()
                        {
                            Id = SpecieId.Ceruledge,
                            Num = 937,
                            Name = "Ceruledge",
                            Types = [PokemonType.Fire, PokemonType.Ghost],
                            BaseStats = new StatsTable
                            {
                                Hp = 75,
                                Atk = 125,
                                Def = 80,
                                SpA = 60,
                                SpD = 100,
                                Spe = 85,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.FlashFire,
                                Hidden = AbilityId.WeakArmor,
                            },
                            HeightM = 1.6,
                            WeightKg = 62,
                            Color = "Purple",
                        },
                        // 938 - Tadbulb
                        [SpecieId.Tadbulb] = new()
                        {
                            Id = SpecieId.Tadbulb,
                            Num = 938,
                            Name = "Tadbulb",
                            Types = [PokemonType.Electric],
                            BaseStats = new StatsTable
                            {
                                Hp = 61,
                                Atk = 31,
                                Def = 41,
                                SpA = 59,
                                SpD = 35,
                                Spe = 45,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.OwnTempo,
                                Slot1 = AbilityId.Static,
                                Hidden = AbilityId.Damp,
                            },
                            HeightM = 0.3,
                            WeightKg = 0.4,
                            Color = "Yellow",
                        },
                        // 939 - Bellibolt
                        [SpecieId.Bellibolt] = new()
                        {
                            Id = SpecieId.Bellibolt,
                            Num = 939,
                            Name = "Bellibolt",
                            Types = [PokemonType.Electric],
                            BaseStats = new StatsTable
                            {
                                Hp = 109,
                                Atk = 64,
                                Def = 91,
                                SpA = 103,
                                SpD = 83,
                                Spe = 45,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Electromorphosis,
                                Slot1 = AbilityId.Static,
                                Hidden = AbilityId.Damp,
                            },
                            HeightM = 1.2,
                            WeightKg = 113,
                            Color = "Green",
                        },
                        // 940 - Wattrel
                        [SpecieId.Wattrel] = new()
                        {
                            Id = SpecieId.Wattrel,
                            Num = 940,
                            Name = "Wattrel",
                            Types = [PokemonType.Electric, PokemonType.Flying],
                            BaseStats = new StatsTable
                            {
                                Hp = 40,
                                Atk = 40,
                                Def = 35,
                                SpA = 55,
                                SpD = 40,
                                Spe = 70,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.WindPower,
                                Slot1 = AbilityId.VoltAbsorb,
                                Hidden = AbilityId.Competitive,
                            },
                            HeightM = 0.4,
                            WeightKg = 3.6,
                            Color = "Black",
                        },
                        // 941 - Kilowattrel
                        [SpecieId.Kilowattrel] = new()
                        {
                            Id = SpecieId.Kilowattrel,
                            Num = 941,
                            Name = "Kilowattrel",
                            Types = [PokemonType.Electric, PokemonType.Flying],
                            BaseStats = new StatsTable
                            {
                                Hp = 70,
                                Atk = 70,
                                Def = 60,
                                SpA = 105,
                                SpD = 60,
                                Spe = 125,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.WindPower,
                                Slot1 = AbilityId.VoltAbsorb,
                                Hidden = AbilityId.Competitive,
                            },
                            HeightM = 1.4,
                            WeightKg = 38.6,
                            Color = "Yellow",
                        },
                        // 942 - Maschiff
                        [SpecieId.Maschiff] = new()
                        {
                            Id = SpecieId.Maschiff,
                            Num = 942,
                            Name = "Maschiff",
                            Types = [PokemonType.Dark],
                            BaseStats = new StatsTable
                            {
                                Hp = 60,
                                Atk = 78,
                                Def = 60,
                                SpA = 40,
                                SpD = 51,
                                Spe = 51,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Intimidate,
                                Slot1 = AbilityId.RunAway,
                                Hidden = AbilityId.Stakeout,
                            },
                            HeightM = 0.5,
                            WeightKg = 16,
                            Color = "Brown",
                        },
                        // 943 - Mabosstiff
                        [SpecieId.Mabosstiff] = new()
                        {
                            Id = SpecieId.Mabosstiff,
                            Num = 943,
                            Name = "Mabosstiff",
                            Types = [PokemonType.Dark],
                            BaseStats = new StatsTable
                            {
                                Hp = 80,
                                Atk = 120,
                                Def = 90,
                                SpA = 60,
                                SpD = 70,
                                Spe = 85,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Intimidate,
                                Slot1 = AbilityId.GuardDog,
                                Hidden = AbilityId.Stakeout,
                            },
                            HeightM = 1.1,
                            WeightKg = 61,
                            Color = "Gray",
                        },
                        // 944 - Shroodle
                        [SpecieId.Shroodle] = new()
                        {
                            Id = SpecieId.Shroodle,
                            Num = 944,
                            Name = "Shroodle",
                            Types = [PokemonType.Poison, PokemonType.Normal],
                            BaseStats = new StatsTable
                            {
                                Hp = 40,
                                Atk = 65,
                                Def = 35,
                                SpA = 40,
                                SpD = 35,
                                Spe = 75,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Unburden,
                                Slot1 = AbilityId.Pickpocket,
                                Hidden = AbilityId.Prankster,
                            },
                            HeightM = 0.2,
                            WeightKg = 0.7,
                            Color = "Gray",
                        },
                        // 945 - Grafaiai
                        [SpecieId.Grafaiai] = new()
                        {
                            Id = SpecieId.Grafaiai,
                            Num = 945,
                            Name = "Grafaiai",
                            Types = [PokemonType.Poison, PokemonType.Normal],
                            BaseStats = new StatsTable
                            {
                                Hp = 63,
                                Atk = 95,
                                Def = 65,
                                SpA = 80,
                                SpD = 72,
                                Spe = 110,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Unburden,
                                Slot1 = AbilityId.PoisonTouch,
                                Hidden = AbilityId.Prankster,
                            },
                            HeightM = 0.7,
                            WeightKg = 27.2,
                            Color = "Gray",
                        },
                        // 946 - Bramblin
                        [SpecieId.Bramblin] = new()
                        {
                            Id = SpecieId.Bramblin,
                            Num = 946,
                            Name = "Bramblin",
                            Types = [PokemonType.Grass, PokemonType.Ghost],
                            BaseStats = new StatsTable
                            {
                                Hp = 40,
                                Atk = 65,
                                Def = 30,
                                SpA = 45,
                                SpD = 35,
                                Spe = 60,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.WindRider,
                                Hidden = AbilityId.Infiltrator,
                            },
                            HeightM = 0.6,
                            WeightKg = 0.6,
                            Color = "Brown",
                        },
                        // 947 - Brambleghast
                        [SpecieId.Brambleghast] = new()
                        {
                            Id = SpecieId.Brambleghast,
                            Num = 947,
                            Name = "Brambleghast",
                            Types = [PokemonType.Grass, PokemonType.Ghost],
                            BaseStats = new StatsTable
                            {
                                Hp = 55,
                                Atk = 115,
                                Def = 70,
                                SpA = 80,
                                SpD = 70,
                                Spe = 90,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.WindRider,
                                Hidden = AbilityId.Infiltrator,
                            },
                            HeightM = 1.2,
                            WeightKg = 6,
                            Color = "Brown",
                        },
                        // 948 - Toedscool
                        [SpecieId.Toedscool] = new()
                        {
                            Id = SpecieId.Toedscool,
                            Num = 948,
                            Name = "Toedscool",
                            Types = [PokemonType.Ground, PokemonType.Grass],
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
                                Slot0 = AbilityId.MyceliumMight,
                            },
                            HeightM = 0.9,
                            WeightKg = 33,
                            Color = "Yellow",
                        },
                        // 949 - Toedscruel
                        [SpecieId.Toedscruel] = new()
                        {
                            Id = SpecieId.Toedscruel,
                            Num = 949,
                            Name = "Toedscruel",
                            Types = [PokemonType.Ground, PokemonType.Grass],
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
                                Slot0 = AbilityId.MyceliumMight,
                            },
                            HeightM = 1.9,
                            WeightKg = 58,
                            Color = "Black",
                        },
                        // 950 - Klawf
                        [SpecieId.Klawf] = new()
                        {
                            Id = SpecieId.Klawf,
                            Num = 950,
                            Name = "Klawf",
                            Types = [PokemonType.Rock],
                            BaseStats = new StatsTable
                            {
                                Hp = 70,
                                Atk = 100,
                                Def = 115,
                                SpA = 35,
                                SpD = 55,
                                Spe = 75,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.AngerShell,
                                Slot1 = AbilityId.ShellArmor,
                                Hidden = AbilityId.Regenerator,
                            },
                            HeightM = 1.3,
                            WeightKg = 79,
                            Color = "Red",
                        },
                    };
                }
            }
