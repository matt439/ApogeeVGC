using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.Utils.Extensions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleAsync
{
    private async Task ExecuteMoveChoiceAsync(PlayerId playerId, SlotChoice.MoveChoice choice)
    {
        Pokemon attacker = choice.Attacker;
        Move move = choice.Move;

        if (attacker.IsFainted)
        {
            throw new InvalidOperationException($"Cannot use move with fainted Pokémon {attacker.Name}" +
                                                $"for player {playerId}");
        }

        // Resolve actual targets using BattleCore
        Side attackingSide = GetSide(attacker.SideId);
        Side defendingSide = GetOpponentSide(attackingSide);
        var actualTargets = BattleCore.ResolveActualTargets(choice, attackingSide, defendingSide);

        if (actualTargets.Count == 0 && RequiresTargets(move))
        {
            if (PrintDebug)
            {
                Console.WriteLine($"Move {move.Name} has no valid targets");
            }
            return;
        }

        // Execute move using BattleCore
        var result = BattleCore.ExecuteMove(attacker, move, actualTargets, Context, playerId);

        if (!result.Success)
        {
            if (PrintDebug)
            {
                Console.WriteLine($"Move execution failed: {result.ErrorMessage}");
            }
            return;
        }

        // Print UI messages for successful move execution
        if (PrintDebug)
        {
            foreach (var targetResult in result.TargetResults)
            {
                if (targetResult.Hit)
                {
                    if (targetResult.Damage > 0)
                    {
                        Console.WriteLine($"{attacker.Name} used {move.Name} on {targetResult.Target.Name} for {targetResult.Damage} damage!");
                    }
                    else
                    {
                        Console.WriteLine($"{attacker.Name} used {move.Name} on {targetResult.Target.Name}!");
                    }
                }
                else
                {
                    Console.WriteLine($"{attacker.Name}'s {move.Name} missed {targetResult.Target.Name}!");
                }
            }
        }

        await Task.CompletedTask;
    }

    private static bool RequiresTargets(Move move)
    {
        return move.Target switch
        {
            MoveTarget.AllySide or MoveTarget.FoeSide or MoveTarget.Field => false,
            _ => true,
        };
    }

    // These methods are now handled by BattleCore but kept for UI purposes
    private void PerformDamagingMove(Pokemon attacker, Move move, Pokemon defender, int numDefendingSidePokemon)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        // Keeping it for backward compatibility but redirecting to BattleCore would be better
        throw new NotImplementedException("Use BattleCore.ExecuteMove instead");
    }

    private static bool IsSpreadMove(Move move)
    {
        return move.Target == MoveTarget.AllAdjacentFoes;
    }

    private void ApplyMoveCondition(Move move, Pokemon attacker, Pokemon target)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ExecuteMove instead");
    }

    private List<MoveTargetResult> ExecuteMoveAgainstTargets(Pokemon attacker, Move move, List<Pokemon> targets,
        PlayerId playerId)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ExecuteMove instead");
    }

    private MoveTargetResult ExecuteMoveAgainstSingleTarget(Pokemon attacker, Move move, Pokemon target,
        PlayerId playerId, int numPokemonDefendingSide)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ExecuteMove instead");
    }

    private void HandlePostMoveEffects(Pokemon attacker, Move move, List<MoveTargetResult> results)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ExecuteMove instead");
    }

    // Helper class to track move results - keeping for compatibility
    private class MoveTargetResult
    {
        public required Pokemon Target { get; init; }
        public int Damage { get; set; }
        public bool Hit { get; set; }
    }

    private void PerformStatusMove(Pokemon attacker, PlayerId playerId, Move move, Pokemon defender)
    {
        if (PrintDebug)
        {
            UiGenerator.PrintStatusMoveAction(attacker, move);
        }

        switch (move.Target)
        {
            case MoveTarget.Field:
                HandleFieldTargetStatusMove(move, attacker);
                return;
            case MoveTarget.AllySide:
                HandleSideTargetStatusMove(move, playerId, attacker);
                return;
            case MoveTarget.FoeSide:
                HandleSideTargetStatusMove(move, playerId.OpposingPlayerId(), attacker);
                return;
            case MoveTarget.AdjacentAlly:
            case MoveTarget.AdjacentAllyOrSelf:
            case MoveTarget.AdjacentFoe:
            case MoveTarget.All:
            case MoveTarget.AllAdjacent:
            case MoveTarget.AllAdjacentFoes:
            case MoveTarget.Allies:
            case MoveTarget.AllyTeam:
            case MoveTarget.Any:
            case MoveTarget.Normal:
            case MoveTarget.RandomNormal:
            case MoveTarget.Scripted:
            case MoveTarget.Self:
            case MoveTarget.None:
                break;
            default:
                throw new InvalidOperationException($"Invalid move target type {move.Target} for move {move.Name}" +
                                                $"used by player {playerId}");
        }

        if (move.Condition is null)
        {
            return;
        }

        move.OnHit?.Invoke(defender, attacker, move, Context);

        switch (move.Target)
        {
            case MoveTarget.Normal:
                defender.AddCondition(move.Condition, Context, attacker, move);
                break;
            case MoveTarget.Self:
                attacker.AddCondition(move.Condition, Context, attacker, move);
                break;
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

    private void HandleSideTargetStatusMove(Move move, PlayerId playerId, Pokemon attacker)
    {
        if (move.SideCondition is null)
        {
            throw new InvalidOperationException($"Status move {move.Name} has no side effect defined.");
        }

        SideCondition? condition = Field.GetSideCondition(move.SideCondition.Id, playerId);
        if (condition is not null)
        {
            // If the side condition is already present, reapply it (which may remove it)
            Field.ReapplySideCondition(condition.Id, GetSide(playerId), Context);
        }
        else // Otherwise, add the new side condition
        {
            Field.AddSideCondition(move.SideCondition, GetSide(playerId), attacker, move, Context);
        }
    }

    private void HandleFieldTargetStatusMove(Move move, Pokemon attacker)
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
                Field.ReapplyPseudoWeather(move.PseudoWeather.Id, AllActivePokemonArray, Context);
            }
            else // Otherwise, add the new pseudo-weather
            {
                Field.AddPseudoWeather(move.PseudoWeather, attacker, move, AllActivePokemonArray, Context);
            }
        }
        if (move.Weather is not null)
        {
            if (Field.HasWeather(move.Weather.Id)) // Reapply weather if it's the same one
            {
                Field.ReapplyWeather(AllActivePokemonArray, Context);
            }
            else if (Field.HasAnyWeather) // Replace existing weather
            {
                Field.RemoveWeather(AllActivePokemonArray, Context);
                Field.AddWeather(move.Weather, attacker, move, AllActivePokemonArray, Context);
            }
            else // No existing weather, just add the new one
            {
                Field.AddWeather(move.Weather, attacker, move, AllActivePokemonArray, Context);
            }
        }
        if (move.Terrain is not null)
        {
            if (Field.HasTerrain(move.Terrain.Id)) // Reapply terrain if it's the same one
            {
                Field.ReapplyTerrain(AllActivePokemonArray, Context);
            }
            else if (Field.HasAnyWeather) // Replace existing terrain
            {
                Field.ReapplyTerrain(AllActivePokemonArray, Context);
                Field.AddTerrain(move.Terrain, attacker, move, AllActivePokemonArray, Context);
            }
            else // No existing terrain, just add the new one
            {
                Field.AddTerrain(move.Terrain, attacker, move, AllActivePokemonArray, Context);
            }
        }
    }

    private bool IsMoveMiss(Pokemon attacker, Move move, Pokemon defender)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ExecuteMove instead");
    }

    private void PerformStruggle(PlayerId playerId)
    {
        Side atkSide = GetSide(playerId);
        Side defSide = GetSide(playerId.OpposingPlayerId());

        Pokemon attacker = atkSide.Slot1;

        if (!Library.Moves.TryGetValue(MoveId.Struggle, out Move? struggle))
        {
            throw new InvalidOperationException($"Struggle move not found in" +
                                                $"library for player {playerId}");
        }
        Pokemon defender = defSide.Slot1;

        // Use BattleCore to execute struggle move
        var targets = new List<Pokemon> { defender };
        var result = BattleCore.ExecuteMove(attacker, struggle, targets, Context, playerId);

        if (PrintDebug && result.Success)
        {
            var targetResult = result.TargetResults.FirstOrDefault();
            if (targetResult != null)
            {
                UiGenerator.PrintStruggleAction(attacker, targetResult.Damage, 
                    (int)(targetResult.Damage * 0.25), defender); // Struggle recoil is 25%
            }
        }
    }

    private int CalculateDamage(Pokemon attacker, Pokemon defender, Move move,
        double moveEffectiveness, bool crit = false, bool applyStab = true)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ExecuteMove instead");
    }

    private List<Pokemon> ResolveActualTargets(SlotChoice.MoveChoice choice)
    {
        // Use BattleCore for target resolution
        Side attackingSide = GetSide(choice.Attacker.SideId);
        Side defendingSide = GetOpponentSide(attackingSide);
        return BattleCore.ResolveActualTargets(choice, attackingSide, defendingSide);
    }

    private List<Pokemon> ResolveNormalTargets(SlotChoice.MoveChoice choice)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ResolveActualTargets instead");
    }

    private Pokemon? GetIntendedTarget(MoveNormalTarget targetType, Pokemon attacker)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ResolveActualTargets instead");
    }

    private List<Pokemon> GetFallbackTargets(MoveNormalTarget originalTarget, Pokemon attacker)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ResolveActualTargets instead");
    }

    private bool ExecutePreMoveChecks(Pokemon attacker, Move move, List<Pokemon> targets, PlayerId playerId)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ExecuteMove instead");
    }

    private List<Pokemon> GetAllAdjacentFoes(Pokemon attacker)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ResolveActualTargets instead");
    }

    private Pokemon? GetAdjacentAlly(Pokemon attacker)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ResolveActualTargets 대신");
    }

    /// <summary>
    /// Execute a switch choice using BattleCore
    /// </summary>
    private async Task ExecuteSwitchChoiceAsync(PlayerId playerId, SlotChoice.SwitchChoice choice)
    {
        Side side = GetSide(playerId);

        // Execute switch using BattleCore
        var result = BattleCore.ExecuteSwitch(
            side,
            choice.SwitchOutSlot,
            choice.SwitchInSlot,
            Field,
            AllActivePokemonArray,
            Context,
            playerId
        );

        if (!result.Success)
        {
            if (PrintDebug)
            {
                Console.WriteLine($"Switch execution failed: {result.ErrorMessage}");
            }
            throw new InvalidOperationException($"Switch failed: {result.ErrorMessage}");
        }

        if (PrintDebug && result.SwitchedOutPokemon != null && result.SwitchedInPokemon != null)
        {
            Console.WriteLine($"{result.SwitchedOutPokemon.Name} was switched out for {result.SwitchedInPokemon.Name}!");
        }

        await Task.CompletedTask;
    }

    #region Timeout Management (formerly BattleAsync.Timeout.cs)

    /// <summary>
    /// Check if game time limit has been exceeded
    /// </summary>
    private bool HasGameTimedOut()
    {
        return DateTime.UtcNow - _gameStartTime > GameTimeLimit;
    }

    /// <summary>
    /// Check if a player has exceeded their total time limit
    /// </summary>
    private bool HasPlayerTimedOut(PlayerId playerId)
    {
        var playerTime = GetPlayerTotalTime(playerId);
        return playerTime > PlayerTotalTimeLimit;
    }

    /// <summary>
    /// Handle game timeout - call tiebreak function
    /// </summary>
    private async Task HandleGameTimeoutAsync()
    {
        if (PrintDebug)
            Console.WriteLine("Game time limit exceeded, calling tiebreak");

        var winner = PerformTiebreak();
        await EndGameAsync(winner, GameEndReason.GameTimeout);
    }

    /// <summary>
    /// Handle player total time timeout - player loses immediately
    /// </summary>
    private async Task HandlePlayerTimeoutAsync(PlayerId playerId)
    {
        if (PrintDebug)
            Console.WriteLine($"Player {playerId} exceeded total time limit, loses the game");

        var winner = playerId == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1;
        await EndGameAsync(winner, GameEndReason.PlayerTimeout);
    }

    /// <summary>
    /// Handle turn limit reached
    /// </summary>
    private async Task HandleTurnLimitReachedAsync()
    {
        if (PrintDebug)
            Console.WriteLine($"Turn limit reached (turn {TurnCounter}), calling tiebreak");

        var winner = PerformTiebreak();
        await EndGameAsync(winner, GameEndReason.TurnLimit);
    }

    /// <summary>
    /// Perform tiebreak logic when game times out or turn limit is reached
    /// </summary>
    private PlayerId PerformTiebreak()
    {
        if (PrintDebug)
            Console.WriteLine("Performing tiebreak calculation...");

        // Tiebreak logic based on Pokemon Showdown rules:
        // 1. Player with more Pokemon remaining wins
        // 2. If tied, player with higher total HP percentage wins
        // 3. If tied, player with higher total HP wins
        // 4. If still tied, it's a true tie (pick randomly or declare draw)

        var side1Stats = CalculateSideStats(Side1);
        var side2Stats = CalculateSideStats(Side2);

        if (PrintDebug)
        {
            Console.WriteLine($"Player 1: {side1Stats.PokemonRemaining} Pokemon, {side1Stats.TotalHpPercentage:F1}% HP, {side1Stats.TotalHp} total HP");
            Console.WriteLine($"Player 2: {side2Stats.PokemonRemaining} Pokemon, {side2Stats.TotalHpPercentage:F1}% HP, {side2Stats.TotalHp} total HP");
        }

        // Compare Pokemon remaining
        if (side1Stats.PokemonRemaining > side2Stats.PokemonRemaining)
        {
            if (PrintDebug)
                Console.WriteLine("Player 1 wins tiebreak: more Pokemon remaining");
            return PlayerId.Player1;
        }
        if (side2Stats.PokemonRemaining > side1Stats.PokemonRemaining)
        {
            if (PrintDebug)
                Console.WriteLine("Player 2 wins tiebreak: more Pokemon remaining");
            return PlayerId.Player2;
        }

        // Compare HP percentage
        if (side1Stats.TotalHpPercentage > side2Stats.TotalHpPercentage)
        {
            if (PrintDebug)
                Console.WriteLine("Player 1 wins tiebreak: higher HP percentage");
            return PlayerId.Player1;
        }
        if (side2Stats.TotalHpPercentage > side1Stats.TotalHpPercentage)
        {
            if (PrintDebug)
                Console.WriteLine("Player 2 wins tiebreak: higher HP percentage");
            return PlayerId.Player2;
        }

        // Compare total HP
        if (side1Stats.TotalHp > side2Stats.TotalHp)
        {
            if (PrintDebug)
                Console.WriteLine("Player 1 wins tiebreak: higher total HP");
            return PlayerId.Player1;
        }
        if (side2Stats.TotalHp > side1Stats.TotalHp)
        {
            if (PrintDebug)
                Console.WriteLine("Player 2 wins tiebreak: higher total HP");
            return PlayerId.Player2;
        }

        // True tie - use random determination
        var randomWinner = BattleRandom.Next(2) == 0 ? PlayerId.Player1 : PlayerId.Player2;
        
        if (PrintDebug)
            Console.WriteLine($"True tie in tiebreak, random winner: {randomWinner}");

        return randomWinner;
    }

    /// <summary>
    /// Calculate statistics for a side for tiebreak purposes
    /// </summary>
    private SideStats CalculateSideStats(Side side)
    {
        var stats = new SideStats();
        var totalMaxHp = 0;

        foreach (var pokemon in side.AllSlots)
        {
            totalMaxHp += pokemon.UnmodifiedHp;

            if (!pokemon.IsFainted)
            {
                stats.PokemonRemaining++;
                stats.TotalHp += pokemon.CurrentHp;
            }
        }

        stats.TotalHpPercentage = totalMaxHp > 0 ? (double)stats.TotalHp / totalMaxHp * 100.0 : 0.0;

        return stats;
    }

    /// <summary>
    /// Check for timeout warnings and send notifications
    /// </summary>
    private async Task CheckAndSendTimeoutWarningsAsync()
    {
        // Check game time warning (at 2 minutes remaining)
        var gameTimeRemaining = GameTimeLimit - (DateTime.UtcNow - _gameStartTime);
        if (gameTimeRemaining <= TimeSpan.FromMinutes(2) && gameTimeRemaining > TimeSpan.FromMinutes(1.9))
        {
            if (PrintDebug)
                Console.WriteLine($"Game time warning: {gameTimeRemaining.TotalMinutes:F1} minutes remaining");
        }

        // Check player time warnings (at 1 minute remaining)
        var player1TimeRemaining = PlayerTotalTimeLimit - _player1TotalTime;
        if (player1TimeRemaining <= TimeSpan.FromMinutes(1) && player1TimeRemaining > TimeSpan.FromSeconds(50))
        {
            await Player1.NotifyTimeoutWarningAsync(player1TimeRemaining);
        }

        var player2TimeRemaining = PlayerTotalTimeLimit - _player2TotalTime;
        if (player2TimeRemaining <= TimeSpan.FromMinutes(1) && player2TimeRemaining > TimeSpan.FromSeconds(50))
        {
            await Player2.NotifyTimeoutWarningAsync(player2TimeRemaining);
        }
    }

    /// <summary>
    /// Get remaining time for a player
    /// </summary>
    public TimeSpan GetPlayerRemainingTime(PlayerId playerId)
    {
        var usedTime = GetPlayerTotalTime(playerId);
        var remaining = PlayerTotalTimeLimit - usedTime;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    /// <summary>
    /// Get remaining game time
    /// </summary>
    public TimeSpan GetGameRemainingTime()
    {
        var elapsed = DateTime.UtcNow - _gameStartTime;
        var remaining = GameTimeLimit - elapsed;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    #endregion
}

/// <summary>
/// Statistics for a side used in tiebreak calculations
/// </summary>
internal class SideStats
{
    public int PokemonRemaining { get; set; }
    public int TotalHp { get; set; }
    public double TotalHpPercentage { get; set; }
}