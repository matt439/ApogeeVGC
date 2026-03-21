using System.Text;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.FormatClasses;
using ApogeeVGC.Sim.Player;
using ApogeeVGC.Sim.SideClasses;
using System.Threading.Channels;

namespace ApogeeVGC.Sim.Core;

public class SimulatorAsync : SimulatorBase
{
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

    public override void ClearMessages()
    {
        if (PrintDebug)
        {
            Console.WriteLine("[SimulatorAsync.ClearMessages] Called");
        }

#if !HEADLESS
        // Directly call ClearMessages on GUI players' BattleGame windows
        if (Player1 is PlayerGui gui1)
        {
            if (PrintDebug)
            {
                Console.WriteLine(
                    $"[SimulatorAsync.ClearMessages] Calling ClearMessages for Player1 (GuiWindow:" +
                    $"{gui1.GuiWindow.GetHashCode()})");
            }

            gui1.GuiWindow.ClearMessages();
        }

        if (Player2 is PlayerGui gui2)
        {
            if (PrintDebug)
            {
                Console.WriteLine(
                    $"[SimulatorAsync.ClearMessages] Calling ClearMessages for Player2 (GuiWindow:" +
                    $"{gui2.GuiWindow.GetHashCode()})");
            }

            gui2.GuiWindow.ClearMessages();
        }
#endif

        if (PrintDebug)
        {
            Console.WriteLine("[SimulatorAsync.ClearMessages] Completed");
        }
    }

    public async Task<SimulatorResult> RunAsync(Library library, BattleOptions battleOptions,
        bool printDebug = true)
    {
        Battle = new Battle(battleOptions, library);
        Player1 = CreatePlayer(SideId.P1, battleOptions.Player1Options);
        Player2 = CreatePlayer(SideId.P2, battleOptions.Player2Options);
        PrintDebug = printDebug;

        // Validate teams against format rules
        TeamValidator validator = new(library, Battle.Format);
        ValidateTeams(validator, battleOptions, printDebug);

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
            Task battleLoopTask = Task.Run(RunBattleLoop, _cancellationTokenSource.Token);

            // Start processing choice responses
            Task choiceProcessingTask = ProcessChoiceResponsesAsync(_cancellationTokenSource.Token);

            // Wait for either the battle to end or a timeout
            Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(300), _cancellationTokenSource.Token);
            Task completedTask = await Task.WhenAny(
                battleLoopTask,
                choiceProcessingTask,
                timeoutTask
            );

            string taskName = completedTask == battleLoopTask ? "battleLoop" :
                completedTask == choiceProcessingTask ? "choiceProcessing" : "timeout";
            if (PrintDebug)
            {
                Console.WriteLine($"[SimulatorAsync.RunAsync] Completed task: {taskName}");
                Console.WriteLine($"[SimulatorAsync.RunAsync] Battle.Ended = {Battle.Ended}");
            }

            // Check if we timed out
            if (completedTask == timeoutTask)
            {
                if (PrintDebug)
                {
                    Console.WriteLine("[SimulatorAsync.RunAsync] Battle timed out - forcing end");
                }

                // Force battle to end if it hasn't already
                if (!Battle.Ended)
                {
                    if (PrintDebug)
                    {
                        Console.WriteLine("[SimulatorAsync.RunAsync] Calling Tiebreak to end battle");
                    }

                    Battle.Tiebreak();
                    if (PrintDebug)
                    {
                        Console.WriteLine(
                            $"[SimulatorAsync.RunAsync] After Tiebreak: Battle.Ended = {Battle.Ended}");
                    }
                }
            }
            else if (completedTask == battleLoopTask)
            {
                if (PrintDebug)
                {
                    Console.WriteLine("[SimulatorAsync.RunAsync] Battle loop completed");
                }
            }
            else if (completedTask == choiceProcessingTask)
            {
                if (PrintDebug)
                {
                    Console.WriteLine("[SimulatorAsync.RunAsync] Choice processing completed");
                }
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

                Console.WriteLine("Stack trace (last 10 frames):");
                IEnumerable<string>? frames = ex.StackTrace?.Split('\n').Take(10);
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
            if (PrintDebug)
            {
                Console.WriteLine("[SimulatorAsync.RunBattleLoop] Starting battle");
            }

            // Start the battle - this will set up team preview or initial turn
            Battle!.Start();

            if (PrintDebug)
            {
                Console.WriteLine(
                    $"[SimulatorAsync.RunBattleLoop] Battle.Start() returned, RequestState: {Battle.RequestState}");
            }

            // After Start() returns, check if there's a pending request and emit it
            // This handles team preview or initial switch-in requests
            if (Battle.RequestState != RequestState.None && !Battle.Ended)
            {
                if (PrintDebug)
                {
                    Console.WriteLine(
                        $"[SimulatorAsync.RunBattleLoop] Emitting initial request: {Battle.RequestState}");
                }

                Battle.RequestPlayerChoices();
            }

            // Wait for the battle to actually end
            if (PrintDebug)
            {
                Console.WriteLine("[SimulatorAsync.RunBattleLoop] Waiting for battle to end...");
            }

            while (!Battle.Ended)
            {
                Thread.Sleep(100); // Check every 100ms
            }

            if (PrintDebug)
            {
                Console.WriteLine("[SimulatorAsync.RunBattleLoop] Battle has ended");
            }
        }
        catch (Exception ex)
        {
            if (PrintDebug)
            {
                Console.WriteLine($"[SimulatorAsync.RunBattleLoop] Error: {ex.Message}");
            }

            throw;
        }
    }

    /// <summary>
    /// Event handler for when Battle requests a choice from a player.
    /// Starts an async task to get the choice from the player.
    /// </summary>
    private void OnChoiceRequested(object? sender, BattleChoiceRequestEventArgs e)
    {
        if (PrintDebug)
        {
            Console.WriteLine($"[SimulatorAsync.OnChoiceRequested] Choice requested for {e.SideId}");
        }

        // Start an async task to get the choice
        Task choiceTask = Task.Run(async () =>
        {
            try
            {
                IPlayer player = GetPlayer(e.SideId);

                // Request choice from the player
                using CancellationTokenSource cts = new(TimeSpan.FromMinutes(5));
                Choice choice = await player.GetNextChoiceAsync(
                    e.Request,
                    e.RequestType,
                    e.Perspective,
                    cts.Token
                );

                if (PrintDebug)
                {
                    Console.WriteLine(
                        $"[SimulatorAsync.OnChoiceRequested] Received choice for {e.SideId}");
                }

                // If the choice is empty (no actions), use AutoChoose to fill it
                if (choice.Actions.Count == 0)
                {
                    if (PrintDebug)
                    {
                        Console.WriteLine(
                            $"[SimulatorAsync.OnChoiceRequested] Empty choice received for {e.SideId}, using AutoChoose");
                    }

                    Side side = Battle!.Sides.First(s => s.Id == e.SideId);
                    if (PrintDebug)
                    {
                        Console.WriteLine(
                            $"[SimulatorAsync.OnChoiceRequested] Before AutoChoose: Choice.Actions.Count = {side.GetChoice().Actions.Count}");
                    }

                    side.AutoChoose();
                    if (PrintDebug)
                    {
                        Console.WriteLine(
                            $"[SimulatorAsync.OnChoiceRequested] After AutoChoose: Choice.Actions.Count = {side.GetChoice().Actions.Count}");
                    }

                    choice = side.GetChoice();
                    if (PrintDebug)
                    {
                        Console.WriteLine(
                            $"[SimulatorAsync.OnChoiceRequested] Final choice.Actions.Count = {choice.Actions.Count}");
                    }
                }

                // Send the choice response directly
                await _choiceResponseChannel!.Writer.WriteAsync(new ChoiceResponse
                {
                    SideId = e.SideId,
                    Choice = choice,
                    Success = true
                }, cts.Token);

                // Log the choice for replay
                LogChoice(e.SideId, choice);
            }
            catch (OperationCanceledException)
            {
                if (PrintDebug)
                {
                    Console.WriteLine(
                        $"[SimulatorAsync.OnChoiceRequested] Timeout for {e.SideId}, auto-choosing");
                }

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
#if DEBUG
                // In DEBUG mode, rethrow immediately to expose errors during debugging
                if (PrintDebug)
                {
                    Console.WriteLine(
                        $"[SimulatorAsync.OnChoiceRequested] ERROR for {e.SideId}: {ex.GetType().Name}: {ex.Message}");
                }

                throw;
#else
                // In RELEASE mode, auto-choose on error for graceful degradation
                if (PrintDebug)
                {
                    Console.WriteLine(
                        $"[SimulatorAsync.OnChoiceRequested] Error for {e.SideId}: {ex.Message}");
                }

                // Auto-choose on error
                Side side = Battle!.Sides.First(s => s.Id == e.SideId);
                side.AutoChoose();

                await _choiceResponseChannel!.Writer.WriteAsync(new ChoiceResponse
                {
                    SideId = e.SideId,
                    Choice = side.GetChoice(),
                    Success = true
                });
#endif
            }
        });

        lock (_pendingChoiceTasks)
        {
            _pendingChoiceTasks[e.SideId] = choiceTask;
        }
    }

    /// <summary>
    /// Event handler for battle end notification.
    /// </summary>
    private void OnBattleEnded(object? sender, BattleEndedEventArgs e)
    {
        if (PrintDebug)
        {
            Console.WriteLine(
                $"[SimulatorAsync.OnBattleEnded] Battle ended, winner: {e.Winner ?? "tie"}");
        }

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
            if (PrintDebug)
            {
                Console.WriteLine("[SimulatorAsync.ProcessChoiceResponsesAsync] Starting");
            }

            await foreach (ChoiceResponse response in _choiceResponseChannel!.Reader.ReadAllAsync(
                               cancellationToken))
            {
                if (PrintDebug)
                {
                    Console.WriteLine(
                        $"[SimulatorAsync.ProcessChoiceResponsesAsync] Processing choice for {response.SideId}");
                }

                // Log the choice for replay purposes
                LogChoice(response.SideId, response.Choice);

                // Submit the choice to the battle
                if (!Battle!.Choose(response.SideId, response.Choice))
                {
                    if (PrintDebug)
                    {
                        Console.WriteLine(
                            $"[SimulatorAsync.ProcessChoiceResponsesAsync] Invalid choice for {response.SideId}");
                    }
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

                if (PrintDebug)
                {
                    Console.WriteLine(
                        $"[SimulatorAsync.ProcessChoiceResponsesAsync] Finished processing choice #{response.SideId}, waiting for next...");
                    Console.WriteLine(
                        $"[SimulatorAsync.ProcessChoiceResponsesAsync] Battle.Ended={Battle.Ended}, Channel completion={_choiceResponseChannel.Reader.Completion.IsCompleted}");
                }

                if (!hasPendingTasks && Battle.RequestState != RequestState.None && !Battle.Ended)
                {
                    if (PrintDebug)
                    {
                        Console.WriteLine(
                            $"[SimulatorAsync.ProcessChoiceResponsesAsync] All choices processed, new request pending: {Battle.RequestState}");
                    }

                    // Small delay to ensure Battle.Choose() call stack has fully unwound
                    await Task.Yield();

                    // Emit the new requests
                    Battle.RequestPlayerChoices();
                }
            }

            if (PrintDebug)
            {
                Console.WriteLine(
                    $"[SimulatorAsync.ProcessChoiceResponsesAsync] ReadAllAsync completed, Battle.Ended={Battle?.Ended ??
                        throw new InvalidOperationException(nameof(Battle.Ended))}");
            }
        }
        catch (OperationCanceledException)
        {
            if (PrintDebug)
            {
                Console.WriteLine("[SimulatorAsync.ProcessChoiceResponsesAsync] Cancelled");
            }
        }
        catch (Exception ex)
        {
#if DEBUG
            // In DEBUG mode, rethrow immediately to expose errors during debugging
            if (PrintDebug)
            {
                Console.WriteLine(
                    $"[SimulatorAsync.ProcessChoiceResponsesAsync] ERROR: {ex.GetType().Name}: {ex.Message}");

                // Print inner exception details if present
                if (ex.InnerException != null)
                {
                    Console.WriteLine(
                        $"  Inner Exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");

                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine(
                            $"    Inner Inner Exception: {ex.InnerException.InnerException.GetType().Name}: {ex.InnerException.InnerException.Message}");
                    }
                }

                // Print first 15 lines of stack trace
                if (ex.StackTrace != null)
                {
                    var stackLines = ex.StackTrace.Split('\n').Take(15);
                    Console.WriteLine("  Stack Trace:");
                    foreach (var line in stackLines)
                    {
                        Console.WriteLine($"    {line.Trim()}");
                    }
                }
            }

            throw;
#else
            // In RELEASE mode, log and continue for graceful degradation
            if (PrintDebug)
            {
                Console.WriteLine(
                    $"[SimulatorAsync.ProcessChoiceResponsesAsync] Exception: {ex.GetType().Name}: {ex.Message}");
            }
            throw;
#endif
        }
        finally
        {
            if (PrintDebug)
            {
                Console.WriteLine(
                    $"[SimulatorAsync.ProcessChoiceResponsesAsync] Exiting, Battle.Ended={Battle?.Ended ?? throw new InvalidOperationException(nameof(Battle.Ended))}");
            }
        }
    }

    /// <summary>
    /// Callback invoked by Battle when all choices are received.
    /// Commits the choices and releases the battle loop to continue.
    /// </summary>
    private void OnAllChoicesReceived()
    {
        if (PrintDebug)
        {
            Console.WriteLine(
                "[SimulatorAsync.OnAllChoicesReceived] All choices received, committing and releasing battle loop");
        }

        // Commit the choices (this continues the battle turn)
        Battle!.CommitChoices();
    }

    protected override IPlayer CreatePlayer(SideId sideId, PlayerOptions options)
    {
        return options.Type switch
        {
            Player.PlayerType.Random => new PlayerRandom(sideId, options, this),
#if !HEADLESS
            Player.PlayerType.Gui => new PlayerGui(sideId, options, this),
#endif
            Player.PlayerType.Console => new PlayerConsole(sideId, options, this),
            Player.PlayerType.Mcts => throw new NotImplementedException(
                "MCTS player not implemented yet"),
            Player.PlayerType.Greedy => new PlayerGreedy(sideId, options, this),
            _ => throw new ArgumentOutOfRangeException($"Unknown player type: {options.Type}"),
        };
    }

    public override void ForceWin(SideId sideId)
    {
        base.ForceWin(sideId);
        InputLog.Add($">forcewin {sideId.ToString().ToLower()}");
    }

    public override void ForceTie()
    {
        base.ForceTie();
        InputLog.Add(">forcetie");
    }

    public override void ForceLose(SideId sideId)
    {
        base.ForceLose(sideId);
        InputLog.Add($">forcelose {sideId.ToString().ToLower()}");
    }

    /// <summary>
    /// Exports the battle replay as a string.
    /// Includes battle initialization and all choices made during the battle.
    /// </summary>
    public string ExportReplay()
    {
        if (Battle == null)
        {
            throw new InvalidOperationException("Battle is not initialized");
        }

        StringBuilder sb = new();

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

        List<string> parts = [];
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
