using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    public SpreadMoveDamage RunMoveEffects(SpreadMoveDamage damage, SpreadMoveTargets targets,
        Pokemon source, ActiveMove move, ActiveMove moveData, bool isSecondary = false, bool isSelf = false)
    {
        BoolIntUndefinedUnion? didAnything = damage.Aggregate<BoolIntUndefinedUnion?, BoolIntUndefinedUnion?>(
            null, CombineResults);

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] is not PokemonPokemonUnion pokemonUnion)
            {
                continue;
            }

            Pokemon target = pokemonUnion.Pokemon;
            BoolIntUndefinedUnion? hitResult;
            BoolIntUndefinedUnion didSomething = BoolIntUndefinedUnion.FromUndefined();

            // Apply boosts
            // Check both moveData.HitEffect?.Boosts and move.HitEffect?.Boosts (for secondary effects)
            SparseBoostsTable? boostsToApply = moveData.HitEffect?.Boosts ?? (move.HitEffect as HitEffect)?.Boosts;
            if (boostsToApply != null && !target.Fainted)
            {
                BoolZeroUnion? boostResult = Battle.Boost(boostsToApply, target, source, move, isSecondary, isSelf);
                hitResult = boostResult switch
                {
                    BoolBoolZeroUnion bbz => bbz.Value,
                    ZeroBoolZeroUnion => 0,
                    null => null,
                    _ => throw new InvalidOperationException("Unexpected boost result type.")
                };
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Apply healing
            if (moveData.Heal != null && !target.Fainted)
            {
                if (target.Hp >= target.MaxHp)
                {
                    if (Battle.DisplayUi)
                    {
                        Battle.Add("-fail", target, "heal");
                        Battle.AttrLastMove("[still]");
                    }
                    damage[i] = CombineResults(damage[i], BoolIntUndefinedUnion.FromBool(false));
                    didAnything = CombineResults(didAnything, null);
                    continue;
                }

                int amount = target.BaseMaxHp * moveData.Heal[0] / moveData.Heal[1];
                int roundedAmount = Battle.Gen < 5 ? (int)Math.Floor((double)amount) : (int)Math.Round((double)amount);

                IntFalseUnion? healResult = Battle.Heal(roundedAmount, target, source,
                    BattleHealEffect.FromIEffect(move));

                if (healResult is not IntIntFalseUnion intHeal || (intHeal.Value == 0 && healResult is FalseIntFalseUnion))
                {
                    if (healResult is not null)
                    {
                        if (Battle.DisplayUi)
                        {
                            Battle.Add("-fail", source);
                            Battle.AttrLastMove("[still]");
                        }
                    }
                    Battle.Debug("heal interrupted");
                    damage[i] = CombineResults(damage[i], BoolIntUndefinedUnion.FromBool(false));
                    didAnything = CombineResults(didAnything, null);
                    continue;
                }

                didSomething = BoolIntUndefinedUnion.FromBool(true);
            }

            // Try to apply status
            // Check both moveData.Status and move.HitEffect?.Status (for secondary effects)
            ConditionId? statusToApply = moveData.Status ?? (move.HitEffect as HitEffect)?.Status;
            if (statusToApply != null)
            {
                bool statusResult = target.TrySetStatus(statusToApply.Value, source,
                    moveData.Ability != null ? Library.Abilities[moveData.Ability.Id] : move);
                hitResult = BoolIntUndefinedUnion.FromBool(statusResult);

                if (!statusResult && (move.Status != null || (move.HitEffect as HitEffect)?.Status != null))
                {
                    damage[i] = CombineResults(damage[i], BoolIntUndefinedUnion.FromBool(false));
                    didAnything = CombineResults(didAnything, null);
                    continue;
                }

                didSomething = CombineResults(didSomething, hitResult);
            }

            // Force status (bypasses immunity)
            if (moveData.ForceStatus != null)
            {
                bool forceStatusResult = target.SetStatus(moveData.ForceStatus.Value, source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(forceStatusResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Apply volatile status
            // Check both moveData.VolatileStatus and move.HitEffect?.VolatileStatus (for secondary effects)
            ConditionId? volatileToApply = moveData.VolatileStatus ?? (move.HitEffect as HitEffect)?.VolatileStatus;
            if (volatileToApply != null)
            {
                Battle.Debug($"[RunMoveEffects] Attempting to apply volatile status {volatileToApply.Value} to {target.Name}");
                RelayVar volatileResult = target.AddVolatile(volatileToApply.Value, source, move);
                Battle.Debug($"[RunMoveEffects] AddVolatile returned: {volatileResult?.GetType().Name ?? "null"}");
                if (volatileResult is BoolRelayVar brvCheck)
                {
                    Battle.Debug($"[RunMoveEffects] Volatile application result: {brvCheck.Value}");
                }
                hitResult = volatileResult switch
                {
                    BoolRelayVar brv => BoolIntUndefinedUnion.FromBool(brv.Value),
                    IntRelayVar irv => BoolIntUndefinedUnion.FromInt(irv.Value),
                    _ => BoolIntUndefinedUnion.FromUndefined()
                };
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Apply side condition
            if (moveData.SideCondition != null)
            {
                bool sideCondResult = target.Side.AddSideCondition(
                    Library.Conditions[moveData.SideCondition.Value], source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(sideCondResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Apply slot condition
            if (moveData.HitEffect?.SlotCondition != null)
            {
                bool slotCondResult = target.Side.AddSlotCondition(target,
                    Library.Conditions[moveData.HitEffect.SlotCondition.Value], source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(slotCondResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Set weather
            if (moveData.Weather != null)
            {
                bool weatherResult = Battle.Field.SetWeather(
                    Library.Conditions[moveData.Weather.Value], source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(weatherResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Set terrain
            if (moveData.HitEffect?.Terrain != null)
            {
                bool terrainResult = Battle.Field.SetTerrain(
                    Library.Conditions[moveData.HitEffect.Terrain.Value], source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(terrainResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Add pseudo weather
            if (moveData.PseudoWeather != null)
            {
                bool pseudoWeatherResult = Battle.Field.AddPseudoWeather(
                    Library.Conditions[moveData.PseudoWeather.Value], source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(pseudoWeatherResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Check force switch
            if (moveData.ForceSwitch != null)
            {
                int canSwitchResult = Battle.CanSwitch(target.Side);
                hitResult = canSwitchResult;
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Hit events
            // These are like the TryHit events, except we don't need a FieldHit event.
            if (move.Target == MoveTarget.All && !isSelf)
            {
                if (moveData.OnHitField != null)
                {
                    RelayVar? fieldHitResult = Battle.SingleEvent(EventId.HitField, moveData, null,
                        target, source, move);
                    hitResult = fieldHitResult switch
                    {
                        BoolRelayVar brv => BoolIntUndefinedUnion.FromBool(brv.Value),
                        IntRelayVar irv => BoolIntUndefinedUnion.FromInt(irv.Value),
                        NullRelayVar => null,
                        _ => BoolIntUndefinedUnion.FromUndefined(),
                    };
                    didSomething = CombineResults(didSomething, hitResult);
                }
            }
            else if (move.Target is MoveTarget.FoeSide or MoveTarget.AllySide && !isSelf)
            {
                if (moveData.OnHitSide != null)
                {
                    RelayVar? sideHitResult = Battle.SingleEvent(EventId.HitSide, moveData, null,
                        target.Side, source, move);
                    hitResult = sideHitResult switch
                    {
                        BoolRelayVar brv => BoolIntUndefinedUnion.FromBool(brv.Value),
                        IntRelayVar irv => BoolIntUndefinedUnion.FromInt(irv.Value),
                        NullRelayVar => null,
                        _ => BoolIntUndefinedUnion.FromUndefined(),
                    };
                    didSomething = CombineResults(didSomething, hitResult);
                }
            }
            else
            {
                if (moveData.OnHit != null)
                {
                    RelayVar? hitEventResult = Battle.SingleEvent(EventId.Hit, moveData, null,
                        target, source, move);
                    hitResult = hitEventResult switch
                    {
                        BoolRelayVar brv => BoolIntUndefinedUnion.FromBool(brv.Value),
                        IntRelayVar irv => BoolIntUndefinedUnion.FromInt(irv.Value),
                        NullRelayVar => null,
                        _ => BoolIntUndefinedUnion.FromUndefined()
                    };
                    didSomething = CombineResults(didSomething, hitResult);
                }

                if (!isSelf && !isSecondary)
                {
                    Battle.RunEvent(EventId.Hit, target, source, move);
                }
            }

            // Handle self-destruct on hit
            if (moveData.SelfDestruct is IfHitMoveSelfDestruct && damage[i] is not BoolBoolIntUndefinedUnion { Value: false })
            {
                Battle.Faint(source, source, move);
            }

            // Move didn't fail because it didn't try to do anything
            if (didSomething is UndefinedBoolIntUndefinedUnion)
            {
                didSomething = BoolIntUndefinedUnion.FromBool(true);
            }

            // Only combine didSomething into damage if damage isn't already an integer (actual damage dealt)
            // If damage was dealt, preserve that integer value instead of replacing it with a boolean
            if (damage[i] is not IntBoolIntUndefinedUnion)
            {
                damage[i] = CombineResults(damage[i], didSomething);
                didAnything = CombineResults(didAnything, didSomething);
            }
        }

        // Check if move succeeded
        // For moves with damage: non-zero integer = success, zero = immunity/failure
        // For moves without damage: any truthy value = success
        // Undefined (NOT_FAIL) means move succeeded but did nothing measurable (e.g., Protect)
      bool moveSucceeded = didAnything switch
        {
            IntBoolIntUndefinedUnion intResult => intResult.Value > 0,  // Only non-zero damage counts as success
            BoolBoolIntUndefinedUnion boolResult => boolResult.Value,   // Boolean true = success
            UndefinedBoolIntUndefinedUnion => true,  // Undefined means NOT_FAIL - move succeeded
            _ => false  // null or false = failure
        };

        // Check if move failed completely
        if (!moveSucceeded && moveData.Self == null && moveData.SelfDestruct == null)
        {
            if (!isSelf && !isSecondary)
            {
     if (didAnything is BoolBoolIntUndefinedUnion { Value: false })
                {
     if (Battle.DisplayUi)
      {
      Battle.Add("-fail", source);
            Battle.AttrLastMove("[still]");
      }
}
            }

        Battle.Debug("move failed because it did nothing");
        }
        else if (moveSucceeded && move.SelfSwitch != null && source.Hp > 0 && !source.Volatiles.ContainsKey(ConditionId.Commanded))
        {
       source.SwitchFlag = move.Id;
        }

        return damage;
    }

    public void SelfDrops(SpreadMoveTargets targets, Pokemon source, ActiveMove move, ActiveMove moveData,
        bool isSecondary = false)
    {
        Battle.Debug($"[SelfDrops] Called for {move.Name}, isSecondary={isSecondary}, moveData.Self != null: {moveData.Self != null}, move.SelfDropped: {move.SelfDropped}");
        
        foreach (PokemonFalseUnion targetUnion in targets)
        {
            if (targetUnion is not PokemonPokemonUnion)
            {
                continue;
            }

            if (moveData.Self != null && move.SelfDropped != true)
            {
                Battle.Debug($"[SelfDrops] Processing self effect for {source.Name}");
                
                if (!isSecondary && moveData.Self.Boosts != null)
                {
                    Battle.Debug($"[SelfDrops] Branch 1: !isSecondary && moveData.Self.Boosts != null");
                    int secondaryRoll = Battle.Random(100);
                    Battle.Debug($"[SelfDrops] secondaryRoll={secondaryRoll}, Chance={moveData.Self.Chance}");
                    
                    if (moveData.Self.Chance == null || secondaryRoll < moveData.Self.Chance)
                    {
                        Battle.Debug($"[SelfDrops] Calling MoveHit for self boosts");
                        // isSelf=true prevents damage calculation (matching TypeScript behavior)
                        MoveHit(source, source, move, moveData.Self, isSecondary, true);
                    }
                    else
                    {
                        Battle.Debug($"[SelfDrops] Chance check failed, skipping self boosts");
                    }
                    
                    if (move.MultiHit == null)
                    {
                        move.SelfDropped = true;
                    }
                }
                else
                {
                    Battle.Debug($"[SelfDrops] Branch 2: else branch (isSecondary={isSecondary} or Boosts=null)");
                    // isSelf=true prevents damage calculation (matching TypeScript behavior)
                    MoveHit(source, source, move, moveData.Self, isSecondary, true);
                }
            }
            else
            {
                if (moveData.Self == null)
                {
                    Battle.Debug($"[SelfDrops] moveData.Self is null, skipping");
                }
                if (move.SelfDropped == true)
                {
                    Battle.Debug($"[SelfDrops] move.SelfDropped is true, skipping");
                }
            }
        }
    }

    public void Secondaries(SpreadMoveTargets targets, Pokemon source, ActiveMove move, ActiveMove moveData,
        bool isSelf = false)
    {
        if (moveData.Secondaries == null) return;

        Battle.Debug($"[Secondaries] Called for move {move.Name}, Secondaries count={moveData.Secondaries.Length}");

        foreach (PokemonFalseUnion targetUnion in targets)
        {
            if (targetUnion is not PokemonPokemonUnion pokemonUnion)
            {
                continue;
            }

            Pokemon target = pokemonUnion.Pokemon;

            Battle.Debug($"[Secondaries] Processing secondary effects for target {target.Name}");

            // Run ModifySecondaries event to get the list of secondary effects
            RelayVar? modifyResult = Battle.RunEvent(EventId.ModifySecondaries, target, source, moveData,
                moveData.Secondaries);

            var secondaries = modifyResult is SecondaryEffectArrayRelayVar secListRv
                ? secListRv.Effects
                : moveData.Secondaries;

            foreach (SecondaryEffect secondary in secondaries)
            {
                Battle.Debug($"[Secondaries] Secondary effect: VolatileStatus={secondary.VolatileStatus}, Chance={secondary.Chance}");
                
                int secondaryRoll = Battle.Random(100);

                // User stat boosts or target stat drops can possibly overflow if it goes beyond 256 in Gen 8 or prior
                bool secondaryOverflow = (secondary.Boosts != null || secondary.Self != null) && Battle.Gen <= 8;

                int effectiveChance = secondary.Chance ?? 100;
                if (secondaryOverflow)
                {
                    effectiveChance %= 256;
                }

                Battle.Debug($"[Secondaries] secondaryRoll={secondaryRoll}, effectiveChance={effectiveChance}");

                if (secondary.Chance == null || secondaryRoll < effectiveChance)
                {
                    Battle.Debug($"[Secondaries] Applying secondary effect, calling MoveHit");
                    MoveHit(target, source, move, secondary, true, isSelf);
                }
                else
                {
                    Battle.Debug($"[Secondaries] Secondary effect chance failed");
                }
            }
        }
    }

    public SpreadMoveDamage ForceSwitch(SpreadMoveDamage damage, SpreadMoveTargets targets, Pokemon source,
        ActiveMove move)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] is not PokemonPokemonUnion pokemonUnion)
            {
                continue;
            }

            Pokemon target = pokemonUnion.Pokemon;

            if (target.Hp > 0 && source.Hp > 0 && Battle.CanSwitch(target.Side) != 0)
            {
                // Pass default relayVar of true so "no handler" = "allow drag-out"
                RelayVar? hitResult = Battle.RunEvent(EventId.DragOut, target, source, move,
                    BoolRelayVar.True);

                switch (hitResult)
                {
                    case BoolRelayVar { Value: true }:
                        // Allow drag-out
                        target.ForceSwitchFlag = true;
                        break;
                    case null:
                        // null = prevent drag-out silently (e.g., Suction Cups)
                        break;
                    case BoolRelayVar { Value: false } when move.Category == MoveCategory.Status:
                        // false on status move = show fail message
                        if (Battle.DisplayUi)
                        {
                            Battle.Add("-fail", source);
                            Battle.AttrLastMove("[still]");
                        }
                        damage[i] = BoolIntUndefinedUnion.FromBool(false);
                        break;
                    case BoolRelayVar { Value: false }:
                        // false on non-status move = prevent silently
                        break;
                }
            }
        }

        return damage;
    }
}