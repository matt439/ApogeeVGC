using System.Collections.Frozen;
using ApogeeVGC.Sim.Conditions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    public FrozenDictionary<ConditionId, Condition> ConditionsData { get; }
    private readonly Library _library;

    public Conditions(Library library)
    {
        _library = library;
        ConditionsData = CreateConditions().ToFrozenDictionary();
    }

    private Dictionary<ConditionId, Condition> CreateConditions()
    {
        var conditions = new Dictionary<ConditionId, Condition>();

        // Combine all partial condition dictionaries
        foreach (var kvp in CreateConditionsAbc())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsDef())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsGhi())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsJkl())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsMno())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsPqr())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsStu())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsVwx())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsYz())
            conditions[kvp.Key] = kvp.Value;

        return conditions;
    }

    // Partial methods to be implemented in other files
    private partial Dictionary<ConditionId, Condition> CreateConditionsAbc();
    private partial Dictionary<ConditionId, Condition> CreateConditionsDef();
    private partial Dictionary<ConditionId, Condition> CreateConditionsGhi();
    private partial Dictionary<ConditionId, Condition> CreateConditionsJkl();
    private partial Dictionary<ConditionId, Condition> CreateConditionsMno();
    private partial Dictionary<ConditionId, Condition> CreateConditionsPqr();
    private partial Dictionary<ConditionId, Condition> CreateConditionsStu();
    private partial Dictionary<ConditionId, Condition> CreateConditionsVwx();
    private partial Dictionary<ConditionId, Condition> CreateConditionsYz();
}