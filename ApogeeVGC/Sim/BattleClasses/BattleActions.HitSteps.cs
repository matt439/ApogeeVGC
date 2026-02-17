using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    /// <summary>
    /// Hit step 0: Check for semi-invulnerability (Fly, Dig, Dive, etc.).
    /// Returns a list of hit results (true = can hit, false = miss due to invulnerability).
    /// </summary>
    public List<BoolIntEmptyUndefinedUnion> HitStepInvulnerabilityEvent(List<Pokemon> targets,
        Pokemon pokemon,
        ActiveMove move)
    {
        // Helping Hand always hits all targets
        if (move.Id == MoveId.HelpingHand)
        {
            return Enumerable.Repeat(BoolIntEmptyUndefinedUnion.FromBool(true), targets.Count)
                .ToList();
        }

        var hitResults = new List<BoolIntEmptyUndefinedUnion>();

        foreach (Pokemon target in targets)
        {
            bool canHit;

            // Commanding Pokemon cannot be targeted
            if (target.Volatiles.ContainsKey(ConditionId.Commanding))
            {
                canHit = false;
            }
            // Gen 8+: Toxic used by Poison-types always hits (even through semi-invulnerability)
            else if (move.Id == MoveId.Toxic && pokemon.HasType(PokemonType.Poison))
            {
                canHit = true;
            }
            else
            {
                // Run Invulnerability event to check if target can be hit
                RelayVar? invulnResult =
                    Battle.RunEvent(EventId.Invulnerability, target, pokemon, move);

                // Convert RelayVar to boolean (false means invulnerable/miss, true/null means can hit)
                canHit = invulnResult is not BoolRelayVar { Value: false };
            }

            hitResults.Add(BoolIntEmptyUndefinedUnion.FromBool(canHit));

            // Handle miss messaging
            if (!canHit)
            {
                if (move.SmartTarget == true)
                {
                    // Smart target moves (Dragon Darts) disable smart targeting on miss
                    move.SmartTarget = false;
                }
                else
                {
                    // Add miss attribute for non-spread moves
                    if (move.SpreadHit != true)
                    {
                        Battle.AttrLastMove("[miss]");
                    }

                    // Display miss message
                    if (Battle.DisplayUi)
                    {
                        Battle.Add("-miss", pokemon, target);
                    }
                }
            }
        }

        return hitResults;
    }

    /// <summary>
    /// Hit step 1: Run TryHit event (handles Protect, Magic Bounce, Volt Absorb, etc.).
    /// Returns a list of hit results (true = can hit, false = blocked, undefined = NOT_FAIL).
    /// </summary>
    public List<BoolIntEmptyUndefinedUnion> HitStepTryEvent(List<Pokemon> targets, Pokemon pokemon,
        ActiveMove move)
    {
        // Run TryHit event for all targets
        RelayVar? hitResult = Battle.RunEvent(EventId.TryHit, targets.ToArray(), pokemon, move);

        List<RelayVar?> hitResults;
        if (hitResult is ArrayRelayVar arv)
        {
            hitResults = [];
            hitResults.AddRange(arv.Values);
        }
        else
        {
            // If single result, apply to all targets
            hitResults = Enumerable.Repeat(hitResult, targets.Count).ToList();
        }

        // Check if move completely failed (no successes but at least one explicit failure)
        bool hasTrue = false;
        bool hasFalse = false;

        foreach (RelayVar? result in hitResults)
        {
            if (result is BoolRelayVar brv)
            {
                if (brv.Value)
                {
                    hasTrue = true;
                }
                else
                {
                    hasFalse = true;
                }
            }
        }

        if (!hasTrue && hasFalse)
        {
            if (Battle.DisplayUi)
            {
                // If there's a single target, include it in the fail message
                if (targets.Count == 1)
                {
                    Battle.Add("-fail", pokemon, targets[0]);
                }
                else
                {
                    Battle.Add("-fail", pokemon);
                }
            }

            Battle.AttrLastMove("[still]");
        }

        // Convert results to BoolIntEmptyUndefinedUnion
        var convertedResults = new List<BoolIntEmptyUndefinedUnion>();
        foreach (RelayVar? result in hitResults)
        {
            Battle.Debug($"[HitStepTryEvent] Result type: {result?.GetType().Name ?? "null"}");

            // Log the actual value if it's a BoolRelayVar
            if (result is BoolRelayVar brv)
            {
                Battle.Debug($"[HitStepTryEvent] BoolRelayVar value: {brv.Value}");
            }

            // If result is NOT_FAIL (null), keep it as undefined
            // Otherwise convert to boolean (default false if not a boolean)
            if (result is UndefinedRelayVar or null)
            {
                convertedResults.Add(BoolIntEmptyUndefinedUnion.FromUndefined());
            }
            else if (result is BoolRelayVar brv2)
            {
                convertedResults.Add(BoolIntEmptyUndefinedUnion.FromBool(brv2.Value));
            }
            else
            {
                // Any other RelayVar type defaults to false
                Battle.Debug("[HitStepTryEvent] Unknown type, defaulting to false");
                convertedResults.Add(BoolIntEmptyUndefinedUnion.FromBool(false));
            }
        }

        Battle.Debug($"[HitStepTryEvent] Returning {convertedResults.Count} results");
        return convertedResults;
    }

    /// <summary>
    /// Hit step 2: Check for type immunity (e.g., Ground-type moves against Flying-types).
    /// Returns a list of hit results (true = not immune, false = immune).
    /// </summary>
    public static List<BoolIntEmptyUndefinedUnion> HitStepTypeImmunity(List<Pokemon> targets,
        Pokemon pokemon,
        ActiveMove move)
    {
        // Default ignoreImmunity for Status moves if not already set
        move.IgnoreImmunity ??= move.Category == MoveCategory.Status;

        return targets.Select(target =>
                target.RunImmunity(move, !(move.SmartTarget ?? false)))
            .Select(BoolIntEmptyUndefinedUnion.FromBool).ToList();
    }

    /// <summary>
    /// Hit step 3: Check for move-specific immunities (Powder moves, TryImmunity event, Prankster).
    /// Returns a list of hit results (true = can hit, false = immune).
    /// </summary>
    public List<BoolIntEmptyUndefinedUnion> HitStepTryImmunity(List<Pokemon> targets,
        Pokemon pokemon,
        ActiveMove move)
    {
        var hitResults = new List<BoolIntEmptyUndefinedUnion>();

        foreach (Pokemon target in targets)
        {
            bool canHit = true;

            // Gen 6+: Check powder move immunity (Grass-types and Overcoat ability)
            if (Battle.Gen >= 6 &&
                move.Flags.Powder == true &&
                target != pokemon &&
                !Battle.Dex.GetImmunity(move.Condition?.Id ?? ConditionId.None, target.Types))
            {
                Battle.Debug("natural powder immunity");

                if (Battle.DisplayUi)
                {
                    Battle.Add("-immune", target);
                }

                canHit = false;
            }
            // Run TryImmunity event (abilities like Wonder Guard, Flash Fire, Volt Absorb, etc.)
            else if (Battle.SingleEvent(EventId.TryImmunity, move, null, target, pokemon,
                         move) is BoolRelayVar { Value: false })
            {
                if (Battle.DisplayUi)
                {
                    Battle.Add("-immune", target);
                }

                canHit = false;
            }
            // Gen 7+: Check Prankster immunity (Dark-types are immune to Prankster-boosted moves from opponents)
            else if (Battle.Gen >= 7 &&
                     move.PranksterBoosted == true &&
                     pokemon.HasAbility(AbilityId.Prankster) &&
                     !target.IsAlly(pokemon) &&
                     !Battle.Dex.GetImmunity(AbilityId.Prankster, target.Types))
            {
                Battle.Debug("natural prankster immunity");

                // Show hint message unless target has Illusion or move would fail anyway due to status immunity
                if (target.Illusion != null ||
                    !(move.Status != null &&
                      !Battle.Dex.GetImmunity(move.Status.Value, target.Types)))
                {
                    if (Battle.DisplayUi)
                    {
                        Battle.Hint("Since gen 7, Dark is immune to Prankster moves.");
                    }
                }

                if (Battle.DisplayUi)
                {
                    Battle.Add("-immune", target);
                }

                canHit = false;
            }

            hitResults.Add(BoolIntEmptyUndefinedUnion.FromBool(canHit));
        }

        return hitResults;
    }

    /// <summary>
    /// Hit step 4: Check accuracy.
    /// Returns a list of hit results (true = hit, false = miss).
    /// </summary>
    public List<BoolIntEmptyUndefinedUnion> HitStepAccuracy(List<Pokemon> targets, Pokemon pokemon,
        ActiveMove move)
    {
        var hitResults = new List<BoolIntEmptyUndefinedUnion>();

        foreach (Pokemon target in targets)
        {
            Battle.ActiveTarget = target;

            // Calculate true accuracy
            IntTrueUnion accuracy = move.Accuracy;

            // Handle OHKO moves (One-Hit Knockout moves like Fissure, Sheer Cold)
            if (move.Ohko != null)
            {
                // OHKO moves bypass accuracy modifiers
                if (!target.IsSemiInvulnerable())
                {
                    // Base accuracy for OHKO moves is 30%
                    int ohkoAccuracy = 30;

                    // Ice-type OHKO moves (Sheer Cold) have 20% accuracy when used by non-Ice types
                    if (move.Ohko is IceMoveOhko && !pokemon.HasType(PokemonType.Ice))
                    {
                        ohkoAccuracy = 20;
                    }

                    // OHKO moves gain accuracy based on level difference
                    // Only works if: target level <= user level AND (move hits all types OR target doesn't have OHKO immunity type)
                    bool hasTypeImmunity = move.Ohko is BoolMoveOhko { Value: true } ||
                                           move.Ohko is IceMoveOhko &&
                                           target.HasType(PokemonType.Ice);

                    if (pokemon.Level >= target.Level && !hasTypeImmunity)
                    {
                        ohkoAccuracy += pokemon.Level - target.Level;
                    }
                    else
                    {
                        // OHKO failed due to level or type immunity
                        if (Battle.DisplayUi)
                        {
                            Battle.Add("-immune", target, "[ohko]");
                        }

                        hitResults.Add(BoolIntEmptyUndefinedUnion.FromBool(false));
                        continue;
                    }

                    accuracy = IntTrueUnion.FromInt(ohkoAccuracy);
                }
            }
            else
            {
                // Non-OHKO moves: Run ModifyAccuracy event
                RelayVar? accuracyEvent = Battle.RunEvent(EventId.ModifyAccuracy, target, pokemon,
                    move, RelayVar.FromIntTrueUnion(accuracy));

                if (accuracyEvent is IntRelayVar irv)
                {
                    accuracy = IntTrueUnion.FromInt(irv.Value);
                }
                else if (accuracyEvent is BoolRelayVar { Value: true })
                {
                    accuracy = IntTrueUnion.FromTrue();
                }

                // Apply accuracy/evasion boosts if accuracy is not "always hit"
                if (accuracy is IntIntTrueUnion intAccuracy)
                {
                    int boost = 0;

                    // Apply user's accuracy boosts (unless move ignores accuracy)
                    if (move.IgnoreAccuracy != true)
                    {
                        // Create a copy of the boosts table for the event
                        var boostsTable = new BoostsTable
                        {
                            Atk = pokemon.Boosts.Atk,
                            Def = pokemon.Boosts.Def,
                            SpA = pokemon.Boosts.SpA,
                            SpD = pokemon.Boosts.SpD,
                            Spe = pokemon.Boosts.Spe,
                            Accuracy = pokemon.Boosts.Accuracy,
                            Evasion = pokemon.Boosts.Evasion,
                        };

                        RelayVar? boostEvent = Battle.RunEvent(EventId.ModifyBoost, pokemon, null,
                            null, boostsTable);

                        if (boostEvent is BoostsTableRelayVar brv)
                        {
                            boostsTable = brv.Table;
                        }

                        boost = Battle.ClampIntRange(boostsTable.Accuracy, -6, 6);
                    }

                    // Subtract target's evasion boosts (unless move ignores evasion)
                    if (move.IgnoreEvasion != true)
                    {
                        // Create a copy of the target's boosts table for the event
                        var targetBoostsTable = new BoostsTable
                        {
                            Atk = target.Boosts.Atk,
                            Def = target.Boosts.Def,
                            SpA = target.Boosts.SpA,
                            SpD = target.Boosts.SpD,
                            Spe = target.Boosts.Spe,
                            Accuracy = target.Boosts.Accuracy,
                            Evasion = target.Boosts.Evasion,
                        };

                        RelayVar? targetBoostEvent = Battle.RunEvent(EventId.ModifyBoost, target,
                            null,
                            null, targetBoostsTable);

                        if (targetBoostEvent is BoostsTableRelayVar tbrv)
                        {
                            targetBoostsTable = tbrv.Table;
                        }

                        boost = Battle.ClampIntRange(boost - targetBoostsTable.Evasion, -6, 6);
                    }

                    // Apply boost multiplier to accuracy
                    if (boost > 0)
                    {
                        // Positive boosts: multiply accuracy
                        int intAcc = Battle.Trunc(intAccuracy.Value * (3 + boost) / 3);
                        accuracy = intAcc;
                    }
                    else if (boost < 0)
                    {
                        // Negative boosts: divide accuracy
                        int intAcc = Battle.Trunc(intAccuracy.Value * 3 / (3 - boost));
                        accuracy = intAcc;
                    }
                }
            }

            // Check for moves that always hit
            if (move.AlwaysHit == true ||
                (move.Id == MoveId.Toxic && pokemon.HasType(PokemonType.Poison)) ||
                (move is { Target: MoveTarget.Self, Category: MoveCategory.Status } &&
                 !target.IsSemiInvulnerable()))
            {
                accuracy = IntTrueUnion.FromTrue(); // Bypasses OHKO accuracy modifiers
            }
            else
            {
                // Run Accuracy event for final accuracy check
                RelayVar? finalAccuracyEvent = Battle.RunEvent(EventId.Accuracy, target, pokemon,
                    move, RelayVar.FromIntTrueUnion(accuracy));

                if (finalAccuracyEvent is IntRelayVar faeIrv)
                {
                    accuracy = IntTrueUnion.FromInt(faeIrv.Value);
                }
                else if (finalAccuracyEvent is BoolRelayVar { Value: true })
                {
                    accuracy = IntTrueUnion.FromTrue();
                }
            }

            // Check if move hits based on accuracy roll
            bool hits = accuracy switch
            {
                TrueIntTrueUnion => true, // Always hits
                IntIntTrueUnion intAcc => Battle.RandomChance(intAcc.Value, 100), // Roll for hit
                _ => false,
            };

            if (!hits)
            {
                // Move missed
                if (move.SmartTarget == true)
                {
                    move.SmartTarget = false;
                }
                else
                {
                    if (move.SpreadHit != true)
                    {
                        Battle.AttrLastMove("[miss]");
                    }

                    if (Battle.DisplayUi)
                    {
                        Battle.Add("-miss", pokemon, target);
                    }
                }

                // Blunder Policy: Boost speed by 2 stages on miss (not for OHKO moves)
                if (move.Ohko == null && pokemon.HasItem(ItemId.BlunderPolicy) && pokemon.UseItem())
                {
                    Battle.Boost(new SparseBoostsTable { Spe = 2 }, pokemon);
                }

                hitResults.Add(BoolIntEmptyUndefinedUnion.FromBool(false));
                continue;
            }

            // Move hit successfully
            hitResults.Add(BoolIntEmptyUndefinedUnion.FromBool(true));
        }

        return hitResults;
    }

    /// <summary>
    /// Hit step 5: Break protection effects (Protect, King's Shield, etc.).
    /// Returns null to indicate this step doesn't filter targets.
    /// </summary>
    public List<BoolIntEmptyUndefinedUnion>? HitStepBreakProtect(List<Pokemon> targets,
        Pokemon pokemon, ActiveMove move)
    {
        if (move.BreaksProtect != true)
        {
            return null;
        }

        foreach (Pokemon target in targets)
        {
            bool broke = false;

            // Remove individual protection volatiles
            var protectionVolatiles = new[]
            {
                ConditionId.BanefulBunker,
                ConditionId.BurningBulwark,
                ConditionId.Protect,
                ConditionId.SilkTrap,
                ConditionId.SpikyShield,
            };

            foreach (ConditionId effectId in protectionVolatiles)
            {
                if (target.RemoveVolatile(Library.Conditions[effectId]))
                {
                    broke = true;
                }
            }

            // Remove side-wide protection conditions (when targeting opponents in Gen 9)
            if (!target.IsAlly(pokemon))
            {
                var sideProtections = new[]
                {
                    ConditionId.QuickGuard,
                    ConditionId.WideGuard,
                };

                foreach (ConditionId effectId in sideProtections)
                {
                    if (target.Side.RemoveSideCondition(effectId))
                    {
                        broke = true;
                    }
                }
            }

            // Display activation message if protection was broken
            if (broke)
            {
                if (Battle.DisplayUi)
                {
                    if (move.Id == MoveId.Feint)
                    {
                        Battle.Add("-activate", target, "move: Feint");
                    }
                    else
                    {
                        Battle.Add("-activate", target, $"move: {move.Name}", "[broken]");
                    }
                }

                // Gen 9: Remove Stall volatile when protection is broken
                target.RemoveVolatile(Library.Conditions[ConditionId.Stall]);
            }
        }

        return null;
    }

    /// <summary>
    /// Hit step 6: Steal positive boosts (Spectral Thief).
    /// Returns null to indicate this step doesn't filter targets.
    /// </summary>
    public List<BoolIntEmptyUndefinedUnion>? HitStepStealBoosts(List<Pokemon> targets,
        Pokemon pokemon, ActiveMove move)
    {
        if (move.StealsBoosts != true)
        {
            return null;
        }

        Pokemon target = targets[0]; // hardcoded - only first target
        var boosts = new SparseBoostsTable();
        bool stolen = false;

        foreach ((BoostId BoostId, int Value) boost in target.Boosts.GetBoosts())
        {
            if (boost.Value <= 0)
            {
                continue;
            }

            boosts.SetBoost(boost.BoostId, boost.Value);
            stolen = true;
        }

        if (!stolen) return null;

        Battle.AttrLastMove("[still]");

        if (Battle.DisplayUi)
        {
            Battle.Add("-clearpositiveboost", target, pokemon, $"move: {move.Name}");
        }

        Battle.Boost(boosts, pokemon, pokemon);

        // Reset the boosts to 0 for setting on target
        var resetBoosts = new SparseBoostsTable
        {
            Atk = boosts.Atk.HasValue ? 0 : null,
            Def = boosts.Def.HasValue ? 0 : null,
            SpA = boosts.SpA.HasValue ? 0 : null,
            SpD = boosts.SpD.HasValue ? 0 : null,
            Spe = boosts.Spe.HasValue ? 0 : null,
            Accuracy = boosts.Accuracy.HasValue ? 0 : null,
            Evasion = boosts.Evasion.HasValue ? 0 : null,
        };
        target.SetBoost(resetBoosts);

        // Add animation for moves that steal boosts
        if (move.StealsBoosts == true && Battle.DisplayUi)
        {
            Battle.AddMove("-anim", StringNumberDelegateObjectUnion.FromObject(pokemon), move.Name,
                StringNumberDelegateObjectUnion.FromObject(target));
        }

        return null;
    }

    private readonly int[] _multiHitSample =
        [2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 5, 5, 5];

    public List<BoolIntEmptyUndefinedUnion> HitStepMoveHitLoop(List<Pokemon> targets,
        Pokemon pokemon, ActiveMove move)
    {
        // Initialize damage array with 0s for each target
        var damage = new List<BoolIntEmptyUndefinedUnion>();
        for (int i = 0; i < targets.Count; i++)
        {
            damage.Add(BoolIntEmptyUndefinedUnion.FromInt(0));
        }

        move.TotalDamage = IntFalseUnion.FromInt(0);
        pokemon.LastDamage = 0;

        // Determine number of hits
        IntIntArrayUnion targetHits = move.MultiHit ?? 1;
        int targetHitResult;

        Battle.Debug(
            $"[HitStepMoveHitLoop] Move: {move.Name}, MultiHit: {move.MultiHit?.GetType().Name ?? "null"}");

        if (targetHits is IntArrayIntIntArrayUnion range)
        {
            // Hardcoded for specific range (2-5 hits)
            if (range.Values[0] == 2 && range.Values[1] == 5)
            {
                // 35-35-15-15 out of 100 for 2-3-4-5 hits
                targetHitResult = Battle.Sample(_multiHitSample);
                if (targetHitResult < 4 && pokemon.HasItem(ItemId.LoadedDice))
                {
                    targetHitResult = 5 - Battle.Random(2);
                }
            }
            else
            {
                targetHitResult = Battle.Random(range.Values[0], range.Values[1] + 1);
            }
        }
        else if (targetHits is IntIntIntArrayUnion singleHit)
        {
            targetHitResult = singleHit.Value;
        }
        else
        {
            // Fallback (shouldn't happen given the null coalescing above)
            targetHitResult = 1;
        }

        Battle.Debug($"[HitStepMoveHitLoop] Target hit count: {targetHitResult}");

        // Loaded Dice for 10-hit moves (Population Bomb)
        if (targetHitResult == 10 && pokemon.HasItem(ItemId.LoadedDice))
        {
            targetHitResult -= Battle.Random(7);
        }

        bool nullDamage = true;
        var moveDamage = new List<BoolIntUndefinedUnion>();

        // There is no need to recursively check the 'sleepUsable' flag as Sleep Talk can only be used while asleep.
        bool isSleepUsable = move.SleepUsable == true ||
                             (move.SourceEffect is MoveEffectStateId mesi &&
                              Library.Moves.TryGetValue(mesi.MoveId, out Move? sourceMove) &&
                              sourceMove.SleepUsable == true);

        var targetsCopy = new List<Pokemon>(targets);
        int hit;

        for (hit = 1; hit <= targetHitResult; hit++)
        {
            Battle.Debug($"[HitStepMoveHitLoop] Starting hit #{hit} of {targetHitResult}");

            // Break if any target has already failed
            if (damage.Any(d => d is BoolBoolIntEmptyUndefinedUnion { Value: false })) break;

            // Break if user fell asleep and move is not sleep-usable
            if (hit > 1 && pokemon.Status == ConditionId.Sleep && !isSleepUsable) break;

            // Break if all targets are fainted
            if (targets.All(t => t.Hp <= 0)) break;

            move.Hit = hit;

            // Handle smart target moves (Dragon Darts)
            if (move.SmartTarget == true && targets.Count > 1)
            {
                targetsCopy = [targets[hit - 1]];
            }
            else
            {
                targetsCopy = new List<Pokemon>(targets);
            }

            // Some relevant-to-single-target-moves-only things are hardcoded
            Pokemon target = targetsCopy[0];

            if (move.SmartTarget != null)
            {
                if (hit > 1)
                {
                    if (Battle.DisplayUi)
                    {
                        Battle.AddMove("-anim",
                            StringNumberDelegateObjectUnion.FromObject(pokemon),
                            move.Name,
                            StringNumberDelegateObjectUnion.FromObject(target));
                    }
                }
                else
                {
                    Battle.RetargetLastMove(target);
                }
            }

            // Triple Kick - multiaccuracy handling
            if (move.MultiAccuracy == true && hit > 1)
            {
                IntTrueUnion accuracy = move.Accuracy;
                double[] boostTable = [1, 4.0 / 3, 5.0 / 3, 2, 7.0 / 3, 8.0 / 3, 3];

                if (accuracy is IntIntTrueUnion intAccuracy)
                {
                    double accValue = intAccuracy.Value;

                    if (move.IgnoreAccuracy != true)
                    {
                        var boosts = new BoostsTable
                        {
                            Atk = pokemon.Boosts.Atk,
                            Def = pokemon.Boosts.Def,
                            SpA = pokemon.Boosts.SpA,
                            SpD = pokemon.Boosts.SpD,
                            Spe = pokemon.Boosts.Spe,
                            Accuracy = pokemon.Boosts.Accuracy,
                            Evasion = pokemon.Boosts.Evasion,
                        };

                        RelayVar? boostEvent = Battle.RunEvent(EventId.ModifyBoost, pokemon, null,
                            null, boosts);

                        if (boostEvent is BoostsTableRelayVar brv)
                        {
                            boosts = brv.Table;
                        }

                        int boost = Battle.ClampIntRange(boosts.Accuracy, -6, 6);
                        if (boost > 0)
                        {
                            accValue *= boostTable[boost];
                        }
                        else if (boost < 0)
                        {
                            accValue /= boostTable[-boost];
                        }
                    }

                    if (move.IgnoreEvasion != true)
                    {
                        var targetBoosts = new BoostsTable
                        {
                            Atk = target.Boosts.Atk,
                            Def = target.Boosts.Def,
                            SpA = target.Boosts.SpA,
                            SpD = target.Boosts.SpD,
                            Spe = target.Boosts.Spe,
                            Accuracy = target.Boosts.Accuracy,
                            Evasion = target.Boosts.Evasion,
                        };

                        RelayVar? targetBoostEvent = Battle.RunEvent(EventId.ModifyBoost, target,
                            null,
                            null, targetBoosts);

                        if (targetBoostEvent is BoostsTableRelayVar tbrv)
                        {
                            targetBoosts = tbrv.Table;
                        }

                        int boost = Battle.ClampIntRange(targetBoosts.Evasion, -6, 6);
                        switch (boost)
                        {
                            case > 0:
                                accValue /= boostTable[boost];
                                break;
                            case < 0:
                                accValue *= boostTable[-boost];
                                break;
                        }
                    }

                    accuracy = IntTrueUnion.FromInt((int)accValue);
                }

                RelayVar? modifyAccEvent = Battle.RunEvent(EventId.ModifyAccuracy, target, pokemon,
                    move, RelayVar.FromIntTrueUnion(accuracy));

                if (modifyAccEvent is IntRelayVar irv)
                {
                    accuracy = IntTrueUnion.FromInt(irv.Value);
                }
                else if (modifyAccEvent is BoolRelayVar { Value: true })
                {
                    accuracy = IntTrueUnion.FromTrue();
                }

                if (move.AlwaysHit != true)
                {
                    RelayVar? accEvent = Battle.RunEvent(EventId.Accuracy, target, pokemon,
                        move, RelayVar.FromIntTrueUnion(accuracy));

                    if (accEvent is IntRelayVar aerv)
                    {
                        accuracy = IntTrueUnion.FromInt(aerv.Value);
                    }
                    else if (accEvent is BoolRelayVar { Value: true })
                    {
                        accuracy = IntTrueUnion.FromTrue();
                    }

                    bool hits = accuracy switch
                    {
                        TrueIntTrueUnion => true,
                        IntIntTrueUnion intAcc => Battle.RandomChance(intAcc.Value, 100),
                        _ => false,
                    };

                    if (!hits) break;
                }
            }

            // Modifies targetsCopy (which is why it's a copy)
            (SpreadMoveDamage moveDamageThisHit, SpreadMoveTargets _) = SpreadMoveHit(
                SpreadMoveTargets.FromPokemonList(targetsCopy),
                pokemon,
                move,
                new HitEffect { OnHit = move.OnHit?.Handler as ResultMoveHandler });

            // When Dragon Darts targets two different pokemon, targetsCopy is a length 1 array each hit
            // so spreadMoveHit returns a length 1 damage array
            if (move.SmartTarget == true)
            {
                moveDamage.AddRange(moveDamageThisHit);
            }
            else
            {
                moveDamage = moveDamageThisHit;
            }

            if (!moveDamage.Any(val => val is not BoolBoolIntUndefinedUnion
                {
                    Value: false
                })) break;
            nullDamage = false;

            for (int i = 0; i < moveDamage.Count; i++)
            {
                if (move.SmartTarget == true && i != hit - 1) continue;

                // Damage from each hit is individually counted for the
                // purposes of Counter, Metal Burst, and Mirror Coat.
                BoolIntUndefinedUnion md = moveDamage[i];
                damage[i] = md switch
                {
                    BoolBoolIntUndefinedUnion { Value: true } => BoolIntEmptyUndefinedUnion.FromInt(
                        0),
                    IntBoolIntUndefinedUnion intDmg => BoolIntEmptyUndefinedUnion.FromInt(
                        intDmg.Value),
                    _ => BoolIntEmptyUndefinedUnion.FromInt(0),
                };

                // Total damage dealt is accumulated for the purposes of recoil (Parental Bond).
                if (damage[i] is IntBoolIntEmptyUndefinedUnion dmgInt)
                {
                    int currentTotal = move.TotalDamage switch
                    {
                        IntIntFalseUnion intVal => intVal.Value,
                        _ => 0,
                    };
                    move.TotalDamage = IntFalseUnion.FromInt(currentTotal + dmgInt.Value);
                }
            }

            if (move.MindBlownRecoil == true)
            {
                int hpBeforeRecoil = pokemon.Hp;

                Battle.Damage((int)Math.Round(pokemon.MaxHp / 2.0), pokemon, pokemon,
                    BattleDamageEffect.FromIEffect(move),
                    true);

                move.MindBlownRecoil = false;
                if (pokemon.Hp <= pokemon.MaxHp / 2 && hpBeforeRecoil > pokemon.MaxHp / 2)
                {
                    Battle.RunEvent(EventId.EmergencyExit, pokemon, pokemon);
                }
            }

            Battle.EachEvent(EventId.Update);

            if (pokemon.Hp <= 0 && targets.Count == 1)
            {
                hit++; // report the correct number of hits for multihit moves
                break;
            }
        }

        // hit is 1 higher than the actual hit count
        if (hit == 1)
        {
            for (int i = 0; i < damage.Count; i++)
            {
                damage[i] = BoolIntEmptyUndefinedUnion.FromBool(false);
            }

            return damage;
        }

        if (nullDamage)
        {
            for (int i = 0; i < damage.Count; i++)
            {
                damage[i] = BoolIntEmptyUndefinedUnion.FromBool(false);
            }
        }

        Battle.FaintMessages(false, false, pokemon.Hp <= 0);

        if (move is { MultiHit: not null, SmartTarget: null })
        {
            if (Battle.DisplayUi)
            {
                Battle.Add("-hitcount", targets[0], hit - 1);
            }
        }

        // Recoil damage
        if ((move.Recoil != null || move.Id == MoveId.Chloroblast) &&
            move.TotalDamage is IntIntFalseUnion { Value: > 0 } totalDamageInt)
        {
            int hpBeforeRecoil = pokemon.Hp;

            Battle.Damage(CalcRecoilDamage(totalDamageInt.Value, move, pokemon), pokemon, pokemon,
                BattleDamageEffect.FromIEffect(Library.Conditions[ConditionId.Recoil]));

            if (pokemon.Hp <= pokemon.MaxHp / 2 && hpBeforeRecoil > pokemon.MaxHp / 2)
            {
                Battle.RunEvent(EventId.EmergencyExit, pokemon, pokemon);
            }
        }

        // Struggle recoil
        if (move.StruggleRecoil == true)
        {
            int hpBeforeRecoil = pokemon.Hp;
            int recoilDamage =
                Battle.ClampIntRange((int)Math.Round(pokemon.BaseMaxHp / 4.0), 1, null);

            Battle.DirectDamage(recoilDamage, pokemon, pokemon,
                Library.Conditions[ConditionId.StruggleRecoil]);

            if (pokemon.Hp <= pokemon.MaxHp / 2 && hpBeforeRecoil > pokemon.MaxHp / 2)
            {
                Battle.RunEvent(EventId.EmergencyExit, pokemon, pokemon);
            }
        }

        // smartTarget messes up targetsCopy, but smartTarget should in theory ensure that targets will never fail, anyway
        if (move.SmartTarget == true)
        {
            targetsCopy = new List<Pokemon>(targets);
        }

        for (int i = 0; i < targetsCopy.Count; i++)
        {
            Pokemon target = targetsCopy[i];
            if (pokemon != target)
            {
                IntFalseUnion? dmg = moveDamage[i] switch
                {
                    IntBoolIntUndefinedUnion intDmg => IntFalseUnion.FromInt(intDmg.Value),
                    BoolBoolIntUndefinedUnion { Value: false } => IntFalseUnion.FromFalse(),
                    _ => null,
                };

                target.GotAttacked(move, dmg, pokemon);

                if (moveDamage[i] is IntBoolIntUndefinedUnion)
                {
                    target.TimesAttacked += move.SmartTarget == true ? 1 : hit - 1;
                }
            }
        }

        if (move.Ohko != null && targets[0].Hp <= 0)
        {
            if (Battle.DisplayUi)
            {
                Battle.Add("-ohko");
            }
        }

        if (!damage.Any(val => (val is IntBoolIntEmptyUndefinedUnion intVal &&
                                intVal.Value != 0) || val is IntBoolIntEmptyUndefinedUnion
            {
                Value: 0
            }))
        {
            return damage;
        }

        Battle.EachEvent(EventId.Update);

        AfterMoveSecondaryEvent(targetsCopy.Where(_ => true).ToList(), pokemon, move);

        if (!(move.HasSheerForce == true && pokemon.HasAbility(AbilityId.SheerForce)))
        {
            for (int i = 0; i < damage.Count; i++)
            {
                // There are no multihit spread moves, so it's safe to use move.totalDamage for multihit moves
                // The previous check was for `move.multihit`, but that fails for Dragon Darts
                int curDamage;
                if (targets.Count == 1)
                {
                    // For single target moves, use TotalDamage if it's an integer
                    curDamage = move.TotalDamage is IntIntFalseUnion totalDmgInt
                        ? totalDmgInt.Value
                        : 0;
                }
                else
                {
                    // For multi-target moves, use individual damage value
                    curDamage = damage[i] is IntBoolIntEmptyUndefinedUnion dmgInt
                        ? dmgInt.Value
                        : 0;
                }

                if (curDamage > 0 && targets[i].Hp > 0)
                {
                    int targetHpBeforeDamage = (targets[i].HurtThisTurn ?? 0) + curDamage;
                    if (targets[i].Hp <= targets[i].MaxHp / 2 &&
                        targetHpBeforeDamage > targets[i].MaxHp / 2)
                    {
                        Battle.RunEvent(EventId.EmergencyExit, targets[i], pokemon);
                    }
                }
            }
        }

        return damage;
    }
}