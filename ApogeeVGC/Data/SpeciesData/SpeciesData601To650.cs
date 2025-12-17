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
            [SpecieId.Volcarona] = new()
            {
                Id = SpecieId.Volcarona,
                Num = 637,
                Name = "Volcarona",
                Types = [PokemonType.Bug, PokemonType.Fire],
                Gender = GenderId.N,
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
            },
        };
    }
}
