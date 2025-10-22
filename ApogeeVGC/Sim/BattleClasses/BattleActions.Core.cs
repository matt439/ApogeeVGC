using ApogeeVGC.Data;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions(IBattle battle)
{
    public IBattle Battle { get; init; } = battle;
    public Library Library => Battle.Library;

    private readonly HashSet<MoveTarget> _choosableTargets =
    [
        MoveTarget.Normal,
        MoveTarget.Any,
        MoveTarget.AdjacentAlly,
        MoveTarget.AdjacentAllyOrSelf,
        MoveTarget.AdjacentFoe,
    ];

    #region Switch

    public bool SwitchIn(Pokemon pokemon, int pos, IEffect? sourceEffect = null, bool isDrag = false)
    {
        // Validate the Pokemon exists and is not already active
        if (pokemon.IsActive)
        {
            // In production, we might want to log this instead of printing
            // Battle.Hint("A switch failed because the Pokémon trying to switch in is already in.");
            return false;
        }

        Side side = pokemon.Side;

        // Validate the switch position
        if (pos >= side.Active.Count)
        {
            throw new ArgumentException($"Invalid switch position {pos} / {side.Active.Count}");
        }

        Pokemon oldActive = side.Active[pos];
        Pokemon? unfaintedActive = oldActive.Hp > 0 ? oldActive : null;

        if (unfaintedActive != null)
        {
            oldActive.BeingCalledBack = true;

            // Determine if we need to copy volatiles (for moves like U-turn, Shed Tail, etc.)
            MoveSelfSwitch? switchCopyFlag = null;
            if (sourceEffect is ActiveMove { SelfSwitch: not null } move)
            {
                switchCopyFlag = move.SelfSwitch;
            }

            // Run BeforeSwitchOut event unless it's a drag or the flag is set
            if (!oldActive.SkipBeforeSwitchOutEventFlag && !isDrag)
            {
                Battle.RunEvent(EventId.BeforeSwitchOut, oldActive);
                if (Battle.Gen >= 5)
                {
                    Battle.EachEvent(EventId.Update);
                }
            }
            oldActive.SkipBeforeSwitchOutEventFlag = false;

            // Run SwitchOut event - can be interrupted in custom formats
            RelayVar? switchOutResult = Battle.RunEvent(EventId.SwitchOut, oldActive);
            if (switchOutResult is BoolRelayVar { Value: false })
            {
                // Warning: DO NOT interrupt a switch-out if you just want to trap a pokemon.
                // To trap a pokemon and prevent it from switching out, (e.g. Mean Look, Magnet Pull)
                // use the 'trapped' flag instead.

                // Note: Nothing in the real games can interrupt a switch-out (except Pursuit KOing,
                // which is handled elsewhere); this is just for custom formats.
                return false;
            }

            // Will definitely switch out at this point

            // Clear Illusion
            oldActive.Illusion = null;

            // End ability and item effects
            Battle.SingleEvent(EventId.End, oldActive.GetAbility(), oldActive.AbilityState, oldActive);
            Battle.SingleEvent(EventId.End, oldActive.GetItem(), oldActive.ItemState, oldActive);

            // If a pokemon is forced out by Whirlwind/etc or Eject Button/Pack, it can't use its chosen move
            Battle.Queue.CancelAction(oldActive);

            // Gen 4 special case: preserve last move
            Move? newMove = null;
            if (Battle.Gen == 4 && sourceEffect != null)
            {
                newMove = oldActive.LastMove;
            }

            // Copy volatiles if needed (U-turn, Shed Tail, etc.)
            if (switchCopyFlag != null)
            {
                switch (switchCopyFlag)
                {
                    case ShedTailMoveSelfSwitch:
                        pokemon.CopyVolatileFrom(oldActive, ConditionId.ShedTail);
                        break;
                    default:
                        pokemon.CopyVolatileFrom(oldActive, false);
                        break;
                }
            }

            // Restore last move if we preserved it
            if (newMove != null)
            {
                pokemon.LastMove = newMove;
            }

            // Clear all volatiles from the old active Pokemon
            oldActive.ClearVolatile();
        }

        // Update Pokemon states and positions
        oldActive.IsActive = false;
        oldActive.IsStarted = false;
        oldActive.UsedItemThisTurn = false;
        oldActive.StatsRaisedThisTurn = false;
        oldActive.StatsLoweredThisTurn = false;
        oldActive.Position = pokemon.Position;

        // Clear status if fainted
        if (oldActive.Fainted)
        {
            oldActive.Status = ConditionId.None;
        }

        // Gen 4 and earlier: transfer last item
        if (Battle.Gen <= 4)
        {
            pokemon.LastItem = oldActive.LastItem;
            oldActive.LastItem = ItemId.None;
        }

        // Swap positions in the side's Pokemon list
        pokemon.Position = pos;
        side.Pokemon[pokemon.Position] = pokemon;
        side.Pokemon[oldActive.Position] = oldActive;

        // Activate the new Pokemon
        pokemon.IsActive = true;
        side.Active[pos] = pokemon;
        pokemon.ActiveTurns = 0;
        pokemon.ActiveMoveActions = 0;

        // Reset move usage flags
        foreach (MoveSlot moveSlot in pokemon.MoveSlots)
        {
            moveSlot.Used = false;
        }

        pokemon.AbilityState.EffectOrder = Battle.EffectOrder++;
        pokemon.ItemState.EffectOrder = Battle.EffectOrder++;

        // Run BeforeSwitchIn event
        Battle.RunEvent(EventId.BeforeSwitchIn, pokemon);

        // Add switch/drag message to battle log
        if (sourceEffect != null)
        {
            UiGenerator.PrintSwitchEvent(pokemon, isDrag, sourceEffect);
        }
        else
        {
            UiGenerator.PrintSwitchEvent(pokemon, isDrag);
        }

        // Gen 2 special case: track drag turn
        if (isDrag && Battle.Gen == 2)
        {
            pokemon.DraggedIn = Battle.Turn;
        }

        pokemon.PreviouslySwitchedIn++;

        // Schedule RunSwitch action
        if (isDrag && Battle.Gen >= 5)
        {
            // runSwitch happens immediately so that Mold Breaker can make hazards bypass Clear Body and Levitate
            RunSwitch(pokemon);
        }
        else
        {
            Battle.Queue.InsertChoice(new RunSwitchAction
            {
                Pokemon = pokemon,
            });
        }

        return true;
    }

    public bool DragIn(Side side, int pos)
    {
        Pokemon? pokemon = Battle.GetRandomSwitchable(side);
        if (pokemon == null || pokemon.IsActive)
        {
            return false;
        }

        Pokemon? oldActive = side.Active[pos];
        if (oldActive == null)
        {
            throw new InvalidOperationException("Nothing to drag out");
        }

        if (oldActive.Hp <= 0)
        {
            return false;
        }

        RelayVar? dragOutResult = Battle.RunEvent(EventId.DragOut, oldActive);

        return dragOutResult is not BoolRelayVar { Value: false } &&
               SwitchIn(pokemon, pos, null, true);
    }

    public bool RunSwitch(Pokemon pokemon)
    {
        List<Pokemon> switchersIn = [pokemon];

        while (Battle.Queue.Peek() is RunSwitchAction)
        {
            IAction? nextSwitch = Battle.Queue.Shift();
            if (nextSwitch is RunSwitchAction runSwitchAction)
            {
                switchersIn.Add(runSwitchAction.Pokemon ?? throw new InvalidOperationException("Pokemon must" +
                    "not be null here."));
            }
        }

        var allActive = Battle.GetAllActive(true);
        Battle.SpeedSort(allActive);
        Battle.SpeedOrder = allActive.Select(a => a.Side.N * a.Battle.Sides.Count + a.Position).ToList();
        Battle.FieldEvent(EventId.SwitchIn, switchersIn);

        foreach (Pokemon poke in switchersIn.Where(poke => poke.Hp > 0))
        {
            poke.IsStarted = true;
            poke.DraggedIn = null;
        }

        return true;
    }

    #endregion

    #region Moves

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
            BoolVoidUnion callbackResult = activeMove.BeforeMoveCallback.GetValueOrThrow()(Battle, pokemon, target, activeMove);
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
                    if (Battle.PrintDebug)
                    {
                        UiGenerator.PrintCantEvent(pokemon, "nopp", activeMove);
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
                if (Battle.PrintDebug)
                {
                    UiGenerator.PrintHint($"Some effects can force a Pokemon to use {activeMove.Name} again in a row.");
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

                if (Battle.PrintDebug)
                {
                    UiGenerator.PrintActivateEvent(dancer, Library.Abilities[AbilityId.Dancer]);
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

        string attrs = "";
        if (sourceEffect != null)
        {
            attrs += $"|[from] {sourceEffect.EffectStateId}"; // Note: Needs proper fullname formatting
        }

        // Add move message to battle log
        UiGenerator.PrintMoveEvent(pokemon, moveName, target, attrs);

        // Handle no target
        if (target == null)
        {
            // Battle.AttrLastMove("[notarget]"); // Skipping attribute
            if (Battle.PrintDebug)
            {
                UiGenerator.PrintFailEvent(pokemon);
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
            IntUndefinedFalseUnion damage = TryMoveHit(targets, pokemon, activeMove);

            // Check for NOT_FAIL result
            if (damage is UndefinedIntUndefinedFalseUnion)
            {
                pokemon.MoveThisTurnResult = null;
            }
            
            // Set moveResult based on damage (true if damage exists, is 0, or is undefined)
            if (damage is IntIntUndefinedFalseUnion or UndefinedIntUndefinedFalseUnion)
            {
                moveResult = true;
            }
        }
        else
        {
            // Single-target moves
            if (targets.Count == 0)
            {
                // Battle.AttrLastMove("[notarget]"); // Skipping attribute
                if (Battle.PrintDebug)
                {
                    UiGenerator.PrintFailEvent(pokemon);
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
        var moveSteps = new List<Func<List<Pokemon>, Pokemon, ActiveMove, List<BoolIntUndefinedUnion>?>>
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

        bool hitResult = tryResult is not BoolRelayVar { Value: false } &&
                        prepareHitResult1 is not BoolRelayVar { Value: false } &&
                        prepareHitResult2 is not BoolRelayVar { Value: false };

        if (!hitResult)
        {
            // Move failed preliminary checks
            if (tryResult is BoolRelayVar { Value: false } ||
                prepareHitResult1 is BoolRelayVar { Value: false } ||
                prepareHitResult2 is BoolRelayVar { Value: false })
            {
                if (Battle.PrintDebug)
                {
                    UiGenerator.PrintFailEvent(pokemon);
                }
                Battle.AttrLastMove("[still]");
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
                hitResults.Any(result => result is BoolBoolIntUndefinedUnion { Value: false });

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
        if (move.SpreadHit == true)
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
    public IntUndefinedFalseUnion TryMoveHit(Pokemon target, Pokemon pokemon, ActiveMove move)
    {
        List<Pokemon> targets = [target];
        return TryMoveHit(targets, pokemon, move);
    }

    /// <summary>
    /// NOTE: used only for moves that target sides/fields rather than pokemon
    /// </summary>
    public IntUndefinedFalseUnion TryMoveHit(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        Pokemon target = targets[0];

        Battle.SetActiveMove(move, pokemon, target);

        RelayVar? tryResult = Battle.SingleEvent(EventId.Try, move, null, pokemon,
            SingleEventSource.FromNullablePokemon(target), move);
        RelayVar? prepareHitResult1 = Battle.SingleEvent(EventId.PrepareHit, move,
            Battle.InitEffectState(), SingleEventTarget.FromNullablePokemon(target), pokemon, move);
        RelayVar? prepareHitResult2 = Battle.RunEvent(EventId.PrepareHit, pokemon,
            RunEventSource.FromNullablePokemon(target), move);

        bool hitResult = tryResult is not BoolRelayVar { Value: false } &&
                        prepareHitResult1 is not BoolRelayVar { Value: false } &&
                        prepareHitResult2 is not BoolRelayVar { Value: false };

        if (!hitResult)
        {
            if (tryResult is BoolRelayVar { Value: false } ||
                prepareHitResult1 is BoolRelayVar { Value: false } ||
                prepareHitResult2 is BoolRelayVar { Value: false })
            {
                if (Battle.PrintDebug)
                {
                    UiGenerator.PrintFailEvent(pokemon);
                }
                Battle.AttrLastMove("[still]");
            }
            
            // Return undefined (NOT_FAIL) if any result was null, otherwise return false
            if (tryResult is null || prepareHitResult1 is null || prepareHitResult2 is null)
            {
                return new Undefined();
            }
            return IntUndefinedFalseUnion.FromFalse();
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
            if (Battle.PrintDebug)
            {
                UiGenerator.PrintFailEvent(pokemon);
            }
            Battle.AttrLastMove("[still]");
            return IntUndefinedFalseUnion.FromFalse();
        }

        return MoveHit(target, pokemon, move);
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

        // Run TryHit events for field/side moves
        if (move.Target == MoveTarget.All && !isSelf)
        {
            RelayVar? hitResult = Battle.SingleEvent(EventId.TryHitField, move, Battle.InitEffectState(),
                SingleEventTarget.FromNullablePokemon(target), pokemon, move);
            
            if (hitResult is BoolRelayVar { Value: false })
            {
                if (Battle.PrintDebug)
                {
                    UiGenerator.PrintFailEvent(pokemon);
                }
                Battle.AttrLastMove("[still]");
                
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
                if (Battle.PrintDebug)
                {
                    UiGenerator.PrintFailEvent(pokemon);
                }
                Battle.AttrLastMove("[still]");
                
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
                if (Battle.PrintDebug)
                {
                    UiGenerator.PrintFailEvent(pokemon);
                }
                Battle.AttrLastMove("[still]");
                
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
        if (move.Self != null && move.SelfDropped != true)
        {
            SelfDrops(targets, pokemon, move, move, isSecondary);
        }

        // 5. secondary effects
        if (move.Secondaries != null)
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

            if (result is not BoolIntUndefinedUnionRelayVar biuu)
            {
                throw new InvalidOperationException("RelayVar must be type BoolIntUndefinedUnionRelayVar here.");
            }
            damage[i] = biuu.Value;
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
                        if (Battle.PrintDebug)
                        {
                            UiGenerator.PrintFailEvent(source);
                        }
                        Battle.AttrLastMove("[still]");
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

        return damage;
    }

    //runMoveEffects(
    //    damage: SpreadMoveDamage, targets: SpreadMoveTargets, source: Pokemon,
    //    move: ActiveMove, moveData: ActiveMove, isSecondary?: boolean, isSelf?: boolean

    //)
    //{
    //    let didAnything: number | boolean | null | undefined = damage.reduce(this.combineResults);
    //    for (const [i, target] of targets.entries()) {
    //        if (target === false) continue;
    //        let hitResult;
    //        let didSomething: number | boolean | null | undefined = undefined;

    //        if (target)
    //        {
    //            if (moveData.boosts && !target.fainted)
    //            {
    //                hitResult = this.battle.boost(moveData.boosts, target, source, move, isSecondary, isSelf);
    //                didSomething = this.combineResults(didSomething, hitResult);
    //            }
    //            if (moveData.heal && !target.fainted)
    //            {
    //                if (target.hp >= target.maxhp)
    //                {
    //                    this.battle.add('-fail', target, 'heal');
    //                    this.battle.attrLastMove('[still]');
    //                    damage[i] = this.combineResults(damage[i], false);
    //                    didAnything = this.combineResults(didAnything, null);
    //                    continue;
    //                }
    //                const amount = target.baseMaxhp * moveData.heal[0] / moveData.heal[1];
    //                const d = this.battle.heal((this.battle.gen < 5 ? Math.floor : Math.round)(amount), target, source, move);
    //                if (!d && d !== 0)
    //                {
    //                    if (d !== null)
    //                    {
    //                        this.battle.add('-fail', source);
    //                        this.battle.attrLastMove('[still]');
    //                    }
    //                    this.battle.debug('heal interrupted');
    //                    damage[i] = this.combineResults(damage[i], false);
    //                    didAnything = this.combineResults(didAnything, null);
    //                    continue;
    //                }
    //                didSomething = true;
    //            }
    //            if (moveData.status)
    //            {
    //                hitResult = target.trySetStatus(moveData.status, source, moveData.ability ? moveData.ability : move);
    //                if (!hitResult && move.status)
    //                {
    //                    damage[i] = this.combineResults(damage[i], false);
    //                    didAnything = this.combineResults(didAnything, null);
    //                    continue;
    //                }
    //                didSomething = this.combineResults(didSomething, hitResult);
    //            }
    //            if (moveData.forceStatus)
    //            {
    //                hitResult = target.setStatus(moveData.forceStatus, source, move);
    //                didSomething = this.combineResults(didSomething, hitResult);
    //            }
    //            if (moveData.volatileStatus)
    //            {
    //                hitResult = target.addVolatile(moveData.volatileStatus, source, move);
    //                didSomething = this.combineResults(didSomething, hitResult);
    //            }
    //            if (moveData.sideCondition)
    //            {
    //                hitResult = target.side.addSideCondition(moveData.sideCondition, source, move);
    //                didSomething = this.combineResults(didSomething, hitResult);
    //            }
    //            if (moveData.slotCondition)
    //            {
    //                hitResult = target.side.addSlotCondition(target, moveData.slotCondition, source, move);
    //                didSomething = this.combineResults(didSomething, hitResult);
    //            }
    //            if (moveData.weather)
    //            {
    //                hitResult = this.battle.field.setWeather(moveData.weather, source, move);
    //                didSomething = this.combineResults(didSomething, hitResult);
    //            }
    //            if (moveData.terrain)
    //            {
    //                hitResult = this.battle.field.setTerrain(moveData.terrain, source, move);
    //                didSomething = this.combineResults(didSomething, hitResult);
    //            }
    //            if (moveData.pseudoWeather)
    //            {
    //                hitResult = this.battle.field.addPseudoWeather(moveData.pseudoWeather, source, move);
    //                didSomething = this.combineResults(didSomething, hitResult);
    //            }
    //            if (moveData.forceSwitch)
    //            {
    //                hitResult = !!this.battle.canSwitch(target.side);
    //                didSomething = this.combineResults(didSomething, hitResult);
    //            }
    //            // Hit events
    //            //   These are like the TryHit events, except we don't need a FieldHit event.
    //            //   Scroll up for the TryHit event documentation, and just ignore the "Try" part. ;)
    //            if (move.target === 'all' && !isSelf)
    //            {
    //                if (moveData.onHitField)
    //                {
    //                    hitResult = this.battle.singleEvent('HitField', moveData, { }, target, source, move);
    //                    didSomething = this.combineResults(didSomething, hitResult);
    //                }
    //            }
    //            else if ((move.target === 'foeSide' || move.target === 'allySide') && !isSelf)
    //            {
    //                if (moveData.onHitSide)
    //                {
    //                    hitResult = this.battle.singleEvent('HitSide', moveData, { }, target.side, source, move);
    //                    didSomething = this.combineResults(didSomething, hitResult);
    //                }
    //            }
    //            else
    //            {
    //                if (moveData.onHit)
    //                {
    //                    hitResult = this.battle.singleEvent('Hit', moveData, { }, target, source, move);
    //                    didSomething = this.combineResults(didSomething, hitResult);
    //                }
    //                if (!isSelf && !isSecondary)
    //                {
    //                    this.battle.runEvent('Hit', target, source, move);
    //                }
    //            }
    //        }
    //        if (moveData.selfdestruct === 'ifHit' && damage[i] !== false)
    //        {
    //            this.battle.faint(source, source, move);
    //        }
    //        if (moveData.selfSwitch)
    //        {
    //            if (this.battle.canSwitch(source.side) && !source.volatiles['commanded'])
    //            {
    //                didSomething = true;
    //            }
    //            else
    //            {
    //                didSomething = this.combineResults(didSomething, false);
    //            }
    //        }
    //        // Move didn't fail because it didn't try to do anything
    //        if (didSomething === undefined) didSomething = true;
    //        damage[i] = this.combineResults(damage[i], didSomething === null ? false : didSomething);
    //        didAnything = this.combineResults(didAnything, didSomething);
    //    }

    //    if (!didAnything && didAnything !== 0 && !moveData.self && !moveData.selfdestruct)
    //    {
    //        if (!isSelf && !isSecondary)
    //        {
    //            if (didAnything === false)
    //            {
    //                this.battle.add('-fail', source);
    //                this.battle.attrLastMove('[still]');
    //            }
    //        }
    //        this.battle.debug('move failed because it did nothing');
    //    }
    //    else if (move.selfSwitch && source.hp && !source.volatiles['commanded'])
    //    {
    //        source.switchFlag = move.id;
    //    }

    //    return damage;
    //}

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
            if (moveData.HitEffect?.Boosts != null && !target.Fainted)
            {
                BoolZeroUnion? boostResult = Battle.Boost(moveData.HitEffect.Boosts, target, source, move, isSecondary, isSelf);
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
                    UiGenerator.PrintFailEvent(target, "heal");
                    Battle.AttrLastMove("[still]");
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
                        UiGenerator.PrintFailEvent(source);
                        Battle.AttrLastMove("[still]");
                    }
                    Battle.Debug("heal interrupted");
                    damage[i] = CombineResults(damage[i], BoolIntUndefinedUnion.FromBool(false));
                    didAnything = CombineResults(didAnything, null);
                    continue;
                }

                didSomething = BoolIntUndefinedUnion.FromBool(true);
            }

            // Try to apply status
            if (moveData.Status != null)
            {
                bool statusResult = target.TrySetStatus(moveData.Status.Value, source,
                    moveData.Ability != null ? Library.Abilities[moveData.Ability.Id] : move);
                hitResult = BoolIntUndefinedUnion.FromBool(statusResult);

                if (!statusResult && move.Status != null)
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
            if (moveData.VolatileStatus != null)
            {
                RelayVar volatileResult = target.AddVolatile(moveData.VolatileStatus.Value, source, move);
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
                        _ => BoolIntUndefinedUnion.FromUndefined()
                    };
                    didSomething = CombineResults(didSomething, hitResult);
                }
            }
            else if ((move.Target == MoveTarget.FoeSide || move.Target == MoveTarget.AllySide) && !isSelf)
            {
                if (moveData.OnHitSide != null)
                {
                    RelayVar? sideHitResult = Battle.SingleEvent(EventId.HitSide, moveData, null,
                        target.Side, source, move);
                    hitResult = sideHitResult switch
                    {
                        BoolRelayVar brv => BoolIntUndefinedUnion.FromBool(brv.Value),
                        IntRelayVar irv => BoolIntUndefinedUnion.FromInt(irv.Value),
                        _ => BoolIntUndefinedUnion.FromUndefined()
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

            // Handle self-switch
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

            damage[i] = CombineResults(damage[i], didSomething);
            didAnything = CombineResults(didAnything, didSomething);
        }

        // Check if move failed completely
        if (didAnything is not (IntBoolIntUndefinedUnion { Value: 0 } or IntBoolIntUndefinedUnion) &&
            moveData.Self == null && moveData.SelfDestruct == null)
        {
            if (!isSelf && !isSecondary)
            {
                if (didAnything is BoolBoolIntUndefinedUnion { Value: false })
                {
                    UiGenerator.PrintFailEvent(source);
                    Battle.AttrLastMove("[still]");
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
        foreach (PokemonFalseUnion targetUnion in targets)
        {
            if (targetUnion is not PokemonPokemonUnion)
            {
                continue;
            }

            if (moveData.Self != null && move.SelfDropped != true)
            {
                if (!isSecondary && moveData.Self.Boosts != null)
                {
                    int secondaryRoll = Battle.Random(100);
                    if (moveData.Self.Chance == null || secondaryRoll < moveData.Self.Chance)
                    {
                        MoveHit(source, source, move, moveData.Self, isSecondary, true);
                    }
                    if (move.MultiHit == null)
                    {
                        move.SelfDropped = true;
                    }
                }
                else
                {
                    MoveHit(source, source, move, moveData.Self, isSecondary, true);
                }
            }
        }
    }

    public void Secondaries(SpreadMoveTargets targets, Pokemon source, ActiveMove move, ActiveMove moveData,
        bool isSelf = false)
    {
        if (moveData.Secondaries == null) return;

        foreach (PokemonFalseUnion targetUnion in targets)
        {
            if (targetUnion is not PokemonPokemonUnion pokemonUnion)
            {
                continue;
            }

            Pokemon target = pokemonUnion.Pokemon;

            // Run ModifySecondaries event to get the list of secondary effects
            RelayVar? modifyResult = Battle.RunEvent(EventId.ModifySecondaries, target, source, moveData,
                moveData.Secondaries);

            var secondaries = modifyResult is SecondaryEffectArrayRelayVar secListRv
                ? secListRv.Effects
                : moveData.Secondaries;

            foreach (SecondaryEffect secondary in secondaries)
            {
                int secondaryRoll = Battle.Random(100);

                // User stat boosts or target stat drops can possibly overflow if it goes beyond 256 in Gen 8 or prior
                bool secondaryOverflow = (secondary.Boosts != null || secondary.Self != null) && Battle.Gen <= 8;

                int effectiveChance = secondary.Chance ?? 100;
                if (secondaryOverflow)
                {
                    effectiveChance %= 256;
                }

                if (secondary.Chance == null || secondaryRoll < effectiveChance)
                {
                    MoveHit(target, source, move, secondary, true, isSelf);
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
                RelayVar? hitResult = Battle.RunEvent(EventId.DragOut, target, source, move);

                switch (hitResult)
                {
                    case BoolRelayVar { Value: true } or null:
                        target.ForceSwitchFlag = true;
                        break;
                    case BoolRelayVar { Value: false } when move.Category == MoveCategory.Status:
                        UiGenerator.PrintFailEvent(source);
                        Battle.AttrLastMove("[still]");
                        damage[i] = BoolIntUndefinedUnion.FromBool(false);
                        break;
                }
            }
        }

        return damage;
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

    public int CalcRecoilDamage(int damageDealt, Move move, Pokemon pokemon)
    {
        // Chloroblast is a special case - returns 50% of max HP as recoil
        if (move.Id == MoveId.Chloroblast)
        {
            return (int)Math.Round(pokemon.MaxHp / 2.0);
        }

        // Standard recoil calculation: damageDealt * recoil[0] / recoil[1]
        // Clamped to minimum of 1
        if (move.Recoil == null) return 0;

        int recoilDamage = (int)Math.Round(damageDealt * move.Recoil.Value.Item1 /
                                           (double)move.Recoil.Value.Item2);
        return Battle.ClampIntRange(recoilDamage, 1, null);

    }

    public bool TargetTypeChoices(MoveTarget type)
    {
        return _choosableTargets.Contains(type);
    }

    /// <summary>
    /// Combines two move result values based on priority.
    /// Used to aggregate results across multiple targets.
    /// Priority order (highest to lowest): undefined, string (success), null, boolean, number.
    /// When both values are numbers, they are summed.
    /// </summary>
    /// <param name="left">First result value</param>
    /// <param name="right">Second result value</param>
    /// <returns>Combined result with the higher priority, or sum if both are numbers</returns>
    public static BoolIntUndefinedUnion CombineResults(
        BoolIntUndefinedUnion? left,
        BoolIntUndefinedUnion? right)
    {
        // Handle null inputs
        if (left == null && right == null) return BoolIntUndefinedUnion.FromUndefined();
        if (left == null) return right!;
        if (right == null) return left;

        int leftPriority = GetPriority(left);
        int rightPriority = GetPriority(right);

        // If left has higher priority, return it
        if (leftPriority < rightPriority)
        {
            return left;
        }

        // If left is truthy and right is falsy (but not 0)
        if (left.IsTruthy() && !right.IsTruthy() && !right.IsTruthy())
        {
            return left;
        }

        // If both are numbers, sum them
        if (left is IntBoolIntUndefinedUnion leftInt &&
            right is IntBoolIntUndefinedUnion rightInt)
        {
            return BoolIntUndefinedUnion.FromInt(leftInt.Value + rightInt.Value);
        }

        // Otherwise return right
        return right;

        // Priority mapping (lower number = higher priority)
        int GetPriority(BoolIntUndefinedUnion value)
        {
            return value switch
            {
                UndefinedBoolIntUndefinedUnion => 0,        // undefined (highest)
                // string/"NOT_FAILURE" case not in our union, would be priority 1
                // null case not in our union, would be priority 2
                BoolBoolIntUndefinedUnion => 3,              // boolean
                IntBoolIntUndefinedUnion => 4,               // number (lowest)
                _ => 5,
            };
        }
    }

    public IntUndefinedFalseUnion? GetDamage(Pokemon source, Pokemon target, ActiveMove move,
        bool suppressMessages = false)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion? GetDamage(Pokemon source, Pokemon target, MoveId move,
        bool suppressMessages = false)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion? GetDamage(Pokemon source, Pokemon target, int move,
        bool suppressMessages = false)
    {
        throw new NotImplementedException();
    }

    public int ModifyDamage(int baseDamage, Pokemon pokemon, Pokemon target, ActiveMove move,
        bool suppressMessages = false)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Calculates confusion self-hit damage.
    /// 
    /// Confusion damage is unique - most typical modifiers that get run when calculating
    /// damage (e.g. Huge Power, Life Orb, critical hits) don't apply. It also uses a 16-bit
    /// context for its damage, unlike the regular damage formula (though this only comes up
    /// for base damage).
    /// </summary>
    /// <param name="pokemon">The confused Pokémon hitting itself</param>
    /// <param name="basePower">Base power of the confusion damage (typically 40)</param>
    /// <returns>The calculated damage amount (minimum 1)</returns>
    public int GetConfusionDamage(Pokemon pokemon, int basePower)
    {
        // Get the Pokémon's attack and defense stats with current boosts applied
        int attack = pokemon.CalculateStat(StatIdExceptHp.Atk, pokemon.Boosts.GetBoost(BoostId.Atk));
        int defense = pokemon.CalculateStat(StatIdExceptHp.Def, pokemon.Boosts.GetBoost(BoostId.Def));
        int level = pokemon.Level;

        // Calculate base damage using the standard Pokémon damage formula
        // Formula: ((2 * level / 5 + 2) * basePower * attack / defense) / 50 + 2
        // Each step is truncated to match game behavior
        int baseDamage = Battle.Trunc(
            Battle.Trunc(
                Battle.Trunc(
                    Battle.Trunc(2 * level / 5 + 2) * basePower * attack
                ) / defense
            ) / 50
        ) + 2;

        // Apply 16-bit truncation for confusion damage
        // This only matters for extremely high damage values (Eternatus-Eternamax level stats)
        int damage = Battle.Trunc(baseDamage, 16);

        // Apply random damage variance (85-100% of calculated damage)
        damage = Battle.Randomizer(damage);

        // Ensure at least 1 damage is dealt
        return Math.Max(1, damage);
    }

    #endregion

    #region Terastallization

    public MoveType? CanTerastallize(IBattle battle, Pokemon pokemon)
    {
        if (Battle.Gen != 9)
        {
            return null;
        }
        return pokemon.TeraType;
    }

    public void Terastallize(Pokemon pokemon)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Helpers

    private IntUndefinedFalseUnion ExecuteMoveHit(List<Pokemon> targets, Pokemon pokemon, ActiveMove move,
        HitEffect? moveData = null, bool isSecondary = false, bool isSelf = false)
    {
        (SpreadMoveDamage damage, _) = SpreadMoveHit(
            SpreadMoveTargets.FromPokemonList(targets),
            pokemon, move, moveData, isSecondary, isSelf);

        if (damage.Count == 0)
        {
            return IntUndefinedFalseUnion.FromFalse();
        }

        BoolIntUndefinedUnion retVal = damage[0];

        return retVal switch
        {
            BoolBoolIntUndefinedUnion { Value: true } => new Undefined(),
            IntBoolIntUndefinedUnion intVal => intVal.Value,
            UndefinedBoolIntUndefinedUnion => new Undefined(),
            _ => IntUndefinedFalseUnion.FromFalse()
        };
    }

    private void ClearActiveMove(bool failed)
    {
        Battle.ActiveMove = null;
        if (failed)
        {
            Battle.ActiveTarget = null;
        }
    }

    #endregion
}