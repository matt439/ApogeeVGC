using ApogeeVGC.Sim.Actions;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public class BattleQueue(Battle battle)
{
    public Battle Battle { get; init; } = battle;
    public List<IAction> List { get; init; } = [];

    public IAction? Shift()
    {
        if (List.Count == 0) return null;
        IAction action = List[0];
        List.RemoveAt(0);
        return action;
    }

    public IAction? Peek(bool end = false)
    {
        if (List.Count == 0) return null;
        return end ? List[^1] : List[0];
    }

    public void Push(IAction action)
    {
        List.Add(action);
    }

    public void Unshift(IAction action)
    {
        List.Insert(0, action);
    }

    public IEnumerator<IAction> GetEnumerator()
    {
        return List.GetEnumerator();
    }

    public IEnumerable<(int Index, IAction Action)> Entries()
    {
        return List.Select((action, index) => (index, action));
    }

    /// <summary>
    /// Takes an ActionChoice, and fills it out into a full Action object.
    /// 
    /// Returns a list of Actions because some ActionChoices (like mega moves)
    /// resolve to multiple Actions (mega evolution + use move)
    /// </summary>
    public List<IAction> ResolveAction(IActionChoice action, bool midTurn = false)
    {
        switch (action)
        {
            case null:
                throw new ArgumentNullException(nameof(action),
                    "Action not passed to ResolveAction");
            // Pass actions return empty list
            case IAction { Choice: ActionId.Pass }:
                return [];
        }

        // Start with the action itself
        List<IAction> actions = [];

        // Populate side from pokemon if not already set (matches TS: if (!action.side && action.pokemon) action.side = action.pokemon.side;)
        // Note: In C# we don't have a mutable side property on IAction, so this is handled implicitly through Pokemon.Side
        // The TypeScript code sets it for convenience, but in C# it's always accessible via action.Pokemon.Side

        // Handle ChosenAction conversion to specific action types
        IAction currentAction;
        if (action is ChosenAction chosenAction)
        {
            // Convert ChosenAction to the appropriate specific action type
            currentAction = chosenAction.Choice switch
            {
                ChoiceType.Team => new TeamAction
                {
                    Choice = ActionId.Team,
                    Pokemon = chosenAction.Pokemon ??
                              throw new InvalidOperationException("Team action requires a Pokemon"),
                    Index = chosenAction.Index ??
                            throw new InvalidOperationException("Team action requires an Index"),
                    Priority = chosenAction.Priority,
                },
                ChoiceType.Move => new MoveAction
                {
                    Choice = ActionId.Move,
                    Pokemon = chosenAction.Pokemon ??
                              throw new InvalidOperationException("Move action requires a Pokemon"),
                    Move = Battle.Library.Moves[chosenAction.MoveId],
                    TargetLoc = chosenAction.TargetLoc ?? 0,
                    Order = 200, // Default order for moves
                    OriginalTarget =
                        chosenAction.Pokemon!, // Will be updated later with actual target
                    Terastallize = chosenAction.Terastallize, // Preserve terastallize flag
                },
                ChoiceType.Switch => new SwitchAction
                {
                    Choice = ActionId.Switch,
                    Pokemon = chosenAction.Pokemon ??
                              throw new InvalidOperationException(
                                  "Switch action requires a Pokemon"),
                    Target = chosenAction.Target ??
                             throw new InvalidOperationException("Switch action requires a Target"),
                    Order = 103, // Order for regular switches
                },
                ChoiceType.InstaSwitch => new SwitchAction
                {
                    Choice = ActionId.InstaSwitch,
                    Pokemon = chosenAction.Pokemon ??
                              throw new InvalidOperationException(
                                  "InstaSwitch action requires a Pokemon"),
                    Target = chosenAction.Target ??
                             throw new InvalidOperationException(
                                 "InstaSwitch action requires a Target"),
                    Order = 3, // Order for instant switches
                },
                _ => action as IAction ??
                     throw new InvalidOperationException(
                         "ChosenAction must be convertible to IAction"),
            };
        }
        else
        {
            // Cast to IAction - all ActionChoices should be IActions in practice
            currentAction = action as IAction ??
                            throw new InvalidOperationException(
                                "ActionChoice must be convertible to IAction");
        }

        // Populate move if missing (from moveId)
        if (currentAction is MoveAction moveAction)
        {
            // Ensure we have the Move object, not just the ID
            currentAction = moveAction with
            {
                Move = moveAction.Move,
            };
        }

        // Set order if not already set
        if (currentAction.Order is FalseIntFalseUnion)
        {
            if (_orders.TryGetValue(currentAction.Choice, out int order))
            {
                // Update order based on action type
                currentAction = currentAction switch
                {
                    MoveAction ma => ma with { Order = order },
                    SwitchAction sa => sa with { Order = order },
                    PokemonAction pa => pa, // PokemonAction.Order returns int.MaxValue
                    _ => currentAction,
                };
            }
            else
            {
                // Default order is 200 for moves and events
                if (currentAction.Choice is not (ActionId.Move or ActionId.Event))
                {
                    throw new InvalidOperationException(
                        $"Unexpected orderless action {currentAction.Choice}");
                }

                if (currentAction is MoveAction maDefault)
                {
                    currentAction = maDefault with { Order = 200 };
                }
            }
        }

        // Process pre-turn actions (not during midTurn)
        if (!midTurn)
        {
            if (currentAction is MoveAction ma)
            {
                // Add BeforeTurnMove action if the move has a beforeTurnCallback (e.g., Focus Punch)
                if (ma.Move.BeforeTurnCallback != null)
                {
                    actions.InsertRange(0, ResolveAction(new MoveAction
                    {
                        Choice = ActionId.BeforeTurnMove,
                        Pokemon = ma.Pokemon,
                        Move = ma.Move,
                        TargetLoc = ma.TargetLoc,
                        Order = 5, // BeforeTurnMove order
                        OriginalTarget = ma.Pokemon, // Will be updated later with actual target
                    }));
                }

                // Note: Mega Evolution and Dynamax are deliberately excluded as per requirements

                // Add Terastallize action if the player chose to terastallize
                if (ma.Terastallize.HasValue &&
                    ma.Pokemon is { CanTerastallize: MoveTypeMoveTypeFalseUnion, Terastallized: null })
                {
                    // Insert Terastallize action before the move
                    actions.InsertRange(0, ResolveAction(new PokemonAction
                    {
                        Choice = ActionId.Terastallize,
                        Pokemon = ma.Pokemon,
                    }));
                }

                // Add PriorityChargeMove action if the move has a priorityChargeCallback (e.g., Beak Blast, Shell Trap)
                if (ma.Move.PriorityChargeCallback != null)
                {
                    actions.InsertRange(0, ResolveAction(new MoveAction
                    {
                        Choice = ActionId.PriorityChargeMove,
                        Pokemon = ma.Pokemon,
                        Move = ma.Move,
                        Order = 107, // PriorityChargeMove order
                        OriginalTarget = ma.Pokemon, // Will be updated later with actual target
                    }));
                }

                // Calculate fractional priority from events
                RelayVar? fractionalPriorityEvent = Battle.RunEvent(
                    EventId.FractionalPriority,
                    ma.Pokemon,
                    null,
                    ma.Move.ToActiveMove(),
                    0
                );

                int fractionalPriority = fractionalPriorityEvent switch
                {
                    IntRelayVar irv => irv.Value,
                    DecimalRelayVar drv => (int)drv.Value,
                    _ => 0,
                };

                currentAction = ma with { FractionalPriority = fractionalPriority };
            }
            else if (currentAction.Choice is ActionId.Switch or ActionId.InstaSwitch)
            {
                // Handle switch source effect if switch was forced by a move
                if (currentAction is SwitchAction sa)
                {
                    // Check if the switch was caused by a move (stored in SwitchFlag)
                    if (sa.Pokemon.SwitchFlag is MoveIdMoveIdBoolUnion moveIdUnion)
                    {
                        // Set the source effect to the move that caused the switch
                        currentAction = sa with
                        {
                            SourceEffect = Battle.Library.Moves[moveIdUnion.MoveId].ToActiveMove(),
                        };
                    }

                    // Clear the switch flag now that we've processed it
                    sa.Pokemon.SwitchFlag = false;
                }
            }
        }

        // Handle target resolution for moves
        if (currentAction is MoveAction moveAct)
        {
            Move move = moveAct.Move;

            // If no target location specified, get a random target
            if (moveAct.TargetLoc == 0)
            {
                Pokemon? target = Battle.GetRandomTarget(moveAct.Pokemon, move);
                if (target is not null)
                {
                    moveAct = moveAct with
                    {
                        TargetLoc =
                        moveAct.Pokemon.GetSlot().GetRelativeLocation(target.GetSlot())
                    };
                }
            }

            // Set the original target based on target location
            currentAction = moveAct with
            {
                OriginalTarget = moveAct.Pokemon.GetAtLoc(moveAct.TargetLoc),
            };
        }

        // Calculate action speed for queue sorting and get the updated action
        currentAction = Battle.GetActionSpeed(currentAction);

        // Add the action to the list
        actions.Add(currentAction);
        return actions;
    }

    /// <summary>
    /// Makes the passed action happen next (skipping speed order).
    /// Removes the action from its current position in the queue,
    /// sets its order to 3 (InstaSwitch priority), and places it at the front.
    /// </summary>
    public void PrioritizeAction(MoveSwitchActionUnion action, IEffect? sourceEffect = null)
    {
        // Extract the actual action from the union
        IAction actualAction = action switch
        {
            MoveActionMoveSwitchActionUnion ma => ma.MoveAction,
            SwitchActionMoveSwitchActionUnion sa => sa.SwitchAction,
            _ => throw new InvalidOperationException(
                "Unknown action type in MoveSwitchActionUnion"),
        };

        // Remove the action from its current position if it exists
        for (int i = 0; i < List.Count; i++)
        {
            if (List[i] == actualAction)
            {
                List.RemoveAt(i);
                break;
            }
        }

        // Update the action with the source effect and priority order
        IAction prioritizedAction = action switch
        {
            MoveActionMoveSwitchActionUnion ma => ma.MoveAction with
            {
                SourceEffect = sourceEffect,
                Order = 3, // InstaSwitch priority
            },
            SwitchActionMoveSwitchActionUnion sa => sa.SwitchAction with
            {
                SourceEffect = sourceEffect,
                Order = 3, // InstaSwitch priority
            },
            _ => throw new InvalidOperationException(
                "Unknown action type in MoveSwitchActionUnion"),
        };

        // Add to the front of the queue
        List.Insert(0, prioritizedAction);
    }

    /// <summary>
    /// Changes a pokemon's action, and inserts its new action
    /// in priority order.
    ///
    /// You'd normally want the OverrideAction event (which doesn't
    /// change priority order).
    /// </summary>
    public void ChangeAction(Pokemon pokemon, IActionChoice action)
    {
        // Cancel any existing actions for this Pokémon
        CancelAction(pokemon);

        // Ensure the action has the Pokémon reference set
        if (action is IAction actionWithPokemon && actionWithPokemon.Pokemon != pokemon)
        {
            // Update the action with the correct Pokémon
            action = action switch
            {
                MoveAction ma => ma with { Pokemon = pokemon },
                SwitchAction sa => sa with { Pokemon = pokemon },
                PokemonAction pa => pa with { Pokemon = pokemon },
                TeamAction ta => ta with { Pokemon = pokemon },
                _ => action,
            };
        }

        // Insert the new action in priority order
        InsertChoice(action);
    }

    public void AddChoice(IActionChoice choice)
    {
        var resolvedChoices = ResolveAction(choice);
        List.AddRange(resolvedChoices);

        // Track last selected move for each side (used by Copycat, Mirror Move, etc.)
        foreach (IAction resolvedChoice in resolvedChoices)
        {
            if (resolvedChoice is MoveAction { Choice: ActionId.Move } ma &&
                ma.Move.Id != MoveId.Recharge)
            {
                ma.Pokemon.Side.LastSelectedMove = ma.Move.Id;
            }
        }
    }

    public void AddChoice(IEnumerable<IActionChoice> choices)
    {
        foreach (IActionChoice choice in choices)
        {
            var resolvedChoices = ResolveAction(choice);
            List.AddRange(resolvedChoices);

            // Track last selected move for each side (used by Copycat, Mirror Move, etc.)
            foreach (IAction resolvedChoice in resolvedChoices)
            {
                if (resolvedChoice is MoveAction { Choice: ActionId.Move } ma &&
                    ma.Move.Id != MoveId.Recharge)
                {
                    ma.Pokemon.Side.LastSelectedMove = ma.Move.Id;
                }
            }
        }
    }

    public IAction? WillAct()
    {
        foreach (IAction action in List)
        {
            if (action.Choice is ActionId.Move or ActionId.Switch or ActionId.InstaSwitch
                or ActionId.Shift)
            {
                return action;
            }
        }

        return null;
    }

    /// <summary>
    /// Checks if the specified Pokémon has a queued move action.
    /// Returns the move action if found, or null if the Pokémon has fainted or has no move queued.
    /// </summary>
    /// <param name="pokemon">The Pokémon to check for a queued move</param>
    /// <returns>The queued MoveAction if found, otherwise null</returns>
    public MoveAction? WillMove(Pokemon pokemon)
    {
        if (pokemon.Fainted) return null;

        foreach (IAction action in List)
        {
            if (action.Choice is ActionId.Move && action is MoveAction moveAction &&
                action.Pokemon == pokemon)
            {
                return moveAction;
            }
        }

        return null;
    }

    /// <summary>
    /// Removes all actions associated with the specified Pokémon from the queue.
    /// </summary>
    /// <param name="pokemon">The Pokémon whose actions should be cancelled</param>
    /// <returns>True if any actions were removed, false otherwise</returns>
    public bool CancelAction(Pokemon pokemon)
    {
        int oldLength = List.Count;

        for (int i = 0; i < List.Count; i++)
        {
            if (List[i].Pokemon != pokemon) continue;
            List.RemoveAt(i);
            i--; // Decrement to account for the removed item
        }

        return List.Count != oldLength;
    }

    /// <summary>
    /// Removes the first queued move action for the specified Pokémon from the queue.
    /// </summary>
    /// <param name="pokemon">The Pokémon whose move action should be cancelled</param>
    /// <returns>True if a move action was removed, false otherwise</returns>
    public bool CancelMove(Pokemon pokemon)
    {
        for (int i = 0; i < List.Count; i++)
        {
            if (List[i].Choice is not ActionId.Move || List[i].Pokemon != pokemon) continue;
            List.RemoveAt(i);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if the specified Pokémon has a queued switch or instaswitch action.
    /// Returns the switch action if found, or null if the Pokémon has no switch queued.
    /// </summary>
    /// <param name="pokemon">The Pokémon to check for a queued switch</param>
    /// <returns>The queued switch action (MoveAction, SwitchAction, TeamAction, or PokemonAction) if found,
    /// otherwise null
    /// </returns>
    public MoveSwitchTeamPokemonActionUnion? WillSwitch(Pokemon pokemon)
    {
        foreach (IAction action in List)
        {
            if (action.Choice is ActionId.Switch or ActionId.InstaSwitch &&
                action.Pokemon == pokemon)
            {
                // Return the appropriate union type based on the action type
                return action switch
                {
                    MoveAction ma => new MoveActionMoveSwitchTeamPokemonActionUnion(ma),
                    SwitchAction sa => new SwitchActionMoveSwitchTeamPokemonActionUnion(sa),
                    TeamAction ta => new TeamActionMoveSwitchTeamPokemonActionUnion(ta),
                    PokemonAction pa => new PokemonActionMoveSwitchTeamPokemonActionUnion(pa),
                    _ => null,
                };
            }
        }

        return null;
    }

    /// <summary>
    /// Inserts the passed action into the action queue when it normally
    /// would have happened (sorting by priority/speed), without
    /// re-sorting the existing actions.
    /// </summary>
    /// <param name="choice">The action choice to insert</param>
    /// <param name="midTurn">Whether this is being inserted mid-turn</param>
    public void InsertChoice(IActionChoice choice, bool midTurn = false)
    {
        // Update the pokemon's speed if it has one
        if (choice is IAction { Pokemon: not null } action)
        {
            action.Pokemon.UpdateSpeed();
        }

        // Resolve the choice into one or more actions
        var actions = ResolveAction(choice, midTurn);

        // If no actions were generated, nothing to insert
        if (actions.Count == 0) return;

        // Find the insertion range based on priority comparison
        int? firstIndex = null;
        int? lastIndex = null;

        for (int i = 0; i < List.Count; i++)
        {
            IAction curAction = List[i];
            int compared = Battle.ComparePriority(actions[0], curAction);

            // Mark the first position where our action should go
            // (when it has equal or higher priority than the current action)
            if (compared <= 0 && firstIndex == null)
            {
                firstIndex = i;
            }

            // Mark the last position in the range
            // (when our action has strictly higher priority)
            if (compared >= 0) continue;
            lastIndex = i;
            break;
        }

        // If we never found a position, append to the end
        if (firstIndex == null)
        {
            List.AddRange(actions);
        }
        else
        {
            // If we didn't find a lastIndex, it means we should insert at the end of the range
            lastIndex ??= List.Count;

            // Randomly select an index within the valid range to preserve speed tie randomness
            int index = firstIndex == lastIndex
                ? firstIndex.Value
                : Battle.Random(firstIndex.Value, lastIndex.Value + 1);

            // Insert the actions at the selected position
            List.InsertRange(index, actions);
        }
    }

    /// <summary>
    /// Inserts multiple action choices into the queue.
    /// This is a convenience overload that calls InsertChoice for each choice.
    /// </summary>
    /// <param name="choices">The list of action choices to insert</param>
    /// <param name="midTurn">Whether this is being inserted mid-turn</param>
    public void InsertChoice(List<IActionChoice> choices, bool midTurn = false)
    {
        foreach (IActionChoice choice in choices)
        {
            InsertChoice(choice, midTurn);
        }
    }

    /// <summary>
    /// Clears all actions from the queue.
    /// </summary>
    public void Clear()
    {
        List.Clear();
    }

    /// <summary>
    /// Returns a debug string representation of an action or the entire queue.
    /// Useful for debugging action ordering and priority issues.
    /// </summary>
    /// <param name="action">The action to debug, or null to debug the entire queue</param>
    /// <returns>A formatted debug string</returns>
    public string Debug(IAction? action = null)
    {
        if (action != null)
        {
            // Debug a single action
            return DebugAction(action);
        }

        // Debug the entire queue
        if (List.Count == 0)
        {
            return "Queue is empty";
        }

        var debugLines = new List<string> { "Battle Queue:" };
        for (int i = 0; i < List.Count; i++)
        {
            debugLines.Add($"  [{i}] {DebugAction(List[i])}");
        }

        return string.Join("\n", debugLines);
    }

    /// <summary>
    /// Helper method to format a single action for debugging.
    /// </summary>
    private static string DebugAction(IAction action)
    {
        string pokemonName = action.Pokemon?.Name ?? "None";
        string choiceStr = action.Choice.ToString();

        return action switch
        {
            MoveAction ma => $"{choiceStr} | {pokemonName} | {ma.Move.Name} | " +
                             $"Order: {ma.Order} | Priority: {ma.Priority} | Speed: {ma.Speed}",
            SwitchAction sa => $"{choiceStr} | {pokemonName} -> {sa.Target.Name} | " +
                               $"Order: {sa.Order} | Speed: {sa.Speed}",
            PokemonAction pa => $"{choiceStr} | {pokemonName} | " +
                                $"Order: {pa.Order} | Speed: {pa.Speed}",
            _ => $"{choiceStr} | {pokemonName}",
        };
    }

    /// <summary>
    /// Sorts the action queue by priority/speed order.
    /// Uses the battle's SpeedSort method which properly handles speed ties.
    /// </summary>
    /// <returns>This queue (for method chaining)</returns>
    public BattleQueue Sort()
    {
        Battle.SpeedSort(List);
        return this;
    }

    #region Helpers

    private readonly Dictionary<ActionId, int> _orders = new()
    {
        { ActionId.Team, 1 },
        { ActionId.Start, 2 },
        { ActionId.InstaSwitch, 3 },
        { ActionId.BeforeTurn, 4 },
        { ActionId.BeforeTurnMove, 5 },
        { ActionId.RevivalBlessing, 6 },

        { ActionId.RunSwitch, 101 },
        { ActionId.Switch, 103 },
        { ActionId.Terastallize, 106 },
        { ActionId.PriorityChargeMove, 107 },

        { ActionId.Shift, 200 },
        // Default for moves is 200

        { ActionId.Residual, 300 },
    };

    #endregion
}