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

public partial class BattleNew
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

        // Resolve actual targets based on move targeting rules
        var actualTargets = ResolveActualTargets(choice);

        if (actualTargets.Count == 0 && RequiresTargets(move))
        {
            if (PrintDebug)
            {
                Console.WriteLine($"Move {move.Name} has no valid targets");
            }
            return;
        }

        // Pre-move setup (same for all targets)
        
        attacker.ActiveMoveActions++;
        attacker.LastMoveUsed = move;

        // Execute pre-move checks and effects
        if (!ExecutePreMoveChecks(attacker, move, actualTargets, playerId))
        {
            return;
        }

        move.UsedPp++;

        // Execute the move against all targets
        var moveResults = ExecuteMoveAgainstTargets(attacker, move, actualTargets, playerId);

        // Handle post-move effects (like recoil) - calculated once per move
        HandlePostMoveEffects(attacker, move, moveResults);

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

    private void PerformDamagingMove(Pokemon attacker, Move move, Pokemon defender, int numDefendingSidePokemon)
    {
        switch (move.Target)
        {
            case MoveTarget.Normal:
            case MoveTarget.AllAdjacentFoes:
            case MoveTarget.Self:
            case MoveTarget.AdjacentAlly:
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

        bool isCrit = BattleRandom.NextDouble() < 1.0 / 16.0; // 1 in 16 chance of critical hit
        MoveEffectiveness effectiveness = Library.TypeChart.GetMoveEffectiveness(
            defender.DefensiveTypes, move.Type);

        if (effectiveness == MoveEffectiveness.Immune)
        {
            if (PrintDebug)
            {
                UiGenerator.PrintMoveImmuneAction(attacker, move, defender);
            }
            return;
        }

        // Calculate base damage
        int damage = CalculateDamage(attacker, defender, move, effectiveness.GetMultiplier(), isCrit);

        // Apply spread move damage reduction for multi-target moves
        if (numDefendingSidePokemon > 1 && IsSpreadMove(move))
        {
            damage = RoundedDownAtHalf(damage * 0.75); // 25% damage reduction for spread moves
        }

        // check for OnAnyModifyDamage conditions on defender
        if (damage > 0)
        {
            double multiplier = defender.Conditions.Aggregate(1.0, (current, condition) =>
                current * (condition.OnAnyModifyDamage?.Invoke(damage, attacker, defender, move, isCrit,
                    numDefendingSidePokemon) ?? 1.0));
            damage = Math.Max(1, (int)(damage * multiplier)); // Always at least 1 damage
        }

        int actualDefenderDamage = defender.Damage(damage);

        if (PrintDebug)
        {
            UiGenerator.PrintDamagingMoveAction(attacker, move, damage, defender, effectiveness, isCrit);
        }

        move.OnHit?.Invoke(defender, attacker, move, Context);

        // Rocky helmet and other contact damage
        defender.Item?.OnDamagingHit?.Invoke(actualDefenderDamage, defender, attacker, move, Context);

        foreach (Condition condition in defender.Conditions.ToList())
        {
            condition.OnDamagingHit?.Invoke(actualDefenderDamage, defender, attacker, move, Context);
        }

        // Check for move condition application
        if (move.Condition is not null)
        {
            ApplyMoveCondition(move, attacker, defender);
        }

        // check if defender fainted
        if (defender.IsFainted)
        {
            // chilling neigh and similar abilities
            attacker.Ability.OnSourceAfterFaint?.Invoke(1, defender, attacker, move, Context);

            // TODO: clear defender's volatile conditions. This is needed in case it remains in
            // an active slot (when there is only 1 alive pokemon left on team in doubles)

            // probable need to clear only conditions that heal attacker (leech seed)
        }
    }

    private static bool IsSpreadMove(Move move)
    {
        return move.Target == MoveTarget.AllAdjacentFoes;
    }

    private void ApplyMoveCondition(Move move, Pokemon attacker, Pokemon target)
    {
        // Apply condition based on move target
        switch (move.Target)
        {
            case MoveTarget.Normal:
            case MoveTarget.AllAdjacentFoes:
            case MoveTarget.AdjacentAlly:
                target.AddCondition(move.Condition!, Context, attacker, move);
                break;
            case MoveTarget.Self:
                attacker.AddCondition(move.Condition!, Context, attacker, move);
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

    private List<MoveTargetResult> ExecuteMoveAgainstTargets(Pokemon attacker, Move move, List<Pokemon> targets,
        PlayerId playerId)
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
                    PerformStatusMove(attacker, playerId, move, attacker); // defender not used for these
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

                // Count defending side Pokemon for multi-target moves
                int numDefendingSidePokemon = targets.Count(target => target.SideId != attacker.SideId);

                results.AddRange(targets.Select(target =>
                    ExecuteMoveAgainstSingleTarget(attacker, move, target, playerId, numDefendingSidePokemon)));
                break;

            case MoveTarget.None:
            default:
                throw new InvalidOperationException($"Invalid move target type {move.Target} for move {move.Name}" +
                                                $"used by player {playerId}" );
        }

        return results;
    }

    private MoveTargetResult ExecuteMoveAgainstSingleTarget(Pokemon attacker, Move move, Pokemon target,
        PlayerId playerId, int numPokemonDefendingSide)
    {
        var result = new MoveTargetResult { Target = target, Damage = 0, Hit = false };

        // Miss check per target
        if (IsMoveMiss(attacker, move, target))
        {
            if (PrintDebug)
                UiGenerator.PrintMoveMissAction(attacker, move, target);
            return result;
        }

        // Immunity check per target
        if (move.OnTryImmunity?.Invoke(target) == true ||
            move.OnPrepareHit?.Invoke(target, attacker, move, Context) == false)
        {
            if (PrintDebug)
                UiGenerator.PrintMoveNoEffectAction(attacker, move, target);
            return result;
        }

        // OnTryHit checks per target
        if (target.Conditions.ToList().Any(condition =>
                condition.OnTryHit?.Invoke(target, attacker, move, Context) == false))
        {
            if (!PrintDebug) return result;

            if (target.HasCondition(ConditionId.Stall))
            {
                UiGenerator.PrintStallMoveProtection(attacker, move, target);
            }
            else
            {
                UiGenerator.PrintMoveNoEffectAction(attacker, move, target);
            }
            return result;
        }

        result.Hit = true;

        // Execute the move effect
        switch (move.Category)
        {
            case MoveCategory.Physical:
            case MoveCategory.Special:
                int initialHp = target.CurrentHp;
                PerformDamagingMove(attacker, move, target, numPokemonDefendingSide);
                result.Damage = initialHp - target.CurrentHp;
                break;
            case MoveCategory.Status:
                PerformStatusMove(attacker, playerId, move, target);
                break;
            default:
                throw new InvalidOperationException($"Invalid move category for move {move.Name}: {move.Category}" );
        }

        return result;
    }

    private void HandlePostMoveEffects(Pokemon attacker, Move move, List<MoveTargetResult> results)
    {
        // Calculate total damage dealt for recoil moves
        int totalDamage = results.Where(r => r.Hit).Sum(r => r.Damage);

        // Handle recoil - calculated once per move based on total damage
        if (move.Recoil is not null && totalDamage > 0)
        {
            int recoilDamage = (int)(totalDamage * move.Recoil.Value);
            attacker.Damage(recoilDamage);
            if (PrintDebug)
            {
                UiGenerator.PrintRecoilDamageAction(attacker, recoilDamage);
            }
        }

        // Handle other post-move effects here (life orb, etc.)
    }

    // Helper class to track move results
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
        int damage = CalculateDamage(attacker, defender, struggle, 1.0, false, false);
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

    private int CalculateDamage(Pokemon attacker, Pokemon defender, Move move,
        double moveEffectiveness, bool crit = false, bool applyStab = true)
    {
        if (moveEffectiveness == 0.0)
        {
            return 0; // No damage if immune
        }

        int level = attacker.Level;
        int attackStat = attacker.GetAttackStat(move, crit);
        int defenseStat = defender.GetDefenseStat(move, crit);
        int basePower = move.BasePowerCallback?.Invoke(attacker, defender, move) ?? move.BasePower;

        double onBasePowerModifier = move.OnBasePower?.Invoke(attacker, defender, move, Context) ?? 1.0;
        if (Math.Abs(onBasePowerModifier - 1.0) > Epsilon)
        {
            basePower = (int)(basePower * onBasePowerModifier);
        }
        double critModifier = crit ? 1.5 : 1.0;
        double random = 0.85 + BattleRandom.NextDouble() * 0.15; // Random factor between 0.85 and 1.0
        double stabModifier = applyStab ? attacker.GetStabMultiplier(move) : 1.0;

        double burnModifier = 1.0;
        if (attacker.HasCondition(ConditionId.Burn) && move.Category == MoveCategory.Physical &&
            attacker.Ability.Id != AbilityId.Guts)
        {
            burnModifier = 0.5;
        }

        int baseDamage = (int)((2 * level / 5.0 + 2) * basePower * attackStat / defenseStat / 50.0 + 2);
        int critMofified = RoundedDownAtHalf(critModifier * baseDamage);
        int randomModified = RoundedDownAtHalf(random * critMofified);
        int stabModified = RoundedDownAtHalf(stabModifier * randomModified);
        int typeModified = RoundedDownAtHalf(moveEffectiveness * stabModified);
        int burnModified = RoundedDownAtHalf(burnModifier * typeModified);
        return Math.Max(1, burnModified);
    }

    private List<Pokemon> ResolveActualTargets(SlotChoice.MoveChoice choice)
    {
        Move move = choice.Move;
        Pokemon attacker = choice.Attacker;
        var targets = new List<Pokemon>();

        switch (move.Target)
        {
            case MoveTarget.Normal:
                targets.AddRange(ResolveNormalTargets(choice));
                break;

            case MoveTarget.AllAdjacentFoes:
                targets.AddRange(GetAllAdjacentFoes(attacker));
                break;

            case MoveTarget.Self:
                targets.Add(attacker);
                break;

            case MoveTarget.AdjacentAlly:
                Pokemon? ally = GetAdjacentAlly(attacker);
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

    private List<Pokemon> ResolveNormalTargets(SlotChoice.MoveChoice choice)
    {
        Pokemon attacker = choice.Attacker;
        Move move = choice.Move;

        // Get the intended target from MoveNormalTarget
        Pokemon? intendedTarget = GetIntendedTarget(choice.MoveNormalTarget, attacker);

        return intendedTarget is { IsFainted: false } ?
            [intendedTarget] :
            // Fallback logic for fainted/invalid targets
            GetFallbackTargets(choice.MoveNormalTarget, attacker);
    }

    private Pokemon? GetIntendedTarget(MoveNormalTarget targetType, Pokemon attacker)
    {
        Side attackingSide = GetSide(attacker.SideId);
        Side defendingSide = GetSide(attacker.SideId.GetOppositeSide());

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

    private List<Pokemon> GetFallbackTargets(MoveNormalTarget originalTarget, Pokemon attacker)
    {
        switch (originalTarget)
        {
            // For foe targets, fallback to any alive foe
            case MoveNormalTarget.FoeSlot1 or MoveNormalTarget.FoeSlot2:
            {
                Side defendingSide = GetSide(attacker.SideId.GetOppositeSide());
                return defendingSide.AliveActivePokemon.ToList();
            }
            // For ally targets, fallback to any alive ally
            case MoveNormalTarget.AllySlot1 or MoveNormalTarget.AllySlot2:
            {
                Side attackingSide = GetSide(attacker.SideId);
                Pokemon? ally = attackingSide.GetAliveAlly(attacker.SlotId);
                return ally != null ? [ally] : [];
            }
            default:
                return [];
        }
    }

    private bool ExecutePreMoveChecks(Pokemon attacker, Move move, List<Pokemon> targets, PlayerId playerId)
    {
        // Item modifications (using first target for single-target effects)
        Pokemon primaryTarget = targets.FirstOrDefault() ?? attacker;
        attacker.Item?.OnModifyMove?.Invoke(move, attacker, primaryTarget, Context);

        if (move.Pp <= 0)
        {
            throw new InvalidOperationException($"Move {move.Name} has no PP left for player {playerId}");
        }

        // Check disable conditions
        foreach (Condition condition in attacker.Conditions.ToList())
        {
            condition.OnDisableMove?.Invoke(attacker, move, Context);
        }

        if (move.Disabled)
        {
            if (PrintDebug)
                UiGenerator.PrintDisabledMoveTry(attacker, move);
            return false;
        }

        // OnBeforeMove checks
        if (attacker.Conditions
            .Where(c => c.OnBeforeMove != null)
            .OrderBy(c => c.OnBeforeMovePriority ?? 0)
            .Any(condition => condition.OnBeforeMove?.Invoke(attacker, primaryTarget, move, Context) == false))
        {
            return false;
        }

        // Stalling move checks
        if (move.StallingMove)
        {
            if (attacker.Conditions.ToList().All(condition =>
                    condition.OnStallMove?.Invoke(attacker, Context) != false))
            {
                return move.OnTry?.Invoke(attacker, primaryTarget, move, Context) != false;
            }

            if (PrintDebug)
            {
                UiGenerator.PrintMoveFailAction(attacker, move);
            }
            return false;
        }

        // OnTry checks
        return move.OnTry?.Invoke(attacker, primaryTarget, move, Context) != false;
    }

    private List<Pokemon> GetAllAdjacentFoes(Pokemon attacker)
    {
        Side defendingSide = GetSide(attacker.SideId.GetOppositeSide());
        return defendingSide.AliveActivePokemon.ToList();
    }

    private Pokemon? GetAdjacentAlly(Pokemon attacker)
    {
        Side attackingSide = GetSide(attacker.SideId);
        return attackingSide.GetAliveAlly(attacker.SlotId);
    }

    /// <summary>
    /// Execute a switch choice
    /// </summary>
    private async Task ExecuteSwitchChoiceAsync(PlayerId playerId, SlotChoice.SwitchChoice choice)
    {
        Side side = GetSide(playerId);
        Pokemon prevActive = choice.SwitchOutPokemon;

        prevActive.OnSwitchOut();
        side.SwitchSlots(choice.SwitchOutSlot, choice.SwitchInSlot);
        Pokemon newActive = choice.SwitchInPokemon;

        Field.OnPokemonSwitchIn(newActive, playerId, Context);
        newActive.OnSwitchIn(Field, AllActivePokemonArray, Context);

        await Task.CompletedTask;
    }
}