using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesJkl()
    {
        return new Dictionary<MoveId, Move>
        {
            [MoveId.JawLock] = new()
            {
                Id = MoveId.JawLock,
                Num = 746,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Jaw Lock",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bite = true,
                },
                OnHit = new OnHitEventInfo((_, target, source, move) =>
                {
                    source.AddVolatile(ConditionId.Trapped, target, move, ConditionId.Trapper);
                    target.AddVolatile(ConditionId.Trapped, source, move, ConditionId.Trapper);
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.JetPunch] = new()
            {
                Id = MoveId.JetPunch,
                Num = 857,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Jet Punch",
                BasePp = 15,
                Priority = 1,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Punch = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.Judgment] = new()
            {
                Id = MoveId.Judgment,
                Num = 449,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Judgment",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnModifyType = new OnModifyTypeEventInfo((_, move, pokemon, _) =>
                {
                    if (pokemon.IgnoringItem()) return;
                    Item item = pokemon.GetItem();
                    if (item.Id != ItemId.None && item.OnPlate != null)
                    {
                        move.Type = (MoveType)item.OnPlate.Value;
                    }
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.JungleHealing] = new()
            {
                Id = MoveId.JungleHealing,
                Num = 816,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Jungle Healing",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Heal = true,
                    BypassSub = true,
                    AllyAnim = true,
                },
                OnHit = new OnHitEventInfo((battle, target, _, _) =>
                {
                    int healAmount = battle.Modify(target.MaxHp, 1, 4); // 25%
                    IntFalseUnion healResult = battle.Heal(healAmount, target);
                    bool success = healResult is not FalseIntFalseUnion;
                    bool cured = target.CureStatus();
                    return (cured || success) ? new VoidReturn() : false;
                }),
                Secondary = null,
                Target = MoveTarget.Allies,
                Type = MoveType.Grass,
            },
            [MoveId.KnockOff] = new()
            {
                Id = MoveId.KnockOff,
                Num = 282,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Physical,
                Name = "Knock Off",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, target, move) =>
                {
                    Item item = target.GetItem();
                    // Check if item can be taken (TakeItem event check)
                    RelayVar? takeResult = battle.SingleEvent(Sim.Events.EventId.TakeItem, item,
                        target.ItemState, target, target, move, item);
                    if (takeResult is BoolRelayVar { Value: false }) return basePower;
                    if (item.Id != ItemId.None)
                    {
                        battle.ChainModify(3, 2); // 1.5x
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }),
                OnAfterHit = new OnAfterHitEventInfo((battle, target, source, _) =>
                {
                    if (source.Hp > 0)
                    {
                        ItemFalseUnion takeResult = target.TakeItem();
                        if (takeResult is ItemItemFalseUnion takenItem)
                        {
                            battle.Add("-enditem", target, takenItem.Item.Name,
                                "[from] move: Knock Off", $"[of] {source}");
                        }
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.KowtowCleave] = new()
            {
                Id = MoveId.KowtowCleave,
                Num = 869,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 85,
                Category = MoveCategory.Physical,
                Name = "Kowtow Cleave",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Slicing = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.Lashout] = new()
            {
                Id = MoveId.Lashout,
                Num = 808,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Lash Out",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, source, _, _) =>
                {
                    if (source.StatsLoweredThisTurn)
                    {
                        battle.Debug("lashout buff");
                        battle.ChainModify(2);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.LastResort] = new()
            {
                Id = MoveId.LastResort,
                Num = 387,
                Accuracy = 100,
                BasePower = 140,
                Category = MoveCategory.Physical,
                Name = "Last Resort",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnTry = new OnTryEventInfo((_, _, source, _) =>
                {
                    // Last Resort fails unless the user knows at least 2 moves
                    if (source.MoveSlots.Count < 2) return false;
                    var hasLastResort = false;
                    foreach (MoveSlot moveSlot in source.MoveSlots)
                    {
                        if (moveSlot.Id == MoveId.LastResort)
                        {
                            hasLastResort = true;
                            continue;
                        }

                        // All other moves must have been used
                        if (!moveSlot.Used) return false;
                    }

                    return hasLastResort ? new VoidReturn() : false;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.LastRespects] = new()
            {
                Id = MoveId.LastRespects,
                Num = 854,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Last Respects",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, source, _, _) =>
                {
                    int bp = 50 + 50 * source.Side.TotalFainted;
                    battle.Debug($"BP: {bp}");
                    return bp;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.LavaPlume] = new()
            {
                Id = MoveId.LavaPlume,
                Num = 436,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Lava Plume",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Fire,
            },
            [MoveId.Leafage] = new()
            {
                Id = MoveId.Leafage,
                Num = 670,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Leafage",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.LeafBlade] = new()
            {
                Id = MoveId.LeafBlade,
                Num = 348,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Leaf Blade",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Slicing = true,
                },
                CritRatio = 2,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.LeafStorm] = new()
            {
                Id = MoveId.LeafStorm,
                Num = 437,
                Accuracy = 90,
                BasePower = 130,
                Category = MoveCategory.Special,
                Name = "Leaf Storm",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { SpA = -2 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.LeechLife] = new()
            {
                Id = MoveId.LeechLife,
                Num = 141,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Leech Life",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Heal = true, Metronome = true
                },
                Drain = (1, 2),
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
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
            [MoveId.Leer] = new()
            {
                Id = MoveId.Leer,
                Num = 43,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Leer",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Reflectable = true, Mirror = true, Metronome = true },
                Boosts = new SparseBoostsTable { Def = -1 },
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Normal,
            },
            [MoveId.Lick] = new()
            {
                Id = MoveId.Lick,
                Num = 122,
                Accuracy = 100,
                BasePower = 30,
                Category = MoveCategory.Physical,
                Name = "Lick",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.LifeDew] = new()
            {
                Id = MoveId.LifeDew,
                Num = 791,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Life Dew",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Snatch = true, Heal = true, BypassSub = true },
                Heal = [1, 4],
                Secondary = null,
                Target = MoveTarget.Allies,
                Type = MoveType.Water,
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
            [MoveId.Liquidation] = new()
            {
                Id = MoveId.Liquidation,
                Num = 710,
                Accuracy = 100,
                BasePower = 85,
                Category = MoveCategory.Physical,
                Name = "Liquidation",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    Boosts = new SparseBoostsTable
                    {
                        Def = -1,
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.LockOn] = new()
            {
                Id = MoveId.LockOn,
                Num = 199,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Lock-On",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnTryHit = new OnTryHitEventInfo((_, _, source, _) =>
                {
                    // Fails if source already has lockon volatile
                    if (source.Volatiles.ContainsKey(ConditionId.LockOn))
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    source.AddVolatile(ConditionId.LockOn, target);
                    battle.Add("-activate", source, "move: Lock-On", $"[of] {target}");
                    return new VoidReturn();
                }),
                Condition = _library.Conditions[ConditionId.LockOn],
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
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
            [MoveId.LowSweep] = new()
            {
                Id = MoveId.LowSweep,
                Num = 490,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Physical,
                Name = "Low Sweep",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Spe = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.LuminaCrash] = new()
            {
                Id = MoveId.LuminaCrash,
                Num = 855,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Lumina Crash",
                BasePp = 10,
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
                    Boosts = new SparseBoostsTable
                    {
                        SpD = -2,
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.Lunge] = new()
            {
                Id = MoveId.Lunge,
                Num = 679,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Lunge",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Atk = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.LunarBlessing] = new()
            {
                Id = MoveId.LunarBlessing,
                Num = 849,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Lunar Blessing",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Heal = true,
                    Metronome = true,
                },
                OnHit = new OnHitEventInfo((battle, target, _, _) =>
                {
                    int healAmount = battle.Modify(target.MaxHp, 1, 4); // 25%
                    IntFalseUnion healResult = battle.Heal(healAmount, target);
                    bool success = healResult is not FalseIntFalseUnion;
                    bool cured = target.CureStatus();
                    return (cured || success) ? new VoidReturn() : false;
                }),
                Secondary = null,
                Target = MoveTarget.Allies,
                Type = MoveType.Psychic,
            },
            [MoveId.LunarDance] = new()
            {
                Id = MoveId.LunarDance,
                Num = 461,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Lunar Dance",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Dance = true,
                    Heal = true,
                    Metronome = true,
                },
                OnTryHit = new OnTryHitEventInfo((battle, _, source, _) =>
                {
                    // Fails if user cannot switch
                    if (battle.CanSwitch(source.Side) == 0)
                    {
                        battle.AttrLastMove("[still]");
                        battle.Add("-fail", source);
                        return new Empty(); // NOT_FAIL equivalent - move "worked" but did nothing
                    }

                    return new VoidReturn();
                }),
                SelfDestruct = MoveSelfDestruct.FromIfHit(),
                SlotCondition = ConditionId.LunarDance,
                Condition = _library.Conditions[ConditionId.LunarDance],
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.LusterPurge] = new()
            {
                Id = MoveId.LusterPurge,
                Num = 295,
                Accuracy = 100,
                BasePower = 95,
                Category = MoveCategory.Special,
                Name = "Luster Purge",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 50,
                    Boosts = new SparseBoostsTable
                    {
                        SpD = -1,
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
        };
    }
}