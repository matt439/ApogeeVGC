using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData851To900()
    {
        return new Dictionary<SpecieId, Species>
        {
            // 851 - Centiskorch
            [SpecieId.Centiskorch] = new()
            {
                Id = SpecieId.Centiskorch,
                Num = 851,
                Name = "Centiskorch",
                Types = [PokemonType.Fire, PokemonType.Bug],
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 115,
                    Def = 65,
                    SpA = 90,
                    SpD = 90,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlashFire,
                    Slot1 = AbilityId.WhiteSmoke,
                    Hidden = AbilityId.FlameBody,
                },
                HeightM = 3,
                WeightKg = 120,
                Color = "Red",
            },
            [SpecieId.CentiskorchGmax] = new()
            {
                Id = SpecieId.CentiskorchGmax,
                Num = 851,
                Name = "Centiskorch-Gmax",
                BaseSpecies = SpecieId.Centiskorch,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Fire, PokemonType.Bug],
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 115,
                    Def = 65,
                    SpA = 90,
                    SpD = 90,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlashFire,
                    Slot1 = AbilityId.WhiteSmoke,
                    Hidden = AbilityId.FlameBody,
                },
                HeightM = 75,
                WeightKg = 0,
                Color = "Red",
            },
            // 852 - Clobbopus
            [SpecieId.Clobbopus] = new()
            {
                Id = SpecieId.Clobbopus,
                Num = 852,
                Name = "Clobbopus",
                Types = [PokemonType.Fighting],
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 68,
                    Def = 60,
                    SpA = 50,
                    SpD = 50,
                    Spe = 32,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Limber,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 0.6,
                WeightKg = 4,
                Color = "Brown",
            },
            // 853 - Grapploct
            [SpecieId.Grapploct] = new()
            {
                Id = SpecieId.Grapploct,
                Num = 853,
                Name = "Grapploct",
                Types = [PokemonType.Fighting],
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 118,
                    Def = 90,
                    SpA = 70,
                    SpD = 80,
                    Spe = 42,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Limber,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 1.6,
                WeightKg = 39,
                Color = "Blue",
            },
            // 854 - Sinistea
            [SpecieId.Sinistea] = new()
            {
                Id = SpecieId.Sinistea,
                Num = 854,
                Name = "Sinistea",
                Types = [PokemonType.Ghost],
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
                    Slot0 = AbilityId.WeakArmor,
                    Hidden = AbilityId.CursedBody,
                },
                HeightM = 0.1,
                WeightKg = 0.2,
                Color = "Purple",
            },
            [SpecieId.SinisteaAntique] = new()
            {
                Id = SpecieId.SinisteaAntique,
                Num = 854,
                Name = "Sinistea-Antique",
                BaseSpecies = SpecieId.Sinistea,
                Forme = FormeId.Antique,
                Types = [PokemonType.Ghost],
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
                    Slot0 = AbilityId.WeakArmor,
                    Hidden = AbilityId.CursedBody,
                },
                HeightM = 0.1,
                WeightKg = 0.2,
                Color = "Purple",
            },
            // 855 - Polteageist
            [SpecieId.Polteageist] = new()
            {
                Id = SpecieId.Polteageist,
                Num = 855,
                Name = "Polteageist",
                Types = [PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 65,
                    Def = 65,
                    SpA = 134,
                    SpD = 114,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WeakArmor,
                    Hidden = AbilityId.CursedBody,
                },
                HeightM = 0.2,
                WeightKg = 0.4,
                Color = "Purple",
            },
            [SpecieId.PolteageistAntique] = new()
            {
                Id = SpecieId.PolteageistAntique,
                Num = 855,
                Name = "Polteageist-Antique",
                BaseSpecies = SpecieId.Polteageist,
                Forme = FormeId.Antique,
                Types = [PokemonType.Ghost],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 65,
                    Def = 65,
                    SpA = 134,
                    SpD = 114,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WeakArmor,
                    Hidden = AbilityId.CursedBody,
                },
                HeightM = 0.2,
                WeightKg = 0.4,
                Color = "Purple",
            },
            // 856 - Hatenna
            [SpecieId.Hatenna] = new()
            {
                Id = SpecieId.Hatenna,
                Num = 856,
                Name = "Hatenna",
                Types = [PokemonType.Psychic],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 42,
                    Atk = 30,
                    Def = 45,
                    SpA = 56,
                    SpD = 53,
                    Spe = 39,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Healer,
                    Slot1 = AbilityId.Anticipation,
                    Hidden = AbilityId.MagicBounce,
                },
                HeightM = 0.4,
                WeightKg = 3.4,
                Color = "Pink",
            },
            // 857 - Hattrem
            [SpecieId.Hattrem] = new()
            {
                Id = SpecieId.Hattrem,
                Num = 857,
                Name = "Hattrem",
                Types = [PokemonType.Psychic],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 57,
                    Atk = 40,
                    Def = 65,
                    SpA = 86,
                    SpD = 73,
                    Spe = 49,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Healer,
                    Slot1 = AbilityId.Anticipation,
                    Hidden = AbilityId.MagicBounce,
                },
                HeightM = 0.6,
                WeightKg = 4.8,
                Color = "Pink",
            },
            // 858 - Hatterene
            [SpecieId.Hatterene] = new()
            {
                Id = SpecieId.Hatterene,
                Num = 858,
                Name = "Hatterene",
                Types = [PokemonType.Psychic, PokemonType.Fairy],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 57,
                    Atk = 90,
                    Def = 95,
                    SpA = 136,
                    SpD = 103,
                    Spe = 29,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Healer,
                    Slot1 = AbilityId.Anticipation,
                    Hidden = AbilityId.MagicBounce,
                },
                HeightM = 2.1,
                WeightKg = 5.1,
                Color = "Pink",
            },
            [SpecieId.HattereneGmax] = new()
            {
                Id = SpecieId.HattereneGmax,
                Num = 858,
                Name = "Hatterene-Gmax",
                BaseSpecies = SpecieId.Hatterene,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Psychic, PokemonType.Fairy],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 57,
                    Atk = 90,
                    Def = 95,
                    SpA = 136,
                    SpD = 103,
                    Spe = 29,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Healer,
                    Slot1 = AbilityId.Anticipation,
                    Hidden = AbilityId.MagicBounce,
                },
                HeightM = 26,
                WeightKg = 0,
                Color = "Pink",
            },
            // 859 - Impidimp
            [SpecieId.Impidimp] = new()
            {
                Id = SpecieId.Impidimp,
                Num = 859,
                Name = "Impidimp",
                Types = [PokemonType.Dark, PokemonType.Fairy],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 45,
                    Def = 30,
                    SpA = 55,
                    SpD = 40,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 0.4,
                WeightKg = 5.5,
                Color = "Pink",
            },
            // 860 - Morgrem
            [SpecieId.Morgrem] = new()
            {
                Id = SpecieId.Morgrem,
                Num = 860,
                Name = "Morgrem",
                Types = [PokemonType.Dark, PokemonType.Fairy],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 60,
                    Def = 45,
                    SpA = 75,
                    SpD = 55,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 0.8,
                WeightKg = 12.5,
                Color = "Pink",
            },
            // 861 - Grimmsnarl (already exists)
            [SpecieId.Grimmsnarl] = new()
            {
                Id = SpecieId.Grimmsnarl,
                Num = 861,
                Name = "Grimmsnarl",
                Types = [PokemonType.Dark, PokemonType.Fairy],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 120,
                    Def = 65,
                    SpA = 95,
                    SpD = 75,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 1.5,
                WeightKg = 61,
                Color = "Purple",
            },
            [SpecieId.GrimmsnarlGmax] = new()
            {
                Id = SpecieId.GrimmsnarlGmax,
                Num = 861,
                Name = "Grimmsnarl-Gmax",
                BaseSpecies = SpecieId.Grimmsnarl,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Dark, PokemonType.Fairy],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 120,
                    Def = 65,
                    SpA = 95,
                    SpD = 75,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.Pickpocket,
                },
                HeightM = 32,
                WeightKg = 0,
                Color = "Purple",
            },
            // 862 - Obstagoon
            [SpecieId.Obstagoon] = new()
            {
                Id = SpecieId.Obstagoon,
                Num = 862,
                Name = "Obstagoon",
                Types = [PokemonType.Dark, PokemonType.Normal],
                BaseStats = new StatsTable
                {
                    Hp = 93,
                    Atk = 90,
                    Def = 101,
                    SpA = 60,
                    SpD = 81,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Reckless,
                    Slot1 = AbilityId.Guts,
                    Hidden = AbilityId.Defiant,
                },
                HeightM = 1.6,
                WeightKg = 46,
                Color = "Gray",
            },
            // 863 - Perrserker
            [SpecieId.Perrserker] = new()
            {
                Id = SpecieId.Perrserker,
                Num = 863,
                Name = "Perrserker",
                Types = [PokemonType.Steel],
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 110,
                    Def = 100,
                    SpA = 50,
                    SpD = 60,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                    Slot1 = AbilityId.ToughClaws,
                    Hidden = AbilityId.SteelySpirit,
                },
                HeightM = 0.8,
                    WeightKg = 28,
                    Color = "Brown",
                },
                // 864 - Cursola
                [SpecieId.Cursola] = new()
                {
                    Id = SpecieId.Cursola,
                    Num = 864,
                    Name = "Cursola",
                    Types = [PokemonType.Ghost],
                    // Note: In TS has genderRatio M: 0.25, F: 0.75 (inherited from Corsola-Galar)
                    BaseStats = new StatsTable
                    {
                        Hp = 60,
                        Atk = 95,
                        Def = 50,
                        SpA = 145,
                        SpD = 130,
                        Spe = 30,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.WeakArmor,
                        Hidden = AbilityId.PerishBody,
                    },
                    HeightM = 1,
                    WeightKg = 0.4,
                    Color = "White",
                },
                // 865 - Sirfetch'd
                [SpecieId.Sirfetchd] = new()
            {
                Id = SpecieId.Sirfetchd,
                Num = 865,
                Name = "Sirfetch'd",
                Types = [PokemonType.Fighting],
                BaseStats = new StatsTable
                {
                    Hp = 62,
                    Atk = 135,
                    Def = 95,
                    SpA = 68,
                    SpD = 82,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Steadfast,
                    Hidden = AbilityId.Scrappy,
                },
                HeightM = 0.8,
                WeightKg = 117,
                Color = "White",
            },
            // 866 - Mr. Rime
            [SpecieId.MrRime] = new()
            {
                Id = SpecieId.MrRime,
                Num = 866,
                Name = "Mr. Rime",
                Types = [PokemonType.Ice, PokemonType.Psychic],
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 85,
                    Def = 75,
                    SpA = 110,
                    SpD = 100,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.TangledFeet,
                    Slot1 = AbilityId.ScreenCleaner,
                    Hidden = AbilityId.IceBody,
                },
                HeightM = 1.5,
                WeightKg = 58.2,
                Color = "Purple",
            },
            // 867 - Runerigus
            [SpecieId.Runerigus] = new()
            {
                Id = SpecieId.Runerigus,
                Num = 867,
                Name = "Runerigus",
                Types = [PokemonType.Ground, PokemonType.Ghost],
                BaseStats = new StatsTable
                {
                    Hp = 58,
                    Atk = 95,
                    Def = 145,
                    SpA = 50,
                    SpD = 105,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.WanderingSpirit,
                },
                HeightM = 1.6,
                WeightKg = 66.6,
                Color = "Gray",
            },
            // 868 - Milcery
            [SpecieId.Milcery] = new()
            {
                Id = SpecieId.Milcery,
                Num = 868,
                Name = "Milcery",
                Types = [PokemonType.Fairy],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 40,
                    Def = 40,
                    SpA = 50,
                    SpD = 61,
                    Spe = 34,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SweetVeil,
                    Hidden = AbilityId.AromaVeil,
                },
                HeightM = 0.2,
                WeightKg = 0.3,
                Color = "White",
            },
            // 869 - Alcremie
            [SpecieId.Alcremie] = new()
            {
                Id = SpecieId.Alcremie,
                Num = 869,
                Name = "Alcremie",
                Types = [PokemonType.Fairy],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 60,
                    Def = 75,
                    SpA = 110,
                    SpD = 121,
                    Spe = 64,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SweetVeil,
                    Hidden = AbilityId.AromaVeil,
                },
                HeightM = 0.3,
                WeightKg = 0.5,
                Color = "White",
            },
            [SpecieId.AlcremieGmax] = new()
            {
                Id = SpecieId.AlcremieGmax,
                Num = 869,
                Name = "Alcremie-Gmax",
                BaseSpecies = SpecieId.Alcremie,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Fairy],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 60,
                    Def = 75,
                    SpA = 110,
                    SpD = 121,
                    Spe = 64,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SweetVeil,
                    Hidden = AbilityId.AromaVeil,
                },
                HeightM = 30,
                WeightKg = 0,
                Color = "Yellow",
            },
            // 870 - Falinks
            [SpecieId.Falinks] = new()
            {
                Id = SpecieId.Falinks,
                Num = 870,
                Name = "Falinks",
                Types = [PokemonType.Fighting],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 100,
                    Def = 100,
                    SpA = 70,
                    SpD = 60,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                    Hidden = AbilityId.Defiant,
                },
                HeightM = 3,
                WeightKg = 62,
                Color = "Yellow",
            },
            [SpecieId.FalinksMega] = new()
            {
                Id = SpecieId.FalinksMega,
                Num = 870,
                Name = "Falinks-Mega",
                BaseSpecies = SpecieId.Falinks,
                Forme = FormeId.Mega,
                Types = [PokemonType.Fighting],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 135,
                    Def = 135,
                    SpA = 70,
                    SpD = 65,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                    Hidden = AbilityId.Defiant,
                },
                HeightM = 1.6,
                WeightKg = 99,
                Color = "Yellow",
            },
            // 871 - Pincurchin
            [SpecieId.Pincurchin] = new()
            {
                Id = SpecieId.Pincurchin,
                Num = 871,
                Name = "Pincurchin",
                Types = [PokemonType.Electric],
                BaseStats = new StatsTable
                {
                    Hp = 48,
                    Atk = 101,
                    Def = 95,
                    SpA = 91,
                    SpD = 85,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.LightningRod,
                    Hidden = AbilityId.ElectricSurge,
                },
                HeightM = 0.3,
                WeightKg = 1,
                Color = "Purple",
            },
            // 872 - Snom
            [SpecieId.Snom] = new()
            {
                Id = SpecieId.Snom,
                Num = 872,
                Name = "Snom",
                Types = [PokemonType.Ice, PokemonType.Bug],
                BaseStats = new StatsTable
                {
                    Hp = 30,
                    Atk = 25,
                    Def = 35,
                    SpA = 45,
                    SpD = 30,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldDust,
                    Hidden = AbilityId.IceScales,
                },
                HeightM = 0.3,
                WeightKg = 3.8,
                Color = "White",
            },
            // 873 - Frosmoth
            [SpecieId.Frosmoth] = new()
            {
                Id = SpecieId.Frosmoth,
                Num = 873,
                Name = "Frosmoth",
                Types = [PokemonType.Ice, PokemonType.Bug],
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 65,
                    Def = 60,
                    SpA = 125,
                    SpD = 90,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShieldDust,
                    Hidden = AbilityId.IceScales,
                },
                HeightM = 1.3,
                WeightKg = 42,
                Color = "White",
            },
            // 874 - Stonjourner
            [SpecieId.Stonjourner] = new()
            {
                Id = SpecieId.Stonjourner,
                Num = 874,
                Name = "Stonjourner",
                Types = [PokemonType.Rock],
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 125,
                    Def = 135,
                    SpA = 20,
                    SpD = 20,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PowerSpot,
                },
                HeightM = 2.5,
                WeightKg = 520,
                Color = "Gray",
            },
            // 875 - Eiscue
            [SpecieId.Eiscue] = new()
            {
                Id = SpecieId.Eiscue,
                Num = 875,
                Name = "Eiscue",
                Types = [PokemonType.Ice],
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 80,
                    Def = 110,
                    SpA = 65,
                    SpD = 90,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.IceFace,
                },
                HeightM = 1.4,
                WeightKg = 89,
                Color = "Blue",
            },
            [SpecieId.EiscueNoice] = new()
            {
                Id = SpecieId.EiscueNoice,
                Num = 875,
                Name = "Eiscue-Noice",
                BaseSpecies = SpecieId.Eiscue,
                Forme = FormeId.Noice,
                Types = [PokemonType.Ice],
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 80,
                    Def = 70,
                    SpA = 65,
                    SpD = 50,
                    Spe = 130,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.IceFace,
                },
                HeightM = 1.4,
                WeightKg = 89,
                Color = "Blue",
            },
            // 876 - Indeedee
            [SpecieId.Indeedee] = new()
            {
                Id = SpecieId.Indeedee,
                Num = 876,
                Name = "Indeedee",
                Types = [PokemonType.Psychic, PokemonType.Normal],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 65,
                    Def = 55,
                    SpA = 105,
                    SpD = 95,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Slot1 = AbilityId.Synchronize,
                    Hidden = AbilityId.PsychicSurge,
                },
                HeightM = 0.9,
                WeightKg = 28,
                Color = "Purple",
            },
            [SpecieId.IndeedeeF] = new()
            {
                Id = SpecieId.IndeedeeF,
                Num = 876,
                Name = "Indeedee-F",
                BaseSpecies = SpecieId.Indeedee,
                Forme = FormeId.F,
                Types = [PokemonType.Psychic, PokemonType.Normal],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 55,
                    Def = 65,
                    SpA = 95,
                    SpD = 105,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.OwnTempo,
                    Slot1 = AbilityId.Synchronize,
                    Hidden = AbilityId.PsychicSurge,
                },
                HeightM = 0.9,
                WeightKg = 28,
                Color = "Purple",
            },
            // 877 - Morpeko (already exists in another file, adding Hangry forme)
            [SpecieId.MorpekoHangry] = new()
            {
                Id = SpecieId.MorpekoHangry,
                Num = 877,
                Name = "Morpeko-Hangry",
                BaseSpecies = SpecieId.Morpeko,
                Forme = FormeId.Hangry,
                Types = [PokemonType.Electric, PokemonType.Dark],
                BaseStats = new StatsTable
                {
                    Hp = 58,
                    Atk = 95,
                    Def = 58,
                    SpA = 70,
                    SpD = 58,
                    Spe = 97,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HungerSwitch,
                },
                HeightM = 0.3,
                WeightKg = 3,
                Color = "Purple",
            },
            // 878 - Cufant
            [SpecieId.Cufant] = new()
            {
                Id = SpecieId.Cufant,
                Num = 878,
                Name = "Cufant",
                Types = [PokemonType.Steel],
                BaseStats = new StatsTable
                {
                    Hp = 72,
                    Atk = 80,
                    Def = 49,
                    SpA = 40,
                    SpD = 49,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SheerForce,
                    Hidden = AbilityId.HeavyMetal,
                },
                HeightM = 1.2,
                WeightKg = 100,
                Color = "Yellow",
            },
            // 879 - Copperajah
            [SpecieId.Copperajah] = new()
            {
                Id = SpecieId.Copperajah,
                Num = 879,
                Name = "Copperajah",
                Types = [PokemonType.Steel],
                BaseStats = new StatsTable
                {
                    Hp = 122,
                    Atk = 130,
                    Def = 69,
                    SpA = 80,
                    SpD = 69,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SheerForce,
                    Hidden = AbilityId.HeavyMetal,
                },
                HeightM = 3,
                WeightKg = 650,
                Color = "Green",
            },
            [SpecieId.CopperajahGmax] = new()
            {
                Id = SpecieId.CopperajahGmax,
                Num = 879,
                Name = "Copperajah-Gmax",
                BaseSpecies = SpecieId.Copperajah,
                Forme = FormeId.Gmax,
                Types = [PokemonType.Steel],
                BaseStats = new StatsTable
                {
                    Hp = 122,
                    Atk = 130,
                    Def = 69,
                    SpA = 80,
                    SpD = 69,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SheerForce,
                    Hidden = AbilityId.HeavyMetal,
                },
                    HeightM = 23,
                    WeightKg = 0,
                    Color = "Green",
                },
                // 880 - Dracozolt
                [SpecieId.Dracozolt] = new()
                {
                    Id = SpecieId.Dracozolt,
                    Num = 880,
                    Name = "Dracozolt",
                    Types = [PokemonType.Electric, PokemonType.Dragon],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 90,
                        Atk = 100,
                        Def = 90,
                        SpA = 80,
                        SpD = 70,
                        Spe = 75,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.VoltAbsorb,
                        Slot1 = AbilityId.Hustle,
                        Hidden = AbilityId.SandRush,
                    },
                    HeightM = 1.8,
                    WeightKg = 190,
                    Color = "Green",
                },
                // 881 - Arctozolt
                [SpecieId.Arctozolt] = new()
                {
                    Id = SpecieId.Arctozolt,
                    Num = 881,
                    Name = "Arctozolt",
                    Types = [PokemonType.Electric, PokemonType.Ice],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 90,
                        Atk = 100,
                        Def = 90,
                        SpA = 90,
                        SpD = 80,
                        Spe = 55,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.VoltAbsorb,
                        Slot1 = AbilityId.Static,
                        Hidden = AbilityId.SlushRush,
                    },
                    HeightM = 2.3,
                    WeightKg = 150,
                    Color = "Blue",
                },
                // 882 - Dracovish
                [SpecieId.Dracovish] = new()
                {
                    Id = SpecieId.Dracovish,
                    Num = 882,
                    Name = "Dracovish",
                    Types = [PokemonType.Water, PokemonType.Dragon],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 90,
                        Atk = 90,
                        Def = 100,
                        SpA = 70,
                        SpD = 80,
                        Spe = 75,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.WaterAbsorb,
                        Slot1 = AbilityId.StrongJaw,
                        Hidden = AbilityId.SandRush,
                    },
                    HeightM = 2.3,
                    WeightKg = 215,
                    Color = "Green",
                },
                // 883 - Arctovish
                [SpecieId.Arctovish] = new()
                {
                    Id = SpecieId.Arctovish,
                    Num = 883,
                    Name = "Arctovish",
                    Types = [PokemonType.Water, PokemonType.Ice],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 90,
                        Atk = 90,
                        Def = 100,
                        SpA = 80,
                        SpD = 90,
                        Spe = 55,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.WaterAbsorb,
                        Slot1 = AbilityId.IceBody,
                        Hidden = AbilityId.SlushRush,
                    },
                    HeightM = 2,
                    WeightKg = 175,
                    Color = "Blue",
                },
                // 884 - Duraludon
                [SpecieId.Duraludon] = new()
                {
                    Id = SpecieId.Duraludon,
                    Num = 884,
                    Name = "Duraludon",
                    Types = [PokemonType.Steel, PokemonType.Dragon],
                    BaseStats = new StatsTable
                    {
                        Hp = 70,
                        Atk = 95,
                        Def = 115,
                        SpA = 120,
                        SpD = 50,
                        Spe = 85,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.LightMetal,
                        Slot1 = AbilityId.HeavyMetal,
                        Hidden = AbilityId.Stalwart,
                    },
                    HeightM = 1.8,
                    WeightKg = 40,
                    Color = "White",
                },
                [SpecieId.DuraludonGmax] = new()
                {
                    Id = SpecieId.DuraludonGmax,
                    Num = 884,
                    Name = "Duraludon-Gmax",
                    BaseSpecies = SpecieId.Duraludon,
                    Forme = FormeId.Gmax,
                    Types = [PokemonType.Steel, PokemonType.Dragon],
                    BaseStats = new StatsTable
                    {
                        Hp = 70,
                        Atk = 95,
                        Def = 115,
                        SpA = 120,
                        SpD = 50,
                        Spe = 85,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.LightMetal,
                        Slot1 = AbilityId.HeavyMetal,
                        Hidden = AbilityId.Stalwart,
                    },
                    HeightM = 43,
                    WeightKg = 0,
                    Color = "White",
                },
                // 885 - Dreepy
                [SpecieId.Dreepy] = new()
                {
                    Id = SpecieId.Dreepy,
                    Num = 885,
                    Name = "Dreepy",
                    Types = [PokemonType.Dragon, PokemonType.Ghost],
                    BaseStats = new StatsTable
                    {
                        Hp = 28,
                        Atk = 60,
                        Def = 30,
                        SpA = 40,
                        SpD = 30,
                        Spe = 82,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.ClearBody,
                        Slot1 = AbilityId.Infiltrator,
                        Hidden = AbilityId.CursedBody,
                    },
                    HeightM = 0.5,
                    WeightKg = 2,
                    Color = "Green",
                },
                // 886 - Drakloak
                [SpecieId.Drakloak] = new()
                {
                    Id = SpecieId.Drakloak,
                    Num = 886,
                    Name = "Drakloak",
                    Types = [PokemonType.Dragon, PokemonType.Ghost],
                    BaseStats = new StatsTable
                    {
                        Hp = 68,
                        Atk = 80,
                        Def = 50,
                        SpA = 60,
                        SpD = 50,
                        Spe = 102,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.ClearBody,
                        Slot1 = AbilityId.Infiltrator,
                        Hidden = AbilityId.CursedBody,
                    },
                    HeightM = 1.4,
                    WeightKg = 11,
                    Color = "Green",
                },
                // 887 - Dragapult
                [SpecieId.Dragapult] = new()
                {
                    Id = SpecieId.Dragapult,
                    Num = 887,
                    Name = "Dragapult",
                    Types = [PokemonType.Dragon, PokemonType.Ghost],
                    BaseStats = new StatsTable
                    {
                        Hp = 88,
                        Atk = 120,
                        Def = 75,
                        SpA = 100,
                        SpD = 75,
                        Spe = 142,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.ClearBody,
                        Slot1 = AbilityId.Infiltrator,
                        Hidden = AbilityId.CursedBody,
                    },
                    HeightM = 3,
                    WeightKg = 50,
                    Color = "Green",
                },
                // 888 - Zacian
                [SpecieId.ZacianHero] = new()
                {
                    Id = SpecieId.ZacianHero,
                    Num = 888,
                    Name = "Zacian",
                    Types = [PokemonType.Fairy],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 92,
                        Atk = 120,
                        Def = 115,
                        SpA = 80,
                        SpD = 115,
                        Spe = 138,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.IntrepidSword,
                    },
                    HeightM = 2.8,
                    WeightKg = 110,
                    Color = "Blue",
                },
                [SpecieId.ZacianCrowned] = new()
                {
                    Id = SpecieId.ZacianCrowned,
                    Num = 888,
                    Name = "Zacian-Crowned",
                    BaseSpecies = SpecieId.Zacian,
                    Forme = FormeId.Crowned,
                    Types = [PokemonType.Fairy, PokemonType.Steel],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 92,
                        Atk = 150,
                        Def = 115,
                        SpA = 80,
                        SpD = 115,
                        Spe = 148,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.IntrepidSword,
                    },
                    HeightM = 2.8,
                    WeightKg = 355,
                    Color = "Blue",
                },
                // 889 - Zamazenta
                [SpecieId.ZamazentaHero] = new()
                {
                    Id = SpecieId.ZamazentaHero,
                    Num = 889,
                    Name = "Zamazenta",
                    Types = [PokemonType.Fighting],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 92,
                        Atk = 120,
                        Def = 115,
                        SpA = 80,
                        SpD = 115,
                        Spe = 138,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.DauntlessShield,
                    },
                    HeightM = 2.9,
                    WeightKg = 210,
                    Color = "Red",
                },
                [SpecieId.ZamazentaCrowned] = new()
                {
                    Id = SpecieId.ZamazentaCrowned,
                    Num = 889,
                    Name = "Zamazenta-Crowned",
                    BaseSpecies = SpecieId.Zamazenta,
                    Forme = FormeId.Crowned,
                    Types = [PokemonType.Fighting, PokemonType.Steel],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 92,
                        Atk = 120,
                        Def = 140,
                        SpA = 80,
                        SpD = 140,
                        Spe = 128,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.DauntlessShield,
                    },
                    HeightM = 2.9,
                    WeightKg = 785,
                    Color = "Red",
                },
                // 890 - Eternatus
                [SpecieId.Eternatus] = new()
                {
                    Id = SpecieId.Eternatus,
                    Num = 890,
                    Name = "Eternatus",
                    Types = [PokemonType.Poison, PokemonType.Dragon],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 140,
                        Atk = 85,
                        Def = 95,
                        SpA = 145,
                        SpD = 95,
                        Spe = 130,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Pressure,
                    },
                    HeightM = 20,
                    WeightKg = 950,
                    Color = "Purple",
                },
                [SpecieId.EternatusEternamax] = new()
                {
                    Id = SpecieId.EternatusEternamax,
                    Num = 890,
                    Name = "Eternatus-Eternamax",
                    BaseSpecies = SpecieId.Eternatus,
                    Forme = FormeId.Eternamax,
                    Types = [PokemonType.Poison, PokemonType.Dragon],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 255,
                        Atk = 115,
                        Def = 250,
                        SpA = 125,
                        SpD = 250,
                        Spe = 130,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Pressure,
                    },
                    HeightM = 100,
                    WeightKg = 0,
                    Color = "Purple",
                },
                // 891 - Kubfu
                [SpecieId.Kubfu] = new()
                {
                    Id = SpecieId.Kubfu,
                    Num = 891,
                    Name = "Kubfu",
                    Types = [PokemonType.Fighting],
                    BaseStats = new StatsTable
                    {
                        Hp = 60,
                        Atk = 90,
                        Def = 60,
                        SpA = 53,
                        SpD = 50,
                        Spe = 72,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.InnerFocus,
                    },
                    HeightM = 0.6,
                    WeightKg = 12,
                    Color = "Gray",
                },
                // 892 - Urshifu
                [SpecieId.Urshifu] = new()
                {
                    Id = SpecieId.Urshifu,
                    Num = 892,
                    Name = "Urshifu",
                    Types = [PokemonType.Fighting, PokemonType.Dark],
                    BaseStats = new StatsTable
                    {
                        Hp = 100,
                        Atk = 130,
                        Def = 100,
                        SpA = 63,
                        SpD = 60,
                        Spe = 97,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.UnseenFist,
                    },
                    HeightM = 1.9,
                    WeightKg = 105,
                    Color = "Gray",
                },
                [SpecieId.UrshifuRapidStrike] = new()
                {
                    Id = SpecieId.UrshifuRapidStrike,
                    Num = 892,
                    Name = "Urshifu-Rapid-Strike",
                    BaseSpecies = SpecieId.Urshifu,
                    Forme = FormeId.RapidStrike,
                    Types = [PokemonType.Fighting, PokemonType.Water],
                    BaseStats = new StatsTable
                    {
                        Hp = 100,
                        Atk = 130,
                        Def = 100,
                        SpA = 63,
                        SpD = 60,
                        Spe = 97,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.UnseenFist,
                    },
                    HeightM = 1.9,
                    WeightKg = 105,
                    Color = "Gray",
                },
                [SpecieId.UrshifuGmax] = new()
                {
                    Id = SpecieId.UrshifuGmax,
                    Num = 892,
                    Name = "Urshifu-Gmax",
                    BaseSpecies = SpecieId.Urshifu,
                    Forme = FormeId.Gmax,
                    Types = [PokemonType.Fighting, PokemonType.Dark],
                    BaseStats = new StatsTable
                    {
                        Hp = 100,
                        Atk = 130,
                        Def = 100,
                        SpA = 63,
                        SpD = 60,
                        Spe = 97,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.UnseenFist,
                    },
                    HeightM = 29,
                    WeightKg = 0,
                    Color = "Gray",
                },
                [SpecieId.UrshifuRapidStrikeGmax] = new()
                {
                    Id = SpecieId.UrshifuRapidStrikeGmax,
                    Num = 892,
                    Name = "Urshifu-Rapid-Strike-Gmax",
                    BaseSpecies = SpecieId.Urshifu,
                    Forme = FormeId.RapidStrikeGmax,
                    Types = [PokemonType.Fighting, PokemonType.Water],
                    BaseStats = new StatsTable
                    {
                        Hp = 100,
                        Atk = 130,
                        Def = 100,
                        SpA = 63,
                        SpD = 60,
                        Spe = 97,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.UnseenFist,
                    },
                    HeightM = 26,
                    WeightKg = 0,
                    Color = "Gray",
                },
                // 893 - Zarude
                [SpecieId.Zarude] = new()
                {
                    Id = SpecieId.Zarude,
                    Num = 893,
                    Name = "Zarude",
                    Types = [PokemonType.Dark, PokemonType.Grass],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 105,
                        Atk = 120,
                        Def = 105,
                        SpA = 70,
                        SpD = 95,
                        Spe = 105,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.LeafGuard,
                    },
                    HeightM = 1.8,
                    WeightKg = 70,
                    Color = "Black",
                },
                [SpecieId.ZarudeDada] = new()
                {
                    Id = SpecieId.ZarudeDada,
                    Num = 893,
                    Name = "Zarude-Dada",
                    BaseSpecies = SpecieId.Zarude,
                    Forme = FormeId.Dada,
                    Types = [PokemonType.Dark, PokemonType.Grass],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 105,
                        Atk = 120,
                        Def = 105,
                        SpA = 70,
                        SpD = 95,
                        Spe = 105,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.LeafGuard,
                    },
                    HeightM = 1.8,
                    WeightKg = 70,
                    Color = "Black",
                },
                // 894 - Regieleki
                [SpecieId.Regieleki] = new()
                {
                    Id = SpecieId.Regieleki,
                    Num = 894,
                    Name = "Regieleki",
                    Types = [PokemonType.Electric],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 80,
                        Atk = 100,
                        Def = 50,
                        SpA = 100,
                        SpD = 50,
                        Spe = 200,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Transistor,
                    },
                    HeightM = 1.2,
                    WeightKg = 145,
                    Color = "Yellow",
                },
                // 895 - Regidrago
                [SpecieId.Regidrago] = new()
                {
                    Id = SpecieId.Regidrago,
                    Num = 895,
                    Name = "Regidrago",
                    Types = [PokemonType.Dragon],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 200,
                        Atk = 100,
                        Def = 50,
                        SpA = 100,
                        SpD = 50,
                        Spe = 80,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.DragonsMaw,
                    },
                    HeightM = 2.1,
                    WeightKg = 200,
                    Color = "Green",
                },
                // 896 - Glastrier
                [SpecieId.Glastrier] = new()
                {
                    Id = SpecieId.Glastrier,
                    Num = 896,
                    Name = "Glastrier",
                    Types = [PokemonType.Ice],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 100,
                        Atk = 145,
                        Def = 130,
                        SpA = 65,
                        SpD = 110,
                        Spe = 30,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.ChillingNeigh,
                    },
                    HeightM = 2.2,
                    WeightKg = 800,
                    Color = "White",
                },
                // 897 - Spectrier
                [SpecieId.Spectrier] = new()
                {
                    Id = SpecieId.Spectrier,
                    Num = 897,
                    Name = "Spectrier",
                    Types = [PokemonType.Ghost],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 100,
                        Atk = 65,
                        Def = 60,
                        SpA = 145,
                        SpD = 80,
                        Spe = 130,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.GrimNeigh,
                    },
                    HeightM = 2,
                    WeightKg = 44.5,
                    Color = "Black",
                },
                // 898 - Calyrex
                [SpecieId.Calyrex] = new()
                {
                    Id = SpecieId.Calyrex,
                    Num = 898,
                    Name = "Calyrex",
                    Types = [PokemonType.Psychic, PokemonType.Grass],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 100,
                        Atk = 80,
                        Def = 80,
                        SpA = 80,
                        SpD = 80,
                        Spe = 80,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Unnerve,
                    },
                    HeightM = 1.1,
                    WeightKg = 7.7,
                    Color = "Green",
                },
                // 898 - Calyrex-Ice (already exists)
            [SpecieId.CalyrexIce] = new()
            {
                Id = SpecieId.CalyrexIce,
                Num = 898,
                Name = "Calyrex-Ice",
                BaseSpecies = SpecieId.Calyrex,
                Forme = FormeId.Ice,
                Types = [PokemonType.Psychic, PokemonType.Ice],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 165,
                    Def = 150,
                    SpA = 85,
                    SpD = 130,
                    Spe = 50,
                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.AsOneGlastrier,
                                },
                                HeightM = 2.4,
                                WeightKg = 809.1,
                                Color = "White",
                            },
                            [SpecieId.CalyrexShadow] = new()
                            {
                                Id = SpecieId.CalyrexShadow,
                                Num = 898,
                                Name = "Calyrex-Shadow",
                                BaseSpecies = SpecieId.Calyrex,
                                Forme = FormeId.Shadow,
                                Types = [PokemonType.Psychic, PokemonType.Ghost],
                                Gender = GenderId.N,
                                BaseStats = new StatsTable
                                {
                                    Hp = 100,
                                    Atk = 85,
                                    Def = 80,
                                    SpA = 165,
                                    SpD = 100,
                                    Spe = 150,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.AsOneSpectrier,
                                },
                                HeightM = 2.4,
                                WeightKg = 53.6,
                                Color = "Black",
                            },
                            // 899 - Wyrdeer
                            [SpecieId.Wyrdeer] = new()
                            {
                                Id = SpecieId.Wyrdeer,
                                Num = 899,
                                Name = "Wyrdeer",
                                Types = [PokemonType.Normal, PokemonType.Psychic],
                                BaseStats = new StatsTable
                                {
                                    Hp = 103,
                                    Atk = 105,
                                    Def = 72,
                                    SpA = 105,
                                    SpD = 75,
                                    Spe = 65,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Intimidate,
                                    Slot1 = AbilityId.Frisk,
                                    Hidden = AbilityId.SapSipper,
                                },
                                HeightM = 1.8,
                                WeightKg = 95.1,
                                Color = "Gray",
                            },
                            // 900 - Kleavor
                            [SpecieId.Kleavor] = new()
                            {
                                Id = SpecieId.Kleavor,
                                Num = 900,
                                Name = "Kleavor",
                                Types = [PokemonType.Bug, PokemonType.Rock],
                                BaseStats = new StatsTable
                                {
                                    Hp = 70,
                                    Atk = 135,
                                    Def = 95,
                                    SpA = 45,
                                    SpD = 70,
                                    Spe = 85,
                                },
                                Abilities = new SpeciesAbility
                                {
                                    Slot0 = AbilityId.Swarm,
                                    Slot1 = AbilityId.SheerForce,
                                    Hidden = AbilityId.Sharpness,
                                },
                                HeightM = 1.8,
                                WeightKg = 89,
                                Color = "Brown",
                            },
                        };
                    }
                }
