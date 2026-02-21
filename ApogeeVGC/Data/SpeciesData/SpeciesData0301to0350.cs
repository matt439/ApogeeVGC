using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData0301to0350()
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
            [SpecieId.Spoink] = new()
            {
                Id = SpecieId.Spoink,
                Num = 325,
                Name = "Spoink",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 25,
                    Def = 35,
                    SpA = 70,
                    SpD = 80,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 0.7,
                WeightKg = 30.6,
                Color = "Black",
            },
            [SpecieId.Grumpig] = new()
            {
                Id = SpecieId.Grumpig,
                Num = 326,
                Name = "Grumpig",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 45,
                    Def = 65,
                    SpA = 90,
                    SpD = 110,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.Gluttony,
                },
                HeightM = 0.9,
                WeightKg = 71.5,
                Color = "Purple",
                Prevo = SpecieId.Spoink,
            },
            [SpecieId.Spinda] = new()
            {
                Id = SpecieId.Spinda,
                Num = 327,
                Name = "Spinda",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 60,
                    Def = 60,
                    SpA = 60,
                    SpD = 60,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.OwnTempo,
                    Slot1 = AbilityId.TangledFeet,
                    Hidden = AbilityId.Contrary,
                },
                HeightM = 1.1,
                WeightKg = 5,
                Color = "Brown",
            },
            [SpecieId.Trapinch] = new()
            {
                Id = SpecieId.Trapinch,
                Num = 328,
                Name = "Trapinch",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 100,
                    Def = 45,
                    SpA = 45,
                    SpD = 45,
                    Spe = 10,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.ArenaTrap,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 0.7,
                WeightKg = 15,
                Color = "Brown",
            },
            [SpecieId.Vibrava] = new()
            {
                Id = SpecieId.Vibrava,
                Num = 329,
                Name = "Vibrava",
                Types = [PokemonType.Ground, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 70,
                    Def = 50,
                    SpA = 50,
                    SpD = 50,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 1.1,
                WeightKg = 15.3,
                Color = "Green",
                Prevo = SpecieId.Trapinch,
            },
            [SpecieId.Flygon] = new()
            {
                Id = SpecieId.Flygon,
                Num = 330,
                Name = "Flygon",
                Types = [PokemonType.Ground, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 100,
                    Def = 80,
                    SpA = 80,
                    SpD = 80,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 2,
                WeightKg = 82,
                Color = "Green",
                Prevo = SpecieId.Vibrava,
            },
            [SpecieId.Cacnea] = new()
            {
                Id = SpecieId.Cacnea,
                Num = 331,
                Name = "Cacnea",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 85,
                    Def = 40,
                    SpA = 85,
                    SpD = 40,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandVeil,
                    Hidden = AbilityId.WaterAbsorb,
                },
                HeightM = 0.4,
                WeightKg = 51.3,
                Color = "Green",
            },
            [SpecieId.Cacturne] = new()
            {
                Id = SpecieId.Cacturne,
                Num = 332,
                Name = "Cacturne",
                Types = [PokemonType.Grass, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 115,
                    Def = 60,
                    SpA = 115,
                    SpD = 60,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandVeil,
                    Hidden = AbilityId.WaterAbsorb,
                },
                HeightM = 1.3,
                WeightKg = 77.4,
                Color = "Green",
                Prevo = SpecieId.Cacnea,
            },
            [SpecieId.Swablu] = new()
            {
                Id = SpecieId.Swablu,
                Num = 333,
                Name = "Swablu",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 40,
                    Def = 60,
                    SpA = 40,
                    SpD = 75,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Hidden = AbilityId.CloudNine,
                },
                HeightM = 0.4,
                WeightKg = 1.2,
                Color = "Blue",
            },
            [SpecieId.Altaria] = new()
            {
                Id = SpecieId.Altaria,
                Num = 334,
                Name = "Altaria",
                Types = [PokemonType.Dragon, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 70,
                    Def = 90,
                    SpA = 70,
                    SpD = 105,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Hidden = AbilityId.CloudNine,
                },
                HeightM = 1.1,
                WeightKg = 20.6,
                Color = "Blue",
                Prevo = SpecieId.Swablu,
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.AltariaMega] = new()
            {
                Id = SpecieId.AltariaMega,
                Num = 334,
                Name = "Altaria-Mega",
                BaseSpecies = SpecieId.Altaria,
                Forme = FormeId.Mega,
                Types = [PokemonType.Dragon, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 110,
                    Def = 110,
                    SpA = 110,
                    SpD = 105,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pixilate,
                },
                HeightM = 1.5,
                WeightKg = 20.6,
                Color = "Blue",
            },
            [SpecieId.Zangoose] = new()
            {
                Id = SpecieId.Zangoose,
                Num = 335,
                Name = "Zangoose",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 73,
                    Atk = 115,
                    Def = 60,
                    SpA = 60,
                    SpD = 60,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Immunity,
                    Hidden = AbilityId.ToxicBoost,
                },
                HeightM = 1.3,
                WeightKg = 40.3,
                Color = "White",
            },
            [SpecieId.Seviper] = new()
            {
                Id = SpecieId.Seviper,
                Num = 336,
                Name = "Seviper",
                Types = [PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 73,
                    Atk = 100,
                    Def = 60,
                    SpA = 100,
                    SpD = 60,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 2.7,
                WeightKg = 52.5,
                Color = "Black",
            },
            [SpecieId.Lunatone] = new()
            {
                Id = SpecieId.Lunatone,
                Num = 337,
                Name = "Lunatone",
                Types = [PokemonType.Rock, PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 55,
                    Def = 65,
                    SpA = 95,
                    SpD = 85,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 1,
                WeightKg = 168,
                Color = "Yellow",
            },
            [SpecieId.Solrock] = new()
            {
                Id = SpecieId.Solrock,
                Num = 338,
                Name = "Solrock",
                Types = [PokemonType.Rock, PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 95,
                    Def = 85,
                    SpA = 55,
                    SpD = 65,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 1.2,
                WeightKg = 154,
                Color = "Red",
            },
            [SpecieId.Barboach] = new()
            {
                Id = SpecieId.Barboach,
                Num = 339,
                Name = "Barboach",
                Types = [PokemonType.Water, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 48,
                    Def = 43,
                    SpA = 46,
                    SpD = 41,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.Anticipation,
                    Hidden = AbilityId.Hydration,
                },
                HeightM = 0.4,
                WeightKg = 1.9,
                Color = "Gray",
            },
            [SpecieId.Whiscash] = new()
            {
                Id = SpecieId.Whiscash,
                Num = 340,
                Name = "Whiscash",
                Types = [PokemonType.Water, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 110,
                    Atk = 78,
                    Def = 73,
                    SpA = 76,
                    SpD = 71,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Oblivious,
                    Slot1 = AbilityId.Anticipation,
                    Hidden = AbilityId.Hydration,
                },
                HeightM = 0.9,
                WeightKg = 23.6,
                Color = "Blue",
                Prevo = SpecieId.Barboach,
            },
            [SpecieId.Corphish] = new()
            {
                Id = SpecieId.Corphish,
                Num = 341,
                Name = "Corphish",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 43,
                    Atk = 80,
                    Def = 65,
                    SpA = 50,
                    SpD = 35,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.Adaptability,
                },
                HeightM = 0.6,
                WeightKg = 11.5,
                Color = "Red",
            },
            [SpecieId.Crawdaunt] = new()
            {
                Id = SpecieId.Crawdaunt,
                Num = 342,
                Name = "Crawdaunt",
                Types = [PokemonType.Water, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 63,
                    Atk = 120,
                    Def = 85,
                    SpA = 90,
                    SpD = 55,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.Adaptability,
                },
                HeightM = 1.1,
                WeightKg = 32.8,
                Color = "Red",
                Prevo = SpecieId.Corphish,
            },
            [SpecieId.Baltoy] = new()
            {
                Id = SpecieId.Baltoy,
                Num = 343,
                Name = "Baltoy",
                Types = [PokemonType.Ground, PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 40,
                    Def = 55,
                    SpA = 40,
                    SpD = 70,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.5,
                WeightKg = 21.5,
                Color = "Brown",
            },
            [SpecieId.Claydol] = new()
            {
                Id = SpecieId.Claydol,
                Num = 344,
                Name = "Claydol",
                Types = [PokemonType.Ground, PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 70,
                    Def = 105,
                    SpA = 70,
                    SpD = 120,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 1.5,
                WeightKg = 108,
                Color = "Black",
                Prevo = SpecieId.Baltoy,
            },
            [SpecieId.Lileep] = new()
            {
                Id = SpecieId.Lileep,
                Num = 345,
                Name = "Lileep",
                Types = [PokemonType.Rock, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 66,
                    Atk = 41,
                    Def = 77,
                    SpA = 61,
                    SpD = 87,
                    Spe = 23,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SuctionCups,
                    Hidden = AbilityId.StormDrain,
                },
                HeightM = 1,
                WeightKg = 23.8,
                Color = "Purple",
            },
            [SpecieId.Cradily] = new()
            {
                Id = SpecieId.Cradily,
                Num = 346,
                Name = "Cradily",
                Types = [PokemonType.Rock, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 86,
                    Atk = 81,
                    Def = 97,
                    SpA = 81,
                    SpD = 107,
                    Spe = 43,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SuctionCups,
                    Hidden = AbilityId.StormDrain,
                },
                HeightM = 1.5,
                WeightKg = 60.4,
                Color = "Green",
                Prevo = SpecieId.Lileep,
            },
            [SpecieId.Anorith] = new()
            {
                Id = SpecieId.Anorith,
                Num = 347,
                Name = "Anorith",
                Types = [PokemonType.Rock, PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 95,
                    Def = 50,
                    SpA = 40,
                    SpD = 50,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 0.7,
                WeightKg = 12.5,
                Color = "Gray",
            },
            [SpecieId.Armaldo] = new()
            {
                Id = SpecieId.Armaldo,
                Num = 348,
                Name = "Armaldo",
                Types = [PokemonType.Rock, PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 125,
                    Def = 100,
                    SpA = 70,
                    SpD = 80,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 1.5,
                WeightKg = 68.2,
                Color = "Gray",
                Prevo = SpecieId.Anorith,
            },
            [SpecieId.Feebas] = new()
            {
                Id = SpecieId.Feebas,
                Num = 349,
                Name = "Feebas",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 20,
                    Atk = 15,
                    Def = 20,
                    SpA = 10,
                    SpD = 55,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwiftSwim,
                    Slot1 = AbilityId.Oblivious,
                    Hidden = AbilityId.Adaptability,
                },
                HeightM = 0.6,
                WeightKg = 7.4,
                Color = "Brown",
            },
            [SpecieId.Milotic] = new()
            {
                Id = SpecieId.Milotic,
                Num = 350,
                Name = "Milotic",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 60,
                    Def = 79,
                    SpA = 100,
                    SpD = 125,
                    Spe = 81,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MarvelScale,
                    Slot1 = AbilityId.Competitive,
                    Hidden = AbilityId.CuteCharm,
                },
                HeightM = 6.2,
                WeightKg = 162,
                Color = "Pink",
                Prevo = SpecieId.Feebas,
            },
        };
    }
}