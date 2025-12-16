using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data;

public partial record SpeciesData
{
    public IReadOnlyDictionary<SpecieId, Species> SpeciesDataDictionary { get; }

    public SpeciesData()
    {
        var combinedSpecies = new Dictionary<SpecieId, Species>();

        // Combine data from all methods
        foreach (var kvp in GenerateSpeciesData1To50())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData51To100())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData101To150())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData151To200())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData201To250())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData251To300())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData301To350())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData351To400())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData401To450())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData451To500())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData501To550())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData551To600())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData601To650())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData651To700())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData701To750())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData751To800())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData801To850())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData851To900())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData901To950())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData951To1000())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in GenerateSpeciesData1001To1050())
        {
            combinedSpecies[kvp.Key] = kvp.Value;
        }

        SpeciesDataDictionary = new ReadOnlyDictionary<SpecieId, Species>(combinedSpecies);
    }

    //private readonly Dictionary<SpecieId, Species> _species = new()
    //{
    //    [SpecieId.Mew] = new Species
    //    {
    //        Id = SpecieId.Mew,
    //        Num = 151,
    //        Name = "Mew",
    //        Types = [PokemonType.Psychic],
    //        Gender = GenderId.N,
    //        BaseStats = new StatsTable
    //        {
    //            Hp = 100,
    //            Atk = 100,
    //            Def = 100,
    //            SpA = 100,
    //            SpD = 100,
    //            Spe = 100,
    //        },
    //        Abilities = new SpeciesAbility
    //        {
    //            Slot0 = AbilityId.Synchronize,
    //        },
    //        HeightM = 0.4,
    //        WeightKg = 4,
    //        Color = "Pink",
    //    },
    //    [SpecieId.Chikorita] = new Species
    //    {
    //        Id = SpecieId.Chikorita,
    //        Num = 152,
    //        Name = "Chikorita",
    //        Types = [PokemonType.Grass],
    //        Gender = GenderId.Empty,
    //        BaseStats = new StatsTable
    //        {
    //            Hp = 45,
    //            Atk = 49,
    //            Def = 65,
    //            SpA = 49,
    //            SpD = 65,
    //            Spe = 45,
    //        },
    //        Abilities = new SpeciesAbility
    //        {
    //            Slot0 = AbilityId.Overgrow,
    //            Hidden = AbilityId.LeafGuard,
    //        },
    //        HeightM = 0.9,
    //        WeightKg = 6.4,
    //        Color = "Green",
    //    },
    //    [SpecieId.Bayleef] = new Species
    //    {
    //        Id = SpecieId.Bayleef,
    //        Num = 153,
    //        Name = "Bayleef",
    //        Types = [PokemonType.Grass],
    //        Gender = GenderId.Empty,
    //        BaseStats = new StatsTable
    //        {
    //            Hp = 60,
    //            Atk = 62,
    //            Def = 80,
    //            SpA = 63,
    //            SpD = 80,
    //            Spe = 60,
    //        },
    //        Abilities = new SpeciesAbility
    //        {
    //            Slot0 = AbilityId.Overgrow,
    //            Hidden = AbilityId.LeafGuard,
    //        },
    //        HeightM = 1.2,
    //        WeightKg = 15.8,
    //        Color = "Green",
    //    },
    //    [SpecieId.Meganium] = new Species
    //    {
    //        Id = SpecieId.Meganium,
    //        Num = 154,
    //        Name = "Meganium",
    //        Types = [PokemonType.Grass],
    //        Gender = GenderId.Empty,
    //        BaseStats = new StatsTable
    //        {
    //            Hp = 80,
    //            Atk = 82,
    //            Def = 100,
    //            SpA = 83,
    //            SpD = 100,
    //            Spe = 80,
    //        },
    //        Abilities = new SpeciesAbility
    //        {
    //            Slot0 = AbilityId.Overgrow,
    //            Hidden = AbilityId.LeafGuard,
    //        },
    //        HeightM = 1.8,
    //        WeightKg = 100.5,
    //        Color = "Green",
    //    },
    //    [SpecieId.MeganiummMega] = new Species
    //    {
    //        Id = SpecieId.MeganiummMega,
    //        Num = 154,
    //        Name = "Meganium-Mega",
    //        BaseSpecies = SpecieId.Meganium,
    //        Forme = FormeId.Mega,
    //        Types = [PokemonType.Grass, PokemonType.Fairy],
    //        Gender = GenderId.Empty,
    //        BaseStats = new StatsTable
    //        {
    //            Hp = 80,
    //            Atk = 92,
    //            Def = 115,
    //            SpA = 143,
    //            SpD = 115,
    //            Spe = 80,
    //        },
    //        Abilities = new SpeciesAbility
    //        {
    //            Slot0 = AbilityId.Overgrow,
    //            Hidden = AbilityId.LeafGuard,
    //        },
    //        HeightM = 2.4,
    //        WeightKg = 201,
    //        Color = "Green",
    //    },
    //    [SpecieId.CalyrexIce] = new Species
    //    {
    //        Id = SpecieId.CalyrexIce,
    //        Num = 898,
    //        Name = "Calyrex-Ice",
    //        BaseSpecies = SpecieId.Calyrex,
    //        Forme = FormeId.Ice,
    //        Types = [PokemonType.Psychic, PokemonType.Ice],
    //        Gender = GenderId.N,
    //        BaseStats = new StatsTable()
    //        {
    //            Hp = 100,
    //            Atk = 165,
    //            Def = 150,
    //            SpA = 85,
    //            SpD = 130,
    //            Spe = 50,
    //        },
    //        Abilities = new SpeciesAbility { Slot0 = AbilityId.AsOneGlastrier },
    //        HeightM = 2.4,
    //        WeightKg = 809.1,
    //        Color = "White",
    //    },
    //    [SpecieId.Miraidon] = new Species
    //    {
    //        Id = SpecieId.Miraidon,
    //        Num = 1008,
    //        Name = "Miraidon",
    //        Types = [PokemonType.Electric, PokemonType.Dragon],
    //        Gender = GenderId.N,
    //        BaseStats = new StatsTable
    //        {
    //            Hp = 100,
    //            Atk = 85,
    //            Def = 100,
    //            SpA = 135,
    //            SpD = 115,
    //            Spe = 135,
    //        },
    //        Abilities = new SpeciesAbility { Slot0 = AbilityId.HadronEngine },
    //        HeightM = 3.5,
    //        WeightKg = 240,
    //        Color = "Purple",
    //    },
    //    [SpecieId.Ursaluna] = new Species
    //    {
    //        Id = SpecieId.Ursaluna,
    //        Num = 901,
    //        Name = "Ursaluna",
    //        Types = [PokemonType.Ground, PokemonType.Normal],
    //        Gender = GenderId.N,
    //        BaseStats = new StatsTable
    //        {
    //            Hp = 130,
    //            Atk = 140,
    //            Def = 105,
    //            SpA = 45,
    //            SpD = 80,
    //            Spe = 50,
    //        },
    //        Abilities = new SpeciesAbility
    //        {
    //            Slot0 = AbilityId.Guts,
    //            Slot1 = AbilityId.Bulletproof,
    //            Hidden = AbilityId.Unnerve,
    //        },
    //        HeightM = 2.4,
    //        WeightKg = 290,
    //        Color = "Brown",
    //    },
    //    [SpecieId.Volcarona] = new Species
    //    {
    //        Id = SpecieId.Volcarona,
    //        Num = 637,
    //        Name = "Volcarona",
    //        Types = [PokemonType.Bug, PokemonType.Fire],
    //        Gender = GenderId.N,
    //        BaseStats = new StatsTable
    //        {
    //            Hp = 85,
    //            Atk = 60,
    //            Def = 65,
    //            SpA = 135,
    //            SpD = 105,
    //            Spe = 100,
    //        },
    //        Abilities = new SpeciesAbility
    //        {
    //            Slot0 = AbilityId.FlameBody,
    //            Hidden = AbilityId.Swarm,
    //        },
    //        HeightM = 1.6,
    //        WeightKg = 46,
    //        Color = "White",
    //    },
    //    [SpecieId.Grimmsnarl] = new Species
    //    {
    //        Id = SpecieId.Grimmsnarl,
    //        Num = 861,
    //        Name = "Grimmsnarl",
    //        Types = [PokemonType.Dark, PokemonType.Fairy],
    //        Gender = GenderId.M,
    //        BaseStats = new StatsTable
    //        {
    //            Hp = 95,
    //            Atk = 120,
    //            Def = 65,
    //            SpA = 95,
    //            SpD = 75,
    //            Spe = 60,
    //        },
    //        Abilities = new SpeciesAbility
    //        {
    //            Slot0 = AbilityId.Prankster,
    //            Slot1 = AbilityId.Frisk,
    //            Hidden = AbilityId.Pickpocket,
    //        },
    //        HeightM = 1.5,
    //        WeightKg = 61,
    //        Color = "Purple",
    //    },
    //    [SpecieId.IronHands] = new Species
    //    {
    //        Id = SpecieId.IronHands,
    //        Num = 992,
    //        Name = "Iron Hands",
    //        Types = [PokemonType.Fighting, PokemonType.Electric],
    //        Gender = GenderId.N,
    //        BaseStats = new StatsTable
    //        {
    //            Hp = 154,
    //            Atk = 140,
    //            Def = 108,
    //            SpA = 50,
    //            SpD = 68,
    //            Spe = 50,
    //        },
    //        Abilities = new SpeciesAbility
    //        {
    //            Slot0 = AbilityId.QuarkDrive,
    //        },
    //        HeightM = 1.8,
    //        WeightKg = 380.7,
    //        Color = "Gray",
    //    },
    //};
}