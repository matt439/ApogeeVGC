using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Data.SpeciesData;

public partial record SpeciesData
{
    private static Dictionary<SpecieId, Species> GenerateSpeciesData451To500()
    {
        return new Dictionary<SpecieId, Species>
        {
            [SpecieId.Skorupi] = new()
            {
                Id = SpecieId.Skorupi,
                Num = 451,
                Name = "Skorupi",
                Types = [PokemonType.Poison, PokemonType.Bug],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 40,
                    Atk = 50,
                    Def = 90,
                    SpA = 30,
                    SpD = 55,
                    Spe = 65,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                    Slot1 = AbilityId.Sniper,
                    Hidden = AbilityId.KeenEye,
                },
                HeightM = 0.8,
                WeightKg = 12,
                Color = "Purple",
            },
            [SpecieId.Drapion] = new()
            {
                Id = SpecieId.Drapion,
                Num = 452,
                Name = "Drapion",
                Types = [PokemonType.Poison, PokemonType.Dark],
                Gender = GenderId.Empty,
                BaseStats = new StatsTable
                {
                    Hp = 70,
                    Atk = 90,
                    Def = 110,
                    SpA = 60,
                    SpD = 75,
                    Spe = 95,
                },
                Abilities = new SpeciesAbility
                {
                    Slot0 = AbilityId.BattleArmor,
                    Slot1 = AbilityId.Sniper,
                    Hidden = AbilityId.KeenEye,
                },
                HeightM = 1.3,
                WeightKg = 61.5,
                Color = "Purple",
                Prevo = SpecieId.Skorupi,
            },
        };
    }
}
