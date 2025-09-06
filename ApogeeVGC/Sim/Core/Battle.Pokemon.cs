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

        (int slot1Selection, int slot2Selection, int slot3Selection, int slot4Selection) side1Indexes =
            player1Choice.DecodeTeamPreviewChoice();

        (int slot1Selection, int slot2Selection, int slot3Selection, int slot4Selection) side2Indexes =
            player2Choice.DecodeTeamPreviewChoice();

        Side side1 = GetSide(PlayerId.Player1);
        Side side2 = GetSide(PlayerId.Player2);

        var fullTeam1 = side1.Team.PokemonSet.Pokemons;
        Pokemon[] activeTeam1 = [fullTeam1[side1Indexes.slot1Selection - 1]!,
            fullTeam1[side1Indexes.slot2Selection - 1]!, fullTeam1[side1Indexes.slot3Selection - 1]!,
            fullTeam1[side1Indexes.slot4Selection - 1]!];

        side1.Team.PokemonSet.Pokemons = activeTeam1;

        side1.Team.Slot1PokemonIndex = 0;
        side1.Team.Slot2PokemonIndex = 1;

        // Remove unused pokemon from the team
        (int unused1, int unused2) = ChoiceTools.DecodeUnusedTeamPreviewChoice(side1Indexes);
        Pokemon[] unusedPokemons1 = [fullTeam1[unused1 - 1]!, fullTeam1[unused2 - 1]!];

        side1.Team.PokemonSet.UnusedPokemons = unusedPokemons1;

        var fullTeam2 = side2.Team.PokemonSet.Pokemons;
        Pokemon[] activeTeam2 = [fullTeam2[side2Indexes.slot1Selection - 1]!,
            fullTeam2[side2Indexes.slot2Selection - 1]!, fullTeam2[side2Indexes.slot3Selection - 1]!,
            fullTeam2[side2Indexes.slot4Selection - 1]!];

        side2.Team.PokemonSet.Pokemons = activeTeam2;

        side2.Team.Slot1PokemonIndex = 0;
        side2.Team.Slot2PokemonIndex = 1;

        // Remove unused pokemon from the team
        (int unused3, int unused4) = ChoiceTools.DecodeUnusedTeamPreviewChoice(side2Indexes);
        Pokemon[] unusedPokemons2 = [fullTeam2[unused3 - 1]!, fullTeam2[unused4 - 1]!];
        side2.Team.PokemonSet.UnusedPokemons = unusedPokemons2;
    }

    /// <returns>True if replaced fainted Pokemon, false if the slot is now empty</returns>
    private bool HandleFaintedPokemon(PlayerId player, SlotId slotId)
    {
        int[] optionIndexs = GetSide(player).Team.SwitchOptionIndexes;

        if (optionIndexs.Length == 0)
        {
            // No available switches, the slot remains empty
            GetSide(player).Team.SetPokemonIndex(null, slotId);
            return false;
        }

        // if there is an available switch in, randomly select one and perform the switch.
        int randomIndex = BattleRandom.Next(optionIndexs.Length);
        int selectedSwitchIndex = optionIndexs[randomIndex];
        GetSide(player).Team.SetPokemonIndex(selectedSwitchIndex, slotId);
        Pokemon? newActive = GetSide(player).Team.GetPokemon(slotId);   
        if (newActive is null)
        {
            throw new InvalidOperationException($"No Pokémon found at the selected switch index for player {player}.");
        }
        Field.OnPokemonSwitchIn(newActive, player, Context);
        newActive.OnSwitchIn(Field, AllActivePokemon, Context);
        if (PrintDebug)
        {
            UiGenerator.PrintFaintedSelectAction(GetSide(player).Team.Trainer.Name, newActive);
        }
        return true;
    }

    private void UpdateFaintedStates()
    {
        (PlayerId, SlotId)[] allSlots =
        [
            (PlayerId.Player1, SlotId.Slot1),
            (PlayerId.Player1, SlotId.Slot2),
            (PlayerId.Player2, SlotId.Slot1),
            (PlayerId.Player2, SlotId.Slot2),
        ];

        foreach ((PlayerId playerId, SlotId slotId) in allSlots)
        {
            Side side = GetSide(playerId);
            Pokemon? pokemon = side.Team.GetPokemon(slotId);

            if (pokemon is null) continue;
            if (!pokemon.IsFainted) continue;

            PlayerState playerState = GetPlayerState(playerId);
            if (playerState is PlayerState.FaintedSelect or PlayerState.ForceSwitchSelect or
                PlayerState.FaintedLocked or PlayerState.ForceSwitchLocked) continue;

            bool switched = HandleFaintedPokemon(playerId, slotId);
            if (switched)
            {
                SetPlayerState(playerId, PlayerState.FaintedLocked);
            }
            else
            {
                // No available switches, the slot is now empty
                SetPlayerState(playerId, PlayerState.Idle);
                if (PrintDebug)
                {
                    UiGenerator.PrintFaintedAction(pokemon);
                }
            }
        }

        SetPlayerState(PlayerId.Player1, PlayerState.Idle);
        SetPlayerState(PlayerId.Player2, PlayerState.Idle);

        // check for fainted pokemon in either slot
        // if there is an available switch in, randomly select one and perform the switch.
        // set the player state to idle


        //Pokemon? side1Slot1Pokemon = Side1.Team.Slot1Pokemon;
        //if (side1Slot1Pokemon is not null)
        //{
        //    if (Side1.Team.Slot1Pokemon!.IsFainted && Player1State != PlayerState.FaintedSelect &&
        //        Player1State != PlayerState.ForceSwitchSelect)
        //    {
        //        Player1State = PlayerState.FaintedSelect;
        //        Player1FaintedSwitches.Add(SlotId.Slot1);
        //        if (PrintDebug)
        //        {
        //            UiGenerator.PrintFaintedAction(Side1.Team.Slot1Pokemon);
        //        }
        //    }
        //}
        //Pokemon? side1Slot2Pokemon = Side1.Team.Slot2Pokemon;
        //if (side1Slot2Pokemon is not null)
        //{
        //    if (Side1.Team.Slot2Pokemon!.IsFainted && Player1State != PlayerState.FaintedSelect &&
        //        Player1State != PlayerState.ForceSwitchSelect)
        //    {
        //        Player1State = PlayerState.FaintedSelect;
        //        Player1FaintedSwitches.Add(SlotId.Slot2);
        //        if (PrintDebug)
        //        {
        //            UiGenerator.PrintFaintedAction(Side1.Team.Slot2Pokemon);
        //        }
        //    }
        //}
        //Pokemon? side2Slot1Pokemon = Side2.Team.Slot1Pokemon;
        //if (side2Slot1Pokemon is not null)
        //{
        //    if (Side2.Team.Slot1Pokemon!.IsFainted && Player2State != PlayerState.FaintedSelect &&
        //        Player2State != PlayerState.ForceSwitchSelect)
        //    {
        //        Player2State = PlayerState.FaintedSelect;
        //        Player2FaintedSwitches.Add(SlotId.Slot1);
        //        if (PrintDebug)
        //        {
        //            UiGenerator.PrintFaintedAction(Side2.Team.Slot1Pokemon);
        //        }
        //    }
        //}
        //Pokemon? side2Slot2Pokemon = Side2.Team.Slot2Pokemon;
        //if (side2Slot2Pokemon is not null)
        //{
        //    if (Side2.Team.Slot2Pokemon!.IsFainted && Player2State != PlayerState.FaintedSelect &&
        //        Player2State != PlayerState.ForceSwitchSelect)
        //    {
        //        Player2State = PlayerState.FaintedSelect;
        //        Player2FaintedSwitches.Add(SlotId.Slot2);
        //        if (PrintDebug)
        //        {
        //            UiGenerator.PrintFaintedAction(Side2.Team.Slot2Pokemon);
        //        }
        //    }
        //}
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