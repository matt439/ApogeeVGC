using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData751To800()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Dewpider] = new()
            {
                Id = SpecieId.Dewpider,
                Num = 751,
                Name = "Dewpider",
                Types = [PokemonType.Water, PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 38,
                    Atk = 40,
                    Def = 52,
                    SpA = 40,
                    SpD = 72,
                    Spe = 27,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterBubble,
                    Hidden = AbilityId.WaterAbsorb,
                },
                HeightM = 0.3,
                WeightKg = 4,
                Color = "Green",
            },
            [SpecieId.Araquanid] = new()
            {
                Id = SpecieId.Araquanid,
                Num = 752,
                Name = "Araquanid",
                Types = [PokemonType.Water, PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 70,
                    Def = 92,
                    SpA = 50,
                    SpD = 132,
                    Spe = 42,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterBubble,
                    Hidden = AbilityId.WaterAbsorb,
                },
                HeightM = 1.8,
                WeightKg = 82,
                Color = "Green",
            },
            [SpecieId.AraquanidTotem] = new()
            {
                Id = SpecieId.AraquanidTotem,
                Num = 752,
                Name = "Araquanid-Totem",
                BaseSpecies = SpecieId.Araquanid,
                Forme = FormeId.Totem,
                Types = [PokemonType.Water, PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 70,
                    Def = 92,
                    SpA = 50,
                    SpD = 132,
                    Spe = 42,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterBubble,
                },
                HeightM = 3.1,
                WeightKg = 217.5,
                Color = "Green",
            },
            [SpecieId.Fomantis] = new()
            {
                Id = SpecieId.Fomantis,
                Num = 753,
                Name = "Fomantis",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 55,
                    Def = 35,
                    SpA = 50,
                    SpD = 35,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LeafGuard,
                    Hidden = AbilityId.Contrary,
                },
                HeightM = 0.3,
                WeightKg = 1.5,
                Color = "Pink",
            },
            [SpecieId.Lurantis] = new()
            {
                Id = SpecieId.Lurantis,
                Num = 754,
                Name = "Lurantis",
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 105,
                    Def = 90,
                    SpA = 80,
                    SpD = 90,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LeafGuard,
                    Hidden = AbilityId.Contrary,
                },
                HeightM = 0.9,
                WeightKg = 18.5,
                Color = "Pink",
            },
            [SpecieId.LurantisTotem] = new()
            {
                Id = SpecieId.LurantisTotem,
                Num = 754,
                Name = "Lurantis-Totem",
                BaseSpecies = SpecieId.Lurantis,
                Forme = FormeId.Totem,
                Types = [PokemonType.Grass],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 105,
                    Def = 90,
                    SpA = 80,
                    SpD = 90,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LeafGuard,
                },
                HeightM = 1.5,
                WeightKg = 58,
                Color = "Pink",
            },
            [SpecieId.Morelull] = new()
            {
                Id = SpecieId.Morelull,
                Num = 755,
                Name = "Morelull",
                Types = [PokemonType.Grass, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 35,
                    Def = 55,
                    SpA = 65,
                    SpD = 75,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Illuminate,
                    Slot1 = AbilityId.EffectSpore,
                    Hidden = AbilityId.RainDish,
                },
                HeightM = 0.2,
                WeightKg = 1.5,
                Color = "Purple",
            },
            [SpecieId.Shiinotic] = new()
            {
                Id = SpecieId.Shiinotic,
                Num = 756,
                Name = "Shiinotic",
                Types = [PokemonType.Grass, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 45,
                    Def = 80,
                    SpA = 90,
                    SpD = 100,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Illuminate,
                    Slot1 = AbilityId.EffectSpore,
                    Hidden = AbilityId.RainDish,
                },
                HeightM = 1.0,
                WeightKg = 11.5,
                Color = "Purple",
            },
            [SpecieId.Salandit] = new()
            {
                Id = SpecieId.Salandit,
                Num = 757,
                Name = "Salandit",
                Types = [PokemonType.Poison, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 48,
                    Atk = 44,
                    Def = 40,
                    SpA = 71,
                    SpD = 40,
                    Spe = 77,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Corrosion,
                    Hidden = AbilityId.Oblivious,
                },
                HeightM = 0.6,
                WeightKg = 4.8,
                Color = "Black",
            },
            [SpecieId.Salazzle] = new()
            {
                Id = SpecieId.Salazzle,
                Num = 758,
                Name = "Salazzle",
                Types = [PokemonType.Poison, PokemonType.Fire],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 64,
                    Def = 60,
                    SpA = 111,
                    SpD = 60,
                    Spe = 117,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Corrosion,
                    Hidden = AbilityId.Oblivious,
                },
                HeightM = 1.2,
                WeightKg = 22.2,
                Color = "Black",
            },
            [SpecieId.SalazzleTotem] = new()
            {
                Id = SpecieId.SalazzleTotem,
                Num = 758,
                Name = "Salazzle-Totem",
                BaseSpecies = SpecieId.Salazzle,
                Forme = FormeId.Totem,
                Types = [PokemonType.Poison, PokemonType.Fire],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 68,
                    Atk = 64,
                    Def = 60,
                    SpA = 111,
                    SpD = 60,
                    Spe = 117,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Corrosion,
                },
                HeightM = 2.1,
                WeightKg = 81,
                Color = "Black",
            },
            [SpecieId.Stufful] = new()
            {
                Id = SpecieId.Stufful,
                Num = 759,
                Name = "Stufful",
                Types = [PokemonType.Normal, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 75,
                    Def = 50,
                    SpA = 45,
                    SpD = 50,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Fluffy,
                    Slot1 = AbilityId.Klutz,
                    Hidden = AbilityId.CuteCharm,
                },
                HeightM = 0.5,
                WeightKg = 6.8,
                Color = "Pink",
            },
            [SpecieId.Bewear] = new()
            {
                Id = SpecieId.Bewear,
                Num = 760,
                Name = "Bewear",
                Types = [PokemonType.Normal, PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 120,
                    Atk = 125,
                    Def = 80,
                    SpA = 55,
                    SpD = 60,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Fluffy,
                    Slot1 = AbilityId.Klutz,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 2.1,
                WeightKg = 135,
                Color = "Pink",
            },
            [SpecieId.Bounsweet] = new()
            {
                Id = SpecieId.Bounsweet,
                Num = 761,
                Name = "Bounsweet",
                Types = [PokemonType.Grass],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 42,
                    Atk = 30,
                    Def = 38,
                    SpA = 30,
                    SpD = 38,
                    Spe = 32,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LeafGuard,
                    Slot1 = AbilityId.Oblivious,
                    Hidden = AbilityId.SweetVeil,
                },
                HeightM = 0.3,
                WeightKg = 3.2,
                Color = "Purple",
            },
            [SpecieId.Steenee] = new()
            {
                Id = SpecieId.Steenee,
                Num = 762,
                Name = "Steenee",
                Types = [PokemonType.Grass],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 52,
                    Atk = 40,
                    Def = 48,
                    SpA = 40,
                    SpD = 48,
                    Spe = 62,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LeafGuard,
                    Slot1 = AbilityId.Oblivious,
                    Hidden = AbilityId.SweetVeil,
                },
                HeightM = 0.7,
                WeightKg = 8.2,
                Color = "Purple",
            },
            [SpecieId.Tsareena] = new()
            {
                Id = SpecieId.Tsareena,
                Num = 763,
                Name = "Tsareena",
                Types = [PokemonType.Grass],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 72,
                    Atk = 120,
                    Def = 98,
                    SpA = 50,
                    SpD = 98,
                    Spe = 72,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LeafGuard,
                    Slot1 = AbilityId.QueenlyMajesty,
                    Hidden = AbilityId.SweetVeil,
                },
                HeightM = 1.2,
                WeightKg = 21.4,
                Color = "Purple",
            },
            [SpecieId.Comfey] = new()
            {
                Id = SpecieId.Comfey,
                Num = 764,
                Name = "Comfey",
                Types = [PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 51,
                    Atk = 52,
                    Def = 90,
                    SpA = 82,
                    SpD = 110,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlowerVeil,
                    Slot1 = AbilityId.Triage,
                    Hidden = AbilityId.NaturalCure,
                },
                HeightM = 0.1,
                WeightKg = 0.3,
                Color = "Green",
            },
            [SpecieId.Oranguru] = new()
            {
                Id = SpecieId.Oranguru,
                Num = 765,
                Name = "Oranguru",
                Types = [PokemonType.Normal, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 60,
                    Def = 80,
                    SpA = 90,
                    SpD = 110,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Slot1 = AbilityId.Telepathy,
                    Hidden = AbilityId.Symbiosis,
                },
                HeightM = 1.5,
                WeightKg = 76,
                Color = "White",
            },
            [SpecieId.Passimian] = new()
            {
                Id = SpecieId.Passimian,
                Num = 766,
                Name = "Passimian",
                Types = [PokemonType.Fighting],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 120,
                    Def = 90,
                    SpA = 40,
                    SpD = 60,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Receiver,
                    Hidden = AbilityId.Defiant,
                },
                HeightM = 2.0,
                WeightKg = 82.8,
                Color = "White",
            },
            [SpecieId.Wimpod] = new()
            {
                Id = SpecieId.Wimpod,
                Num = 767,
                Name = "Wimpod",
                Types = [PokemonType.Bug, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 25,
                    Atk = 35,
                    Def = 40,
                    SpA = 20,
                    SpD = 30,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WimpOut,
                },
                HeightM = 0.5,
                WeightKg = 12,
                Color = "Gray",
            },
            [SpecieId.Golisopod] = new()
            {
                Id = SpecieId.Golisopod,
                Num = 768,
                Name = "Golisopod",
                Types = [PokemonType.Bug, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 125,
                    Def = 140,
                    SpA = 60,
                    SpD = 90,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.EmergencyExit,
                },
                HeightM = 2.0,
                WeightKg = 108,
                Color = "Gray",
            },
            [SpecieId.Sandygast] = new()
            {
                Id = SpecieId.Sandygast,
                Num = 769,
                Name = "Sandygast",
                Types = [PokemonType.Ghost, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 55,
                    Def = 80,
                    SpA = 70,
                    SpD = 45,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterCompaction,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 0.5,
                WeightKg = 70,
                Color = "Brown",
            },
            [SpecieId.Palossand] = new()
            {
                Id = SpecieId.Palossand,
                Num = 770,
                Name = "Palossand",
                Types = [PokemonType.Ghost, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 75,
                    Def = 110,
                    SpA = 100,
                    SpD = 75,
                    Spe = 35,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WaterCompaction,
                    Hidden = AbilityId.SandVeil,
                },
                HeightM = 1.3,
                WeightKg = 250,
                Color = "Brown",
            },
            [SpecieId.Pyukumuku] = new()
            {
                Id = SpecieId.Pyukumuku,
                Num = 771,
                Name = "Pyukumuku",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 60,
                    Def = 130,
                    SpA = 30,
                    SpD = 130,
                    Spe = 5,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnardsOut,
                    Hidden = AbilityId.Unaware,
                },
                HeightM = 0.3,
                WeightKg = 1.2,
                Color = "Black",
            },
            [SpecieId.TypeNull] = new()
            {
                Id = SpecieId.TypeNull,
                Num = 772,
                Name = "Type: Null",
                Types = [PokemonType.Normal],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 59,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                },
                HeightM = 1.9,
                WeightKg = 120.5,
                Color = "Gray",
            },
            [SpecieId.Silvally] = new()
            {
                Id = SpecieId.Silvally,
                Num = 773,
                Name = "Silvally",
                Types = [PokemonType.Normal],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyBug] = new()
            {
                Id = SpecieId.SilvallyBug,
                Num = 773,
                Name = "Silvally-Bug",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Bug,
                Types = [PokemonType.Bug],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyDark] = new()
            {
                Id = SpecieId.SilvallyDark,
                Num = 773,
                Name = "Silvally-Dark",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Dark,
                Types = [PokemonType.Dark],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyDragon] = new()
            {
                Id = SpecieId.SilvallyDragon,
                Num = 773,
                Name = "Silvally-Dragon",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Dragon,
                Types = [PokemonType.Dragon],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyElectric] = new()
            {
                Id = SpecieId.SilvallyElectric,
                Num = 773,
                Name = "Silvally-Electric",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Electric,
                Types = [PokemonType.Electric],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyFairy] = new()
            {
                Id = SpecieId.SilvallyFairy,
                Num = 773,
                Name = "Silvally-Fairy",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Fairy,
                Types = [PokemonType.Fairy],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyFighting] = new()
            {
                Id = SpecieId.SilvallyFighting,
                Num = 773,
                Name = "Silvally-Fighting",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Fighting,
                Types = [PokemonType.Fighting],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyFire] = new()
            {
                Id = SpecieId.SilvallyFire,
                Num = 773,
                Name = "Silvally-Fire",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Fire,
                Types = [PokemonType.Fire],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyFlying] = new()
            {
                Id = SpecieId.SilvallyFlying,
                Num = 773,
                Name = "Silvally-Flying",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Flying,
                Types = [PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyGhost] = new()
            {
                Id = SpecieId.SilvallyGhost,
                Num = 773,
                Name = "Silvally-Ghost",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Ghost,
                Types = [PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyGrass] = new()
            {
                Id = SpecieId.SilvallyGrass,
                Num = 773,
                Name = "Silvally-Grass",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Grass,
                Types = [PokemonType.Grass],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyGround] = new()
            {
                Id = SpecieId.SilvallyGround,
                Num = 773,
                Name = "Silvally-Ground",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Ground,
                Types = [PokemonType.Ground],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyIce] = new()
            {
                Id = SpecieId.SilvallyIce,
                Num = 773,
                Name = "Silvally-Ice",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Ice,
                Types = [PokemonType.Ice],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyPoison] = new()
            {
                Id = SpecieId.SilvallyPoison,
                Num = 773,
                Name = "Silvally-Poison",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Poison,
                Types = [PokemonType.Poison],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyPsychic] = new()
            {
                Id = SpecieId.SilvallyPsychic,
                Num = 773,
                Name = "Silvally-Psychic",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Psychic,
                Types = [PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyRock] = new()
            {
                Id = SpecieId.SilvallyRock,
                Num = 773,
                Name = "Silvally-Rock",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Rock,
                Types = [PokemonType.Rock],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallySteel] = new()
            {
                Id = SpecieId.SilvallySteel,
                Num = 773,
                Name = "Silvally-Steel",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Steel,
                Types = [PokemonType.Steel],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.SilvallyWater] = new()
            {
                Id = SpecieId.SilvallyWater,
                Num = 773,
                Name = "Silvally-Water",
                BaseSpecies = SpecieId.Silvally,
                Forme = FormeId.Water,
                Types = [PokemonType.Water],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 95,
                    Def = 95,
                    SpA = 95,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RKSSystem,
                },
                HeightM = 2.3,
                WeightKg = 100.5,
                Color = "Gray",
            },
            [SpecieId.Minior] = new()
            {
                Id = SpecieId.Minior,
                Num = 774,
                Name = "Minior",
                Types = [PokemonType.Rock, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 100,
                    Def = 60,
                    SpA = 100,
                    SpD = 60,
                    Spe = 120,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldsDown,
                },
                HeightM = 0.3,
                WeightKg = 0.3,
                Color = "Red",
            },
            [SpecieId.MiniorMeteor] = new()
            {
                Id = SpecieId.MiniorMeteor,
                Num = 774,
                Name = "Minior-Meteor",
                BaseSpecies = SpecieId.Minior,
                Forme = FormeId.Meteor,
                Types = [PokemonType.Rock, PokemonType.Flying],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 60,
                    Def = 100,
                    SpA = 60,
                    SpD = 100,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldsDown,
                },
                HeightM = 0.3,
                WeightKg = 40,
                Color = "Brown",
            },
            [SpecieId.Komala] = new()
            {
                Id = SpecieId.Komala,
                Num = 775,
                Name = "Komala",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 115,
                    Def = 65,
                    SpA = 75,
                    SpD = 95,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Comatose,
                },
                HeightM = 0.4,
                WeightKg = 19.9,
                Color = "Blue",
            },
        };
    }
}
