using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesPqr()
    {
        return new Dictionary<MoveId, Move>
        {
            // ===== P MOVES =====

            [MoveId.PainSplit] = new()
            {
                Id = MoveId.PainSplit,
                Num = 220,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Pain Split",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                // TODO: onHit - average HP between user and target
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.ParabolicCharge] = new()
            {
                Id = MoveId.ParabolicCharge,
                Num = 570,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Special,
                Name = "Parabolic Charge",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Heal = true,
                    Metronome = true,
                },
                Drain = (1, 2),
                Secondary = null,
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Electric,
            },
            [MoveId.PartingShot] = new()
            {
                Id = MoveId.PartingShot,
                Num = 575,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Parting Shot",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Sound = true,
                    BypassSub = true,
                    Metronome = true,
                },
                // TODO: boosts - lower target's Atk and SpA by 1
                SelfSwitch = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.Payback] = new()
            {
                Id = MoveId.Payback,
                Num = 371,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Payback",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // TODO: basePowerCallback - double power if target already moved this turn
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.PayDay] = new()
            {
                Id = MoveId.PayDay,
                Num = 6,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Pay Day",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Peck] = new()
            {
                Id = MoveId.Peck,
                Num = 64,
                Accuracy = 100,
                BasePower = 35,
                Category = MoveCategory.Physical,
                Name = "Peck",
                BasePp = 35,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Distance = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.PerishSong] = new()
            {
                Id = MoveId.PerishSong,
                Num = 195,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Perish Song",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Sound = true,
                    Distance = true,
                    BypassSub = true,
                    Metronome = true,
                },
                // TODO: onHitField - add perish song condition to all active Pokemon
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Normal,
            },
            [MoveId.PetalBlizzard] = new()
            {
                Id = MoveId.PetalBlizzard,
                Num = 572,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Petal Blizzard",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Wind = true,
                },
                Secondary = null,
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Grass,
            },
            [MoveId.PetalDance] = new()
            {
                Id = MoveId.PetalDance,
                Num = 80,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Petal Dance",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Dance = true,
                    Metronome = true,
                    FailInstruct = true,
                },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.LockedMove,
                },
                Secondary = null,
                Target = MoveTarget.RandomNormal,
                Type = MoveType.Grass,
            },
            [MoveId.PhantomForce] = new()
            {
                Id = MoveId.PhantomForce,
                Num = 566,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Phantom Force",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Charge = true,
                    Mirror = true,
                    Metronome = true,
                    NoSleepTalk = true,
                    NoAssist = true,
                    FailInstruct = true,
                },
                BreaksProtect = true,
                // TODO: onTryMove - vanish on turn 1, attack on turn 2
                Condition = _library.Conditions[ConditionId.PhantomForce],
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.PhotonGeyser] = new()
            {
                Id = MoveId.PhotonGeyser,
                Num = 722,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Photon Geyser",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                IgnoreAbility = true,
                // TODO: onModifyMove - use Atk instead of SpA if Atk > SpA
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PinMissile] = new()
            {
                Id = MoveId.PinMissile,
                Num = 42,
                Accuracy = 95,
                BasePower = 25,
                Category = MoveCategory.Physical,
                Name = "Pin Missile",
                BasePp = 20,
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
                Type = MoveType.Bug,
            },
            [MoveId.PlayNice] = new()
            {
                Id = MoveId.PlayNice,
                Num = 589,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Play Nice",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Reflectable = true,
                    Mirror = true,
                    BypassSub = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Atk = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.PlayRough] = new()
            {
                Id = MoveId.PlayRough,
                Num = 583,
                Accuracy = 90,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Play Rough",
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
                    Chance = 10,
                    Boosts = new SparseBoostsTable { Atk = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.Pluck] = new()
            {
                Id = MoveId.Pluck,
                Num = 365,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Pluck",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Distance = true,
                    Metronome = true,
                },
                // TODO: onAfterHit - eat target's berry
                Secondary = null,
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.PoisonFang] = new()
            {
                Id = MoveId.PoisonFang,
                Num = 305,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Poison Fang",
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
                    Chance = 50,
                    Status = ConditionId.Toxic,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.PoisonGas] = new()
            {
                Id = MoveId.PoisonGas,
                Num = 139,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Poison Gas",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Status = ConditionId.Poison,
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Poison,
            },
            [MoveId.PoisonJab] = new()
            {
                Id = MoveId.PoisonJab,
                Num = 398,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Poison Jab",
                BasePp = 20,
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
                    Status = ConditionId.Poison,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.PoisonPowder] = new()
            {
                Id = MoveId.PoisonPowder,
                Num = 77,
                Accuracy = 75,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Poison Powder",
                BasePp = 35,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                    Powder = true,
                },
                Status = ConditionId.Poison,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.PoisonSting] = new()
            {
                Id = MoveId.PoisonSting,
                Num = 40,
                Accuracy = 100,
                BasePower = 15,
                Category = MoveCategory.Physical,
                Name = "Poison Sting",
                BasePp = 35,
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
                    Status = ConditionId.Poison,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.PoisonTail] = new()
            {
                Id = MoveId.PoisonTail,
                Num = 342,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Poison Tail",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                CritRatio = 2,
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Poison,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.PollenPuff] = new()
            {
                Id = MoveId.PollenPuff,
                Num = 676,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Pollen Puff",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    AllyAnim = true,
                    Metronome = true,
                    Bullet = true,
                },
                // TODO: onTryHit - heal ally instead of damaging
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.Poltergeist] = new()
            {
                Id = MoveId.Poltergeist,
                Num = 809,
                Accuracy = 90,
                BasePower = 110,
                Category = MoveCategory.Physical,
                Name = "Poltergeist",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // TODO: onTry - fail if target has no item
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.PopulationBomb] = new()
            {
                Id = MoveId.PopulationBomb,
                Num = 860,
                Accuracy = 90,
                BasePower = 20,
                Category = MoveCategory.Physical,
                Name = "Population Bomb",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Slicing = true,
                },
                MultiHit = 10,
                MultiAccuracy = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Pounce] = new()
            {
                Id = MoveId.Pounce,
                Num = 884,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Pounce",
                BasePp = 20,
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
                    Boosts = new SparseBoostsTable { Spe = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.Pound] = new()
            {
                Id = MoveId.Pound,
                Num = 1,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Pound",
                BasePp = 35,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.PowderSnow] = new()
            {
                Id = MoveId.PowderSnow,
                Num = 181,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Powder Snow",
                BasePp = 25,
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
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Ice,
            },
            [MoveId.PowerGem] = new()
            {
                Id = MoveId.PowerGem,
                Num = 408,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Power Gem",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.PowerSplit] = new()
            {
                Id = MoveId.PowerSplit,
                Num = 471,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Power Split",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                // TODO: onHit - average Atk and SpA between user and target
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PowerSwap] = new()
            {
                Id = MoveId.PowerSwap,
                Num = 384,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Power Swap",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    BypassSub = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                // TODO: onHit - swap Atk and SpA boosts with target
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PowerTrick] = new()
            {
                Id = MoveId.PowerTrick,
                Num = 379,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Power Trick",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.PowerTrick,
                Condition = _library.Conditions[ConditionId.PowerTrick],
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.PowerTrip] = new()
            {
                Id = MoveId.PowerTrip,
                Num = 681,
                Accuracy = 100,
                BasePower = 20,
                Category = MoveCategory.Physical,
                Name = "Power Trip",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // TODO: basePowerCallback - add 20 per positive boost (base 20)
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.PowerUpPunch] = new()
            {
                Id = MoveId.PowerUpPunch,
                Num = 612,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Power-Up Punch",
                BasePp = 20,
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
                    Chance = 100,
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { Atk = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.PowerWhip] = new()
            {
                Id = MoveId.PowerWhip,
                Num = 438,
                Accuracy = 85,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Power Whip",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.PrecipiceBlades] = new()
            {
                Id = MoveId.PrecipiceBlades,
                Num = 619,
                Accuracy = 85,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Precipice Blades",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    NonSky = true,
                },
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Ground,
            },
            [MoveId.Present] = new()
            {
                Id = MoveId.Present,
                Num = 217,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Present",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // TODO: onModifyMove - random effect (heal or damage)
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.PrismaticLaser] = new()
            {
                Id = MoveId.PrismaticLaser,
                Num = 711,
                Accuracy = 100,
                BasePower = 160,
                Category = MoveCategory.Special,
                Name = "Prismatic Laser",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Recharge = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.MustRecharge,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
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

                OnPrepareHit = new OnPrepareHitEventInfo((battle, _, source, _) =>
                {
                    // source is the Pokemon using Protect
                    // Always run both checks, let Stall condition handle the logic
                    bool willAct = battle.Queue.WillAct() is not null;
                    RelayVar? stallResult = battle.RunEvent(EventId.StallMove, source);
                    bool stallSuccess = stallResult is BoolRelayVar { Value: true };
                    bool result = willAct && stallSuccess;

                    // Return BoolEmptyVoidUnion explicitly
                    return result ? true : (BoolEmptyVoidUnion)false;
                }),

                OnHit = new OnHitEventInfo((battle, _, source, _) =>
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
            [MoveId.PsyBeam] = new()
            {
                Id = MoveId.PsyBeam,
                Num = 60,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Special,
                Name = "Psybeam",
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
                    Chance = 10,
                    VolatileStatus = ConditionId.Confusion,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.Psyblade] = new()
            {
                Id = MoveId.Psyblade,
                Num = 875,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Psyblade",
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
                // TODO: onBasePower - 1.5x in Electric Terrain
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PsychUp] = new()
            {
                Id = MoveId.PsychUp,
                Num = 244,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Psych Up",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    BypassSub = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                // TODO: onHit - copy target's boosts
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Psychic] = new()
            {
                Id = MoveId.Psychic,
                Num = 94,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Psychic",
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
                    Boosts = new SparseBoostsTable { SpD = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PsychicFangs] = new()
            {
                Id = MoveId.PsychicFangs,
                Num = 706,
                Accuracy = 100,
                BasePower = 85,
                Category = MoveCategory.Physical,
                Name = "Psychic Fangs",
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
                // TODO: onAfterHit - remove Reflect, Light Screen, and Aurora Veil from target's side
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PsychicNoise] = new()
            {
                Id = MoveId.PsychicNoise,
                Num = 917,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Special,
                Name = "Psychic Noise",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Sound = true,
                    BypassSub = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    VolatileStatus = ConditionId.HealBlock,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PsychicTerrain] = new()
            {
                Id = MoveId.PsychicTerrain,
                Num = 678,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Psychic Terrain",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    NonSky = true,
                    Metronome = true,
                },
                Condition = _library.Conditions[ConditionId.PsychicTerrain],
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Psychic,
            },
            [MoveId.PsychoBoost] = new()
            {
                Id = MoveId.PsychoBoost,
                Num = 354,
                Accuracy = 90,
                BasePower = 140,
                Category = MoveCategory.Special,
                Name = "Psycho Boost",
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
                    Boosts = new SparseBoostsTable { SpA = -2 },
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PsychoCut] = new()
            {
                Id = MoveId.PsychoCut,
                Num = 427,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Psycho Cut",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Slicing = true,
                },
                CritRatio = 2,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PsyshieldBash] = new()
            {
                Id = MoveId.PsyshieldBash,
                Num = 828,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Psyshield Bash",
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
                    Chance = 100,
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { Def = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PsyShock] = new()
            {
                Id = MoveId.PsyShock,
                Num = 473,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Psyshock",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OverrideDefensiveStat = StatIdExceptHp.Def,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.Psystrike] = new()
            {
                Id = MoveId.Psystrike,
                Num = 540,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Psystrike",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OverrideDefensiveStat = StatIdExceptHp.Def,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.PyroBall] = new()
            {
                Id = MoveId.PyroBall,
                Num = 780,
                Accuracy = 90,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Pyro Ball",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Defrost = true,
                    Bullet = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },

            // ===== Q MOVES =====

            [MoveId.Quash] = new()
            {
                Id = MoveId.Quash,
                Num = 511,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Quash",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                // TODO: onHit - make target move last this turn (only in doubles/triples)
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.QuickAttack] = new()
            {
                Id = MoveId.QuickAttack,
                Num = 98,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Quick Attack",
                BasePp = 30,
                Priority = 1,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.QuickGuard] = new()
            {
                Id = MoveId.QuickGuard,
                Num = 501,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Quick Guard",
                BasePp = 15,
                Priority = 3,
                Flags = new MoveFlags
                {
                    Snatch = true,
                },
                SideCondition = ConditionId.QuickGuard,
                Condition = _library.Conditions[ConditionId.QuickGuard],
                // TODO: onTry - fail if ally side already has Quick Guard this turn
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Fighting,
            },
            [MoveId.QuiverDance] = new()
            {
                Id = MoveId.QuiverDance,
                Num = 483,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Quiver Dance",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Dance = true,
                    Metronome = true,
                },
                SelfBoost = new SparseBoostsTable { SpA = 1, SpD = 1, Spe = 1 },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Bug,
            },

            // ===== R MOVES =====

            [MoveId.RageFist] = new()
            {
                Id = MoveId.RageFist,
                Num = 889,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Rage Fist",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Punch = true,
                },
                // TODO: basePowerCallback - 50 + 50 * timesAttacked, max 350
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.RagePowder] = new()
            {
                Id = MoveId.RagePowder,
                Num = 476,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Rage Powder",
                BasePp = 20,
                Priority = 2,
                Flags = new MoveFlags
                {
                    NoAssist = true,
                    FailCopycat = true,
                    Powder = true,
                },
                VolatileStatus = ConditionId.RagePowder,
                Condition = _library.Conditions[ConditionId.RagePowder],
                // TODO: onTry - fail if not in doubles
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Bug,
            },
            [MoveId.RagingBull] = new()
            {
                Id = MoveId.RagingBull,
                Num = 873,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Raging Bull",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                },
                // TODO: onTryHit - remove screens before hitting
                // TODO: onModifyType - change type based on Tauros form
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.RagingFury] = new()
            {
                Id = MoveId.RagingFury,
                Num = 833,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Raging Fury",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.LockedMove,
                },
                Secondary = null,
                Target = MoveTarget.RandomNormal,
                Type = MoveType.Fire,
            },
            [MoveId.RainDance] = new()
            {
                Id = MoveId.RainDance,
                Num = 240,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Rain Dance",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Metronome = true,
                },
                Condition = _library.Conditions[ConditionId.RainDance],
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Water,
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
            [MoveId.RapidSpin] = new()
            {
                Id = MoveId.RapidSpin,
                Num = 229,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Rapid Spin",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // TODO: onAfterHit - remove hazards and volatile statuses
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { Spe = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.RazorLeaf] = new()
            {
                Id = MoveId.RazorLeaf,
                Num = 75,
                Accuracy = 95,
                BasePower = 55,
                Category = MoveCategory.Physical,
                Name = "Razor Leaf",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Slicing = true,
                },
                CritRatio = 2,
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Grass,
            },
            [MoveId.RazorShell] = new()
            {
                Id = MoveId.RazorShell,
                Num = 534,
                Accuracy = 95,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Razor Shell",
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
                Secondary = new SecondaryEffect
                {
                    Chance = 50,
                    Boosts = new SparseBoostsTable { Def = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.Recover] = new()
            {
                Id = MoveId.Recover,
                Num = 105,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Recover",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Heal = true,
                    Metronome = true,
                },
                Heal = [1, 2],
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.ReflectType] = new()
            {
                Id = MoveId.ReflectType,
                Num = 513,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Reflect Type",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    BypassSub = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                // TODO: onHit - copy target's types
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.RelicSong] = new()
            {
                Id = MoveId.RelicSong,
                Num = 547,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Special,
                Name = "Relic Song",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Sound = true,
                    BypassSub = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Sleep,
                },
                // TODO: onHit - change Meloetta forme
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Normal,
            },
            [MoveId.RevelationDance] = new()
            {
                Id = MoveId.RevelationDance,
                Num = 686,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Revelation Dance",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Dance = true,
                    Metronome = true,
                },
                // TODO: onModifyType - change to user's primary type
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.RisingVoltage] = new()
            {
                Id = MoveId.RisingVoltage,
                Num = 804,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Rising Voltage",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // TODO: basePowerCallback - double power on grounded targets in Electric Terrain
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.Roar] = new()
            {
                Id = MoveId.Roar,
                Num = 46,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Roar",
                BasePp = 20,
                Priority = -6,
                Flags = new MoveFlags
                {
                    Reflectable = true,
                    Mirror = true,
                    Sound = true,
                    BypassSub = true,
                    AllyAnim = true,
                    Metronome = true,
                    NoAssist = true,
                    FailCopycat = true,
                },
                ForceSwitch = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.RoarOfTime] = new()
            {
                Id = MoveId.RoarOfTime,
                Num = 459,
                Accuracy = 90,
                BasePower = 150,
                Category = MoveCategory.Special,
                Name = "Roar of Time",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Recharge = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.MustRecharge,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.RockBlast] = new()
            {
                Id = MoveId.RockBlast,
                Num = 350,
                Accuracy = 90,
                BasePower = 25,
                Category = MoveCategory.Physical,
                Name = "Rock Blast",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bullet = true,
                },
                MultiHit = new[] { 2, 5 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.RockPolish] = new()
            {
                Id = MoveId.RockPolish,
                Num = 397,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Rock Polish",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                SelfBoost = new SparseBoostsTable { Spe = 2 },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Rock,
            },
            [MoveId.RockSmash] = new()
            {
                Id = MoveId.RockSmash,
                Num = 249,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Rock Smash",
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
                    Chance = 50,
                    Boosts = new SparseBoostsTable { Def = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.RockThrow] = new()
            {
                Id = MoveId.RockThrow,
                Num = 88,
                Accuracy = 90,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Rock Throw",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.RockWrecker] = new()
            {
                Id = MoveId.RockWrecker,
                Num = 439,
                Accuracy = 90,
                BasePower = 150,
                Category = MoveCategory.Physical,
                Name = "Rock Wrecker",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Recharge = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bullet = true,
                },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.MustRecharge,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.RolePlay] = new()
            {
                Id = MoveId.RolePlay,
                Num = 272,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Role Play",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    BypassSub = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                // TODO: onTryHit - fail if same ability or certain abilities
                // TODO: onHit - copy target's ability
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.Rollout] = new()
            {
                Id = MoveId.Rollout,
                Num = 205,
                Accuracy = 90,
                BasePower = 30,
                Category = MoveCategory.Physical,
                Name = "Rollout",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    FailInstruct = true,
                    NoParentalBond = true,
                },
                // TODO: basePowerCallback - doubles each turn for 5 turns, 2x if Defense Curl used
                Condition = _library.Conditions[ConditionId.Rollout],
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.Ruination] = new()
            {
                Id = MoveId.Ruination,
                Num = 877,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Special,
                Name = "Ruination",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                // TODO: damageCallback - deal half of target's current HP
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
        };
    }
}
