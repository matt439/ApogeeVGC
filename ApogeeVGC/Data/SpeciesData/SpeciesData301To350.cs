using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData301To350()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Skitty] = new()
            {
                Id = SpecieId.Skitty,
                Num = 300,
                Name = "Skitty",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 45,
                    Def = 45,
                    SpA = 35,
                    SpD = 35,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.Normalize,
                    Hidden = AbilityId.WonderSkin,
                },
                HeightM = 0.6,
                WeightKg = 11,
                Color = "Pink",
            },
            [SpecieId.Delcatty] = new()
            {
                Id = SpecieId.Delcatty,
                Num = 301,
                Name = "Delcatty",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 65,
                    Def = 65,
                    SpA = 55,
                    SpD = 55,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CuteCharm,
                    Slot1 = AbilityId.Normalize,
                    Hidden = AbilityId.WonderSkin,
                },
                HeightM = 1.1,
                WeightKg = 32.6,
                Color = "Purple",
            },
            [SpecieId.Sableye] = new()
            {
                Id = SpecieId.Sableye,
                Num = 302,
                Name = "Sableye",
                Types = [PokemonType.Dark, PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 75,
                    Def = 75,
                    SpA = 65,
                    SpD = 65,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.Stall,
                    Hidden = AbilityId.Prankster,
                },
                HeightM = 0.5,
                WeightKg = 11,
                Color = "Purple",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.SableyeMega] = new()
            {
                Id = SpecieId.SableyeMega,
                Num = 302,
                Name = "Sableye-Mega",
                BaseSpecies = SpecieId.Sableye,
                Forme = FormeId.Mega,
                Types = [PokemonType.Dark, PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 85,
                    Def = 125,
                    SpA = 85,
                    SpD = 115,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagicBounce,
                },
                HeightM = 0.5,
                WeightKg = 161,
                Color = "Purple",
                RequiredItem = Sim.Items.ItemId.Sablenite,
            },
            [SpecieId.Mawile] = new()
            {
                Id = SpecieId.Mawile,
                Num = 303,
                Name = "Mawile",
                Types = [PokemonType.Steel, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 85,
                    Def = 85,
                    SpA = 55,
                    SpD = 55,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.Intimidate,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 0.6,
                WeightKg = 11.5,
                Color = "Black",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.MawileMega] = new()
            {
                Id = SpecieId.MawileMega,
                Num = 303,
                Name = "Mawile-Mega",
                BaseSpecies = SpecieId.Mawile,
                Forme = FormeId.Mega,
                Types = [PokemonType.Steel, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 105,
                    Def = 125,
                    SpA = 55,
                    SpD = 95,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HugePower,
                },
                HeightM = 1,
                WeightKg = 23.5,
                Color = "Black",
                RequiredItem = Sim.Items.ItemId.Mawilite,
            },
            [SpecieId.Aron] = new()
            {
                Id = SpecieId.Aron,
                Num = 304,
                Name = "Aron",
                Types = [PokemonType.Steel, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 70,
                    Def = 100,
                    SpA = 40,
                    SpD = 40,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Slot1 = AbilityId.RockHead,
                    Hidden = AbilityId.HeavyMetal,
                },
                HeightM = 0.4,
                WeightKg = 60,
                Color = "Gray",
            },
            [SpecieId.Lairon] = new()
            {
                Id = SpecieId.Lairon,
                Num = 305,
                Name = "Lairon",
                Types = [PokemonType.Steel, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 90,
                    Def = 140,
                    SpA = 50,
                    SpD = 50,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Slot1 = AbilityId.RockHead,
                    Hidden = AbilityId.HeavyMetal,
                },
                HeightM = 0.9,
                WeightKg = 120,
                Color = "Gray",
                Prevo = SpecieId.Aron,
            },
            [SpecieId.Aggron] = new()
            {
                Id = SpecieId.Aggron,
                Num = 306,
                Name = "Aggron",
                Types = [PokemonType.Steel, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 110,
                    Def = 180,
                    SpA = 60,
                    SpD = 60,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Slot1 = AbilityId.RockHead,
                    Hidden = AbilityId.HeavyMetal,
                },
                HeightM = 2.1,
                WeightKg = 360,
                Color = "Gray",
                Prevo = SpecieId.Lairon,
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.AggronMega] = new()
            {
                Id = SpecieId.AggronMega,
                Num = 306,
                Name = "Aggron-Mega",
                BaseSpecies = SpecieId.Aggron,
                Forme = FormeId.Mega,
                Types = [PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 140,
                    Def = 230,
                    SpA = 60,
                    SpD = 80,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Filter,
                },
                HeightM = 2.2,
                WeightKg = 395,
                Color = "Gray",
                RequiredItem = Sim.Items.ItemId.Aggronite,
            },
            [SpecieId.Meditite] = new()
            {
                Id = SpecieId.Meditite,
                Num = 307,
                Name = "Meditite",
                Types = [PokemonType.Fighting, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 40,
                    Def = 55,
                    SpA = 40,
                    SpD = 55,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PurePower,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 0.6,
                WeightKg = 11.2,
                Color = "Blue",
            },
            [SpecieId.Medicham] = new()
            {
                Id = SpecieId.Medicham,
                Num = 308,
                Name = "Medicham",
                Types = [PokemonType.Fighting, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 60,
                    Def = 75,
                    SpA = 60,
                    SpD = 75,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PurePower,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 1.3,
                WeightKg = 31.5,
                Color = "Red",
                Prevo = SpecieId.Meditite,
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.MedichamMega] = new()
            {
                Id = SpecieId.MedichamMega,
                Num = 308,
                Name = "Medicham-Mega",
                BaseSpecies = SpecieId.Medicham,
                Forme = FormeId.Mega,
                Types = [PokemonType.Fighting, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 100,
                    Def = 85,
                    SpA = 80,
                    SpD = 85,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PurePower,
                },
                HeightM = 1.3,
                WeightKg = 31.5,
                Color = "Red",
                RequiredItem = Sim.Items.ItemId.Medichamite,
            },
            [SpecieId.Electrike] = new()
            {
                Id = SpecieId.Electrike,
                Num = 309,
                Name = "Electrike",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 45,
                    Def = 40,
                    SpA = 65,
                    SpD = 40,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Slot1 = AbilityId.LightningRod,
                    Hidden = AbilityId.Minus,
                },
                HeightM = 0.6,
                WeightKg = 15.2,
                Color = "Green",
            },
            [SpecieId.Manectric] = new()
            {
                Id = SpecieId.Manectric,
                Num = 310,
                Name = "Manectric",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 75,
                    Def = 60,
                    SpA = 105,
                    SpD = 60,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Static,
                    Slot1 = AbilityId.LightningRod,
                    Hidden = AbilityId.Minus,
                },
                HeightM = 1.5,
                WeightKg = 40.2,
                Color = "Yellow",
                Prevo = SpecieId.Electrike,
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.ManectricMega] = new()
            {
                Id = SpecieId.ManectricMega,
                Num = 310,
                Name = "Manectric-Mega",
                BaseSpecies = SpecieId.Manectric,
                Forme = FormeId.Mega,
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 75,
                    Def = 80,
                    SpA = 135,
                    SpD = 80,
                    Spe = 135,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                },
                HeightM = 1.8,
                WeightKg = 44,
                Color = "Yellow",
                RequiredItem = Sim.Items.ItemId.Manectite,
            },
            [SpecieId.Plusle] = new()
            {
                Id = SpecieId.Plusle,
                Num = 311,
                Name = "Plusle",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 50,
                    Def = 40,
                    SpA = 85,
                    SpD = 75,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Plus,
                    Hidden = AbilityId.LightningRod,
                },
                HeightM = 0.4,
                WeightKg = 4.2,
                Color = "Yellow",
            },
            [SpecieId.Minun] = new()
            {
                Id = SpecieId.Minun,
                Num = 312,
                Name = "Minun",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 40,
                    Def = 50,
                    SpA = 75,
                    SpD = 85,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Minus,
                    Hidden = AbilityId.VoltAbsorb,
                },
                HeightM = 0.4,
                WeightKg = 4.2,
                Color = "Yellow",
            },
            [SpecieId.Volbeat] = new()
            {
                Id = SpecieId.Volbeat,
                Num = 313,
                Name = "Volbeat",
                Types = [PokemonType.Bug],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 73,
                    Def = 75,
                    SpA = 47,
                    SpD = 85,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Illuminate,
                    Slot1 = AbilityId.Swarm,
                    Hidden = AbilityId.Prankster,
                },
                HeightM = 0.7,
                WeightKg = 17.7,
                Color = "Gray",
            },
            [SpecieId.Illumise] = new()
            {
                Id = SpecieId.Illumise,
                Num = 314,
                Name = "Illumise",
                Types = [PokemonType.Bug],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 47,
                    Def = 75,
                    SpA = 73,
                    SpD = 85,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.TintedLens,
                    Hidden = AbilityId.Prankster,
                },
                HeightM = 0.6,
                WeightKg = 17.7,
                Color = "Purple",
            },
            [SpecieId.Roselia] = new()
            {
                Id = SpecieId.Roselia,
                Num = 315,
                Name = "Roselia",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 60,
                    Def = 45,
                    SpA = 100,
                    SpD = 80,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Slot1 = AbilityId.PoisonPoint,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 0.3,
                WeightKg = 2,
                Color = "Green",
            },
            [SpecieId.Gulpin] = new()
            {
                Id = SpecieId.Gulpin,
                Num = 316,
                Name = "Gulpin",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 43,
                    Def = 53,
                    SpA = 43,
                    SpD = 53,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LiquidOoze,
                    Slot1 = AbilityId.StickyHold,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 0.4,
                WeightKg = 10.3,
                Color = "Green",
            },
            [SpecieId.Swalot] = new()
            {
                Id = SpecieId.Swalot,
                Num = 317,
                Name = "Swalot",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 73,
                    Def = 83,
                    SpA = 73,
                    SpD = 83,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LiquidOoze,
                    Slot1 = AbilityId.StickyHold,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 1.7,
                WeightKg = 80,
                Color = "Purple",
                Prevo = SpecieId.Gulpin,
            },
            [SpecieId.Carvanha] = new()
            {
                Id = SpecieId.Carvanha,
                Num = 318,
                Name = "Carvanha",
                Types = [PokemonType.Water, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 90,
                    Def = 20,
                    SpA = 65,
                    SpD = 20,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RoughSkin,
                    Hidden = AbilityId.SpeedBoost,
                },
                HeightM = 0.8,
                WeightKg = 20.8,
                Color = "Red",
            },
            [SpecieId.Sharpedo] = new()
            {
                Id = SpecieId.Sharpedo,
                Num = 319,
                Name = "Sharpedo",
                Types = [PokemonType.Water, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 120,
                    Def = 40,
                    SpA = 95,
                    SpD = 40,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RoughSkin,
                    Hidden = AbilityId.SpeedBoost,
                },
                HeightM = 1.8,
                WeightKg = 88.8,
                Color = "Blue",
                Prevo = SpecieId.Carvanha,
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.SharpedomMega] = new()
            {
                Id = SpecieId.SharpedomMega,
                Num = 319,
                Name = "Sharpedo-Mega",
                BaseSpecies = SpecieId.Sharpedo,
                Forme = FormeId.Mega,
                Types = [PokemonType.Water, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 140,
                    Def = 70,
                    SpA = 110,
                    SpD = 65,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.StrongJaw,
                },
                HeightM = 2.5,
                WeightKg = 130.3,
                Color = "Blue",
                RequiredItem = Sim.Items.ItemId.Sharpedonite,
            },
            [SpecieId.Wailmer] = new()
            {
                Id = SpecieId.Wailmer,
                Num = 320,
                Name = "Wailmer",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 130,
                    Atk = 70,
                    Def = 35,
                    SpA = 70,
                    SpD = 35,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterVeil,
                    Slot1 = AbilityId.Oblivious,
                    Hidden = AbilityId.Pressure,
                },
                HeightM = 2,
                WeightKg = 130,
                Color = "Blue",
            },
            [SpecieId.Wailord] = new()
            {
                Id = SpecieId.Wailord,
                Num = 321,
                Name = "Wailord",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 170,
                    Atk = 90,
                    Def = 45,
                    SpA = 90,
                    SpD = 45,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterVeil,
                    Slot1 = AbilityId.Oblivious,
                    Hidden = AbilityId.Pressure,
                },
                HeightM = 14.5,
                WeightKg = 398,
                Color = "Blue",
                Prevo = SpecieId.Wailmer,
            },
            [SpecieId.Numel] = new()
            {
                Id = SpecieId.Numel,
                Num = 322,
                Name = "Numel",
                Types = [PokemonType.Fire, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 60,
                    Def = 40,
                    SpA = 65,
                    SpD = 45,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.Simple,
                    Hidden = AbilityId.OwnTempo,
                },
                HeightM = 0.7,
                WeightKg = 24,
                Color = "Yellow",
            },
            [SpecieId.Camerupt] = new()
            {
                Id = SpecieId.Camerupt,
                Num = 323,
                Name = "Camerupt",
                Types = [PokemonType.Fire, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 100,
                    Def = 70,
                    SpA = 105,
                    SpD = 75,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagmaArmor,
                    Slot1 = AbilityId.SolidRock,
                    Hidden = AbilityId.AngerPoint,
                },
                HeightM = 1.9,
                WeightKg = 220,
                Color = "Red",
                Prevo = SpecieId.Numel,
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.CameruptMega] = new()
            {
                Id = SpecieId.CameruptMega,
                Num = 323,
                Name = "Camerupt-Mega",
                BaseSpecies = SpecieId.Camerupt,
                Forme = FormeId.Mega,
                Types = [PokemonType.Fire, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 120,
                    Def = 100,
                    SpA = 145,
                    SpD = 105,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SheerForce,
                },
                HeightM = 2.5,
                WeightKg = 320.5,
                Color = "Red",
                RequiredItem = Sim.Items.ItemId.Cameruptite,
            },
            [SpecieId.Torkoal] = new()
            {
                Id = SpecieId.Torkoal,
                Num = 324,
                Name = "Torkoal",
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 85,
                    Def = 140,
                    SpA = 85,
                    SpD = 70,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WhiteSmoke,
                    Slot1 = AbilityId.Drought,
                    Hidden = AbilityId.ShellArmor,
                },
                HeightM = 0.5,
                WeightKg = 80.4,
                Color = "Brown",
            },
        };
    }
}
