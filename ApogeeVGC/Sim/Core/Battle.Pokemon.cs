using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.Core;

public partial class Battle
{
    private void PerformSwitch(PlayerId playerId, SlotChoice.SwitchChoice choice)
    {
        if (IsWinner() != PlayerId.None)
        {
            throw new InvalidOperationException("Cannot switch Pokémon when the battle has already ended.");
        }

        Side side = GetSide(playerId);
        Pokemon prevActive = choice.SwitchOutPokemon;
        prevActive.OnSwitchOut();
        side.SwitchSlots(choice.SwitchOutSlot, choice.SwitchInSlot);
        Pokemon newActive = choice.SwitchInPokemon;
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
                        side.Slot1);
                }
                break;
            case PlayerState.FaintedLocked:
                if (PrintDebug)
                {
                    UiGenerator.PrintFaintedSelectAction(side.Team.Trainer.Name,
                        side.Slot1);
                }
                break;
            case PlayerState.ForceSwitchLocked:
                if (PrintDebug)
                {
                    UiGenerator.PrintForceSwitchInAction(side.Team.Trainer.Name, side.Slot1);
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

    private void PerformTeamPreviewSelect(TeamPreviewChoice player1Choice, TeamPreviewChoice player2Choice)
    {
        if (IsWinner() != PlayerId.None)
        {
            throw new InvalidOperationException("Cannot perform team preview select when the battle has already ended.");
        }

        Side1.SetSlotsWithCopies(player1Choice.Pokemon);
        Side2.SetSlotsWithCopies(player2Choice.Pokemon);
    }

    private void UpdateFaintedStates()
    {
        if (Side1.Slot1.IsFainted && Player1State != PlayerState.FaintedSelect &&
            Player1State != PlayerState.ForceSwitchSelect)
        {
            Player1State = PlayerState.FaintedSelect;
            if (PrintDebug)
            {
                UiGenerator.PrintFaintedAction(Side1.Slot1);
            }
        }
        if (Side2.Slot1.IsFainted && Player2State != PlayerState.FaintedSelect &&
            Player2State != PlayerState.ForceSwitchSelect)
        {
            Player2State = PlayerState.FaintedSelect;
            if (PrintDebug)
            {
                UiGenerator.PrintFaintedAction(Side2.Slot1);
            }
        }
    }

    private PlayerId IsWinner()
    {
        if (Side1.IsDefeated)
        {
            return PlayerId.Player2;
        }
        if (Side2.IsDefeated)
        {
            return PlayerId.Player1;
        }
        return PlayerId.None;
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
}