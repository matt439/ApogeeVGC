using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData701To750()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Hawlucha] = new()
            {
                Id = SpecieId.Hawlucha,
                Num = 701,
                Name = "Hawlucha",
                Types = [PokemonType.Fighting, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 92,
                    Def = 75,
                    SpA = 74,
                    SpD = 63,
                    Spe = 118,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Limber,
                    Slot1 = AbilityId.Unburden,
                    Hidden = AbilityId.MoldBreaker,
                },
                HeightM = 0.8,
                WeightKg = 21.5,
                Color = "Green",
            },
            [SpecieId.HawluchaMega] = new()
            {
                Id = SpecieId.HawluchaMega,
                Num = 701,
                Name = "Hawlucha-Mega",
                BaseSpecies = SpecieId.Hawlucha,
                Forme = FormeId.Mega,
                Types = [PokemonType.Fighting, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 137,
                    Def = 100,
                    SpA = 74,
                    SpD = 93,
                    Spe = 118,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Limber,
                    Slot1 = AbilityId.Unburden,
                    Hidden = AbilityId.MoldBreaker,
                },
                HeightM = 1.0,
                WeightKg = 25,
                Color = "Green",
            },
            [SpecieId.Dedenne] = new()
            {
                Id = SpecieId.Dedenne,
                Num = 702,
                Name = "Dedenne",
                Types = [PokemonType.Electric, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 67,
                    Atk = 58,
                    Def = 57,
                    SpA = 81,
                    SpD = 67,
                    Spe = 101,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.CheekPouch,
                    Slot1 = AbilityId.Pickup,
                    Hidden = AbilityId.Plus,
                },
                HeightM = 0.2,
                WeightKg = 2.2,
                Color = "Yellow",
            },
            [SpecieId.Carbink] = new()
            {
                Id = SpecieId.Carbink,
                Num = 703,
                Name = "Carbink",
                Types = [PokemonType.Rock, PokemonType.Fairy],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 50,
                    Def = 150,
                    SpA = 50,
                    SpD = 150,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ClearBody,
                    Hidden = AbilityId.Sturdy,
                },
                HeightM = 0.3,
                WeightKg = 5.7,
                Color = "Gray",
            },
            [SpecieId.Goomy] = new()
            {
                Id = SpecieId.Goomy,
                Num = 704,
                Name = "Goomy",
                Types = [PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 50,
                    Def = 35,
                    SpA = 55,
                    SpD = 75,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SapSipper,
                    Slot1 = AbilityId.Hydration,
                    Hidden = AbilityId.Gooey,
                },
                HeightM = 0.3,
                WeightKg = 2.8,
                Color = "Purple",
            },
            [SpecieId.Sliggoo] = new()
            {
                Id = SpecieId.Sliggoo,
                Num = 705,
                Name = "Sliggoo",
                Types = [PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 75,
                    Def = 53,
                    SpA = 83,
                    SpD = 113,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SapSipper,
                    Slot1 = AbilityId.Hydration,
                    Hidden = AbilityId.Gooey,
                },
                HeightM = 0.8,
                WeightKg = 17.5,
                Color = "Purple",
            },
            [SpecieId.SliggooHisui] = new()
            {
                Id = SpecieId.SliggooHisui,
                Num = 705,
                Name = "Sliggoo-Hisui",
                BaseSpecies = SpecieId.Sliggoo,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Steel, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 58,
                    Atk = 75,
                    Def = 83,
                    SpA = 83,
                    SpD = 113,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SapSipper,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.Gooey,
                },
                HeightM = 0.7,
                WeightKg = 68.5,
                Color = "Purple",
            },
            [SpecieId.Goodra] = new()
            {
                Id = SpecieId.Goodra,
                Num = 706,
                Name = "Goodra",
                Types = [PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 100,
                    Def = 70,
                    SpA = 110,
                    SpD = 150,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SapSipper,
                    Slot1 = AbilityId.Hydration,
                    Hidden = AbilityId.Gooey,
                },
                HeightM = 2.0,
                WeightKg = 150.5,
                Color = "Purple",
            },
            [SpecieId.GoodraHisui] = new()
            {
                Id = SpecieId.GoodraHisui,
                Num = 706,
                Name = "Goodra-Hisui",
                BaseSpecies = SpecieId.Goodra,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Steel, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 100,
                    Def = 100,
                    SpA = 110,
                    SpD = 150,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SapSipper,
                    Slot1 = AbilityId.ShellArmor,
                    Hidden = AbilityId.Gooey,
                },
                HeightM = 1.7,
                WeightKg = 334.1,
                Color = "Purple",
            },
            [SpecieId.Klefki] = new()
            {
                Id = SpecieId.Klefki,
                Num = 707,
                Name = "Klefki",
                Types = [PokemonType.Steel, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 57,
                    Atk = 80,
                    Def = 91,
                    SpA = 80,
                    SpD = 87,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Hidden = AbilityId.Magician,
                },
                HeightM = 0.2,
                WeightKg = 3.0,
                Color = "Gray",
            },
            [SpecieId.Phantump] = new()
            {
                Id = SpecieId.Phantump,
                Num = 708,
                Name = "Phantump",
                Types = [PokemonType.Ghost, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 43,
                    Atk = 70,
                    Def = 48,
                    SpA = 50,
                    SpD = 60,
                    Spe = 38,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Harvest,
                },
                HeightM = 0.4,
                WeightKg = 7.0,
                Color = "Brown",
            },
            [SpecieId.Trevenant] = new()
            {
                Id = SpecieId.Trevenant,
                Num = 709,
                Name = "Trevenant",
                Types = [PokemonType.Ghost, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 110,
                    Def = 76,
                    SpA = 65,
                    SpD = 82,
                    Spe = 56,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Harvest,
                },
                HeightM = 1.5,
                WeightKg = 71.0,
                Color = "Brown",
            },
            [SpecieId.Pumpkaboo] = new()
            {
                Id = SpecieId.Pumpkaboo,
                Num = 710,
                Name = "Pumpkaboo",
                Types = [PokemonType.Ghost, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 49,
                    Atk = 66,
                    Def = 70,
                    SpA = 44,
                    SpD = 55,
                    Spe = 51,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Insomnia,
                },
                HeightM = 0.4,
                WeightKg = 5.0,
                Color = "Brown",
            },
            [SpecieId.PumpkabooSmall] = new()
            {
                Id = SpecieId.PumpkabooSmall,
                Num = 710,
                Name = "Pumpkaboo-Small",
                BaseSpecies = SpecieId.Pumpkaboo,
                Forme = FormeId.Small,
                Types = [PokemonType.Ghost, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 44,
                    Atk = 66,
                    Def = 70,
                    SpA = 44,
                    SpD = 55,
                    Spe = 56,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Insomnia,
                },
                HeightM = 0.3,
                WeightKg = 3.5,
                Color = "Brown",
            },
            [SpecieId.PumpkabooLarge] = new()
            {
                Id = SpecieId.PumpkabooLarge,
                Num = 710,
                Name = "Pumpkaboo-Large",
                BaseSpecies = SpecieId.Pumpkaboo,
                Forme = FormeId.Large,
                Types = [PokemonType.Ghost, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 54,
                    Atk = 66,
                    Def = 70,
                    SpA = 44,
                    SpD = 55,
                    Spe = 46,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Insomnia,
                },
                HeightM = 0.5,
                WeightKg = 7.5,
                Color = "Brown",
            },
            [SpecieId.PumpkabooSuper] = new()
            {
                Id = SpecieId.PumpkabooSuper,
                Num = 710,
                Name = "Pumpkaboo-Super",
                BaseSpecies = SpecieId.Pumpkaboo,
                Forme = FormeId.Super,
                Types = [PokemonType.Ghost, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 59,
                    Atk = 66,
                    Def = 70,
                    SpA = 44,
                    SpD = 55,
                    Spe = 41,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Insomnia,
                },
                HeightM = 0.8,
                WeightKg = 15.0,
                Color = "Brown",
            },
            [SpecieId.Gourgeist] = new()
            {
                Id = SpecieId.Gourgeist,
                Num = 711,
                Name = "Gourgeist",
                Types = [PokemonType.Ghost, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 90,
                    Def = 122,
                    SpA = 58,
                    SpD = 75,
                    Spe = 84,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Insomnia,
                },
                HeightM = 0.9,
                WeightKg = 12.5,
                Color = "Brown",
            },
            [SpecieId.GourgeistSmall] = new()
            {
                Id = SpecieId.GourgeistSmall,
                Num = 711,
                Name = "Gourgeist-Small",
                BaseSpecies = SpecieId.Gourgeist,
                Forme = FormeId.Small,
                Types = [PokemonType.Ghost, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 85,
                    Def = 122,
                    SpA = 58,
                    SpD = 75,
                    Spe = 99,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Insomnia,
                },
                HeightM = 0.7,
                WeightKg = 9.5,
                Color = "Brown",
            },
            [SpecieId.GourgeistLarge] = new()
            {
                Id = SpecieId.GourgeistLarge,
                Num = 711,
                Name = "Gourgeist-Large",
                BaseSpecies = SpecieId.Gourgeist,
                Forme = FormeId.Large,
                Types = [PokemonType.Ghost, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 95,
                    Def = 122,
                    SpA = 58,
                    SpD = 75,
                    Spe = 69,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Insomnia,
                },
                HeightM = 1.1,
                WeightKg = 14.0,
                Color = "Brown",
            },
            [SpecieId.GourgeistSuper] = new()
            {
                Id = SpecieId.GourgeistSuper,
                Num = 711,
                Name = "Gourgeist-Super",
                BaseSpecies = SpecieId.Gourgeist,
                Forme = FormeId.Super,
                Types = [PokemonType.Ghost, PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 100,
                    Def = 122,
                    SpA = 58,
                    SpD = 75,
                    Spe = 54,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pickup,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Insomnia,
                },
                HeightM = 1.7,
                WeightKg = 39.0,
                Color = "Brown",
            },
            [SpecieId.Bergmite] = new()
            {
                Id = SpecieId.Bergmite,
                Num = 712,
                Name = "Bergmite",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 69,
                    Def = 85,
                    SpA = 32,
                    SpD = 35,
                    Spe = 28,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.OwnTempo,
                    Slot1 = AbilityId.IceBody,
                    Hidden = AbilityId.Sturdy,
                },
                HeightM = 1.0,
                WeightKg = 99.5,
                Color = "Blue",
            },
            [SpecieId.Avalugg] = new()
            {
                Id = SpecieId.Avalugg,
                Num = 713,
                Name = "Avalugg",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 117,
                    Def = 184,
                    SpA = 44,
                    SpD = 46,
                    Spe = 28,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.OwnTempo,
                    Slot1 = AbilityId.IceBody,
                    Hidden = AbilityId.Sturdy,
                },
                HeightM = 2.0,
                WeightKg = 505.0,
                Color = "Blue",
            },
            [SpecieId.AvaluggHisui] = new()
            {
                Id = SpecieId.AvaluggHisui,
                Num = 713,
                Name = "Avalugg-Hisui",
                BaseSpecies = SpecieId.Avalugg,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Ice, PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 127,
                    Def = 184,
                    SpA = 34,
                    SpD = 36,
                    Spe = 38,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.StrongJaw,
                    Slot1 = AbilityId.IceBody,
                    Hidden = AbilityId.Sturdy,
                },
                HeightM = 1.4,
                WeightKg = 262.4,
                Color = "Blue",
            },
            [SpecieId.Noibat] = new()
            {
                Id = SpecieId.Noibat,
                Num = 714,
                Name = "Noibat",
                Types = [PokemonType.Flying, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 30,
                    Def = 35,
                    SpA = 45,
                    SpD = 40,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Frisk,
                    Slot1 = AbilityId.Infiltrator,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 0.5,
                WeightKg = 8.0,
                Color = "Purple",
            },
            [SpecieId.Noivern] = new()
            {
                Id = SpecieId.Noivern,
                Num = 715,
                Name = "Noivern",
                Types = [PokemonType.Flying, PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 70,
                    Def = 80,
                    SpA = 97,
                    SpD = 80,
                    Spe = 123,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Frisk,
                    Slot1 = AbilityId.Infiltrator,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 1.5,
                WeightKg = 85.0,
                Color = "Purple",
            },
            [SpecieId.Xerneas] = new()
            {
                Id = SpecieId.Xerneas,
                Num = 716,
                Name = "Xerneas",
                Types = [PokemonType.Fairy],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 126,
                    Atk = 131,
                    Def = 95,
                    SpA = 131,
                    SpD = 98,
                    Spe = 99,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FairyAura,
                },
                HeightM = 3.0,
                WeightKg = 215.0,
                Color = "Blue",
            },
            [SpecieId.XerneasNeutral] = new()
            {
                Id = SpecieId.XerneasNeutral,
                Num = 716,
                Name = "Xerneas-Neutral",
                BaseSpecies = SpecieId.Xerneas,
                Forme = FormeId.Neutral,
                Types = [PokemonType.Fairy],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 126,
                    Atk = 131,
                    Def = 95,
                    SpA = 131,
                    SpD = 98,
                    Spe = 99,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FairyAura,
                },
                HeightM = 3.0,
                WeightKg = 215.0,
                Color = "Blue",
            },
            [SpecieId.XerneasActive] = new()
            {
                Id = SpecieId.XerneasActive,
                Num = 716,
                Name = "Xerneas-Active",
                BaseSpecies = SpecieId.Xerneas,
                Forme = FormeId.Active,
                Types = [PokemonType.Fairy],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 126,
                    Atk = 131,
                    Def = 95,
                    SpA = 131,
                    SpD = 98,
                    Spe = 99,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FairyAura,
                },
                HeightM = 3.0,
                WeightKg = 215.0,
                Color = "Blue",
            },
            [SpecieId.Yveltal] = new()
            {
                Id = SpecieId.Yveltal,
                Num = 717,
                Name = "Yveltal",
                Types = [PokemonType.Dark, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 126,
                    Atk = 131,
                    Def = 95,
                    SpA = 131,
                    SpD = 98,
                    Spe = 99,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.DarkAura,
                },
                HeightM = 5.8,
                WeightKg = 203.0,
                Color = "Red",
            },
            [SpecieId.Zygarde] = new()
            {
                Id = SpecieId.Zygarde,
                Num = 718,
                Name = "Zygarde",
                Types = [PokemonType.Dragon, PokemonType.Ground],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 108,
                    Atk = 100,
                    Def = 121,
                    SpA = 81,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.AuraBreak,
                },
                HeightM = 5.0,
                WeightKg = 305.0,
                Color = "Green",
            },
            [SpecieId.Zygarde10] = new()
            {
                Id = SpecieId.Zygarde10,
                Num = 718,
                Name = "Zygarde-10%",
                BaseSpecies = SpecieId.Zygarde,
                Forme = FormeId.TenPercent,
                Types = [PokemonType.Dragon, PokemonType.Ground],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 54,
                    Atk = 100,
                    Def = 71,
                    SpA = 61,
                    SpD = 85,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.AuraBreak,
                },
                HeightM = 1.2,
                WeightKg = 33.5,
                Color = "Black",
            },
            [SpecieId.ZygardeComplete] = new()
            {
                Id = SpecieId.ZygardeComplete,
                Num = 718,
                Name = "Zygarde-Complete",
                BaseSpecies = SpecieId.Zygarde,
                Forme = FormeId.Complete,
                Types = [PokemonType.Dragon, PokemonType.Ground],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 216,
                    Atk = 100,
                    Def = 121,
                    SpA = 91,
                    SpD = 95,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PowerConstruct,
                },
                HeightM = 4.5,
                WeightKg = 610.0,
                Color = "Black",
            },
            [SpecieId.ZygardeMega] = new()
            {
                Id = SpecieId.ZygardeMega,
                Num = 718,
                Name = "Zygarde-Mega",
                BaseSpecies = SpecieId.Zygarde,
                Forme = FormeId.Mega,
                Types = [PokemonType.Dragon, PokemonType.Ground],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 216,
                    Atk = 70,
                    Def = 91,
                    SpA = 216,
                    SpD = 85,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.AuraBreak,
                },
                HeightM = 7.7,
                WeightKg = 610.0,
                Color = "Green",
            },
            [SpecieId.Diancie] = new()
            {
                Id = SpecieId.Diancie,
                Num = 719,
                Name = "Diancie",
                Types = [PokemonType.Rock, PokemonType.Fairy],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 100,
                    Def = 150,
                    SpA = 100,
                    SpD = 150,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ClearBody,
                },
                HeightM = 0.7,
                WeightKg = 8.8,
                Color = "Pink",
            },
            [SpecieId.DiancieMega] = new()
            {
                Id = SpecieId.DiancieMega,
                Num = 719,
                Name = "Diancie-Mega",
                BaseSpecies = SpecieId.Diancie,
                Forme = FormeId.Mega,
                Types = [PokemonType.Rock, PokemonType.Fairy],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 160,
                    Def = 110,
                    SpA = 160,
                    SpD = 110,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagicBounce,
                },
                HeightM = 1.1,
                WeightKg = 27.8,
                Color = "Pink",
            },
            [SpecieId.Hoopa] = new()
            {
                Id = SpecieId.Hoopa,
                Num = 720,
                Name = "Hoopa",
                Types = [PokemonType.Psychic, PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 110,
                    Def = 60,
                    SpA = 150,
                    SpD = 130,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Magician,
                },
                HeightM = 0.5,
                WeightKg = 9.0,
                Color = "Purple",
            },
            [SpecieId.HoopaUnbound] = new()
            {
                Id = SpecieId.HoopaUnbound,
                Num = 720,
                Name = "Hoopa-Unbound",
                BaseSpecies = SpecieId.Hoopa,
                Forme = FormeId.Unbound,
                Types = [PokemonType.Psychic, PokemonType.Dark],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 160,
                    Def = 60,
                    SpA = 170,
                    SpD = 130,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Magician,
                },
                HeightM = 6.5,
                WeightKg = 490.0,
                Color = "Purple",
            },
            [SpecieId.Volcanion] = new()
            {
                Id = SpecieId.Volcanion,
                Num = 721,
                Name = "Volcanion",
                Types = [PokemonType.Fire, PokemonType.Water],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 110,
                    Def = 120,
                    SpA = 130,
                    SpD = 90,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterAbsorb,
                },
                HeightM = 1.7,
                WeightKg = 195.0,
                Color = "Brown",
            },
            [SpecieId.Rowlet] = new()
            {
                Id = SpecieId.Rowlet,
                Num = 722,
                Name = "Rowlet",
                Types = [PokemonType.Grass, PokemonType.Flying],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 55,
                    Def = 55,
                    SpA = 50,
                    SpD = 50,
                    Spe = 42,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LongReach,
                },
                HeightM = 0.3,
                WeightKg = 1.5,
                Color = "Brown",
            },
            [SpecieId.Dartrix] = new()
            {
                Id = SpecieId.Dartrix,
                Num = 723,
                Name = "Dartrix",
                Types = [PokemonType.Grass, PokemonType.Flying],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 75,
                    Def = 75,
                    SpA = 70,
                    SpD = 70,
                    Spe = 52,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LongReach,
                },
                HeightM = 0.7,
                WeightKg = 16.0,
                Color = "Brown",
            },
            [SpecieId.Decidueye] = new()
            {
                Id = SpecieId.Decidueye,
                Num = 724,
                Name = "Decidueye",
                Types = [PokemonType.Grass, PokemonType.Ghost],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 78,
                    Atk = 107,
                    Def = 75,
                    SpA = 100,
                    SpD = 100,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.LongReach,
                },
                HeightM = 1.6,
                WeightKg = 36.6,
                Color = "Brown",
            },
            [SpecieId.DecidueyeHisui] = new()
            {
                Id = SpecieId.DecidueyeHisui,
                Num = 724,
                Name = "Decidueye-Hisui",
                BaseSpecies = SpecieId.Decidueye,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Grass, PokemonType.Fighting],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 88,
                    Atk = 112,
                    Def = 80,
                    SpA = 95,
                    SpD = 95,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Overgrow,
                    Hidden = AbilityId.Scrappy,
                },
                HeightM = 1.6,
                WeightKg = 37.0,
                Color = "Brown",
            },
            [SpecieId.Litten] = new()
            {
                Id = SpecieId.Litten,
                Num = 725,
                Name = "Litten",
                Types = [PokemonType.Fire],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 65,
                    Def = 40,
                    SpA = 60,
                    SpD = 40,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Intimidate,
                },
                HeightM = 0.4,
                WeightKg = 4.3,
                Color = "Red",
            },
            [SpecieId.Torracat] = new()
            {
                Id = SpecieId.Torracat,
                Num = 726,
                Name = "Torracat",
                Types = [PokemonType.Fire],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 85,
                    Def = 50,
                    SpA = 80,
                    SpD = 50,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Intimidate,
                },
                HeightM = 0.7,
                WeightKg = 25.0,
                Color = "Red",
            },
            [SpecieId.Incineroar] = new()
            {
                Id = SpecieId.Incineroar,
                Num = 727,
                Name = "Incineroar",
                Types = [PokemonType.Fire, PokemonType.Dark],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 115,
                    Def = 90,
                    SpA = 80,
                    SpD = 90,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Blaze,
                    Hidden = AbilityId.Intimidate,
                },
                HeightM = 1.8,
                WeightKg = 83.0,
                Color = "Red",
            },
            [SpecieId.Popplio] = new()
            {
                Id = SpecieId.Popplio,
                Num = 728,
                Name = "Popplio",
                Types = [PokemonType.Water],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 54,
                    Def = 54,
                    SpA = 66,
                    SpD = 56,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.LiquidVoice,
                },
                HeightM = 0.4,
                WeightKg = 7.5,
                Color = "Blue",
            },
            [SpecieId.Brionne] = new()
            {
                Id = SpecieId.Brionne,
                Num = 729,
                Name = "Brionne",
                Types = [PokemonType.Water],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 69,
                    Def = 69,
                    SpA = 91,
                    SpD = 81,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.LiquidVoice,
                },
                HeightM = 0.6,
                WeightKg = 17.5,
                Color = "Blue",
            },
            [SpecieId.Primarina] = new()
            {
                Id = SpecieId.Primarina,
                Num = 730,
                Name = "Primarina",
                Types = [PokemonType.Water, PokemonType.Fairy],
                Gender = GenderId.M875F125,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 74,
                    Def = 74,
                    SpA = 126,
                    SpD = 116,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Torrent,
                    Hidden = AbilityId.LiquidVoice,
                },
                HeightM = 1.8,
                WeightKg = 44.0,
                Color = "Blue",
            },
            [SpecieId.Pikipek] = new()
            {
                Id = SpecieId.Pikipek,
                Num = 731,
                Name = "Pikipek",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 75,
                    Def = 30,
                    SpA = 30,
                    SpD = 30,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.SkillLink,
                    Hidden = AbilityId.Pickup,
                },
                HeightM = 0.3,
                WeightKg = 1.2,
                Color = "Black",
            },
            [SpecieId.Trumbeak] = new()
            {
                Id = SpecieId.Trumbeak,
                Num = 732,
                Name = "Trumbeak",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 85,
                    Def = 50,
                    SpA = 40,
                    SpD = 50,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.SkillLink,
                    Hidden = AbilityId.Pickup,
                },
                HeightM = 0.6,
                WeightKg = 14.8,
                Color = "Black",
            },
            [SpecieId.Toucannon] = new()
            {
                Id = SpecieId.Toucannon,
                Num = 733,
                Name = "Toucannon",
                Types = [PokemonType.Normal, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 75,
                    SpA = 75,
                    SpD = 75,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.SkillLink,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 1.1,
                WeightKg = 26.0,
                Color = "Black",
            },
            [SpecieId.Yungoos] = new()
            {
                Id = SpecieId.Yungoos,
                Num = 734,
                Name = "Yungoos",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 48,
                    Atk = 70,
                    Def = 30,
                    SpA = 30,
                    SpD = 30,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Stakeout,
                    Slot1 = AbilityId.StrongJaw,
                    Hidden = AbilityId.Adaptability,
                },
                HeightM = 0.4,
                WeightKg = 6.0,
                Color = "Brown",
            },
            [SpecieId.Gumshoos] = new()
            {
                Id = SpecieId.Gumshoos,
                Num = 735,
                Name = "Gumshoos",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 88,
                    Atk = 110,
                    Def = 60,
                    SpA = 55,
                    SpD = 60,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Stakeout,
                    Slot1 = AbilityId.StrongJaw,
                    Hidden = AbilityId.Adaptability,
                },
                HeightM = 0.7,
                WeightKg = 14.2,
                Color = "Brown",
            },
            [SpecieId.GumshoosTotem] = new()
            {
                Id = SpecieId.GumshoosTotem,
                Num = 735,
                Name = "Gumshoos-Totem",
                BaseSpecies = SpecieId.Gumshoos,
                Forme = FormeId.Totem,
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 88,
                    Atk = 110,
                    Def = 60,
                    SpA = 55,
                    SpD = 60,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Adaptability,
                },
                HeightM = 1.4,
                WeightKg = 60.0,
                Color = "Brown",
            },
            [SpecieId.Grubbin] = new()
            {
                Id = SpecieId.Grubbin,
                Num = 736,
                Name = "Grubbin",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 47,
                    Atk = 62,
                    Def = 45,
                    SpA = 55,
                    SpD = 45,
                    Spe = 46,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                },
                HeightM = 0.4,
                WeightKg = 4.4,
                Color = "Gray",
            },
            [SpecieId.Charjabug] = new()
            {
                Id = SpecieId.Charjabug,
                Num = 737,
                Name = "Charjabug",
                Types = [PokemonType.Bug, PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 57,
                    Atk = 82,
                    Def = 95,
                    SpA = 55,
                    SpD = 75,
                    Spe = 36,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Battery,
                },
                HeightM = 0.5,
                WeightKg = 10.5,
                Color = "Green",
            },
            [SpecieId.Vikavolt] = new()
            {
                Id = SpecieId.Vikavolt,
                Num = 738,
                Name = "Vikavolt",
                Types = [PokemonType.Bug, PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 77,
                    Atk = 70,
                    Def = 90,
                    SpA = 145,
                    SpD = 75,
                    Spe = 43,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 1.5,
                WeightKg = 45.0,
                Color = "Blue",
            },
            [SpecieId.VikavoltTotem] = new()
            {
                Id = SpecieId.VikavoltTotem,
                Num = 738,
                Name = "Vikavolt-Totem",
                BaseSpecies = SpecieId.Vikavolt,
                Forme = FormeId.Totem,
                Types = [PokemonType.Bug, PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 77,
                    Atk = 70,
                    Def = 90,
                    SpA = 145,
                    SpD = 75,
                    Spe = 43,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 2.6,
                WeightKg = 147.5,
                Color = "Blue",
            },
            [SpecieId.Crabrawler] = new()
            {
                Id = SpecieId.Crabrawler,
                Num = 739,
                Name = "Crabrawler",
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 47,
                    Atk = 82,
                    Def = 57,
                    SpA = 42,
                    SpD = 47,
                    Spe = 63,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.IronFist,
                    Hidden = AbilityId.AngerPoint,
                },
                HeightM = 0.6,
                WeightKg = 7.0,
                Color = "Purple",
            },
            [SpecieId.Crabominable] = new()
            {
                Id = SpecieId.Crabominable,
                Num = 740,
                Name = "Crabominable",
                Types = [PokemonType.Fighting, PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 97,
                    Atk = 132,
                    Def = 77,
                    SpA = 62,
                    SpD = 67,
                    Spe = 43,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.IronFist,
                    Hidden = AbilityId.AngerPoint,
                },
                HeightM = 1.7,
                WeightKg = 180.0,
                Color = "White",
            },
            [SpecieId.Oricorio] = new()
            {
                Id = SpecieId.Oricorio,
                Num = 741,
                Name = "Oricorio",
                Types = [PokemonType.Fire, PokemonType.Flying],
                Gender = GenderId.M25F75,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 70,
                    Def = 70,
                    SpA = 98,
                    SpD = 70,
                    Spe = 93,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Dancer,
                },
                HeightM = 0.6,
                WeightKg = 3.4,
                Color = "Red",
            },
            [SpecieId.OricorioPomPom] = new()
            {
                Id = SpecieId.OricorioPomPom,
                Num = 741,
                Name = "Oricorio-Pom-Pom",
                BaseSpecies = SpecieId.Oricorio,
                Forme = FormeId.PomPom,
                Types = [PokemonType.Electric, PokemonType.Flying],
                Gender = GenderId.M25F75,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 70,
                    Def = 70,
                    SpA = 98,
                    SpD = 70,
                    Spe = 93,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Dancer,
                },
                HeightM = 0.6,
                WeightKg = 3.4,
                Color = "Yellow",
            },
            [SpecieId.OricorioPau] = new()
            {
                Id = SpecieId.OricorioPau,
                Num = 741,
                Name = "Oricorio-Pa'u",
                BaseSpecies = SpecieId.Oricorio,
                Forme = FormeId.Pau,
                Types = [PokemonType.Psychic, PokemonType.Flying],
                Gender = GenderId.M25F75,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 70,
                    Def = 70,
                    SpA = 98,
                    SpD = 70,
                    Spe = 93,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Dancer,
                },
                HeightM = 0.6,
                WeightKg = 3.4,
                Color = "Pink",
            },
            [SpecieId.OricorioSensu] = new()
            {
                Id = SpecieId.OricorioSensu,
                Num = 741,
                Name = "Oricorio-Sensu",
                BaseSpecies = SpecieId.Oricorio,
                Forme = FormeId.Sensu,
                Types = [PokemonType.Ghost, PokemonType.Flying],
                Gender = GenderId.M25F75,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 70,
                    Def = 70,
                    SpA = 98,
                    SpD = 70,
                    Spe = 93,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Dancer,
                },
                HeightM = 0.6,
                WeightKg = 3.4,
                Color = "Purple",
            },
            [SpecieId.Cutiefly] = new()
            {
                Id = SpecieId.Cutiefly,
                Num = 742,
                Name = "Cutiefly",
                Types = [PokemonType.Bug, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 45,
                    Def = 40,
                    SpA = 55,
                    SpD = 40,
                    Spe = 84,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HoneyGather,
                    Slot1 = AbilityId.ShieldDust,
                    Hidden = AbilityId.SweetVeil,
                },
                HeightM = 0.1,
                WeightKg = 0.2,
                Color = "Yellow",
            },
            [SpecieId.Ribombee] = new()
            {
                Id = SpecieId.Ribombee,
                Num = 743,
                Name = "Ribombee",
                Types = [PokemonType.Bug, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 55,
                    Def = 60,
                    SpA = 95,
                    SpD = 70,
                    Spe = 124,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HoneyGather,
                    Slot1 = AbilityId.ShieldDust,
                    Hidden = AbilityId.SweetVeil,
                },
                HeightM = 0.2,
                WeightKg = 0.5,
                Color = "Yellow",
            },
            [SpecieId.RibombeeTotem] = new()
            {
                Id = SpecieId.RibombeeTotem,
                Num = 743,
                Name = "Ribombee-Totem",
                BaseSpecies = SpecieId.Ribombee,
                Forme = FormeId.Totem,
                Types = [PokemonType.Bug, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 55,
                    Def = 60,
                    SpA = 95,
                    SpD = 70,
                    Spe = 124,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SweetVeil,
                },
                HeightM = 0.4,
                WeightKg = 2.0,
                Color = "Yellow",
            },
            [SpecieId.Rockruff] = new()
            {
                Id = SpecieId.Rockruff,
                Num = 744,
                Name = "Rockruff",
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 65,
                    Def = 40,
                    SpA = 30,
                    SpD = 40,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.VitalSpirit,
                    Hidden = AbilityId.Steadfast,
                },
                HeightM = 0.5,
                WeightKg = 9.2,
                Color = "Brown",
            },
            [SpecieId.RockruffDusk] = new()
            {
                Id = SpecieId.RockruffDusk,
                Num = 744,
                Name = "Rockruff-Dusk",
                BaseSpecies = SpecieId.Rockruff,
                Forme = FormeId.Dusk,
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 65,
                    Def = 40,
                    SpA = 30,
                    SpD = 40,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.OwnTempo,
                },
                HeightM = 0.5,
                WeightKg = 9.2,
                Color = "Brown",
            },
            [SpecieId.Lycanroc] = new()
            {
                Id = SpecieId.Lycanroc,
                Num = 745,
                Name = "Lycanroc",
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 115,
                    Def = 65,
                    SpA = 55,
                    SpD = 65,
                    Spe = 112,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.SandRush,
                    Hidden = AbilityId.Steadfast,
                },
                HeightM = 0.8,
                WeightKg = 25.0,
                Color = "Brown",
            },
            [SpecieId.LycanrocMidnight] = new()
            {
                Id = SpecieId.LycanrocMidnight,
                Num = 745,
                Name = "Lycanroc-Midnight",
                BaseSpecies = SpecieId.Lycanroc,
                Forme = FormeId.Midnight,
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 115,
                    Def = 75,
                    SpA = 55,
                    SpD = 75,
                    Spe = 82,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.KeenEye,
                    Slot1 = AbilityId.VitalSpirit,
                    Hidden = AbilityId.NoGuard,
                },
                HeightM = 1.1,
                WeightKg = 25.0,
                Color = "Red",
            },
            [SpecieId.LycanrocDusk] = new()
            {
                Id = SpecieId.LycanrocDusk,
                Num = 745,
                Name = "Lycanroc-Dusk",
                BaseSpecies = SpecieId.Lycanroc,
                Forme = FormeId.Dusk,
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 117,
                    Def = 65,
                    SpA = 55,
                    SpD = 65,
                    Spe = 110,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ToughClaws,
                },
                HeightM = 0.8,
                WeightKg = 25.0,
                Color = "Brown",
            },
            [SpecieId.Wishiwashi] = new()
            {
                Id = SpecieId.Wishiwashi,
                Num = 746,
                Name = "Wishiwashi",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 20,
                    Def = 20,
                    SpA = 25,
                    SpD = 25,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Schooling,
                },
                HeightM = 0.2,
                WeightKg = 0.3,
                Color = "Blue",
            },
            [SpecieId.WishiwashiSchool] = new()
            {
                Id = SpecieId.WishiwashiSchool,
                Num = 746,
                Name = "Wishiwashi-School",
                BaseSpecies = SpecieId.Wishiwashi,
                Forme = FormeId.School,
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 140,
                    Def = 130,
                    SpA = 140,
                    SpD = 135,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Schooling,
                },
                HeightM = 8.2,
                WeightKg = 78.6,
                Color = "Blue",
            },
            [SpecieId.Mareanie] = new()
            {
                Id = SpecieId.Mareanie,
                Num = 747,
                Name = "Mareanie",
                Types = [PokemonType.Poison, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 53,
                    Def = 62,
                    SpA = 43,
                    SpD = 52,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Merciless,
                    Slot1 = AbilityId.Limber,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 0.4,
                WeightKg = 8.0,
                Color = "Blue",
            },
            [SpecieId.Toxapex] = new()
            {
                Id = SpecieId.Toxapex,
                Num = 748,
                Name = "Toxapex",
                Types = [PokemonType.Poison, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 63,
                    Def = 152,
                    SpA = 53,
                    SpD = 142,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Merciless,
                    Slot1 = AbilityId.Limber,
                    Hidden = AbilityId.Regenerator,
                },
                HeightM = 0.7,
                WeightKg = 14.5,
                Color = "Blue",
            },
            [SpecieId.Mudbray] = new()
            {
                Id = SpecieId.Mudbray,
                Num = 749,
                Name = "Mudbray",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 100,
                    Def = 70,
                    SpA = 45,
                    SpD = 55,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.OwnTempo,
                    Slot1 = AbilityId.Stamina,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 1.0,
                WeightKg = 110.0,
                Color = "Brown",
            },
            [SpecieId.Mudsdale] = new()
            {
                Id = SpecieId.Mudsdale,
                Num = 750,
                Name = "Mudsdale",
                Types = [PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 125,
                    Def = 100,
                    SpA = 55,
                    SpD = 85,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.OwnTempo,
                    Slot1 = AbilityId.Stamina,
                    Hidden = AbilityId.InnerFocus,
                },
                HeightM = 2.5,
                WeightKg = 920.0,
                Color = "Brown",
            },
        };
    }
}