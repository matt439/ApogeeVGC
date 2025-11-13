using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    // Infinite request loop detection
    private int _consecutiveMoveRequests = 0;
    private const int MAX_CONSECUTIVE_MOVE_REQUESTS = 50;

    public void MakeRequest(RequestState? type = null)
    {
        Console.WriteLine($"[MakeRequest] Called with type={type}, current RequestState={RequestState}");

        // Don't make requests if battle has ended
        if (Ended)
        {
            Console.WriteLine("[MakeRequest] Battle has ended, not making new request");
            return;
        }

        // Detect infinite request loops
        if (type == RequestState.Move)
        {
            _consecutiveMoveRequests++;

            if (_consecutiveMoveRequests >= MAX_CONSECUTIVE_MOVE_REQUESTS)
            {
                Console.WriteLine($"[MakeRequest] ERROR: {MAX_CONSECUTIVE_MOVE_REQUESTS} consecutive move requests detected!");
                Console.WriteLine("[MakeRequest] This indicates an infinite loop - aborting battle");

                if (DisplayUi)
                {
                    Add("message", "Battle ended due to infinite request loop detection");
                }
                Tie();
                return;
            }
        }
        else
        {
            // Reset counter for non-move requests
            _consecutiveMoveRequests = 0;
        }

        // Update request state if provided, otherwise use current state
        if (type.HasValue)
        {
            RequestState = type.Value;
            Console.WriteLine($"[MakeRequest] Updated Battle RequestState to {RequestState}");

            // Clear all sides' choices when starting a new request
            // AND update each side's RequestState to match the battle
            foreach (Side side in Sides)
            {
                side.ClearChoice();
                side.RequestState = type.Value;
                Console.WriteLine($"[MakeRequest] Updated {side.Name} RequestState to {side.RequestState}");
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
        Console.WriteLine($"[MakeRequest] Generated {requests.Count} requests");

        // Assign requests to each side
        for (int i = 0; i < Sides.Count; i++)
        {
            Sides[i].ActiveRequest = requests[i];
            Console.WriteLine($"[MakeRequest] Assigned request to {Sides[i].Name}: {requests[i]?.GetType().Name}");
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
    // Don't check PokemonLeft here - a side can make moves if they have active Pokemon
    // PokemonLeft is for win condition checking, not for determining if moves can be made

 // Get move request data for each active, non-fainted Pokemon
             var activeData = side.Active
  .Where(pokemon => pokemon != null && !pokemon.Fainted)
             .Select(pokemon => pokemon!.GetMoveRequestData())
         .ToList();

     // Only create a move request if there are active Pokemon that can make moves
       if (activeData.Count > 0)
   {
     var moveRequest = new MoveRequest
         {
       Active = activeData,
  Side = side.GetRequestData()
               };

  requests[i] = moveRequest;
     }
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
        Console.WriteLine($"[Battle.Choose] Called for {sideId} with {input.Actions.Count} actions");
Side side = GetSide(sideId);

        if (!side.Choose(input))
        {
    Console.WriteLine($"[Battle.Choose] side.Choose returned false for {sideId}");
        if (string.IsNullOrEmpty(side.GetChoice().Error))
     {
    side.EmitChoiceError(
      $"Unknown error for choice: {input}. If you're not using a custom client," +
          $"please report this as a bug.");
    }
       return false;
        }

   Console.WriteLine($"[Battle.Choose] side.Choose returned true for {sideId}");

        if (!side.IsChoiceDone())
        {
     Console.WriteLine($"[Battle.Choose] Choice not done for {sideId}");
            side.EmitChoiceError($"Incomplete choice: {input} - missing other pokemon");
return false;
        }

        Console.WriteLine($"[Battle.Choose] Choice complete for {sideId}");

        // Check if all choices are done
        if (AllChoicesDone())
        {
            Console.WriteLine($"[Battle.Choose] All choices done, invoking completion callback");
    
  // Invoke the completion callback if one was provided
            // The callback is responsible for calling CommitChoices()
  Action? callback = _choicesCompletionCallback;
          _choicesCompletionCallback = null; // Clear it so it's only called once
            
     callback?.Invoke();
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
   Console.WriteLine("[CommitChoices] Starting");
      UpdateSpeed();

   // Reset consecutive request counter when choices are successfully committed
    _consecutiveMoveRequests = 0;

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

        // Log each side's choice to the input log and history
        foreach (Side side in Sides)
    {
            string? choice = side.GetChoice().ToString();
     if (!string.IsNullOrEmpty(choice))
            {
     InputLog.Add($"> {side.Id} {choice}");
       
         // Record choice in history
      History.RecordChoice(side.Id, choice);
 }
        }

        // Add each side's actions to the queue
     foreach (Side side in Sides)
        {
    Queue.AddChoice(side.Choice.Actions);
        }
        
      Console.WriteLine($"[CommitChoices] Added {Queue.List.Count} actions to queue");

        ClearRequest();

        // Sort the new actions by priority/speed
        Queue.Sort();

  // Append the old queue actions after the new ones
        Queue.List.AddRange(oldQueue);
   
     Console.WriteLine($"[CommitChoices] Total queue size: {Queue.List.Count}");

        // Clear request state
        RequestState = RequestState.None;
        foreach (Side side in Sides)
        {
         side.ActiveRequest = null;
        }

        // Continue executing the turn
        Console.WriteLine("[CommitChoices] Calling TurnLoop");
      TurnLoop();
        Console.WriteLine("[CommitChoices] TurnLoop returned");

        // Check if battle ended during TurnLoop
        if (Ended)
        {
   Console.WriteLine("[CommitChoices] Battle ended during TurnLoop, exiting");
            return;
        }

      // Workaround for tests - send updates if log is getting large
        if (Log.Count - SentLogPos > 500)
      {
  SendUpdates();
   }
    }

// Callback to invoke when all choices are received
    private Action? _choicesCompletionCallback;

    /// <summary>
    /// Emits choice request events for all sides that need to make choices.
    /// The battle will pause here until all choices are submitted via Choose().
    /// When all choices are received, the completion callback will be invoked.
    /// </summary>
  /// <param name="onComplete">Callback to invoke when all choices have been received</param>
    public void RequestPlayerChoices(Action? onComplete = null)
 {
   // Verify that we have active requests
  if (Sides.Any(side => side.ActiveRequest == null))
   {
   throw new InvalidOperationException("Cannot request choices from players: Some sides have no active request");
    }

      // Reset the wait handle - Battle will block until this is set
        _choiceWaitHandle.Reset();
  Console.WriteLine("[RequestPlayerChoices] Wait handle reset - Battle will block");

        // Wrap the callback to signal the wait handle after execution
   Action wrappedCallback = () =>
     {
    Console.WriteLine("[RequestPlayerChoices] Callback invoked, executing original callback");
     onComplete?.Invoke();
 Console.WriteLine("[RequestPlayerChoices] Setting wait handle - Battle will continue");
 _choiceWaitHandle.Set();
        };

        // Store the wrapped completion callback
        _choicesCompletionCallback = wrappedCallback;

        // Emit choice request events for each side that needs to make a choice
   foreach (Side side in Sides)
        {
 // Skip sides that don't need to make a choice (already done)
if (side.IsChoiceDone())
  {
  continue;
   }

      // Determine the request type
BattleRequestType requestType = RequestState switch
{
RequestState.TeamPreview => BattleRequestType.TeamPreview,
    RequestState.Move => BattleRequestType.TurnStart,
    RequestState.SwitchIn or RequestState.Switch => BattleRequestType.ForceSwitch,
  _ => BattleRequestType.TurnStart,
      };

          // Emit the choice request event - external handler (Simulator) will handle async player interaction
       RequestPlayerChoice(side.Id, side.ActiveRequest!, requestType);
        }

    // Battle execution continues only when:
     // 1. All choices are submitted via Choose()
   // 2. Choose() detects AllChoicesDone()
   // 3. The completion callback is invoked (if provided)
   // 4. CommitChoices() is called to process the choices
        // 5. The wait handle is set, allowing Battle.Start() or TurnLoop() to continue
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
}