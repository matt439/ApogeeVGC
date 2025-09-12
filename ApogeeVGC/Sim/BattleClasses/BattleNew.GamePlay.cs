using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Turns;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    public async Task ProcessGameplayTurnAsync(GameplayTurn turn, CancellationToken cancellationToken)
    {
        if (PrintDebug)
            Console.WriteLine($"Processing Gameplay turn {turn.TurnCounter}...");

        try
        {
            if (TurnCounter == 1)
            {
                // Handle switch-in effects at the start of the first turn
                HandleEndOfTeamPreviewTurn();
            }

            // Phase 1: Start-of-turn effects
            await HandleStartOfTurn();

            // Phase 2: Collect turn start choices
            var initialActions = await CollectTurnStartActionsAsync(cancellationToken);

            // Phase 3: Execute actions in priority order with dynamic interruptions
            await ExecuteActionsWithDynamicSwitchesAsync(initialActions, cancellationToken);

            // Phase 4: Apply end-of-turn effects
            await ApplyEndOfTurnEffectsAsync();

            // Phase 5: Handle end-of-turn forced switches (fainted Pokémon)
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
                BattleRequestType.TurnStart, TimeSpan.FromSeconds(StandardTurnLimitSeconds),
                cancellationToken);

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
        // TODO: Have teras activate after switches but before moves
        HandleTerastalization(actions);
        
        // Sort by priority/speed
        var sortedActions = SortActionsBySpeedOrder(actions);

        foreach (ActionWithChoice action in sortedActions)
        {
            // Check if executor is still able to act
            if (action.Executor.IsFainted || !GetCurrentSide(action.PlayerId).IsActivePokemon(action.Executor))
            {
                if (PrintDebug)
                    Console.WriteLine($"Skipping action for {action.Executor.Name} as it can no longer act.");
                continue;
            }

            await ExecuteSingleActionAsync(action);

            // Check for immediate forced switches (Volt Switch, U-turn, etc.)
            if (RequiresImmediateForcedSwitch(action))
            {
                await HandleImmediateForcedSwitchAsync(action.Executor, action.PlayerId, cancellationToken);
            }

            // Check for game end after each action
            if (CheckForGameEndConditions())
            {
                break;
            }

            // Check for fainted Pokémon and notify via UI (switches handled at end of turn)
            var faintedPokemon = GetAllFaintedActivePokemon();
            foreach (Pokemon fainted in faintedPokemon)
            {
                if (PrintDebug)
                {
                    UiGenerator.PrintFaintedAction(fainted);
                }
            }

            // TODO: Update execution order after each action (dynamic speed)
        }
    }

    private void HandleTerastalization(List<ActionWithChoice> actions)
    {
        foreach (ActionWithChoice action in actions)
        {
            if (action.Choice is SlotChoice.MoveChoice { IsTera: true } moveChoice)
            {
                moveChoice.Attacker.Terastillize(Context);
            }
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
                BattleRequestType.ForceSwitch, TimeSpan.FromSeconds(StandardTurnLimitSeconds),
                cancellationToken);

            if (choice is not SlotChoice.SwitchChoice switchChoice)
            {
                throw new InvalidOperationException("Expected a SwitchChoice for forced switch.");
            }

            if (PrintDebug)
            {
                UiGenerator.PrintForceSwitchOutAction(switchChoice.Trainer.Name, switchChoice.SwitchOutPokemon);
            }

            await ExecuteSwitchChoiceAsync(playerId, switchChoice);
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
            BattleChoice choice = await RequestChoiceFromPlayerAsync(playerId, switchChoices,
                BattleRequestType.FaintSwitch,
                TimeSpan.FromSeconds(StandardTurnLimitSeconds), cancellationToken);

            if (choice is not SlotChoice.SwitchChoice switchChoice)
            {
                throw new InvalidOperationException("Expected a SwitchChoice for fainted Pokémon switch.");
            }

            if (PrintDebug)
            {
                UiGenerator.PrintFaintedSelectAction(switchChoice.Trainer.Name, switchChoice.SwitchInPokemon);
            }

            await ExecuteSwitchChoiceAsync(playerId, switchChoice);
        }
    }

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
                _ => throw new InvalidOperationException($"Unknown slot choice type: {slotChoice.GetType().Name}"),
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
        switch (choice)
        {
            case SlotChoice.MoveChoice moveChoice:
                Pokemon attacker = moveChoice.Attacker;
                int priority = moveChoice.Move.Priority;
                return attacker.Ability.OnModifyPriority?.Invoke(priority, moveChoice.Move) ?? priority;
            case SlotChoice.SwitchChoice:
                return 6;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Sort actions by speed/priority order. Handles Trick Room logic.
    /// </summary>
    private List<ActionWithChoice> SortActionsBySpeedOrder(List<ActionWithChoice> actions)
    {
        bool isTrickRoom = Field.HasPseudoWeather(PseudoWeatherId.TrickRoom);

        var sorted = actions.OrderByDescending(a => a.SpeedPriority);

        if (isTrickRoom)
        {
            // In Trick Room, slower Pokémon go first (but priority still takes precedence)
            sorted = sorted.ThenBy(a => a.Executor.CurrentSpe);
        }
        else
        {
            // Normal: faster Pokémon go first
            sorted = sorted.ThenByDescending(a => a.Executor.CurrentSpe);
        }

        return sorted
            .ThenBy(a => BattleRandom.Next()) // Random tiebreaker
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
            case DoubleSlotChoice doubleSlotChoice:
                throw new NotImplementedException();
            case TeamPreviewChoice _:
                throw new InvalidOperationException("Team preview choices should not be executed during gameplay.");
            default:
                throw new InvalidOperationException($"Unknown choice type: {action.Choice.GetType().Name}");
        }
    }

    /// <summary>
    /// Execute a slot choice (move, switch, etc.)
    /// </summary>
    private async Task ExecuteSlotChoiceAsync(PlayerId playerId, SlotChoice slotChoice)
    {
        //if (PrintDebug)
        //    Console.WriteLine($"Executing slot choice for {playerId}: {slotChoice}");

        switch (slotChoice)
        {
            case SlotChoice.MoveChoice moveChoice:
                await ExecuteMoveChoiceAsync(playerId, moveChoice);
                break;
                
            case SlotChoice.SwitchChoice switchChoice:
                await ExecuteSwitchChoiceAsync(playerId, switchChoice);
                break;
                
            default:
                throw new InvalidOperationException($"Unknown slot choice type: {slotChoice.GetType().Name}");
        }
    }

    /// <summary>
    /// Apply end-of-turn effects (weather, status, etc.)
    /// </summary>
    private async Task ApplyEndOfTurnEffectsAsync()
    {
        if (PrintDebug)
        {
            UiGenerator.PrintBlankLine();
        }
        Field.OnTurnEnd(Side1, Side2, Context);
        HandleBeforeResiduals();
        HandleResiduals();
        HandleConditionTurnEnds();

        await Task.CompletedTask;
    }

    private async Task HandleStartOfTurn()
    {
        Field.OnTurnStart(Side1, Side2, Context);
        HandleItemTurnStarts();
        HandleConditionTurnStarts();
        await Task.CompletedTask;
    }

    //private void HandleEndOfTurn()
    //{
    //    if (PrintDebug)
    //    {
    //        UiGenerator.PrintBlankLine();
    //    }
    //    Field.OnTurnEnd(Side1, Side2, Context);
    //    HandleBeforeResiduals();
    //    HandleResiduals();
    //    // some residual effects may have caused pokemon to faint
    //    UpdateFaintedStates();
    //    HandleConditionTurnEnds();
    //}

    private void HandleEndOfTeamPreviewTurn()
    {
        foreach (Pokemon pokemon in AllActivePokemon)
        {
            if (pokemon.IsFainted) continue; // Skip if fainted
            pokemon.OnSwitchIn(Field, AllActivePokemon.ToArray(), Context); // Trigger switch-in effects
        }
    }

    private void HandleBeforeResiduals()
    {
        // check all active pokemon items with OnBeforeResiduals
        foreach (Pokemon pokemon in AllActivePokemon)
        {
            if (pokemon.IsFainted) continue; // Skip if fainted
            pokemon.Item?.OnBeforeResiduals?.Invoke(pokemon, Context);
        }
    }

    private void HandleResiduals()
    {
        Condition[] side1Residuals = [];
        if (!Side1.Slot1.IsFainted) // Skip if fainted
        {
            side1Residuals = Side1.Slot1.GetAllResidualConditions();
        }
        List<(Pokemon, Condition, PlayerId)> side1ResidualsList = [];
        foreach (Condition condition in side1Residuals)
        {
            if (condition.OnResidual != null)
            {
                side1ResidualsList.Add((Side1.Slot1, condition, PlayerId.Player1));
            }
        }

        Condition[] side2Residuals = [];
        if (!Side2.Slot1.IsFainted) // Skip if fainted
        {
            side2Residuals = Side2.Slot1.GetAllResidualConditions();
        }
        List<(Pokemon, Condition, PlayerId)> side2ResidualsList = [];
        foreach (Condition condition in side2Residuals)
        {
            if (condition.OnResidual != null)
            {
                side2ResidualsList.Add((Side2.Slot1, condition, PlayerId.Player2));
            }
        }

        // Combine and sort by OnResidualOrder
        var allResiduals = side1ResidualsList.Concat(side2ResidualsList)
            .OrderBy(t => t.Item2.OnResidualOrder ?? int.MaxValue)
            .ToList();

        foreach ((Pokemon pokemon, Condition condition, PlayerId playerId) in allResiduals)
        {
            Side sourceSide = GetSide(playerId.OpposingPlayerId());

            condition.OnResidual?.Invoke(pokemon, sourceSide, condition, Context);

            if (!condition.Duration.HasValue) continue;

            condition.Duration--;
            if (condition.Duration <= 0)
            {
                pokemon.RemoveCondition(condition.Id);
            }
        }
    }

    private void HandleConditionTurnStarts()
    {

    }

    private void HandleConditionTurnEnds()
    {
        List<Condition> side1Conditions = [];
        if (!Side1.Slot1.IsFainted) // Skip if fainted
        {
            side1Conditions = Side1.Slot1.Conditions.ToList();
        }
        foreach (Condition condition in side1Conditions.ToList())
        {
            condition.OnTurnEnd?.Invoke(Side1.Slot1, Context);
            if (!condition.Duration.HasValue) continue;
            condition.Duration--;
            if (condition.Duration <= 0)
            {
                Side1.Slot1.RemoveCondition(condition.Id);
            }
        }

        List<Condition> side2Conditions = [];
        if (!Side2.Slot1.IsFainted) // Skip if fainted
        {
            side2Conditions = Side2.Slot1.Conditions.ToList();
        }
        foreach (Condition condition in side2Conditions.ToList())
        {
            condition.OnTurnEnd?.Invoke(Side2.Slot1, Context);
            if (!condition.Duration.HasValue) continue;
            condition.Duration--;
            if (condition.Duration <= 0)
            {
                Side2.Slot1.RemoveCondition(condition.Id);
            }
        }
    }

    private void HandleItemTurnStarts()
    {
        // check all active pokemon items with OnStart
        foreach (Pokemon pokemon in AllActivePokemon)
        {
            if (pokemon.IsFainted) continue; // Skip if fainted
            pokemon.Item?.OnStart?.Invoke(pokemon, Context);
        }
    }
}