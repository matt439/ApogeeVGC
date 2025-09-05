using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.Core;

public partial class Battle
{
    private void PerformSwitch(PlayerId playerId, SlotId slotId)
    {
        if (IsWinner() != PlayerId.None)
        {
            throw new InvalidOperationException("Cannot switch Pokémon when the battle has already ended.");
        }

        var choice = GetPendingChoice(playerId, slotId);
        if (choice is null)
        {
            throw new InvalidOperationException($"No pending choice found for player {playerId} in slot {slotId}.");
        }

        if (!choice.Value.IsSwitchChoice())
        {
            throw new ArgumentException("Choice must be a switch choice to switch Pokémon.", nameof(choice));
        }

        Side side = GetSide(playerId);
        Pokemon? prevActive = side.Team.GetPokemon(slotId);
        if (prevActive is null)
        {
            throw new InvalidOperationException($"No active Pokémon found in slot {slotId} for player {playerId}.");
        }
        prevActive.OnSwitchOut();
        side.Team.SetPokemonIndex(choice.Value.GetSwitchIndexFromChoice(), slotId);
        Pokemon? newActive = side.Team.GetPokemon(slotId);
        if (newActive is null)
        {
            throw new InvalidOperationException($"No Pokémon found at the selected switch index for player {playerId}.");
        }
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
                        newActive);
                }
                break;
            case PlayerState.FaintedLocked:
                if (PrintDebug)
                {
                    UiGenerator.PrintFaintedSelectAction(side.Team.Trainer.Name, newActive);
                }
                break;
            case PlayerState.ForceSwitchLocked:
                if (PrintDebug)
                {
                    UiGenerator.PrintForceSwitchInAction(side.Team.Trainer.Name, newActive);
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

        (int slot1Selection, int slot2Selection) side1Indexes = player1Choice.DecodeTeamPreviewChoice();
        (int slot1Selection, int slot2Selection) side2Indexes = player2Choice.DecodeTeamPreviewChoice();


        Side side1 = GetSide(PlayerId.Player1);
        Side side2 = GetSide(PlayerId.Player2);
        // Set the selected Pokémon for each player
        side1.Team.Slot1PokemonIndex = side1Indexes.slot1Selection;
        side1.Team.Slot2PokemonIndex = side1Indexes.slot2Selection;
        side2.Team.Slot1PokemonIndex = side2Indexes.slot1Selection;
        side2.Team.Slot2PokemonIndex = side2Indexes.slot2Selection;

        // TODO: Add unselected Pokémon to the unused list
    }

    private void UpdateFaintedStates()
    {
        Pokemon? side1Slot1Pokemon = Side1.Team.Slot1Pokemon;
        if (side1Slot1Pokemon is not null)
        {
            if (Side1.Team.Slot1Pokemon!.IsFainted && Player1State != PlayerState.FaintedSelect &&
                Player1State != PlayerState.ForceSwitchSelect)
            {
                Player1State = PlayerState.FaintedSelect;
                if (PrintDebug)
                {
                    UiGenerator.PrintFaintedAction(Side1.Team.Slot1Pokemon);
                }
            }
        }
        Pokemon? side1Slot2Pokemon = Side1.Team.Slot2Pokemon;
        if (side1Slot2Pokemon is not null)
        {
            if (Side1.Team.Slot2Pokemon!.IsFainted && Player1State != PlayerState.FaintedSelect &&
                Player1State != PlayerState.ForceSwitchSelect)
            {
                Player1State = PlayerState.FaintedSelect;
                if (PrintDebug)
                {
                    UiGenerator.PrintFaintedAction(Side1.Team.Slot2Pokemon);
                }
            }
        }
        Pokemon? side2Slot1Pokemon = Side2.Team.Slot1Pokemon;
        if (side2Slot1Pokemon is not null)
        {
            if (Side2.Team.Slot1Pokemon!.IsFainted && Player2State != PlayerState.FaintedSelect &&
                Player2State != PlayerState.ForceSwitchSelect)
            {
                Player2State = PlayerState.FaintedSelect;
                if (PrintDebug)
                {
                    UiGenerator.PrintFaintedAction(Side2.Team.Slot1Pokemon);
                }
            }
        }
        Pokemon? side2Slot2Pokemon = Side2.Team.Slot2Pokemon;
        if (side2Slot2Pokemon is not null)
        {
            if (Side2.Team.Slot2Pokemon!.IsFainted && Player2State != PlayerState.FaintedSelect &&
                Player2State != PlayerState.ForceSwitchSelect)
            {
                Player2State = PlayerState.FaintedSelect;
                if (PrintDebug)
                {
                    UiGenerator.PrintFaintedAction(Side2.Team.Slot2Pokemon);
                }
            }
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