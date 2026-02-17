using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesGhi()
    {
        return new Dictionary<AbilityId, Ability>
        {
            [AbilityId.GaleWings] = new()
            {
                Id = AbilityId.GaleWings,
                Name = "Gale Wings",
                Num = 177,
                Rating = 1.5,
                OnModifyPriority =
                    OnModifyPriorityEventInfo.Create((_, priority, pokemon, _, move) =>
                    {
                        if (move.Type == MoveType.Flying && pokemon.Hp == pokemon.MaxHp)
                        {
                            return priority + 1;
                        }

                        return new VoidReturn();
                    }),
            },
            [AbilityId.Galvanize] = new()
            {
                Id = AbilityId.Galvanize,
                Name = "Galvanize",
                Num = 206,
                Rating = 4.0,
                // OnModifyTypePriority = -1
                OnModifyType = OnModifyTypeEventInfo.Create((battle, move, pokemon, _) =>
                {
                    // Moves that should not have their type changed
                    var noModifyType = new[]
                    {
                        MoveId.Judgment, MoveId.RevelationDance, MoveId.TerrainPulse,
                        MoveId.WeatherBall
                        // Note: MultiAttack, NaturalGift, and TechnoBlast are not in Gen 9 VGC
                    };

                    if (move.Type == MoveType.Normal &&
                        !noModifyType.Contains(move.Id) &&
                        !(move.Id == MoveId.TeraBlast && pokemon.Terastallized != null))
                    {
                        move.Type = MoveType.Electric;
                        move.TypeChangerBoosted = battle.Effect;
                    }
                }, -1),
                // OnBasePowerPriority = 23
                OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, _, _, move) =>
                {
                    if (move.TypeChangerBoosted == battle.Effect)
                    {
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 23),
            },
            [AbilityId.Gluttony] = new()
            {
                Id = AbilityId.Gluttony,
                Name = "Gluttony",
                Num = 82,
                Rating = 1.5,
                OnStart = OnStartEventInfo.Create((_, pokemon) => { pokemon.AbilityState.Gluttony = true; }),
                OnDamage = OnDamageEventInfo.Create((_, _, target, _, _) =>
                {
                    target.AbilityState.Gluttony = true;
                    return new VoidReturn();
                }),
            },
            [AbilityId.GoodAsGold] = new()
            {
                Id = AbilityId.GoodAsGold,
                Name = "Good as Gold",
                Num = 283,
                Rating = 5.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = OnTryHitEventInfo.Create((battle, target, source, move) =>
                {
                    if (move.Category == MoveCategory.Status && target != source)
                    {
                        battle.Add("-immune", target, "[from] ability: Good as Gold");
                        return null;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Gooey] = new()
            {
                Id = AbilityId.Gooey,
                Name = "Gooey",
                Num = 183,
                Rating = 2.0,
                OnDamagingHit = OnDamagingHitEventInfo.Create((battle, _, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target, true))
                    {
                        battle.Add("-ability", target, "Gooey");
                        battle.Boost(new SparseBoostsTable { Spe = -1 }, source, target, null,
                            true);
                    }
                }),
            },
            [AbilityId.GorillaTactics] = new()
            {
                Id = AbilityId.GorillaTactics,
                Name = "Gorilla Tactics",
                Num = 255,
                Rating = 4.5,
                OnStart = OnStartEventInfo.Create((_, pokemon) => { pokemon.AbilityState.ChoiceLock = null; }),
                OnBeforeMove = OnBeforeMoveEventInfo.Create((battle, pokemon, _, move) =>
                {
                    if (move.Id == MoveId.Struggle) return new VoidReturn();
                    var choiceLock = pokemon.AbilityState.ChoiceLock;
                    if (choiceLock != null && choiceLock != move.Id)
                    {
                        // Fails unless ability is being ignored
                        battle.Add("move", pokemon, move.Name);
                        battle.AttrLastMove("[still]");
                        battle.Debug("Disabled by Gorilla Tactics");
                        battle.Add("-fail", pokemon);
                        return false;
                    }

                    return new VoidReturn();
                }),
                OnModifyMove = OnModifyMoveEventInfo.Create((_, move, pokemon, _) =>
                {
                    if (pokemon.AbilityState.ChoiceLock != null || move.Id == MoveId.Struggle)
                        return;
                    pokemon.AbilityState.ChoiceLock = move.Id;
                }),
                // OnModifyAtkPriority = 1
                OnModifyAtk = OnModifyAtkEventInfo.Create((battle, atk, _, _, _) =>
                {
                    battle.Debug("Gorilla Tactics Atk Boost");
                    battle.ChainModify(1.5);
                    return battle.FinalModify(atk);
                }, 1),
                OnDisableMove = OnDisableMoveEventInfo.Create((battle, pokemon) =>
                {
                    var choiceLock = pokemon.AbilityState.ChoiceLock;
                    if (choiceLock == null) return;
                    foreach (MoveSlot moveSlot in pokemon.MoveSlots)
                    {
                        if (moveSlot.Id != choiceLock)
                        {
                            pokemon.DisableMove(moveSlot.Id, false,
                                battle.EffectState.SourceEffect);
                        }
                    }
                }),
                OnEnd = OnEndEventInfo.Create((_, pokemonUnion) =>
                {
                    if (pokemonUnion is PokemonSideFieldPokemon psfp)
                    {
                        psfp.Pokemon.AbilityState.ChoiceLock = null;
                    }
                }),
            },
            [AbilityId.GrassPelt] = new()
            {
                Id = AbilityId.GrassPelt,
                Name = "Grass Pelt",
                Num = 179,
                Rating = 0.5,
                Flags = new AbilityFlags { Breakable = true },
                // OnModifyDefPriority = 6
                OnModifyDef = OnModifyDefEventInfo.Create((battle, def, _, _, _) =>
                {
                    if (battle.Field.IsTerrain(ConditionId.GrassyTerrain, null))
                    {
                        battle.ChainModify(1.5);
                        return battle.FinalModify(def);
                    }

                    return def;
                }, 6),
            },
            [AbilityId.GrassySurge] = new()
            {
                Id = AbilityId.GrassySurge,
                Name = "Grassy Surge",
                Num = 229,
                Rating = 4.0,
                OnStart = OnStartEventInfo.Create((battle, _) =>
                {
                    battle.Field.SetTerrain(_library.Conditions[ConditionId.GrassyTerrain]);
                }),
            },
            [AbilityId.GrimNeigh] = new()
            {
                Id = AbilityId.GrimNeigh,
                Name = "Grim Neigh",
                Num = 265,
                Rating = 3.0,
                OnSourceAfterFaint =
                    OnSourceAfterFaintEventInfo.Create((battle, length, _, source, effect) =>
                    {
                        if (effect is ActiveMove)
                        {
                            battle.Boost(new SparseBoostsTable { SpA = length }, source);
                        }
                    }),
            },
            [AbilityId.GuardDog] = new()
            {
                Id = AbilityId.GuardDog,
                Name = "Guard Dog",
                Num = 275,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                // OnDragOutPriority = 1
                OnDragOut = OnDragOutEventInfo.Create((battle, pokemon, _, _) =>
                {
                    battle.Add("-activate", pokemon, "ability: Guard Dog");
                    return null; // Prevent drag-out
                }, 1),
                // OnTryBoostPriority = 2
                OnTryBoost = OnTryBoostEventInfo.Create((battle, boost, target, _, effect) =>
                {
                    if (effect?.EffectStateId == AbilityId.Intimidate && boost.Atk != null)
                    {
                        boost.Atk = null; // Delete the atk boost
                        battle.Boost(new SparseBoostsTable { Atk = 1 }, target, target, null, false,
                            true);
                    }
                }, 2),
            },
            [AbilityId.GulpMissile] = new()
            {
                Id = AbilityId.GulpMissile,
                Name = "Gulp Missile",
                Num = 241,
                Rating = 2.5,
                Flags = new AbilityFlags { CantSuppress = true, NoTransform = true },
                OnDamagingHit = OnDamagingHitEventInfo.Create((battle, _, target, source, move) =>
                {
                    if (source.Hp == 0 || !source.IsActive || target.IsSemiInvulnerable()) return;

                    SpecieId specieId = target.Species.Id;
                    if (specieId is SpecieId.CramorantGulping or SpecieId.CramorantGorging)
                    {
                        battle.Damage(source.BaseMaxHp / 4, source, target);
                        if (specieId == SpecieId.CramorantGulping)
                        {
                            battle.Boost(new SparseBoostsTable { Def = -1 }, source, target, null,
                                true);
                        }
                        else
                        {
                            source.TrySetStatus(ConditionId.Paralysis, target, move);
                        }

                        target.FormeChange(SpecieId.Cramorant, move);
                    }
                }),
                OnSourceTryPrimaryHit =
                    OnSourceTryPrimaryHitEventInfo.Create((_, _, source, move) =>
                    {
                        if (move.Id == MoveId.Surf && source.HasAbility(AbilityId.GulpMissile) &&
                            source.Species.Id == SpecieId.Cramorant)
                        {
                            SpecieId forme = source.Hp <= source.MaxHp / 2
                                ? SpecieId.CramorantGorging
                                : SpecieId.CramorantGulping;
                            source.FormeChange(forme, move);
                        }

                        return new VoidReturn();
                    }),
            },
            [AbilityId.Guts] = new()
            {
                Id = AbilityId.Guts,
                Name = "Guts",
                Num = 62,
                Rating = 3.5,
                //OnModifyAtkPriority = 5,
                OnModifyAtk = OnModifyAtkEventInfo.Create((battle, atk, pokemon, _, _) =>
                {
                    if (pokemon.Status != ConditionId.None)
                    {
                        battle.ChainModify(1.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
            },
            [AbilityId.HadronEngine] = new()
            {
                Id = AbilityId.HadronEngine,
                Name = "Hadron Engine",
                Num = 289,
                Rating = 4.5,
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    bool terrainSet = battle.Field.SetTerrain(
                        _library.Conditions[ConditionId.ElectricTerrain]);

                    if (!terrainSet &&
                        battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "ability: Hadron Engine");
                        }
                    }
                }),
                //OnModifySpAPriority = 5,
                OnModifySpA = OnModifySpAEventInfo.Create((battle, spa, _, _, _) =>
                    {
                        if (!battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                            return spa;
                        battle.Debug("Hadron Engine boost");
                        battle.ChainModify([5461, 4096]);
                        return battle.FinalModify(spa);
                    },
                    5),
            },
            [AbilityId.Harvest] = new()
            {
                Id = AbilityId.Harvest,
                Name = "Harvest",
                Num = 139,
                Rating = 2.5,
                // OnResidualOrder = 28, OnResidualSubOrder = 2
                OnResidual = OnResidualEventInfo.Create((battle, pokemon, _, _) =>
                {
                    // 50% chance normally, 100% in sun
                    bool canHarvest =
                        battle.Field.IsWeather([ConditionId.SunnyDay, ConditionId.DesolateLand]) ||
                        battle.RandomChance(1, 2);

                    if (!canHarvest) return;
                    if (pokemon.Hp == 0) return;
                    if (pokemon.Item != ItemId.None) return; // Already has an item, can't harvest

                    Item? lastItem = battle.Library.Items.GetValueOrDefault(pokemon.LastItem);
                    if (lastItem is not { IsBerry: true }) return;

                    pokemon.SetItem(lastItem.Id);
                    pokemon.LastItem = default;
                    battle.Add("-item", pokemon, lastItem.Name, "[from] ability: Harvest");
                }, order: 28, subOrder: 2),
            },
            [AbilityId.Healer] = new()
            {
                Id = AbilityId.Healer,
                Name = "Healer",
                Num = 131,
                Rating = 0.0,
                // OnResidualOrder = 5, OnResidualSubOrder = 3
                OnResidual = OnResidualEventInfo.Create((battle, pokemon, _, _) =>
                {
                    foreach (Pokemon allyActive in pokemon.AdjacentAllies())
                    {
                        if (allyActive.Status != ConditionId.None && battle.RandomChance(3, 10))
                        {
                            battle.Add("-activate", pokemon, "ability: Healer");
                            allyActive.CureStatus();
                        }
                    }
                }, order: 5, subOrder: 3),
            },
            [AbilityId.Heatproof] = new()
            {
                Id = AbilityId.Heatproof,
                Name = "Heatproof",
                Num = 85,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                // OnSourceModifyAtkPriority = 6
                OnSourceModifyAtk = OnSourceModifyAtkEventInfo.Create((battle, atk, _, _, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        battle.Debug("Heatproof Atk weaken");
                        battle.ChainModify(0.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 6),
                // OnSourceModifySpAPriority = 5
                OnSourceModifySpA = OnSourceModifySpAEventInfo.Create((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        battle.Debug("Heatproof SpA weaken");
                        battle.ChainModify(0.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
                OnDamage = OnDamageEventInfo.Create((_, damage, _, _, effect) =>
                {
                    if (effect is Condition { Id: ConditionId.Burn })
                    {
                        return damage / 2;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.HeavyMetal] = new()
            {
                Id = AbilityId.HeavyMetal,
                Name = "Heavy Metal",
                Num = 134,
                Rating = 0.0,
                Flags = new AbilityFlags { Breakable = true },
                // OnModifyWeightPriority = 1
                OnModifyWeight = OnModifyWeightEventInfo.Create((_, weighthg, _) => weighthg * 2, 1),
            },
            [AbilityId.HoneyGather] = new()
            {
                Id = AbilityId.HoneyGather,
                Name = "Honey Gather",
                Num = 118,
                Rating = 0.0,
                // No competitive effect
            },
            [AbilityId.Hospitality] = new()
            {
                Id = AbilityId.Hospitality,
                Name = "Hospitality",
                Num = 299,
                Rating = 0.0,
                // OnSwitchInPriority = -2
                OnSwitchIn = OnSwitchInEventInfo.Create((_, _) => { }, -2),
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    foreach (Pokemon ally in pokemon.AdjacentAllies())
                    {
                        battle.Heal(ally.BaseMaxHp / 4, ally, pokemon);
                    }
                }),
            },
            [AbilityId.HugePower] = new()
            {
                Id = AbilityId.HugePower,
                Name = "Huge Power",
                Num = 37,
                Rating = 5.0,
                // OnModifyAtkPriority = 5
                OnModifyAtk = OnModifyAtkEventInfo.Create((battle, atk, _, _, _) =>
                {
                    battle.ChainModify(2);
                    return battle.FinalModify(atk);
                }, 5),
            },
            [AbilityId.HungerSwitch] = new()
            {
                Id = AbilityId.HungerSwitch,
                Name = "Hunger Switch",
                Num = 258,
                Rating = 1.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    NoTransform = true,
                },
                // OnResidualOrder = 29
                OnResidual = OnResidualEventInfo.Create((_, pokemon, _, _) =>
                {
                    if (pokemon.Species.BaseSpecies != SpecieId.Morpeko) return;
                    if (pokemon.Terastallized != null) return;

                    SpecieId targetForme = pokemon.Species.Id == SpecieId.Morpeko
                        ? SpecieId.MorpekoHangry
                        : SpecieId.Morpeko;
                    pokemon.FormeChange(targetForme);
                }, order: 29),
            },
            [AbilityId.Hustle] = new()
            {
                Id = AbilityId.Hustle,
                Name = "Hustle",
                Num = 55,
                Rating = 3.5,
                // OnModifyAtkPriority = 5
                // Note: In TS this uses this.modify() directly instead of chainModify
                OnModifyAtk = OnModifyAtkEventInfo.Create((_, atk, _, _, _) => (int)(atk * 1.5), 5),
                // OnSourceModifyAccuracyPriority = -1
                OnSourceModifyAccuracy = OnSourceModifyAccuracyEventInfo.Create(
                    (battle, accuracy, _, _, move) =>
                    {
                        // Only modify accuracy for physical moves with numeric accuracy
                        // (TypeScript checks: move.category === 'Physical' && typeof accuracy === 'number')
                        if (move.Category == MoveCategory.Physical && accuracy.HasValue)
                        {
                            return battle.ChainModify([3277, 4096]);
                        }

                        return new VoidReturn();
                    }, -1),
            },
            [AbilityId.Hydration] = new()
            {
                Id = AbilityId.Hydration,
                Name = "Hydration",
                Num = 93,
                Rating = 1.5,
                // OnResidualOrder = 5, OnResidualSubOrder = 3
                OnResidual = OnResidualEventInfo.Create((battle, pokemon, _, _) =>
                {
                    if (pokemon.Status != ConditionId.None &&
                        pokemon.EffectiveWeather() is ConditionId.RainDance
                            or ConditionId.PrimordialSea)
                    {
                        battle.Debug("hydration");
                        battle.Add("-activate", pokemon, "ability: Hydration");
                        pokemon.CureStatus();
                    }
                }, order: 5, subOrder: 3),
            },
            [AbilityId.HyperCutter] = new()
            {
                Id = AbilityId.HyperCutter,
                Name = "Hyper Cutter",
                Num = 52,
                Rating = 1.5,
                Flags = new AbilityFlags { Breakable = true },
                OnTryBoost = OnTryBoostEventInfo.Create((battle, boost, target, source, effect) =>
                {
                    if (source != null && target == source) return;
                    if (boost.Atk is < 0)
                    {
                        boost.Atk = null;
                        if (effect is not ActiveMove { Secondaries: not null })
                        {
                            battle.Add("-fail", target, "unboost", "Attack",
                                "[from] ability: Hyper Cutter", $"[of] {target}");
                        }
                    }
                }),
            },
            // ==================== 'I' Abilities ====================
            [AbilityId.IceBody] = new()
            {
                Id = AbilityId.IceBody,
                Name = "Ice Body",
                Num = 115,
                Rating = 1.0,
                OnWeather = OnWeatherEventInfo.Create((battle, target, _, effect) =>
                {
                    if (effect.Id is ConditionId.Snowscape)
                    {
                        battle.Heal(target.BaseMaxHp / 16);
                    }
                }),
                // Note: TS has onImmunity for hail damage, but hail doesn't exist in Gen 9
                // and Snowscape doesn't deal residual damage, so the handler is omitted.
            },
            [AbilityId.IceFace] = new()
            {
                Id = AbilityId.IceFace,
                Name = "Ice Face",
                Num = 248,
                Rating = 3.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                    Breakable = true,
                    NoTransform = true,
                },
                // OnSwitchInPriority = -2
                OnSwitchIn = OnSwitchInEventInfo.Create((_, _) => { }, -2),
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (battle.Field.IsWeather([ConditionId.Snowscape]) &&
                        pokemon.Species.Id == SpecieId.EiscueNoice)
                    {
                        battle.Add("-activate", pokemon, "ability: Ice Face");
                        battle.EffectState.Busted = false;
                        pokemon.FormeChange(SpecieId.Eiscue, battle.Effect, true);
                    }
                }),
                // OnDamagePriority = 1
                OnDamage = OnDamageEventInfo.Create((battle, _, target, _, effect) =>
                {
                    if (effect is ActiveMove { Category: MoveCategory.Physical } &&
                        target.Species.Id == SpecieId.Eiscue)
                    {
                        battle.Add("-activate", target, "ability: Ice Face");
                        battle.EffectState.Busted = true;
                        return 0;
                    }

                    return new VoidReturn();
                }, 1),
                OnCriticalHit = OnCriticalHitEventInfo.Create(
                    (Func<Battle, Pokemon, object?, Move, BoolVoidUnion>)((_, target, _, move) =>
                    {
                        if (target is null) return new VoidReturn();
                        if (move.Category != MoveCategory.Physical ||
                            target.Species.Id != SpecieId.Eiscue) return new VoidReturn();
                        // Check if the move hits the substitute (not the Pokemon directly)
                        bool hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                      move.Flags.BypassSub != true &&
                                      (move is not ActiveMove am || am.Infiltrates != true);
                        if (hitSub) return new VoidReturn();
                        if (move is ActiveMove activeMove && !target.RunImmunity(activeMove))
                            return new VoidReturn();
                        return BoolVoidUnion.FromBool(false);
                    })),
                OnEffectiveness = OnEffectivenessEventInfo.Create((_, _, target, _, move) =>
                {
                    if (target is null || move.Category != MoveCategory.Physical ||
                        target.Species.Id != SpecieId.Eiscue) return new VoidReturn();
                    bool hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                  (move.Flags.BypassSub != true) && move.Infiltrates != true;
                    if (hitSub) return new VoidReturn();
                    if (!target.RunImmunity(move)) return new VoidReturn();
                    return 0;
                }),
                OnUpdate = OnUpdateEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.Species.Id == SpecieId.Eiscue &&
                        (battle.EffectState.Busted ?? false))
                    {
                        pokemon.FormeChange(SpecieId.EiscueNoice, battle.Effect, true);
                    }
                }),
                OnWeatherChange = OnWeatherChangeEventInfo.Create((battle, pokemon, _, sourceEffect) =>
                {
                    // Snow/hail resuming because Cloud Nine/Air Lock ended does not trigger Ice Face
                    if (sourceEffect is Ability { SuppressWeather: true }) return;
                    if (pokemon.Hp == 0) return;
                    if (battle.Field.IsWeather([ConditionId.Snowscape]) &&
                        pokemon.Species.Id == SpecieId.EiscueNoice)
                    {
                        battle.Add("-activate", pokemon, "ability: Ice Face");
                        battle.EffectState.Busted = false;
                        pokemon.FormeChange(SpecieId.Eiscue, battle.Effect, true);
                    }
                }),
            },
            [AbilityId.IceScales] = new()
            {
                Id = AbilityId.IceScales,
                Name = "Ice Scales",
                Num = 246,
                Rating = 4.0,
                Flags = new AbilityFlags { Breakable = true },
                OnSourceModifyDamage =
                    OnSourceModifyDamageEventInfo.Create((battle, damage, _, _, move) =>
                    {
                        if (move.Category == MoveCategory.Special)
                        {
                            battle.ChainModify(0.5);
                            return battle.FinalModify(damage);
                        }

                        return damage;
                    }),
            },
            [AbilityId.Illuminate] = new()
            {
                Id = AbilityId.Illuminate,
                Name = "Illuminate",
                Num = 35,
                Rating = 0.5,
                Flags = new AbilityFlags { Breakable = true },
                OnTryBoost = OnTryBoostEventInfo.Create((battle, boost, target, source, effect) =>
                {
                    if (source != null && target == source) return;
                    if (boost.Accuracy is < 0)
                    {
                        boost.Accuracy = null;
                        if (effect is not ActiveMove { Secondaries: not null })
                        {
                            battle.Add("-fail", target, "unboost", "accuracy",
                                "[from] ability: Illuminate", $"[of] {target}");
                        }
                    }
                }),
                OnModifyMove = OnModifyMoveEventInfo.Create((_, move, _, _) => { move.IgnoreEvasion = true; }),
            },
            [AbilityId.Illusion] = new()
            {
                Id = AbilityId.Illusion,
                Name = "Illusion",
                Num = 149,
                Rating = 4.5,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                },
                OnBeforeSwitchIn = OnBeforeSwitchInEventInfo.Create((_, pokemon) =>
                {
                    pokemon.Illusion = null;
                    // Find a non-fainted Pokemon to the right of this Pokemon to use as illusion
                    for (int i = pokemon.Side.Pokemon.Count - 1; i > pokemon.Position; i--)
                    {
                        Pokemon possibleTarget = pokemon.Side.Pokemon[i];
                        if (!possibleTarget.Fainted)
                        {
                            // If Ogerpon/Terapagos is in the last slot while the Illusion Pokemon is Terastallized
                            // Illusion will not disguise as anything
                            if (pokemon.Terastallized == null ||
                                (possibleTarget.Species.BaseSpecies != SpecieId.Ogerpon &&
                                 possibleTarget.Species.BaseSpecies != SpecieId.Terapagos))
                            {
                                pokemon.Illusion = possibleTarget;
                            }

                            break;
                        }
                    }
                }),
                OnDamagingHit = OnDamagingHitEventInfo.Create((battle, _, target, source, move) =>
                {
                    if (target.Illusion != null)
                    {
                        battle.SingleEvent(EventId.End,
                            battle.Library.Abilities[AbilityId.Illusion],
                            target.AbilityState, target, source, move);
                    }
                }),
                OnEnd = OnEndEventInfo.Create((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                    Pokemon pokemon = psfp.Pokemon;
                    if (pokemon.Illusion != null)
                    {
                        battle.Debug("illusion cleared");
                        pokemon.Illusion = null;
                        Pokemon.PokemonDetails details = pokemon.GetUpdatedDetails();
                        battle.Add("replace", pokemon, details.ToString());
                        battle.Add("-end", pokemon, "Illusion");
                    }
                }),
                OnFaint = OnFaintEventInfo.Create((_, pokemon, _, _) => { pokemon.Illusion = null; }),
            },
            [AbilityId.Immunity] = new()
            {
                Id = AbilityId.Immunity,
                Name = "Immunity",
                Num = 17,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                OnUpdate = OnUpdateEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.Status is ConditionId.Poison or ConditionId.Toxic)
                    {
                        battle.Add("-activate", pokemon, "ability: Immunity");
                        pokemon.CureStatus();
                    }
                }),
                OnSetStatus = OnSetStatusEventInfo.Create((battle, status, target, _, effect) =>
                {
                    if (status.Id is not (ConditionId.Poison or ConditionId.Toxic))
                        return new VoidReturn();
                    if (effect is ActiveMove { Status: not ConditionId.None })
                    {
                        battle.Add("-immune", target, "[from] ability: Immunity");
                    }

                    return false;
                }),
            },
            [AbilityId.Imposter] = new()
            {
                Id = AbilityId.Imposter,
                Name = "Imposter",
                Num = 150,
                Rating = 5.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                },
                OnSwitchIn = OnSwitchInEventInfo.Create((battle, pokemon) =>
                {
                    // Imposter copies the Pokemon across from it
                    int targetIndex = pokemon.Side.Foe.Active.Count - 1 - pokemon.Position;
                    if (targetIndex >= 0 && targetIndex < pokemon.Side.Foe.Active.Count)
                    {
                        Pokemon? target = pokemon.Side.Foe.Active[targetIndex];
                        if (target != null)
                        {
                            pokemon.TransformInto(target,
                                battle.Library.Abilities[AbilityId.Imposter]);
                        }
                    }
                }),
            },
            [AbilityId.Infiltrator] = new()
            {
                Id = AbilityId.Infiltrator,
                Name = "Infiltrator",
                Num = 151,
                Rating = 2.5,
                OnModifyMove = OnModifyMoveEventInfo.Create((_, move, _, _) => { move.Infiltrates = true; }),
            },
            [AbilityId.InnardsOut] = new()
            {
                Id = AbilityId.InnardsOut,
                Name = "Innards Out",
                Num = 215,
                Rating = 4.0,
                // OnDamagingHitOrder = 1
                OnDamagingHit = OnDamagingHitEventInfo.Create((battle, damage, target, source, _) =>
                {
                    if (target.Hp == 0)
                    {
                        battle.Damage(target.GetUndynamaxedHp(damage), source, target);
                    }
                }, 1),
            },
            [AbilityId.InnerFocus] = new()
            {
                Id = AbilityId.InnerFocus,
                Name = "Inner Focus",
                Num = 39,
                Rating = 1.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryAddVolatile = OnTryAddVolatileEventInfo.Create((_, status, _, _, _) =>
                {
                    if (status.Id == ConditionId.Flinch) return null;
                    return new VoidReturn();
                }),
                OnTryBoost = OnTryBoostEventInfo.Create((battle, boost, target, _, effect) =>
                {
                    if (effect?.EffectStateId == AbilityId.Intimidate && boost.Atk != null)
                    {
                        boost.Atk = null;
                        battle.Add("-fail", target, "unboost", "Attack",
                            "[from] ability: Inner Focus", $"[of] {target}");
                    }
                }),
            },
            [AbilityId.Insomnia] = new()
            {
                Id = AbilityId.Insomnia,
                Name = "Insomnia",
                Num = 15,
                Rating = 1.5,
                Flags = new AbilityFlags { Breakable = true },
                OnUpdate = OnUpdateEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Sleep)
                    {
                        battle.Add("-activate", pokemon, "ability: Insomnia");
                        pokemon.CureStatus();
                    }
                }),
                OnSetStatus = OnSetStatusEventInfo.Create((battle, status, target, _, effect) =>
                {
                    if (status.Id != ConditionId.Sleep) return new VoidReturn();
                    if (effect is ActiveMove { Status: not ConditionId.None })
                    {
                        battle.Add("-immune", target, "[from] ability: Insomnia");
                    }

                    return false;
                }),
                OnTryAddVolatile = OnTryAddVolatileEventInfo.Create((battle, status, target, _, _) =>
                {
                    if (status.Id == ConditionId.Yawn)
                    {
                        battle.Add("-immune", target, "[from] ability: Insomnia");
                        return null;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Intimidate] = new()
            {
                Id = AbilityId.Intimidate,
                Name = "Intimidate",
                Num = 22,
                Rating = 3.5,
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    var activated = false;
                    foreach (Pokemon target in pokemon.AdjacentFoes())
                    {
                        if (!activated)
                        {
                            battle.Add("-ability", pokemon, "Intimidate", "boost");
                            activated = true;
                        }

                        if (target.Volatiles.ContainsKey(ConditionId.Substitute))
                        {
                            battle.Add("-immune", target);
                        }
                        else
                        {
                            battle.Boost(new SparseBoostsTable { Atk = -1 }, target, pokemon, null,
                                true);
                        }
                    }
                }),
            },
            [AbilityId.IntrepidSword] = new()
            {
                Id = AbilityId.IntrepidSword,
                Name = "Intrepid Sword",
                Num = 234,
                Rating = 4.0,
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.SwordBoost) return;
                    pokemon.SwordBoost = true;
                    battle.Boost(new SparseBoostsTable { Atk = 1 }, pokemon);
                }),
            },
            [AbilityId.IronBarbs] = new()
            {
                Id = AbilityId.IronBarbs,
                Name = "Iron Barbs",
                Num = 160,
                Rating = 2.5,
                // OnDamagingHitOrder = 1
                OnDamagingHit = OnDamagingHitEventInfo.Create((battle, _, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target, true))
                    {
                        battle.Damage(source.BaseMaxHp / 8, source, target);
                    }
                }, 1),
            },
            [AbilityId.IronFist] = new()
            {
                Id = AbilityId.IronFist,
                Name = "Iron Fist",
                Num = 89,
                Rating = 3.0,
                // OnBasePowerPriority = 23
                OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, _, _, move) =>
                {
                    if (move.Flags.Punch == true)
                    {
                        battle.Debug("Iron Fist boost");
                        battle.ChainModify([4915, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 23),
            },
        };
    }
}