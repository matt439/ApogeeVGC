using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    /// <summary>
    /// Request a choice from a player with timeout handling
    /// </summary>
    private async Task<BattleChoice> RequestChoiceFromPlayerAsync(PlayerId playerId, BattleChoice[] availableChoices, TimeSpan timeLimit, CancellationToken cancellationToken)
    {
        var player = GetPlayer(playerId);
        var playerTokenSource = GetPlayerCancellationTokenSource(playerId);

        if (PrintDebug)
            Console.WriteLine($"Requesting choice from {playerId} with {availableChoices.Length} options (timeout: {timeLimit.TotalSeconds}s)");

        // Create timeout cancellation token
        using var timeoutTokenSource = new CancellationTokenSource(timeLimit);
        
        // Combine timeout, player-specific, and global cancellation tokens
        using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, 
            timeoutTokenSource.Token, 
            playerTokenSource.Token);

        try
        {
            // Start timing for this player
            var actionStartTime = DateTime.UtcNow;

            // Fire choice requested event
            var eventArgs = new ChoiceRequestEventArgs
            {
                AvailableChoices = availableChoices,
                TimeLimit = timeLimit,
                RequestTime = actionStartTime
            };

            // Note: We cannot invoke events directly on interfaces, so we skip this
            // The player implementations should handle this internally if needed

            // Start timeout warning task (warn at 10 seconds remaining)
            var warningTask = StartTimeoutWarningTask(player, timeLimit, combinedTokenSource.Token);

            // Request choice asynchronously
            var choice = await player.GetNextChoiceAsync(availableChoices, combinedTokenSource.Token);

            // Validate the choice
            if (!IsValidChoice(playerId, choice, availableChoices))
            {
                if (PrintDebug)
                    Console.WriteLine($"Invalid choice from {playerId}, using default");
                choice = GetDefaultChoice(playerId, availableChoices);
            }

            // Update player time tracking
            var actionDuration = DateTime.UtcNow - actionStartTime;
            UpdatePlayerTime(playerId, actionDuration);

            if (PrintDebug)
                Console.WriteLine($"Choice received from {playerId}: {choice} (took {actionDuration.TotalSeconds:F1}s)");

            return choice;
        }
        catch (OperationCanceledException) when (timeoutTokenSource.Token.IsCancellationRequested)
        {
            // Action timeout occurred
            if (PrintDebug)
                Console.WriteLine($"Action timeout for {playerId}");

            await player.NotifyChoiceTimeoutAsync();
            throw new TimeoutException($"Player {playerId} action timed out");
        }
        catch (OperationCanceledException) when (playerTokenSource.Token.IsCancellationRequested)
        {
            // Player cancelled
            if (PrintDebug)
                Console.WriteLine($"Player {playerId} cancelled");
            throw;
        }
        catch (Exception ex)
        {
            if (PrintDebug)
                Console.WriteLine($"Error getting choice from {playerId}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Start a task to warn player about upcoming timeout
    /// </summary>
    private Task StartTimeoutWarningTask(IPlayerNew player, TimeSpan timeLimit, CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            try
            {
                var warningDelay = timeLimit.Subtract(TimeSpan.FromSeconds(10));
                if (warningDelay > TimeSpan.Zero)
                {
                    await Task.Delay(warningDelay, cancellationToken);
                    await player.NotifyTimeoutWarningAsync(TimeSpan.FromSeconds(10));
                }
            }
            catch (OperationCanceledException)
            {
                // Warning cancelled, which is fine
            }
            catch (Exception ex)
            {
                if (PrintDebug)
                    Console.WriteLine($"Warning task error: {ex.Message}");
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Generate available choices for a pending action
    /// </summary>
    private BattleChoice[] GetAvailableChoicesForAction(PendingAction action)
    {
        var side = GetSide(action.PlayerId);
        var choices = new List<BattleChoice>();

        if (PrintDebug)
            Console.WriteLine($"Generating choices for {action.PlayerId} action {action.ActionIndex}");

        // Get active Pokemon for this action
        var activePokemon = GetActivePokemonForAction(side, action.ActionIndex);
        
        if (activePokemon != null)
        {
            // Add move choices
            choices.AddRange(GenerateMoveChoices(activePokemon));
            
            // Add switch choices if Pokemon can switch
            if (CanPokemonSwitch(activePokemon))
            {
                choices.AddRange(GenerateSwitchChoices(side, activePokemon));
            }
        }

        if (choices.Count == 0)
        {
            // Fallback: create a basic struggle choice if no other options
            choices.Add(CreateStruggleChoice(action.PlayerId));
        }

        if (PrintDebug)
            Console.WriteLine($"Generated {choices.Count} choices for {action.PlayerId}");

        return choices.ToArray();
    }

    /// <summary>
    /// Get the active Pokemon for a specific action index
    /// </summary>
    private Pokemon? GetActivePokemonForAction(Side side, int actionIndex)
    {
        var activePokemon = side.AllSlots.Where(p => !p.IsFainted && IsActivePokemon(p, side)).ToList();
        
        if (actionIndex < activePokemon.Count)
            return activePokemon[actionIndex];
            
        return activePokemon.FirstOrDefault();
    }

    /// <summary>
    /// Check if a Pokemon is in an active slot
    /// </summary>
    private bool IsActivePokemon(Pokemon pokemon, Side side)
    {
        return side.BattleFormat switch
        {
            BattleFormat.Singles => pokemon.SlotId == SlotId.Slot1,
            BattleFormat.Doubles => pokemon.SlotId is SlotId.Slot1 or SlotId.Slot2,
            _ => false
        };
    }

    /// <summary>
    /// Generate move choices for a Pokemon
    /// </summary>
    private List<BattleChoice> GenerateMoveChoices(Pokemon pokemon)
    {
        var choices = new List<BattleChoice>();

        // Generate move choices for each available move
        foreach (var move in pokemon.Moves)
        {
            if (move.Pp > 0) // Only include moves with PP
            {
                try
                {
                    var moveChoice = SlotChoice.CreateMove(pokemon, move, false, MoveNormalTarget.None, []);
                    choices.Add(moveChoice);
                }
                catch (Exception ex)
                {
                    if (PrintDebug)
                        Console.WriteLine($"Could not create move choice for {move.Name}: {ex.Message}");
                }
            }
        }

        // If no moves available, add struggle
        if (choices.Count == 0)
        {
            // TODO: Create struggle move choice
            // For now, we'll skip this and let the calling method handle it
        }

        return choices;
    }

    /// <summary>
    /// Generate switch choices for a Pokemon
    /// </summary>
    private List<BattleChoice> GenerateSwitchChoices(Side side, Pokemon currentPokemon)
    {
        var choices = new List<BattleChoice>();

        foreach (var switchOption in side.SwitchOptionSlots)
        {
            if (!switchOption.IsFainted)
            {
                try
                {
                    var switchChoice = SlotChoice.CreateSwitch(currentPokemon, switchOption, side.BattleFormat);
                    choices.Add(switchChoice);
                }
                catch (Exception ex)
                {
                    if (PrintDebug)
                        Console.WriteLine($"Could not create switch choice to {switchOption.Specie.Name}: {ex.Message}");
                }
            }
        }

        return choices;
    }

    /// <summary>
    /// Check if a Pokemon can switch out
    /// </summary>
    private bool CanPokemonSwitch(Pokemon pokemon)
    {
        // TODO: Implement switch checking logic
        // Check for trapping moves, abilities, etc.
        return true; // Placeholder
    }

    /// <summary>
    /// Create a struggle choice as fallback
    /// </summary>
    private BattleChoice CreateStruggleChoice(PlayerId playerId)
    {
        // Get the first active Pokemon for this player
        var side = GetSide(playerId);
        var activePokemon = side.AllSlots.FirstOrDefault(p => !p.IsFainted && IsActivePokemon(p, side));
        
        if (activePokemon == null)
            throw new InvalidOperationException($"No active Pokemon found for {playerId}");

        // Find struggle move or use the first move as fallback
        var struggleMove = activePokemon.Moves.FirstOrDefault(m => m.Id == MoveId.Struggle) 
                          ?? activePokemon.Moves.First();
        
        return SlotChoice.CreateMove(activePokemon, struggleMove, false, MoveNormalTarget.None, []);
    }

    /// <summary>
    /// Get default choice for an action (index 0 behavior)
    /// </summary>
    private BattleChoice GetDefaultChoiceForAction(PendingAction action)
    {
        var availableChoices = GetAvailableChoicesForAction(action);
        return GetDefaultChoice(action.PlayerId, availableChoices);
    }

    /// <summary>
    /// Get default choice from available choices (first choice)
    /// </summary>
    private BattleChoice GetDefaultChoice(PlayerId playerId, BattleChoice[] availableChoices)
    {
        if (availableChoices.Length > 0)
        {
            if (PrintDebug)
                Console.WriteLine($"Using default choice for {playerId}: {availableChoices[0]}");
            return availableChoices[0];
        }

        throw new InvalidOperationException($"No available choices for {playerId}");
    }

    /// <summary>
    /// Validate that a choice is in the available choices
    /// </summary>
    private bool IsValidChoice(PlayerId playerId, BattleChoice choice, BattleChoice[] availableChoices)
    {
        // Check if the choice is in the available choices
        bool isValid = availableChoices.Contains(choice);
        
        if (!isValid && PrintDebug)
            Console.WriteLine($"Invalid choice from {playerId}: {choice} not in available choices");
            
        return isValid;
    }

    /// <summary>
    /// Event handler for when players submit choices (for external observers)
    /// </summary>
    private async void OnChoiceSubmitted(object? sender, BattleChoice choice)
    {
        if (sender is not IPlayerNew player) return;

        await _choiceSubmissionLock.WaitAsync();
        try
        {
            if (PrintDebug)
                Console.WriteLine($"Choice submitted by {player.PlayerId}: {choice}");

            // This event is mainly for logging/observers
            // The actual choice processing happens in RequestChoiceFromPlayerAsync
        }
        finally
        {
            _choiceSubmissionLock.Release();
        }
    }
}