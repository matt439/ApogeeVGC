using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesDef()
    {
        return new Dictionary<MoveId, Move>
        {
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
            [MoveId.DarkestLariat] = new()
            {
                Id = MoveId.DarkestLariat,
                Num = 663,
                Accuracy = 100,
                BasePower = 85,
                Category = MoveCategory.Physical,
                Name = "Darkest Lariat",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                IgnoreEvasion = true,
                IgnoreDefensive = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.DarkPulse] = new()
            {
                Id = MoveId.DarkPulse,
                Num = 399,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Dark Pulse",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Distance = true, Metronome = true, Pulse = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Any,
                Type = MoveType.Dark,
            },
            [MoveId.DarkVoid] = new()
            {
                Id = MoveId.DarkVoid,
                Num = 464,
                Accuracy = 50,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Dark Void",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Reflectable = true, Mirror = true, Metronome = true },
                Status = ConditionId.Sleep,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Dark,
                // TODO: onTry - Darkrai-only check
            },
            [MoveId.Decorate] = new()
            {
                Id = MoveId.Decorate,
                Num = 777,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Decorate",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { AllyAnim = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Atk = 2, SpA = 2 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.DefendOrder] = new()
            {
                Id = MoveId.DefendOrder,
                Num = 455,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Defend Order",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                SelfBoost = new SparseBoostsTable { Def = 1, SpD = 1 },
                Target = MoveTarget.Self,
                Type = MoveType.Bug,
            },
            [MoveId.DefenseCurl] = new()
            {
                Id = MoveId.DefenseCurl,
                Num = 111,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Defense Curl",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                SelfBoost = new SparseBoostsTable { Def = 1 },
                VolatileStatus = ConditionId.DefenseCurl,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Defog] = new()
            {
                Id = MoveId.Defog,
                Num = 432,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Defog",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Reflectable = true, Mirror = true, BypassSub = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Flying,
                // TODO: onHit - lowers evasion, removes hazards, screens, terrain
            },
            [MoveId.DestinyBond] = new()
            {
                Id = MoveId.DestinyBond,
                Num = 194,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Destiny Bond",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { BypassSub = true, NoAssist = true, FailCopycat = true },
                VolatileStatus = ConditionId.DestinyBond,
                Target = MoveTarget.Self,
                Type = MoveType.Ghost,
                // TODO: onPrepareHit - check if already active
            },
            [MoveId.Detect] = new()
            {
                Id = MoveId.Detect,
                Num = 197,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Detect",
                BasePp = 5,
                Priority = 4,
                Flags = new MoveFlags { NoAssist = true, FailCopycat = true },
                StallingMove = true,
                VolatileStatus = ConditionId.Protect,
                Target = MoveTarget.Self,
                Type = MoveType.Fighting,
                // TODO: onPrepareHit and onHit for stalling logic
            },
            [MoveId.DiamondStorm] = new()
            {
                Id = MoveId.DiamondStorm,
                Num = 591,
                Accuracy = 95,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Diamond Storm",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Self = new SecondaryEffect
                {
                    Chance = 50,
                    Boosts = new SparseBoostsTable { Def = 2 },
                },
                Secondary = new SecondaryEffect
                {
                    // Sheer Force negates the self boost
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Rock,
            },
            [MoveId.Dig] = new()
            {
                Id = MoveId.Dig,
                Num = 91,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Dig",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Charge = true, Protect = true, Mirror = true, NonSky = true, Metronome = true, NoSleepTalk = true, NoAssist = true, FailInstruct = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
                // TODO: onTryMove - two-turn attack logic
            },
            [MoveId.Disable] = new()
            {
                Id = MoveId.Disable,
                Num = 50,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Disable",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Reflectable = true, Mirror = true, BypassSub = true, Metronome = true },
                VolatileStatus = ConditionId.Disable,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
                // TODO: onTryHit - check if target has last move
            },
            [MoveId.DisarmingVoice] = new()
            {
                Id = MoveId.DisarmingVoice,
                Num = 574,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Disarming Voice",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Sound = true, BypassSub = true, Metronome = true },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Fairy,
            },
            [MoveId.Discharge] = new()
            {
                Id = MoveId.Discharge,
                Num = 435,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Discharge",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Electric,
            },
            [MoveId.DireClaw] = new()
            {
                Id = MoveId.DireClaw,
                Num = 827,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Dire Claw",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 50,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
                // TODO: secondary.onHit - random status (poison, paralysis, or sleep)
            },
            [MoveId.Dive] = new()
            {
                Id = MoveId.Dive,
                Num = 291,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Dive",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Charge = true, Protect = true, Mirror = true, NonSky = true, AllyAnim = true, Metronome = true, NoSleepTalk = true, NoAssist = true, FailInstruct = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
                // TODO: onTryMove - two-turn attack logic, Cramorant interaction
            },
            [MoveId.Doodle] = new()
            {
                Id = MoveId.Doodle,
                Num = 867,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Doodle",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { },
                Target = MoveTarget.AdjacentFoe,
                Type = MoveType.Normal,
                // TODO: onHit - copy target's ability to user and allies
            },
            [MoveId.DoomDesire] = new()
            {
                Id = MoveId.DoomDesire,
                Num = 353,
                Accuracy = 100,
                BasePower = 140,
                Category = MoveCategory.Special,
                Name = "Doom Desire",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Metronome = true, FutureMove = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
                // TODO: onTry - future move setup
            },
            [MoveId.DoubleEdge] = new()
            {
                Id = MoveId.DoubleEdge,
                Num = 38,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Double-Edge",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Recoil = (33, 100),
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.DoubleHit] = new()
            {
                Id = MoveId.DoubleHit,
                Num = 458,
                Accuracy = 90,
                BasePower = 35,
                Category = MoveCategory.Physical,
                Name = "Double Hit",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                MultiHit = 2,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.DoubleKick] = new()
            {
                Id = MoveId.DoubleKick,
                Num = 24,
                Accuracy = 100,
                BasePower = 30,
                Category = MoveCategory.Physical,
                Name = "Double Kick",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                MultiHit = 2,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.DoubleShock] = new()
            {
                Id = MoveId.DoubleShock,
                Num = 892,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Double Shock",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true },
                Self = new SecondaryEffect
                {
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
                // TODO: onTryMove - check if user has Electric type
                // TODO: self.onHit - remove Electric type from user
            },
            [MoveId.Doubleteam] = new()
            {
                Id = MoveId.Doubleteam,
                Num = 104,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Double Team",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                SelfBoost = new SparseBoostsTable { Evasion = 1 },
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
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
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Self = new SecondaryEffect { Boosts = new SparseBoostsTable { SpA = -2 } },
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.DragonAscent] = new()
            {
                Id = MoveId.DragonAscent,
                Num = 620,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Dragon Ascent",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Distance = true },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { Def = -1, SpD = -1 },
                },
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.DragonBreath] = new()
            {
                Id = MoveId.DragonBreath,
                Num = 225,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Dragon Breath",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.DragonCheer] = new()
            {
                Id = MoveId.DragonCheer,
                Num = 913,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Dragon Cheer",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { BypassSub = true, AllyAnim = true, Metronome = true },
                VolatileStatus = ConditionId.DragonCheer,
                Target = MoveTarget.AdjacentAlly,
                Type = MoveType.Dragon,
            },
            [MoveId.DragonClaw] = new()
            {
                Id = MoveId.DragonClaw,
                Num = 337,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Dragon Claw",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.DragonDance] = new()
            {
                Id = MoveId.DragonDance,
                Num = 349,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Dragon Dance",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Dance = true, Metronome = true },
                SelfBoost = new SparseBoostsTable { Atk = 1, Spe = 1 },
                Target = MoveTarget.Self,
                Type = MoveType.Dragon,
            },
            [MoveId.DragonDarts] = new()
            {
                Id = MoveId.DragonDarts,
                Num = 751,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Dragon Darts",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                MultiHit = 2,
                SmartTarget = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.DragonEnergy] = new()
            {
                Id = MoveId.DragonEnergy,
                Num = 820,
                Accuracy = 100,
                BasePower = 150,
                Category = MoveCategory.Special,
                Name = "Dragon Energy",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Dragon,
                // TODO: basePowerCallback - scales with user's HP
            },
            [MoveId.DragonHammer] = new()
            {
                Id = MoveId.DragonHammer,
                Num = 692,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Dragon Hammer",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.DragonPulse] = new()
            {
                Id = MoveId.DragonPulse,
                Num = 406,
                Accuracy = 100,
                BasePower = 85,
                Category = MoveCategory.Special,
                Name = "Dragon Pulse",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Distance = true, Metronome = true, Pulse = true },
                Target = MoveTarget.Any,
                Type = MoveType.Dragon,
            },
            [MoveId.DragonRush] = new()
            {
                Id = MoveId.DragonRush,
                Num = 407,
                Accuracy = 75,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Dragon Rush",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.DragonTail] = new()
            {
                Id = MoveId.DragonTail,
                Num = 525,
                Accuracy = 90,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Dragon Tail",
                BasePp = 10,
                Priority = -6,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true, NoAssist = true, FailCopycat = true },
                ForceSwitch = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.DrainingKiss] = new()
            {
                Id = MoveId.DrainingKiss,
                Num = 577,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Draining Kiss",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Heal = true, Metronome = true },
                Drain = (3, 4),
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.DrainPunch] = new()
            {
                Id = MoveId.DrainPunch,
                Num = 409,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Drain Punch",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Punch = true, Heal = true, Metronome = true },
                Drain = (1, 2),
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.DreamEater] = new()
            {
                Id = MoveId.DreamEater,
                Num = 138,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Dream Eater",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Heal = true, Metronome = true },
                Drain = (1, 2),
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
                // TODO: onTryImmunity - only works on sleeping targets
            },
            [MoveId.DrillPeck] = new()
            {
                Id = MoveId.DrillPeck,
                Num = 65,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Drill Peck",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Distance = true, Metronome = true },
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.DrillRun] = new()
            {
                Id = MoveId.DrillRun,
                Num = 529,
                Accuracy = 95,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Drill Run",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                CritRatio = 2,
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
            },
            [MoveId.DrumBeating] = new()
            {
                Id = MoveId.DrumBeating,
                Num = 778,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Drum Beating",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Spe = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.DualWingbeat] = new()
            {
                Id = MoveId.DualWingbeat,
                Num = 814,
                Accuracy = 90,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Dual Wingbeat",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                MultiHit = 2,
                Target = MoveTarget.Normal,
                Type = MoveType.Flying,
            },
            [MoveId.DynamaxCannon] = new()
            {
                Id = MoveId.DynamaxCannon,
                Num = 744,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Dynamax Cannon",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, FailEncore = true, NoSleepTalk = true, FailCopycat = true, FailMimic = true, FailInstruct = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.DynamicPunch] = new()
            {
                Id = MoveId.DynamicPunch,
                Num = 223,
                Accuracy = 50,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Dynamic Punch",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Punch = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    VolatileStatus = ConditionId.Confusion,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
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
                                                        battle.Debug("[Facade.OnBasePower] Facade is increasing move damage.");
                                                        battle.ChainModify(2);
                                                    }

                                                    return new VoidReturn();
                                                }),
                                                Secondary = null,
                                                Target = MoveTarget.Normal,
                                                Type = MoveType.Normal,
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
                                        };
                                    }
                                }
