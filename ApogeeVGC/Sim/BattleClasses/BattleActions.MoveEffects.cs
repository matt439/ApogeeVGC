using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    public SpreadMoveDamage RunMoveEffects(SpreadMoveDamage damage, SpreadMoveTargets targets,
        Pokemon source, ActiveMove move, ActiveMove moveData, bool isSecondary = false, bool isSelf = false)
    {
        BoolIntUndefinedUnion? didAnything = damage.Aggregate<BoolIntUndefinedUnion?, BoolIntUndefinedUnion?>(
            null, CombineResults);

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] is not PokemonPokemonUnion pokemonUnion)
            {
                continue;
            }

            Pokemon target = pokemonUnion.Pokemon;
            BoolIntUndefinedUnion? hitResult;
            BoolIntUndefinedUnion didSomething = BoolIntUndefinedUnion.FromUndefined();

            // Apply boosts
            if (moveData.HitEffect?.Boosts != null && !target.Fainted)
            {
                BoolZeroUnion? boostResult = Battle.Boost(moveData.HitEffect.Boosts, target, source, move, isSecondary, isSelf);
                hitResult = boostResult switch
                {
                    BoolBoolZeroUnion bbz => bbz.Value,
                    ZeroBoolZeroUnion => 0,
                    null => null,
                    _ => throw new InvalidOperationException("Unexpected boost result type.")
                };
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Apply healing
            if (moveData.Heal != null && !target.Fainted)
            {
                if (target.Hp >= target.MaxHp)
                {
                    if (Battle.DisplayUi)
                    {
                        Battle.Add("-fail", target, "heal");
                        Battle.AttrLastMove("[still]");
                    }
                    damage[i] = CombineResults(damage[i], BoolIntUndefinedUnion.FromBool(false));
                    didAnything = CombineResults(didAnything, null);
                    continue;
                }

                int amount = target.BaseMaxHp * moveData.Heal[0] / moveData.Heal[1];
                int roundedAmount = Battle.Gen < 5 ? (int)Math.Floor((double)amount) : (int)Math.Round((double)amount);

                IntFalseUnion? healResult = Battle.Heal(roundedAmount, target, source,
                    BattleHealEffect.FromIEffect(move));

                if (healResult is not IntIntFalseUnion intHeal || (intHeal.Value == 0 && healResult is FalseIntFalseUnion))
                {
                    if (healResult is not null)
                    {
                        if (Battle.DisplayUi)
                        {
                            Battle.Add("-fail", source);
                            Battle.AttrLastMove("[still]");
                        }
                    }
                    Battle.Debug("heal interrupted");
                    damage[i] = CombineResults(damage[i], BoolIntUndefinedUnion.FromBool(false));
                    didAnything = CombineResults(didAnything, null);
                    continue;
                }

                didSomething = BoolIntUndefinedUnion.FromBool(true);
            }

            // Try to apply status
            if (moveData.Status != null)
            {
                bool statusResult = target.TrySetStatus(moveData.Status.Value, source,
                    moveData.Ability != null ? Library.Abilities[moveData.Ability.Id] : move);
                hitResult = BoolIntUndefinedUnion.FromBool(statusResult);

                if (!statusResult && move.Status != null)
                {
                    damage[i] = CombineResults(damage[i], BoolIntUndefinedUnion.FromBool(false));
                    didAnything = CombineResults(didAnything, null);
                    continue;
                }

                didSomething = CombineResults(didSomething, hitResult);
            }

            // Force status (bypasses immunity)
            if (moveData.ForceStatus != null)
            {
                bool forceStatusResult = target.SetStatus(moveData.ForceStatus.Value, source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(forceStatusResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Apply volatile status
            if (moveData.VolatileStatus != null)
            {
                RelayVar volatileResult = target.AddVolatile(moveData.VolatileStatus.Value, source, move);
                hitResult = volatileResult switch
                {
                    BoolRelayVar brv => BoolIntUndefinedUnion.FromBool(brv.Value),
                    IntRelayVar irv => BoolIntUndefinedUnion.FromInt(irv.Value),
                    _ => BoolIntUndefinedUnion.FromUndefined()
                };
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Apply side condition
            if (moveData.SideCondition != null)
            {
                bool sideCondResult = target.Side.AddSideCondition(
                    Library.Conditions[moveData.SideCondition.Value], source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(sideCondResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Apply slot condition
            if (moveData.HitEffect?.SlotCondition != null)
            {
                bool slotCondResult = target.Side.AddSlotCondition(target,
                    Library.Conditions[moveData.HitEffect.SlotCondition.Value], source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(slotCondResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Set weather
            if (moveData.Weather != null)
            {
                bool weatherResult = Battle.Field.SetWeather(
                    Library.Conditions[moveData.Weather.Value], source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(weatherResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Set terrain
            if (moveData.HitEffect?.Terrain != null)
            {
                bool terrainResult = Battle.Field.SetTerrain(
                    Library.Conditions[moveData.HitEffect.Terrain.Value], source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(terrainResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Add pseudo weather
            if (moveData.PseudoWeather != null)
            {
                bool pseudoWeatherResult = Battle.Field.AddPseudoWeather(
                    Library.Conditions[moveData.PseudoWeather.Value], source, move);
                hitResult = BoolIntUndefinedUnion.FromBool(pseudoWeatherResult);
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Check force switch
            if (moveData.ForceSwitch != null)
            {
                int canSwitchResult = Battle.CanSwitch(target.Side);
                hitResult = canSwitchResult;
                didSomething = CombineResults(didSomething, hitResult);
            }

            // Hit events
            // These are like the TryHit events, except we don't need a FieldHit event.
            if (move.Target == MoveTarget.All && !isSelf)
            {
                if (moveData.OnHitField != null)
                {
                    RelayVar? fieldHitResult = Battle.SingleEvent(EventId.HitField, moveData, null,
                        target, source, move);
                    hitResult = fieldHitResult switch
                    {
                        BoolRelayVar brv => BoolIntUndefinedUnion.FromBool(brv.Value),
                        IntRelayVar irv => BoolIntUndefinedUnion.FromInt(irv.Value),
                        _ => BoolIntUndefinedUnion.FromUndefined()
                    };
                    didSomething = CombineResults(didSomething, hitResult);
                }
            }
            else if ((move.Target == MoveTarget.FoeSide || move.Target == MoveTarget.AllySide) && !isSelf)
            {
                if (moveData.OnHitSide != null)
                {
                    RelayVar? sideHitResult = Battle.SingleEvent(EventId.HitSide, moveData, null,
                        target.Side, source, move);
                    hitResult = sideHitResult switch
                    {
                        BoolRelayVar brv => BoolIntUndefinedUnion.FromBool(brv.Value),
                        IntRelayVar irv => BoolIntUndefinedUnion.FromInt(irv.Value),
                        _ => BoolIntUndefinedUnion.FromUndefined()
                    };
                    didSomething = CombineResults(didSomething, hitResult);
                }
            }
            else
            {
                if (moveData.OnHit != null)
                {
                    RelayVar? hitEventResult = Battle.SingleEvent(EventId.Hit, moveData, null,
                        target, source, move);
                    hitResult = hitEventResult switch
                    {
                        BoolRelayVar brv => BoolIntUndefinedUnion.FromBool(brv.Value),
                        IntRelayVar irv => BoolIntUndefinedUnion.FromInt(irv.Value),
                        _ => BoolIntUndefinedUnion.FromUndefined()
                    };
                    didSomething = CombineResults(didSomething, hitResult);
                }

                if (!isSelf && !isSecondary)
                {
                    Battle.RunEvent(EventId.Hit, target, source, move);
                }
            }

            // Handle self-destruct on hit
            if (moveData.SelfDestruct is IfHitMoveSelfDestruct && damage[i] is not BoolBoolIntUndefinedUnion { Value: false })
            {
                Battle.Faint(source, source, move);
            }

            // Handle self-switch
            if (moveData.SelfSwitch != null)
            {
                if (Battle.CanSwitch(source.Side) != 0 && !source.Volatiles.ContainsKey(ConditionId.Commanded))
                {
                    didSomething = BoolIntUndefinedUnion.FromBool(true);
                }
                else
                {
                    didSomething = CombineResults(didSomething, BoolIntUndefinedUnion.FromBool(false));
                }
            }

            // Move didn't fail because it didn't try to do anything
            if (didSomething is UndefinedBoolIntUndefinedUnion)
            {
                didSomething = BoolIntUndefinedUnion.FromBool(true);
            }

            damage[i] = CombineResults(damage[i], didSomething);
            didAnything = CombineResults(didAnything, didSomething);
        }

        // Check if move failed completely
        if (didAnything is not (IntBoolIntUndefinedUnion { Value: 0 } or IntBoolIntUndefinedUnion) &&
            moveData.Self == null && moveData.SelfDestruct == null)
        {
            if (!isSelf && !isSecondary)
            {
                if (didAnything is BoolBoolIntUndefinedUnion { Value: false })
                {
                    if (Battle.DisplayUi)
                    {
                        Battle.Add("-fail", source);
                        Battle.AttrLastMove("[still]");
                    }
                }
            }
            Battle.Debug("move failed because it did nothing");
        }
        else if (move.SelfSwitch != null && source.Hp > 0 && !source.Volatiles.ContainsKey(ConditionId.Commanded))
        {
            source.SwitchFlag = move.Id;
        }

        return damage;
    }

    public void SelfDrops(SpreadMoveTargets targets, Pokemon source, ActiveMove move, ActiveMove moveData,
        bool isSecondary = false)
    {
        foreach (PokemonFalseUnion targetUnion in targets)
        {
            if (targetUnion is not PokemonPokemonUnion)
            {
                continue;
            }

            if (moveData.Self != null && move.SelfDropped != true)
            {
                if (!isSecondary && moveData.Self.Boosts != null)
                {
                    int secondaryRoll = Battle.Random(100);
                    if (moveData.Self.Chance == null || secondaryRoll < moveData.Self.Chance)
                    {
                        MoveHit(source, source, move, moveData.Self, isSecondary, true);
                    }
                    if (move.MultiHit == null)
                    {
                        move.SelfDropped = true;
                    }
                }
                else
                {
                    MoveHit(source, source, move, moveData.Self, isSecondary, true);
                }
            }
        }
    }

    public void Secondaries(SpreadMoveTargets targets, Pokemon source, ActiveMove move, ActiveMove moveData,
        bool isSelf = false)
    {
        if (moveData.Secondaries == null) return;

        foreach (PokemonFalseUnion targetUnion in targets)
        {
            if (targetUnion is not PokemonPokemonUnion pokemonUnion)
            {
                continue;
            }

            Pokemon target = pokemonUnion.Pokemon;

            // Run ModifySecondaries event to get the list of secondary effects
            RelayVar? modifyResult = Battle.RunEvent(EventId.ModifySecondaries, target, source, moveData,
                moveData.Secondaries);

            var secondaries = modifyResult is SecondaryEffectArrayRelayVar secListRv
                ? secListRv.Effects
                : moveData.Secondaries;

            foreach (SecondaryEffect secondary in secondaries)
            {
                int secondaryRoll = Battle.Random(100);

                // User stat boosts or target stat drops can possibly overflow if it goes beyond 256 in Gen 8 or prior
                bool secondaryOverflow = (secondary.Boosts != null || secondary.Self != null) && Battle.Gen <= 8;

                int effectiveChance = secondary.Chance ?? 100;
                if (secondaryOverflow)
                {
                    effectiveChance %= 256;
                }

                if (secondary.Chance == null || secondaryRoll < effectiveChance)
                {
                    MoveHit(target, source, move, secondary, true, isSelf);
                }
            }
        }
    }

    public SpreadMoveDamage ForceSwitch(SpreadMoveDamage damage, SpreadMoveTargets targets, Pokemon source,
        ActiveMove move)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] is not PokemonPokemonUnion pokemonUnion)
            {
                continue;
            }

            Pokemon target = pokemonUnion.Pokemon;

            if (target.Hp > 0 && source.Hp > 0 && Battle.CanSwitch(target.Side) != 0)
            {
                RelayVar? hitResult = Battle.RunEvent(EventId.DragOut, target, source, move);

                switch (hitResult)
                {
                    case BoolRelayVar { Value: true } or null:
                        target.ForceSwitchFlag = true;
                        break;
                    case BoolRelayVar { Value: false } when move.Category == MoveCategory.Status:
                        if (Battle.DisplayUi)
                        {
                            Battle.Add("-fail", source);
                            Battle.AttrLastMove("[still]");
                        }
                        damage[i] = BoolIntUndefinedUnion.FromBool(false);
                        break;
                }
            }
        }

        return damage;
    }
}