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

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

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
                AssociatedMove = MoveId.AllySwitch,
                Duration = 2,
                OnStart = new OnStartEventInfo((_, pokemon, _, _) =>
                {
                    pokemon.Volatiles[ConditionId.AllySwitch].Counter = 3;
                    return new VoidReturn();
                }),
                OnRestart = new OnRestartEventInfo((battle, pokemon, _, _) =>
                {
                    int counter = pokemon.Volatiles[ConditionId.AllySwitch].Counter ?? 1;
                    battle.Debug($"Ally Switch success chance: {Math.Round(100.0 / counter)}%");
                    bool success = battle.RandomChance(1, counter);
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
                AssociatedMove = MoveId.AquaRing,
                OnStart = new OnStartEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Add("-start", pokemon, "Aqua Ring");
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo(
                    (battle, pokemon, _, _) => { battle.Heal(pokemon.BaseMaxHp / 16, pokemon); }, 6),
            },
            [ConditionId.Attract] = new()
            {
                Id = ConditionId.Attract,
                Name = "Attract",
                AssociatedMove = MoveId.Attract,
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

                    RelayVar? runEventResult = battle.RunEvent(EventId.Attract, pokemon, source);
                    if (runEventResult is BoolRelayVar { Value: false })
                    {
                        battle.Debug("Attract event failed");
                        return BoolVoidUnion.FromBool(false);
                    }

                    if (effect?.Name == "Cute Charm")
                    {
                        battle.Add("-start", pokemon, "Attract", "[from] ability: Cute Charm",
                            $"[of] {source}");
                    }
                    else if (effect?.Name == "Destiny Knot")
                    {
                        battle.Add("-start", pokemon, "Attract", "[from] item: Destiny Knot",
                            $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-start", pokemon, "Attract");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnUpdate = new OnUpdateEventInfo((battle, pokemon) =>
                {
                    if (pokemon.Volatiles[ConditionId.Attract].Source is { IsActive: false } &&
                        pokemon.Volatiles.ContainsKey(ConditionId.Attract))
                    {
                        battle.Debug($"Removing Attract volatile on {pokemon}");
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Attract]);
                    }
                }),
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Add("-activate", pokemon, "move: Attract",
                        $"[of] {pokemon.Volatiles[ConditionId.Attract].Source}");
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
                AssociatedMove = MoveId.AuroraVeil,
                Duration = 5,
                DurationCallback = new DurationCallbackEventInfo((_, _, source, _) =>
                {
                    if (source != null && source.HasItem(ItemId.LightClay))
                    {
                        return 8;
                    }

                    return 5;
                }),
                OnAnyModifyDamage =
                    new OnAnyModifyDamageEventInfo((battle, damage, source, target, move) =>
                    {
                        if (target != source &&
                            target.Side.GetSideCondition(ConditionId.AuroraVeil) != null)
                        {
                            if ((target.Side.GetSideCondition(ConditionId.Reflect) != null &&
                                 move.Category == MoveCategory.Physical) ||
                                (target.Side.GetSideCondition(ConditionId.LightScreen) != null &&
                                 move.Category == MoveCategory.Special))
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
                OnSideResidual = new OnSideResidualEventInfo((_, _, _, _) =>
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
                    if (!battle.DisplayUi) return new VoidReturn();

                    if (sourceEffect is Item { Id: ItemId.FlameOrb })
                    {
                        battle.Add("-status", target, "brn", "[from] item: Flame Orb");
                    }
                    else if (sourceEffect?.EffectType == EffectType.Ability)
                    {
                        battle.Add("-status", target, "brn", "[from] ability: " +
                                                             sourceEffect.Name, $"[of] {source}");
                    }
                    else
                    {
                        battle.Add("-status", target, "brn");
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
                    // Check if the volatile exists first
                    if (!pokemon.Volatiles.TryGetValue(ConditionId.ChoiceLock,
                            out EffectState? effectState))
                    {
                        battle.Debug(
                            $"[ChoiceLock.OnDisableMove] {pokemon.Name}: No ChoiceLock volatile found");
                        return;
                    }

                    battle.Debug(
                        $"[ChoiceLock.OnDisableMove] {pokemon.Name}: LockedMove={effectState.Move}");

                    // Check if Pokemon still has a choice item and the locked move
                    if (!(pokemon.GetItem().IsChoice ?? false) ||
                        (effectState.Move != null && !pokemon.HasMove((MoveId)effectState.Move)))
                    {
                        battle.Debug(
                            $"[ChoiceLock.OnDisableMove] {pokemon.Name}: Removing volatile");
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.ChoiceLock]);
                        return;
                    }

                    if (pokemon.IgnoringItem()) return;

                    // Only disable moves if a move has been locked
                    if (effectState.Move == null)
                    {
                        battle.Debug(
                            $"[ChoiceLock.OnDisableMove] {pokemon.Name}: No move locked yet");
                        return;
                    }

                    battle.Debug(
                        $"[ChoiceLock.OnDisableMove] {pokemon.Name}: Disabling all except {effectState.Move}");

                    // Disable all moves except the locked move
                    foreach (MoveSlot moveSlot in pokemon.MoveSlots.Where(moveSlot =>
                                 moveSlot.Move != effectState.Move))
                    {
                        pokemon.DisableMove(moveSlot.Id, false,
                            effectState.SourceEffect);
                    }
                }),
            },
            [ConditionId.Counter] = new()
            {
                Id = ConditionId.Counter,
                Name = "Counter",
                Duration = 1,
                NoCopy = true,
                OnStart = new OnStartEventInfo((_, pokemon, _, _) =>
                {
                    // Initialize the volatile's state on the pokemon that has the Counter volatile
                    pokemon.Volatiles[ConditionId.Counter].Slot = null;
                    pokemon.Volatiles[ConditionId.Counter].TotalDamage = 0;
                    return BoolVoidUnion.FromVoid();
                }),
                OnRedirectTarget = new OnRedirectTargetEventInfo(
                    (battle, _, source, _, move) =>
                    {
                        if (move.Id != MoveId.Counter) return PokemonVoidUnion.FromVoid();

                        // source is the pokemon using Counter - check if it has the Counter volatile
                        if (!source.Volatiles.TryGetValue(ConditionId.Counter, out var effectState) ||
                            effectState.Slot == null)
                        {
                            return PokemonVoidUnion.FromVoid();
                        }

                        Pokemon? redirectTarget = battle.GetAtSlot(effectState.Slot);
                        if (redirectTarget != null)
                        {
                            return redirectTarget;
                        }

                        return PokemonVoidUnion.FromVoid();
                    }, -1),
                OnDamagingHit = new OnDamagingHitEventInfo((battle, damage, target, source, move) =>
                {
                    if (source.IsAlly(target)) return;
                    if (battle.GetCategory(move) != MoveCategory.Physical) return;

                    if (!target.Volatiles.TryGetValue(ConditionId.Counter,
                            out EffectState? effectState))
                        return;

                    effectState.Slot = source.GetSlot();
                    effectState.TotalDamage = 2 * damage;
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

                    // Set Time on the pokemon's volatile state, not battle.EffectState
                    target.Volatiles[ConditionId.Confusion].Time = battle.Random(min, 6);
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
                        AbilityId abilityId = pokemon.Ability;
                        if (pokemon.Transformed ||
                            (abilityId != AbilityId.Multitype && battle.Gen >= 8))
                        {
                            return types;
                        }

                        var type = PokemonType.Normal;
                        if (abilityId == AbilityId.Multitype)
                        {
                            Item item = pokemon.GetItem();
                            type = item.OnPlate ?? PokemonType.Normal;
                        }

                        return new[] { type };
                    },
                    1),
            },
            [ConditionId.BanefulBunker] = new()
            {
                Id = ConditionId.BanefulBunker,
                Name = "Baneful Bunker",
                AssociatedMove = MoveId.BanefulBunker,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    battle.Add("-singleturn", target, "move: Protect");
                    return BoolVoidUnion.FromVoid();
                }),
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    if (!(move.Flags.Protect ?? false))
                    {
                        // Z-Moves and Max Moves can break through, but we don't track zBrokeProtect here
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    if (move.SmartTarget ?? false)
                    {
                        move.SmartTarget = false;
                    }
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Protect");
                    }

                    // Reset Outrage counter if locked
                    EffectState? lockedMove = source.GetVolatile(ConditionId.LockedMove);
                    if (lockedMove is not null &&
                        source.Volatiles[ConditionId.LockedMove].Duration == 2)
                    {
                        source.RemoveVolatile(_library.Conditions[ConditionId.LockedMove]);
                    }

                    // Poison on contact
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        source.TrySetStatus(ConditionId.Poison, target);
                    }

                    return new Empty(); // NOT_FAIL - move is blocked but doesn't "fail"
                }, 3),
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    // Handle Z-Moves/Max Moves that break through but still make contact
                    // Note: IsZOrMaxPowered property would need to be checked if available
                    // For now, this handles the case where the move hits through protect
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        source.TrySetStatus(ConditionId.Poison, target);
                    }

                    return BoolEmptyVoidUnion.FromVoid();
                }),
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
            [ConditionId.Bounce] = new()
            {
                Id = ConditionId.Bounce,
                Name = "Bounce",
                EffectType = EffectType.Condition,
                AssociatedMove = MoveId.Bounce,
                Duration = 2,
                OnInvulnerability = new OnInvulnerabilityEventInfo((_, _, _, move) =>
                {
                    // SkyUppercut and ThousandArrows are isNonstandard: "Past" in Gen 9
                    if (move.Id is MoveId.Gust or MoveId.Twister
                        or MoveId.Thunder or MoveId.Hurricane or MoveId.SmackDown)
                    {
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    return BoolIntEmptyVoidUnion.FromBool(false);
                }),
                OnSourceBasePower =
                    new OnSourceBasePowerEventInfo((battle, basePower, _, _, move) =>
                    {
                        if (move.Id is MoveId.Gust or MoveId.Twister)
                        {
                            return battle.ChainModify(2);
                        }

                        return DoubleVoidUnion.FromVoid();
                    }),
            },
            [ConditionId.BurningBulwark] = new()
            {
                Id = ConditionId.BurningBulwark,
                Name = "Burning Bulwark",
                AssociatedMove = MoveId.BurningBulwark,
                Duration = 1,
                OnStart = new OnStartEventInfo((battle, target, _, _) =>
                {
                    battle.Add("-singleturn", target, "move: Protect");
                    return BoolVoidUnion.FromVoid();
                }),
                OnTryHit = new OnTryHitEventInfo((battle, target, source, move) =>
                {
                    // BurningBulwark only blocks damaging moves with protect flag (not Status)
                    if (!(move.Flags.Protect ?? false) || move.Category == MoveCategory.Status)
                    {
                        // Z-Moves and Max Moves can break through, but we don't track zBrokeProtect here
                        return BoolIntEmptyVoidUnion.FromVoid();
                    }

                    if (move.SmartTarget ?? false)
                    {
                        move.SmartTarget = false;
                    }
                    else if (battle.DisplayUi)
                    {
                        battle.Add("-activate", target, "move: Protect");
                    }

                    // Reset Outrage counter if locked
                    EffectState? lockedMove = source.GetVolatile(ConditionId.LockedMove);
                    if (lockedMove is not null &&
                        source.Volatiles[ConditionId.LockedMove].Duration == 2)
                    {
                        source.RemoveVolatile(_library.Conditions[ConditionId.LockedMove]);
                    }

                    // Burn on contact
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        source.TrySetStatus(ConditionId.Burn, target);
                    }

                    return new Empty(); // NOT_FAIL - move is blocked but doesn't "fail"
                }, 3),
                OnHit = new OnHitEventInfo((battle, target, source, move) =>
                {
                    // Handle Z-Moves/Max Moves that break through but still make contact
                    if (battle.CheckMoveMakesContact(move, source, target))
                    {
                        source.TrySetStatus(ConditionId.Burn, target);
                    }

                    return BoolEmptyVoidUnion.FromVoid();
                }),
            },
            [ConditionId.Charge] = new()
            {
                Id = ConditionId.Charge,
                Name = "Charge",
                AssociatedMove = MoveId.Charge,
                OnStart = new OnStartEventInfo((battle, pokemon, _, effect) =>
                {
                    if (effect is { Name: "Electromorphosis" or "Wind Power" })
                    {
                        // TypeScript: this.add('-start', pokemon, 'Charge', this.activeMove!.name, '[from] ability: ' + effect.name)
                        battle.Add("-start", pokemon, "Charge", battle.ActiveMove?.Name ?? "",
                            $"[from] ability: {effect.Name}");
                    }
                    else
                    {
                        battle.Add("-start", pokemon, "Charge");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnRestart = new OnRestartEventInfo((battle, pokemon, _, effect) =>
                {
                    if (effect is { Name: "Electromorphosis" or "Wind Power" })
                    {
                        // TypeScript: this.add('-start', pokemon, 'Charge', this.activeMove!.name, '[from] ability: ' + effect.name)
                        battle.Add("-start", pokemon, "Charge", battle.ActiveMove?.Name ?? "",
                            $"[from] ability: {effect.Name}");
                    }
                    else
                    {
                        battle.Add("-start", pokemon, "Charge");
                    }

                    return BoolVoidUnion.FromVoid();
                }),
                OnEnd = new OnEndEventInfo((battle, pokemon) => { battle.Add("-end", pokemon, "Charge", "[silent]"); }),
                OnBasePower = new OnBasePowerEventInfo((battle, _, _, _, move) =>
                {
                    if (move.Type == MoveType.Electric)
                    {
                        battle.Debug("charge boost");
                        return battle.ChainModify(2);
                    }

                    return DoubleVoidUnion.FromVoid();
                }, 9),
                OnMoveAborted = new OnMoveAbortedEventInfo((_, pokemon, _, move) =>
                {
                    if (move.Type == MoveType.Electric && move.Id != MoveId.Charge)
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Charge]);
                    }
                }),
                OnAfterMove = new OnAfterMoveEventInfo((_, pokemon, _, move) =>
                {
                    if (move.Type == MoveType.Electric && move.Id != MoveId.Charge)
                    {
                        pokemon.RemoveVolatile(_library.Conditions[ConditionId.Charge]);
                    }
                }),
            },
            [ConditionId.ChillyReception] = new()
            {
                Id = ConditionId.ChillyReception,
                Name = "Chilly Reception",
                AssociatedMove = MoveId.ChillyReception,
                Duration = 1,
                OnBeforeMove = new OnBeforeMoveEventInfo((battle, source, _, move) =>
                {
                    // Only show preparation animation for Chilly Reception move
                    if (move.Id != MoveId.ChillyReception) return BoolVoidUnion.FromVoid();

                    battle.Add("-prepare", source, "Chilly Reception", "[premajor]");
                    return BoolVoidUnion.FromVoid();
                }, 100),
            },
            [ConditionId.Curse] = new()
            {
                Id = ConditionId.Curse,
                Name = "Curse",
                AssociatedMove = MoveId.Curse,
                OnStart = new OnStartEventInfo((battle, pokemon, source, _) =>
                {
                    battle.Add("-start", pokemon, "Curse", $"[of] {source}");
                    return BoolVoidUnion.FromVoid();
                }),
                OnResidual = new OnResidualEventInfo((battle, pokemon, _, _) =>
                {
                    battle.Damage(pokemon.BaseMaxHp / 4, pokemon, pokemon,
                        BattleDamageEffect.FromIEffect(_library.Conditions[ConditionId.Curse]));
                }, 12),
            },
            [ConditionId.Commanded] = new()
            {
                Id = ConditionId.Commanded,
                Name = "Commanded",
                EffectType = EffectType.Condition,
                NoCopy = true,
                AssociatedAbility = AbilityId.Commander,
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
                OnTrapPokemon =
                    new OnTrapPokemonEventInfo(
                        (_, pokemon) => { pokemon.Trapped = PokemonTrapped.True; }, -11),
            },
            [ConditionId.Commanding] = new()
            {
                Id = ConditionId.Commanding,
                Name = "Commanding",
                EffectType = EffectType.Condition,
                NoCopy = true,
                AssociatedAbility = AbilityId.Commander,
                // This is applied to Tatsugiri when it uses Commander (hides inside Dondozo)
                OnDragOut = new OnDragOutEventInfo((battle, pokemon, _, _) =>
                {
                    if (battle.DisplayUi)
                    {
                        battle.Add("-block", pokemon, "Commanding");
                    }

                    return null; // Prevent drag-out
                }, 2),
                OnTrapPokemon =
                    new OnTrapPokemonEventInfo(
                        (_, pokemon) => { pokemon.Trapped = PokemonTrapped.True; }, -11),
                OnInvulnerability = new OnInvulnerabilityEventInfo((_, _, _, _) =>
                    BoolIntEmptyVoidUnion.FromBool(false)),
                OnBeforeTurn = new OnBeforeTurnEventInfo((battle, pokemon) =>
                {
                    // Cancel Tatsugiri's action since it's hiding
                    battle.Queue.CancelAction(pokemon);
                }),
            },
            // CantUseTwice volatile condition placeholder - tracks that Blood Moon was used last turn
            [ConditionId.BloodMoon] = new()
            {
                Id = ConditionId.BloodMoon,
                Name = "Blood Moon",
                AssociatedMove = MoveId.BloodMoon,
                EffectType = EffectType.Condition,
                // No handlers needed - this is just a volatile marker for cantUseTwice tracking
            },
        };
    }
}