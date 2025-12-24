using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Events.Handlers.EventMethods;
using ApogeeVGC.Sim.Events.Handlers.ConditionSpecific;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    private partial Dictionary<ConditionId, Condition> CreateConditionsABC()
    {
        return new Dictionary<ConditionId, Condition>
        {
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
        };
    }
}