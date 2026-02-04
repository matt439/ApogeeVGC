using ApogeeVGC.Sim.Abilities;
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
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesStu()
    {
        return new Dictionary<AbilityId, Ability>
        {
            // ==================== 'S' Abilities ====================
            [AbilityId.SandForce] = new()
            {
                Id = AbilityId.SandForce,
                Name = "Sand Force",
                Num = 159,
                Rating = 2.0,
                // OnBasePowerPriority = 21
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (battle.Field.IsWeather(ConditionId.Sandstorm))
                    {
                        if (move.Type is MoveType.Rock or MoveType.Ground or MoveType.Steel)
                        {
                            battle.Debug("Sand Force boost");
                            battle.ChainModify([5325, 4096]);
                            return battle.FinalModify(basePower);
                        }
                    }

                    return basePower;
                }, 21),
                OnImmunity = new OnImmunityEventInfo((_, type, _) =>
                {
                    if (type is { IsConditionId: true, AsConditionId: ConditionId.Sandstorm })
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.SandRush] = new()
            {
                Id = AbilityId.SandRush,
                Name = "Sand Rush",
                Num = 146,
                Rating = 3.0,
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, _) =>
                {
                    if (battle.Field.IsWeather(ConditionId.Sandstorm))
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spe);
                    }

                    return spe;
                }),
                OnImmunity = new OnImmunityEventInfo((_, type, _) =>
                {
                    if (type is { IsConditionId: true, AsConditionId: ConditionId.Sandstorm })
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.SandSpit] = new()
            {
                Id = AbilityId.SandSpit,
                Name = "Sand Spit",
                Num = 245,
                Rating = 1.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, _, _, _) =>
                {
                    battle.Field.SetWeather(_library.Conditions[ConditionId.Sandstorm]);
                }),
            },
            [AbilityId.SandStream] = new()
            {
                Id = AbilityId.SandStream,
                Name = "Sand Stream",
                Num = 45,
                Rating = 4.0,
                OnStart = new OnStartEventInfo((battle, _) =>
                {
                    battle.Field.SetWeather(_library.Conditions[ConditionId.Sandstorm]);
                }),
            },
            [AbilityId.SandVeil] = new()
            {
                Id = AbilityId.SandVeil,
                Name = "Sand Veil",
                Num = 8,
                Rating = 1.5,
                Flags = new AbilityFlags { Breakable = true },
                OnImmunity = new OnImmunityEventInfo((_, type, _) =>
                {
                    if (type is { IsConditionId: true, AsConditionId: ConditionId.Sandstorm })
                    {
                        return false;
                    }

                    return new VoidReturn();
                }),
                // OnModifyAccuracyPriority = -1
                OnModifyAccuracy = new OnModifyAccuracyEventInfo((battle, accuracy, _, _, _) =>
                {
                    // Only modify numeric accuracy
                    if (accuracy.HasValue && battle.Field.IsWeather(ConditionId.Sandstorm))
                    {
                        battle.Debug("Sand Veil - decreasing accuracy");
                        return battle.ChainModify([3277, 4096]);
                    }

                    return new VoidReturn();
                }, -1),
            },
            [AbilityId.SapSipper] = new()
            {
                Id = AbilityId.SapSipper,
                Name = "Sap Sipper",
                Num = 157,
                Rating = 3.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Grass)
                    {
                        if (!(battle.Boost(new SparseBoostsTable { Atk = 1 })?.IsTruthy() ?? false))
                        {
                            battle.Add("-immune", target, "[from] ability: Sap Sipper");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }, 1),
                OnAllyTryHitSide = new OnAllyTryHitSideEventInfo((battle, target, source, move) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder,
                        } || source == abilityHolder || !target.IsAlly(source))
                        return new VoidReturn();
                    if (move.Type == MoveType.Grass)
                    {
                        battle.Boost(new SparseBoostsTable { Atk = 1 }, abilityHolder);
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Schooling] = new()
            {
                Id = AbilityId.Schooling,
                Name = "Schooling",
                Num = 208,
                Rating = 3.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                },
                // OnSwitchInPriority = -1
                OnStart = new OnStartEventInfo((_, pokemon) =>
                {
                    if (pokemon.BaseSpecies.BaseSpecies != SpecieId.Wishiwashi ||
                        pokemon.Level < 20 || pokemon.Transformed)
                        return;

                    if (pokemon.Hp > pokemon.MaxHp / 4)
                    {
                        if (pokemon.Species.Id == SpecieId.Wishiwashi)
                        {
                            pokemon.FormeChange(SpecieId.WishiwashiSchool);
                        }
                    }
                    else
                    {
                        if (pokemon.Species.Id == SpecieId.WishiwashiSchool)
                        {
                            pokemon.FormeChange(SpecieId.Wishiwashi);
                        }
                    }
                }, -1),
                // OnResidualOrder = 29
                OnResidual = new OnResidualEventInfo((_, pokemon, _, _) =>
                {
                    if (pokemon.BaseSpecies.BaseSpecies != SpecieId.Wishiwashi ||
                        pokemon.Level < 20 || pokemon.Transformed || pokemon.Hp == 0)
                        return;

                    if (pokemon.Hp > pokemon.MaxHp / 4)
                    {
                        if (pokemon.Species.Id == SpecieId.Wishiwashi)
                        {
                            pokemon.FormeChange(SpecieId.WishiwashiSchool);
                        }
                    }
                    else
                    {
                        if (pokemon.Species.Id == SpecieId.WishiwashiSchool)
                        {
                            pokemon.FormeChange(SpecieId.Wishiwashi);
                        }
                    }
                }, order: 29),
            },
            [AbilityId.Scrappy] = new()
            {
                Id = AbilityId.Scrappy,
                Name = "Scrappy",
                Num = 113,
                Rating = 3.0,
                // OnModifyMovePriority = -5
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    // If IgnoreImmunity is null, create a new dictionary
                    move.IgnoreImmunity ??= new Dictionary<PokemonType, bool>();

                    // If IgnoreImmunity is true (ignore all), don't modify
                    if (move.IgnoreImmunity is BoolMoveDataIgnoreImmunity { Value: true })
                    {
                        return;
                    }

                    // If it's a dictionary, add Fighting and Normal immunities
                    if (move.IgnoreImmunity is TypeMoveDataIgnoreImmunity typeIgnore)
                    {
                        typeIgnore.TypeImmunities[PokemonType.Fighting] = true;
                        typeIgnore.TypeImmunities[PokemonType.Normal] = true;
                    }
                }, -5),
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, _, effect) =>
                {
                    if (effect?.EffectStateId == AbilityId.Intimidate && boost.Atk != null)
                    {
                        boost.Atk = null;
                        battle.Add("-fail", target, "unboost", "Attack", "[from] ability: Scrappy",
                            $"[of] {target}");
                    }
                }),
            },
            [AbilityId.ScreenCleaner] = new()
            {
                Id = AbilityId.ScreenCleaner,
                Name = "Screen Cleaner",
                Num = 251,
                Rating = 2.0,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    var activated = false;
                    ConditionId[] screens =
                        [ConditionId.Reflect, ConditionId.LightScreen, ConditionId.AuroraVeil];
                    foreach (ConditionId sideCondition in screens)
                    {
                        foreach (Side side in new[] { pokemon.Side }.Concat(
                                     pokemon.Side.FoeSidesWithConditions()))
                        {
                            if (side.GetSideCondition(sideCondition) != null)
                            {
                                if (!activated)
                                {
                                    battle.Add("-activate", pokemon, "ability: Screen Cleaner");
                                    activated = true;
                                }

                                side.RemoveSideCondition(sideCondition);
                            }
                        }
                    }
                }),
            },
            [AbilityId.SeedSower] = new()
            {
                Id = AbilityId.SeedSower,
                Name = "Seed Sower",
                Num = 269,
                Rating = 2.5,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, _, _, _) =>
                {
                    battle.Field.SetTerrain(_library.Conditions[ConditionId.GrassyTerrain]);
                }),
            },
            [AbilityId.SereneGrace] = new()
            {
                Id = AbilityId.SereneGrace,
                Name = "Serene Grace",
                Num = 32,
                Rating = 3.5,
                // OnModifyMovePriority = -2
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, _, _) =>
                {
                    if (move.Secondaries != null)
                    {
                        battle.Debug("doubling secondary chance");
                        foreach (SecondaryEffect secondary in move.Secondaries)
                        {
                            if (secondary.Chance != null)
                            {
                                secondary.Chance *= 2;
                            }
                        }
                    }

                    if (move.Self?.Chance != null)
                    {
                        move.Self.Chance *= 2;
                    }
                }, -2),
            },
            [AbilityId.ShadowShield] = new()
            {
                Id = AbilityId.ShadowShield,
                Name = "Shadow Shield",
                Num = 231,
                Rating = 3.5,
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, _, target, _) =>
                    {
                        if (target.Hp >= target.MaxHp)
                        {
                            battle.Debug("Shadow Shield weaken");
                            battle.ChainModify(0.5);
                            return battle.FinalModify(damage);
                        }

                        return damage;
                    }),
            },
            [AbilityId.ShadowTag] = new()
            {
                Id = AbilityId.ShadowTag,
                Name = "Shadow Tag",
                Num = 23,
                Rating = 5.0,
                OnFoeTrapPokemon = new OnFoeTrapPokemonEventInfo((battle, pokemon) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder,
                        })
                        return;
                    if (!pokemon.HasAbility(AbilityId.ShadowTag) &&
                        pokemon.IsAdjacent(abilityHolder))
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
                        if (!pokemon.HasAbility(AbilityId.ShadowTag))
                        {
                            pokemon.MaybeTrapped = true;
                        }
                    }),
            },
            [AbilityId.Sharpness] = new()
            {
                Id = AbilityId.Sharpness,
                Name = "Sharpness",
                Num = 292,
                Rating = 3.5,
                // OnBasePowerPriority = 19
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Flags.Slicing == true)
                    {
                        battle.Debug("Sharpness boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 19),
            },
            [AbilityId.ShedSkin] = new()
            {
                Id = AbilityId.ShedSkin,
                Name = "Shed Skin",
                Num = 61,
                Rating = 3.0,
                // OnResidualOrder = 5, OnResidualSubOrder = 3
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.Hp > 0 && pokemon.Status != ConditionId.None &&
                        battle.RandomChance(33, 100))
                    {
                        battle.Debug("shed skin");
                        battle.Add("-activate", pokemon, "ability: Shed Skin");
                        pokemon.CureStatus();
                    }
                }, order: 5, subOrder: 3),
            },
            [AbilityId.SheerForce] = new()
            {
                Id = AbilityId.SheerForce,
                Name = "Sheer Force",
                Num = 125,
                Rating = 3.5,
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    if (move.Secondaries != null)
                    {
                        move.HasSheerForce = true;
                        // Note: Secondaries removal is handled in the battle engine
                    }
                }),
                // OnBasePowerPriority = 21
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.HasSheerForce == true)
                    {
                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 21),
            },
            [AbilityId.ShellArmor] = new()
            {
                Id = AbilityId.ShellArmor,
                Name = "Shell Armor",
                Num = 75,
                Rating = 1.0,
                Flags = new AbilityFlags { Breakable = true },
                OnCriticalHit = new OnCriticalHitEventInfo(false),
            },
            [AbilityId.ShieldDust] = new()
            {
                Id = AbilityId.ShieldDust,
                Name = "Shield Dust",
                Num = 19,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                OnModifySecondaries = new OnModifySecondariesEventInfo((_, secondaries, _, _, _) =>
                {
                    // Filter out secondaries that don't target self (return only self-targeting effects)
                    return secondaries.Where(effect => effect.Self != null).ToArray();
                }),
            },
            [AbilityId.ShieldsDown] = new()
            {
                Id = AbilityId.ShieldsDown,
                Name = "Shields Down",
                Num = 197,
                Rating = 3.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                },
                // OnSwitchInPriority = -1
                OnStart = new OnStartEventInfo((_, pokemon) =>
                {
                    if (pokemon.BaseSpecies.BaseSpecies != SpecieId.Minior || pokemon.Transformed)
                        return;

                    if (pokemon.Hp > pokemon.MaxHp / 2)
                    {
                        if (pokemon.Species.Forme != FormeId.Meteor)
                        {
                            pokemon.FormeChange(SpecieId.MiniorMeteor);
                        }
                    }
                    else
                    {
                        if (pokemon.Species.Forme == FormeId.Meteor)
                        {
                            pokemon.FormeChange(pokemon.Set.Species);
                        }
                    }
                }, -1),
                // OnResidualOrder = 29
                OnResidual = new OnResidualEventInfo((_, pokemon, _, _) =>
                {
                    if (pokemon.BaseSpecies.BaseSpecies != SpecieId.Minior ||
                        pokemon.Transformed || pokemon.Hp == 0)
                        return;

                    if (pokemon.Hp > pokemon.MaxHp / 2)
                    {
                        if (pokemon.Species.Forme != FormeId.Meteor)
                        {
                            pokemon.FormeChange(SpecieId.MiniorMeteor);
                        }
                    }
                    else
                    {
                        if (pokemon.Species.Forme == FormeId.Meteor)
                        {
                            pokemon.FormeChange(pokemon.Set.Species);
                        }
                    }
                }, order: 29),
                OnSetStatus = new OnSetStatusEventInfo((battle, _, target, _, effect) =>
                {
                    // Only Minior-Meteor is immune to status, and not if transformed
                    if (target.Species.Id != SpecieId.MiniorMeteor || target.Transformed)
                        return new VoidReturn();
                    if (effect is ActiveMove { Status: not ConditionId.None })
                    {
                        battle.Add("-immune", target, "[from] ability: Shields Down");
                    }

                    return false;
                }),
                OnTryAddVolatile =
                    new OnTryAddVolatileEventInfo((battle, status, target, _, _) =>
                    {
                        // Only Minior-Meteor is immune to yawn, and not if transformed
                        if (target.Species.Id != SpecieId.MiniorMeteor || target.Transformed)
                            return new VoidReturn();
                        if (status.Id != ConditionId.Yawn) return new VoidReturn();
                        battle.Add("-immune", target, "[from] ability: Shields Down");
                        return null;
                    }),
            },
            [AbilityId.Simple] = new()
            {
                Id = AbilityId.Simple,
                Name = "Simple",
                Num = 86,
                Rating = 4.0,
                Flags = new AbilityFlags { Breakable = true },
                OnChangeBoost = new OnChangeBoostEventInfo((_, boost, _, _, _) =>
                {
                    // Gen 9 doesn't need the Z-Power check
                    if (boost.Atk != null) boost.Atk *= 2;
                    if (boost.Def != null) boost.Def *= 2;
                    if (boost.SpA != null) boost.SpA *= 2;
                    if (boost.SpD != null) boost.SpD *= 2;
                    if (boost.Spe != null) boost.Spe *= 2;
                    if (boost.Accuracy != null) boost.Accuracy *= 2;
                    if (boost.Evasion != null) boost.Evasion *= 2;
                }),
            },
            [AbilityId.SkillLink] = new()
            {
                Id = AbilityId.SkillLink,
                Name = "Skill Link",
                Num = 92,
                Rating = 3.0,
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    if (move.MultiHit is IntArrayIntIntArrayUnion { Values: [_, var max] })
                    {
                        move.MultiHit = max;
                    }

                    if (move.MultiAccuracy == true)
                    {
                        move.MultiAccuracy = false;
                    }
                }),
            },
            [AbilityId.SlowStart] = new()
            {
                Id = AbilityId.SlowStart,
                Name = "Slow Start",
                Num = 112,
                Rating = -1.0,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    battle.Add("-start", pokemon, "ability: Slow Start");
                    battle.EffectState.Counter = 5;
                }),
                // OnResidualOrder = 28, OnResidualSubOrder = 2
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.ActiveTurns > 0 && battle.EffectState.Counter != null)
                    {
                        battle.EffectState.Counter--;
                        if (battle.EffectState.Counter <= 0)
                        {
                            battle.Add("-end", pokemon, "Slow Start");
                            battle.EffectState.Counter = null;
                        }
                    }
                }, order: 28, subOrder: 2),
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, _, _, _) =>
                {
                    if (battle.EffectState.Counter != null)
                    {
                        battle.ChainModify(0.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, _) =>
                {
                    if (battle.EffectState.Counter != null)
                    {
                        battle.ChainModify(0.5);
                        return battle.FinalModify(spe);
                    }

                    return spe;
                }),
            },
            [AbilityId.SlushRush] = new()
            {
                Id = AbilityId.SlushRush,
                Name = "Slush Rush",
                Num = 202,
                Rating = 3.0,
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, _) =>
                {
                    if (battle.Field.IsWeather(ConditionId.Snowscape))
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spe);
                    }

                    return spe;
                }),
            },
            [AbilityId.Sniper] = new()
            {
                Id = AbilityId.Sniper,
                Name = "Sniper",
                Num = 97,
                Rating = 2.0,
                OnModifyDamage = new OnModifyDamageEventInfo((battle, damage, _, target, move) =>
                {
                    if (target.GetMoveHitData(move).Crit)
                    {
                        battle.Debug("Sniper boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(damage);
                    }

                    return damage;
                }),
            },
            [AbilityId.SnowCloak] = new()
            {
                Id = AbilityId.SnowCloak,
                Name = "Snow Cloak",
                Num = 81,
                Rating = 1.5,
                Flags = new AbilityFlags { Breakable = true },
                // Note: In Gen 9, Snowscape replaced Hail and does not deal damage,
                // so OnImmunity is not needed. The TS version checks for 'hail' immunity
                // which dealt damage in earlier gens. Kept for consistency but has no effect.
                // OnModifyAccuracyPriority = -1
                OnModifyAccuracy = new OnModifyAccuracyEventInfo((battle, accuracy, _, _, _) =>
                {
                    // Only modify numeric accuracy
                    if (accuracy.HasValue && battle.Field.IsWeather(ConditionId.Snowscape))
                    {
                        battle.Debug("Snow Cloak - decreasing accuracy");
                        return battle.ChainModify([3277, 4096]);
                    }

                    return new VoidReturn();
                }, -1),
            },
            [AbilityId.SnowWarning] = new()
            {
                Id = AbilityId.SnowWarning,
                Name = "Snow Warning",
                Num = 117,
                Rating = 4.0,
                OnStart = new OnStartEventInfo((battle, _) =>
                {
                    battle.Field.SetWeather(_library.Conditions[ConditionId.Snowscape]);
                }),
            },
            [AbilityId.SolarPower] = new()
            {
                Id = AbilityId.SolarPower,
                Name = "Solar Power",
                Num = 94,
                Rating = 2.0,
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, attacker, _, _) =>
                {
                    // Use attacker's effective weather which accounts for Utility Umbrella
                    ConditionId effectiveWeather = attacker.EffectiveWeather();
                    if (effectiveWeather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        battle.ChainModify(1.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
                OnWeather = new OnWeatherEventInfo((battle, target, _, effect) =>
                {
                    if (target.HasItem(ItemId.UtilityUmbrella)) return;
                    if (effect.Id is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        battle.Damage(target.BaseMaxHp / 8, target, target);
                    }
                }),
            },
            [AbilityId.SolidRock] = new()
            {
                Id = AbilityId.SolidRock,
                Name = "Solid Rock",
                Num = 116,
                Rating = 3.0,
                Flags = new AbilityFlags { Breakable = true },
                OnSourceModifyDamage =
                    new OnSourceModifyDamageEventInfo((battle, damage, _, target, move) =>
                    {
                        if (target.GetMoveHitData(move).TypeMod > 0)
                        {
                            battle.Debug("Solid Rock neutralize");
                            battle.ChainModify(0.75);
                            return battle.FinalModify(damage);
                        }

                        return damage;
                    }),
            },
            [AbilityId.SoulHeart] = new()
            {
                Id = AbilityId.SoulHeart,
                Name = "Soul-Heart",
                Num = 220,
                Rating = 3.5,
                // OnAnyFaintPriority = 1
                OnAnyFaint = new OnAnyFaintEventInfo((battle, _, _, _) =>
                {
                    if (battle.EffectState.Target is PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder,
                        })
                    {
                        battle.Boost(new SparseBoostsTable { SpA = 1 }, abilityHolder);
                    }
                }, 1),
            },
            [AbilityId.Soundproof] = new()
            {
                Id = AbilityId.Soundproof,
                Name = "Soundproof",
                Num = 43,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (target != source && move.Flags.Sound == true)
                    {
                        battle.Add("-immune", target, "[from] ability: Soundproof");
                        return null;
                    }

                    return new VoidReturn();
                }, 1),
                OnAllyTryHitSide = new OnAllyTryHitSideEventInfo((battle, _, _, move) =>
                {
                    if (move.Flags.Sound == true)
                    {
                        if (battle.EffectState.Target is PokemonEffectStateTarget
                            {
                                Pokemon: var abilityHolder,
                            })
                        {
                            battle.Add("-immune", abilityHolder, "[from] ability: Soundproof");
                        }
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.SpeedBoost] = new()
            {
                Id = AbilityId.SpeedBoost,
                Name = "Speed Boost",
                Num = 3,
                Rating = 4.5,
                // OnResidualOrder = 28, OnResidualSubOrder = 2
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.ActiveTurns > 0)
                    {
                        battle.Boost(new SparseBoostsTable { Spe = 1 });
                    }
                }, order: 28, subOrder: 2),
            },
            [AbilityId.Stakeout] = new()
            {
                Id = AbilityId.Stakeout,
                Name = "Stakeout",
                Num = 198,
                Rating = 4.5,
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, _, defender, _) =>
                {
                    if (defender.ActiveTurns == 0)
                    {
                        battle.Debug("Stakeout boost");
                        battle.ChainModify(2);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, defender, _) =>
                {
                    if (defender.ActiveTurns == 0)
                    {
                        battle.Debug("Stakeout boost");
                        battle.ChainModify(2);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.Stall] = new()
            {
                Id = AbilityId.Stall,
                Name = "Stall",
                Num = 100,
                Rating = -1.0,
                OnFractionalPriority = new OnFractionalPriorityEventInfo(
                    (ModifierSourceMoveHandler)((_, _, _, _, _) => -0.1), -1),
            },
            [AbilityId.Stalwart] = new()
            {
                Id = AbilityId.Stalwart,
                Name = "Stalwart",
                Num = 242,
                Rating = 0.0,
                // OnModifyMovePriority = 1
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    // Most of the implementation is in Battle.GetTarget
                    move.TracksTarget = move.Target != MoveTarget.Scripted;
                }, 1),
            },
            [AbilityId.Stamina] = new()
            {
                Id = AbilityId.Stamina,
                Name = "Stamina",
                Num = 192,
                Rating = 4.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, _, _, _) =>
                {
                    battle.Boost(new SparseBoostsTable { Def = 1 });
                }),
            },
            // Note: In Gen 9, King's Shield is isNonstandard: "Past", so the Shield Forme reversion
            // mechanism doesn't exist in Gen 9 standard play. Aegislash IS available in Gen 9.
            // This implementation only handles Blade Forme transformation which is sufficient.
            // TODO: For National Dex support, add King's Shield handling to revert to Shield Forme.
            [AbilityId.StanceChange] = new()
            {
                Id = AbilityId.StanceChange,
                Name = "Stance Change",
                Num = 176,
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
                // OnModifyMovePriority = 1
                OnModifyMove = new OnModifyMoveEventInfo((_, move, attacker, _) =>
                {
                    if (attacker.BaseSpecies.BaseSpecies != SpecieId.Aegislash ||
                        attacker.Transformed)
                        return;
                    // In Gen 9, King's Shield doesn't exist, so only handle attacking moves -> Blade Forme
                    // TS logic: if (move.category === 'Status' && move.id !== 'kingsshield') return;
                    if (move.Category == MoveCategory.Status)
                        return;

                    const SpecieId targetForme = SpecieId.AegislashBlade;

                    if (attacker.Species.Id != targetForme)
                    {
                        attacker.FormeChange(targetForme);
                    }
                }, 1),
            },
            [AbilityId.Static] = new()
            {
                Id = AbilityId.Static,
                Name = "Static",
                Num = 9,
                Rating = 2.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        if (battle.RandomChance(3, 10))
                        {
                            source.TrySetStatus(ConditionId.Paralysis, target);
                        }
                    }
                }),
            },
            [AbilityId.Steadfast] = new()
            {
                Id = AbilityId.Steadfast,
                Name = "Steadfast",
                Num = 80,
                Rating = 1.0,
                OnFlinch = new OnFlinchEventInfo(
                    (Func<Battle, Pokemon, object?, Move, BoolVoidUnion>)((battle, _, _, _) =>
                    {
                        battle.Boost(new SparseBoostsTable { Spe = 1 });
                        return new VoidReturn();
                    })),
            },
            [AbilityId.SteamEngine] = new()
            {
                Id = AbilityId.SteamEngine,
                Name = "Steam Engine",
                Num = 243,
                Rating = 2.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, _, _, move) =>
                {
                    if (move.Type is MoveType.Water or MoveType.Fire)
                    {
                        battle.Boost(new SparseBoostsTable { Spe = 6 });
                    }
                }),
            },
            [AbilityId.Steelworker] = new()
            {
                Id = AbilityId.Steelworker,
                Name = "Steelworker",
                Num = 200,
                Rating = 3.5,
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, _, _, move) =>
                {
                    if (move.Type == MoveType.Steel)
                    {
                        battle.Debug("Steelworker boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Steel)
                    {
                        battle.Debug("Steelworker boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.SteelySpirit] = new()
            {
                Id = AbilityId.SteelySpirit,
                Name = "Steely Spirit",
                Num = 252,
                Rating = 3.5,
                // OnAllyBasePowerPriority = 22
                OnAllyBasePower = new OnAllyBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Type == MoveType.Steel)
                    {
                        battle.Debug("Steely Spirit boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 22),
            },
            [AbilityId.Stench] = new()
            {
                Id = AbilityId.Stench,
                Name = "Stench",
                Num = 1,
                Rating = 0.5,
                // OnModifyMovePriority = -1
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    if (move.Category != MoveCategory.Status)
                    {
                        // Check if flinch secondary already exists
                        if (move.Secondaries == null ||
                            move.Secondaries.All(s => s.VolatileStatus != ConditionId.Flinch))
                        {
                            SecondaryEffect flinchEffect = new()
                            {
                                Chance = 10,
                                VolatileStatus = ConditionId.Flinch,
                            };
                            move.Secondaries = move.Secondaries == null
                                ? [flinchEffect]
                                : [..move.Secondaries, flinchEffect];
                        }
                    }
                }, -1),
            },
            [AbilityId.StickyHold] = new()
            {
                Id = AbilityId.StickyHold,
                Name = "Sticky Hold",
                Num = 60,
                Rating = 1.5,
                Flags = new AbilityFlags { Breakable = true },
                OnTakeItem = new OnTakeItemEventInfo(
                    (Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)((battle, _,
                        pokemon, source, _) =>
                    {
                        if (pokemon.Hp == 0 || pokemon.Item == ItemId.StickyBarb)
                            return BoolVoidUnion.FromVoid();
                        // Prevent item theft if source is another Pokemon OR if the move is Knock Off
                        if ((source != null && source != pokemon) ||
                            battle.ActiveMove?.Id == MoveId.KnockOff)
                        {
                            battle.Add("-activate", pokemon, "ability: Sticky Hold");
                            return
                                BoolVoidUnion
                                    .FromBool(false); // Return false to prevent item removal
                        }

                        return BoolVoidUnion.FromVoid();
                    })),
            },
            [AbilityId.StormDrain] = new()
            {
                Id = AbilityId.StormDrain,
                Name = "Storm Drain",
                Num = 114,
                Rating = 3.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Water)
                    {
                        if (!(battle.Boost(new SparseBoostsTable { SpA = 1 })?.IsTruthy() ?? false))
                        {
                            battle.Add("-immune", target, "[from] ability: Storm Drain");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }, 1),

                OnAnyRedirectTarget =
                    new OnAnyRedirectTargetEventInfo((battle, target, source, _, move) =>
                    {
                        if (move.Type != MoveType.Water || move.Flags.PledgeCombo == true ||
                            battle.EffectState.Target is not PokemonEffectStateTarget
                            {
                                Pokemon: var abilityHolder,
                            })
                            return new VoidReturn();
                        MoveTarget redirectTarget =
                            move.Target is MoveTarget.RandomNormal or MoveTarget.AdjacentFoe
                                ? MoveTarget.Normal
                                : move.Target;
                        if (battle.ValidTarget(abilityHolder, source, redirectTarget))
                        {
                            if (move.SmartTarget == true) move.SmartTarget = false;
                            if (abilityHolder != target)
                            {
                                battle.Add("-activate", abilityHolder, "ability: Storm Drain");
                            }

                            return abilityHolder;
                        }

                        return new VoidReturn();
                    }),
            },
            [AbilityId.StrongJaw] = new()
            {
                Id = AbilityId.StrongJaw,
                Name = "Strong Jaw",
                Num = 173,
                Rating = 3.5,
                // OnBasePowerPriority = 19
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Flags.Bite == true)
                    {
                        battle.ChainModify(1.5);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 19),
            },
            [AbilityId.Sturdy] = new()
            {
                Id = AbilityId.Sturdy,
                Name = "Sturdy",
                Num = 5,
                Rating = 3.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = new OnTryHitEventInfo((battle, pokemon, _, move) =>
                {
                    if (move.Ohko != null)
                    {
                        battle.Add("-immune", pokemon, "[from] ability: Sturdy");
                        return null;
                    }

                    return new VoidReturn();
                }, 1),
                // OnDamagePriority = -30
                OnDamage = new OnDamageEventInfo((battle, damage, target, _, effect) =>
                {
                    if (target.Hp == target.MaxHp && damage >= target.Hp && effect is ActiveMove)
                    {
                        battle.Add("-ability", target, "Sturdy");
                        return target.Hp - 1;
                    }

                    return new VoidReturn();
                }, -30),
            },
            [AbilityId.SuctionCups] = new()
            {
                Id = AbilityId.SuctionCups,
                Name = "Suction Cups",
                Num = 21,
                Rating = 1.0,
                Flags = new AbilityFlags { Breakable = true },
                // OnDragOutPriority = 1
                OnDragOut = new OnDragOutEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Add("-activate", pokemon, "ability: Suction Cups");
                    return null; // Prevent drag-out silently
                }, 1),
            },
            [AbilityId.SuperLuck] = new()
            {
                Id = AbilityId.SuperLuck,
                Name = "Super Luck",
                Num = 105,
                Rating = 1.5,
                OnModifyCritRatio =
                    new OnModifyCritRatioEventInfo((_, critRatio, _, _, _) => critRatio + 1),
            },
            [AbilityId.SupersweetSyrup] = new()
            {
                Id = AbilityId.SupersweetSyrup,
                Name = "Supersweet Syrup",
                Num = 306,
                Rating = 1.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (pokemon.SyrupTriggered) return;
                    pokemon.SyrupTriggered = true;
                    battle.Add("-ability", pokemon, "Supersweet Syrup");
                    foreach (Pokemon target in pokemon.AdjacentFoes())
                    {
                        if (target.Volatiles.ContainsKey(ConditionId.Substitute))
                        {
                            battle.Add("-immune", target);
                        }
                        else
                        {
                            battle.Boost(new SparseBoostsTable { Evasion = -1 }, target, pokemon,
                                null, true);
                        }
                    }
                }),
            },
            [AbilityId.SupremeOverlord] = new()
            {
                Id = AbilityId.SupremeOverlord,
                Name = "Supreme Overlord",
                Num = 293,
                Rating = 4.0,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Side.TotalFainted > 0)
                    {
                        battle.Add("-activate", pokemon, "ability: Supreme Overlord");
                        int fallen = Math.Min(pokemon.Side.TotalFainted, 5);
                        battle.Add("-start", pokemon, $"fallen{fallen}", "[silent]");
                        battle.EffectState.Counter = fallen;
                    }
                }),
                OnEnd = new OnEndEventInfo((battle, pokemonUnion) =>
                {
                    if (battle.EffectState.Counter != null &&
                        pokemonUnion is PokemonSideFieldPokemon { Pokemon: var pokemon })
                    {
                        battle.Add("-end", pokemon, $"fallen{battle.EffectState.Counter}", "[silent]");
                    }
                }),
                // OnBasePowerPriority = 21
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, _) =>
                {
                    if (battle.EffectState.Counter is { } fallen and > 0)
                    {
                        int[] powMod = [4096, 4506, 4915, 5325, 5734, 6144];
                        battle.Debug($"Supreme Overlord boost: {powMod[fallen]}/4096");
                        battle.ChainModify([powMod[fallen], 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 21),
            },
            [AbilityId.SurgeSurfer] = new()
            {
                Id = AbilityId.SurgeSurfer,
                Name = "Surge Surfer",
                Num = 207,
                Rating = 3.0,
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, _) =>
                {
                    if (battle.Field.IsTerrain(ConditionId.ElectricTerrain, null))
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spe);
                    }

                    return spe;
                }),
            },
            [AbilityId.Swarm] = new()
            {
                Id = AbilityId.Swarm,
                Name = "Swarm",
                Num = 68,
                Rating = 2.0,
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Bug && attacker.Hp <= attacker.MaxHp / 3)
                    {
                        battle.Debug("Swarm boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Bug && attacker.Hp <= attacker.MaxHp / 3)
                    {
                        battle.Debug("Swarm boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.SweetVeil] = new()
            {
                Id = AbilityId.SweetVeil,
                Name = "Sweet Veil",
                Num = 175,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                OnAllySetStatus = new OnAllySetStatusEventInfo((battle, status, target, _, _) =>
                {
                    if (status.Id == ConditionId.Sleep)
                    {
                        battle.Debug("Sweet Veil interrupts sleep");
                        if (battle.EffectState.Target is PokemonEffectStateTarget
                            {
                                Pokemon: var effectHolder,
                            })
                        {
                            battle.Add("-block", target, "ability: Sweet Veil",
                                $"[of] {effectHolder}");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }),
                OnAllyTryAddVolatile =
                    new OnAllyTryAddVolatileEventInfo((battle, status, target, _, _) =>
                    {
                        if (status.Id == ConditionId.Yawn)
                        {
                            battle.Debug("Sweet Veil blocking yawn");
                            if (battle.EffectState.Target is PokemonEffectStateTarget
                                {
                                    Pokemon: var effectHolder,
                                })
                            {
                                battle.Add("-block", target, "ability: Sweet Veil",
                                    $"[of] {effectHolder}");
                            }

                            return null;
                        }

                        return new VoidReturn();
                    }),
            },
            [AbilityId.SwiftSwim] = new()
            {
                Id = AbilityId.SwiftSwim,
                Name = "Swift Swim",
                Num = 33,
                Rating = 3.0,
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
                {
                    // Use pokemon's effective weather which accounts for Utility Umbrella
                    ConditionId effectiveWeather = pokemon.EffectiveWeather();
                    if (effectiveWeather is ConditionId.RainDance or ConditionId.PrimordialSea)
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spe);
                    }

                    return spe;
                }),
            },
            [AbilityId.SwordOfRuin] = new()
            {
                Id = AbilityId.SwordOfRuin,
                Name = "Sword of Ruin",
                Num = 285,
                Rating = 4.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (battle.SuppressingAbility(pokemon)) return;
                    battle.Add("-ability", pokemon, "Sword of Ruin");
                }),
                OnAnyModifyDef = new OnAnyModifyDefEventInfo((battle, def, target, _, move) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder,
                        })
                        return def;
                    if (target.HasAbility(AbilityId.SwordOfRuin)) return def;
                    if (move.RuinedDef?.HasAbility(AbilityId.SwordOfRuin) != true)
                        move.RuinedDef = abilityHolder;
                    if (move.RuinedDef != abilityHolder) return def;
                    battle.Debug("Sword of Ruin Def drop");
                    battle.ChainModify(0.75);
                    return battle.FinalModify(def);
                }),
            },
            [AbilityId.Symbiosis] = new()
            {
                Id = AbilityId.Symbiosis,
                Name = "Symbiosis",
                Num = 180,
                Rating = 0.0,
                OnAllyAfterUseItem = new OnAllyAfterUseItemEventInfo((battle, _, pokemon) =>
                {
                    if (pokemon.SwitchFlag == true) return;
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var source,
                        })
                        return;
                    ItemFalseUnion myItemResult = source.TakeItem();
                    if (myItemResult is not ItemItemFalseUnion { Item: var myItem }) return;

                    // Check if the item can be received by the ally (e.g., Memory items only for Silvally)
                    // TODO: TS passes source.itemState as effectState parameter instead of null.
                    // If item state is needed for certain item TakeItem checks, this might cause issues.
                    Ability symbiosis = battle.Library.Abilities[AbilityId.Symbiosis];
                    if (battle.SingleEvent(EventId.TakeItem, myItem, null, pokemon, source,
                            symbiosis, myItem) is BoolRelayVar { Value: false } ||
                        !pokemon.SetItem(myItem.Id))
                    {
                        source.Item = myItem.Id;
                        return;
                    }

                    battle.Add("-activate", source, "ability: Symbiosis", myItem.Name,
                        $"[of] {pokemon}");
                }),
            },
            [AbilityId.Synchronize] = new()
            {
                Id = AbilityId.Synchronize,
                Name = "Synchronize",
                Num = 28,
                Rating = 2.0,
                OnAfterSetStatus =
                    new OnAfterSetStatusEventInfo((battle, status, target, source, effect) =>
                    {
                        if (source == null || source == target) return;
                        if (effect is Condition { Id: ConditionId.ToxicSpikes }) return;
                        if (status.Id is ConditionId.Sleep or ConditionId.Freeze) return;
                        battle.Add("-activate", target, "ability: Synchronize");
                        // TS uses a hack { status: status.id, id: 'synchronize' } to make status-prevention
                        // abilities think Synchronize is a status move. We pass the ability as the effect.
                        source.TrySetStatus(status.Id, target, sourceEffect: _library.Abilities[target.Ability]);
                    }),
            },

            // ==================== 'T' Abilities ====================
            [AbilityId.TabletsOfRuin] = new()
            {
                Id = AbilityId.TabletsOfRuin,
                Name = "Tablets of Ruin",
                Num = 284,
                Rating = 4.5,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (battle.SuppressingAbility(pokemon)) return;
                    battle.Add("-ability", pokemon, "Tablets of Ruin");
                }),
                OnAnyModifyAtk = new OnAnyModifyAtkEventInfo((battle, atk, source, _, move) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var abilityHolder,
                        })
                        return atk;
                    if (source.HasAbility(AbilityId.TabletsOfRuin)) return atk;
                    if (move.RuinedAtk?.HasAbility(AbilityId.TabletsOfRuin) != true)
                        move.RuinedAtk = abilityHolder;
                    if (move.RuinedAtk != abilityHolder) return atk;
                    battle.Debug("Tablets of Ruin Atk drop");
                    battle.ChainModify(0.75);
                    return battle.FinalModify(atk);
                }),
            },
            [AbilityId.TangledFeet] = new()
            {
                Id = AbilityId.TangledFeet,
                Name = "Tangled Feet",
                Num = 77,
                Rating = 1.0,
                Flags = new AbilityFlags { Breakable = true },
                // OnModifyAccuracyPriority = -1
                OnModifyAccuracy = new OnModifyAccuracyEventInfo((battle, accuracy, target, _, _) =>
                {
                    // Only modify numeric accuracy
                    if (accuracy.HasValue && target.Volatiles.ContainsKey(ConditionId.Confusion))
                    {
                        battle.Debug("Tangled Feet - decreasing accuracy");
                        return battle.ChainModify(0.5);
                    }

                    return new VoidReturn();
                }, -1),
            },
            [AbilityId.TanglingHair] = new()
            {
                Id = AbilityId.TanglingHair,
                Name = "Tangling Hair",
                Num = 221,
                Rating = 2.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    if (battle.CheckMoveMakesContact(move, source, target, true))
                    {
                        battle.Add("-ability", target, "Tangling Hair");
                        battle.Boost(new SparseBoostsTable { Spe = -1 }, source, target, null,
                            true);
                    }
                }),
            },
            [AbilityId.Technician] = new()
            {
                Id = AbilityId.Technician,
                Name = "Technician",
                Num = 101,
                Rating = 3.5,
                // OnBasePowerPriority = 30
                // Technician boosts moves with base power 60 or less (after previous modifiers)
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, _) =>
                {
                    // Calculate effective base power after all previous modifiers in the chain
                    int basePowerAfterMultiplier =
                        battle.Modify(basePower, battle.Event.Modifier ?? 1.0);
                    battle.Debug($"Base Power: {basePowerAfterMultiplier}");
                    if (basePowerAfterMultiplier <= 60)
                    {
                        battle.Debug("Technician boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 30),
            },
            [AbilityId.Telepathy] = new()
            {
                Id = AbilityId.Telepathy,
                Name = "Telepathy",
                Num = 140,
                Rating = 0.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (target != source && target.IsAlly(source) &&
                        move.Category != MoveCategory.Status)
                    {
                        battle.Add("-activate", target, "ability: Telepathy");
                        return null;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.TeraformZero] = new()
            {
                Id = AbilityId.TeraformZero,
                Name = "Teraform Zero",
                Num = 309,
                Rating = 3.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                },
                OnAfterTerastallization = new OnAfterTerastallizationEventInfo((battle, pokemon) =>
                {
                    if (pokemon.BaseSpecies.Id != SpecieId.TerapagosStellar) return;
                    if (battle.Field.Weather != ConditionId.None ||
                        battle.Field.Terrain != ConditionId.None)
                    {
                        battle.Add("-ability", pokemon, "Teraform Zero");
                        battle.Field.ClearWeather();
                        battle.Field.ClearTerrain();
                    }
                }),
            },
            [AbilityId.TeraShell] = new()
            {
                Id = AbilityId.TeraShell,
                Name = "Tera Shell",
                Num = 308,
                Rating = 3.5,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    Breakable = true,
                },
                // Effectiveness implemented in Pokemon.RunEffectiveness
                // Needs two checks to reset between regular moves and future attacks
                OnAnyBeforeMove = new OnAnyBeforeMoveEventInfo((battle, _, _, _) =>
                {
                    battle.EffectState.Resisted = null;
                    return new VoidReturn();
                }),
                OnAnyAfterMove = new OnAnyAfterMoveEventInfo((battle, _, _, _) =>
                {
                    battle.EffectState.Resisted = null;
                    return new VoidReturn();
                }),
            },
            [AbilityId.TeraShift] = new()
            {
                Id = AbilityId.TeraShift,
                Name = "Tera Shift",
                Num = 307,
                Rating = 3.0,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                    FailSkillSwap = true,
                    CantSuppress = true,
                    NoTransform = true,
                },
                // OnSwitchInPriority = 2
                OnSwitchIn = new OnSwitchInEventInfo((battle, pokemon) =>
                {
                    if (pokemon.BaseSpecies.BaseSpecies != SpecieId.Terapagos) return;
                    if (pokemon.Species.Id != SpecieId.TerapagosTerastal)
                    {
                        battle.Add("-activate", pokemon, "ability: Tera Shift");
                        pokemon.FormeChange(SpecieId.TerapagosTerastal, battle.Effect, true);
                    }
                }, 2),
            },
            [AbilityId.Teravolt] = new()
            {
                Id = AbilityId.Teravolt,
                Name = "Teravolt",
                Num = 164,
                Rating = 3.0,
                OnStart = new OnStartEventInfo((battle, pokemon) => { battle.Add("-ability", pokemon, "Teravolt"); }),
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) => { move.IgnoreAbility = true; }),
            },
            [AbilityId.ThermalExchange] = new()
            {
                Id = AbilityId.ThermalExchange,
                Name = "Thermal Exchange",
                Num = 270,
                Rating = 2.5,
                Flags = new AbilityFlags { Breakable = true },
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, _, _, move) =>
                {
                    if (move.Type == MoveType.Fire)
                    {
                        battle.Boost(new SparseBoostsTable { Atk = 1 });
                    }
                }),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Burn)
                    {
                        battle.Add("-activate", pokemon, "ability: Thermal Exchange");
                        pokemon.CureStatus();
                    }
                }),
                OnSetStatus = new OnSetStatusEventInfo((battle, status, target, _, effect) =>
                {
                    if (status.Id != ConditionId.Burn) return new VoidReturn();
                    if (effect is ActiveMove { Status: not ConditionId.None })
                    {
                        battle.Add("-immune", target, "[from] ability: Thermal Exchange");
                    }

                    return false;
                }),
            },
            [AbilityId.ThickFat] = new()
            {
                Id = AbilityId.ThickFat,
                Name = "Thick Fat",
                Num = 47,
                Rating = 3.5,
                Flags = new AbilityFlags { Breakable = true },
                // OnSourceModifyAtkPriority = 6
                OnSourceModifyAtk = new OnSourceModifyAtkEventInfo((battle, atk, _, _, move) =>
                {
                    if (move.Type is MoveType.Ice or MoveType.Fire)
                    {
                        battle.Debug("Thick Fat weaken");
                        battle.ChainModify(0.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 6),
                // OnSourceModifySpAPriority = 5
                OnSourceModifySpA = new OnSourceModifySpAEventInfo((battle, spa, _, _, move) =>
                {
                    if (move.Type is MoveType.Ice or MoveType.Fire)
                    {
                        battle.Debug("Thick Fat weaken");
                        battle.ChainModify(0.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.TintedLens] = new()
            {
                Id = AbilityId.TintedLens,
                Name = "Tinted Lens",
                Num = 110,
                Rating = 4.0,
                OnModifyDamage = new OnModifyDamageEventInfo((battle, damage, _, target, move) =>
                {
                    if (target.GetMoveHitData(move).TypeMod < 0)
                    {
                        battle.Debug("Tinted Lens boost");
                        battle.ChainModify(2);
                        return battle.FinalModify(damage);
                    }

                    return damage;
                }),
            },
            [AbilityId.Torrent] = new()
            {
                Id = AbilityId.Torrent,
                Name = "Torrent",
                Num = 67,
                Rating = 2.0,
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Water && attacker.Hp <= attacker.MaxHp / 3)
                    {
                        battle.Debug("Torrent boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, attacker, _, move) =>
                {
                    if (move.Type == MoveType.Water && attacker.Hp <= attacker.MaxHp / 3)
                    {
                        battle.Debug("Torrent boost");
                        battle.ChainModify(1.5);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.ToughClaws] = new()
            {
                Id = AbilityId.ToughClaws,
                Name = "Tough Claws",
                Num = 181,
                Rating = 3.5,
                // OnBasePowerPriority = 21
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, move) =>
                {
                    if (move.Flags.Contact == true)
                    {
                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 21),
            },
            [AbilityId.ToxicBoost] = new()
            {
                Id = AbilityId.ToxicBoost,
                Name = "Toxic Boost",
                Num = 137,
                Rating = 3.0,
                // OnBasePowerPriority = 19
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, attacker, _, move) =>
                {
                    if ((attacker.Status is ConditionId.Poison or ConditionId.Toxic) &&
                        move.Category == MoveCategory.Physical)
                    {
                        battle.ChainModify(1.5);
                        return battle.FinalModify(basePower);
                    }

                    return basePower;
                }, 19),
            },
            [AbilityId.ToxicChain] = new()
            {
                Id = AbilityId.ToxicChain,
                Name = "Toxic Chain",
                Num = 305,
                Rating = 4.5,
                OnSourceDamagingHit =
                    new OnSourceDamagingHitEventInfo((battle, _, target, source, _) =>
                    {
                        // Despite not being a secondary, Shield Dust / Covert Cloak block Toxic Chain's effect
                        if (target.HasAbility(AbilityId.ShieldDust)) return;
                        if (target.HasItem(ItemId.CovertCloak)) return;

                        if (battle.RandomChance(3, 10))
                        {
                            target.TrySetStatus(ConditionId.Toxic, source);
                        }
                    }),
            },
            [AbilityId.ToxicDebris] = new()
            {
                Id = AbilityId.ToxicDebris,
                Name = "Toxic Debris",
                Num = 295,
                Rating = 3.5,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    Side side = source.IsAlly(target) ? source.Side.Foe : source.Side;
                    EffectState? toxicSpikesData =
                        side.GetSideConditionData(ConditionId.ToxicSpikes);
                    if (move.Category == MoveCategory.Physical && (toxicSpikesData == null ||
                                                                   (toxicSpikesData.Layers ?? 0) < 2))
                    {
                        battle.Add("-activate", target, "ability: Toxic Debris");
                        side.AddSideCondition(_library.Conditions[ConditionId.ToxicSpikes], target);
                    }
                }),
            },
            [AbilityId.Trace] = new()
            {
                Id = AbilityId.Trace,
                Name = "Trace",
                Num = 36,
                Rating = 2.5,
                Flags = new AbilityFlags
                {
                    FailRolePlay = true,
                    NoReceiver = true,
                    NoEntrain = true,
                    NoTrace = true,
                },
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    battle.EffectState.Seek = true;
                    // Interaction with No Ability and Ability Shield
                    if (pokemon.AdjacentFoes().Any(foe => foe.Ability == AbilityId.None))
                    {
                        battle.EffectState.Seek = false;
                    }

                    if (pokemon.HasItem(ItemId.AbilityShield))
                    {
                        battle.Add("-block", pokemon, "item: Ability Shield");
                        battle.EffectState.Seek = false;
                    }

                    if (battle.EffectState.Seek == true)
                    {
                        battle.SingleEvent(EventId.Update, battle.Effect, battle.EffectState,
                            pokemon);
                    }
                }),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (battle.EffectState.Seek != true) return;

                    var possibleTargets = pokemon.AdjacentFoes()
                        .Where(target =>
                            target.GetAbility().Flags.NoTrace != true &&
                            target.Ability != AbilityId.None)
                        .ToList();

                    if (possibleTargets.Count == 0) return;

                    Pokemon traceTarget = battle.Sample(possibleTargets);
                    Ability ability = traceTarget.GetAbility();
                    pokemon.SetAbility(ability.Id, traceTarget);
                }),
            },
            [AbilityId.Transistor] = new()
            {
                Id = AbilityId.Transistor,
                Name = "Transistor",
                Num = 262,
                Rating = 3.5,
                // OnModifyAtkPriority = 5
                OnModifyAtk = new OnModifyAtkEventInfo((battle, atk, _, _, move) =>
                {
                    if (move.Type == MoveType.Electric)
                    {
                        battle.Debug("Transistor boost");
                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(atk);
                    }

                    return atk;
                }, 5),
                // OnModifySpAPriority = 5
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, _, _, move) =>
                {
                    if (move.Type == MoveType.Electric)
                    {
                        battle.Debug("Transistor boost");
                        battle.ChainModify([5325, 4096]);
                        return battle.FinalModify(spa);
                    }

                    return spa;
                }, 5),
            },
            [AbilityId.Triage] = new()
            {
                Id = AbilityId.Triage,
                Name = "Triage",
                Num = 205,
                Rating = 3.5,
                OnModifyPriority = new OnModifyPriorityEventInfo((_, priority, _, _, move) =>
                {
                    if (move.Flags.Heal == true)
                    {
                        return priority + 3;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Truant] = new()
            {
                Id = AbilityId.Truant,
                Name = "Truant",
                Num = 54,
                Rating = -1.0,
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    pokemon.RemoveVolatile(battle.Library.Conditions[ConditionId.Truant]);
                    // Handle mid-battle ability gain (e.g., via Skill Swap, Entrainment)
                    // If already active and either moved this turn or won't move, set up Truant
                    if (pokemon.ActiveTurns > 0 &&
                        (pokemon.MoveThisTurnResult != null ||
                         battle.Queue.WillMove(pokemon) == null))
                    {
                        pokemon.AddVolatile(ConditionId.Truant);
                    }
                }),
                // OnBeforeMovePriority = 9
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                {
                    if (pokemon.RemoveVolatile(battle.Library.Conditions[ConditionId.Truant]))
                    {
                        battle.Add("cant", pokemon, "ability: Truant");
                        return false;
                    }

                    pokemon.AddVolatile(ConditionId.Truant);
                    return new VoidReturn();
                }, 9),
            },
            [AbilityId.Turboblaze] = new()
            {
                Id = AbilityId.Turboblaze,
                Name = "Turboblaze",
                Num = 163,
                Rating = 3.0,
                OnStart = new OnStartEventInfo((battle, pokemon) => { battle.Add("-ability", pokemon, "Turboblaze"); }),
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) => { move.IgnoreAbility = true; }),
            },

            // ==================== 'U' Abilities ====================
            [AbilityId.Unaware] = new()
            {
                Id = AbilityId.Unaware,
                Name = "Unaware",
                Num = 109,
                Rating = 4.0,
                Flags = new AbilityFlags { Breakable = true },
                OnAnyModifyBoost = new OnAnyModifyBoostEventInfo((battle, boosts, pokemon) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var unawareUser,
                        })
                        return new VoidReturn();
                    if (unawareUser == pokemon) return new VoidReturn();

                    // When Unaware user is attacking: ignore target's Def, SpD, and Evasion boosts
                    if (unawareUser == battle.ActivePokemon && pokemon == battle.ActiveTarget)
                    {
                        boosts.Def = 0;
                        boosts.SpD = 0;
                        boosts.Evasion = 0;
                    }

                    // When Unaware user is being attacked: ignore attacker's Atk, Def (for Body Press), SpA, and Accuracy boosts
                    if (pokemon == battle.ActivePokemon && unawareUser == battle.ActiveTarget)
                    {
                        boosts.Atk = 0;
                        boosts.Def = 0;
                        boosts.SpA = 0;
                        boosts.Accuracy = 0;
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Unburden] = new()
            {
                Id = AbilityId.Unburden,
                Name = "Unburden",
                Num = 84,
                Rating = 3.5,
                OnAfterUseItem = new OnAfterUseItemEventInfo((battle, _, pokemon) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget
                        {
                            Pokemon: var target,
                        })
                        return;
                    if (pokemon != target) return;
                    pokemon.AddVolatile(ConditionId.Unburden);
                }),
                OnTakeItem = new OnTakeItemEventInfo(
                    (Func<Battle, Item, Pokemon, Pokemon, Move?, BoolVoidUnion>)((_, _, pokemon,
                        _, _) =>
                    {
                        pokemon.AddVolatile(ConditionId.Unburden);
                        return BoolVoidUnion.FromVoid();
                    })),
                OnEnd = new OnEndEventInfo((battle, pokemonUnion) =>
                {
                    if (pokemonUnion is PokemonSideFieldPokemon { Pokemon: var pokemon })
                    {
                        pokemon.RemoveVolatile(battle.Library.Conditions[ConditionId.Unburden]);
                    }
                }),
                // Condition implemented in Conditions file - doubles speed when item is lost
            },
            [AbilityId.Unnerve] = new()
            {
                Id = AbilityId.Unnerve,
                Name = "Unnerve",
                Rating = 1.0,
                Num = 127,
                OnSwitchIn = new OnSwitchInEventInfo((_, _) => { }, 1),
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    if (battle.EffectState.Unnerved ?? false) return;
                    battle.Add("-ability", pokemon, "Unnerve");
                    battle.EffectState.Unnerved = true;
                }),
                OnEnd = new OnEndEventInfo((battle, _) => { battle.EffectState.Unnerved = false; }),
                OnFoeTryEatItem = new OnFoeTryEatItemEventInfo(
                    OnTryEatItem.FromFunc((battle, _, _) =>
                        !(battle.EffectState.Unnerved ?? false))),
            },
            [AbilityId.UnseenFist] = new()
            {
                Id = AbilityId.UnseenFist,
                Name = "Unseen Fist",
                Num = 260,
                Rating = 2.0,
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    if (move.Flags.Contact == true)
                    {
                        move.Flags = move.Flags with { Protect = false };
                    }
                }),
            },
        };
    }
}