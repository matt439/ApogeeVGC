using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData401To450()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Kricketot] = new()
            {
                Id = SpecieId.Kricketot,
                Num = 401,
                Name = "Kricketot",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 37,
                    Atk = 25,
                    Def = 41,
                    SpA = 25,
                    SpD = 41,
                    Spe = 25,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.ShedSkin,
                    Hidden = AbilityId.RunAway,
                },
                HeightM = 0.3,
                WeightKg = 2.2,
                Color = "Red",
            },
            [SpecieId.Kricketune] = new()
            {
                Id = SpecieId.Kricketune,
                Num = 402,
                Name = "Kricketune",
                Types = [PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 77,
                    Atk = 85,
                    Def = 51,
                    SpA = 55,
                    SpD = 51,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Swarm,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 1,
                WeightKg = 25.5,
                Color = "Red",
                Prevo = SpecieId.Kricketot,
            },
            [SpecieId.Shinx] = new()
            {
                Id = SpecieId.Shinx,
                Num = 403,
                Name = "Shinx",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 65,
                    Def = 34,
                    SpA = 40,
                    SpD = 34,
                    Spe = 45,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.Intimidate,
                    Hidden = AbilityId.Guts,
                },
                HeightM = 0.5,
                WeightKg = 9.5,
                Color = "Blue",
            },
            [SpecieId.Luxio] = new()
            {
                Id = SpecieId.Luxio,
                Num = 404,
                Name = "Luxio",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 85,
                    Def = 49,
                    SpA = 60,
                    SpD = 49,
                    Spe = 60,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.Intimidate,
                    Hidden = AbilityId.Guts,
                },
                HeightM = 0.9,
                WeightKg = 30.5,
                Color = "Blue",
                Prevo = SpecieId.Shinx,
            },
            [SpecieId.Luxray] = new()
            {
                Id = SpecieId.Luxray,
                Num = 405,
                Name = "Luxray",
                Types = [PokemonType.Electric],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 80,
                    Atk = 120,
                    Def = 79,
                    SpA = 95,
                    SpD = 79,
                    Spe = 70,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rivalry,
                    Slot1 = AbilityId.Intimidate,
                    Hidden = AbilityId.Guts,
                },
                HeightM = 1.4,
                WeightKg = 42,
                Color = "Blue",
                Prevo = SpecieId.Luxio,
            },
            [SpecieId.Budew] = new()
            {
                Id = SpecieId.Budew,
                Num = 406,
                Name = "Budew",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 30,
                    Def = 35,
                    SpA = 50,
                    SpD = 70,
                    Spe = 55,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Slot1 = AbilityId.PoisonPoint,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 0.2,
                WeightKg = 1.2,
                Color = "Green",
            },
            [SpecieId.Roserade] = new()
            {
                Id = SpecieId.Roserade,
                Num = 407,
                Name = "Roserade",
                Types = [PokemonType.Grass, PokemonType.Poison],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 70,
                    Def = 65,
                    SpA = 125,
                    SpD = 105,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.NaturalCure,
                    Slot1 = AbilityId.PoisonPoint,
                    Hidden = AbilityId.Technician,
                },
                HeightM = 0.9,
                WeightKg = 14.5,
                Color = "Green",
            },
            [SpecieId.Cranidos] = new()
            {
                Id = SpecieId.Cranidos,
                Num = 408,
                Name = "Cranidos",
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 67,
                    Atk = 125,
                    Def = 40,
                    SpA = 30,
                    SpD = 30,
                    Spe = 58,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MoldBreaker,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 0.9,
                WeightKg = 31.5,
                Color = "Blue",
            },
            [SpecieId.Rampardos] = new()
            {
                Id = SpecieId.Rampardos,
                Num = 409,
                Name = "Rampardos",
                Types = [PokemonType.Rock],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 97,
                    Atk = 165,
                    Def = 60,
                    SpA = 65,
                    SpD = 50,
                    Spe = 58,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.MoldBreaker,
                    Hidden = AbilityId.SheerForce,
                },
                HeightM = 1.6,
                WeightKg = 102.5,
                Color = "Blue",
                Prevo = SpecieId.Cranidos,
            },
        };
    }
}
