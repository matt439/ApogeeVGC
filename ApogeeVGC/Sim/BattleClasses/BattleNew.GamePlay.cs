using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Turns;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils.Extensions;
using static ApogeeVGC.Sim.Choices.SlotChoice;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    public async Task ProcessGameplayTurnAsync(GameplayTurn turn, CancellationToken cancellationToken)
    {
        if (TurnCounter == 1)
        {
            // Handle switch-in effects at the start of the first turn
            HandleEndOfTeamPreviewTurn();
        }

        ExecutionStage = GameplayExecutionStage.TurnStart;

        // Phase 1: Start-of-turn effects
        await HandleStartOfTurn();

        // Phase 2: Collect turn start choices
        var initialActions = await CollectTurnStartActionsAsync(cancellationToken);

        // Phase 3: Execute actions in priority order with dynamic interruptions
        await ExecuteActionsWithDynamicSwitchesAsync(initialActions, cancellationToken);

        // Check for game end conditions
        if (CheckForGameEndConditions())
        {
            await HandleNormalGameEndAsync();
            return;
        }

        // Phase 4: Apply end-of-turn effects
        await ApplyEndOfTurnEffectsAsync();

        // Check for game end conditions
        if (CheckForGameEndConditions())
        {
            await HandleNormalGameEndAsync();
            return;
        }

        // Phase 5: Handle end-of-turn forced switches (fainted Pokémon)
        ExecutionStage = GameplayExecutionStage.FaintedSwitch;
        await HandleFaintedSwitchesAsync(cancellationToken);

        // Complete the turn
        CompleteTurnWithEndStates();

        if (!CheckForGameEndConditions())
        {
            await CreateNextTurnAsync();
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
        var activePokemon = side.AliveActivePokemon.ToArray();

        var availableChoices = activePokemon.Length switch
        {
            1 => GenerateTurnStartChoices(activePokemon[0]),
            2 => GenerateDoublesTurnStartChoices(side.GetSlot(SlotId.Slot1), side.GetSlot(SlotId.Slot2)),
            _ => throw new InvalidOperationException(
                $"Expected 1 or 2 active Pokémon for {playerId}, but found {activePokemon.Length}."),
        };

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

        return actions;
    }

    private async Task ExecuteActionsWithDynamicSwitchesAsync(List<ActionWithChoice> actions,
        CancellationToken cancellationToken)
    {
        // split any double actions into single actions
        List<ActionWithChoice> singleActions = [];
        foreach (ActionWithChoice action in actions)
        {
            if (action.Choice is DoubleSlotChoice doubleSlotChoice)
            {
                // Split into two single actions
                var action1 = new ActionWithChoice
                {
                    PlayerId = action.PlayerId,
                    Choice = doubleSlotChoice.Slot1Choice,
                    SpeedPriority = GetSpeedPriorityForChoice(doubleSlotChoice.Slot1Choice),
                    ActionOrder = action.ActionOrder,
                    Executor = GetExecutorPokemonForChoice(action.PlayerId, doubleSlotChoice.Slot1Choice),
                };
                var action2 = new ActionWithChoice
                {
                    PlayerId = action.PlayerId,
                    Choice = doubleSlotChoice.Slot2Choice,
                    SpeedPriority = GetSpeedPriorityForChoice(doubleSlotChoice.Slot2Choice),
                    ActionOrder = action.ActionOrder + 1,
                    Executor = GetExecutorPokemonForChoice(action.PlayerId, doubleSlotChoice.Slot2Choice),
                };
                singleActions.Add(action1);
                singleActions.Add(action2);
            }
            else
            {
                singleActions.Add(action);
            }
        }

        // TODO: Have teras activate after switches but before moves
        HandleTerastalization(singleActions);
        
        // Sort by priority/speed
        var sortedActions = SortActionsBySpeedOrder(singleActions);

        foreach (ActionWithChoice action in sortedActions)
        {
            if (CurrentTurn is not GameplayTurn gameplayTurn)
            {
                throw new InvalidOperationException();
            }
            // save move history into turn for debugging
            gameplayTurn.SetChoice(action.Choice.SideId, action.Executor.SlotId, action.Choice);
            
            // Check for game end before each action
            if (CheckForGameEndConditions())
            {
                await HandleNormalGameEndAsync();
                return;
            }

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
                ExecutionStage = GameplayExecutionStage.ForceSwitch;
                ForceSwitcher = action.Executor;
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
            if (action.Choice is MoveChoice { IsTera: true } moveChoice)
            {
                moveChoice.Attacker.Terastillize(Context);
            }
        }
    }

    private bool RequiresImmediateForcedSwitch(ActionWithChoice action)
    {
        // Check if this action causes an immediate forced switch
        if (action.Choice is MoveChoice moveChoice)
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

            if (choice is not SwitchChoice switchChoice)
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

    private async Task HandleFaintedSwitchesAsync(CancellationToken cancellationToken)
    {
        // Handle fainted Pokémon switches in player order
        await HandleFaintedSwitchesForPlayer(PlayerId.Player1, cancellationToken);
        await HandleFaintedSwitchesForPlayer(PlayerId.Player2, cancellationToken);
    }

    private BattleChoice[] GenerateFaintedSwitchChoicesForPlayer(PlayerId playerId)
    {
        Side side = GetCurrentSide(playerId);

        if (side.SwitchOptionsCount <= 0)
        {
            return []; // No available switches
        }

        var faintedActivePokemon = side.FaintedActivePokemon.ToArray();

        BattleChoice[] switchChoices;

        switch (faintedActivePokemon.Length)
        {
            case 0:
                return []; // No fainted Pokémon to switch
            case 1:
                switchChoices = GenerateFaintedSwitchChoices(faintedActivePokemon[0]);
                break;
            case 2:
                switchChoices = GenerateDoublesFaintedSwitchChoices(faintedActivePokemon[0], faintedActivePokemon[1]);
                break;
            default:
                throw new InvalidOperationException(
                    $"Expected 1 or 2 fainted active Pokémon for {playerId}, but found {faintedActivePokemon.Length}.");
        }

        return switchChoices;
    }

    private async Task HandleFaintedSwitchesForPlayer(PlayerId playerId, CancellationToken cancellationToken)
    {
        var switchChoices = GenerateFaintedSwitchChoicesForPlayer(playerId);

        if (switchChoices.Length == 0)
        {
            return; // No switches needed
        }

        BattleChoice choice = await RequestChoiceFromPlayerAsync(playerId, switchChoices,
            BattleRequestType.FaintSwitch, TimeSpan.FromSeconds(StandardTurnLimitSeconds), cancellationToken);

        switch (choice)
        { 
            case SwitchChoice switchChoice:
                if (PrintDebug)
                {
                    UiGenerator.PrintFaintedSelectAction(switchChoice.Trainer.Name, switchChoice.SwitchInPokemon);
                }
                await ExecuteSwitchChoiceAsync(playerId, switchChoice);
                break;
            case DoubleSlotChoice doubleSlotChoice:

                if (doubleSlotChoice.Slot1Choice is not SwitchChoice switchChoice1 ||
                    doubleSlotChoice.Slot2Choice is not SwitchChoice switchChoice2)
                {
                    throw new InvalidOperationException("Both choices in DoubleSlotChoice must be SwitchChoices.");
                }
                if (PrintDebug)
                {
                    UiGenerator.PrintFaintedSelectAction(switchChoice1.Trainer.Name, switchChoice2.SwitchInPokemon);
                }
                await ExecuteSwitchChoiceAsync(playerId, switchChoice1);
                await ExecuteSwitchChoiceAsync(playerId, switchChoice2);
                break;
            default:
                throw new InvalidOperationException("Expected a SwitchChoice or DoubleSlotChoice for fainted switch.");
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
                MoveChoice moveChoice => moveChoice.Attacker,
                SwitchChoice switchChoice => switchChoice.SwitchOutPokemon,
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
            case MoveChoice moveChoice:
                Pokemon attacker = moveChoice.Attacker;
                int priority = moveChoice.Move.Priority;
                return attacker.Ability.OnModifyPriority?.Invoke(priority, moveChoice.Move) ?? priority;
            case SwitchChoice:
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
            .ThenBy(_ => BattleRandom.Next()) // Random tiebreaker
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
            case DoubleSlotChoice:
            case TeamPreviewChoice:
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
        switch (slotChoice)
        {
            case MoveChoice moveChoice:
                await ExecuteMoveChoiceAsync(playerId, moveChoice);
                break;
                
            case SwitchChoice switchChoice:
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
        var allResiduals = new List<(Pokemon, Condition, PlayerId)>();

        // Process Side1 active Pokemon
        foreach (Pokemon pokemon in Side1.AliveActivePokemon)
        {
            AddResidualConditions(allResiduals, pokemon, PlayerId.Player1);
        }

        // Process Side2 active Pokemon  
        foreach (Pokemon pokemon in Side2.AliveActivePokemon)
        {
            AddResidualConditions(allResiduals, pokemon, PlayerId.Player2);
        }

        // Sort and execute residuals
        var sortedResiduals = allResiduals
            .OrderBy(t => t.Item2.OnResidualOrder ?? int.MaxValue)
            .ThenBy(t => t.Item3) // Player order as tiebreaker
            .ThenBy(t => t.Item1.SlotId) // Slot order as secondary tiebreaker
            .ToList();

        foreach ((Pokemon pokemon, Condition condition, PlayerId playerId) in sortedResiduals)
        {
            if (pokemon.IsFainted) continue;

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

    private void HandleConditionTurnEnds()
    {
        // Process each side's active Pokemon
        HandleConditionTurnEndsForSide(Side1);
        HandleConditionTurnEndsForSide(Side2);
    }

    // Helper methods
    private static void AddResidualConditions(List<(Pokemon, Condition, PlayerId)> residuals,
        Pokemon pokemon, PlayerId playerId)
    {
        var conditions = pokemon.GetAllResidualConditions();
        foreach (Condition condition in conditions)
        {
            if (condition.OnResidual != null)
            {
                residuals.Add((pokemon, condition, playerId));
            }
        }
    }

    private void HandleConditionTurnEndsForSide(Side side)
    {
        foreach (Pokemon pokemon in side.AliveActivePokemon.ToList())
        {
            var conditions = pokemon.Conditions.ToList();

            foreach (Condition condition in conditions.TakeWhile(_ => !pokemon.IsFainted))
            {
                condition.OnTurnEnd?.Invoke(pokemon, Context);

                if (!condition.Duration.HasValue) continue;

                condition.Duration--;
                if (condition.Duration <= 0)
                {
                    pokemon.RemoveCondition(condition.Id);
                }
            }
        }
    }

    private void HandleConditionTurnStarts()
    {

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