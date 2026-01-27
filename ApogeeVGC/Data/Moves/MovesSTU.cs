using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesStu()
    {
        return new Dictionary<MoveId, Move>
        {
            // ===== S MOVES =====

            [MoveId.SacredFire] = new()
            {
                Id = MoveId.SacredFire,
                Num = 221,
                Accuracy = 95,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Sacred Fire",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Defrost = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 50,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.SacredSword] = new()
            {
                Id = MoveId.SacredSword,
                Num = 533,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Sacred Sword",
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
                IgnoreEvasion = true,
                IgnoreDefensive = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.Safeguard] = new()
            {
                Id = MoveId.Safeguard,
                Num = 219,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Safeguard",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                SideCondition = ConditionId.Safeguard,
                Condition = _library.Conditions[ConditionId.Safeguard],
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Normal,
            },
            [MoveId.SaltCure] = new()
            {
                Id = MoveId.SaltCure,
                Num = 864,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Salt Cure",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    VolatileStatus = ConditionId.SaltCure,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.SandAttack] = new()
            {
                Id = MoveId.SandAttack,
                Num = 28,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Sand Attack",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
                Boosts = new SparseBoostsTable { Accuracy = -1 },
            },
            [MoveId.SandsearStorm] = new()
            {
                Id = MoveId.SandsearStorm,
                Num = 848,
                Accuracy = 80,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Sandsear Storm",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Wind = true,
                },
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, _, _) =>
                {
                    var weather = battle.Field.EffectiveWeather();
                    if (weather is ConditionId.RainDance or ConditionId.PrimordialSea)
                    {
                        move.Accuracy = IntTrueUnion.FromTrue();
                    }
                }),
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Ground,
            },
            [MoveId.Sandstorm] = new()
            {
                Id = MoveId.Sandstorm,
                Num = 201,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Sandstorm",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Metronome = true,
                    Wind = true,
                },
                Weather = ConditionId.Sandstorm,
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Rock,
            },
            [MoveId.SandTomb] = new()
            {
                Id = MoveId.SandTomb,
                Num = 328,
                Accuracy = 85,
                BasePower = 35,
                Category = MoveCategory.Physical,
                Name = "Sand Tomb",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.PartiallyTrapped,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
            },
            [MoveId.Scald] = new()
            {
                Id = MoveId.Scald,
                Num = 503,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Scald",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Defrost = true,
                    Metronome = true,
                },
                ThawsTarget = true,
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.ScaleShot] = new()
            {
                Id = MoveId.ScaleShot,
                Num = 799,
                Accuracy = 90,
                BasePower = 25,
                Category = MoveCategory.Physical,
                Name = "Scale Shot",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                MultiHit = new[] { 2, 5 },
                SelfBoost = new SparseBoostsTable { Def = -1, Spe = 1 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.ScaryFace] = new()
            {
                Id = MoveId.ScaryFace,
                Num = 184,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Scary Face",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Spe = -2 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.ScorchingSands] = new()
            {
                Id = MoveId.ScorchingSands,
                Num = 815,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Scorching Sands",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Defrost = true,
                    Metronome = true,
                },
                ThawsTarget = true,
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
            },
            [MoveId.Scratch] = new()
            {
                Id = MoveId.Scratch,
                Num = 10,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Scratch",
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
            [MoveId.Screech] = new()
            {
                Id = MoveId.Screech,
                Num = 103,
                Accuracy = 85,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Screech",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Sound = true,
                    BypassSub = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Def = -2 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.SecretSword] = new()
            {
                Id = MoveId.SecretSword,
                Num = 548,
                Accuracy = 100,
                BasePower = 85,
                Category = MoveCategory.Special,
                Name = "Secret Sword",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Slicing = true,
                },
                OverrideDefensiveStat = StatIdExceptHp.Def,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.SeedBomb] = new()
            {
                Id = MoveId.SeedBomb,
                Num = 402,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Seed Bomb",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bullet = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.SeedFlare] = new()
            {
                Id = MoveId.SeedFlare,
                Num = 465,
                Accuracy = 85,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Seed Flare",
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
                    Chance = 40,
                    Boosts = new SparseBoostsTable { SpD = -2 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.SeismicToss] = new()
            {
                Id = MoveId.SeismicToss,
                Num = 69,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Seismic Toss",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    NonSky = true,
                    Metronome = true,
                },
                Damage = MoveDamage.FromLevel(),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.SelfDestruct] = new()
            {
                Id = MoveId.SelfDestruct,
                Num = 120,
                Accuracy = 100,
                BasePower = 200,
                Category = MoveCategory.Physical,
                Name = "Self-Destruct",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    NoParentalBond = true,
                },
                SelfDestruct = MoveSelfDestruct.FromAlways(),
                Secondary = null,
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Normal,
            },
            [MoveId.ShadowBall] = new()
            {
                Id = MoveId.ShadowBall,
                Num = 247,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Shadow Ball",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bullet = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    Boosts = new SparseBoostsTable { SpD = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.ShadowClaw] = new()
            {
                Id = MoveId.ShadowClaw,
                Num = 421,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Shadow Claw",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                CritRatio = 2,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.ShadowForce] = new()
            {
                Id = MoveId.ShadowForce,
                Num = 467,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Shadow Force",
                BasePp = 5,
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
                OnTryMove = new OnTryMoveEventInfo((battle, source, target, move) =>
                {
                    // If we have the volatile from turn 1, remove it and continue with the attack
                    if (source.RemoveVolatile(battle.Library.Conditions[ConditionId.TwoTurnMove]))
                    {
                        return new VoidReturn(); // Continue with the attack
                    }

                    // Turn 1: Prepare the move
                    battle.Add("-prepare", source, move.Name);
                    var chargeResult =
                        battle.RunEvent(EventId.ChargeMove, source, target, move);
                    if (chargeResult is BoolRelayVar { Value: false })
                    {
                        return new VoidReturn(); // ChargeMove failed - continue but don't set up
                    }

                    source.AddVolatile(ConditionId.TwoTurnMove, target);
                    return null; // Stop the move on turn 1
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.ShadowPunch] = new()
            {
                Id = MoveId.ShadowPunch,
                Num = 325,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Shadow Punch",
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
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.ShadowSneak] = new()
            {
                Id = MoveId.ShadowSneak,
                Num = 425,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Shadow Sneak",
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
                Type = MoveType.Ghost,
            },
            [MoveId.ShedTail] = new()
            {
                Id = MoveId.ShedTail,
                Num = 880,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Shed Tail",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags(),
                VolatileStatus = ConditionId.Substitute,
                OnTryHit = new OnTryHitEventInfo((battle, source, _, _) =>
                {
                    if (battle.CanSwitch(source.Side) == 0 ||
                        source.Volatiles.ContainsKey(ConditionId.Commanded))
                    {
                        battle.Add("-fail", source);
                        return new Empty();
                    }

                    if (source.Volatiles.ContainsKey(ConditionId.Substitute))
                    {
                        battle.Add("-fail", source, "move: Shed Tail");
                        return new Empty();
                    }

                    if (source.Hp <= Math.Ceiling(source.MaxHp / 2.0))
                    {
                        battle.Add("-fail", source, "move: Shed Tail", "[weak]");
                        return new Empty();
                    }

                    return new VoidReturn();
                }),
                OnHit = new OnHitEventInfo((battle, target, _, _) =>
                {
                    battle.DirectDamage((int)Math.Ceiling(target.MaxHp / 2.0));
                    return new VoidReturn();
                }),
                Self = new SecondaryEffect
                {
                    OnHit = (_, _, source, _) =>
                    {
                        source.SkipBeforeSwitchOutEventFlag = true;
                        return new VoidReturn();
                    },
                },
                SelfSwitch = MoveSelfSwitch.FromShedTail(),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.SheerCold] = new()
            {
                Id = MoveId.SheerCold,
                Num = 329,
                Accuracy = 30,
                BasePower = 0,
                Category = MoveCategory.Special,
                Name = "Sheer Cold",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Ohko = MoveOhko.FromIce(),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.ShellSideArm] = new()
            {
                Id = MoveId.ShellSideArm,
                Num = 801,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Shell Side Arm",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnPrepareHit = new OnPrepareHitEventInfo((battle, source, target, move) =>
                {
                    if (!source.IsAlly(target))
                    {
                        battle.AttrLastMove("[anim] Shell Side Arm " + move.Category);
                    }

                    return new VoidReturn();
                }),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, target) =>
                {
                    if (target == null) return;
                    var atk = pokemon.GetStat(StatIdExceptHp.Atk, false, true);
                    var spa = pokemon.GetStat(StatIdExceptHp.SpA, false, true);
                    var def = target.GetStat(StatIdExceptHp.Def, false, true);
                    var spd = target.GetStat(StatIdExceptHp.SpD, false, true);
                    var physical = (int)Math.Floor(Math.Floor(
                        Math.Floor(Math.Floor(2.0 * pokemon.Level / 5 + 2) * 90 * atk) / def) / 50);
                    var special = (int)Math.Floor(Math.Floor(
                        Math.Floor(Math.Floor(2.0 * pokemon.Level / 5 + 2) * 90 * spa) / spd) / 50);
                    if (physical > special || (physical == special && battle.RandomChance(1, 2)))
                    {
                        move.Category = MoveCategory.Physical;
                        move.Flags.Contact = true;
                    }
                }),
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    if (!source.IsAlly(target)) battle.Hint(move.Category + " Shell Side Arm");
                    return new VoidReturn();
                }),
                OnAfterSubDamage =
                    new OnAfterSubDamageEventInfo((battle, _, target, source, move) =>
                    {
                        if (!source.IsAlly(target)) battle.Hint(move.Category + " Shell Side Arm");
                    }),
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    Status = ConditionId.Poison,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.ShellSmash] = new()
            {
                Id = MoveId.ShellSmash,
                Num = 504,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Shell Smash",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Def = -1, SpD = -1, Atk = 2, SpA = 2, Spe = 2 },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Shelter] = new()
            {
                Id = MoveId.Shelter,
                Num = 842,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Shelter",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Def = 2 },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Steel,
            },
            [MoveId.ShiftGear] = new()
            {
                Id = MoveId.ShiftGear,
                Num = 508,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Shift Gear",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Spe = 2, Atk = 1 },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Steel,
            },
            [MoveId.ShockWave] = new()
            {
                Id = MoveId.ShockWave,
                Num = 351,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Shock Wave",
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
                Type = MoveType.Electric,
            },
            [MoveId.ShoreUp] = new()
            {
                Id = MoveId.ShoreUp,
                Num = 659,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Shore Up",
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
                    var factor = 0.5;
                    // Heal 2/3 in Sandstorm, 1/2 otherwise
                    if (battle.Field.IsWeather(ConditionId.Sandstorm))
                    {
                        factor = 0.667;
                    }

                    var healAmount = battle.Modify(target.MaxHp, factor);
                    var result = battle.Heal(healAmount, target);

                    if (result is FalseIntFalseUnion)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-fail", target, "heal");
                        }

                        return new Empty(); // NOT_FAIL - move worked but heal failed
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Ground,
            },
            [MoveId.SilkTrap] = new()
            {
                Id = MoveId.SilkTrap,
                Num = 852,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Silk Trap",
                BasePp = 10,
                Priority = 4,
                Flags = new MoveFlags(),
                StallingMove = true,
                VolatileStatus = ConditionId.SilkTrap,
                OnPrepareHit = new OnPrepareHitEventInfo((battle, pokemon, _, _) =>
                    battle.Queue.WillAct() is not null &&
                    battle.RunEvent(EventId.StallMove, pokemon) is not null
                        ? new VoidReturn()
                        : false),
                OnHit = new OnHitEventInfo((_, pokemon, _, _) =>
                {
                    pokemon.AddVolatile(ConditionId.Stall);
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Bug,
            },
            [MoveId.SimpleBeam] = new()
            {
                Id = MoveId.SimpleBeam,
                Num = 493,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Simple Beam",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                OnTryHit = new OnTryHitEventInfo((_, target, _, _) =>
                {
                    // Fails if ability can't be suppressed, already Simple, or has Truant
                    var targetAbility = target.GetAbility();
                    if (targetAbility.Flags.CantSuppress == true ||
                        target.Ability == AbilityId.Simple ||
                        target.Ability == AbilityId.Truant)
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
                OnHit = new OnHitEventInfo((_, target, source, _) =>
                {
                    var oldAbility = target.SetAbility(AbilityId.Simple, source);
                    if (oldAbility != null)
                    {
                        return new VoidReturn();
                    }

                    return false;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Sing] = new()
            {
                Id = MoveId.Sing,
                Num = 47,
                Accuracy = 55,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Sing",
                BasePp = 15,
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
                Status = ConditionId.Sleep,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Sketch] = new()
            {
                Id = MoveId.Sketch,
                Num = 166,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Sketch",
                BasePp = 1,
                NoPpBoosts = true,
                Priority = 0,
                Flags = new MoveFlags
                {
                    BypassSub = true,
                    AllyAnim = true,
                    FailEncore = true,
                    NoSleepTalk = true,
                    NoAssist = true,
                    FailCopycat = true,
                    FailMimic = true,
                    FailInstruct = true,
                    NoSketch = true,
                },
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    var move = target.LastMove;
                    if (source.Transformed || move == null || source.Moves.Contains(move.Id))
                        return false;
                    if (move.Flags.NoSketch == true) return false;

                    var sketchIndex = source.Moves.IndexOf(MoveId.Sketch);
                    if (sketchIndex < 0) return false;

                    var sketchedMove = new MoveSlot()
                    {
                        Move = move.Id,
                        Pp = move.BasePp,
                        MaxPp = move.BasePp,
                        Target = move.Target,
                        Disabled = false,
                        Used = false,
                    };
                    source.MoveSlots[sketchIndex] = sketchedMove;
                    source.BaseMoveSlots[sketchIndex] = sketchedMove;
                    battle.Add("-activate", source, "move: Sketch", move.Name);
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.SkillSwap] = new()
            {
                Id = MoveId.SkillSwap,
                Num = 285,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Skill Swap",
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
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    var targetAbility = target.GetAbility();
                    var sourceAbility = source.GetAbility();
                    if (sourceAbility.Flags.FailSkillSwap == true ||
                        targetAbility.Flags.FailSkillSwap == true)
                    {
                        return false;
                    }

                    // Check if source can receive target's ability
                    var sourceCanBeSet = battle.RunEvent(EventId.SetAbility, source, source, move,
                        targetAbility);
                    if (sourceCanBeSet is BoolRelayVar { Value: false })
                    {
                        return false;
                    }

                    // Check if target can receive source's ability
                    var targetCanBeSet = battle.RunEvent(EventId.SetAbility, target, source, move,
                        sourceAbility);
                    if (targetCanBeSet is BoolRelayVar { Value: false })
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    var targetAbility = target.GetAbility();
                    var sourceAbility = source.GetAbility();
                    if (target.IsAlly(source))
                    {
                        battle.Add("-activate", source, "move: Skill Swap", "", "",
                            $"[of] {target}");
                    }
                    else
                    {
                        battle.Add("-activate", source, "move: Skill Swap", targetAbility.Name,
                            sourceAbility.Name, $"[of] {target}");
                    }

                    var oldAbility = source.SetAbility(targetAbility.Id, target);
                    if (oldAbility == null) return false;
                    target.SetAbility(sourceAbility.Id, source);
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.SkitterSmack] = new()
            {
                Id = MoveId.SkitterSmack,
                Num = 806,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Skitter Smack",
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
                    Boosts = new SparseBoostsTable { SpA = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.SkyAttack] = new()
            {
                Id = MoveId.SkyAttack,
                Num = 143,
                Accuracy = 90,
                BasePower = 140,
                Category = MoveCategory.Physical,
                Name = "Sky Attack",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Charge = true,
                    Protect = true,
                    Mirror = true,
                    Distance = true,
                    Metronome = true,
                    NoSleepTalk = true,
                    FailInstruct = true,
                },
                CritRatio = 2,
                OnTryMove = new OnTryMoveEventInfo((battle, source, target, move) =>
                {
                    // If we have the volatile from turn 1, remove it and continue with the attack
                    if (source.RemoveVolatile(battle.Library.Conditions[ConditionId.TwoTurnMove]))
                    {
                        return new VoidReturn(); // Continue with the attack
                    }

                    // Turn 1: Prepare the move
                    battle.Add("-prepare", source, move.Name);
                    var chargeResult =
                        battle.RunEvent(EventId.ChargeMove, source, target, move);
                    if (chargeResult is BoolRelayVar { Value: false })
                    {
                        return new VoidReturn(); // ChargeMove failed - continue but don't set up
                    }

                    source.AddVolatile(ConditionId.TwoTurnMove, target);
                    return null; // Stop the move on turn 1
                }),
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.SlackOff] = new()
            {
                Id = MoveId.SlackOff,
                Num = 303,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Slack Off",
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
            [MoveId.Slam] = new()
            {
                Id = MoveId.Slam,
                Num = 21,
                Accuracy = 75,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Slam",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    NonSky = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Slash] = new()
            {
                Id = MoveId.Slash,
                Num = 163,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Slash",
                BasePp = 20,
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
                Type = MoveType.Normal,
            },
            [MoveId.SleepPowder] = new()
            {
                Id = MoveId.SleepPowder,
                Num = 79,
                Accuracy = 75,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Sleep Powder",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                    Powder = true,
                },
                Status = ConditionId.Sleep,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.SleepTalk] = new()
            {
                Id = MoveId.SleepTalk,
                Num = 214,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Sleep Talk",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    FailEncore = true,
                    NoSleepTalk = true,
                    NoAssist = true,
                    FailCopycat = true,
                    FailMimic = true,
                    FailInstruct = true,
                },
                SleepUsable = true,
                OnTry = new OnTryEventInfo((_, source, _, _) =>
                    source.Status == ConditionId.Sleep ||
                    source.HasAbility(AbilityId.Comatose)),
                OnHit = new OnHitEventInfo((battle, _, source, _) =>
                {
                    var moves = new List<MoveId>();
                    foreach (var moveSlot in source.MoveSlots)
                    {
                        var moveid = moveSlot.Id;
                        if (moveid == MoveId.None) continue;
                        var move = _library.Moves[moveid];
                        if (move.Flags.NoSleepTalk == true || move.Flags.Charge == true)
                        {
                            continue;
                        }

                        moves.Add(moveid);
                    }

                    if (moves.Count == 0) return false;
                    var randomMove = battle.Sample(moves);
                    battle.Actions.UseMove(randomMove, source);
                    return new VoidReturn();
                }),
                CallsMove = true,
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Sludge] = new()
            {
                Id = MoveId.Sludge,
                Num = 124,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Special,
                Name = "Sludge",
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
                    Chance = 30,
                    Status = ConditionId.Poison,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.SludgeBomb] = new()
            {
                Id = MoveId.SludgeBomb,
                Num = 188,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Sludge Bomb",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bullet = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Poison,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.SludgeWave] = new()
            {
                Id = MoveId.SludgeWave,
                Num = 482,
                Accuracy = 100,
                BasePower = 95,
                Category = MoveCategory.Special,
                Name = "Sludge Wave",
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
                    Status = ConditionId.Poison,
                },
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Poison,
            },
            [MoveId.SmackDown] = new()
            {
                Id = MoveId.SmackDown,
                Num = 479,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Smack Down",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    NonSky = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.SmackDown,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.SmartStrike] = new()
            {
                Id = MoveId.SmartStrike,
                Num = 684,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Smart Strike",
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
                Type = MoveType.Steel,
            },
            [MoveId.Smog] = new()
            {
                Id = MoveId.Smog,
                Num = 123,
                Accuracy = 70,
                BasePower = 30,
                Category = MoveCategory.Special,
                Name = "Smog",
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
                    Chance = 40,
                    Status = ConditionId.Poison,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.Smokescreen] = new()
            {
                Id = MoveId.Smokescreen,
                Num = 108,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Smokescreen",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Accuracy = -1 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Snarl] = new()
            {
                Id = MoveId.Snarl,
                Num = 555,
                Accuracy = 95,
                BasePower = 55,
                Category = MoveCategory.Special,
                Name = "Snarl",
                BasePp = 15,
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
                    Chance = 100,
                    Boosts = new SparseBoostsTable { SpA = -1 },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Dark,
            },
            [MoveId.SnipeShot] = new()
            {
                Id = MoveId.SnipeShot,
                Num = 745,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Snipe Shot",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                CritRatio = 2,
                TracksTarget = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.Snore] = new()
            {
                Id = MoveId.Snore,
                Num = 173,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Snore",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Sound = true,
                    BypassSub = true,
                },
                SleepUsable = true,
                OnTry = new OnTryEventInfo((_, source, _, _) =>
                    source.Status == ConditionId.Sleep ||
                    source.HasAbility(AbilityId.Comatose)),
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Snowscape] = new()
            {
                Id = MoveId.Snowscape,
                Num = 883,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Snowscape",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags(),
                Weather = ConditionId.Snowscape,
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Ice,
            },
            [MoveId.Soak] = new()
            {
                Id = MoveId.Soak,
                Num = 487,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Soak",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                OnHit = new OnHitEventInfo((battle, target, _, _) =>
                {
                    // Check if already pure Water type or if SetType fails
                    var types = target.GetTypes();
                    var isAlreadyPureWater = types is [PokemonType.Water];
                    if (isAlreadyPureWater || !target.SetType(PokemonType.Water))
                    {
                        // Soak should animate even when it fails.
                        // Returning false would suppress the animation.
                        if (battle.DisplayUi)
                        {
                            battle.Add("-fail", target);
                        }

                        return new Empty(); // null equivalent - move worked but failed silently
                    }

                    if (battle.DisplayUi)
                    {
                        battle.Add("-start", target, "typechange", "Water");
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.SoftBoiled] = new()
            {
                Id = MoveId.SoftBoiled,
                Num = 135,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Soft-Boiled",
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
            [MoveId.SolarBeam] = new()
            {
                Id = MoveId.SolarBeam,
                Num = 76,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Solar Beam",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Charge = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    NoSleepTalk = true,
                    FailInstruct = true,
                },
                OnTryMove = new OnTryMoveEventInfo((battle, source, target, move) =>
                {
                    // If we have the volatile from turn 1, remove it and continue with the attack
                    if (source.RemoveVolatile(battle.Library.Conditions[ConditionId.TwoTurnMove]))
                    {
                        return new VoidReturn(); // Continue with the attack
                    }

                    // Turn 1: Prepare the move
                    battle.Add("-prepare", source, move.Name);
                    // In sun, skip charge turn
                    var weather = battle.Field.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        battle.AttrLastMove("[still]");
                        battle.AddMove("-anim", StringNumberDelegateObjectUnion.FromObject(source),
                            move.Name,
                            StringNumberDelegateObjectUnion.FromObject(target));
                        return new VoidReturn();
                    }

                    var chargeResult =
                        battle.RunEvent(EventId.ChargeMove, source, target, move);
                    if (chargeResult is BoolRelayVar { Value: false })
                    {
                        return false;
                    }

                    source.AddVolatile(ConditionId.TwoTurnMove, target);
                    return null; // Stop the move on turn 1
                }),
                OnBasePower = new OnBasePowerEventInfo((battle, _, _, _, _) =>
                {
                    var weakWeathers = new[]
                    {
                        ConditionId.RainDance, ConditionId.PrimordialSea, ConditionId.Sandstorm,
                        ConditionId.Snowscape,
                    };
                    var weather = battle.Field.EffectiveWeather();
                    if (weakWeathers.Contains(weather))
                    {
                        battle.Debug("weakened by weather");
                        return battle.ChainModify(0.5);
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.SolarBlade] = new()
            {
                Id = MoveId.SolarBlade,
                Num = 669,
                Accuracy = 100,
                BasePower = 125,
                Category = MoveCategory.Physical,
                Name = "Solar Blade",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Charge = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    NoSleepTalk = true,
                    FailInstruct = true,
                    Slicing = true,
                },
                OnTryMove = new OnTryMoveEventInfo((battle, source, target, move) =>
                {
                    // If we have the volatile from turn 1, remove it and continue with the attack
                    if (source.RemoveVolatile(battle.Library.Conditions[ConditionId.TwoTurnMove]))
                    {
                        return new VoidReturn(); // Continue with the attack
                    }

                    // Turn 1: Prepare the move
                    battle.Add("-prepare", source, move.Name);
                    // In sun, skip charge turn
                    var weather = battle.Field.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        battle.AttrLastMove("[still]");
                        battle.AddMove("-anim", StringNumberDelegateObjectUnion.FromObject(source),
                            move.Name,
                            StringNumberDelegateObjectUnion.FromObject(target));
                        return new VoidReturn();
                    }

                    var chargeResult =
                        battle.RunEvent(EventId.ChargeMove, source, target, move);
                    if (chargeResult is BoolRelayVar { Value: false })
                    {
                        return false;
                    }

                    source.AddVolatile(ConditionId.TwoTurnMove, target);
                    return null; // Stop the move on turn 1
                }),
                OnBasePower = new OnBasePowerEventInfo((battle, _, _, _, _) =>
                {
                    var weakWeathers = new[]
                    {
                        ConditionId.RainDance, ConditionId.PrimordialSea, ConditionId.Sandstorm,
                        ConditionId.Snowscape,
                    };
                    var weather = battle.Field.EffectiveWeather();
                    if (weakWeathers.Contains(weather))
                    {
                        battle.Debug("weakened by weather");
                        return battle.ChainModify(0.5);
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.SpacialRend] = new()
            {
                Id = MoveId.SpacialRend,
                Num = 460,
                Accuracy = 95,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Spacial Rend",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                CritRatio = 2,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dragon,
            },
            [MoveId.Spark] = new()
            {
                Id = MoveId.Spark,
                Num = 209,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Physical,
                Name = "Spark",
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
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.SparklingAria] = new()
            {
                Id = MoveId.SparklingAria,
                Num = 664,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Sparkling Aria",
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
                    VolatileStatus = ConditionId.SparklingAria,
                },
                OnAfterMove = new OnAfterMoveEventInfo((battle, source, _, move) =>
                {
                    if (source.Fainted || move.HitTargets == null || move.HasSheerForce == true)
                    {
                        // Make sure the volatiles are cleared
                        foreach (var pokemon in battle.GetAllActive())
                        {
                            pokemon.DeleteVolatile(ConditionId.SparklingAria);
                        }

                        return new VoidReturn();
                    }

                    var numberTargets = move.HitTargets.Count;
                    foreach (var pokemon in move.HitTargets)
                    {
                        // Bypasses Shield Dust when hitting multiple targets
                        if (pokemon != source && pokemon.IsActive &&
                            (pokemon.RemoveVolatile(
                                 _library.Conditions[ConditionId.SparklingAria]) ||
                             numberTargets > 1) &&
                            pokemon.Status == ConditionId.Burn)
                        {
                            pokemon.CureStatus();
                        }
                    }

                    return new VoidReturn();
                }),
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Water,
            },
            [MoveId.SpeedSwap] = new()
            {
                Id = MoveId.SpeedSwap,
                Num = 683,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Speed Swap",
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
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    (target.StoredStats.Spe, source.StoredStats.Spe) = (source.StoredStats.Spe,
                        target.StoredStats.Spe);
                    battle.Add("-activate", source, "move: Speed Swap", $"[of] {target}");
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.SpicyExtract] = new()
            {
                Id = MoveId.SpicyExtract,
                Num = 858,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Spicy Extract",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                },
                Boosts = new SparseBoostsTable { Atk = 2, Def = -2 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.Spikes] = new()
            {
                Id = MoveId.Spikes,
                Num = 191,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Spikes",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Reflectable = true,
                    NonSky = true,
                    Metronome = true,
                    MustPressure = true,
                },
                SideCondition = ConditionId.Spikes,
                Condition = _library.Conditions[ConditionId.Spikes],
                Secondary = null,
                Target = MoveTarget.FoeSide,
                Type = MoveType.Ground,
            },
            [MoveId.SpikyShield] = new()
            {
                Id = MoveId.SpikyShield,
                Num = 596,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Spiky Shield",
                BasePp = 10,
                Priority = 4,
                Flags = new MoveFlags
                {
                    NoAssist = true,
                    FailCopycat = true,
                },
                StallingMove = true,
                VolatileStatus = ConditionId.SpikyShield,
                OnPrepareHit = new OnPrepareHitEventInfo((battle, pokemon, _, _) =>
                    battle.Queue.WillAct() is not null &&
                    battle.RunEvent(EventId.StallMove, pokemon) is not null
                        ? new VoidReturn()
                        : false),
                OnHit = new OnHitEventInfo((_, pokemon, _, _) =>
                {
                    pokemon.AddVolatile(ConditionId.Stall);
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Grass,
            },
            [MoveId.SpinOut] = new()
            {
                Id = MoveId.SpinOut,
                Num = 859,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Spin Out",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { Spe = -2 },
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
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
            [MoveId.SpiritShackle] = new()
            {
                Id = MoveId.SpiritShackle,
                Num = 662,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Spirit Shackle",
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
                    OnHit = (_, target, source, move) =>
                    {
                        if (source.IsActive)
                            target.AddVolatile(ConditionId.Trapped, source, move,
                                ConditionId.Trapper);
                        return new VoidReturn();
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.SpitUp] = new()
            {
                Id = MoveId.SpitUp,
                Num = 255,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Special,
                Name = "Spit Up",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Metronome = true,
                },
                BasePowerCallback = new BasePowerCallbackEventInfo((_, source, _, _) =>
                {
                    if (!source.Volatiles.TryGetValue(ConditionId.StockpileStorage,
                            out var vol)) return 0;
                    return vol.Layers * 100;
                }),
                OnTry = new OnTryEventInfo((_, source, _, _) =>
                    source.Volatiles.ContainsKey(ConditionId.StockpileStorage)),
                OnAfterMove = new OnAfterMoveEventInfo((_, source, _, _) =>
                {
                    source.RemoveVolatile(_library.Conditions[ConditionId.StockpileStorage]);
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Spite] = new()
            {
                Id = MoveId.Spite,
                Num = 180,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Spite",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    BypassSub = true,
                    Metronome = true,
                },
                OnHit = new OnHitEventInfo((battle, target, _, _) =>
                {
                    var move = target.LastMove;
                    if (move == null) return false;
                    var ppDeducted = target.DeductPp(move.Id, 4);
                    if (ppDeducted == 0) return false;
                    battle.Add("-activate", target, "move: Spite", move.Name, ppDeducted);
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.Splash] = new()
            {
                Id = MoveId.Splash,
                Num = 150,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Splash",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Gravity = true,
                    Metronome = true,
                },
                OnTry = new OnTryEventInfo((battle, source, _, move) =>
                {
                    if (battle.Field.GetPseudoWeather(ConditionId.Gravity) is not null)
                    {
                        battle.Add("cant", source, "move: Gravity", move);
                        return null; // Silent failure - message already shown
                    }

                    return true;
                }),
                OnTryHit = new OnTryHitEventInfo((battle, _, _, _) =>
                {
                    battle.Add("-nothing");
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Spore] = new()
            {
                Id = MoveId.Spore,
                Num = 147,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Spore",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                    Powder = true,
                },
                Status = ConditionId.Sleep,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.SpringtideStorm] = new()
            {
                Id = MoveId.SpringtideStorm,
                Num = 831,
                Accuracy = 80,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Springtide Storm",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Wind = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Boosts = new SparseBoostsTable { Atk = -1 },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Fairy,
            },
            [MoveId.StealthRock] = new()
            {
                Id = MoveId.StealthRock,
                Num = 446,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Stealth Rock",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Reflectable = true,
                    Metronome = true,
                    MustPressure = true,
                },
                SideCondition = ConditionId.StealthRock,
                Condition = _library.Conditions[ConditionId.StealthRock],
                Secondary = null,
                Target = MoveTarget.FoeSide,
                Type = MoveType.Rock,
            },
            [MoveId.SteamEruption] = new()
            {
                Id = MoveId.SteamEruption,
                Num = 592,
                Accuracy = 95,
                BasePower = 110,
                Category = MoveCategory.Special,
                Name = "Steam Eruption",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Defrost = true,
                },
                ThawsTarget = true,
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.SteelBeam] = new()
            {
                Id = MoveId.SteelBeam,
                Num = 796,
                Accuracy = 95,
                BasePower = 140,
                Category = MoveCategory.Special,
                Name = "Steel Beam",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                MindBlownRecoil = true,
                OnAfterMove = new OnAfterMoveEventInfo((battle, source, _, move) =>
                {
                    if (move.MindBlownRecoil == true && source.Hp > 0 &&
                        move.MultiHit is null)
                    {
                        var hpBeforeRecoil = source.Hp;
                        battle.Damage(source.MaxHp / 2, source, source,
                            BattleDamageEffect.FromIEffect(move),
                            true);
                        if (source.Hp <= source.MaxHp / 2 && hpBeforeRecoil > source.MaxHp / 2)
                        {
                            battle.RunEvent(EventId.EmergencyExit, source);
                        }
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.SteelRoller] = new()
            {
                Id = MoveId.SteelRoller,
                Num = 798,
                Accuracy = 100,
                BasePower = 130,
                Category = MoveCategory.Physical,
                Name = "Steel Roller",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnTry = new OnTryEventInfo((battle, source, _, _) =>
                {
                    if (battle.Field.Terrain == ConditionId.None)
                    {
                        battle.Add("-fail", source, "move: Steel Roller");
                        return false;
                    }

                    return true;
                }),
                OnHit = new OnHitEventInfo((battle, _, _, _) =>
                {
                    battle.Field.ClearTerrain();
                    return new VoidReturn();
                }),
                OnAfterSubDamage = new OnAfterSubDamageEventInfo((battle, _, _, _, _) =>
                {
                    battle.Field.ClearTerrain();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.SteelWing] = new()
            {
                Id = MoveId.SteelWing,
                Num = 211,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Steel Wing",
                BasePp = 25,
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
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { Def = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.StickyWeb] = new()
            {
                Id = MoveId.StickyWeb,
                Num = 564,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Sticky Web",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Reflectable = true,
                    Metronome = true,
                },
                SideCondition = ConditionId.StickyWeb,
                Condition = _library.Conditions[ConditionId.StickyWeb],
                Secondary = null,
                Target = MoveTarget.FoeSide,
                Type = MoveType.Bug,
            },
            [MoveId.Stockpile] = new()
            {
                Id = MoveId.Stockpile,
                Num = 254,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Stockpile",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                OnTry = new OnTryEventInfo((_, source, _, _) =>
                {
                    if (source.Volatiles.TryGetValue(ConditionId.StockpileStorage,
                            out var vol) && vol.Layers >= 3) return false;
                    return true;
                }),
                VolatileStatus = ConditionId.StockpileStorage,
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Stomp] = new()
            {
                Id = MoveId.Stomp,
                Num = 23,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Physical,
                Name = "Stomp",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    NonSky = true,
                    Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.StompingTantrum] = new()
            {
                Id = MoveId.StompingTantrum,
                Num = 707,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Stomping Tantrum",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, source, _, move) =>
                {
                    if (source.MoveLastTurnResult?.IsTruthy() == false)
                    {
                        battle.Debug("doubling Stomping Tantrum BP due to previous move failure");
                        return move.BasePower * 2;
                    }

                    return move.BasePower;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
            },
            [MoveId.StoneAxe] = new()
            {
                Id = MoveId.StoneAxe,
                Num = 830,
                Accuracy = 90,
                BasePower = 65,
                Category = MoveCategory.Physical,
                Name = "Stone Axe",
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
                OnAfterHit = new OnAfterHitEventInfo((_, _, source, move) =>
                {
                    if (move.HasSheerForce != true && source.Hp > 0)
                    {
                        foreach (var side in source.Side.FoeSidesWithConditions())
                        {
                            side.AddSideCondition(ConditionId.StealthRock);
                        }
                    }

                    return new VoidReturn();
                }),
                OnAfterSubDamage =
                    new OnAfterSubDamageEventInfo((_, _, _, source, move) =>
                    {
                        if (move.HasSheerForce != true && source.Hp > 0)
                        {
                            foreach (var side in source.Side.FoeSidesWithConditions())
                            {
                                side.AddSideCondition(ConditionId.StealthRock);
                            }
                        }
                    }),
                Secondary = new SecondaryEffect(), // For Sheer Force
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.StoneEdge] = new()
            {
                Id = MoveId.StoneEdge,
                Num = 444,
                Accuracy = 80,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Stone Edge",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                CritRatio = 2,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.StoredPower] = new()
            {
                Id = MoveId.StoredPower,
                Num = 500,
                Accuracy = 100,
                BasePower = 20,
                Category = MoveCategory.Special,
                Name = "Stored Power",
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
                    // Count all positive stat boosts
                    var positiveBoosts = 0;
                    foreach (var (_, value) in source.Boosts.GetBoosts())
                    {
                        if (value > 0) positiveBoosts += value;
                    }

                    var storedPower = _library.Moves[MoveId.StoredPower];
                    var bp = storedPower.BasePower + 20 * positiveBoosts;
                    battle.Debug($"[Stored Power] BP: {bp}");
                    return bp;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.StrangeSteam] = new()
            {
                Id = MoveId.StrangeSteam,
                Num = 790,
                Accuracy = 95,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Strange Steam",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    VolatileStatus = ConditionId.Confusion,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.Strength] = new()
            {
                Id = MoveId.Strength,
                Num = 70,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Strength",
                BasePp = 15,
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
            [MoveId.StrengthSap] = new()
            {
                Id = MoveId.StrengthSap,
                Num = 668,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Strength Sap",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Heal = true,
                    Metronome = true,
                },
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    // Fail if target's attack is already at minimum
                    if (target.Boosts.GetBoost(BoostId.Atk) == -6) return false;

                    // Get target's attack stat (with boosts applied) BEFORE lowering
                    var targetAtk = target.GetStat(StatIdExceptHp.Atk, false, true);

                    // Lower target's attack by 1
                    var boostResult = battle.Boost(new SparseBoostsTable { Atk = -1 },
                        target, source, null, false, true);

                    // Heal source by target's (pre-boost) attack stat
                    // Pass the move as effect so LiquidOoze can detect it
                    var healResult = battle.Heal(targetAtk, source, target,
                        BattleHealEffect.FromIEffect(move));

                    // Move succeeds if either heal or boost worked
                    var success = healResult is not FalseIntFalseUnion ||
                                  boostResult?.IsTruthy() == true;
                    return success ? new VoidReturn() : false;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.StringShot] = new()
            {
                Id = MoveId.StringShot,
                Num = 81,
                Accuracy = 95,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "String Shot",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Spe = -2 },
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Bug,
            },
            [MoveId.Struggle] = new()
            {
                Id = MoveId.Struggle,
                Num = 165,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Struggle",
                BasePp = 1,
                NoPpBoosts = true,
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
                    move.Type = MoveType.Unknown; // Typeless damage (???)
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
            [MoveId.StuffCheeks] = new()
            {
                Id = MoveId.StuffCheeks,
                Num = 747,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Stuff Cheeks",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                OnDisableMove = new OnDisableMoveEventInfo((_, pokemon) =>
                {
                    var item = pokemon.GetItem();
                    if (!item.IsBerry) pokemon.DisableMove(MoveId.StuffCheeks);
                }),
                OnTry = new OnTryEventInfo((_, source, _, _) =>
                {
                    var item = source.GetItem();
                    return item.IsBerry;
                }),
                OnHit = new OnHitEventInfo((battle, _, source, _) =>
                {
                    // Boost first, then eat item
                    var boostResult =
                        battle.Boost(new SparseBoostsTable { Def = 2 }, source);
                    if (boostResult?.IsTruthy() != true) return null;
                    source.EatItem(true);
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.StunSpore] = new()
            {
                Id = MoveId.StunSpore,
                Num = 78,
                Accuracy = 75,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Stun Spore",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                    Powder = true,
                },
                Status = ConditionId.Paralysis,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.Substitute] = new()
            {
                Id = MoveId.Substitute,
                Num = 164,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Substitute",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    NonSky = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.Substitute,
                OnTryHit = new OnTryHitEventInfo((battle, source, _, _) =>
                {
                    if (source.Volatiles.ContainsKey(ConditionId.Substitute))
                    {
                        battle.Add("-fail", source, "move: Substitute");
                        return new Empty(); // NOT_FAIL - message already shown
                    }

                    if (source.Hp <= source.MaxHp / 4 || source.MaxHp == 1)
                    {
                        battle.Add("-fail", source, "move: Substitute", "[weak]");
                        return new Empty(); // NOT_FAIL - message already shown
                    }

                    return new VoidReturn();
                }),
                OnHit = new OnHitEventInfo((battle, target, _, _) =>
                {
                    battle.DirectDamage(target.MaxHp / 4);
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.SuckerPunch] = new()
            {
                Id = MoveId.SuckerPunch,
                Num = 389,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Sucker Punch",
                BasePp = 5,
                Priority = 1,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnTry = new OnTryEventInfo((battle, _, target, _) =>
                {
                    var action = battle.Queue.WillMove(target);
                    var move = action?.Choice == ActionId.Move ? action.Move : null;
                    if (move == null ||
                        move.Category == MoveCategory.Status ||
                        target.Volatiles.ContainsKey(ConditionId.MustRecharge))
                    {
                        return false;
                    }

                    return true;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.SunnyDay] = new()
            {
                Id = MoveId.SunnyDay,
                Num = 241,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Sunny Day",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Metronome = true,
                },
                Weather = ConditionId.SunnyDay,
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Fire,
            },
            [MoveId.SunsteelStrike] = new()
            {
                Id = MoveId.SunsteelStrike,
                Num = 713,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Sunsteel Strike",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                },
                IgnoreAbility = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.SupercellSlam] = new()
            {
                Id = MoveId.SupercellSlam,
                Num = 916,
                Accuracy = 95,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Supercell Slam",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                HasCrashDamage = true,
                OnMoveFail = new OnMoveFailEventInfo((battle, _, source, _) =>
                {
                    battle.Damage(source.BaseMaxHp / 2, source, source,
                        BattleDamageEffect.FromIEffect(
                            _library.Conditions[ConditionId.SupercellSlam]));
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.SuperFang] = new()
            {
                Id = MoveId.SuperFang,
                Num = 162,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Super Fang",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                DamageCallback = new DamageCallbackEventInfo((battle, _, target, _) =>
                    battle.ClampIntRange(target.GetUndynamaxedHp() / 2, 1, int.MaxValue)),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Superpower] = new()
            {
                Id = MoveId.Superpower,
                Num = 276,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Superpower",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { Atk = -1, Def = -1 },
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.Supersonic] = new()
            {
                Id = MoveId.Supersonic,
                Num = 48,
                Accuracy = 55,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Supersonic",
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
                VolatileStatus = ConditionId.Confusion,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Surf] = new()
            {
                Id = MoveId.Surf,
                Num = 57,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Surf",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    NonSky = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Water,
            },
            [MoveId.SurgingStrikes] = new()
            {
                Id = MoveId.SurgingStrikes,
                Num = 818,
                Accuracy = 100,
                BasePower = 25,
                Category = MoveCategory.Physical,
                Name = "Surging Strikes",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Punch = true,
                },
                WillCrit = true,
                MultiHit = 3,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.Swagger] = new()
            {
                Id = MoveId.Swagger,
                Num = 207,
                Accuracy = 85,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Swagger",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.Confusion,
                Boosts = new SparseBoostsTable { Atk = 2 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Swallow] = new()
            {
                Id = MoveId.Swallow,
                Num = 256,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Swallow",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Heal = true,
                    Metronome = true,
                },
                OnTry = new OnTryEventInfo((_, source, _, _) =>
                    source.Volatiles.ContainsKey(ConditionId.StockpileStorage)),
                OnHit = new OnHitEventInfo((battle, target, _, _) =>
                {
                    var layers =
                        target.Volatiles.TryGetValue(ConditionId.StockpileStorage,
                            out var vol)
                            ? vol.Layers ?? 1
                            : 1;
                    double[] healAmount = [0.25, 0.5, 1.0];
                    var success =
                        battle.Heal(battle.Modify(target.MaxHp, healAmount[layers - 1])) is not
                            FalseIntFalseUnion;
                    if (!success) battle.Add("-fail", target, "heal");
                    target.RemoveVolatile(_library.Conditions[ConditionId.StockpileStorage]);
                    // Always return success or NOT_FAIL (Empty) - stockpile is consumed regardless
                    return success ? new VoidReturn() : new Empty();
                }),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.SweetKiss] = new()
            {
                Id = MoveId.SweetKiss,
                Num = 186,
                Accuracy = 75,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Sweet Kiss",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.Confusion,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.SweetScent] = new()
            {
                Id = MoveId.SweetScent,
                Num = 230,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Sweet Scent",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Evasion = -2 },
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Normal,
            },
            [MoveId.Swift] = new()
            {
                Id = MoveId.Swift,
                Num = 129,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Swift",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Normal,
            },
            [MoveId.Switcheroo] = new()
            {
                Id = MoveId.Switcheroo,
                Num = 415,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Switcheroo",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    AllyAnim = true,
                    NoAssist = true,
                    FailCopycat = true,
                },
                OnTryImmunity = new OnTryImmunityEventInfo((_, target, _, _) =>
                    !target.HasAbility(AbilityId.StickyHold)),
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    var yourItem = target.TakeItem(source);
                    var myItem = source.TakeItem();
                    if (target.Item != ItemId.None || source.Item != ItemId.None ||
                        (yourItem == null && myItem == null))
                    {
                        if (yourItem is ItemItemFalseUnion yourItemVal)
                            target.Item = yourItemVal.Item.Id;
                        if (myItem is ItemItemFalseUnion myItemVal) source.Item = myItemVal.Item.Id;
                        return false;
                    }

                    // Check if receiving Pokemon can take the item (TakeItem event)
                    var myItemFailed = myItem is ItemItemFalseUnion myItemCheck &&
                                       battle.SingleEvent(EventId.TakeItem, myItemCheck.Item, source.ItemState,
                                               new PokemonSingleEventTarget(target), source, move, myItemCheck.Item)
                                           is BoolRelayVar { Value: false };
                    var yourItemFailed = yourItem is ItemItemFalseUnion yourItemCheck &&
                                         battle.SingleEvent(EventId.TakeItem, yourItemCheck.Item, target.ItemState,
                                                 new PokemonSingleEventTarget(source), target, move,
                                                 yourItemCheck.Item)
                                             is BoolRelayVar { Value: false };

                    if (myItemFailed || yourItemFailed)
                    {
                        if (yourItem is ItemItemFalseUnion yourItemVal2)
                            target.Item = yourItemVal2.Item.Id;
                        if (myItem is ItemItemFalseUnion myItemVal2)
                            source.Item = myItemVal2.Item.Id;
                        return false;
                    }

                    battle.Add("-activate", source, "move: Trick", $"[of] {target}");
                    if (myItem is ItemItemFalseUnion myItemUnion)
                    {
                        target.SetItem(myItemUnion.Item.Id);
                        battle.Add("-item", target, myItemUnion.Item.Name,
                            "[from] move: Switcheroo");
                    }
                    else if (yourItem is ItemItemFalseUnion yourItemForEnditem)
                    {
                        battle.Add("-enditem", target, yourItemForEnditem.Item.Name, "[silent]",
                            "[from] move: Switcheroo");
                    }

                    if (yourItem is ItemItemFalseUnion yourItemUnion)
                    {
                        source.SetItem(yourItemUnion.Item.Id);
                        battle.Add("-item", source, yourItemUnion.Item.Name,
                            "[from] move: Switcheroo");
                    }
                    else if (myItem is ItemItemFalseUnion myItemForEnditem)
                    {
                        battle.Add("-enditem", source, myItemForEnditem.Item.Name, "[silent]",
                            "[from] move: Switcheroo");
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.SwordsDance] = new()
            {
                Id = MoveId.SwordsDance,
                Num = 14,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Swords Dance",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Dance = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Atk = 2 },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Synthesis] = new()
            {
                Id = MoveId.Synthesis,
                Num = 235,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Synthesis",
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
                    var factor = 0.5;
                    var weather = battle.Field.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                        factor = 0.667;
                    else if (weather is ConditionId.RainDance or ConditionId.PrimordialSea
                             or ConditionId.Sandstorm or ConditionId.Snowscape)
                        factor = 0.25;

                    var success =
                        battle.Heal(battle.Modify(target.MaxHp, factor)) is not FalseIntFalseUnion;
                    if (!success)
                    {
                        battle.Add("-fail", target, "heal");
                        return false; // NOT_FAIL
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Grass,
            },
            [MoveId.SyrupBomb] = new()
            {
                Id = MoveId.SyrupBomb,
                Num = 903,
                Accuracy = 85,
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Syrup Bomb",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Bullet = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    VolatileStatus = ConditionId.SyrupBomb,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },

            // ===== T MOVES =====

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
            [MoveId.TachyonCutter] = new()
            {
                Id = MoveId.TachyonCutter,
                Num = 911,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Tachyon Cutter",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Slicing = true,
                },
                MultiHit = 2,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.Tackle] = new()
            {
                Id = MoveId.Tackle,
                Num = 33,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Tackle",
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
            [MoveId.TailGlow] = new()
            {
                Id = MoveId.TailGlow,
                Num = 294,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Tail Glow",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { SpA = 3 },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Bug,
            },
            [MoveId.TailSlap] = new()
            {
                Id = MoveId.TailSlap,
                Num = 541,
                Accuracy = 85,
                BasePower = 25,
                Category = MoveCategory.Physical,
                Name = "Tail Slap",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                MultiHit = new[] { 2, 5 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.TailWhip] = new()
            {
                Id = MoveId.TailWhip,
                Num = 39,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Tail Whip",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Def = -1 },
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Normal,
            },
            [MoveId.TakeDown] = new()
            {
                Id = MoveId.TakeDown,
                Num = 36,
                Accuracy = 85,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Take Down",
                BasePp = 20,
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
                Type = MoveType.Normal,
            },
            [MoveId.TakeHeart] = new()
            {
                Id = MoveId.TakeHeart,
                Num = 850,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Take Heart",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                OnHit = new OnHitEventInfo((battle, _, source, _) =>
                {
                    // TypeScript: const success = !!this.boost({ spa: 1, spd: 1 });
                    // return pokemon.cureStatus() || success;
                    var boostSuccess = battle.Boost(
                        new SparseBoostsTable { SpA = 1, SpD = 1 }, source, source, null, false, true);
                    var cured = source.CureStatus();
                    // Return true if either boost or cureStatus succeeded
                    return (boostSuccess?.IsTruthy() ?? false) || cured
                        ? new VoidReturn()
                        : false;
                }),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.TarShot] = new()
            {
                Id = MoveId.TarShot,
                Num = 749,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Tar Shot",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.TarShot,
                Boosts = new SparseBoostsTable { Spe = -1 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.Taunt] = new()
            {
                Id = MoveId.Taunt,
                Num = 269,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Taunt",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    BypassSub = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.Taunt,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.TearfulLook] = new()
            {
                Id = MoveId.TearfulLook,
                Num = 715,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Tearful Look",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Atk = -1, SpA = -1 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Teatime] = new()
            {
                Id = MoveId.Teatime,
                Num = 752,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Teatime",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    BypassSub = true,
                    Metronome = true,
                },
                OnHitField = new OnHitFieldEventInfo((battle, _, source, move) =>
                {
                    var targets = new List<Pokemon>();

                    // Check each active Pokemon for invulnerability, TryHit events, and if they have a berry
                    foreach (var pokemon in battle.GetAllActive())
                    {
                        // Check invulnerability
                        var invulnResult = battle.RunEvent(EventId.Invulnerability, pokemon,
                            source, move);
                        if (invulnResult is BoolRelayVar { Value: false })
                        {
                            if (battle.DisplayUi)
                            {
                                battle.Add("-miss", source, pokemon);
                            }

                            continue;
                        }

                        // Check TryHit event and if Pokemon has a berry
                        var tryHitResult =
                            battle.RunEvent(EventId.TryHit, pokemon, source, move);
                        var item = pokemon.GetItem();
                        if (tryHitResult is not null && item.IsBerry)
                        {
                            targets.Add(pokemon);
                        }
                    }

                    // Always show field activate message
                    if (battle.DisplayUi)
                    {
                        battle.Add("-fieldactivate", "move: Teatime");
                    }

                    // If no valid targets, fail the move
                    if (targets.Count == 0)
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-fail", source, "move: Teatime");
                            battle.AttrLastMove("[still]");
                        }

                        // Return NOT_FAIL equivalent - move animation played but had no effect
                        return new Empty();
                    }

                    // Eat the berry for all valid targets
                    foreach (var pokemon in targets)
                    {
                        pokemon.EatItem(true);
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Normal,
            },
            [MoveId.TeeterDance] = new()
            {
                Id = MoveId.TeeterDance,
                Num = 298,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Teeter Dance",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Dance = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.Confusion,
                Secondary = null,
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Normal,
            },
            [MoveId.Teleport] = new()
            {
                Id = MoveId.Teleport,
                Num = 100,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Teleport",
                BasePp = 20,
                Priority = -6,
                Flags = new MoveFlags
                {
                    Metronome = true,
                },
                // TypeScript: return !!this.canSwitch(source.side);
                // Fail if there are no Pokemon to switch to
                OnTry = new OnTryEventInfo((battle, source, _, _) => battle.CanSwitch(source.Side) > 0),
                SelfSwitch = true,
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.TemperFlare] = new()
            {
                Id = MoveId.TemperFlare,
                Num = 915,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Temper Flare",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, source, _, move) =>
                {
                    if (source.MoveLastTurnResult?.IsTruthy() == false)
                    {
                        battle.Debug("doubling Temper Flare BP due to previous move failure");
                        return move.BasePower * 2;
                    }

                    return move.BasePower;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.TeraBlast] = new()
            {
                Id = MoveId.TeraBlast,
                Num = 851,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Tera Blast",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    MustPressure = true,
                },
                BasePowerCallback = new BasePowerCallbackEventInfo((_, source, _, move) =>
                {
                    if (source.Terastallized == MoveType.Stellar)
                    {
                        return 100;
                    }

                    return move.BasePower;
                }),
                OnPrepareHit = new OnPrepareHitEventInfo((battle, source, _, _) =>
                {
                    if (source.Terastallized != null)
                    {
                        battle.AttrLastMove("[anim] Tera Blast " + source.TeraType);
                    }

                    return new VoidReturn();
                }),
                OnModifyType = new OnModifyTypeEventInfo((_, move, source, _) =>
                {
                    if (source.Terastallized != null)
                    {
                        move.Type = source.TeraType;
                    }
                }),
                OnModifyMove = new OnModifyMoveEventInfo((_, move, source, _) =>
                {
                    if (source.Terastallized != null &&
                        source.GetStat(StatIdExceptHp.Atk, false, true) >
                        source.GetStat(StatIdExceptHp.SpA, false, true))
                    {
                        move.Category = MoveCategory.Physical;
                    }

                    if (source.Terastallized == MoveType.Stellar)
                    {
                        move.Self = new SecondaryEffect
                            { Boosts = new SparseBoostsTable { Atk = -1, SpA = -1 } };
                    }
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.TeraStarStorm] = new()
            {
                Id = MoveId.TeraStarStorm,
                Num = 906,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Tera Starstorm",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    NoAssist = true,
                    FailCopycat = true,
                    FailMimic = true,
                    NoSketch = true,
                },
                OnModifyType = new OnModifyTypeEventInfo((_, move, source, _) =>
                {
                    if (source.Species.Name == "Terapagos-Stellar")
                    {
                        move.Type = MoveType.Stellar;
                        if (source.Terastallized != null &&
                            source.GetStat(StatIdExceptHp.Atk, false, true) >
                            source.GetStat(StatIdExceptHp.SpA, false, true))
                        {
                            move.Category = MoveCategory.Physical;
                        }
                    }
                }),
                OnModifyMove = new OnModifyMoveEventInfo((_, move, source, _) =>
                {
                    if (source.Species.Name == "Terapagos-Stellar")
                    {
                        move.Target = MoveTarget.AllAdjacentFoes;
                    }
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.TerrainPulse] = new()
            {
                Id = MoveId.TerrainPulse,
                Num = 805,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Terrain Pulse",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Pulse = true,
                },
                OnModifyType = new OnModifyTypeEventInfo((battle, move, source, _) =>
                {
                    // Only change type if the user is grounded
                    if (source.IsGrounded() != true) return;

                    if (battle.Field.IsTerrain(ConditionId.ElectricTerrain, source))
                        move.Type = MoveType.Electric;
                    else if (battle.Field.IsTerrain(ConditionId.GrassyTerrain, source))
                        move.Type = MoveType.Grass;
                    else if (battle.Field.IsTerrain(ConditionId.MistyTerrain, source))
                        move.Type = MoveType.Fairy;
                    else if (battle.Field.IsTerrain(ConditionId.PsychicTerrain, source))
                        move.Type = MoveType.Psychic;
                }),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, source, _) =>
                {
                    if (battle.Field.Terrain != ConditionId.None && source.IsGrounded() == true)
                    {
                        move.BasePower *= 2;
                        battle.Debug("BP doubled in Terrain");
                    }
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Thief] = new()
            {
                Id = MoveId.Thief,
                Num = 168,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Thief",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    FailMeFirst = true,
                    NoAssist = true,
                    FailCopycat = true,
                },
                OnAfterHit = new OnAfterHitEventInfo((battle, target, source, move) =>
                {
                    if (source.Item != ItemId.None || source.Volatiles.ContainsKey(ConditionId.Gem))
                    {
                        return new VoidReturn();
                    }

                    var yourItem = target.TakeItem(source);
                    if (yourItem is not ItemItemFalseUnion yourItemUnion)
                    {
                        return new VoidReturn();
                    }

                    // Check if source can receive the item (TakeItem event for the receiver)
                    var takeItemResult = battle.SingleEvent(EventId.TakeItem,
                        yourItemUnion.Item, target.ItemState,
                        new PokemonSingleEventTarget(source), target, move, yourItemUnion.Item);
                    if (takeItemResult is BoolRelayVar { Value: false } ||
                        !source.SetItem(yourItemUnion.Item.Id))
                    {
                        // Put item back on target (bypass SetItem to avoid breaking choicelock)
                        target.Item = yourItemUnion.Item.Id;
                        return new VoidReturn();
                    }

                    battle.Add("-enditem", target, yourItemUnion.Item.Name, "[silent]",
                        "[from] move: Thief",
                        $"[of] {source}");
                    battle.Add("-item", source, yourItemUnion.Item.Name, "[from] move: Thief",
                        $"[of] {target}");
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.Thrash] = new()
            {
                Id = MoveId.Thrash,
                Num = 37,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Thrash",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    FailInstruct = true,
                },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.LockedMove,
                },
                Secondary = null,
                Target = MoveTarget.RandomNormal,
                Type = MoveType.Normal,
            },
            [MoveId.ThroatChop] = new()
            {
                Id = MoveId.ThroatChop,
                Num = 675,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Throat Chop",
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
                    Chance = 100,
                    VolatileStatus = ConditionId.ThroatChop,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.Thunder] = new()
            {
                Id = MoveId.Thunder,
                Num = 87,
                Accuracy = 70,
                BasePower = 110,
                Category = MoveCategory.Special,
                Name = "Thunder",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, target) =>
                {
                    if (target == null) return;
                    var weather = target.EffectiveWeather();
                    if (weather is ConditionId.RainDance or ConditionId.PrimordialSea)
                    {
                        move.Accuracy = IntTrueUnion.FromTrue();
                    }
                    else if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        move.Accuracy = 50;
                    }
                }),
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.ThunderBolt] = new()
            {
                Id = MoveId.ThunderBolt,
                Num = 85,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Thunderbolt",
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
                    Chance = 10,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.ThunderCage] = new()
            {
                Id = MoveId.ThunderCage,
                Num = 819,
                Accuracy = 90,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Thunder Cage",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                VolatileStatus = ConditionId.PartiallyTrapped,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.Thunderclap] = new()
            {
                Id = MoveId.Thunderclap,
                Num = 909,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Thunderclap",
                BasePp = 5,
                Priority = 1,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                OnTry = new OnTryEventInfo((battle, source, target, _) =>
                {
                    var action = battle.Queue.WillMove(target);
                    var targetMove = action?.Choice == ActionId.Move ? action.Move : null;
                    if (targetMove == null ||
                        targetMove.Category == MoveCategory.Status ||
                        target.Volatiles.ContainsKey(ConditionId.MustRecharge))
                    {
                        battle.Add("-fail", source);
                        battle.AttrLastMove("[still]");
                        return false;
                    }

                    return true;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.ThunderFang] = new()
            {
                Id = MoveId.ThunderFang,
                Num = 422,
                Accuracy = 95,
                BasePower = 65,
                Category = MoveCategory.Physical,
                Name = "Thunder Fang",
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
                Secondaries =
                [
                    new() { Chance = 10, Status = ConditionId.Paralysis },
                    new() { Chance = 10, VolatileStatus = ConditionId.Flinch },
                ],
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.ThunderousKick] = new()
            {
                Id = MoveId.ThunderousKick,
                Num = 823,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Thunderous Kick",
                BasePp = 10,
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
                    Boosts = new SparseBoostsTable { Def = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.ThunderPunch] = new()
            {
                Id = MoveId.ThunderPunch,
                Num = 9,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Thunder Punch",
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
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.ThunderShock] = new()
            {
                Id = MoveId.ThunderShock,
                Num = 84,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Thunder Shock",
                BasePp = 30,
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
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.Tickle] = new()
            {
                Id = MoveId.Tickle,
                Num = 321,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Tickle",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Atk = -1, Def = -1 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.TidyUp] = new()
            {
                Id = MoveId.TidyUp,
                Num = 882,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Tidy Up",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags(),
                OnHit = new OnHitEventInfo((battle, _, source, _) =>
                {
                    var success = false;
                    foreach (var active in battle.GetAllActive())
                    {
                        if (active.RemoveVolatile(_library.Conditions[ConditionId.Substitute]))
                            success = true;
                    }

                    // Remove hazards from both user's side and foe sides
                    var sideConditions = new[]
                    {
                        ConditionId.Spikes, ConditionId.ToxicSpikes, ConditionId.StealthRock,
                        ConditionId.StickyWeb,
                    };
                    var sides = new List<Side> { source.Side };
                    sides.AddRange(source.Side.FoeSidesWithConditions());

                    foreach (var side in sides)
                    {
                        foreach (var condition in sideConditions)
                        {
                            if (side.RemoveSideCondition(condition))
                            {
                                if (battle.DisplayUi)
                                {
                                    var cond = _library.Conditions[condition];
                                    battle.Add("-sideend", side, cond.Name);
                                }

                                success = true;
                            }
                        }
                    }

                    if (success && battle.DisplayUi)
                    {
                        battle.Add("-activate", source, "move: Tidy Up");
                    }

                    var boosted =
                        battle.Boost(new SparseBoostsTable { Atk = 1, Spe = 1 }, source, source, null, false, true);
                    return (boosted?.IsTruthy() ?? false) || success
                        ? new VoidReturn()
                        : false;
                }),
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.TopsyTurvy] = new()
            {
                Id = MoveId.TopsyTurvy,
                Num = 576,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Topsy-Turvy",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    AllyAnim = true,
                    Metronome = true,
                },
                OnHit = new OnHitEventInfo((battle, target, _, _) =>
                {
                    var success = false;
                    foreach (var (stat, val) in target.Boosts.GetBoosts())
                    {
                        if (val == 0) continue;
                        target.Boosts.SetBoost(stat, -val);
                        success = true;
                    }

                    if (!success) return false;
                    battle.Add("-invertboost", target, "[from] move: Topsy-Turvy");
                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.TorchSong] = new()
            {
                Id = MoveId.TorchSong,
                Num = 871,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Torch Song",
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
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { SpA = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.Torment] = new()
            {
                Id = MoveId.Torment,
                Num = 259,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Torment",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    BypassSub = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.Torment,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.Toxic] = new()
            {
                Id = MoveId.Toxic,
                Num = 92,
                Accuracy = 90,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Toxic",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Status = ConditionId.Toxic,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.ToxicSpikes] = new()
            {
                Id = MoveId.ToxicSpikes,
                Num = 390,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Toxic Spikes",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Reflectable = true,
                    NonSky = true,
                    Metronome = true,
                    MustPressure = true,
                },
                SideCondition = ConditionId.ToxicSpikes,
                Condition = _library.Conditions[ConditionId.ToxicSpikes],
                Secondary = null,
                Target = MoveTarget.FoeSide,
                Type = MoveType.Poison,
            },
            [MoveId.ToxicThread] = new()
            {
                Id = MoveId.ToxicThread,
                Num = 672,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Toxic Thread",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                Status = ConditionId.Poison,
                Boosts = new SparseBoostsTable { Spe = -1 },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.TrailBlaze] = new()
            {
                Id = MoveId.TrailBlaze,
                Num = 885,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Trailblaze",
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
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { Spe = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.Transform] = new()
            {
                Id = MoveId.Transform,
                Num = 144,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Transform",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    AllyAnim = true,
                    FailEncore = true,
                    NoAssist = true,
                    FailCopycat = true,
                    FailMimic = true,
                    FailInstruct = true,
                },
                OnHit = new OnHitEventInfo((_, target, source, _) =>
                {
                    if (!source.TransformInto(target))
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.TriAttack] = new()
            {
                Id = MoveId.TriAttack,
                Num = 161,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Tri Attack",
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
                    Chance = 20,
                    OnHit = (battle, target, source, _) =>
                    {
                        var result = battle.Random(3);
                        if (result == 0)
                        {
                            target.TrySetStatus(ConditionId.Burn, source);
                        }
                        else if (result == 1)
                        {
                            target.TrySetStatus(ConditionId.Paralysis, source);
                        }
                        else
                        {
                            target.TrySetStatus(ConditionId.Freeze, source);
                        }

                        return new VoidReturn();
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Trick] = new()
            {
                Id = MoveId.Trick,
                Num = 271,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Trick",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    AllyAnim = true,
                    NoAssist = true,
                    FailCopycat = true,
                },
                OnTryImmunity = new OnTryImmunityEventInfo((_, target, _, _) =>
                    !target.HasAbility(AbilityId.StickyHold)),
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    var yourItem = target.TakeItem(source);
                    var myItem = source.TakeItem();
                    if (target.Item != ItemId.None || source.Item != ItemId.None ||
                        (yourItem == null && myItem == null))
                    {
                        if (yourItem is ItemItemFalseUnion yourItemVal)
                            target.Item = yourItemVal.Item.Id;
                        if (myItem is ItemItemFalseUnion myItemVal) source.Item = myItemVal.Item.Id;
                        return false;
                    }

                    // Check if receiving Pokemon can take the item (TakeItem event)
                    var myItemFailed = myItem is ItemItemFalseUnion myItemCheck &&
                                       battle.SingleEvent(EventId.TakeItem, myItemCheck.Item, source.ItemState,
                                               new PokemonSingleEventTarget(target), source, move, myItemCheck.Item)
                                           is BoolRelayVar { Value: false };
                    var yourItemFailed = yourItem is ItemItemFalseUnion yourItemCheck &&
                                         battle.SingleEvent(EventId.TakeItem, yourItemCheck.Item, target.ItemState,
                                                 new PokemonSingleEventTarget(source), target, move,
                                                 yourItemCheck.Item)
                                             is BoolRelayVar { Value: false };

                    if (myItemFailed || yourItemFailed)
                    {
                        if (yourItem is ItemItemFalseUnion yourItemVal2)
                            target.Item = yourItemVal2.Item.Id;
                        if (myItem is ItemItemFalseUnion myItemVal2)
                            source.Item = myItemVal2.Item.Id;
                        return false;
                    }

                    battle.Add("-activate", source, "move: Trick", $"[of] {target}");
                    if (myItem is ItemItemFalseUnion myItemUnion)
                    {
                        target.SetItem(myItemUnion.Item.Id);
                        battle.Add("-item", target, myItemUnion.Item.Name, "[from] move: Trick");
                    }
                    else if (yourItem is ItemItemFalseUnion yourItemForEnditem)
                    {
                        battle.Add("-enditem", target, yourItemForEnditem.Item.Name, "[silent]",
                            "[from] move: Trick");
                    }

                    if (yourItem is ItemItemFalseUnion yourItemUnion)
                    {
                        source.SetItem(yourItemUnion.Item.Id);
                        battle.Add("-item", source, yourItemUnion.Item.Name, "[from] move: Trick");
                    }
                    else if (myItem is ItemItemFalseUnion myItemForEnditem)
                    {
                        battle.Add("-enditem", source, myItemForEnditem.Item.Name, "[silent]",
                            "[from] move: Trick");
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.TripleArrows] = new()
            {
                Id = MoveId.TripleArrows,
                Num = 843,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Triple Arrows",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                CritRatio = 2,
                Secondaries =
                [
                    new() { Chance = 50, Boosts = new SparseBoostsTable { Def = -1 } },
                    new() { Chance = 30, VolatileStatus = ConditionId.Flinch },
                ],
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.TripleAxel] = new()
            {
                Id = MoveId.TripleAxel,
                Num = 813,
                Accuracy = 90,
                BasePower = 20,
                Category = MoveCategory.Physical,
                Name = "Triple Axel",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, _, move) =>
                {
                    // BP scales with hit number: 20, 40, 60
                    var bp = 20 * move.Hit;
                    battle.Debug($"[Triple Axel] Hit {move.Hit}, BP: {bp}");
                    return bp;
                }),
                MultiHit = 3,
                MultiAccuracy = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.TripleDive] = new()
            {
                Id = MoveId.TripleDive,
                Num = 865,
                Accuracy = 95,
                BasePower = 30,
                Category = MoveCategory.Physical,
                Name = "Triple Dive",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                MultiHit = 3,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.TripleKick] = new()
            {
                Id = MoveId.TripleKick,
                Num = 167,
                Accuracy = 90,
                BasePower = 10,
                Category = MoveCategory.Physical,
                Name = "Triple Kick",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, _, move) =>
                {
                    // BP scales with hit number: 10, 20, 30
                    var bp = 10 * move.Hit;
                    battle.Debug($"[Triple Kick] Hit {move.Hit}, BP: {bp}");
                    return bp;
                }),
                MultiHit = 3,
                MultiAccuracy = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.TropKick] = new()
            {
                Id = MoveId.TropKick,
                Num = 688,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Trop Kick",
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
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Atk = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.TwinBeam] = new()
            {
                Id = MoveId.TwinBeam,
                Num = 888,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Twin Beam",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                MultiHit = 2,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.Twister] = new()
            {
                Id = MoveId.Twister,
                Num = 239,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Twister",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    Wind = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Dragon,
            },

            // ===== U MOVES =====
        };
    }
}