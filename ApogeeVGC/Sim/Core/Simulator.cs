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
            _cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(30));

            // Start the battle loop task
            var battleLoopTask = Task.Run(() => RunBattleLoop(), _cancellationTokenSource.Token);

            // Start processing choice responses
            var choiceProcessingTask = ProcessChoiceResponsesAsync(_cancellationTokenSource.Token);

            // Wait for battle to complete
            await Task.WhenAny(battleLoopTask, choiceProcessingTask);

            // If battle loop completed, wait a bit for final choices to process
            if (battleLoopTask.IsCompleted)
            {
                await Task.Delay(100); // Give time for any final messages
            }

            if (PrintDebug)
            {
                Console.WriteLine("Battle has ended.");
            }

            return DetermineWinner();
        }
        catch (OperationCanceledException)
        {
            if (PrintDebug)
            {
                Console.WriteLine("Battle was cancelled due to timeout");
            }

            return DetermineTiebreakWinner();
        }
        catch (Exception ex)
        {
            if (PrintDebug)
            {
                Console.WriteLine($"Battle error: {ex.Message}");
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

            _choiceResponseChannel?.Writer.Complete();
            _cancellationTokenSource?.Dispose();
        }
    }

    /// <summary>
    /// Runs the synchronous battle loop, waiting for async choices when needed.
    /// </summary>
    private void RunBattleLoop()
    {
        try
        {
            Console.WriteLine("[Simulator.RunBattleLoop] Starting battle");

            // Start the battle - this is synchronous and will call RequestPlayerChoices when needed
            Battle!.Start();

            Console.WriteLine(
                "[Simulator.RunBattleLoop] Battle.Start() returned - battle should be complete");
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
        var choiceTask = Task.Run(async () =>
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
                // AutoChoose modifies Side state only, which is safe from the async task
                if (choice.Actions.Count == 0)
                {
                    Console.WriteLine($"[Simulator.OnChoiceRequested] Empty choice received for {e.SideId}, using AutoChoose");
                    Side side = Battle!.Sides.First(s => s.Id == e.SideId);
                    Console.WriteLine($"[Simulator.OnChoiceRequested] Before AutoChoose: Choice.Actions.Count = {side.GetChoice().Actions.Count}");
                    side.AutoChoose();
                    Console.WriteLine($"[Simulator.OnChoiceRequested] After AutoChoose: Choice.Actions.Count = {side.GetChoice().Actions.Count}");
                    choice = side.GetChoice();
                    Console.WriteLine($"[Simulator.OnChoiceRequested] Final choice.Actions.Count = {choice.Actions.Count}");
                }

                // Send the choice response directly - let Battle.Choose() handle empty choices
                // Do NOT call side.AutoChoose() here as it accesses battle state from async task
                await _choiceResponseChannel!.Writer.WriteAsync(new ChoiceResponse
                {
                    SideId = e.SideId,
                    Choice = choice,
                    Success = true
                });
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

                // Submit the choice to the battle (this will trigger the callback when all choices are done)
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
        // Check if we have a winner
        if (!string.IsNullOrEmpty(Battle!.Winner))
        {
            // Winner is stored as side ID string ("p1" or "p2")
            bool isP1Winner = Battle.Winner.Equals("p1", StringComparison.OrdinalIgnoreCase);

            if (PrintDebug)
            {
                string winnerName = isP1Winner ? "Player 1" : "Player 2";
                Console.WriteLine($"Winner: {winnerName}");
            }

            return isP1Winner ? SimulatorResult.Player1Win : SimulatorResult.Player2Win;
        }

        // No clear winner - use tiebreak
        if (PrintDebug)
        {
            Console.WriteLine("Battle ended without a clear winner");
        }

        return DetermineTiebreakWinner();
    }

    private SimulatorResult DetermineTiebreakWinner()
    {
        // For now, always return Player 1 as winner in tiebreak
        Console.WriteLine("Tiebreaker applied: Player 1 declared winner by default.");
        return SimulatorResult.Player1Win;
    }

    private IPlayer CreatePlayer(SideId sideId, PlayerOptions options)
    {
        return options.Type switch
        {
            Player.PlayerType.Random => new PlayerRandom(sideId, options, this),
            Player.PlayerType.Gui => CreateGuiPlayer(sideId, options),
            Player.PlayerType.Console => new PlayerConsole(sideId, options, this),
            Player.PlayerType.Mcts => throw new NotImplementedException("MCTS player not implemented yet"),
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