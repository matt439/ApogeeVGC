using ApogeeVGC.Data;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;

namespace ApogeeVGC.Sim.Generators;

public static class TeamGenerator
{
    public static List<PokemonSet> GenerateTestTeam(Library library)
    {
        return
        [
            new PokemonSet
            {
                Name = "calyrex-ice",
                Species = SpecieId.CalyrexIce,
                Item = ItemId.Leftovers,
                Ability = AbilityId.AsOneGlastrier,
                Moves = [ MoveId.GlacialLance, MoveId.LeechSeed, MoveId.TrickRoom, MoveId.Protect ],
                Nature = library.Natures[NatureId.Adamant],
                Gender = GenderId.M,
                Evs = new StatsTable { Hp = 236, Atk = 36, SpD = 236 },
                TeraType = MoveType.Water,
            },
            new PokemonSet
            {
                Name = "miraidon",
                Species = SpecieId.Miraidon,
                Item = ItemId.ChoiceSpecs,
                Ability = AbilityId.HadronEngine,
                Moves = [ MoveId.VoltSwitch, MoveId.DazzlingGleam, MoveId.ElectroDrift, MoveId.DracoMeteor ],
                Nature = library.Natures[NatureId.Modest],
                Gender = GenderId.M,
                Evs = new StatsTable { Hp = 236, Def = 52, SpA = 124, SpD = 68, Spe = 28 },
                TeraType = MoveType.Fairy,
            },
            new PokemonSet
            {
                Name = "ursaluna",
                Species = SpecieId.Ursaluna,
                Item = ItemId.FlameOrb,
                Ability = AbilityId.Guts,
                Moves = [ MoveId.Facade, MoveId.Crunch, MoveId.HeadlongRush, MoveId.Protect ],
                Nature = library.Natures[NatureId.Adamant],
                Gender = GenderId.M,
                Evs = new StatsTable { Hp = 108, Atk = 156, Def = 4, SpD = 116, Spe = 124 },
                TeraType = MoveType.Ghost,
            },
            new PokemonSet
            {
                Name = "volcarona",
                Species = SpecieId.Volcarona,
                Item = ItemId.RockyHelmet,
                Ability = AbilityId.FlameBody,
                Moves = [ MoveId.StruggleBug, MoveId.Overheat, MoveId.Protect, MoveId.Tailwind ],
                Nature = library.Natures[NatureId.Bold],
                Gender = GenderId.M,
                Evs = new StatsTable { Hp = 252, Def = 196, SpD = 60 },
                TeraType = MoveType.Water,
            },
            new PokemonSet
            {
                Name = "grimmsnarl",
                Species = SpecieId.Grimmsnarl,
                Item = ItemId.LightClay,
                Ability = AbilityId.Prankster,
                Moves = [ MoveId.SpiritBreak, MoveId.ThunderWave, MoveId.Reflect, MoveId.LightScreen ],
                Nature = library.Natures[NatureId.Careful],
                Gender = GenderId.M,
                Evs = new StatsTable { Hp = 236, Atk = 4, Def = 140, SpD = 116, Spe = 12 },
                TeraType = MoveType.Ghost,
            },
            new PokemonSet
            {
                Name = "ironhands",
                Species = SpecieId.IronHands,
                Item = ItemId.AssaultVest,
                Ability = AbilityId.QuarkDrive,
                Moves = [ MoveId.FakeOut, MoveId.HeavySlam, MoveId.LowKick, MoveId.WildCharge ],
                Nature = library.Natures[NatureId.Adamant],
                Gender = GenderId.M,
                Evs = new StatsTable { Hp = 236, SpD = 236, Spe = 36 },
                TeraType = MoveType.Bug,
            },
        ];
    }
}