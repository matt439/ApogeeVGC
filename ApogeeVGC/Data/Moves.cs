using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data;

public record Moves
{
    public IReadOnlyDictionary<MoveId, Move> MovesData { get; }
    private readonly Library _library;

    public Moves(Library library)
    {
        _library = library;
        MovesData = new ReadOnlyDictionary<MoveId, Move>(CreateMoves());
    }

    private Dictionary<MoveId, Move> CreateMoves()
    {
        return new Dictionary<MoveId, Move>
        {
            [MoveId.GlacialLance] = new()
            {
                Id = MoveId.GlacialLance,
                Num = 824,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Glacial Lance",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Ice,
            },
            [MoveId.LeechSeed] = new()
            {
                Id = MoveId.LeechSeed,
                Num = 73,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Leech Seed",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.LeechSeed,
                Condition = _library.Conditions[ConditionId.LeechSeed],
                OnTryImmunity = new OnTryImmunityEventInfo((_, target, _, _) =>
                    !target.HasType(PokemonType.Grass)),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.TrickRoom] = new()
            {
                Id = MoveId.TrickRoom,
                Num = 433,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Trick Room",
                BasePp = 5,
                Priority = -7,
                Flags = new MoveFlags
                {
                    Mirror = true,
                    Metronome = true,
                },
                PseudoWeather = ConditionId.TrickRoom,
                Condition = _library.Conditions[ConditionId.TrickRoom],
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Psychic,
            },
            [MoveId.Protect] = new()
            {
                Id = MoveId.Protect,
                Num = 182,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Protect",
                BasePp = 10,
                Priority = 4,
                Flags = new MoveFlags
                {
                    NoAssist = true,
                    FailCopycat = true,
                },
                StallingMove = true,
                VolatileStatus = ConditionId.Protect,

                OnPrepareHit = new OnPrepareHitEventInfo((battle, target, source, move) =>
                {
                    // source is the Pokemon using Protect
                    // Always run both checks, let Stall condition handle the logic
                    bool willAct = battle.Queue.WillAct() is not null;
                    RelayVar? stallResult = battle.RunEvent(EventId.StallMove, source);
                    bool stallSuccess = stallResult is BoolRelayVar { Value: true };
                    bool result = willAct && stallSuccess;

                    // Return BoolEmptyVoidUnion explicitly
                    return result ? (BoolEmptyVoidUnion)true : (BoolEmptyVoidUnion)false;
                }),

                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    // source is the Pokemon using Protect
                    battle.Debug(
                        $"[Protect.OnHit] BEFORE AddVolatile: {source.Name} has Stall volatile = {source.Volatiles.ContainsKey(ConditionId.Stall)}");

                    source.AddVolatile(ConditionId.Stall);

                    battle.Debug(
                        $"[Protect.OnHit] AFTER AddVolatile: {source.Name} has Stall volatile = {source.Volatiles.ContainsKey(ConditionId.Stall)}");
                    if (source.Volatiles.TryGetValue(ConditionId.Stall, out var stallState))
                    {
                        battle.Debug(
                            $"[Protect.OnHit] Stall volatile state: Counter={stallState.Counter}, Duration={stallState.Duration}");
                    }

                    return new VoidReturn();
                }),
                Condition = _library.Conditions[ConditionId.Protect],
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.VoltSwitch] = new()
            {
                Id = MoveId.VoltSwitch,
                Num = 521,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Volt Switch",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                SelfSwitch = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.DazzlingGleam] = new()
            {
                Id = MoveId.DazzlingGleam,
                Num = 605,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Dazzling Gleam",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Fairy,
            },
            [MoveId.ElectroDrift] = new()
            {
                Id = MoveId.ElectroDrift,
                Num = 879,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Electro Drift",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags()
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                },
                OnBasePower = new OnBasePowerEventInfo((battle, _, _, target, move) =>
                {
                    if (target.RunEffectiveness(move) <= 0.0) return new VoidReturn();
                    if (battle.DisplayUi)
                    {
                        battle.Debug("electro drift super effective buff");
                    }

                    battle.ChainModify([5461, 4096]);
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.DracoMeteor] = new()
            {
                Id = MoveId.DracoMeteor,
                Num = 434,
                Accuracy = 90,
                BasePower = 130,
                Category = MoveCategory.Special,
                Name = "Draco Meteor",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect { Boosts = new SparseBoostsTable { SpA = -2, }, },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.Facade] = new()
            {
                Id = MoveId.Facade,
                Num = 263,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Facade",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnBasePower = new OnBasePowerEventInfo((battle, _, pokemon, _, _) =>
                {
                    if (pokemon.Status is not ConditionId.None &&
                        pokemon.Status != ConditionId.Sleep)
                    {
                        battle.ChainModify(2);
                        return new VoidReturn();
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Crunch] = new()
            {
                Id = MoveId.Crunch,
                Num = 242,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Crunch",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bite = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    Boosts = new SparseBoostsTable { Def = -1, },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.HeadlongRush] = new()
            {
                Id = MoveId.HeadlongRush,
                Num = 838,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Headlong Rush",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Punch = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable
                    {
                        Def = -1,
                        SpD = -1,
                    },
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
            },
            [MoveId.StruggleBug] = new()
            {
                Id = MoveId.StruggleBug,
                Num = 522,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Struggle Bug",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { SpA = -1 },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Bug,
            },
            [MoveId.Overheat] = new()
            {
                Id = MoveId.Overheat,
                Num = 315,
                Accuracy = 90,
                BasePower = 130,
                Category = MoveCategory.Special,
                Name = "Overheat",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { SpA = -2, },
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.Tailwind] = new()
            {
                Id = MoveId.Tailwind,
                Num = 366,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Tailwind",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                    Wind = true,
                },
                SideCondition = ConditionId.Tailwind,
                Condition = _library.Conditions[ConditionId.Tailwind],
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Flying,
            },
            [MoveId.SpiritBreak] = new()
            {
                Id = MoveId.SpiritBreak,
                Num = 789,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Spirit Break",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { SpA = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.ThunderWave] = new()
            {
                Id = MoveId.ThunderWave,
                Num = 87,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Thunder Wave",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Status = ConditionId.Paralysis,
                IgnoreImmunity = false,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.Reflect] = new()
            {
                Id = MoveId.Reflect,
                Num = 115,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Reflect",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                SideCondition = ConditionId.Reflect,
                Condition = _library.Conditions[ConditionId.Reflect],
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Psychic,
            },
            [MoveId.LightScreen] = new()
            {
                Id = MoveId.LightScreen,
                Num = 113,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Light Screen",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                SideCondition = ConditionId.LightScreen,
                Condition = _library.Conditions[ConditionId.LightScreen],
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Psychic,
            },
            [MoveId.FakeOut] = new()
            {
                Id = MoveId.FakeOut,
                Num = 252,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Fake Out",
                BasePp = 10,
                Priority = 3,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnTry = new OnTryEventInfo((battle, source, _, _) =>
                {
                    if (source.ActiveMoveActions <= 1) return new VoidReturn();
                    if (battle.DisplayUi)
                    {
                        battle.Hint("Fake out only works on your first turn out.");
                    }

                    return false;
                }),
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.HeavySlam] = new()
            {
                Id = MoveId.HeavySlam,
                Num = 484,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, pokemon, target, _) =>
                {
                    int targetWeight = target.GetWeight();
                    int pokemonWeight = pokemon.GetWeight();
                    int bp;
                    if (pokemonWeight >= targetWeight * 5)
                    {
                        bp = 120;
                    }
                    else if (pokemonWeight >= targetWeight * 4)
                    {
                        bp = 100;
                    }
                    else if (pokemonWeight >= targetWeight * 3)
                    {
                        bp = 80;
                    }
                    else if (pokemonWeight >= targetWeight * 2)
                    {
                        bp = 60;
                    }
                    else
                    {
                        bp = 40;
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Debug($"BP: {bp}");
                    }

                    return bp;
                }),
                Category = MoveCategory.Physical,
                Name = "Heavy Slam",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    NonSky = true,
                    Metronome = true,
                },
                // OnTryHit only applies to dynamax
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.LowKick] = new()
            {
                Id = MoveId.LowKick,
                Num = 67,
                Accuracy = 100,
                BasePower = 0,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, target, _) =>
                {
                    int targetWeight = target.GetWeight();
                    int bp = targetWeight switch
                    {
                        >= 2000 => 120,
                        >= 1000 => 100,
                        >= 500 => 80,
                        >= 250 => 60,
                        >= 100 => 40,
                        _ => 20,
                    };
                    if (battle.DisplayUi)
                    {
                        battle.Debug($"BP: {bp}");
                    }

                    return bp;
                }),
                Category = MoveCategory.Physical,
                Name = "Low Kick",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // OnTryHit only applies to dynamax
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.WildCharge] = new()
            {
                Id = MoveId.WildCharge,
                Num = 528,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Wild Charge",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Recoil = (1, 4),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },


            // Struggle
            [MoveId.Struggle] = new()
            {
                Id = MoveId.Struggle,
                Num = 165,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Struggle",
                BasePp = 1,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    FailEncore = true,
                    FailMeFirst = true,
                    NoSleepTalk = true,
                    NoAssist = true,
                    FailCopycat = true,
                    FailMimic = true,
                    FailInstruct = true,
                    NoSketch = true,
                },
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    move.Type = MoveType.Fighting;
                    if (battle.DisplayUi)
                    {
                        battle.Add("-activate", pokemon, "move: Struggle");
                    }
                }),
                StruggleRecoil = true,
                Secondary = null,
                Target = MoveTarget.RandomNormal,
                Type = MoveType.Normal,
            },


            //[MoveId.Confused] = new()
            //{
            //    Choice = MoveId.Confused,
            //    Num = 10019,
            //    Accuracy = IntTrueUnion.FromTrue(),
            //    Name = "Confused",
            //    Value = MoveType.Normal,
            //},
        };
    }
}