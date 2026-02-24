using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesDef()
    {
        return new Dictionary<AbilityId, Ability>
        {
            [AbilityId.Damp] = new()
            {
                Id = AbilityId.Damp,
                Name = "Damp",
                Num = 6,
                Rating = 0.5,
                Flags = new AbilityFlags { Breakable = true },
                OnAnyTryMove = OnAnyTryMoveEventInfo.Create((battle, target, _, move) =>
                {
                    MoveId[] blockedMoves =
                    [
                        MoveId.Explosion, MoveId.MistyExplosion,
                        MoveId.SelfDestruct,
                    ];
                    if (blockedMoves.Contains(move.Id))
                    {
                        battle.AttrLastMove("[still]");
                        if (battle.EffectState.Target is PokemonEffectStateTarget
                            {
                                Pokemon: var dampHolder,
                            })
                        {
                            battle.Add("cant", dampHolder, "ability: Damp", move.Name,
                                $"[of] {target}");
                        }

                        return false;
                    }

                    return new VoidReturn();
                }),
                OnAnyDamage = OnAnyDamageEventInfo.Create((_, _, _, _, effect) =>
                {
                    // Block Aftermath damage
                    if (effect is Ability { Id: AbilityId.Aftermath })
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Dancer] = new()
            {
                Id = AbilityId.Dancer,
                Name = "Dancer",
                Num = 216,
                Rating = 1.5,
                // implementation is in Battle.RunMove
            },
            [AbilityId.DarkAura] = new()
            {
                Id = AbilityId.DarkAura,
                Name = "Dark Aura",
                Num = 186,
                Rating = 3.0,
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (battle.SuppressingAbility(pokemon)) return;
                    battle.Add("-ability", pokemon, "Dark Aura");
                }),
                OnAnyBasePower = OnAnyBasePowerEventInfo.Create(
                    (battle, basePower, source, target, move) =>
                    {
                        if (target == source || move.Category == MoveCategory.Status ||
                            move.Type != MoveType.Dark)
                            return basePower;

                        if (battle.EffectState.Target is not PokemonEffectStateTarget
                            {
                                Pokemon: var abilityHolder,
                            })
                            return basePower;

                        if (move.AuraBooster?.HasAbility(AbilityId.DarkAura) != true)
                            move.AuraBooster = abilityHolder;
                        if (move.AuraBooster != abilityHolder) return basePower;

                        battle.ChainModify(move.HasAuraBreak == true ? [3072, 4096] : [5448, 4096]);
                        return new VoidReturn();
                    }, 20),
            },
            [AbilityId.DauntlessShield] = new()
            {
                Id = AbilityId.DauntlessShield,
                Name = "Dauntless Shield",
                Num = 235,
                Rating = 3.5,
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.ShieldBoost) return;
                    pokemon.ShieldBoost = true;
                    battle.Boost(new SparseBoostsTable { Def = 1 }, pokemon);
                }),
            },
            [AbilityId.Dazzling] = new()
            {
                Id = AbilityId.Dazzling,
                Name = "Dazzling",
                Num = 219,
                Rating = 2.5,
                Flags = new AbilityFlags { Breakable = true },
                OnFoeTryMove = OnFoeTryMoveEventInfo.Create((battle, target, source, move) =>
                {
                    MoveId[] targetAllExceptions = [MoveId.PerishSong];
                    if (move.Target == MoveTarget.FoeSide ||
                        (move.Target == MoveTarget.All &&
                         !targetAllExceptions.Contains(move.Id)))
                    {
                        return new VoidReturn();
                    }

                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var dazzlingHolder,
                        })
                        return new VoidReturn();

                    if ((source.IsAlly(dazzlingHolder) || move.Target == MoveTarget.All) &&
                        move.Priority > 0.1)
                    {
                        battle.AttrLastMove("[still]");
                        battle.Add("cant", dazzlingHolder, "ability: Dazzling", move.Name,
                            $"[of] {target}");
                        return false;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Defeatist] = new()
            {
                Id = AbilityId.Defeatist,
                Name = "Defeatist",
                Num = 129,
                Rating = -1.0,
                // OnModifyAtkPriority = 5
                OnModifyAtk = OnModifyAtkEventInfo.Create((battle, atk, pokemon, _, _) =>
                {
                    if (pokemon.Hp <= pokemon.MaxHp / 2)
                    {
                        battle.ChainModify(0.5);
                        return new VoidReturn();
                    }

                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = OnModifySpAEventInfo.Create((battle, spa, pokemon, _, _) =>
                {
                    if (pokemon.Hp <= pokemon.MaxHp / 2)
                    {
                        battle.ChainModify(0.5);
                        return new VoidReturn();
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.Defiant] = new()
            {
                Id = AbilityId.Defiant,
                Name = "Defiant",
                Num = 128,
                Rating = 3.0,
                OnAfterEachBoost =
                    OnAfterEachBoostEventInfo.Create((battle, boost, target, source, _) =>
                    {
                        if (source == null || target.IsAlly(source)) return;

                        bool statsLowered = boost.Atk is < 0 || boost.Def is < 0 || boost.SpA is < 0
                                            || boost.SpD is < 0 || boost.Spe is < 0 ||
                                            boost.Accuracy is < 0
                                            || boost.Evasion is < 0;

                        if (statsLowered)
                        {
                            battle.Boost(new SparseBoostsTable { Atk = 2 }, target, target, null,
                                false, true);
                        }
                    }),
            },
            [AbilityId.DeltaStream] = new()
            {
                Id = AbilityId.DeltaStream,
                Name = "Delta Stream",
                Num = 191,
                Rating = 4.0,
                OnStart = OnStartEventInfo.Create((battle, _) => { battle.Field.SetWeather(ConditionId.DeltaStream); }),
                OnAnySetWeather = OnAnySetWeatherEventInfo.Create((battle, _, _, weather) =>
                {
                    ConditionId[] strongWeathers =
                    [
                        ConditionId.DesolateLand, ConditionId.PrimordialSea,
                        ConditionId.DeltaStream,
                    ];
                    if (battle.Field.GetWeather().Id == ConditionId.DeltaStream &&
                        !strongWeathers.Contains(weather.Id))
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
                OnEnd = OnEndEventInfo.Create((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                    Pokemon pokemon = psfp.Pokemon;
                    if (battle.Field.WeatherState.Source != pokemon) return;
                    foreach (Pokemon target in battle.GetAllActive())
                    {
                        if (target == pokemon) continue;
                        if (target.HasAbility(AbilityId.DeltaStream))
                        {
                            battle.Field.WeatherState.Source = target;
                            return;
                        }
                    }

                    battle.Field.ClearWeather();
                }),
            },
            [AbilityId.DesolateLand] = new()
            {
                Id = AbilityId.DesolateLand,
                Name = "Desolate Land",
                Num = 190,
                Rating = 4.5,
                OnStart = OnStartEventInfo.Create((battle, _) => { battle.Field.SetWeather(ConditionId.DesolateLand); }),
                OnAnySetWeather = OnAnySetWeatherEventInfo.Create((battle, _, _, weather) =>
                {
                    ConditionId[] strongWeathers =
                    [
                        ConditionId.DesolateLand, ConditionId.PrimordialSea,
                        ConditionId.DeltaStream,
                    ];
                    if (battle.Field.GetWeather().Id == ConditionId.DesolateLand &&
                        !strongWeathers.Contains(weather.Id))
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
                OnEnd = OnEndEventInfo.Create((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is not PokemonSideFieldPokemon psfp) return;
                    Pokemon pokemon = psfp.Pokemon;
                    if (battle.Field.WeatherState.Source != pokemon) return;
                    foreach (Pokemon target in battle.GetAllActive())
                    {
                        if (target == pokemon) continue;
                        if (target.HasAbility(AbilityId.DesolateLand))
                        {
                            battle.Field.WeatherState.Source = target;
                            return;
                        }
                    }

                    battle.Field.ClearWeather();
                }),
            },
            [AbilityId.Disguise] = new()
            {
                Id = AbilityId.Disguise,
                Name = "Disguise",
                Num = 209,
                Rating = 3.5,
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
                // OnDamagePriority = 1
                OnDamage = OnDamageEventInfo.Create((battle, damage, target, _, effect) =>
                {
                    if (effect is { EffectType: EffectType.Move } &&
                        target.Species.Id is SpecieId.Mimikyu or SpecieId.MimikyuTotem)
                    {
                        battle.Add("-activate", target, "ability: Disguise");
                        battle.EffectState.Busted = true;
                        return 0;
                    }

                    return damage;
                }, 1),
                OnCriticalHit = OnCriticalHitEventInfo.Create(
                    (Func<Battle, Pokemon, object?, Move, BoolVoidUnion>)((_, target, _,
                        move) =>
                    {
                        if (target is null) return new VoidReturn();
                        if (target.Species.Id != SpecieId.Mimikyu &&
                            target.Species.Id != SpecieId.MimikyuTotem)
                        {
                            return new VoidReturn();
                        }

                        bool infiltrates = move is ActiveMove { Infiltrates: true };
                        bool hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                      (move.Flags.BypassSub != true) &&
                                      !infiltrates;
                        if (hitSub) return new VoidReturn();

                        if (move is ActiveMove am && !target.RunImmunity(am))
                            return new VoidReturn();
                        return BoolVoidUnion.FromBool(false);
                    })),
                OnEffectiveness = OnEffectivenessEventInfo.Create((_, _, target, _, move) =>
                {
                    if (target is null || move.Category == MoveCategory.Status ||
                        (target.Species.Id != SpecieId.Mimikyu &&
                         target.Species.Id != SpecieId.MimikyuTotem))
                        return new VoidReturn();

                    bool hitSub = target.Volatiles.ContainsKey(ConditionId.Substitute) &&
                                  (move.Flags.BypassSub != true) && move.Infiltrates != true;
                    if (hitSub || !target.RunImmunity(move)) return new VoidReturn();

                    return 0;
                }),
                OnUpdate = OnUpdateEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.Species.Id is SpecieId.Mimikyu or SpecieId.MimikyuTotem &&
                        (battle.EffectState.Busted ?? false))
                    {
                        SpecieId bustedSpeciesId = pokemon.Species.Id == SpecieId.MimikyuTotem
                            ? SpecieId.MimikyuBustedTotem
                            : SpecieId.MimikyuBusted;
                        pokemon.FormeChange(bustedSpeciesId, battle.Effect, true);
                        battle.Damage(pokemon.BaseMaxHp / 8, pokemon, pokemon,
                            BattleDamageEffect.FromIEffect(battle.Library.Species[bustedSpeciesId]));
                    }
                }),
            },
            [AbilityId.Download] = new()
            {
                Id = AbilityId.Download,
                Name = "Download",
                Num = 88,
                Rating = 3.5,
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    var totalDef = 0;
                    var totalSpd = 0;
                    foreach (Pokemon target in pokemon.Foes())
                    {
                        totalDef += target.GetStat(StatIdExceptHp.Def, false, true);
                        totalSpd += target.GetStat(StatIdExceptHp.SpD, false, true);
                    }

                    if (totalDef > 0 && totalDef >= totalSpd)
                    {
                        battle.Boost(new SparseBoostsTable { SpA = 1 });
                    }
                    else if (totalSpd > 0)
                    {
                        battle.Boost(new SparseBoostsTable { Atk = 1 });
                    }
                }),
            },
            [AbilityId.DragonsMaw] = new()
            {
                Id = AbilityId.DragonsMaw,
                Name = "Dragon's Maw",
                Num = 263,
                Rating = 3.5,
                // OnModifyAtkPriority = 5
                OnModifyAtk = OnModifyAtkEventInfo.Create((battle, atk, _, _, move) =>
                {
                    if (move.Type == MoveType.Dragon)
                    {
                        battle.Debug("Dragon's Maw boost");
                        battle.ChainModify(1.5);
                        return new VoidReturn();
                    }

                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = OnModifySpAEventInfo.Create((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Dragon)
                    {
                        battle.Debug("Dragon's Maw boost");
                        battle.ChainModify(1.5);
                        return new VoidReturn();
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.Drizzle] = new()
            {
                Id = AbilityId.Drizzle,
                Name = "Drizzle",
                Num = 2,
                Rating = 4.0,
                OnStart = OnStartEventInfo.Create((battle, _) =>
                {
                    // Note: Primal Reversion was removed in Gen 9
                    battle.Field.SetWeather(_library.Conditions[ConditionId.RainDance]);
                }),
            },
            [AbilityId.Drought] = new()
            {
                Id = AbilityId.Drought,
                Name = "Drought",
                Num = 70,
                Rating = 4.0,
                OnStart = OnStartEventInfo.Create((battle, _) =>
                {
                    // Note: Primal Reversion was removed in Gen 9
                    battle.Field.SetWeather(_library.Conditions[ConditionId.SunnyDay]);
                }),
            },
            [AbilityId.DrySkin] = new()
            {
                Id = AbilityId.DrySkin,
                Name = "Dry Skin",
                Num = 87,
                Rating = 3.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = OnTryHitEventInfo.Create((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Water)
                    {
                        IntFalseUnion healResult = battle.Heal(target.BaseMaxHp / 4, target);
                        if (healResult is FalseIntFalseUnion)
                        {
                            battle.Add("-immune", target, "[from] ability: Dry Skin");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }),
                // OnSourceBasePowerPriority = 17
                OnSourceBasePower = OnSourceBasePowerEventInfo.Create(
                    (battle, basePower, _, _, move) =>
                    {
                        if (move.Type == MoveType.Fire)
                        {
                            battle.ChainModify(1.25);
                            return new VoidReturn();
                        }

                        return basePower;
                    }, 17),
                OnWeather = OnWeatherEventInfo.Create((battle, target, _, effect) =>
                {
                    if (target.HasItem(ItemId.UtilityUmbrella)) return;
                    if (effect.Id is ConditionId.RainDance or ConditionId.PrimordialSea)
                    {
                        battle.Heal(target.BaseMaxHp / 8, target);
                    }
                    else if (effect.Id is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        battle.Damage(target.BaseMaxHp / 8, target, target);
                    }
                }),
            },
            [AbilityId.EarlyBird] = new()
            {
                Id = AbilityId.EarlyBird,
                Name = "Early Bird",
                Num = 48,
                Rating = 1.5,
                // Implementation is in statuses.ts (Sleep condition)
                // Early Bird causes sleep to count down twice as fast
            },
            [AbilityId.EarthEater] = new()
            {
                Id = AbilityId.EarthEater,
                Name = "Earth Eater",
                Num = 297,
                Rating = 3.5,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = OnTryHitEventInfo.Create((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Ground)
                    {
                        IntFalseUnion healResult = battle.Heal(target.BaseMaxHp / 4, target);
                        if (healResult is FalseIntFalseUnion)
                        {
                            battle.Add("-immune", target, "[from] ability: Earth Eater");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.EffectSpore] = new()
            {
                Id = AbilityId.EffectSpore,
                Name = "Effect Spore",
                Num = 27,
                Rating = 2.0,
                OnDamagingHit = OnDamagingHitEventInfo.Create((battle, _, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target) &&
                        source.Status == ConditionId.None &&
                        source.RunStatusImmunity(ConditionId.Powder))
                    {
                        int r = battle.Random(100);
                        if (r < 11)
                        {
                            source.SetStatus(ConditionId.Sleep, target);
                        }
                        else if (r < 21)
                        {
                            source.SetStatus(ConditionId.Paralysis, target);
                        }
                        else if (r < 30)
                        {
                            source.SetStatus(ConditionId.Poison, target);
                        }
                    }
                }),
            },
            [AbilityId.ElectricSurge] = new()
            {
                Id = AbilityId.ElectricSurge,
                Name = "Electric Surge",
                Num = 226,
                Rating = 4.0,
                OnStart = OnStartEventInfo.Create((battle, _) =>
                {
                    battle.Field.SetTerrain(_library.Conditions[ConditionId.ElectricTerrain]);
                }),
            },
            [AbilityId.Electromorphosis] = new()
            {
                Id = AbilityId.Electromorphosis,
                Name = "Electromorphosis",
                Num = 280,
                Rating = 3.0,
                // OnDamagingHitOrder = 1
                OnDamagingHit = OnDamagingHitEventInfo.Create(
                    (_, _, target, _, _) => { target.AddVolatile(ConditionId.Charge); }, 1),
            },
            [AbilityId.EmbodyAspectCornerstone] = new()
            {
                Id = AbilityId.EmbodyAspectCornerstone,
                Name = "Embody Aspect (Cornerstone)",
                Num = 304,
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
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.BaseSpecies.Id == SpecieId.OgerponCornerstoneTera &&
                        pokemon.Terastallized != null &&
                        !(battle.EffectState.Embodied ?? false))
                    {
                        battle.EffectState.Embodied = true;
                        battle.Boost(new SparseBoostsTable { Def = 1 }, pokemon);
                    }
                }),
            },
            [AbilityId.EmbodyAspectHearthflame] = new()
            {
                Id = AbilityId.EmbodyAspectHearthflame,
                Name = "Embody Aspect (Hearthflame)",
                Num = 303,
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
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.BaseSpecies.Id == SpecieId.OgerponHearthflameTera &&
                        pokemon.Terastallized != null &&
                        !(battle.EffectState.Embodied ?? false))
                    {
                        battle.EffectState.Embodied = true;
                        battle.Boost(new SparseBoostsTable { Atk = 1 }, pokemon);
                    }
                }),
            },
            [AbilityId.EmbodyAspectTeal] = new()
            {
                Id = AbilityId.EmbodyAspectTeal,
                Name = "Embody Aspect (Teal)",
                Num = 301,
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
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.BaseSpecies.Id == SpecieId.OgerponTealTera &&
                        pokemon.Terastallized != null &&
                        !(battle.EffectState.Embodied ?? false))
                    {
                        battle.EffectState.Embodied = true;
                        battle.Boost(new SparseBoostsTable { Spe = 1 }, pokemon);
                    }
                }),
            },
            [AbilityId.EmbodyAspectWellspring] = new()
            {
                Id = AbilityId.EmbodyAspectWellspring,
                Name = "Embody Aspect (Wellspring)",
                Num = 302,
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
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.BaseSpecies.Id == SpecieId.OgerponWellspringTera &&
                        pokemon.Terastallized != null &&
                        !(battle.EffectState.Embodied ?? false))
                    {
                        battle.EffectState.Embodied = true;
                        battle.Boost(new SparseBoostsTable { SpD = 1 }, pokemon);
                    }
                }),
            },
            [AbilityId.EmergencyExit] = new()
            {
                Id = AbilityId.EmergencyExit,
                Name = "Emergency Exit",
                Num = 194,
                Rating = 1.0,
                OnEmergencyExit = OnEmergencyExitEventInfo.Create((battle, target) =>
                {
                    if (battle.CanSwitch(target.Side) <= 0 || target.ForceSwitchFlag ||
                        target.SwitchFlag.IsTrue())
                        return;

                    // Clear all switch flags
                    foreach (Side side in battle.Sides)
                    {
                        foreach (Pokemon? active in side.Active)
                        {
                            if (active != null)
                                active.SwitchFlag = false;
                        }
                    }

                    target.SwitchFlag = true;
                    battle.Add("-activate", target, "ability: Emergency Exit");
                }),
            },
            [AbilityId.FairyAura] = new()
            {
                Id = AbilityId.FairyAura,
                Name = "Fairy Aura",
                Num = 187,
                Rating = 3.0,
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (battle.SuppressingAbility(pokemon)) return;
                    battle.Add("-ability", pokemon, "Fairy Aura");
                }),
                OnAnyBasePower = OnAnyBasePowerEventInfo.Create(
                    (battle, basePower, source, target, move) =>
                    {
                        if (target == source || move.Category == MoveCategory.Status ||
                            move.Type != MoveType.Fairy)
                            return basePower;

                        if (battle.EffectState.Target is not PokemonEffectStateTarget
                            {
                                Pokemon: var abilityHolder,
                            })
                            return basePower;

                        if (move.AuraBooster?.HasAbility(AbilityId.FairyAura) != true)
                            move.AuraBooster = abilityHolder;
                        if (move.AuraBooster != abilityHolder) return basePower;

                        battle.ChainModify(move.HasAuraBreak == true ? [3072, 4096] : [5448, 4096]);
                        return new VoidReturn();
                    }, 20),
            },
            [AbilityId.Filter] = new()
            {
                Id = AbilityId.Filter,
                Name = "Filter",
                Num = 111,
                Rating = 3.0,
                Flags = new AbilityFlags { Breakable = true },
                OnSourceModifyDamage =
                    OnSourceModifyDamageEventInfo.Create((battle, damage, _, target, move) =>
                    {
                        if (target.GetMoveHitData(move).TypeMod > 0)
                        {
                            battle.Debug("Filter neutralize");
                            battle.ChainModify(0.75);
                            return new VoidReturn();
                        }

                        return damage;
                    }),
            },
            [AbilityId.FlameBody] = new()
            {
                Id = AbilityId.FlameBody,
                Name = "Flame Body",
                Num = 49,
                Rating = 2.0,
                OnDamagingHit = OnDamagingHitEventInfo.Create((battle, _, target, source, move) =>
                {
                    if (!battle.CheckMoveMakesContact(move, source, target)) return;

                    if (battle.RandomChance(3, 10))
                    {
                        source.TrySetStatus(ConditionId.Burn, target);
                    }
                }),
            },
            [AbilityId.FlareBoost] = new()
            {
                Id = AbilityId.FlareBoost,
                Name = "Flare Boost",
                Num = 138,
                Rating = 2.0,
                // OnBasePowerPriority = 19
                OnBasePower = OnBasePowerEventInfo.Create((battle, basePower, attacker, _, move) =>
                {
                    if (attacker.Status == ConditionId.Burn &&
                        move.Category == MoveCategory.Special)
                    {
                        battle.ChainModify(1.5);
                        return new VoidReturn();
                    }

                    return basePower;
                }, 19),
            },
            [AbilityId.FlashFire] = new()
            {
                Id = AbilityId.FlashFire,
                Name = "Flash Fire",
                Num = 18,
                Rating = 3.5,
                Flags = new AbilityFlags { Breakable = true },
                Condition = ConditionId.FlashFire,
                OnTryHit = OnTryHitEventInfo.Create((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Fire)
                    {
                        // Note: In TS, move.accuracy is set to true here
                        // We cannot modify init-only Accuracy, but the hit will still be blocked
                        RelayVar addResult = target.AddVolatile(ConditionId.FlashFire);
                        if (addResult is BoolRelayVar { Value: false })
                        {
                            battle.Add("-immune", target, "[from] ability: Flash Fire");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }),
                OnEnd = OnEndEventInfo.Create((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is PokemonSideFieldPokemon { Pokemon: var pokemon })
                    {
                        pokemon.RemoveVolatile(battle.Library.Conditions[ConditionId.FlashFire]);
                    }
                }),
            },
            [AbilityId.FlowerGift] = new()
            {
                Id = AbilityId.FlowerGift,
                Name = "Flower Gift",
                Num = 122,
                Rating = 1.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    Breakable = true,
                },
                // OnSwitchInPriority = -2
                OnSwitchIn = OnSwitchInEventInfo.Create((_, _) =>
                {
                    // Trigger the weather check to potentially change forme
                    // This is implemented via OnStart which calls OnWeatherChange
                }, -2),
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    // Trigger the weather change event to potentially change forme
                    // OnStart delegates to the weather change logic
                    if (!pokemon.IsActive ||
                        pokemon.BaseSpecies.BaseSpecies != SpecieId.Cherrim ||
                        pokemon.Transformed) return;
                    if (pokemon.Hp == 0) return;

                    ConditionId? weather = pokemon.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        if (pokemon.Species.Id != SpecieId.CherrimSunshine)
                        {
                            pokemon.FormeChange(SpecieId.CherrimSunshine, battle.Effect, false,
                                message: "[msg]");
                        }
                    }
                    else
                    {
                        if (pokemon.Species.Id == SpecieId.CherrimSunshine)
                        {
                            pokemon.FormeChange(SpecieId.Cherrim, battle.Effect, false,
                                message: "[msg]");
                        }
                    }
                }),
                OnWeatherChange = OnWeatherChangeEventInfo.Create((battle, pokemon, _, _) =>
                {
                    if (!pokemon.IsActive ||
                        pokemon.BaseSpecies.BaseSpecies != SpecieId.Cherrim ||
                        pokemon.Transformed) return;
                    if (pokemon.Hp == 0) return;

                    ConditionId? weather = pokemon.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        if (pokemon.Species.Id != SpecieId.CherrimSunshine)
                        {
                            pokemon.FormeChange(SpecieId.CherrimSunshine, battle.Effect, false,
                                message: "[msg]");
                        }
                    }
                    else
                    {
                        if (pokemon.Species.Id == SpecieId.CherrimSunshine)
                        {
                            pokemon.FormeChange(SpecieId.Cherrim, battle.Effect, false,
                                message: "[msg]");
                        }
                    }
                }),
                // OnAllyModifyAtkPriority = 3
                OnAllyModifyAtk = OnAllyModifyAtkEventInfo.Create((battle, atk, pokemon, _, _) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var flowerGiftHolder,
                        })
                        return atk;
                    if (flowerGiftHolder.BaseSpecies.BaseSpecies != SpecieId.Cherrim) return atk;
                    ConditionId? weather = pokemon.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        battle.ChainModify(1.5);
                        return new VoidReturn();
                    }

                    return atk;
                }, 3),
                // OnAllyModifySpDPriority = 4
                OnAllyModifySpD = OnAllyModifySpDEventInfo.Create((battle, spd, pokemon, _, _) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var flowerGiftHolder,
                        })
                        return spd;
                    if (flowerGiftHolder.BaseSpecies.BaseSpecies != SpecieId.Cherrim) return spd;
                    ConditionId? weather = pokemon.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        battle.ChainModify(1.5);
                        return new VoidReturn();
                    }

                    return spd;
                }, 4),
            },
            [AbilityId.FlowerVeil] = new()
            {
                Id = AbilityId.FlowerVeil,
                Name = "Flower Veil",
                Num = 166,
                Rating = 0.0,
                Flags = new AbilityFlags { Breakable = true },
                OnAllyTryBoost =
                    OnAllyTryBoostEventInfo.Create((battle, boost, target, source, effect) =>
                    {
                        if ((source != null && target == source) ||
                            !target.HasType(PokemonType.Grass)) return;
                        var showMsg = false;
                        if (boost.Atk is < 0)
                        {
                            boost.Atk = null;
                            showMsg = true;
                        }

                        if (boost.Def is < 0)
                        {
                            boost.Def = null;
                            showMsg = true;
                        }

                        if (boost.SpA is < 0)
                        {
                            boost.SpA = null;
                            showMsg = true;
                        }

                        if (boost.SpD is < 0)
                        {
                            boost.SpD = null;
                            showMsg = true;
                        }

                        if (boost.Spe is < 0)
                        {
                            boost.Spe = null;
                            showMsg = true;
                        }

                        if (boost.Accuracy is < 0)
                        {
                            boost.Accuracy = null;
                            showMsg = true;
                        }

                        if (boost.Evasion is < 0)
                        {
                            boost.Evasion = null;
                            showMsg = true;
                        }

                        if (showMsg && effect is not ActiveMove { Secondaries.Length: > 0 })
                        {
                            if (battle.EffectState.Target is PokemonEffectStateTarget
                                {
                                    Pokemon: var effectHolder,
                                })
                            {
                                battle.Add("-block", target, "ability: Flower Veil",
                                    $"[of] {effectHolder}");
                            }
                        }
                    }),
                OnAllySetStatus =
                    OnAllySetStatusEventInfo.Create((battle, status, target, source, effect) =>
                    {
                        // Check if effect is Yawn
                        bool isYawn = effect?.EffectStateId == ConditionId.Yawn ||
                                      status.Id == ConditionId.Yawn;
                        if (!target.HasType(PokemonType.Grass) || source == null ||
                            target == source ||
                            effect == null || isYawn)
                            return new VoidReturn();
                        battle.Debug("interrupting setStatus with Flower Veil");
                        if (effect.EffectStateId == AbilityId.Synchronize ||
                            (effect.EffectType == EffectType.Move &&
                             effect is ActiveMove
                             {
                                 Secondaries: null or { Length: 0 },
                             }))
                        {
                            if (battle.EffectState.Target is PokemonEffectStateTarget
                                {
                                    Pokemon: var effectHolder,
                                })
                            {
                                battle.Add("-block", target, "ability: Flower Veil",
                                    $"[of] {effectHolder}");
                            }
                        }

                        return null;
                    }),
                OnAllyTryAddVolatile =
                    OnAllyTryAddVolatileEventInfo.Create((battle, status, target, _, _) =>
                    {
                        if (target.HasType(PokemonType.Grass) && status.Id == ConditionId.Yawn)
                        {
                            battle.Debug("Flower Veil blocking yawn");
                            if (battle.EffectState.Target is PokemonEffectStateTarget
                                {
                                    Pokemon: var effectHolder,
                                })
                            {
                                battle.Add("-block", target, "ability: Flower Veil",
                                    $"[of] {effectHolder}");
                            }

                            return null;
                        }

                        return new VoidReturn();
                    }),
            },
            [AbilityId.Fluffy] = new()
            {
                Id = AbilityId.Fluffy,
                Name = "Fluffy",
                Num = 218,
                Rating = 3.5,
                Flags = new AbilityFlags { Breakable = true },
                OnSourceModifyDamage =
                    OnSourceModifyDamageEventInfo.Create((battle, damage, _, _, move) =>
                    {
                        var mod = 1.0;
                        if (move.Type == MoveType.Fire) mod *= 2;
                        if (move.Flags.Contact == true) mod /= 2;
                        battle.ChainModify(mod);
                        return new VoidReturn();
                    }),
            },
            [AbilityId.Forecast] = new()
            {
                Id = AbilityId.Forecast,
                Name = "Forecast",
                Num = 59,
                Rating = 2.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                },
                // OnSwitchInPriority = -2
                OnSwitchIn = OnSwitchInEventInfo.Create((_, _) =>
                {
                    // Trigger the weather check to potentially change forme
                    // This is implemented via OnStart which calls OnWeatherChange
                }, -2),
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    if (pokemon.BaseSpecies.BaseSpecies != SpecieId.Castform ||
                        pokemon.Transformed) return;

                    ConditionId? weather = pokemon.EffectiveWeather();
                    SpecieId targetForme = weather switch
                    {
                        ConditionId.SunnyDay or ConditionId.DesolateLand => SpecieId.CastformSunny,
                        ConditionId.RainDance or ConditionId.PrimordialSea =>
                            SpecieId.CastformRainy,
                        ConditionId.Snowscape => SpecieId.CastformSnowy,
                        _ => SpecieId.Castform,
                    };

                    if (pokemon.IsActive && pokemon.Species.Id != targetForme)
                    {
                        pokemon.FormeChange(targetForme, battle.Effect, false, message: "[msg]");
                    }
                }),
                OnWeatherChange = OnWeatherChangeEventInfo.Create((battle, pokemon, _, _) =>
                {
                    if (pokemon.BaseSpecies.BaseSpecies != SpecieId.Castform ||
                        pokemon.Transformed) return;

                    ConditionId? weather = pokemon.EffectiveWeather();
                    SpecieId targetForme = weather switch
                    {
                        ConditionId.SunnyDay or ConditionId.DesolateLand => SpecieId.CastformSunny,
                        ConditionId.RainDance or ConditionId.PrimordialSea =>
                            SpecieId.CastformRainy,
                        ConditionId.Snowscape => SpecieId.CastformSnowy,
                        _ => SpecieId.Castform,
                    };

                    if (pokemon.IsActive && pokemon.Species.Id != targetForme)
                    {
                        pokemon.FormeChange(targetForme, battle.Effect, false, message: "[msg]");
                    }
                }),
            },
            [AbilityId.Forewarn] = new()
            {
                Id = AbilityId.Forewarn,
                Name = "Forewarn",
                Num = 108,
                Rating = 0.5,
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    List<(Move Move, Pokemon Target)> warnMoves = [];
                    var warnBp = 1;
                    foreach (Pokemon target in pokemon.Foes())
                    {
                        foreach (MoveSlot moveSlot in target.MoveSlots)
                        {
                            Move move = battle.Library.Moves[moveSlot.Id];
                            int bp = move.BasePower;
                            if (move.Ohko != null) bp = 150;
                            MoveId[] counterMoves =
                                [MoveId.Counter, MoveId.MetalBurst, MoveId.MirrorCoat];
                            if (counterMoves.Contains(move.Id)) bp = 120;
                            if (bp == 1) bp = 80;
                            if (bp == 0 && move.Category != MoveCategory.Status) bp = 80;
                            if (bp > warnBp)
                            {
                                warnMoves = [(move, target)];
                                warnBp = bp;
                            }
                            else if (bp == warnBp)
                            {
                                warnMoves.Add((move, target));
                            }
                        }
                    }

                    if (warnMoves.Count == 0) return;
                    (Move warnMove, Pokemon warnTarget) = battle.Sample(warnMoves);
                    battle.Add("-activate", pokemon, "ability: Forewarn", warnMove.Name,
                        $"[of] {warnTarget}");
                }),
            },
            [AbilityId.FriendGuard] = new()
            {
                Id = AbilityId.FriendGuard,
                Name = "Friend Guard",
                Num = 132,
                Rating = 0.0,
                Flags = new AbilityFlags { Breakable = true },
                OnAnyModifyDamage = OnAnyModifyDamageEventInfo.Create((battle, damage, _, target, _) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var effectTarget,
                        })
                        return damage;
                    if (target != effectTarget && target.IsAlly(effectTarget))
                    {
                        battle.Debug("Friend Guard weaken");
                        battle.ChainModify(0.75);
                        return new VoidReturn();
                    }

                    return damage;
                }),
            },
            [AbilityId.Frisk] = new()
            {
                Id = AbilityId.Frisk,
                Name = "Frisk",
                Num = 119,
                Rating = 1.5,
                OnStart = OnStartEventInfo.Create((battle, pokemon) =>
                {
                    foreach (Pokemon target in pokemon.Foes()
                                 .Where(target => target.Item != ItemId.None))
                    {
                        battle.Add("-item", target, battle.Library.Items[target.Item].Name,
                            "[from] ability: Frisk", $"[of] {pokemon}");
                    }
                }),
            },
            [AbilityId.FullMetalBody] = new()
            {
                Id = AbilityId.FullMetalBody,
                Name = "Full Metal Body",
                Num = 230,
                Rating = 2.0,
                OnTryBoost = OnTryBoostEventInfo.Create((battle, boost, target, source, effect) =>
                {
                    if (source != null && target == source) return;
                    var showMsg = false;
                    if (boost.Atk is < 0)
                    {
                        boost.Atk = null;
                        showMsg = true;
                    }

                    if (boost.Def is < 0)
                    {
                        boost.Def = null;
                        showMsg = true;
                    }

                    if (boost.SpA is < 0)
                    {
                        boost.SpA = null;
                        showMsg = true;
                    }

                    if (boost.SpD is < 0)
                    {
                        boost.SpD = null;
                        showMsg = true;
                    }

                    if (boost.Spe is < 0)
                    {
                        boost.Spe = null;
                        showMsg = true;
                    }

                    if (boost.Accuracy is < 0)
                    {
                        boost.Accuracy = null;
                        showMsg = true;
                    }

                    if (boost.Evasion is < 0)
                    {
                        boost.Evasion = null;
                        showMsg = true;
                    }

                    // Show message if stats were lowered and effect doesn't have secondaries
                    // Note: Octolock check removed as it's not in Gen 9 (isNonstandard: Past)
                    if (showMsg && effect is not ActiveMove { Secondaries.Length: > 0 })
                    {
                        battle.Add("-fail", target, "unboost", "[from] ability: Full Metal Body",
                            $"[of] {target}");
                    }
                }),
            },
            [AbilityId.FurCoat] = new()
            {
                Id = AbilityId.FurCoat,
                Name = "Fur Coat",
                Num = 169,
                Rating = 4.0,
                Flags = new AbilityFlags { Breakable = true },
                // OnModifyDefPriority = 6
                OnModifyDef = OnModifyDefEventInfo.Create((battle, def, _, _, _) =>
                {
                    battle.ChainModify(2);
                    return new VoidReturn();
                }, 6),
            },
        };
    }
}