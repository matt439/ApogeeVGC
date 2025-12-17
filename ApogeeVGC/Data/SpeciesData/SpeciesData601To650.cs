using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData601To650()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Klinklang] = new()
            {
                Id = SpecieId.Klinklang,
                Num = 601,
                Name = "Klinklang",
                Types = [PokemonType.Steel],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 100,
                    Def = 115,
                    SpA = 70,
                    SpD = 85,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Plus,
                    Slot1 = AbilityId.Minus,
                    Hidden = AbilityId.ClearBody,
                },
                HeightM = 0.6,
                WeightKg = 81,
                Color = "Gray",
                Prevo = SpecieId.Klang,
            },
            [SpecieId.Tynamo] = new()
            {
                Id = SpecieId.Tynamo,
                Num = 602,
                Name = "Tynamo",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 35,
                    Atk = 55,
                    Def = 40,
                    SpA = 45,
                    SpD = 40,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.2,
                WeightKg = 0.3,
                Color = "White",
            },
            [SpecieId.Eelektrik] = new()
            {
                Id = SpecieId.Eelektrik,
                Num = 603,
                Name = "Eelektrik",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 85,
                    Def = 70,
                    SpA = 75,
                    SpD = 70,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 1.2,
                WeightKg = 22,
                Color = "Blue",
                Prevo = SpecieId.Tynamo,
            },
            [SpecieId.Eelektross] = new()
            {
                Id = SpecieId.Eelektross,
                Num = 604,
                Name = "Eelektross",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 115,
                    Def = 80,
                    SpA = 105,
                    SpD = 80,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 2.1,
                WeightKg = 80.5,
                Color = "Blue",
                Prevo = SpecieId.Eelektrik,
            },
            [SpecieId.EelektrossMega] = new()
            {
                Id = SpecieId.EelektrossMega,
                Num = 604,
                Name = "Eelektross-Mega",
                BaseSpecies = SpecieId.Eelektross,
                Forme = FormeId.Mega,
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 145,
                    Def = 80,
                    SpA = 135,
                    SpD = 90,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 3,
                WeightKg = 160,
                Color = "Blue",
            },
            [SpecieId.Elgyem] = new()
            {
                Id = SpecieId.Elgyem,
                Num = 605,
                Name = "Elgyem",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 55,
                    Def = 55,
                    SpA = 85,
                    SpD = 55,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Telepathy,
                    Slot1 = AbilityId.Synchronize,
                    Hidden = AbilityId.Analytic,
                },
                HeightM = 0.5,
                WeightKg = 9,
                Color = "Blue",
            },
            [SpecieId.Beheeyem] = new()
            {
                Id = SpecieId.Beheeyem,
                Num = 606,
                Name = "Beheeyem",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 75,
                    Def = 75,
                    SpA = 125,
                    SpD = 95,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Telepathy,
                    Slot1 = AbilityId.Synchronize,
                    Hidden = AbilityId.Analytic,
                },
                HeightM = 1,
                WeightKg = 34.5,
                Color = "Brown",
                Prevo = SpecieId.Elgyem,
            },
            [SpecieId.Litwick] = new()
            {
                Id = SpecieId.Litwick,
                Num = 607,
                Name = "Litwick",
                Types = [PokemonType.Ghost, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 30,
                    Def = 55,
                    SpA = 65,
                    SpD = 55,
                    Spe = 20,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlashFire,
                    Slot1 = AbilityId.FlameBody,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 0.3,
                WeightKg = 3.1,
                Color = "White",
            },
            [SpecieId.Lampent] = new()
            {
                Id = SpecieId.Lampent,
                Num = 608,
                Name = "Lampent",
                Types = [PokemonType.Ghost, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 40,
                    Def = 60,
                    SpA = 95,
                    SpD = 60,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlashFire,
                    Slot1 = AbilityId.FlameBody,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 0.6,
                WeightKg = 13,
                Color = "Black",
                Prevo = SpecieId.Litwick,
            },
            [SpecieId.Chandelure] = new()
            {
                Id = SpecieId.Chandelure,
                Num = 609,
                Name = "Chandelure",
                Types = [PokemonType.Ghost, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 55,
                    Def = 90,
                    SpA = 145,
                    SpD = 90,
                    Spe = 80,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlashFire,
                    Slot1 = AbilityId.FlameBody,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 1,
                WeightKg = 34.3,
                Color = "Black",
                Prevo = SpecieId.Lampent,
            },
            [SpecieId.ChandelureMega] = new()
            {
                Id = SpecieId.ChandelureMega,
                Num = 609,
                Name = "Chandelure-Mega",
                BaseSpecies = SpecieId.Chandelure,
                Forme = FormeId.Mega,
                Types = [PokemonType.Ghost, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 75,
                    Def = 110,
                    SpA = 175,
                    SpD = 110,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlashFire,
                    Slot1 = AbilityId.FlameBody,
                    Hidden = AbilityId.Infiltrator,
                },
                HeightM = 2.5,
                WeightKg = 69.6,
                Color = "Black",
            },
            [SpecieId.Axew] = new()
            {
                Id = SpecieId.Axew,
                Num = 610,
                Name = "Axew",
                Types = [PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 46,
                    Atk = 87,
                    Def = 60,
                    SpA = 30,
                    SpD = 40,
                    Spe = 57,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.MoldBreaker,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 0.6,
                WeightKg = 18,
                Color = "Green",
            },
            [SpecieId.Fraxure] = new()
            {
                Id = SpecieId.Fraxure,
                Num = 611,
                Name = "Fraxure",
                Types = [PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 66,
                    Atk = 117,
                    Def = 70,
                    SpA = 40,
                    SpD = 50,
                    Spe = 67,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.MoldBreaker,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 1,
                WeightKg = 36,
                Color = "Green",
                Prevo = SpecieId.Axew,
            },
            [SpecieId.Haxorus] = new()
            {
                Id = SpecieId.Haxorus,
                Num = 612,
                Name = "Haxorus",
                Types = [PokemonType.Dragon],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 76,
                    Atk = 147,
                    Def = 90,
                    SpA = 60,
                    SpD = 70,
                    Spe = 97,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.MoldBreaker,
                    Hidden = AbilityId.Unnerve,
                },
                HeightM = 1.8,
                WeightKg = 105.5,
                Color = "Yellow",
                Prevo = SpecieId.Fraxure,
            },
            [SpecieId.Cubchoo] = new()
            {
                Id = SpecieId.Cubchoo,
                Num = 613,
                Name = "Cubchoo",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 55,
                    Atk = 70,
                    Def = 40,
                    SpA = 60,
                    SpD = 40,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowCloak,
                    Slot1 = AbilityId.SlushRush,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 0.5,
                WeightKg = 8.5,
                Color = "White",
            },
            [SpecieId.Beartic] = new()
            {
                Id = SpecieId.Beartic,
                Num = 614,
                Name = "Beartic",
                Types = [PokemonType.Ice],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 130,
                    Def = 80,
                    SpA = 70,
                    SpD = 80,
                    Spe = 50,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SnowCloak,
                    Slot1 = AbilityId.SlushRush,
                    Hidden = AbilityId.SwiftSwim,
                },
                HeightM = 2.6,
                WeightKg = 260,
                Color = "White",
                Prevo = SpecieId.Cubchoo,
            },
            [SpecieId.Cryogonal] = new()
            {
                Id = SpecieId.Cryogonal,
                Num = 615,
                Name = "Cryogonal",
                Types = [PokemonType.Ice],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 50,
                    Def = 50,
                    SpA = 95,
                    SpD = 135,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                    HeightM = 1.1,
                    WeightKg = 148,
                    Color = "Blue",
                },
                [SpecieId.Shelmet] = new()
                {
                    Id = SpecieId.Shelmet,
                    Num = 616,
                    Name = "Shelmet",
                    Types = [PokemonType.Bug],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 50,
                        Atk = 40,
                        Def = 85,
                        SpA = 40,
                        SpD = 65,
                        Spe = 25,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Hydration,
                        Slot1 = AbilityId.ShellArmor,
                        Hidden = AbilityId.Overcoat,
                    },
                    HeightM = 0.4,
                    WeightKg = 7.7,
                    Color = "Red",
                },
                [SpecieId.Accelgor] = new()
                {
                    Id = SpecieId.Accelgor,
                    Num = 617,
                    Name = "Accelgor",
                    Types = [PokemonType.Bug],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 80,
                        Atk = 70,
                        Def = 40,
                        SpA = 100,
                        SpD = 60,
                        Spe = 145,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Hydration,
                        Slot1 = AbilityId.StickyHold,
                        Hidden = AbilityId.Unburden,
                    },
                    HeightM = 0.8,
                    WeightKg = 25.3,
                    Color = "Red",
                    Prevo = SpecieId.Shelmet,
                },
                [SpecieId.Stunfisk] = new()
                {
                    Id = SpecieId.Stunfisk,
                    Num = 618,
                    Name = "Stunfisk",
                    Types = [PokemonType.Ground, PokemonType.Electric],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 109,
                        Atk = 66,
                        Def = 84,
                        SpA = 81,
                        SpD = 99,
                        Spe = 32,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Static,
                        Slot1 = AbilityId.Limber,
                        Hidden = AbilityId.SandVeil,
                    },
                    HeightM = 0.7,
                    WeightKg = 11,
                    Color = "Brown",
                },
                [SpecieId.StunfiskGalar] = new()
                {
                    Id = SpecieId.StunfiskGalar,
                    Num = 618,
                    Name = "Stunfisk-Galar",
                    BaseSpecies = SpecieId.Stunfisk,
                    Forme = FormeId.Galar,
                    Types = [PokemonType.Ground, PokemonType.Steel],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 109,
                        Atk = 81,
                        Def = 99,
                        SpA = 66,
                        SpD = 84,
                        Spe = 32,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Mimicry,
                    },
                    HeightM = 0.7,
                    WeightKg = 20.5,
                    Color = "Green",
                },
                [SpecieId.Mienfoo] = new()
                {
                    Id = SpecieId.Mienfoo,
                    Num = 619,
                    Name = "Mienfoo",
                    Types = [PokemonType.Fighting],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 45,
                        Atk = 85,
                        Def = 50,
                        SpA = 55,
                        SpD = 50,
                        Spe = 65,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.InnerFocus,
                        Slot1 = AbilityId.Regenerator,
                        Hidden = AbilityId.Reckless,
                    },
                    HeightM = 0.9,
                    WeightKg = 20,
                    Color = "Yellow",
                },
                [SpecieId.Mienshao] = new()
                {
                    Id = SpecieId.Mienshao,
                    Num = 620,
                    Name = "Mienshao",
                    Types = [PokemonType.Fighting],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 65,
                        Atk = 125,
                        Def = 60,
                        SpA = 95,
                        SpD = 60,
                        Spe = 105,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.InnerFocus,
                        Slot1 = AbilityId.Regenerator,
                        Hidden = AbilityId.Reckless,
                    },
                    HeightM = 1.4,
                    WeightKg = 35.5,
                    Color = "Purple",
                    Prevo = SpecieId.Mienfoo,
                },
                [SpecieId.Druddigon] = new()
                {
                    Id = SpecieId.Druddigon,
                    Num = 621,
                    Name = "Druddigon",
                    Types = [PokemonType.Dragon],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 77,
                        Atk = 120,
                        Def = 90,
                        SpA = 60,
                        SpD = 90,
                        Spe = 48,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.RoughSkin,
                        Slot1 = AbilityId.SheerForce,
                        Hidden = AbilityId.MoldBreaker,
                    },
                    HeightM = 1.6,
                    WeightKg = 139,
                    Color = "Red",
                },
                [SpecieId.Golett] = new()
                {
                    Id = SpecieId.Golett,
                    Num = 622,
                    Name = "Golett",
                    Types = [PokemonType.Ground, PokemonType.Ghost],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 59,
                        Atk = 74,
                        Def = 50,
                        SpA = 35,
                        SpD = 50,
                        Spe = 35,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.IronFist,
                        Slot1 = AbilityId.Klutz,
                        Hidden = AbilityId.NoGuard,
                    },
                    HeightM = 1,
                    WeightKg = 92,
                    Color = "Green",
                },
                [SpecieId.Golurk] = new()
                {
                    Id = SpecieId.Golurk,
                    Num = 623,
                    Name = "Golurk",
                    Types = [PokemonType.Ground, PokemonType.Ghost],
                    Gender = GenderId.N,
                    BaseStats = new StatsTable
                    {
                        Hp = 89,
                        Atk = 124,
                        Def = 80,
                        SpA = 55,
                        SpD = 80,
                        Spe = 55,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.IronFist,
                        Slot1 = AbilityId.Klutz,
                        Hidden = AbilityId.NoGuard,
                    },
                    HeightM = 2.8,
                    WeightKg = 330,
                    Color = "Green",
                    Prevo = SpecieId.Golett,
                },
                [SpecieId.Pawniard] = new()
                {
                    Id = SpecieId.Pawniard,
                    Num = 624,
                    Name = "Pawniard",
                    Types = [PokemonType.Dark, PokemonType.Steel],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 45,
                        Atk = 85,
                        Def = 70,
                        SpA = 40,
                        SpD = 40,
                        Spe = 60,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Defiant,
                        Slot1 = AbilityId.InnerFocus,
                        Hidden = AbilityId.Pressure,
                    },
                    HeightM = 0.5,
                    WeightKg = 10.2,
                    Color = "Red",
                },
                [SpecieId.Bisharp] = new()
                {
                    Id = SpecieId.Bisharp,
                    Num = 625,
                    Name = "Bisharp",
                    Types = [PokemonType.Dark, PokemonType.Steel],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 65,
                        Atk = 125,
                        Def = 100,
                        SpA = 60,
                        SpD = 70,
                        Spe = 70,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Defiant,
                        Slot1 = AbilityId.InnerFocus,
                        Hidden = AbilityId.Pressure,
                    },
                    HeightM = 1.6,
                    WeightKg = 70,
                    Color = "Red",
                    Prevo = SpecieId.Pawniard,
                },
                [SpecieId.Bouffalant] = new()
                {
                    Id = SpecieId.Bouffalant,
                    Num = 626,
                    Name = "Bouffalant",
                    Types = [PokemonType.Normal],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 95,
                        Atk = 110,
                        Def = 95,
                        SpA = 40,
                        SpD = 95,
                        Spe = 55,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Reckless,
                        Slot1 = AbilityId.SapSipper,
                        Hidden = AbilityId.Soundproof,
                    },
                    HeightM = 1.6,
                    WeightKg = 94.6,
                    Color = "Brown",
                },
                [SpecieId.Rufflet] = new()
                {
                    Id = SpecieId.Rufflet,
                    Num = 627,
                    Name = "Rufflet",
                    Types = [PokemonType.Normal, PokemonType.Flying],
                    Gender = GenderId.M,
                    BaseStats = new StatsTable
                    {
                        Hp = 70,
                        Atk = 83,
                        Def = 50,
                        SpA = 37,
                        SpD = 50,
                        Spe = 60,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.KeenEye,
                        Slot1 = AbilityId.SheerForce,
                        Hidden = AbilityId.Hustle,
                    },
                    HeightM = 0.5,
                    WeightKg = 10.5,
                    Color = "White",
                },
                [SpecieId.Braviary] = new()
                {
                    Id = SpecieId.Braviary,
                    Num = 628,
                    Name = "Braviary",
                    Types = [PokemonType.Normal, PokemonType.Flying],
                    Gender = GenderId.M,
                    BaseStats = new StatsTable
                    {
                        Hp = 100,
                        Atk = 123,
                        Def = 75,
                        SpA = 57,
                        SpD = 75,
                        Spe = 80,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.KeenEye,
                        Slot1 = AbilityId.SheerForce,
                        Hidden = AbilityId.Defiant,
                    },
                    HeightM = 1.5,
                    WeightKg = 41,
                    Color = "Red",
                    Prevo = SpecieId.Rufflet,
                },
                [SpecieId.BraviaryHisui] = new()
                {
                    Id = SpecieId.BraviaryHisui,
                    Num = 628,
                    Name = "Braviary-Hisui",
                    BaseSpecies = SpecieId.Braviary,
                    Forme = FormeId.Hisui,
                    Types = [PokemonType.Psychic, PokemonType.Flying],
                    Gender = GenderId.M,
                    BaseStats = new StatsTable
                    {
                        Hp = 110,
                        Atk = 83,
                        Def = 70,
                        SpA = 112,
                        SpD = 70,
                        Spe = 65,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.KeenEye,
                        Slot1 = AbilityId.SheerForce,
                        Hidden = AbilityId.TintedLens,
                    },
                    HeightM = 1.7,
                    WeightKg = 43.4,
                    Color = "White",
                    Prevo = SpecieId.Rufflet,
                },
                [SpecieId.Vullaby] = new()
                {
                    Id = SpecieId.Vullaby,
                    Num = 629,
                    Name = "Vullaby",
                    Types = [PokemonType.Dark, PokemonType.Flying],
                    Gender = GenderId.F,
                    BaseStats = new StatsTable
                    {
                        Hp = 70,
                        Atk = 55,
                        Def = 75,
                        SpA = 45,
                        SpD = 65,
                        Spe = 60,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.BigPecks,
                        Slot1 = AbilityId.Overcoat,
                        Hidden = AbilityId.WeakArmor,
                    },
                    HeightM = 0.5,
                    WeightKg = 9,
                    Color = "Brown",
                },
                [SpecieId.Mandibuzz] = new()
                {
                    Id = SpecieId.Mandibuzz,
                    Num = 630,
                    Name = "Mandibuzz",
                    Types = [PokemonType.Dark, PokemonType.Flying],
                    Gender = GenderId.F,
                    BaseStats = new StatsTable
                    {
                        Hp = 110,
                        Atk = 65,
                        Def = 105,
                        SpA = 55,
                        SpD = 95,
                        Spe = 80,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.BigPecks,
                        Slot1 = AbilityId.Overcoat,
                        Hidden = AbilityId.WeakArmor,
                    },
                    HeightM = 1.2,
                    WeightKg = 39.5,
                    Color = "Brown",
                    Prevo = SpecieId.Vullaby,
                },
                [SpecieId.Heatmor] = new()
                {
                    Id = SpecieId.Heatmor,
                    Num = 631,
                    Name = "Heatmor",
                    Types = [PokemonType.Fire],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 85,
                        Atk = 97,
                        Def = 66,
                        SpA = 105,
                        SpD = 66,
                        Spe = 65,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Gluttony,
                        Slot1 = AbilityId.FlashFire,
                        Hidden = AbilityId.WhiteSmoke,
                    },
                    HeightM = 1.4,
                    WeightKg = 58,
                    Color = "Red",
                },
                [SpecieId.Durant] = new()
                {
                    Id = SpecieId.Durant,
                    Num = 632,
                    Name = "Durant",
                    Types = [PokemonType.Bug, PokemonType.Steel],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 58,
                        Atk = 109,
                        Def = 112,
                        SpA = 48,
                        SpD = 48,
                        Spe = 109,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Swarm,
                        Slot1 = AbilityId.Hustle,
                        Hidden = AbilityId.Truant,
                    },
                    HeightM = 0.3,
                    WeightKg = 33,
                    Color = "Gray",
                },
                [SpecieId.Deino] = new()
                {
                    Id = SpecieId.Deino,
                    Num = 633,
                    Name = "Deino",
                    Types = [PokemonType.Dark, PokemonType.Dragon],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 52,
                        Atk = 65,
                        Def = 50,
                        SpA = 45,
                        SpD = 50,
                        Spe = 38,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Hustle,
                    },
                    HeightM = 0.8,
                    WeightKg = 17.3,
                    Color = "Blue",
                },
                [SpecieId.Zweilous] = new()
                {
                    Id = SpecieId.Zweilous,
                    Num = 634,
                    Name = "Zweilous",
                    Types = [PokemonType.Dark, PokemonType.Dragon],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 72,
                        Atk = 85,
                        Def = 70,
                        SpA = 65,
                        SpD = 70,
                        Spe = 58,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Hustle,
                    },
                    HeightM = 1.4,
                    WeightKg = 50,
                    Color = "Blue",
                    Prevo = SpecieId.Deino,
                },
                [SpecieId.Hydreigon] = new()
                {
                    Id = SpecieId.Hydreigon,
                    Num = 635,
                    Name = "Hydreigon",
                    Types = [PokemonType.Dark, PokemonType.Dragon],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 92,
                        Atk = 105,
                        Def = 90,
                        SpA = 125,
                        SpD = 90,
                        Spe = 98,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.Levitate,
                    },
                    HeightM = 1.8,
                    WeightKg = 160,
                    Color = "Blue",
                    Prevo = SpecieId.Zweilous,
                },
                [SpecieId.Larvesta] = new()
                {
                    Id = SpecieId.Larvesta,
                    Num = 636,
                    Name = "Larvesta",
                    Types = [PokemonType.Bug, PokemonType.Fire],
                    Gender = GenderId.Empty,
                    BaseStats = new StatsTable
                    {
                        Hp = 55,
                        Atk = 85,
                        Def = 55,
                        SpA = 50,
                        SpD = 55,
                        Spe = 60,
                    },
                    Abilities = new SpeciesAbility
                    {
                        Slot0 = AbilityId.FlameBody,
                        Hidden = AbilityId.Swarm,
                    },
                    HeightM = 1.1,
                    WeightKg = 28.8,
                    Color = "White",
                },
            [SpecieId.Volcarona] = new()
            {
                Id = SpecieId.Volcarona,
                Num = 637,
                Name = "Volcarona",
                Types = [PokemonType.Bug, PokemonType.Fire],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 85,
                    Atk = 60,
                    Def = 65,
                    SpA = 135,
                    SpD = 105,
                    Spe = 100,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.FlameBody,
                    Hidden = AbilityId.Swarm,
                },
                HeightM = 1.6,
                WeightKg = 46,
                Color = "White",
                Prevo = SpecieId.Larvesta,
            },
            [SpecieId.Cobalion] = new()
            {
                Id = SpecieId.Cobalion,
                Num = 638,
                Name = "Cobalion",
                Types = [PokemonType.Steel, PokemonType.Fighting],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 91,
                    Atk = 90,
                    Def = 129,
                    SpA = 90,
                    SpD = 72,
                    Spe = 108,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Justified,
                },
                HeightM = 2.1,
                WeightKg = 250,
                Color = "Blue",
            },
            [SpecieId.Terrakion] = new()
            {
                Id = SpecieId.Terrakion,
                Num = 639,
                Name = "Terrakion",
                Types = [PokemonType.Rock, PokemonType.Fighting],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 91,
                    Atk = 129,
                    Def = 90,
                    SpA = 72,
                    SpD = 90,
                    Spe = 108,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Justified,
                },
                HeightM = 1.9,
                WeightKg = 260,
                Color = "Gray",
            },
            [SpecieId.Virizion] = new()
            {
                Id = SpecieId.Virizion,
                Num = 640,
                Name = "Virizion",
                Types = [PokemonType.Grass, PokemonType.Fighting],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 91,
                    Atk = 90,
                    Def = 72,
                    SpA = 90,
                    SpD = 129,
                    Spe = 108,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Justified,
                },
                HeightM = 2,
                WeightKg = 200,
                Color = "Green",
            },
            [SpecieId.Tornadus] = new()
            {
                Id = SpecieId.Tornadus,
                Num = 641,
                Name = "Tornadus",
                BaseForme = FormeId.Incarnate,
                Types = [PokemonType.Flying],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 79,
                    Atk = 115,
                    Def = 70,
                    SpA = 125,
                    SpD = 80,
                    Spe = 111,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Hidden = AbilityId.Defiant,
                },
                HeightM = 1.5,
                WeightKg = 63,
                Color = "Green",
            },
            [SpecieId.TornadusTherian] = new()
            {
                Id = SpecieId.TornadusTherian,
                Num = 641,
                Name = "Tornadus-Therian",
                BaseSpecies = SpecieId.Tornadus,
                Forme = FormeId.Therian,
                Types = [PokemonType.Flying],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 79,
                    Atk = 100,
                    Def = 80,
                    SpA = 110,
                    SpD = 90,
                    Spe = 121,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Regenerator,
                },
                HeightM = 1.4,
                WeightKg = 63,
                Color = "Green",
            },
            [SpecieId.Thundurus] = new()
            {
                Id = SpecieId.Thundurus,
                Num = 642,
                Name = "Thundurus",
                BaseForme = FormeId.Incarnate,
                Types = [PokemonType.Electric, PokemonType.Flying],
                Gender = GenderId.M,
                BaseStats = new StatsTable
                {
                    Hp = 79,
                    Atk = 115,
                    Def = 70,
                    SpA = 125,
                    SpD = 80,
                    Spe = 111,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Hidden = AbilityId.Defiant,
                },
                HeightM = 1.5,
                WeightKg = 61,
                Color = "Blue",
            },
                        [SpecieId.ThundurusTherian] = new()
                        {
                            Id = SpecieId.ThundurusTherian,
                            Num = 642,
                            Name = "Thundurus-Therian",
                            BaseSpecies = SpecieId.Thundurus,
                            Forme = FormeId.Therian,
                            Types = [PokemonType.Electric, PokemonType.Flying],
                            Gender = GenderId.M,
                            BaseStats = new StatsTable
                            {
                                Hp = 79,
                                Atk = 105,
                                Def = 70,
                                SpA = 145,
                                SpD = 80,
                                Spe = 101,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.VoltAbsorb,
                            },
                            HeightM = 3,
                            WeightKg = 61,
                            Color = "Blue",
                        },
                        [SpecieId.Reshiram] = new()
                        {
                            Id = SpecieId.Reshiram,
                            Num = 643,
                            Name = "Reshiram",
                            Types = [PokemonType.Dragon, PokemonType.Fire],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 100,
                                Atk = 120,
                                Def = 100,
                                SpA = 150,
                                SpD = 120,
                                Spe = 90,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Turboblaze,
                            },
                            HeightM = 3.2,
                            WeightKg = 330,
                            Color = "White",
                        },
                        [SpecieId.Zekrom] = new()
                        {
                            Id = SpecieId.Zekrom,
                            Num = 644,
                            Name = "Zekrom",
                            Types = [PokemonType.Dragon, PokemonType.Electric],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 100,
                                Atk = 150,
                                Def = 120,
                                SpA = 120,
                                SpD = 100,
                                Spe = 90,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Teravolt,
                            },
                            HeightM = 2.9,
                            WeightKg = 345,
                            Color = "Black",
                        },
                        [SpecieId.Landorus] = new()
                        {
                            Id = SpecieId.Landorus,
                            Num = 645,
                            Name = "Landorus",
                            BaseForme = FormeId.Incarnate,
                            Types = [PokemonType.Ground, PokemonType.Flying],
                            Gender = GenderId.M,
                            BaseStats = new StatsTable
                            {
                                Hp = 89,
                                Atk = 125,
                                Def = 90,
                                SpA = 115,
                                SpD = 80,
                                Spe = 101,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.SandForce,
                                Hidden = AbilityId.SheerForce,
                            },
                            HeightM = 1.5,
                            WeightKg = 68,
                            Color = "Brown",
                        },
                        [SpecieId.LandorusTherian] = new()
                        {
                            Id = SpecieId.LandorusTherian,
                            Num = 645,
                            Name = "Landorus-Therian",
                            BaseSpecies = SpecieId.Landorus,
                            Forme = FormeId.Therian,
                            Types = [PokemonType.Ground, PokemonType.Flying],
                            Gender = GenderId.M,
                            BaseStats = new StatsTable
                            {
                                Hp = 89,
                                Atk = 145,
                                Def = 90,
                                SpA = 105,
                                SpD = 80,
                                Spe = 91,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Intimidate,
                            },
                            HeightM = 1.3,
                            WeightKg = 68,
                            Color = "Brown",
                        },
                        [SpecieId.Kyurem] = new()
                        {
                            Id = SpecieId.Kyurem,
                            Num = 646,
                            Name = "Kyurem",
                            Types = [PokemonType.Dragon, PokemonType.Ice],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 125,
                                Atk = 130,
                                Def = 90,
                                SpA = 130,
                                SpD = 90,
                                Spe = 95,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Pressure,
                            },
                            HeightM = 3,
                            WeightKg = 325,
                            Color = "Gray",
                        },
                        [SpecieId.KyuremBlack] = new()
                        {
                            Id = SpecieId.KyuremBlack,
                            Num = 646,
                            Name = "Kyurem-Black",
                            BaseSpecies = SpecieId.Kyurem,
                            Forme = FormeId.Black,
                            Types = [PokemonType.Dragon, PokemonType.Ice],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 125,
                                Atk = 170,
                                Def = 100,
                                SpA = 120,
                                SpD = 90,
                                Spe = 95,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Teravolt,
                            },
                            HeightM = 3.3,
                            WeightKg = 325,
                            Color = "Gray",
                        },
                        [SpecieId.KyuremWhite] = new()
                        {
                            Id = SpecieId.KyuremWhite,
                            Num = 646,
                            Name = "Kyurem-White",
                            BaseSpecies = SpecieId.Kyurem,
                            Forme = FormeId.White,
                            Types = [PokemonType.Dragon, PokemonType.Ice],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 125,
                                Atk = 120,
                                Def = 90,
                                SpA = 170,
                                SpD = 100,
                                Spe = 95,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Turboblaze,
                            },
                            HeightM = 3.6,
                            WeightKg = 325,
                            Color = "Gray",
                        },
                        [SpecieId.Keldeo] = new()
                        {
                            Id = SpecieId.Keldeo,
                            Num = 647,
                            Name = "Keldeo",
                            BaseForme = FormeId.Ordinary,
                            Types = [PokemonType.Water, PokemonType.Fighting],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 91,
                                Atk = 72,
                                Def = 90,
                                SpA = 129,
                                SpD = 90,
                                Spe = 108,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Justified,
                            },
                            HeightM = 1.4,
                            WeightKg = 48.5,
                            Color = "Yellow",
                        },
                        [SpecieId.KeldeoResolute] = new()
                        {
                            Id = SpecieId.KeldeoResolute,
                            Num = 647,
                            Name = "Keldeo-Resolute",
                            BaseSpecies = SpecieId.Keldeo,
                            Forme = FormeId.Resolute,
                            Types = [PokemonType.Water, PokemonType.Fighting],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 91,
                                Atk = 72,
                                Def = 90,
                                SpA = 129,
                                SpD = 90,
                                Spe = 108,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Justified,
                            },
                            HeightM = 1.4,
                            WeightKg = 48.5,
                            Color = "Yellow",
                        },
                        [SpecieId.Meloetta] = new()
                        {
                            Id = SpecieId.Meloetta,
                            Num = 648,
                            Name = "Meloetta",
                            BaseForme = FormeId.Aria,
                            Types = [PokemonType.Normal, PokemonType.Psychic],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 100,
                                Atk = 77,
                                Def = 77,
                                SpA = 128,
                                SpD = 128,
                                Spe = 90,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.SereneGrace,
                            },
                            HeightM = 0.6,
                            WeightKg = 6.5,
                            Color = "White",
                        },
                        [SpecieId.MeloettaPirouette] = new()
                        {
                            Id = SpecieId.MeloettaPirouette,
                            Num = 648,
                            Name = "Meloetta-Pirouette",
                            BaseSpecies = SpecieId.Meloetta,
                            Forme = FormeId.Pirouette,
                            Types = [PokemonType.Normal, PokemonType.Fighting],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 100,
                                Atk = 128,
                                Def = 90,
                                SpA = 77,
                                SpD = 77,
                                Spe = 128,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.SereneGrace,
                            },
                            HeightM = 0.6,
                            WeightKg = 6.5,
                            Color = "White",
                        },
                        [SpecieId.Genesect] = new()
                        {
                            Id = SpecieId.Genesect,
                            Num = 649,
                            Name = "Genesect",
                            Types = [PokemonType.Bug, PokemonType.Steel],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 71,
                                Atk = 120,
                                Def = 95,
                                SpA = 120,
                                SpD = 95,
                                Spe = 99,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Download,
                            },
                            HeightM = 1.5,
                            WeightKg = 82.5,
                            Color = "Purple",
                        },
                        [SpecieId.GenesectDouse] = new()
                        {
                            Id = SpecieId.GenesectDouse,
                            Num = 649,
                            Name = "Genesect-Douse",
                            BaseSpecies = SpecieId.Genesect,
                            Forme = FormeId.Douse,
                            Types = [PokemonType.Bug, PokemonType.Steel],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 71,
                                Atk = 120,
                                Def = 95,
                                SpA = 120,
                                SpD = 95,
                                Spe = 99,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Download,
                            },
                            HeightM = 1.5,
                            WeightKg = 82.5,
                            Color = "Purple",
                        },
                        [SpecieId.GenesectShock] = new()
                        {
                            Id = SpecieId.GenesectShock,
                            Num = 649,
                            Name = "Genesect-Shock",
                            BaseSpecies = SpecieId.Genesect,
                            Forme = FormeId.Shock,
                            Types = [PokemonType.Bug, PokemonType.Steel],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 71,
                                Atk = 120,
                                Def = 95,
                                SpA = 120,
                                SpD = 95,
                                Spe = 99,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Download,
                            },
                            HeightM = 1.5,
                            WeightKg = 82.5,
                            Color = "Purple",
                        },
                        [SpecieId.GenesectBurn] = new()
                        {
                            Id = SpecieId.GenesectBurn,
                            Num = 649,
                            Name = "Genesect-Burn",
                            BaseSpecies = SpecieId.Genesect,
                            Forme = FormeId.Burn,
                            Types = [PokemonType.Bug, PokemonType.Steel],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 71,
                                Atk = 120,
                                Def = 95,
                                SpA = 120,
                                SpD = 95,
                                Spe = 99,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Download,
                            },
                            HeightM = 1.5,
                            WeightKg = 82.5,
                            Color = "Purple",
                        },
                        [SpecieId.GenesectChill] = new()
                        {
                            Id = SpecieId.GenesectChill,
                            Num = 649,
                            Name = "Genesect-Chill",
                            BaseSpecies = SpecieId.Genesect,
                            Forme = FormeId.Chill,
                            Types = [PokemonType.Bug, PokemonType.Steel],
                            Gender = GenderId.N,
                            BaseStats = new StatsTable
                            {
                                Hp = 71,
                                Atk = 120,
                                Def = 95,
                                SpA = 120,
                                SpD = 95,
                                Spe = 99,
                            },
                            Abilities = new SpeciesAbility
                            {
                                Slot0 = AbilityId.Download,
                            },
                            HeightM = 1.5,
                            WeightKg = 82.5,
                            Color = "Purple",
                        },
                    };
                }
            }
