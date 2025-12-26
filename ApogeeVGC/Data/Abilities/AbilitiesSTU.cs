using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.PokemonEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

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
                    if (type.IsConditionId && type.AsConditionId == ConditionId.Sandstorm)
                    {
                        // Immune to sandstorm damage
                    }
                }),
            },
            [AbilityId.SandRush] = new()
            {
                Id = AbilityId.SandRush,
                Name = "Sand Rush",
                Num = 146,
                Rating = 3.0,
                OnModifySpe = new OnModifySpeEventInfo((battle, spe, pokemon) =>
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
                    if (type.IsConditionId && type.AsConditionId == ConditionId.Sandstorm)
                    {
                        // Immune to sandstorm damage
                    }
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
                    if (type.IsConditionId && type.AsConditionId == ConditionId.Sandstorm)
                    {
                        // Immune to sandstorm damage
                    }
                }),
                // OnModifyAccuracyPriority = -1
                OnModifyAccuracy = new OnModifyAccuracyEventInfo((battle, accuracy, _, _, _) =>
                {
                    if (accuracy is int acc)
                    {
                        if (battle.Field.IsWeather(ConditionId.Sandstorm))
                        {
                            battle.Debug("Sand Veil - decreasing accuracy");
                            battle.ChainModify([3277, 4096]);
                            return battle.FinalModify(acc);
                        }
                    }
                    return accuracy;
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
                        if (!battle.Boost(new SparseBoostsTable { Atk = 1 }))
                        {
                            battle.Add("-immune", target, "[from] ability: Sap Sipper");
                        }
                        return null;
                    }
                    return new VoidReturn();
                }, 1),
                OnAllyTryHitSide = new OnAllyTryHitSideEventInfo((battle, target, source, move) =>
                {
                    if (battle.EffectState.Target is not PokemonEffectStateTarget { Pokemon: var abilityHolder })
                        return new VoidReturn();
                    if (source == abilityHolder || !target.IsAlly(source)) return new VoidReturn();
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
                // TODO: Schooling requires forme change logic for Wishiwashi
                // OnStart and OnResidual check HP threshold and level to switch formes
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
                    move.IgnoreImmunity ??= new MoveIgnoreImmunity();
                    move.IgnoreImmunity.Fighting = true;
                    move.IgnoreImmunity.Normal = true;
                }, -5),
                OnTryBoost = new OnTryBoostEventInfo((battle, boost, target, source, effect) =>
                {
                    if (effect?.Name == "Intimidate" && boost.Atk != null)
                    {
                        boost.Atk = null;
                        battle.Add("-fail", target, "unboost", "Attack", "[from] ability: Scrappy", $"[of] {target}");
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
                    bool activated = false;
                    ConditionId[] screens = [ConditionId.Reflect, ConditionId.LightScreen];
                    // TODO: Add AuroraVeil when available
                    foreach (var sideCondition in screens)
                    {
                        foreach (var side in new[] { pokemon.Side }.Concat(pokemon.Side.FoeSidesWithConditions()))
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
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) =>
                {
                    if (move.Secondaries != null)
                    {
                        foreach (var secondary in move.Secondaries)
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
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, damage, _, target, _) =>
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
                    if (battle.EffectState.Target is not PokemonEffectStateTarget { Pokemon: var abilityHolder })
                        return;
                    if (!pokemon.HasAbility(AbilityId.ShadowTag) && pokemon.IsAdjacent(abilityHolder))
                    {
                        pokemon.TryTrap(true);
                    }
                }),
                OnFoeMaybeTrapPokemon = new OnFoeMaybeTrapPokemonEventInfo((battle, pokemon, source) =>
                {
                    if (source == null && battle.EffectState.Target is PokemonEffectStateTarget { Pokemon: var holder })
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
                    if (pokemon.Hp > 0 && pokemon.Status != ConditionId.None && battle.RandomChance(33, 100))
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
                        move.Secondaries = null;
                        move.Self = null;
                        move.HasSheerForce = true;
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
                OnCriticalHit = new OnCriticalHitEventInfo((_, _, _, _) => false),
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
                    // Filter out secondaries that don't target self
                    return secondaries?.Where(effect => effect.Self != null).ToList();
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
                // TODO: Shields Down requires forme change logic for Minior
                // Also provides status immunity in Meteor forme
            },
            [AbilityId.Simple] = new()
            {
                Id = AbilityId.Simple,
                Name = "Simple",
                Num = 86,
                Rating = 4.0,
                Flags = new AbilityFlags { Breakable = true },
                OnChangeBoost = new OnChangeBoostEventInfo((_, boost, _, _, effect) =>
                {
                    if (effect?.Id == ConditionId.None) return; // zpower check not needed in gen9
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
                    if (move.MultiHit is IntIntArrayUnion { IntArray: [_, int max] })
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
                    if (battle.Field.IsWeather(ConditionId.Hail) || battle.Field.IsWeather(ConditionId.Snowscape))
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
                OnImmunity = new OnImmunityEventInfo((_, type, _) =>
                {
                    if (type.IsConditionId && type.AsConditionId == ConditionId.Hail)
                    {
                        // Immune to hail damage
                    }
                }),
                // OnModifyAccuracyPriority = -1
                OnModifyAccuracy = new OnModifyAccuracyEventInfo((battle, accuracy, _, _, _) =>
                {
                    if (accuracy is int acc)
                    {
                        if (battle.Field.IsWeather(ConditionId.Hail) || battle.Field.IsWeather(ConditionId.Snowscape))
                        {
                            battle.Debug("Snow Cloak - decreasing accuracy");
                            battle.ChainModify([3277, 4096]);
                            return battle.FinalModify(acc);
                        }
                    }
                    return accuracy;
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
                OnModifySpA = new OnModifySpAEventInfo((battle, spa, pokemon, _, _) =>
                {
                    if (battle.Field.IsWeather(ConditionId.SunnyDay) || battle.Field.IsWeather(ConditionId.DesolateLand))
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
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, damage, _, target, move) =>
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
                OnAnyFaint = new OnAnyFaintEventInfo((battle, _, _) =>
                {
                    if (battle.EffectState.Target is PokemonEffectStateTarget { Pokemon: var abilityHolder })
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
                        if (battle.EffectState.Target is PokemonEffectStateTarget { Pokemon: var abilityHolder })
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
                // TODO: Stance Change requires forme change logic for Aegislash
                // OnModifyMove changes forme based on whether using King's Shield or attacking
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
                OnFlinch = new OnFlinchEventInfo((battle, _) =>
                {
                    battle.Boost(new SparseBoostsTable { Spe = 1 });
                }),
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
                        move.Secondaries ??= [];
                        // Check if flinch secondary already exists
                        if (!move.Secondaries.Any(s => s.VolatileStatus == ConditionId.Flinch))
                        {
                            move.Secondaries.Add(new SecondaryEffect
                            {
                                Chance = 10,
                                VolatileStatus = ConditionId.Flinch,
                            });
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
                OnTakeItem = new OnTakeItemEventInfo((battle, item, pokemon, source) =>
                {
                    if (battle.ActiveMove == null)
                        throw new InvalidOperationException("Battle.ActiveMove is null");
                    if (pokemon.Hp == 0 || pokemon.Item == ItemId.StickyBarb) return new VoidReturn();
                    if ((source != null && source != pokemon) || battle.ActiveMove.Id == MoveId.KnockOff)
                    {
                        battle.Add("-activate", pokemon, "ability: Sticky Hold");
                        return false;
                    }
                    return new VoidReturn();
                }),
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
                        if (!battle.Boost(new SparseBoostsTable { SpA = 1 }))
                        {
                            battle.Add("-immune", target, "[from] ability: Storm Drain");
                        }
                        return null;
                    }
                    return new VoidReturn();
                }, 1),
                OnAnyRedirectTarget = new OnAnyRedirectTargetEventInfo((battle, _, source, _, move) =>
                {
                    if (move.Type != MoveType.Water || move.Flags.PledgeCombo == true) return null;
                    if (battle.EffectState.Target is not PokemonEffectStateTarget { Pokemon: var abilityHolder })
                        return null;
                    MoveTarget redirectTarget = move.Target is MoveTarget.RandomNormal or MoveTarget.AdjacentFoe
                        ? MoveTarget.Normal : move.Target;
                    if (battle.ValidTarget(abilityHolder, source, redirectTarget))
                    {
                        if (move.SmartTarget == true) move.SmartTarget = false;
                        battle.Add("-activate", abilityHolder, "ability: Storm Drain");
                        return abilityHolder;
                    }
                    return null;
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
                OnDragOut = new OnDragOutEventInfo((battle, pokemon) =>
                {
                    battle.Add("-activate", pokemon, "ability: Suction Cups");
                    return null;
                }, 1),
            },
            [AbilityId.SuperLuck] = new()
            {
                Id = AbilityId.SuperLuck,
                Name = "Super Luck",
                Num = 105,
                Rating = 1.5,
                OnModifyCritRatio = new OnModifyCritRatioEventInfo((_, critRatio, _, _, _) => critRatio + 1),
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
                            battle.Boost(new SparseBoostsTable { Evasion = -1 }, target, pokemon, null, true);
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
                OnEnd = new OnEndEventInfo((battle, _) =>
                {
                    if (battle.EffectState.Counter != null)
                    {
                        // battle.Add("-end", pokemon, $"fallen{battle.EffectState.Counter}", "[silent]");
                    }
                }),
                // OnBasePowerPriority = 21
                OnBasePower = new OnBasePowerEventInfo((battle, basePower, _, _, _) =>
                {
                    if (battle.EffectState.Counter is int fallen and > 0)
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
                        if (battle.EffectState.Target is PokemonEffectStateTarget { Pokemon: var effectHolder })
                        {
                            battle.Add("-block", target, "ability: Sweet Veil", $"[of] {effectHolder}");
                        }
                        return null;
                    }
                    return new VoidReturn();
                }),
                OnAllyTryAddVolatile = new OnAllyTryAddVolatileEventInfo((battle, status, target, _, _) =>
                {
                    if (status.Id == ConditionId.Yawn)
                    {
                        battle.Debug("Sweet Veil blocking yawn");
                        if (battle.EffectState.Target is PokemonEffectStateTarget { Pokemon: var effectHolder })
                        {
                            battle.Add("-block", target, "ability: Sweet Veil", $"[of] {effectHolder}");
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
                    if (battle.Field.IsWeather(ConditionId.RainDance) || battle.Field.IsWeather(ConditionId.PrimordialSea))
                    {
                        battle.ChainModify(2);
                        return battle.FinalModify(spe);
                    }
                    return spe;
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
                    if (battle.EffectState.Target is not PokemonEffectStateTarget { Pokemon: var source })
                        return;
                    var myItemResult = source.TakeItem();
                    if (myItemResult is not ItemItemFalseUnion { Item: var myItem }) return;

                    // TODO: SingleEvent TakeItem check
                    if (!pokemon.SetItem(myItem.Id))
                    {
                        source.Item = myItem.Id;
                        return;
                    }
                    battle.Add("-activate", source, "ability: Symbiosis", myItem.Name, $"[of] {pokemon}");
                }),
            },
            [AbilityId.Synchronize] = new()
            {
                Id = AbilityId.Synchronize,
                Name = "Synchronize",
                Num = 28,
                Rating = 2.0,
                OnAfterSetStatus = new OnAfterSetStatusEventInfo((battle, status, target, source, effect) =>
                {
                    if (source == null || source == target) return;
                    if (effect is Condition { Id: ConditionId.ToxicSpikes }) return;
                    if (status.Id is ConditionId.Sleep or ConditionId.Freeze) return;
                    battle.Add("-activate", target, "ability: Synchronize");
                    // Hack to make status-prevention abilities think Synchronize is a status move
                    source.TrySetStatus(status.Id, target);
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
                    if (battle.EffectState.Target is not PokemonEffectStateTarget { Pokemon: var abilityHolder })
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

            // ==================== 'T' Abilities ====================
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
            [AbilityId.Truant] = new()
            {
                Id = AbilityId.Truant,
                Name = "Truant",
                Num = 54,
                Rating = -1.0,
                // TODO: Truant requires volatile condition implementation
                // OnStart removes truant volatile
                // OnBeforeMove checks and adds truant volatile
            },

            // ==================== 'U' Abilities ====================
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
        };
    }
}
