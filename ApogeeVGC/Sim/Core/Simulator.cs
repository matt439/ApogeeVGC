using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.SideClasses;
using System.Threading.Channels;

namespace ApogeeVGC.Sim.Core;

public class Simulator : IBattleController
{
    public Battle? Battle { get; set; }
    public IPlayer? Player1 { get; set; }
    public IPlayer? Player2 { get; set; }
    public bool PrintDebug { get; set; }

    /// <summary>
    /// Log of all choices and commands submitted during the battle.
    /// Useful for replay functionality and debugging.
    /// </summary>
    public List<string> InputLog { get; } = [];

    // Channels for async coordination
    private Channel<ChoiceResponse>? _choiceResponseChannel;
    private readonly Dictionary<SideId, Task> _pendingChoiceTasks = new();
    private readonly SemaphoreSlim _battleSemaphore = new(0, 1); // Start at 0 - Battle waits
    private CancellationTokenSource? _cancellationTokenSource;

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
        Console.WriteLine("[Simulator.ClearMessages] Called");
        // Directly call ClearMessages on GUI players' BattleGame windows
        if (Player1 is PlayerGui gui1)
        {
            Console.WriteLine(
                $"[Simulator.ClearMessages] Calling ClearMessages for Player1 (GuiWindow:" +
                $"{gui1.GuiWindow.GetHashCode()})");
            gui1.GuiWindow.ClearMessages();
        }

        if (Player2 is PlayerGui gui2)
        {
            Console.WriteLine(
                $"[Simulator.ClearMessages] Calling ClearMessages for Player2 (GuiWindow:" +
                $"{gui2.GuiWindow.GetHashCode()})");
            gui2.GuiWindow.ClearMessages();
        }

        Console.WriteLine("[Simulator.ClearMessages] Completed");
    }

    public async Task<SimulatorResult> RunAsync(Library library, BattleOptions battleOptions,
        bool printDebug = true)
    {
        Battle = new Battle(battleOptions, library);
        Player1 = CreatePlayer(SideId.P1, battleOptions.Player1Options);
        Player2 = CreatePlayer(SideId.P2, battleOptions.Player2Options);
        PrintDebug = printDebug;

        // Subscribe to Battle events
        Battle.ChoiceRequested += OnChoiceRequested;
        Battle.UpdateRequested += OnUpdateRequested;
        Battle.BattleEnded += OnBattleEnded;
        Battle.ClearMessagesRequested += OnClearMessagesRequested;

        // Create channel for choice responses
        _choiceResponseChannel = Channel.CreateUnbounded<ChoiceResponse>();
        _cancellationTokenSource = new CancellationTokenSource();

        if (PrintDebug)
        {
            Console.WriteLine("Starting battle simulation...");
        }

        try
        {
            // Set a reasonable timeout for the entire battle
            _cancellationTokenSource.CancelAfter(
                TimeSpan.FromSeconds(300)); // Shorter timeout for testing

            // Start the battle loop task
            Task battleLoopTask = Task.Run(() => RunBattleLoop(), _cancellationTokenSource.Token);

            // Start processing choice responses
            Task choiceProcessingTask = ProcessChoiceResponsesAsync(_cancellationTokenSource.Token);

            // Wait for either the battle to end or a timeout
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30), _cancellationTokenSource.Token);
            var completedTask = await Task.WhenAny(
                battleLoopTask,
                choiceProcessingTask,
                timeoutTask
            );

            string taskName = completedTask == battleLoopTask ? "battleLoop" :
                completedTask == choiceProcessingTask ? "choiceProcessing" : "timeout";
            Console.WriteLine($"[Simulator.RunAsync] Completed task: {taskName}");
            Console.WriteLine($"[Simulator.RunAsync] Battle.Ended = {Battle.Ended}");

            // Check if we timed out
            if (completedTask == timeoutTask)
            {
                Console.WriteLine("[Simulator.RunAsync] Battle timed out - forcing end");

                // Force battle to end if it hasn't already
                if (!Battle.Ended)
                {
                    Console.WriteLine("[Simulator.RunAsync] Calling Tiebreak to end battle");
                    Battle.Tiebreak();
                    Console.WriteLine(
                        $"[Simulator.RunAsync] After Tiebreak: Battle.Ended = {Battle.Ended}");
                }
            }
            else if (completedTask == battleLoopTask)
            {
                Console.WriteLine("[Simulator.RunAsync] Battle loop completed");
            }
            else if (completedTask == choiceProcessingTask)
            {
                Console.WriteLine("[Simulator.RunAsync] Choice processing completed");
            }

            // Wait a bit for any final processing
            await Task.Delay(100);

            if (PrintDebug)
            {
                Console.WriteLine($"Battle has ended: {Battle.Ended}");
            }

            return DetermineWinner();
        }
        catch (OperationCanceledException)
        {
            if (PrintDebug)
            {
                Console.WriteLine("Battle was cancelled due to timeout");
            }

            // Force battle to end if it hasn't
            if (!Battle.Ended)
            {
                Battle.Tiebreak();
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

                Console.WriteLine($"Stack trace (last 10 frames):");
                var frames = ex.StackTrace?.Split('\n').Take(10);
                if (frames != null)
                {
                    foreach (string frame in frames)
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
                Battle.ClearMessagesRequested -= OnClearMessagesRequested;
            }

            // Complete the channel if not already completed
            try
            {
                _choiceResponseChannel?.Writer.Complete();
            }
            catch (ChannelClosedException)
            {
                // Already closed, that's fine
            }

            _cancellationTokenSource?.Dispose();
        }
    }

    /// <summary>
    /// Runs the asynchronous battle loop.
    /// Checks for pending requests after Battle.Start() and emits them.
    /// Waits for the battle to actually end before returning.
    /// </summary>
    private void RunBattleLoop()
    {
        try
        {
            Console.WriteLine("[Simulator.RunBattleLoop] Starting battle");

            // Start the battle - this will set up team preview or initial turn
            Battle!.Start();

            Console.WriteLine(
                $"[Simulator.RunBattleLoop] Battle.Start() returned, RequestState: {Battle.RequestState}");

            // After Start() returns, check if there's a pending request and emit it
            // This handles team preview or initial switch-in requests
            if (Battle.RequestState != RequestState.None && !Battle.Ended)
            {
                Console.WriteLine(
                    $"[Simulator.RunBattleLoop] Emitting initial request: {Battle.RequestState}");
                Battle.RequestPlayerChoices();
            }

            // Wait for the battle to actually end
            // The battle ends when OnBattleEnded is fired, which completes the choice response channel
            // ProcessChoiceResponsesAsync will then complete, and this loop can exit
            Console.WriteLine("[Simulator.RunBattleLoop] Waiting for battle to end...");
            while (!Battle.Ended)
            {
                Thread.Sleep(100); // Check every 100ms
            }

            Console.WriteLine("[Simulator.RunBattleLoop] Battle has ended");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Simulator.RunBattleLoop] Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Event handler for when Battle requests a choice from a player.
    /// Starts an async task to get the choice from the player.
    /// </summary>
    private void OnChoiceRequested(object? sender, BattleChoiceRequestEventArgs e)
    {
        Console.WriteLine($"[Simulator.OnChoiceRequested] Choice requested for {e.SideId}");

        // Start an async task to get the choice
        Task choiceTask = Task.Run(async () =>
        {
            try
            {
                IPlayer player = GetPlayer(e.SideId);

                // Request choice from the player
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                Choice choice = await player.GetNextChoiceAsync(
                    e.Request,
                    e.RequestType,
                    e.Perspective,
                    cts.Token
                );

                Console.WriteLine($"[Simulator.OnChoiceRequested] Received choice for {e.SideId}");

                // If the choice is empty (no actions), use AutoChoose to fill it
                // THREAD SAFETY: AutoChoose is called from async task but is safe because:
                // 1. It only modifies the Side's Choice object (Side.Choice = new Choice {...})
                // 2. Each Side has its own Choice object
                // 3. The Battle loop is blocked waiting for choices (not reading Side state)
                // 4. No other tasks are modifying this Side's Choice concurrently
                if (choice.Actions.Count == 0)
                {
                    Console.WriteLine(
                        $"[Simulator.OnChoiceRequested] Empty choice received for {e.SideId}, using AutoChoose");
                    Side side = Battle!.Sides.First(s => s.Id == e.SideId);
                    Console.WriteLine(
                        $"[Simulator.OnChoiceRequested] Before AutoChoose: Choice.Actions.Count = {side.GetChoice().Actions.Count}");
                    side.AutoChoose();
                    Console.WriteLine(
                        $"[Simulator.OnChoiceRequested] After AutoChoose: Choice.Actions.Count = {side.GetChoice().Actions.Count}");
                    choice = side.GetChoice();
                    Console.WriteLine(
                        $"[Simulator.OnChoiceRequested] Final choice.Actions.Count = {choice.Actions.Count}");
                }

                // Send the choice response directly - let Battle.Choose() handle empty choices
                // Do NOT call side.AutoChoose() here as it accesses battle state from async task
                await _choiceResponseChannel!.Writer.WriteAsync(new ChoiceResponse
                {
                    SideId = e.SideId,
                    Choice = choice,
                    Success = true
                });

                // Log the choice for replay
                LogChoice(e.SideId, choice);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(
                    $"[Simulator.OnChoiceRequested] Timeout for {e.SideId}, auto-choosing");

                // Auto-choose on timeout
                Side side = Battle!.Sides.First(s => s.Id == e.SideId);
                side.AutoChoose();

                await _choiceResponseChannel!.Writer.WriteAsync(new ChoiceResponse
                {
                    SideId = e.SideId,
                    Choice = side.GetChoice(),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[Simulator.OnChoiceRequested] Error for {e.SideId}: {ex.Message}");

                // Auto-choose on error
                Side side = Battle!.Sides.First(s => s.Id == e.SideId);
                side.AutoChoose();

                await _choiceResponseChannel!.Writer.WriteAsync(new ChoiceResponse
                {
                    SideId = e.SideId,
                    Choice = side.GetChoice(),
                    Success = true
                });
            }
        });

        lock (_pendingChoiceTasks)
        {
            _pendingChoiceTasks[e.SideId] = choiceTask;
        }
    }

    /// <summary>
    /// Event handler for UI updates from the battle.
    /// Routes the update to the appropriate player.
    /// </summary>
    private void OnUpdateRequested(object? sender, BattleUpdateEventArgs e)
    {
        IPlayer player = GetPlayer(e.SideId);
        player.UpdateUi(e.Perspective);
        player.UpdateMessages(e.Messages);
    }

    /// <summary>
    /// Event handler for battle end notification.
    /// </summary>
    private void OnBattleEnded(object? sender, BattleEndedEventArgs e)
    {
        Console.WriteLine($"[Simulator.OnBattleEnded] Battle ended, winner: {e.Winner ?? "tie"}");

        // Complete the choice response channel to signal end of battle
        _choiceResponseChannel?.Writer.Complete();

        // Release the battle semaphore in case it's waiting
        try
        {
            _battleSemaphore.Release();
        }
        catch (SemaphoreFullException)
        {
            // Already released, that's fine
        }
    }

    /// <summary>
    /// Event handler for clear messages request.
    /// </summary>
    private void OnClearMessagesRequested(object? sender, EventArgs e)
    {
        ClearMessages();
    }

    /// <summary>
    /// Processes choice responses from players and submits them to the battle.
    /// Runs asynchronously until the battle ends.
    /// </summary>
    private async Task ProcessChoiceResponsesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (ChoiceResponse response in _choiceResponseChannel!.Reader.ReadAllAsync(
                               cancellationToken))
            {
                Console.WriteLine(
                    $"[Simulator.ProcessChoiceResponsesAsync] Processing choice for {response.SideId}");

                // Log the choice for replay purposes
                LogChoice(response.SideId, response.Choice);

                // Submit the choice to the battle
                // Note: This may trigger CommitChoices() if all choices are received
                if (!Battle!.Choose(response.SideId, response.Choice))
                {
                    Console.WriteLine(
                        $"[Simulator.ProcessChoiceResponsesAsync] Invalid choice for {response.SideId}");
                }

                // Clean up the completed task
                lock (_pendingChoiceTasks)
                {
                    _pendingChoiceTasks.Remove(response.SideId);
                }

                // Check if there are any pending choice tasks still running
                bool hasPendingTasks;
                lock (_pendingChoiceTasks)
                {
                    hasPendingTasks = _pendingChoiceTasks.Count > 0;
                }

                // Only check for new requests if:
                // 1. No pending choice tasks (all choices in current batch are processed)
                // 2. There's a new pending request
                // 3. Battle hasn't ended
                // This avoids calling RequestPlayerChoices() while still processing choices,
                // which would cause nested Battle.Choose() calls and stack overflow
                if (!hasPendingTasks && Battle.RequestState != RequestState.None && !Battle.Ended)
                {
                    Console.WriteLine(
                        $"[Simulator.ProcessChoiceResponsesAsync] All choices processed, new request pending: {Battle.RequestState}");

                    // Small delay to ensure Battle.Choose() call stack has fully unwound
                    await Task.Yield();

                    // Emit the new requests
                    Battle.RequestPlayerChoices();
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("[Simulator.ProcessChoiceResponsesAsync] Cancelled");
        }
    }

    /// <summary>
    /// Callback invoked by Battle when all choices are received.
    /// Commits the choices and releases the battle loop to continue.
    /// </summary>
    private void OnAllChoicesReceived()
    {
        Console.WriteLine(
            "[Simulator.OnAllChoicesReceived] All choices received, committing and releasing battle loop");

        // Commit the choices (this continues the battle turn)
        Battle!.CommitChoices();
    }

    private SimulatorResult DetermineWinner()
    {
        // Battle must have ended to determine a winner
        if (!Battle!.Ended)
        {
            if (PrintDebug)
            {
                Console.WriteLine("WARNING: DetermineWinner called but battle hasn't ended yet");
            }

            // Default to tie if battle somehow hasn't ended
            return SimulatorResult.Tie;
        }

        // Check if we have a winner (Winner will be empty string for ties)
        if (!string.IsNullOrEmpty(Battle.Winner))
        {
            // Winner is stored as side ID string ("p1" or "p2")
            bool isP1Winner = Battle.Winner.Equals("p1", StringComparison.OrdinalIgnoreCase) ||
                              Battle.Winner.Equals("Random 1", StringComparison.OrdinalIgnoreCase);

            if (PrintDebug)
            {
                string winnerName = isP1Winner ? "Player 1" : "Player 2";
                Console.WriteLine($"Winner: {winnerName}");
            }

            return isP1Winner ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
        }

        // Battle ended with no winner - it's a tie
        if (PrintDebug)
        {
            Console.WriteLine("Battle ended in a tie");
        }

        return SimulatorResult.Tie;
    }

    private SimulatorResult DetermineTiebreakWinner()
    {
        if (PrintDebug)
        {
            Console.WriteLine("Executing tiebreaker...");
        }

        // Use Battle's tiebreak logic which checks:
        // 1. Remaining Pokemon count
        // 2. HP percentage
        // 3. Total HP
        Battle!.Tiebreak();

        // After tiebreak, check the result
        // Tiebreak() calls Win() which sets Ended = true and Winner appropriately
        return DetermineWinner();
    }

    private IPlayer CreatePlayer(SideId sideId, PlayerOptions options)
    {
        return options.Type switch
        {
            Player.PlayerType.Random => new PlayerRandom(sideId, options, this),
            Player.PlayerType.Gui => CreateGuiPlayer(sideId, options),
            Player.PlayerType.Console => new PlayerConsole(sideId, options, this),
            Player.PlayerType.Mcts => throw new NotImplementedException(
                "MCTS player not implemented yet"),
            _ => throw new ArgumentOutOfRangeException($"Unknown player type: {options.Type}"),
        };
    }

    private PlayerGui CreateGuiPlayer(SideId sideId, PlayerOptions options)
    {
        // PlayerGui constructor now handles GuiWindow from options
        return new PlayerGui(sideId, options, this);
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
        InputLog.Add($">forcewin {sideId.ToString().ToLower()}");
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

        Battle.Tie();
        InputLog.Add(">forcetie");
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
        InputLog.Add($">forcelose {sideId.ToString().ToLower()}");
    }

    /// <summary>
    /// Exports the battle replay as a string.
    /// Includes battle initialization and all choices made during the battle.
    /// </summary>
    /// <returns>String representation of the battle replay</returns>
    public string ExportReplay()
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        var sb = new System.Text.StringBuilder();

        // Add start command with options (simplified)
        sb.AppendLine($">start {{\"formatid\":\"{Battle.Format.FormatId}\"}}");

        // Add PRNG seed
        sb.AppendLine($">reseed {Battle.PrngSeed}");

        // Add all logged choices and commands
        foreach (string log in InputLog)
        {
            sb.AppendLine(log);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Logs a choice made by a player for replay purposes.
    /// </summary>
    private void LogChoice(SideId sideId, Choice choice)
    {
        // Format choice as battle-stream protocol
        string choiceStr = FormatChoiceForLog(choice);
        InputLog.Add($">{sideId.ToString().ToLower()} {choiceStr}");
    }

    /// <summary>
    /// Formats a choice object as a string for logging.
    /// </summary>
    private string FormatChoiceForLog(Choice choice)
    {
        // Simple implementation - can be enhanced based on choice type
        if (choice.Actions.Count == 0)
        {
            return "pass";
        }

        var parts = new List<string>();
        foreach (ChosenAction action in choice.Actions)
        {
            // Format each action - this is a simplified version
            parts.Add(action.ToString() ?? "default");
        }

        return string.Join(",", parts);
    }
}

/// <summary>
/// Internal record for passing choice responses through the channel.
/// </summary>
internal record ChoiceResponse
{
    public required SideId SideId { get; init; }
    public required Choice Choice { get; init; }
    public required bool Success { get; init; }
}