using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;
using PokemonType = ApogeeVGC.Sim.PokemonClasses.PokemonType;

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
                OnTry = new OnTryEventInfo((battle, _, source, move) =>
                {
                    // Only Darkrai can use Dark Void (or if the move has bounced)
                    if (source.Species.Name == "Darkrai" || (move.HasBounced ?? false))
                    {
                        return new VoidReturn();
                    }
                    if (battle.DisplayUi)
                    {
                        battle.Hint("Only a Pokemon whose form is Darkrai can use this move.");
                    }
                    return false;
                }),
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
                OnPrepareHit = new OnPrepareHitEventInfo((battle, _, source, _) =>
                {
                    // Fails if Destiny Bond is already active on the user
                    // Returns false (move succeeds) if volatile was removed, true (fails) if it wasn't there
                    return !source.RemoveVolatile(battle.Library.Conditions[ConditionId.DestinyBond]);
                }),
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
                OnPrepareHit = new OnPrepareHitEventInfo((battle, _, source, _) =>
                {
                    // Check if queue will act and run StallMove event
                    bool willAct = battle.Queue.WillAct() is not null;
                    RelayVar? stallResult = battle.RunEvent(EventId.StallMove, source);
                    bool stallSuccess = stallResult is BoolRelayVar { Value: true };
                    bool result = willAct && stallSuccess;
                    return result ? true : (BoolEmptyVoidUnion)false;
                }),
                OnHit = new OnHitEventInfo((_, _, source, _) =>
                {
                    // Add Stall volatile to track consecutive uses
                    source.AddVolatile(ConditionId.Stall);
                    return new VoidReturn();
                }),
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
                OnTryHit = new OnTryHitEventInfo((_, target, _, _) =>
                {
                    // Fails if target has no last move, or if last move is a Z-move, Max move, or Struggle
                    if (target.LastMove == null || target.LastMove.Id == MoveId.Struggle)
                    {
                        return false;
                    }
                    // In the actual implementation, Z-moves and Max moves would also be checked
                    // but those aren't relevant for Gen 9 VGC
                    return new VoidReturn();
                }),
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
                    OnHit = (battle, target, source, _) =>
                    {
                        // Randomly inflict poison, paralysis, or sleep
                        var result = battle.Random(3);
                        if (result == 0)
                        {
                            target.TrySetStatus(ConditionId.Poison, source);
                        }
                        else if (result == 1)
                        {
                            target.TrySetStatus(ConditionId.Paralysis, source);
                        }
                        else
                        {
                            target.TrySetStatus(ConditionId.Sleep, source);
                        }
                        return new VoidReturn();
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
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
                    OnHit = (battle, _, source, _) =>
                    {
                        // Remove Electric type from user, replacing it with Unknown (???)
                        var currentTypes = source.GetTypes(true);
                        var newTypes = currentTypes.Select(t => t == PokemonType.Electric ? PokemonType.Unknown : t).ToArray();
                        source.SetType(newTypes);

                        if (battle.DisplayUi)
                        {
                            var typeString = string.Join("/", source.GetTypes().Where(t => t != PokemonType.Unknown).Select(t => t.ToString()));
                            battle.Add("-start", source, "typechange", typeString, "[from] move: Double Shock");
                        }
                        return new VoidReturn();
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
                OnTryMove = new OnTryMoveEventInfo((battle, _, source, _) =>
                {
                    // Only works if user has Electric type
                    if (source.HasType(PokemonType.Electric))
                    {
                        return new VoidReturn();
                    }
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fail", source, "move: Double Shock");
                    }
                    return false;
                }),
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
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, source, _, move) =>
                {
                    // Base power scales with user's HP percentage
                    var bp = move.BasePower * source.Hp / source.MaxHp;
                    battle.Debug($"[Dragon Energy] BP: {bp}");
                    return (int)bp;
                }),
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
                OnTryImmunity = new OnTryImmunityEventInfo((_, target, _, _) =>
                {
                    // Dream Eater only works on sleeping targets (or Pokemon with Comatose)
                    return target.Status == ConditionId.Sleep || target.HasAbility(AbilityId.Comatose);
                }),
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
            [MoveId.EarthPower] = new()
            {
                Id = MoveId.EarthPower,
                Num = 414,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Earth Power",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, NonSky = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Boosts = new SparseBoostsTable { SpD = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
            },
            [MoveId.Earthquake] = new()
            {
                Id = MoveId.Earthquake,
                Num = 89,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Earthquake",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, NonSky = true, Metronome = true },
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Ground,
            },
            [MoveId.EchoedVoice] = new()
            {
                Id = MoveId.EchoedVoice,
                Num = 497,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Echoed Voice",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Sound = true, BypassSub = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
                // TODO: onTryMove - add pseudoWeather 'echoedvoice'
                // TODO: basePowerCallback - scales with multiplier from pseudoWeather
            },
            [MoveId.EerieImpulse] = new()
            {
                Id = MoveId.EerieImpulse,
                Num = 598,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Eerie Impulse",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Reflectable = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { SpA = -2 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.EerieSpell] = new()
            {
                Id = MoveId.EerieSpell,
                Num = 826,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Eerie Spell",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Sound = true, BypassSub = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    OnHit = (battle, target, _, _) =>
                    {
                        // Only deduct PP if target still has HP
                        if (target.Hp <= 0) return new VoidReturn();

                        // Get target's last move
                        var lastMove = target.LastMove;
                        if (lastMove == null || lastMove.Id == MoveId.Struggle) return new VoidReturn();

                        // Deduct 3 PP from the last move
                        int ppDeducted = target.DeductPp(lastMove.Id, 3);

                        if (ppDeducted > 0 && battle.DisplayUi)
                        {
                            battle.Add("-activate", target, "move: Eerie Spell", lastMove.Name, ppDeducted);
                        }

                        return new VoidReturn();
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.ElectroBall] = new()
            {
                Id = MoveId.ElectroBall,
                Num = 486,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Special,
                Name = "Electro Ball",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true, Bullet = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, source, target, _) =>
                {
                    // Base power varies based on speed ratio
                    var targetSpeed = target.GetStat(StatIdExceptHp.Spe);
                    var ratio = targetSpeed > 0 ? (int)(source.GetStat(StatIdExceptHp.Spe) / targetSpeed) : 0;

                    // Clamp ratio to max of 4
                    if (ratio > 4) ratio = 4;

                    // BP values: [40, 60, 80, 120, 150] for ratios [0, 1, 2, 3, 4+]
                    var bp = ratio switch
                    {
                        0 => 40,
                        1 => 60,
                        2 => 80,
                        3 => 120,
                        _ => 150
                    };

                    battle.Debug($"[Electro Ball] BP: {bp}");
                    return bp;
                }),
            },
            [MoveId.ElectricTerrain] = new()
            {
                Id = MoveId.ElectricTerrain,
                Num = 604,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Electric Terrain",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { NonSky = true, Metronome = true },
                // TODO: Set terrain to ElectricTerrain
                Target = MoveTarget.All,
                Type = MoveType.Electric,
            },
            [MoveId.ElectroShot] = new()
            {
                Id = MoveId.ElectroShot,
                Num = 905,
                Accuracy = 100,
                BasePower = 130,
                Category = MoveCategory.Special,
                Name = "Electro Shot",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Charge = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
                // TODO: onTryMove - two-turn attack logic, boosts SpA, skips charge in rain
            },
            [MoveId.Electroweb] = new()
            {
                Id = MoveId.Electroweb,
                Num = 527,
                Accuracy = 95,
                BasePower = 55,
                Category = MoveCategory.Special,
                Name = "Electroweb",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Spe = -1 },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Electric,
            },
            [MoveId.Ember] = new()
            {
                Id = MoveId.Ember,
                Num = 52,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Ember",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.Encore] = new()
            {
                Id = MoveId.Encore,
                Num = 227,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Encore",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Reflectable = true, Mirror = true, BypassSub = true, Metronome = true, FailEncore = true },
                VolatileStatus = ConditionId.Encore,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Endeavor] = new()
            {
                Id = MoveId.Endeavor,
                Num = 283,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Endeavor",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true, NoParentalBond = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
                DamageCallback = new DamageCallbackEventInfo((_, source, target, _) =>
                {
                    // Damage equals target's HP minus user's HP (bringing target down to user's HP level)
                    return target.Hp - source.Hp;
                }),
                OnTryImmunity = new OnTryImmunityEventInfo((_, target, source, _) =>
                {
                    // Only works if user's HP is less than target's HP
                    return source.Hp < target.Hp;
                }),
            },
            [MoveId.Endure] = new()
            {
                Id = MoveId.Endure,
                Num = 203,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Endure",
                BasePp = 10,
                Priority = 4,
                Flags = new MoveFlags { NoAssist = true, FailCopycat = true },
                StallingMove = true,
                VolatileStatus = ConditionId.Endure,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
                OnPrepareHit = new OnPrepareHitEventInfo((battle, _, source, _) =>
                {
                    // Check if queue will act and run StallMove event
                    bool willAct = battle.Queue.WillAct() is not null;
                    RelayVar? stallResult = battle.RunEvent(EventId.StallMove, source);
                    bool stallSuccess = stallResult is BoolRelayVar { Value: true };
                    bool result = willAct && stallSuccess;
                    return result ? true : (BoolEmptyVoidUnion)false;
                }),
                OnHit = new OnHitEventInfo((_, _, source, _) =>
                {
                    // Add Stall volatile to track consecutive uses
                    source.AddVolatile(ConditionId.Stall);
                    return new VoidReturn();
                }),
            },
            [MoveId.EnergyBall] = new()
            {
                Id = MoveId.EnergyBall,
                Num = 412,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Energy Ball",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true, Bullet = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Boosts = new SparseBoostsTable { SpD = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.Entrainment] = new()
            {
                Id = MoveId.Entrainment,
                Num = 494,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Entrainment",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Reflectable = true, Mirror = true, AllyAnim = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
                // TODO: onTryHit - check for valid ability transfer
                // TODO: onHit - copy user's ability to target
            },
            [MoveId.Eruption] = new()
            {
                Id = MoveId.Eruption,
                Num = 284,
                Accuracy = 100,
                BasePower = 150,
                Category = MoveCategory.Special,
                Name = "Eruption",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Fire,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, source, _, move) =>
                {
                    // Base power scales with user's HP percentage
                    var bp = move.BasePower * source.Hp / source.MaxHp;
                    battle.Debug($"[Eruption] BP: {bp}");
                    return (int)bp;
                }),
            },
            [MoveId.EsperWing] = new()
            {
                Id = MoveId.EsperWing,
                Num = 840,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Esper Wing",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                CritRatio = 2,
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { Spe = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.ExpandingForce] = new()
            {
                Id = MoveId.ExpandingForce,
                Num = 797,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Expanding Force",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, source, _, _) =>
                {
                    // 1.5x base power in Psychic Terrain if user is grounded
                    if (battle.Field.IsTerrain(ConditionId.PsychicTerrain, source) && (source.IsGrounded() ?? false))
                    {
                        battle.Debug("terrain buff");
                        battle.ChainModify(3, 2); // 1.5x = 3/2
                        return battle.FinalModify(basePower);
                    }
                    return basePower;
                }),
                // TODO: onModifyMove - change target to allAdjacentFoes in Psychic Terrain
                // Cannot implement: Move.Target is init-only and cannot be modified in event handlers
            },
            [MoveId.Explosion] = new()
            {
                Id = MoveId.Explosion,
                Num = 153,
                Accuracy = 100,
                BasePower = 250,
                Category = MoveCategory.Physical,
                Name = "Explosion",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true, NoParentalBond = true },
                SelfDestruct = MoveSelfDestruct.FromAlways(),
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Normal,
            },
            [MoveId.Extrasensory] = new()
            {
                Id = MoveId.Extrasensory,
                Num = 326,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Extrasensory",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.ExtremeSpeed] = new()
            {
                Id = MoveId.ExtremeSpeed,
                Num = 245,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Extreme Speed",
                BasePp = 5,
                Priority = 2,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.FairyLock] = new()
            {
                Id = MoveId.FairyLock,
                Num = 587,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Fairy Lock",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Mirror = true, BypassSub = true, Metronome = true },
                PseudoWeather = ConditionId.FairyLock,
                Target = MoveTarget.All,
                Type = MoveType.Fairy,
            },
            [MoveId.FairyWind] = new()
            {
                Id = MoveId.FairyWind,
                Num = 584,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Fairy Wind",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true, Wind = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.FakeTears] = new()
            {
                Id = MoveId.FakeTears,
                Num = 313,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Fake Tears",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Reflectable = true, Mirror = true, AllyAnim = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { SpD = -2 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.FalseSurrender] = new()
            {
                Id = MoveId.FalseSurrender,
                Num = 793,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "False Surrender",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.FalseSwipe] = new()
            {
                Id = MoveId.FalseSwipe,
                Num = 206,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "False Swipe",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.FeatherDance] = new()
            {
                Id = MoveId.FeatherDance,
                Num = 297,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Feather Dance",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Reflectable = true, Mirror = true, Dance = true, AllyAnim = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Atk = -2 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Flying,
            },
            [MoveId.Feint] = new()
            {
                Id = MoveId.Feint,
                Num = 364,
                Accuracy = 100,
                BasePower = 30,
                Category = MoveCategory.Physical,
                Name = "Feint",
                BasePp = 10,
                Priority = 2,
                Flags = new MoveFlags { Mirror = true, Metronome = true, NoAssist = true, FailCopycat = true },
                BreaksProtect = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.FellStinger] = new()
            {
                Id = MoveId.FellStinger,
                Num = 565,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Fell Stinger",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.FickleBeam] = new()
            {
                Id = MoveId.FickleBeam,
                Num = 907,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Fickle Beam",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, pokemon, _, _) =>
                {
                    // 30% chance to double base power
                    if (battle.RandomChance(3, 10))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "move: Fickle Beam");
                        }
                        battle.ChainModify(2);
                        return battle.FinalModify(basePower);
                    }
                    return basePower;
                }),
            },
            [MoveId.FieryDance] = new()
            {
                Id = MoveId.FieryDance,
                Num = 552,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Fiery Dance",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Dance = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 50,
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { SpA = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.FieryWrath] = new()
            {
                Id = MoveId.FieryWrath,
                Num = 822,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Fiery Wrath",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Dark,
            },
            [MoveId.FilletAway] = new()
            {
                Id = MoveId.FilletAway,
                Num = 868,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Fillet Away",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true },
                SelfBoost = new SparseBoostsTable { Atk = 2, SpA = 2, Spe = 2 },
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
                OnTry = new OnTryEventInfo((_, _, source, _) =>
                {
                    // Fail if HP is less than or equal to half of max HP, or if max HP is 1
                    if (source.Hp <= source.MaxHp / 2 || source.MaxHp == 1)
                    {
                        return false;
                    }
                    return new VoidReturn();
                }),
                OnHit = new OnHitEventInfo((battle, target, _, _) =>
                {
                    // Deal direct damage equal to half of max HP
                    battle.DirectDamage(target.MaxHp / 2, target);
                    return new VoidReturn();
                }),
            },
            [MoveId.FinalGambit] = new()
            {
                Id = MoveId.FinalGambit,
                Num = 515,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Special,
                Name = "Final Gambit",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Metronome = true, NoParentalBond = true },
                SelfDestruct = MoveSelfDestruct.FromIfHit(),
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
                DamageCallback = new DamageCallbackEventInfo((_, source, _, _) =>
                {
                    // Damage equals user's HP, and user faints (handled by SelfDestruct)
                    return source.Hp;
                }),
            },
            [MoveId.FireBlast] = new()
            {
                Id = MoveId.FireBlast,
                Num = 126,
                Accuracy = 85,
                BasePower = 110,
                Category = MoveCategory.Special,
                Name = "Fire Blast",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.FireFang] = new()
            {
                Id = MoveId.FireFang,
                Num = 424,
                Accuracy = 95,
                BasePower = 65,
                Category = MoveCategory.Physical,
                Name = "Fire Fang",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true, Bite = true },
                Secondaries = new[]
                {
                    new SecondaryEffect { Chance = 10, Status = ConditionId.Burn },
                    new SecondaryEffect { Chance = 10, VolatileStatus = ConditionId.Flinch },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.FireLash] = new()
            {
                Id = MoveId.FireLash,
                Num = 680,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Fire Lash",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Def = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.FirePunch] = new()
            {
                Id = MoveId.FirePunch,
                Num = 7,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Fire Punch",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Punch = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.FireSpin] = new()
            {
                Id = MoveId.FireSpin,
                Num = 83,
                Accuracy = 85,
                BasePower = 35,
                Category = MoveCategory.Special,
                Name = "Fire Spin",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                VolatileStatus = ConditionId.PartiallyTrapped,
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.FirePledge] = new()
            {
                Id = MoveId.FirePledge,
                Num = 519,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Fire Pledge",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, NonSky = true, Metronome = true, PledgeCombo = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.FirstImpression] = new()
            {
                Id = MoveId.FirstImpression,
                Num = 660,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "First Impression",
                BasePp = 10,
                Priority = 2,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
                OnTry = new OnTryEventInfo((battle, source, _, _) =>
                {
                    if (source.ActiveMoveActions <= 1) return new VoidReturn();
                    if (battle.DisplayUi)
                    {
                        battle.Hint("First Impression only works on your first turn out.");
                    }
                    return false;
                }),
            },
            [MoveId.Fissure] = new()
            {
                Id = MoveId.Fissure,
                Num = 90,
                Accuracy = 30,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Fissure",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, NonSky = true, Metronome = true },
                Ohko = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
            },
            [MoveId.Flail] = new()
            {
                Id = MoveId.Flail,
                Num = 175,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Flail",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.FlameCharge] = new()
            {
                Id = MoveId.FlameCharge,
                Num = 488,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Flame Charge",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { Spe = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.Flamethrower] = new()
            {
                Id = MoveId.Flamethrower,
                Num = 53,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Flamethrower",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.FlameWheel] = new()
            {
                Id = MoveId.FlameWheel,
                Num = 172,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Flame Wheel",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Defrost = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.FlareBlitz] = new()
            {
                Id = MoveId.FlareBlitz,
                Num = 394,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Flare Blitz",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Defrost = true, Metronome = true },
                Recoil = (33, 100),
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.FlashCannon] = new()
            {
                Id = MoveId.FlashCannon,
                Num = 430,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Flash Cannon",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Boosts = new SparseBoostsTable { SpD = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.Flatter] = new()
            {
                Id = MoveId.Flatter,
                Num = 260,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Flatter",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Reflectable = true, Mirror = true, AllyAnim = true, Metronome = true },
                VolatileStatus = ConditionId.Confusion,
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { SpA = 1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.FleurCannon] = new()
            {
                Id = MoveId.FleurCannon,
                Num = 705,
                Accuracy = 90,
                BasePower = 130,
                Category = MoveCategory.Special,
                Name = "Fleur Cannon",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { SpA = -2 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.Fling] = new()
            {
                Id = MoveId.Fling,
                Num = 374,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Fling",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, AllyAnim = true, Metronome = true, NoParentalBond = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.FlipTurn] = new()
            {
                Id = MoveId.FlipTurn,
                Num = 812,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Flip Turn",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                SelfSwitch = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.FloralHealing] = new()
            {
                Id = MoveId.FloralHealing,
                Num = 666,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Floral Healing",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Reflectable = true, Heal = true, AllyAnim = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    int healAmount;
                    // Check if Grassy Terrain is active
                    if (battle.Field.IsTerrain(ConditionId.GrassyTerrain, target))
                    {
                        // Heal 2/3 of max HP in Grassy Terrain
                        healAmount = battle.Modify(target.BaseMaxHp, 2, 3);
                    }
                    else
                    {
                        // Heal 1/2 of max HP normally (using ceiling to match PS)
                        healAmount = (int)Math.Ceiling(target.BaseMaxHp * 0.5);
                    }

                    var result = battle.Heal(healAmount, target);

                    // Mark as externally influenced if healing opponent
                    if (result is not FalseIntFalseUnion && !target.IsAlly(source))
                    {
                        target.Staleness = StalenessId.External;
                    }

                    // Show fail message if healing failed
                    if (result is FalseIntFalseUnion)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-fail", target, "heal");
                        }
                        return false;
                    }

                    return new VoidReturn();
                }),
            },
            [MoveId.FlowerTrick] = new()
            {
                Id = MoveId.FlowerTrick,
                Num = 870,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Flower Trick",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                WillCrit = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.Fly] = new()
            {
                Id = MoveId.Fly,
                Num = 19,
                Accuracy = 95,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Fly",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Charge = true, Protect = true, Mirror = true, Gravity = true, Distance = true, Metronome = true, NoSleepTalk = true, NoAssist = true, FailInstruct = true },
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.FlyingPress] = new()
            {
                Id = MoveId.FlyingPress,
                Num = 560,
                Accuracy = 95,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Flying Press",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Gravity = true, Distance = true, NonSky = true, Metronome = true },
                Target = MoveTarget.Any,
                Type = MoveType.Fighting,
            },
            [MoveId.FocusBlast] = new()
            {
                Id = MoveId.FocusBlast,
                Num = 411,
                Accuracy = 70,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Focus Blast",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true, Bullet = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Boosts = new SparseBoostsTable { SpD = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.FocusEnergy] = new()
            {
                Id = MoveId.FocusEnergy,
                Num = 116,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Focus Energy",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                VolatileStatus = ConditionId.FocusEnergy,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.FocusPunch] = new()
            {
                Id = MoveId.FocusPunch,
                Num = 264,
                Accuracy = 100,
                BasePower = 150,
                Category = MoveCategory.Physical,
                Name = "Focus Punch",
                BasePp = 20,
                Priority = -3,
                Flags = new MoveFlags { Contact = true, Protect = true, Punch = true, FailMeFirst = true, NoSleepTalk = true, FailInstruct = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.FollowMe] = new()
            {
                Id = MoveId.FollowMe,
                Num = 266,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Follow Me",
                BasePp = 20,
                Priority = 2,
                Flags = new MoveFlags { NoAssist = true, FailCopycat = true },
                VolatileStatus = ConditionId.FollowMe,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.ForcePalm] = new()
            {
                Id = MoveId.ForcePalm,
                Num = 395,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Force Palm",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.ForestsCurse] = new()
            {
                Id = MoveId.ForestsCurse,
                Num = 571,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Forest's Curse",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Reflectable = true, Mirror = true, AllyAnim = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.FoulPlay] = new()
            {
                Id = MoveId.FoulPlay,
                Num = 492,
                Accuracy = 100,
                BasePower = 95,
                Category = MoveCategory.Physical,
                Name = "Foul Play",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                OverrideOffensiveStat = StatIdExceptHp.Atk,
                OverrideOffensivePokemon = MoveOverridePokemon.Target,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.FreezeDry] = new()
            {
                Id = MoveId.FreezeDry,
                Num = 573,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Freeze-Dry",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Freeze,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.FreezeShock] = new()
            {
                Id = MoveId.FreezeShock,
                Num = 553,
                Accuracy = 90,
                BasePower = 140,
                Category = MoveCategory.Physical,
                Name = "Freeze Shock",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Charge = true, Protect = true, Mirror = true, NoSleepTalk = true, FailInstruct = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
                // TODO: onTryMove - two-turn attack logic
            },
            [MoveId.FreezingGlare] = new()
            {
                Id = MoveId.FreezingGlare,
                Num = 821,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Freezing Glare",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Freeze,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.FrenzyPlant] = new()
            {
                Id = MoveId.FrenzyPlant,
                Num = 338,
                Accuracy = 90,
                BasePower = 150,
                Category = MoveCategory.Special,
                Name = "Frenzy Plant",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Recharge = true, Protect = true, Mirror = true, NonSky = true, Metronome = true },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.MustRecharge,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.FrostBreath] = new()
            {
                Id = MoveId.FrostBreath,
                Num = 524,
                Accuracy = 90,
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Frost Breath",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                WillCrit = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.Frustration] = new()
            {
                Id = MoveId.Frustration,
                Num = 218,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Frustration",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.FuryAttack] = new()
            {
                Id = MoveId.FuryAttack,
                Num = 31,
                Accuracy = 85,
                BasePower = 15,
                Category = MoveCategory.Physical,
                Name = "Fury Attack",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                MultiHit = new[] { 2, 5 },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.FuryCutter] = new()
            {
                Id = MoveId.FuryCutter,
                Num = 210,
                Accuracy = 95,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Fury Cutter",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true, Slicing = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.FurySwipes] = new()
            {
                Id = MoveId.FurySwipes,
                Num = 154,
                Accuracy = 80,
                BasePower = 18,
                Category = MoveCategory.Physical,
                Name = "Fury Swipes",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true, Metronome = true },
                MultiHit = new[] { 2, 5 },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.FusionBolt] = new()
            {
                Id = MoveId.FusionBolt,
                Num = 559,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Fusion Bolt",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, _) =>
                {
                    // Double base power if Fusion Flare was used successfully this turn
                    if (battle.LastSuccessfulMoveThisTurn == MoveId.FusionFlare)
                    {
                        battle.Debug("double power");
                        battle.ChainModify(2);
                        return battle.FinalModify(basePower);
                    }
                    return basePower;
                }),
            },
            [MoveId.FusionFlare] = new()
            {
                Id = MoveId.FusionFlare,
                Num = 558,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Fusion Flare",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Defrost = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.FutureSight] = new()
            {
                Id = MoveId.FutureSight,
                Num = 248,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Future Sight",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Metronome = true, FutureMove = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
                                        };
                                    }
                                }
