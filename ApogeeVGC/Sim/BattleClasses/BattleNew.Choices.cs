using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleNew
{
    /// <summary>
    /// Request a choice from a player with timeout handling
    /// </summary>
    private async Task<BattleChoice> RequestChoiceFromPlayerAsync(PlayerId playerId,
        BattleChoice[] availableChoices, BattleRequestType requestType, TimeSpan timeLimit,
        CancellationToken cancellationToken)
    {
        IPlayerNew player = GetPlayer(playerId);
        CancellationTokenSource playerTokenSource = GetPlayerCancellationTokenSource(playerId);

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
            DateTime actionStartTime = DateTime.UtcNow;

            // Fire choice requested event
            var eventArgs = new ChoiceRequestEventArgs
            {
                AvailableChoices = availableChoices,
                TimeLimit = timeLimit,
                RequestTime = actionStartTime,
            };

            // Note: We cannot invoke events directly on interfaces, so we skip this
            // The player implementations should handle this internally if needed

            // Start timeout warning task (warn at 10 seconds remaining)
            Task warningTask = StartTimeoutWarningTask(player, timeLimit, combinedTokenSource.Token);

            // Request choice asynchronously
            BattleChoice choice = await player.GetNextChoiceAsync(availableChoices, requestType,
                GetPerspective(playerId), combinedTokenSource.Token);

            // Validate the choice
            if (!IsValidChoice(playerId, choice, availableChoices))
            {
                if (PrintDebug)
                    Console.WriteLine($"Invalid choice from {playerId}, using default");
                choice = GetDefaultChoice(playerId, availableChoices);
            }

            // Update player time tracking
            TimeSpan actionDuration = DateTime.UtcNow - actionStartTime;
            UpdatePlayerTime(playerId, actionDuration);

            if (PrintDebug)
                Console.WriteLine($"Choice received from {playerId}: {choice} (took {actionDuration.TotalSeconds:F1}s)");

            return choice;
        }
        catch (OperationCanceledException) when (timeoutTokenSource.Token.IsCancellationRequested)
        {
            // TurnStart timeout occurred
            if (PrintDebug)
                Console.WriteLine($"TurnStart timeout for {playerId}");

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
                TimeSpan warningDelay = timeLimit.Subtract(TimeSpan.FromSeconds(10));
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
        Side side = GetSide(action.PlayerId);
        var choices = new List<BattleChoice>();

        if (PrintDebug)
            Console.WriteLine($"Generating choices for {action.PlayerId} action {action.ActionIndex}");

        // Get active Pokemon for this action
        Pokemon? activePokemon = GetActivePokemonForAction(side, action.ActionIndex);
        
        if (activePokemon != null)
        {
            // Add move choices
            choices.AddRange(GetMoveChoices(side));
            
            // Add switch choices if Pokemon can switch
            if (CanPokemonSwitch(activePokemon))
            {
                choices.AddRange(GetSwitchChoices(side, Format));
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
            _ => false,
        };
    }

    private BattleChoice[] GetMoveChoices(Side side)
    {
        List<BattleChoice> choices = [];
        Side defendingSide = GetOpponentSide(side);
        Pokemon attacker = side.Slot1;
        bool isTeraUsed = side.AnyTeraUsed;

        if (attacker.IsFainted)
        {
            throw new InvalidOperationException("Cannot get move choices for a fainted Pokémon.");
        }

        Pokemon defender = defendingSide.Slot1;
        Pokemon[] aliveDefenders = defender.IsFainted ? [] : [defender];

        if (aliveDefenders.Length == 0)
        {
            throw new InvalidOperationException("No alive opposing Pokémon to target with moves.");
        }

        Pokemon? ally = null;

        foreach (Move move in attacker.Moves)
        {
            // Check if the move is available (has PP left and not disabled)
            if (move is not { Pp: > 0, Disabled: false }) continue;

            MoveTarget target = move.Target;
            MoveNormalTarget targetType;
            if (target == MoveTarget.Normal)
            {
                targetType = MoveNormalTarget.FoeSlot1;
            }
            else
            {
                targetType = MoveNormalTarget.None;
            }

            var possibleTargets = GetPossibleTargets(move, attacker, ally, aliveDefenders);

            SlotChoice.MoveChoice moveChoice = new(attacker, move,
                    false, targetType, possibleTargets);
            choices.Add(moveChoice);

            if (isTeraUsed) continue;

            SlotChoice.MoveChoice moveChoiceTera = new(attacker, move,
                true, targetType, possibleTargets);
            choices.Add(moveChoiceTera);
        }

        if (choices.Count != 0) return choices.ToArray();

        // If no moves are available, Struggle is the only option
        SlotChoice.MoveChoice struggleChoice = new(attacker, Library.Moves[MoveId.Struggle],
            false, MoveNormalTarget.FoeSlot1, [GetStruggleTarget([defender])]);
        choices.Add(struggleChoice);

        return choices.ToArray();
    }

    private Pokemon[] GetPossibleTargets(Move move, Pokemon attacker, Pokemon? ally, Pokemon[] opponents)
    {
        switch (move.Target)
        {
            case MoveTarget.AdjacentAlly:
            case MoveTarget.AdjacentAllyOrSelf:
            case MoveTarget.AdjacentFoe:
            case MoveTarget.All:
            case MoveTarget.AllAdjacent:
                throw new NotImplementedException();
            case MoveTarget.AllAdjacentFoes:
                return opponents;
            case MoveTarget.Allies:
                throw new NotImplementedException();
            case MoveTarget.AllySide:
                return [];
            case MoveTarget.AllyTeam:
            case MoveTarget.Any:
                throw new NotImplementedException();
            case MoveTarget.FoeSide:
                return [];
            case MoveTarget.Normal:
                {
                    var targets = opponents.ToList();
                    if (ally is not null)
                    {
                        targets.Add(ally);
                    }
                    return targets.ToArray();
                }
            case MoveTarget.RandomNormal:
            case MoveTarget.Scripted:
                throw new NotImplementedException();
            case MoveTarget.Self:
                return [attacker];
            case MoveTarget.None:
            case MoveTarget.Field:
                return [];
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Pokemon GetStruggleTarget(Pokemon[] targets)
    {
        // In a singles battle, Struggle always targets the opposing active Pokémon
        // In a doubles battle, Struggle randomly targets one of the opposing active Pokémon

        // TODO: Implement for doubles battles
        return targets[0];
    }

    /// <summary>
    /// Generate switch choices for a Pokemon
    /// </summary>
    private static BattleChoice[] GetSwitchChoices(Side side, BattleFormat format)
    {
        List<BattleChoice> choices = [];
        var switchOptionSlots = side.SwitchOptionSlots;

        choices.AddRange(switchOptionSlots.Select(pokemon =>
            new SlotChoice.SwitchChoice(side.Slot1, pokemon, format)));
        return choices.ToArray();
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
        Side side = GetSide(playerId);
        Pokemon? activePokemon = side.AllSlots.FirstOrDefault(p => !p.IsFainted && IsActivePokemon(p, side));
        
        if (activePokemon == null)
            throw new InvalidOperationException($"No active Pokemon found for {playerId}");

        // Find struggle move or use the first move as fallback
        Move struggleMove = activePokemon.Moves.FirstOrDefault(m => m.Id == MoveId.Struggle) 
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