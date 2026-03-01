using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData0251to0300()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Celebi] = new()
            {
                Id = SpecieId.Celebi,
                Num = 251,
                Name = "Celebi",
                Types = [PokemonType.Psychic, PokemonType.Grass],
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
                    Slot0 = AbilityId.NaturalCure,
                },
                HeightM = 0.6,
                WeightKg = 5,
                Color = "Green",
            },
            [SpecieId.Treecko] = new()
            {
                Id = SpecieId.Treecko,
                Num = 252,
                Name = "Treecko",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 45,
                    Def = 35,
                    SpA = 65,
                    SpD = 55,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Unburden,
                },
                HeightM = 0.5,
                WeightKg = 5,
                Color = "Green",
            },
            [SpecieId.Grovyle] = new()
            {
                Id = SpecieId.Grovyle,
                Num = 253,
                Name = "Grovyle",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 45,
                    SpA = 85,
                    SpD = 65,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Unburden,
                },
                HeightM = 0.9,
                WeightKg = 21.6,
                Color = "Green",
            },
            [SpecieId.Sceptile] = new()
            {
                Id = SpecieId.Sceptile,
                Num = 254,
                Name = "Sceptile",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 85,
                    Def = 65,
                    SpA = 105,
                    SpD = 85,
                    Spe = 120,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Unburden,
                },
                HeightM = 1.7,
                WeightKg = 52.2,
                Color = "Green",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.SceptileMega] = new()
            {
                Id = SpecieId.SceptileMega,
                Num = 254,
                Name = "Sceptile-Mega",
                BaseSpecies = SpecieId.Sceptile,
                Forme = FormeId.Mega,
                Types = [PokemonType.Grass, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 110,
                    Def = 75,
                    SpA = 145,
                    SpD = 85,
                    Spe = 145,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LightningRod,
                },
                HeightM = 1.9,
                WeightKg = 55.2,
                Color = "Green",
                RequiredItem = ItemId.Sceptilite,
            },
            [SpecieId.Torchic] = new()
            {
                Id = SpecieId.Torchic,
                Num = 255,
                Name = "Torchic",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 60,
                    Def = 40,
                    SpA = 70,
                    SpD = 50,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.SpeedBoost,
                },
                HeightM = 0.4,
                WeightKg = 2.5,
                Color = "Red",
            },
            [SpecieId.Combusken] = new()
            {
                Id = SpecieId.Combusken,
                Num = 256,
                Name = "Combusken",
                Types = [PokemonType.Fire, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 85,
                    Def = 60,
                    SpA = 85,
                    SpD = 60,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.SpeedBoost,
                },
                HeightM = 0.9,
                WeightKg = 19.5,
                Color = "Red",
            },
            [SpecieId.Blaziken] = new()
            {
                Id = SpecieId.Blaziken,
                Num = 257,
                Name = "Blaziken",
                Types = [PokemonType.Fire, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 70,
                    SpA = 110,
                    SpD = 70,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.SpeedBoost,
                },
                HeightM = 1.9,
                WeightKg = 52,
                Color = "Red",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.BlazikenMega] = new()
            {
                Id = SpecieId.BlazikenMega,
                Num = 257,
                Name = "Blaziken-Mega",
                BaseSpecies = SpecieId.Blaziken,
                Forme = FormeId.Mega,
                Types = [PokemonType.Fire, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 160,
                    Def = 80,
                    SpA = 130,
                    SpD = 80,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SpeedBoost,
                },
                HeightM = 1.9,
                WeightKg = 52,
                Color = "Red",
                RequiredItem = ItemId.Blazikenite,
            },
            [SpecieId.Mudkip] = new()
            {
                Id = SpecieId.Mudkip,
                Num = 258,
                Name = "Mudkip",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 70,
                    Def = 50,
                    SpA = 50,
                    SpD = 50,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Damp,
                },
                HeightM = 0.4,
                WeightKg = 7.6,
                Color = "Blue",
            },
            [SpecieId.Marshtomp] = new()
            {
                Id = SpecieId.Marshtomp,
                Num = 259,
                Name = "Marshtomp",
                Types = [PokemonType.Water, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 85,
                    Def = 70,
                    SpA = 60,
                    SpD = 70,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Damp,
                },
                HeightM = 0.7,
                WeightKg = 28,
                Color = "Blue",
            },
            [SpecieId.Swampert] = new()
            {
                Id = SpecieId.Swampert,
                Num = 260,
                Name = "Swampert",
                Types = [PokemonType.Water, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 110,
                    Def = 90,
                    SpA = 85,
                    SpD = 90,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.Damp,
                },
                HeightM = 1.5,
                WeightKg = 81.9,
                Color = "Blue",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.SwampertMega] = new()
            {
                Id = SpecieId.SwampertMega,
                Num = 260,
                Name = "Swampert-Mega",
                BaseSpecies = SpecieId.Swampert,
                Forme = FormeId.Mega,
                Types = [PokemonType.Water, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 150,
                    Def = 110,
                    SpA = 95,
                    SpD = 110,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                },
                HeightM = 1.9,
                WeightKg = 102,
                Color = "Blue",
                RequiredItem = ItemId.Swampertite,
            },
            [SpecieId.Poochyena] = new()
            {
                Id = SpecieId.Poochyena,
                Num = 261,
                Name = "Poochyena",
                Types = [PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 35,
                    SpA = 30,
                    SpD = 30,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.QuickFeet,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 0.5,
                WeightKg = 13.6,
                Color = "Gray",
            },
            [SpecieId.Mightyena] = new()
            {
                Id = SpecieId.Mightyena,
                Num = 262,
                Name = "Mightyena",
                Types = [PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 90,
                    Def = 70,
                    SpA = 60,
                    SpD = 60,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.QuickFeet,
                    Hidden = AbilityId.Moxie,
                },
                HeightM = 1,
                WeightKg = 37,
                Color = "Gray",
            },
            [SpecieId.Zigzagoon] = new()
            {
                Id = SpecieId.Zigzagoon,
                Num = 263,
                Name = "Zigzagoon",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 38,
                    Atk = 30,
                    Def = 41,
                    SpA = 30,
                    SpD = 41,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.QuickFeet,
                },
                HeightM = 0.4,
                WeightKg = 17.5,
                Color = "Brown",
            },
            [SpecieId.ZigzagoonGalar] = new()
            {
                Id = SpecieId.ZigzagoonGalar,
                Num = 263,
                Name = "Zigzagoon-Galar",
                BaseSpecies = SpecieId.Zigzagoon,
                Forme = FormeId.Galar,
                Types = [PokemonType.Dark, PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 38,
                    Atk = 30,
                    Def = 41,
                    SpA = 30,
                    SpD = 41,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.QuickFeet,
                },
                HeightM = 0.4,
                WeightKg = 17.5,
                Color = "White",
            },
            [SpecieId.Linoone] = new()
            {
                Id = SpecieId.Linoone,
                Num = 264,
                Name = "Linoone",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 70,
                    Def = 61,
                    SpA = 50,
                    SpD = 61,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.QuickFeet,
                },
                HeightM = 0.5,
                WeightKg = 32.5,
                Color = "White",
            },
            [SpecieId.LinooneGalar] = new()
            {
                Id = SpecieId.LinooneGalar,
                Num = 264,
                Name = "Linoone-Galar",
                BaseSpecies = SpecieId.Linoone,
                Forme = FormeId.Galar,
                Types = [PokemonType.Dark, PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 70,
                    Def = 61,
                    SpA = 50,
                    SpD = 61,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.QuickFeet,
                },
                HeightM = 0.5,
                WeightKg = 32.5,
                Color = "White",
            },
            [SpecieId.Wurmple] = new()
            {
                Id = SpecieId.Wurmple,
                Num = 265,
                Name = "Wurmple",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 45,
                    Def = 35,
                    SpA = 20,
                    SpD = 30,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldDust,
                    Hidden = AbilityId.RunAway,
                },
                HeightM = 0.3,
                WeightKg = 3.6,
                Color = "Red",
            },
            [SpecieId.Silcoon] = new()
            {
                Id = SpecieId.Silcoon,
                Num = 266,
                Name = "Silcoon",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 35,
                    Def = 55,
                    SpA = 25,
                    SpD = 25,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                },
                HeightM = 0.6,
                WeightKg = 10,
                Color = "White",
            },
            [SpecieId.Beautifly] = new()
            {
                Id = SpecieId.Beautifly,
                Num = 267,
                Name = "Beautifly",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 70,
                    Def = 50,
                    SpA = 100,
                    SpD = 50,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Hidden = AbilityId.Rivalry,
                },
                HeightM = 1,
                WeightKg = 28.4,
                Color = "Yellow",
            },
            [SpecieId.Cascoon] = new()
            {
                Id = SpecieId.Cascoon,
                Num = 268,
                Name = "Cascoon",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 35,
                    Def = 55,
                    SpA = 25,
                    SpD = 25,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                },
                HeightM = 0.7,
                WeightKg = 11.5,
                Color = "Purple",
            },
            [SpecieId.Dustox] = new()
            {
                Id = SpecieId.Dustox,
                Num = 269,
                Name = "Dustox",
                Types = [PokemonType.Bug, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 50,
                    Def = 70,
                    SpA = 50,
                    SpD = 90,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldDust,
                    Hidden = AbilityId.CompoundEyes,
                },
                HeightM = 1.2,
                WeightKg = 31.6,
                Color = "Green",
            },
            [SpecieId.Lotad] = new()
            {
                Id = SpecieId.Lotad,
                Num = 270,
                Name = "Lotad",
                Types = [PokemonType.Water, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 30,
                    Def = 30,
                    SpA = 40,
                    SpD = 50,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.RainDish,
                    Hidden = AbilityId.OwnTempo,
                },
                HeightM = 0.5,
                WeightKg = 2.6,
                Color = "Green",
            },
            [SpecieId.Lombre] = new()
            {
                Id = SpecieId.Lombre,
                Num = 271,
                Name = "Lombre",
                Types = [PokemonType.Water, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 50,
                    Def = 50,
                    SpA = 60,
                    SpD = 70,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.RainDish,
                    Hidden = AbilityId.OwnTempo,
                },
                HeightM = 1.2,
                WeightKg = 32.5,
                Color = "Green",
            },
            [SpecieId.Ludicolo] = new()
            {
                Id = SpecieId.Ludicolo,
                Num = 272,
                Name = "Ludicolo",
                Types = [PokemonType.Water, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 70,
                    Def = 70,
                    SpA = 90,
                    SpD = 100,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.RainDish,
                    Hidden = AbilityId.OwnTempo,
                },
                HeightM = 1.5,
                WeightKg = 55,
                Color = "Green",
            },
            [SpecieId.Seedot] = new()
            {
                Id = SpecieId.Seedot,
                Num = 273,
                Name = "Seedot",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 40,
                    Def = 50,
                    SpA = 30,
                    SpD = 30,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.EarlyBird,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 0.5,
                WeightKg = 4,
                Color = "Brown",
            },
            [SpecieId.Nuzleaf] = new()
            {
                Id = SpecieId.Nuzleaf,
                Num = 274,
                Name = "Nuzleaf",
                Types = [PokemonType.Grass, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 70,
                    Def = 40,
                    SpA = 60,
                    SpD = 40,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.EarlyBird,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 1,
                WeightKg = 28,
                Color = "Brown",
            },
            [SpecieId.Shiftry] = new()
            {
                Id = SpecieId.Shiftry,
                Num = 275,
                Name = "Shiftry",
                Types = [PokemonType.Grass, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 100,
                    Def = 60,
                    SpA = 90,
                    SpD = 60,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.WindRider,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 1.3,
                WeightKg = 59.6,
                Color = "Brown",
            },
            [SpecieId.Taillow] = new()
            {
                Id = SpecieId.Taillow,
                Num = 276,
                Name = "Taillow",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 55,
                    Def = 30,
                    SpA = 30,
                    SpD = 30,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Guts,
                    Hidden = AbilityId.Scrappy,
                },
                HeightM = 0.3,
                WeightKg = 2.3,
                Color = "Blue",
            },
            [SpecieId.Swellow] = new()
            {
                Id = SpecieId.Swellow,
                Num = 277,
                Name = "Swellow",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 85,
                    Def = 60,
                    SpA = 75,
                    SpD = 50,
                    Spe = 125,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Guts,
                    Hidden = AbilityId.Scrappy,
                },
                HeightM = 0.7,
                WeightKg = 19.8,
                Color = "Blue",
            },
            [SpecieId.Wingull] = new()
            {
                Id = SpecieId.Wingull,
                Num = 278,
                Name = "Wingull",
                Types = [PokemonType.Water, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 30,
                    Def = 30,
                    SpA = 55,
                    SpD = 30,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.Hydration,
                    Hidden = AbilityId.RainDish,
                },
                HeightM = 0.6,
                WeightKg = 9.5,
                Color = "White",
            },
            [SpecieId.Pelipper] = new()
            {
                Id = SpecieId.Pelipper,
                Num = 279,
                Name = "Pelipper",
                Types = [PokemonType.Water, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 50,
                    Def = 100,
                    SpA = 95,
                    SpD = 70,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.Drizzle,
                    Hidden = AbilityId.RainDish,
                },
                HeightM = 1.2,
                WeightKg = 28,
                Color = "Yellow",
            },
            [SpecieId.Ralts] = new()
            {
                Id = SpecieId.Ralts,
                Num = 280,
                Name = "Ralts",
                Types = [PokemonType.Psychic, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 28,
                    Atk = 25,
                    Def = 25,
                    SpA = 45,
                    SpD = 35,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                    Slot1 = AbilityId.Trace,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 0.4,
                WeightKg = 6.6,
                Color = "White",
            },
            [SpecieId.Kirlia] = new()
            {
                Id = SpecieId.Kirlia,
                Num = 281,
                Name = "Kirlia",
                Types = [PokemonType.Psychic, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 38,
                    Atk = 35,
                    Def = 35,
                    SpA = 65,
                    SpD = 55,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                    Slot1 = AbilityId.Trace,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 0.8,
                WeightKg = 20.2,
                Color = "White",
            },
            [SpecieId.Gardevoir] = new()
            {
                Id = SpecieId.Gardevoir,
                Num = 282,
                Name = "Gardevoir",
                Types = [PokemonType.Psychic, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 65,
                    Def = 65,
                    SpA = 125,
                    SpD = 115,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                    Slot1 = AbilityId.Trace,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 1.6,
                WeightKg = 48.4,
                Color = "White",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.GardevoirMega] = new()
            {
                Id = SpecieId.GardevoirMega,
                Num = 282,
                Name = "Gardevoir-Mega",
                BaseSpecies = SpecieId.Gardevoir,
                Forme = FormeId.Mega,
                Types = [PokemonType.Psychic, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 85,
                    Def = 65,
                    SpA = 165,
                    SpD = 135,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pixilate,
                },
                HeightM = 1.6,
                WeightKg = 48.4,
                Color = "White",
                RequiredItem = ItemId.Gardevoirite,
            },
            [SpecieId.Surskit] = new()
            {
                Id = SpecieId.Surskit,
                Num = 283,
                Name = "Surskit",
                Types = [PokemonType.Bug, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 30,
                    Def = 32,
                    SpA = 50,
                    SpD = 52,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Hidden = AbilityId.RainDish,
                },
                HeightM = 0.5,
                WeightKg = 1.7,
                Color = "Blue",
            },
            [SpecieId.Masquerain] = new()
            {
                Id = SpecieId.Masquerain,
                Num = 284,
                Name = "Masquerain",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 60,
                    Def = 62,
                    SpA = 100,
                    SpD = 82,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 0.8,
                WeightKg = 3.6,
                Color = "Blue",
            },
            [SpecieId.Shroomish] = new()
            {
                Id = SpecieId.Shroomish,
                Num = 285,
                Name = "Shroomish",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 40,
                    Def = 60,
                    SpA = 40,
                    SpD = 60,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EffectSpore,
                    Slot1 = AbilityId.PoisonHeal,
                    Hidden = AbilityId.QuickFeet,
                },
                HeightM = 0.4,
                WeightKg = 4.5,
                Color = "Brown",
            },
            [SpecieId.Breloom] = new()
            {
                Id = SpecieId.Breloom,
                Num = 286,
                Name = "Breloom",
                Types = [PokemonType.Grass, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 130,
                    Def = 80,
                    SpA = 60,
                    SpD = 60,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EffectSpore,
                    Slot1 = AbilityId.PoisonHeal,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 1.2,
                WeightKg = 39.2,
                Color = "Green",
            },
            [SpecieId.Slakoth] = new()
            {
                Id = SpecieId.Slakoth,
                Num = 287,
                Name = "Slakoth",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 60,
                    Def = 60,
                    SpA = 35,
                    SpD = 35,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Truant,
                },
                HeightM = 0.8,
                WeightKg = 24,
                Color = "Brown",
            },
            [SpecieId.Vigoroth] = new()
            {
                Id = SpecieId.Vigoroth,
                Num = 288,
                Name = "Vigoroth",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 80,
                    Def = 80,
                    SpA = 55,
                    SpD = 55,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VitalSpirit,
                },
                HeightM = 1.4,
                WeightKg = 46.5,
                Color = "White",
            },
            [SpecieId.Slaking] = new()
            {
                Id = SpecieId.Slaking,
                Num = 289,
                Name = "Slaking",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 150,
                    Atk = 160,
                    Def = 100,
                    SpA = 95,
                    SpD = 65,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Truant,
                },
                HeightM = 2,
                WeightKg = 130.5,
                Color = "Brown",
            },
            [SpecieId.Nincada] = new()
            {
                Id = SpecieId.Nincada,
                Num = 290,
                Name = "Nincada",
                Types = [PokemonType.Bug, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 31,
                    Atk = 45,
                    Def = 90,
                    SpA = 30,
                    SpD = 30,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CompoundEyes,
                    Hidden = AbilityId.RunAway,
                },
                HeightM = 0.5,
                WeightKg = 5.5,
                Color = "Gray",
            },
            [SpecieId.Ninjask] = new()
            {
                Id = SpecieId.Ninjask,
                Num = 291,
                Name = "Ninjask",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 61,
                    Atk = 90,
                    Def = 45,
                    SpA = 50,
                    SpD = 50,
                    Spe = 160,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SpeedBoost,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 0.8,
                WeightKg = 12,
                Color = "Yellow",
            },
            [SpecieId.Shedinja] = new()
            {
                Id = SpecieId.Shedinja,
                Num = 292,
                Name = "Shedinja",
                Types = [PokemonType.Bug, PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 1,
                    Atk = 90,
                    Def = 45,
                    SpA = 30,
                    SpD = 30,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WonderGuard,
                },
                HeightM = 0.8,
                WeightKg = 1.2,
                Color = "Brown",
            },
            [SpecieId.Whismur] = new()
            {
                Id = SpecieId.Whismur,
                Num = 293,
                Name = "Whismur",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 64,
                    Atk = 51,
                    Def = 23,
                    SpA = 51,
                    SpD = 23,
                    Spe = 28,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Soundproof,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 0.6,
                WeightKg = 16.3,
                Color = "Pink",
            },
            [SpecieId.Loudred] = new()
            {
                Id = SpecieId.Loudred,
                Num = 294,
                Name = "Loudred",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 84,
                    Atk = 71,
                    Def = 43,
                    SpA = 71,
                    SpD = 43,
                    Spe = 48,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Soundproof,
                    Hidden = AbilityId.Scrappy,
                },
                HeightM = 1,
                WeightKg = 40.5,
                Color = "Blue",
            },
            [SpecieId.Exploud] = new()
            {
                Id = SpecieId.Exploud,
                Num = 295,
                Name = "Exploud",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 104,
                    Atk = 91,
                    Def = 63,
                    SpA = 91,
                    SpD = 73,
                    Spe = 68,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Soundproof,
                    Hidden = AbilityId.Scrappy,
                },
                HeightM = 1.5,
                WeightKg = 84,
                Color = "Blue",
            },
            [SpecieId.Makuhita] = new()
            {
                Id = SpecieId.Makuhita,
                Num = 296,
                Name = "Makuhita",
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 72,
                    Atk = 60,
                    Def = 30,
                    SpA = 20,
                    SpD = 30,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.Guts,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 1,
                WeightKg = 86.4,
                Color = "Yellow",
            },
            [SpecieId.Hariyama] = new()
            {
                Id = SpecieId.Hariyama,
                Num = 297,
                Name = "Hariyama",
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 144,
                    Atk = 120,
                    Def = 60,
                    SpA = 40,
                    SpD = 60,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.Guts,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 2.3,
                WeightKg = 253.8,
                Color = "Brown",
            },
            [SpecieId.Azurill] = new()
            {
                Id = SpecieId.Azurill,
                Num = 298,
                Name = "Azurill",
                Types = [PokemonType.Normal, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 20,
                    Def = 40,
                    SpA = 20,
                    SpD = 40,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.HugePower,
                    Hidden = AbilityId.SapSipper,
                },
                HeightM = 0.2,
                WeightKg = 2,
                Color = "Blue",
            },
            [SpecieId.Nosepass] = new()
            {
                Id = SpecieId.Nosepass,
                Num = 299,
                Name = "Nosepass",
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 45,
                    Def = 135,
                    SpA = 45,
                    SpD = 90,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Slot1 = AbilityId.MagnetPull,
                    Hidden = AbilityId.SandForce,
                },
                HeightM = 1,
                WeightKg = 97,
                Color = "Gray",
            },
        };
    }
}