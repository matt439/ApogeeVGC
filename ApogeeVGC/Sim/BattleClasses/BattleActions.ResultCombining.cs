using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    /// <summary>
    /// Showdown's type priority for combineResults. Maps JS typeof strings to indices:
    /// ['undefined' (0), 'string'/NOT_FAIL (1), 'object'/null (2), 'boolean' (3), 'number' (4)]
    /// If left has a higher type priority than right, left wins regardless of truthiness.
    /// This is NOT dead code — it handles cases like combineResults(0, false) where
    /// 0 (number, priority 4) outranks false (boolean, priority 3), returning 0.
    /// </summary>
    private static int GetJsTypePriority(BoolIntUndefinedUnion? value)
    {
        return value switch
        {
            UndefinedBoolIntUndefinedUnion => 0,  // typeof undefined = 'undefined'
            null or NullBoolIntUndefinedUnion => 2, // typeof null = 'object'
            BoolBoolIntUndefinedUnion => 3,        // typeof false/true = 'boolean'
            IntBoolIntUndefinedUnion => 4,          // typeof 0/42 = 'number'
            _ => -1
        };
    }

    /// <summary>
    /// Combines two move result values following Showdown's combineResults semantics.
    /// The full logic is:
    /// 1. Type priority: if left's JS typeof has higher priority, return left
    /// 2. Truthy check: if left is truthy and right is falsy (not 0), return left
    /// 3. Both numbers: sum them
    /// 4. Otherwise: return right
    /// </summary>
    public static BoolIntUndefinedUnion CombineResults(BoolIntUndefinedUnion? left,
        BoolIntUndefinedUnion? right)
    {
        // Step 1: Type priority check (Showdown's resultsPriorities indexOf comparison)
        int leftPriority = GetJsTypePriority(left);
        int rightPriority = GetJsTypePriority(right);

        if (leftPriority > rightPriority)
        {
            return left ?? NullBoolIntUndefinedUnion.Instance;
        }

        // Step 2: If left is truthy and right is falsy (but not 0)
        // In JS: left && !right && right !== 0
        bool leftTruthy = left is not (null or NullBoolIntUndefinedUnion) && left.IsTruthy();
        bool rightFalsyNonZero = right switch
        {
            null or NullBoolIntUndefinedUnion => true,  // null is falsy and not 0
            _ => !right.IsTruthy() && !right.IsZero()
        };

        if (leftTruthy && rightFalsyNonZero)
        {
            return left!;
        }

        // Step 3: If both are numbers, sum them
        if (left is IntBoolIntUndefinedUnion leftInt && right is IntBoolIntUndefinedUnion rightInt)
        {
            return BoolIntUndefinedUnion.FromInt(leftInt.Value + rightInt.Value);
        }

        // Step 4: Otherwise return right
        return right ?? NullBoolIntUndefinedUnion.Instance;
    }

    /// <summary>
    /// Showdown's type priority for combineResults with Empty (NOT_FAIL) union.
    /// </summary>
    private static int GetJsTypePriority(BoolIntEmptyUndefinedUnion? value)
    {
        return value switch
        {
            UndefinedBoolIntEmptyUndefinedUnion => 0,   // typeof undefined = 'undefined'
            EmptyBoolIntEmptyUndefinedUnion => 1,       // typeof '' = 'string' (NOT_FAIL)
            null => 2,                                   // typeof null = 'object'
            BoolBoolIntEmptyUndefinedUnion => 3,        // typeof false/true = 'boolean'
            IntBoolIntEmptyUndefinedUnion => 4,          // typeof 0/42 = 'number'
            _ => -1
        };
    }

    /// <summary>
    /// Combines two move result values following Showdown's combineResults semantics.
    /// Same logic as the BoolIntUndefinedUnion overload but with Empty (NOT_FAIL) support.
    /// </summary>
    public static BoolIntEmptyUndefinedUnion CombineResults(BoolIntEmptyUndefinedUnion? left,
        BoolIntEmptyUndefinedUnion? right)
    {
        // Step 1: Type priority check
        int leftPriority = GetJsTypePriority(left);
        int rightPriority = GetJsTypePriority(right);

        if (leftPriority > rightPriority)
        {
            return left ?? BoolIntEmptyUndefinedUnion.FromEmpty();
        }

        // Step 2: If left is truthy and right is falsy (but not 0)
        bool leftTruthy = left != null && left.IsTruthy();
        bool rightFalsyNonZero = right == null || (!right.IsTruthy() && !right.IsZero());

        if (leftTruthy && rightFalsyNonZero)
        {
            return left!;
        }

        // Step 3: If both are numbers, sum them
        if (left is IntBoolIntEmptyUndefinedUnion leftInt && right is IntBoolIntEmptyUndefinedUnion rightInt)
        {
            return BoolIntEmptyUndefinedUnion.FromInt(leftInt.Value + rightInt.Value);
        }

        // Step 4: Otherwise return right
        return right ?? BoolIntEmptyUndefinedUnion.FromEmpty();
    }
}
