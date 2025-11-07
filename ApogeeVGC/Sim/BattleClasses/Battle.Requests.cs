using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    public void MakeRequest(RequestState? type = null)
    {
        // Update request state if provided, otherwise use current state
        if (type.HasValue)
        {
            RequestState = type.Value;

            // Clear all sides' choices when starting a new request
            foreach (Side side in Sides)
            {
                side.ClearChoice();
            }
        }
        else
        {
            type = RequestState;
        }

        // Clear all active requests before generating new ones
        foreach (Side side in Sides)
        {
            side.ActiveRequest = null;
        }

        // Add team preview message if applicable
        if (type.Value == RequestState.TeamPreview)
        {
            // `pickedTeamSize = 6` means the format wants the user to select
            // the entire team order, unlike `pickedTeamSize = null` which
            // will only ask the user to select their lead(s).
            int? pickedTeamSize = RuleTable.PickedTeamSize;

            if (DisplayUi)
            {
                if (pickedTeamSize.HasValue)
                {
                    Add("teampreview", pickedTeamSize.Value);
                }
                else
                {
                    Add("teampreview");
                }
            }
        }

        // Generate appropriate requests for the current state
        var requests = GetRequests(type.Value);

        // Assign requests to each side
        for (int i = 0; i < Sides.Count; i++)
        {
            Sides[i].ActiveRequest = requests[i];
        }

        // Verify that choices aren't already done (would indicate a bug)
        if (Sides.All(side => side.IsChoiceDone()))
        {
            throw new InvalidOperationException("Choices are done immediately after a request");
        }
    }

    /// <summary>
    /// Clears the current request state and resets all sides' active requests and choices.
    /// Called when a turn's decision phase is complete or when canceling pending requests.
    /// </summary>
    public void ClearRequest()
    {
        RequestState = RequestState.None;
        foreach (Side side in Sides)
        {
            side.ActiveRequest = null;
            side.ClearChoice();
        }
    }

    public List<IChoiceRequest> GetRequests(RequestState type)
    {
        // Default to no request (null for each side)
        var requests = new IChoiceRequest?[Sides.Count];

        switch (type)
        {
            case RequestState.SwitchIn:
                for (int i = 0; i < Sides.Count; i++)
                {
                    Side side = Sides[i];
                    if (side.PokemonLeft <= 0) continue;

                    // Create a table of which active Pokemon need to switch
                    // Convert MoveIdBoolUnion to bool using IsTrue() method
                    var switchTable = side.Active
                        .Select(pokemon => pokemon?.SwitchFlag.IsTrue() ?? false)
                        .ToList();

                    // Only create a switch request if at least one Pokemon needs to switch
                    if (switchTable.Any(flag => flag))
                    {
                        requests[i] = new SwitchRequest
                        {
                            ForceSwitch = switchTable,
                            Side = side.GetRequestData(),
                        };
                    }
                }
                break;

            case RequestState.TeamPreview:
                for (int i = 0; i < Sides.Count; i++)
                {
                    Side side = Sides[i];
                    int? maxChosenTeamSize = RuleTable.PickedTeamSize > 0
                        ? RuleTable.PickedTeamSize
                        : null;

                    requests[i] = new TeamPreviewRequest
                    {
                        MaxChosenTeamSize = maxChosenTeamSize,
                        Side = side.GetRequestData(),
                    };
                }
                break;

            default:
                // Regular move requests
                for (int i = 0; i < Sides.Count; i++)
                {
                    Side side = Sides[i];
                    if (side.PokemonLeft <= 0) continue;

                    // Get move request data for each active Pokemon
                    var activeData = side.Active
                        .Where(pokemon => pokemon != null)
                        .Select(pokemon => pokemon!.GetMoveRequestData())
                        .ToList();

                    var moveRequest = new MoveRequest
                    {
                        Active = activeData,
                        Side = side.GetRequestData()
                    };

                    requests[i] = moveRequest;
                }
                break;
        }

        // Check if multiple requests exist (multiple players need to make choices)
        bool multipleRequestsExist = requests.Count(r => r != null) >= 2;

        // Finalize all requests
        for (int i = 0; i < Sides.Count; i++)
        {
            if (requests[i] != null)
            {
                // Set noCancel if cancellation is not supported or only one player is choosing
                if (!SupportCancel || !multipleRequestsExist)
                {
                    requests[i] = requests[i] switch
                    {
                        SwitchRequest sr => sr with { NoCancel = true },
                        TeamPreviewRequest tpr => tpr with { NoCancel = true },
                        MoveRequest mr => mr with { NoCancel = true },
                        _ => requests[i],
                    };
                }
            }
            else
            {
                // Create a wait request for sides that don't need to make a choice
                requests[i] = new WaitRequest
                {
                    Side = Sides[i].GetRequestData(),
                };
            }
        }

        return requests.Where(r => r != null).Cast<IChoiceRequest>().ToList();
    }

    /// <summary>
    /// Takes a choice passed from the client. Starts the next
    /// turn if all required choices have been made.
    /// </summary>
    /// <param name="sideId">The ID of the side making the choice</param>
    /// <param name="input">The choice being made</param>
    /// <returns>True if the choice was valid and processed, false otherwise</returns>
    public bool Choose(SideId sideId, Choice input)
    {
        Side side = GetSide(sideId);

        if (!side.Choose(input))
        {
            if (string.IsNullOrEmpty(side.GetChoice().Error))
            {
                side.EmitChoiceError(
                    $"Unknown error for choice: {input}. If you're not using a custom client," +
                    $"please report this as a bug.");
            }
            return false;
        }

        if (!side.IsChoiceDone())
        {
            side.EmitChoiceError($"Incomplete choice: {input} - missing other pokemon");
            return false;
        }

        if (AllChoicesDone())
        {
            CommitChoices();
        }

        return true;
    }

    /// <summary>
    /// Convenience method for easily making choices for multiple sides.
    /// If inputs are provided, applies them to the corresponding sides.
    /// If no inputs are provided, auto-chooses for all sides.
    /// </summary>
    /// <param name="inputs">Optional list of choices for each side. Pass null or empty list for auto-choice.</param>
    public void MakeChoices(List<Choice>? inputs = null)
    {
        if (inputs is { Count: > 0 })
        {
            // Apply provided choices to each corresponding side
            for (int i = 0; i < inputs.Count; i++)
            {
                Choice input = inputs[i];
                Sides[i].Choose(input);
            }
        }
        else
        {
            // Auto-choose for all sides
            foreach (Side side in Sides)
            {
                side.AutoChoose();
            }
        }

        // Commit all choices
        CommitChoices();
    }

    public void CommitChoices()
    {
        UpdateSpeed();

        // Sometimes you need to make switch choices mid-turn (e.g. U-turn,
        // fainting). When this happens, the rest of the turn is saved (and not
        // re-sorted), but the new switch choices are sorted and inserted before
        // the rest of the turn.
        var oldQueue = Queue.List.ToList(); // Create a copy of the current queue
        Queue.Clear();

        if (!AllChoicesDone())
        {
            throw new InvalidOperationException("Not all choices done");
        }

        // Log each side's choice to the input log
        foreach (Side side in Sides)
        {
            string? choice = side.GetChoice().ToString();
            if (!string.IsNullOrEmpty(choice))
            {
                InputLog.Add($"> {side.Id} {choice}");
            }
        }

        // Add each side's actions to the queue
        foreach (Side side in Sides)
        {
            Queue.AddChoice(side.Choice.Actions);
        }

        ClearRequest();

        // Sort the new actions by priority/speed
        Queue.Sort();

        // Append the old queue actions after the new ones
        Queue.List.AddRange(oldQueue);

        // Clear request state
        RequestState = RequestState.None;
        foreach (Side side in Sides)
        {
            side.ActiveRequest = null;
        }

        // Start executing the turn
        TurnLoop();

        // Workaround for tests - send updates if log is getting large
        if (Log.Count - SentLogPos > 500)
        {
            SendUpdates();
        }
    }

    public void UndoChoice(SideId sideId)
    {
        Side side = GetSide(sideId);

        // No active request - nothing to undo
        if (RequestState == RequestState.None)
            return;

        // Check if undo would leak information
        if (side.GetChoice().CantUndo)
        {
            side.EmitChoiceError("Can't undo: A trapping/disabling effect would cause undo to leak information");
            return;
        }

        bool updated = false;

        // If undoing a move selection, update disabled moves for each Pokémon
        if (side.RequestState == RequestState.Move)
        {
            foreach (ChosenAction action in side.GetChoice().Actions)
            {
                // Skip if not a move action
                if (action.Choice != ChoiceType.Move)
                    continue;

                Pokemon? pokemon = action.Pokemon;
                if (pokemon == null)
                    continue;

                // Update the request for this Pokémon, refreshing disabled moves
                if (side.UpdateRequestForPokemon(pokemon, req =>
                        side.UpdateDisabledRequest(pokemon, req)))
                {
                    updated = true;
                }
            }
        }

        // Clear the current choice
        side.ClearChoice();

        // If we updated any move availability, send the updated request to the client
        if (updated && side.ActiveRequest != null)
        {
            side.EmitRequest(side.ActiveRequest, updatedRequest: true);
        }
    }

    /// <summary>
    /// Returns true if both decisions are complete.
    /// Checks if all sides have completed their choices for the current turn.
    /// When supportCancel is disabled, locks completed choices to prevent information leaks.
    /// </summary>
    /// <returns>True if all sides have completed their choices, false otherwise</returns>
    public bool AllChoicesDone()
    {
        int totalActions = 0;

        foreach (Side side in Sides.Where(side => side.IsChoiceDone()))
        {
            // If cancellation is not supported, lock the choice to prevent undoing
            // This prevents information leaks from trapping/disabling effects
            if (!SupportCancel)
            {
                Choice choice = side.GetChoice();
                choice.CantUndo = true;
            }

            totalActions++;
        }

        return totalActions >= Sides.Count;
    }

    /// <summary>
    /// Sends the current active requests to all players and waits for their choices.
    /// This method coordinates the async request/response flow with each player.
    /// </summary>
    public async Task RequestAndWaitForChoicesAsync(CancellationToken cancellationToken = default)
    {
        // Verify that we have active requests
        if (Sides.Any(side => side.ActiveRequest == null))
        {
            throw new InvalidOperationException("Cannot send requests to players: Some sides have no active request");
        }

        // Create tasks for each side to get their choice
        var choiceTasks = new List<Task>();

        foreach (Side side in Sides)
        {
            // Skip sides that don't need to make a choice (already done)
            if (side.IsChoiceDone())
            {
                continue;
            }

            // Create a task to get this side's choice
            Task choiceTask = Task.Run(async () =>
            {
                try
                {
                    // Get the perspective for this side
                    BattlePerspective perspective = GetPerspectiveForSide(side.Id);

                    // Determine the request type
                    BattleRequestType requestType = RequestState switch
                    {
                        RequestState.TeamPreview => BattleRequestType.TeamPreview,
                        RequestState.Move => BattleRequestType.TurnStart,
                        RequestState.SwitchIn or RequestState.Switch => BattleRequestType.ForceSwitch,
                        _ => BattleRequestType.TurnStart,
                    };

                    // Request choice from the player
                    Choice choice = await PlayerController.RequestChoiceAsync(
                        side.Id,
                        side.ActiveRequest!,
                        requestType,
                        perspective,
                        cancellationToken
                    );

                    // Submit the choice to the battle
                    if (!Choose(side.Id, choice))
                    {
                        // Choice was invalid - the Choose method already sent an error
                        // For now, we'll auto-choose for the player
                        if (DisplayUi)
                        {
                            Debug($"Invalid choice from {side.Name}, auto-choosing");
                        }
                        side.AutoChoose();
                    }
                }
                catch (OperationCanceledException)
                {
                    // Timeout or cancellation - auto-choose for this player
                    if (DisplayUi)
                    {
                        Debug($"Timeout for {side.Name}, auto-choosing");
                    }
                    side.AutoChoose();
                }
                catch (Exception ex)
                {
                    if (DisplayUi)
                    {
                        Debug($"Error getting choice from {side.Name}: {ex.Message}, auto-choosing");
                    }
                    side.AutoChoose();
                }
            }, cancellationToken);

            choiceTasks.Add(choiceTask);
        }

        // Wait for all sides to make their choices
        if (choiceTasks.Count > 0)
        {
            await Task.WhenAll(choiceTasks);
        }

        // If all choices are done, commit them
        if (AllChoicesDone())
        {
            CommitChoices();
        }
    }
}