using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Extensions;

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
            if (!IsValidChoice())
            {
                if (PrintDebug)
                    Console.WriteLine($"Invalid choice from {playerId}, using default");
                choice = GetDefaultChoice(playerId, availableChoices);
            }

            // Update player time tracking
            TimeSpan actionDuration = DateTime.UtcNow - actionStartTime;
            UpdatePlayerTime(playerId, actionDuration);

            //if (PrintDebug)
            //    Console.WriteLine($"Choice received from {playerId}: {choice} (took {actionDuration.TotalSeconds:F1}s)");

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
                TimeSpan warningDelay = timeLimit.Subtract(TimeSpan.FromSeconds(TimeoutWarningThresholdSeconds));
                if (warningDelay > TimeSpan.Zero)
                {
                    await Task.Delay(warningDelay, cancellationToken);
                    await player.NotifyTimeoutWarningAsync(TimeSpan.FromSeconds(TimeoutWarningThresholdSeconds));
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

    private BattleChoice[] GenerateForcedSwitchChoices(Pokemon pokemon)
    {
        return GenerateSwitchChoices(pokemon);
    }

    private BattleChoice[] GenerateDoublesFaintedSwitchChoices(Pokemon slot1Pokemon, Pokemon slot2Pokemon)
    {
        List<BattleChoice> choices = [];
        choices.AddRange(GenerateFaintedSwitchChoices(slot1Pokemon));
        choices.AddRange(GenerateFaintedSwitchChoices(slot2Pokemon));
        return choices.ToArray();
    }

    private BattleChoice[] GenerateFaintedSwitchChoices(Pokemon pokemon)
    {
        return GenerateSwitchChoices(pokemon);
    }

    private BattleChoice[] GenerateDoublesTurnStartChoices(Pokemon slot1Pokemon, Pokemon slot2Pokemon)
    {
        List<BattleChoice> choices = [];
        choices.AddRange(GenerateTurnStartChoices(slot1Pokemon));
        choices.AddRange(GenerateTurnStartChoices(slot2Pokemon));
        return choices.ToArray();
    }

    /// <summary>
    /// Generate choices at the start of a turn for a given Pokemon.
    /// These can include move choices and switch choices.
    /// </summary>
    private BattleChoice[] GenerateTurnStartChoices(Pokemon pokemon)
    {
        List<BattleChoice> choices = [];
        choices.AddRange(GenerateMoveChoices(pokemon));
        choices.AddRange(GenerateSwitchChoices(pokemon));
        return choices.ToArray();
    }

    /// <summary>
    /// Generate move choices for a given Pokemon.
    /// </summary>
    private BattleChoice[] GenerateMoveChoices(Pokemon pokemon)
    {
        List<BattleChoice> choices = [];
        Side attackingSide = GetSide(pokemon.SideId);
        Side defendingSide = GetSide(pokemon.SideId.GetOppositeSide());
        Pokemon attacker = attackingSide.Slot1;

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

            if (attackingSide.AnyTeraUsed) continue;

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
    /// Generate switch choices for a given Pokemon.
    /// </summary>
    private BattleChoice[] GenerateSwitchChoices(Pokemon pokemon)
    {
        List<BattleChoice> choices = [];
        Side side = GetSide(pokemon.SideId);
        var switchOptionSlots = side.SwitchOptionSlots;

        choices.AddRange(switchOptionSlots.Select(inPokemon =>
            new SlotChoice.SwitchChoice(side.Slot1, inPokemon, Format)));
        return choices.ToArray();
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

    private bool IsValidChoice()
    {
        return true; // TODO: Implement actual validation logic
    }
}