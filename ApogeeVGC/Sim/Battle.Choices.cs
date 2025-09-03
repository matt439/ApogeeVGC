using ApogeeVGC.Player;

namespace ApogeeVGC.Sim;

public partial class Battle
{
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

    private Choice[] GetAvailableChoices(Side side)
    {
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
            // If no moves are available, Struggle is the only option
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
}