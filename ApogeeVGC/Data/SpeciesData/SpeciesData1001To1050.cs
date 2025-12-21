using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData1001To1050()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.WoChien] = new()
            {
                Id = SpecieId.WoChien,
                Num = 1001,
                Name = "Wo-Chien",
                Types = [PokemonType.Dark, PokemonType.Grass],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 85,
                    Def = 100,
                    SpA = 95,
                    SpD = 135,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.TabletsOfRuin,
                },
                HeightM = 1.5,
                WeightKg = 74.2,
                Color = "Brown",
                Tags = [SpeciesTag.SubLegendary],
            },
            [SpecieId.ChienPao] = new()
            {
                Id = SpecieId.ChienPao,
                Num = 1002,
                Name = "Chien-Pao",
                Types = [PokemonType.Dark, PokemonType.Ice],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 80,
                    SpA = 90,
                    SpD = 65,
                    Spe = 135,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SwordOfRuin,
                },
                HeightM = 1.9,
                WeightKg = 152.2,
                Color = "White",
                Tags = [SpeciesTag.SubLegendary],
            },
            [SpecieId.TingLu] = new()
            {
                Id = SpecieId.TingLu,
                Num = 1003,
                Name = "Ting-Lu",
                Types = [PokemonType.Dark, PokemonType.Ground],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 155,
                    Atk = 110,
                    Def = 125,
                    SpA = 55,
                    SpD = 80,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.VesselOfRuin,
                },
                HeightM = 2.7,
                WeightKg = 699.7,
                Color = "Brown",
                Tags = [SpeciesTag.SubLegendary],
            },
            [SpecieId.ChiYu] = new()
            {
                Id = SpecieId.ChiYu,
                Num = 1004,
                Name = "Chi-Yu",
                Types = [PokemonType.Dark, PokemonType.Fire],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 80,
                    Def = 80,
                    SpA = 135,
                    SpD = 120,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BeadsOfRuin,
                },
                HeightM = 0.4,
                WeightKg = 4.9,
                Color = "Red",
                Tags = [SpeciesTag.SubLegendary],
            },
            [SpecieId.RoaringMoon] = new()
            {
                Id = SpecieId.RoaringMoon,
                Num = 1005,
                Name = "Roaring Moon",
                Types = [PokemonType.Dragon, PokemonType.Dark],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 105,
                    Atk = 139,
                    Def = 71,
                    SpA = 55,
                    SpD = 101,
                    Spe = 119,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Protosynthesis,
                },
                HeightM = 2,
                WeightKg = 380,
                Color = "Blue",
                Tags = [SpeciesTag.Paradox],
            },
            [SpecieId.IronValiant] = new()
            {
                Id = SpecieId.IronValiant,
                Num = 1006,
                Name = "Iron Valiant",
                Types = [PokemonType.Fairy, PokemonType.Fighting],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 74,
                    Atk = 130,
                    Def = 90,
                    SpA = 120,
                    SpD = 60,
                    Spe = 116,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.QuarkDrive,
                },
                HeightM = 1.4,
                WeightKg = 35,
                Color = "White",
                Tags = [SpeciesTag.Paradox],
            },
            [SpecieId.Koraidon] = new()
            {
                Id = SpecieId.Koraidon,
                Num = 1007,
                Name = "Koraidon",
                Types = [PokemonType.Fighting, PokemonType.Dragon],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 135,
                    Def = 115,
                    SpA = 85,
                    SpD = 100,
                    Spe = 135,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.OrichalcumPulse,
                },
                HeightM = 2.5,
                WeightKg = 303,
                Color = "Red",
                Tags = [SpeciesTag.RestrictedLegendary],
            },
            [SpecieId.Miraidon] = new()
            {
                Id = SpecieId.Miraidon,
                Num = 1008,
                Name = "Miraidon",
                Types = [PokemonType.Electric, PokemonType.Dragon],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 85,
                    Def = 100,
                    SpA = 135,
                    SpD = 115,
                    Spe = 135,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HadronEngine,
                },
                HeightM = 3.5,
                WeightKg = 240,
                Color = "Purple",
                Tags = [SpeciesTag.RestrictedLegendary],
            },
            [SpecieId.WalkingWake] = new()
            {
                Id = SpecieId.WalkingWake,
                Num = 1009,
                Name = "Walking Wake",
                Types = [PokemonType.Water, PokemonType.Dragon],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 99,
                    Atk = 83,
                    Def = 91,
                    SpA = 125,
                    SpD = 83,
                    Spe = 109,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Protosynthesis,
                },
                HeightM = 3.5,
                WeightKg = 280,
                Color = "Blue",
                Tags = [SpeciesTag.Paradox],
            },
            [SpecieId.IronLeaves] = new()
            {
                Id = SpecieId.IronLeaves,
                Num = 1010,
                Name = "Iron Leaves",
                Types = [PokemonType.Grass, PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 130,
                    Def = 88,
                    SpA = 70,
                    SpD = 108,
                    Spe = 104,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.QuarkDrive,
                },
                HeightM = 1.5,
                WeightKg = 125,
                Color = "Green",
                Tags = [SpeciesTag.Paradox],
            },
            [SpecieId.Dipplin] = new()
            {
                Id = SpecieId.Dipplin,
                Num = 1011,
                Name = "Dipplin",
                Types = [PokemonType.Grass, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 80,
                    Def = 110,
                    SpA = 95,
                    SpD = 80,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SupersweetSyrup,
                    Slot1 = AbilityId.Gluttony,
                    Hidden = AbilityId.StickyHold,
                },
                HeightM = 0.4,
                WeightKg = 4.4,
                Color = "Green",
                Prevo = SpecieId.Applin,
                Evos = [SpecieId.Hydrapple],
            },
            [SpecieId.Poltchageist] = new()
            {
                Id = SpecieId.Poltchageist,
                Num = 1012,
                Name = "Poltchageist",
                BaseForme = FormeId.Counterfeit,
                Types = [PokemonType.Grass, PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 45,
                    Def = 45,
                    SpA = 74,
                    SpD = 54,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Hospitality,
                    Hidden = AbilityId.Heatproof,
                },
                HeightM = 0.1,
                WeightKg = 1.1,
                Color = "Green",
                Evos = [SpecieId.Sinistcha],
                OtherFormes = [FormeId.Artisan],
                FormeOrder = [FormeId.Counterfeit, FormeId.Artisan],
            },
            [SpecieId.PoltchageistArtisan] = new()
            {
                Id = SpecieId.PoltchageistArtisan,
                Num = 1012,
                Name = "Poltchageist-Artisan",
                BaseSpecies = SpecieId.Poltchageist,
                Forme = FormeId.Artisan,
                Types = [PokemonType.Grass, PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 45,
                    Def = 45,
                    SpA = 74,
                    SpD = 54,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Hospitality,
                    Hidden = AbilityId.Heatproof,
                },
                HeightM = 0.1,
                WeightKg = 1.1,
                Color = "Green",
                Evos = [SpecieId.SinistchaMasterpiece],
            },
            [SpecieId.Sinistcha] = new()
            {
                Id = SpecieId.Sinistcha,
                Num = 1013,
                Name = "Sinistcha",
                BaseForme = FormeId.Unremarkable,
                Types = [PokemonType.Grass, PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 71,
                    Atk = 60,
                    Def = 106,
                    SpA = 121,
                    SpD = 80,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Hospitality,
                    Hidden = AbilityId.Heatproof,
                },
                HeightM = 0.2,
                WeightKg = 2.2,
                Color = "Green",
                Prevo = SpecieId.Poltchageist,
                OtherFormes = [FormeId.Masterpiece],
                FormeOrder = [FormeId.Unremarkable, FormeId.Masterpiece],
            },
            [SpecieId.SinistchaMasterpiece] = new()
            {
                Id = SpecieId.SinistchaMasterpiece,
                Num = 1013,
                Name = "Sinistcha-Masterpiece",
                BaseSpecies = SpecieId.Sinistcha,
                Forme = FormeId.Masterpiece,
                Types = [PokemonType.Grass, PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 71,
                    Atk = 60,
                    Def = 106,
                    SpA = 121,
                    SpD = 80,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Hospitality,
                    Hidden = AbilityId.Heatproof,
                },
                HeightM = 0.2,
                WeightKg = 2.2,
                Color = "Green",
                Prevo = SpecieId.PoltchageistArtisan,
            },
            [SpecieId.Okidogi] = new()
            {
                Id = SpecieId.Okidogi,
                Num = 1014,
                Name = "Okidogi",
                Types = [PokemonType.Poison, PokemonType.Fighting],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 88,
                    Atk = 128,
                    Def = 115,
                    SpA = 58,
                    SpD = 86,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ToxicChain,
                    Hidden = AbilityId.GuardDog,
                },
                HeightM = 1.8,
                WeightKg = 92,
                Color = "Black",
                Tags = [SpeciesTag.SubLegendary],
            },
            [SpecieId.Munkidori] = new()
            {
                Id = SpecieId.Munkidori,
                Num = 1015,
                Name = "Munkidori",
                Types = [PokemonType.Poison, PokemonType.Psychic],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 88,
                    Atk = 75,
                    Def = 66,
                    SpA = 130,
                    SpD = 90,
                    Spe = 106,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ToxicChain,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 1,
                WeightKg = 12.2,
                Color = "Black",
                Tags = [SpeciesTag.SubLegendary],
            },
            [SpecieId.Fezandipiti] = new()
            {
                Id = SpecieId.Fezandipiti,
                Num = 1016,
                Name = "Fezandipiti",
                Types = [PokemonType.Poison, PokemonType.Fairy],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 88,
                    Atk = 91,
                    Def = 82,
                    SpA = 70,
                    SpD = 125,
                    Spe = 99,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ToxicChain,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 1.4,
                WeightKg = 30.1,
                Color = "Black",
                Tags = [SpeciesTag.SubLegendary],
            },
            [SpecieId.Ogerpon] = new()
            {
                Id = SpecieId.Ogerpon,
                Num = 1017,
                Name = "Ogerpon",
                BaseForme = FormeId.Teal,
                Types = [PokemonType.Grass],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 84,
                    SpA = 60,
                    SpD = 96,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Defiant,
                },
                HeightM = 1.2,
                WeightKg = 39.8,
                Color = "Green",
                Tags = [SpeciesTag.SubLegendary],
                OtherFormes = [FormeId.Wellspring, FormeId.Hearthflame, FormeId.Cornerstone, FormeId.TealTera, FormeId.WellspringTera, FormeId.HearthflameTera, FormeId.CornerstoneTera],
                FormeOrder = [FormeId.Teal, FormeId.Wellspring, FormeId.Hearthflame, FormeId.Cornerstone, FormeId.TealTera, FormeId.WellspringTera, FormeId.HearthflameTera, FormeId.CornerstoneTera],
            },
            [SpecieId.OgerponWellspring] = new()
            {
                Id = SpecieId.OgerponWellspring,
                Num = 1017,
                Name = "Ogerpon-Wellspring",
                BaseSpecies = SpecieId.Ogerpon,
                Forme = FormeId.Wellspring,
                Types = [PokemonType.Grass, PokemonType.Water],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 84,
                    SpA = 60,
                    SpD = 96,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterAbsorb,
                },
                HeightM = 1.2,
                WeightKg = 39.8,
                Color = "Blue",
                RequiredItem = ItemId.WellspringMask,
                ChangesFrom = FormeId.Teal,
            },
            [SpecieId.OgerponHearthflame] = new()
            {
                Id = SpecieId.OgerponHearthflame,
                Num = 1017,
                Name = "Ogerpon-Hearthflame",
                BaseSpecies = SpecieId.Ogerpon,
                Forme = FormeId.Hearthflame,
                Types = [PokemonType.Grass, PokemonType.Fire],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 84,
                    SpA = 60,
                    SpD = 96,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MoldBreaker,
                },
                HeightM = 1.2,
                WeightKg = 39.8,
                Color = "Red",
                RequiredItem = ItemId.HearthflameMask,
                ChangesFrom = FormeId.Teal,
            },
            [SpecieId.OgerponCornerstone] = new()
            {
                Id = SpecieId.OgerponCornerstone,
                Num = 1017,
                Name = "Ogerpon-Cornerstone",
                BaseSpecies = SpecieId.Ogerpon,
                Forme = FormeId.Cornerstone,
                Types = [PokemonType.Grass, PokemonType.Rock],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 84,
                    SpA = 60,
                    SpD = 96,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                },
                HeightM = 1.2,
                WeightKg = 39.8,
                Color = "Gray",
                RequiredItem = ItemId.CornerstoneMask,
                ChangesFrom = FormeId.Teal,
            },
            [SpecieId.OgerponTealTera] = new()
            {
                Id = SpecieId.OgerponTealTera,
                Num = 1017,
                Name = "Ogerpon-Teal-Tera",
                BaseSpecies = SpecieId.Ogerpon,
                Forme = FormeId.TealTera,
                Types = [PokemonType.Grass],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 84,
                    SpA = 60,
                    SpD = 96,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EmbodyAspectTeal,
                },
                HeightM = 1.2,
                WeightKg = 39.8,
                Color = "Green",
                BattleOnly = FormeId.Teal,
            },
            [SpecieId.OgerponWellspringTera] = new()
            {
                Id = SpecieId.OgerponWellspringTera,
                Num = 1017,
                Name = "Ogerpon-Wellspring-Tera",
                BaseSpecies = SpecieId.Ogerpon,
                Forme = FormeId.WellspringTera,
                Types = [PokemonType.Grass, PokemonType.Water],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 84,
                    SpA = 60,
                    SpD = 96,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EmbodyAspectWellspring,
                },
                HeightM = 1.2,
                WeightKg = 39.8,
                Color = "Blue",
                RequiredItem = ItemId.WellspringMask,
                BattleOnly = FormeId.Wellspring,
            },
            [SpecieId.OgerponHearthflameTera] = new()
            {
                Id = SpecieId.OgerponHearthflameTera,
                Num = 1017,
                Name = "Ogerpon-Hearthflame-Tera",
                BaseSpecies = SpecieId.Ogerpon,
                Forme = FormeId.HearthflameTera,
                Types = [PokemonType.Grass, PokemonType.Fire],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 84,
                    SpA = 60,
                    SpD = 96,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EmbodyAspectHearthflame,
                },
                HeightM = 1.2,
                WeightKg = 39.8,
                Color = "Red",
                RequiredItem = ItemId.HearthflameMask,
                BattleOnly = FormeId.Hearthflame,
            },
            [SpecieId.OgerponCornerstoneTera] = new()
            {
                Id = SpecieId.OgerponCornerstoneTera,
                Num = 1017,
                Name = "Ogerpon-Cornerstone-Tera",
                BaseSpecies = SpecieId.Ogerpon,
                Forme = FormeId.CornerstoneTera,
                Types = [PokemonType.Grass, PokemonType.Rock],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 84,
                    SpA = 60,
                    SpD = 96,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EmbodyAspectCornerstone,
                },
                HeightM = 1.2,
                WeightKg = 39.8,
                Color = "Gray",
                RequiredItem = ItemId.CornerstoneMask,
                BattleOnly = FormeId.Cornerstone,
            },
            [SpecieId.Archaludon] = new()
            {
                Id = SpecieId.Archaludon,
                Num = 1018,
                Name = "Archaludon",
                Types = [PokemonType.Steel, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 105,
                    Def = 130,
                    SpA = 125,
                    SpD = 65,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Stamina,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.Stalwart,
                },
                HeightM = 2,
                WeightKg = 60,
                Color = "White",
                Prevo = SpecieId.Duraludon,
            },
            [SpecieId.Hydrapple] = new()
            {
                Id = SpecieId.Hydrapple,
                Num = 1019,
                Name = "Hydrapple",
                Types = [PokemonType.Grass, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 106,
                    Atk = 80,
                    Def = 110,
                    SpA = 120,
                    SpD = 80,
                    Spe = 44,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SupersweetSyrup,
                    Slot1 = AbilityId.Regenerator,
                    Hidden = AbilityId.StickyHold,
                },
                HeightM = 1.8,
                WeightKg = 93,
                Color = "Green",
                Prevo = SpecieId.Dipplin,
            },
            [SpecieId.GougingFire] = new()
            {
                Id = SpecieId.GougingFire,
                Num = 1020,
                Name = "Gouging Fire",
                Types = [PokemonType.Fire, PokemonType.Dragon],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 105,
                    Atk = 115,
                    Def = 121,
                    SpA = 65,
                    SpD = 93,
                    Spe = 91,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Protosynthesis,
                },
                HeightM = 3.5,
                WeightKg = 590,
                Color = "Brown",
                Tags = [SpeciesTag.Paradox],
            },
            [SpecieId.RagingBolt] = new()
            {
                Id = SpecieId.RagingBolt,
                Num = 1021,
                Name = "Raging Bolt",
                Types = [PokemonType.Electric, PokemonType.Dragon],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 125,
                    Atk = 73,
                    Def = 91,
                    SpA = 137,
                    SpD = 89,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Protosynthesis,
                },
                HeightM = 5.2,
                WeightKg = 480,
                Color = "Yellow",
                Tags = [SpeciesTag.Paradox],
            },
            [SpecieId.IronBoulder] = new()
            {
                Id = SpecieId.IronBoulder,
                Num = 1022,
                Name = "Iron Boulder",
                Types = [PokemonType.Rock, PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 120,
                    Def = 80,
                    SpA = 68,
                    SpD = 108,
                    Spe = 124,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.QuarkDrive,
                },
                HeightM = 1.5,
                WeightKg = 162.5,
                Color = "Gray",
                Tags = [SpeciesTag.Paradox],
            },
            [SpecieId.IronCrown] = new()
            {
                Id = SpecieId.IronCrown,
                Num = 1023,
                Name = "Iron Crown",
                Types = [PokemonType.Steel, PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 72,
                    Def = 100,
                    SpA = 122,
                    SpD = 108,
                    Spe = 98,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.QuarkDrive,
                },
                HeightM = 1.6,
                WeightKg = 156,
                Color = "Blue",
                Tags = [SpeciesTag.Paradox],
            },
            [SpecieId.Terapagos] = new()
            {
                Id = SpecieId.Terapagos,
                Num = 1024,
                Name = "Terapagos",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 65,
                    Def = 85,
                    SpA = 65,
                    SpD = 85,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.TeraShift,
                },
                HeightM = 0.2,
                WeightKg = 6.5,
                Color = "Blue",
                Tags = [SpeciesTag.RestrictedLegendary],
                OtherFormes = [FormeId.Terastal, FormeId.Stellar],
                FormeOrder = [FormeId.None, FormeId.Terastal, FormeId.Stellar],
            },
            [SpecieId.TerapagosTerastal] = new()
            {
                Id = SpecieId.TerapagosTerastal,
                Num = 1024,
                Name = "Terapagos-Terastal",
                BaseSpecies = SpecieId.Terapagos,
                Forme = FormeId.Terastal,
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 110,
                    SpA = 105,
                    SpD = 110,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.TeraShell,
                },
                HeightM = 0.3,
                WeightKg = 16,
                Color = "Blue",
                BattleOnly = FormeId.None,
            },
            [SpecieId.TerapagosStellar] = new()
            {
                Id = SpecieId.TerapagosStellar,
                Num = 1024,
                Name = "Terapagos-Stellar",
                BaseSpecies = SpecieId.Terapagos,
                Forme = FormeId.Stellar,
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 160,
                    Atk = 105,
                    Def = 110,
                    SpA = 130,
                    SpD = 110,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.TeraformZero,
                },
                HeightM = 1.7,
                WeightKg = 77,
                Color = "Blue",
                BattleOnly = FormeId.None,
            },
            [SpecieId.Pecharunt] = new()
            {
                Id = SpecieId.Pecharunt,
                Num = 1025,
                Name = "Pecharunt",
                Types = [PokemonType.Poison, PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 88,
                    Atk = 88,
                    Def = 160,
                    SpA = 88,
                    SpD = 88,
                    Spe = 88,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPuppeteer,
                },
                HeightM = 0.3,
                WeightKg = 0.3,
                Color = "Purple",
                Tags = [SpeciesTag.Mythical],
            },
        };
    }
}
