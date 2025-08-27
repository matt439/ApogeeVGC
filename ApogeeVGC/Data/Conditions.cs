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
            EffectType = EffectType.Status,
            OnStart = (pokemon, source) =>
            {

                return true;
            },

        },
        [ConditionId.Paralysis] = new Condition { Name = "Paralysis" },
        [ConditionId.Sleep] = new Condition { Name = "Sleep" },
        [ConditionId.Freeze] = new Condition { Name = "Freeze" },
        [ConditionId.Poison] = new Condition { Name = "Poison" },
        [ConditionId.Toxic] = new Condition { Name = "Toxic Poison" },
        [ConditionId.Confusion] = new Condition { Name = "Confusion" },
        [ConditionId.Flinch] = new Condition { Name = "Flinch" },
        [ConditionId.ChoiceLock] = new Condition { Name = "Choice Lock" },
    };

}