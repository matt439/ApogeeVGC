using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    // Infinite request loop detection
    private int _consecutiveMoveRequests;
    private const int MaxConsecutiveMoveRequests = 50;

    public void MakeRequest(RequestState? type = null)
    {
      if (DebugMode)
        {
   Console.WriteLine($"[MakeRequest] ENTRY: queue size = {Queue.List.Count}, type = {type}");
        Debug($"MakeRequest called with type={type}, current RequestState={RequestState}");
      }

        // Don't make requests if battle has ended
        if (Ended)
        {
            if (DebugMode)
            {
                Debug("MakeRequest: Battle has ended, not making new request");
            }

            return;
        }

        // Detect infinite request loops
        if (type == RequestState.Move)
        {
            _consecutiveMoveRequests++;

            if (_consecutiveMoveRequests >= MaxConsecutiveMoveRequests)
            {
                if (DebugMode)
                {
                    Debug(
                        $"ERROR: {MaxConsecutiveMoveRequests} consecutive move requests detected!");
                    Debug("This indicates an infinite loop - aborting battle");
                }

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
        
        // Special validation for SwitchIn requests - check if any switches are actually possible
        if (type == RequestState.SwitchIn)
     {
            bool anyValidSwitches = false;
   
            foreach (Side side in Sides)
            {
   // Check if this side has any Pokemon that need to switch
     bool hasSwitchNeeded = side.Active.Any(p => p?.SwitchFlag.IsTrue() == true);
         
                if (hasSwitchNeeded)
            {
   // Check if there are any Pokemon available to switch in
              int availableSwitches = CanSwitch(side);
   
          // Check for Revival Blessing (special case)
         bool hasRevivalBlessing = side.Active.Any(p => 
           p != null && side.GetSlotCondition(p.Position, ConditionId.RevivalBlessing) != null);
     
    if (availableSwitches > 0 || hasRevivalBlessing)
        {
   anyValidSwitches = true;
       }
       else
         {
   // This side needs to switch but has no Pokemon available
        // This means they've lost - no need to make a switch request
     if (DebugMode)
           {
         Debug($"{side.Name} needs to switch but has no Pokemon available - checking win condition");
              }
   }
   }
   }
       
    // If no valid switches are possible, the battle should end
            // The win condition check should have already determined the winner
            if (!anyValidSwitches)
     {
     if (DebugMode)
         {
            Debug("No valid switches possible for any side, checking win conditions");
  }
    
      // Check win conditions - this will end the battle if appropriate
                foreach (Side side in Sides)
    {
         if (side.PokemonLeft <= 0)
       {
     if (DebugMode)
         {
    Debug($"{side.Name} has no Pokemon left, losing");
           }
      
        Lose(side);
       return;
       }
                }
      
       // If we get here, something is wrong - tie the battle
         if (DebugMode)
         {
     Debug("No switches possible but no clear winner - tying battle");
          }
  
                Tie();
      return;
       }
        }

        // Update request state if provided, otherwise use current state
    if (type.HasValue)
        {
RequestState = type.Value;

        if (DebugMode)
            {
     Debug($"Updated Battle RequestState to {RequestState}");
            }

            // Clear all sides' choices when starting a new request
      // AND update each side's RequestState to match the battle
   foreach (Side side in Sides)
            {
side.ClearChoice();
        side.RequestState = type.Value;

        if (DebugMode)
   {
               Debug($"Updated {side.Name} RequestState to {side.RequestState}");
    }
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

        if (DebugMode)
        {
            Debug($"Generated {requests.Count} requests");
        }

   // Check if any sides got WaitRequest because they have no valid actions
 // This can happen when all active Pokemon are fainted during a move request
 // In this case, we need to check if the battle should end
        if (type.Value == RequestState.Move)
        {
            for (int i = 0; i < requests.Count; i++)
 {
       if (requests[i] is WaitRequest)
    {
    Side side = Sides[i];
      
      // Check if this side has fainted active Pokemon
     bool hasFaintedActive = side.Active.Any(p => p is { Fainted: true });
   
 if (hasFaintedActive)
         {
     if (DebugMode)
    {
       Debug($"WARNING: {side.Name} received WaitRequest but has fainted active Pokemon");
    Debug("This indicates the battle should have ended or switched before requesting moves");
        }
         
     // Check if battle should end
     CheckWin();
    
     if (Ended)
   {
         if (DebugMode)
         {
      Debug("Battle ended after checking win conditions");
  }
         return;
 }
   }
 }
   }
        }

        // Assign requests to each side
        for (int i = 0; i < Sides.Count; i++)
        {
            Sides[i].ActiveRequest = requests[i];

            if (DebugMode)
            {
                Debug($"Assigned request to {Sides[i].Name}: {requests[i].GetType().Name}");
            }
        }

        // Verify that choices aren't already done (would indicate a bug)
        if (Sides.All(side => side.IsChoiceDone()))
        {
      throw new InvalidOperationException("Choices are done immediately after a request");
        }

  if (DebugMode)
     {
   Console.WriteLine($"[MakeRequest] EXIT: queue size = {Queue.List.Count}");
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

        if (DebugMode)
        {
 Console.WriteLine($"[GetRequests] Called with type={type}");
    Console.WriteLine($"[GetRequests] Checking {Sides.Count} sides");
        }

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
    .Where(pokemon => pokemon is { Fainted: false })
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
          else
 {
    // If a side has no active non-fainted Pokemon during a move request,
    // this indicates either:
    // 1. The battle should have ended (this side has no Pokemon left)
     // 2. A switch should have been requested before this point
           // 
     // This is an error state - the battle logic should have called
// FaintMessages() which would have checked win conditions and/or
    // set switch flags, leading to a SwitchIn request instead of a Move request.
     //
          // To prevent an endless loop, we'll check win conditions in MakeRequest.
            if (DebugMode)
    {
       Debug($"WARNING: {side.Name} has no active non-fainted Pokemon during move request phase");
   Debug($"  PokemonLeft: {side.PokemonLeft}");
Debug($"  This usually indicates a bug in the fainting/switching logic");
    }
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
        if (DebugMode)
        {
            Console.WriteLine(
         $"[Battle.Choose] Called for {sideId} with {input.Actions.Count} actions");
    Debug($"Battle.Choose called for {sideId} with {input.Actions.Count} actions");
        }

     Side side = GetSide(sideId);

    if (!side.Choose(input))
        {
         if (DebugMode)
      {
            Debug($"side.Choose returned false for {sideId}");
            }

    if (string.IsNullOrEmpty(side.GetChoice().Error))
      {
      side.EmitChoiceError(
           $"Unknown error for choice: {input}. If you're not using a custom client," +
         $"please report this as a bug.");
        }

    return false;
        }

  if (DebugMode)
  {
            Debug($"side.Choose returned true for {sideId}");
        }

      if (!side.IsChoiceDone())
        {
   if (DebugMode)
    {
       Debug($"Choice not done for {sideId}");
 }

            side.EmitChoiceError($"Incomplete choice: {input} - missing other pokemon");
       return false;
        }

        if (DebugMode)
        {
            Console.WriteLine(
             $"[Battle.Choose] Choice complete for {sideId}, checking if all done...");
        Debug($"Choice complete for {sideId}");
        }

      // Check if all choices are done
        if (AllChoicesDone())
        {
         if (DebugMode)
       {
              Console.WriteLine("[Battle.Choose] All choices done! Calling CommitChoices");
     Debug("All choices done, calling CommitChoices");
  }

       // All choices received - process them and continue the turn
       CommitChoices();
        }
        else
      {
if (DebugMode)
       {
    Console.WriteLine("[Battle.Choose] Not all choices done yet");
       }
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
        if (DebugMode)
      {
        Debug("CommitChoices starting");
        Console.WriteLine($"[CommitChoices] Starting, queue size = {Queue.List.Count}");
    if (Queue.List.Count > 0)
    {
    Console.WriteLine($"[CommitChoices] Current queue:");
            for (int i = 0; i < Queue.List.Count; i++)
       {
      Console.WriteLine($"  [{i}] {Queue.List[i].Choice}");
        }
        }
        }

        UpdateSpeed();

    // Reset consecutive request counter when choices are successfully committed
  _consecutiveMoveRequests = 0;

        // Sometimes you need to make switch choices mid-turn (e.g. U-turn,
        // fainting). When this happens, the rest of the turn is saved (and not
   // re-sorted), but the new switch choices are sorted and inserted before
        // the rest of the turn.
        var oldQueue = Queue.List.ToList(); // Create a copy of the current queue
 if (DebugMode)
   {
    Console.WriteLine($"[CommitChoices] Saved oldQueue with {oldQueue.Count} items");
        }

        Queue.Clear();
        if (DebugMode)
     {
        Console.WriteLine($"[CommitChoices] Cleared queue");
        }

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

   if (DebugMode)
        {
  Debug($"Added {Queue.List.Count} actions to queue");
  Console.WriteLine($"[CommitChoices] After adding side actions, queue size = {Queue.List.Count}");
   }

  ClearRequest();

      // Sort the new actions by priority/speed
        Queue.Sort();

        if (DebugMode)
 {
 Console.WriteLine($"[CommitChoices] After sorting, queue size = {Queue.List.Count}");
 }

        // Append the old queue actions after the new ones
   Queue.List.AddRange(oldQueue);

        if (DebugMode)
      {
        Console.WriteLine($"[CommitChoices] After restoring oldQueue, queue size = {Queue.List.Count}");
 if (Queue.List.Count > 0)
        {
 Console.WriteLine($"[CommitChoices] Final queue:");
            for (int i = 0; i < Math.Min(20, Queue.List.Count); i++)
     {
  Console.WriteLine($"  [{i}] {Queue.List[i].Choice}");
   }
        }
      Debug($"Total queue size: {Queue.List.Count}");
  }

        // Clear request state
        RequestState = RequestState.None;
        foreach (Side side in Sides)
     {
          side.ActiveRequest = null;
  }

      // Continue executing the turn
        if (DebugMode)
 {
            Debug("Calling TurnLoop");
    }

     TurnLoop();

        if (DebugMode)
      {
 Debug("TurnLoop returned");
        }

        // In synchronous mode, do NOT call RequestPlayerChoices here
        // This would cause infinite recursion since the simulator handles requests synchronously
    // Instead, return and let the caller (simulator) check for pending requests

        // Check if battle ended during TurnLoop
        if (Ended)
  {
   if (DebugMode)
            {
     Debug("Battle ended during TurnLoop, exiting");
    }

            return;
        }

        // Workaround for tests - send updates if log is getting large
 if (Log.Count - SentLogPos > 500)
    {
         SendUpdates();
        }
    }

  /// <summary>
/// Emits choice request events for all sides that need to make choices.
    /// The battle will pause (return) and wait for Simulator to call Choose() with all choices.
 /// When all choices are received, CommitChoices() will be called automatically.
 /// </summary>
    public void RequestPlayerChoices()
    {
        // Verify that we have active requests
        if (Sides.Any(side => side.ActiveRequest == null))
        {
    throw new InvalidOperationException(
  "Cannot request choices from players: Some sides have no active request");
        }

   // Send all battle updates to players before requesting choices
// This ensures players see all messages (moves, damage, etc.) before making decisions
        UpdateAllPlayersUi();

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

 // Battle returns immediately - doesn't wait
 // Simulator will call Choose() -> CommitChoices() to continue
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
  side.EmitChoiceError(
    "Can't undo: A trapping/disabling effect would cause undo to leak information");
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