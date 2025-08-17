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
    SwitchSelect,
    SwitchLocked,
    //TagetSelect,
    //TargetLocked,
    // TODO: Implement these above states
    Idle,
}

public class Battle
{
    public required Library Library { get; init; }
    public required Field Field { get; init; }
    public required Side Side1 { get; init; }
    public required Side Side2 { get; init; }
    public int Turn { get; private set; }
    //public BattleRequestState RequestState
    //{
    //    get
    //    {
    //        lock (ChoiceLock)
    //        {
    //            return field;
    //        }
    //    }
    //    private set
    //    {
    //        lock (ChoiceLock)
    //        {
    //            field = value;
    //        }
    //    }
    //} = BattleRequestState.RequestingBothPlayersInput;
    //private BattleState State { get; set; } = BattleState.Playing;
    private PlayerState Player1State { get; set; } = PlayerState.MoveSwitchSelect;
    private PlayerState Player2State { get; set; } = PlayerState.MoveSwitchSelect;
    private Choice? Player1PendingChoice { get; set; }
    private Choice? Player2PendingChoice { get; set; }
    private object ChoiceLock { get; } = new();

    public void SubmitChoice(PlayerId playerId, Choice choice)
    {
        lock (ChoiceLock)
        {
            CheckForChoiceError(playerId, choice);

            PlayerState playerState = GetPlayerState(playerId);

            SetPendingChoice(playerId, choice);

            if (IsReadyForMoveSwitchProcessing())
            {
                PerformMoveSwitches(Player1PendingChoice ??
                                    throw new ArgumentException("Player1PendingChoice cannot be null"),
                    Player2PendingChoice ?? throw new ArgumentException("Player2PendingChoice cannot be null"));
                ClearPendingChoices();
            }
            else if (IsReadyForSwitchProcessing())
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

            if (!IsEndOfTurn()) return;

            Turn++;
            // Reset player states for the next turn
            Player1State = PlayerState.MoveSwitchSelect;
            Player2State = PlayerState.MoveSwitchSelect;
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
    }

    private void PerformMoveSwitches(Choice player1Choice, Choice player2Choice)
    {
        PlayerId nextPlayer = MovesNext(player1Choice, player2Choice);

        // Create execution order based on priority
        var executionOrder = nextPlayer == PlayerId.Player1
            ? new[] { (PlayerId.Player1, player1Choice), (PlayerId.Player2, player2Choice) }
            : new[] { (PlayerId.Player2, player2Choice), (PlayerId.Player1, player1Choice) };

        // Execute choices in priority order
        foreach ((PlayerId playerId, Choice choice) in executionOrder)
        {
            if (ExecutePlayerChoice(playerId, choice)) continue;

            // Battle ended, stop processing
            return;
        }
        ClearPendingChoices();
    }

    /// <summary>
    /// Execute a single player's choice and handle any resulting fainted states
    /// Returns false if the battle has ended, true otherwise
    /// </summary>
    private bool ExecutePlayerChoice(PlayerId playerId, Choice choice)
    {
        if (choice.IsMoveChoice())
        {
            PerformMove(playerId, choice);
            SetPendingChoice(playerId, null);
            SetPlayerState(playerId, PlayerState.Idle);

            // Check for battle end after move
            if (IsWinner() != PlayerId.None)
            {
                return false; // Battle has ended
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
        else
        {
            throw new InvalidOperationException($"Invalid choice for {playerId}: {choice}");
        }

        return true; // Continue battle
    }

    /// <summary>
    /// Check both sides for fainted Pokemon and update player states accordingly
    /// </summary>
    private void UpdateFaintedStates()
    {
        if (Side1.Team.ActivePokemon.IsFainted)
        {
            Player1State = PlayerState.SwitchSelect;
        }
        if (Side2.Team.ActivePokemon.IsFainted)
        {
            Player2State = PlayerState.SwitchSelect;
        }
    }

    private void ClearPendingChoices()
    {
        Player1PendingChoice = null;
        Player2PendingChoice = null;
    }

    private bool IsReadyForMoveSwitchProcessing()
    {
        return Player1State == PlayerState.MoveSwitchLocked && Player2State == PlayerState.MoveSwitchLocked;
    }

    private bool IsReadyForSwitchProcessing()
    {
        // 1 or 2 players can be locked in switch state, but not both
        return Player1State == PlayerState.SwitchLocked && Player2State == PlayerState.SwitchLocked ||
               Player1State == PlayerState.SwitchLocked && Player2State == PlayerState.Idle ||
               Player1State == PlayerState.Idle && Player2State == PlayerState.SwitchLocked;
    }

    private bool IsEndOfTurn()
    {
        if (IsWinner() != PlayerId.None)
        {
            return true; // Battle has ended, no more turns
        }
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
                _ => false
            };
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

    private void PerformMove(PlayerId playerId, Choice choice)
    {
        Side atkSide = GetSide(playerId);
        Side defSide = GetSide(playerId.OpposingPlayerId());

        int moveIndex = choice.GetMoveIndexFromChoice();
        Pokemon attacker = atkSide.Team.ActivePokemon;
        Move move = attacker.Moves[moveIndex];
        Pokemon defender = defSide.Team.ActivePokemon;
        int damage = Damage(attacker, defender, move);
        defender.Damage(damage);
        UiGenerator.PrintMoveAction(attacker, move, damage, defender);
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
        side.Team.ActivePokemonIndex = choice.GetSwitchIndexFromChoice();
        UiGenerator.PrintSwitchAction(side.Team.Trainer.Name, prevActive,
            side.Team.ActivePokemon);
    }


    //public void ApplyChoice(PlayerId playerId, Choice choice)
    //{
    //    bool player1Input = playerId == PlayerId.Player1 && RequestState == BattleRequestState.RequestingPlayer1Input;
    //    bool player2Input = playerId == PlayerId.Player2 && RequestState == BattleRequestState.RequestingPlayer2Input;
    //    if (!player1Input && !player2Input)
    //    {
    //        throw new InvalidOperationException("Cannot apply choice when not in the correct state for the player.");
    //    }
    //    if (RequestState == BattleRequestState.RequestingBothPlayersInput)
    //    {
    //        throw new InvalidOperationException("Cannot apply choice when both players are expected to input.");
    //    }
    //    if (IsWinner() != PlayerId.None)
    //    {
    //        throw new InvalidOperationException("Cannot apply choice when the battle has already ended.");
    //    }

    //    if (player1Input)
    //    {
    //        ApplyChoice(choice, Side1, Side2);
    //    }
    //    else if (player2Input)
    //    {
    //        ApplyChoice(choice, Side2, Side1);
    //    }

    //    CheckForAndHandleWin();
    //}

    //public void ApplyChoices(Choice player1Choice, Choice player2Choice)
    //{
    //    PlayerId nextPlayer = MovesNext(player1Choice, player2Choice);
    //    if (nextPlayer == PlayerId.Player1)
    //    {
    //        ApplyChoice(player1Choice, Side1, Side2);
    //        if (CheckForAndHandleWin())
    //        {
    //            // If Player 1 wins, we don't need to apply Player 2's choice.
    //            return;
    //        }
    //        if (CheckForAndHandleFainted())
    //            //if (Side1.Team.ActivePokemon.IsFainted)
    //            //{
    //            //    UiGenerator.PrintSwitchAction(Side1.Team.Trainer.Name, Side1.Team.ActivePokemon,
    //            //        Side1.Team.ActivePokemon);
    //            //}
    //            //else
    //            //{
    //            //    UiGenerator.PrintSwitchAction(Side2.Team.Trainer.Name, Side2.Team.ActivePokemon,
    //            //        Side2.Team.ActivePokemon);
    //            //}
    //            ApplyChoice(player2Choice, Side2, Side1);
    //    }
    //    else
    //    {
    //        ApplyChoice(player2Choice, Side2, Side1);
    //        if (CheckForAndHandleWin())
    //        {
    //            // If Player 2 wins, we don't need to apply Player 1's choice.
    //            return;
    //        }
    //        ApplyChoice(player1Choice, Side1, Side2);
    //    }
    //    CheckForAndHandleWin();
    //    Turn++;
    //}

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
                _ => throw new ArgumentException("Invalid player ID", nameof(playerId))
            };
        }
    }

    public Side GetSide(PlayerId playerId)
    {
        return playerId switch
        {
            PlayerId.Player1 => Side1,
            PlayerId.Player2 => Side2,
            PlayerId.None => throw new ArgumentException("PlayerId cannot be 'None'", nameof(playerId)),
            _ => throw new ArgumentException("Invalid player ID", nameof(playerId))
        };
    }

    //private bool CheckForAndHandleFainted()
    //{
    //    bool player1Fainted = Side1.Team.ActivePokemon.IsFainted;
    //    bool player2Fainted = Side2.Team.ActivePokemon.IsFainted;

    //    if (!player1Fainted && !player2Fainted)
    //    {
    //        return false; // No fainted Pokémon, nothing to handle
    //    }




    //    return true;
    //}

    //private bool CheckForAndHandleWin()
    //{
    //    PlayerId winner = IsWinner();
    //    if (winner == PlayerId.None) return false;

    //    RequestState = winner == PlayerId.Player1 ? BattleRequestState.Player1Win : BattleRequestState.Player2Win;
    //    Console.WriteLine($"{winner} wins the battle!");
    //    return true;
    //}

    private PlayerId IsWinner()
    {
        if (Side1.Team.IsDefeated)
        {
            return PlayerId.Player1;
        }
        if (Side2.Team.IsDefeated)
        {
            return PlayerId.Player2;
        }
        return PlayerId.None;
    }


    private int Damage(Pokemon attacker, Pokemon defender, Move move, bool crit = false)
    {
        int level = attacker.Level;
        int attackStat = attacker.GetAttackStat(move);
        int defenseStat = defender.GetDefenseStat(move);
        int basePower = move.BasePower;
        double critModifier = crit ? 1.5 : 1.0;
        double random = 0.85 + new Random().NextDouble() * 0.15; // Random factor between 0.85 and 1.0
        bool stab = attacker.IsStab(move);
        double stabModifier = stab ? 1.5 : 1.0;
        double typeEffectiveness = Library.TypeChart.GetEffectiveness(defender.Specie.Types, move.Type);

        int baseDamage = (int)((2 * level / 5.0 + 2) * basePower * attackStat / defenseStat / 50.0 + 2);
        int critMofified = RoundedDownAtHalf(critModifier * baseDamage);
        int randomModified = RoundedDownAtHalf(random * critMofified);
        int stabModified = RoundedDownAtHalf(stabModifier * randomModified);
        int typeModified = RoundedDownAtHalf(typeEffectiveness * stabModified);
        return Math.Max(1, typeModified);
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
        if (player1Pokemon.CurrentSpe > player2Pokemon.CurrentSpe)
        {
            return PlayerId.Player1;
        }
        if (player2Pokemon.CurrentSpe > player1Pokemon.CurrentSpe)
        {
            return PlayerId.Player2;
        }
        // Speed ties are resolved randomly
        return new Random().Next(2) == 0 ? PlayerId.Player1 : PlayerId.Player2;
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
            return move.Priority;
        }
        return 0; // Default priority for other choices
    }

    //private void ApplyChoice(Choice choice, Side atkSide, Side defSide)
    //{
    //    if (choice.IsSwitchChoice())
    //    {
            
    //    }
    //    else if (choice.IsMoveChoice())
    //    {
            
    //    }
    //}

    private Choice[] GetAvailableChoices(Side side)
    {
        // TODO: Implement logic to determine available choices based on the current state of the battle.

        PlayerId playerId = side.PlayerId;

        PlayerState playerState = GetPlayerState(playerId);

        switch (playerState)
        {
            case PlayerState.TeamPreviewSelect:
                throw new NotImplementedException();
            case PlayerState.MoveSwitchSelect:
                return GetMoveSwitchChoices(side);
            case PlayerState.SwitchSelect:
                return GetSwitchChoices(side);
            case PlayerState.TeamPreviewLocked:
            case PlayerState.MoveSwitchLocked:
            case PlayerState.SwitchLocked:
            case PlayerState.Idle:
            default: return [];
        }
    }

    private static Choice[] GetMoveSwitchChoices(Side side)
    {
        // TODO: Implement logic to determine available moves based on the current state of the battle.
        
        Choice[] choices = [];
        for (int i = 0; i < side.Team.ActivePokemon.Moves.Length; i++)
        {
            choices = choices.Append(i.GetChoiceFromMoveIndex()).ToArray();
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
            throw new InvalidOperationException($"Player {playerId} cannot submit choice in current state");
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
            PlayerState.SwitchLocked or
            PlayerState.MoveSwitchLocked)
        {
            throw new InvalidOperationException($"Player {playerId} cannot submit choice in current state: {playerState}");
        }
        bool isSwitchChoice = choice.IsSwitchChoice();
        if (isSwitchChoice && playerState is not (PlayerState.SwitchSelect or PlayerState.MoveSwitchSelect))
        {
            throw new InvalidOperationException($"Player {playerId} cannot submit switch choice in current state: {playerState}");
        }
        bool isMoveChoice = choice.IsMoveChoice();
        if (isMoveChoice && playerState != PlayerState.MoveSwitchSelect)
        {
            throw new InvalidOperationException($"Player {playerId} cannot submit move choice in current state: {playerState}");
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
            PlayerState.SwitchSelect => true,
            PlayerState.SwitchLocked => false,
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
    public static Battle GenerateTestBattle(Library library, string trainerName1, string trainerName2)
    {
        return new Battle
        {
            Library = library,
            Field = new Field(),
            Side1 = SideGenerator.GenerateTestSide(library, trainerName1, PlayerId.Player1),
            Side2 = SideGenerator.GenerateTestSide(library, trainerName2, PlayerId.Player2)
        };
    }
}