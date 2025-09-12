using System.Net.Http.Headers;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Turns;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    public async Task ProcessGameplayTurnAsync(GameplayTurn turn, CancellationToken cancellationToken)
    {
        if (PrintDebug)
            Console.WriteLine($"Processing Gameplay turn {turn.TurnCounter}...");

        try
        {
            // Phase 1: Collect turn start choices
            var initialActions = await CollectTurnStartActionsAsync(cancellationToken);

            // Phase 2: Execute actions in priority order with dynamic interruptions
            await ExecuteActionsWithDynamicSwitchesAsync(initialActions, cancellationToken);

            // Phase 3: Apply end-of-turn effects
            await ApplyEndOfTurnEffectsAsync();

            // Phase 4: Handle end-of-turn forced switches (fainted Pokémon)
            await HandleEndOfTurnForcedSwitchesAsync(cancellationToken);

            // Complete the turn
            CompleteTurnWithEndStates();

            if (!CheckForGameEndConditions())
            {
                await CreateNextTurnAsync();
            }
        }
        catch (Exception ex)
        {
            if (PrintDebug)
                Console.WriteLine($"Gameplay turn error: {ex.Message}");
            throw;
        }
    }


    private async Task<List<ActionWithChoice>> CollectTurnStartActionsAsync(CancellationToken cancellationToken)
    {
        var actions = new List<ActionWithChoice>();

        // Each player submits one choice for each active Pokémon
        var player1Choices = await RequestTurnStartPlayerChoicesAsync(
            PlayerId.Player1, cancellationToken);

        var player2Choices = await RequestTurnStartPlayerChoicesAsync(
            PlayerId.Player2, cancellationToken);

        actions.AddRange(player1Choices);
        actions.AddRange(player2Choices);

        return actions;
    }


    private async Task<List<ActionWithChoice>> RequestTurnStartPlayerChoicesAsync(PlayerId playerId,
        CancellationToken cancellationToken)
    {
        Side side = GetCurrentSide(playerId);

        if (!side.IsTurnStartSideValid)
        {
            throw new InvalidOperationException($"Side {playerId} is not in a valid state to start the turn.");
        }

        var actions = new List<ActionWithChoice>();
        var activePokemon = side.AliveActivePokemon;

        foreach (Pokemon pokemon in activePokemon)
        {
            var availableChoices = GenerateTurnStartChoices(pokemon);
            BattleChoice choice = await RequestChoiceFromPlayerAsync(playerId, availableChoices,
                BattleRequestType.TurnStart, TimeSpan.FromSeconds(45), cancellationToken);
            var pendingAction = new PendingAction
            {
                PlayerId = playerId,
                ActionIndex = actions.Count, // Maintain order of submission
                RequestTime = DateTime.UtcNow,
            };
            actions.Add(CreateActionWithChoice(pendingAction, choice));
        }
        return actions;
    }

    private async Task ExecuteActionsWithDynamicSwitchesAsync(List<ActionWithChoice> actions,
        CancellationToken cancellationToken)
    {
        // Sort by priority/speed
        var sortedActions = SortActionsBySpeedOrder(actions);

        foreach (ActionWithChoice action in sortedActions)
        {
            if (PrintDebug)
                Console.WriteLine($"Executing: {action.PlayerId} - {action.Choice}");

            await ExecuteSingleActionAsync(action);

            // Check for immediate forced switches (Volt Switch, U-turn, etc.)
            if (RequiresImmediateForcedSwitch(action))
            {
                await HandleImmediateForcedSwitchAsync(action.Executor, action.PlayerId, cancellationToken);
            }

            // Check for game end after each action
            if (CheckForGameEndConditions())
                break;
        }
    }

    private bool RequiresImmediateForcedSwitch(ActionWithChoice action)
    {
        // Check if this action causes an immediate forced switch
        if (action.Choice is SlotChoice.MoveChoice moveChoice)
        {
            // Volt Switch, U-turn, Baton Pass, etc.
            return moveChoice.Move.SelfSwitch &&
                   !action.Executor.IsFainted &&
                   GetCurrentSide(action.PlayerId).SwitchOptionsCount > 0;
        }
        return false;
    }

    private async Task HandleImmediateForcedSwitchAsync(Pokemon pokemon, PlayerId playerId,
        CancellationToken cancellationToken)
    {
        var switchChoices = GenerateForcedSwitchChoices(pokemon);

        if (switchChoices.Length > 0)
        {
            BattleChoice choice = await RequestChoiceFromPlayerAsync(playerId, switchChoices,
                BattleRequestType.ForceSwitch, TimeSpan.FromSeconds(30), cancellationToken);

            await ExecuteSlotChoiceAsync(playerId, (SlotChoice)choice);
        }
    }

    private async Task HandleEndOfTurnForcedSwitchesAsync(CancellationToken cancellationToken)
    {
        // Handle fainted Pokémon switches in player order
        await HandleFaintedSwitchesForPlayer(PlayerId.Player1, cancellationToken);
        await HandleFaintedSwitchesForPlayer(PlayerId.Player2, cancellationToken);
    }

    private async Task HandleFaintedSwitchesForPlayer(PlayerId playerId, CancellationToken cancellationToken)
    {
        Side side = GetCurrentSide(playerId);
        var faintedActivePokemon = side.FaintedActivePokemon;

        foreach (Pokemon pokemon in faintedActivePokemon)
        {
            if (side.SwitchOptionsCount <= 0) continue;

            var switchChoices = GenerateFaintedSwitchChoices(pokemon);
            BattleChoice choice = await RequestChoiceFromPlayerAsync(playerId, switchChoices, BattleRequestType.FaintSwitch,
                TimeSpan.FromSeconds(30), cancellationToken);

            await ExecuteSlotChoiceAsync(playerId, (SlotChoice)choice);
        }
    }

    // Helper methods that need to be implemented

    ///// <summary>
    ///// Generate standard move/switch choices for a Pokemon
    ///// </summary>
    //private BattleChoice[] GenerateStandardChoices(Pokemon pokemon)
    //{
    //    var choices = new List<BattleChoice>();
        
    //    // Add move choices
    //    choices.AddRange(GetMoveChoices(pokemon.SideId));
        
    //    // Add switch choices if applicable
    //    Side side = GetCurrentSide(pokemon.SideId == SideId.Side1 ? PlayerId.Player1 : PlayerId.Player2);
    //    if (CanPokemonSwitch(pokemon))
    //    {
    //        choices.AddRange(GetSwitchChoices(side, Format));
    //    }
        
    //    return choices.ToArray();
    //}

    ///// <summary>
    ///// Generate forced switch choices for immediate switches (Volt Switch, etc.)
    ///// </summary>
    //private BattleChoice[] GenerateForcedSwitchChoices(Side side)
    //{
    //    var choices = new List<BattleChoice>();
        
    //    // Get the first active Pokemon (the one that used the forcing move)
    //    var activePokemon = GetAliveActivePokemon(side).FirstOrDefault();
    //    if (activePokemon != null)
    //    {
    //        choices.AddRange(GetSwitchChoices(side, Format));
    //    }
        
    //    return choices.ToArray();
    //}

    ///// <summary>
    ///// Generate switch choices for fainted Pokemon
    ///// </summary>
    //private BattleChoice[] GenerateFaintedSwitchChoices(Side side, SlotId faintedSlotId)
    //{
    //    var choices = new List<BattleChoice>();
        
    //    // Create a placeholder Pokemon for the fainted slot to generate switch choices
    //    Pokemon faintedPokemon = side.GetSlot(faintedSlotId);
    //    if (faintedPokemon.IsFainted)
    //    {
    //        choices.AddRange(GetSwitchChoices(side, Format));
    //    }
        
    //    return choices.ToArray();
    //}

    /// <summary>
    /// Create an ActionWithChoice from a pending action and chosen battle choice
    /// </summary>
    private ActionWithChoice CreateActionWithChoice(PendingAction pendingAction, BattleChoice choice)
    {
        Pokemon executor = GetExecutorPokemonForChoice(pendingAction.PlayerId, choice);
        int speedPriority = GetSpeedPriorityForChoice(choice);
        
        return new ActionWithChoice
        {
            PlayerId = pendingAction.PlayerId,
            Choice = choice,
            SpeedPriority = speedPriority,
            ActionOrder = pendingAction.ActionIndex,
            Executor = executor,
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
        Side side = GetCurrentSide(playerId);
        foreach (Pokemon pokemon in side.AllSlots)
        {
            if (!pokemon.IsFainted && side.IsActivePokemon(pokemon))
                return pokemon;
        }

        throw new InvalidOperationException($"No active Pokemon found for {playerId}");
    }

    /// <summary>
    /// Get speed/priority value for a choice (for action ordering)
    /// </summary>
    private int GetSpeedPriorityForChoice(BattleChoice choice)
    {
        return choice switch
        {
            SlotChoice.MoveChoice moveChoice => moveChoice.Move.Priority,
            SlotChoice.SwitchChoice => 6,
            _ => 0,
        };

        // Default priority for other actions
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
        if (PrintDebug)
            Console.WriteLine($"Executing slot choice for {playerId}: {slotChoice}");

        switch (slotChoice)
        {
            case SlotChoice.MoveChoice moveChoice:
                await ExecuteMoveChoiceAsync(playerId, moveChoice);
                break;
                
            case SlotChoice.SwitchChoice switchChoice:
                await ExecuteSwitchChoiceAsync(playerId, switchChoice);
                break;
                
            default:
                if (PrintDebug)
                    Console.WriteLine($"Unknown slot choice type: {slotChoice.GetType().Name}");
                break;
        }
    }

    /// <summary>
    /// Execute a move choice
    /// </summary>
    private async Task ExecuteMoveChoiceAsync(PlayerId playerId, SlotChoice.MoveChoice moveChoice)
    {
        if (PrintDebug)
            Console.WriteLine($"Executing move {moveChoice.Move.Name} by {moveChoice.Attacker.Specie.Name}");

        // TODO: Integrate with actual battle logic
        // This would involve:
        // - Move accuracy checks
        // - Damage calculation
        // - Effect application
        // - Target Pokemon updates
        // - Field condition changes

        await Task.CompletedTask;
    }

    /// <summary>
    /// Execute a switch choice
    /// </summary>
    private async Task ExecuteSwitchChoiceAsync(PlayerId playerId, SlotChoice.SwitchChoice switchChoice)
    {
        if (PrintDebug)
            Console.WriteLine($"Switching {switchChoice.SwitchOutPokemon.Specie.Name} for {switchChoice.SwitchInPokemon.Specie.Name}");

        // Perform the actual switch
        var side = GetCurrentSide(playerId);
        side.SwitchSlots(switchChoice.SwitchOutSlot, switchChoice.SwitchInSlot);

        // TODO: Integrate with actual battle logic
        // This would involve:
        // - Switch-in abilities triggering
        // - Field effect interactions
        // - Status condition transfers if applicable

        await Task.CompletedTask;
    }

    /// <summary>
    /// Apply end-of-turn effects (weather, status, etc.)
    /// </summary>
    private async Task ApplyEndOfTurnEffectsAsync()
    {
        if (PrintDebug)
            Console.WriteLine("Applying end-of-turn effects...");

        //// Apply weather effects
        //ApplyWeatherEffects();

        //// Apply status effects (poison, burn, etc.)
        //ApplyStatusEffectsToSide(Side1);
        //ApplyStatusEffectsToSide(Side2);

        //// Update field conditions
        //UpdateFieldConditions();

        // TODO: Add more end-of-turn effect processing

        await Task.CompletedTask;
    }

    ///// <summary>
    ///// Apply weather effects to both sides
    ///// </summary>
    //private void ApplyWeatherEffects()
    //{
    //    // TODO: Implement weather effect logic
    //    // This would check current weather and apply appropriate effects
    //}

    ///// <summary>
    ///// Apply status effects to a side
    ///// </summary>
    //private void ApplyStatusEffectsToSide(Side side)
    //{
    //    foreach (var pokemon in side.AllSlots)
    //    {
    //        if (!pokemon.IsFainted && IsActivePokemon(pokemon, side))
    //        {
    //            // TODO: Apply status effects (poison, burn, etc.)
    //            // pokemon.ApplyStatusEffects();
    //        }
    //    }
    //}

    ///// <summary>
    ///// Update field conditions
    ///// </summary>
    //private void UpdateFieldConditions()
    //{
    //    // TODO: Update field condition durations, apply field effects
    //    // Field.UpdateConditions();
    //}
}