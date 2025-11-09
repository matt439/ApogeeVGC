using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    /// <summary>
    /// Applies damage to a single Pokémon.
    /// This is a convenience wrapper around SpreadDamage for single-target damage.
    /// </summary>
    /// <param name="damage">Amount of damage to deal</param>
    /// <param name="target">Target Pokémon (defaults to event target)</param>
    /// <param name="source">Source Pokémon causing the damage (defaults to event source)</param>
    /// <param name="effect">Effect causing the damage (defaults to current effect)</param>
    /// <param name="instafaint">If true, immediately processes fainting instead of queueing it</param>
    /// <returns>
    /// - The actual damage dealt (as int) if successful
    /// - 0 if target has no HP
    /// - false if target is not active
    /// - null if damage was prevented by an event
    /// </returns>
    public IntFalseUndefinedUnion Damage(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleDamageEffect? effect = null, bool instafaint = false)
    {
        // Default target to event target if available
        if (target == null && Event.Target is PokemonSingleEventTarget eventTarget)
        {
            target = eventTarget.Pokemon;
        }

        // Default source to event source if available
        if (source == null && Event.Source is PokemonSingleEventSource eventSource)
        {
            source = eventSource.Pokemon;
        }

        // Default effect to current effect if available
        effect ??= BattleDamageEffect.FromIEffect(Effect);

        // Create single-element arrays for SpreadDamage
        var damageArray = new SpreadMoveDamage { damage };
        var targetArray = new SpreadMoveTargets
        {
            target ?? throw new InvalidOperationException(),
        };

        // Call SpreadDamage and return the first (only) result
        SpreadMoveDamage results = SpreadDamage(damageArray, targetArray, source, effect, instafaint);
        return results[0].ToIntFalseUndefinedUnion();
    }

    public SpreadMoveDamage SpreadDamage(SpreadMoveDamage damage, SpreadMoveTargets? targetArray = null,
        Pokemon? source = null, BattleDamageEffect? effect = null, bool instaFaint = false)
    {
        // Return early if no targets
        if (targetArray == null) return [0];

        var retVals = new SpreadMoveDamage();

        // Convert effect to Condition
        Condition? effectCondition;
        switch (effect)
        {
            case EffectBattleDamageEffect ebde:
                effectCondition = ebde.Effect as Condition;
                break;
            case DrainBattleDamageEffect:
                effectCondition = Library.Conditions[ConditionId.Drain];
                break;
            case RecoilBattleDamageEffect:
                effectCondition = Library.Conditions[ConditionId.Recoil];
                break;
            case null:
                effectCondition = null;
                break;
            default:
                throw new InvalidOperationException("Unknown BattleDamageEffect type.");
        }

        // Process each target
        for (int i = 0; i < damage.Count; i++)
        {
            SpreadMoveDamage curDamage = new([damage[i]]);

            // Extract Pokemon from union type
            Pokemon? target = targetArray[i] switch
            {
                PokemonPokemonUnion p => p.Pokemon,
                _ => null,
            };

            int targetDamage = curDamage[0].ToInt();

            // Handle undefined damage values
            if (curDamage[0] is UndefinedBoolIntUndefinedUnion)
            {
                retVals.Add(curDamage[0]);
                continue;
            }

            // Target has no HP - return 0
            if (target is not { Hp: > 0 })
            {
                retVals.Add(0);
                continue;
            }

            // Target is not active - return false
            if (!target.IsActive)
            {
                retVals.Add(false);
                continue;
            }

            // Clamp damage to minimum of 1 (if non-zero)
            if (targetDamage != 0)
                targetDamage = ClampIntRange(targetDamage, 1, null);

            // Run Damage event unless this is struggle recoil
            if (effectCondition?.Id != ConditionId.StruggleRecoil)
            {
                // Check weather immunity
                if (effectCondition?.EffectType == EffectType.Weather &&
                    !target.RunStatusImmunity(effectCondition.Id))
                {
                    Debug("weather immunity");
                    retVals.Add(0);
                    continue;
                }

                // Run Damage event
                RelayVar? damageResult = RunEvent(
                    EventId.Damage,
                    target,
                    RunEventSource.FromNullablePokemon(source),
                    effectCondition,
                    new IntRelayVar(targetDamage)
                );

                if (damageResult is not IntRelayVar damageInt)
                {
                    Debug("damage event failed");
                    retVals.Add(new Undefined());
                    continue;
                }

                targetDamage = damageInt.Value;
            }

            // Clamp damage again after events
            if (targetDamage != 0)
                targetDamage = ClampIntRange(targetDamage, 1, null);

            // Apply damage to target
            targetDamage = target.Damage(targetDamage, source, effectCondition);
            retVals.Add(targetDamage);

            // Track that the Pokemon was hurt this turn
            if (targetDamage != 0)
                target.HurtThisTurn = target.Hp;

            // Track source's last damage if this was a move
            if (source != null && effectCondition?.EffectType == EffectType.Move)
                source.LastDamage = targetDamage;

            // Log damage messages with the actual damage amount
            PrintDamageMessage(target, targetDamage, source, effectCondition);

            // Handle drain for moves (Gen 9 uses rounding)
            if (effect is EffectBattleDamageEffect { Effect: ActiveMove move })
            {
                if (targetDamage > 0 && effectCondition?.EffectType == EffectType.Move &&
                    move.Drain != null && source != null)
                {
                    int drainAmount = Trunc(Math.Round(targetDamage * move.Drain.Value.Item1 /
                                                       (double)move.Drain.Value.Item2));
                    Heal(drainAmount, source, target, new DrainBattleHealEffect());
                }
            }
        }

        // Handle instafaint if requested
        if (instaFaint)
        {
            for (int i = 0; i < targetArray.Count; i++)
            {
                if (retVals[i] is UndefinedBoolIntUndefinedUnion || retVals[i] == false)
                    continue;

                Pokemon? target = targetArray[i] switch
                {
                    PokemonPokemonUnion p => p.Pokemon,
                    _ => null,
                };

                if (!(target?.Hp <= 0)) continue;

                Debug($"instafaint: {string.Join(", ", FaintQueue.Select(entry => entry.Target.Name))}");
                FaintMessages(lastFirst: true);
            }
        }

        return retVals;
    }

    /// <summary>
    /// Applies damage directly to a Pokémon without triggering the Damage event.
    /// Used for recoil damage, struggle damage, confusion damage, and other effects
    /// that should bypass normal damage modification abilities/items.
    /// </summary>
    /// <param name="damage">Amount of damage to deal</param>
    /// <param name="target">Target Pokémon (defaults to event target)</param>
    /// <param name="source">Source Pokémon causing the damage (defaults to event source)</param>
    /// <param name="effect">Effect causing the damage (defaults to current effect)</param>
    /// <returns>The actual amount of damage dealt (0 if target has no HP or damage was 0)</returns>
    public int DirectDamage(int damage, Pokemon? target = null, Pokemon? source = null, IEffect? effect = null)
    {
        // Default target to event target if available
        if (target == null && Event.Target is PokemonSingleEventTarget eventTarget)
        {
            target = eventTarget.Pokemon;
        }

        // Default source to event source if available
        if (source == null && Event.Source is PokemonSingleEventSource eventSource)
        {
            source = eventSource.Pokemon;
        }

        // Default effect to current effect if available
        effect ??= Effect;

        // Return 0 if target has no HP
        if (target?.Hp <= 0) return 0;

        // Return 0 if no damage to deal
        if (damage == 0) return 0;

        // Clamp damage to minimum of 1
        damage = ClampIntRange(damage, 1, null);

        // Apply damage directly (bypasses Damage event)
        if (target == null) return damage;

        damage = target.Damage(damage, source, effect);

        // Log the damage with special cases for certain effects
        if (DisplayUi)
        {
            if (effect is Condition condition)
            {
                switch (condition.Id)
                {
                    case ConditionId.StruggleRecoil:
                        // Struggle recoil has special messaging
                        Add("-damage", target, target.GetHealth, "[from] recoil");
                        break;

                    case ConditionId.Confusion:
                        // Confusion damage has special messaging
                        Add("-damage", target, target.GetHealth, "[from] confusion");
                        break;

                    default:
                        // Regular direct damage message
                        Add("-damage", target, target.GetHealth);
                        break;
                }
            }
            else
            {
                // No effect or non-condition effect - simple damage message
                Add("-damage", target, target.GetHealth);
            }
        }

        // If target fainted from the damage, trigger faint immediately
        if (target.Fainted)
        {
            Faint(target);
        }

        return damage;
    }

    public IntFalseUnion Heal(int damage, Pokemon? target = null, Pokemon? source = null,
        BattleHealEffect? effect = null)
    {
        // Default target to event target if available
        if (target == null && Event.Target is PokemonSingleEventTarget eventTarget)
        {
            target = eventTarget.Pokemon;
        }

        // Default source to event source if available
        if (source == null && Event.Source is PokemonSingleEventSource eventSource)
        {
            source = eventSource.Pokemon;
        }

        // Convert BattleHealEffect to Condition
        Condition? effectCondition = effect switch
        {
            DrainBattleHealEffect => Library.Conditions[ConditionId.Drain],
            EffectBattleHealEffect ebhe => ebhe.Effect as Condition,
            null => Effect as Condition,
            _ => throw new InvalidOperationException("Unknown BattleHealEffect type."),
        };

        // Clamp damage to minimum of 1 if non-zero
        if (damage != 0 && damage <= 1)
        {
            damage = 1;
        }

        // Truncate damage
        damage = Trunc(damage);

        // Run TryHeal event (allows effects like Liquid Ooze to trigger even when nothing is healed)
        RelayVar? tryHealResult = RunEvent(
            EventId.TryHeal,
            RunEventTarget.FromNullablePokemon(target),
            RunEventSource.FromNullablePokemon(source),
            effectCondition,
            new IntRelayVar(damage)
        );

        // If event prevented healing, return the result
        if (tryHealResult is not IntRelayVar healAmount)
        {
            return new IntIntFalseUnion(0);
        }

        if (healAmount.Value == 0)
        {
            return new IntIntFalseUnion(0);
        }

        damage = healAmount.Value;

        // Return false if target has no HP
        if (target?.Hp <= 0)
        {
            return new FalseIntFalseUnion();
        }

        // Return false if target is not active
        if (target is { IsActive: false })
        {
            return new FalseIntFalseUnion();
        }

        // Return false if target is already at max HP
        if (target != null && target.Hp >= target.MaxHp)
        {
            return new FalseIntFalseUnion();
        }

        if (target is null)
        {
            throw new InvalidOperationException("Target Pokémon is null.");
        }

        // Apply healing to target
        int finalDamage = target.Heal(damage, source, effectCondition).ToInt();

        // Log healing messages based on effect type
        PrintHealMessage(target, source, effectCondition);

        // Run Heal event
        RunEvent(
            EventId.Heal,
            target,
            RunEventSource.FromNullablePokemon(source),
            effectCondition,
            new IntRelayVar(finalDamage)
        );

        return new IntIntFalseUnion(finalDamage);
    }

    /// <summary>
    /// Modifies a Pokémon's stat stages (boosts) during battle.
    /// 
    /// Process:
    /// 1. Validates the target has HP and is active
    /// 2. Runs ChangeBoost and TryBoost events for modification
    /// 3. Applies boosts via target.BoostBy() for each stat
    /// 4. Logs boost messages to battle log
    /// 5. Triggers AfterEachBoost and AfterBoost events
    /// 6. Updates statsRaisedThisTurn/statsLoweredThisTurn flags
    /// 
    /// Returns:
    /// - null if boost succeeded
    /// - 0 if target has no HP
    /// - false if target is inactive or no foes remain (Gen 9)
    /// </summary>
    public BoolZeroUnion? Boost(SparseBoostsTable boost, Pokemon? target = null, Pokemon? source = null,
        IEffect? effect = null, bool isSecondary = false, bool isSelf = false)
    {
        if (target is null && Event.Target is PokemonSingleEventTarget eventTarget)
        {
            target = eventTarget.Pokemon;
        }
        if (source is null && Event.Source is PokemonSingleEventSource eventSource)
        {
            source = eventSource.Pokemon;
        }
        effect ??= Event.Effect;

        // Validate target has HP
        if (target?.Hp <= 0) return new ZeroBoolZeroUnion();

        // Validate target is active
        if (!(target?.IsActive ?? false)) return new BoolBoolZeroUnion(false);

        // Gen 9: Check if any foes remain
        if (target.Side.FoePokemonLeft() <= 0) return new BoolBoolZeroUnion(false);

        // Run ChangeBoost event to allow modifications
        RelayVar modifiedBoost = RunEvent(EventId.ChangeBoost, target,
            RunEventSource.FromNullablePokemon(source), effect, boost) ?? boost;

        if (modifiedBoost is not SparseBoostsTableRelayVar modifiedBoostTable)
        {
            throw new InvalidOperationException("ChangeBoost event did not return a valid SparseBoostsTable.");
        }

        // Cap the boosts to valid ranges (-6 to +6)
        SparseBoostsTable cappedBoost = target.GetCappedBoost(modifiedBoostTable.Table);

        // Run TryBoost event to allow prevention
        RelayVar finalBoost = RunEvent(EventId.TryBoost, target, RunEventSource.FromNullablePokemon(source),
                                   effect, cappedBoost) ?? cappedBoost;

        if (finalBoost is not SparseBoostsTableRelayVar finalBoostTable)
        {
            throw new InvalidOperationException("TryBoost event did not return a valid SparseBoostsTable.");
        }

        bool? success = null;
        bool boosted = isSecondary;

        // Apply each boost
        foreach (BoostId boostId in Enum.GetValues<BoostId>())
        {
            int? boostValue = finalBoostTable.Table.GetBoost(boostId);
            if (!boostValue.HasValue) continue;

            // Create a sparse table for just this stat
            var currentBoost = new SparseBoostsTable();
            currentBoost.SetBoost(boostId, boostValue.Value);

            // Apply the boost and get the actual change
            int boostBy = target.BoostBy(currentBoost);

            // Determine message type
            string msg = "-boost";
            if (boostValue.Value < 0 || target.Boosts.GetBoost(boostId) == -6)
            {
                msg = "-unboost";
                boostBy = -boostBy;
            }

            if (boostBy != 0)
            {
                success = true;

                // Handle special cases
                EffectStateId effectId = effect?.EffectStateId ?? EffectStateId.FromEmpty();

                if (DisplayUi)
                {
                    if (effectId is MoveEffectStateId { MoveId: MoveId.BellyDrum } or
                        AbilityEffectStateId { AbilityId: AbilityId.AngerPoint })
                    {
                        // Use -setboost for moves that set boosts to maximum
                        Add("-setboost", target, boostId.ConvertToString(),
                            target.Boosts.GetBoost(boostId),
                            "[from]", PartFuncUnion.FromIEffect(effect!));
                    }
                    else if (effect is not null)
                    {
                        switch (effect.EffectType)
                        {
                            case EffectType.Move:
                                Add(msg, target, boostId.ConvertToString(), boostBy);
                                break;

                            case EffectType.Item:
                                Add(msg, target, boostId.ConvertToString(), boostBy,
                                    "[from]", $"item: {effect.Name}");
                                break;

                            default:
                                if (effect.EffectType == EffectType.Ability && !boosted)
                                {
                                    Add("-ability", target, PartFuncUnion.FromIEffect(effect), "boost");
                                    boosted = true;
                                }
                                Add(msg, target, boostId.ConvertToString(), boostBy);
                                break;
                        }
                    }
                }

                // Trigger AfterEachBoost event
                RunEvent(EventId.AfterEachBoost, target, RunEventSource.FromNullablePokemon(source), effect,
                    currentBoost);
            }
            else if (effect?.EffectType == EffectType.Ability)
            {
                if (DisplayUi && (isSecondary || isSelf))
                {
                    Add(msg, target, boostId.ConvertToString(), boostBy);
                }
            }
            else if (!isSecondary && !isSelf)
            {
                if (DisplayUi)
                {
                    Add(msg, target, boostId.ConvertToString(), boostBy);
                }
            }
        }

        // Trigger AfterBoost event
        RunEvent(EventId.AfterBoost, target, RunEventSource.FromNullablePokemon(source), effect, finalBoost);

        // Update turn flags
        if (success == true)
        {
            // Check if any boosts were positive or negative
            bool hasPositiveBoost = false;
            bool hasNegativeBoost = false;

            foreach (BoostId boostId in Enum.GetValues<BoostId>())
            {
                int? boostValue = finalBoostTable.Table.GetBoost(boostId);
                switch (boostValue)
                {
                    case null:
                        continue;
                    case > 0:
                        hasPositiveBoost = true;
                        break;
                    case < 0:
                        hasNegativeBoost = true;
                        break;
                }
            }

            if (hasPositiveBoost) target.StatsRaisedThisTurn = true;
            if (hasNegativeBoost) target.StatsLoweredThisTurn = true;
        }

        return success.HasValue ? new BoolBoolZeroUnion(success.Value) : null;
    }

    public bool CheckMoveMakesContact(Move move, Pokemon attacker, Pokemon defender, bool announcePads = false)
    {
        if (move.Flags.Contact is not true || !attacker.HasItem(ItemId.ProtectivePads))
        {
            return move.Flags.Contact is true;
        }

        if (!announcePads) return false;

        if (DisplayUi)
        {
            Add("-activate", defender, PartFuncUnion.FromIEffect(Effect));
            Add("-activate", attacker, "item: Protective Pads");
        }

        return false;
    }
}