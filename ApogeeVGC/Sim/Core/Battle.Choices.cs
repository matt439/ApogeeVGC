using System.Runtime.CompilerServices;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Ui;

namespace ApogeeVGC.Sim.Core;

public partial class Battle
{
    public void SubmitChoice(PlayerId playerId, BattleChoice choice)
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
                if (Player1PendingChoice is not null)
                {
                    if (Player1PendingChoice is SlotChoice { IsSwitchChoice: true } slotChoice)
                    {
                        PerformSwitch(PlayerId.Player1, (SlotChoice.SwitchChoice)slotChoice);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid choice for Player 1 in FaintedSelect state:" +
                                                            $"{Player1PendingChoice}");
                    }
                }
                if (Player2PendingChoice is not null)
                {
                    if (Player2PendingChoice is SlotChoice { IsSwitchChoice: true } slotChoice)
                    {
                        PerformSwitch(PlayerId.Player2, (SlotChoice.SwitchChoice)slotChoice);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid choice for Player 2 in FaintedSelect state:" +
                                                            $"{Player2PendingChoice}");
                    }
                }
                ClearPendingChoices();
            }
            else if (IsReadyForForceSwitchProcessing())
            {
                // Process forced switch choices
                if (Player1PendingChoice is SlotChoice {IsSwitchChoice: true} slotChoice)
                {
                    PerformSwitch(PlayerId.Player1, (SlotChoice.SwitchChoice)slotChoice);
                    CLearPendingChoice(PlayerId.Player1);

                    // If the other player's state is not idle, they still need to execute their move/struggle
                    if (Player2State == PlayerState.MoveSwitchLocked && Player2PendingChoice != null)
                    {
                        ExecutePlayerChoice(PlayerId.Player2, Player2PendingChoice);
                        CLearPendingChoice(PlayerId.Player2);
                        SetPlayerState(PlayerId.Player2, PlayerState.Idle);
                        // Update player states for any fainted Pokemon
                        UpdateFaintedStates();
                    }
                }
                else if (Player2PendingChoice is SlotChoice { IsSwitchChoice: true } slotChoice2)
                {
                    PerformSwitch(PlayerId.Player2, (SlotChoice.SwitchChoice)slotChoice2);
                    CLearPendingChoice(PlayerId.Player2);

                    // If the other player's state is not idle, they still need to execute their move/struggle
                    if (Player1State == PlayerState.MoveSwitchLocked && Player1PendingChoice != null)
                    {
                        ExecutePlayerChoice(PlayerId.Player1, Player1PendingChoice);
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

            bool requestingPlayer1Input = BattleTools.CanSubmitChoice(Player1State);
            bool requestingPlayer2Input = BattleTools.CanSubmitChoice(Player2State);

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

    public BattleChoice[] GetAvailableChoices(PlayerId playerId)
    {
        lock (ChoiceLock)
        {
            if (!CanPlayerSubmitChoice(playerId))
            {
                throw new InvalidOperationException($"Player {playerId} cannot submit choice in current state");
            }

            return playerId switch
            {
                PlayerId.Player1 => GetAvailableChoices((Side)Side1),
                PlayerId.Player2 => GetAvailableChoices((Side)Side2),
                PlayerId.None => throw new ArgumentException("PlayerId cannot be 'None'"),
                _ => throw new ArgumentException("Invalid player ID", nameof(playerId)),
            };
        }
    }

    private BattleChoice[] GetAvailableChoices(Side side)
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

    private static BattleChoice[] GetTeamPreviewChoices(Side side)
    {
        List<BattleChoice> choices = [];
        int count = side.Team.PokemonSet.PokemonCount;
        if (count != 6)
        {
            throw new InvalidOperationException("Team preview choices can only be generated" +
                                                "for full teams of 6 Pokémon.");
        }

        // Get all 6 Pokémon from the team
        var pokemon = side.AllSlots;

        // Generate all permutations of the 6 Pokémon
        var permutations = GeneratePermutations(pokemon.ToArray());

        // Create a TeamPreviewChoice for each permutation
        choices.AddRange(permutations.Select(permutation =>
            permutation.Select((p, index) =>
                CreatePokemonWithSlot(p, (SlotId)(index + 1))).ToList()).
            Select(TeamPreviewChoice.CreateSinglesTeamPreview));

        return choices.ToArray();
    }

    /// <summary>
    /// Generates all permutations of the given array using Heap's algorithm.
    /// For 6 Pokémon, this generates 6! = 720 permutations.
    /// </summary>
    private static IEnumerable<T[]> GeneratePermutations<T>(T[] array)
    {
        int n = array.Length;
        int[] c = new int[n];
        var result = new T[n];
        Array.Copy(array, result, n);

        yield return (T[])result.Clone();

        int i = 0;
        while (i < n)
        {
            if (c[i] < i)
            {
                if (i % 2 == 0)
                {
                    // Swap first and i-th element
                    (result[0], result[i]) = (result[i], result[0]);
                }
                else
                {
                    // Swap c[i]-th and i-th element
                    (result[c[i]], result[i]) = (result[i], result[c[i]]);
                }

                yield return (T[])result.Clone();
                c[i]++;
                i = 0;
            }
            else
            {
                c[i] = 0;
                i++;
            }
        }
    }

    /// <summary>
    /// Creates a copy of a Pokémon with a new SlotId.
    /// This is needed because team preview determines the final slot ordering.
    /// </summary>
    private static Pokemon CreatePokemonWithSlot(Pokemon original, SlotId newSlotId)
    {
        Pokemon copy = original.Copy();
        copy.SlotId = newSlotId;
        return copy;
    }

    private BattleChoice[] GetMoveSwitchChoices(Side side, bool isTeraUsed)
    {
        List<BattleChoice> choices = [];
        Side defendingSide = GetOtherSide(side);
        Pokemon attacker = side.Slot1;
        Pokemon defender = defendingSide.Slot1;

        foreach (Move move in attacker.Moves)
        {
            // Check if the move is available (has PP left and not disabled)
            if (move is not { Pp: > 0, Disabled: false }) continue;

            SlotChoice.MoveChoice moveChoice = new(attacker, move,
                false, MoveNormalTarget.FoeSlot1, [defender]);
            choices.Add(moveChoice);

            if (isTeraUsed) continue;
            SlotChoice.MoveChoice moveChoiceTera = new(attacker, move,
                true, MoveNormalTarget.FoeSlot1, [defender]);
            choices.Add(moveChoiceTera);
        }

        if (choices.Count == 0)
        {
            // If no moves are available, Struggle is the only option
            SlotChoice.MoveChoice struggleChoice = new(attacker, Library.Moves[MoveId.Struggle],
                false, MoveNormalTarget.FoeSlot1, [defender]);
            choices.Add(struggleChoice);
        }

        var switchChoices = GetSwitchChoices(side);
        if (switchChoices.Length > 0)
        {
            choices.AddRange(switchChoices);
        }

        return choices.ToArray();
    }

    private Side GetOtherSide(Side side)
    {
        PlayerId player = side.PlayerId;
        return player == PlayerId.Player1 ? Side2 : Side1;
    }

    private static BattleChoice[] GetSwitchChoices(Side side)
    {
        List<BattleChoice> choices = [];
        var switchOptionSlots = side.SwitchOptionSlots;

        choices.AddRange(switchOptionSlots.Select(pokemon =>
            new SlotChoice.SwitchChoice(side.Slot1, pokemon)));
        return choices.ToArray();
    }

    private void CheckForChoiceError(PlayerId playerId, BattleChoice choice)
    {
        if (!CanPlayerSubmitChoice(playerId))
        {
            throw new InvalidOperationException($"Player {playerId} cannot submit choice in" +
                                                $"current state");
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
        bool isSelectChoice = choice is TeamPreviewChoice;
        if (isSelectChoice && playerState != PlayerState.TeamPreviewSelect)
        {
            throw new InvalidOperationException($"Player {playerId} cannot submit select choice" +
                                                $"in current state: {playerState}");
        }

        bool isSwitchChoice = choice is SlotChoice { IsSwitchChoice: true };
        if (isSwitchChoice && playerState is not
                (PlayerState.FaintedSelect or PlayerState.ForceSwitchSelect or PlayerState.MoveSwitchSelect))
        {
            throw new InvalidOperationException($"Player {playerId} cannot submit switch choice" +
                                                $"in current state: {playerState}");
        }

        bool isMoveChoice = choice is SlotChoice { IsMoveChoice: true };
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