using System.Collections.ObjectModel;
using ApogeeVGC.Sim.Conditions;

namespace ApogeeVGC.Data.Conditions;

public partial record Conditions
{
    public IReadOnlyDictionary<ConditionId, Condition> ConditionsData { get; }
    private readonly Library _library;

    public Conditions(Library library)
    {
        _library = library;
        ConditionsData = new ReadOnlyDictionary<ConditionId, Condition>(CreateConditions());
    }

    private Dictionary<ConditionId, Condition> CreateConditions()
    {
        var conditions = new Dictionary<ConditionId, Condition>();

        // Combine all partial condition dictionaries
        foreach (var kvp in CreateConditionsBCEF())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsLNPQ())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsRST())
            conditions[kvp.Key] = kvp.Value;

        return conditions;
    }

    // Partial methods to be implemented in other files
    private partial Dictionary<ConditionId, Condition> CreateConditionsBCEF();
    private partial Dictionary<ConditionId, Condition> CreateConditionsLNPQ();
    private partial Dictionary<ConditionId, Condition> CreateConditionsRST();
}
