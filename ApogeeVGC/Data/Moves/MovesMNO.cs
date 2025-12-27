using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesMno()
    {
        return new Dictionary<MoveId, Move>
        {
            [MoveId.MachPunch] = new()
            {
                Id = MoveId.MachPunch,
                Num = 183,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Mach Punch",
                BasePp = 30,
                Priority = 1,
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
                Type = MoveType.Fighting,
            },
            [MoveId.MagicalLeaf] = new()
            {
                Id = MoveId.MagicalLeaf,
                Num = 345,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Magical Leaf",
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
                Type = MoveType.Grass,
            },
            [MoveId.MagicPowder] = new()
            {
                Id = MoveId.MagicPowder,
                Num = 750,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Magic Powder",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Reflectable = true,
                    Mirror = true,
                    AllyAnim = true,
                    Metronome = true,
                    Powder = true,
                },
                // TODO: onHit - set target type to Psychic
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.MagicRoom] = new()
            {
                Id = MoveId.MagicRoom,
                Num = 478,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Magic Room",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Mirror = true,
                    Metronome = true,
                },
                PseudoWeather = ConditionId.MagicRoom,
                Condition = _library.Conditions[ConditionId.MagicRoom],
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Psychic,
            },
            [MoveId.MagmaStorm] = new()
            {
                Id = MoveId.MagmaStorm,
                Num = 463,
                Accuracy = 75,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Magma Storm",
                BasePp = 5,
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
                Type = MoveType.Fire,
            },
            [MoveId.MagneticFlux] = new()
            {
                Id = MoveId.MagneticFlux,
                Num = 602,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Magnetic Flux",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Distance = true,
                    BypassSub = true,
                    Metronome = true,
                },
                // TODO: onHitSide - boost Def and SpD of allies with Plus or Minus abilities
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Electric,
            },
            [MoveId.MagnetRise] = new()
            {
                Id = MoveId.MagnetRise,
                Num = 393,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Magnet Rise",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Gravity = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.MagnetRise,
                Condition = _library.Conditions[ConditionId.MagnetRise],
                // TODO: onTry - check if target has smackdown or ingrain, or if gravity is active
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Electric,
            },
            [MoveId.MakeItRain] = new()
            {
                Id = MoveId.MakeItRain,
                Num = 874,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Make It Rain",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { SpA = -1 },
                },
                Secondary = null,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Steel,
            },
            [MoveId.MalignantChain] = new()
            {
                Id = MoveId.MalignantChain,
                Num = 919,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Malignant Chain",
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
                    Status = ConditionId.Toxic,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.MatchaGotcha] = new()
            {
                Id = MoveId.MatchaGotcha,
                Num = 902,
                Accuracy = 90,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Matcha Gotcha",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Defrost = true,
                    Heal = true,
                    Metronome = true,
                },
                Drain = (1, 2),
                ThawsTarget = true,
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Grass,
            },
            [MoveId.MeanLook] = new()
            {
                Id = MoveId.MeanLook,
                Num = 212,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Mean Look",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Reflectable = true,
                    Mirror = true,
                    Metronome = true,
                },
                // TODO: onHit - add 'trapped' volatile to target
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.MegaDrain] = new()
            {
                Id = MoveId.MegaDrain,
                Num = 72,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Mega Drain",
                BasePp = 15,
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
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.MegaHorn] = new()
            {
                Id = MoveId.MegaHorn,
                Num = 224,
                Accuracy = 85,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Megahorn",
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
                Type = MoveType.Bug,
            },
            [MoveId.MegaKick] = new()
            {
                Id = MoveId.MegaKick,
                Num = 25,
                Accuracy = 75,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Mega Kick",
                BasePp = 5,
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
            [MoveId.MegaPunch] = new()
            {
                Id = MoveId.MegaPunch,
                Num = 5,
                Accuracy = 85,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Mega Punch",
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
                Type = MoveType.Normal,
            },
            [MoveId.Memento] = new()
            {
                Id = MoveId.Memento,
                Num = 262,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Memento",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // TODO: boosts - lower target's Atk and SpA by 2
                SelfDestruct = MoveSelfDestruct.FromIfHit(),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.MetalBurst] = new()
            {
                Id = MoveId.MetalBurst,
                Num = 368,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Metal Burst",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                    FailMeFirst = true,
                },
                // TODO: damageCallback - return last damage taken * 1.5
                // TODO: onTry - check if source was damaged this turn
                // TODO: onModifyTarget - target the pokemon that damaged source
                Secondary = null,
                Target = MoveTarget.Scripted,
                Type = MoveType.Steel,
            },
            [MoveId.MetalClaw] = new()
            {
                Id = MoveId.MetalClaw,
                Num = 232,
                Accuracy = 95,
                BasePower = 50,
                Category = MoveCategory.Physical,
                Name = "Metal Claw",
                BasePp = 35,
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
                        Boosts = new SparseBoostsTable { Atk = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.MetalSound] = new()
            {
                Id = MoveId.MetalSound,
                Num = 319,
                Accuracy = 85,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Metal Sound",
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
                // TODO: boosts - lower target's SpD by 2
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.MeteorBeam] = new()
            {
                Id = MoveId.MeteorBeam,
                Num = 800,
                Accuracy = 90,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Meteor Beam",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Charge = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // TODO: onTryMove - boost SpA by 1 on charge turn, then attack on next turn
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.MeteorMash] = new()
            {
                Id = MoveId.MeteorMash,
                Num = 309,
                Accuracy = 90,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Meteor Mash",
                BasePp = 10,
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
                    Chance = 20,
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { Atk = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.Metronome] = new()
            {
                Id = MoveId.Metronome,
                Num = 118,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Metronome",
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
                // TODO: onHit - use a random move that has metronome flag
                CallsMove = true,
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.MightyCleave] = new()
            {
                Id = MoveId.MightyCleave,
                Num = 910,
                Accuracy = 100,
                BasePower = 95,
                Category = MoveCategory.Physical,
                Name = "Mighty Cleave",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Mirror = true,
                    Metronome = true,
                    Slicing = true,
                },
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.MilkDrink] = new()
            {
                Id = MoveId.MilkDrink,
                Num = 208,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Milk Drink",
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
            [MoveId.Mimic] = new()
            {
                Id = MoveId.Mimic,
                Num = 102,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Mimic",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    BypassSub = true,
                    AllyAnim = true,
                    FailEncore = true,
                    NoSleepTalk = true,
                    NoAssist = true,
                    FailCopycat = true,
                    FailMimic = true,
                    FailInstruct = true,
                },
                // TODO: onHit - replace Mimic move slot with target's last move
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Minimize] = new()
            {
                Id = MoveId.Minimize,
                Num = 107,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Minimize",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.Minimize,
                Condition = _library.Conditions[ConditionId.Minimize],
                // TODO: boosts - raise evasion by 2
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.MirrorCoat] = new()
            {
                Id = MoveId.MirrorCoat,
                Num = 243,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Special,
                Name = "Mirror Coat",
                BasePp = 20,
                Priority = -5,
                Flags = new MoveFlags
                {
                    Protect = true,
                    FailMeFirst = true,
                    NoAssist = true,
                },
                // TODO: damageCallback - return double the special damage received this turn
                // TODO: beforeTurnCallback - add mirrorcoat volatile
                // TODO: onTry - fail if no special damage was taken
                // TODO: condition - track special damage and redirect target
                Secondary = null,
                Target = MoveTarget.Scripted,
                Type = MoveType.Psychic,
            },
            [MoveId.Mist] = new()
            {
                Id = MoveId.Mist,
                Num = 54,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Mist",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Metronome = true,
                },
                SideCondition = ConditionId.Mist,
                Condition = _library.Conditions[ConditionId.Mist],
                Secondary = null,
                Target = MoveTarget.AllySide,
                Type = MoveType.Ice,
            },
            [MoveId.MistBall] = new()
            {
                Id = MoveId.MistBall,
                Num = 296,
                Accuracy = 100,
                BasePower = 95,
                Category = MoveCategory.Special,
                Name = "Mist Ball",
                BasePp = 5,
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
                    Chance = 50,
                    Boosts = new SparseBoostsTable { SpA = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.MistyExplosion] = new()
            {
                Id = MoveId.MistyExplosion,
                Num = 802,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Misty Explosion",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                SelfDestruct = MoveSelfDestruct.FromAlways(),
                // TODO: onBasePower - 1.5x if Misty Terrain is active and source is grounded
                Secondary = null,
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Fairy,
            },
            [MoveId.MistyTerrain] = new()
            {
                Id = MoveId.MistyTerrain,
                Num = 581,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Misty Terrain",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    NonSky = true,
                    Metronome = true,
                },
                Condition = _library.Conditions[ConditionId.MistyTerrain],
                Secondary = null,
                Target = MoveTarget.All,
                Type = MoveType.Fairy,
            },
            [MoveId.Moonblast] = new()
            {
                Id = MoveId.Moonblast,
                Num = 585,
                Accuracy = 100,
                BasePower = 95,
                Category = MoveCategory.Special,
                Name = "Moonblast",
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
                    Boosts = new SparseBoostsTable { SpA = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.MoongeistBeam] = new()
            {
                Id = MoveId.MoongeistBeam,
                Num = 714,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Moongeist Beam",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true,
                    Mirror = true,
                },
                IgnoreAbility = true,
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.Moonlight] = new()
            {
                Id = MoveId.Moonlight,
                Num = 236,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Moonlight",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Heal = true,
                    Metronome = true,
                },
                // TODO: onHit - heal 50% in normal weather, 66.7% in sun, 25% in other weather
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Fairy,
            },
            [MoveId.MorningSun] = new()
            {
                Id = MoveId.MorningSun,
                Num = 234,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Morning Sun",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Snatch = true,
                    Heal = true,
                    Metronome = true,
                },
                // TODO: onHit - heal 50% in normal weather, 66.7% in sun, 25% in other weather
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.MortalSpin] = new()
            {
                Id = MoveId.MortalSpin,
                Num = 866,
                Accuracy = 100,
                BasePower = 30,
                Category = MoveCategory.Physical,
                Name = "Mortal Spin",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true,
                    Protect = true,
                    Mirror = true,
                    Metronome = true,
                },
                // TODO: onAfterHit - remove hazards from user's side and leech seed
                // TODO: onAfterSubDamage - remove hazards from user's side and leech seed
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Status = ConditionId.Poison,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Poison,
            },
            [MoveId.MountainGale] = new()
            {
                Id = MoveId.MountainGale,
                Num = 836,
                Accuracy = 85,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Mountain Gale",
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
            [MoveId.MysticalFire] = new()
            {
                Id = MoveId.MysticalFire,
                Num = 595,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Special,
                Name = "Mystical Fire",
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
                    Boosts = new SparseBoostsTable { SpA = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.MysticalPower] = new()
            {
                Id = MoveId.MysticalPower,
                Num = 832,
                Accuracy = 90,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Mystical Power",
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
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { SpA = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
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
        };
    }
}
