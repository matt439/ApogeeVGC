using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class BattleActions
{
    public static BoolIntUndefinedUnion CombineResults(BoolIntUndefinedUnion? left,
        BoolIntUndefinedUnion? right)
    {
        // Treat C# null and NullBoolIntUndefinedUnion as JS null
        bool leftIsJsNull = left is null or NullBoolIntUndefinedUnion;
        bool rightIsJsNull = right is null or NullBoolIntUndefinedUnion;

        // Both JS-null → JS null
        if (leftIsJsNull && rightIsJsNull) return NullBoolIntUndefinedUnion.Instance;

        // Left JS-null → return right (JS: return right)
        if (leftIsJsNull) return right!;

        // Right JS-null → if left is truthy return left, else return JS null
        // Matches JS: `if (left && !right && right !== 0) return left;` then `return right;`
        if (rightIsJsNull) return left!.IsTruthy() ? left! : NullBoolIntUndefinedUnion.Instance;

        // Neither is null: standard combineResults logic
        // If left is truthy and right is falsy (but not 0)
        if (left!.IsTruthy() && !right!.IsTruthy() && !right!.IsZero())
        {
            return left;
        }

        // If both are numbers, sum them
        if (left is IntBoolIntUndefinedUnion leftInt && right is IntBoolIntUndefinedUnion rightInt)
        {
            return BoolIntUndefinedUnion.FromInt(leftInt.Value + rightInt.Value);
        }

        // Otherwise return right
        return right!;
    }

    /// <summary>
    /// Combines two move result values following Showdown's combineResults semantics.
    /// In Showdown, the dominated/indexOf(typeof) check is dead code (it compares type strings
    /// against a value array, always returning -1). The effective logic is:
    /// 1. If left is truthy and right is falsy (not 0), return left
    /// 2. If both are numbers, sum them
    /// 3. Otherwise return right
    /// </summary>
    public static BoolIntEmptyUndefinedUnion CombineResults(BoolIntEmptyUndefinedUnion? left,
        BoolIntEmptyUndefinedUnion? right)
    {
        switch (left)
        {
            // Handle null inputs
            // When both are null (NOT_FAIL/continue), return Empty to represent NOT_FAIL
            // This preserves the semantic meaning that neither result failed
            case null when right == null:
                return BoolIntEmptyUndefinedUnion.FromEmpty();
            case null:
                return right;
        }

        if (right == null) return left;

        // Note: Showdown's dominated/indexOf(typeof) priority check is dead code —
        // it uses indexOf(typeof value) on a value array, always returning -1.
        // We intentionally omit it here to match Showdown's actual behavior.

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