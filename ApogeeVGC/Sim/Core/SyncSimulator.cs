using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.SideClasses;

namespace ApogeeVGC.Sim.Core;

/// <summary>
/// Synchronous battle simulator for AI training (MCTS).
/// No async/threading complexity - perfect for fast rollouts.
/// </summary>
public class SyncSimulator : IBattleController
{
    public Battle? Battle { get; set; }
    public IPlayer? Player1 { get; set; }
    public IPlayer? Player2 { get; set; }
    public bool PrintDebug { get; set; }

    public BattlePerspective GetCurrentPerspective(SideId sideId)
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        return Battle.GetPerspectiveForSide(sideId);
    }

    public void ClearMessages()
    {
        // No-op for sync simulator
    }

    public SimulatorResult Run(Library library, BattleOptions battleOptions, bool printDebug = true)
    {
        Battle = new Battle(battleOptions, library);
        Player1 = CreatePlayer(SideId.P1, battleOptions.Player1Options);
        Player2 = CreatePlayer(SideId.P2, battleOptions.Player2Options);
        PrintDebug = printDebug;

        // Validate teams against format rules
        var validator = new TeamValidator(library, Battle.Format);
        ValidateTeams(validator, battleOptions, printDebug);

        // Subscribe to Battle events
        Battle.ChoiceRequested += OnChoiceRequested;
        Battle.UpdateRequested += OnUpdateRequested;
        Battle.BattleEnded += OnBattleEnded;

        try
        {
            // Start the battle - this will set up team preview or first turn
            Battle.Start();

            // Main battle loop - keep processing until battle ends
            // Flow:
            // 1. Battle.Start() or TurnLoop() sets RequestState != None when choices are needed
            // 2. We call RequestPlayerChoices() which emits events synchronously
            // 3. Event handlers call GetChoiceSync() and Battle.Choose()
            // 4. When all choices are submitted, Choose() calls CommitChoices() which calls TurnLoop()
            // 5. TurnLoop() processes actions and may set a new RequestState or end the battle
            // 6. Control returns to us, and we loop to check for new requests

            while (!Battle.Ended)
            {
                // Check if there are pending requests
                if (Battle.RequestState != RequestState.None)
                {
                    // Double-check that battle hasn't ended before requesting choices
                    // This can happen if GetRequests() detects a win/loss condition
                    if (Battle.Ended)
                    {
                        break;
                    }

                    // Emit request events to get choices from players
                    // This will trigger OnChoiceRequested for each side that needs to make a choice
                    // In synchronous mode, all event handlers run immediately within this call
                    Battle.RequestPlayerChoices();

                    // After RequestPlayerChoices() completes, all OnChoiceRequested events have been
                    // processed synchronously. The handlers called GetChoiceSync() and Battle.Choose().
                    // When the last side submitted their choice, Choose() called CommitChoices() 
                    // which called TurnLoop() to process the actions.
                    //
                    // During TurnLoop():
                    // - Actions are processed (moves, switches, etc.)
                    // - Turn may complete and EndTurn() called, which sets RequestState = Move again
                    // - Or a mid-turn switch is needed, which sets RequestState = SwitchIn
                    // - Or the battle ends, which sets Ended = true
                    //
                    // After all this synchronous processing, TurnLoop returns to CommitChoices,
                    // which returns to the event handler, which returns to RequestPlayerChoices,
                    // which returns to us here.
                    //
                    // Note: RequestState may be the SAME value as before (e.g., Move after turn ends)
                    // but this is normal - it means a new turn started and needs new move choices.
                    // The loop will continue and RequestPlayerChoices will be called again.
                    //
                    // The loop only exits when:
                    // - Battle.Ended becomes true
                    // - RequestState becomes None (shouldn't happen if battle isn't ended)
                }
                else
                {
                    // No pending request - exit loop
                    // Either battle ended or no more requests
                    break;
                }
            }

            return DetermineWinner();
        }
        catch (Exception ex)
        {
            if (PrintDebug)
            {
                Console.WriteLine($"Battle error: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine(
                        $"Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                }

                Console.WriteLine("Stack trace (last 10 frames):");
                var frames = ex.StackTrace?.Split('\n').Take(10);
                if (frames != null)
                {
                    foreach (var frame in frames)
                    {
                        Console.WriteLine($"  {frame.Trim()}");
                    }
                }
            }

            throw;
        }
        finally
        {
            // Unsubscribe from events
            if (Battle != null)
            {
                Battle.ChoiceRequested -= OnChoiceRequested;
                Battle.UpdateRequested -= OnUpdateRequested;
                Battle.BattleEnded -= OnBattleEnded;
            }
        }
    }

    /// <summary>
    /// Event handler for when Battle requests a choice from a player.
    /// Synchronously gets the choice and submits it to the battle.
    /// Retries up to MAX_RETRY_ATTEMPTS if validation fails.
    /// </summary>
    private void OnChoiceRequested(object? sender, BattleChoiceRequestEventArgs e)
    {
        const int MAX_RETRY_ATTEMPTS = 100;
        
        IPlayer player = GetPlayer(e.SideId);
        Side side = Battle!.Sides.First(s => s.Id == e.SideId);
        
        int retryCount = 0;
        
        while (retryCount < MAX_RETRY_ATTEMPTS)
        {
            // IMPORTANT: If this is a retry (not the first attempt), regenerate the request
            // to get fresh state data. The cached request may be stale if partial validation
            // succeeded but then failed, leaving Pokemon state modified.
            if (retryCount > 0 && e.Request is SwitchRequest)
            {
                // Regenerate the switch request with current Active Pokemon state
                var freshSwitchTable = side.Active
                    .Select(pokemon => pokemon == null || pokemon.SwitchFlag.IsTrue())
                    .ToList();
                
                // Only regenerate if there are switches needed
                if (freshSwitchTable.Any(flag => flag))
                {
                    var freshRequest = new SwitchRequest
                    {
                        ForceSwitch = freshSwitchTable,
                        Side = side.GetRequestData(),
                    };
                    
                    e = new BattleChoiceRequestEventArgs
                    {
                        SideId = e.SideId,
                        Request = freshRequest,
                        RequestType = e.RequestType,
                        Perspective = e.Perspective
                    };
                    
                    // Update the side's active request with the fresh data
                    side.ActiveRequest = freshRequest;
                }
            }
            
            // Get choice synchronously
            Choice choice = player.GetChoiceSync(e.Request, e.RequestType, e.Perspective);

            // If the choice is empty, use AutoChoose
            if (choice.Actions.Count == 0)
            {
                side.AutoChoose();
                choice = side.GetChoice();
            }

            // Submit the choice immediately (synchronous)
            bool success = Battle.Choose(e.SideId, choice);
            
            if (success)
            {
                // Success - exit the retry loop
                if (retryCount > 0 && PrintDebug)
                {
                    Console.WriteLine($"[SyncSimulator] Choice succeeded for {e.SideId} after {retryCount} retries");
                }
                return;
            }
            
            // Choice validation failed - log and retry
            retryCount++;
            
            // ALWAYS log validation failures (not just when PrintDebug is true)
            Console.WriteLine($"\n[SyncSimulator] ===== VALIDATION FAILURE #{retryCount} for {e.SideId} =====");
            Console.WriteLine($"[SyncSimulator] Error: {side.GetChoice().Error}");
            Console.WriteLine($"[SyncSimulator] RequestState: {side.RequestState}, RequestType: {e.RequestType}");
            Console.WriteLine($"[SyncSimulator] Active.Count: {side.Active.Count}");
            for (int i = 0; i < side.Active.Count; i++)
            {
                var p = side.Active[i];
                Console.WriteLine($"[SyncSimulator]   Active[{i}] = {(p == null ? "NULL" : $"{p.Name}, SwitchFlag={p.SwitchFlag}")}");
            }
            Console.WriteLine($"[SyncSimulator] ForcedSwitchesLeft: {side.Choice.ForcedSwitchesLeft}");
            Console.WriteLine($"[SyncSimulator] Side.Choice.Actions.Count: {side.Choice.Actions.Count}");
            Console.WriteLine($"[SyncSimulator] Input.Actions.Count: {choice.Actions.Count}");
            for (int i = 0; i < choice.Actions.Count; i++)
            {
                var action = choice.Actions[i];
                Console.WriteLine($"[SyncSimulator]   InputAction[{i}]: Type={action.Choice}, Index={action.Index}");
            }
            if (e.Request is SwitchRequest sr)
            {
                Console.WriteLine($"[SyncSimulator] ForceSwitch: [{string.Join(", ", sr.ForceSwitch)}]");
            }
            Console.WriteLine($"[SyncSimulator] ============================================\n");
        }
        
        // Max retries exceeded - throw exception with diagnostics
        string lastError = side.GetChoice().Error;
        throw new InvalidOperationException(
            $"Failed to generate valid choice for {e.SideId} after {MAX_RETRY_ATTEMPTS} attempts. " +
            $"Last error: {lastError}. " +
            $"RequestType: {e.RequestType}, RequestState: {side.RequestState}");
    }

    /// <summary>
    /// Event handler for UI updates from the battle.
    /// </summary>
    private void OnUpdateRequested(object? sender, BattleUpdateEventArgs e)
    {
        IPlayer player = GetPlayer(e.SideId);
        player.UpdateEvents(e.Events);
    }

    /// <summary>
    /// Event handler for battle end notification.
    /// </summary>
    private void OnBattleEnded(object? sender, BattleEndedEventArgs e)
    {
        // Battle ended - no action needed, just for notification
    }

    private SimulatorResult DetermineWinner()
    {
        // Check if we have a winner
        if (!string.IsNullOrEmpty(Battle!.Winner))
        {
            // Winner is stored as side name (e.g., "Player 1", "Player 2")
            // Compare against both P1 and P2 side names to determine the winner
            string p1Name = Battle.P1.Name;
            string p2Name = Battle.P2.Name;

            if (Battle.Winner.Equals(p1Name, StringComparison.OrdinalIgnoreCase))
            {
                return SimulatorResult.Player1Win;
            }
            else if (Battle.Winner.Equals(p2Name, StringComparison.OrdinalIgnoreCase))
            {
                return SimulatorResult.Player2Win;
            }

            // Fallback: also check for side ID strings ("p1", "p2") for backwards compatibility
            if (Battle.Winner.Equals("p1", StringComparison.OrdinalIgnoreCase))
            {
                return SimulatorResult.Player1Win;
            }
            else if (Battle.Winner.Equals("p2", StringComparison.OrdinalIgnoreCase))
            {
                return SimulatorResult.Player2Win;
            }

            // Unknown winner format - log error and treat as tie
            Console.WriteLine($"WARNING: Unknown winner format: '{Battle.Winner}'");
            return SimulatorResult.Tie;
        }

        // No clear winner - use tiebreak
        return DetermineTiebreakWinner();
    }

    private SimulatorResult DetermineTiebreakWinner()
    {
        // Use Battle's tiebreak logic which checks:
        // 1. Remaining Pokemon count
        // 2. HP percentage
        // 3. Total HP
        Battle!.Tiebreak();

        // After tiebreak, check if a winner was determined
        if (!string.IsNullOrEmpty(Battle.Winner))
        {
            return DetermineWinner();
        }

        // If tiebreak didn't determine a winner, it's a true tie
        return SimulatorResult.Tie;
    }

    /// <summary>
    /// Validates both player teams against the format rules.
    /// Throws InvalidOperationException if either team is invalid.
    /// </summary>
    private static void ValidateTeams(TeamValidator validator, BattleOptions battleOptions, bool printDebug)
    {
        var player1Result = validator.ValidateTeam(battleOptions.Player1Options.Team);
        var player2Result = validator.ValidateTeam(battleOptions.Player2Options.Team);

        var allProblems = new List<string>();

        if (!player1Result.IsValid)
        {
            allProblems.Add($"Player 1 ({battleOptions.Player1Options.Name}) team validation failed:");
            foreach (var problem in player1Result.Problems)
            {
                allProblems.Add($"  - {problem}");
            }
        }

        if (!player2Result.IsValid)
        {
            allProblems.Add($"Player 2 ({battleOptions.Player2Options.Name}) team validation failed:");
            foreach (var problem in player2Result.Problems)
            {
                allProblems.Add($"  - {problem}");
            }
        }

        if (allProblems.Count > 0)
        {
            var errorMessage = string.Join(Environment.NewLine, allProblems);
            if (printDebug)
            {
                Console.WriteLine($"[SyncSimulator] Team validation failed:{Environment.NewLine}{errorMessage}");
            }

            throw new InvalidOperationException($"Team validation failed:{Environment.NewLine}{errorMessage}");
        }

        if (printDebug)
        {
            Console.WriteLine("[SyncSimulator] Team validation passed for both players");
        }
    }

    private IPlayer CreatePlayer(SideId sideId, PlayerOptions options)
    {
        return options.Type switch
        {
            Player.PlayerType.Random => new PlayerRandom(sideId, options, this),
            Player.PlayerType.Console => throw new NotSupportedException(
                "Console player cannot be used in synchronous mode"),
            Player.PlayerType.Gui => throw new NotSupportedException(
                "GUI player cannot be used in synchronous mode"),
            Player.PlayerType.Mcts => throw new NotImplementedException(
                "MCTS player not implemented yet"),
            _ => throw new ArgumentOutOfRangeException($"Unknown player type: {options.Type}"),
        };
    }

    private IPlayer GetPlayer(SideId sideId)
    {
        return sideId switch
        {
            SideId.P1 => Player1 ??
                         throw new InvalidOperationException("Player 1 is not initialized"),
            SideId.P2 => Player2 ??
                         throw new InvalidOperationException("Player 2 is not initialized"),
            _ => throw new ArgumentOutOfRangeException(nameof(sideId), $"Invalid SideId: {sideId}"),
        };
    }

    // Force win/lose/tie methods for testing purposes

    /// <summary>
    /// Forces the specified side to win the battle.
    /// Useful for testing and debugging scenarios.
    /// </summary>
    public void ForceWin(SideId sideId)
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        Side side = Battle.Sides.First(s => s.Id == sideId);
        Battle.Win(side);
    }

    /// <summary>
    /// Forces a tie in the battle.
    /// Useful for testing and debugging scenarios.
    /// </summary>
    public void ForceTie()
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        Battle.Win((Side?)null);
    }

    /// <summary>
    /// Forces the specified side to lose the battle.
    /// Useful for testing and debugging scenarios.
    /// </summary>
    public void ForceLose(SideId sideId)
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        Side side = Battle.Sides.First(s => s.Id == sideId);
        Battle.Lose(side);
    }
}