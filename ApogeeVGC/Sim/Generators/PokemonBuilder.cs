using ApogeeVGC.Data;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.Generators;

public struct MoveSetup
{
    public MoveId Id { get; init; }
    public int PpUp
    {
        get;
        init
        {
            if (value is < 0 or > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "PP Up must be between 0 and 3.");
            }
            field = value;
        }
    } = 0;
    public MoveSetup(MoveId id, int ppUp = 0)
    {
        Id = id;
        PpUp = ppUp;
    }
}

public static class PokemonBuilder
{
    public static Pokemon Build(
        Library library,
        SpecieId specie,
        MoveSetup[] moves,
        ItemId item,
        AbilityId ability,
        StatsTable evs,
        NatureType nature,
        MoveType terraType,
        Trainer trainer,
        SideId sideId,
        bool pringDebug = false,
        StatsTable? ivs = null,
        string? nickname = null,
        bool shiny = false,
        int level = 50)
    {
        List<Move> movesList = [];

        int i = 0;
        foreach (MoveSetup moveSetup in moves)
        {
            if (!library.Moves.TryGetValue(moveSetup.Id, out Move? move))
            {
                throw new ArgumentException($"Move {moveSetup.Id} not found in library.");
            }
            move = move with { PpUp = moveSetup.PpUp }; // Set PP Up
            move = move with { MoveSlot = (MoveSlot)i };
            movesList.Add(move);
            i++;
        }

        Specie spec = library.Species[specie] ??
                      throw new ArgumentException($"Specie {specie} not found in library.");
        Nature nat = library.Natures[nature] ??
                     throw new ArgumentException($"Nature {nature} not found in library.");

        Pokemon pokemon = new(spec, evs, ivs ?? StatsTable.PerfectIvs, nat, level, trainer, sideId)
        {
            Moves = movesList.ToArray(),
            Item = library.Items[item] ?? throw new ArgumentException($"Item {item} not found in library."),
            Ability = library.Abilities[ability] ??
                      throw new ArgumentException($"Ability {ability} not found in library."),
            Evs = evs,
            Name = nickname ?? library.Species[specie].Name,
            Shiny = shiny,
            PrintDebug = pringDebug,
            TeraType = terraType,
        };

        return PokemonValidator.IsValid(library, pokemon) ? pokemon
            : throw new ArgumentException("Invalid Pokemon configuration.");
    }

    public static PokemonSet BuildTestSet(Library library, Trainer trainer, SideId sideId, bool printDebug = false)
    {
        return new PokemonSet()
        {
            Pokemons =
            [
                Build(
                    library,
                    SpecieId.CalyrexIce,
                    [new MoveSetup(MoveId.GlacialLance, 3),
                        new MoveSetup(MoveId.LeechSeed, 3),
                        new MoveSetup(MoveId.TrickRoom, 3),
                        new MoveSetup(MoveId.Protect, 3)],
                    ItemId.Leftovers,
                    AbilityId.AsOneGlastrier,
                    new StatsTable { Hp = 236, Atk = 36, SpD = 236 },
                    NatureType.Adamant,
                    MoveType.Water,
                    trainer,
                    sideId,
                    printDebug
                ),
                Build(
                    library,
                    SpecieId.Miraidon,
                    [new MoveSetup(MoveId.VoltSwitch, 3),
                        new MoveSetup(MoveId.DazzlingGleam, 3),
                        new MoveSetup(MoveId.ElectroDrift, 3),
                        new MoveSetup(MoveId.DracoMeteor, 3)],
                    ItemId.ChoiceSpecs,
                    AbilityId.HadronEngine,
                    new StatsTable { Hp = 236, Def = 52, SpA = 124, SpD = 68, Spe = 28 },
                    NatureType.Modest,
                    MoveType.Fairy,
                    trainer,
                    sideId,
                    printDebug
                ),
                Build(
                    library,
                    SpecieId.Ursaluna,
                    [new MoveSetup(MoveId.Facade, 3),
                        new MoveSetup(MoveId.Crunch, 3),
                        new MoveSetup(MoveId.HeadlongRush, 3),
                        new MoveSetup(MoveId.Protect, 3)],
                    ItemId.FlameOrb,
                    AbilityId.Guts,
                    new StatsTable { Hp = 108, Atk = 156, Def = 4, SpD = 116, Spe = 124 },
                    NatureType.Adamant,
                    MoveType.Ghost,
                    trainer,
                    sideId,
                    printDebug
                ),
                Build(
                    library,
                    SpecieId.Volcarona,
                    [new MoveSetup(MoveId.StruggleBug, 3),
                        new MoveSetup(MoveId.Overheat, 3),
                        new MoveSetup(MoveId.Protect, 3),
                        new MoveSetup(MoveId.Tailwind, 3)],
                    ItemId.RockyHelmet,
                    AbilityId.FlameBody,
                    new StatsTable { Hp = 252, Def = 196, SpD = 60 },
                    NatureType.Bold,
                    MoveType.Water,
                    trainer,
                    sideId,
                    printDebug
                ),
                Build(
                    library,
                    SpecieId.Grimmsnarl,
                    [new MoveSetup(MoveId.SpiritBreak, 3),
                        new MoveSetup(MoveId.ThunderWave, 3),
                        new MoveSetup(MoveId.Reflect, 3),
                        new MoveSetup(MoveId.LightScreen, 3)],
                    ItemId.LightClay,
                    AbilityId.Prankster,
                    new StatsTable { Hp = 236, Atk = 4, Def = 140, SpD = 116, Spe = 12 },
                    NatureType.Careful,
                    MoveType.Ghost,
                    trainer,
                    sideId,
                    printDebug
                ),
                Build(
                    library,
                    SpecieId.IronHands,
                    [new MoveSetup(MoveId.FakeOut, 3),
                        new MoveSetup(MoveId.HeavySlam, 3),
                        new MoveSetup(MoveId.LowKick, 3),
                        new MoveSetup(MoveId.WildCharge, 3)],
                    ItemId.AssaultVest,
                    AbilityId.QuarkDrive,
                    new StatsTable { Atk = 236, SpD = 236, Spe = 36 },
                    NatureType.Adamant,
                    MoveType.Bug,
                    trainer,
                    sideId,
                    printDebug
                ),
            ],
            SideId = sideId,
        };
    }

    public static Pokemon BuildDefaultPokemon(Library library, Trainer trainer, SideId sideId)
    {
        return Build(
            library,
            SpecieId.Bulbasaur,
            [new MoveSetup(MoveId.LeechSeed),
                new MoveSetup(MoveId.HeavySlam),
                new MoveSetup(MoveId.HeadlongRush),
                new MoveSetup(MoveId.Protect)],
            ItemId.Leftovers,
            AbilityId.Guts,
            new StatsTable { Hp = 252, SpA = 252, Spe = 4 },
            NatureType.Modest,
            MoveType.Fire,
            trainer,
            sideId,
            false,
            StatsTable.PerfectIvs,
            "Default");
    }
}