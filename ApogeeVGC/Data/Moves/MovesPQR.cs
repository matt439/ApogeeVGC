using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.MoveEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
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
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    int targetHp = target.Hp;
                    int averageHp = Math.Max(1, (targetHp + source.Hp) / 2);
                    int targetChange = targetHp - averageHp;
                    target.SetHp(target.Hp - targetChange);
                    source.SetHp(averageHp);
                    battle.Add("-sethp", target,
                        (Func<SideSecretSharedResult>)(() => target.GetHealth()),
                        "[from] move: Pain Split", "[silent]");
                    battle.Add("-sethp", source,
                        (Func<SideSecretSharedResult>)(() => source.GetHealth()),
                        "[from] move: Pain Split");
                    return new VoidReturn();
                }),
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
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    BoolZeroUnion? success =
                        battle.Boost(new SparseBoostsTable { Atk = -1, SpA = -1 }, target, source,
                            move);
                    // If the boost fails completely and target doesn't have Mirror Armor, prevent the switch
                    if (success?.IsTruthy() != true && !target.HasAbility(AbilityId.MirrorArmor))
                    {
                        move.SelfSwitch = null;
                    }

                    return new VoidReturn();
                }),
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
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, target, _) =>
                {
                    // Double power if target has already moved this turn
                    if (target.NewlySwitched || battle.Queue.WillMove(target) is not null)
                    {
                        return 50; // Normal base power
                    }

                    return 100; // Doubled base power
                }),
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
                OnHitField = new OnHitFieldEventInfo((battle, _, source, move) =>
                {
                    bool result = false;
                    bool message = false;
                    foreach (Pokemon pokemon in battle.GetAllActive())
                    {
                        // Check invulnerability
                        RelayVar? invulnResult = battle.RunEvent(EventId.Invulnerability, pokemon,
                            source, move);
                        if (invulnResult is BoolRelayVar { Value: false })
                        {
                            battle.Add("-miss", source, pokemon);
                            result = true;
                            continue;
                        }

                        // Check TryHit
                        RelayVar? tryHitResult =
                            battle.RunEvent(EventId.TryHit, pokemon, source, move);
                        if (tryHitResult == null)
                        {
                            result = true;
                            continue;
                        }

                        // Add volatile if not already present
                        if (!pokemon.Volatiles.ContainsKey(ConditionId.PerishSong))
                        {
                            pokemon.AddVolatile(ConditionId.PerishSong);
                            battle.Add("-start", pokemon, "perish3", "[silent]");
                            result = true;
                            message = true;
                        }
                    }

                    if (!result) return false;
                    if (message) battle.Add("-fieldactivate", "move: Perish Song");
                    return new VoidReturn();
                }),
                Condition = _library.Conditions[ConditionId.PerishSong],
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
                OnTryMove = new OnTryMoveEventInfo((battle, source, target, move) =>
                {
                    // If we have the volatile from turn 1, remove it and continue with the attack
                    if (source.RemoveVolatile(battle.Library.Conditions[ConditionId.TwoTurnMove]))
                    {
                        return new VoidReturn(); // Continue with the attack
                    }

                    // Turn 1: Prepare the move
                    battle.Add("-prepare", source, move.Name);
                    RelayVar? chargeResult =
                        battle.RunEvent(EventId.ChargeMove, source, target, move);
                    if (chargeResult is BoolRelayVar { Value: false })
                    {
                        return false;
                    }

                    source.AddVolatile(ConditionId.TwoTurnMove, target);
                    return null; // Stop the move on turn 1
                }),
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
                OnModifyMove = new OnModifyMoveEventInfo((_, move, source, _) =>
                {
                    // Use Physical category if Attack > Special Attack
                    if (source.GetStat(StatIdExceptHp.Atk, false, true) >
                        source.GetStat(StatIdExceptHp.SpA, false, true))
                    {
                        move.Category = MoveCategory.Physical;
                    }
                }),
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
                Boosts = new SparseBoostsTable { Atk = -1 },
                Secondary = null,
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
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    Item item = target.GetItem();
                    if (source.Hp > 0 && item.IsBerry)
                    {
                        ItemFalseUnion takeResult = target.TakeItem(source);
                        if (takeResult is ItemItemFalseUnion takenItem)
                        {
                            battle.Add("-enditem", target, takenItem.Item.Name, "[from] stealeat",
                                "[move] Pluck", $"[of] {source}");
                            // Trigger the Eat event on the item for the source
                            battle.SingleEvent(EventId.Eat, takenItem.Item, target.ItemState,
                                source, source, move);
                            battle.RunEvent(EventId.EatItem, source, source, move, takenItem.Item);
                            if (takenItem.Item.OnEat != null)
                            {
                                source.AteBerry = true;
                            }
                        }
                    }

                    return new VoidReturn();
                }),
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
                OnTryMove = new OnTryMoveEventInfo((battle, source, target, move) =>
                {
                    // Fail if targeting ally while under a heal-blocking effect
                    // Note: TypeScript checks for 'healblock' volatile. HealBlock is not in Gen 9 VGC.
                    // We use PsychicNoise as a simplified placeholder for heal-blocking effects.
                    if (source.IsAlly(target) &&
                        source.Volatiles.ContainsKey(ConditionId.PsychicNoise))
                    {
                        battle.AttrLastMove("[still]");
                        battle.Add("cant", source, "move: Heal Block", move);
                        return false;
                    }

                    return new VoidReturn();
                }),
                OnTryHit = new OnTryHitEventInfo((_, target, source, move) =>
                {
                    if (source.IsAlly(target))
                    {
                        // When targeting an ally, set power to 0 (we heal instead)
                        move.BasePower = 0;
                        move.Infiltrates = true;
                    }

                    return new VoidReturn();
                }),
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    if (source.IsAlly(target))
                    {
                        // Heal ally by 50% of their max HP
                        IntFalseUnion healResult =
                            battle.Heal(target.BaseMaxHp / 2, target, source);
                        if (healResult is FalseIntFalseUnion)
                        {
                            return BoolEmptyVoidUnion.FromEmpty(); // NOT_FAIL equivalent
                        }
                    }

                    return new VoidReturn();
                }),
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
                OnTry = new OnTryEventInfo((battle, _, target, _) =>
                {
                    // Fail if target has no item
                    if (target.Item == ItemId.None)
                    {
                        return false;
                    }

                    // Display the item being manipulated
                    Item item = battle.Library.Items[target.Item];
                    battle.Add("-activate", target, "move: Poltergeist", item.Name);
                    return new VoidReturn();
                }),
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
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    // Average Atk and SpA stats between user and target
                    int newAtk = (target.StoredStats[StatIdExceptHp.Atk] +
                                  source.StoredStats[StatIdExceptHp.Atk]) / 2;
                    target.StoredStats[StatIdExceptHp.Atk] = newAtk;
                    source.StoredStats[StatIdExceptHp.Atk] = newAtk;

                    int newSpA = (target.StoredStats[StatIdExceptHp.SpA] +
                                  source.StoredStats[StatIdExceptHp.SpA]) / 2;
                    target.StoredStats[StatIdExceptHp.SpA] = newSpA;
                    source.StoredStats[StatIdExceptHp.SpA] = newSpA;

                    battle.Add("-activate", source, "move: Power Split", $"[of] {target}");
                    return new VoidReturn();
                }),
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
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    // Swap Atk and SpA boosts with target
                    int targetAtk = target.Boosts.Atk;
                    int targetSpA = target.Boosts.SpA;
                    int sourceAtk = source.Boosts.Atk;
                    int sourceSpA = source.Boosts.SpA;

                    source.SetBoost(new SparseBoostsTable { Atk = targetAtk, SpA = targetSpA });
                    target.SetBoost(new SparseBoostsTable { Atk = sourceAtk, SpA = sourceSpA });

                    battle.Add("-swapboost", source, target, "atk, spa", "[from] move: Power Swap");
                    return new VoidReturn();
                }),
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
                BasePowerCallback = new BasePowerCallbackEventInfo((_, source, _, _) =>
                    20 + 20 * source.PositiveBoosts()),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
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
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, _, _) =>
                {
                    int rand = battle.Random(10);
                    if (rand < 2)
                    {
                        // 20% chance: heal target 1/4 HP (use Infiltrates as marker)
                        move.BasePower = 0;
                        move.Infiltrates = true;
                    }
                    else if (rand < 6)
                    {
                        // 40% chance: 40 BP
                        move.BasePower = 40;
                    }
                    else if (rand < 9)
                    {
                        // 30% chance: 80 BP
                        move.BasePower = 80;
                    }
                    else
                    {
                        // 10% chance: 120 BP
                        move.BasePower = 120;
                    }
                }),
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    // If Infiltrates is true, this is a heal roll - heal the target
                    if (move.Infiltrates == true)
                    {
                        int healAmount = target.BaseMaxHp / 4;
                        battle.Heal(healAmount, target, source);
                        return new VoidReturn();
                    }

                    return null;
                }),
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
                    if (source.Volatiles.TryGetValue(ConditionId.Stall,
                            out EffectState? stallState))
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
                OnBasePower = new OnBasePowerEventInfo((battle, _, _, _, _) =>
                {
                    if (battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        battle.Debug("psyblade electric terrain boost");
                        return battle.ChainModify(1.5);
                    }

                    return new VoidReturn();
                }),
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
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    // Copy all boosts from target to source
                    source.Boosts.Atk = target.Boosts.Atk;
                    source.Boosts.Def = target.Boosts.Def;
                    source.Boosts.SpA = target.Boosts.SpA;
                    source.Boosts.SpD = target.Boosts.SpD;
                    source.Boosts.Spe = target.Boosts.Spe;
                    source.Boosts.Accuracy = target.Boosts.Accuracy;
                    source.Boosts.Evasion = target.Boosts.Evasion;

                    // Also copy certain volatiles (Focus Energy, Dragon Cheer)
                    var volatilesToCopy = new[]
                        { ConditionId.FocusEnergy, ConditionId.DragonCheer };
                    foreach (ConditionId volatileId in volatilesToCopy)
                    {
                        source.RemoveVolatile(battle.Library.Conditions[volatileId]);
                    }

                    foreach (ConditionId volatileId in volatilesToCopy)
                    {
                        if (target.Volatiles.ContainsKey(volatileId))
                        {
                            source.AddVolatile(volatileId);
                            // Copy special properties
                            if (volatileId == ConditionId.DragonCheer &&
                                target.Volatiles.TryGetValue(volatileId,
                                    out EffectState? targetState) &&
                                source.Volatiles.TryGetValue(volatileId,
                                    out EffectState? sourceState))
                            {
                                sourceState.HasDragonType = targetState.HasDragonType;
                            }
                        }
                    }

                    battle.Add("-copyboost", source, target, "[from] move: Psych Up");
                    return new VoidReturn();
                }),
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
                OnTryHit = new OnTryHitEventInfo((_, target, _, _) =>
                {
                    // Remove screens before hitting (will shatter screens through sub)
                    target.Side.RemoveSideCondition(ConditionId.Reflect);
                    target.Side.RemoveSideCondition(ConditionId.LightScreen);
                    target.Side.RemoveSideCondition(ConditionId.AuroraVeil);
                    // TS has no return (undefined) - continue with the move
                    return new VoidReturn();
                }),
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
                // Note: TypeScript applies 'healblock' volatile, but HealBlock is not in Gen 9 VGC.
                // This implementation uses PsychicNoise as a simplified placeholder for the heal-blocking effect.
                // In Gen 9, Psychic Noise prevents the target from healing for 2 turns.
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    VolatileStatus = ConditionId.PsychicNoise,
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
                Terrain = ConditionId.PsychicTerrain,
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
                OnHit = new OnHitEventInfo((battle, target, _, _) =>
                {
                    // Fails in singles
                    if (battle.ActivePerHalf == 1)
                    {
                        return false;
                    }

                    // Get the target's queued move action
                    MoveAction? action = battle.Queue.WillMove(target);
                    if (action == null)
                    {
                        return false;
                    }

                    // Find the action in the queue and replace it with one that has order 201
                    int index = battle.Queue.List.IndexOf(action);
                    if (index == -1)
                    {
                        return false;
                    }

                    // Create a new action with order 201 (makes it move last)
                    MoveAction newAction = action with { Order = 201 };
                    battle.Queue.List[index] = newAction;

                    battle.Add("-activate", target, "move: Quash");
                    return new VoidReturn();
                }),
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
                OnTry = new OnTryEventInfo((battle, _, _, _) => battle.Queue.WillAct() is not null
                    ? new VoidReturn()
                    : false),
                OnHitSide = new OnHitSideEventInfo((_, _, source, _) =>
                {
                    source.AddVolatile(ConditionId.Stall);
                    return true;
                }),
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
                Boosts = new SparseBoostsTable { SpA = 1, SpD = 1, Spe = 1 },
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
                BasePowerCallback = new BasePowerCallbackEventInfo((_, source, _, _) =>
                    Math.Min(350, 50 + 50 * source.TimesAttacked)),
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
                OnTry = new OnTryEventInfo((battle, _, _, _) => battle.ActivePerHalf > 1
                    ? new VoidReturn()
                    : false),
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
                OnTryHit = new OnTryHitEventInfo((_, target, _, _) =>
                {
                    // Remove screens before hitting (will shatter screens through sub)
                    target.Side.RemoveSideCondition(ConditionId.Reflect);
                    target.Side.RemoveSideCondition(ConditionId.LightScreen);
                    target.Side.RemoveSideCondition(ConditionId.AuroraVeil);
                    return null; // Continue with the move
                }),
                OnModifyType = new OnModifyTypeEventInfo((_, move, source, _) =>
                {
                    // Change type based on Tauros form
                    FormeId formeName = source.Species.Forme;
                    if (formeName == FormeId.PaldeaCombat)
                    {
                        move.Type = MoveType.Fighting;
                    }
                    else if (formeName == FormeId.PaldeaBlaze)
                    {
                        move.Type = MoveType.Fire;
                    }
                    else if (formeName == FormeId.PaldeaAqua)
                    {
                        move.Type = MoveType.Water;
                    }
                }),
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
                OnAfterHit = new OnAfterHitEventInfo((battle, _, pokemon, move) =>
                {
                    // Skip if move has Sheer Force boost (suppresses secondary effects)
                    if (move.HasSheerForce ?? false) return BoolVoidUnion.FromVoid();

                    // Remove Leech Seed from user
                    if (pokemon.Hp > 0 &&
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.LeechSeed]))
                    {
                        battle.Add("-end", pokemon, "Leech Seed", "[from] move: Rapid Spin",
                            $"[of] {pokemon}");
                    }

                    // Remove hazards from user's side
                    // Note: G-Max Steelsurge is not in Gen 9 VGC
                    var sideConditions = new[]
                    {
                        ConditionId.Spikes, ConditionId.ToxicSpikes, ConditionId.StealthRock,
                        ConditionId.StickyWeb
                    };
                    foreach (ConditionId condition in sideConditions)
                    {
                        if (pokemon.Hp > 0 && pokemon.Side.RemoveSideCondition(condition))
                        {
                            battle.Add("-sideend", pokemon.Side,
                                battle.Library.Conditions[condition].Name,
                                "[from] move: Rapid Spin", $"[of] {pokemon}");
                        }
                    }

                    // Remove partial trapping from user
                    if (pokemon.Hp > 0 &&
                        pokemon.Volatiles.ContainsKey(ConditionId.PartiallyTrapped))
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.PartiallyTrapped]);
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnAfterSubDamage = new OnAfterSubDamageEventInfo((battle, _, _, pokemon, move) =>
                {
                    // Same logic as OnAfterHit - applies even when hitting a substitute
                    if (move.HasSheerForce ?? false) return;

                    if (pokemon.Hp > 0 &&
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.LeechSeed]))
                    {
                        battle.Add("-end", pokemon, "Leech Seed", "[from] move: Rapid Spin",
                            $"[of] {pokemon}");
                    }

                    var sideConditions = new[]
                    {
                        ConditionId.Spikes, ConditionId.ToxicSpikes, ConditionId.StealthRock,
                        ConditionId.StickyWeb
                    };
                    foreach (ConditionId condition in sideConditions)
                    {
                        if (pokemon.Hp > 0 && pokemon.Side.RemoveSideCondition(condition))
                        {
                            battle.Add("-sideend", pokemon.Side,
                                battle.Library.Conditions[condition].Name,
                                "[from] move: Rapid Spin", $"[of] {pokemon}");
                        }
                    }

                    if (pokemon.Hp > 0 &&
                        pokemon.Volatiles.ContainsKey(ConditionId.PartiallyTrapped))
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.PartiallyTrapped]);
                    }
                }),
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
            [MoveId.Recharge] = new()
            {
                Id = MoveId.Recharge,
                Num = 0,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Recharge",
                BasePp = 1,
                Priority = 0,
                Flags = new MoveFlags(),
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Recycle] = new()
            {
                Id = MoveId.Recycle,
                Num = 278,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Recycle",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Metronome = true },
                Target = MoveTarget.Self,
                Type = MoveType.Normal,
            },
            [MoveId.Rest] = new()
            {
                Id = MoveId.Rest,
                Num = 156,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Rest",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Heal = true, Metronome = true },
                Target = MoveTarget.Self,
                Type = MoveType.Psychic,
            },
            [MoveId.Retaliate] = new()
            {
                Id = MoveId.Retaliate,
                Num = 514,
                Accuracy = 100,
                BasePower = 70,
                Category = MoveCategory.Physical,
                Name = "Retaliate",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, source, _, _) =>
                {
                    if (source.Side.FaintedLastTurn != null)
                    {
                        battle.Debug("Boosted for a faint last turn");
                        return battle.ChainModify(2);
                    }

                    return basePower;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
            },
            [MoveId.Revenge] = new()
            {
                Id = MoveId.Revenge,
                Num = 279,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Revenge",
                BasePp = 10,
                Priority = -4,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.Reversal] = new()
            {
                Id = MoveId.Reversal,
                Num = 179,
                Accuracy = 100,
                BasePower = 0,
                Category = MoveCategory.Physical,
                Name = "Reversal",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                    { Contact = true, Protect = true, Mirror = true, Metronome = true },
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, source, _, _) =>
                {
                    int ratio = Math.Max((int)Math.Floor(source.Hp * 48.0 / source.MaxHp), 1);
                    int bp;
                    if (ratio < 2)
                    {
                        bp = 200;
                    }
                    else if (ratio < 5)
                    {
                        bp = 150;
                    }
                    else if (ratio < 10)
                    {
                        bp = 100;
                    }
                    else if (ratio < 17)
                    {
                        bp = 80;
                    }
                    else if (ratio < 33)
                    {
                        bp = 40;
                    }
                    else
                    {
                        bp = 20;
                    }

                    battle.Debug($"BP: {bp}");
                    return bp;
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Fighting,
            },
            [MoveId.RevivalBlessing] = new()
            {
                Id = MoveId.RevivalBlessing,
                Num = 863,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Revival Blessing",
                BasePp = 1,
                Priority = 0,
                Flags = new MoveFlags { Heal = true },
                Target = MoveTarget.Self,
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
                OnHit = new OnHitEventInfo((battle, target, source, _) =>
                {
                    // Arceus and Silvally cannot change types this way
                    if (source.Species.BaseSpecies == SpecieId.Arceus ||
                        source.Species.BaseSpecies == SpecieId.Silvally)
                    {
                        return false;
                    }

                    // Terastallized Pokemon cannot change types
                    if (source.Terastallized != null)
                    {
                        return false;
                    }

                    // Get target's types
                    var newTypes = target.GetTypes(true).Where(t => t != PokemonType.Unknown)
                        .ToList();
                    if (newTypes.Count == 0)
                    {
                        // If no types, check for added type, otherwise fail
                        if (target.AddedType != null)
                        {
                            newTypes.Add(PokemonType.Normal);
                        }
                        else
                        {
                            return false;
                        }
                    }

                    battle.Add("-start", source, "typechange", "[from] move: Reflect Type",
                        $"[of] {target}");
                    source.SetType(newTypes.ToArray());
                    source.AddedType = target.AddedType;
                    return new VoidReturn();
                }),
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
                OnAfterMoveSecondarySelf =
                    new OnAfterMoveSecondarySelfEventInfo((_, source, _, move) =>
                    {
                        // Change Meloetta forme if applicable
                        if (source.Species.BaseSpecies == SpecieId.Meloetta && !source.Transformed)
                        {
                            // Toggle between Aria and Pirouette formes
                            SpecieId newSpecies = source.Species.Forme == FormeId.Pirouette
                                ? SpecieId.Meloetta // Aria is the base form
                                : SpecieId.MeloettaPirouette;
                            source.FormeChange(newSpecies, move, message: "[msg]");
                        }

                        return new VoidReturn();
                    }),
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
                OnModifyType = new OnModifyTypeEventInfo((_, move, source, _) =>
                {
                    // Change to user's primary type
                    var types = source.GetTypes();
                    PokemonType primaryType = types.Length > 0 ? types[0] : PokemonType.Normal;
                    if (primaryType == PokemonType.Unknown && types.Length > 1)
                    {
                        primaryType = types[1];
                    }

                    move.Type = (MoveType)primaryType;
                }),
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
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, target, _) =>
                {
                    // Double power on grounded targets in Electric Terrain
                    if (battle.Field.IsTerrain(ConditionId.ElectricTerrain, target) &&
                        target.IsGrounded() == true)
                    {
                        return 140; // Doubled base power
                    }

                    return 70; // Normal base power
                }),
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
                Boosts = new SparseBoostsTable { Spe = 2 },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Rock,
            },
            [MoveId.RockSlide] = new()
            {
                Id = MoveId.RockSlide,
                Num = 157,
                Accuracy = 90,
                BasePower = 75,
                Category = MoveCategory.Physical,
                Name = "Rock Slide",
                BasePp = 10,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 30,
                    VolatileStatus = ConditionId.Flinch,
                },
                Target = MoveTarget.AllAdjacentFoes,
                Type = MoveType.Rock,
            },
            [MoveId.RockTomb] = new()
            {
                Id = MoveId.RockTomb,
                Num = 317,
                Accuracy = 95,
                BasePower = 60,
                Category = MoveCategory.Physical,
                Name = "Rock Tomb",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags { Protect = true, Mirror = true, Metronome = true },
                Secondary = new SecondaryEffect
                {
                    Chance = 100,
                    Boosts = new SparseBoostsTable { Spe = -1 },
                },
                Target = MoveTarget.Normal,
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
                OnTryHit = new OnTryHitEventInfo((_, target, source, _) =>
                {
                    // Fail if same ability
                    if (target.Ability == source.Ability)
                    {
                        return false;
                    }

                    // Fail if target has FailRolePlay flag or source has CantSuppress flag
                    Ability targetAbility = target.GetAbility();
                    Ability sourceAbility = source.GetAbility();
                    if (targetAbility.Flags.FailRolePlay == true ||
                        sourceAbility.Flags.CantSuppress == true)
                    {
                        return false;
                    }

                    return null;
                }),
                OnHit = new OnHitEventInfo((_, target, source, _) =>
                {
                    // Copy target's ability
                    if (target.Ability == AbilityId.None)
                    {
                        return false;
                    }

                    AbilityIdFalseUnion? oldAbility = source.SetAbility(target.Ability, target);
                    if (oldAbility is FalseAbilityIdFalseUnion)
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
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
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Rock,
            },
            [MoveId.Roost] = new()
            {
                Id = MoveId.Roost,
                Num = 355,
                Accuracy = IntTrueUnion.FromTrue(),
                BasePower = 0,
                Category = MoveCategory.Status,
                Name = "Roost",
                BasePp = 5,
                Priority = 0,
                Flags = new MoveFlags { Snatch = true, Heal = true, Metronome = true },
                Heal = [1, 2],
                Self = new SecondaryEffect
                {
                    VolatileStatus = ConditionId.Roost,
                },
                Secondary = null,
                Target = MoveTarget.Self,
                Type = MoveType.Flying,
            },
            [MoveId.Round] = new()
            {
                Id = MoveId.Round,
                Num = 496,
                Accuracy = 100,
                BasePower = 60,
                Category = MoveCategory.Special,
                Name = "Round",
                BasePp = 15,
                Priority = 0,
                Flags = new MoveFlags
                {
                    Protect = true, Mirror = true, Sound = true, BypassSub = true, Metronome = true
                },
                BasePowerCallback = new BasePowerCallbackEventInfo((battle, _, _, move) =>
                {
                    // Double power if triggered by ally's Round
                    if (move.SourceEffect is MoveEffectStateId { MoveId: MoveId.Round })
                    {
                        battle.Debug("BP doubled");
                        return 120; // move.basePower * 2
                    }

                    return 60;
                }),
                OnTry = new OnTryEventInfo((battle, _, _, move) =>
                {
                    // Prioritize ally's Round actions so they go immediately after this one
                    foreach (IAction action in battle.Queue.List)
                    {
                        if (action is not MoveAction moveAction) continue;
                        if (moveAction.Pokemon == null || moveAction.Move == null) continue;
                        if (moveAction.Move.Id == MoveId.Round)
                        {
                            battle.Queue.PrioritizeAction(moveAction, move);
                            return new VoidReturn();
                        }
                    }

                    return new VoidReturn();
                }),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Normal,
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
                DamageCallback =
                    new DamageCallbackEventInfo((_, _, target, _) => Math.Max(1, target.Hp / 2)),
                Secondary = null,
                Target = MoveTarget.Normal,
                Type = MoveType.Dark,
            },
        };
    }
}