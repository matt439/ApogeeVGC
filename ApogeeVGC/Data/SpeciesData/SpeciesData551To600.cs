using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData551To600()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Cottonee] = new()
            {
                Id = SpecieId.Cottonee,
                Num = 546,
                Name = "Cottonee",
                Types = [PokemonType.Grass, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 27,
                    Def = 60,
                    SpA = 37,
                    SpD = 50,
                    Spe = 66,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Slot1 = AbilityId.Infiltrator,
                    Hidden = AbilityId.Chlorophyll,
                },
                HeightM = 0.3,
                WeightKg = 0.6,
                Color = "Green",
            },
            [SpecieId.Whimsicott] = new()
            {
                Id = SpecieId.Whimsicott,
                Num = 547,
                Name = "Whimsicott",
                Types = [PokemonType.Grass, PokemonType.Fairy],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 67,
                    Def = 85,
                    SpA = 77,
                    SpD = 75,
                    Spe = 116,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Prankster,
                    Slot1 = AbilityId.Infiltrator,
                    Hidden = AbilityId.Chlorophyll,
                },
                HeightM = 0.7,
                WeightKg = 6.6,
                Color = "Green",
                Prevo = SpecieId.Cottonee,
            },
            [SpecieId.Petilil] = new()
            {
                Id = SpecieId.Petilil,
                Num = 548,
                Name = "Petilil",
                Types = [PokemonType.Grass],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 45,
                    Atk = 35,
                    Def = 50,
                    SpA = 70,
                    SpD = 50,
                    Spe = 30,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 0.5,
                WeightKg = 6.6,
                Color = "Green",
            },
            [SpecieId.Lilligant] = new()
            {
                Id = SpecieId.Lilligant,
                Num = 549,
                Name = "Lilligant",
                Types = [PokemonType.Grass],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 60,
                    Def = 75,
                    SpA = 110,
                    SpD = 75,
                    Spe = 90,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.OwnTempo,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 1.1,
                WeightKg = 16.3,
                Color = "Green",
                Prevo = SpecieId.Petilil,
            },
            [SpecieId.LilligantHisui] = new()
            {
                Id = SpecieId.LilligantHisui,
                Num = 549,
                Name = "Lilligant-Hisui",
                Types = [PokemonType.Grass, PokemonType.Fighting],
                Gender = GenderId.F,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 105,
                    Def = 75,
                    SpA = 50,
                    SpD = 75,
                    Spe = 105,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Chlorophyll,
                    Slot1 = AbilityId.Hustle,
                    Hidden = AbilityId.LeafGuard,
                },
                HeightM = 1.2,
                WeightKg = 19.2,
                Color = "Green",
                BaseSpecies = SpecieId.Lilligant,
                Forme = FormeId.Hisui,
                Prevo = SpecieId.Petilil,
            },
            [SpecieId.Basculin] = new()
            {
                Id = SpecieId.Basculin,
                Num = 550,
                Name = "Basculin",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 92,
                    Def = 65,
                    SpA = 80,
                    SpD = 55,
                    Spe = 98,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Reckless,
                    Slot1 = AbilityId.Adaptability,
                    Hidden = AbilityId.MoldBreaker,
                },
                HeightM = 1,
                WeightKg = 18,
                Color = "Green",
            },
            [SpecieId.BasculinBlueStriped] = new()
            {
                Id = SpecieId.BasculinBlueStriped,
                Num = 550,
                Name = "Basculin-Blue-Striped",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 92,
                    Def = 65,
                    SpA = 80,
                    SpD = 55,
                    Spe = 98,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.RockHead,
                    Slot1 = AbilityId.Adaptability,
                    Hidden = AbilityId.MoldBreaker,
                },
                HeightM = 1,
                WeightKg = 18,
                Color = "Green",
                BaseSpecies = SpecieId.Basculin,
                Forme = FormeId.BlueStriped,
            },
            [SpecieId.BasculinWhiteStriped] = new()
            {
                Id = SpecieId.BasculinWhiteStriped,
                Num = 550,
                Name = "Basculin-White-Striped",
                Types = [PokemonType.Water],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 92,
                    Def = 65,
                    SpA = 80,
                    SpD = 55,
                    Spe = 98,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Rattled,
                    Slot1 = AbilityId.Adaptability,
                    Hidden = AbilityId.MoldBreaker,
                },
                HeightM = 1,
                WeightKg = 18,
                Color = "Green",
                BaseSpecies = SpecieId.Basculin,
                Forme = FormeId.WhiteStriped,
            },
            [SpecieId.Sandile] = new()
            {
                Id = SpecieId.Sandile,
                Num = 551,
                Name = "Sandile",
                Types = [PokemonType.Ground, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 50,
                    Atk = 72,
                    Def = 35,
                    SpA = 35,
                    SpD = 35,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.Moxie,
                    Hidden = AbilityId.AngerPoint,
                },
                HeightM = 0.7,
                WeightKg = 15.2,
                Color = "Brown",
            },
            [SpecieId.Krokorok] = new()
            {
                Id = SpecieId.Krokorok,
                Num = 552,
                Name = "Krokorok",
                Types = [PokemonType.Ground, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 60,
                    Atk = 82,
                    Def = 45,
                    SpA = 45,
                    SpD = 45,
                    Spe = 74,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.Moxie,
                    Hidden = AbilityId.AngerPoint,
                },
                HeightM = 1,
                WeightKg = 33.4,
                Color = "Brown",
                Prevo = SpecieId.Sandile,
            },
            [SpecieId.Krookodile] = new()
            {
                Id = SpecieId.Krookodile,
                Num = 553,
                Name = "Krookodile",
                Types = [PokemonType.Ground, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 95,
                    Atk = 117,
                    Def = 80,
                    SpA = 65,
                    SpD = 70,
                    Spe = 92,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.Intimidate,
                    Slot1 = AbilityId.Moxie,
                    Hidden = AbilityId.AngerPoint,
                },
                HeightM = 1.5,
                WeightKg = 96.3,
                Color = "Red",
                Prevo = SpecieId.Krokorok,
            },
        };
    }
}
