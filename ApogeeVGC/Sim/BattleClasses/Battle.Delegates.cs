using System.Collections.Concurrent;
using System.Reflection;
using ApogeeVGC.Sim.Utils.Unions;

namespace ApogeeVGC.Sim.BattleClasses;

public partial class Battle
{
    /// <summary>
    /// Cache for delegate parameter info to avoid repeated reflection calls.
    /// 
    /// NOTE: This is now only used for legacy handlers during migration.
    /// New handlers should use EventContext instead.
    /// </summary>
    [Obsolete(
        "Use EventContext-based handlers instead. This will be removed after migration is complete.")]
    private static readonly ConcurrentDictionary<MethodInfo, ParameterInfo[]> ParameterInfoCache =
        new();

    /// <summary>
    /// Converts various return value types from event handlers to RelayVar.
    /// 
    /// NOTE: This is now handled by EventHandlerAdapter for legacy handlers.
    /// New context-based handlers return RelayVar directly.
    /// </summary>
    [Obsolete("Use EventContext-based handlers that return RelayVar directly.")]
    private RelayVar? ConvertReturnValueToRelayVar(object? returnValue)
    {
        if (returnValue == null)
        {
            return null;
        }

        // If it's already a RelayVar, return it directly
        if (returnValue is RelayVar relayVar)
        {
            return relayVar;
        }

        // Convert common union types to RelayVar
        return returnValue switch
        {
            // BoolVoidUnion -> bool or null
            BoolBoolVoidUnion boolVoid => new BoolRelayVar(boolVoid.Value),
            VoidBoolVoidUnion => null,

            // BoolEmptyVoidUnion -> bool or null
            BoolBoolEmptyVoidUnion boolEmptyVoid => new BoolRelayVar(boolEmptyVoid.Value),
            EmptyBoolEmptyVoidUnion => null,
            VoidUnionBoolEmptyVoidUnion => null,

            // IntBoolVoidUnion -> int, bool, or null
            IntIntBoolVoidUnion intBoolVoid => new IntRelayVar(intBoolVoid.Value),
            BoolIntBoolVoidUnion boolIntVoid => new BoolRelayVar(boolIntVoid.Value),
            VoidIntBoolVoidUnion => null,

            // BoolIntEmptyVoidUnion -> bool, int, or null
            BoolBoolIntEmptyVoidUnion boolIntEmptyVoid => new BoolRelayVar(boolIntEmptyVoid.Value),
            IntBoolIntEmptyVoidUnion intBoolEmptyVoid => new IntRelayVar(intBoolEmptyVoid.Value),
            EmptyBoolIntEmptyVoidUnion => null,
            VoidUnionBoolIntEmptyVoidUnion => null,

            // BoolUndefinedUnion -> bool or undefined
            BoolBoolUndefinedUnion boolUndefined => new BoolRelayVar(boolUndefined.Value),
            UndefinedBoolUndefinedUnion => new UndefinedRelayVar(),

            // BoolIntUndefinedUnion -> bool, int, or undefined
            BoolBoolIntUndefinedUnion boolIntUndefined => new BoolRelayVar(boolIntUndefined.Value),
            IntBoolIntUndefinedUnion intIntUndefined => new IntRelayVar(intIntUndefined.Value),
            UndefinedBoolIntUndefinedUnion => new UndefinedRelayVar(),

            // IntFalseUndefinedUnion -> int, false, or undefined (used by Damage method)
            IntIntFalseUndefined intFalseUndefined => new IntRelayVar(intFalseUndefined.Value),
            FalseIntFalseUndefined => new BoolRelayVar(false),
            UndefinedIntFalseUndefined => new UndefinedRelayVar(),

            // IntFalseUnion -> int or false (used by Heal method)
            IntIntFalseUnion intFalse => new IntRelayVar(intFalse.Value),
            FalseIntFalseUnion => new BoolRelayVar(false),

            // IntBoolUnion -> int or bool
            IntIntBoolUnion intBool => new IntRelayVar(intBool.Value),
            BoolIntBoolUnion boolBool => new BoolRelayVar(boolBool.Value),

// IntVoidUnion -> int or null
            IntIntVoidUnion intVoid => new IntRelayVar(intVoid.Value),
            VoidIntVoidUnion => null,

            // DoubleVoidUnion -> double or null
            DoubleDoubleVoidUnion doubleVoid => new DecimalRelayVar((decimal)doubleVoid.Value),
            VoidDoubleVoidUnion => null,

            // MoveIdVoidUnion -> MoveId or null
            MoveIdMoveIdVoidUnion moveIdVoid => new MoveIdRelayVar(moveIdVoid.MoveId),
            VoidMoveIdVoidUnion => null,

            // PokemonVoidUnion -> Pokemon or null
            PokemonPokemonVoidUnion pokemonVoid => new PokemonRelayVar(pokemonVoid.Pokemon),
            VoidPokemonVoidUnion => null,

            // Primitive types
            bool boolValue => new BoolRelayVar(boolValue),
            int intValue => new IntRelayVar(intValue),
            decimal decValue => new DecimalRelayVar(decValue),
            double doubleValue => new DecimalRelayVar((decimal)doubleValue),

            // VoidReturn means no value (return null)
            VoidReturn => null,

            // Undefined means undefined (special RelayVar)
            Undefined => new UndefinedRelayVar(),

            // If we don't recognize the type, throw an informative error
            _ => throw new InvalidOperationException(
                $"Unable to convert return value of type '{returnValue.GetType().Name}' to RelayVar. " +
                $"Event handler returned an unexpected type that needs to be handled in ConvertReturnValueToRelayVar. " +
                $"This usually means a new union type was added but the conversion case wasn't added.")
        };
    }

    /// <summary>
    /// Checks if a parameter type can accept a RelayVar value (like int, decimal, etc.)
    /// 
    /// NOTE: This is now handled by EventHandlerAdapter for legacy handlers.
    /// </summary>
    [Obsolete("Use EventContext-based handlers instead.")]
    private static bool IsRelayVarCompatibleType(Type paramType, RelayVar relayVar)
    {
        return relayVar switch
        {
            IntRelayVar when paramType == typeof(int) => true,
            DecimalRelayVar when paramType == typeof(decimal) => true,
            BoolRelayVar when paramType == typeof(bool) => true,
            _ => false
        };
    }

    /// <summary>
    /// Extracts the actual value from a RelayVar based on the expected parameter type.
    /// 
    /// NOTE: This is now handled by EventHandlerAdapter for legacy handlers.
    /// </summary>
    [Obsolete("Use EventContext-based handlers instead.")]
    private static object ExtractValueFromRelayVar(RelayVar relayVar, Type paramType)
    {
        return relayVar switch
        {
            IntRelayVar intVar when paramType == typeof(int) => intVar.Value,
            DecimalRelayVar decVar when paramType == typeof(decimal) => decVar.Value,
            BoolRelayVar boolVar when paramType == typeof(bool) => boolVar.Value,
            _ => relayVar // Return the RelayVar itself if no conversion needed
        };
    }
}