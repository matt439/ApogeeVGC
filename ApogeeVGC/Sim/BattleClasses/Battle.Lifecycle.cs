using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    public void Start()
    {
        // Need all players to start
        if (!Sides.All(_ => true))
        {
            throw new InvalidOperationException($"Missing sides.");
        }

        if (Started)
        {
            throw new InvalidOperationException("Battle already started");
        }

        Started = true;

        // Set up foe relationships (standard 1v1 or 2v2)
        Sides[1].Foe = Sides[0];
        Sides[0].Foe = Sides[1];

        // If there are more than 2 sides (FFA - free-for-all)
        if (Sides.Count > 2)
        {
            Sides[2].Foe = Sides[3];
            Sides[3].Foe = Sides[2];
        }

        if (DisplayUi)
        {
            // Log generation
            Add("gen", Gen);

            // Log tier
            Add("tier", Format.Name);

            // Log rated status
            if (Rated)
            {
                string ratedMessage = Rated ? string.Empty : Rated.ToString();
                Add("rated", ratedMessage);
            }
        }

        // Call format's OnBegin handler
        Format.OnBegin?.Invoke(this);

        // Call OnBegin for each rule in the rule table
        foreach (Format subFormat in from rule in RuleTable.Keys
                 let ruleString = rule.ToString()
                 where ruleString.Length <= 0 || !"+*-!".Contains(ruleString[0])
                 select Library.Rulesets[rule])
        {
            subFormat.OnBegin?.Invoke(this);
        }

        // Validate that all sides have at least one Pokemon
        if (Sides.Any(side => side.Pokemon.Count == 0))
        {
            throw new InvalidOperationException("Battle not started: A player has an empty team.");
        }

        // Check EV balance in debug mode
        CheckEvBalance();

        // Add start action to queue BEFORE team preview
        // This ensures it's preserved when CommitChoices processes team preview actions
        // In synchronous mode, RequestPlayerChoices triggers immediate processing,
        // so the StartGameAction must be in the queue before RunPickTeam is called
        Debug("[Battle.Start] Adding StartGameAction to queue");
        Queue.InsertChoice(new StartGameAction());
        Debug($"[Battle.Start] StartGameAction added, queue size = {Queue.List.Count}");

        // Run team preview/selection phase
        Debug($"[Battle.Start] About to call RunPickTeam(), RequestState = {RequestState}");

        RunPickTeam();

        Debug($"[Battle.Start] RunPickTeam() returned, RequestState = {RequestState}");

        // Set mid-turn flag
        MidTurn = true;

        // Start turn loop if no request is pending
        Debug($"[Battle.Start] Checking RequestState: {RequestState}");

        if (RequestState == RequestState.None)
        {
            Debug("[Battle.Start] No request - calling TurnLoop()");
            TurnLoop();
        }
        else
        {
            Debug(
                $"[Battle.Start] Request pending ({RequestState}) - returning, waiting for choices");
        }

        // Return immediately - Battle doesn't wait for choices
        // Simulator will call Choose() which will call CommitChoices() -> TurnLoop()
    }

    public void EndTurn()
    {
        // Increment turn counter and reset last successful move
        Turn++;
        LastSuccessfulMoveThisTurn = null;

        // Record turn start in history
        History.RecordTurnStart(Turn);

        // Process each side
        var trappedBySide = new List<bool>();
        var stalenessBySide = new List<StalenessId?>();

        foreach (Side side in Sides)
        {
            bool sideTrapped = true;
            StalenessId? sideStaleness = null;

            foreach (Pokemon? pokemon in side.Active)
            {
                if (pokemon == null) continue;

                // Reset move tracking
                pokemon.MoveThisTurn = false;
                pokemon.NewlySwitched = false;
                pokemon.MoveLastTurnResult = pokemon.MoveThisTurnResult;
                pokemon.MoveThisTurnResult = null;

                // Reset turn-specific flags (except on turn 1)
                if (Turn != 1)
                {
                    pokemon.UsedItemThisTurn = false;
                    pokemon.StatsRaisedThisTurn = false;
                    pokemon.StatsLoweredThisTurn = false;
                    pokemon.HurtThisTurn = null;
                }

                // Reset move disable tracking
                pokemon.MaybeDisabled = false;
                pokemon.MaybeLocked = null;

                // Clear disabled flags on all move slots
                foreach (MoveSlot moveSlot in pokemon.MoveSlots)
                {
                    moveSlot.Disabled = false;
                    moveSlot.DisabledSource = null;
                }

                // Run DisableMove event to determine which moves should be disabled
                RunEvent(EventId.DisableMove, pokemon);

                // Check each move for specific disable conditions
                foreach (MoveSlot moveSlot in pokemon.MoveSlots)
                {
                    var activeMove = Library.Moves[moveSlot.Id].ToActiveMove();

                    // Run DisableMove event on the specific move
                    SingleEvent(EventId.DisableMove, activeMove, null, pokemon);

                    // Disable moves with "cantusetwice" flag if used last turn
                    if (activeMove.Flags.CantUseTwice == true &&
                        pokemon.LastMove?.Id == moveSlot.Id)
                    {
                        pokemon.DisableMove(pokemon.LastMove.Id);
                    }
                }

                // If it was an illusion, it's not any more (Gen 7+)
                if (pokemon.GetLastAttackedBy() != null)
                {
                    pokemon.KnownType = true;
                }

                // Clean up attack tracking
                for (int i = pokemon.AttackedBy.Count - 1; i >= 0; i--)
                {
                    Attacker attack = pokemon.AttackedBy[i];
                    if (attack.Source.IsActive)
                    {
                        // Mark attack as not from this turn
                        pokemon.AttackedBy[i] = attack with { ThisTurn = false };
                    }
                    else
                    {
                        // Remove attacks from Pokemon that are no longer active
                        pokemon.AttackedBy.RemoveAt(i);
                    }
                }

                // Update apparent type display (not Terastallized)
                if (pokemon.Terastallized == null)
                {
                    // Get the visible Pokemon (accounting for Illusion)
                    Pokemon seenPokemon = pokemon.Illusion ?? pokemon;

                    // Get actual types as a string
                    string realTypeString = string.Join("/",
                        seenPokemon.GetTypes(excludeAdded: true).Select(t => t.ToString()));

                    string currentApparentType = string.Join("/", seenPokemon.ApparentType);
                    if (realTypeString != currentApparentType)
                    {
                        if (DisplayUi)
                        {
                            Add("-start", pokemon, "typechange", realTypeString, "[silent]");
                        }

                        seenPokemon.ApparentType =
                            seenPokemon.GetTypes(excludeAdded: true).ToList();

                        if (pokemon.AddedType != null)
                        {
                            if (DisplayUi)
                            {
                                Add("-start", pokemon, "typeadd", pokemon.AddedType?.ToString() ??
                                    throw new InvalidOperationException(
                                        "Added type should not be null"),
                                    "[silent]");
                            }
                        }
                    }
                }

                // Reset trapping status
                pokemon.Trapped = PokemonTrapped.False;
                pokemon.MaybeTrapped = false;

                // Run trap events
                RunEvent(EventId.TrapPokemon, pokemon);

                if (pokemon.KnownType || Dex.GetImmunity(ConditionId.Trapped, pokemon.Types))
                {
                    RunEvent(EventId.MaybeTrapPokemon, pokemon);
                }

                // Check foe abilities for potential trapping
                foreach (Pokemon source in pokemon.Foes())
                {
                    Species species = (source.Illusion ?? source).Species;

                    foreach (SpeciesAbilityType abilitySlot in Enum.GetValues<SpeciesAbilityType>())
                    {
                        var abilityId = species.Abilities.GetAbility(abilitySlot);
                        if (abilityId == null) continue;
                        if (abilityId == source.Ability) continue;

                        // TODO: log or do something if key not found in library
                        if (!Library.Abilities.TryGetValue(abilityId.Value, out Ability? ability))
                        {
                            continue;
                        }

                        if (RuleTable.Has(ability.Id)) continue;

                        if (pokemon.KnownType &&
                            !Dex.GetImmunity(ConditionId.Trapped, pokemon.Types))
                            continue;

                        SingleEvent(EventId.FoeMaybeTrapPokemon, ability, null, pokemon, source);
                    }
                }

                if (pokemon.Fainted) continue;

                // Update side-wide trap status
                sideTrapped = sideTrapped && pokemon.Trapped == PokemonTrapped.True;

                // Update side-wide staleness
                StalenessId? staleness = pokemon.VolatileStaleness ?? pokemon.Staleness;
                if (staleness != null)
                {
                    sideStaleness = sideStaleness == StalenessId.External
                        ? sideStaleness
                        : staleness;
                }

                pokemon.ActiveTurns++;
            }

            trappedBySide.Add(sideTrapped);
            stalenessBySide.Add(sideStaleness);

            side.FaintedLastTurn = side.FaintedThisTurn;
            side.FaintedThisTurn = null;
        }

        // Check for endless battle clause
        if (MaybeTriggerEndlessBattleClause(trappedBySide, stalenessBySide))
        {
            return;
        }

        if (Ended)
        {
            return;
        }

        // Display turn number
        if (DisplayUi)
        {
            Add("turn", Turn);
        }

        // Request move choices for the new turn
        MakeRequest(RequestState.Move);

        // Return immediately - Battle doesn't wait for choices
        // The caller (CommitChoices or Start) will call RequestPlayerChoices after TurnLoop returns
        // This avoids infinite recursion in synchronous mode
    }

    /// <summary>
    /// Generally called at the beginning of a turn, to go through the
    /// turn one action at a time.
    /// 
    /// If there is a mid-turn decision (like U-Turn), this will return
    /// and be called again later to resume the turn.
    /// </summary>
    public void TurnLoop()
    {
        Debug(
            $"[TurnLoop] STARTING - Queue size: {Queue.List.Count}, MidTurn: {MidTurn}, RequestState: {RequestState}");

        if (DisplayUi)
        {
            Add(string.Empty);
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Add("t:", timestamp);
        }

        if (RequestState != RequestState.None)
        {
            RequestState = RequestState.None;
        }

        // First time through - set up turn structure
        if (!MidTurn)
        {
            Debug(
                "[TurnLoop] First time through, adding BeforeTurnAction and ResidualAction");

            Queue.InsertChoice(new BeforeTurnAction());
            Queue.AddChoice(new ResidualAction());
            MidTurn = true;
        }

        Debug($"[TurnLoop] About to process queue - size: {Queue.List.Count}");

        // Process actions one at a time
        int actionCount = 0;
        while (Queue.Shift() is { } action)
        {
            actionCount++;
            Debug($"[TurnLoop] Processing action {actionCount}: {action.Choice}");

            RunAction(action);

            // Exit early if we need to wait for a request or battle ended
            // Battle returns here, Simulator will call us back when choices are made
            if (RequestState != RequestState.None || Ended)
            {
                Debug($"[TurnLoop] Exiting early - RequestState: {RequestState}, Ended: {Ended}");
                return;
            }
        }

        Debug($"[TurnLoop] Queue empty after processing {actionCount} actions");

        // Turn is complete - reset flags and start next turn
        MidTurn = false;
        Queue.Clear();

        Debug("[TurnLoop] Calling EndTurn");

        EndTurn();
    }

    public bool RunAction(IAction action)
    {
        int? pokemonOriginalHp = action switch
        {
            PokemonAction pa => pa.Pokemon.Hp,
            MoveAction ma => ma.Pokemon.Hp,
            SwitchAction sa => sa.Pokemon.Hp,
            _ => null,
        };

        List<(Pokemon pokemon, int hp)> residualPokemon = [];

        // Returns whether or not we ended in a callback
        switch (action.Choice)
        {
            case ActionId.Start:
            {
                foreach (Side side in Sides)
                {
                    if (side.PokemonLeft > 0)
                        side.PokemonLeft = side.Pokemon.Count;

                    if (DisplayUi)
                    {
                        Add("teamsize", side.Id.GetSideIdName(), side.Pokemon.Count.ToString());
                    }
                }

                if (DisplayUi)
                {
                    Add("start");
                }

                // Change Zacian/Zamazenta into their Crowned forme
                foreach (Pokemon pokemon in GetAllPokemon())
                {
                    Species? rawSpecies = null;
                    if (pokemon.Species.Id == SpecieId.Zacian && pokemon.Item == ItemId.RustedSword)
                    {
                        rawSpecies = Library.Species[SpecieId.ZacianCrowned];
                    }
                    else if (pokemon.Species.Id == SpecieId.Zamazenta &&
                             pokemon.Item == ItemId.RustedShield)
                    {
                        rawSpecies = Library.Species[SpecieId.ZamazentaCrowned];
                    }

                    if (rawSpecies == null) continue;

                    Species? species = pokemon.SetSpecie(rawSpecies, Effect);
                    if (species == null) continue;

                    pokemon.BaseSpecies = rawSpecies;
                    pokemon.Details = pokemon.GetUpdatedDetails();
                    pokemon.SetAbility(species.Abilities.GetAbility(SpeciesAbilityType.Slot0)
                                       ?? throw new InvalidOperationException(
                                           "Species has no ability in slot 0"),
                        isFromFormeChange: true);
                    pokemon.BaseAbility = pokemon.Ability;

                    // Replace Iron Head with Behemoth Blade/Bash
                    Dictionary<SpecieId, MoveId> behemothMoves = new()
                    {
                        { SpecieId.ZacianCrowned, MoveId.BehemothBlade },
                        { SpecieId.ZamazentaCrowned, MoveId.BehemothBash },
                    };

                    int ironHeadIndex = pokemon.BaseMoves.IndexOf(MoveId.IronHead);
                    if (ironHeadIndex >= 0)
                    {
                        Move move = Library.Moves[behemothMoves[rawSpecies.Id]];
                        pokemon.BaseMoveSlots[ironHeadIndex] = new MoveSlot
                        {
                            Move = move.Id,
                            Id = move.Id,
                            Pp = move.NoPpBoosts ? move.BasePp : move.BasePp * 8 / 5,
                            MaxPp = move.NoPpBoosts ? move.BasePp : move.BasePp * 8 / 5,
                            Target = move.Target,
                            Disabled = false,
                            DisabledSource = null,
                            Used = false,
                        };
                        pokemon.MoveSlots = [.. pokemon.BaseMoveSlots];
                    }
                }

                // Call format's OnBattleStart handler
                Format.OnBattleStart?.Invoke(this);

                foreach (RuleId rule in RuleTable.Keys)
                {
                    string ruleString = rule.ToString();
                    if (ruleString.Length > 0 && "+*-!".Contains(ruleString[0])) continue;
                    Format subFormat = Library.Rulesets[rule];
                    subFormat.OnBattleStart?.Invoke(this);
                }

                foreach (Side side in Sides)
                {
                    for (int i = 0; i < side.Active.Count; i++)
                    {
                        if (side.PokemonLeft <= 0)
                        {
                            // Forfeited before starting
                            side.Active[i] = side.Pokemon[i];
                            Pokemon assignedPokemon = side.Active[i]
                                                      ?? throw new InvalidOperationException(
                                                          $"Failed to assign Pokemon to Active slot {i} for {side.Name}");
                            assignedPokemon.Fainted = true;
                            assignedPokemon.Hp = 0;
                        }
                        else
                        {
                            Actions.SwitchIn(side.Pokemon[i], i);
                        }
                    }
                }

                foreach (Pokemon pokemon in GetAllPokemon())
                {
                    if (pokemon.Species.Conditon != ConditionId.None)
                    {
                        Condition speciesCondition = Library.Conditions[pokemon.Species.Conditon];
                        SingleEvent(EventId.Start, speciesCondition, pokemon.SpeciesState, pokemon);
                    }
                }

                // Messages will be sent when RequestPlayerChoices() is called
                // No need to send here - just accumulate them in the log

                MidTurn = true;
                break;
            }

            case ActionId.Move:
            {
                var moveAction = (MoveAction)action;
                if (!moveAction.Pokemon.IsActive) return false;
                if (moveAction.Pokemon.Fainted) return false;
                Actions.RunMove(moveAction.Move, moveAction.Pokemon, moveAction.TargetLoc,
                    new BattleActions.RunMoveOptions
                    {
                        SourceEffect = moveAction.SourceEffect,
                        OriginalTarget = moveAction.OriginalTarget,
                    });
                break;
            }

            case ActionId.Terastallize:
            {
                var teraAction = (PokemonAction)action;
                Actions.Terastallize(teraAction.Pokemon);
                break;
            }

            case ActionId.BeforeTurnMove:
            {
                var btmAction = (MoveAction)action;
                if (!btmAction.Pokemon.IsActive) return false;
                if (btmAction.Pokemon.Fainted) return false;

                if (DisplayUi)
                {
                    Debug($"before turn callback: {btmAction.Move.Id}");
                }

                Pokemon? target = GetTarget(btmAction.Pokemon, btmAction.Move, btmAction.TargetLoc);
                if (target == null) return false;
                if (btmAction.Move.BeforeTurnCallback == null)
                    throw new InvalidOperationException("beforeTurnMove has no beforeTurnCallback");
                this.InvokeCallback<object>(
                    btmAction.Move.BeforeTurnCallback,
                    this,
                    btmAction.Pokemon,
                    target,
                    btmAction.Move);
                break;
            }

            case ActionId.PriorityChargeMove:
            {
                var pcmAction = (MoveAction)action;
                if (!pcmAction.Pokemon.IsActive) return false;
                if (pcmAction.Pokemon.Fainted) return false;

                if (DisplayUi)
                {
                    Debug($"priority charge callback: {pcmAction.Move.Id}");
                }

                if (pcmAction.Move.PriorityChargeCallback != null)
                {
                    this.InvokeCallback<object>(
                        pcmAction.Move.PriorityChargeCallback,
                        this,
                        pcmAction.Pokemon);
                }

                break;
            }

            case ActionId.Event:
            {
                var eventAction = (PokemonAction)action;
                RunEvent(eventAction.Event ??
                         throw new InvalidOperationException("Event action must have an event"),
                    eventAction.Pokemon);
                break;
            }

            case ActionId.Team:
            {
                var teamAction = (TeamAction)action;
                if (teamAction.Index == 0)
                {
                    teamAction.Pokemon.Side.Pokemon = [];
                }

                teamAction.Pokemon.Side.Pokemon.Add(teamAction.Pokemon);
                teamAction.Pokemon.Position = teamAction.Index;
                return false;
            }

            case ActionId.Pass:
                return false;

            case ActionId.InstaSwitch:
            case ActionId.Switch:
            {
                var switchAction = (SwitchAction)action;
                if (switchAction.Choice == ActionId.Switch &&
                    switchAction.Pokemon.Status != ConditionId.None)
                {
                    Ability naturalCure = Library.Abilities[AbilityId.NaturalCure];
                    SingleEvent(EventId.CheckShow, naturalCure, null, switchAction.Pokemon);
                }

                Actions.SwitchIn(switchAction.Target, switchAction.Pokemon.Position,
                    switchAction.SourceEffect);
                break;
            }

            case ActionId.RevivalBlessing:
            {
                var rbAction = (SwitchAction)action;
                rbAction.Pokemon.Side.PokemonLeft++;
                if (rbAction.Target.Position < rbAction.Pokemon.Side.Active.Count)
                {
                    Queue.AddChoice(new SwitchAction
                    {
                        Choice = ActionId.InstaSwitch,
                        Pokemon = rbAction.Target,
                        Target = rbAction.Target,
                        Order = 3,
                    });
                }

                rbAction.Target.Fainted = false;
                rbAction.Target.FaintQueued = false;
                rbAction.Target.SubFainted = false;
                rbAction.Target.Status = ConditionId.None;
                rbAction.Target.Hp = 1;
                rbAction.Target.SetHp(rbAction.Target.MaxHp / 2);

                if (DisplayUi)
                {
                    Add("-heal", rbAction.Target, rbAction.Target.GetHealth,
                        "[from] move: Revival Blessing");
                }

                rbAction.Pokemon.Side.RemoveSlotCondition(rbAction.Pokemon,
                    ConditionId.RevivalBlessing);
                break;
            }

            case ActionId.RunSwitch:
            {
                var rsAction = (RunSwitchAction)action;
                Actions.RunSwitch(rsAction.Pokemon!);
                break;
            }

            case ActionId.Shift:
            {
                var shiftAction = (PokemonAction)action;
                if (!shiftAction.Pokemon.IsActive) return false;
                if (shiftAction.Pokemon.Fainted) return false;
                SwapPosition(shiftAction.Pokemon, 1);
                break;
            }

            case ActionId.BeforeTurn:
                EachEvent(EventId.BeforeTurn);
                break;

            case ActionId.Residual:
                if (DisplayUi)
                {
                    Add(string.Empty);
                }

                Debug($"[RunAction] About to call FieldEvent(Residual)");

                ClearActiveMove(failed: true);
                UpdateSpeed();
                residualPokemon = GetAllActive()
                    .Select(p => (p, p.GetUndynamaxedHp()))
                    .ToList();
                FieldEvent(EventId.Residual);

                Debug($"[RunAction] FieldEvent(Residual) returned");

                if (!Ended && DisplayUi)
                {
                    Add("upkeep");
                }

                break;
        }

        // Phazing (Roar, etc)
        foreach (Side side in Sides)
        {
            foreach (Pokemon? pokemon in side.Active)
            {
                if (pokemon == null) continue;

                if (pokemon.ForceSwitchFlag)
                {
                    if (pokemon.Hp > 0) Actions.DragIn(pokemon.Side, pokemon.Position);
                    pokemon.ForceSwitchFlag = false;
                }
            }
        }

        ClearActiveMove();

        // Fainting
        FaintMessages();
        if (Ended) return true;

        // Cancel queued actions for all fainted Pokemon
        foreach (Side side in Sides)
        {
            foreach (Pokemon? pokemon in side.Active)
            {
                if (pokemon?.Fainted == true)
                {
                    Queue.CancelAction(pokemon);
                }
            }
        }

        // Switching (fainted pokemon, U-turn, Baton Pass, etc)
        if (Queue.Peek()?.Choice == ActionId.InstaSwitch)
        {
            return false;
        }

        // Emergency Exit / Wimp Out check
        if (action.Choice != ActionId.Start)
        {
            EachEvent(EventId.Update);
            foreach ((Pokemon pokemon, int originalHp) in residualPokemon)
            {
                int maxHp = pokemon.GetUndynamaxedHp(pokemon.MaxHp);
                if (pokemon.Hp > 0 && pokemon.GetUndynamaxedHp() <= maxHp / 2 &&
                    originalHp > maxHp / 2)
                {
                    RunEvent(EventId.EmergencyExit, pokemon);
                }
            }
        }

        if (action.Choice == ActionId.RunSwitch)
        {
            var runSwitchAction = (RunSwitchAction)action;
            Pokemon pokemon = runSwitchAction.Pokemon!;
            if (pokemon.Hp > 0 && pokemon.Hp <= pokemon.MaxHp / 2 &&
                pokemonOriginalHp > pokemon.MaxHp / 2)
            {
                RunEvent(EventId.EmergencyExit, pokemon);
            }
        }

        // Check for switches
        var switches = Sides
            .Select(side => side.Active.Any(p => p != null && p.SwitchFlag.IsTrue()))
            .ToList();

        for (int i = 0; i < Sides.Count; i++)
        {
            bool reviveSwitch = false;
            if (switches[i] && CanSwitch(Sides[i]) == 0)
            {
                foreach (Pokemon? pokemon in Sides[i].Active)
                {
                    if (pokemon == null) continue;

                    IEffect? revivalBlessing = Sides[i].GetSlotCondition(pokemon.Position,
                        ConditionId.RevivalBlessing);
                    if (revivalBlessing != null)
                    {
                        reviveSwitch = true;
                        continue;
                    }

                    pokemon.SwitchFlag = false;
                }

                if (!reviveSwitch)
                {
                    switches[i] = false;

                    // If this side needs to switch but has no Pokemon available, they've lost
                    // Check if the battle should end
                    if (Sides[i].PokemonLeft <= 0)
                    {
                        Debug($"{Sides[i].Name} has no Pokemon left to switch in, losing");

                        Lose(Sides[i]);
                        return true;
                    }
                }
            }
            else if (switches[i])
            {
                foreach (Pokemon? pokemon in Sides[i].Active)
                {
                    if (pokemon == null) continue;

                    if (pokemon.Hp > 0 &&
                        pokemon.SwitchFlag.IsTrue() &&
                        pokemon.SwitchFlag != MoveId.RevivalBlessing &&
                        !pokemon.SkipBeforeSwitchOutEventFlag)
                    {
                        RunEvent(EventId.BeforeSwitchOut, pokemon);
                        pokemon.SkipBeforeSwitchOutEventFlag = true;
                        FaintMessages();
                        if (Ended) return true;
                        if (pokemon.Fainted)
                        {
                            switches[i] = Sides[i].Active
                                .Any(p => p != null && p.SwitchFlag.IsTrue());
                        }
                    }
                }
            }
        }

        foreach (bool playerSwitch in switches)
        {
            if (playerSwitch)
            {
                MakeRequest(RequestState.SwitchIn);

                // Return immediately - Simulator handles getting switch choices
                // When Simulator calls Choose() -> CommitChoices(), TurnLoop() will be called again
                return true;
            }
        }

        // In Gen 8+, speed is updated dynamically
        IAction? nextAction = Queue.Peek();
        if (nextAction?.Choice == ActionId.Move)
        {
            UpdateSpeed();
            foreach (IAction queueAction in Queue.List)
                GetActionSpeed(queueAction);

            Queue.Sort();
        }

        return false;
    }
}