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
        [ConditionId.Paralysis] = new Condition { Name = "Paralysis" },
        [ConditionId.Sleep] = new Condition { Name = "Sleep" },
        [ConditionId.Freeze] = new Condition { Name = "Freeze" },
        [ConditionId.Poison] = new Condition { Name = "Poison" },
        [ConditionId.Toxic] = new Condition { Name = "Toxic Poison" },
        [ConditionId.Confusion] = new Condition { Name = "Confusion" },
        [ConditionId.Flinch] = new Condition { Name = "Flinch" },
        [ConditionId.ChoiceLock] = new Condition { Name = "Choice Lock" },
        [ConditionId.LeechSeed] = new Condition
        {
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


    };

}