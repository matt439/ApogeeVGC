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
        };
    }
}
