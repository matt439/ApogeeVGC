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

public class BattleActions(IBattle battle)
{
    public IBattle Battle { get; init; } = battle;
    public Library Library => Battle.Library;

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
        Pokemon? target = GetTarget(pokemon, move, targetLoc, originalTarget);
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

        SetActiveMove(activeMove, pokemon, target);

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

    private Pokemon? GetTarget(Pokemon pokemon, Move move, int targetLoc, Pokemon? originalTarget)
    {
        // This method needs to be implemented based on your battle system
        // For now, returning the original target or getting the Pokemon at the target location
        if (originalTarget != null)
        {
            return originalTarget;
        }

        return pokemon.GetAtLoc(targetLoc);
    }

    private void SetActiveMove(ActiveMove move, Pokemon pokemon, Pokemon? target)
    {
        Battle.ActiveMove = move;
        Battle.ActiveTarget = target;
    }

    private void ClearActiveMove(bool failed)
    {
        Battle.ActiveMove = null;
        if (failed)
        {
            Battle.ActiveTarget = null;
        }
    }

    public record UseMoveOptions
    {
        public Pokemon? Target { get; init; }
        public IEffect? SourceEffect { get; init; }
    }

    public bool UseMove(MoveId moveId, Pokemon pokemon, UseMoveOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public bool UseMove(Move move, Pokemon pokemon, UseMoveOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public void UseMoveInner(MoveId moveId, Pokemon pokemon, UseMoveOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public void UseMoveInner(Move move, Pokemon pokemon, UseMoveOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public bool TrySpreadMoveHit(List<Pokemon> targets, Pokemon pokemon, ActiveMove move, bool notActive = false)
    {
        throw new NotImplementedException();
    }

    public List<RelayVar> HitStepInvulnerabilityEvent(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public List<RelayVar> HitStepTryEvent(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public List<bool> HitStepTypeImmunity(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public List<bool> HitStepTryImmunity(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public List<bool> HitStepAccuracy(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public Undefined HitStepBreakProtect(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public Undefined HitStepStealProtect(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public Undefined AfterMoveSecondaryEvent(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion TryMoveHit(Pokemon target, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion TryMoveHit(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public List<BoolIntUndefinedUnion> HitStepMoveHitLoop(List<Pokemon> targets, Pokemon pokemon, ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public (SpreadMoveDamage, SpreadMoveTargets) SpreadMoveHit(SpreadMoveTargets targets, Pokemon pokemon,
        ActiveMove move, HitEffect? hitEffect = null, bool isSecondary = false, bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public SpreadMoveDamage TryPrimaryHitEvent(SpreadMoveDamage damage, SpreadMoveTargets targets,
        Pokemon pokemon, ActiveMove move, ActiveMove moveData, bool isSecondary = false)
    {
        throw new NotImplementedException();
    }

    public SpreadMoveDamage GetSpreadDamage(SpreadMoveDamage damage, SpreadMoveTargets targets, Pokemon source,
        ActiveMove move, ActiveMove moveData, bool isSecondary = false, bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public SpreadMoveDamage RunMoveEffects(SpreadMoveDamage damage, SpreadMoveTargets targets,
        Pokemon source, ActiveMove move, ActiveMove moveData, bool isSecondary = false, bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public void SelfDrops(SpreadMoveTargets targets, Pokemon source, ActiveMove move, ActiveMove moveData,
        bool isSecondary = false)
    {
        throw new NotImplementedException();
    }

    public void Secondaries(SpreadMoveTargets targets, Pokemon source, ActiveMove move, ActiveMove moveData,
        bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public SpreadMoveDamage ForceSwitch(SpreadMoveDamage damage, SpreadMoveTargets targets, Pokemon source,
        ActiveMove move)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion MoveHit(Pokemon? target, Pokemon pokemon, ActiveMove move,
        HitEffect? moveData, bool isSecondary = false, bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public IntUndefinedFalseUnion MoveHit(List<Pokemon?> targets, Pokemon pokemon, ActiveMove move,
        HitEffect? moveData, bool isSecondary = false, bool isSelf = false)
    {
        throw new NotImplementedException();
    }

    public int CalcRecoilDamage(int damageDealt, Move move, Pokemon pokemon)
    {
        throw new NotImplementedException();
    }

    public bool TargetTypeChoices(MoveTarget type)
    {
        throw new NotImplementedException();
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
}