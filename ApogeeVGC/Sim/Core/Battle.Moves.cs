using ApogeeVGC.Player;
using ApogeeVGC.Data;
using ApogeeVGC.Sim.Choices;
using ApogeeVGC.Sim.Ui;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Sim.Core;

public partial class Battle
{
    private MoveAction PerformMove(PlayerId playerId, SlotChoice.MoveChoice choice)
    {
        Pokemon attacker = choice.Attacker;
        Pokemon defender = choice.PossibleTargets.Count > 0 ? choice.PossibleTargets[0] : attacker;
        Move move = choice.Move;

        // This is where choice lock and choice benefits come into play
        attacker.Item?.OnModifyMove?.Invoke(move, attacker, defender, Context);

        if (move.Pp <= 0)
        {
            throw new InvalidOperationException($"Move {move.Name} has no PP left" +
                                                $"for player {playerId}");
        }

        // Check OnDisableMove conditions on attacker
        foreach (Condition condition in attacker.Conditions.ToList())
        {
            condition.OnDisableMove?.Invoke(attacker, move, Context);
        }
        if (move.Disabled)
        {
            if (PrintDebug)
            {
                UiGenerator.PrintDisabledMoveTry(attacker, move);
            }
            return MoveAction.None;
        }

        move.UsedPp++;  // Decrease PP for the move used
        attacker.ActiveMoveActions++; // Increment the count of moves used this battle (for fake out, etc.)
        attacker.LastMoveUsed = move;

        // Check for conditions with OnBeforeMove on attacker (e.g. flinch, paralysis)
        if (attacker.Conditions
            .Where(c => c.OnBeforeMove != null)
            .OrderBy(c => c.OnBeforeMovePriority ?? 0)
            .ToList().Any(condition => condition.OnBeforeMove == null ||
                                       !condition.OnBeforeMove(attacker, defender, move, Context)))
        {
            return MoveAction.None;
        }

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

                return MoveAction.None;
            }
        }

        // OnTry checks on the attacker's move (incl fake out, etc)
        if (move.OnTry?.Invoke(attacker, defender, move, Context) == false)
        {
            return MoveAction.None;
        }

        // Miss check
        if (IsMoveMiss(attacker, move, defender))
        {
            if (PrintDebug)
            {
                UiGenerator.PrintMoveMissAction(attacker, move, defender);
            }
            return MoveAction.None;
        }

        // Immunity check. Note that this does not check for normal immunity, only special cases.
        // Regular immunity check is done in PerformDamagingMove
        if (move.OnTryImmunity != null && move.OnTryImmunity(defender) ||
            move.OnPrepareHit?.Invoke(defender, attacker, move, Context) == false)
        {
            if (PrintDebug)
            {
                UiGenerator.PrintMoveNoEffectAction(attacker, move, defender);
            }
            return MoveAction.None;
        }

        // check every condition on defender for OnTryHit effects
        foreach (Condition condition in defender.Conditions.ToList())
        {
            if (condition.OnTryHit == null || condition.OnTryHit(defender, attacker, move, Context)) continue;

            if (!PrintDebug) return MoveAction.None;

            // Check if the defender is protected by a stall move like Protect, Detect, etc.
            if (defender.HasCondition(ConditionId.Stall))
            {
                UiGenerator.PrintStallMoveProtection(attacker, move, defender);
            }
            else
            {
                UiGenerator.PrintMoveNoEffectAction(attacker, move, defender);
            }
            return MoveAction.None;
        }

        return move.Category switch
        {
            MoveCategory.Physical or MoveCategory.Special => PerformDamagingMove(attacker, playerId, move, defender),
            MoveCategory.Status => PerformStatusMove(attacker, playerId, move, defender),
            _ => throw new InvalidOperationException($"Invalid move category for move {move.Name}: {move.Category}"),
        };
    }

    private MoveAction PerformDamagingMove(Pokemon attacker, PlayerId attackingPlayer, Move move, Pokemon defender)
    {
        switch (move.Target)
        {
            case MoveTarget.Normal:
            case MoveTarget.AllAdjacentFoes:
                break;
            case MoveTarget.Self:
            case MoveTarget.AdjacentAlly:
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
            return MoveAction.None;
        }

        int damage = CalculateDamage(attacker, defender, move, effectiveness.GetMultiplier(), isCrit);

        // check for OnAnyModifyDamage conditions on defender
        if (damage > 0)
        {
            // TODO: numPokemonDefendingSide for multi-battles
            int numPokemonDefendingSide = 1;

            double multiplier = defender.Conditions.Aggregate(1.0, (current, condition) =>
                current * (condition.OnAnyModifyDamage?.Invoke(damage, attacker, defender, move, isCrit,
                    numPokemonDefendingSide) ?? 1.0));
            damage = Math.Max(1, (int)(damage * multiplier)); // Always at least 1 damage
        }

        int actualDefenderDamage = defender.Damage(damage);
        
        if (PrintDebug)
        {
            UiGenerator.PrintDamagingMoveAction(attacker, move, damage, defender, effectiveness, isCrit);
        }

        move.OnHit?.Invoke(defender, attacker, move, Context);

        // Rocky helmet
        defender.Item?.OnDamagingHit?.Invoke(actualDefenderDamage, defender, attacker, move, Context);
        
        foreach (Condition condition in defender.Conditions.ToList())
        {
            condition.OnDamagingHit?.Invoke(actualDefenderDamage, defender, attacker, move, Context);
        }

        // Check for move condition application
        if (move.Condition is not null)
        {
            // Apply condition based on move target
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

        // check for recoil
        if (move.Recoil is not null)
        {
            int recoilDamage = (int)(damage * move.Recoil.Value);
            attacker.Damage(recoilDamage);
            if (PrintDebug)
            {
                UiGenerator.PrintRecoilDamageAction(attacker, recoilDamage);
            }
        }

        // check if defender fainted
        if (defender.IsFainted)
        {
            // chilling neigh
            attacker.Ability.OnSourceAfterFaint?.Invoke(1, defender, attacker, move, Context);
        }

        return move.SelfSwitch ? MoveAction.SwitchAttackerOut : MoveAction.None;
    }

    private MoveAction PerformStatusMove(Pokemon attacker, PlayerId playerId, Move move, Pokemon defender)
    {
        if (PrintDebug)
        {
            UiGenerator.PrintStatusMoveAction(attacker, move);
        }

        if (move.Target == MoveTarget.Field)
        {
            HandleFieldTargetStatusMove(move, attacker);
            return MoveAction.None;
        }

        if (move.Target == MoveTarget.AllySide)
        {
            HandleSideTargetStatusMove(move, playerId, attacker);
            return MoveAction.None;
        }
        if (move.Target == MoveTarget.FoeSide)
        {
            HandleSideTargetStatusMove(move, playerId.OpposingPlayerId(), attacker);
            return MoveAction.None;
        }

        if (move.Condition is null)
        {
            return MoveAction.None;
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

        return MoveAction.None;
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
                Field.ReapplyPseudoWeather(move.PseudoWeather.Id, AllActivePokemon, Context);
            }
            else // Otherwise, add the new pseudo-weather
            {
                Field.AddPseudoWeather(move.PseudoWeather, attacker, move, AllActivePokemon, Context);
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
                Field.AddWeather(move.Weather, attacker, move, AllActivePokemon, Context);
            }
            else // No existing weather, just add the new one
            {
                Field.AddWeather(move.Weather, attacker, move, AllActivePokemon, Context);
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
                Field.AddTerrain(move.Terrain, attacker, move, AllActivePokemon, Context);
            }
            else // No existing terrain, just add the new one
            {
                Field.AddTerrain(move.Terrain, attacker, move, AllActivePokemon, Context);
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
}