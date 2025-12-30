using ApogeeVGC.Sim.Abilities;
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
using ApogeeVGC.Sim.Utils.Unions;
using static ApogeeVGC.Sim.BattleClasses.BattleActions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesMno()
    {
        return new Dictionary<AbilityId, Ability>
        {
            // ==================== 'M' Abilities ====================
            [AbilityId.MagicBounce] = new()
            {
                Id = AbilityId.MagicBounce,
                Name = "Magic Bounce",
                Num = 156,
                Rating = 4.0,
                Flags = new AbilityFlags { Breakable = true },
                // OnTryHitPriority = 1
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (target == source || move.HasBounced == true ||
                        move.Flags.Reflectable != true || target.IsSemiInvulnerable())
                    {
                        return new VoidReturn();
                    }

                    // Get the base move from library and convert to active move
                    Move baseMove = battle.Library.Moves[move.Id];
                    var newMove = baseMove.ToActiveMove();
                    newMove.HasBounced = true;
                    newMove.PranksterBoosted = false;

                    // Bounce the move back to source
                    battle.Actions.UseMove(newMove, target, new UseMoveOptions { Target = source });
                    return null;
                }, 1),
                OnAllyTryHitSide = new OnAllyTryHitSideEventInfo((battle, target, source, move) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder,
                        })
                        return new VoidReturn();

                    if (target.IsAlly(source) || move.HasBounced == true ||
                        move.Flags.Reflectable != true || abilityHolder.IsSemiInvulnerable())
                    {
                        return new VoidReturn();
                    }

                    // Get the base move from library and convert to active move
                    Move baseMove = battle.Library.Moves[move.Id];
                    var newMove = baseMove.ToActiveMove();
                    newMove.HasBounced = true;
                    newMove.PranksterBoosted = false;

                    // Bounce the move back to source from the ability holder
                    battle.Actions.UseMove(newMove, abilityHolder,
                        new UseMoveOptions { Target = source });
                    move.HasBounced = true; // only bounce once in free-for-all battles
                    return null;
                }),
            },
            [AbilityId.MagicGuard] = new()
            {
                Id = AbilityId.MagicGuard,
                Name = "Magic Guard",
                Num = 98,
                Rating = 4.0,
                OnDamage = new OnDamageEventInfo((battle, _, _, source, effect) =>
                {
                    if (effect.EffectType != EffectType.Move)
                    {
                        if (effect.EffectType == EffectType.Ability)
                        {
                            battle.Add("-activate", source, $"ability: {effect.Name}");
                        }

                        return false;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Magician] = new()
            {
                Id = AbilityId.Magician,
                Name = "Magician",
                Num = 170,
                Rating = 1.0,
                OnAfterMoveSecondarySelf =
                    new OnAfterMoveSecondarySelfEventInfo((battle, source, _, move) =>
                    {
                        if (source.SwitchFlag == true || move.HitTargets == null ||
                            source.Item != ItemId.None ||
                            source.Volatiles.ContainsKey(ConditionId.Gem) ||
                            move.Id == MoveId.Fling || move.Category == MoveCategory.Status)
                            return;

                        var hitTargets = move.HitTargets.ToList();
                        battle.SpeedSort(hitTargets);
                        foreach (Pokemon pokemon in hitTargets)
                        {
                            if (pokemon != source)
                            {
                                ItemFalseUnion yourItem = pokemon.TakeItem(source);
                                if (yourItem is not ItemItemFalseUnion { Item: var item }) continue;
                                if (!source.SetItem(item.Id))
                                {
                                    pokemon.Item = item.Id; // bypass setItem
                                    continue;
                                }

                                battle.Add("-item", source, item.Name,
                                    "[from] ability: Magician", $"[of] {pokemon}");
                                return;
                            }
                        }
                    }),
            },
            [AbilityId.MagmaArmor] = new()
            {
                Id = AbilityId.MagmaArmor,
                Name = "Magma Armor",
                Num = 40,
                Rating = 0.5,
                Flags = new AbilityFlags { Breakable = true },
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Freeze)
                    {
                        battle.Add("-activate", pokemon, "ability: Magma Armor");
                        pokemon.CureStatus();
                    }
                }),
                // OnImmunity for Freeze is handled in Pokemon.RunStatusImmunity
            },
            [AbilityId.MagnetPull] = new()
            {
                Id = AbilityId.MagnetPull,
                Name = "Magnet Pull",
                Num = 42,
                Rating = 4.0,
                OnFoeTrapPokemon = new OnFoeTrapPokemonEventInfo((battle, pokemon) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder,
                        })
                        return;
                    if (pokemon.HasType(PokemonType.Steel) && pokemon.IsAdjacent(abilityHolder))
                    {
                        pokemon.TryTrap(true);
                    }
                }),
                OnFoeMaybeTrapPokemon =
                    new OnFoeMaybeTrapPokemonEventInfo((battle, pokemon, source) =>
                    {
                        if (source == null && battle.EffectState.Target is PokemonEffectStateTarget
                            {
                                Pokemon: var holder,
                            })
                        {
                            source = holder;
                        }

                        if (source == null || !pokemon.IsAdjacent(source)) return;
                        if (!pokemon.KnownType || pokemon.HasType(PokemonType.Steel))
                        {
                            pokemon.MaybeTrapped = true;
                        }
                    }),
            },
            [AbilityId.MarvelScale] = new()
            {
                Id = AbilityId.MarvelScale,
                Name = "Marvel Scale",
                Num = 63,
                Rating = 2.5,
                Flags = new AbilityFlags { Breakable = true },
                // OnModifyDefPriority = 6
                OnModifyDef = new OnModifyDefEventInfo((battle, def, pokemon, _, _) =>
                {
                    if (pokemon.Status != ConditionId.None)
                    {
                        battle.ChainModify(1.5);
                        return battle.FinalModify(def);
                    }

                    return def;
                }, 6),
            },
            [AbilityId.MegaLauncher] = new()
            {
                Id = AbilityId.MegaLauncher,
                Name = "Mega Launcher",
                Num = 178,
                Rating = 3.0,
                // OnBasePowerPriority = 19
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Flags.Pulse == true)
                    {
                        battle.ChainModify(1.5);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 19),
            },
            [AbilityId.Merciless] = new()
            {
                Id = AbilityId.Merciless,
                Name = "Merciless",
                Num = 196,
                Rating = 1.5,
                OnModifyCritRatio = new OnModifyCritRatioEventInfo((_, critRatio, _, target, _) =>
                {
                    if (target is { Status: ConditionId.Poison or ConditionId.Toxic })
                    {
                        return 5;
                    }

                    return critRatio;
                }),
            },
            [AbilityId.Mimicry] = new()
            {
                Id = AbilityId.Mimicry,
                Name = "Mimicry",
                Num = 250,
                Rating = 0.0,
                // OnSwitchInPriority = -1
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    battle.SingleEvent(EventId.TerrainChange, battle.Effect, battle.EffectState,
                        pokemon);
                }, -1),
                OnTerrainChange = new OnTerrainChangeEventInfo((battle, pokemon, _, _) =>
                {
                    ConditionId terrain = battle.Field.Terrain;
                    var types = terrain switch
                    {
                        ConditionId.ElectricTerrain => [PokemonType.Electric],
                        ConditionId.GrassyTerrain => [PokemonType.Grass],
                        ConditionId.MistyTerrain => [PokemonType.Fairy],
                        ConditionId.PsychicTerrain => [PokemonType.Psychic],
                        _ => pokemon.BaseSpecies.Types.ToArray(),
                    };

                    var oldTypes = pokemon.GetTypes();
                    if (oldTypes.SequenceEqual(types) || !pokemon.SetType(types)) return;

                    if (terrain != ConditionId.None || pokemon.Transformed)
                    {
                        battle.Add("-start", pokemon, "typechange", string.Join("/", types),
                            "[from] ability: Mimicry");
                        if (terrain == ConditionId.None)
                        {
                            battle.Hint(
                                "Transform Mimicry changes you to your original un-transformed types.");
                        }
                    }
                    else
                    {
                        battle.Add("-activate", pokemon, "ability: Mimicry");
                        battle.Add("-end", pokemon, "typechange", "[silent]");
                    }
                }),
            },
            [AbilityId.MindsEye] = new()
            {
                Id = AbilityId.MindsEye,
                Name = "Mind's Eye",
                Num = 300,
                Rating = 0.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, source, effect) =>
                {
                    if (source != null && target == source) return;
                    if (boost.Accuracy is < 0)
                    {
                        boost.Accuracy = null;
                        if (effect is not ActiveMove { Secondaries: not null })
                        {
                            battle.Add("-fail", target, "unboost", "accuracy",
                                "[from] ability: Mind's Eye", $"[of] {target}");
                        }
                    }
                }),
                // OnModifyMovePriority = -5
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    move.IgnoreEvasion = true;
                    // Set IgnoreImmunity to allow hitting Ghost types with Fighting/Normal
                    // If IgnoreImmunity is already set to true (full immunity ignoring), don't change it
                    if (move.IgnoreImmunity is BoolMoveDataIgnoreImmunity { Value: true })
                    {
                        return;
                    }

                    // Create or update the type-specific immunity dictionary
                    Dictionary<PokemonType, bool> typeImmunities;
                    if (move.IgnoreImmunity is TypeMoveDataIgnoreImmunity existing)
                    {
                        typeImmunities = existing.TypeImmunities;
                    }
                    else
                    {
                        typeImmunities = new Dictionary<PokemonType, bool>();
                        move.IgnoreImmunity = typeImmunities;
                    }

                    typeImmunities[PokemonType.Fighting] = true;
                    typeImmunities[PokemonType.Normal] = true;
                }, -5),
            },
            [AbilityId.Minus] = new()
            {
                Id = AbilityId.Minus,
                Name = "Minus",
                Num = 58,
                Rating = 0.0,
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, pokemon, _, _) =>
                {
                    foreach (Pokemon allyActive in pokemon.Allies())
                    {
                        if (allyActive.HasAbility(AbilityId.Minus) ||
                            allyActive.HasAbility(AbilityId.Plus))
                        {
                            battle.ChainModify(1.5);
                            return battle.FinalModify(spa);
                        }
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.MirrorArmor] = new()
            {
                Id = AbilityId.MirrorArmor,
                Name = "Mirror Armor",
                Num = 240,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, source, effect) =>
                {
                    // Don't bounce self stat changes, or boosts that have already bounced
                    if (source == null || target == source || effect.Name == "Mirror Armor") return;

                    foreach (BoostId b in Enum.GetValues<BoostId>())
                    {
                        int? boostValue = boost.GetBoost(b);
                        if (boostValue is < 0)
                        {
                            int targetBoost = target.Boosts.GetBoost(b);
                            if (targetBoost == -6) continue;

                            var negativeBoost = new SparseBoostsTable();
                            negativeBoost.SetBoost(b, boostValue.Value);
                            boost.ClearBoost(b); // Delete this boost entry
                            if (source.Hp > 0)
                            {
                                battle.Add("-ability", target, "Mirror Armor");
                                battle.Boost(negativeBoost, source, target, null, true);
                            }
                        }
                    }
                }),
            },
            [AbilityId.MistySurge] = new()
            {
                Id = AbilityId.MistySurge,
                Name = "Misty Surge",
                Num = 228,
                Rating = 3.5,
                OnStart = new OnStartEventInfo((battle, _) =>
                {
                    battle.Field.SetTerrain(_library.Conditions[ConditionId.MistyTerrain]);
                }),
            },
            [AbilityId.MoldBreaker] = new()
            {
                Id = AbilityId.MoldBreaker,
                Name = "Mold Breaker",
                Num = 104,
                Rating = 3.0,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    battle.Add("-ability", pokemon, "Mold Breaker");
                }),
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    move.IgnoreAbility = true;
                }),
            },
            [AbilityId.Moody] = new()
            {
                Id = AbilityId.Moody,
                Name = "Moody",
                Num = 141,
                Rating = 5.0,
                // OnResidualOrder = 28, OnResidualSubOrder = 2
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    List<BoostId> statsToRaise = [];
                    var boost = new SparseBoostsTable();

                    // Find stats that can be raised (not at +6)
                    foreach (BoostId stat in Enum.GetValues<BoostId>())
                    {
                        if (stat is BoostId.Accuracy or BoostId.Evasion) continue;
                        if (pokemon.Boosts.GetBoost(stat) < 6)
                        {
                            statsToRaise.Add(stat);
                        }
                    }

                    BoostId? randomStatPlus = statsToRaise.Count > 0
                        ? battle.Sample(statsToRaise)
                        : null;
                    if (randomStatPlus != null)
                    {
                        boost.SetBoost(randomStatPlus.Value, 2);
                    }

                    // Find stats that can be lowered (not at -6, and not the one we just raised)
                    List<BoostId> statsToLower = [];
                    foreach (BoostId stat in Enum.GetValues<BoostId>())
                    {
                        if (stat is BoostId.Accuracy or BoostId.Evasion) continue;
                        if (pokemon.Boosts.GetBoost(stat) > -6 && stat != randomStatPlus)
                        {
                            statsToLower.Add(stat);
                        }
                    }

                    BoostId? randomStatMinus = statsToLower.Count > 0
                        ? battle.Sample(statsToLower)
                        : null;
                    if (randomStatMinus != null)
                    {
                        boost.SetBoost(randomStatMinus.Value, -1);
                    }

                    battle.Boost(boost, pokemon, pokemon);
                }, order: 28, subOrder: 2),
            },
            [AbilityId.MotorDrive] = new()
            {
                Id = AbilityId.MotorDrive,
                Name = "Motor Drive",
                Num = 78,
                Rating = 3.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Electric)
                    {
                        BoolZeroUnion? boostResult =
                            battle.Boost(new SparseBoostsTable { Spe = 1 });
                        if (boostResult is ZeroBoolZeroUnion)
                        {
                            battle.Add("-immune", target, "[from] ability: Motor Drive");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Moxie] = new()
            {
                Id = AbilityId.Moxie,
                Name = "Moxie",
                Num = 153,
                Rating = 3.0,
                OnSourceAfterFaint =
                    new OnSourceAfterFaintEventInfo((battle, length, _, source, effect) =>
                    {
                        if (effect is ActiveMove)
                        {
                            battle.Boost(new SparseBoostsTable { Atk = length }, source);
                        }
                    }),
            },
            [AbilityId.Multiscale] = new()
            {
                Id = AbilityId.Multiscale,
                Name = "Multiscale",
                Num = 136,
                Rating = 3.5,
                Flags = new AbilityFlags { Breakable = true },
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, _, target, _) =>
                    {
                        if (target.Hp >= target.MaxHp)
                        {
                            battle.Debug("Multiscale weaken");
                            battle.ChainModify(0.5);
                            return battle.FinalModify(damage);
                        }

                        return damage;
                    }),
            },
            [AbilityId.Multitype] = new()
            {
                Id = AbilityId.Multitype,
                Name = "Multitype",
                Num = 121,
                Rating = 4.0,
                // Multitype's type-changing is implemented in statuses.js
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
            [AbilityId.Mummy] = new()
            {
                Id = AbilityId.Mummy,
                Name = "Mummy",
                Num = 152,
                Rating = 2.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    Ability sourceAbility = source.GetAbility();
                    if (sourceAbility.Flags.CantSuppress == true ||
                        sourceAbility.Id == AbilityId.Mummy)
                    {
                        return;
                    }

                    if (battle.CheckMoveMakesContact(move, source, target, !source.IsAlly(target)))
                    {
                        AbilityIdFalseUnion? oldAbilityResult =
                            source.SetAbility(AbilityId.Mummy, target);
                        if (oldAbilityResult is AbilityIdAbilityIdFalseUnion
                            {
                                AbilityId: var oldAbilityId,
                            })
                        {
                            string oldAbilityName =
                                battle.Library.Abilities.TryGetValue(oldAbilityId,
                                    out Ability? oldAbilityData)
                                    ? oldAbilityData.Name
                                    : oldAbilityId.ToString();
                            battle.Add("-activate", target, "ability: Mummy", oldAbilityName,
                                $"[of] {source}");
                        }
                    }
                }),
            },
            [AbilityId.MyceliumMight] = new()
            {
                Id = AbilityId.MyceliumMight,
                Name = "Mycelium Might",
                Num = 298,
                Rating = 2.0,
                // OnFractionalPriorityPriority = -1
                OnFractionalPriority =
                    new OnFractionalPriorityEventInfo(
                        (ModifierSourceMoveHandler)((_, _, _, _, move) =>
                        {
                            if (move.Category == MoveCategory.Status)
                            {
                                return -0.1;
                            }

                            return new VoidReturn();
                        }), -1),
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    if (move.Category == MoveCategory.Status)
                    {
                        move.IgnoreAbility = true;
                    }
                }),
            },

            // ==================== 'N' Abilities ====================
            [AbilityId.NaturalCure] = new()
            {
                Id = AbilityId.NaturalCure,
                Name = "Natural Cure",
                Num = 30,
                Rating = 2.5,
                OnCheckShow = new OnCheckShowEventInfo((battle, pokemon) =>
                {
                    // This is complicated
                    // For the most part, in-game, it's obvious whether or not Natural Cure activated,
                    // since you can see how many of your opponent's pokemon are statused.
                    // The only ambiguous situation happens in Doubles/Triples, where multiple pokemon
                    // that could have Natural Cure switch out, but only some of them get cured.
                    if (pokemon.Side.Active.Count == 1) return;
                    if (pokemon.ShowCure is true or false) return;

                    List<Pokemon> cureList = [];
                    int noCureCount = 0;

                    foreach (Pokemon? curPoke in pokemon.Side.Active)
                    {
                        // pokemon not statused
                        if (curPoke?.Status == ConditionId.None || curPoke == null)
                        {
                            continue;
                        }

                        if (curPoke.ShowCure != null)
                        {
                            continue;
                        }

                        Species species = curPoke.Species;
                        // pokemon can't get Natural Cure
                        bool canHaveNaturalCure =
                            species.Abilities.Slot0 == AbilityId.NaturalCure ||
                            species.Abilities.Slot1 == AbilityId.NaturalCure ||
                            species.Abilities.Hidden == AbilityId.NaturalCure;

                        if (!canHaveNaturalCure)
                        {
                            continue;
                        }

                        // pokemon's ability is known to be Natural Cure
                        if (species.Abilities.Slot1 == null && species.Abilities.Hidden == null)
                        {
                            continue;
                        }

                        // pokemon isn't switching this turn
                        if (curPoke != pokemon && battle.Queue.WillSwitch(curPoke) == null)
                        {
                            continue;
                        }

                        if (curPoke.HasAbility(AbilityId.NaturalCure))
                        {
                            cureList.Add(curPoke);
                        }
                        else
                        {
                            noCureCount++;
                        }
                    }

                    if (cureList.Count == 0 || noCureCount == 0)
                    {
                        // It's possible to know what pokemon were cured
                        foreach (Pokemon pkmn in cureList)
                        {
                            pkmn.ShowCure = true;
                        }
                    }
                    else
                    {
                        // It's not possible to know what pokemon were cured
                        // Unlike a -hint, this is real information that battlers need, so we use a -message
                        string plural = cureList.Count == 1 ? "was" : "were";
                        battle.Add("-message",
                            $"({cureList.Count} of {pokemon.Side.Name}'s pokemon {plural} cured by Natural Cure.)");

                        foreach (Pokemon pkmn in cureList)
                        {
                            pkmn.ShowCure = false;
                        }
                    }
                }),
                OnSwitchOut = new OnSwitchOutEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.None) return;

                    // If pokemon.ShowCure is undefined, it was skipped because its ability is known
                    if (pokemon.ShowCure == null) pokemon.ShowCure = true;

                    if (pokemon.ShowCure == true)
                    {
                        battle.Add("-curestatus", pokemon, pokemon.Status.ToString(),
                            "[from] ability: Natural Cure");
                    }

                    pokemon.ClearStatus();

                    // only reset .ShowCure if it's false
                    // (once you know a Pokemon has Natural Cure, its cures are always known)
                    if (pokemon.ShowCure == false) pokemon.ShowCure = null;
                }),
            },
            [AbilityId.Neuroforce] = new()
            {
                Id = AbilityId.Neuroforce,
                Name = "Neuroforce",
                Num = 233,
                Rating = 2.5,
                OnModifyDamage = new OnModifyDamageEventInfo((battle, damage, _, target, move) =>
                {
                    if (target.GetMoveHitData(move).TypeMod > 0)
                    {
                        battle.ChainModify([5120, 4096]);
                        return battle.FinalModify(damage);
                    }

                    return damage;
                }),
            },
            [AbilityId.NeutralizingGas] = new()
            {
                Id = AbilityId.NeutralizingGas,
                Name = "Neutralizing Gas",
                Num = 256,
                Rating = 3.5,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    NoTransform = true,
                },
                // OnSwitchInPriority = 2
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    battle.Add("-ability", pokemon, "Neutralizing Gas");
                    pokemon.AbilityState.Ending = false;
                    // The actual suppression is handled in Pokemon.IgnoringAbility()

                    AbilityId[] strongWeathers =
                    [
                        AbilityId.DesolateLand,
                        AbilityId.PrimordialSea,
                        AbilityId.DeltaStream,
                    ];

                    foreach (Pokemon target in battle.GetAllActive())
                    {
                        if (target.HasItem(ItemId.AbilityShield))
                        {
                            battle.Add("-block", target, "item: Ability Shield");
                            continue;
                        }

                        // Can't suppress a Tatsugiri inside of Dondozo already
                        if (target.Volatiles.ContainsKey(ConditionId.Commanding))
                        {
                            continue;
                        }

                        if (target.Illusion != null)
                        {
                            Ability illusionAbility = battle.Library.Abilities[AbilityId.Illusion];
                            battle.SingleEvent(EventId.End, illusionAbility, target.AbilityState,
                                target, pokemon);
                        }

                        // Note: Slow Start is handled via ability state counter, not a volatile
                        // Gen 9 VGC doesn't include Regigigas, so this check is omitted

                        if (Array.Exists(strongWeathers, w => w == target.GetAbility().Id))
                        {
                            Ability targetAbility = target.GetAbility();
                            battle.SingleEvent(EventId.End, targetAbility, target.AbilityState,
                                target, pokemon);
                        }
                    }
                }, 2),
                OnEnd = new OnEndEventInfo((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                    Pokemon source = psfp.Pokemon;
                    if (source.Transformed) return;

                    foreach (Pokemon pokemon in battle.GetAllActive())
                    {
                        if (pokemon != source && pokemon.HasAbility(AbilityId.NeutralizingGas))
                        {
                            return;
                        }
                    }

                    battle.Add("-end", source, "ability: Neutralizing Gas");

                    // Mark this pokemon's ability as ending so Pokemon.IgnoringAbility skips it
                    if (source.AbilityState.Ending == true) return;
                    source.AbilityState.Ending = true;

                    var sortedActive = battle.GetAllActive().ToList();
                    battle.SpeedSort(sortedActive);
                    foreach (Pokemon pokemon in sortedActive)
                    {
                        if (pokemon != source)
                        {
                            Ability ability = pokemon.GetAbility();
                            if (ability.Flags.CantSuppress == true) continue;
                            if (pokemon.HasItem(ItemId.AbilityShield)) continue;
                            // Will be suppressed by Pokemon.IgnoringAbility if needed
                            battle.SingleEvent(EventId.Start, ability, pokemon.AbilityState,
                                pokemon);
                            if (pokemon.Ability == AbilityId.Gluttony)
                            {
                                pokemon.AbilityState.Gluttony = false;
                            }
                        }
                    }
                }),
            },
            [AbilityId.NoGuard] = new()
            {
                Id = AbilityId.NoGuard,
                Name = "No Guard",
                Num = 99,
                Rating = 4.0,
                // OnAnyInvulnerabilityPriority = 1
                OnAnyInvulnerability =
                    new OnAnyInvulnerabilityEventInfo((battle, target, source, move) =>
                    {
                        if (move != null && battle.EffectState.Target is PokemonEffectStateTarget
                            {
                                Pokemon: var effectHolder,
                            })
                        {
                            if (source == effectHolder || target == effectHolder)
                            {
                                return 0;
                            }
                        }

                        return new VoidReturn();
                    }, 1),
                OnAnyAccuracy =
                    new OnAnyAccuracyEventInfo((battle, accuracy, target, source, move) =>
                    {
                        if (move != null && battle.EffectState.Target is PokemonEffectStateTarget
                            {
                                Pokemon: var effectHolder,
                            })
                        {
                            if (source == effectHolder || target == effectHolder)
                            {
                                return true;
                            }
                        }

                        return accuracy;
                    }),
            },
            [AbilityId.Normalize] = new()
            {
                Id = AbilityId.Normalize,
                Name = "Normalize",
                Num = 96,
                Rating = 0.0,
                // OnModifyTypePriority = 1
                OnModifyType = new OnModifyTypeEventInfo((battle, move, pokemon, _) =>
                {
                    // Skip certain moves that shouldn't be affected by Normalize
                    // Note: NaturalGift and Technoblast not in Gen 9; MultiAttack handled by Silvally's RKS System
                    MoveId[] noModifyType =
                    [
                        MoveId.Judgment, MoveId.RevelationDance, MoveId.TerrainPulse,
                        MoveId.WeatherBall, MoveId.Struggle,
                    ];
                    if (Array.Exists(noModifyType, id => id == move.Id)) return;
                    if (move.Name == "Tera Blast" && pokemon.Terastallized != null) return;

                    move.Type = MoveType.Normal;
                    move.TypeChangerBoosted = battle.Effect;
                }, 1),
                // OnBasePowerPriority = 23
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.TypeChangerBoosted == battle.Effect)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 23),
            },

            // ==================== 'O' Abilities ====================
            [AbilityId.Oblivious] = new()
            {
                Id = AbilityId.Oblivious,
                Name = "Oblivious",
                Num = 12,
                Rating = 1.5,
                Flags = new AbilityFlags { Breakable = true },
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Volatiles.ContainsKey(ConditionId.Attract))
                    {
                        battle.Add("-activate", pokemon, "ability: Oblivious");
                        pokemon.RemoveVolatile(battle.Library.Conditions[ConditionId.Attract]);
                        battle.Add("-end", pokemon, "move: Attract", "[from] ability: Oblivious");
                    }

                    if (pokemon.Volatiles.ContainsKey(ConditionId.Taunt))
                    {
                        battle.Add("-activate", pokemon, "ability: Oblivious");
                        pokemon.RemoveVolatile(battle.Library.Conditions[ConditionId.Taunt]);
                        // Taunt's volatile already sends the -end message when removed
                    }
                }),
                // OnImmunity for 'attract' is handled in Pokemon.RunImmunity
                OnTryHit = new OnTryHitEventInfo((battle, pokemon, _, move) =>
                {
                    // Note: Captivate was removed from Gen 8+, so not included here
                    if (move.Id is MoveId.Attract or MoveId.Taunt)
                    {
                        battle.Add("-immune", pokemon, "[from] ability: Oblivious");
                        return null;
                    }

                    return new VoidReturn();
                }),
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, _, effect) =>
                {
                    if (effect.Name == "Intimidate" && boost.Atk != null)
                    {
                        boost.Atk = null;
                        battle.Add("-fail", target, "unboost", "Attack",
                            "[from] ability: Oblivious", $"[of] {target}");
                    }
                }),
            },
            [AbilityId.Opportunist] = new()
            {
                Id = AbilityId.Opportunist,
                Name = "Opportunist",
                Num = 290,
                Rating = 3.0,
                OnFoeAfterBoost = new OnFoeAfterBoostEventInfo((battle, boost, _, _, effect) =>
                {
                    if (effect.Name is "Opportunist" or "Mirror Herb") return;
                    battle.EffectState.Boosts ??= new SparseBoostsTable();

                    SparseBoostsTable boostPlus = battle.EffectState.Boosts;
                    foreach (BoostId stat in Enum.GetValues<BoostId>())
                    {
                        int? boostValue = boost.GetBoost(stat);
                        if (boostValue is > 0)
                        {
                            int existing = boostPlus.GetBoost(stat) ?? 0;
                            boostPlus.SetBoost(stat, existing + boostValue.Value);
                        }
                    }
                }),
                // OnAnySwitchInPriority = -3
                OnAnySwitchIn = new OnAnySwitchInEventInfo((battle, _) =>
                {
                    if (battle.EffectState.Boosts == null) return;
                    if (battle.EffectState.Target is PokemonEffectStateTarget
                        {
                            Pokemon: var effectHolder,
                        })
                    {
                        battle.Boost(battle.EffectState.Boosts, effectHolder);
                    }

                    battle.EffectState.Boosts = null;
                }, -3),
                OnAnyAfterTerastallization =
                    new OnAnyAfterTerastallizationEventInfo((battle, _) =>
                    {
                        if (battle.EffectState.Boosts == null) return;
                        if (battle.EffectState.Target is PokemonEffectStateTarget
                            {
                                Pokemon: var effectHolder,
                            })
                        {
                            battle.Boost(battle.EffectState.Boosts, effectHolder);
                        }

                        battle.EffectState.Boosts = null;
                    }),
                OnAnyAfterMove = new OnAnyAfterMoveEventInfo((battle, _, _, _) =>
                {
                    if (battle.EffectState.Boosts == null) return new VoidReturn();
                    if (battle.EffectState.Target is PokemonEffectStateTarget
                        {
                            Pokemon: var effectHolder,
                        })
                    {
                        battle.Boost(battle.EffectState.Boosts, effectHolder);
                    }

                    battle.EffectState.Boosts = null;
                    return new VoidReturn();
                }),
                // OnResidualOrder = 29
                OnResidual = new OnResidualEventInfo((battle, _, _, _) =>
                {
                    if (battle.EffectState.Boosts == null) return;
                    if (battle.EffectState.Target is PokemonEffectStateTarget
                        {
                            Pokemon: var effectHolder,
                        })
                    {
                        battle.Boost(battle.EffectState.Boosts, effectHolder);
                    }

                    battle.EffectState.Boosts = null;
                }, order: 29),
                OnEnd = new OnEndEventInfo((battle, _) => { battle.EffectState.Boosts = null; }),
            },
            [AbilityId.OrichalcumPulse] = new()
            {
                Id = AbilityId.OrichalcumPulse,
                Name = "Orichalcum Pulse",
                Num = 288,
                Rating = 4.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (battle.Field.SetWeather(_library.Conditions[ConditionId.SunnyDay]))
                    {
                        battle.Add("-activate", pokemon, "Orichalcum Pulse", "[source]");
                    }
                    else if (battle.Field.IsWeather(ConditionId.SunnyDay))
                    {
                        battle.Add("-activate", pokemon, "ability: Orichalcum Pulse");
                    }
                }),
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
                {
                    ConditionId weather = pokemon.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        battle.Debug("Orichalcum boost");
                        battle.ChainModify([5461, 4096]);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
            },
            [AbilityId.Overcoat] = new()
            {
                Id = AbilityId.Overcoat,
                Name = "Overcoat",
                Num = 142,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                // OnImmunity for 'sandstorm', 'hail', and 'powder' handled in Pokemon.RunImmunity
                // OnTryHitPriority = 1
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    // Grass types are naturally immune to powder moves, so Overcoat only shows
                    // its message if the target is NOT Grass type (otherwise it's redundant)
                    if (move.Flags.Powder == true && target != source &&
                        !target.HasType(PokemonType.Grass))
                    {
                        battle.Add("-immune", target, "[from] ability: Overcoat");
                        return null;
                    }

                    return new VoidReturn();
                }, 1),
            },
            [AbilityId.Overgrow] = new()
            {
                Id = AbilityId.Overgrow,
                Name = "Overgrow",
                Num = 65,
                Rating = 2.0,
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Grass && attacker.Hp <= attacker.MaxHp / 3)
                    {
                        battle.Debug("Overgrow boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Grass && attacker.Hp <= attacker.MaxHp / 3)
                    {
                        battle.Debug("Overgrow boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.OwnTempo] = new()
            {
                Id = AbilityId.OwnTempo,
                Name = "Own Tempo",
                Num = 20,
                Rating = 1.5,
                Flags = new AbilityFlags { Breakable = true },
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Volatiles.ContainsKey(ConditionId.Confusion))
                    {
                        battle.Add("-activate", pokemon, "ability: Own Tempo");
                        pokemon.RemoveVolatile(battle.Library.Conditions[ConditionId.Confusion]);
                    }
                }),
                OnTryAddVolatile = new OnTryAddVolatileEventInfo((_, status, _, _, _) =>
                {
                    if (status.Id == ConditionId.Confusion)
                    {
                        return null;
                    }

                    return new VoidReturn();
                }),
                OnHit = new OnHitEventInfo((battle, target, _, move) =>
                {
                    if (move.VolatileStatus == ConditionId.Confusion)
                    {
                        battle.Add("-immune", target, "confusion", "[from] ability: Own Tempo");
                    }

                    return new VoidReturn();
                }),
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, _, effect) =>
                {
                    if (effect.Name == "Intimidate" && boost.Atk != null)
                    {
                        boost.Atk = null;
                        battle.Add("-fail", target, "unboost", "Attack",
                            "[from] ability: Own Tempo", $"[of] {target}");
                    }
                }),
            },
        };
    }
}