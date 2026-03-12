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
            // Showdown: if (target === false) continue;
            // Only skip false targets. Null targets (Substitute absorbed) must still be
            // processed for selfSwitch, selfDestruct, and didAnything/damage combining.
            if (targets[i] is FalsePokemonUnion)
            {
                continue;
            }

            // Extract target — null for Substitute hits (NullPokemonUnion)
            Pokemon? target = targets[i] is PokemonPokemonUnion pokemonUnion
                ? pokemonUnion.Pokemon
                : null;
            BoolIntUndefinedUnion? hitResult;
            BoolIntUndefinedUnion didSomething = BoolIntUndefinedUnion.FromUndefined();

            // All target-dependent effects are guarded by target != null
            if (target != null)
            {
                // Apply boosts
                SparseBoostsTable? boostsToApply = moveData.Boosts ?? moveData.HitEffect?.Boosts ?? move.Boosts;
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
                if (moveData.Heal != null && !isSelf && !target.Fainted)
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

                    double amount = (double)(target.BaseMaxHp * moveData.Heal[0]) / moveData.Heal[1];
                    int roundedAmount = Battle.Gen < 5 ? (int)Math.Floor(amount) : (int)Math.Round(amount, MidpointRounding.AwayFromZero);

                    IntFalseUnion? healResult = Battle.Heal(roundedAmount, target, source,
                        BattleHealEffect.FromIEffect(move));

                    if (healResult is not { IsInt: true } intHeal || (intHeal.Value == 0 && healResult is { IsFalse: true }))
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
                if (moveData.ForceStatus != null && !isSelf)
                {
                    bool forceStatusResult = target.SetStatus(moveData.ForceStatus.Value, source, move);
                    hitResult = BoolIntUndefinedUnion.FromBool(forceStatusResult);
                    didSomething = CombineResults(didSomething, hitResult);
                }

                // Apply volatile status
                ConditionId? volatileToApply = moveData.VolatileStatus ?? (move.HitEffect as HitEffect)?.VolatileStatus;
                if (volatileToApply != null)
                {
                    if (Battle.DebugMode) Battle.Debug($"[RunMoveEffects] Attempting to apply volatile status {volatileToApply.Value} to {target.Name}");
                    RelayVar volatileResult = target.AddVolatile(volatileToApply.Value, source, move);
                    if (Battle.DebugMode) Battle.Debug($"[RunMoveEffects] AddVolatile returned: {volatileResult?.GetType().Name ?? "null"}");
                    if (volatileResult is BoolRelayVar brvCheck)
                    {
                        if (Battle.DebugMode) Battle.Debug($"[RunMoveEffects] Volatile application result: {brvCheck.Value}");
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
                ConditionId? slotCondToApply = moveData.SlotCondition ?? moveData.HitEffect?.SlotCondition;
                if (slotCondToApply != null)
                {
                    bool slotCondResult = target.Side.AddSlotCondition(target,
                        Library.Conditions[slotCondToApply.Value], source, move);
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
                ConditionId? terrainToApply = moveData.Terrain ?? moveData.HitEffect?.Terrain;
                if (terrainToApply != null)
                {
                    bool terrainResult = Battle.Field.SetTerrain(
                        Library.Conditions[terrainToApply.Value], source, move);
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
                    bool canSwitchResult = Battle.CanSwitch(target.Side) != 0;
                    hitResult = BoolIntUndefinedUnion.FromBool(canSwitchResult);
                    didSomething = CombineResults(didSomething, hitResult);
                }

                // Hit events
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
                    if (isSecondary || isSelf)
                    {
                        var hitEffectOnHit = (move.HitEffect as HitEffect)?.OnHit;
                        if (hitEffectOnHit != null)
                        {
                            var result = hitEffectOnHit(Battle, target, source, move);
                            hitResult = result switch
                            {
                                BoolBoolEmptyVoidUnion b => BoolIntUndefinedUnion.FromBool(b.Value),
                                EmptyBoolEmptyVoidUnion => null,
                                VoidUnionBoolEmptyVoidUnion => BoolIntUndefinedUnion.FromUndefined(),
                                null => BoolIntUndefinedUnion.FromUndefined(),
                                _ => BoolIntUndefinedUnion.FromUndefined(),
                            };
                            didSomething = CombineResults(didSomething, hitResult);
                        }
                    }
                    else if (moveData.OnHit != null)
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
            }

            // --- Non-target-dependent effects (run for both Pokemon and null/Substitute targets) ---

            // Handle self-destruct on hit
            if (moveData.SelfDestruct is IfHitMoveSelfDestruct && damage[i] is not BoolBoolIntUndefinedUnion { Value: false })
            {
                Battle.Faint(source, source, move);
            }

            // Showdown lines 1307-1313: selfSwitch sets didSomething = true inside the
            // per-target loop (before combining into didAnything). This ensures that moves
            // like U-turn still count as "succeeded" even when damage is 0 (e.g. Disguise)
            // or when the hit was absorbed by Substitute (null target).
            if (moveData.SelfSwitch != null)
            {
                if (Battle.CanSwitch(source.Side) != 0 && !source.Volatiles.ContainsKey(ConditionId.Commanded))
                {
                    didSomething = BoolIntUndefinedUnion.FromBool(true);
                }
                else
                {
                    didSomething = CombineResults(didSomething, BoolIntUndefinedUnion.FromBool(false));
                }
            }

            // Move didn't fail because it didn't try to do anything
            if (didSomething is UndefinedBoolIntUndefinedUnion)
            {
                didSomething = BoolIntUndefinedUnion.FromBool(true);
            }

            // Only combine didSomething into damage if damage isn't already an integer (actual damage dealt)
            if (damage[i] is not IntBoolIntUndefinedUnion)
            {
                BoolIntUndefinedUnion effectiveDidSomething = didSomething is NullBoolIntUndefinedUnion
                    ? BoolIntUndefinedUnion.FromBool(false)
                    : didSomething;
                damage[i] = CombineResults(damage[i], effectiveDidSomething);
            }
            // didAnything always updated (matches Showdown line 1317), even when damage[i]
            // is an integer. This ensures selfSwitch's didSomething=true propagates to
            // moveSucceeded even when damage is 0 (e.g. U-turn vs Disguise).
            didAnything = CombineResults(didAnything, didSomething);
        }

        // Showdown post-loop logic (lines 1320-1330):
        //   if (!didAnything && didAnything !== 0 && !moveData.self && !moveData.selfdestruct) {
        //     if (!isSelf && !isSecondary) { if (didAnything === false) { add('-fail'); attrLastMove('[still]'); } }
        //     debug('move failed because it did nothing');
        //   } else if (move.selfSwitch && source.hp && !source.volatiles['commanded']) {
        //     source.switchFlag = move.id;
        //   }
        //
        // Key: !didAnything is true for 0, false, null, undefined.
        //      didAnything !== 0 is true for everything EXCEPT 0.
        //      Combined: fail only when didAnything is false, null, or undefined — NOT when 0.
        //      (undefined is handled by the "didn't try" check at line 1315, so effectively fail = false or null)
        bool didAnythingFalsy = didAnything switch
        {
            IntBoolIntUndefinedUnion i => i.Value == 0,  // !0 = true in JS
            BoolBoolIntUndefinedUnion b => !b.Value,     // !false = true, !true = false
            UndefinedBoolIntUndefinedUnion => true,       // !undefined = true
            NullBoolIntUndefinedUnion => true,            // !null = true
            null => true,
            _ => true
        };
        bool didAnythingNotZero = didAnything is not IntBoolIntUndefinedUnion { Value: 0 };
        bool moveFailed = didAnythingFalsy && didAnythingNotZero;

        if (moveFailed && moveData.Self == null && moveData.SelfDestruct == null)
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
        else if (move.SelfSwitch != null && source.Hp > 0 && !source.Volatiles.ContainsKey(ConditionId.Commanded))
        {
            source.SwitchFlag = move.Id;
        }

        return damage;
    }

    public void SelfDrops(SpreadMoveTargets targets, Pokemon source, ActiveMove move, ActiveMove moveData,
        bool isSecondary = false)
    {
        if (Battle.DebugMode) Battle.Debug($"[SelfDrops] Called for {move.Name}, isSecondary={isSecondary}, moveData.Self != null: {moveData.Self != null}, move.SelfDropped: {move.SelfDropped}");

        foreach (PokemonFalseUnion targetUnion in targets)
        {
            // Showdown: if (target === false) continue;
            // Only skip explicitly false targets. Null targets (from Substitute hits) still
            // allow self effects to fire (e.g., Close Combat's -def/-spd through Substitute).
            if (targetUnion is FalsePokemonUnion)
            {
                continue;
            }

            if (moveData.Self != null && move.SelfDropped != true)
            {
                if (Battle.DebugMode) Battle.Debug($"[SelfDrops] Processing self effect for {source.Name}");

                // Showdown: if (!move.multihit) move.selfDropped = true;
                // Set BEFORE MoveHit to prevent re-entry via recursive SpreadMoveHit→SelfDrops.
                // In Showdown, re-entry is prevented because inner spreadMoveHit uses moveData=SecondaryEffect
                // (which has no .self). In C# we always pass move, so we must guard via selfDropped.
                if (move.MultiHit == null)
                {
                    move.SelfDropped = true;
                }

                if (!isSecondary && moveData.Self.Boosts != null)
                {
                    int secondaryRoll = Battle.Random(100);
                    if (Battle.DebugMode) Battle.Debug($"[SelfDrops] secondaryRoll={secondaryRoll}, Chance={moveData.Self.Chance}");

                    if (moveData.Self.Chance == null || secondaryRoll < moveData.Self.Chance)
                    {
                        // isSelf=true prevents damage calculation (matching TypeScript behavior)
                        MoveHit(source, source, move, moveData.Self, isSecondary, true);
                    }
                }
                else
                {
                    // isSelf=true prevents damage calculation (matching TypeScript behavior)
                    MoveHit(source, source, move, moveData.Self, isSecondary, true);
                }
            }
        }
    }

    /// <summary>
    /// Processes the Self effect from a secondary effect (e.g., Rapid Spin's speed boost).
    /// In Showdown, moveData.self is checked when moveData is the secondary effect object.
    /// </summary>
    public void SelfDropsFromHitEffect(SpreadMoveTargets targets, Pokemon source, ActiveMove move,
        HitEffect self, bool isSecondary = false)
    {
        foreach (PokemonFalseUnion targetUnion in targets)
        {
            // Showdown: if (target === false) continue;
            if (targetUnion is FalsePokemonUnion) continue;

            if (move.SelfDropped == true) break;

            if (!isSecondary && self.Boosts != null)
            {
                int secondaryRoll = Battle.Random(100);
                var selfAsSecondary = self as SecondaryEffect;
                if (selfAsSecondary?.Chance == null || secondaryRoll < selfAsSecondary.Chance)
                {
                    MoveHit(source, source, move, self, isSecondary, true);
                }
                if (move.MultiHit == null) move.SelfDropped = true;
            }
            else
            {
                MoveHit(source, source, move, self, isSecondary, true);
            }
        }
    }

    public void Secondaries(SpreadMoveTargets targets, Pokemon source, ActiveMove move, ActiveMove moveData,
        bool isSelf = false)
    {
        if (moveData.Secondaries == null) return;

        if (Battle.DebugMode) Battle.Debug($"[Secondaries] Called for move {move.Name}, Secondaries count={moveData.Secondaries.Length}");

        foreach (PokemonFalseUnion targetUnion in targets)
        {
            // Showdown: if (target === false) continue;
            // Only skip FalsePokemonUnion (explicit failure). NullPokemonUnion (substitute absorbed)
            // must still be processed to consume PRNG calls for secondary effect chances,
            // matching Showdown where null !== false so the loop body still runs.
            if (targetUnion is FalsePokemonUnion)
            {
                continue;
            }

            // Extract target Pokemon. For NullPokemonUnion (substitute), target will be null.
            Pokemon? target = targetUnion is PokemonPokemonUnion pokemonUnion
                ? pokemonUnion.Pokemon
                : null;

            if (Battle.DebugMode) Battle.Debug($"[Secondaries] Processing secondary effects for target {target?.Name ?? "null (substitute)"}");

            // Run ModifySecondaries event to get the list of secondary effects
            // For null targets (substitute absorbed), use the unmodified secondaries list
            SecondaryEffect[] secondaries;
            if (target != null)
            {
                RelayVar? modifyResult = Battle.RunEvent(EventId.ModifySecondaries, target, source, moveData,
                    moveData.Secondaries);
                secondaries = modifyResult is SecondaryEffectArrayRelayVar secListRv
                    ? secListRv.Effects
                    : moveData.Secondaries;
            }
            else
            {
                secondaries = moveData.Secondaries;
            }

            foreach (SecondaryEffect secondary in secondaries)
            {
                if (Battle.DebugMode) Battle.Debug($"[Secondaries] Secondary effect: VolatileStatus={secondary.VolatileStatus}, Chance={secondary.Chance}");

                int secondaryRoll = Battle.Random(100);

                // User stat boosts or target stat drops can possibly overflow if it goes beyond 256 in Gen 8 or prior
                bool secondaryOverflow = (secondary.Boosts != null || secondary.Self != null) && Battle.Gen <= 8;

                int effectiveChance = secondary.Chance ?? 100;
                if (secondaryOverflow)
                {
                    effectiveChance %= 256;
                }

                if (Battle.DebugMode) Battle.Debug($"[Secondaries] secondaryRoll={secondaryRoll}, effectiveChance={effectiveChance}");

                if (secondary.Chance == null || secondaryRoll < effectiveChance)
                {
                    if (Battle.DebugMode) Battle.Debug($"[Secondaries] Applying secondary effect, calling MoveHit");
                    if (target != null)
                    {
                        MoveHit(target, source, move, secondary, true, isSelf);
                    }
                    else if (secondary.Self != null && move.SelfDropped != true)
                    {
                        // Target is null (Substitute absorbed). In Showdown, moveHit(null, ...) is
                        // still called, and spreadMoveHit skips damage/hit steps but still processes
                        // selfDrops for the secondary's .self (e.g., Aqua Step's speed boost).
                        if (move.MultiHit == null) move.SelfDropped = true;
                        MoveHit(source, source, move, secondary.Self, true, true);
                    }
                }
                else
                {
                    if (Battle.DebugMode) Battle.Debug($"[Secondaries] Secondary effect chance failed");
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
            // Note: when canSwitch == 0, the target is already falsified by RunMoveEffects
            // (which sets didSomething = false via the forceSwitch check), so we never reach
            // here with a live target that can't switch. Matches Showdown's forceSwitch()
            // which has no else branch.
        }

        return damage;
    }
}