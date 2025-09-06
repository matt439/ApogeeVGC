using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
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
        // Split the double choices into their individual slot choices
        var player1Slot1Choice = player1Choice.GetSlot1Choice();
        var player1Slot2Choice = player1Choice.GetSlot2Choice();
        var player2Slot1Choice = player2Choice.GetSlot1Choice();
        var player2Slot2Choice = player2Choice.GetSlot2Choice();

        Pokemon? player1Slot1Pokemon = Side1.Team.Slot1Pokemon;
        Pokemon? player1Slot2Pokemon = Side1.Team.Slot2Pokemon;
        Pokemon? player2Slot1Pokemon = Side2.Team.Slot1Pokemon;
        Pokemon? player2Slot2Pokemon = Side2.Team.Slot2Pokemon;

        List<(Choice?, Pokemon?)> allChoicesAndPokemons = [
            (player1Slot1Choice, player1Slot1Pokemon),
            (player1Slot2Choice, player1Slot2Pokemon),
            (player2Slot1Choice, player2Slot1Pokemon),
            (player2Slot2Choice, player2Slot2Pokemon),
        ];
        List<(Choice, Pokemon)> validChoicesAndPokemons = [];

        foreach (var choiceAndPokemon in allChoicesAndPokemons)
        {
            if (choiceAndPokemon.Item1 is not null && choiceAndPokemon.Item2 is null)
            {
                throw new InvalidOperationException("Choice cannot be made for a non-existent Pokémon.");
            }
            if (choiceAndPokemon.Item1 is not null && choiceAndPokemon.Item2!.IsFainted)
            {
                throw new InvalidOperationException("Move choice cannot be made for a fainted Pokémon.");
            }
            if (choiceAndPokemon.Item1 is null && choiceAndPokemon.Item2 is not null)
            {
                throw new InvalidOperationException("No choice made for an active Pokémon.");
            }
            if (choiceAndPokemon.Item1 is not null && choiceAndPokemon.Item2 is not null)
            {
                validChoicesAndPokemons.Add((choiceAndPokemon.Item1.Value, choiceAndPokemon.Item2));
            }
        }

        // Check if either player is terastilizing
        bool player1Slot1Tera = player1Slot1Choice?.IsMoveWithTeraChoice() ?? false;
        bool player1Slot2Tera = player1Slot2Choice?.IsMoveWithTeraChoice() ?? false;
        bool player2Slot1Tera = player2Slot1Choice?.IsMoveWithTeraChoice() ?? false;
        bool player2Slot2Tera = player2Slot2Choice?.IsMoveWithTeraChoice() ?? false;

        bool player1Tera = player1Slot1Tera || player1Slot2Tera;
        bool player2Tera = player2Slot1Tera || player2Slot2Tera;

        if (player1Tera && player2Tera)
        {
            Pokemon player1Pokemon = player1Slot1Tera
                ? player1Slot1Pokemon ?? throw new InvalidOperationException()
                : player1Slot2Pokemon ?? throw new InvalidOperationException();

            Pokemon player2Pokemon = player2Slot1Tera
                ? player2Slot1Pokemon ?? throw new InvalidOperationException()
                : player2Slot2Pokemon ?? throw new InvalidOperationException();

            // Both players are terastilizing. Determine order by speed.
            var teraSpeedOrder = CalculateSpeedOrder([(PlayerId.Player1, SlotId.Slot1),
            (PlayerId.Player2, SlotId.Slot1)]);

            if (teraSpeedOrder[0].Item1 == PlayerId.Player1)
            {
                player1Pokemon.Terastillize(Context);
                player2Pokemon.Terastillize(Context);
            }
            else
            {
                player2Pokemon.Terastillize(Context);
                player1Pokemon.Terastillize(Context);
            }

            player1Choice = player1Choice.ConvertMoveWithTeraToMove();
            Player1PendingChoice = player1Choice;

            player2Choice = player2Choice.ConvertMoveWithTeraToMove();
            Player2PendingChoice = player2Choice;
        }
        else if (player1Tera)
        {
            Pokemon player1Pokemon = player1Slot1Tera
                ? player1Slot1Pokemon ?? throw new InvalidOperationException()
                : player1Slot2Pokemon ?? throw new InvalidOperationException();

            player1Pokemon.Terastillize(Context);
            player1Choice = player1Choice.ConvertMoveWithTeraToMove();
            Player1PendingChoice = player1Choice;
        }
        else if (player2Tera)
        {
            Pokemon player2Pokemon = player2Slot1Tera
                ? player2Slot1Pokemon ?? throw new InvalidOperationException()
                : player2Slot2Pokemon ?? throw new InvalidOperationException();

            player2Pokemon.Terastillize(Context);
            player2Choice = player2Choice.ConvertMoveWithTeraToMove();
            Player2PendingChoice = player2Choice;
        }

        var speedOrder = MovesNext();

        List<PlayerId> executedPlayers = [];

        // Execute choices in priority order
        foreach ((PlayerId playerId, SlotId slotId) in speedOrder)
        {
            // TODO: Should perform speed checks after each action for dynamic speed changes
            switch (ExecutePlayerChoice(playerId, slotId))
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

    private MoveAction ExecutePlayerChoice(PlayerId playerId, SlotId slotId)
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

        var choice = GetPendingChoice(playerId, slotId);

        if (choice is null)
        {
            throw new InvalidOperationException($"No pending choice for {playerId} slot {slotId}.");
        }

        if (choice.Value.IsMoveChoice())
        {
            MoveAction moveAction = PerformMove(playerId, slotId);
            //SetPendingChoice(playerId, null);

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
        else if (choice.Value.IsSwitchChoice())
        {
            PerformSwitch(playerId, slotId);
            //SetPendingChoice(playerId, null);
            SetPlayerState(playerId, PlayerState.Idle);
        }
        else if (choice == Choice.Struggle)
        {
            PerformStruggle(playerId, slotId);
            //SetPendingChoice(playerId, null);
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
        List<(Pokemon, Condition, PlayerId)> allResidualsList = [];

        // Process all active Pokémon on both sides
        var allActivePokemon = new List<(Pokemon pokemon, PlayerId playerId)>();

        // Add Player 1's active Pokémon
        if (Side1.Team.Slot1Pokemon is { IsFainted: false })
        {
            allActivePokemon.Add((Side1.Team.Slot1Pokemon, PlayerId.Player1));
        }
        if (Side1.Team.Slot2Pokemon is { IsFainted: false })
        {
            allActivePokemon.Add((Side1.Team.Slot2Pokemon, PlayerId.Player1));
        }

        // Add Player 2's active Pokémon
        if (Side2.Team.Slot1Pokemon is { IsFainted: false })
        {
            allActivePokemon.Add((Side2.Team.Slot1Pokemon, PlayerId.Player2));
        }
        if (Side2.Team.Slot2Pokemon is { IsFainted: false })
        {
            allActivePokemon.Add((Side2.Team.Slot2Pokemon, PlayerId.Player2));
        }

        // Collect residual conditions from all active Pokémon
        foreach ((Pokemon pokemon, PlayerId playerId) in allActivePokemon)
        {
            var residuals = pokemon.GetAllResidualConditions();

            foreach (Condition condition in residuals)
            {
                if (condition.OnResidual != null)
                {
                    allResidualsList.Add((pokemon, condition, playerId));
                }
            }
        }

        // Combine and sort by OnResidualOrder
        var allResiduals = allResidualsList
            .OrderBy(t => t.Item2.OnResidualOrder ?? int.MaxValue)
            .ToList();

        // Execute residuals in order
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
        // Process all active Pokémon on both sides
        var allActivePokemon = new List<(Pokemon pokemon, PlayerId playerId)>();

        // Add Player 1's active Pokémon
        if (Side1.Team.Slot1Pokemon is { IsFainted: false })
        {
            allActivePokemon.Add((Side1.Team.Slot1Pokemon, PlayerId.Player1));
        }
        if (Side1.Team.Slot2Pokemon is { IsFainted: false })
        {
            allActivePokemon.Add((Side1.Team.Slot2Pokemon, PlayerId.Player1));
        }

        // Add Player 2's active Pokémon
        if (Side2.Team.Slot1Pokemon is { IsFainted: false })
        {
            allActivePokemon.Add((Side2.Team.Slot1Pokemon, PlayerId.Player2));
        }
        if (Side2.Team.Slot2Pokemon is { IsFainted: false })
        {
            allActivePokemon.Add((Side2.Team.Slot2Pokemon, PlayerId.Player2));
        }

        // Process turn-end conditions for each active Pokémon
        foreach ((Pokemon pokemon, PlayerId playerId) in allActivePokemon)
        {
            var conditions = pokemon.Conditions.ToList();

            foreach (Condition condition in conditions.ToList())
            {
                condition.OnTurnEnd?.Invoke(pokemon, Context);

                if (!condition.Duration.HasValue) continue;

                condition.Duration--;
                if (condition.Duration <= 0)
                {
                    pokemon.RemoveCondition(condition.Id);
                }
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

    private (PlayerId, SlotId)[] MovesNext()
    {
        int? player1Slot1Priority = Player1Slot1PendingChoice is not null ?
            Priority(Player1Slot1PendingChoice.Value, Side1, SlotId.Slot1) : null;
        int? player1Slot2Priority = Player1Slot2PendingChoice is not null ?
            Priority(Player1Slot2PendingChoice.Value, Side1, SlotId.Slot2) : null;
        int? player2Slot1Priority = Player2Slot1PendingChoice is not null ?
            Priority(Player2Slot1PendingChoice.Value, Side2, SlotId.Slot1) : null;
        int? player2Slot2Priority = Player2Slot2PendingChoice is not null ?
            Priority(Player2Slot2PendingChoice.Value, Side2, SlotId.Slot2) : null;

        var priorityList = new List<((PlayerId, SlotId) playerSlot, int priority)>();
        if (player1Slot1Priority.HasValue)
        {
            priorityList.Add(((PlayerId.Player1, SlotId.Slot1), player1Slot1Priority.Value));
        }
        if (player1Slot2Priority.HasValue)
        {
            priorityList.Add(((PlayerId.Player1, SlotId.Slot2), player1Slot2Priority.Value));
        }
        if (player2Slot1Priority.HasValue)
        {
            priorityList.Add(((PlayerId.Player2, SlotId.Slot1), player2Slot1Priority.Value));
        }
        if (player2Slot2Priority.HasValue)
        {
            priorityList.Add(((PlayerId.Player2, SlotId.Slot2), player2Slot2Priority.Value));
        }

        if (priorityList.Count == 0)
        {
            throw new InvalidOperationException("No pending move choices to determine move order.");
        }

        // Group by priority and sort each group by speed
        var orderedMoves = new List<(PlayerId, SlotId)>();

        // Get all unique priority values, sorted from highest to lowest
        var priorityTiers = priorityList
            .Select(t => t.priority)
            .Distinct()
            .OrderByDescending(p => p)
            .ToList();

        // Process each priority tier
        foreach (int priority in priorityTiers)
        {
            var movesInTier = priorityList
                .Where(t => t.priority == priority)
                .Select(t => t.playerSlot)
                .ToArray();

            if (movesInTier.Length == 1)
            {
                // Only one move at this priority level
                orderedMoves.Add(movesInTier[0]);
            }
            else
            {
                // Multiple moves at same priority - resolve by speed
                var speedOrder = CalculateSpeedOrder(movesInTier);
                orderedMoves.AddRange(speedOrder);
            }
        }

        return orderedMoves.ToArray();
    }

    //private (PlayerId, SlotId)[] CalculateSpeedOrder((PlayerId, SlotId)[] pokemon)
    //{
    //    // Create a list of active Pokemon and sort by speed
    //    var speedOrder = new List<Pokemon> { player1Pokemon, player2Pokemon }
    //        .OrderByDescending(p => p.CurrentSpe)
    //        .ToList();

    //    if (Field.HasPseudoWeather(PseudoWeatherId.TrickRoom))
    //    {
    //        speedOrder.Reverse(); // Reverse order if Trick Room is active
    //    }

    //    // Check if the fastest Pokemon is unique
    //    if (speedOrder[0].CurrentSpe != speedOrder[1].CurrentSpe)
    //    {
    //        return speedOrder[0] == player1Pokemon ? PlayerId.Player1 : PlayerId.Player2;
    //    }

    //    // If speeds are tied, decide randomly
    //    return BattleRandom.Next(2) == 0 ? PlayerId.Player1 : PlayerId.Player2;
    //}

    private (PlayerId, SlotId)[] CalculateSpeedOrder((PlayerId, SlotId)[] pokemonSlots)
    {
        // Get Pokemon objects for each slot and their speeds
        var pokemonWithSpeed = new List<(Pokemon pokemon, (PlayerId, SlotId) slot, int speed)>();

        foreach ((PlayerId playerId, SlotId slotId) in pokemonSlots)
        {
            Side side = GetSide(playerId);
            Pokemon? pokemon = side.Team.GetPokemon(slotId);

            if (pokemon is { IsFainted: false })
            {
                pokemonWithSpeed.Add((pokemon, (playerId, slotId), pokemon.CurrentSpe));
            }
        }

        if (pokemonWithSpeed.Count == 0)
        {
            throw new InvalidOperationException("No active Pokemon to determine speed order.");
        }

        // Sort by speed (highest first)
        var speedOrder = pokemonWithSpeed
            .OrderByDescending(p => p.speed)
            .ToList();

        // Reverse order if Trick Room is active (slowest goes first)
        if (Field.HasPseudoWeather(PseudoWeatherId.TrickRoom))
        {
            speedOrder.Reverse();
        }

        // Handle speed ties with randomization
        var finalOrder = new List<(PlayerId, SlotId)>();
        var currentGroup = new List<(Pokemon pokemon, (PlayerId, SlotId) slot, int speed)>();

        for (int i = 0; i < speedOrder.Count; i++)
        {
            currentGroup.Add(speedOrder[i]);

            // If this is the last Pokemon or the next Pokemon has different speed
            if (i != speedOrder.Count - 1 && speedOrder[i].speed == speedOrder[i + 1].speed) continue;
            // Randomize the current speed tier
            if (currentGroup.Count > 1)
            {
                // Shuffle Pokemon with the same speed
                for (int j = currentGroup.Count - 1; j > 0; j--)
                {
                    int randomIndex = BattleRandom.Next(j + 1);
                    (currentGroup[j], currentGroup[randomIndex]) = (currentGroup[randomIndex], currentGroup[j]);
                }
            }

            // Add to final order
            finalOrder.AddRange(currentGroup.Select(p => p.slot));
            currentGroup.Clear();
        }

        return finalOrder.ToArray();
    }

    private static int Priority(Choice choice, Side side, SlotId slot)
    {
        if (choice.IsSwitchChoice())
        {
            return 6;
        }
        if (choice.IsMoveChoice())
        {
            int moveIndex = choice.GetMoveNumber();
            Pokemon? attacker = side.Team.GetPokemon(slot);
            if (attacker is null)
            {
                throw new InvalidOperationException("Attacker cannot be null for move choice.");
            }
            Move move = attacker.Moves[moveIndex];
            int priority = move.Priority;
            // Check for abilities that modify move priority
            priority = attacker.Ability.OnModifyPriority?.Invoke(priority, move) ?? priority;
            return priority;
        }
        return 0; // Default priority for other choices
    }
}