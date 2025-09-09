using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;

namespace ApogeeVGC.Sim.Core;

public partial class Battle
{
    private void SetPendingChoice(PlayerId playerId, BattleChoice? choice)
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

    private void UpdatePlayerState(PlayerId playerId, BattleChoice? choice)
    {
        if (choice is null) return; // No choice to update state with

        PlayerState playerState = GetPlayerState(playerId);

        switch (playerState)
        {
            case PlayerState.MoveSwitchSelect:
                if (choice is SlotChoice slotChoice && (slotChoice.IsMoveChoice || slotChoice.IsSwitchChoice))
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
                if (choice is SlotChoice { IsSwitchChoice: true })
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
                if (choice is SlotChoice { IsSwitchChoice: true })
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
                if (choice is TeamPreviewChoice { IsSinglesTeamPreviewChoice: true })
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

    // State checking methods
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
        lock (ChoiceLock)
        {
            return !Player1State.CanSubmitChoice() && !Player2State.CanSubmitChoice();
        }
    }
}