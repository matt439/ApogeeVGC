using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

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
                // TODO: Boosts accuracy -1
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
                // TODO: onModifyMove - if weather is rain/primordialsea, accuracy becomes true
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
                // TODO: weather = 'Sandstorm'
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
                MultiHit = new int[] { 2, 5 },
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
                // TODO: boosts spe -2
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
                // TODO: boosts def -2
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
                // TODO: onTryMove - charge turn logic
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
                // TODO: onTryHit - check if can switch, check if already has substitute, check if hp > maxhp/4
                // TODO: onHit - direct damage maxhp/2
                // TODO: self.onHit - skip beforeSwitchOut event
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
                // TODO: onPrepareHit - animation
                // TODO: onModifyMove - calculate if physical or special would do more damage
                // TODO: onHit/onAfterSubDamage - hint category
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
                // TODO: boosts def -1, spd -1, atk +2, spa +2, spe +2
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
                // TODO: boosts def +2
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
                // TODO: boosts spe +2, atk +1
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
                // TODO: onHit - heal 1/2, or 2/3 in sandstorm
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
                // TODO: onPrepareHit - stall move check
                // TODO: onHit - add stall volatile
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
                // TODO: onTryHit - check if target can't suppress, already has simple, or has truant
                // TODO: onHit - set ability to simple
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
                // TODO: onHit - copy target's last move permanently
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
                // TODO: onTryHit - check if abilities can be swapped
                // TODO: onHit - swap abilities
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
                // TODO: onTryMove - charge turn logic
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
                // TODO: onTry - check if user is asleep or has Comatose
                // TODO: onHit - use random move from user's moveset
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
                // TODO: boosts accuracy -1
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
                // TODO: onTry - check if user is asleep or has Comatose
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
                // TODO: weather = 'snowscape'
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
                // TODO: onHit - set target's type to Water
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
                // TODO: onTryMove - charge turn logic, skip charge in sun
                // TODO: onBasePower - halve in rain/sandstorm/hail/snow
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
                // TODO: onTryMove - charge turn logic, skip charge in sun
                // TODO: onBasePower - halve in rain/sandstorm/hail/snow
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
                // TODO: onAfterMove - cure burn on hit targets
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
                // TODO: onHit - swap speed stats with target
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
                // TODO: boosts atk +2, def -2
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
                // TODO: onPrepareHit - stall move check
                // TODO: onHit - add stall volatile
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
                // TODO: secondary - add trapped volatile if source is active
                Secondary = null,
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
                // TODO: basePowerCallback - layers * 100
                // TODO: onTry - check if stockpile exists
                // TODO: onAfterMove - remove stockpile volatile
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
                // TODO: onHit - deduct 4 PP from target's last move
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
                // TODO: onTry - check for gravity
                // TODO: onTryHit - add '-nothing'
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
                // TODO: onAfterMove - recoil 1/2 maxhp with emergency exit check
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
                // TODO: onTry - fail if no terrain active
                // TODO: onHit/onAfterSubDamage - clear terrain
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
                // TODO: onTry - check if already at 3 layers
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
                // TODO: basePowerCallback - double if last move failed
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
                // TODO: onAfterHit/onAfterSubDamage - add stealth rock to foe sides if source has HP and not sheer force
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
                // TODO: basePowerCallback - 20 + 20 * positive boosts
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
                // TODO: onHit - heal by target's attack stat, lower target's attack by 1
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
                // TODO: boosts spe -2
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
                // TODO: onDisableMove - disable if no berry
                // TODO: onTry - check if has berry
                // TODO: onHit - boost def +2, eat berry
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
                // TODO: onTryHit - check if already has sub, check if hp > maxhp/4
                // TODO: onHit - direct damage maxhp/4
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
                // TODO: onTry - check if target is using damaging move
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
                // TODO: weather = 'sunnyday'
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
                // TODO: onMoveFail - crash damage 1/2 maxhp
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
                // TODO: damageCallback - target's undynamaxed HP / 2
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
                // TODO: boosts atk +2
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
                // TODO: onTry - check if snatch or has stockpile
                // TODO: onHit - heal based on layers (1=1/4, 2=1/2, 3=1/1), remove stockpile
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
                // TODO: boosts evasion -2
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
                // TODO: onTryImmunity - check sticky hold
                // TODO: onHit - swap items
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
                // TODO: boosts atk +2
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
                // TODO: onHit - heal 1/2 normally, 2/3 in sun, 1/4 in other weather
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
                // TODO: boosts spa +3
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
                MultiHit = new int[] { 2, 5 },
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
                // TODO: boosts def -1
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
                Recoil = new int[] { 1, 4 },
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
                // TODO: onHit - boost {spa: 1, spd: 1}, cure status
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
                // TODO: boosts spe -1
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
                // TODO: boosts atk -1, spa -1
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
                // TODO: onHitField - make all Pokemon eat their berries
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
                // TODO: onTry - check if can switch
                SelfSwitch = MoveSelfSwitch.FromSelfSwitch(),
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
                // TODO: basePowerCallback - doubles if previous move failed
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
                // TODO: basePowerCallback - 100 if Stellar Terastallized
                // TODO: onModifyType - changes type based on Tera type
                // TODO: onModifyMove - changes category if atk > spa when Terastallized, self boosts if Stellar
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
                // TODO: onModifyType - changes type based on terrain
                // TODO: onModifyMove - doubles power on terrain
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
                // TODO: onAfterHit - steal target's item
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
                // TODO: onModifyMove - 100% accuracy in rain, 50% in sun
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
                // TODO: onTry - fails if target isn't using a damaging move
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
                Secondaries = new List<SecondaryEffect>
                {
                    new() { Chance = 10, Status = ConditionId.Paralysis },
                    new() { Chance = 10, VolatileStatus = ConditionId.Flinch },
                },
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
                // TODO: boosts atk -1, def -1
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
                // TODO: onHit - remove hazards and substitutes, boost atk +1, spe +1
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
                // TODO: onHit - invert all stat changes
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
                // TODO: boosts spe -1
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
                // TODO: onHit - transform into target
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
                // TODO: secondary - 20% chance to randomly inflict burn/paralysis/freeze
                Secondary = null,
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
                // TODO: onTryImmunity - check sticky hold
                // TODO: onHit - swap items with target
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
                Secondaries = new List<SecondaryEffect>
                {
                    new() { Chance = 50, Boosts = new SparseBoostsTable { Def = -1 } },
                    new() { Chance = 30, VolatileStatus = ConditionId.Flinch },
                },
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
                // TODO: basePowerCallback - 20 * move.hit
                MultiHit = 3,
                Multiaccuracy = true,
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
                // TODO: basePowerCallback - 10 * move.hit
                MultiHit = 3,
                Multiaccuracy = true,
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
