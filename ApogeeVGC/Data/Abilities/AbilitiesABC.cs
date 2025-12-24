using ApogeeVGC.Sim.Abilities;
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
                OnModifyStab = new OnModifyStabEventInfo((battle, stab, source, target, move) =>
                {
                    if ((move.ForceStab ?? false) ||
                        source.HasType(move.Type.ConvertToPokemonType()))
                    {
                        if (stab == 2)
                        {
                            return 2.25;
                        }

                        return 2.0;
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
                        // Change Normal-type moves to Flying
                        // TODO: Add checks for specific moves like Judgment, Multi-Attack, etc.
                        if (move.Type == MoveType.Normal && move.Category != MoveCategory.Status)
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
                        foreach (var target in battle.GetAllActive())
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
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    if (target.Hp == 0) return new VoidReturn();
                    if (move is not ActiveMove am) return new VoidReturn();
                    if (!target.GetMoveHitData(am).Crit) return new VoidReturn();

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
                OnDamage = new OnDamageEventInfo((battle, _, target, source, effect) =>
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
                        ItemId.BerryJuice
                    ];

                    if (healingItems.Contains(item.Id))
                    {
                        return BoolVoidUnion.FromBool(battle.EffectState.CheckedAngerShell ?? true);
                    }

                    return BoolVoidUnion.FromBool(true);
                })),
                OnAfterMoveSecondary = new OnAfterMoveSecondaryEventInfo((battle, target, source, move) =>
                {
                    battle.EffectState.CheckedAngerShell = true;
                    if (source is null || source == target || target.Hp == 0) return;
                    if (move.TotalDamage is not IntIntFalseUnion totalDamage) return;

                    var lastAttackedBy = target.GetLastAttackedBy();
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
                    foreach (var target in pokemon.Foes())
                    {
                        foreach (var moveSlot in target.MoveSlots)
                        {
                            var move = battle.Library.Moves[moveSlot.Id];
                            if (move.Category == MoveCategory.Status) continue;

                            // Get the move type (Hidden Power is not used in Gen 9, so we just use move type directly)
                            PokemonType moveType = move.Type.ConvertToPokemonType();

                            // Check for super-effective or OHKO moves
                            if ((battle.Dex.GetImmunity(moveType.ConvertToMoveType(), pokemon) &&
                                 battle.Dex.GetEffectiveness(moveType.ConvertToMoveType(), pokemon).ToModifier() > 0) ||
                                move.Ohko != null)
                            {
                                battle.Add("-ability", pokemon, "Anticipation");
                                return;
                            }
                        }
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
                    if (battle.EffectState.Target is not PokemonEffectStateTarget { Pokemon: var abilityHolder })
                        return;
                    if (!pokemon.IsAdjacent(abilityHolder)) return;
                    if (pokemon.IsGrounded() == true)
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
                    string[] targetAllExceptions = ["perishsong", "flowershield", "rototiller"];
                    if (move.Target == MoveTarget.FoeSide ||
                        (move.Target == MoveTarget.All && !targetAllExceptions.Contains(move.Id.ToString().ToLower())))
                    {
                        return new VoidReturn();
                    }

                    if (battle.EffectState.Target is not PokemonEffectStateTarget { Pokemon: var armorTailHolder })
                        return new VoidReturn();

                    if ((source.IsAlly(armorTailHolder) || move.Target == MoveTarget.All) && move.Priority > 0.1)
                    {
                        battle.AttrLastMove("[still]");
                        battle.Add("cant", armorTailHolder, "ability: Armor Tail", move.Name, $"[of] {target}");
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
                // TODO: OnAllyTryAddVolatile - blocks attract, disable, encore, healblock, taunt, torment
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
        };
    }
}
