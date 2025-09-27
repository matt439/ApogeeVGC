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
        Side defendingSide = GetSide(attacker.SideId.GetOppositeSide());
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
        Side defendingSide = GetSide(choice.Attacker.SideId.GetOppositeSide());
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
        throw new NotImplementedException("Use BattleCore.ResolveActualTargets 대신");
    }

    private bool ExecutePreMoveChecks(Pokemon attacker, Move move, List<Pokemon> targets, PlayerId playerId)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ExecuteMove 대신");
    }

    private List<Pokemon> GetAllAdjacentFoes(Pokemon attacker)
    {
        // This method is now deprecated - BattleCore handles the actual logic
        throw new NotImplementedException("Use BattleCore.ResolveActualTargets 대신");
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
}