using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData251To300()
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
        };
    }
}
