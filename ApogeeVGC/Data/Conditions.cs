using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
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
            OnStart = (target, source, sourceEffect) =>
            {
                switch (sourceEffect.EffectType)
                {
                    case EffectType.Item:
                    {
                        if (sourceEffect is Item { Name: "Flame Orb" })
                        {
                            UiGenerator.PrintBurnStartFromFlameOrb(target);
                        }
                        break;
                    }
                    case EffectType.Ability:
                    {
                        if (sourceEffect is Ability ability)
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
                        UiGenerator.PrintBurnStart(target);
                        break;
                }
                return true;
            },
            OnResidualOrder = 10,
            OnResidual = (target, source, effect) =>
            {
                int damage = target.UnmodifiedHp / 16;
                if (damage < 1) damage = 1;
                target.Damage(damage);
                UiGenerator.PrintBurnDamage(target);
            }
        },
        [ConditionId.Paralysis] = new Condition
        {
            Id = ConditionId.Paralysis,
            Name = "Paralysis"
        },
        [ConditionId.Sleep] = new Condition
        {
            Id = ConditionId.Sleep,
            Name = "Sleep"
        },
        [ConditionId.Freeze] = new Condition
        {
            Id = ConditionId.Freeze,
            Name = "Freeze"
        },
        [ConditionId.Poison] = new Condition
        {
            Id = ConditionId.Poison,
            Name = "Poison"
        },
        [ConditionId.Toxic] = new Condition
        {
            Id = ConditionId.Toxic,
            Name = "Toxic"
        },
        [ConditionId.Confusion] = new Condition
        {
            Id = ConditionId.Confusion,
            Name = "Confusion"
        },
        [ConditionId.Flinch] = new Condition
        {
            Id = ConditionId.Flinch,
            Name = "Flinch"
        },
        [ConditionId.ChoiceLock] = new Condition
        {
            Id = ConditionId.ChoiceLock,
            Name = "Choice Lock"
        },
        [ConditionId.LeechSeed] = new Condition
        {
            Id = ConditionId.LeechSeed,
            Name = "Leech Seed",
            ConditionEffectType = ConditionEffectType.Status,
            OnStart = (target, source, sourceEffect) =>
            {
                UiGenerator.PrintLeechSeedStart(target);
                return true;
            },
            OnResidualOrder = 8,
            OnResidual = (target, source, effect) =>
            {
                int damage = target.UnmodifiedHp / 8;
                if (damage < 1) damage = 1;

                if (target.CurrentHp <= 0)
                {
                    throw new InvalidOperationException("Target has fainted.");
                }
                target.Damage(damage);

                if (source is not null)
                {
                    source.Heal(damage);
                    UiGenerator.PrintLeechSeedDamage(target, source, damage);
                }
                else
                {
                    throw new ArgumentNullException($"Source pokemon is null.");
                }
            }
        },
        [ConditionId.TrickRoom] = new Condition
        {
            Id = ConditionId.TrickRoom,
            Duration = 5,
            Name = "Trick Room",
            ConditionEffectType = ConditionEffectType.Terrain,
            DurationCallback = (sourcePokemon, _) =>
                sourcePokemon.Ability.Id == AbilityId.Persistent ? 7 : 5,
            OnFieldStart = (_, _, _) =>
            {
                UiGenerator.PrintTrickRoomStart();
            },
            OnFieldRestart = (_, _, _) =>
            {
                UiGenerator.PrintTrickRoomRestart();
            },
            OnFieldResidualOrder = 27,
            OnFieldResidualSubOrder = 1,
            OnFieldEnd = (_) =>
            {
                UiGenerator.PrintTrickRoomEnd();
            },
        },
        [ConditionId.Stall] = new Condition
        {
            Id = ConditionId.Stall,
            Name = "Stall",
            Duration = 2,
            CounterMax = 729,
            OnStart = (target, _, _) =>
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
        },
    };
}