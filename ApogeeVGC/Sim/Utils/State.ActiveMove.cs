using ApogeeVGC.Sim.BattleClasses;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Utils.Unions;
using System.Reflection;
using System.Text.Json.Nodes;

namespace ApogeeVGC.Sim.Utils;

public static partial class State
{
    public static bool IsActiveMove(JsonObject obj)
    {
        // Simply looking for a 'hit' field to determine if an object is an ActiveMove or not seems
        // pretty fragile, but it's no different than what the simulator is doing. We go further and
        // also check if the object has an 'id', as that's what we will interpret as the Move.
        return obj.ContainsKey("hit") &&
               (obj.ContainsKey("id") || obj.ContainsKey("move"));
    }

    public static JsonObject SerializeActiveMove(ActiveMove activeMove, IBattle battle)
    {
        // ActiveMove is somewhat problematic as it sometimes extends a Move and adds on
        // some mutable fields. We'd like to avoid displaying all the readonly fields of Move
        // (which in theory should not be changed by the ActiveMove...), so we collapse them
        // into a 'move: [Move:...]' reference. If isActiveMove returns a false positive *and*
        // an object contains an 'id' field matching a Move *and* it contains fields with the
        // same name as said Move then we'll miss them during serialization and won't
        // deserialize properly. This is unlikely to be the case, and would probably indicate
        // a bug in the simulator if it ever happened.

        Move baseMove = battle.Library.Moves[activeMove.Id];
        var skip = new HashSet<string>(ActiveMove);

        // Skip fields that haven't changed from the base Move
        // We use reflection to compare properties
        Type activeMoveType = typeof(ActiveMove);
        Type baseMoveType = typeof(Move);
        var baseProperties = baseMoveType.GetProperties(BindingFlags.Public |
                                                        BindingFlags.Instance);

        foreach (PropertyInfo prop in baseProperties)
        {
            if (!prop.CanRead) continue;

            object? baseValue = prop.GetValue(baseMove);
            object? activeValue = prop.GetValue(activeMove);

            // This should really be a deepEquals check to see if anything on ActiveMove was
            // modified from the base Move, but that ends up being expensive and mostly unnecessary
            // as ActiveMove currently only mutates its simple fields (eg. `type`, `target`) anyway.
            bool valuesEqual = false;

            if (baseValue == null && activeValue == null)
            {
                valuesEqual = true;
            }
            else if (baseValue != null && activeValue != null)
            {
                // For complex types (reference types that aren't strings), just check reference equality
                // For value types and strings, use Equals
                if (baseValue.GetType().IsClass && baseValue.GetType() != typeof(string))
                {
                    valuesEqual = ReferenceEquals(baseValue, activeValue);
                }
                else
                {
                    valuesEqual = baseValue.Equals(activeValue);
                }
            }

            if (valuesEqual)
            {
                skip.Add(prop.Name);
            }
        }

        JsonObject state = Serialize(activeMove, skip.ToList(), battle);
        state["move"] = ToRef((Referable)baseMove);

        return state;
    }

    public static ActiveMove DeserializeActiveMove(JsonObject state, IBattle battle)
    {
        // First, get the base move reference
        if (!state.ContainsKey("move"))
        {
            throw new InvalidOperationException("ActiveMove serialization missing 'move' reference");
        }

        string moveRef = state["move"]?.GetValue<string>() ??
            throw new InvalidOperationException("ActiveMove 'move' reference is null");

        ReferableUndefinedUnion referableUnion = FromRef(moveRef, battle);
        Move baseMove = referableUnion switch
        {
            ReferableReferableUndefinedUnion { Referable: MoveReferable m } => m.Move,
            _ => throw new InvalidOperationException($"Invalid move reference: {moveRef}"),
        };

        // Use the Move's ToActiveMove method to create a(n) ActiveMove with all properties copied
        var activeMove = baseMove.ToActiveMove();

        // Now deserialize the changed properties onto the active move
        Deserialize(state, activeMove, ActiveMove.ToList(), battle);

        return activeMove;
    }
}