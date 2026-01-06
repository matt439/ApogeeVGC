using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Utils.Unions;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using PokemonType = ApogeeVGC.Sim.PokemonClasses.PokemonType;

namespace ApogeeVGC.Data.Moves;

public partial record Moves
{
    private Dictionary<MoveId, Move> CreateMovesAbc()
    {
        return new Dictionary<MoveId, Move>
        {
            [MoveId.Absorb] = new()
            {
                Id = MoveId.Absorb,
                Num = 71,
                Accuracy = 100,
                BasePower = 20,
                Category = MoveCategory.Special,
                Name = "Absorb",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Mirror = true, Heal = true, Metronome = true },
                Drain = (1, 2),
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.Accelerock] = new()
            {
                Id = MoveId.Accelerock,
                Num = 709,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Accelerock",
                BasePp = 20,
                Priority = 1,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.Acid] = new()
            {
                Id = MoveId.Acid,
                Num = 51,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Acid",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Boosts = new SparseBoostsTable { SpD = -1 },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Poison,
            },
            [MoveId.AcidSpray] = new()
            {
                Id = MoveId.AcidSpray,
                Num = 491,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Special,
                Name = "Acid Spray",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Mirror = true, Metronome = true, Bullet = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { SpD = -2 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.Acrobatics] = new()
            {
                Id = MoveId.Acrobatics,
                Num = 512,
                Accuracy = 100,
                BasePower = 55,
                Category = MoveCategory.Physical,
                Name = "Acrobatics",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Distance = true,
                    Metronome = true,
                },
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, pokemon, _, move) =>
                {
                    if (pokemon.Item != ItemId.None) return move.BasePower;
                    battle.Debug("BP doubled for no item");
                    return move.BasePower * 2;
                }),
            },
            [MoveId.Acupressure] = new()
            {
                Id = MoveId.Acupressure,
                Num = 367,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Acupressure",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Metronome = true },
                Target = MoveTarget.AdjacentAllyOrSelf,
                Type = MoveType.Normal,
            },
            [MoveId.AerialAce] = new()
            {
                Id = MoveId.AerialAce,
                Num = 332,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Aerial Ace",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Distance = true,
                    Metronome = true, Slicing = true,
                },
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.Aeroblast] = new()
            {
                Id = MoveId.Aeroblast,
                Num = 177,
                Accuracy = 95,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Aeroblast",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Mirror = true, Distance = true, Metronome = true, Wind = true,
                },
                CritRatio = 2,
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.AfterYou] = new()
            {
                Id = MoveId.AfterYou,
                Num = 495,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "After You",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { BypassSub = true, AllyAnim = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Agility] = new()
            {
                Id = MoveId.Agility,
                Num = 97,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Agility",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                SelfBoost = new SparseBoostsTable { Spe = 2 },
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.AirCutter] = new()
            {
                Id = MoveId.AirCutter,
                Num = 314,
                Accuracy = 95,
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Air Cutter",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Mirror = true, Metronome = true, Slicing = true, Wind = true,
                },
                CritRatio = 2,
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Flying,
            },
            [MoveId.AirSlash] = new()
            {
                Id = MoveId.AirSlash,
                Num = 403,
                Accuracy = 95,
                BasePower = 75,
                Category = MoveCategory.Special,
                Name = "Air Slash",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Mirror = true, Distance = true, Metronome = true,
                    Slicing = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.AlluringVoice] = new()
            {
                Id = MoveId.AlluringVoice,
                Num = 914,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Alluring Voice",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Mirror = true, Sound = true, BypassSub = true, Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    OnHit = (_, target, source, move) =>
                    {
                        if (target.StatsRaisedThisTurn)
                        {
                            target.AddVolatile(ConditionId.Confusion, source, move);
                        }

                        return new VoidReturn();
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.AllySwitch] = new()
            {
                Id = MoveId.AllySwitch,
                Num = 502,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Ally Switch",
                BasePp = 15,
                Priority = 2,
                Flags = new MoveFlags { Metronome = true },
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.Amnesia] = new()
            {
                Id = MoveId.Amnesia,
                Num = 133,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Amnesia",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                SelfBoost = new SparseBoostsTable { SpD = 2 },
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.AncientPower] = new()
            {
                Id = MoveId.AncientPower,
                Num = 246,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Ancient Power",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable
                            { Atk = 1, Def = 1, SpA = 1, SpD = 1, Spe = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.AppleAcid] = new()
            {
                Id = MoveId.AppleAcid,
                Num = 787,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Apple Acid",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { SpD = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.AquaCutter] = new()
            {
                Id = MoveId.AquaCutter,
                Num = 895,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Aqua Cutter",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Mirror = true, Metronome = true, Slicing = true },
                CritRatio = 2,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.AquaJet] = new()
            {
                Id = MoveId.AquaJet,
                Num = 453,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Aqua Jet",
                BasePp = 20,
                Priority = 1,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.AquaRing] = new()
            {
                Id = MoveId.AquaRing,
                Num = 392,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Aqua Ring",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                VolatileStatus = ConditionId.AquaRing,
                Target = MoveTarget.Self,
                Type = MoveType.Water,
            },
            [MoveId.AquaStep] = new()
            {
                Id = MoveId.AquaStep,
                Num = 872,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Aqua Step",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Dance = true, Metronome = true,
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
                Type = MoveType.Water,
            },
            [MoveId.AquaTail] = new()
            {
                Id = MoveId.AquaTail,
                Num = 401,
                Accuracy = 90,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Aqua Tail",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.ArmorCannon] = new()
            {
                Id = MoveId.ArmorCannon,
                Num = 890,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Armor Cannon",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { Def = -1, SpD = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.ArmThrust] = new()
            {
                Id = MoveId.ArmThrust,
                Num = 292,
                Accuracy = 100,
                BasePower = 15,
                Category = MoveCategory.Physical,
                Name = "Arm Thrust",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                MultiHit = new[] { 2, 5 },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.AromaticMist] = new()
            {
                Id = MoveId.AromaticMist,
                Num = 597,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Aromatic Mist",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { BypassSub = true, Metronome = true },
                Boosts = new SparseBoostsTable { SpD = 1 },
                Target = MoveTarget.AdjacentAlly,
                Type = MoveType.Fairy,
            },
            [MoveId.Assurance] = new()
            {
                Id = MoveId.Assurance,
                Num = 372,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Assurance",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, target, move) =>
                {
                    if (target.HurtThisTurn is > 0)
                    {
                        battle.Debug("BP doubled on damaged target");
                        return move.BasePower * 2;
                    }

                    return move.BasePower;
                }),
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.Astonish] = new()
            {
                Id = MoveId.Astonish,
                Num = 310,
                Accuracy = 100,
                BasePower = 30,
                Category = MoveCategory.Physical,
                Name = "Astonish",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.AstralBarrage] = new()
            {
                Id = MoveId.AstralBarrage,
                Num = 825,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Astral Barrage",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Ghost,
            },
            [MoveId.AttackOrder] = new()
            {
                Id = MoveId.AttackOrder,
                Num = 454,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Attack Order",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                CritRatio = 2,
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.Attract] = new()
            {
                Id = MoveId.Attract,
                Num = 213,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Attract",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Reflectable = true, Mirror = true, BypassSub = true,
                    Metronome = true,
                },
                VolatileStatus = ConditionId.Attract,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.AuraSphere] = new()
            {
                Id = MoveId.AuraSphere,
                Num = 396,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 80,
                Category = MoveCategory.Special,
                Name = "Aura Sphere",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Mirror = true, Distance = true, Metronome = true, Bullet = true,
                    Pulse = true,
                },
                Target = MoveTarget.Any,
                Type = MoveType.Fighting,
            },
            [MoveId.AuraWheel] = new()
            {
                Id = MoveId.AuraWheel,
                Num = 783,
                Accuracy = 100,
                BasePower = 110,
                Category = MoveCategory.Physical,
                Name = "Aura Wheel",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { Spe = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.AuroraBeam] = new()
            {
                Id = MoveId.AuroraBeam,
                Num = 62,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Special,
                Name = "Aurora Beam",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Boosts = new SparseBoostsTable { Atk = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.AuroraVeil] = new()
            {
                Id = MoveId.AuroraVeil,
                Num = 694,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Aurora Veil",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                SideCondition = ConditionId.AuroraVeil,
                Target = MoveTarget.AllySide,
                Type = MoveType.Ice,
            },
            [MoveId.Avalanche] = new()
            {
                Id = MoveId.Avalanche,
                Num = 419,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Avalanche",
                BasePp = 10,
                Priority = -4,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                BasePowerCallback =
                    new BasePowerCallbackEventInfo((battle, pokemon, target, move) =>
                    {
                        bool damagedByTarget = pokemon.AttackedBy.Any(p =>
                            p.Source == target && p is { Damage: > 0, ThisTurn: true }
                        );
                        if (damagedByTarget)
                        {
                            battle.Debug($"BP doubled for getting hit by {target}");
                            return move.BasePower * 2;
                        }

                        return move.BasePower;
                    }),
                Target = MoveTarget.Normal,
                Type = MoveType.Ice,
            },
            [MoveId.AxeKick] = new()
            {
                Id = MoveId.AxeKick,
                Num = 853,
                Accuracy = 90,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Axe Kick",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                HasCrashDamage = true,
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Confusion,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.BabyDollEyes] = new()
            {
                Id = MoveId.BabyDollEyes,
                Num = 608,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Baby-Doll Eyes",
                BasePp = 30,
                Priority = 1,
                Flags = new MoveFlags
                {
                    Protect = true, Reflectable = true, Mirror = true, AllyAnim = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Atk = -1 },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.BanefulBunker] = new()
            {
                Id = MoveId.BanefulBunker,
                Num = 661,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Baneful Bunker",
                BasePp = 10,
                Priority = 4,
                Flags = new MoveFlags { NoAssist = true, FailCopycat = true },
                StallingMove = true,
                VolatileStatus = ConditionId.BanefulBunker,
                Target = MoveTarget.Self,
                Type = MoveType.Poison,
            },
            [MoveId.BarbBarrage] = new()
            {
                Id = MoveId.BarbBarrage,
                Num = 839,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Barb Barrage",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                OnBasePower = new OnBasePowerEventInfo((battle, _, _, target, _) =>
                {
                    if (target.Status is ConditionId.Poison or ConditionId.Toxic)
                    {
                        return battle.ChainModify(2);
                    }

                    return DoubleVoidUnion.FromVoid();
                }),
                Secondary = new SecondaryEffect
                {
                    Chance = 50,
                    Status = ConditionId.Poison,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.BatonPass] = new()
            {
                Id = MoveId.BatonPass,
                Num = 226,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Baton Pass",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags { Metronome = true },
                SelfSwitch = MoveSelfSwitch.FromCopyVolatile(),
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.BeatUp] = new()
            {
                Id = MoveId.BeatUp,
                Num = 251,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Beat Up",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Mirror = true, AllyAnim = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.Belch] = new()
            {
                Id = MoveId.Belch,
                Num = 562,
                Accuracy = 90,
                BasePower = 120,
                Category = MoveCategory.Special,
                Name = "Belch",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, FailMeFirst = true, NoSleepTalk = true, NoAssist = true,
                    FailCopycat = true, FailMimic = true, FailInstruct = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.BellyDrum] = new()
            {
                Id = MoveId.BellyDrum,
                Num = 187,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Belly Drum",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Bind] = new()
            {
                Id = MoveId.Bind,
                Num = 20,
                Accuracy = 85,
                BasePower = 15,
                Category = MoveCategory.Physical,
                Name = "Bind",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                VolatileStatus = ConditionId.PartiallyTrapped,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Bite] = new()
            {
                Id = MoveId.Bite,
                Num = 44,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Bite",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Metronome = true, Bite = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
            [MoveId.BitterBlade] = new()
            {
                Id = MoveId.BitterBlade,
                Num = 891,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Physical,
                Name = "Bitter Blade",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Heal = true, Metronome = true,
                    Slicing = true,
                },
                Drain = (1, 2),
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.BitterMalice] = new()
            {
                Id = MoveId.BitterMalice,
                Num = 841,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Special,
                Name = "Bitter Malice",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Atk = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.BlastBurn] = new()
            {
                Id = MoveId.BlastBurn,
                Num = 307,
                Accuracy = 90,
                BasePower = 150,
                Category = MoveCategory.Special,
                Name = "Blast Burn",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Recharge = true, Protect = true, Mirror = true, Metronome = true },
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.MustRecharge,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.BlazeKick] = new()
            {
                Id = MoveId.BlazeKick,
                Num = 299,
                Accuracy = 90,
                BasePower = 85,
                Category = MoveCategory.Physical,
                Name = "Blaze Kick",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                CritRatio = 2,
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.BleakwindStorm] = new()
            {
                Id = MoveId.BleakwindStorm,
                Num = 846,
                Accuracy = 80,
                BasePower = 100,
                Category = MoveCategory.Special,
                Name = "Bleakwind Storm",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Mirror = true, Metronome = true, Wind = true },
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, target) =>
                {
                    if (target != null)
                    {
                        ConditionId effectiveWeather = target.EffectiveWeather();
                        if (effectiveWeather is ConditionId.RainDance or ConditionId.PrimordialSea)
                        {
                            move.Accuracy = IntTrueUnion.FromTrue();
                        }
                    }
                }),
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Boosts = new SparseBoostsTable { Spe = -1 },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Flying,
            },
            [MoveId.Blizzard] = new()
            {
                Id = MoveId.Blizzard,
                Num = 59,
                Accuracy = 70,
                BasePower = 110,
                Category = MoveCategory.Special,
                Name = "Blizzard",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Mirror = true, Metronome = true, Wind = true },
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, _, _) =>
                {
                    if (battle.Field.IsWeather([ConditionId.Snowscape]))
                    {
                        move.Accuracy = IntTrueUnion.FromTrue();
                    }
                }),
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Freeze,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Ice,
            },
            [MoveId.Block] = new()
            {
                Id = MoveId.Block,
                Num = 335,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Block",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Reflectable = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.BloodMoon] = new()
            {
                Id = MoveId.BloodMoon,
                Num = 901,
                Accuracy = 100,
                BasePower = 140,
                Category = MoveCategory.Special,
                Name = "Blood Moon",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Mirror = true, Metronome = true, CantUseTwice = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.BlueFlare] = new()
            {
                Id = MoveId.BlueFlare,
                Num = 551,
                Accuracy = 85,
                BasePower = 130,
                Category = MoveCategory.Special,
                Name = "Blue Flare",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    Status = ConditionId.Burn,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fire,
            },
            [MoveId.BodyPress] = new()
            {
                Id = MoveId.BodyPress,
                Num = 776,
                Accuracy = 100,
                BasePower = 80,
                Category = MoveCategory.Physical,
                Name = "Body Press",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                OverrideOffensiveStat = StatIdExceptHp.Def, // Uses Def instead of Atk
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.BodySlam] = new()
            {
                Id = MoveId.BodySlam,
                Num = 34,
                Accuracy = 100,
                BasePower = 85,
                Category = MoveCategory.Physical,
                Name = "Body Slam",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Metronome = true, NonSky = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.BoltStrike] = new()
            {
                Id = MoveId.BoltStrike,
                Num = 550,
                Accuracy = 85,
                BasePower = 130,
                Category = MoveCategory.Physical,
                Name = "Bolt Strike",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 20,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.BoneRush] = new()
            {
                Id = MoveId.BoneRush,
                Num = 198,
                Accuracy = 90,
                BasePower = 25,
                Category = MoveCategory.Physical,
                Name = "Bone Rush",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                MultiHit = new[] { 2, 5 },
                Target = MoveTarget.Normal,
                Type = MoveType.Ground,
            },
            [MoveId.Boomburst] = new()
            {
                Id = MoveId.Boomburst,
                Num = 586,
                Accuracy = 100,
                BasePower = 140,
                Category = MoveCategory.Special,
                Name = "Boomburst",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Mirror = true, Sound = true, BypassSub = true, Metronome = true,
                },
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Normal,
            },
            [MoveId.Bounce] = new()
            {
                Id = MoveId.Bounce,
                Num = 340,
                Accuracy = 85,
                BasePower = 85,
                Category = MoveCategory.Physical,
                Name = "Bounce",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Charge = true, Protect = true, Mirror = true, Gravity = true,
                    Distance = true, Metronome = true, NoSleepTalk = true, NoAssist = true,
                    FailInstruct = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    Status = ConditionId.Paralysis,
                },
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.BranchPoke] = new()
            {
                Id = MoveId.BranchPoke,
                Num = 785,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Branch Poke",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.BraveBird] = new()
            {
                Id = MoveId.BraveBird,
                Num = 413,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Brave Bird",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Distance = true,
                    Metronome = true,
                },
                Recoil = (33, 100),
                Target = MoveTarget.Any,
                Type = MoveType.Flying,
            },
            [MoveId.BreakingSwipe] = new()
            {
                Id = MoveId.BreakingSwipe,
                Num = 784,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Breaking Swipe",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Atk = -1 },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Dragon,
            },
            [MoveId.BrickBreak] = new()
            {
                Id = MoveId.BrickBreak,
                Num = 280,
                Accuracy = 100,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Brick Break",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.Brine] = new()
            {
                Id = MoveId.Brine,
                Num = 362,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Special,
                Name = "Brine",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                OnBasePower = new OnBasePowerEventInfo((battle, _, _, target, _) =>
                {
                    if (target.Hp * 2 <= target.MaxHp)
                    {
                        return battle.ChainModify(2);
                    }

                    return DoubleVoidUnion.FromVoid();
                }),
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.BrutalSwing] = new()
            {
                Id = MoveId.BrutalSwing,
                Num = 693,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Brutal Swing",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Dark,
            },
            [MoveId.BubbleBeam] = new()
            {
                Id = MoveId.BubbleBeam,
                Num = 61,
                Accuracy = 100,
                BasePower = 65,
                Category = MoveCategory.Special,
                Name = "Bubble Beam",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Boosts = new SparseBoostsTable { Spe = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.BugBite] = new()
            {
                Id = MoveId.BugBite,
                Num = 450,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Bug Bite",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.BugBuzz] = new()
            {
                Id = MoveId.BugBuzz,
                Num = 405,
                Accuracy = 100,
                BasePower = 90,
                Category = MoveCategory.Special,
                Name = "Bug Buzz",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Mirror = true, Sound = true, BypassSub = true, Metronome = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Boosts = new SparseBoostsTable { SpD = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Bug,
            },
            [MoveId.BulkUp] = new()
            {
                Id = MoveId.BulkUp,
                Num = 339,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Bulk Up",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                SelfBoost = new SparseBoostsTable { Atk = 1, Def = 1 },
                Target = MoveTarget.Self,
                Type = MoveType.Fighting,
            },
            [MoveId.Bulldoze] = new()
            {
                Id = MoveId.Bulldoze,
                Num = 523,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Bulldoze",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Mirror = true, Metronome = true, NonSky = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Spe = -1 },
                },
                Target = MoveTarget.AllAdjacent,
                Type = MoveType.Ground,
            },
            [MoveId.BulletPunch] = new()
            {
                Id = MoveId.BulletPunch,
                Num = 418,
                Accuracy = 100,
                BasePower = 40,
                Category = MoveCategory.Physical,
                Name = "Bullet Punch",
                BasePp = 30,
                Priority = 1,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Punch = true, Metronome = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Steel,
            },
            [MoveId.BulletSeed] = new()
            {
                Id = MoveId.BulletSeed,
                Num = 331,
                Accuracy = 100,
                BasePower = 25,
                Category = MoveCategory.Physical,
                Name = "Bullet Seed",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Mirror = true, Metronome = true, Bullet = true },
                MultiHit = new[] { 2, 5 },
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.BurningBulwark] = new()
            {
                Id = MoveId.BurningBulwark,
                Num = 908,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Burning Bulwark",
                BasePp = 10,
                Priority = 4,
                Flags = new MoveFlags { Metronome = true, NoAssist = true, FailCopycat = true },
                StallingMove = true,
                VolatileStatus = ConditionId.BurningBulwark,
                Target = MoveTarget.Self,
                Type = MoveType.Fire,
            },
            [MoveId.BurningJealousy] = new()
            {
                Id = MoveId.BurningJealousy,
                Num = 807,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Special,
                Name = "Burning Jealousy",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    OnHit = (_, target, source, move) =>
                    {
                        if (target.StatsRaisedThisTurn)
                        {
                            target.TrySetStatus(ConditionId.Burn, source, move);
                        }

                        return new VoidReturn();
                    },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Fire,
            },
            [MoveId.CalmMind] = new()
            {
                Id = MoveId.CalmMind,
                Num = 347,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Calm Mind",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                SelfBoost = new SparseBoostsTable { SpA = 1, SpD = 1 },
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.CeaselessEdge] = new()
            {
                Id = MoveId.CeaselessEdge,
                Num = 845,
                Accuracy = 90,
                BasePower = 65,
                Category = MoveCategory.Physical,
                Name = "Ceaseless Edge",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Metronome = true, Slicing = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
                OnAfterHit = new OnAfterHitEventInfo((_, _, source, move) =>
                {
                    if ((move.HasSheerForce ?? false) || source.Hp <= 0) return new VoidReturn();
                    foreach (Side side in source.Side.FoeSidesWithConditions())
                    {
                        side.AddSideCondition(ConditionId.Spikes);
                    }

                    return new VoidReturn();
                }),
                OnAfterSubDamage = new OnAfterSubDamageEventInfo((_, _, _, source, move) =>
                {
                    if ((move.HasSheerForce ?? false) || source.Hp <= 0) return;
                    foreach (Side side in source.Side.FoeSidesWithConditions())
                    {
                        side.AddSideCondition(ConditionId.Spikes);
                    }
                }),
            },
            [MoveId.Celebrate] = new()
            {
                Id = MoveId.Celebrate,
                Num = 606,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Celebrate",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags
                {
                    NoSleepTalk = true, NoAssist = true, FailCopycat = true, FailMimic = true,
                    FailInstruct = true,
                },
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
                OnTryHit = new OnTryHitEventInfo((battle, target, _, _) =>
                {
                    battle.Add("-activate", target, "move: Celebrate");
                    return new VoidReturn();
                }),
            },
            [MoveId.Charge] = new()
            {
                Id = MoveId.Charge,
                Num = 268,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Charge",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                VolatileStatus = ConditionId.Charge,
                SelfBoost = new SparseBoostsTable { SpD = 1 },
                Target = MoveTarget.Self,
                Type = MoveType.Electric,
            },
            [MoveId.ChargeBeam] = new()
            {
                Id = MoveId.ChargeBeam,
                Num = 451,
                Accuracy = 90,
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Charge Beam",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 70,
                    Self = new SecondaryEffect
                    {
                        Boosts = new SparseBoostsTable { SpA = 1 },
                    },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Electric,
            },
            [MoveId.Charm] = new()
            {
                Id = MoveId.Charm,
                Num = 204,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Charm",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Reflectable = true, Mirror = true, AllyAnim = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { Atk = -2 },
                Target = MoveTarget.Normal,
                Type = MoveType.Fairy,
            },
            [MoveId.ChillingWater] = new()
            {
                Id = MoveId.ChillingWater,
                Num = 886,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Chilling Water",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Atk = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.ChillyReception] = new()
            {
                Id = MoveId.ChillyReception,
                Num = 881,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Chilly Reception",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags(),
                SelfSwitch = true,
                Target = MoveTarget.All,
                Type = MoveType.Ice,
                PriorityChargeCallback = new PriorityChargeCallbackEventInfo((_, source) =>
                {
                    source.AddVolatile(ConditionId.ChillyReception);
                }),
                OnHitField = new OnHitFieldEventInfo((battle, _, source, _) =>
                {
                    battle.Field.SetWeather(ConditionId.Snowscape, source);
                    return new VoidReturn();
                }),
            },
            [MoveId.Chloroblast] = new()
            {
                Id = MoveId.Chloroblast,
                Num = 835,
                Accuracy = 95,
                BasePower = 150,
                Category = MoveCategory.Special,
                Name = "Chloroblast",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                MindBlownRecoil = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Grass,
            },
            [MoveId.CircleThrow] = new()
            {
                Id = MoveId.CircleThrow,
                Num = 509,
                Accuracy = 90,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Circle Throw",
                BasePp = 10,
                Priority = -6,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Metronome = true,
                    NoAssist = true, FailCopycat = true,
                },
                ForceSwitch = true,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.ClangingScales] = new()
            {
                Id = MoveId.ClangingScales,
                Num = 691,
                Accuracy = 100,
                BasePower = 110,
                Category = MoveCategory.Special,
                Name = "Clanging Scales",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Mirror = true, Sound = true, BypassSub = true, Metronome = true,
                },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { Def = -1 },
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Dragon,
            },
            [MoveId.ClangorousSoul] = new()
            {
                Id = MoveId.ClangorousSoul,
                Num = 775,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Clangorous Soul",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Sound = true, Dance = true },
                SelfBoost = new SparseBoostsTable { Atk = 1, Def = 1, SpA = 1, SpD = 1, Spe = 1 },
                Target = MoveTarget.Self,
                Type = MoveType.Dragon,
                OnTry = new OnTryEventInfo((_, source, _, _) =>
                {
                    // Fails if HP is 33% or less, or if maxhp is 1 (Shedinja)
                    if (source.Hp <= source.MaxHp * 33 / 100 || source.MaxHp == 1)
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
                OnTryHit = new OnTryHitEventInfo((battle, _, source, move) =>
                {
                    if (move.Boosts is null)
                    {
                        battle.Debug("Clangorous Soul has no boosts to apply");
                        return new VoidReturn();
                    }

                    // Apply boosts first, fail if boosts couldn't be applied
                    BoolZeroUnion? boostResult = battle.Boost(move.Boosts, source, source, move);
                    if (boostResult == null || !boostResult.IsTruthy()) return false;
                    return new VoidReturn();
                }),
                OnHit = new OnHitEventInfo((battle, _, source, _) =>
                {
                    // Deal 33% recoil damage
                    battle.DirectDamage(source.MaxHp * 33 / 100, source);
                    return new VoidReturn();
                }),
            },
            [MoveId.ClearSmog] = new()
            {
                Id = MoveId.ClearSmog,
                Num = 499,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Clear Smog",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
                OnHit = new OnHitEventInfo((battle, target, _, _) =>
                {
                    target.Boosts = new BoostsTable();
                    battle.Add("-clearboost", target);
                    return null;
                }),
            },
            [MoveId.CloseCombat] = new()
            {
                Id = MoveId.CloseCombat,
                Num = 370,
                Accuracy = 100,
                BasePower = 120,
                Category = MoveCategory.Physical,
                Name = "Close Combat",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Self = new SecondaryEffect
                {
                    Boosts = new SparseBoostsTable { Def = -1, SpD = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.Coaching] = new()
            {
                Id = MoveId.Coaching,
                Num = 811,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Coaching",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { BypassSub = true, AllyAnim = true, Metronome = true },
                Boosts = new SparseBoostsTable { Atk = 1, Def = 1 },
                Target = MoveTarget.AdjacentAlly,
                Type = MoveType.Fighting,
            },
            [MoveId.Coil] = new()
            {
                Id = MoveId.Coil,
                Num = 489,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Coil",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                SelfBoost = new SparseBoostsTable { Atk = 1, Def = 1, Accuracy = 1 },
                Target = MoveTarget.Self,
                Type = MoveType.Poison,
            },
            [MoveId.CollisionCourse] = new()
            {
                Id = MoveId.CollisionCourse,
                Num = 878,
                Accuracy = 100,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Collision Course",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Contact = true, Protect = true, Mirror = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
                OnBasePower = new OnBasePowerEventInfo((battle, _, _, target, move) =>
                {
                    if (target.RunEffectiveness(move) <= 0.0) return new VoidReturn();
                    if (battle.DisplayUi)
                    {
                        battle.Debug("collision course super effective buff");
                    }

                    battle.ChainModify([5461, 4096]);
                    return new VoidReturn();
                }),
            },
            [MoveId.Comeuppance] = new()
            {
                Id = MoveId.Comeuppance,
                Num = 894,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Comeuppance",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, FailMeFirst = true },
                Target = MoveTarget.Scripted,
                Type = MoveType.Dark,
                DamageCallback = new DamageCallbackEventInfo((_, pokemon, _, _) =>
                {
                    Attacker? lastDamagedBy = pokemon.GetLastDamagedBy(true);
                    if (lastDamagedBy != null)
                    {
                        int damage = (int)(lastDamagedBy.Damage * 1.5);
                        return damage == 0 ? 1 : damage;
                    }

                    return 0;
                }),
                OnTry = new OnTryEventInfo((_, source, _, _) =>
                {
                    Attacker? lastDamagedBy = source.GetLastDamagedBy(true);
                    if (lastDamagedBy == null || !lastDamagedBy.ThisTurn) return false;
                    return new VoidReturn();
                }),
            },
            [MoveId.Confide] = new()
            {
                Id = MoveId.Confide,
                Num = 590,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Confide",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Reflectable = true, Mirror = true, Sound = true, BypassSub = true,
                    Metronome = true,
                },
                Boosts = new SparseBoostsTable { SpA = -1 },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Confuseray] = new()
            {
                Id = MoveId.Confuseray,
                Num = 109,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Confuse Ray",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Protect = true, Reflectable = true, Mirror = true, Metronome = true },
                VolatileStatus = ConditionId.Confusion,
                Target = MoveTarget.Normal,
                Type = MoveType.Ghost,
            },
            [MoveId.Confusion] = new()
            {
                Id = MoveId.Confusion,
                Num = 93,
                Accuracy = 100,
                BasePower = 50,
                Category = MoveCategory.Special,
                Name = "Confusion",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    VolatileStatus = ConditionId.Confusion,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Psychic,
            },
            [MoveId.Conversion] = new()
            {
                Id = MoveId.Conversion,
                Num = 160,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Conversion",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Conversion2] = new()
            {
                Id = MoveId.Conversion2,
                Num = 176,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Conversion 2",
                BasePp = 30,
                Priority = 0,
                Flags = new MoveFlags { Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Copycat] = new()
            {
                Id = MoveId.Copycat,
                Num = 383,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Copycat",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    FailEncore = true, NoSleepTalk = true, NoAssist = true, FailCopycat = true,
                    FailMimic = true, FailInstruct = true,
                },
                CallsMove = true,
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
                OnHit = new OnHitEventInfo((battle, _, source, _) =>
                {
                    ActiveMove? lastMove = battle.LastMove;
                    if (lastMove == null) return false;

                    // Get the move to use - if it has a base move, use that
                    MoveId moveId = lastMove.BaseMove ?? lastMove.Id;
                    Move moveToUse = battle.Library.Moves[moveId];

                    // Check if the move can be copied
                    if (moveToUse.Flags.FailCopycat == true) return false;

                    battle.Actions.UseMove(moveToUse.Id, source);
                    return new VoidReturn();
                }),
            },
            [MoveId.CosmicPower] = new()
            {
                Id = MoveId.CosmicPower,
                Num = 322,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Cosmic Power",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                SelfBoost = new SparseBoostsTable { Def = 1, SpD = 1 },
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.CottonGuard] = new()
            {
                Id = MoveId.CottonGuard,
                Num = 538,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Cotton Guard",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                SelfBoost = new SparseBoostsTable { Def = 3 },
                Target = MoveTarget.Self,
                Type = MoveType.Grass,
            },
            [MoveId.CottonSpore] = new()
            {
                Id = MoveId.CottonSpore,
                Num = 178,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Cotton Spore",
                BasePp = 40,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Reflectable = true, Mirror = true, Metronome = true,
                    Powder = true,
                },
                Boosts = new SparseBoostsTable { Spe = -2 },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Grass,
            },
            [MoveId.Counter] = new()
            {
                Id = MoveId.Counter,
                Num = 68,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Counter",
                BasePp = 20,
                Priority = -5,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, FailMeFirst = true, NoAssist = true,
                    FailCopycat = true,
                },
                Target = MoveTarget.Scripted,
                Type = MoveType.Fighting,
                Condition = _library.Conditions[ConditionId.Counter],
                DamageCallback = new DamageCallbackEventInfo((_, pokemon, _, _) =>
                {
                    if (!pokemon.Volatiles.TryGetValue(ConditionId.Counter,
                            out EffectState? effectState))
                    {
                        return IntFalseUnion.FromInt(0);
                    }

                    return IntFalseUnion.FromInt(effectState.TotalDamage ?? 1);
                }),
                BeforeTurnCallback = new BeforeTurnCallbackEventInfo((_, pokemon, _, _) =>
                {
                    pokemon.AddVolatile(ConditionId.Counter);
                }),
                OnTry = new OnTryEventInfo((_, source, _, _) =>
                {
                    if (!source.Volatiles.TryGetValue(ConditionId.Counter,
                            out EffectState? effectState))
                    {
                        return BoolEmptyVoidUnion.FromBool(false);
                    }

                    if (effectState.Slot == null)
                    {
                        return BoolEmptyVoidUnion.FromBool(false);
                    }

                    return BoolEmptyVoidUnion.FromVoid();
                }),
            },
            [MoveId.CourtChange] = new()
            {
                Id = MoveId.CourtChange,
                Num = 756,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Court Change",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Mirror = true, Metronome = true },
                Target = MoveTarget.All,
                Type = MoveType.Normal,
                OnHitField = new OnHitFieldEventInfo((battle, _, source, _) =>
                {
                    ConditionId[] sideConditions =
                    [
                        ConditionId.Mist, ConditionId.LightScreen, ConditionId.Reflect,
                        ConditionId.Spikes,
                        ConditionId.Safeguard, ConditionId.Tailwind, ConditionId.ToxicSpikes,
                        ConditionId.StealthRock,
                        ConditionId.StickyWeb, ConditionId.AuroraVeil,
                    ];
                    bool success = false;

                    Side sourceSide = source.Side;
                    Side targetSide = source.Side.Foe;

                    // Store conditions from both sides
                    var sourceConditions = new Dictionary<ConditionId, EffectState>();
                    var targetConditions = new Dictionary<ConditionId, EffectState>();

                    foreach (ConditionId id in sideConditions)
                    {
                        if (sourceSide.SideConditions.TryGetValue(id, out EffectState? sourceState))
                        {
                            sourceConditions[id] = sourceState;
                            sourceSide.SideConditions.Remove(id);
                            success = true;
                        }

                        if (targetSide.SideConditions.TryGetValue(id, out EffectState? targetState))
                        {
                            targetConditions[id] = targetState;
                            targetSide.SideConditions.Remove(id);
                            success = true;
                        }
                    }

                    // Swap conditions
                    foreach ((ConditionId id, EffectState state) in sourceConditions)
                    {
                        state.Target = targetSide;
                        targetSide.SideConditions[id] = state;
                    }

                    foreach ((ConditionId id, EffectState state) in targetConditions)
                    {
                        state.Target = sourceSide;
                        sourceSide.SideConditions[id] = state;
                    }

                    if (!success) return false;
                    battle.Add("-swapsideconditions");
                    battle.Add("-activate", source, "move: Court Change");
                    return new VoidReturn();
                }),
            },
            [MoveId.Covet] = new()
            {
                Id = MoveId.Covet,
                Num = 343,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Covet",
                BasePp = 25,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, FailMeFirst = true,
                    NoAssist = true, FailCopycat = true,
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
                OnAfterHit = new OnAfterHitEventInfo((battle, target, source, move) =>
                {
                    // Can't steal if source already has an item or used a gem
                    if (source.Item != ItemId.None || source.Volatiles.ContainsKey(ConditionId.Gem))
                    {
                        return new VoidReturn();
                    }

                    ItemFalseUnion yourItemResult = target.TakeItem(source);
                    if (yourItemResult is not ItemItemFalseUnion yourItemUnion)
                    {
                        return new VoidReturn();
                    }

                    Item yourItem = yourItemUnion.Item;
                    RelayVar? takeItemResult = battle.SingleEvent(EventId.TakeItem, yourItem,
                        target.ItemState, source, target, move);
                    if (takeItemResult is BoolRelayVar { Value: false } ||
                        !source.SetItem(yourItem.Id))
                    {
                        target.Item = yourItem.Id; // Bypass setItem so we don't break choice lock
                        return new VoidReturn();
                    }

                    battle.Add("-item", source, yourItem.Name, "[from] move: Covet",
                        $"[of] {target}");
                    return new VoidReturn();
                }),
            },
            [MoveId.Crabhammer] = new()
            {
                Id = MoveId.Crabhammer,
                Num = 152,
                Accuracy = 90,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Crabhammer",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                CritRatio = 2,
                Target = MoveTarget.Normal,
                Type = MoveType.Water,
            },
            [MoveId.CrossChop] = new()
            {
                Id = MoveId.CrossChop,
                Num = 238,
                Accuracy = 80,
                BasePower = 100,
                Category = MoveCategory.Physical,
                Name = "Cross Chop",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                CritRatio = 2,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.CrossPoison] = new()
            {
                Id = MoveId.CrossPoison,
                Num = 440,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Cross Poison",
                BasePp = 20,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Contact = true, Protect = true, Mirror = true, Metronome = true, Slicing = true,
                },
                Secondary = new SecondaryEffect
                {
                    Chance = 10,
                    Status = ConditionId.Poison,
                },
                CritRatio = 2,
                Target = MoveTarget.Normal,
                Type = MoveType.Poison,
            },
            [MoveId.CrushClaw] = new()
            {
                Id = MoveId.CrushClaw,
                Num = 306,
                Accuracy = 95,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Crush Claw",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 50,
                    Boosts = new SparseBoostsTable { Def = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.CrushGrip] = new()
            {
                Id = MoveId.CrushGrip,
                Num = 462,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Crush Grip",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, target, _) =>
                {
                    int hp = target.Hp;
                    int maxHp = target.MaxHp;
                    int bp = (int)Math.Floor((int)Math.Floor(
                        (120 * (100 * (int)Math.Floor((double)hp * 4096 / maxHp)) + 2048 - 1) /
                        4096.0) / 100.0);
                    bp = bp == 0 ? 1 : bp;
                    battle.Debug($"BP for {hp}/{maxHp} HP: {bp}");
                    return bp;
                }),
            },
            [MoveId.Curse] = new()
            {
                Id = MoveId.Curse,
                Num = 174,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Curse",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { BypassSub = true, Metronome = true },
                VolatileStatus = ConditionId.Curse,
                Target = MoveTarget.Normal,
                NonGhostTarget = MoveTarget.Self,
                Type = MoveType.Ghost,
                OnModifyMove = new OnModifyMoveEventInfo((_, move, source, target) =>
                {
                    if (!source.HasType(PokemonType.Ghost))
                    {
                        // Non-Ghost: change target to self
                        move.Target = move.NonGhostTarget ?? MoveTarget.Self;
                    }
                    else if (source.IsAlly(target))
                    {
                        // Ghost targeting ally: pick random foe instead
                        move.Target = MoveTarget.RandomNormal;
                    }
                }),
                OnTryHit = new OnTryHitEventInfo((_, target, source, move) =>
                {
                    if (!source.HasType(PokemonType.Ghost))
                    {
                        // Non-Ghost variant: add stat boosts instead of curse
                        // The volatile status will still try to apply but target is self
                        move.Self = new SecondaryEffect
                        {
                            Boosts = new SparseBoostsTable { Spe = -1, Atk = 1, Def = 1 },
                        };
                    }
                    else if (target.Volatiles.ContainsKey(ConditionId.Curse))
                    {
                        // Ghost variant: fail if target already cursed
                        return false;
                    }

                    return new VoidReturn();
                }),
                OnHit = new OnHitEventInfo((battle, _, source, _) =>
                {
                    // Ghost variant: user loses 50% HP
                    if (source.HasType(PokemonType.Ghost))
                    {
                        battle.DirectDamage(source.MaxHp / 2, source, source);
                    }

                    return new VoidReturn();
                }),
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
                    Boosts = new SparseBoostsTable { Def = -1 },
                },
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
        };
    }
}