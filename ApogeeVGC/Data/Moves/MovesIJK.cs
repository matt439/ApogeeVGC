using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesIjk()
    {
        return new Dictionary<MoveId, Move>
        {
            [MoveId.IceBeam] = new()
            {
                Id = MoveId.IceBeam,
                Num = 58,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Ice Beam",
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
                    Chance = 10,
                    Status = ConditionId.Freeze,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.IcePunch] = new()
            {
                Id = MoveId.IcePunch,
                Num = 8,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Ice Punch",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Punch = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Freeze,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.IcicleCrash] = new()
            {
                Id = MoveId.IcicleCrash,
                Num = 556,
                Accuracy = 90,
                BasePower = 85,
                Category = MoveCategory.Physical,
                Name = "Icicle Crash",
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
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.IcicleSpear] = new()
            {
                Id = MoveId.IcicleSpear,
                Num = 333,
                Accuracy = 100,
                BasePower = 25,
                Category = MoveCategory.Physical,
                Name = "Icicle Spear",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                MultiHit = new[] { 2, 5 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.IcyWind] = new()
            {
                Id = MoveId.IcyWind,
                Num = 196,
                Accuracy = 95,
                BasePower = 55,
                Category = MoveCategory.Special,
                Name = "Icy Wind",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Wind = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable
                    {
                        Spe = -1,
                    },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Ice,
            },
            [MoveId.Imprison] = new()
            {
                Id = MoveId.Imprison,
                Num = 286,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Imprison",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    BypassSub = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.Imprison,
                Condition = _library.Conditions[ConditionId.Imprison],
                OnTryHit = new OnTryHitEventInfo((_, _, source, _) =>
                {
                    // Check if at least one foe has one of the user's moves
                    if (source.Foes().Any(foe =>
                            source.MoveSlots.Where(moveSlot => moveSlot.Id != MoveId.Struggle)
                                .Any(moveSlot => foe.HasMove(moveSlot.Id))))
                    {
                        return new VoidReturn();
                    }

                    // No foe shares a move - fail
                    return false;
                }),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.Incinerate] = new()
            {
                Id = MoveId.Incinerate,
                Num = 510,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Incinerate",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    Item item = target.GetItem();
                    if (item.IsBerry || item.IsGem)
                    {
                        ItemFalseUnion takeResult = target.TakeItem(source);
                        if (takeResult is ItemItemFalseUnion takenItem)
                        {
                            battle.Add("-enditem", target, takenItem.Item.Name,
                                "[from] move: Incinerate");
                        }
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Fire,
            },
            [MoveId.Inferno] = new()
            {
                Id = MoveId.Inferno,
                Num = 517,
                Accuracy = 50,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Inferno",
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
                    Chance = 100,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.Infestation] = new()
            {
                Id = MoveId.Infestation,
                Num = 611,
                Accuracy = 100,
                BasePower = 20,
                Category = MoveCategory.Special,
                Name = "Infestation",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.PartiallyTrapped,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.Ingrain] = new()
            {
                Id = MoveId.Ingrain,
                Num = 275,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Ingrain",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    NonSky = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.Ingrain,
                Condition = _library.Conditions[ConditionId.Ingrain],
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Grass,
            },
            [MoveId.IronDefense] = new()
            {
                Id = MoveId.IronDefense,
                Num = 334,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Iron Defense",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                SelfBoost = new SparseBoostsTable
                {
                    Def = 2,
                },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Steel,
            },
            [MoveId.IronHead] = new()
            {
                Id = MoveId.IronHead,
                Num = 442,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Iron Head",
                BasePp = 15,
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
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
        };
    }
}