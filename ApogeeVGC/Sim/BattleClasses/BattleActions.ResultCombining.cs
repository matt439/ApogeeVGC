using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    public static BoolIntUndefinedUnion CombineResults(BoolIntUndefinedUnion? left,
        BoolIntUndefinedUnion? right)
    {
        return CombineResults(
                BoolIntEmptyUndefinedUnion.FromNullableBoolIntUndefinedUnion(left),
                BoolIntEmptyUndefinedUnion.FromNullableBoolIntUndefinedUnion(right)).
            ToBoolIntUndefinedUnion();
    }

    /// <summary>
    /// Combines two move result values based on priority.
    /// Used to aggregate results across multiple targets.
    /// Priority order (highest to lowest): undefined, string (success), null, boolean, number.
    /// When both values are numbers, they are summed.
    /// </summary>
    /// <param name="left">First result value</param>
    /// <param name="right">Second result value</param>
    /// <returns>Combined result with the higher priority, or sum if both are numbers</returns>
    public static BoolIntEmptyUndefinedUnion CombineResults(BoolIntEmptyUndefinedUnion? left,
        BoolIntEmptyUndefinedUnion? right)
    {
        switch (left)
        {
            // Handle null inputs
            case null when right == null:
                return BoolIntEmptyUndefinedUnion.FromUndefined();
            case null:
                return right;
        }

        if (right == null) return left;

        int leftPriority = GetBattleActionsPriority(left);
        int rightPriority = GetBattleActionsPriority(right);

        // If left has higher priority, return it
        if (leftPriority < rightPriority)
        {
            return left;
        }

        // If left is truthy and right is falsy (but not 0)
        // In TS: left && !right && right !== 0
        if (left.IsTruthy() && !right.IsTruthy() && !right.IsZero())
        {
            return left;
        }

        // If both are numbers, sum them
        if (left is IntBoolIntEmptyUndefinedUnion leftInt && right is IntBoolIntEmptyUndefinedUnion rightInt)
        {
            return BoolIntEmptyUndefinedUnion.FromInt(leftInt.Value + rightInt.Value);
        }

        // Otherwise return right
        return right;
    }
}