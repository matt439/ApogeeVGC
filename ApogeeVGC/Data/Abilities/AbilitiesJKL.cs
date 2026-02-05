using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.AbilityEventMethods;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace ApogeeVGC.Data.Abilities;

public partial record Abilities
{
    private partial Dictionary<AbilityId, Ability> CreateAbilitiesJkl()
    {
        return new Dictionary<AbilityId, Ability>
        {
            // ==================== 'J' Abilities ====================
            [AbilityId.Justified] = new()
            {
                Id = AbilityId.Justified,
                Name = "Justified",
                Num = 154,
                Rating = 2.5,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, _, _, move) =>
                {
                    if (move.Type == MoveType.Dark)
                    {
                        battle.Boost(new SparseBoostsTable { Atk = 1 });
                    }
                }),
            },

            // ==================== 'K' Abilities ====================
            [AbilityId.KeenEye] = new()
            {
                Id = AbilityId.KeenEye,
                Name = "Keen Eye",
                Num = 51,
                Rating = 0.5,
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
                                "[from] ability: Keen Eye", $"[of] {target}");
                        }
                    }
                }),
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) => { move.IgnoreEvasion = true; }),
            },
            [AbilityId.Klutz] = new()
            {
                Id = AbilityId.Klutz,
                Name = "Klutz",
                Num = 103,
                Rating = -1.0,
                // Item suppression is implemented in Pokemon.IgnoringItem()
                // Note: Klutz activates early enough via OnSwitchInPriority: 1 to beat most items
                // In TypeScript this is onStart with onSwitchInPriority: 1
                OnStart = new OnStartEventInfo((battle, pokemon) =>
                {
                    // End the item effect when switching in
                    Item item = pokemon.GetItem();
                    battle.SingleEvent(EventId.End, item, pokemon.ItemState, pokemon);
                }, priority: 1),
            },

            // ==================== 'L' Abilities ====================
            [AbilityId.LeafGuard] = new()
            {
                Id = AbilityId.LeafGuard,
                Name = "Leaf Guard",
                Num = 102,
                Rating = 0.5,
                Flags = new AbilityFlags { Breakable = true },
                OnSetStatus = new OnSetStatusEventInfo((battle, _, target, _, effect) =>
                {
                    ConditionId weather = target.EffectiveWeather();
                    if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                    {
                        if (effect is ActiveMove { Status: not ConditionId.None })
                        {
                            battle.Add("-immune", target, "[from] ability: Leaf Guard");
                        }

                        return false;
                    }

                    return new VoidReturn();
                }),
                OnTryAddVolatile = new OnTryAddVolatileEventInfo((battle, status, target, _, _) =>
                {
                    if (status.Id == ConditionId.Yawn)
                    {
                        ConditionId weather = target.EffectiveWeather();
                        if (weather is ConditionId.SunnyDay or ConditionId.DesolateLand)
                        {
                            battle.Add("-immune", target, "[from] ability: Leaf Guard");
                            return null;
                        }
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.Levitate] = new()
            {
                Id = AbilityId.Levitate,
                Name = "Levitate",
                Num = 26,
                Rating = 3.5,
                Flags = new AbilityFlags { Breakable = true },
                // Airborneness is implemented in Pokemon.IsGrounded()
            },
            [AbilityId.Libero] = new()
            {
                Id = AbilityId.Libero,
                Name = "Libero",
                Num = 236,
                Rating = 4.0,
                OnPrepareHit = new OnPrepareHitEventInfo((battle, source, _, move) =>
                {
                    if (battle.EffectState.Libero == true) return new VoidReturn();
                    // Note: TypeScript also checks move.sourceEffect === 'snatch', but Snatch
                    // was removed in Gen 8+, so this check is omitted for Gen 9 targeting.
                    if (move.HasBounced == true || move.Flags.FutureMove == true ||
                        move.CallsMove == true)
                        return new VoidReturn();

                    MoveType type = move.Type;
                    PokemonType pokemonType = type.ConvertToPokemonType();
                    var currentTypes = source.GetTypes();

                    // TypeScript: source.getTypes().join() !== type
                    // Only change type if current types (joined) don't match the move type
                    // This means: only skip type change if Pokemon is mono-typed with exactly that type
                    string joinedTypes = string.Join(",", currentTypes.Select(t => t.ToString()));
                    var moveTypeStr = pokemonType.ToString();

                    if (type != MoveType.Unknown && joinedTypes != moveTypeStr)
                    {
                        if (!source.SetType(pokemonType)) return new VoidReturn();
                        battle.EffectState.Libero = true;
                        battle.Add("-start", source, "typechange", type.ToString(),
                            "[from] ability: Libero");
                    }

                    return new VoidReturn();
                }),
            },
            [AbilityId.LightMetal] = new()
            {
                Id = AbilityId.LightMetal,
                Name = "Light Metal",
                Num = 135,
                Rating = 1.0,
                Flags = new AbilityFlags { Breakable = true },
                OnModifyWeight =
                    new OnModifyWeightEventInfo((battle, weighthg, _) => battle.Trunc(weighthg / 2)),
            },
            [AbilityId.LightningRod] = new()
            {
                Id = AbilityId.LightningRod,
                Name = "Lightning Rod",
                Num = 31,
                Rating = 3.0,
                Flags = new AbilityFlags { Breakable = true },
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (target != source && move.Type == MoveType.Electric)
                    {
                        BoolZeroUnion? boostResult =
                            battle.Boost(new SparseBoostsTable { SpA = 1 });
                        if (boostResult is not BoolBoolZeroUnion { Value: true })
                        {
                            battle.Add("-immune", target, "[from] ability: Lightning Rod");
                        }

                        return null;
                    }

                    return new VoidReturn();
                }),
                OnAnyRedirectTarget =
                    new OnAnyRedirectTargetEventInfo((battle, target, source, _, move) =>
                    {
                        if (move.Type != MoveType.Electric || move.Flags.PledgeCombo == true)
                            return target;

                        MoveTarget redirectTarget =
                            move.Target is MoveTarget.RandomNormal or MoveTarget.AdjacentFoe
                                ? MoveTarget.Normal
                                : move.Target;

                        if (battle.EffectState.Target is PokemonEffectStateTarget
                            {
                                Pokemon: var abilityHolder,
                            })
                        {
                            if (battle.ValidTarget(abilityHolder, source, redirectTarget))
                            {
                                if (move.SmartTarget == true) move.SmartTarget = false;
                                if (abilityHolder != target)
                                {
                                    battle.Add("-activate", abilityHolder,
                                        "ability: Lightning Rod");
                                }

                                return abilityHolder;
                            }
                        }

                        return target;
                    }),
            },
            [AbilityId.Limber] = new()
            {
                Id = AbilityId.Limber,
                Name = "Limber",
                Num = 7,
                Rating = 2.0,
                Flags = new AbilityFlags { Breakable = true },
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Status == ConditionId.Paralysis)
                    {
                        battle.Add("-activate", pokemon, "ability: Limber");
                        pokemon.CureStatus();
                    }
                }),
                OnSetStatus = new OnSetStatusEventInfo((battle, status, target, _, effect) =>
                {
                    if (status.Id != ConditionId.Paralysis) return new VoidReturn();
                    if (effect is ActiveMove { Status: not ConditionId.None })
                    {
                        battle.Add("-immune", target, "[from] ability: Limber");
                    }

                    return false;
                }),
            },
            [AbilityId.LingeringAroma] = new()
            {
                Id = AbilityId.LingeringAroma,
                Name = "Lingering Aroma",
                Num = 268,
                Rating = 2.0,
                OnDamagingHit = new OnDamagingHitEventInfo((battle, _, target, source, move) =>
                {
                    Ability sourceAbility = source.GetAbility();
                    if (sourceAbility.Flags.CantSuppress == true ||
                        sourceAbility.Id == AbilityId.LingeringAroma)
                    {
                        return;
                    }

                    if (battle.CheckMoveMakesContact(move, source, target, !source.IsAlly(target)))
                    {
                        AbilityIdFalseUnion? oldAbilityResult =
                            source.SetAbility(AbilityId.LingeringAroma, target);
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
                            battle.Add("-activate", target, "ability: Lingering Aroma",
                                oldAbilityName, $"[of] {source}");
                        }
                    }
                }),
            },
            [AbilityId.LiquidOoze] = new()
            {
                Id = AbilityId.LiquidOoze,
                Name = "Liquid Ooze",
                Num = 64,
                Rating = 2.5,
                OnSourceTryHeal = new OnSourceTryHealEventInfo(
                    (Func<Battle, int, Pokemon, Pokemon, IEffect, IntBoolUnion?>)((battle, damage,
                        target, source, effect) =>
                    {
                        // Null check: effect can be null for healing from sources without an associated effect
                        if (effect == null)
                        {
                            return null;
                        }

                        battle.Debug($"Heal is occurring: {target} <- {source} :: {effect.Name}");

                        // Check if this healing effect should trigger Liquid Ooze damage
                        // TypeScript: const canOoze = ['drain', 'leechseed', 'strengthsap'];
                        bool shouldOoze = effect switch
                        {
                            Condition c => c.Id is ConditionId.Drain or ConditionId.LeechSeed,
                            ActiveMove m => m.Id == MoveId.StrengthSap,
                            _ => false,
                        };

                        if (shouldOoze)
                        {
                            battle.Damage(damage);
                            return 0;
                        }

                        return
                            null; // Return null to allow default heal behavior (matches TypeScript returning undefined)
                    })),
            },
            [AbilityId.LiquidVoice] = new()
            {
                Id = AbilityId.LiquidVoice,
                Name = "Liquid Voice",
                Num = 204,
                Rating = 1.5,
                // OnModifyTypePriority = -1
                OnModifyType = new OnModifyTypeEventInfo((_, move, _, _) =>
                {
                    if (move.Flags.Sound == true)
                    {
                        move.Type = MoveType.Water;
                    }
                }, -1),
            },
            [AbilityId.LongReach] = new()
            {
                Id = AbilityId.LongReach,
                Name = "Long Reach",
                Num = 203,
                Rating = 1.0,
                OnModifyMove = new OnModifyMoveEventInfo((_, move, _, _) => { move.Flags.Contact = false; }),
            },
        };
    }
}