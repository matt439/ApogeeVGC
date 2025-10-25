using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    public record RunMoveOptions
    {
        public IEffect? SourceEffect { get; init; }
        public bool? ExternalMove { get; init; }
        public Pokemon? OriginalTarget { get; init; }
    }

    /// <summary>
    /// RunMove is the "outside" move caller. It handles deducting PP,
    /// flinching, full paralysis, etc. All the stuff up to and including
    /// the "POKEMON used MOVE" message.
    /// 
    /// For details of the difference between RunMove and UseMove, see UseMove's info.
    /// 
    /// ExternalMove skips LockMove and PP deduction, mostly for use by Dancer.
    /// </summary>
    public void RunMove(MoveId moveId, Pokemon pokemon, int targetLoc, RunMoveOptions? options = null)
    {
        Move baseMove = Library.Moves[moveId];
        RunMove(baseMove, pokemon, targetLoc, options);
    }

    /// <summary>
    /// RunMove is the "outside" move caller. It handles deducting PP,
    /// flinching, full paralysis, etc. All the stuff up to and including
    /// the "POKEMON used MOVE" message.
    /// 
    /// For details of the difference between RunMove and UseMove, see UseMove's info.
    /// 
    /// ExternalMove skips LockMove and PP deduction, mostly for use by Dancer.
    /// </summary>
    public void RunMove(Move move, Pokemon pokemon, int targetLoc, RunMoveOptions? options = null)
    {
        pokemon.ActiveMoveActions++;

        bool externalMove = options?.ExternalMove ?? false;
        Pokemon? originalTarget = options?.OriginalTarget;
        IEffect? sourceEffect = options?.SourceEffect;

        // Get the target for this move
        Pokemon? target = Battle.GetTarget(pokemon, move, targetLoc, originalTarget);
        var baseMove = move.ToActiveMove();
        int priority = baseMove.Priority;
        bool pranksterBoosted = baseMove.PranksterBoosted ?? false;

        // Allow move override via OverrideAction event (e.g., Assault Vest, Choice items)
        if (baseMove.Id != MoveId.Struggle && !externalMove)
        {
            RelayVar? changedMoveResult = Battle.RunEvent(EventId.OverrideAction, pokemon,
                RunEventSource.FromNullablePokemon(target), baseMove);
            if (changedMoveResult is MoveIdRelayVar moveIdRv && moveIdRv.MoveId != baseMove.Id)
            {
                baseMove = Library.Moves[moveIdRv.MoveId].ToActiveMove();
                baseMove.Priority = priority;
                if (pranksterBoosted)
                {
                    baseMove.PranksterBoosted = pranksterBoosted;
                }
                target = Battle.GetRandomTarget(pokemon, baseMove);
            }
        }

        ActiveMove activeMove = baseMove;
        activeMove.IsExternal = externalMove;
        Battle.ActiveMove = activeMove;
        Battle.ActiveTarget = target;

        // Run BeforeMove event - can prevent the move from happening
        RelayVar? willTryMove = Battle.RunEvent(EventId.BeforeMove, pokemon,
            RunEventSource.FromNullablePokemon(target), activeMove);

        if (willTryMove is BoolRelayVar { Value: false } or null)
        {
            Battle.RunEvent(EventId.MoveAborted, pokemon, RunEventSource.FromNullablePokemon(target), activeMove);
            ClearActiveMove(true);

            // The event 'BeforeMove' could have returned false or null
            // false indicates that this counts as a move failing for the purpose of calculating Stomping Tantrum's base power
            // null indicates the opposite, as the Pokemon didn't have an option to choose anything
            pokemon.MoveThisTurnResult = willTryMove is BoolRelayVar brv ? brv.Value : null;
            return;
        }

        // Used exclusively for a hint later (moves that can't be used twice in a row)
        if (activeMove.Flags.CantUseTwice ?? false)
        {
            if (pokemon.LastMove?.Id == activeMove.Id)
            {
                pokemon.AddVolatile(Library.Conditions[activeMove.Id.ToConditionId()].Id, pokemon, activeMove);
            }
        }

        // Execute beforeMoveCallback if present
        if (activeMove.BeforeMoveCallback is not null)
        {
            BoolVoidUnion callbackResult = activeMove.BeforeMoveCallback(Battle, pokemon, target, activeMove);
            if (callbackResult is BoolBoolVoidUnion { Value: true })
            {
                ClearActiveMove(true);
                pokemon.MoveThisTurnResult = false;
                return;
            }
        }

        pokemon.LastDamage = 0;
        MoveId? lockedMove = null;

        if (!externalMove)
        {
            // Check if Pokemon is locked into a move (e.g., Outrage, Rollout)
            RelayVar? lockedMoveResult = Battle.RunEvent(EventId.LockMove, pokemon);
            lockedMove = lockedMoveResult switch
            {
                MoveIdRelayVar lockedMoveRv => lockedMoveRv.MoveId,
                BoolRelayVar { Value: true } => null,
                _ => lockedMove,
            };

            if (lockedMove == null)
            {
                // Deduct PP
                int ppDeducted = pokemon.DeductPp(baseMove, null,
                    PokemonFalseUnion.FromNullablePokemon(target));

                if (ppDeducted == 0 && activeMove.Id != MoveId.Struggle)
                {
                    if (Battle.DisplayUi)
                    {
                        Battle.Add("cant", pokemon, "nopp", activeMove);
                    }
                    ClearActiveMove(true);
                    pokemon.MoveThisTurnResult = false;
                    return;
                }
            }
            else
            {
                sourceEffect = Library.Conditions[ConditionId.LockedMove];
            }

            pokemon.MoveUsed(activeMove, targetLoc);
        }

        // Dancer Petal Dance hack - track if we need to preserve lock state
        bool noLock = externalMove && !pokemon.Volatiles.ContainsKey(ConditionId.LockedMove);

        // Actually use the move
        bool moveDidSomething = UseMove(baseMove, pokemon, new UseMoveOptions
        {
            Target = target,
            SourceEffect = sourceEffect,
        });

        Battle.LastSuccessfulMoveThisTurn = moveDidSomething && Battle.ActiveMove != null
            ? Battle.ActiveMove.Id
            : null;

        if (Battle.ActiveMove != null)
        {
            activeMove = Battle.ActiveMove;
        }

        Battle.SingleEvent(EventId.AfterMove, activeMove, null, pokemon,
            SingleEventSource.FromNullablePokemon(target), activeMove);

        Battle.RunEvent(EventId.AfterMove, pokemon, RunEventSource.FromNullablePokemon(target), activeMove);

        if (activeMove.Flags.CantUseTwice ?? false)
        {
            if (pokemon.RemoveVolatile(Library.Conditions[activeMove.Id.ToConditionId()]))
            {
                if (Battle.DisplayUi)
                {
                    Battle.Hint($"Some effects can force a Pokemon to use {activeMove.Name} again in a row.");
                }
            }
        }

        // Handle Dancer ability - activates in order of lowest speed stat to highest
        if ((activeMove.Flags.Dance ?? false) && moveDidSomething && !(activeMove.IsExternal ?? false))
        {
            List<Pokemon> dancers = [];
            foreach (Pokemon currentPoke in Battle.GetAllActive())
            {
                if (pokemon == currentPoke) continue;
                if (currentPoke.HasAbility(AbilityId.Dancer) && !currentPoke.IsSemiInvulnerable())
                {
                    dancers.Add(currentPoke);
                }
            }

            // Dancer activates in order of lowest speed stat to highest
            // Note that the speed stat used is after any volatile replacements like Speed Swap,
            // but before any multipliers like Agility or Choice Scarf
            // Ties go to whichever Pokemon has had the ability for the least amount of time
            dancers.Sort((a, b) =>
            {
                int speedDiff = a.StoredStats[StatIdExceptHp.Spe] - b.StoredStats[StatIdExceptHp.Spe];
                if (speedDiff != 0) return speedDiff;
                return a.AbilityState.EffectOrder - b.AbilityState.EffectOrder;
            });

            Pokemon? targetOf1StDance = Battle.ActiveTarget;
            foreach (Pokemon dancer in dancers)
            {
                if (Battle.FaintMessages() ?? false) break;
                if (dancer.Fainted) continue;

                if (Battle.DisplayUi)
                {
                    Battle.Add("-activate", dancer, "ability: Dancer");
                }

                Pokemon dancersTarget = targetOf1StDance != null &&
                                       !targetOf1StDance.IsAlly(dancer) &&
                                       pokemon.IsAlly(dancer)
                    ? targetOf1StDance
                    : pokemon;

                int dancersTargetLoc = dancer.GetLocOf(dancersTarget);
                RunMove(activeMove.Id, dancer, dancersTargetLoc, new RunMoveOptions
                {
                    SourceEffect = Library.Abilities[AbilityId.Dancer],
                    ExternalMove = true,
                });
            }
        }

        // Clear locked move volatile if this was an external move and the Pokemon has the volatile
        if (noLock && pokemon.Volatiles.ContainsKey(ConditionId.LockedMove))
        {
            pokemon.DeleteVolatile(ConditionId.LockedMove);
        }

        Battle.FaintMessages();
        Battle.CheckWin();
    }

    public record UseMoveOptions
    {
        public Pokemon? Target { get; init; }
        public IEffect? SourceEffect { get; init; }
    }

    /// <summary>
    /// UseMove is the "inside" move caller. It handles effects of the
    /// move itself, but not the idea of using the move.
    /// Most caller effects, like Sleep Talk, Nature Power, Magic Bounce,
    /// etc use useMove.
    /// The only ones that use runMove are Instruct, Pursuit, and
    /// Dancer.
    /// </summary>
    public bool UseMove(MoveId moveId, Pokemon pokemon, UseMoveOptions? options = null)
    {
        Move move = Library.Moves[moveId];
        return UseMove(move, pokemon, options);
    }

    public bool UseMove(Move move, Pokemon pokemon, UseMoveOptions? options = null)
    {
        pokemon.MoveThisTurnResult = new Undefined();
        BoolUndefinedUnion? oldMoveResult = pokemon.MoveThisTurnResult;
        bool moveResult = UseMoveInner(move, pokemon, options);
        if (oldMoveResult == pokemon.MoveThisTurnResult)
        {
            pokemon.MoveThisTurnResult = moveResult;
        }
        return moveResult;
    }

    public void UseMoveInner(MoveId moveId, Pokemon pokemon, UseMoveOptions? options = null)
    {
        Move move = Library.Moves[moveId];
        UseMoveInner(move, pokemon, options);
    }

    public bool UseMoveInner(Move move, Pokemon pokemon, UseMoveOptions? options = null)
    {
        Pokemon? target = options?.Target;
        IEffect? sourceEffect = options?.SourceEffect;

        // Default sourceEffect to battle effect if not provided and battle has an active effect
        if (sourceEffect == null && Battle.Effect.EffectStateId != EffectStateId.FromEmpty())
        {
            sourceEffect = Battle.Effect;
        }

        // Clear sourceEffect for Instruct and Custap Berry
        if (sourceEffect is ActiveMove { Id: MoveId.Instruct } or Item { Id: ItemId.CustapBerry })
        {
            sourceEffect = null;
        }

        // Get active move
        var activeMove = move.ToActiveMove();
        pokemon.LastMoveUsed = activeMove;

        // Copy priority and prankster boost from active move if it exists
        if (Battle.ActiveMove != null)
        {
            activeMove.Priority = Battle.ActiveMove.Priority;
            if (activeMove.HasBounced != true)
            {
                activeMove.PranksterBoosted = Battle.ActiveMove.PranksterBoosted;
            }
        }

        // Store base target for later comparison
        MoveTarget baseTarget = activeMove.Target;

        // Run ModifyTarget event
        RelayVar? targetRelayVar = Battle.RunEvent(EventId.ModifyTarget, pokemon,
            RunEventSource.FromNullablePokemon(target), activeMove, RelayVar.FromNullablePokemon(target));

        if (targetRelayVar is PokemonRelayVar prv)
        {
            target = prv.Pokemon;
        }

        // Get random target if target is undefined
        target ??= Battle.GetRandomTarget(pokemon, activeMove);

        // Self/allies target always targets the user
        if (activeMove.Target is MoveTarget.Self or MoveTarget.Allies)
        {
            target = pokemon;
        }

        // Set source effect information
        if (sourceEffect != null)
        {
            activeMove.SourceEffect = sourceEffect.EffectStateId;
            if (sourceEffect is ActiveMove sourceMove)
            {
                activeMove.IgnoreAbility = sourceMove.IgnoreAbility;
            }
        }

        // Set as active move
        Battle.SetActiveMove(activeMove, pokemon, target);

        // Run ModifyType event (single)
        Battle.SingleEvent(EventId.ModifyType, activeMove, null, pokemon,
            SingleEventSource.FromNullablePokemon(target), activeMove, activeMove);

        // Run ModifyMove event (single)
        Battle.SingleEvent(EventId.ModifyMove, activeMove, null, pokemon,
            SingleEventSource.FromNullablePokemon(target), activeMove, activeMove);

        // Check if target changed and adjust
        if (baseTarget != activeMove.Target)
        {
            target = Battle.GetRandomTarget(pokemon, activeMove);
        }

        // Run ModifyType event (global)
        RelayVar? modifyTypeResult = Battle.RunEvent(EventId.ModifyType, pokemon,
            RunEventSource.FromNullablePokemon(target), activeMove, activeMove);

        if (modifyTypeResult is EffectRelayVar { Effect: ActiveMove modifiedMove1 })
        {
            activeMove = modifiedMove1;
        }

        // Run ModifyMove event (global)
        RelayVar? modifyMoveResult = Battle.RunEvent(EventId.ModifyMove, pokemon,
            RunEventSource.FromNullablePokemon(target), activeMove, activeMove);

        if (modifyMoveResult is EffectRelayVar { Effect: ActiveMove modifiedMove2 })
        {
            activeMove = modifiedMove2;
        }

        // Check if target changed again and adjust
        if (baseTarget != activeMove.Target)
        {
            target = Battle.GetRandomTarget(pokemon, activeMove);
        }

        // Early exit if move is null or pokemon fainted
        if (pokemon.Fainted)
        {
            return false;
        }

        // Build move message attributes
        string moveName = activeMove.Name;

        // Add move message to battle log
        if (Battle.DisplayUi)
        {
            if (target is null)
            {
                throw new InvalidOperationException("Target cannot be null when displaying move message.");
            }

            if (sourceEffect != null)
            {
                Battle.AddMove("move", StringNumberDelegateObjectUnion.FromObject(pokemon), moveName,
                    StringNumberDelegateObjectUnion.FromObject(target), $"[from] {sourceEffect.EffectStateId}");
            }
            else
            {
                Battle.AddMove("move", StringNumberDelegateObjectUnion.FromObject(pokemon), moveName,
                    StringNumberDelegateObjectUnion.FromObject(target));
            }
        }

        // Handle no target
        if (target == null)
        {
            if (Battle.DisplayUi)
            {
                Battle.AttrLastMove("[notarget]");
                Battle.Add(Battle.Gen >= 5 ? "-fail" : "-notarget", pokemon);
            }

            return false;
        }

        // Get move targets for Pressure PP deduction
        Pokemon.MoveTargets moveTargets = pokemon.GetMoveTargets(activeMove, target);
        var targets = moveTargets.Targets;
        var pressureTargets = moveTargets.PressureTargets;

        // Update target to last in list (for redirection)
        if (targets.Count > 0)
        {
            target = targets[^1];
        }

        // Determine if this is a caller move for Pressure
        ActiveMove? callerMoveForPressure = null;
        if (sourceEffect is ActiveMove { Pp: > 0 } sm)
        {
            callerMoveForPressure = sm;
        }

        // Handle Pressure ability PP deduction
        if (sourceEffect == null || callerMoveForPressure != null)
        {
            int extraPp = 0;
            foreach (RelayVar? ppDropEvent in pressureTargets.Select(pressureSource =>
                         Battle.RunEvent(EventId.DeductPp, pressureSource, pokemon, activeMove)))
            {
                if (ppDropEvent is IntRelayVar irv)
                {
                    extraPp += irv.Value;
                }
                else if (ppDropEvent is not BoolRelayVar { Value: true })
                {
                    extraPp += 0;
                }
            }

            if (extraPp > 0)
            {
                pokemon.DeductPp(callerMoveForPressure ?? activeMove, extraPp);
            }
        }

        // Run TryMove events
        bool tryMoveResult = Battle.SingleEvent(EventId.TryMove, activeMove, null, pokemon,
                                 SingleEventSource.FromNullablePokemon(target), activeMove)
                                 is not BoolRelayVar { Value: false }
                             && Battle.RunEvent(EventId.TryMove, pokemon,
                                 RunEventSource.FromNullablePokemon(target), activeMove)
                                 is not BoolRelayVar { Value: false };

        if (!tryMoveResult)
        {
            activeMove.MindBlownRecoil = false;
            return false;
        }

        // Run UseMoveMessage event
        Battle.SingleEvent(EventId.UseMoveMessage, activeMove, null, pokemon,
            SingleEventSource.FromNullablePokemon(target), activeMove);

        // Set default ignoreImmunity for Status moves
        activeMove.IgnoreImmunity ??= activeMove.Category == MoveCategory.Status;

        // Gen 5-8: Self-destruct moves always faint the user immediately
        // Gen 9 checks selfdestruct later
        if (Battle.Gen != 4 && activeMove.SelfDestruct is AlwaysMoveSelfDestruct)
        {
            Battle.Faint(pokemon, pokemon, activeMove);
        }

        bool moveResult = false;

        // Execute move and track result

        if (activeMove.Target is MoveTarget.All or MoveTarget.FoeSide or MoveTarget.AllySide or MoveTarget.AllyTeam)
        {
            // Multi-target moves
            IntUndefinedFalseEmptyUnion damage = TryMoveHit(targets, pokemon, activeMove);

            // Check for NOT_FAIL result
            if (damage is UndefinedIntUndefinedFalseEmptyUnion)
            {
                pokemon.MoveThisTurnResult = null;
            }

            // Set moveResult based on damage (true if damage exists, is 0, or is undefined)
            if (damage is IntIntUndefinedFalseEmptyUnion or UndefinedIntUndefinedFalseEmptyUnion)
            {
                moveResult = true;
            }
        }
        else
        {
            // Single-target moves
            if (targets.Count == 0)
            {
                if (Battle.DisplayUi)
                {
                    Battle.AttrLastMove("[notarget]");
                    Battle.Add(Battle.Gen >= 5 ? "-fail" : "-notarget", pokemon);
                }

                return false;
            }

            // Gen 4: Self-destruct moves faint the user before hitting
            if (Battle.Gen == 4 && activeMove.SelfDestruct is AlwaysMoveSelfDestruct)
            {
                Battle.Faint(pokemon, pokemon, activeMove);
            }

            moveResult = TrySpreadMoveHit(targets, pokemon, activeMove, notActive: false);
        }

        // Apply self-boost if move succeeded
        if (activeMove.SelfBoost != null && moveResult)
        {
            MoveHit(pokemon, pokemon, activeMove, new HitEffect { Boosts = activeMove.SelfBoost },
                false, true);
        }

        // Check if user fainted
        if (pokemon.Hp <= 0)
        {
            Battle.Faint(pokemon, pokemon, activeMove);
        }

        // If move failed, trigger MoveFail event
        if (!moveResult)
        {
            Battle.SingleEvent(EventId.MoveFail, activeMove, null, target, pokemon,
                activeMove);
            return false;
        }

        // Handle AfterMoveSecondary effects (excluding Sheer Force and future moves)
        if (!(activeMove.HasSheerForce == true && pokemon.HasAbility(AbilityId.SheerForce)) &&
            activeMove.Flags.FutureMove != true)
        {
            int originalHp = pokemon.Hp;

            // Trigger AfterMoveSecondarySelf events
            Battle.SingleEvent(EventId.AfterMoveSecondarySelf, activeMove, null, pokemon,
                SingleEventSource.FromNullablePokemon(target), activeMove);
            Battle.RunEvent(EventId.AfterMoveSecondarySelf, pokemon,
                RunEventSource.FromNullablePokemon(target), activeMove);

            // Check for Emergency Exit activation (if user's HP dropped below 50%)
            if (pokemon != target && activeMove.Category != MoveCategory.Status)
            {
                if (pokemon.Hp <= pokemon.MaxHp / 2 && originalHp > pokemon.MaxHp / 2)
                {
                    Battle.RunEvent(EventId.EmergencyExit, pokemon, pokemon);
                }
            }
        }

        return true;
    }

    public IntUndefinedFalseUnion MoveHit(Pokemon? target, Pokemon pokemon, ActiveMove move,
        HitEffect? moveData = null, bool isSecondary = false, bool isSelf = false)
    {
        List<Pokemon> targets = target != null ? [target] : [];
        return ExecuteMoveHit(targets, pokemon, move, moveData, isSecondary, isSelf);
    }

    public IntUndefinedFalseUnion MoveHit(List<Pokemon?> targets, Pokemon pokemon, ActiveMove move,
        HitEffect? moveData = null, bool isSecondary = false, bool isSelf = false)
    {
        var validTargets = targets.Where(t => t != null).Cast<Pokemon>().ToList();
        return ExecuteMoveHit(validTargets, pokemon, move, moveData, isSecondary, isSelf);
    }

    public bool TargetTypeChoices(MoveTarget type)
    {
        return _choosableTargets.Contains(type);
    }
}