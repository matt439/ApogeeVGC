using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    /// <summary>
    /// Tries to hit multiple targets with a move. NOTE: includes single-target moves.
    /// This method processes a move through various hit validation steps (invulnerability,
    /// type immunity, accuracy, etc.) and determines which targets are successfully hit.
    /// </summary>
    /// <param name="targets">List of Pokemon to target</param>
    /// <param name="pokemon">The Pokemon using the move</param>
    /// <param name="move">The move being used</param>
    /// <param name="notActive">If true, sets this as the active move before processing</param>
    /// <returns>True if at least one target was hit, false otherwise</returns>
    public bool TrySpreadMoveHit(List<Pokemon> targets, Pokemon pokemon, ActiveMove move, bool notActive = false)
    {
        // Mark as spread move if targeting multiple Pokemon
        if (targets.Count > 1 && move.SmartTarget != true)
        {
            move.SpreadHit = true;
        }

        // Define the sequence of hit validation steps
        // Each step filters out targets that fail its check
        var moveSteps = new List<Func<List<Pokemon>, Pokemon, ActiveMove, List<BoolIntEmptyUndefinedUnion>?>>
    {
        // 0. check for semi invulnerability
        HitStepInvulnerabilityEvent,

        // 1. run the 'TryHit' event (Protect, Magic Bounce, Volt Absorb, etc.) 
        // (this is step 2 in gens 5 & 6, and step 4 in gen 4)
        HitStepTryEvent,

        // 2. check for type immunity (this is step 1 in gens 4-6)
        HitStepTypeImmunity,

        // 3. check for various move-specific immunities
        HitStepTryImmunity,

        // 4. check accuracy
        HitStepAccuracy,

        // 5. break protection effects
        HitStepBreakProtect,

        // 6. steal positive boosts (Spectral Thief)
        HitStepStealBoosts,

        // 7. loop that processes each hit of the move (has its own steps per iteration)
        HitStepMoveHitLoop,
    };

        // Set as active move if needed
        if (notActive)
        {
            Battle.SetActiveMove(move, pokemon, targets.Count > 0 ? targets[0] : null);
        }

        // Run preliminary events to check if the move can be used at all
        RelayVar? tryResult = Battle.SingleEvent(EventId.Try, move, null, pokemon,
            targets.Count > 0 ? SingleEventSource.FromNullablePokemon(targets[0]) : null, move);

        RelayVar? prepareHitResult1 = Battle.SingleEvent(EventId.PrepareHit, move,
     Battle.InitEffectState(),
          targets.Count > 0 ? SingleEventTarget.FromNullablePokemon(targets[0]) : null,
  pokemon, move);

        RelayVar? prepareHitResult2 = Battle.RunEvent(EventId.PrepareHit, pokemon,
  targets.Count > 0 ? RunEventSource.FromNullablePokemon(targets[0]) : null, move);

    bool hitResult = tryResult is not BoolRelayVar { Value: false } and not NullRelayVar &&
      prepareHitResult1 is not BoolRelayVar { Value: false } and not NullRelayVar &&
        prepareHitResult2 is not BoolRelayVar { Value: false } and not NullRelayVar;

   if (!hitResult)
     {
// Move failed preliminary checks — only show "-fail" for explicit false,
// not for NullRelayVar (TS null = "failed silently")
if (tryResult is BoolRelayVar { Value: false } ||
       prepareHitResult1 is BoolRelayVar { Value: false } ||
             prepareHitResult2 is BoolRelayVar { Value: false })
         {
      if (Battle.DisplayUi)
   {
  Battle.Add("-fail", pokemon);
    Battle.AttrLastMove("[still]");
  }
      }

      // Return true only if this is a "not a failure" case (null result means NOT_FAIL)
    return tryResult is null || prepareHitResult1 is null || prepareHitResult2 is null;
    }

        // Process each hit validation step
        bool atLeastOneFailure = false;

        foreach (var step in moveSteps)
        {
            var hitResults = step(targets, pokemon, move);

            if (hitResults == null)
            {
                continue;
            }

            // Filter targets based on step results
            // Keep targets where result is truthy or is the number 0 (which represents 0 damage but still a hit)
            var newTargets = new List<Pokemon>();
            for (int i = 0; i < targets.Count && i < hitResults.Count; i++)
            {
                if (hitResults[i].IsTruthy() || hitResults[i].IsZero())
                {
                    newTargets.Add(targets[i]);
                }
            }
            targets = newTargets;

            // Track if any target failed this step
            atLeastOneFailure = atLeastOneFailure ||
                hitResults.Any(result => result is BoolBoolIntEmptyUndefinedUnion { Value: false });

            // Disable smart targeting if there was a failure
            if (move.SmartTarget == true && atLeastOneFailure)
            {
                move.SmartTarget = false;
            }

            // No targets left - stop processing
            if (targets.Count == 0)
            {
                break;
            }
        }

        // Store final hit targets in the move
        move.HitTargets = targets;

        bool moveResult = targets.Count > 0;

        // If move completely failed with no specific failures, set moveThisTurnResult to null (NOT_FAIL)
        if (!moveResult && !atLeastOneFailure)
        {
            pokemon.MoveThisTurnResult = null;
        }

        // Add spread move attribute to battle log if applicable
        if (Battle.DisplayUi && move.SpreadHit == true)
        {
            var hitSlots = targets.Select(p => p.Position).ToList();
            Battle.AttrLastMove($"[spread] {string.Join(",", hitSlots)}");
        }

        return moveResult;
    }



    public Undefined AfterMoveSecondaryEvent(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        if ((move.HasSheerForce ?? false) && pokemon.HasAbility(AbilityId.SheerForce)) return new Undefined();

        Battle.SingleEvent(EventId.AfterMoveSecondary, move, null, targets[0], pokemon,
            move);
        Battle.RunEvent(EventId.AfterMoveSecondary, targets.ToArray(), pokemon, move);

        return new Undefined();
    }

    /// <summary>
    /// NOTE: used only for moves that target sides/fields rather than pokemon
    /// </summary>
    public IntUndefinedFalseEmptyUnion TryMoveHit(Pokemon target, Pokemon pokemon, ActiveMove move)
    {
        List<Pokemon> targets = [target];
        return TryMoveHit(targets, pokemon, move);
    }

    /// <summary>
    /// NOTE: used only for moves that target sides/fields rather than pokemon
    /// </summary>
    public IntUndefinedFalseEmptyUnion TryMoveHit(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        Pokemon target = targets[0];

        Battle.SetActiveMove(move, pokemon, target);

        RelayVar? tryResult = Battle.SingleEvent(EventId.Try, move, null, pokemon,
            SingleEventSource.FromNullablePokemon(target), move);
        RelayVar? prepareHitResult1 = Battle.SingleEvent(EventId.PrepareHit, move,
            Battle.InitEffectState(), SingleEventTarget.FromNullablePokemon(target), pokemon, move);
        RelayVar? prepareHitResult2 = Battle.RunEvent(EventId.PrepareHit, pokemon,
            RunEventSource.FromNullablePokemon(target), move);

        bool hitResult = tryResult is not BoolRelayVar { Value: false } and not NullRelayVar &&
                        prepareHitResult1 is not BoolRelayVar { Value: false } and not NullRelayVar &&
                        prepareHitResult2 is not BoolRelayVar { Value: false } and not NullRelayVar;

        if (!hitResult)
        {
            // Only show "-fail" for explicit false, not for NullRelayVar (TS null = "failed silently")
            if (tryResult is BoolRelayVar { Value: false } ||
                prepareHitResult1 is BoolRelayVar { Value: false } ||
                prepareHitResult2 is BoolRelayVar { Value: false })
            {
                if (Battle.DisplayUi)
                {
                    Battle.Add("-fail", pokemon);
                    Battle.AttrLastMove("[still]");
                }
            }

            // Return undefined (NOT_FAIL) if any result was null, otherwise return false
            if (tryResult is null || prepareHitResult1 is null || prepareHitResult2 is null)
            {
                return new Undefined();
            }
            return IntUndefinedFalseEmptyUnion.FromFalse();
        }

        if (move.Target == MoveTarget.All)
        {
            RelayVar? fieldHitResult = Battle.RunEvent(EventId.TryHitField, target, pokemon, move);
            hitResult = fieldHitResult is not BoolRelayVar { Value: false };
        }
        else
        {
            RelayVar? sideHitResult = Battle.RunEvent(EventId.TryHitSide, target, pokemon, move);
            hitResult = sideHitResult is not BoolRelayVar { Value: false };
        }

        if (!hitResult)
        {
            if (Battle.DisplayUi)
            {
                Battle.Add("-fail", pokemon);
                Battle.AttrLastMove("[still]");
            }
            return IntUndefinedFalseEmptyUnion.FromFalse();
        }

        return IntUndefinedFalseEmptyUnion.FromIntUndefinedFalseUnion(MoveHit(target, pokemon, move));
    }

    public (SpreadMoveDamage, SpreadMoveTargets) SpreadMoveHit(SpreadMoveTargets targets, Pokemon pokemon,
    ActiveMove move, HitEffect? hitEffect = null, bool isSecondary = false, bool isSelf = false)
    {
        // Hardcoded for single-target purposes
        // (no spread moves have any kind of onTryHit handler)
        Pokemon target = SpreadMoveTargets.ToPokemonList(targets)[0];
        var damage = new SpreadMoveDamage();

        for (int i = 0; i < targets.Count; i++)
        {
            damage.Add(BoolIntUndefinedUnion.FromBool(true));
        }

        // Store hitEffect in move.HitEffect so RunMoveEffects can access it
        // This matches TypeScript pattern: let moveData = hitEffect as ActiveMove; if (!moveData) moveData = move;
        if (hitEffect != null)
        {
            move.HitEffect = hitEffect;
        }

        // Run TryHit events for field/side moves
        if (move.Target == MoveTarget.All && !isSelf)
        {
            RelayVar? hitResult = Battle.SingleEvent(EventId.TryHitField, move, Battle.InitEffectState(),
                SingleEventTarget.FromNullablePokemon(target), pokemon, move);

            if (hitResult is BoolRelayVar { Value: false })
            {
                if (Battle.DisplayUi)
                {
                    Battle.Add("-fail", pokemon);
                    Battle.AttrLastMove("[still]");
                }

                damage[0] = BoolIntUndefinedUnion.FromBool(false);
                return (damage, targets);
            }
        }
        else if (move.Target is MoveTarget.FoeSide or MoveTarget.AllySide or MoveTarget.AllyTeam && !isSelf)
        {
            RelayVar? hitResult = Battle.SingleEvent(EventId.TryHitSide, move, Battle.InitEffectState(),
                SingleEventTarget.FromNullablePokemon(target), pokemon, move);

            if (hitResult is BoolRelayVar { Value: false })
            {
                if (Battle.DisplayUi)
                {
                    Battle.Add("-fail", pokemon);
                    Battle.AttrLastMove("[still]");
                }

                damage[0] = BoolIntUndefinedUnion.FromBool(false);
                return (damage, targets);
            }
        }
        else
        {
            RelayVar? hitResult = Battle.SingleEvent(EventId.TryHit, move, Battle.InitEffectState(),
                target, pokemon, move);

            if (hitResult is BoolRelayVar { Value: false })
            {
                if (Battle.DisplayUi)
                {
                    Battle.Add("-fail", pokemon);
                    Battle.AttrLastMove("[still]");
                }

                damage[0] = BoolIntUndefinedUnion.FromBool(false);
                return (damage, targets);
            }
        }

        // 0. check for substitute
        if (!isSecondary && !isSelf)
        {
            if (move.Target != MoveTarget.All && move.Target != MoveTarget.AllyTeam &&
                move.Target != MoveTarget.AllySide && move.Target != MoveTarget.FoeSide)
            {
                damage = TryPrimaryHitEvent(damage, targets, pokemon, move, move, isSecondary);
            }
        }

        for (int i = 0; i < targets.Count; i++)
        {
            if (damage[i] == Battle.HitSubstitute)
            {
                damage[i] = BoolIntUndefinedUnion.FromBool(true);
                targets[i] = PokemonFalseUnion.FromFalse();
            }

            // TypeScript: if (targets[i] && isSecondary && !moveData.self)
            // Skip damage calculation for secondary effects that don't have self-targeting
            if (targets[i] is PokemonPokemonUnion && isSecondary && move.Self == null)
            {
                damage[i] = BoolIntUndefinedUnion.FromBool(true);
            }

            if (damage[i] is BoolBoolIntUndefinedUnion { Value: false })
            {
                targets[i] = PokemonFalseUnion.FromFalse();
            }
        }

        // 1. call to Battle.GetDamage
        damage = GetSpreadDamage(damage, targets, pokemon, move, move, isSecondary, isSelf);

        for (int i = 0; i < targets.Count; i++)
        {
            if (damage[i] is BoolBoolIntUndefinedUnion { Value: false })
            {
                targets[i] = PokemonFalseUnion.FromFalse();
            }
        }

        // 2. call to Battle.SpreadDamage
        damage = Battle.SpreadDamage(damage, targets, pokemon, BattleDamageEffect.FromIEffect(move));

        for (int i = 0; i < targets.Count; i++)
        {
            if (damage[i] is BoolBoolIntUndefinedUnion { Value: false })
            {
                targets[i] = PokemonFalseUnion.FromFalse();
            }
        }

        // 3. onHit event happens here
        damage = RunMoveEffects(damage, targets, pokemon, move, move, isSecondary, isSelf);

        for (int i = 0; i < targets.Count; i++)
        {
            if (!(damage[i] is IntBoolIntUndefinedUnion || damage[i] is IntBoolIntUndefinedUnion { Value: 0 }))
            {
                targets[i] = PokemonFalseUnion.FromFalse();
            }
        }

        // steps 4 and 5 can mess with Battle.ActiveTarget, which needs to be preserved for Dancer
        Pokemon? activeTarget = Battle.ActiveTarget;

        // 4. self drops (start checking for targets[i] === false here)
        Battle.Debug($"[SpreadMoveHit] Step 4: Checking self drops for {move.Name}, move.Self != null: {move.Self != null}, move.SelfDropped: {move.SelfDropped}");
        if (move.Self != null && move.SelfDropped != true)
        {
            Battle.Debug($"[SpreadMoveHit] Calling SelfDrops for {move.Name}");
            SelfDrops(targets, pokemon, move, move, isSecondary);
        }
        else
        {
            if (move.Self == null)
            {
                Battle.Debug($"[SpreadMoveHit] Skipping SelfDrops: move.Self is null");
            }
            if (move.SelfDropped == true)
            {
                Battle.Debug($"[SpreadMoveHit] Skipping SelfDrops: move.SelfDropped is true");
            }
        }

        // 5. secondary effects
        // Only process secondaries if this is not already a secondary effect (prevents infinite recursion)
        if (move.Secondaries != null && !isSecondary)
        {
            Secondaries(targets, pokemon, move, move, isSelf);
        }

        Battle.ActiveTarget = activeTarget;

        // 6. force switch
        if (move.ForceSwitch != null)
        {
            damage = ForceSwitch(damage, targets, pokemon, move);
        }

        for (int i = 0; i < targets.Count; i++)
        {
            if (!(damage[i] is IntBoolIntUndefinedUnion || damage[i] is IntBoolIntUndefinedUnion { Value: 0 }))
            {
                targets[i] = PokemonFalseUnion.FromFalse();
            }
        }

        var damagedTargets = new List<Pokemon>();
        var damagedDamage = new List<int>();

        for (int i = 0; i < targets.Count; i++)
        {
            if (damage[i] is IntBoolIntUndefinedUnion intDmg &&
                targets[i] is PokemonPokemonUnion pokemonUnion)
            {
                damagedTargets.Add(pokemonUnion.Pokemon);
                damagedDamage.Add(intDmg.Value);
            }
        }

        int pokemonOriginalHp = pokemon.Hp;

        if (damagedDamage.Count > 0 && !isSecondary && !isSelf)
        {
            Battle.RunEvent(EventId.DamagingHit, damagedTargets.ToArray(), pokemon, move,
                new ArrayRelayVar(damagedDamage.Select(RelayVar (d) => new IntRelayVar(d)).ToList()));

            if (move.OnAfterHit != null)
            {
                foreach (Pokemon t in damagedTargets)
                {
                    Battle.SingleEvent(EventId.AfterHit, move, null, t, pokemon, move);
                }
            }

            if (pokemon.Hp > 0 && pokemon.Hp <= pokemon.MaxHp / 2 && pokemonOriginalHp > pokemon.MaxHp / 2)
            {
                Battle.RunEvent(EventId.EmergencyExit, pokemon, pokemon);
            }
        }

        return (damage, targets);
    }

    public SpreadMoveDamage TryPrimaryHitEvent(SpreadMoveDamage damage, SpreadMoveTargets targets,
        Pokemon pokemon, ActiveMove move, ActiveMove moveData, bool isSecondary = false)
    {
        for (int i = 0; i < targets.Count; i++)
    {
    if (targets[i] is not PokemonPokemonUnion pokemonUnion)
            {
         continue;
            }

  Pokemon target = pokemonUnion.Pokemon;
  RelayVar? result = Battle.RunEvent(EventId.TryPrimaryHit, target, pokemon, moveData);

            // Convert various RelayVar types to BoolIntUndefinedUnion
  // null means NOT_FAIL/continue - treat as success for hit processing
            // VoidReturn means explicit void return - also treat as success
   BoolIntUndefinedUnion damageValue = result switch
     {
    null => BoolIntUndefinedUnion.FromBool(true), // null means NOT_FAIL/continue
     VoidReturnRelayVar => BoolIntUndefinedUnion.FromBool(true), // VoidReturn also means continue
     BoolIntUndefinedUnionRelayVar biuu => biuu.Value, // Already the right type
          BoolRelayVar brv => BoolIntUndefinedUnion.FromBool(brv.Value), // Convert bool
     IntRelayVar irv => BoolIntUndefinedUnion.FromInt(irv.Value), // Convert int
           UndefinedRelayVar => BoolIntUndefinedUnion.FromUndefined(), // Convert undefined
        _ => BoolIntUndefinedUnion.FromBool(true), // Default to true for unknown types
            };

     damage[i] = damageValue;
     }

        return damage;
  }

    public SpreadMoveDamage GetSpreadDamage(SpreadMoveDamage damage, SpreadMoveTargets targets, Pokemon source,
        ActiveMove move, ActiveMove moveData, bool isSecondary = false, bool isSelf = false)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] is not PokemonPokemonUnion pokemonUnion)
            {
                continue;
            }

            Pokemon target = pokemonUnion.Pokemon;
            Battle.ActiveTarget = target;
            damage[i] = BoolIntUndefinedUnion.FromUndefined();

            // Skip damage calculation for self-targeting effects and secondary effects
            // This matches TypeScript behavior and prevents infinite recursion
            // Secondary effects should only apply status/volatile/boosts, not damage
            if (!isSelf && !isSecondary)
            {
                IntUndefinedFalseUnion? curDamage = GetDamage(source, target, moveData);

                // getDamage has several possible return values:
                //
                //   a number:
                //     means that much damage is dealt (0 damage still counts as dealing
                //     damage for the purposes of things like Static)
                //   false:
                //     gives error message: "But it failed!" and move ends
                //   null:
                //     the move ends, with no message (usually, a custom fail message
                //     was already output by an event handler)
                //   undefined:
                //     means no damage is dealt and the move continues
                //
                // basically, these values have the same meanings as they do for event
                // handlers.

                switch (curDamage)
                {
                    case FalseIntUndefinedFalseUnion or null:
                        {
                            if (damage[i] is BoolBoolIntUndefinedUnion { Value: false } && !isSecondary && !isSelf)
                            {
                                if (Battle.DisplayUi)
                                {
                                    Battle.Add("-fail", source);
                                    Battle.AttrLastMove("[still]");
                                }
                            }

                            Battle.Debug("damage calculation interrupted");
                            damage[i] = BoolIntUndefinedUnion.FromBool(false);
                            continue;
                        }
                    case IntIntUndefinedFalseUnion intDamage:
                        damage[i] = BoolIntUndefinedUnion.FromInt(intDamage.Value);
                        break;
                }
            }
        }

        return damage;
    }
}