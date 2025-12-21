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
        foreach (var kvp in CreateConditionsABC())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsDEF())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsGHI())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsJKL())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsMNO())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsPQR())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsSTU())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsVWX())
            conditions[kvp.Key] = kvp.Value;

        foreach (var kvp in CreateConditionsYZ())
            conditions[kvp.Key] = kvp.Value;

        return conditions;
    }

    // Partial methods to be implemented in other files
    private partial Dictionary<ConditionId, Condition> CreateConditionsABC();
    private partial Dictionary<ConditionId, Condition> CreateConditionsDEF();
    private partial Dictionary<ConditionId, Condition> CreateConditionsGHI();
    private partial Dictionary<ConditionId, Condition> CreateConditionsJKL();
    private partial Dictionary<ConditionId, Condition> CreateConditionsMNO();
    private partial Dictionary<ConditionId, Condition> CreateConditionsPQR();
    private partial Dictionary<ConditionId, Condition> CreateConditionsSTU();
    private partial Dictionary<ConditionId, Condition> CreateConditionsVWX();
    private partial Dictionary<ConditionId, Condition> CreateConditionsYZ();
}
