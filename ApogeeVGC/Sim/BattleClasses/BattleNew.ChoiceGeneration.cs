using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Turns;
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
        Side side = GetSide(slot1Pokemon.SideId);
        var availableBenchPokemon = side.SwitchOptionSlots.ToArray();

        switch (availableBenchPokemon.Length)
        {
            case 0:
                break;
            case 1:
            {
                Pokemon singleBenchPokemon = availableBenchPokemon[0];
                Pokemon fasterFaintedPokemon = GetFasterPokemon(slot1Pokemon, slot2Pokemon);
                choices.Add(new SlotChoice.SwitchChoice(fasterFaintedPokemon, singleBenchPokemon, Format));
                break;
            }
            default: // availableBenchPokemon.Length >= 2
                {
                var validCombinations = from i in
                        Enumerable.Range(0, availableBenchPokemon.Length)
                    from j in Enumerable.Range(0, availableBenchPokemon.Length)
                    select new
                    {
                        Index1 = i,
                        Index2 = j,
                        Pokemon1 = availableBenchPokemon[i],
                        Pokemon2 = availableBenchPokemon[j],
                    };

                foreach (var combo in validCombinations)
                {
                    try
                    {
                        var switch1 = new SlotChoice.SwitchChoice(slot1Pokemon, combo.Pokemon1, Format);
                        var switch2 = new SlotChoice.SwitchChoice(slot2Pokemon, combo.Pokemon2, Format);
                        choices.Add(new DoubleSlotChoice(switch1, switch2));
                    }
                    catch (ArgumentException)
                    {
                        // Invalid combination, skip
                    }
                }
                break;
            }
        }

        return choices.ToArray();
    }

    private static Pokemon GetFasterPokemon(Pokemon pokemon1, Pokemon pokemon2)
    {
        int speed1 = pokemon1.CurrentSpe;
        int speed2 = pokemon2.CurrentSpe;

        if (speed1 > speed2)
        {
            return pokemon1;
        }
        return speed2 > speed1 ? pokemon2 : pokemon1; // Slot1 gets priority in ties
    }

    private BattleChoice[] GenerateFaintedSwitchChoices(Pokemon pokemon)
    {
        return GenerateSwitchChoices(pokemon);
    }

    private BattleChoice[] GenerateDoublesTurnStartChoices(Pokemon slot1Pokemon, Pokemon slot2Pokemon)
    {
        // Ensure slot1Pokemon is actually in slot 1 and slot2Pokemon is in slot 2
        if (slot1Pokemon.SlotId != SlotId.Slot1)
        {
            throw new ArgumentException($"slot1Pokemon must be in Slot1, but was in {slot1Pokemon.SlotId}");
        }
        if (slot2Pokemon.SlotId != SlotId.Slot2)
        {
            throw new ArgumentException($"slot2Pokemon must be in Slot2, but was in {slot2Pokemon.SlotId}");
        }

        var slot1Choices = GenerateTurnStartChoices(slot1Pokemon).Cast<SlotChoice>().ToList();
        var slot2Choices = GenerateTurnStartChoices(slot2Pokemon).Cast<SlotChoice>().ToList();

        List<BattleChoice> choices = [];

        foreach (SlotChoice slot1Choice in slot1Choices)
        {
            foreach (SlotChoice slot2Choice in slot2Choices)
            {
                try
                {
                    var doubleChoice = new DoubleSlotChoice(slot1Choice, slot2Choice);
                    choices.Add(doubleChoice);
                }
                catch (ArgumentException)
                {
                    // DoubleSlotChoice validation failed, skip this combination
                }
            }
        }
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

        if (pokemon.IsFainted)
            throw new InvalidOperationException("Cannot get move choices for a fainted Pokémon.");

        var aliveDefenders = defendingSide.AliveActivePokemon.ToArray();
        if (aliveDefenders.Length == 0)
            throw new InvalidOperationException("No alive opposing Pokémon to target with moves.");

        Pokemon? ally = attackingSide.GetAliveAlly(pokemon.SlotId);

        foreach (Move move in pokemon.Moves)
        {
            if (move is not { Pp: > 0, Disabled: false }) continue;

            var possibleTargets = GetPossibleTargets(move, pokemon, ally, aliveDefenders);

            if (move.Target == MoveTarget.Normal)
            {
                // Generate choices for each specific target
                var specificTargets = new List<Pokemon>();

                if (ally is not null)
                {
                    specificTargets.Add(ally);
                }
                specificTargets.AddRange(aliveDefenders);

                foreach (Pokemon target in specificTargets)
                {
                    MoveNormalTarget targetType = MoveEnumTools.CalculateMoveNormalTarget(pokemon, move, 
                        target);

                    Pokemon[] specificTargetArray = [target];

                    AddMoveChoices(choices, pokemon, move, targetType, specificTargetArray,
                        attackingSide.AnyTeraUsed);
                }
            }
            else
            {
                // All other target types don't require specific target selection
                AddMoveChoices(choices, pokemon, move, MoveNormalTarget.None, possibleTargets,
                    attackingSide.AnyTeraUsed);
            }
        }

        if (choices.Count != 0) return choices.ToArray();

        // Struggle fallback
        Pokemon struggleTarget = GetStruggleTarget(aliveDefenders);
        Move struggle = Library.Moves[MoveId.Struggle];
        MoveNormalTarget struggleTargetType = MoveEnumTools.CalculateMoveNormalTarget(pokemon, struggle,
            struggleTarget);

        SlotChoice.MoveChoice struggleChoice = new(pokemon, struggle, false, struggleTargetType,
            [struggleTarget]);
        choices.Add(struggleChoice);

        return choices.ToArray();
    }

    /// <summary>
    /// Helper method to add both normal and Tera move choices
    /// </summary>
    private void AddMoveChoices(List<BattleChoice> choices, Pokemon attacker, Move move,
        MoveNormalTarget targetType, Pokemon[] possibleTargets, bool teraAlreadyUsed)
    {
        // Add normal move choice
        choices.Add(new SlotChoice.MoveChoice(attacker, move, false, targetType, possibleTargets));

        // Add Tera variant if not already used
        if (!teraAlreadyUsed)
        {
            choices.Add(new SlotChoice.MoveChoice(attacker, move, true, targetType, possibleTargets));
        }
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
        return targets.Length switch
        {
            1 => targets[0],
            2 => targets[BattleRandom.Next(2)], // Randomly select one of the two targets
            _ => throw new InvalidOperationException("No valid targets available for Struggle."),
        };
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
            new SlotChoice.SwitchChoice(pokemon, inPokemon, Format)));
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

    private Pokemon? ForceSwitcher { get; set; }

    private GameplayExecutionStage ExecutionStage { get; set; }

    public BattleChoice[] GenerateChoicesForMcts(PlayerId playerId)
    {
        Side side = GetSide(playerId);

        if (Turns.Count == 0)
        {
            throw new InvalidOperationException("Battle has not been started. The Turns list is empty, which indicates that Battle.Start() was not called before generating choices for MCTS.");
        }
        
        switch (CurrentTurn)
        {
            case TeamPreviewTurn:
                return GetTeamPreviewChoices(side);
            case GameplayTurn:
                switch (ExecutionStage)
                {
                    case GameplayExecutionStage.TurnStart:
                        // Use direct synchronous choice generation instead of async player requests
                        return GenerateTurnStartChoicesForMcts(playerId);

                    case GameplayExecutionStage.ForceSwitch:
                        return GenerateForcedSwitchChoices(ForceSwitcher ?? 
                                                           throw new InvalidOperationException());

                    case GameplayExecutionStage.FaintedSwitch:
                        return GenerateFaintedSwitchChoicesForPlayer(playerId);

                    default:
                        throw new InvalidOperationException("Invalid gameplay execution type.");
                }
            case PostGameTurn:
                throw new InvalidOperationException("No choices are available when the game is over.");
            default:
                throw new InvalidOperationException("Invalid turn type");
        }
    }

    /// <summary>
    /// Generate turn start choices for MCTS without going through async player request system
    /// This prevents infinite recursion when MCTS is generating choices
    /// </summary>
    private BattleChoice[] GenerateTurnStartChoicesForMcts(PlayerId playerId)
    {
        Side side = GetCurrentSide(playerId);

        if (!side.IsTurnStartSideValid)
        {
            throw new InvalidOperationException($"Side {playerId} is not in a valid state to start the turn.");
        }

        switch (Format)
        {
            case BattleFormat.Singles:
                // Singles: only one active Pokémon
                Pokemon activePokemon = side.ActivePokemon.First();
                
                // If the active Pokémon is fainted, we should be in FaintedSwitch stage instead
                if (activePokemon.IsFainted)
                {
                    throw new InvalidOperationException($"Cannot generate turn start choices for fainted Pokémon {activePokemon.Name}. Should be in FaintedSwitch stage.");
                }
                
                return GenerateTurnStartChoices(activePokemon);

            case BattleFormat.Doubles:
                // Doubles: two active Pokémon - check for fainted Pokémon
                Pokemon slot1Pokemon = side.GetSlot(SlotId.Slot1);
                Pokemon slot2Pokemon = side.GetSlot(SlotId.Slot2);
                
                // Count alive active Pokémon
                var alivePokemon = new List<Pokemon>();
                if (!slot1Pokemon.IsFainted) alivePokemon.Add(slot1Pokemon);
                if (!slot2Pokemon.IsFainted) alivePokemon.Add(slot2Pokemon);
                
                switch (alivePokemon.Count)
                {
                    case 0:
                        // Both Pokémon are fainted - should be in FaintedSwitch stage
                        throw new InvalidOperationException("Cannot generate turn start choices when both active Pokémon are fainted. Should be in FaintedSwitch stage.");
                    
                    case 1:
                        // Only one Pokémon alive - generate single choices
                        return GenerateTurnStartChoices(alivePokemon[0]);
                        
                    case 2:
                        // Both Pokémon alive - generate double choices
                        return GenerateDoublesTurnStartChoices(slot1Pokemon, slot2Pokemon);
                        
                    default:
                        throw new InvalidOperationException($"Unexpected number of alive active Pokémon: {alivePokemon.Count}");
                }

            default:
                throw new InvalidOperationException($"Unsupported battle format: {Format}");
        }
    }
}