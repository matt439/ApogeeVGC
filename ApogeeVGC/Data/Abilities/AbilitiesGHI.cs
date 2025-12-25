using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

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
                    new OnModifyPriorityEventInfo((battle, priority, pokemon, _, move) =>
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
                OnModifyType = new OnModifyTypeEventInfo((battle, move, pokemon, _) =>
                {
                    // TODO: Add checks for specific moves like Judgment, Multi-Attack, etc.
                    if (move.Type == MoveType.Normal && move.Category != MoveCategory.Status &&
                        !(move.Name == "Tera Blast" && pokemon.Terastallized != null))
                    {
                        move.Type = MoveType.Electric;
                        move.TypeChangerBoosted = battle.Effect;
                    }
                }, -1),
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
            [AbilityId.Gluttony] = new()
            {
                Id = AbilityId.Gluttony,
                Name = "Gluttony",
                Num = 82,
                Rating = 1.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    pokemon.AbilityState.Gluttony = true;
                }),
                OnDamage = new OnDamageEventInfo((battle, _, target, _, _) =>
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
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
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
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
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
                OnStart = new OnStartEventInfo((_, pokemon) =>
                {
                    pokemon.AbilityState.ChoiceLock = null;
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, move) =>
                {
                    if (move.Id == MoveId.Struggle) return new VoidReturn();
                    MoveId? choiceLock = pokemon.AbilityState.ChoiceLock;
                    if (choiceLock != null && choiceLock != move.Id)
                    {
                        // Fails unless ability is being ignored
                        // Note: In TS this uses this.addMove but we use Add directly
                        battle.Add("move", pokemon, move.Name);
                        battle.AttrLastMove("[still]");
                        battle.Debug("Disabled by Gorilla Tactics");
                        battle.Add("-fail", pokemon);
                        return false;
                    }

                    return new VoidReturn();
                }),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    if (pokemon.AbilityState.ChoiceLock != null || move.Id == MoveId.Struggle)
                        return;
                    pokemon.AbilityState.ChoiceLock = move.Id;
                }),
                // OnModifyAtkPriority = 1
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
                {
                    // PLACEHOLDER - Skip if dynamaxed (not relevant for Gen 9)
                    battle.Debug("Gorilla Tactics Atk Boost");
                    battle.ChainModify(1.5);
                    return battle.FinalModify(atk);
                }, 1),
                OnDisableMove = new OnDisableMoveEventInfo((battle, pokemon) =>
                {
                    MoveId? choiceLock = pokemon.AbilityState.ChoiceLock;
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
                OnEnd = new OnEndEventInfo((_, pokemonUnion) =>
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
                OnModifyDef = new OnModifyDefEventInfo((battle, def, _, _, _) =>
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
                OnStart = new OnStartEventInfo((battle, _) =>
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
                    new OnSourceAfterFaintEventInfo((battle, length, _, source, effect) =>
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
                OnDragOut = new OnDragOutEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Add("-activate", pokemon, "ability: Guard Dog");
                    // Return null to prevent the drag out - handled differently in C#
                    // The event system should interpret this as blocking the drag out
                }, 1),
                // OnTryBoostPriority = 2
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, _, effect) =>
                {
                    if (effect.Name == "Intimidate" && boost.Atk != null)
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
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
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
                    new OnSourceTryPrimaryHitEventInfo((battle, _, source, move) =>
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
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, pokemon, _, _) =>
                    {
                        if (pokemon.Status is not ConditionId.None)
                        {
                            battle.ChainModify(1.5);
                            return battle.FinalModify(atk);
                        }

                        return atk;
                    },
                    5),
            },
            [AbilityId.HadronEngine] = new()
            {
                Id = AbilityId.HadronEngine,
                Name = "Hadron Engine",
                Num = 289,
                Rating = 4.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    battle.Debug($"[HadronEngine.OnStart] HANDLER EXECUTING for {pokemon.Name}");

                    bool terrainSet = battle.Field.SetTerrain(
                        _library.Conditions[ConditionId.ElectricTerrain]);

                    battle.Debug($"[HadronEngine.OnStart] SetTerrain returned: {terrainSet}");
                    battle.Debug($"[HadronEngine.OnStart] Current terrain: {battle.Field.Terrain}");

                    if (!terrainSet &&
                        battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        battle.Debug(
                            $"[HadronEngine.OnStart] Terrain already Electric, showing activate");
                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "ability: Hadron Engine");
                        }
                    }
                }),
                //OnModifySpAPriority = 5,
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, _) =>
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
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    // 50% chance normally, 100% in sun
                    bool canHarvest =
                        battle.Field.IsWeather([ConditionId.SunnyDay, ConditionId.DesolateLand]) ||
                        battle.RandomChance(1, 2);

                    if (!canHarvest) return;
                    if (pokemon.Hp == 0) return;
                    if (pokemon.Item != null) return;

                    var lastItem = battle.Library.Items.GetValueOrDefault(pokemon.LastItem);
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
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    foreach (var allyActive in pokemon.AdjacentAllies())
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
                OnSourceModifyAtk = new OnSourceModifyAtkEventInfo((battle, atk, _, _, move) =>
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
                OnSourceModifySpA = new OnSourceModifySpAEventInfo((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        battle.Debug("Heatproof SpA weaken");
                        battle.ChainModify(0.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
                OnDamage = new OnDamageEventInfo((battle, damage, target, _, effect) =>
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
                OnModifyWeight = new OnModifyWeightEventInfo((_, weighthg, _) => weighthg * 2, 1),
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
                OnSwitchIn = new OnSwitchInEventInfo((_, _) => { }, -2),
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    foreach (var ally in pokemon.AdjacentAllies())
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
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, _, _, _) =>
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
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
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
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, _, _, _) =>
                {
                    // Apply 1.5x directly (not chain)
                    return (int)(atk * 1.5);
                }, 5),
                // OnSourceModifyAccuracyPriority = -1
                OnSourceModifyAccuracy = new OnSourceModifyAccuracyEventInfo(
                    (battle, accuracy, _, _, move) =>
                    {
                        if (move.Category == MoveCategory.Physical)
                        {
                            battle.ChainModify([3277, 4096]);
                            return battle.FinalModify(accuracy);
                        }

                        return accuracy;
                    }, -1),
            },
            [AbilityId.Hydration] = new()
            {
                Id = AbilityId.Hydration,
                Name = "Hydration",
                Num = 93,
                Rating = 1.5,
                // OnResidualOrder = 5, OnResidualSubOrder = 3
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.Status != ConditionId.None &&
                        battle.Field.IsWeather([
                            ConditionId.RainDance, ConditionId.PrimordialSea
                        ]) &&
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
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, source, effect) =>
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
        };
    }
}