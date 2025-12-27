using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
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
                // TODO: onHit - add 'trapped' volatile to both source and target
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
                // TODO: onModifyType - change type based on plate held (item.onPlate)
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
                // TODO: onHit - heal 25% of target's max HP and cure status
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
                // TODO: onBasePower - 1.5x if target has a removable item
                // TODO: onAfterHit - remove target's item if source is not fainted
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
                // TODO: onTry - check if user knows at least 2 moves and all other moves have been used
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
                // TODO: basePowerCallback - 50 + 50 * pokemon.side.totalFainted
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
                // TODO: onTryHit - check if source already has lockon volatile
                // TODO: onHit - add lockon volatile to source targeting the target
                VolatileStatus = ConditionId.LockOn,
                Condition = _library.Conditions[ConditionId.LockOn],
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
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
                // TODO: onHit - heal 25% of target's max HP and cure status
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
                // TODO: onTryHit - check if user can switch
                SelfDestruct = MoveSelfDestruct.FromIfHit(),
                // TODO: slotCondition - lunardance that heals replacement pokemon
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
