using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.Core;

public partial class Battle
{
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