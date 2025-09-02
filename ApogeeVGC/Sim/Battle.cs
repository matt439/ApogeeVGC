using ApogeeVGC.Data;
using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

//public enum BattleState
//{
//    //TeamPreviewNoLock, // No player has locked in their team yet
//    //TeamPreviewPlayer1L
//    //TeamPreviewWaitingPlayer2,
//    //Player1PokemonSelect,
//    //Player2PokemonSelect,
//    //BothPlayersPokemonSelect,
//    //Player1MoveSelect,
//    //Player2MoveSelect,
//    //BothPlayersMoveSelect,

//    TeamPreviewSelect,
//    Playing,
//}

public enum BattleRequestState
{
    RequestingPlayer1Input,
    RequestingPlayer2Input,
    RequestingBothPlayersInput,
    Player1Win,
    Player2Win,
}

public enum PlayerState
{
    TeamPreviewSelect,
    TeamPreviewLocked,
    MoveSwitchSelect,
    MoveSwitchLocked,
    FaintedSelect,
    FaintedLocked,
    ForceSwitchSelect,
    ForceSwitchLocked,
    //TagetSelect,
    //TargetLocked,
    // TODO: Implement these above states
    Idle,
}

public enum MoveAction
{
    None,
    SwitchAttackerOut,
}


public record BattleContext
{
    public required Library Library { get; init; }
    public required Random Random { get; init; }
    public bool PrintDebug { get; init; }
}

public class Battle
{
    public required Library Library { get; init; }
    public required Field Field { get; init; }
    public required Side Side1 { get; init; }
    public required Side Side2 { get; init; }
    public int Turn { get; private set; } = -1; // Start at -1 for team preview turn
    public bool PrintDebug { get; set; }
    public int? BattleSeed { get; init; }
    public bool IsTeamPreview => Player1State == PlayerState.TeamPreviewSelect ||
                                 Player2State == PlayerState.TeamPreviewSelect ||
                                 Player1State == PlayerState.TeamPreviewLocked ||
                                 Player2State == PlayerState.TeamPreviewLocked;
    private BattleContext Context => new()
    {
        Library = Library,
        Random = BattleRandom,
        PrintDebug = PrintDebug,
    };
    private Pokemon[] AllActivePokemon => [Side1.Team.ActivePokemon, Side2.Team.ActivePokemon];
    private PlayerState Player1State { get; set; } = PlayerState.TeamPreviewSelect;
    private PlayerState Player2State { get; set; } = PlayerState.TeamPreviewSelect;
    private Choice? Player1PendingChoice { get; set; }
    private Choice? Player2PendingChoice { get; set; }
    private object ChoiceLock { get; } = new();

    private const int TurnLimit = 1000;
    private const double Epsilon = 1e-10;

    // Lazy-initialized seeded random number generator for deterministic battle simulation
    private Random? _battleRandom;
    private Random BattleRandom => _battleRandom ??= BattleSeed.HasValue ?
        new Random(BattleSeed.Value) : new Random();

    /// <summary>
    /// Creates a deep copy of the battle state for MCTS simulation purposes.
    /// This method creates independent copies of all mutable state while sharing immutable references.
    /// </summary>
    /// <returns>A new Battle instance with copied state</returns>
    public Battle DeepCopy(bool? printDebug = null)
    {
        return new Battle
        {
            // Shared immutable references
            Library = Library, // Library is read-only, safe to share
            
            // Deep copy mutable components using their Copy methods
            Field = Field.Copy(),
            Side1 = Side1.Copy(),
            Side2 = Side2.Copy(),
            
            // Copy simple state
            Turn = Turn,
            Player1State = Player1State,
            Player2State = Player2State,
            Player1PendingChoice = Player1PendingChoice,
            Player2PendingChoice = Player2PendingChoice,
            PrintDebug = printDebug ?? PrintDebug,
            BattleSeed = BattleSeed,
            // Note: ChoiceLock gets a new instance automatically
            // Note: _battleRandom will be initialized with the same seed when first accessed
        };
    }

    /// <summary>
    /// Creates a deep copy of the battle and applies a choice to it.
    /// This is the main method used by MCTS for creating child nodes.
    /// </summary>
    /// <param name="playerId">The player making the choice</param>
    /// <param name="choice">The choice to apply</param>
    /// <param name="printDebug">Manyually set debug printing</param>
    /// <returns>A new Battle instance with the choice applied</returns>
    public Battle DeepCopyAndApplyChoice(PlayerId playerId, Choice choice, bool? printDebug = null)
    {
        Battle copy = DeepCopy(printDebug);
        
        try
        {
            // Apply the choice to the copied battle
            copy.SubmitChoice(playerId, choice);
            return copy;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to apply choice {choice} for player {playerId}: {ex.Message}", ex);
        }
    }

    public void SubmitChoice(PlayerId playerId, Choice choice)
    {
        lock (ChoiceLock)
        {
            if (Turn > TurnLimit)
            {
                throw new InvalidOperationException($"Battle exceeded maximum turn limit." +
                                                    $"Current states: P1={Player1State}, P2={Player2State}");
            }

            CheckForChoiceError(playerId, choice);

            SetPendingChoice(playerId, choice);

            if (IsReadyForTeamPreviewProcessing())
            {
                PerformTeamPreviewSelect(Player1PendingChoice ??
                    throw new ArgumentException("Player1PendingChoice cannot be null"),
                    Player2PendingChoice ??
                        throw new ArgumentException("Player2PendingChoice cannot be null"));
                ClearPendingChoices();

                //// Don't increment turn after team preview processing
                //incrementTurn = false;
            }
            else if (IsReadyForMoveSwitchProcessing())
            {
                var executedPlayers = PerformMoveSwitches(Player1PendingChoice ??
                    throw new ArgumentException("Player1PendingChoice cannot be null"),
                        Player2PendingChoice ??
                            throw new ArgumentException("Player2PendingChoice cannot be null"));

                switch (executedPlayers.Count)
                {
                    case 0:
                        throw new InvalidOperationException("No players executed their move/switch.");
                    case > 2:
                        throw new InvalidOperationException("More than two players executed their move/switch.");
                    case 2: // Both players executed their choices. Clear both choices
                        ClearPendingChoices();
                        break;
                    case 1: // Only one player executed their choice.
                            // Clear that player's choice and set them to force switch select
                        CLearPendingChoice(executedPlayers[0]);
                        SetPlayerState(executedPlayers[0], PlayerState.ForceSwitchSelect);
                        Side side = GetSide(executedPlayers[0]);
                        if (PrintDebug)
                        {
                            UiGenerator.PrintForceSwitchOutAction(side.Team.Trainer.Name, side.Team.ActivePokemon);
                        }
                        break;
                }
            }
            else if (IsReadyForFaintedSelectProcessing())
            {
                // Process switch choices
                if (Player1PendingChoice != null)
                {
                    PerformSwitch(PlayerId.Player1, Player1PendingChoice.Value);
                }
                if (Player2PendingChoice != null)
                {
                    PerformSwitch(PlayerId.Player2, Player2PendingChoice.Value);
                }
                ClearPendingChoices();
            }
            else if (IsReadyForForceSwitchProcessing())
            {
                // Process forced switch choices
                if (Player1PendingChoice != null && Player1PendingChoice.Value.IsSwitchChoice())
                {
                    PerformSwitch(PlayerId.Player1, Player1PendingChoice.Value);
                    CLearPendingChoice(PlayerId.Player1);

                    // If the other player's state is not idle, they still need to execute their move/struggle
                    if (Player2State == PlayerState.MoveSwitchLocked && Player2PendingChoice != null)
                    {
                        ExecutePlayerChoice(PlayerId.Player2, Player2PendingChoice.Value);
                        CLearPendingChoice(PlayerId.Player2);
                        SetPlayerState(PlayerId.Player2, PlayerState.Idle);
                        // Update player states for any fainted Pokemon
                        UpdateFaintedStates();
                    }
                }
                else if (Player2PendingChoice != null && Player2PendingChoice.Value.IsSwitchChoice())
                {
                    PerformSwitch(PlayerId.Player2, Player2PendingChoice.Value);
                    CLearPendingChoice(PlayerId.Player2);

                    // If the other player's state is not idle, they still need to execute their move/struggle
                    if (Player1State == PlayerState.MoveSwitchLocked && Player1PendingChoice != null)
                    {
                        ExecutePlayerChoice(PlayerId.Player1, Player1PendingChoice.Value);
                        CLearPendingChoice(PlayerId.Player1);
                        SetPlayerState(PlayerId.Player1, PlayerState.Idle);
                        // Update player states for any fainted Pokemon
                        UpdateFaintedStates();
                    }
                }
            }

            // Check if the battle has been won
            if (IsWinner() != PlayerId.None) return;
            if (!IsEndOfTurn()) return;

            if (Turn > -1) // Skip end of turn processing for team preview turn
            {
                HandleEndOfTurn();
            }
            else // Team preview turn just ended. Trigger ability/item on switch in effects
            {
                HandleEndOfTeamPreviewTurn();
            }

            Turn++;

            HandleStartOfTurn();

            // Reset player states for the next turn
            Player1State = PlayerState.MoveSwitchSelect;
            Player2State = PlayerState.MoveSwitchSelect;
        }
    }

    public BattleRequestState GetRequestState()
    {
        lock (ChoiceLock)
        {
            if (IsWinner() != PlayerId.None)
            {
                return IsWinner() == PlayerId.Player1 ?
                    BattleRequestState.Player1Win :
                    BattleRequestState.Player2Win;
            }

            if (Player1State == PlayerState.ForceSwitchSelect &&
                Player2State == PlayerState.ForceSwitchSelect)
            {
                throw new InvalidOperationException("Invalid battle state: both players are in ForceSwitchSelect state.");
            }
            if (Player1State == PlayerState.ForceSwitchSelect)
            {
                return BattleRequestState.RequestingPlayer1Input;
            }
            if (Player2State == PlayerState.ForceSwitchSelect)
            {
                return BattleRequestState.RequestingPlayer2Input;
            }

            bool requestingPlayer1Input = Player1State.CanSubmitChoice();
            bool requestingPlayer2Input = Player2State.CanSubmitChoice();

            if (requestingPlayer1Input && requestingPlayer2Input)
            {
                return BattleRequestState.RequestingBothPlayersInput;
            }
            if (requestingPlayer1Input)
            {
                return BattleRequestState.RequestingPlayer1Input;
            }
            if (requestingPlayer2Input)
            {
                return BattleRequestState.RequestingPlayer2Input;
            }
            throw new InvalidOperationException("Invalid battle state: both players are not requesting input.");
        }
    }

    public Choice[] GetAvailableChoices(PlayerId playerId)
    {
        lock (ChoiceLock)
        {
            if (!CanPlayerSubmitChoice(playerId))
            {
                throw new InvalidOperationException($"Player {playerId} cannot submit choice in current state");
            }

            return playerId switch
            {
                PlayerId.Player1 => GetAvailableChoices(Side1),
                PlayerId.Player2 => GetAvailableChoices(Side2),
                PlayerId.None => throw new ArgumentException("PlayerId cannot be 'None'"),
                _ => throw new ArgumentException("Invalid player ID", nameof(playerId)),
            };
        }
    }

    public Side GetSide(PlayerId playerId)
    {
        lock (ChoiceLock)
        {
            return playerId switch
            {
                PlayerId.Player1 => Side1,
                PlayerId.Player2 => Side2,
                PlayerId.None => throw new ArgumentException("PlayerId cannot be 'None'", nameof(playerId)),
                _ => throw new ArgumentException("Invalid player ID", nameof(playerId))
            };
        }
    }

    private void HandleEndOfTurn()
    {
        if (PrintDebug)
        {
            UiGenerator.PrintBlankLine();
        }
        Field.OnTurnEnd(Side1, Side2, Context);
        HandleBeforeResiduals();
        HandleResiduals();
        HandleConditionTurnEnds();
    }

    private void HandleStartOfTurn()
    {
        Field.OnTurnStart(Side1, Side2, Context);
        HandleItemTurnStarts();
        HandleConditionTurnStarts();
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

    private void HandleItemTurnStarts()
    {
        // check all active pokemon items with OnStart
        foreach (Pokemon pokemon in AllActivePokemon)
        {
            if (pokemon.IsFainted) continue; // Skip if fainted
            pokemon.Item?.OnStart?.Invoke(pokemon, Context);
        }
    }

    private void HandleConditionTurnStarts()
    {

    }

    private void HandleConditionTurnEnds()
    {
        List<Condition> side1Conditions = [];
        if (!Side1.Team.ActivePokemon.IsFainted) // Skip if fainted
        {
            side1Conditions = Side1.Team.ActivePokemon.Conditions.ToList();
        }
        foreach (Condition condition in side1Conditions.ToList())
        {
            condition.OnTurnEnd?.Invoke(Side1.Team.ActivePokemon, Context);
            if (!condition.Duration.HasValue) continue;
            condition.Duration--;
            if (condition.Duration <= 0)
            {
                Side1.Team.ActivePokemon.RemoveCondition(condition.Id);
            }
        }

        List<Condition> side2Conditions = [];
        if (!Side2.Team.ActivePokemon.IsFainted) // Skip if fainted
        {
            side2Conditions = Side2.Team.ActivePokemon.Conditions.ToList();
        }
        foreach (Condition condition in side2Conditions.ToList())
        {
            condition.OnTurnEnd?.Invoke(Side2.Team.ActivePokemon, Context);
            if (!condition.Duration.HasValue) continue;
            condition.Duration--;
            if (condition.Duration <= 0)
            {
                Side2.Team.ActivePokemon.RemoveCondition(condition.Id);
            }
        }
    }

    private void HandleResiduals()
    {
        Condition[] side1Residuals = [];
        if (!Side1.Team.ActivePokemon.IsFainted) // Skip if fainted
        {
            side1Residuals = Side1.Team.ActivePokemon.GetAllResidualConditions();
        }
        List<(Pokemon, Condition, PlayerId)> side1ResidualsList = [];
        foreach (Condition condition in side1Residuals)
        {
            if (condition.OnResidual != null)
            {
                side1ResidualsList.Add((Side1.Team.ActivePokemon, condition, PlayerId.Player1));
            }
        }

        Condition[] side2Residuals = [];
        if (!Side2.Team.ActivePokemon.IsFainted) // Skip if fainted
        {
            side2Residuals = Side2.Team.ActivePokemon.GetAllResidualConditions();
        }
        List<(Pokemon, Condition, PlayerId)> side2ResidualsList = [];
        foreach (Condition condition in side2Residuals)
        {
            if (condition.OnResidual != null)
            {
                side2ResidualsList.Add((Side2.Team.ActivePokemon, condition, PlayerId.Player2));
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

    private void SetPendingChoice(PlayerId playerId, Choice? choice)
    {
        switch (playerId)
        {
            case PlayerId.Player1:
                Player1PendingChoice = choice;
                break;
            case PlayerId.Player2:
                Player2PendingChoice = choice;
                break;
            case PlayerId.None:
                throw new ArgumentException("PlayerId cannot be 'None'", nameof(playerId));
            default:
                throw new ArgumentException("Invalid player ID", nameof(playerId));
        }

        UpdatePlayerState(playerId, choice);
    }

    private void UpdatePlayerState(PlayerId playerId, Choice? choice)
    {
        if (choice is null) return; // No choice to update state with

        PlayerState playerState = GetPlayerState(playerId);

        switch (playerState)
        {
            case PlayerState.MoveSwitchSelect:
                if (choice.Value.IsSwitchChoice() || choice.Value.IsMoveChoice() ||
                    choice.Value.IsMoveWithTeraChoice() || choice == Choice.Struggle)
                {
                    SetPlayerState(playerId, PlayerState.MoveSwitchLocked);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Invalid choice for MoveSwitchSelect: {choice}");
                }
                break;
            case PlayerState.FaintedSelect:
                if (choice.Value.IsSwitchChoice())
                {
                    SetPlayerState(playerId, PlayerState.FaintedLocked);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Invalid choice for FaintedSelect: {choice}");
                }
                break;
            case PlayerState.ForceSwitchSelect:
                if (choice.Value.IsSwitchChoice())
                {
                    SetPlayerState(playerId, PlayerState.ForceSwitchLocked);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Invalid choice for ForceSwitchSelect: {choice}");
                }
                break;
            case PlayerState.TeamPreviewSelect:
                if (choice.Value.IsSelectChoice())
                {
                    SetPlayerState(playerId, PlayerState.TeamPreviewLocked);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Invalid choice for TeamPreviewSelect: {choice}");
                }
                break;
            case PlayerState.TeamPreviewLocked:
            case PlayerState.MoveSwitchLocked:
            case PlayerState.FaintedLocked:
            case PlayerState.ForceSwitchLocked:
                throw new InvalidOperationException(
                    $"Player {playerId} is already locked in state: {playerState}");
            case PlayerState.Idle:
                throw new InvalidOperationException("Player cannot submit choice in Idle state.");
            default:
                throw new ArgumentOutOfRangeException(nameof(playerState), playerState, null);
        }
    }

    /// <returns>A list of player IDs of players who have executed their choice.</returns>
    private List<PlayerId> PerformMoveSwitches(Choice player1Choice, Choice player2Choice)
    {
        // Check if either player is terastilizing
        bool player1Tera = player1Choice.IsMoveWithTeraChoice();
        bool player2Tera = player2Choice.IsMoveWithTeraChoice();
        if (player1Tera && player2Tera)
        {
            // Both players are terastilizing. Determine order by speed.
            PlayerId fasterPlayer = CalculateSpeedOrder(Side1.Team.ActivePokemon, Side2.Team.ActivePokemon);
            if (fasterPlayer == PlayerId.Player1)
            {
                Side1.Team.ActivePokemon.Terastillize(Context);
                Side2.Team.ActivePokemon.Terastillize(Context);
            }
            else
            {
                Side2.Team.ActivePokemon.Terastillize(Context);
                Side1.Team.ActivePokemon.Terastillize(Context);
            }

            player1Choice = player1Choice.ConvertMoveWithTeraToMove();
            Player1PendingChoice = player1Choice;
            player2Choice = player2Choice.ConvertMoveWithTeraToMove();
            Player2PendingChoice = player2Choice;
        }
        else if (player1Tera)
        {
            Side1.Team.ActivePokemon.Terastillize(Context);
            player1Choice = player1Choice.ConvertMoveWithTeraToMove();
            Player1PendingChoice = player1Choice;
        }
        else if (player2Tera)
        {
            Side2.Team.ActivePokemon.Terastillize(Context);
            player2Choice = player2Choice.ConvertMoveWithTeraToMove();
            Player2PendingChoice = player2Choice;
        }

        PlayerId nextPlayer = MovesNext(player1Choice, player2Choice);

        // Create execution order based on priority
        var executionOrder = nextPlayer == PlayerId.Player1
            ? new[] { (PlayerId.Player1, player1Choice), (PlayerId.Player2, player2Choice) }
            : new[] { (PlayerId.Player2, player2Choice), (PlayerId.Player1, player1Choice) };

        List<PlayerId> executedPlayers = [];

        // Execute choices in priority order
        foreach ((PlayerId playerId, Choice choice) in executionOrder)
        {
            switch (ExecutePlayerChoice(playerId, choice))
            {
                case MoveAction.None:
                    executedPlayers.Add(playerId);
                    break;
                case MoveAction.SwitchAttackerOut:
                    executedPlayers.Add(playerId);
                    // In case the force switch was triggered before the opponent's move,
                    // don't execute the opponent's move now. The switch will be handled
                    // first, then the opponent's move will be handled after.
                    return executedPlayers;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return executedPlayers;
    }

    private MoveAction ExecutePlayerChoice(PlayerId playerId, Choice choice)
    {
        var action = MoveAction.None;
        PlayerState playerState = GetPlayerState(playerId);

        switch (playerState)
        {
            // This occurs when a player's pokemon faints and they need to select a new one
            // Their previous choice isn't valid anymore as it was based on the now-fainted pokemon
            case PlayerState.FaintedSelect:
            // This occurs when a player's pokemon needs to force switch and they need to select a new one
            // Their previous choice isn't valid anymore as it was based on the now-switching pokemon
            case PlayerState.ForceSwitchSelect:
                return action;
            case PlayerState.TeamPreviewLocked:
            case PlayerState.MoveSwitchLocked:
            case PlayerState.FaintedLocked:
            case PlayerState.ForceSwitchLocked:
            case PlayerState.Idle:
                break;
            case PlayerState.TeamPreviewSelect:
            case PlayerState.MoveSwitchSelect:
                throw new InvalidOperationException("Player cannot submit choice in non-locked state.");
            default:
                throw new ArgumentOutOfRangeException(nameof(playerState), playerState, null);
        }

        if (choice.IsMoveChoice())
        {
            MoveAction moveAction = PerformMove(playerId, choice);
            SetPendingChoice(playerId, null);

            // Only set to force switch select if there are valid switch options
            if (moveAction == MoveAction.SwitchAttackerOut &&
                GetSide(playerId).Team.SwitchOptionsCount > 0)
            {
                SetPlayerState(playerId, PlayerState.ForceSwitchSelect);
                action = MoveAction.SwitchAttackerOut;
            }
            else
            {
                SetPlayerState(playerId, PlayerState.Idle);
            }

            // Update player states for any fainted Pokemon
            UpdateFaintedStates();
        }
        else if (choice.IsSwitchChoice())
        {
            PerformSwitch(playerId, choice);
            SetPendingChoice(playerId, null);
            SetPlayerState(playerId, PlayerState.Idle);
        }
        else if (choice == Choice.Struggle)
        {
            PerformStruggle(playerId);
            SetPendingChoice(playerId, null);
            SetPlayerState(playerId, PlayerState.Idle);

            // Update player states for any fainted Pokemon
            UpdateFaintedStates();
        }
        else
        {
            throw new InvalidOperationException($"Invalid choice for {playerId}: {choice}");
        }

        return action;
    }

    private void UpdateFaintedStates()
    {
        if (Side1.Team.ActivePokemon.IsFainted && Player1State != PlayerState.FaintedSelect &&
            Player1State != PlayerState.ForceSwitchSelect)
        {
            Player1State = PlayerState.FaintedSelect;
            if (PrintDebug)
            {
                UiGenerator.PrintFaintedAction(Side1.Team.ActivePokemon);
            }
        }
        if (Side2.Team.ActivePokemon.IsFainted && Player2State != PlayerState.FaintedSelect &&
            Player2State != PlayerState.ForceSwitchSelect)
        {
            Player2State = PlayerState.FaintedSelect;
            if (PrintDebug)
            {
                UiGenerator.PrintFaintedAction(Side2.Team.ActivePokemon);
            }
        }
    }

    private void ClearPendingChoices()
    {
        Player1PendingChoice = null;
        Player2PendingChoice = null;
    }

    private void CLearPendingChoice(PlayerId playerId)
    {
        switch (playerId)
        {
            case PlayerId.Player1:
                Player1PendingChoice = null;
                break;
            case PlayerId.Player2:
                Player2PendingChoice = null;
                break;
            case PlayerId.None:
                throw new ArgumentException("PlayerId cannot be 'None'", nameof(playerId));
            default:
                throw new ArgumentException("Invalid player ID", nameof(playerId));
        }
    }

    private bool IsReadyForTeamPreviewProcessing()
    {
        return Player1State == PlayerState.TeamPreviewLocked &&
               Player2State == PlayerState.TeamPreviewLocked;
    }

    private bool IsReadyForMoveSwitchProcessing()
    {
        return Player1State == PlayerState.MoveSwitchLocked &&
               Player2State == PlayerState.MoveSwitchLocked;
    }

    private bool IsReadyForFaintedSelectProcessing()
    {
        // 1 or 2 players can be locked in switch state, but not both
        return Player1State == PlayerState.FaintedLocked && Player2State == PlayerState.FaintedLocked ||
               Player1State == PlayerState.FaintedLocked && Player2State == PlayerState.Idle ||
               Player1State == PlayerState.Idle && Player2State == PlayerState.FaintedLocked;
    }

    private bool IsReadyForForceSwitchProcessing()
    {
        if (Player1State == PlayerState.ForceSwitchLocked && Player2State == PlayerState.ForceSwitchLocked)
        {
            throw new InvalidOperationException("Invalid battle state: both players are in ForceSwitchLocked state.");
        }

        return Player1State == PlayerState.ForceSwitchLocked || Player2State == PlayerState.ForceSwitchLocked;
    }

    private bool IsEndOfTurn()
    {
        //if (IsWinner() != PlayerId.None)
        //{
        //    return true; // Battle has ended, no more turns
        //}
        lock (ChoiceLock)
        {
            return !Player1State.CanSubmitChoice() && !Player2State.CanSubmitChoice();
        }
    }

    private bool CanPlayerSubmitChoice(PlayerId playerId)
    {
        if (IsWinner() != PlayerId.None) return false;
        lock (ChoiceLock)
        {
            return playerId switch
            {
                PlayerId.Player1 => Player1State.CanSubmitChoice(),
                PlayerId.Player2 => Player2State.CanSubmitChoice(),
                _ => false,
            };
        }
    }

    private MoveAction PerformMove(PlayerId playerId, Choice choice)
    {
        Side atkSide = GetSide(playerId);
        Side defSide = GetSide(playerId.OpposingPlayerId());

        int moveIndex = choice.GetMoveIndexFromChoice();
        Pokemon attacker = atkSide.Team.ActivePokemon;
        Pokemon defender = defSide.Team.ActivePokemon;
        Move move = attacker.Moves[moveIndex];

        // This is where choice lock and choice benefits come into play
        attacker.Item?.OnModifyMove?.Invoke(move, attacker, defender, Context);

        if (move.Pp <= 0)
        {
            throw new InvalidOperationException($"Move {move.Name} has no PP left" +
                                                $"for player {playerId}");
        }

        // Check OnDisableMove conditions on attacker
        foreach (Condition condition in attacker.Conditions.ToList())
        {
            condition.OnDisableMove?.Invoke(attacker, move, Context);
        }
        if (move.Disabled)
        {
            if (PrintDebug)
            {
                UiGenerator.PrintDisabledMoveTry(attacker, move);
            }
            return MoveAction.None;
        }

        move.UsedPp++;  // Decrease PP for the move used
        attacker.ActiveMoveActions++; // Increment the count of moves used this battle (for fake out, etc.)
        attacker.LastMoveUsed = move;
        

        // Check for conditions with OnBeforeMove on attacker (e.g. flinch, paralysis)
        if (attacker.Conditions
            .Where(c => c.OnBeforeMove != null)
            .OrderBy(c => c.OnBeforeMovePriority ?? 0)
            .ToList().Any(condition => condition.OnBeforeMove == null ||
                                       !condition.OnBeforeMove(attacker, defender, move, Context)))
        {
            return MoveAction.None;
        }

        if (move.StallingMove)
        {
            // check for conditions with OnStallMove on attacker
            foreach (Condition condition in attacker.Conditions.ToList())
            {
                if (condition.OnStallMove == null || condition.OnStallMove(attacker, Context)) continue;
                if (PrintDebug)
                {
                    UiGenerator.PrintMoveFailAction(attacker, move);
                }

                return MoveAction.None;
            }
        }

        // OnTry checks on the attacker's move (incl fake out, etc)
        if (move.OnTry?.Invoke(attacker, defender, move, Context) == false)
        {
            return MoveAction.None;
        }

        // Miss check
        if (IsMoveMiss(attacker, move, defender))
        {
            if (PrintDebug)
            {
                UiGenerator.PrintMoveMissAction(attacker, move, defender);
            }
            return MoveAction.None;
        }

        // Immunity check. Note that this does not check for normal immunity, only special cases.
        // Regular immunity check is done in PerformDamagingMove
        if (move.OnTryImmunity != null && move.OnTryImmunity(defender) ||
            move.OnPrepareHit?.Invoke(defender, attacker, move, Context) == false)
        {
            if (PrintDebug)
            {
                UiGenerator.PrintMoveNoEffectAction(attacker, move, defender);
            }
            return MoveAction.None;
        }

        // check every condition on defender for OnTryHit effects
        foreach (Condition condition in defender.Conditions.ToList())
        {
            if (condition.OnTryHit == null || condition.OnTryHit(defender, attacker, move, Context)) continue;

            if (!PrintDebug) return MoveAction.None;

            // Check if the defender is protected by a stall move like Protect, Detect, etc.
            if (defender.HasCondition(ConditionId.Stall))
            {
                UiGenerator.PrintStallMoveProtection(attacker, move, defender);
            }
            else
            {
                UiGenerator.PrintMoveNoEffectAction(attacker, move, defender);
            }
            return MoveAction.None;
        }

        return move.Category switch
        {
            MoveCategory.Physical or MoveCategory.Special => PerformDamagingMove(attacker, playerId, move, defender),
            MoveCategory.Status => PerformStatusMove(attacker, playerId, move, defender),
            _ => throw new InvalidOperationException($"Invalid move category for move {move.Name}: {move.Category}"),
        };
    }

    private MoveAction PerformDamagingMove(Pokemon attacker, PlayerId attackingPlayer, Move move, Pokemon defender)
    {
        switch (move.Target)
        {
            case MoveTarget.Normal:
            case MoveTarget.AllAdjacentFoes:
                break;
            case MoveTarget.Self:
            case MoveTarget.AdjacentAlly:
            case MoveTarget.AdjacentAllyOrSelf:
            case MoveTarget.AdjacentFoe:
            case MoveTarget.All:
            case MoveTarget.AllAdjacent:
            case MoveTarget.Allies:
            case MoveTarget.AllySide:
            case MoveTarget.AllyTeam:
            case MoveTarget.Any:
            case MoveTarget.FoeSide:
            case MoveTarget.RandomNormal:
            case MoveTarget.Scripted:
            case MoveTarget.None:
                throw new NotImplementedException();
            case MoveTarget.Field:
                throw new InvalidOperationException("Field target should be handled separately");
            default:
                throw new ArgumentOutOfRangeException();
        }

        bool isCrit = BattleRandom.NextDouble() < 1.0 / 16.0; // 1 in 16 chance of critical hit
        MoveEffectiveness effectiveness = Library.TypeChart.GetMoveEffectiveness(
            defender.DefensiveTypes, move.Type);

        if (effectiveness == MoveEffectiveness.Immune)
        {
            if (PrintDebug)
            {
                UiGenerator.PrintMoveImmuneAction(attacker, move, defender);
            }
            return MoveAction.None;
        }

        int damage = CalculateDamage(attacker, defender, move, effectiveness.GetMultiplier(), isCrit);

        // check for OnAnyModifyDamage conditions on defender
        if (damage > 0)
        {
            int numPokemonDefendingSide = GetSide(attackingPlayer.OpposingPlayerId()).Team.AllActivePokemonCount;

            double multiplier = defender.Conditions.Aggregate(1.0, (current, condition) =>
                current * (condition.OnAnyModifyDamage?.Invoke(damage, attacker, defender, move, isCrit,
                    numPokemonDefendingSide) ?? 1.0));
            damage = Math.Max(1, (int)(damage * multiplier)); // Always at least 1 damage
        }

        int actualDefenderDamage = defender.Damage(damage);
        
        if (PrintDebug)
        {
            UiGenerator.PrintDamagingMoveAction(attacker, move, damage, defender, effectiveness, isCrit);
        }

        move.OnHit?.Invoke(defender, attacker, move, Context);

        // Rocky helmet
        defender.Item?.OnDamagingHit?.Invoke(actualDefenderDamage, defender, attacker, move, Context);
        
        foreach (Condition condition in defender.Conditions.ToList())
        {
            condition.OnDamagingHit?.Invoke(actualDefenderDamage, defender, attacker, move, Context);
        }

        // Check for move condition application
        if (move.Condition is not null)
        {
            // Apply condition based on move target
            switch (move.Target)
            {
                case MoveTarget.Normal:
                    defender.AddCondition(move.Condition, Context, attacker, move);
                    break;
                case MoveTarget.Self:
                    attacker.AddCondition(move.Condition, Context, attacker, move);
                    break;
                case MoveTarget.AdjacentAlly:
                case MoveTarget.AdjacentAllyOrSelf:
                case MoveTarget.AdjacentFoe:
                case MoveTarget.All:
                case MoveTarget.AllAdjacent:
                case MoveTarget.AllAdjacentFoes:
                case MoveTarget.Allies:
                case MoveTarget.AllySide:
                case MoveTarget.AllyTeam:
                case MoveTarget.Any:
                case MoveTarget.FoeSide:
                case MoveTarget.RandomNormal:
                case MoveTarget.Scripted:
                case MoveTarget.None:
                    throw new NotImplementedException();
                case MoveTarget.Field:
                    throw new InvalidOperationException("Field target should be handled separately");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // check for recoil
        if (move.Recoil is not null)
        {
            int recoilDamage = (int)(damage * move.Recoil.Value);
            attacker.Damage(recoilDamage);
            if (PrintDebug)
            {
                UiGenerator.PrintRecoilDamageAction(attacker, recoilDamage);
            }
        }

        // check if defender fainted
        if (defender.IsFainted)
        {
            // chilling neigh
            attacker.Ability.OnSourceAfterFaint?.Invoke(1, defender, attacker, move, Context);
        }

        return move.SelfSwitch ? MoveAction.SwitchAttackerOut : MoveAction.None;
    }

    private MoveAction PerformStatusMove(Pokemon attacker, PlayerId playerId, Move move, Pokemon defender)
    {
        if (PrintDebug)
        {
            UiGenerator.PrintStatusMoveAction(attacker, move);
        }

        if (move.Target == MoveTarget.Field)
        {
            HandleFieldTargetStatusMove(move, attacker);
            return MoveAction.None;
        }

        if (move.Target == MoveTarget.AllySide)
        {
            HandleSideTargetStatusMove(move, playerId, attacker);
            return MoveAction.None;
        }
        if (move.Target == MoveTarget.FoeSide)
        {
            HandleSideTargetStatusMove(move, playerId.OpposingPlayerId(), attacker);
            return MoveAction.None;
        }

        if (move.Condition is null)
        {
            return MoveAction.None;
        }

        move.OnHit?.Invoke(defender, attacker, move, Context);

        switch (move.Target)
        {
            case MoveTarget.Normal:
                defender.AddCondition(move.Condition, Context, attacker, move);
                break;
            case MoveTarget.Self:
                attacker.AddCondition(move.Condition, Context, attacker, move);
                break;
            case MoveTarget.AdjacentAlly:
            case MoveTarget.AdjacentAllyOrSelf:
            case MoveTarget.AdjacentFoe:
            case MoveTarget.All:
            case MoveTarget.AllAdjacent:
            case MoveTarget.AllAdjacentFoes:
            case MoveTarget.Allies:
            case MoveTarget.AllySide:
            case MoveTarget.AllyTeam:
            case MoveTarget.Any:
            case MoveTarget.FoeSide:
            case MoveTarget.RandomNormal:
            case MoveTarget.Scripted:
            case MoveTarget.None:
                throw new NotImplementedException();
            case MoveTarget.Field:
                throw new InvalidOperationException("Field target should be handled separately");
            default:
                throw new ArgumentOutOfRangeException();
        }

        return MoveAction.None;
    }

    private void HandleSideTargetStatusMove(Move move, PlayerId playerId, Pokemon attacker)
    {
        if (move.SideCondition is null)
        {
            throw new InvalidOperationException($"Status move {move.Name} has no side effect defined.");
        }

        SideCondition? condition = Field.GetSideCondition(move.SideCondition.Id, playerId);
        if (condition is not null)
        {
            // If the side condition is already present, reapply it (which may remove it)
            Field.ReapplySideCondition(condition.Id, GetSide(playerId), Context);
        }
        else // Otherwise, add the new side condition
        {
            Field.AddSideCondition(move.SideCondition, GetSide(playerId), attacker, move, Context);
        }
    }

    private void HandleFieldTargetStatusMove(Move move, Pokemon attacker)
    {
        if (move.PseudoWeather is null && move.Weather is null && move.Terrain is null)
        {
            throw new InvalidOperationException($"Status move {move.Name} has no field effect defined.");
        }

        if (move.PseudoWeather is not null)
        {
            // If the pseudo-weather is already present, reapply it (which may remove it)
            if (Field.HasPseudoWeather(move.PseudoWeather.Id))
            {
                Field.ReapplyPseudoWeather(move.PseudoWeather.Id, AllActivePokemon, Context);
            }
            else // Otherwise, add the new pseudo-weather
            {
                Field.AddPseudoWeather(move.PseudoWeather, attacker, move, AllActivePokemon, Context);
            }
        }
        if (move.Weather is not null)
        {
            if (Field.HasWeather(move.Weather.Id)) // Reapply weather if it's the same one
            {
                Field.ReapplyWeather(AllActivePokemon, Context);
            }
            else if (Field.HasAnyWeather) // Replace existing weather
            {
                Field.RemoveWeather(AllActivePokemon, Context);
                Field.AddWeather(move.Weather, attacker, move, AllActivePokemon, Context);
            }
            else // No existing weather, just add the new one
            {
                Field.AddWeather(move.Weather, attacker, move, AllActivePokemon, Context);
            }
        }
        if (move.Terrain is not null)
        {
            if (Field.HasTerrain(move.Terrain.Id)) // Reapply terrain if it's the same one
            {
                Field.ReapplyTerrain(AllActivePokemon, Context);
            }
            else if (Field.HasAnyWeather) // Replace existing terrain
            {
                Field.ReapplyTerrain(AllActivePokemon, Context);
                Field.AddTerrain(move.Terrain, attacker, move, AllActivePokemon, Context);
            }
            else // No existing terrain, just add the new one
            {
                Field.AddTerrain(move.Terrain, attacker, move, AllActivePokemon, Context);
            }
        }
    }

    private bool IsMoveMiss(Pokemon attacker, Move move, Pokemon defender)
    {
        if (move.AlwaysHit)
        {
            return false; // Move always hits
        }

        // get move accuracy
        int moveAccuracy = move.Accuracy;

        // get attacker accuracy stage
        double attackerAccuracyStage = attacker.StatModifiers.AccuracyMultiplier;

        // get defender evasion stage
        double defenderEvasionStage = defender.StatModifiers.EvasionMultiplier;

        // Calculate modified accuracy
        double modifiedAccuracy = moveAccuracy * attackerAccuracyStage * defenderEvasionStage;

        // generate random between 1 and 100
        int roll = BattleRandom.Next(1, 101);

        // move hits if roll is less than or equal to modified accuracy
        return !(roll <= modifiedAccuracy);
    }

    private void HandleEndOfTeamPreviewTurn()
    {
        foreach (Pokemon pokemon in AllActivePokemon)
        {
            if (pokemon.IsFainted) continue; // Skip if fainted
            pokemon.OnSwitchIn(Field, AllActivePokemon, Context); // Trigger switch-in effects
        }
    }

    private void PerformSwitch(PlayerId playerId, Choice choice)
    {
        if (IsWinner() != PlayerId.None)
        {
            throw new InvalidOperationException("Cannot switch Pokémon when the battle has already ended.");
        }
        if (!choice.IsSwitchChoice())
        {
            throw new ArgumentException("Choice must be a switch choice to switch Pokémon.", nameof(choice));
        }

        Side side = GetSide(playerId);
        Pokemon prevActive = side.Team.ActivePokemon;
        prevActive.OnSwitchOut();
        side.Team.ActivePokemonIndex = choice.GetSwitchIndexFromChoice();
        Pokemon newActive = side.Team.ActivePokemon;
        Field.OnPokemonSwitchIn(newActive, playerId, Context);
        newActive.OnSwitchIn(Field, AllActivePokemon, Context);

        if (!PrintDebug) return;

        PlayerState playerState = GetPlayerState(playerId);
        switch (playerState)
        {
            case PlayerState.MoveSwitchLocked:
                if (PrintDebug)
                {
                    UiGenerator.PrintSwitchAction(side.Team.Trainer.Name, prevActive,
                        side.Team.ActivePokemon);
                }
                break;
            case PlayerState.FaintedLocked:
                if (PrintDebug)
                {
                    UiGenerator.PrintFaintedSelectAction(side.Team.Trainer.Name,
                        side.Team.ActivePokemon);
                }
                break;
            case PlayerState.ForceSwitchLocked:
                if (PrintDebug)
                {
                    UiGenerator.PrintForceSwitchInAction(side.Team.Trainer.Name, side.Team.ActivePokemon);
                }
                break;
            case PlayerState.Idle:
            case PlayerState.TeamPreviewSelect:
            case PlayerState.TeamPreviewLocked:
            case PlayerState.MoveSwitchSelect:
            case PlayerState.FaintedSelect:
            case PlayerState.ForceSwitchSelect:
                throw new InvalidOperationException($"Player {playerId} cannot switch Pokémon" +
                                                    $"in current state: {playerState}");
            default:
                throw new ArgumentOutOfRangeException(nameof(playerState), playerState, null);
        }
    }

    private void PerformTeamPreviewSelect(Choice player1Choice, Choice player2Choice)
    {
        if (IsWinner() != PlayerId.None)
        {
            throw new InvalidOperationException("Cannot perform team preview select when the battle has already ended.");
        }
        if (!player1Choice.IsSelectChoice() || !player2Choice.IsSelectChoice())
        {
            throw new ArgumentException("Both choices must be select choices for team preview select.");
        }
        Side side1 = GetSide(PlayerId.Player1);
        Side side2 = GetSide(PlayerId.Player2);
        // Set the selected Pokémon for each player
        side1.Team.ActivePokemonIndex = player1Choice.GetSelectIndexFromChoice();
        side2.Team.ActivePokemonIndex = player2Choice.GetSelectIndexFromChoice();
    }

    private void PerformStruggle(PlayerId playerId)
    {
        Side atkSide = GetSide(playerId);
        Side defSide = GetSide(playerId.OpposingPlayerId());

        Pokemon attacker = atkSide.Team.ActivePokemon;

        if (!Library.Moves.TryGetValue(MoveId.Struggle, out Move? struggle))
        {
            throw new InvalidOperationException($"Struggle move not found in" +
                                                $"library for player {playerId}");
        }
        Move struggleCopy = struggle.Copy();

        Pokemon defender = defSide.Team.ActivePokemon;
        int damage = CalculateDamage(attacker, defender, struggleCopy, 1.0, false, false);
        defender.Damage(damage);

        // Struggle always deals recoil damage to the attacker
        // The recoil is 1/4 of the damage dealt, rounded down at half
        // but at least 1 damage
        int recoil = Math.Max(RoundedDownAtHalf(damage / 4.0), 1);
        attacker.Damage(recoil);

        if (PrintDebug)
        {
            UiGenerator.PrintStruggleAction(attacker, damage, recoil, defender);
        }
    }

    private PlayerId IsWinner()
    {
        if (Side1.Team.IsDefeated)
        {
            return PlayerId.Player2;
        }
        if (Side2.Team.IsDefeated)
        {
            return PlayerId.Player1;
        }
        return PlayerId.None;
    }


    private int CalculateDamage(Pokemon attacker, Pokemon defender, Move move,
        double moveEffectiveness, bool crit = false, bool applyStab = true)
    {
        if (moveEffectiveness == 0.0)
        {
            return 0; // No damage if immune
        }

        int level = attacker.Level;
        int attackStat = attacker.GetAttackStat(move, crit);
        int defenseStat = defender.GetDefenseStat(move, crit);
        int basePower = move.BasePowerCallback?.Invoke(attacker, defender, move) ?? move.BasePower;

        double onBasePowerModifier = move.OnBasePower?.Invoke(attacker, defender, move, Context) ?? 1.0;
        if (Math.Abs(onBasePowerModifier - 1.0) > Epsilon)
        {
            basePower = (int)(basePower * onBasePowerModifier);
        }
        double critModifier = crit ? 1.5 : 1.0;
        double random = 0.85 + BattleRandom.NextDouble() * 0.15; // Random factor between 0.85 and 1.0
        double stabModifier = applyStab ? attacker.GetStabMultiplier(move) : 1.0;

        double burnModifier = 1.0;
        if (attacker.HasCondition(ConditionId.Burn) && move.Category == MoveCategory.Physical &&
            attacker.Ability.Id != AbilityId.Guts)
        {
            burnModifier = 0.5;
        }

        int baseDamage = (int)((2 * level / 5.0 + 2) * basePower * attackStat / defenseStat / 50.0 + 2);
        int critMofified = RoundedDownAtHalf(critModifier * baseDamage);
        int randomModified = RoundedDownAtHalf(random * critMofified);
        int stabModified = RoundedDownAtHalf(stabModifier * randomModified);
        int typeModified = RoundedDownAtHalf(moveEffectiveness * stabModified);
        int burnModified = RoundedDownAtHalf(burnModifier * typeModified);
        return Math.Max(1, burnModified);
    }

    private PlayerId MovesNext(Choice player1Choice, Choice player2Choice)
    {
        int player1Priority = Priority(player1Choice, Side1);
        int player2Priority = Priority(player2Choice, Side2);

        if (player1Priority > player2Priority)
        {
            return PlayerId.Player1;
        }
        if (player2Priority > player1Priority)
        {
            return PlayerId.Player2;
        }
        // If priorities are equal, use the speed of the active Pokémon to determine who moves first
        Pokemon player1Pokemon = Side1.Team.ActivePokemon;
        Pokemon player2Pokemon = Side2.Team.ActivePokemon;

        return CalculateSpeedOrder(player1Pokemon, player2Pokemon);
    }

    private PlayerId CalculateSpeedOrder(Pokemon player1Pokemon, Pokemon player2Pokemon)
    {
        // Create a list of active Pokemon and sort by speed
        var speedOrder = new List<Pokemon> { player1Pokemon, player2Pokemon }
            .OrderByDescending(p => p.CurrentSpe)
            .ToList();

        if (Field.HasPseudoWeather(PseudoWeatherId.TrickRoom))
        {
            speedOrder.Reverse(); // Reverse order if Trick Room is active
        }

        // Check if the fastest Pokemon is unique
        if (speedOrder[0].CurrentSpe != speedOrder[1].CurrentSpe)
        {
            return speedOrder[0] == player1Pokemon ? PlayerId.Player1 : PlayerId.Player2;
        }

        // If speeds are tied, decide randomly
        return BattleRandom.Next(2) == 0 ? PlayerId.Player1 : PlayerId.Player2;
    }

    private static int Priority(Choice choice, Side side)
    {
        if (choice.IsSwitchChoice())
        {
            return 6;
        }
        if (choice.IsMoveChoice())
        {
            int moveIndex = choice.GetMoveIndexFromChoice();
            Pokemon attacker = side.Team.ActivePokemon;
            Move move = attacker.Moves[moveIndex];
            int priority = move.Priority;
            // Check for abilities that modify move priority
            priority = attacker.Ability.OnModifyPriority?.Invoke(priority, move) ?? priority;
            return priority;
        }
        return 0; // Default priority for other choices
    }

    private Choice[] GetAvailableChoices(Side side)
    {
        // TODO: Implement logic to determine available choices based on the
        // current state of the battle.

        PlayerId playerId = side.PlayerId;

        PlayerState playerState = GetPlayerState(playerId);

        switch (playerState)
        {
            case PlayerState.TeamPreviewSelect:
                return GetTeamPreviewChoices(side);
            case PlayerState.MoveSwitchSelect:
                return GetMoveSwitchChoices(side, side.Team.PokemonSet.AnyTeraUsed);
            case PlayerState.FaintedSelect:
            case PlayerState.ForceSwitchSelect:
                return GetSwitchChoices(side);
            case PlayerState.TeamPreviewLocked:
            case PlayerState.MoveSwitchLocked:
            case PlayerState.FaintedLocked:
            case PlayerState.ForceSwitchLocked:
            case PlayerState.Idle:
            default: return [];
        }
    }

    private static Choice[] GetTeamPreviewChoices(Side side)
    {
        List<Choice> choices = [];
        int count = side.Team.PokemonSet.PokemonCount;
        for (int i = 0; i < count; i++)
        {
            choices.Add(i.GetChoiceFromSelectIndex());
        }
        return choices.ToArray();
    }

    private static Choice[] GetMoveSwitchChoices(Side side, bool isTeraUsed)
    {
        // TODO: Implement logic to determine available moves
        // based on the current state of the battle.
        
        Choice[] choices = [];
        for (int i = 0; i < side.Team.ActivePokemon.Moves.Length; i++)
        {
            Move move = side.Team.ActivePokemon.Moves[i];

            // Check if the move is available (has PP left and not disabled)
            if (move is not { Pp: > 0, Disabled: false }) continue;
            choices = choices.Append(i.GetChoiceFromMoveIndex()).ToArray();
            if (isTeraUsed) continue;
            choices = choices.Append(i.GetChoiceFromMoveWithTeraIndex()).ToArray();
        }

        if (choices.Length == 0)
        {
            // If no moves are available, Stuggle is the only option
            choices = choices.Append(Choice.Struggle).ToArray();
        }

        var switchChoices = GetSwitchChoices(side);
        if (switchChoices.Length > 0)
        {
            choices = choices.Concat(switchChoices).ToArray();
        }

        return choices;
    }

    private static Choice[] GetSwitchChoices(Side side)
    {
        List<Choice> choices = [];
        int[] switchablePokemon = side.Team.SwitchOptionIndexes;
        if (switchablePokemon.Length > 0)
        {
            choices.AddRange(switchablePokemon.Select(ChoiceTools.GetChoiceFromSwitchIndex));
        }
        return choices.ToArray();
    }

    private void CheckForChoiceError(PlayerId playerId, Choice choice)
    {
        if (!CanPlayerSubmitChoice(playerId))
        {
            throw new InvalidOperationException($"Player {playerId} cannot submit choice in" +
                                                $"current state");
        }
        if (choice == Choice.None)
        {
            throw new ArgumentException("Choice cannot be 'None'", nameof(choice));
        }
        if (choice == Choice.Invalid)
        {
            throw new ArgumentException("Choice cannot be 'Invalid'", nameof(choice));
        }
        if (playerId == PlayerId.None)
        {
            throw new ArgumentException("PlayerId cannot be 'None'", nameof(playerId));
        }

        PlayerState playerState = playerId == PlayerId.Player1 ? Player1State : Player2State;
        if (playerState is PlayerState.TeamPreviewLocked or
            PlayerState.FaintedLocked or
            PlayerState.ForceSwitchLocked or
            PlayerState.MoveSwitchLocked)
        {
            throw new InvalidOperationException($"Player {playerId} cannot submit choice in" +
                                                $"current state: {playerState}");
        }
        bool isSelectChoice = choice.IsSelectChoice();
        if (isSelectChoice && playerState != PlayerState.TeamPreviewSelect)
        {
            throw new InvalidOperationException($"Player {playerId} cannot submit select choice" +
                                                $"in current state: {playerState}");
        }
        bool isSwitchChoice = choice.IsSwitchChoice();
        if (isSwitchChoice && playerState is not
                (PlayerState.FaintedSelect or PlayerState.ForceSwitchSelect or PlayerState.MoveSwitchSelect))
        {
            throw new InvalidOperationException($"Player {playerId} cannot submit switch choice" +
                                                $"in current state: {playerState}");
        }
        bool isMoveChoice = choice.IsMoveChoice();
        if (isMoveChoice && playerState != PlayerState.MoveSwitchSelect)
        {
            throw new InvalidOperationException($"Player {playerId} cannot submit move choice" +
                                                $"in current state: {playerState}");
        }
    }

    private static int RoundedDownAtHalf(double value)
    {
        return (int)(value + 0.5 - double.Epsilon);
    }

    private PlayerState GetPlayerState(PlayerId playerId)
    {
        return playerId switch
        {
            PlayerId.Player1 => Player1State,
            PlayerId.Player2 => Player2State,
            PlayerId.None => throw new ArgumentException("PlayerId cannot be 'None'", nameof(playerId)),
            _ => throw new ArgumentException("Invalid player ID", nameof(playerId))
        };
    }

    private void SetPlayerState(PlayerId playerId, PlayerState state)
    {
        switch (playerId)
        {
            case PlayerId.Player1:
                Player1State = state;
                break;
            case PlayerId.Player2:
                Player2State = state;
                break;
            case PlayerId.None:
            default:
                throw new ArgumentException("Invalid player ID", nameof(playerId));
        }
    }
}

public static class BattleTools
{
    public static bool CanSubmitChoice(this PlayerState playerState)
    {
        return playerState switch
        {
            PlayerState.TeamPreviewSelect => true,
            PlayerState.TeamPreviewLocked => false,
            PlayerState.MoveSwitchSelect => true,
            PlayerState.MoveSwitchLocked => false,
            PlayerState.FaintedSelect => true,
            PlayerState.FaintedLocked => false,
            PlayerState.ForceSwitchSelect => true,
            PlayerState.ForceSwitchLocked => false,
            PlayerState.Idle => false,
            _ => throw new ArgumentOutOfRangeException(nameof(playerState), playerState, null)
        };
    }

    public static PlayerId OpposingPlayerId(this PlayerId playerId)
    {
        return playerId switch
        {
            PlayerId.Player1 => PlayerId.Player2,
            PlayerId.Player2 => PlayerId.Player1,
            PlayerId.None => throw new ArgumentException("PlayerId cannot be 'None'", nameof(playerId)),
            _ => throw new ArgumentException("Invalid player ID", nameof(playerId))
        };
    }
}

public static class BattleGenerator
{
    public static Battle GenerateTestBattle(Library library, string trainerName1,
        string trainerName2, bool printDebug = false, int? seed = null)
    {
        return new Battle
        {
            Library = library,
            Field = new Field(),
            Side1 = SideGenerator.GenerateTestSide(library, trainerName1, PlayerId.Player1, printDebug),
            Side2 = SideGenerator.GenerateTestSide(library, trainerName2, PlayerId.Player2, printDebug),
            PrintDebug = printDebug,
            BattleSeed = seed,
        };
    }
}