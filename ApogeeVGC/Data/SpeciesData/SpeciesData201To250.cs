using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData201To250()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Unown] = new()
            {
                Id = SpecieId.Unown,
                Num = 201,
                Name = "Unown",
                Types = [PokemonType.Psychic],
                Gender = GenderId.N,
                BaseStats = new StatsTable
                {
                    Hp = 48,
                    Atk = 72,
                    Def = 48,
                    SpA = 72,
                    SpD = 48,
                    Spe = 48,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Levitate,
                },
                HeightM = 0.5,
                WeightKg = 5,
                Color = "Black",
            },
            [SpecieId.Wobbuffet] = new()
            {
                Id = SpecieId.Wobbuffet,
                Num = 202,
                Name = "Wobbuffet",
                Types = [PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 190,
                    Atk = 33,
                    Def = 58,
                    SpA = 33,
                    SpD = 58,
                    Spe = 33,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShadowTag,
                    Hidden = AbilityId.Telepathy,
                },
                HeightM = 1.3,
                WeightKg = 28.5,
                Color = "Blue",
            },
            [SpecieId.Girafarig] = new()
            {
                Id = SpecieId.Girafarig,
                Num = 203,
                Name = "Girafarig",
                Types = [PokemonType.Normal, PokemonType.Psychic],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 80,
                    Def = 65,
                    SpA = 90,
                    SpD = 65,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.InnerFocus,
                    Slot1 = AbilityId.EarlyBird,
                    Hidden = AbilityId.SapSipper,
                },
                HeightM = 1.5,
                WeightKg = 41.5,
                Color = "Yellow",
            },
            [SpecieId.Pineco] = new()
            {
                Id = SpecieId.Pineco,
                Num = 204,
                Name = "Pineco",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 65,
                    Def = 90,
                    SpA = 35,
                    SpD = 35,
                    Spe = 15,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Hidden = AbilityId.Overcoat,
                },
                HeightM = 0.6,
                WeightKg = 7.2,
                Color = "Gray",
            },
            [SpecieId.Forretress] = new()
            {
                Id = SpecieId.Forretress,
                Num = 205,
                Name = "Forretress",
                Types = [PokemonType.Bug, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 90,
                    Def = 140,
                    SpA = 60,
                    SpD = 60,
                    Spe = 40,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Sturdy,
                    Hidden = AbilityId.Overcoat,
                },
                HeightM = 1.2,
                WeightKg = 125.8,
                Color = "Purple",
            },
            [SpecieId.Dunsparce] = new()
            {
                Id = SpecieId.Dunsparce,
                Num = 206,
                Name = "Dunsparce",
                Types = [PokemonType.Normal],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 100,
                    Atk = 70,
                    Def = 70,
                    SpA = 65,
                    SpD = 65,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SereneGrace,
                    Slot1 = AbilityId.RunAway,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 1.5,
                WeightKg = 14,
                Color = "Yellow",
            },
            [SpecieId.Gligar] = new()
            {
                Id = SpecieId.Gligar,
                Num = 207,
                Name = "Gligar",
                Types = [PokemonType.Ground, PokemonType.Flying],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 75,
                    Def = 105,
                    SpA = 35,
                    SpD = 65,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.HyperCutter,
                    Slot1 = AbilityId.SandVeil,
                    Hidden = AbilityId.Immunity,
                },
                HeightM = 1.1,
                WeightKg = 64.8,
                Color = "Purple",
            },
            [SpecieId.Steelix] = new()
            {
                Id = SpecieId.Steelix,
                Num = 208,
                Name = "Steelix",
                Types = [PokemonType.Steel, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 85,
                    Def = 200,
                    SpA = 55,
                    SpD = 65,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                    Slot1 = AbilityId.Sturdy,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 9.2,
                WeightKg = 400,
                Color = "Gray",
            },
            [SpecieId.SteelixMega] = new()
            {
                Id = SpecieId.SteelixMega,
                Num = 208,
                Name = "Steelix-Mega",
                BaseSpecies = SpecieId.Steelix,
                Forme = FormeId.Mega,
                Types = [PokemonType.Steel, PokemonType.Ground],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 75,
                    Atk = 125,
                    Def = 230,
                    SpA = 55,
                    SpD = 95,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.SandForce,
                },
                HeightM = 10.5,
                WeightKg = 740,
                Color = "Gray",
            },
            [SpecieId.Snubbull] = new()
            {
                Id = SpecieId.Snubbull,
                Num = 209,
                Name = "Snubbull",
                Types = [PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 80,
                    Def = 50,
                    SpA = 40,
                    SpD = 40,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.RunAway,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 0.6,
                WeightKg = 7.8,
                Color = "Pink",
            },
            [SpecieId.Granbull] = new()
            {
                Id = SpecieId.Granbull,
                Num = 210,
                Name = "Granbull",
                Types = [PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 90,
                    Atk = 120,
                    Def = 75,
                    SpA = 60,
                    SpD = 60,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.QuickFeet,
                    Hidden = AbilityId.Rattled,
                },
                HeightM = 1.4,
                WeightKg = 48.7,
                Color = "Purple",
            },
            [SpecieId.Qwilfish] = new()
            {
                Id = SpecieId.Qwilfish,
                Num = 211,
                Name = "Qwilfish",
                Types = [PokemonType.Water, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 95,
                    Def = 85,
                    SpA = 55,
                    SpD = 55,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.SwiftSwim,
                    Hidden = AbilityId.Intimidate,
                },
                HeightM = 0.5,
                WeightKg = 3.9,
                Color = "Gray",
            },
            [SpecieId.QwilfishHisui] = new()
            {
                Id = SpecieId.QwilfishHisui,
                Num = 211,
                Name = "Qwilfish-Hisui",
                BaseSpecies = SpecieId.Qwilfish,
                Forme = FormeId.Hisui,
                Types = [PokemonType.Dark, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 65,
                    Atk = 95,
                    Def = 85,
                    SpA = 55,
                    SpD = 55,
                    Spe = 85,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.PoisonPoint,
                    Slot1 = AbilityId.SwiftSwim,
                    Hidden = AbilityId.Intimidate,
                },
                HeightM = 0.5,
                WeightKg = 3.9,
                Color = "Black",
            },
            [SpecieId.Scizor] = new()
            {
                Id = SpecieId.Scizor,
                Num = 212,
                Name = "Scizor",
                Types = [PokemonType.Bug, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 130,
                    Def = 100,
                    SpA = 55,
                    SpD = 80,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Slot1 = AbilityId.Technician,
                    Hidden = AbilityId.LightMetal,
                },
                HeightM = 1.8,
                WeightKg = 118,
                Color = "Red",
            },
            [SpecieId.ScizorMega] = new()
            {
                Id = SpecieId.ScizorMega,
                Num = 212,
                Name = "Scizor-Mega",
                BaseSpecies = SpecieId.Scizor,
                Forme = FormeId.Mega,
                Types = [PokemonType.Bug, PokemonType.Steel],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 150,
                    Def = 140,
                    SpA = 65,
                    SpD = 100,
                    Spe = 75,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Technician,
                },
                HeightM = 2.0,
                WeightKg = 125,
                Color = "Red",
            },
        };
    }
}
