using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.Core;

public partial class Battle
{
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

    private void HandleStartOfTurn()
    {
        Field.OnTurnStart(Side1, Side2, Context);
        HandleItemTurnStarts();
        HandleConditionTurnStarts();
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

    private void HandleEndOfTeamPreviewTurn()
    {
        foreach (Pokemon pokemon in AllActivePokemon)
        {
            if (pokemon.IsFainted) continue; // Skip if fainted
            pokemon.OnSwitchIn(Field, AllActivePokemon, Context); // Trigger switch-in effects
        }
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

    private void HandleItemTurnStarts()
    {
        // check all active pokemon items with OnStart
        foreach (Pokemon pokemon in AllActivePokemon)
        {
            if (pokemon.IsFainted) continue; // Skip if fainted
            pokemon.Item?.OnStart?.Invoke(pokemon, Context);
        }
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
}