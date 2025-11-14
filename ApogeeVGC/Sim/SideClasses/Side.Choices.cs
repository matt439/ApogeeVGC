using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;
using System.Text.Json;

namespace ApogeeVGC.Sim.SideClasses;

public partial class Side
{
    public Choice GetChoice()
    {
        return Choice;
    }

    public bool ChooseMove(MoveIdIntUnion? moveText = null, int targetLoc = 0,
        EventType eventType = EventType.None)
    {
        // Step 1: Validate request state
        if (RequestState != RequestState.Move)
        {
            return EmitChoiceError($"Can't move: You need a {RequestState} response");
        }

        // Step 2: Get the active pokemon index
        int index = GetChoiceIndex();
        if (index >= Active.Count)
        {
            return EmitChoiceError("Can't move: You sent more choices than unfainted Pokémon.");
        }

        // Step 3: Determine auto-choose and get pokemon
        bool autoChoose = moveText == null;
        Pokemon pokemon = GetActiveAt(index);

        // Step 4: Parse moveText (name or index)
        PokemonMoveRequestData request = pokemon.GetMoveRequestData();
        var moveid = MoveId.None;

        if (autoChoose) moveText = 1; // Default to first move

        // Handle the union type: either MoveId or int
        switch (moveText)
        {
            case IntMoveIdIntUnion intUnion:
            {
                // Parse a one-based move index
                int moveIndex = intUnion.Value - 1;
                if (moveIndex < 0 || moveIndex >= request.Moves.Count)
                {
                    return EmitChoiceError(
                        $"Can't move: Your {pokemon.Name} doesn't have a move {intUnion.Value}");
                }

                moveid = request.Moves[moveIndex].Id;
                break;
            }
            case MoveIdMoveIdIntUnion moveIdUnion:
            {
                // Parse a move ID directly
                moveid = moveIdUnion.MoveId;

                // Find the move in the request
                bool found = request.Moves.Any(pokemonMoveData => pokemonMoveData.Id == moveid);

                if (!found)
                {
                    return EmitChoiceError(
                        $"Can't move: Your {pokemon.Name} doesn't have a move matching {moveid}");
                }

                break;
            }
        }

        // Step 5: Get available moves
        var moves = pokemon.GetMoves();

        // Step 6: Auto-choose first available move if needed
        if (autoChoose)
        {
            foreach (PokemonMoveData pokemonMoveData in request.Moves)
            {
                if (pokemonMoveData.Disabled is MoveIdMoveIdBoolUnion or BoolMoveIdBoolUnion
                    {
                        Value: true
                    })
                {
                    continue;
                }

                moveid = pokemonMoveData.Id;
                break;
            }
        }

        Move move = Battle.Library.Moves[moveid];

        // Step 7: Validate targeting
        if (autoChoose)
        {
            targetLoc = 0;
        }
        else if (Battle.Actions.TargetTypeChoices(move.Target))
        {
            if (targetLoc == 0 && Active.Count >= 2)
            {
                return EmitChoiceError($"Can't move: {move.Name} needs a target");
            }

            if (!Battle.ValidTargetLoc(targetLoc, pokemon, move.Target))
            {
                return EmitChoiceError($"Can't move: Invalid target for {move.Name}");
            }
        }
        else
        {
            if (targetLoc != 0)
            {
                return EmitChoiceError($"Can't move: You can't choose a target for {move.Name}");
            }
        }

        // Step 8: Handle locked moves (multi-turn moves like Outrage)
        var lockedMove = pokemon.GetLockedMove();
        if (lockedMove != null)
        {
            int lockedMoveTargetLoc = pokemon.LastMoveTargetLoc ?? 0;

            // Note: In the original TS code, it checks pokemon.volatiles[lockedMoveID]?.targetLoc
            // but EffectState doesn't have a targetLoc property in our C# implementation
            // This would need to be added to EffectState if this functionality is needed

            if (pokemon.MaybeLocked ?? false) Choice.CantUndo = true;

            Choice.Actions =
            [
                .. Choice.Actions, new ChosenAction
                {
                    Choice = ChoiceType.Move,
                    Pokemon = pokemon,
                    TargetLoc = lockedMoveTargetLoc,
                    MoveId = lockedMove.Value,
                }
            ];

            return true;
        }

        // Step 9: Handle Struggle when no moves have PP
        if (moves.Count == 0)
        {
            // Gen 4 and earlier announce Pokemon has no moves left
            if (Battle.Gen <= 4)
            {
                Send("-activate", pokemon, "move: Struggle");
            }

            if (pokemon.MaybeLocked ?? false) Choice.CantUndo = true;

            Choice.Actions =
            [
                .. Choice.Actions, new ChosenAction
                {
                    Choice = ChoiceType.Move,
                    Pokemon = pokemon,
                    MoveId = MoveId.Struggle,
                }
            ];

            return true;
        }

        // Step 10: Check for disabled moves
        bool isEnabled = false;
        string disabledSource = string.Empty;

        foreach (PokemonMoveData m in moves.Where(m => m.Id == moveid))
        {
            if (m.Disabled is null or BoolMoveIdBoolUnion { Value: false })
            {
                isEnabled = true;
                break;
            }
            else if (m.DisabledSource != null)
            {
                disabledSource = m.DisabledSource.Name;
            }
        }

        if (!isEnabled)
        {
            if (autoChoose)
            {
                throw new InvalidOperationException("autoChoose chose a disabled move");
            }

            return EmitChoiceError(
                $"Can't move: {pokemon.Name}'s {move.Name} is disabled",
                (pokemon, req =>
                    UpdateDisabledRequestForMove(pokemon, req, moveid, disabledSource))
            );
        }

        // Step 11: Terastallization (Gen 9 only)
        bool terastallize = eventType == EventType.Terastallize;

        if (terastallize && request.CanTerastallize is null or FalseMoveTypeFalseUnion)
        {
            return EmitChoiceError($"Can't move: {pokemon.Name} can't Terastallize.");
        }

        if (terastallize && Choice.Terastallize)
        {
            return EmitChoiceError("Can't move: You can only Terastallize once per battle.");
        }

        if (terastallize && Battle.Gen != 9)
        {
            return EmitChoiceError("Can't move: You can only Terastallize in Gen 9.");
        }

        // Step 12: Add action to choice
        Choice.Actions =
        [
            .. Choice.Actions, new ChosenAction
            {
                Choice = ChoiceType.Move,
                Pokemon = pokemon,
                TargetLoc = targetLoc,
                MoveId = moveid,
                Terastallize = terastallize ? pokemon.TeraType : null,
            }
        ];

        // Step 13: Handle maybeDisabled flag
        if ((pokemon.MaybeDisabled) &&
            (Battle.GameType == GameType.Singles ||
             (Battle.Gen <= 3 && !Battle.Actions.TargetTypeChoices(move.Target))))
        {
            Choice.CantUndo = true;
        }

        // Step 14: Update choice flags
        if (terastallize)
        {
            Choice.Terastallize = true;
        }

        return true;
    }

    public SideBoolUnion ChooseSwitch(PokemonIntUnion? slotText = null)
    {
        // Step 1: Validate request state
        if (RequestState != RequestState.Move && RequestState != RequestState.Switch &&
            RequestState != RequestState.SwitchIn)
        {
            return EmitChoiceError($"Can't switch: You need a {RequestState} response");
        }

        // Step 2: Get the active pokemon index
        int index = GetChoiceIndex();
        if (index >= Active.Count)
        {
            if (RequestState == RequestState.Switch || RequestState == RequestState.SwitchIn)
            {
                return EmitChoiceError(
                    "Can't switch: You sent more switches than Pokémon that need to switch");
            }

            return EmitChoiceError("Can't switch: You sent more choices than unfainted Pokémon");
        }

        // Step 3: Get the currently active pokemon
        Pokemon pokemon = GetActiveAt(index);
        int slot;

        // Step 4: Determine the target slot
        if (slotText == null)
        {
            // Auto-select mode
            if (RequestState != RequestState.Switch && RequestState != RequestState.SwitchIn)
            {
                return EmitChoiceError("Can't switch: You need to select a Pokémon to switch in");
            }

            // Check for Revival Blessing slot condition
            if (SlotConditions[pokemon.Position].ContainsKey(ConditionId.RevivalBlessing))
            {
                // Find first fainted Pokemon
                slot = 0;
                while (slot < Pokemon.Count && !Pokemon[slot].Fainted)
                {
                    slot++;
                }
            }
            else
            {
                // Normal forced switch - auto-select first available
                if (Choice.ForcedSwitchesLeft <= 0)
                {
                    return ChoosePass();
                }

                slot = Active.Count;
                while (slot < Pokemon.Count &&
                       (Choice.SwitchIns.Contains(slot) || Pokemon[slot].Fainted))
                {
                    slot++;
                }
            }
        }
        else
        {
            // Parse the slot from the union type
            slot = slotText switch
            {
                IntPokemonIntUnion intUnion => intUnion.Value -
                                               1, // Convert from 1-based to 0-based
                PokemonPokemonIntUnion pokemonUnion => pokemonUnion.Pokemon.Position,
                _ => -1, // Invalid - will trigger error below
            };

            // If slot is still invalid after parsing, return error
            if (slot < 0)
            {
                return EmitChoiceError($"Can't switch: Invalid switch target \"{slotText}\"");
            }
        }

        // Step 5: Validate slot index
        if (slot >= Pokemon.Count)
        {
            return EmitChoiceError(
                $"Can't switch: You do not have a Pokémon in slot {slot + 1} to switch to");
        }
        else if (slot < Active.Count &&
                 !SlotConditions[pokemon.Position].ContainsKey(ConditionId.RevivalBlessing))
        {
            return EmitChoiceError("Can't switch: You can't switch to an active Pokémon");
        }
        else if (Choice.SwitchIns.Contains(slot))
        {
            return EmitChoiceError(
                $"Can't switch: The Pokémon in slot {slot + 1} can only switch in once");
        }

        // Step 6: Get target Pokemon
        Pokemon targetPokemon = Pokemon[slot];

        // Step 7: Handle Revival Blessing special case
        if (SlotConditions[pokemon.Position].ContainsKey(ConditionId.RevivalBlessing))
        {
            if (!targetPokemon.Fainted)
            {
                return EmitChoiceError("Can't switch: You have to pass to a fainted Pokémon");
            }

            // Decrement forced switches (clamp to prevent negative)
            Choice.ForcedSwitchesLeft = Math.Max(0, Choice.ForcedSwitchesLeft - 1);
            pokemon.SwitchFlag = false;

            Choice.Actions =
            [
                .. Choice.Actions, new ChosenAction
                {
                    MoveId = MoveId.RevivalBlessing,
                    Choice = ChoiceType.RevivalBlessing,
                    Pokemon = pokemon,
                    Target = targetPokemon,
                }
            ];

            return true;
        }

        // Step 8: Validate target is not fainted (for normal switches)
        if (targetPokemon.Fainted)
        {
            return EmitChoiceError("Can't switch: You can't switch to a fainted Pokémon");
        }

        // Step 9: Handle move phase switching (check for trapped)
        if (RequestState == RequestState.Move)
        {
            if (pokemon.Trapped == PokemonTrapped.True)
            {
                return EmitChoiceError(
                    "Can't switch: The active Pokémon is trapped",
                    (pokemon, req =>
                        {
                            bool updated = false;

                            if (req.MaybeTrapped != null)
                            {
                                req.MaybeTrapped = null;
                                updated = true;
                            }

                            if (req.Trapped != true)
                            {
                                req.Trapped = true;
                                updated = true;
                            }

                            return BoolVoidUnion.FromBool(updated);
                        }
                    )
                );
            }

            if (pokemon.MaybeTrapped)
            {
                Choice.CantUndo = true;
            }
        }
        else if (RequestState == RequestState.Switch || RequestState == RequestState.SwitchIn)
        {
            // Step 10: Handle forced switches
            if (Choice.ForcedSwitchesLeft <= 0)
            {
                throw new InvalidOperationException("Player somehow switched too many Pokemon");
            }

            Choice.ForcedSwitchesLeft--;
        }

        // Step 11: Record the switch
        Choice.SwitchIns.Add(slot);

        ChoiceType choiceType =
            (RequestState == RequestState.Switch || RequestState == RequestState.SwitchIn)
                ? ChoiceType.InstaSwitch
                : ChoiceType.Switch;

        Choice.Actions =
        [
            .. Choice.Actions, new ChosenAction
            {
                MoveId = MoveId.None,
                Choice = choiceType,
                Pokemon = pokemon,
                Target = targetPokemon,
            }
        ];

        return true;
    }

    public bool ChooseTeam(String data = "")
    {
        // Step 1: Validate request state
        if (RequestState != RequestState.TeamPreview)
        {
            return EmitChoiceError(
                "Can't choose for Team Preview: You're not in a Team Preview phase");
        }

        // Step 2: Parse positions from input
        List<int> positions;
        if (String.IsNullOrWhiteSpace(data))
        {
            positions = [];
        }
        else
        {
            char separator = data.Contains(',') ? ',' : ' ';
            positions = data.Split(separator)
                .Select(s =>
                    int.TryParse(s.Trim(), out int val) ? val - 1 : -1) // Convert to 0-based
                .ToList();
        }

        int pickedTeamSize = PickedTeamSize();

        // Step 3: Auto-fill positions if needed
        if (positions.Count == 0)
        {
            // No input - use all Pokémon in order
            for (int i = 0; i < pickedTeamSize; i++)
            {
                positions.Add(i);
            }
        }
        else if (positions.Count < pickedTeamSize)
        {
            // Partial input - fill remaining slots with unused Pokémon
            for (int i = 0; i < pickedTeamSize && positions.Count < pickedTeamSize; i++)
            {
                if (!positions.Contains(i))
                {
                    positions.Add(i);
                }
            }
        }
        else if (positions.Count > pickedTeamSize)
        {
            // Too many positions - trim to size
            positions = positions.Take(pickedTeamSize).ToList();
        }

        // Step 4: Validate positions
        for (int index = 0; index < positions.Count; index++)
        {
            int pos = positions[index];

            // Check if position is valid
            if (pos < 0 || pos >= Pokemon.Count)
            {
                return EmitChoiceError(
                    $"Can't choose for Team Preview: You do not have a Pokémon in slot {pos + 1}");
            }

            // Check for duplicates
            if (positions.IndexOf(pos) != index)
            {
                return EmitChoiceError(
                    $"Can't choose for Team Preview: The Pokémon in slot {pos + 1} can only switch in once");
            }
        }

        // Step 5: Create team actions
        for (int index = 0; index < positions.Count; index++)
        {
            int pos = positions[index];
            Choice.SwitchIns.Add(pos);
            Choice.Actions =
            [
                .. Choice.Actions, new ChosenAction
                {
                    MoveId = MoveId.None,
                    Choice = ChoiceType.Team,
                    Index = index,
                    Pokemon = Pokemon[pos],
                    Priority = -index, // Earlier picks have higher priority
                }
            ];
        }

        return true;
    }

    public bool ChooseShift()
    {
        throw new NotImplementedException(
            "This is only used in Triple Battles which are not yet implemented.");
    }

    public void ClearChoice()
    {
        int forcedSwitches = 0;
        int forcedPasses = 0;

        // Calculate forced switches if we're in switch request state
        if (Battle.RequestState == RequestState.Switch ||
            Battle.RequestState == RequestState.SwitchIn)
        {
            // Count active Pokemon that need to switch out
            int canSwitchOut = Active.Count(pokemon => pokemon?.SwitchFlag.IsTrue() == true);

            // Count bench Pokemon available to switch in (not active, not fainted)
            int canSwitchIn = Pokemon
                .Skip(Active.Count) // Skip active slots
                .Count(pokemon => pokemon is { Fainted: false });

            // Can only force as many switches as we have Pokemon to switch in
            forcedSwitches = Math.Min(canSwitchOut, canSwitchIn);

            // Any switches we can't fulfill become forced passes
            forcedPasses = canSwitchOut - forcedSwitches;
        }

        // Reset choice to default state
        Choice = new Choice
        {
            CantUndo = false,
            Error = string.Empty,
            Actions = [],
            ForcedSwitchesLeft = forcedSwitches,
            ForcedPassesLeft = forcedPasses,
            SwitchIns = [],
            Terastallize = false,
        };
    }

    /// <summary>
    /// Process a choice object containing one or more actions.
    /// This is the main entry point for making battle choices using the Choice class.
    /// </summary>
    /// <param name="input">The Choice object containing the actions to execute</param>
    /// <returns>True if all choices were valid and processed, false otherwise</returns>
    public bool Choose(Choice input)
    {
        // Step 1: Validate that it's the player's turn
        if (RequestState == RequestState.None)
        {
            string message = Battle.Ended
                ? "Can't do anything: The game is over"
                : "Can't do anything: It's not your turn";
            return EmitChoiceError(message);
        }

        // Step 2: Check if undo is allowed
        if (Choice.CantUndo)
        {
            return EmitChoiceError(
                "Can't undo: A trapping/disabling effect would cause undo to leak information");
        }

        // Step 3: Clear existing choice
        ClearChoice();

        // Step 3.5: For SwitchIn/Switch requests, validate that we have actions (not empty)
        // Empty choices are invalid for switch requests - the battle should have ended if no switches are possible
        if ((RequestState == RequestState.Switch || RequestState == RequestState.SwitchIn) &&
            input.Actions.Count == 0)
        {
            Console.WriteLine(
                $"[Side.Choose] Empty choice for {RequestState} request - rejecting!");
            return EmitChoiceError(
                "Can't submit empty choice for switch request. You must switch in a Pokemon or the battle should have ended.");
        }

        // Step 4: Validate number of actions doesn't exceed expected count
        // For team preview, allow up to full team size
        // For other requests, limit to active Pokemon count
        int maxActions = RequestState == RequestState.TeamPreview
            ? Pokemon.Count
            : Active.Count;

        if (input.Actions.Count > maxActions)
        {
            return EmitChoiceError(
                $"Can't make choices: You sent choices for {input.Actions.Count} Pokémon, but only {maxActions} are allowed!"
            );
        }

        // Debug logging
        Console.WriteLine(
            $"[Side.Choose] Processing {input.Actions.Count} actions for {Name} (RequestState: {RequestState})");
        for (int i = 0; i < input.Actions.Count; i++)
        {
            var action = input.Actions[i];
            Console.WriteLine(
                $"[Side.Choose]   Action {i}: Type={action.Choice}, Index={action.Index}, Pokemon={(action.Pokemon?.Name ?? "null")}");
        }

        // Step 5: Process each action in the choice
        if (input.Actions.Select((action, index) =>
            {
                bool success = action.Choice switch
                {
                    ChoiceType.Move => ProcessChosenMoveAction(action),
                    ChoiceType.Switch or ChoiceType.InstaSwitch =>
                        ProcessChosenSwitchAction(action),
                    ChoiceType.Team => ProcessChosenTeamAction(action),
                    ChoiceType.Pass => ChoosePass().IsTrue(),
                    ChoiceType.RevivalBlessing => ProcessChosenRevivalBlessingAction(action),
                    _ => EmitChoiceError($"Unrecognized choice type: {action.Choice}"),
                };

                if (!success)
                {
                    Console.WriteLine($"[Side.Choose] Action {index} failed: {Choice.Error}");
                }

                return success;
            }).Any(success => !success))
        {
            Console.WriteLine($"[Side.Choose] Overall choice failed for {Name}");
            return false;
        }

        // Step 6: Apply choice-level settings
        if (input.Terastallize)
        {
            Choice.Terastallize = true;
        }

        if (input.CantUndo)
        {
            Choice.CantUndo = true;
        }

        bool result = string.IsNullOrEmpty(Choice.Error);
        Console.WriteLine(
            $"[Side.Choose] Choice processing complete for {Name}: {(result ? "SUCCESS" : $"FAILED - {Choice.Error}")}");
        return result;
    }

    private bool ProcessChosenMoveAction(ChosenAction action)
    {
        // Determine event type based on Terastallize flag
        EventType eventType = action.Terastallize != null
            ? EventType.Terastallize
            : EventType.None;

        // Use ChooseMove with the move ID and target location
        return ChooseMove(
            moveText: action.MoveId,
            targetLoc: action.TargetLoc ?? 0,
            eventType: eventType
        );
    }

    private bool ProcessChosenSwitchAction(ChosenAction action)
    {
        // Support both Target (Pokemon object) and Index (int) for switch actions
      if (action.Target != null)
        {
        return ChooseSwitch(action.Target).IsTrue();
       }
        else if (action.Index.HasValue)
        {
         // Convert 0-based index to 1-based for ChooseSwitch
 return ChooseSwitch(action.Index.Value + 1).IsTrue();
   }
   else
        {
       return EmitChoiceError("Can't switch: No target Pokemon specified");
        }
    }

    private bool ProcessChosenTeamAction(ChosenAction action)
    {
        Console.WriteLine(
            $"[ProcessChosenTeamAction] Processing action: Index={action.Index}, Pokemon={(action.Pokemon?.Name ?? "null")}, Priority={action.Priority}");

        // For team preview, the actions are already ordered by priority
        // We just need to verify this is a valid team choice

        // Handle GUI case where Pokemon is null but Index is provided
        int slot;
        if (action.Pokemon == null)
        {
            Console.WriteLine($"[ProcessChosenTeamAction] Pokemon is null, using Index");
            if (!action.Index.HasValue || action.Index.Value < 0 ||
                action.Index.Value >= Pokemon.Count)
            {
                Console.WriteLine($"[ProcessChosenTeamAction] Invalid index: {action.Index}");
                return EmitChoiceError(
                    "Can't choose for Team Preview: Invalid Pokemon index specified");
            }

            slot = action.Index.Value;
            Console.WriteLine($"[ProcessChosenTeamAction] Looking up Pokemon at slot {slot}");
            // Update the action with the Pokemon reference for proper processing
            action = action with { Pokemon = Pokemon[slot] };
            Console.WriteLine($"[ProcessChosenTeamAction] Pokemon found: {action.Pokemon.Name}");
        }
        else
        {
            Console.WriteLine(
                $"[ProcessChosenTeamAction] Pokemon already set: {action.Pokemon.Name}");
            slot = action.Pokemon.Position;
        }

        Console.WriteLine($"[ProcessChosenTeamAction] Attempting to add slot {slot} to SwitchIns");
        if (!Choice.SwitchIns.Add(slot))
        {
            Console.WriteLine($"[ProcessChosenTeamAction] Slot {slot} already in SwitchIns");
            return EmitChoiceError(
                $"Can't choose for Team Preview: The Pokémon in slot {slot + 1} can only switch in once"
            );
        }

        Console.WriteLine(
            $"[ProcessChosenTeamAction] Adding action to Choice.Actions (current count: {Choice.Actions.Count})");
        Choice.Actions = [.. Choice.Actions, action];
        Console.WriteLine(
            $"[ProcessChosenTeamAction] Action added successfully, new count: {Choice.Actions.Count}");

        return true;
    }

    private bool ProcessChosenRevivalBlessingAction(ChosenAction action)
    {
        if (action.Pokemon == null || action.Target == null)
        {
            return EmitChoiceError("Can't use Revival Blessing: Missing Pokemon or target");
        }

        // Validate that the target is fainted
        if (!action.Target.Fainted)
        {
            return EmitChoiceError("Can't use Revival Blessing: Target must be fainted");
        }

        // Validate that we have a Revival Blessing slot condition
        if (!SlotConditions[action.Pokemon.Position].ContainsKey(ConditionId.RevivalBlessing))
        {
            return EmitChoiceError("Can't use Revival Blessing: No Revival Blessing active");
        }

        // Process the Revival Blessing
        Choice.ForcedSwitchesLeft = Math.Max(0, Choice.ForcedSwitchesLeft - 1);
        action.Pokemon.SwitchFlag = false;

        Choice.Actions = [.. Choice.Actions, action];

        return true;
    }

    public int GetChoiceIndex(bool isPass = false)
    {
        int index = Choice.Actions.Count;

        if (!isPass)
        {
            switch (RequestState)
            {
                case RequestState.Move:
                    // auto-pass
                    while (
                        index < Active.Count &&
                        Active[index] != null &&
                        (Active[index]!.Fainted ||
                         Active[index]!.Volatiles.ContainsKey(ConditionId.Commanding))
                    )
                    {
                        ChoosePass();
                        index++;
                    }

                    break;
                case RequestState.Switch:
                case RequestState.SwitchIn:
                    while (index < Active.Count && Active[index] != null &&
                           !Active[index]!.SwitchFlag.IsTrue())
                    {
                        ChoosePass();
                        index++;
                    }

                    break;
            }
        }

        return index;
    }

    public SideBoolUnion ChoosePass()
    {
        int index = GetChoiceIndex(true);
        if (index >= Active.Count) return false;
        Pokemon pokemon = GetActiveAt(index);

        switch (RequestState)
        {
            case RequestState.Switch:
            case RequestState.SwitchIn:
                if (pokemon.SwitchFlag.IsTrue())
                {
                    // This condition will always happen if called by Battle#choose()
                    if (Choice.ForcedPassesLeft <= 0)
                    {
                        return EmitChoiceError($"Can't pass: You need to switch in a Pokémon to" +
                                               $"replace {pokemon.Name}");
                    }

                    Choice.ForcedPassesLeft--;
                }

                break;

            case RequestState.Move:
                if (!pokemon.Fainted && !pokemon.Volatiles.ContainsKey(ConditionId.Commanding))
                {
                    return EmitChoiceError(
                        $"Can't pass: Your {pokemon.Name} must make a move (or switch)");
                }

                break;

            default:
                return EmitChoiceError("Can't pass: Not a move or switch request");
        }

        Choice.Actions =
        [
            .. Choice.Actions, new ChosenAction
            {
                MoveId = MoveId.None,
                Choice = ChoiceType.Pass,
            }
        ];

        return true;
    }

    /// <summary>
    /// Automatically finish a choice if not currently complete.
    /// </summary>
    public bool AutoChoose()
    {
        if (RequestState == RequestState.TeamPreview)
        {
            if (!IsChoiceDone())
            {
                ChooseTeam();
            }
        }
        else if (RequestState == RequestState.Switch || RequestState == RequestState.SwitchIn)
        {
            int i = 0;
            while (!IsChoiceDone())
            {
                // Safety check: if we're being asked to switch but have no available Pokemon, 
                // the battle should have already ended. This indicates a bug.
                int availableSwitches = Battle.CanSwitch(this);
                bool hasRevivalBlessing = Active.Any(p =>
                    p != null && SlotConditions[p.Position]
                        .ContainsKey(ConditionId.RevivalBlessing));

                if (availableSwitches == 0 && !hasRevivalBlessing)
                {
                    throw new InvalidOperationException(
                        $"AutoChoose switch failed: {Name} has no available Pokemon to switch in. " +
                        $"The battle should have ended before this point. This is a bug.");
                }

                SideBoolUnion result = ChooseSwitch();
                if (!result.IsTrue())
                {
                    throw new InvalidOperationException(
                        $"autoChoose switch crashed: {Choice.Error}");
                }

                i++;
                if (i > 10)
                {
                    throw new InvalidOperationException("autoChoose failed: infinite looping");
                }
            }
        }
        else if (RequestState == RequestState.Move)
        {
            int i = 0;
            while (!IsChoiceDone())
            {
                if (!ChooseMove())
                {
                    throw new InvalidOperationException($"autoChoose crashed: {Choice.Error}");
                }

                i++;
                if (i > 10)
                {
                    throw new InvalidOperationException("autoChoose failed: infinite looping");
                }
            }
        }

        return true;
    }

    /// <summary>
    /// The number of pokemon you must choose in Team Preview.
    /// 
    /// Note that PS doesn't support choosing fewer than this number of pokemon.
    /// In the games, it is sometimes possible to bring fewer than this, but
    /// since that's nearly always a mistake, we haven't gotten around to
    /// supporting it.
    /// </summary>
    public int PickedTeamSize()
    {
        int pokemonLength = Pokemon.Count;
        int ruleTableSize = Battle.RuleTable.PickedTeamSize ?? int.MaxValue;
        return Math.Min(pokemonLength, ruleTableSize);
    }

    public void Send(params object[] parts)
    {
        string sideUpdate = "|" + string.Join("|", parts.Select(part =>
        {
            if (part is Func<Side, object> func)
                return func(this).ToString() ?? string.Empty;
            return part.ToString() ?? string.Empty;
        }));

        Battle.Send(SendType.SideUpdate, [$"{Id}\n{sideUpdate}"]);
    }

    public void EmitRequest(IChoiceRequest? update = null, bool updatedRequest = false)
    {
        update ??= ActiveRequest;

        if (updatedRequest && update is MoveRequest moveRequest)
        {
            moveRequest.Update = true;
        }
        else if (updatedRequest && update is SwitchRequest switchRequest)
        {
            switchRequest.Update = true;
        }

        string json = JsonSerializer.Serialize(update);
        Battle.Send(SendType.SideUpdate, [$"{Id}\n|request|{json}"]);
        ActiveRequest = update;
    }

    public bool EmitChoiceError(string message,
        (Pokemon pokemon, Func<PokemonMoveRequestData, BoolVoidUnion> update)? updateInfo = null)
    {
        Choice.Error = message;

        bool? updated = updateInfo.HasValue
            ? UpdateRequestForPokemon(updateInfo.Value.pokemon, updateInfo.Value.update)
            : null;

        string type = updated == true ? "[Unavailable choice]" : "[Invalid choice]";
        Battle.Send(SendType.SideUpdate, [$"{Id}\n|error|{type} {message}"]);

        if (updated == true)
        {
            EmitRequest(ActiveRequest, true);
        }

        //if (Battle.StrictChoices)
        //{
        //    throw new InvalidOperationException($"{type} {message}");
        //}

        return false;
    }

    public bool IsChoiceDone()
    {
        if (RequestState == RequestState.None) return true;
        if (Choice.ForcedSwitchesLeft > 0) return false;

        // For Switch/SwitchIn requests, also check if we have forced passes left
        // This prevents IsChoiceDone from returning true when we still need to handle passes
        if ((RequestState == RequestState.Switch || RequestState == RequestState.SwitchIn) &&
            Choice.ForcedPassesLeft > 0) return false;

        if (RequestState == RequestState.TeamPreview)
        {
            return Choice.Actions.Count >= PickedTeamSize();
        }

        GetChoiceIndex();
        return Choice.Actions.Count >= Active.Count;
    }


    private BoolVoidUnion UpdateDisabledRequestForMove(Pokemon pokemon, PokemonMoveRequestData req,
        MoveId moveid, string disabledSource)
    {
        bool updated = UpdateDisabledRequest(pokemon, req);

        foreach (PokemonMoveData m in req.Moves)
        {
            if (m.Id != moveid) continue;

            // Check if we need to update the disabled state
            bool needsUpdate = m.Disabled is null or BoolMoveIdBoolUnion { Value: false } ||
                               m.DisabledSource?.Name != disabledSource;

            if (needsUpdate)
            {
                updated = true;
            }

            break;
        }

        return BoolVoidUnion.FromBool(updated);
    }

    public bool UpdateDisabledRequest(Pokemon pokemon, PokemonMoveRequestData req)
    {
        bool updated = false;

        // Clear maybeLocked if it's set
        if (pokemon.MaybeLocked ?? false)
        {
            pokemon.MaybeLocked = false;
            req.MaybeLocked = null;
            updated = true;
        }

        // Handle maybeDisabled in non-singles formats
        if (pokemon.MaybeDisabled && Battle.GameType != GameType.Singles)
        {
            // Gen 4+ behavior
            if (Battle.Gen >= 4)
            {
                pokemon.MaybeDisabled = false;
                req.MaybeDisabled = null;
                updated = true;
            }

            // Update individual move disabled states
            foreach (PokemonMoveData m in req.Moves)
            {
                MoveSlot? moveData = pokemon.GetMoveData(m.Id);
                BoolHiddenUnion? disabled = moveData?.Disabled;

                // Check if move should be marked as disabled
                if (disabled != null &&
                    (Battle.Gen >= 4 ||
                     Battle.Actions.TargetTypeChoices(m.Target ?? MoveTarget.None)))
                {
                    m.Disabled = true;
                    updated = true;
                }
            }
        }

        // If all moves are disabled or only Struggle is available
        bool allMovesDisabled = req.Moves.All(m =>
            m.Disabled is BoolMoveIdBoolUnion { Value: true } ||
            m.Id == MoveId.Struggle);

        if (allMovesDisabled)
        {
            // Disable Terastallization (Gen 9 mechanic)
            if (req.CanTerastallize is not null and not FalseMoveTypeFalseUnion)
            {
                req.CanTerastallize = null;
                updated = true;
            }
        }

        return updated;
    }

    public bool UpdateRequestForPokemon(Pokemon pokemon,
        Func<PokemonMoveRequestData, BoolVoidUnion> update)
    {
        // Ensure we have an active request with Pokemon data
        if (ActiveRequest is not MoveRequest moveRequest || moveRequest.Active == null)
        {
            throw new InvalidOperationException("Can't update a request without active Pokemon");
        }

        // Find the Pokemon in the request
        PokemonMoveRequestData? req = moveRequest.Active[pokemon.Position];
        if (req == null)
        {
            throw new InvalidOperationException("Pokemon not found in request's active field");
        }

        // Apply the update function
        BoolVoidUnion result = update(req);

        // Return true if the update function returned true, otherwise default to true
        return result switch
        {
            BoolBoolVoidUnion { Value: var b } => b,
            _ => true,
        };
    }
}