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
    FaintedSelect,
    FaintedLocked,
    //TagetSelect,
    //TargetLocked,
    // TODO: Implement these above states
    Idle,
}


public record BattleContext
{
    public required Library Library { get; init; }
    public required Random Random { get; init; }
    public bool PrintDebug { get; init; }
}

public class Battle
{
    public required Library Library { get; init; }
    public required Field Field { get; init; }
    public required Side Side1 { get; init; }
    public required Side Side2 { get; init; }
    public int Turn { get; private set; }
    public bool PrintDebug { get; set; }
    public int? BattleSeed { get; init; }
    public bool IsTeamPreview => Player1State == PlayerState.TeamPreviewSelect ||
                                 Player2State == PlayerState.TeamPreviewSelect ||
                                 Player1State == PlayerState.TeamPreviewLocked ||
                                 Player2State == PlayerState.TeamPreviewLocked;
    private BattleContext Context => new()
    {
        Library = Library,
        Random = BattleRandom,
        PrintDebug = PrintDebug,
    };
    private Pokemon[] AllActivePokemon => [Side1.Team.ActivePokemon, Side2.Team.ActivePokemon];
    private PlayerState Player1State { get; set; } = PlayerState.TeamPreviewSelect;
    private PlayerState Player2State { get; set; } = PlayerState.TeamPreviewSelect;
    private Choice? Player1PendingChoice { get; set; }
    private Choice? Player2PendingChoice { get; set; }
    private object ChoiceLock { get; } = new();
    
    // Lazy-initialized seeded random number generator for deterministic battle simulation
    private Random? _battleRandom;
    private Random BattleRandom => _battleRandom ??= BattleSeed.HasValue ?
        new Random(BattleSeed.Value) : new Random();
    

    /// <summary>
    /// Creates a deep copy of the battle state for MCTS simulation purposes.
    /// This method creates independent copies of all mutable state while sharing immutable references.
    /// </summary>
    /// <returns>A new Battle instance with copied state</returns>
    public Battle DeepCopy(bool? printDebug = null)
    {
        return new Battle
        {
            // Shared immutable references
            Library = Library, // Library is read-only, safe to share
            
            // Deep copy mutable components using their Copy methods
            Field = Field.Copy(),
            Side1 = Side1.Copy(),
            Side2 = Side2.Copy(),
            
            // Copy simple state
            Turn = Turn,
            Player1State = Player1State,
            Player2State = Player2State,
            Player1PendingChoice = Player1PendingChoice,
            Player2PendingChoice = Player2PendingChoice,
            PrintDebug = printDebug ?? PrintDebug,
            BattleSeed = BattleSeed,
            // Note: ChoiceLock gets a new instance automatically
            // Note: _battleRandom will be initialized with the same seed when first accessed
        };
    }

    /// <summary>
    /// Creates a deep copy of the battle and applies a choice to it.
    /// This is the main method used by MCTS for creating child nodes.
    /// </summary>
    /// <param name="playerId">The player making the choice</param>
    /// <param name="choice">The choice to apply</param>
    /// <param name="printDebug">Manyually set debug printing</param>
    /// <returns>A new Battle instance with the choice applied</returns>
    public Battle DeepCopyAndApplyChoice(PlayerId playerId, Choice choice, bool? printDebug = null)
    {
        Battle copy = DeepCopy(printDebug);
        
        try
        {
            // Apply the choice to the copied battle
            copy.SubmitChoice(playerId, choice);
            return copy;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to apply choice {choice} for player {playerId}: {ex.Message}", ex);
        }
    }

    public void SubmitChoice(PlayerId playerId, Choice choice)
    {
        lock (ChoiceLock)
        {
            bool incrementTurn = true;

            CheckForChoiceError(playerId, choice);

            SetPendingChoice(playerId, choice);

            if (IsReadyForTeamPreviewProcessing())
            {
                PerformTeamPreviewSelect(Player1PendingChoice ??
                    throw new ArgumentException("Player1PendingChoice cannot be null"),
                    Player2PendingChoice ??
                        throw new ArgumentException("Player2PendingChoice cannot be null"));
                ClearPendingChoices();

                // Don't increment turn after team preview processing
                incrementTurn = false;
            }
            else if (IsReadyForMoveSwitchProcessing())
            {
                PerformMoveSwitches(Player1PendingChoice ??
                    throw new ArgumentException("Player1PendingChoice cannot be null"),
                        Player2PendingChoice ??
                            throw new ArgumentException("Player2PendingChoice cannot be null"));
                ClearPendingChoices();
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

            if (!IsEndOfTurn()) return;

            HandleEndOfTurn();

            if (incrementTurn)
            { 
                Turn++;
            }

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

    private void HandleEndOfTurn()
    {
        UiGenerator.PrintBlankLine();
        Field.OnTurnEnd(AllActivePokemon, Context);
        HandleResiduals();
        HandleConditionTurnEnds();
    }

    private void HandleConditionTurnEnds()
    {
        //// get all conditions with OnTurnEnd from both active Pokemon
        //var side1Conditions = Side1.Team.ActivePokemon.GetAllConditionsWithTurnEnd();
        //var side2Conditions = Side2.Team.ActivePokemon.GetAllConditionsWithTurnEnd();

        //foreach (Condition condition in side1Conditions)
        //{
        //    condition.OnTurnEnd?.Invoke(Side1.Team.ActivePokemon, Context);
        //}

        //foreach (Condition condition in side2Conditions)
        //{
        //    condition.OnTurnEnd?.Invoke(Side2.Team.ActivePokemon, Context);
        //}

        var side1Conditions = Side1.Team.ActivePokemon.Conditions.ToList();
        foreach (Condition condition in side1Conditions)
        {
            condition.OnTurnEnd?.Invoke(Side1.Team.ActivePokemon, Context);
            if (!condition.Duration.HasValue) continue;
            condition.Duration--;
            if (condition.Duration <= 0)
            {
                Side1.Team.ActivePokemon.RemoveCondition(condition.Id);
            }
        }

        var side2Conditions = Side2.Team.ActivePokemon.Conditions.ToList();
        foreach (Condition condition in side2Conditions)
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

    private void HandleResiduals()
    {
        var side1Residuals = Side1.Team.ActivePokemon.GetAllResidualConditions();
        List<(Pokemon, Condition, PlayerId)> side1ResidualsList = [];
        foreach (Condition condition in side1Residuals)
        {
            if (condition.OnResidual != null)
            {
                side1ResidualsList.Add((Side1.Team.ActivePokemon, condition, PlayerId.Player1));
            }
        }

        var side2Residuals = Side2.Team.ActivePokemon.GetAllResidualConditions();
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

        UpdatePlayerState(playerId, choice);
    }

    private void UpdatePlayerState(PlayerId playerId, Choice? choice)
    {
        if (choice is null) return; // No choice to update state with

        PlayerState playerState = GetPlayerState(playerId);

        switch (playerState)
        {
            case PlayerState.MoveSwitchSelect:
                if (choice.Value.IsSwitchChoice() || choice.Value.IsMoveChoice() ||
                    choice == Choice.Struggle)
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
                if (choice.Value.IsSwitchChoice())
                {
                    SetPlayerState(playerId, PlayerState.FaintedLocked);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Invalid choice for SwitchSelect: {choice}");
                }
                break;
            case PlayerState.TeamPreviewSelect:
                if (choice.Value.IsSelectChoice())
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
                throw new InvalidOperationException(
                    $"Player {playerId} is already locked in state: {playerState}");
            case PlayerState.Idle:
                throw new InvalidOperationException("Player cannot submit choice in Idle state.");
            default:
                throw new ArgumentOutOfRangeException(nameof(playerState), playerState, null);
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
            ExecutePlayerChoice(playerId, choice);
        }
    }

    private void ExecutePlayerChoice(PlayerId playerId, Choice choice)
    {
        PlayerState playerState = GetPlayerState(playerId);

        // This occurs when a player's pokemon faints and they need to select a new one
        // Their previous choice isn't valid anymore as it was based on the now-fainted pokemon
        if (playerState == PlayerState.FaintedSelect)
        {
            return;
        }

        if (choice.IsMoveChoice())
        {
            PerformMove(playerId, choice);
            SetPendingChoice(playerId, null);
            SetPlayerState(playerId, PlayerState.Idle);

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
    }

    private void UpdateFaintedStates()
    {
        if (Side1.Team.ActivePokemon.IsFainted)
        {
            Player1State = PlayerState.FaintedSelect;
            if (PrintDebug)
            {
                UiGenerator.PrintFaintedAction(Side1.Team.ActivePokemon);
            }
        }
        if (Side2.Team.ActivePokemon.IsFainted)
        {
            Player2State = PlayerState.FaintedSelect;
            if (PrintDebug)
            {
                UiGenerator.PrintFaintedAction(Side2.Team.ActivePokemon);
            }
        }
    }

    private void ClearPendingChoices()
    {
        Player1PendingChoice = null;
        Player2PendingChoice = null;
    }

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

    private void PerformMove(PlayerId playerId, Choice choice)
    {
        Side atkSide = GetSide(playerId);
        Side defSide = GetSide(playerId.OpposingPlayerId());

        int moveIndex = choice.GetMoveIndexFromChoice();
        Pokemon attacker = atkSide.Team.ActivePokemon;
        Move move = attacker.Moves[moveIndex];
        if (move.Pp <= 0)
        {
            throw new InvalidOperationException($"Move {move.Name} has no PP left" +
                                                $"for player {playerId}");
        }
        move.UsedPp++;  // Decrease PP for the move used

        if (move.StallingMove)
        {
            // check for conditions with OnStallMove on attacker
            foreach (Condition condition in attacker.Conditions.ToList())
            {
                if (condition.OnStallMove == null || condition.OnStallMove(attacker, Context)) continue;
                if (PrintDebug)
                {
                    UiGenerator.PrintMoveFailAction(attacker, move);
                }
                return;
            }
        }

        Pokemon defender = defSide.Team.ActivePokemon;

        if (IsMoveMiss(attacker, move, defender))
        {
            if (PrintDebug)
            {
                UiGenerator.PrintMoveMissAction(attacker, move, defender);
            }
            return;
        }

        if (move.OnTryImmunity != null && move.OnTryImmunity(defender) ||
            move.OnPrepareHit?.Invoke(defender, attacker, move, Context) == false)
        {
            if (PrintDebug)
            {
                UiGenerator.PrintMoveNoEffectAction(attacker, move, defender);
            }
            return;
        }

        // check every condition on defender for OnTryHit effects
        foreach (Condition condition in defender.Conditions)
        {
            if (condition.OnTryHit == null || condition.OnTryHit(defender, attacker, move, Context)) continue;
            if (PrintDebug)
            {
                UiGenerator.PrintMoveNoEffectAction(attacker, move, defender);
            }
            return;
        }

        switch (move.Category)
        {
            case MoveCategory.Physical:
            case MoveCategory.Special:
                PerformDamagingMove(attacker, move, defender);
                break;
            case MoveCategory.Status:
                PerformStatusMove(attacker, move, defender);
                break;
            default:
                throw new InvalidOperationException($"Invalid move category for move {move.Name}: {move.Category}");
        }
    }

    private void PerformDamagingMove(Pokemon attacker, Move move, Pokemon defender)
    {
        bool isCrit = BattleRandom.NextDouble() < 1.0 / 16.0; // 1 in 16 chance of critical hit
        MoveEffectiveness effectiveness = Library.TypeChart.GetMoveEffectiveness(
            defender.Specie.Types, move.Type);
        int damage = Damage(attacker, defender, move, effectiveness.GetMultiplier(), isCrit);
        move.OnHit?.Invoke(defender, attacker, move, Context);
        defender.Damage(damage);
        
        if (PrintDebug)
        {
            UiGenerator.PrintDamagingMoveAction(attacker, move, damage, defender, effectiveness, isCrit);
        }
    }

    private void PerformStatusMove(Pokemon attacker, Move move, Pokemon defender)
    {
        if (move.Target == MoveTarget.Field)
        {
            HandleFieldTargetStatusMove(move);
            return;
        }

        if (move.Condition is null)
        {
            return;
        }

        if (PrintDebug)
        {
            UiGenerator.PrintStatusMoveAction(attacker, move);
        }

        move.OnHit?.Invoke(defender, attacker, move, Context);

        switch (move.Target)
        {
            case MoveTarget.Normal:
                defender.AddCondition(move.Condition, Context);
                break;
            case MoveTarget.Self:
                attacker.AddCondition(move.Condition, Context);
                break;
            //case MoveTarget.Field:
            //    HandleFieldTargetStatusMove(move);
            //    break;
            case MoveTarget.AdjacentAlly:
            case MoveTarget.AdjacentAllyOrSelf:
            case MoveTarget.AdjacentFoe:
            case MoveTarget.All:
            case MoveTarget.AllAdjacent:
            case MoveTarget.AllAdjacentFoes:
            case MoveTarget.Allies:
            case MoveTarget.AllySide:
            case MoveTarget.AllyTeam:
            case MoveTarget.Any:
            case MoveTarget.FoeSide:
            case MoveTarget.RandomNormal:
            case MoveTarget.Scripted:
            case MoveTarget.None:
                throw new NotImplementedException();
            case MoveTarget.Field:
                throw new InvalidOperationException("Field target should be handled separately");
            default:
                throw new ArgumentOutOfRangeException();
        }

        
    }

    private void HandleFieldTargetStatusMove(Move move)
    {
        if (move.PseudoWeather is null && move.Weather is null && move.Terrain is null)
        {
            throw new InvalidOperationException($"Status move {move.Name} has no field effect defined.");
        }

        if (move.PseudoWeather is not null)
        {
            // If the pseudo-weather is already present, reapply it (which may remove it)
            if (Field.HasPseudoWeather(move.PseudoWeather.Id))
            {
                Field.ReapplyPseudoWeather(move.PseudoWeather.Id, AllActivePokemon, Context);
            }
            else // Otherwise, add the new pseudo-weather
            {
                Field.AddPseudoWeather(move.PseudoWeather, AllActivePokemon, Context);
            }
        }
        if (move.Weather is not null)
        {
            if (Field.HasWeather(move.Weather.Id)) // Reapply weather if it's the same one
            {
                Field.ReapplyWeather(AllActivePokemon, Context);
            }
            else if (Field.HasAnyWeather) // Replace existing weather
            {
                Field.RemoveWeather(AllActivePokemon, Context);
                Field.AddWeather(move.Weather, AllActivePokemon, Context);
            }
            else // No existing weather, just add the new one
            {
                Field.AddWeather(move.Weather, AllActivePokemon, Context);
            }
        }
        if (move.Terrain is not null)
        {
            if (Field.HasTerrain(move.Terrain.Id)) // Reapply terrain if it's the same one
            {
                Field.ReapplyTerrain(AllActivePokemon, Context);
            }
            else if (Field.HasAnyWeather) // Replace existing terrain
            {
                Field.ReapplyTerrain(AllActivePokemon, Context);
                Field.AddTerrain(move.Terrain, AllActivePokemon, Context);
            }
            else // No existing terrain, just add the new one
            {
                Field.AddTerrain(move.Terrain, AllActivePokemon, Context);
            }
        }
    }

    private bool IsMoveMiss(Pokemon attacker, Move move, Pokemon defender)
    {
        if (move.AlwaysHit)
        {
            return false; // Move always hits
        }

        // get move accuracy
        int moveAccuracy = move.Accuracy;

        // get attacker accuracy stage
        double attackerAccuracyStage = attacker.StatModifiers.AccuracyMultiplier;

        // get defender evasion stage
        double defenderEvasionStage = defender.StatModifiers.EvasionMultiplier;

        // Calculate modified accuracy
        double modifiedAccuracy = moveAccuracy * attackerAccuracyStage * defenderEvasionStage;

        // generate random between 1 and 100
        int roll = BattleRandom.Next(1, 101);

        // move hits if roll is less than or equal to modified accuracy
        return !(roll <= modifiedAccuracy);
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
        prevActive.OnSwitchOut();
        side.Team.ActivePokemonIndex = choice.GetSwitchIndexFromChoice();
        Pokemon newActive = side.Team.ActivePokemon;
        Field.OnPokemonSwitchIn(newActive, Context);
        newActive.OnSwitchIn();

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
            case PlayerState.TeamPreviewSelect:
            case PlayerState.TeamPreviewLocked:
            case PlayerState.MoveSwitchSelect:
            case PlayerState.FaintedSelect:
            case PlayerState.Idle:
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

    private void PerformStruggle(PlayerId playerId)
    {
        Side atkSide = GetSide(playerId);
        Side defSide = GetSide(playerId.OpposingPlayerId());

        Pokemon attacker = atkSide.Team.ActivePokemon;

        if (!Library.Moves.TryGetValue(MoveId.Struggle, out Move? move))
        {
            throw new InvalidOperationException($"Struggle move not found in" +
                                                $"library for player {playerId}");
        }

        Pokemon defender = defSide.Team.ActivePokemon;
        int damage = Damage(attacker, defender, move, 1.0);
        defender.Damage(damage);

        // Struggle always deals recoil damage to the attacker
        // The recoil is 1/4 of the damage dealt, rounded down at half
        // but at least 1 damage
        int recoil = Math.Max(RoundedDownAtHalf(damage / 4.0), 1);
        attacker.Damage(recoil);

        if (PrintDebug)
        {
            UiGenerator.PrintStruggleAction(attacker, damage, recoil, defender);
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


    private int Damage(Pokemon attacker, Pokemon defender, Move move,
        double moveEffectiveness, bool crit = false)
    {
        int level = attacker.Level;
        int attackStat = attacker.GetAttackStat(move);
        int defenseStat = defender.GetDefenseStat(move);
        int basePower = move.BasePower;
        double critModifier = crit ? 1.5 : 1.0;
        double random = 0.85 + BattleRandom.NextDouble() * 0.15; // Random factor between 0.85 and 1.0
        bool stab = attacker.IsStab(move);
        double stabModifier = stab ? 1.5 : 1.0;

        int baseDamage = (int)((2 * level / 5.0 + 2) * basePower * attackStat / defenseStat / 50.0 + 2);
        int critMofified = RoundedDownAtHalf(critModifier * baseDamage);
        int randomModified = RoundedDownAtHalf(random * critMofified);
        int stabModified = RoundedDownAtHalf(stabModifier * randomModified);
        int typeModified = RoundedDownAtHalf(moveEffectiveness * stabModified);
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
            return move.Priority;
        }
        return 0; // Default priority for other choices
    }

    private Choice[] GetAvailableChoices(Side side)
    {
        // TODO: Implement logic to determine available choices based on the
        // current state of the battle.

        PlayerId playerId = side.PlayerId;

        PlayerState playerState = GetPlayerState(playerId);

        switch (playerState)
        {
            case PlayerState.TeamPreviewSelect:
                return GetTeamPreviewChoices(side);
            case PlayerState.MoveSwitchSelect:
                return GetMoveSwitchChoices(side);
            case PlayerState.FaintedSelect:
                return GetSwitchChoices(side);
            case PlayerState.TeamPreviewLocked:
            case PlayerState.MoveSwitchLocked:
            case PlayerState.FaintedLocked:
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

    private static Choice[] GetMoveSwitchChoices(Side side)
    {
        // TODO: Implement logic to determine available moves
        // based on the current state of the battle.
        
        Choice[] choices = [];
        for (int i = 0; i < side.Team.ActivePokemon.Moves.Length; i++)
        {
            // Check if the move is available (has PP left)
            if (side.Team.ActivePokemon.Moves[i].Pp > 0)
            {
                choices = choices.Append(i.GetChoiceFromMoveIndex()).ToArray();
            }
            // TODO: Check for choice lock, encore, disable, imprison, taunt,
            // torment, assault vest, etc.
        }

        if (choices.Length == 0)
        {
            // If no moves are available, Stuggle is the only option
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
                (PlayerState.FaintedSelect or PlayerState.MoveSwitchSelect))
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
            PlayerState.FaintedSelect => true,
            PlayerState.FaintedLocked => false,
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
    public static Battle GenerateTestBattle(Library library, string trainerName1,
        string trainerName2, bool printDebug = false, int? seed = null)
    {
        return new Battle
        {
            Library = library,
            Field = new Field(),
            Side1 = SideGenerator.GenerateTestSide(library, trainerName1, PlayerId.Player1, printDebug),
            Side2 = SideGenerator.GenerateTestSide(library, trainerName2, PlayerId.Player2, printDebug),
            PrintDebug = printDebug,
            BattleSeed = seed,
        };
    }
}