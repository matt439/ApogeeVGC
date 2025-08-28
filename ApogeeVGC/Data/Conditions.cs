using System.Collections.ObjectModel;
using ApogeeVGC.Sim;

namespace ApogeeVGC.Data;

public record Conditions
{
    public IReadOnlyDictionary<ConditionId, Condition> ConditionsData { get; }

    public Conditions()
    {
        ConditionsData = new ReadOnlyDictionary<ConditionId, Condition>(_conditions);
    }

    private readonly Dictionary<ConditionId, Condition> _conditions = new()
    {
        [ConditionId.Burn] = new Condition
        {
            Id = ConditionId.Burn,
            Name = "Burn",
            ConditionEffectType = ConditionEffectType.Status,
            ConditionVolatility = ConditionVolatility.NonVolatile,
            OnStart = (target, _, sourceEffect, debug) =>
            {
                if (sourceEffect is null)
                {
                    throw new ArgumentNullException($"Source effect is null when trying to apply" +
                                                    $"{ConditionId.Burn} to" + $"pokemon {target.Name}.");
                }
                
                switch (sourceEffect.EffectType)
                {
                    case EffectType.Item:
                    {
                        if (sourceEffect is Item { Name: "Flame Orb" } && debug)
                        {
                            UiGenerator.PrintBurnStartFromFlameOrb(target);
                        }
                        break;
                    }
                    case EffectType.Ability:
                    {
                        if (sourceEffect is Ability ability && debug)
                        {
                            UiGenerator.PrintBurnStartFromAbility(target, ability);
                        }
                        break;
                    }
                    case EffectType.Move:
                    case EffectType.Specie:
                    case EffectType.Condition:
                    case EffectType.Format:
                        return false;
                    default:
                        if (debug)
                        {
                            UiGenerator.PrintBurnStart(target);
                        }
                        break;
                }
                return true;
            },
            OnResidualOrder = 10,
            OnResidual = (target, _, _, debug) =>
            {
                int damage = target.UnmodifiedHp / 16;
                if (damage < 1) damage = 1;
                target.Damage(damage);
                if (debug)
                {
                    UiGenerator.PrintBurnDamage(target);
                }
            },
        },
        [ConditionId.Paralysis] = new Condition
        {
            Id = ConditionId.Paralysis,
            Name = "Paralysis",
            ConditionVolatility = ConditionVolatility.NonVolatile,
        },
        [ConditionId.Sleep] = new Condition
        {
            Id = ConditionId.Sleep,
            Name = "Sleep",
            ConditionVolatility = ConditionVolatility.NonVolatile,
        },
        [ConditionId.Freeze] = new Condition
        {
            Id = ConditionId.Freeze,
            Name = "Freeze",
            ConditionVolatility = ConditionVolatility.NonVolatile,
        },
        [ConditionId.Poison] = new Condition
        {
            Id = ConditionId.Poison,
            Name = "Poison",
            ConditionVolatility = ConditionVolatility.NonVolatile,
        },
        [ConditionId.Toxic] = new Condition
        {
            Id = ConditionId.Toxic,
            Name = "Toxic",
            ConditionVolatility = ConditionVolatility.NonVolatile,
        },
        [ConditionId.Confusion] = new Condition
        {
            Id = ConditionId.Confusion,
            Name = "Confusion",
            ConditionVolatility = ConditionVolatility.Volatile,
        },
        [ConditionId.Flinch] = new Condition
        {
            Id = ConditionId.Flinch,
            Name = "Flinch",
            ConditionVolatility = ConditionVolatility.Volatile,
        },
        [ConditionId.ChoiceLock] = new Condition
        {
            Id = ConditionId.ChoiceLock,
            Name = "Choice Lock",
            ConditionVolatility = ConditionVolatility.Volatile,
        },
        [ConditionId.LeechSeed] = new Condition
        {
            Id = ConditionId.LeechSeed,
            Name = "Leech Seed",
            ConditionEffectType = ConditionEffectType.Status,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnStart = (target, _, _, debug) =>
            {
                if (debug)
                {
                    UiGenerator.PrintLeechSeedStart(target);
                }
                return true;
            },
            OnResidualOrder = 8,
            OnResidual = (target, source, _, debug) =>
            {
                int damage = target.UnmodifiedHp / 8;
                if (damage < 1) damage = 1;

                if (target.CurrentHp <= 0)
                {
                    throw new InvalidOperationException("Target has fainted.");
                }
                int actualDamage = target.Damage(damage);

                if (source is not null)
                {
                     int actualHeal = source.Team.ActivePokemon.Heal(damage);
                    if (debug)
                    {
                        UiGenerator.PrintLeechSeedDamage(target, actualDamage, source.Team.ActivePokemon,
                            actualHeal);
                    }
                }
                else
                {
                    throw new ArgumentNullException($"Source is null when trying to apply" +
                                                    $"{ConditionId.LeechSeed} damage to" + $"pokemon {target.Name}.");
                }
            },
        },
        // This condition is the effect placed on each Pokemon by Trick Room.
        // Trick Room itself is a PseudoWeather condition on the field which applies
        // this condition to each Pokemon.
        // This condition doesn't have apply any effect to the Pokemon itself but could
        // be useful for UI etc.
        [ConditionId.TrickRoom] = new Condition
        {
            Id = ConditionId.TrickRoom,
            Name = "Trick Room",
            ConditionEffectType = ConditionEffectType.PseudoWeather,
            ConditionVolatility = ConditionVolatility.Volatile,
            //OnFieldStart = (_, _, _) =>
            //{
            //    UiGenerator.PrintTrickRoomStart();
            //},
            //OnFieldRestart = (_, _, _) =>
            //{
            //    UiGenerator.PrintTrickRoomRestart();
            //},
            //OnFieldResidualOrder = 27,
            //OnFieldResidualSubOrder = 1,
            //OnFieldEnd = (_) =>
            //{
            //    UiGenerator.PrintTrickRoomEnd();
            //},
        },
        [ConditionId.Stall] = new Condition
        {
            Id = ConditionId.Stall,
            Name = "Stall",
            Duration = 2,
            CounterMax = 729,
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            OnStart = (target, _, _, _) =>
            {
                Condition? condition = target.GetCondition(ConditionId.Stall);

                if (condition is null)
                {
                    throw new NullReferenceException($"Condition {ConditionId.Stall} not found on" +
                                                     $"pokemon {target.Name}.");
                }

                condition.Counter = 3;
                return true;
            },
            OnStallMove = (random, pokemon) =>
            {
                Condition? condition = pokemon.GetCondition(ConditionId.Stall);

                if (condition is null)
                {
                    throw new NullReferenceException($"Condition {ConditionId.Stall} not found on" +
                                                     $"pokemon {pokemon.Name}.");
                }

                int counter = condition.Counter ?? 1;
                double successChance = 1.0 / counter;
                bool success = random.NextDouble() < successChance;

                if (success) return success;

                if (!pokemon.RemoveCondition(ConditionId.Stall))
                {
                    throw new InvalidOperationException("Failed to remove Stall condition.");
                }
                return success;
            },
            OnRestart = (target, _, _) =>
            {
                Condition? condition = target.GetCondition(ConditionId.Stall);

                if (condition is null)
                {
                    throw new NullReferenceException($"Condition {ConditionId.Stall} not found on" +
                                                     $"pokemon {target.Name}.");
                }

                int counter = condition.Counter ?? 1;
                int counterMax = condition.CounterMax ?? 729;

                if (counter < counterMax)
                {
                    condition.Counter = counter * 3;
                }

                condition.Duration = 2;

                return true;
            }
        },
        [ConditionId.Protect] = new Condition
        {
            Id = ConditionId.Protect,
            Name = "Protect",
            Duration = 1,
            ConditionEffectType = ConditionEffectType.Condition,
            ConditionVolatility = ConditionVolatility.Volatile,
            // OnStart
            OnTryHitPriority = 3,
            OnTryHit = (_, _, move) =>
            {
                if (!(move.Flags.Protect ?? false))
                {
                    return true;
                }
                // TODO: Check for smart target (dragon darts), outrage lock
                return false;
            },
        },
    };
}