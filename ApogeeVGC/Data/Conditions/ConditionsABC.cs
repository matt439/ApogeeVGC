using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Events.Handlers.SideEventMethods;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsAbc()
    {
        return new Dictionary<ConditionId, Condition>
        {
            [ConditionId.AllySwitch] = new()
            {
                Id = ConditionId.AllySwitch,
                Name = "Ally Switch",
                Duration = 2,
                OnRestart = new OnRestartEventInfo((battle, pokemon, _, _) =>
                {
                    var counter = pokemon.Volatiles[ConditionId.AllySwitch].Counter ?? 1;
                    battle.Debug($"Ally Switch success chance: {Math.Round(100.0 / counter)}%");
                    var success = battle.RandomChance(1, counter);
                    if (!success)
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.AllySwitch]);
                        return BoolVoidUnion.FromBool(false);
                    }
                    if (pokemon.Volatiles[ConditionId.AllySwitch].Counter < 729) // CounterMax
                    {
                        pokemon.Volatiles[ConditionId.AllySwitch].Counter *= 3;
                    }
                    pokemon.Volatiles[ConditionId.AllySwitch].Duration = 2;
                    return BoolVoidUnion.FromVoid();
                }),
            },
            [ConditionId.AquaRing] = new()
            {
                Id = ConditionId.AquaRing,
                Name = "Aqua Ring",
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Add("-start", pokemon, "Aqua Ring");
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Heal(pokemon.BaseMaxHp / 16, pokemon);
                }, 6),
            },
            [ConditionId.Attract] = new()
            {
                Id = ConditionId.Attract,
                Name = "Attract",
                NoCopy = true,
                OnStart = new OnStartEventInfo((battle, pokemon, source, effect) =>
                {
                    if (source == null) return BoolVoidUnion.FromVoid();
                    if (!((pokemon.Gender == GenderId.M && source.Gender == GenderId.F) ||
                          (pokemon.Gender == GenderId.F && source.Gender == GenderId.M)))
                    {
                        battle.Debug("incompatible gender");
                        return BoolVoidUnion.FromBool(false);
                    }
                    var runEventResult = battle.RunEvent(EventId.Attract, pokemon, source);
                    if (runEventResult is BoolRelayVar { Value: false })
                    {
                        battle.Debug("Attract event failed");
                        return BoolVoidUnion.FromBool(false);
                    }

                    if (effect?.Name == "Cute Charm")
                    {
                        battle.Add("-start", pokemon, "Attract", "[from] ability: Cute Charm", $"[of] {source}");
                    }
                    else if (effect?.Name == "Destiny Knot")
                    {
                        battle.Add("-start", pokemon, "Attract", "[from] item: Destiny Knot", $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-start", pokemon, "Attract");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Volatiles[ConditionId.Attract].Source != null &&
                        !pokemon.Volatiles[ConditionId.Attract].Source.IsActive &&
                        pokemon.Volatiles.ContainsKey(ConditionId.Attract))
                    {
                        battle.Debug($"Removing Attract volatile on {pokemon}");
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Attract]);
                    }
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Add("-activate", pokemon, "move: Attract", $"[of] {pokemon.Volatiles[ConditionId.Attract].Source}");
                    if (battle.RandomChance(1, 2))
                    {
                        battle.Add("cant", pokemon, "Attract");
                        return BoolVoidUnion.FromBool(false);
                    }
                    return BoolVoidUnion.FromVoid();
                }, 2),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    battle.Add("-end", pokemon, "Attract", "[silent]");
                }),
            },
            [ConditionId.AuroraVeil] = new()
            {
                Id = ConditionId.AuroraVeil,
                Name = "Aurora Veil",
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((battle, target, source, _) =>
                {
                    if (source != null && source.HasItem(ItemId.LightClay))
                    {
                        return 8;
                    }
                    return 5;
                }),
                OnAnyModifyDamage = new OnAnyModifyDamageEventInfo((battle, damage, source, target, move) =>
                {
                    if (target != source && target.Side.GetSideCondition(ConditionId.AuroraVeil) != null)
                    {
                        if ((target.Side.GetSideCondition(ConditionId.Reflect) != null && move.Category == MoveCategory.Physical) ||
                            (target.Side.GetSideCondition(ConditionId.LightScreen) != null && move.Category == MoveCategory.Special))
                        {
                            return damage;
                        }
                        if (!target.GetMoveHitData(move).Crit && !(move.Infiltrates ?? false))
                        {
                            battle.Debug("Aurora Veil weaken");
                            if (battle.ActivePerHalf > 1) 
                            {
                                battle.ChainModify(2732, 4096);
                                return damage;
                            }
                            battle.ChainModify(0.5);
                            return damage;
                        }
                    }
                    return damage;
                }),
                OnSideStart = new OnSideStartEventInfo((battle, side, _, _) =>
                {
                    battle.Add("-sidestart", side, "move: Aurora Veil");
                }),
                OnSideResidual = new OnSideResidualEventInfo((battle, side, _, _) =>
                {
                    // Handled by duration
                }, 26, 10),
                OnSideEnd = new OnSideEndEventInfo((battle, side) =>
                {
                    battle.Add("-sideend", side, "move: Aurora Veil");
                }),
            },
            [ConditionId.Burn] = new()
            {
                Id = ConditionId.Burn,
                Name = "Burn",
                EffectType = EffectType.Status,
                ImmuneTypes = [PokemonType.Fire],
                OnStart = new OnStartEventInfo((battle, target, source, sourceEffect) =>
                {
                    if (sourceEffect is null)
                    {
                        throw new ArgumentNullException(
                            $"Source effect is null when trying to apply" +
                            $"{ConditionId.Burn} to" + $"pokemon {target.Name}.");
                    }

                    if (!battle.DisplayUi) return new VoidReturn();

                    switch (sourceEffect.EffectType)
                    {
                        case EffectType.Item:
                            if (sourceEffect is Item { Id: ItemId.FlameOrb })
                            {
                                battle.Add("-status", target, "brn", "[from] item: Flame Orb");
                            }

                            break;
                        case EffectType.Ability:
                            if (sourceEffect is Ability)
                            {
                                battle.Add("-status", target, "brn", "[from] ability: " +
                                    sourceEffect.Name, $"[of] {source}");
                            }

                            break;
                        case EffectType.Move:
                            battle.Add("-status", target, "brn");
                            break;
                    }

                    return new VoidReturn();
                }),
                //OnResidualOrder = 10,
                OnResidual = new OnResidualEventInfo(
                    (battle, pokemon, _, _) => { battle.Damage(pokemon.BaseMaxHp / 16); },
                    10),
            },
            [ConditionId.ChoiceLock] = new()
            {
                Id = ConditionId.ChoiceLock,
                NoCopy = true,
                EffectType = EffectType.Condition,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    // Don't set the move here - it will be set in OnModifyMove
                    // This handler is called when the volatile is added, which happens
                    // during the Choice item's OnModifyMove
                    battle.Debug(
                        $"[ChoiceLock.OnStart] {pokemon.Name}: Volatile added, move will be set by ChoiceLock.OnModifyMove");
                    return new VoidReturn();
                }),
                OnModifyMove = new OnModifyMoveEventInfo((battle, move, pokemon, _) =>
                {
                    // Set the locked move if it hasn't been set yet
                    if (pokemon.Volatiles[ConditionId.ChoiceLock].Move == null)
                    {
                        battle.Debug(
                            $"[ChoiceLock.OnModifyMove] {pokemon.Name}: Locking to {move.Id}");
                        pokemon.Volatiles[ConditionId.ChoiceLock].Move = move.Id;
                    }
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, move) =>
                {
                    battle.Debug(
                        $"[ChoiceLock.OnBeforeMove] {pokemon.Name}: Attempting {move.Id}, Locked={pokemon.Volatiles[ConditionId.ChoiceLock].Move}, HasItem={pokemon.GetItem().IsChoice}");

                    if (!(pokemon.GetItem().IsChoice ?? false))
                    {
                        battle.Debug(
                            $"[ChoiceLock.OnBeforeMove] {pokemon.Name}: No choice item, removing volatile");
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                        return new VoidReturn();
                    }

                    if (pokemon.IgnoringItem())
                    {
                        battle.Debug($"[ChoiceLock.OnBeforeMove] {pokemon.Name}: Ignoring item");
                        return new VoidReturn();
                    }

                    // The move should already be locked by OnModifyMove
                    // Check if attempting to use a different move
                    var lockedMove = pokemon.Volatiles[ConditionId.ChoiceLock].Move;
                    if (lockedMove != null && move.Id != lockedMove && move.Id != MoveId.Struggle)
                    {
                        // Move is blocked by choice lock
                        battle.Debug(
                            $"[ChoiceLock.OnBeforeMove] {pokemon.Name}: Move {move.Id} BLOCKED!");

                        if (!battle.DisplayUi) return false;

                        battle.AddMove("move", StringNumberDelegateObjectUnion.FromObject(pokemon),
                            move.Name);
                        battle.AttrLastMove("[still]");
                        battle.Debug("Disabled by Choice item lock");
                        battle.Add("-fail", pokemon);
                        return false;
                    }

                    battle.Debug($"[ChoiceLock.OnBeforeMove] {pokemon.Name}: Move allowed");
                    return new VoidReturn();
                }),
                OnDisableMove = new OnDisableMoveEventInfo((battle, pokemon) =>
                {
                    battle.Debug(
                        $"[ChoiceLock.OnDisableMove] {pokemon.Name}: LockedMove={pokemon.Volatiles[ConditionId.ChoiceLock].Move}");

                    // Check if Pokemon still has a choice item and the locked move
                    if (!(pokemon.GetItem().IsChoice ?? false) ||
                        (pokemon.Volatiles[ConditionId.ChoiceLock].Move != null &&
                         !pokemon.HasMove((MoveId)pokemon.Volatiles[ConditionId.ChoiceLock].Move)))
                    {
                        battle.Debug(
                            $"[ChoiceLock.OnDisableMove] {pokemon.Name}: Removing volatile");
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                        return;
                    }

                    if (pokemon.IgnoringItem()) return;

                    // Only disable moves if a move has been locked
                    if (pokemon.Volatiles[ConditionId.ChoiceLock].Move == null)
                    {
                        battle.Debug(
                            $"[ChoiceLock.OnDisableMove] {pokemon.Name}: No move locked yet");
                        return;
                    }

                    battle.Debug(
                        $"[ChoiceLock.OnDisableMove] {pokemon.Name}: Disabling all except {pokemon.Volatiles[ConditionId.ChoiceLock].Move}");

                    // Disable all moves except the locked move
                    foreach (MoveSlot moveSlot in pokemon.MoveSlots.Where(moveSlot =>
                                 moveSlot.Move != pokemon.Volatiles[ConditionId.ChoiceLock].Move))
                    {
                        pokemon.DisableMove(moveSlot.Id, false,
                            pokemon.Volatiles[ConditionId.ChoiceLock].SourceEffect);
                    }
                }),
            },
            [ConditionId.Confusion] = new()
            {
                Id = ConditionId.Confusion,
                Name = "Confusion",
                EffectType = EffectType.Condition,
                OnStart = new OnStartEventInfo((battle, target, source, sourceEffect) =>
                {
                    if (battle.DisplayUi)
                    {
                        switch (sourceEffect)
                        {
                            case Condition { Id: ConditionId.LockedMove }:
                                battle.Add("-start", target, "confusion", "[fatigue]");
                                break;
                            case Ability:
                                battle.Add("-start", target, "confusion", "[from] ability: " +
                                    sourceEffect.Name, $"[of] {source}");
                                break;
                            default:
                                battle.Add("-start", target, "confusion");
                                break;
                        }
                    }

                    int min = 2;
                    if (sourceEffect is ActiveMove { Id: MoveId.AxeKick })
                    {
                        min = 3;
                    }

                    battle.EffectState.Time = battle.Random(min, 6);
                    return new VoidReturn();
                }),
                OnEnd = new OnEndEventInfo((battle, target) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-end", target, "confusion");
                    }
                }),
                //OnBeforeMovePriority = 3,
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                    {
                        pokemon.Volatiles[ConditionId.Confusion].Time--;
                        if (pokemon.Volatiles[ConditionId.Confusion].Time < 1)
                        {
                            pokemon.RemoveVolatile(_library.Conditions[ConditionId.Confusion]);
                            return new VoidReturn();
                        }

                        if (battle.DisplayUi)
                        {
                            battle.Add("-activate", pokemon, "confusion");
                        }

                        if (!battle.RandomChance(33, 100))
                        {
                            return new VoidReturn();
                        }

                        battle.ActiveTarget = pokemon;
                        int damage = battle.Actions.GetConfusionDamage(pokemon, 40);

                        ActiveMove activeMove = new()
                        {
                            Name = "Confused",
                            Id = MoveId.Confused,
                            Accuracy = IntTrueUnion.FromTrue(),
                            Num = 100200,
                            Type = MoveType.Normal,
                            MoveSlot = new MoveSlot(),
                        };
                        battle.Damage(damage, pokemon, pokemon,
                            BattleDamageEffect.FromIEffect(activeMove));
                        return false;
                    },
                    3),
            },
            [ConditionId.Arceus] = new()
            {
                Id = ConditionId.Arceus,
                Name = "Arceus",
                EffectType = EffectType.Condition,
                //OnTypePriority = 1,
                OnType = new OnTypeEventInfo((battle, types, pokemon) =>
                    {
                        var abilityId = pokemon.Ability;
                        if (pokemon.Transformed ||
                            (abilityId != AbilityId.Multitype && battle.Gen >= 8))
                        {
                            return types;
                        }

                        PokemonType type = PokemonType.Normal;
                        if (abilityId == AbilityId.Multitype)
                        {
                            var item = pokemon.GetItem();
                            // OnPlate property not yet implemented on Item
                            // Default to Normal type for now
                            type = PokemonType.Normal;
                        }

                        return new PokemonType[] { type };
                    },
                    1),
            },
            [ConditionId.BanefulBunker] = new()
            {
                Id = ConditionId.BanefulBunker,
                Name = "Baneful Bunker",
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    battle.Add("-singleturn", target, "move: Protect");
                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: Implement full BanefulBunker logic - needs OnTryHit handler with protect logic and contact poison
            },
            [ConditionId.BeakBlast] = new()
            {
                Id = ConditionId.BeakBlast,
                Name = "Beak Blast",
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Add("-singleturn", pokemon, "move: Beak Blast");
                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: Implement contact checking to burn attackers
            },
            [ConditionId.Block] = new()
            {
                Id = ConditionId.Block,
                Name = "Block",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Block,
                // Block just adds the 'trapped' volatile with 'trapper' marker
                // The actual implementation is in the Trapped condition
                // This is a marker condition
            },
            [ConditionId.Bide] = new()
            {
                Id = ConditionId.Bide,
                Name = "Bide",
                Duration = 3,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Add("-start", pokemon, "move: Bide");
                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: Implement Bide full logic - requires OnLockMove, OnDamage tracking, OnBeforeMove
                // This is complex logic that would need multiple event handlers and state tracking
            },
            [ConditionId.Bounce] = new()
            {
                Id = ConditionId.Bounce,
                Name = "Bounce",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Bounce,
                Duration = 2,
                OnImmunity = new OnImmunityEventInfo((battle, type, pokemon) =>
                {
                    // Immune to Sandstorm and Hail damage while in the air
                    // This is void-returning - immunity is handled by the caller
                }),
                OnInvulnerability = new OnInvulnerabilityEventInfo((battle, target, source, move) =>
                {
                    if (move.Id == MoveId.Gust || move.Id == MoveId.Twister ||
                        move.Id == MoveId.SkyUppercut || move.Id == MoveId.Thunder ||
                        move.Id == MoveId.Hurricane || move.Id == MoveId.SmackDown ||
                        move.Id == MoveId.ThousandArrows)
                    {
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }
                    return BoolIntEmptyVoidUnion.FromBool(false);
                }),
                OnSourceModifyDamage = new OnSourceModifyDamageEventInfo((battle, damage, source, target, move) =>
                {
                    if (move.Id == MoveId.Gust || move.Id == MoveId.Twister)
                    {
                        return battle.ChainModify(2);
                    }
                    return damage;
                }),
            },
            [ConditionId.BurningBulwark] = new()
            {
                Id = ConditionId.BurningBulwark,
                Name = "Burning Bulwark",
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    battle.Add("-singleturn", target, "move: Protect");
                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: Implement full BurningBulwark logic - needs OnTryHit handler with protect logic and contact burn
            },
            [ConditionId.Charge] = new()
            {
                Id = ConditionId.Charge,
                Name = "Charge",
                OnStart = new OnStartEventInfo((battle, pokemon, source, effect) =>
                {
                    if (effect != null && (effect.Name == "Electromorphosis" || effect.Name == "Wind Power"))
                    {
                        battle.Add("-start", pokemon, "Charge", $"[from] ability: {effect.Name}");
                    }
                    else
                    {
                        battle.Add("-start", pokemon, "Charge");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnRestart = new OnRestartEventInfo((battle, pokemon, source, effect) =>
                {
                    if (effect != null && (effect.Name == "Electromorphosis" || effect.Name == "Wind Power"))
                    {
                        battle.Add("-start", pokemon, "Charge", $"[from] ability: {effect.Name}");
                    }
                    else
                    {
                        battle.Add("-start", pokemon, "Charge");
                    }
                    return BoolVoidUnion.FromVoid();
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) =>
                {
                    battle.Add("-end", pokemon, "Charge", "[silent]");
                }),
                // TODO: OnBasePower - doubles Electric move power
                // TODO: OnMoveAborted/OnAfterMove - removes charge after Electric move use
            },
            [ConditionId.ChillyReception] = new()
            {
                Id = ConditionId.ChillyReception,
                Name = "Chilly Reception",
                // TODO: Implement ChillyReception volatile for preparation phase
            },
            [ConditionId.Curse] = new()
            {
                Id = ConditionId.Curse,
                Name = "Curse",
                OnStart = new OnStartEventInfo((battle, pokemon, source, _) =>
                {
                    battle.Add("-start", pokemon, "Curse", $"[of] {source}");
                    return BoolVoidUnion.FromVoid();
                }),
                // TODO: OnResidual - deals 1/4 max HP damage per turn
            },
            [ConditionId.Commanded] = new()
            {
                Id = ConditionId.Commanded,
                Name = "Commanded",
                EffectType = EffectType.Condition,
                NoCopy = true,
                // This is applied to Dondozo when Tatsugiri uses Commander
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    // Boost all stats by 2 stages
                    battle.Boost(new SparseBoostsTable
                    {
                        Atk = 2,
                        Def = 2,
                        SpA = 2,
                        SpD = 2,
                        Spe = 2
                    }, pokemon);
                    return BoolVoidUnion.FromVoid();
                }),
                OnDragOut = new OnDragOutEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-block", pokemon, "Commanded");
                    }
                    return null; // Prevent drag-out
                }, 2),
                OnTrapPokemon = new OnTrapPokemonEventInfo((battle, pokemon) =>
                {
                    pokemon.Trapped = PokemonTrapped.True;
                }, -11),
            },
            [ConditionId.Commanding] = new()
            {
                Id = ConditionId.Commanding,
                Name = "Commanding",
                EffectType = EffectType.Condition,
                NoCopy = true,
                // This is applied to Tatsugiri when it uses Commander (hides inside Dondozo)
                OnDragOut = new OnDragOutEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-block", pokemon, "Commanding");
                    }
                    return null; // Prevent drag-out
                }, 2),
                OnTrapPokemon = new OnTrapPokemonEventInfo((battle, pokemon) =>
                {
                    pokemon.Trapped = PokemonTrapped.True;
                }, -11),
                OnInvulnerability = new OnInvulnerabilityEventInfo((_, _, _, _) =>
                {
                    // Tatsugiri is invulnerable while commanding
                    return BoolIntEmptyVoidUnion.FromBool(false);
                }),
                OnBeforeTurn = new OnBeforeTurnEventInfo((battle, pokemon) =>
                {
                    // Cancel Tatsugiri's action since it's hiding
                    // TODO: Implement battle.queue.cancelAction(pokemon)
                }),
            },
        };
    }
}