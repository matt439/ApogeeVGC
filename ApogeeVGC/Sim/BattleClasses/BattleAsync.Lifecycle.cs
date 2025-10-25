using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;
using ApogeeVGC.Sim.SpeciesClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    public void Start()
    {
        // Deserialized games should use Restart()
        if (Deserialized) return;

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
        if (DebugMode)
        {
            CheckEvBalance();
        }

        // Run team preview/selection phase
        RunPickTeam();

        // Add start action to queue
        Queue.InsertChoice(new StartGameAction());

        // Set mid-turn flag
        MidTurn = true;

        // Start turn loop if no request is pending
        if (RequestState == RequestState.None)
        {
            TurnLoop();
        }
    }

    public void Restart(Action<string, List<string>>? send)
    {
        if (!Deserialized)
        {
            throw new InvalidOperationException("Attempt to restart a battle which has not been deserialized");
        }
        throw new Exception("Not sure what this is suppsed to do");
    }

    public void EndTurn()
    {
        // Increment turn counter and reset last successful move
        Turn++;
        LastSuccessfulMoveThisTurn = null;

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
                    // It shouldn't be possible in a normal battle for a Pokemon to be damaged before turn 1's move selection
                    // However, this could be potentially relevant in certain OMs
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
                        // Mark attack as not from this turn (create new record since it's immutable)
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
                    // In Gen 7+, the real type of every Pokemon is visible to all players via the bottom screen while making choices
                    // Get the visible Pokemon (accounting for Illusion)
                    Pokemon seenPokemon = pokemon.Illusion ?? pokemon;

                    // Get actual types as a string (e.g., "Fire/Flying")
                    string realTypeString = string.Join("/",
                        seenPokemon.GetTypes(excludeAdded: true).Select(t => t.ToString()));

                    string currentApparentType = string.Join("/", seenPokemon.ApparentType);
                    if (realTypeString != currentApparentType)
                    {
                        // Update apparent type (this is for display purposes)
                        if (DisplayUi)
                        {
                            Add("-start", pokemon, "typechange", realTypeString, "[silent]");
                        }
                        seenPokemon.ApparentType = seenPokemon.GetTypes(excludeAdded: true).ToList();

                        if (pokemon.AddedType != null)
                        {
                            // The typechange message removes the added type, so put it back
                            if (DisplayUi)
                            {
                                Add("-start", pokemon, "typeadd", pokemon.AddedType?.ToString() ??
                                    throw new InvalidOperationException("Added type should not be null"),
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

                // Canceling switches would leak information if a foe might have a trapping ability
                if (pokemon.KnownType || Dex.GetImmunity(ConditionId.Trapped, pokemon.Types))
                {
                    RunEvent(EventId.MaybeTrapPokemon, pokemon);
                }

                // Check foe abilities for potential trapping
                foreach (Pokemon source in pokemon.Foes())
                {
                    // Get the species to check (accounting for Illusion)
                    Species species = (source.Illusion ?? source).Species;

                    // Check each ability slot the species could have
                    foreach (SpeciesAbilityType abilitySlot in Enum.GetValues<SpeciesAbilityType>())
                    {
                        var abilityId = species.Abilities.GetAbility(abilitySlot);
                        if (abilityId == null) continue;

                        // Skip if this is the source's current ability (already checked above)
                        if (abilityId == source.Ability) continue;

                        // Get the ability
                        Ability ability = Library.Abilities[abilityId.Value];

                        // Check if ability is banned
                        if (RuleTable.Has(ability.Id)) continue;

                        // Skip immunity check if type is known and already not immune
                        if (pokemon.KnownType && !Dex.GetImmunity(ConditionId.Trapped, pokemon.Types))
                            continue;

                        // Run the FoeMaybeTrapPokemon event for this potential ability
                        SingleEvent(EventId.FoeMaybeTrapPokemon, ability, null, pokemon, source);
                    }
                }

                // Skip if Pokemon fainted
                if (pokemon.Fainted) continue;

                // Update side-wide trap status
                sideTrapped = sideTrapped && pokemon.Trapped == PokemonTrapped.True;

                // Update side-wide staleness
                StalenessId? staleness = pokemon.VolatileStaleness ?? pokemon.Staleness;
                if (staleness != null)
                {
                    // External staleness takes priority
                    sideStaleness = sideStaleness == StalenessId.External ? sideStaleness : staleness;
                }

                // Increment active turn counter
                pokemon.ActiveTurns++;
            }

            // Store trap and staleness status for this side
            trappedBySide.Add(sideTrapped);
            stalenessBySide.Add(sideStaleness);

            // Update fainted Pokemon tracking
            side.FaintedLastTurn = side.FaintedThisTurn;
            side.FaintedThisTurn = null;
        }

        // Check for endless battle clause
        if (MaybeTriggerEndlessBattleClause(trappedBySide, stalenessBySide))
        {
            return;
        }

        // Display turn number
        if (DisplayUi)
        {
            Add("turn", Turn);
        }

        // Pre-calculate Quick Claw roll for Gen 2-3 (skipped for Gen 9)
        // Gen 9 doesn't use Quick Claw rolls the same way

        // Request move choices for the new turn
        MakeRequest(RequestState.Move);
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
        if (DisplayUi)
        {
            // Add empty line for formatting
            Add(string.Empty);

            // Add timestamp in Unix epoch seconds
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Add("t:", timestamp);
        }

        // Clear request state if it exists
        if (RequestState != RequestState.None)
        {
            RequestState = RequestState.None;
        }

        // First time through - set up turn structure
        if (!MidTurn)
        {
            // Insert BeforeTurn action at the front of the queue
            Queue.InsertChoice(new BeforeTurnAction());

            // Add Residual action at the end of the queue
            Queue.AddChoice(new ResidualAction());

            MidTurn = true;
        }

        // Process actions one at a time
        while (Queue.Shift() is { } action)
        {
            RunAction(action);

            // Exit early if we need to wait for a request or battle ended
            if (RequestState != RequestState.None || Ended)
            {
                return;
            }
        }

        // Turn is complete
        EndTurn();
        MidTurn = false;
        Queue.Clear();
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
                            Add("teamsize", side.Id.ToString(), side.Pokemon.Count.ToString());
                        }
                    }

                    if (DisplayUi)
                    {
                        Add("start");
                    }

                    // Change Zacian/Zamazenta into their Crowned formes
                    foreach (Pokemon pokemon in GetAllPokemon())
                    {
                        Species? rawSpecies = null;
                        if (pokemon.Species.Id == SpecieId.Zacian && pokemon.Item == ItemId.RustedSword)
                        {
                            rawSpecies = Library.Species[SpecieId.ZacianCrowned];
                        }
                        else if (pokemon.Species.Id == SpecieId.Zamazenta && pokemon.Item == ItemId.RustedShield)
                        {
                            rawSpecies = Library.Species[SpecieId.ZamazentaCrowned];
                        }

                        if (rawSpecies == null) continue;

                        Species? species = pokemon.SetSpecie(rawSpecies, Effect);
                        if (species == null) continue;

                        pokemon.BaseSpecies = rawSpecies;
                        pokemon.Details = pokemon.GetUpdatedDetails();
                        pokemon.SetAbility(species.Abilities.GetAbility(SpeciesAbilityType.Slot0)
                            ?? throw new InvalidOperationException("Species has no ability in slot 0"),
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
                                // Forfeited before starting - assign the pokemon but mark as fainted
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
                        // Only apply species condition if it's not None
                        if (pokemon.Species.Conditon != ConditionId.None)
                        {
                            Condition speciesCondition = Library.Conditions[pokemon.Species.Conditon];
                            SingleEvent(EventId.Start, speciesCondition, pokemon.SpeciesState, pokemon);
                        }
                    }

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
                    btmAction.Move.BeforeTurnCallback(this, btmAction.Pokemon, target,
                        btmAction.Move.ToActiveMove());
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

                    if (pcmAction.Move.PriorityChargeCallback == null)
                        throw new InvalidOperationException("priorityChargeMove has no priorityChargeCallback");
                    pcmAction.Move.PriorityChargeCallback(this, pcmAction.Pokemon);
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
                    // We return here because the update event would crash since there are no active pokemon yet
                    return false;
                }

            case ActionId.Pass:
                return false;

            case ActionId.InstaSwitch:
            case ActionId.Switch:
                {
                    var switchAction = (SwitchAction)action;
                    if (switchAction.Choice == ActionId.Switch && switchAction.Pokemon.Status != ConditionId.None)
                    {
                        Ability naturalCure = Library.Abilities[AbilityId.NaturalCure];
                        SingleEvent(EventId.CheckShow, naturalCure, null, switchAction.Pokemon);
                    }

                    Actions.SwitchIn(switchAction.Target, switchAction.Pokemon.Position, switchAction.SourceEffect);
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
                    rbAction.Target.Hp = 1; // Needed so HP functions work
                    rbAction.Target.SetHp(rbAction.Target.MaxHp / 2);

                    if (DisplayUi)
                    {
                        Add("-heal", rbAction.Target, rbAction.Target.GetHealth, "[from] move: Revival Blessing");
                    }

                    rbAction.Pokemon.Side.RemoveSlotCondition(rbAction.Pokemon, ConditionId.RevivalBlessing);
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

                ClearActiveMove(failed: true);
                UpdateSpeed();
                residualPokemon = GetAllActive()
                    .Select(p => (p, p.GetUndynamaxedHp()))
                    .ToList();
                FieldEvent(EventId.Residual);

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

        // Switching (fainted pokemon, U-turn, Baton Pass, etc)
        if (Queue.Peek()?.Choice == ActionId.InstaSwitch)
        {
            return false;
        }

        // Emergency Exit / Wimp Out check (Gen 5+)
        if (action.Choice != ActionId.Start)
        {
            EachEvent(EventId.Update);
            foreach ((Pokemon pokemon, int originalHp) in residualPokemon)
            {
                int maxHp = pokemon.GetUndynamaxedHp(pokemon.MaxHp);
                if (pokemon.Hp > 0 && pokemon.GetUndynamaxedHp() <= maxHp / 2 && originalHp > maxHp / 2)
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
            bool reviveSwitch = false; // Used to ignore the fake switch for Revival Blessing
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
                if (!reviveSwitch) switches[i] = false;
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
                        FaintMessages(); // Pokemon may have fainted in BeforeSwitchOut
                        if (Ended) return true;
                        if (pokemon.Fainted)
                        {
                            switches[i] = Sides[i].Active.Any(p => p != null && p.SwitchFlag.IsTrue());
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
                return true;
            }
        }

        // In Gen 8+, speed is updated dynamically
        IAction? nextAction = Queue.Peek();
        if (nextAction?.Choice == ActionId.Move)
        {
            // Update the queue's speed properties and sort it
            UpdateSpeed();
            foreach (IAction queueAction in Queue.List)
            {
                GetActionSpeed(queueAction);
            }
            Queue.Sort();
        }

        return false;
    }
}