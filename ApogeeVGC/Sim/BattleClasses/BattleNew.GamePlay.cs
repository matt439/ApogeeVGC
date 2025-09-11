using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Stats;
using ApogeeVGC.Sim.Turns;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    /// <summary>
    /// Process gameplay turn - handle multiple actions per player
    /// </summary>
    public async Task ProcessGameplayTurnAsync(GameplayTurn turn, CancellationToken cancellationToken)
    {
        if (PrintDebug)
            Console.WriteLine($"Processing Gameplay turn {turn.TurnCounter}...");

        try
        {
            // Determine required actions for each player this turn
            var remainingActions = GetRemainingActionsForTurn(turn);

            if (PrintDebug)
                Console.WriteLine($"Total actions required this turn: {remainingActions.Count}");

            // Collect all choices for this turn
            var collectedActions = new List<ActionWithChoice>();

            // Process each required action
            while (remainingActions.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                var nextAction = remainingActions.First();
                
                if (PrintDebug)
                    Console.WriteLine($"Requesting action {nextAction.ActionIndex} from {nextAction.PlayerId}");

                try
                {
                    // Check if player has exceeded their total time limit
                    if (HasPlayerTimedOut(nextAction.PlayerId))
                    {
                        await HandlePlayerTimeoutAsync(nextAction.PlayerId);
                        return;
                    }

                    // Generate available choices for this action
                    var availableChoices = GetAvailableChoicesForAction(nextAction);
                    
                    // Request choice with 45-second action timeout
                    var actionTimeLimit = TimeSpan.FromSeconds(45);
                    var choice = await RequestChoiceFromPlayerAsync(nextAction.PlayerId, availableChoices, actionTimeLimit, cancellationToken);

                    // Create action with choice for execution
                    var actionWithChoice = CreateActionWithChoice(nextAction, choice);
                    collectedActions.Add(actionWithChoice);

                    if (PrintDebug)
                        Console.WriteLine($"Action collected from {nextAction.PlayerId}: {choice}");
                }
                catch (TimeoutException)
                {
                    if (PrintDebug)
                        Console.WriteLine($"Action timeout for {nextAction.PlayerId}, using default choice");

                    // Use default choice (index 0 behavior)
                    var defaultChoice = GetDefaultChoiceForAction(nextAction);
                    var actionWithChoice = CreateActionWithChoice(nextAction, defaultChoice);
                    collectedActions.Add(actionWithChoice);
                }

                remainingActions.Remove(nextAction);
            }

            // All choices collected, execute the turn
            await ExecuteTurnActionsAsync(turn, collectedActions);
        }
        catch (Exception ex)
        {
            if (PrintDebug)
                Console.WriteLine($"Gameplay turn error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Determine what actions are still needed for this turn
    /// </summary>
    private List<PendingAction> GetRemainingActionsForTurn(GameplayTurn turn)
    {
        var actions = new List<PendingAction>();

        // Determine how many actions each player needs based on active Pokemon
        var player1ActionCount = GetRequiredActionCount(PlayerId.Player1);
        var player2ActionCount = GetRequiredActionCount(PlayerId.Player2);

        if (PrintDebug)
        {
            Console.WriteLine($"Player 1 needs {player1ActionCount} actions");
            Console.WriteLine($"Player 2 needs {player2ActionCount} actions");
        }

        // Create pending actions for each player
        for (int i = 0; i < player1ActionCount; i++)
        {
            actions.Add(new PendingAction
            {
                PlayerId = PlayerId.Player1,
                ActionIndex = i,
                RequestTime = DateTime.UtcNow
            });
        }

        for (int i = 0; i < player2ActionCount; i++)
        {
            actions.Add(new PendingAction
            {
                PlayerId = PlayerId.Player2,
                ActionIndex = i,
                RequestTime = DateTime.UtcNow
            });
        }

        return actions;
    }

    /// <summary>
    /// Get the number of actions required for a player this turn
    /// </summary>
    private int GetRequiredActionCount(PlayerId playerId)
    {
        var side = GetSide(playerId);
        
        // Count active, non-fainted Pokemon that need to submit actions
        var activeCount = 0;
        foreach (var pokemon in side.AllSlots)
        {
            if (!pokemon.IsFainted && IsActivePokemon(pokemon, side))
                activeCount++;
        }

        // Ensure at least 1 action is required
        return Math.Max(1, activeCount);
    }

    /// <summary>
    /// Create an ActionWithChoice from a pending action and chosen battle choice
    /// </summary>
    private ActionWithChoice CreateActionWithChoice(PendingAction pendingAction, BattleChoice choice)
    {
        var executor = GetExecutorPokemonForChoice(pendingAction.PlayerId, choice);
        var speedPriority = GetSpeedPriorityForChoice(choice);
        
        return new ActionWithChoice
        {
            PlayerId = pendingAction.PlayerId,
            Choice = choice,
            SpeedPriority = speedPriority,
            ActionOrder = pendingAction.ActionIndex,
            Executor = executor
        };
    }

    /// <summary>
    /// Get the Pokemon that will execute a given choice
    /// </summary>
    private Pokemon GetExecutorPokemonForChoice(PlayerId playerId, BattleChoice choice)
    {
        // Extract executor from the choice itself
        if (choice is SlotChoice slotChoice)
        {
            return slotChoice switch
            {
                SlotChoice.MoveChoice moveChoice => moveChoice.Attacker,
                SlotChoice.SwitchChoice switchChoice => switchChoice.SwitchOutPokemon,
                _ => throw new InvalidOperationException($"Unknown slot choice type: {slotChoice.GetType().Name}")
            };
        }

        // Fallback: get first active Pokemon
        var side = GetSide(playerId);
        foreach (var pokemon in side.AllSlots)
        {
            if (!pokemon.IsFainted && IsActivePokemon(pokemon, side))
                return pokemon;
        }

        throw new InvalidOperationException($"No active Pokemon found for {playerId}");
    }

    /// <summary>
    /// Get speed/priority value for a choice (for action ordering)
    /// </summary>
    private int GetSpeedPriorityForChoice(BattleChoice choice)
    {
        // This would depend on your choice system
        // Move choices would have move priority, switches typically have higher priority, etc.
        
        if (choice is SlotChoice.MoveChoice moveChoice)
        {
            return moveChoice.Move.Priority;
        }
        
        if (choice is SlotChoice.SwitchChoice)
        {
            return 6; // Switches have high priority
        }

        // Default priority for other actions
        return 0;
    }

    /// <summary>
    /// Execute all collected actions for the turn
    /// </summary>
    private async Task ExecuteTurnActionsAsync(GameplayTurn turn, List<ActionWithChoice> collectedActions)
    {
        if (PrintDebug)
            Console.WriteLine($"Executing {collectedActions.Count} actions for turn {turn.TurnCounter}");

        // Sort actions by speed/priority order
        var sortedActions = SortActionsBySpeedOrder(collectedActions);

        // Execute actions in order
        foreach (var action in sortedActions)
        {
            if (PrintDebug)
                Console.WriteLine($"Executing action: {action.PlayerId} - {action.Choice}");

            await ExecuteSingleActionAsync(action);

            // Check for game end conditions after each action
            if (CheckForGameEndConditions())
            {
                if (PrintDebug)
                    Console.WriteLine("Game end condition detected during action execution");
                break;
            }
        }

        // Apply end-of-turn effects
        await ApplyEndOfTurnEffectsAsync();

        // Complete the turn
        CompleteTurnWithEndStates();

        // Check if we need to create another turn or end the game
        if (!CheckForGameEndConditions())
        {
            await CreateNextTurnAsync();
        }

        if (PrintDebug)
            Console.WriteLine($"Turn {turn.TurnCounter} execution completed");
    }

    /// <summary>
    /// Sort actions by speed/priority order
    /// </summary>
    private List<ActionWithChoice> SortActionsBySpeedOrder(List<ActionWithChoice> actions)
    {
        // Sort by priority (higher first), then by speed (higher first), then by original order
        return actions
            .OrderByDescending(a => a.SpeedPriority)
            .ThenByDescending(a => a.Executor.CurrentSpe) // Use Current Speed stat
            .ThenBy(a => a.ActionOrder)
            .ToList();
    }

    /// <summary>
    /// Execute a single action against the battle state
    /// </summary>
    private async Task ExecuteSingleActionAsync(ActionWithChoice action)
    {
        switch (action.Choice)
        {
            case SlotChoice slotChoice:
                await ExecuteSlotChoiceAsync(action.PlayerId, slotChoice);
                break;
            
            // Add other choice types as needed
            default:
                if (PrintDebug)
                    Console.WriteLine($"Unknown choice type: {action.Choice.GetType().Name}");
                break;
        }
    }

    /// <summary>
    /// Execute a slot choice (move, switch, etc.)
    /// </summary>
    private async Task ExecuteSlotChoiceAsync(PlayerId playerId, SlotChoice slotChoice)
    {
        // This is where you'd integrate with your existing Battle.cs logic
        // For now, just log the action
        
        if (PrintDebug)
            Console.WriteLine($"Executing slot choice for {playerId}: {slotChoice}");

        // TODO: Integrate with actual battle logic
        // Examples:
        // - If it's a move choice, apply move effects
        // - If it's a switch choice, perform the switch
        // - Update Pokemon HP, status, field conditions, etc.

        await Task.CompletedTask;
    }

    /// <summary>
    /// Apply end-of-turn effects (weather, status, etc.)
    /// </summary>
    private async Task ApplyEndOfTurnEffectsAsync()
    {
        if (PrintDebug)
            Console.WriteLine("Applying end-of-turn effects...");

        // Apply weather effects
        ApplyWeatherEffects();

        // Apply status effects (poison, burn, etc.)
        ApplyStatusEffectsToSide(Side1);
        ApplyStatusEffectsToSide(Side2);

        // Update field conditions
        UpdateFieldConditions();

        // TODO: Add more end-of-turn effect processing

        await Task.CompletedTask;
    }

    /// <summary>
    /// Apply weather effects to both sides
    /// </summary>
    private void ApplyWeatherEffects()
    {
        // TODO: Implement weather effect logic
        // This would check current weather and apply appropriate effects
    }

    /// <summary>
    /// Apply status effects to a side
    /// </summary>
    private void ApplyStatusEffectsToSide(Side side)
    {
        foreach (var pokemon in side.AllSlots)
        {
            if (!pokemon.IsFainted && IsActivePokemon(pokemon, side))
            {
                // TODO: Apply status effects (poison, burn, etc.)
                // pokemon.ApplyStatusEffects();
            }
        }
    }

    /// <summary>
    /// Update field conditions
    /// </summary>
    private void UpdateFieldConditions()
    {
        // TODO: Update field condition durations, apply field effects
        // Field.UpdateConditions();
    }
}