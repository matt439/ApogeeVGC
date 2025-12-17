using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData351To400()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Castform] = new()
            {
                Id = SpecieId.Castform,
                Num = 351,
                Name = "Castform",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 70,
                    Def = 70,
                    SpA = 70,
                    SpD = 70,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Forecast,
                },
                HeightM = 0.3,
                WeightKg = 0.8,
                Color = "Gray",
                OtherFormes = [FormeId.Sunny, FormeId.Rainy, FormeId.Snowy],
            },
            [SpecieId.CastformSunny] = new()
            {
                Id = SpecieId.CastformSunny,
                Num = 351,
                Name = "Castform-Sunny",
                BaseSpecies = SpecieId.Castform,
                Forme = FormeId.Sunny,
                Types = [PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 70,
                    Def = 70,
                    SpA = 70,
                    SpD = 70,
                    Spe = 70,
                },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Forecast,
                    },
                    HeightM = 0.3,
                    WeightKg = 0.8,
                    Color = "Red",
                },
            [SpecieId.CastformRainy] = new()
            {
                Id = SpecieId.CastformRainy,
                Num = 351,
                Name = "Castform-Rainy",
                BaseSpecies = SpecieId.Castform,
                Forme = FormeId.Rainy,
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 70,
                    Def = 70,
                    SpA = 70,
                    SpD = 70,
                    Spe = 70,
                },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Forecast,
                    },
                    HeightM = 0.3,
                    WeightKg = 0.8,
                    Color = "Blue",
                },
            [SpecieId.CastformSnowy] = new()
            {
                Id = SpecieId.CastformSnowy,
                Num = 351,
                Name = "Castform-Snowy",
                BaseSpecies = SpecieId.Castform,
                Forme = FormeId.Snowy,
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 70,
                    Def = 70,
                    SpA = 70,
                    SpD = 70,
                    Spe = 70,
                },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Forecast,
                    },
                    HeightM = 0.3,
                    WeightKg = 0.8,
                    Color = "White",
                },
            [SpecieId.Kecleon] = new()
            {
                Id = SpecieId.Kecleon,
                Num = 352,
                Name = "Kecleon",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 90,
                    Def = 70,
                    SpA = 60,
                    SpD = 120,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ColorChange,
                    Hidden = AbilityId.Protean,
                },
                HeightM = 1,
                WeightKg = 22,
                Color = "Green",
            },
            [SpecieId.Shuppet] = new()
            {
                Id = SpecieId.Shuppet,
                Num = 353,
                Name = "Shuppet",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 44,
                    Atk = 75,
                    Def = 35,
                    SpA = 63,
                    SpD = 33,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.CursedBody,
                },
                HeightM = 0.6,
                WeightKg = 2.3,
                Color = "Black",
            },
            [SpecieId.Banette] = new()
            {
                Id = SpecieId.Banette,
                Num = 354,
                Name = "Banette",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 64,
                    Atk = 115,
                    Def = 65,
                    SpA = 83,
                    SpD = 63,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Insomnia,
                    Slot1 = AbilityId.Frisk,
                    Hidden = AbilityId.CursedBody,
                },
                HeightM = 1.1,
                WeightKg = 12.5,
                Color = "Black",
                Prevo = SpecieId.Shuppet,
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.BanetteMega] = new()
            {
                Id = SpecieId.BanetteMega,
                Num = 354,
                Name = "Banette-Mega",
                BaseSpecies = SpecieId.Banette,
                Forme = FormeId.Mega,
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 64,
                    Atk = 165,
                    Def = 75,
                    SpA = 93,
                    SpD = 83,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                },
                HeightM = 1.2,
                WeightKg = 13,
                Color = "Black",
                RequiredItem = Sim.Items.ItemId.Banettite,
            },
            [SpecieId.Duskull] = new()
            {
                Id = SpecieId.Duskull,
                Num = 355,
                Name = "Duskull",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 20,
                    Atk = 40,
                    Def = 90,
                    SpA = 30,
                    SpD = 90,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 0.8,
                WeightKg = 15,
                Color = "Black",
            },
            [SpecieId.Dusclops] = new()
            {
                Id = SpecieId.Dusclops,
                Num = 356,
                Name = "Dusclops",
                Types = [PokemonType.Ghost],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 70,
                    Def = 130,
                    SpA = 60,
                    SpD = 130,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Hidden = AbilityId.Frisk,
                },
                HeightM = 1.6,
                WeightKg = 30.6,
                Color = "Black",
                Prevo = SpecieId.Duskull,
            },
            [SpecieId.Tropius] = new()
            {
                Id = SpecieId.Tropius,
                Num = 357,
                Name = "Tropius",
                Types = [PokemonType.Grass, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 99,
                    Atk = 68,
                    Def = 83,
                    SpA = 72,
                    SpD = 87,
                    Spe = 51,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.SolarPower,
                    Hidden = AbilityId.Harvest,
                },
                HeightM = 2,
                WeightKg = 100,
                Color = "Green",
            },
            [SpecieId.Chimecho] = new()
            {
                Id = SpecieId.Chimecho,
                Num = 358,
                Name = "Chimecho",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 50,
                    Def = 80,
                    SpA = 95,
                    SpD = 90,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.6,
                WeightKg = 1,
                Color = "Blue",
            },
            [SpecieId.Absol] = new()
            {
                Id = SpecieId.Absol,
                Num = 359,
                Name = "Absol",
                Types = [PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 130,
                    Def = 60,
                    SpA = 75,
                    SpD = 60,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Pressure,
                    Slot1 = AbilityId.SuperLuck,
                    Hidden = AbilityId.Justified,
                },
                HeightM = 1.2,
                WeightKg = 47,
                Color = "White",
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.AbsolMega] = new()
            {
                Id = SpecieId.AbsolMega,
                Num = 359,
                Name = "Absol-Mega",
                BaseSpecies = SpecieId.Absol,
                Forme = FormeId.Mega,
                Types = [PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 150,
                    Def = 60,
                    SpA = 115,
                    SpD = 60,
                    Spe = 115,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MagicBounce,
                },
                HeightM = 1.2,
                WeightKg = 49,
                Color = "White",
                RequiredItem = Sim.Items.ItemId.Absolite,
            },
            [SpecieId.Wynaut] = new()
            {
                Id = SpecieId.Wynaut,
                Num = 360,
                Name = "Wynaut",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 23,
                    Def = 48,
                    SpA = 23,
                    SpD = 48,
                    Spe = 23,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShadowTag,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 0.6,
                WeightKg = 14,
                Color = "Blue",
            },
            [SpecieId.Snorunt] = new()
            {
                Id = SpecieId.Snorunt,
                Num = 361,
                Name = "Snorunt",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 50,
                    Def = 50,
                    SpA = 50,
                    SpD = 50,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Slot1 = AbilityId.IceBody,
                    Hidden = AbilityId.Moody,
                },
                HeightM = 0.7,
                WeightKg = 16.8,
                Color = "Gray",
            },
            [SpecieId.Glalie] = new()
            {
                Id = SpecieId.Glalie,
                Num = 362,
                Name = "Glalie",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 80,
                    Def = 80,
                    SpA = 80,
                    SpD = 80,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Slot1 = AbilityId.IceBody,
                    Hidden = AbilityId.Moody,
                },
                HeightM = 1.5,
                WeightKg = 256.5,
                Color = "Gray",
                Prevo = SpecieId.Snorunt,
                OtherFormes = [FormeId.Mega],
            },
            [SpecieId.GlalieMega] = new()
            {
                Id = SpecieId.GlalieMega,
                Num = 362,
                Name = "Glalie-Mega",
                BaseSpecies = SpecieId.Glalie,
                Forme = FormeId.Mega,
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 80,
                    SpA = 120,
                    SpD = 80,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Refrigerate,
                },
                HeightM = 2.1,
                WeightKg = 350.2,
                Color = "Gray",
                RequiredItem = Sim.Items.ItemId.Glalitite,
            },
            [SpecieId.Spheal] = new()
            {
                Id = SpecieId.Spheal,
                Num = 363,
                Name = "Spheal",
                Types = [PokemonType.Ice, PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 40,
                    Def = 50,
                    SpA = 55,
                    SpD = 50,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ThickFat,
                    Slot1 = AbilityId.IceBody,
                    Hidden = AbilityId.Oblivious,
                },
                HeightM = 0.8,
                WeightKg = 39.5,
                Color = "Blue",
            },
                        [SpecieId.Sealeo] = new()
                        {
                            Id = SpecieId.Sealeo,
                            Num = 364,
                            Name = "Sealeo",
                            Types = [PokemonType.Ice, PokemonType.Water],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 90,
                                Atk = 60,
                                Def = 70,
                                SpA = 75,
                                SpD = 70,
                                Spe = 45,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.ThickFat,
                                Slot1 = AbilityId.IceBody,
                                Hidden = AbilityId.Oblivious,
                            },
                            HeightM = 1.1,
                            WeightKg = 87.6,
                            Color = "Blue",
                            Prevo = SpecieId.Spheal,
                        },
                        [SpecieId.Walrein] = new()
                        {
                            Id = SpecieId.Walrein,
                            Num = 365,
                            Name = "Walrein",
                            Types = [PokemonType.Ice, PokemonType.Water],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 110,
                                Atk = 80,
                                Def = 90,
                                SpA = 95,
                                SpD = 90,
                                Spe = 65,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.ThickFat,
                                Slot1 = AbilityId.IceBody,
                                Hidden = AbilityId.Oblivious,
                            },
                            HeightM = 1.4,
                            WeightKg = 150.6,
                            Color = "Blue",
                            Prevo = SpecieId.Sealeo,
                        },
                        [SpecieId.Clamperl] = new()
                        {
                            Id = SpecieId.Clamperl,
                            Num = 366,
                            Name = "Clamperl",
                            Types = [PokemonType.Water],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 35,
                                Atk = 64,
                                Def = 85,
                                SpA = 74,
                                SpD = 55,
                                Spe = 32,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.ShellArmor,
                                Hidden = AbilityId.Rattled,
                            },
                            HeightM = 0.4,
                            WeightKg = 52.5,
                            Color = "Blue",
                        },
                        [SpecieId.Huntail] = new()
                        {
                            Id = SpecieId.Huntail,
                            Num = 367,
                            Name = "Huntail",
                            Types = [PokemonType.Water],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 55,
                                Atk = 104,
                                Def = 105,
                                SpA = 94,
                                SpD = 75,
                                Spe = 52,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.SwiftSwim,
                                Hidden = AbilityId.WaterVeil,
                            },
                            HeightM = 1.7,
                            WeightKg = 27,
                            Color = "Blue",
                            Prevo = SpecieId.Clamperl,
                        },
                        [SpecieId.Gorebyss] = new()
                        {
                            Id = SpecieId.Gorebyss,
                            Num = 368,
                            Name = "Gorebyss",
                            Types = [PokemonType.Water],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 55,
                                Atk = 84,
                                Def = 105,
                                SpA = 114,
                                SpD = 75,
                                Spe = 52,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.SwiftSwim,
                                Hidden = AbilityId.Hydration,
                            },
                            HeightM = 1.8,
                            WeightKg = 22.6,
                            Color = "Pink",
                            Prevo = SpecieId.Clamperl,
                        },
                        [SpecieId.Relicanth] = new()
                        {
                            Id = SpecieId.Relicanth,
                            Num = 369,
                            Name = "Relicanth",
                            Types = [PokemonType.Water, PokemonType.Rock],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 100,
                                Atk = 90,
                                Def = 130,
                                SpA = 45,
                                SpD = 65,
                                Spe = 55,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.SwiftSwim,
                                Slot1 = AbilityId.RockHead,
                                Hidden = AbilityId.Sturdy,
                            },
                            HeightM = 1,
                            WeightKg = 23.4,
                            Color = "Gray",
                        },
                        [SpecieId.Luvdisc] = new()
                        {
                            Id = SpecieId.Luvdisc,
                            Num = 370,
                            Name = "Luvdisc",
                            Types = [PokemonType.Water],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 43,
                                Atk = 30,
                                Def = 55,
                                SpA = 40,
                                SpD = 65,
                                Spe = 97,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.SwiftSwim,
                                Hidden = AbilityId.Hydration,
                            },
                            HeightM = 0.6,
                            WeightKg = 8.7,
                            Color = "Pink",
                        },
                        [SpecieId.Bagon] = new()
                        {
                            Id = SpecieId.Bagon,
                            Num = 371,
                            Name = "Bagon",
                            Types = [PokemonType.Dragon],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 45,
                                Atk = 75,
                                Def = 60,
                                SpA = 40,
                                SpD = 30,
                                Spe = 50,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.RockHead,
                                Hidden = AbilityId.SheerForce,
                            },
                            HeightM = 0.6,
                            WeightKg = 42.1,
                            Color = "Blue",
                        },
                        [SpecieId.Shelgon] = new()
                        {
                            Id = SpecieId.Shelgon,
                            Num = 372,
                            Name = "Shelgon",
                            Types = [PokemonType.Dragon],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 65,
                                Atk = 95,
                                Def = 100,
                                SpA = 60,
                                SpD = 50,
                                Spe = 50,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.RockHead,
                                Hidden = AbilityId.Overcoat,
                            },
                            HeightM = 1.1,
                            WeightKg = 110.5,
                            Color = "White",
                            Prevo = SpecieId.Bagon,
                        },
                        [SpecieId.Salamence] = new()
                        {
                            Id = SpecieId.Salamence,
                            Num = 373,
                            Name = "Salamence",
                            Types = [PokemonType.Dragon, PokemonType.Flying],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 95,
                                Atk = 135,
                                Def = 80,
                                SpA = 110,
                                SpD = 80,
                                Spe = 100,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Intimidate,
                                Hidden = AbilityId.Moxie,
                            },
                            HeightM = 1.5,
                            WeightKg = 102.6,
                            Color = "Blue",
                            Prevo = SpecieId.Shelgon,
                            OtherFormes = [FormeId.Mega],
                        },
                        [SpecieId.SalamenceMega] = new()
                        {
                            Id = SpecieId.SalamenceMega,
                            Num = 373,
                            Name = "Salamence-Mega",
                            BaseSpecies = SpecieId.Salamence,
                            Forme = FormeId.Mega,
                            Types = [PokemonType.Dragon, PokemonType.Flying],
                            Gender = GenderId.Empty,
                            BaseStats = new StatsTable
                            {
                                Hp = 95,
                                Atk = 145,
                                Def = 130,
                                SpA = 120,
                                SpD = 90,
                                Spe = 120,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Aerilate,
                            },
                            HeightM = 1.8,
                            WeightKg = 112.6,
                            Color = "Blue",
                            RequiredItem = Sim.Items.ItemId.Salamencite,
                        },
                        [SpecieId.Beldum] = new()
                        {
                            Id = SpecieId.Beldum,
                            Num = 374,
                            Name = "Beldum",
                            Types = [PokemonType.Steel, PokemonType.Psychic],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 40,
                                Atk = 55,
                                Def = 80,
                                SpA = 35,
                                SpD = 60,
                                Spe = 30,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.ClearBody,
                                Hidden = AbilityId.LightMetal,
                            },
                            HeightM = 0.6,
                            WeightKg = 95.2,
                            Color = "Blue",
                        },
                        [SpecieId.Metang] = new()
                        {
                            Id = SpecieId.Metang,
                            Num = 375,
                            Name = "Metang",
                            Types = [PokemonType.Steel, PokemonType.Psychic],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 60,
                                Atk = 75,
                                Def = 100,
                                SpA = 55,
                                SpD = 80,
                                Spe = 50,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.ClearBody,
                                Hidden = AbilityId.LightMetal,
                            },
                            HeightM = 1.2,
                            WeightKg = 202.5,
                            Color = "Blue",
                            Prevo = SpecieId.Beldum,
                        },
                        [SpecieId.Metagross] = new()
                        {
                            Id = SpecieId.Metagross,
                            Num = 376,
                            Name = "Metagross",
                            Types = [PokemonType.Steel, PokemonType.Psychic],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 80,
                                Atk = 135,
                                Def = 130,
                                SpA = 95,
                                SpD = 90,
                                Spe = 70,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.ClearBody,
                                Hidden = AbilityId.LightMetal,
                            },
                            HeightM = 1.6,
                            WeightKg = 550,
                            Color = "Blue",
                            Prevo = SpecieId.Metang,
                            OtherFormes = [FormeId.Mega],
                        },
                        [SpecieId.MetagrossMega] = new()
                        {
                            Id = SpecieId.MetagrossMega,
                            Num = 376,
                            Name = "Metagross-Mega",
                            BaseSpecies = SpecieId.Metagross,
                            Forme = FormeId.Mega,
                            Types = [PokemonType.Steel, PokemonType.Psychic],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 80,
                                Atk = 145,
                                Def = 150,
                                SpA = 105,
                                SpD = 110,
                                Spe = 110,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.ToughClaws,
                            },
                            HeightM = 2.5,
                            WeightKg = 942.9,
                            Color = "Blue",
                            RequiredItem = Sim.Items.ItemId.Metagrossite,
                        },
                        [SpecieId.Regirock] = new()
                        {
                            Id = SpecieId.Regirock,
                            Num = 377,
                            Name = "Regirock",
                            Types = [PokemonType.Rock],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 80,
                                Atk = 100,
                                Def = 200,
                                SpA = 50,
                                SpD = 100,
                                Spe = 50,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.ClearBody,
                                Hidden = AbilityId.Sturdy,
                            },
                            HeightM = 1.7,
                            WeightKg = 230,
                            Color = "Brown",
                        },
                        [SpecieId.Regice] = new()
                        {
                            Id = SpecieId.Regice,
                            Num = 378,
                            Name = "Regice",
                            Types = [PokemonType.Ice],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 80,
                                Atk = 50,
                                Def = 100,
                                SpA = 100,
                                SpD = 200,
                                Spe = 50,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.ClearBody,
                                Hidden = AbilityId.IceBody,
                            },
                            HeightM = 1.8,
                            WeightKg = 175,
                            Color = "Blue",
                        },
                        [SpecieId.Registeel] = new()
                        {
                            Id = SpecieId.Registeel,
                            Num = 379,
                            Name = "Registeel",
                            Types = [PokemonType.Steel],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 80,
                                Atk = 75,
                                Def = 150,
                                SpA = 75,
                                SpD = 150,
                                Spe = 50,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.ClearBody,
                                Hidden = AbilityId.LightMetal,
                            },
                            HeightM = 1.9,
                            WeightKg = 205,
                            Color = "Gray",
                        },
                        [SpecieId.Latias] = new()
                        {
                            Id = SpecieId.Latias,
                            Num = 380,
                            Name = "Latias",
                            Types = [PokemonType.Dragon, PokemonType.Psychic],
                            Gender = GenderId.F,
                            BaseStats = new StatsTable
                            {
                                Hp = 80,
                                Atk = 80,
                                Def = 90,
                                SpA = 110,
                                SpD = 130,
                                Spe = 110,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Levitate,
                            },
                            HeightM = 1.4,
                            WeightKg = 40,
                            Color = "Red",
                            OtherFormes = [FormeId.Mega],
                        },
                        [SpecieId.LatiasMega] = new()
                        {
                            Id = SpecieId.LatiasMega,
                            Num = 380,
                            Name = "Latias-Mega",
                            BaseSpecies = SpecieId.Latias,
                            Forme = FormeId.Mega,
                            Types = [PokemonType.Dragon, PokemonType.Psychic],
                            Gender = GenderId.F,
                            BaseStats = new StatsTable
                            {
                                Hp = 80,
                                Atk = 100,
                                Def = 120,
                                SpA = 140,
                                SpD = 150,
                                Spe = 110,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Levitate,
                            },
                            HeightM = 1.8,
                            WeightKg = 52,
                            Color = "Purple",
                            RequiredItem = Sim.Items.ItemId.Latiasite,
                        },
                        [SpecieId.Latios] = new()
                        {
                            Id = SpecieId.Latios,
                            Num = 381,
                            Name = "Latios",
                            Types = [PokemonType.Dragon, PokemonType.Psychic],
                            Gender = GenderId.M,
                            BaseStats = new StatsTable
                            {
                                Hp = 80,
                                Atk = 90,
                                Def = 80,
                                SpA = 130,
                                SpD = 110,
                                Spe = 110,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Levitate,
                            },
                            HeightM = 2,
                            WeightKg = 60,
                            Color = "Blue",
                            OtherFormes = [FormeId.Mega],
                        },
                        [SpecieId.LatiosMega] = new()
                        {
                            Id = SpecieId.LatiosMega,
                            Num = 381,
                            Name = "Latios-Mega",
                            BaseSpecies = SpecieId.Latios,
                            Forme = FormeId.Mega,
                            Types = [PokemonType.Dragon, PokemonType.Psychic],
                            Gender = GenderId.M,
                            BaseStats = new StatsTable
                            {
                                Hp = 80,
                                Atk = 130,
                                Def = 100,
                                SpA = 160,
                                SpD = 120,
                                Spe = 110,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Levitate,
                            },
                            HeightM = 2.3,
                            WeightKg = 70,
                            Color = "Purple",
                            RequiredItem = Sim.Items.ItemId.Latiosite,
                        },
                        [SpecieId.Kyogre] = new()
                        {
                            Id = SpecieId.Kyogre,
                            Num = 382,
                            Name = "Kyogre",
                            Types = [PokemonType.Water],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 100,
                                Atk = 100,
                                Def = 90,
                                SpA = 150,
                                SpD = 140,
                                Spe = 90,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Drizzle,
                            },
                            HeightM = 4.5,
                            WeightKg = 352,
                            Color = "Blue",
                            OtherFormes = [FormeId.Primal],
                        },
                        [SpecieId.KyogrePrimal] = new()
                        {
                            Id = SpecieId.KyogrePrimal,
                            Num = 382,
                            Name = "Kyogre-Primal",
                            BaseSpecies = SpecieId.Kyogre,
                            Forme = FormeId.Primal,
                            Types = [PokemonType.Water],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 100,
                                Atk = 150,
                                Def = 90,
                                SpA = 180,
                                SpD = 160,
                                Spe = 90,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.PrimordialSea,
                            },
                            HeightM = 9.8,
                            WeightKg = 430,
                            Color = "Blue",
                            RequiredItem = Sim.Items.ItemId.BlueOrb,
                        },
                        [SpecieId.Groudon] = new()
                        {
                            Id = SpecieId.Groudon,
                            Num = 383,
                            Name = "Groudon",
                            Types = [PokemonType.Ground],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 100,
                                Atk = 150,
                                Def = 140,
                                SpA = 100,
                                SpD = 90,
                                Spe = 90,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Drought,
                            },
                            HeightM = 3.5,
                            WeightKg = 950,
                            Color = "Red",
                            OtherFormes = [FormeId.Primal],
                        },
                        [SpecieId.GroudonPrimal] = new()
                        {
                            Id = SpecieId.GroudonPrimal,
                            Num = 383,
                            Name = "Groudon-Primal",
                            BaseSpecies = SpecieId.Groudon,
                            Forme = FormeId.Primal,
                            Types = [PokemonType.Ground, PokemonType.Fire],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 100,
                                Atk = 180,
                                Def = 160,
                                SpA = 150,
                                SpD = 90,
                                Spe = 90,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.DesolateLand,
                            },
                            HeightM = 5,
                            WeightKg = 999.7,
                            Color = "Red",
                            RequiredItem = Sim.Items.ItemId.RedOrb,
                        },
                        [SpecieId.Rayquaza] = new()
                        {
                            Id = SpecieId.Rayquaza,
                            Num = 384,
                            Name = "Rayquaza",
                            Types = [PokemonType.Dragon, PokemonType.Flying],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 105,
                                Atk = 150,
                                Def = 90,
                                SpA = 150,
                                SpD = 90,
                                Spe = 95,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.AirLock,
                            },
                            HeightM = 7,
                            WeightKg = 206.5,
                            Color = "Green",
                            OtherFormes = [FormeId.Mega],
                        },
                        [SpecieId.RayquazaMega] = new()
                        {
                            Id = SpecieId.RayquazaMega,
                            Num = 384,
                            Name = "Rayquaza-Mega",
                            BaseSpecies = SpecieId.Rayquaza,
                            Forme = FormeId.Mega,
                            Types = [PokemonType.Dragon, PokemonType.Flying],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 105,
                                Atk = 180,
                                Def = 100,
                                SpA = 180,
                                SpD = 100,
                                Spe = 115,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.DeltaStream,
                            },
                            HeightM = 10.8,
                            WeightKg = 392,
                            Color = "Green",
                        },
                        [SpecieId.Jirachi] = new()
                        {
                            Id = SpecieId.Jirachi,
                            Num = 385,
                            Name = "Jirachi",
                            Types = [PokemonType.Steel, PokemonType.Psychic],
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
                                Slot0 = AbilityId.SereneGrace,
                            },
                            HeightM = 0.3,
                            WeightKg = 1.1,
                            Color = "Yellow",
                        },
                        [SpecieId.Deoxys] = new()
                        {
                            Id = SpecieId.Deoxys,
                            Num = 386,
                            Name = "Deoxys",
                            Types = [PokemonType.Psychic],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 50,
                                Atk = 150,
                                Def = 50,
                                SpA = 150,
                                SpD = 50,
                                Spe = 150,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Pressure,
                            },
                            HeightM = 1.7,
                            WeightKg = 60.8,
                            Color = "Red",
                            OtherFormes = [FormeId.Attack, FormeId.Defense, FormeId.Speed],
                        },
                        [SpecieId.DeoxysAttack] = new()
                        {
                            Id = SpecieId.DeoxysAttack,
                            Num = 386,
                            Name = "Deoxys-Attack",
                            BaseSpecies = SpecieId.Deoxys,
                            Forme = FormeId.Attack,
                            Types = [PokemonType.Psychic],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 50,
                                Atk = 180,
                                Def = 20,
                                SpA = 180,
                                SpD = 20,
                                Spe = 150,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Pressure,
                            },
                            HeightM = 1.7,
                            WeightKg = 60.8,
                            Color = "Red",
                        },
                        [SpecieId.DeoxysDefense] = new()
                        {
                            Id = SpecieId.DeoxysDefense,
                            Num = 386,
                            Name = "Deoxys-Defense",
                            BaseSpecies = SpecieId.Deoxys,
                            Forme = FormeId.Defense,
                            Types = [PokemonType.Psychic],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 50,
                                Atk = 70,
                                Def = 160,
                                SpA = 70,
                                SpD = 160,
                                Spe = 90,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Pressure,
                            },
                            HeightM = 1.7,
                            WeightKg = 60.8,
                            Color = "Red",
                        },
                        [SpecieId.DeoxysSpeed] = new()
                        {
                            Id = SpecieId.DeoxysSpeed,
                            Num = 386,
                            Name = "Deoxys-Speed",
                            BaseSpecies = SpecieId.Deoxys,
                            Forme = FormeId.Speed,
                            Types = [PokemonType.Psychic],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 50,
                                Atk = 95,
                                Def = 90,
                                SpA = 95,
                                SpD = 90,
                                Spe = 180,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Pressure,
                            },
                            HeightM = 1.7,
                            WeightKg = 60.8,
                            Color = "Red",
                        },
                    };
                }
            }
