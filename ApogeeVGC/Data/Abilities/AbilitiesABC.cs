using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesAbc()
    {
        return new Dictionary<AbilityId, Ability>
        {
            [AbilityId.Adaptability] = new()
            {
                Id = AbilityId.Adaptability,
                Name = "Adaptability",
                Num = 91,
                Rating = 4.0,
                OnModifyStab = new OnModifyStabEventInfo((_, stab, source, _, move) =>
                {
                    if ((move.ForceStab ?? false) ||
                        source.HasType(move.Type.ConvertToPokemonType()))
                    {
                        return stab == 2 ? 2.25 : 2.0;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Aerilate] = new()
            {
                Id = AbilityId.Aerilate,
                Name = "Aerilate",
                Num = 184,
                Rating = 4.0,
                //OnModifyTypePriority = -1,
                OnModifyType = new OnModifyTypeEventInfo((battle, move, pokemon, _) =>
                    {
                        // List of moves that should not be affected by type-changing abilities
                        MoveId[] noModifyType =
                        [
                            MoveId.Judgment,
                            MoveId.RevelationDance, MoveId.TerrainPulse,
                            MoveId.WeatherBall,
                        ];

                        // Change Normal-type moves to Flying
                        if (move.Type == MoveType.Normal &&
                            !noModifyType.Contains(move.Id) &&
                            !(move.Name == "Tera Blast" && pokemon.Terastallized != null) &&
                            move.Category != MoveCategory.Status)
                        {
                            move.Type = MoveType.Flying;
                            move.TypeChangerBoosted = battle.Effect;
                        }
                    },
                    -1),
                //OnBasePowerPriority = 23,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                    {
                        if (move.TypeChangerBoosted == battle.Effect)
                        {
                            battle.ChainModify([4915, 4096]);
                            return battle.FinalModify(basePower);
                        }

                        return basePower;
                    },
                    23),
            },
            [AbilityId.Aftermath] = new()
            {
                Id = AbilityId.Aftermath,
                Name = "Aftermath",
                Num = 106,
                Rating = 2.0,
                //OnDamagingHitOrder = 1,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                    {
                        if (target.Hp == 0 &&
                            battle.CheckMoveMakesContact(move, source, target, true))
                        {
                            battle.Damage(source.BaseMaxHp / 4, source, target);
                        }
                    },
                    1),
            },
            [AbilityId.AirLock] = new()
            {
                Id = AbilityId.AirLock,
                Name = "Air Lock",
                Num = 76,
                Rating = 1.5,
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    // Air Lock does not activate when Skill Swapped or when Neutralizing Gas leaves the field
                    battle.Add("-ability", pokemon, "Air Lock");
                    // Call onStart
                    battle.SingleEvent(EventId.Start, battle.Effect, battle.EffectState, pokemon);
                }),
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    pokemon.AbilityState.Ending = false; // Clear the ending flag
                    battle.EachEvent(EventId.WeatherChange, battle.Effect);
                }),
                OnEnd = new OnEndEventInfo((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                    psfp.Pokemon.AbilityState.Ending = true;
                    battle.EachEvent(EventId.WeatherChange, battle.Effect);
                }),
                SuppressWeather = true,
            },
            [AbilityId.Analytic] = new()
            {
                Id = AbilityId.Analytic,
                Name = "Analytic",
                Num = 148,
                Rating = 2.5,
                //OnBasePowerPriority = 21,
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, pokemon, _, _) =>
                    {
                        bool boosted = true;
                        foreach (Pokemon target in battle.GetAllActive())
                        {
                            if (target == pokemon) continue;
                            if (battle.Queue.WillMove(target) != null)
                            {
                                boosted = false;
                                break;
                            }
                        }

                        if (boosted)
                        {
                            battle.Debug("Analytic boost");
                            battle.ChainModify([5325, 4096]);
                            return battle.FinalModify(basePower);
                        }

                        return basePower;
                    },
                    21),
            },
            [AbilityId.AngerPoint] = new()
            {
                Id = AbilityId.AngerPoint,
                Name = "Anger Point",
                Num = 83,
                Rating = 1.0,
                OnHit = new OnHitEventInfo((battle, target, _, move) =>
                {
                    if (target.Hp == 0) return new VoidReturn();
                    if (move is not { }) return new VoidReturn();
                    if (!target.GetMoveHitData(move).Crit) return new VoidReturn();

                    battle.Boost(new SparseBoostsTable { Atk = 12 }, target, target);
                    return new VoidReturn();
                }),
            },
            [AbilityId.AngerShell] = new()
            {
                Id = AbilityId.AngerShell,
                Name = "Anger Shell",
                Num = 271,
                Rating = 3.0,
                OnDamage = new OnDamageEventInfo((battle, _, _, source, effect) =>
                {
                    if (effect is not ActiveMove move)
                    {
                        battle.EffectState.CheckedAngerShell = true;
                        return new VoidReturn();
                    }

                    if (move.MultiHit != null ||
                        (move.HasSheerForce == true && source.HasAbility(AbilityId.SheerForce)))
                    {
                        battle.EffectState.CheckedAngerShell = true;
                    }
                    else
                    {
                        battle.EffectState.CheckedAngerShell = false;
                    }

                    return new VoidReturn();
                }),
                OnTryEatItem = new OnTryEatItemEventInfo(OnTryEatItem.FromFunc((battle, item, _) =>
                {
                    ItemId[] healingItems =
                    [
                        ItemId.AguavBerry, ItemId.EnigmaBerry, ItemId.FigyBerry, ItemId.IapapaBerry,
                        ItemId.MagoBerry, ItemId.SitrusBerry, ItemId.WikiBerry, ItemId.OranBerry,
                        ItemId.BerryJuice,
                    ];

                    return healingItems.Contains(item.Id)
                        ? BoolVoidUnion.FromBool(battle.EffectState.CheckedAngerShell ?? true)
                        : BoolVoidUnion.FromBool(true);
                })),
                OnAfterMoveSecondary =
                    new OnAfterMoveSecondaryEventInfo((battle, target, source, move) =>
                    {
                        battle.EffectState.CheckedAngerShell = true;
                        if (source is null || source == target || target.Hp == 0) return;
                        if (move.TotalDamage is not IntIntFalseUnion totalDamage) return;

                        Attacker? lastAttackedBy = target.GetLastAttackedBy();
                        if (lastAttackedBy == null) return;

                        int damage = move.MultiHit != null && move.SmartTarget != true
                            ? totalDamage.Value
                            : lastAttackedBy.Damage;

                        if (target.Hp <= target.MaxHp / 2 && target.Hp + damage > target.MaxHp / 2)
                        {
                            battle.Boost(new SparseBoostsTable
                            {
                                Atk = 1,
                                SpA = 1,
                                Spe = 1,
                                Def = -1,
                                SpD = -1
                            }, target, target);
                        }
                    }),
            },
            [AbilityId.Anticipation] = new()
            {
                Id = AbilityId.Anticipation,
                Name = "Anticipation",
                Num = 107,
                Rating = 0.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Foes().Any(target =>
                            (from moveSlot in target.MoveSlots
                                select battle.Library.Moves[moveSlot.Id] into move
                                where move.Category != MoveCategory.Status
                                let moveType = move.Type.ConvertToPokemonType()
                                where (battle.Dex.GetImmunity(moveType.ConvertToMoveType(), pokemon) &&
                                battle.Dex.GetEffectiveness(moveType.ConvertToMoveType(), pokemon)
                                    .ToModifier() > 0) ||
                            move.Ohko != null select move).Any()))
                    {
                        battle.Add("-ability", pokemon, "Anticipation");
                    }
                }),
            },
            [AbilityId.ArenaTrap] = new()
            {
                Id = AbilityId.ArenaTrap,
                Name = "Arena Trap",
                Num = 71,
                Rating = 5.0,
                OnFoeTrapPokemon = new OnFoeTrapPokemonEventInfo((battle, pokemon) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder
                        })
                        return;
                    if (!pokemon.IsAdjacent(abilityHolder)) return;
                    if (pokemon.IsGrounded() == true)
                    {
                        pokemon.TryTrap(true);
                    }
                }),
                OnFoeMaybeTrapPokemon =
                    new OnFoeMaybeTrapPokemonEventInfo((battle, pokemon, source) =>
                    {
                        if (source == null && battle.EffectState.Target is PokemonEffectStateTarget
                            {
                                Pokemon: var holder
                            })
                        {
                            source = holder;
                        }

                        if (source == null || !pokemon.IsAdjacent(source)) return;

                        // If type is unknown, negate immunity check
                        if (pokemon.IsGrounded(!pokemon.KnownType) == true)
                        {
                            pokemon.MaybeTrapped = true;
                        }
                    }),
            },
            [AbilityId.ArmorTail] = new()
            {
                Id = AbilityId.ArmorTail,
                Name = "Armor Tail",
                Num = 296,
                Rating = 2.5,
                Flags = new AbilityFlags { Breakable = true },
                OnFoeTryMove = new OnFoeTryMoveEventInfo((battle, target, source, move) =>
                {
                    MoveId[] targetAllExceptions = [MoveId.PerishSong, MoveId.FlowerShield, MoveId.Rototiller];
                    if (move.Target == MoveTarget.FoeSide ||
                        (move.Target == MoveTarget.All &&
                         !targetAllExceptions.Contains(move.Id)))
                    {
                        return new VoidReturn();
                    }

                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var armorTailHolder
                        })
                        return new VoidReturn();

                    if ((source.IsAlly(armorTailHolder) || move.Target == MoveTarget.All) &&
                        move.Priority > 0.1)
                    {
                        battle.AttrLastMove("[still]");
                        battle.Add("cant", armorTailHolder, "ability: Armor Tail", move.Name,
                            $"[of] {target}");
                        return false;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.AromaVeil] = new()
            {
                Id = AbilityId.AromaVeil,
                Name = "Aroma Veil",
                Num = 165,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                OnAllyTryAddVolatile =
                    new OnAllyTryAddVolatileEventInfo((battle, status, target, _, effect) =>
                    {
                        ConditionId[] blockedVolatiles =
                        [
                            ConditionId.Attract,
                            ConditionId.Disable,
                            ConditionId.Encore,
                            ConditionId.HealBlock,
                            ConditionId.Taunt,
                            ConditionId.Torment,
                        ];

                        if (blockedVolatiles.Contains(status.Id))
                        {
                            if (effect.EffectType == EffectType.Move)
                            {
                                if (battle.EffectState.Target is PokemonEffectStateTarget
                                    {
                                        Pokemon: var effectHolder
                                    })
                                {
                                    battle.Add("-block", target, "ability: Aroma Veil",
                                        $"[of] {effectHolder}");
                                }
                            }

                            return null;
                        }

                        return new VoidReturn();
                    }),
            },
            [AbilityId.AsOneGlastrier] = new()
            {
                Id = AbilityId.AsOneGlastrier,
                Name = "As One (Glastrier)",
                Num = 266,
                Rating = 3.5,
                //OnSwitchInPriority = 1,
                OnSwitchIn = new OnSwitchInEventInfo((_, _) => { },
                    1
                ),
                OnStart = new OnStartEventInfo(
                    (battle, pokemon) =>
                    {
                        if (battle.EffectState.Unnerved is true) return;
                        if (battle.DisplayUi)
                        {
                            //UiGenerator.PrintAbilityEvent(pokemon, "As One");
                            //UiGenerator.PrintAbilityEvent(pokemon, _library.Abilities[AbilityId.Unnerve]);

                            battle.Add("-ability", pokemon, "As One");
                            battle.Add("-ability", pokemon, "Unnerve");
                        }

                        battle.EffectState.Unnerved = true;
                    },
                    1
                ),
                OnEnd = new OnEndEventInfo((battle, _) => { battle.EffectState.Unnerved = false; }),
                OnFoeTryEatItem = new OnFoeTryEatItemEventInfo(
                    OnTryEatItem.FromFunc((battle, _, _) =>
                        BoolVoidUnion.FromBool(!(battle.EffectState.Unnerved ?? false)))),
                OnSourceAfterFaint =
                    new OnSourceAfterFaintEventInfo((battle, length, _, source, effect) =>
                    {
                        if (effect is null || effect.EffectType != EffectType.Move) return;

                        battle.Boost(new SparseBoostsTable { Atk = length }, source, source,
                            _library.Abilities[AbilityId.ChillingNeigh]);
                    }),
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                },
            },
            [AbilityId.AsOneSpectrier] = new()
            {
                Id = AbilityId.AsOneSpectrier,
                Name = "As One (Spectrier)",
                Num = 267,
                Rating = 3.5,
                //OnSwitchInPriority = 1,
                OnSwitchIn = new OnSwitchInEventInfo((_, _) => { },
                    1
                ),
                OnStart = new OnStartEventInfo(
                    (battle, pokemon) =>
                    {
                        if (battle.EffectState.Unnerved is true) return;
                        if (battle.DisplayUi)
                        {
                            battle.Add("-ability", pokemon, "As One");
                            battle.Add("-ability", pokemon, "Unnerve");
                        }

                        battle.EffectState.Unnerved = true;
                    },
                    1
                ),
                OnEnd = new OnEndEventInfo((battle, _) => { battle.EffectState.Unnerved = false; }),
                OnFoeTryEatItem = new OnFoeTryEatItemEventInfo(
                    OnTryEatItem.FromFunc((battle, _, _) =>
                        BoolVoidUnion.FromBool(!(battle.EffectState.Unnerved ?? false)))),
                OnSourceAfterFaint =
                    new OnSourceAfterFaintEventInfo((battle, length, _, source, effect) =>
                    {
                        if (effect is null || effect.EffectType != EffectType.Move) return;

                        battle.Boost(new SparseBoostsTable { SpA = length }, source, source,
                            _library.Abilities[AbilityId.GrimNeigh]);
                    }),
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                },
            },
            [AbilityId.AuraBreak] = new()
            {
                Id = AbilityId.AuraBreak,
                Name = "Aura Break",
                Num = 188,
                Rating = 1.0,
                Flags = new AbilityFlags { Breakable = true },
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    battle.Add("-ability", pokemon, "Aura Break");
                }),
                OnAnyTryPrimaryHit = new OnAnyTryPrimaryHitEventInfo((_, target, source, move) =>
                {
                    if (target == source || move.Category == MoveCategory.Status) return new VoidReturn();
                    move.HasAuraBreak = true;
                    return new VoidReturn();
                }),
            },
            [AbilityId.BadDreams] = new()
            {
                Id = AbilityId.BadDreams,
                Name = "Bad Dreams",
                Num = 123,
                Rating = 1.5,
                // OnResidualOrder = 28, OnResidualSubOrder = 2
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.Hp == 0) return;
                    foreach (Pokemon target in pokemon.Foes())
                    {
                        if (target.Status == ConditionId.Sleep ||
                            target.HasAbility(AbilityId.Comatose))
                        {
                            battle.Damage(target.BaseMaxHp / 8, target, pokemon);
                        }
                    }
                }, order: 28, subOrder: 2),
            },
            [AbilityId.BallFetch] = new()
            {
                Id = AbilityId.BallFetch,
                Name = "Ball Fetch",
                Num = 237,
                Rating = 0.0,
                // No in-battle effect
            },
            [AbilityId.Battery] = new()
            {
                Id = AbilityId.Battery,
                Name = "Battery",
                Num = 217,
                Rating = 0.0,
                // OnAllyBasePowerPriority = 22
                OnAllyBasePower = new OnAllyBasePowerEventInfo(
                    (battle, basePower, attacker, _, move) =>
                    {
                        if (battle.EffectState.Target is not PokemonEffectStateTarget
                            {
                                Pokemon: var effectHolder
                            })
                            return basePower;
                        if (attacker != effectHolder && move.Category == MoveCategory.Special)
                        {
                            battle.Debug("Battery boost");
                            battle.ChainModify([5325, 4096]);
                            return battle.FinalModify(basePower);
                        }

                        return basePower;
                    }, 22),
            },
            [AbilityId.BattleArmor] = new()
            {
                Id = AbilityId.BattleArmor,
                Name = "Battle Armor",
                Num = 4,
                Rating = 1.0,
                Flags = new AbilityFlags { Breakable = true },
                OnCriticalHit = new OnCriticalHitEventInfo(false),
            },
            [AbilityId.BattleBond] = new()
            {
                Id = AbilityId.BattleBond,
                Name = "Battle Bond",
                Num = 210,
                Rating = 3.5,
                OnSourceAfterFaint =
                    new OnSourceAfterFaintEventInfo((battle, _, _, source, effect) =>
                    {
                        if (source.BondTriggered) return;
                        if (effect.EffectType != EffectType.Move) return;
                        if (source.Species.Id == SpecieId.GreninjaBond && source
                                is { Hp: > 0, Transformed: false } && source.Side.FoePokemonLeft() > 0)
                        {
                            battle.Boost(new SparseBoostsTable { Atk = 1, SpA = 1, Spe = 1 },
                                source, source, battle.Effect);
                            battle.Add("-activate", source, "ability: Battle Bond");
                            source.BondTriggered = true;
                        }
                    }),
                //OnModifyMovePriority = -1,
                OnModifyMove = new OnModifyMoveEventInfo((_, move, attacker, _) =>
                {
                    if (move.Id == MoveId.WaterShuriken &&
                        attacker.Species.Id == SpecieId.GreninjaAsh &&
                        !attacker.Transformed)
                    {
                        move.MultiHit = 3;
                    }
                }, -1),
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                },
            },
            [AbilityId.BeadsOfRuin] = new()
            {
                Id = AbilityId.BeadsOfRuin,
                Name = "Beads of Ruin",
                Num = 284,
                Rating = 4.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (battle.SuppressingAbility(pokemon)) return;
                    battle.Add("-ability", pokemon, "Beads of Ruin");
                }),
                OnAnyModifySpD = new OnAnyModifySpDEventInfo((battle, spd, target, _, move) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder
                        })
                        return spd;
                    if (target.HasAbility(AbilityId.BeadsOfRuin)) return spd;
                    if (move.RuinedSpD?.HasAbility(AbilityId.BeadsOfRuin) != true)
                        move.RuinedSpD = abilityHolder;
                    if (move.RuinedSpD != abilityHolder) return spd;
                    battle.Debug("Beads of Ruin SpD drop");
                    battle.ChainModify(0.75);
                    return battle.FinalModify(spd);
                }),
            },
            [AbilityId.BeastBoost] = new()
            {
                Id = AbilityId.BeastBoost,
                Name = "Beast Boost",
                Num = 224,
                Rating = 3.5,
                OnSourceAfterFaint =
                    new OnSourceAfterFaintEventInfo((battle, length, _, source, effect) =>
                    {
                        if (effect is null || effect.EffectType != EffectType.Move) return;
                        StatIdExceptHp bestStat = source.GetBestStat(true, true);
                        var boostTable = new SparseBoostsTable();
                        boostTable.SetBoost(bestStat.ConvertToBoostId(), length);
                        battle.Boost(boostTable, source);
                    }),
            },
            [AbilityId.Berserk] = new()
            {
                Id = AbilityId.Berserk,
                Name = "Berserk",
                Num = 201,
                Rating = 2.0,
                OnDamage = new OnDamageEventInfo((battle, damage, _, source, effect) =>
                {
                    if (effect.EffectType == EffectType.Move &&
                        effect is Move { MultiHit: null } move &&
                        !(move.HasSheerForce == true && source != null && source.HasAbility(AbilityId.SheerForce)))
                    {
                        battle.EffectState.CheckedBerserk = false;
                    }
                    else
                    {
                        battle.EffectState.CheckedBerserk = true;
                    }
                    return damage;
                }),
                OnTryEatItem = new OnTryEatItemEventInfo(OnTryEatItem.FromFunc((battle, item, _) =>
                {
                    var healingItems = new[]
                    {
                        ItemId.AguavBerry, ItemId.EnigmaBerry, ItemId.FigyBerry, ItemId.IapapaBerry,
                        ItemId.MagoBerry, ItemId.SitrusBerry, ItemId.WikiBerry, ItemId.OranBerry, ItemId.BerryJuice
                    };
                        if (healingItems.Contains(item.Id))
                        {
                            return BoolVoidUnion.FromBool(battle.EffectState.CheckedBerserk ?? false);
                        }
                        return BoolVoidUnion.FromBool(true);
                    })),
                    OnAfterMoveSecondary = new OnAfterMoveSecondaryEventInfo((battle, target, source, move) =>
                {
                    battle.EffectState.CheckedBerserk = true;
                    if (source == null || source == target || target.Hp == 0 || move.TotalDamage == null) return;

                    var lastAttackedBy = target.GetLastAttackedBy();
                    if (lastAttackedBy == null) return;

                    int damage = move.MultiHit != null && !(move.SmartTarget ?? false) && move.TotalDamage is IntIntFalseUnion totalDmg ? totalDmg.Value : lastAttackedBy.Damage;
                    if (target.Hp <= target.MaxHp / 2 && target.Hp + damage > target.MaxHp / 2)
                    {
                        battle.Boost(new SparseBoostsTable { SpA = 1 }, target, target);
                    }
                }),
            },
            [AbilityId.BigPecks] = new()
            {
                Id = AbilityId.BigPecks,
                Name = "Big Pecks",
                Num = 145,
                Rating = 0.5,
                Flags = new AbilityFlags { Breakable = true },
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, source, effect) =>
                {
                    if (source != null && target == source) return;
                    if (boost.Def is not null && boost.Def < 0)
                    {
                        boost.Def = null;
                        if (effect is not ActiveMove { Secondaries: not null } &&
                            effect is not Condition { Id: ConditionId.Octolock })
                        {
                            battle.Add("-fail", target, "unboost", "Defense",
                                "[from] ability: Big Pecks", $"[of] {target}");
                        }
                    }
                }),
            },
            [AbilityId.Blaze] = new()
            {
                Id = AbilityId.Blaze,
                Name = "Blaze",
                Num = 66,
                Rating = 2.0,
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Fire && attacker.Hp <= attacker.MaxHp / 3)
                    {
                        battle.Debug("Blaze boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Fire && attacker.Hp <= attacker.MaxHp / 3)
                    {
                        battle.Debug("Blaze boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.Bulletproof] = new()
            {
                Id = AbilityId.Bulletproof,
                Name = "Bulletproof",
                Num = 171,
                Rating = 3.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = new OnTryHitEventInfo((battle, pokemon, _, move) =>
                {
                    if (move.Flags.Bullet == true)
                    {
                        battle.Add("-immune", pokemon, "[from] ability: Bulletproof");
                        return null;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.CheekPouch] = new()
            {
                Id = AbilityId.CheekPouch,
                Name = "Cheek Pouch",
                Num = 167,
                Rating = 2.0,
                OnEatItem = new OnEatItemEventInfo((battle, _, pokemon, _, _) =>
                {
                    battle.Heal(pokemon.BaseMaxHp / 3, pokemon);
                }),
            },
            [AbilityId.ChillingNeigh] = new()
            {
                Id = AbilityId.ChillingNeigh,
                Name = "Chilling Neigh",
                Num = 264,
                Rating = 3.0,
                OnSourceAfterFaint =
                    new OnSourceAfterFaintEventInfo((battle, length, _, source, effect) =>
                    {
                        if (effect is null || effect.EffectType != EffectType.Move) return;
                        battle.Boost(new SparseBoostsTable { Atk = length }, source);
                    }),
            },
            [AbilityId.Chlorophyll] = new()
            {
                Id = AbilityId.Chlorophyll,
                Name = "Chlorophyll",
                Num = 34,
                Rating = 3.0,
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    ConditionId[] sunnyWeathers = [ConditionId.SunnyDay, ConditionId.DesolateLand];
                    if (sunnyWeathers.Contains(pokemon.EffectiveWeather()))
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spe);
                    }

                    return spe;
                }),
            },
            [AbilityId.ClearBody] = new()
            {
                Id = AbilityId.ClearBody,
                Name = "Clear Body",
                Num = 29,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, source, effect) =>
                {
                    if (source != null && target == source) return;
                    bool showMsg = false;

                    // Check and remove all negative boosts
                    if (boost.Atk is not null && boost.Atk < 0)
                    {
                        boost.Atk = null;
                        showMsg = true;
                    }

                    if (boost.Def is not null && boost.Def < 0)
                    {
                        boost.Def = null;
                        showMsg = true;
                    }

                    if (boost.SpA is not null && boost.SpA < 0)
                    {
                        boost.SpA = null;
                        showMsg = true;
                    }

                    if (boost.SpD is not null && boost.SpD < 0)
                    {
                        boost.SpD = null;
                        showMsg = true;
                    }

                    if (boost.Spe is not null && boost.Spe < 0)
                    {
                        boost.Spe = null;
                        showMsg = true;
                    }

                    if (boost.Accuracy is not null && boost.Accuracy < 0)
                    {
                        boost.Accuracy = null;
                        showMsg = true;
                    }

                    if (boost.Evasion is not null && boost.Evasion < 0)
                    {
                        boost.Evasion = null;
                        showMsg = true;
                    }

                    if (showMsg && effect is not ActiveMove { Secondaries: not null } &&
                        effect is not Condition { Id: ConditionId.Octolock })
                    {
                        battle.Add("-fail", target, "unboost", "[from] ability: Clear Body",
                            $"[of] {target}");
                    }
                }),
            },
            [AbilityId.CloudNine] = new()
            {
                Id = AbilityId.CloudNine,
                Name = "Cloud Nine",
                Num = 13,
                Rating = 1.5,
                SuppressWeather = true,
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    // Cloud Nine does not activate when Skill Swapped or when Neutralizing Gas leaves the field
                    battle.Add("-ability", pokemon, "Cloud Nine");
                    // Call onStart
                    battle.SingleEvent(EventId.Start, battle.Effect, battle.EffectState, pokemon);
                }),
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    pokemon.AbilityState.Ending = false; // Clear the ending flag
                    battle.EachEvent(EventId.WeatherChange, battle.Effect);
                }),
                OnEnd = new OnEndEventInfo((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                    psfp.Pokemon.AbilityState.Ending = true;
                    battle.EachEvent(EventId.WeatherChange, battle.Effect);
                }),
            },
            [AbilityId.ColorChange] = new()
            {
                Id = AbilityId.ColorChange,
                Name = "Color Change",
                Num = 16,
                Rating = 0.0,
                OnAfterMoveSecondary =
                    new OnAfterMoveSecondaryEventInfo((battle, target, _, move) =>
                    {
                        if (target.Hp == 0) return;
                        MoveType type = move.Type;
                        if (target.IsActive &&
                            move.EffectType == EffectType.Move &&
                            move.Category != MoveCategory.Status &&
                            type != MoveType.Unknown &&
                            !target.HasType(type.ConvertToPokemonType()))
                        {
                            if (!target.SetType(type.ConvertToPokemonType())) return;
                            battle.Add("-start", target, "typechange", type.ToString(),
                                "[from] ability: Color Change");

                            // Curse Glitch for doubles (when in position 1)
                            if (target.Side.Active.Count == 2 && target.Position == 1)
                            {
                                MoveAction? action = battle.Queue.WillMove(target);
                                if (action is { Move.Id: MoveId.Curse })
                                {
                                    action.TargetLoc = -1;
                                }
                            }
                        }
                    }),
            },
            [AbilityId.Comatose] = new()
            {
                Id = AbilityId.Comatose,
                Name = "Comatose",
                Num = 213,
                Rating = 4.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                },
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    battle.Add("-ability", pokemon, "Comatose");
                }),
                OnSetStatus = new OnSetStatusEventInfo((battle, _, target, _, effect) =>
                {
                    if (effect is ActiveMove { Status: not null })
                    {
                        battle.Add("-immune", target, "[from] ability: Comatose");
                    }

                    return false;
                }),
                // Permanent sleep "status" implemented in the relevant sleep-checking effects
            },
            [AbilityId.Commander] = new()
            {
                Id = AbilityId.Commander,
                Name = "Commander",
                Num = 279,
                Rating = 0.0, // Only useful in Doubles with Dondozo
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                },
                // OnAnySwitchInPriority = -2
                OnAnySwitchIn = new OnAnySwitchInEventInfo((battle, _) =>
                {
                    if (battle.EffectState.Target is PokemonEffectStateTarget
                        {
                            Pokemon: var pokemon
                        })
                    {
                        // Call onUpdate logic
                        CommanderUpdateLogic(battle, pokemon);
                    }
                }, -2),
                OnStart = new OnStartEventInfo(CommanderUpdateLogic),
                OnUpdate = new OnUpdateEventInfo(CommanderUpdateLogic),
            },
            [AbilityId.Competitive] = new()
            {
                Id = AbilityId.Competitive,
                Name = "Competitive",
                Num = 172,
                Rating = 2.5,
                OnAfterEachBoost =
                    new OnAfterEachBoostEventInfo((battle, boost, target, source, _) =>
                    {
                        if (source == null || target.IsAlly(source)) return;

                        bool statsLowered = boost.Atk is < 0 || boost.Def is < 0 || boost.SpA is < 0
                                            || boost.SpD is < 0 || boost.Spe is < 0 || boost.Accuracy is < 0
                                            || boost.Evasion is < 0;

                        if (statsLowered)
                        {
                            battle.Boost(new SparseBoostsTable { SpA = 2 }, target, target, null,
                                false, true);
                        }
                    }),
            },
            [AbilityId.CompoundEyes] = new()
            {
                Id = AbilityId.CompoundEyes,
                Name = "Compound Eyes",
                Num = 14,
                Rating = 3.0,
                // OnSourceModifyAccuracyPriority = -1
                OnSourceModifyAccuracy = new OnSourceModifyAccuracyEventInfo(
                    (battle, accuracy, _, _, _) =>
                    {
                        battle.Debug("compoundeyes - enhancing accuracy");
                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(accuracy);
                    }, -1),
            },
            [AbilityId.Contrary] = new()
            {
                Id = AbilityId.Contrary,
                Name = "Contrary",
                Num = 126,
                Rating = 4.5,
                Flags = new AbilityFlags { Breakable = true },
                OnChangeBoost = new OnChangeBoostEventInfo((_, boost, _, _, _) =>
                {
                    // Invert all boosts
                    if (boost.Atk is not null) boost.Atk *= -1;
                    if (boost.Def is not null) boost.Def *= -1;
                    if (boost.SpA is not null) boost.SpA *= -1;
                    if (boost.SpD is not null) boost.SpD *= -1;
                    if (boost.Spe is not null) boost.Spe *= -1;
                    if (boost.Accuracy is not null) boost.Accuracy *= -1;
                    if (boost.Evasion is not null) boost.Evasion *= -1;
                }),
            },
            [AbilityId.Corrosion] = new()
            {
                Id = AbilityId.Corrosion,
                Name = "Corrosion",
                Num = 212,
                Rating = 2.5,
                // Implemented in sim/pokemon.cs:Pokemon#SetStatus
            },
            [AbilityId.Costar] = new()
            {
                Id = AbilityId.Costar,
                Name = "Costar",
                Num = 294,
                Rating = 0.0, // Only useful in Doubles
                // OnSwitchInPriority = -2
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    Pokemon? ally = pokemon.Allies().FirstOrDefault();
                    if (ally == null) return;

                    // Copy all boosts from ally
                    pokemon.Boosts.Atk = ally.Boosts.Atk;
                    pokemon.Boosts.Def = ally.Boosts.Def;
                    pokemon.Boosts.SpA = ally.Boosts.SpA;
                    pokemon.Boosts.SpD = ally.Boosts.SpD;
                    pokemon.Boosts.Spe = ally.Boosts.Spe;
                    pokemon.Boosts.Accuracy = ally.Boosts.Accuracy;
                    pokemon.Boosts.Evasion = ally.Boosts.Evasion;

                    // Copy crit-boosting volatiles
                    ConditionId[] volatilesToCopy =
                    [
                        ConditionId.DragonCheer, ConditionId.FocusEnergy, ConditionId.LaserFocus,
                    ];

                    // Remove existing volatiles first
                    foreach (ConditionId volatileId in volatilesToCopy)
                    {
                        if (pokemon.Volatiles.TryGetValue(volatileId, out EffectState? _))
                        {
                            pokemon.RemoveVolatile(battle.Library.Conditions[volatileId]);
                        }
                    }

                    // Copy volatiles from ally
                    foreach (ConditionId volatileId in volatilesToCopy)
                    {
                        if (ally.Volatiles.TryGetValue(volatileId, out EffectState? @volatile))
                        {
                            pokemon.AddVolatile(volatileId);
                            // Copy layers/special data if needed
                            if (volatileId == ConditionId.DragonCheer &&
                                pokemon.Volatiles.TryGetValue(volatileId, out EffectState? arg2Volatile))
                            {
                                arg2Volatile.HasDragonType =
                                    @volatile.HasDragonType;
                            }
                        }
                    }

                    battle.Add("-copyboost", pokemon, ally, "[from] ability: Costar");
                }, -2),
            },
            [AbilityId.CottonDown] = new()
            {
                Id = AbilityId.CottonDown,
                Name = "Cotton Down",
                Num = 238,
                Rating = 2.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, _, _) =>
                {
                    bool activated = false;
                    foreach (Pokemon pokemon in battle.GetAllActive())
                    {
                        if (pokemon == target || pokemon.Fainted) continue;
                        if (!activated)
                        {
                            battle.Add("-ability", target, "Cotton Down");
                            activated = true;
                        }

                        battle.Boost(new SparseBoostsTable { Spe = -1 }, pokemon, target, null,
                            true);
                    }
                }),
            },
            [AbilityId.CudChew] = new()
            {
                Id = AbilityId.CudChew,
                Name = "Cud Chew",
                Num = 291,
                Rating = 2.0,
                OnEatItem = new OnEatItemEventInfo((battle, item, _, _, effect) =>
                {
                    // Only trigger for berries that weren't stolen by Bug Bite or Pluck
                    bool isStolenBerry = effect is ActiveMove move &&
                                         (move.Id == MoveId.BugBite || move.Id == MoveId.Pluck);

                    if (item.IsBerry && !isStolenBerry)
                    {
                        battle.EffectState.Berry = item;
                        battle.EffectState.Counter = 2;
                        // If eaten during residuals, decrement counter
                        if (battle.Queue.Peek() == null)
                        {
                            battle.EffectState.Counter--;
                        }
                    }
                }),
                // OnResidualOrder = 28, OnResidualSubOrder = 2
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.EffectState.Berry == null || pokemon.Hp == 0) return;
                    battle.EffectState.Counter--;
                    if (battle.EffectState.Counter <= 0)
                    {
                        Item? item = battle.EffectState.Berry;
                        battle.Add("-activate", pokemon, "ability: Cud Chew");
                        battle.Add("-enditem", pokemon, item.Name, "[eat]");

                        // Trigger berry eat effects
                        if (battle.SingleEvent(EventId.Eat, item, null, pokemon) is BoolRelayVar)
                        {
                            battle.RunEvent(EventId.EatItem, pokemon, null, null, item);
                        }

                        // Mark that the pokemon ate a berry (for item-related effects)
                        if (item.OnEat != null)
                        {
                            pokemon.AteBerry = true;
                        }

                        battle.EffectState.Berry = null;
                        battle.EffectState.Counter = null;
                    }
                }, order: 28, subOrder: 2),
            },
            [AbilityId.CuriousMedicine] = new()
            {
                Id = AbilityId.CuriousMedicine,
                Name = "Curious Medicine",
                Num = 261,
                Rating = 0.0, // Only useful in Doubles
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    foreach (Pokemon ally in pokemon.AdjacentAllies())
                    {
                        ally.ClearBoosts();
                        battle.Add("-clearboost", ally, "[from] ability: Curious Medicine",
                            $"[of] {pokemon}");
                    }
                }),
            },
            [AbilityId.CursedBody] = new()
            {
                Id = AbilityId.CursedBody,
                Name = "Cursed Body",
                Num = 130,
                Rating = 2.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, _, source, move) =>
                {
                    if (source.Volatiles.ContainsKey(ConditionId.Disable)) return;
                    if (move.Flags.FutureMove != true && move.Id != MoveId.Struggle)
                    {
                        if (battle.RandomChance(3, 10))
                        {
                            if (battle.EffectState.Target is PokemonEffectStateTarget
                                {
                                    Pokemon: var effectHolder
                                })
                            {
                                source.AddVolatile(ConditionId.Disable, effectHolder);
                            }
                        }
                    }
                }),
            },
            [AbilityId.CuteCharm] = new()
            {
                Id = AbilityId.CuteCharm,
                Name = "Cute Charm",
                Num = 56,
                Rating = 0.5,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        if (battle.RandomChance(3, 10))
                        {
                            if (battle.EffectState.Target is PokemonEffectStateTarget
                                {
                                    Pokemon: var effectHolder
                                })
                            {
                                source.AddVolatile(ConditionId.Attract, effectHolder);
                            }
                        }
                    }
                }),
            },
        };
    }

    // Helper method for Commander ability
    private static void CommanderUpdateLogic(Battle battle, Pokemon pokemon)
    {
        if (battle.GameType != GameType.Doubles) return;

        // Don't run between when a Pokemon switches in and the resulting onSwitchIn event
        IAction? peek = battle.Queue.Peek();
        if (peek?.Choice == ActionId.RunSwitch) return;

        var allies = pokemon.Allies().ToList();
        Pokemon? ally = allies.FirstOrDefault();
        if (pokemon.SwitchFlag.IsTrue() || ally?.SwitchFlag.IsTrue() == true) return;

        if (ally == null ||
            pokemon.BaseSpecies.BaseSpecies != SpecieId.Tatsugiri ||
            ally.BaseSpecies.BaseSpecies != SpecieId.Dondozo)
        {
            // Handle edge cases - remove commanding volatile if present
            if (pokemon.GetVolatile(ConditionId.Commanding) != null)
            {
                pokemon.RemoveVolatile(battle.Library.Conditions[ConditionId.Commanding]);
            }

            return;
        }

        if (pokemon.GetVolatile(ConditionId.Commanding) == null)
        {
            // If Dondozo already was commanded this fails
            if (ally.GetVolatile(ConditionId.Commanded) != null) return;

            // Cancel all actions this turn for pokemon if applicable
            battle.Queue.CancelAction(pokemon);

            // Add volatiles to both pokemon
            battle.Add("-activate", pokemon, "ability: Commander", $"[of] {ally}");
            pokemon.AddVolatile(ConditionId.Commanding);
            ally.AddVolatile(ConditionId.Commanded, pokemon);
            // Continued in conditions.ts in the volatiles
        }
        else
        {
            if (!ally.Fainted) return;
            pokemon.RemoveVolatile(battle.Library.Conditions[ConditionId.Commanding]);
        }
    }
}