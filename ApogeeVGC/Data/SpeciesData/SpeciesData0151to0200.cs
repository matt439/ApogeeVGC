using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData0151to0200()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Mew] = new()
            {
                Id = SpecieId.Mew,
                Num = 151,
                Name = "Mew",
                Types = [PokemonType.Psychic],
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
                    Slot0 = AbilityId.Synchronize,
                },
                HeightM = 0.4,
                WeightKg = 4,
                Color = "Pink",
            },
            [SpecieId.Chikorita] = new()
            {
                Id = SpecieId.Chikorita,
                Num = 152,
                Name = "Chikorita",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 49,
                    Def = 65,
                    SpA = 49,
                    SpD = 65,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 0.9,
                WeightKg = 6.4,
                Color = "Green",
            },
            [SpecieId.Bayleef] = new()
            {
                Id = SpecieId.Bayleef,
                Num = 153,
                Name = "Bayleef",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 62,
                    Def = 80,
                    SpA = 63,
                    SpD = 80,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 1.2,
                WeightKg = 15.8,
                Color = "Green",
            },
            [SpecieId.Meganium] = new()
            {
                Id = SpecieId.Meganium,
                Num = 154,
                Name = "Meganium",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 82,
                    Def = 100,
                    SpA = 83,
                    SpD = 100,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 1.8,
                WeightKg = 100.5,
                Color = "Green",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.MeganiummMega] = new()
            {
                Id = SpecieId.MeganiummMega,
                Num = 154,
                Name = "Meganium-Mega",
                BaseSpecies = SpecieId.Meganium,
                Forme = FormeId.Mega,
                Types = [PokemonType.Grass, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 92,
                    Def = 115,
                    SpA = 143,
                    SpD = 115,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 2.4,
                WeightKg = 201,
                Color = "Green",
                RequiredItem = ItemId.Meganiummite,
            },
            [SpecieId.Cyndaquil] = new()
            {
                Id = SpecieId.Cyndaquil,
                Num = 155,
                Name = "Cyndaquil",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 39,
                    Atk = 52,
                    Def = 43,
                    SpA = 60,
                    SpD = 50,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.FlashFire,
                },
                HeightM = 0.5,
                WeightKg = 7.9,
                Color = "Yellow",
            },
            [SpecieId.Quilava] = new()
            {
                Id = SpecieId.Quilava,
                Num = 156,
                Name = "Quilava",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 58,
                    Atk = 64,
                    Def = 58,
                    SpA = 80,
                    SpD = 65,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.FlashFire,
                },
                HeightM = 0.9,
                WeightKg = 19,
                Color = "Yellow",
            },
            [SpecieId.Typhlosion] = new()
            {
                Id = SpecieId.Typhlosion,
                Num = 157,
                Name = "Typhlosion",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 84,
                    Def = 78,
                    SpA = 109,
                    SpD = 85,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.FlashFire,
                },
                HeightM = 1.7,
                WeightKg = 79.5,
                Color = "Yellow",
            },
            [SpecieId.TyphlosionHisui] = new()
            {
                Id = SpecieId.TyphlosionHisui,
                Num = 157,
                Name = "Typhlosion-Hisui",
                BaseSpecies = SpecieId.Typhlosion,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Fire, PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 73,
                    Atk = 84,
                    Def = 78,
                    SpA = 119,
                    SpD = 85,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 1.6,
                WeightKg = 69.8,
                Color = "Yellow",
            },
            [SpecieId.Totodile] = new()
            {
                Id = SpecieId.Totodile,
                Num = 158,
                Name = "Totodile",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 64,
                    SpA = 44,
                    SpD = 48,
                    Spe = 43,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 0.6,
                WeightKg = 9.5,
                Color = "Blue",
            },
            [SpecieId.Croconaw] = new()
            {
                Id = SpecieId.Croconaw,
                Num = 159,
                Name = "Croconaw",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 80,
                    Def = 80,
                    SpA = 59,
                    SpD = 63,
                    Spe = 58,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 1.1,
                WeightKg = 25,
                Color = "Blue",
            },
            [SpecieId.Feraligatr] = new()
            {
                Id = SpecieId.Feraligatr,
                Num = 160,
                Name = "Feraligatr",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 105,
                    Def = 100,
                    SpA = 79,
                    SpD = 83,
                    Spe = 78,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 2.3,
                WeightKg = 88.8,
                Color = "Blue",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.FeraligatrMega] = new()
            {
                Id = SpecieId.FeraligatrMega,
                Num = 160,
                Name = "Feraligatr-Mega",
                BaseSpecies = SpecieId.Feraligatr,
                Forme = FormeId.Mega,
                Types = [PokemonType.Water, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 160,
                    Def = 125,
                    SpA = 89,
                    SpD = 93,
                    Spe = 78,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 2.3,
                WeightKg = 108.8,
                Color = "Blue",
                RequiredItem = ItemId.Feraligatrite,
            },
            [SpecieId.Sentret] = new()
            {
                Id = SpecieId.Sentret,
                Num = 161,
                Name = "Sentret",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 46,
                    Def = 34,
                    SpA = 35,
                    SpD = 45,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.KeenEye,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 0.8,
                WeightKg = 6,
                Color = "Brown",
            },
            [SpecieId.Furret] = new()
            {
                Id = SpecieId.Furret,
                Num = 162,
                Name = "Furret",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 76,
                    Def = 64,
                    SpA = 45,
                    SpD = 55,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.KeenEye,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 1.8,
                WeightKg = 32.5,
                Color = "Brown",
            },
            [SpecieId.Hoothoot] = new()
            {
                Id = SpecieId.Hoothoot,
                Num = 163,
                Name = "Hoothoot",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 30,
                    Def = 30,
                    SpA = 36,
                    SpD = 56,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Slot1 = AbilityId.KeenEye,
                    Hidden = AbilityId.TintedLens,
                },
                HeightM = 0.7,
                WeightKg = 21.2,
                Color = "Brown",
            },
            [SpecieId.Noctowl] = new()
            {
                Id = SpecieId.Noctowl,
                Num = 164,
                Name = "Noctowl",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 50,
                    Def = 50,
                    SpA = 86,
                    SpD = 96,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Slot1 = AbilityId.KeenEye,
                    Hidden = AbilityId.TintedLens,
                },
                HeightM = 1.6,
                WeightKg = 40.8,
                Color = "Brown",
            },
            [SpecieId.Ledyba] = new()
            {
                Id = SpecieId.Ledyba,
                Num = 165,
                Name = "Ledyba",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 20,
                    Def = 30,
                    SpA = 40,
                    SpD = 80,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Slot1 = AbilityId.EarlyBird,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 1.0,
                WeightKg = 10.8,
                Color = "Red",
            },
            [SpecieId.Ledian] = new()
            {
                Id = SpecieId.Ledian,
                Num = 166,
                Name = "Ledian",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 35,
                    Def = 50,
                    SpA = 55,
                    SpD = 110,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Slot1 = AbilityId.EarlyBird,
                    Hidden = AbilityId.IronFist,
                },
                HeightM = 1.4,
                WeightKg = 35.6,
                Color = "Red",
            },
            [SpecieId.Spinarak] = new()
            {
                Id = SpecieId.Spinarak,
                Num = 167,
                Name = "Spinarak",
                Types = [PokemonType.Bug, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 60,
                    Def = 40,
                    SpA = 40,
                    SpD = 40,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Slot1 = AbilityId.Insomnia,
                    Hidden = AbilityId.Sniper,
                },
                HeightM = 0.5,
                WeightKg = 8.5,
                Color = "Green",
            },
            [SpecieId.Ariados] = new()
            {
                Id = SpecieId.Ariados,
                Num = 168,
                Name = "Ariados",
                Types = [PokemonType.Bug, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 90,
                    Def = 70,
                    SpA = 60,
                    SpD = 70,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Slot1 = AbilityId.Insomnia,
                    Hidden = AbilityId.Sniper,
                },
                HeightM = 1.1,
                WeightKg = 33.5,
                Color = "Red",
            },
            [SpecieId.Crobat] = new()
            {
                Id = SpecieId.Crobat,
                Num = 169,
                Name = "Crobat",
                Types = [PokemonType.Poison, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 90,
                    Def = 80,
                    SpA = 70,
                    SpD = 80,
                    Spe = 130,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 1.8,
                WeightKg = 75,
                Color = "Purple",
            },
            [SpecieId.Chinchou] = new()
            {
                Id = SpecieId.Chinchou,
                Num = 170,
                Name = "Chinchou",
                Types = [PokemonType.Water, PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 38,
                    Def = 38,
                    SpA = 56,
                    SpD = 56,
                    Spe = 67,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VoltAbsorb,
                    Slot1 = AbilityId.Illuminate,
                    Hidden = AbilityId.WaterAbsorb,
                },
                HeightM = 0.5,
                WeightKg = 12,
                Color = "Blue",
            },
            [SpecieId.Lanturn] = new()
            {
                Id = SpecieId.Lanturn,
                Num = 171,
                Name = "Lanturn",
                Types = [PokemonType.Water, PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 125,
                    Atk = 58,
                    Def = 58,
                    SpA = 76,
                    SpD = 76,
                    Spe = 67,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VoltAbsorb,
                    Slot1 = AbilityId.Illuminate,
                    Hidden = AbilityId.WaterAbsorb,
                },
                HeightM = 1.2,
                WeightKg = 22.5,
                Color = "Blue",
            },
            [SpecieId.Pichu] = new()
            {
                Id = SpecieId.Pichu,
                Num = 172,
                Name = "Pichu",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 20,
                    Atk = 40,
                    Def = 15,
                    SpA = 35,
                    SpD = 35,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.3,
                WeightKg = 2,
                Color = "Yellow",
            },
            [SpecieId.PichuSpikyEared] = new()
            {
                Id = SpecieId.PichuSpikyEared,
                Num = 172,
                Name = "Pichu-Spiky-eared",
                BaseSpecies = SpecieId.Pichu,
                Forme = FormeId.SpikyEared,
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 20,
                    Atk = 40,
                    Def = 15,
                    SpA = 35,
                    SpD = 35,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                },
                HeightM = 0.3,
                WeightKg = 2,
                Color = "Yellow",
            },
            [SpecieId.Cleffa] = new()
            {
                Id = SpecieId.Cleffa,
                Num = 173,
                Name = "Cleffa",
                Types = [PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 25,
                    Def = 28,
                    SpA = 45,
                    SpD = 55,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.MagicGuard,
                    Hidden = AbilityId.FriendGuard,
                },
                HeightM = 0.3,
                WeightKg = 3,
                Color = "Pink",
            },
            [SpecieId.Igglybuff] = new()
            {
                Id = SpecieId.Igglybuff,
                Num = 174,
                Name = "Igglybuff",
                Types = [PokemonType.Normal, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 30,
                    Def = 15,
                    SpA = 40,
                    SpD = 20,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.Competitive,
                    Hidden = AbilityId.FriendGuard,
                },
                HeightM = 0.3,
                WeightKg = 1,
                Color = "Pink",
            },
            [SpecieId.Togepi] = new()
            {
                Id = SpecieId.Togepi,
                Num = 175,
                Name = "Togepi",
                Types = [PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 20,
                    Def = 65,
                    SpA = 40,
                    SpD = 65,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Hustle,
                    Slot1 = AbilityId.SereneGrace,
                    Hidden = AbilityId.SuperLuck,
                },
                HeightM = 0.3,
                WeightKg = 1.5,
                Color = "White",
            },
            [SpecieId.Togetic] = new()
            {
                Id = SpecieId.Togetic,
                Num = 176,
                Name = "Togetic",
                Types = [PokemonType.Fairy, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 40,
                    Def = 85,
                    SpA = 80,
                    SpD = 105,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Hustle,
                    Slot1 = AbilityId.SereneGrace,
                    Hidden = AbilityId.SuperLuck,
                },
                HeightM = 0.6,
                WeightKg = 3.2,
                Color = "White",
            },
            [SpecieId.Natu] = new()
            {
                Id = SpecieId.Natu,
                Num = 177,
                Name = "Natu",
                Types = [PokemonType.Psychic, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 50,
                    Def = 45,
                    SpA = 70,
                    SpD = 45,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                    Slot1 = AbilityId.EarlyBird,
                    Hidden = AbilityId.MagicBounce,
                },
                HeightM = 0.2,
                WeightKg = 2,
                Color = "Green",
            },
            [SpecieId.Xatu] = new()
            {
                Id = SpecieId.Xatu,
                Num = 178,
                Name = "Xatu",
                Types = [PokemonType.Psychic, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 75,
                    Def = 70,
                    SpA = 95,
                    SpD = 70,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                    Slot1 = AbilityId.EarlyBird,
                    Hidden = AbilityId.MagicBounce,
                },
                HeightM = 1.5,
                WeightKg = 15,
                Color = "Green",
            },
            [SpecieId.Mareep] = new()
            {
                Id = SpecieId.Mareep,
                Num = 179,
                Name = "Mareep",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 40,
                    Def = 40,
                    SpA = 65,
                    SpD = 45,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.Plus,
                },
                HeightM = 0.6,
                WeightKg = 7.8,
                Color = "White",
            },
            [SpecieId.Flaaffy] = new()
            {
                Id = SpecieId.Flaaffy,
                Num = 180,
                Name = "Flaaffy",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 55,
                    Def = 55,
                    SpA = 80,
                    SpD = 60,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.Plus,
                },
                HeightM = 0.8,
                WeightKg = 13.3,
                Color = "Pink",
            },
            [SpecieId.Ampharos] = new()
            {
                Id = SpecieId.Ampharos,
                Num = 181,
                Name = "Ampharos",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 75,
                    Def = 85,
                    SpA = 115,
                    SpD = 90,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Hidden = AbilityId.Plus,
                },
                HeightM = 1.4,
                WeightKg = 61.5,
                Color = "Yellow",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.AmpharosMega] = new()
            {
                Id = SpecieId.AmpharosMega,
                Num = 181,
                Name = "Ampharos-Mega",
                BaseSpecies = SpecieId.Ampharos,
                Forme = FormeId.Mega,
                Types = [PokemonType.Electric, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 95,
                    Def = 105,
                    SpA = 165,
                    SpD = 110,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MoldBreaker,
                },
                HeightM = 1.4,
                WeightKg = 61.5,
                Color = "Yellow",
                RequiredItem = ItemId.Ampharosite,
            },
            [SpecieId.Bellossom] = new()
            {
                Id = SpecieId.Bellossom,
                Num = 182,
                Name = "Bellossom",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 80,
                    Def = 95,
                    SpA = 90,
                    SpD = 100,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Hidden = AbilityId.Healer,
                },
                HeightM = 0.4,
                WeightKg = 5.8,
                Color = "Green",
            },
            [SpecieId.Marill] = new()
            {
                Id = SpecieId.Marill,
                Num = 183,
                Name = "Marill",
                Types = [PokemonType.Water, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 20,
                    Def = 50,
                    SpA = 20,
                    SpD = 50,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.HugePower,
                    Hidden = AbilityId.SapSipper,
                },
                HeightM = 0.4,
                WeightKg = 8.5,
                Color = "Blue",
            },
            [SpecieId.Azumarill] = new()
            {
                Id = SpecieId.Azumarill,
                Num = 184,
                Name = "Azumarill",
                Types = [PokemonType.Water, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 50,
                    Def = 80,
                    SpA = 60,
                    SpD = 80,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.HugePower,
                    Hidden = AbilityId.SapSipper,
                },
                HeightM = 0.8,
                WeightKg = 28.5,
                Color = "Blue",
            },
            [SpecieId.Sudowoodo] = new()
            {
                Id = SpecieId.Sudowoodo,
                Num = 185,
                Name = "Sudowoodo",
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 100,
                    Def = 115,
                    SpA = 30,
                    SpD = 65,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Slot1 = AbilityId.RockHead,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 1.2,
                WeightKg = 38,
                Color = "Brown",
            },
            [SpecieId.Politoed] = new()
            {
                Id = SpecieId.Politoed,
                Num = 186,
                Name = "Politoed",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 75,
                    Def = 75,
                    SpA = 90,
                    SpD = 100,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterAbsorb,
                    Slot1 = AbilityId.Damp,
                    Hidden = AbilityId.Drizzle,
                },
                HeightM = 1.1,
                WeightKg = 33.9,
                Color = "Green",
            },
            [SpecieId.Hoppip] = new()
            {
                Id = SpecieId.Hoppip,
                Num = 187,
                Name = "Hoppip",
                Types = [PokemonType.Grass, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 35,
                    Def = 40,
                    SpA = 35,
                    SpD = 55,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.LeafGuard,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 0.4,
                WeightKg = 0.5,
                Color = "Pink",
            },
            [SpecieId.Skiploom] = new()
            {
                Id = SpecieId.Skiploom,
                Num = 188,
                Name = "Skiploom",
                Types = [PokemonType.Grass, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 45,
                    Def = 50,
                    SpA = 45,
                    SpD = 65,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.LeafGuard,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 0.6,
                WeightKg = 1,
                Color = "Green",
            },
            [SpecieId.Jumpluff] = new()
            {
                Id = SpecieId.Jumpluff,
                Num = 189,
                Name = "Jumpluff",
                Types = [PokemonType.Grass, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 55,
                    Def = 70,
                    SpA = 55,
                    SpD = 95,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.LeafGuard,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 0.8,
                WeightKg = 3,
                Color = "Blue",
            },
            [SpecieId.Aipom] = new()
            {
                Id = SpecieId.Aipom,
                Num = 190,
                Name = "Aipom",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 70,
                    Def = 55,
                    SpA = 40,
                    SpD = 55,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RunAway,
                    Slot1 = AbilityId.Pickup,
                    Hidden = AbilityId.SkillLink,
                },
                HeightM = 0.8,
                WeightKg = 11.5,
                Color = "Purple",
            },
            [SpecieId.Sunkern] = new()
            {
                Id = SpecieId.Sunkern,
                Num = 191,
                Name = "Sunkern",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 30,
                    Def = 30,
                    SpA = 30,
                    SpD = 30,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.SolarPower,
                    Hidden = AbilityId.EarlyBird,
                },
                HeightM = 0.3,
                WeightKg = 1.8,
                Color = "Yellow",
            },
            [SpecieId.Sunflora] = new()
            {
                Id = SpecieId.Sunflora,
                Num = 192,
                Name = "Sunflora",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 75,
                    Def = 55,
                    SpA = 105,
                    SpD = 85,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.SolarPower,
                    Hidden = AbilityId.EarlyBird,
                },
                HeightM = 0.8,
                WeightKg = 8.5,
                Color = "Yellow",
            },
            [SpecieId.Yanma] = new()
            {
                Id = SpecieId.Yanma,
                Num = 193,
                Name = "Yanma",
                Types = [PokemonType.Bug, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 65,
                    Def = 45,
                    SpA = 75,
                    SpD = 45,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SpeedBoost,
                    Slot1 = AbilityId.CompoundEyes,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 1.2,
                WeightKg = 38,
                Color = "Red",
            },
            [SpecieId.Wooper] = new()
            {
                Id = SpecieId.Wooper,
                Num = 194,
                Name = "Wooper",
                Types = [PokemonType.Water, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 45,
                    Def = 45,
                    SpA = 25,
                    SpD = 25,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Damp,
                    Slot1 = AbilityId.WaterAbsorb,
                    Hidden = AbilityId.Unaware,
                },
                HeightM = 0.4,
                WeightKg = 8.5,
                Color = "Blue",
            },
            [SpecieId.WooperPaldea] = new()
            {
                Id = SpecieId.WooperPaldea,
                Num = 194,
                Name = "Wooper-Paldea",
                BaseSpecies = SpecieId.Wooper,
                Forme = FormeId.Paldea,
                Types = [PokemonType.Poison, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 45,
                    Def = 45,
                    SpA = 25,
                    SpD = 25,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.WaterAbsorb,
                    Hidden = AbilityId.Unaware,
                },
                HeightM = 0.4,
                WeightKg = 11,
                Color = "Brown",
            },
            [SpecieId.Quagsire] = new()
            {
                Id = SpecieId.Quagsire,
                Num = 195,
                Name = "Quagsire",
                Types = [PokemonType.Water, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 85,
                    Def = 85,
                    SpA = 65,
                    SpD = 65,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Damp,
                    Slot1 = AbilityId.WaterAbsorb,
                    Hidden = AbilityId.Unaware,
                },
                HeightM = 1.4,
                WeightKg = 75,
                Color = "Blue",
            },
            [SpecieId.Espeon] = new()
            {
                Id = SpecieId.Espeon,
                Num = 196,
                Name = "Espeon",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 65,
                    Def = 60,
                    SpA = 130,
                    SpD = 95,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                    Hidden = AbilityId.MagicBounce,
                },
                HeightM = 0.9,
                WeightKg = 26.5,
                Color = "Purple",
            },
            [SpecieId.Umbreon] = new()
            {
                Id = SpecieId.Umbreon,
                Num = 197,
                Name = "Umbreon",
                Types = [PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 65,
                    Def = 110,
                    SpA = 60,
                    SpD = 130,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Synchronize,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 1.0,
                WeightKg = 27,
                Color = "Black",
            },
            [SpecieId.Murkrow] = new()
            {
                Id = SpecieId.Murkrow,
                Num = 198,
                Name = "Murkrow",
                Types = [PokemonType.Dark, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 85,
                    Def = 42,
                    SpA = 85,
                    SpD = 42,
                    Spe = 91,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Slot1 = AbilityId.SuperLuck,
                    Hidden = AbilityId.Prankster,
                },
                HeightM = 0.5,
                WeightKg = 2.1,
                Color = "Black",
            },
            [SpecieId.Slowking] = new()
            {
                Id = SpecieId.Slowking,
                Num = 199,
                Name = "Slowking",
                Types = [PokemonType.Water, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 75,
                    Def = 80,
                    SpA = 100,
                    SpD = 110,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 2.0,
                WeightKg = 79.5,
                Color = "Pink",
            },
            [SpecieId.SlowkingGalar] = new()
            {
                Id = SpecieId.SlowkingGalar,
                Num = 199,
                Name = "Slowking-Galar",
                BaseSpecies = SpecieId.Slowking,
                Forme = FormeId.Galar,
                Types = [PokemonType.Poison, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 65,
                    Def = 80,
                    SpA = 110,
                    SpD = 110,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuriousMedicine,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 1.8,
                WeightKg = 79.5,
                Color = "Pink",
            },
            [SpecieId.Misdreavus] = new()
            {
                Id = SpecieId.Misdreavus,
                Num = 200,
                Name = "Misdreavus",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 60,
                    Def = 60,
                    SpA = 85,
                    SpD = 85,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.7,
                WeightKg = 1,
                Color = "Gray",
            },
        };
    }
}