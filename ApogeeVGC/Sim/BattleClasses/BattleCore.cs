using ApogeeVGC.Data;
using ApogeeVGC.Player;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Extensions;
using static ApogeeVGC.Sim.Choices.SlotChoice;

namespace ApogeeVGC.Sim.BattleClasses;

/// <summary>
/// Core battle logic extracted from BattleAsync for synchronous battle operations.
/// Contains pure battle mechanics without async/await, events, or UI concerns.
/// Used by both BattleAsync (for human players) and BattleSync (for MCTS simulations).
/// </summary>
public static class BattleCore
{
    #region Constants
    
    private const int TurnLimit = 250;
    private const double Epsilon = 1e-10;
    
    #endregion

    #region Battle State Operations

    /// <summary>
    /// Check if the battle has reached game end conditions
    /// </summary>
    public static bool CheckForGameEndConditions(Side side1, Side side2)
    {
        return IsAllPokemonFainted(side1) || IsAllPokemonFainted(side2);
    }

    /// <summary>
    /// Determine the winner of the battle
    /// </summary>
    public static PlayerId? DetermineWinner(Side side1, Side side2)
    {
        bool side1AllFainted = IsAllPokemonFainted(side1);
        bool side2AllFainted = IsAllPokemonFainted(side2);

        if (side1AllFainted && side2AllFainted)
        {
            // Tie scenario - could implement tiebreak logic here
            return null;
        }

        if (side1AllFainted)
            return side2.PlayerId;
        
        if (side2AllFainted)
            return side1.PlayerId;

        return null; // Game not over yet
    }

    /// <summary>
    /// Check if all Pokemon on a side are fainted
    /// </summary>
    public static bool IsAllPokemonFainted(Side side)
    {
        return side.AllSlots.All(pokemon => pokemon.IsFainted);
    }

    /// <summary>
    /// Check if battle has exceeded turn limit
    /// </summary>
    public static bool HasExceededTurnLimit(int turnCounter)
    {
        return turnCounter >= TurnLimit;
    }

    #endregion

    #region Move Execution

    /// <summary>
    /// Execute a move choice synchronously
    /// </summary>
    public static MoveExecutionResult ExecuteMove(Pokemon attacker, Move move, List<Pokemon> targets, 
        IBattle context, PlayerId playerId)
    {
        var result = new MoveExecutionResult();

        if (attacker.IsFainted)
        {
            result.Success = false;
            result.ErrorMessage = $"Cannot use move with fainted Pokémon {attacker.Name}";
            return result;
        }

        // Pre-move setup
        attacker.ActiveMoveActions++;
        attacker.LastMoveUsed = move;

        // Execute pre-move checks
        if (!ExecutePreMoveChecks(attacker, move, targets, context))
        {
            result.Success = false;
            result.ErrorMessage = "Pre-move checks failed";
            return result;
        }

        move.UsedPp++;

        // Execute the move against all targets
        var moveResults = ExecuteMoveAgainstTargets(attacker, move, targets, context, playerId);
        result.TargetResults = moveResults;

        // Handle post-move effects
        HandlePostMoveEffects(attacker, move, moveResults, context);

        result.Success = true;
        return result;
    }

    /// <summary>
    /// Execute a switch choice synchronously
    /// </summary>
    public static SwitchExecutionResult ExecuteSwitch(Side side, SlotId switchOutSlot, SlotId switchInSlot, 
        Field field, Pokemon[] allActivePokemon, IBattle context, PlayerId playerId)
    {
        var result = new SwitchExecutionResult();

        Pokemon prevActive = switchOutSlot == SlotId.Slot1 ? side.Slot1 : side.Slot2;
        Pokemon newActive = switchInSlot == SlotId.Slot1 ? side.Slot1 : side.Slot2;

        if (prevActive.IsFainted && !newActive.IsFainted)
        {
            // Valid switch - switching out fainted Pokemon
        }
        else if (!prevActive.IsFainted && !newActive.IsFainted)
        {
            // Valid switch - voluntary switch
        }
        else
        {
            result.Success = false;
            result.ErrorMessage = "Invalid switch: cannot switch to fainted Pokemon";
            return result;
        }

        // Execute the switch
        side.SwitchSlots(switchOutSlot, switchInSlot);

        result.Success = true;
        result.SwitchedOutPokemon = prevActive;
        result.SwitchedInPokemon = newActive;
        return result;
    }

    #endregion

    #region Move Resolution

    private static List<MoveTargetResult> ExecuteMoveAgainstTargets(Pokemon attacker, Move move, 
        List<Pokemon> targets, IBattle context, PlayerId playerId)
    {
        var results = new List<MoveTargetResult>();

        switch (move.Target)
        {
            case MoveTarget.AllySide:
            case MoveTarget.FoeSide:
            case MoveTarget.Field:
                // Handle side/field effects
                if (move.Category == MoveCategory.Status)
                {
                    PerformStatusMove(attacker, move, attacker, context); // defender not used for these
                }
                results.Add(new MoveTargetResult { Target = attacker, Damage = 0, Hit = true });
                break;

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
                // Handle individual target effects
                int numDefendingSidePokemon = targets.Count(target => target.SideId != attacker.SideId);
                results.AddRange(targets.Select(target =>
                    ExecuteMoveAgainstSingleTarget(attacker, move, target, numDefendingSidePokemon, context)));
                break;

            case MoveTarget.None:
            default:
                throw new InvalidOperationException($"Invalid move target type {move.Target} for move {move.Name}");
        }

        return results;
    }

    private static MoveTargetResult ExecuteMoveAgainstSingleTarget(Pokemon attacker, Move move, Pokemon target,
        int numPokemonDefendingSide, IBattle context)
    {
        var result = new MoveTargetResult { Target = target, Damage = 0, Hit = false };

        // Miss check per target
        if (IsMoveMiss(attacker, move, target, context.Random))
        {
            return result;
        }

        result.Hit = true;

        // Execute the move effect
        switch (move.Category)
        {
            case MoveCategory.Physical:
            case MoveCategory.Special:
                int initialHp = target.CurrentHp;
                PerformDamagingMove(attacker, move, target, numPokemonDefendingSide, context);
                result.Damage = initialHp - target.CurrentHp;
                break;
            case MoveCategory.Status:
                PerformStatusMove(attacker, move, target, context);
                break;
            default:
                throw new InvalidOperationException($"Invalid move category for move {move.Name}: {move.Category}");
        }

        return result;
    }

    private static void PerformDamagingMove(Pokemon attacker, Move move, Pokemon defender, 
        int numDefendingSidePokemon, IBattle context)
    {
        bool isCrit = context.Random.NextDouble() < 1.0 / 16.0; // 1 in 16 chance of critical hit
        MoveEffectiveness effectiveness = context.Library.TypeChart.GetMoveEffectiveness(
            defender.DefensiveTypes, move.Type);

        if (effectiveness == MoveEffectiveness.Immune)
        {
            return;
        }

        // Calculate base damage
        int damage = CalculateDamage(attacker, defender, move, effectiveness.GetMultiplier(), isCrit, context);

        // Apply spread move damage reduction for multi-target moves
        if (numDefendingSidePokemon > 1 && IsSpreadMove(move))
        {
            damage = RoundedDownAtHalf(damage * 0.75); // 25% damage reduction for spread moves
        }

        // Check for OnAnyModifyDamage conditions on defender
        if (damage > 0)
        {
            double multiplier = 1.0;
            damage = Math.Max(1, (int)(damage * multiplier)); // Always at least 1 damage
        }

        int actualDefenderDamage = defender.Damage(damage);

        // Check for move condition application
        if (move.Condition is not null)
        {
            ApplyMoveCondition(move, attacker, defender, context);
        }

        // Check if defender fainted
        if (defender.IsFainted)
        {
            // Chilling neigh and similar abilities
            //attacker.Ability.OnSourceAfterFaint?.Invoke(1, defender, attacker, move, context);
        }
    }

    private static void PerformStatusMove(Pokemon attacker, Move move, Pokemon defender, IBattle context)
    {
        if (move.Condition is null)
        {
            return;
        }

        switch (move.Target)
        {
            case MoveTarget.Normal:
                //defender.AddCondition(move.Condition, context, attacker, move);
                break;
            case MoveTarget.Self:
                //attacker.AddCondition(move.Condition, context, attacker, move);
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

    #endregion

    #region Utility Methods

    private static bool ExecutePreMoveChecks(Pokemon attacker, Move move, List<Pokemon> targets, IBattle context)
    {
        // Item modifications (using first target for single-target effects)
        Pokemon primaryTarget = targets.FirstOrDefault() ?? attacker;

        if (move.Pp <= 0 && move.Id != MoveId.Struggle)
        {
            return false;
        }

        if (move.Disabled)
        {
            return false;
        }

        return true;
    }

    private static void HandlePostMoveEffects(Pokemon attacker, Move move, List<MoveTargetResult> results, IBattle context)
    {
        // Calculate total damage dealt for recoil moves
        int totalDamage = results.Where(r => r.Hit).Sum(r => r.Damage);

        // Handle recoil - calculated once per move based on total damage
        if (move.Recoil is not null && totalDamage > 0)
        {
            int recoilDamage = (int)(totalDamage * move.Recoil.Value);
            attacker.Damage(recoilDamage);
        }

        // Handle other post-move effects here (life orb, etc.)
    }

    private static void ApplyMoveCondition(Move move, Pokemon attacker, Pokemon target, IBattle context)
    {
        // Apply condition based on move target
        switch (move.Target)
        {
            case MoveTarget.Normal:
            case MoveTarget.AllAdjacentFoes:
            case MoveTarget.AdjacentAlly:
                //target.AddCondition(move.Condition!, context, attacker, move);
                break;
            case MoveTarget.Self:
                //attacker.AddCondition(move.Condition!, context, attacker, move);
                break;
            case MoveTarget.AdjacentAllyOrSelf:
            case MoveTarget.AdjacentFoe:
            case MoveTarget.All:
            case MoveTarget.AllAdjacent:
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

    private static bool IsMoveMiss(Pokemon attacker, Move move, Pokemon defender, Random random)
    {
        if (move.AlwaysHit)
        {
            return false; // Move always hits
        }

        // Get move accuracy
        int moveAccuracy = move.Accuracy;

        // Get attacker accuracy stage
        double attackerAccuracyStage = attacker.StatModifiers.AccuracyMultiplier;

        // Get defender evasion stage
        double defenderEvasionStage = defender.StatModifiers.EvasionMultiplier;

        // Calculate modified accuracy
        double modifiedAccuracy = moveAccuracy * attackerAccuracyStage * defenderEvasionStage;

        // Generate random between 1 and 100
        int roll = random.Next(1, 101);

        // Move hits if roll is less than or equal to modified accuracy
        return !(roll <= modifiedAccuracy);
    }

    private static int CalculateDamage(Pokemon attacker, Pokemon defender, Move move,
        double moveEffectiveness, bool crit, IBattle context, bool applyStab = true)
    {
        if (Math.Abs(moveEffectiveness) < Epsilon)
        {
            return 0; // No damage if immune
        }

        int level = attacker.Level;
        int attackStat = attacker.GetAttackStat(move, crit);
        int defenseStat = defender.GetDefenseStat(move, crit);
        int basePower = move.BasePower;

        double onBasePowerModifier = 1.0;
        if (Math.Abs(onBasePowerModifier - 1.0) > Epsilon)
        {
            basePower = (int)(basePower * onBasePowerModifier);
        }

        double critModifier = crit ? 1.5 : 1.0;
        double random = 0.85 + context.Random.NextDouble() * 0.15; // Random factor between 0.85 and 1.0
        double stabModifier = applyStab ? attacker.GetStabMultiplier(move) : 1.0;

        double burnModifier = 1.0;

        int baseDamage = (int)((2 * level / 5.0 + 2) * basePower * attackStat / defenseStat / 50.0 + 2);
        int critModified = RoundedDownAtHalf(critModifier * baseDamage);
        int randomModified = RoundedDownAtHalf(random * critModified);
        int stabModified = RoundedDownAtHalf(stabModifier * randomModified);
        int typeModified = RoundedDownAtHalf(moveEffectiveness * stabModified);
        int burnModified = RoundedDownAtHalf(burnModifier * typeModified);
        return Math.Max(1, burnModified);
    }

    private static bool IsSpreadMove(Move move)
    {
        return move.Target == MoveTarget.AllAdjacentFoes;
    }

    private static int RoundedDownAtHalf(double value)
    {
        return (int)(value + 0.5 - double.Epsilon);
    }

    #endregion

    #region Target Resolution

    /// <summary>
    /// Resolve actual targets for a move choice
    /// </summary>
    public static List<Pokemon> ResolveActualTargets(MoveChoice choice, Side attackingSide, Side defendingSide)
    {
        Move move = choice.Move;
        Pokemon attacker = choice.Attacker;
        var targets = new List<Pokemon>();

        switch (move.Target)
        {
            case MoveTarget.Normal:
                targets.AddRange(ResolveNormalTargets(choice, attackingSide, defendingSide));
                break;

            case MoveTarget.AllAdjacentFoes:
                targets.AddRange(GetAllAdjacentFoes(attacker, defendingSide));
                break;

            case MoveTarget.Self:
                targets.Add(attacker);
                break;

            case MoveTarget.AdjacentAlly:
                Pokemon? ally = GetAdjacentAlly(attacker, attackingSide);
                if (ally != null) targets.Add(ally);
                break;

            case MoveTarget.AllySide:
            case MoveTarget.FoeSide:
            case MoveTarget.Field:
                // These don't target specific Pokemon
                break;

            case MoveTarget.AdjacentAllyOrSelf:
            case MoveTarget.AdjacentFoe:
            case MoveTarget.All:
            case MoveTarget.AllAdjacent:
            case MoveTarget.Allies:
            case MoveTarget.AllyTeam:
            case MoveTarget.Any:
            case MoveTarget.RandomNormal:
            case MoveTarget.Scripted:
            case MoveTarget.None:
            default:
                throw new NotImplementedException($"Move target {move.Target} not implemented");
        }

        return targets.Where(t => !t.IsFainted).ToList();
    }

    private static List<Pokemon> ResolveNormalTargets(MoveChoice choice, Side attackingSide, Side defendingSide)
    {
        Pokemon attacker = choice.Attacker;

        // Get the intended target from MoveNormalTarget
        Pokemon? intendedTarget = GetIntendedTarget(choice.MoveNormalTarget, attackingSide, defendingSide);

        return intendedTarget is { IsFainted: false } ?
            [intendedTarget] :
            // Fallback logic for fainted/invalid targets
            GetFallbackTargets(choice.MoveNormalTarget, attacker, attackingSide, defendingSide);
    }

    private static Pokemon? GetIntendedTarget(MoveNormalTarget targetType, Side attackingSide, Side defendingSide)
    {
        return targetType switch
        {
            MoveNormalTarget.FoeSlot1 => defendingSide.Slot1,
            MoveNormalTarget.FoeSlot2 => defendingSide.Slot2,
            MoveNormalTarget.AllySlot1 => attackingSide.Slot1,
            MoveNormalTarget.AllySlot2 => attackingSide.Slot2,
            MoveNormalTarget.None => null,
            _ => throw new ArgumentOutOfRangeException(nameof(targetType)),
        };
    }

    private static List<Pokemon> GetFallbackTargets(MoveNormalTarget originalTarget, Pokemon attacker, Side attackingSide, Side defendingSide)
    {
        switch (originalTarget)
        {
            // For foe targets, fallback to any alive foe
            case MoveNormalTarget.FoeSlot1 or MoveNormalTarget.FoeSlot2:
                return defendingSide.AliveActivePokemon.ToList();
            // For ally targets, fallback to any alive ally
            case MoveNormalTarget.AllySlot1 or MoveNormalTarget.AllySlot2:
            {
                Pokemon? ally = attackingSide.GetAliveAlly(attacker.SlotId);
                return ally != null ? [ally] : [];
            }
            default:
                return [];
        }
    }

    private static List<Pokemon> GetAllAdjacentFoes(Pokemon attacker, Side defendingSide)
    {
        return defendingSide.AliveActivePokemon.ToList();
    }

    private static Pokemon? GetAdjacentAlly(Pokemon attacker, Side attackingSide)
    {
        return attackingSide.GetAliveAlly(attacker.SlotId);
    }

    #endregion
}

#region Result Classes

/// <summary>
/// Result of executing a move
/// </summary>
public class MoveExecutionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<MoveTargetResult> TargetResults { get; set; } = [];
}

/// <summary>
/// Result of executing a switch
/// </summary>
public class SwitchExecutionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Pokemon? SwitchedOutPokemon { get; set; }
    public Pokemon? SwitchedInPokemon { get; set; }
}

/// <summary>
/// Result of a move against a single target
/// </summary>
public class MoveTargetResult
{
    public required Pokemon Target { get; init; }
    public int Damage { get; set; }
    public bool Hit { get; set; }
}

#endregion