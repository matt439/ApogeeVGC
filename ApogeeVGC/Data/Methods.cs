//using ApogeeVGC.Sim.GameObjects;
//using System.Collections.ObjectModel;
//using ApogeeVGC.Sim.Core;
//using ApogeeVGC.Sim.Effects;
//using ApogeeVGC.Sim.Moves;
//using ApogeeVGC.Sim.PokemonClasses;

//namespace ApogeeVGC.Data;

///// <summary>int | bool</summary>
//public abstract record IntBoolUnion
//{
//    public static implicit operator IntBoolUnion(int value) => new IntIntBoolUnion(value);
//    public static implicit operator IntBoolUnion(bool value) => new BoolIntBoolUnion(value);
//}
//public record IntIntBoolUnion(int Value) : IntBoolUnion;
//public record BoolIntBoolUnion(bool Value) : IntBoolUnion;

//public enum CommonHandlerId
//{
//    ModifierEffect,
//    ModifierMove,
//    ResultMove,
//    ExtResultMove,
//    VoidEffect,
//    VoidMove,
//    ModifierSourceEffect,
//    ModifierSourceMove,
//    ResultSourceMove,
//    ExtResultSourceMove,
//    VoidSourceEffect,
//    VoidSourceMove,
//}

///// <summary>
///// Common handler delegate signatures for Pokemon battle effects and moves.
///// Provides standardized function signatures for various battle event handlers.
///// </summary>
//public interface ICommonHandlers
//{
//    /// <summary>battle, relayVar, target, source, effect</summary>
//    /// <returns>The modified value, or null if no modification should occur</returns>
//    public Func<Battle, int, Pokemon, Pokemon, IEffect, int?> ModifierEffect { get; }

//    /// <summary>battle, relayVar, target, source, move</summary>
//    /// <returns>The modified value, or null if no modification should occur</returns>
//    public Func<Battle, int, Pokemon, Pokemon, Move, int?> ModifierMove { get; }

//    /// <summary>battle, target, source, move</summary>
//    /// <returns>True/false for the result, or null if no override should occur</returns>
//    public Func<Battle, Pokemon, Pokemon, Move, bool?> ResultMove { get; }

//    /// <summary>battle, target, source, move</summary>
//    /// <returns>Boolean, integer, or null result depending on the specific interaction</returns>
//    public Func<Battle, Pokemon, Pokemon, Move, IntBoolUnion?> ExtResultMove { get; }

//    /// <summary>battle, target, source, effect</summary>
//    public Action<Battle, Pokemon, Pokemon, IEffect> VoidEffect { get; }

//    /// <summary>battle, target, source, move</summary>
//    public Action<Battle, Pokemon, Pokemon, Move> VoidMove { get; }

//    /// <summary>battle, relayVar, source, target, effect</summary>
//    /// <returns>The modified value, or null if no modification should occur</returns>
//    public Func<Battle, int, Pokemon, Pokemon, IEffect, int?> ModifierSourceEffect { get; }

//    /// <summary>battle, relayVar, source, target, move</summary>
//    /// <returns>The modified value, or null if no modification should occur</returns>
//    public Func<Battle, int, Pokemon, Pokemon, Move, int?> ModifierSourceMove { get; }

//    /// <summary>battle, source, target, move</summary>
//    /// <returns>True/false for the result, or null if no override should occur</returns>
//    public Func<Battle, Pokemon, Pokemon, Move, bool?> ResultSourceMove { get; }

//    /// <summary>battle, source, target, move</summary>
//    /// <returns>Boolean, integer, or null result depending on the specific interaction</returns>
//    public Func<Battle, Pokemon, Pokemon, Move, IntBoolUnion?> ExtResultSourceMove { get; }

//    /// <summary>battle, source, target, effect</summary>
//    public Action<Battle, Pokemon, Pokemon, IEffect> VoidSourceEffect { get; }

//    /// <summary>battle, source, target, move</summary>
//    public Action<Battle, Pokemon, Pokemon, Move> VoidSourceMove { get; }
//}

///// <summary>
///// Centralized methods registry that provides common handler delegates accessible by enum keys.
///// This reduces code duplication when different classes need the same delegate types.
///// </summary>
//public class Methods
//{
//    public IReadOnlyDictionary<CommonHandlerId, Delegate> MethodsData { get; }
//    private readonly Library _library;
//    private readonly Dictionary<CommonHandlerId, Delegate> _commonHandlers;

//    public Methods(Library library)
//    {
//        _library = library;
//        MethodsData = new ReadOnlyDictionary<CommonHandlerId, Delegate>(CreateCommonHandlers());
//        _commonHandlers = CreateCommonHandlers();
//    }

//    /// <summary>
//    /// Gets a common handler delegate by its identifier
//    /// </summary>
//    /// <typeparam name="T">The delegate type to cast to</typeparam>
//    /// <param name="handlerId">The handler identifier</param>
//    /// <returns>The delegate cast to the specified type</returns>
//    /// <exception cref="ArgumentException">Thrown when handler ID is not found or cast fails</exception>
//    public T GetHandler<T>(CommonHandlerId handlerId) where T : Delegate
//    {
//        if (!_commonHandlers.TryGetValue(handlerId, out Delegate? handler))
//        {
//            throw new ArgumentException($"Handler with ID {handlerId} not found", nameof(handlerId));
//        }

//        if (handler is not T typedHandler)
//        {
//            throw new ArgumentException($"Handler {handlerId} cannot be cast to {typeof(T).Name}", nameof(handlerId));
//        }

//        return typedHandler;
//    }

//    /// <summary>
//    /// Checks if a handler exists for the given identifier
//    /// </summary>
//    /// <param name="handlerId">The handler identifier to check</param>
//    /// <returns>True if the handler exists, false otherwise</returns>
//    public bool HasHandler(CommonHandlerId handlerId)
//    {
//        return _commonHandlers.ContainsKey(handlerId);
//    }

//    /// <summary>
//    /// Gets all available handler identifiers
//    /// </summary>
//    /// <returns>Collection of all registered handler IDs</returns>
//    public IEnumerable<CommonHandlerId> GetAllHandlerIds()
//    {
//        return _commonHandlers.Keys;
//    }

//    /// <summary>
//    /// Creates the dictionary of common handlers
//    /// </summary>
//    /// <returns>Dictionary mapping handler IDs to delegate instances</returns>
//    private Dictionary<CommonHandlerId, Delegate> CreateCommonHandlers()
//    {
//        return new Dictionary<CommonHandlerId, Delegate>
//        {
//            // Target-focused handlers
//            [CommonHandlerId.ModifierEffect] = CreateModifierEffectHandler(),
//            [CommonHandlerId.ModifierMove] = CreateModifierMoveHandler(),
//            [CommonHandlerId.ResultMove] = CreateResultMoveHandler(),
//            [CommonHandlerId.ExtResultMove] = CreateExtResultMoveHandler(),
//            [CommonHandlerId.VoidEffect] = CreateVoidEffectHandler(),
//            [CommonHandlerId.VoidMove] = CreateVoidMoveHandler(),

//            // Source-focused handlers
//            [CommonHandlerId.ModifierSourceEffect] = CreateModifierSourceEffectHandler(),
//            [CommonHandlerId.ModifierSourceMove] = CreateModifierSourceMoveHandler(),
//            [CommonHandlerId.ResultSourceMove] = CreateResultSourceMoveHandler(),
//            [CommonHandlerId.ExtResultSourceMove] = CreateExtResultSourceMoveHandler(),
//            [CommonHandlerId.VoidSourceEffect] = CreateVoidSourceEffectHandler(),
//            [CommonHandlerId.VoidSourceMove] = CreateVoidSourceMoveHandler(),
//        };
//    }

//    // Factory methods for creating handler delegates
//    private Func<Battle, int, Pokemon, Pokemon, IEffect, int?> CreateModifierEffectHandler()
//    {
//        return (battle, relayVar, target, source, effect) =>
//        {
//            // Default implementation - can be overridden by specific effects
//            return null; // No modification by default
//        };
//    }

//    private Func<Battle, int, Pokemon, Pokemon, Move, int?> CreateModifierMoveHandler()
//    {
//        return (battle, relayVar, target, source, move) =>
//        {
//            // Default implementation - can be overridden by specific moves
//            return null; // No modification by default
//        };
//    }

//    private Func<Battle, Pokemon, Pokemon, Move, bool?> CreateResultMoveHandler()
//    {
//        return (battle, target, source, move) =>
//        {
//            // Default implementation - can be overridden by specific moves
//            return null; // No override by default
//        };
//    }

//    private Func<Battle, Pokemon, Pokemon, Move, IntBoolUnion?> CreateExtResultMoveHandler()
//    {
//        return (battle, target, source, move) =>
//        {
//            // Default implementation - can be overridden by specific moves
//            return null; // No result by default
//        };
//    }

//    private Action<Battle, Pokemon, Pokemon, IEffect> CreateVoidEffectHandler()
//    {
//        return (battle, target, source, effect) =>
//        {
//            // Default implementation - can be overridden by specific effects
//            // No action by default
//        };
//    }

//    private Action<Battle, Pokemon, Pokemon, Move> CreateVoidMoveHandler()
//    {
//        return (battle, target, source, move) =>
//        {
//            // Default implementation - can be overridden by specific moves
//            // No action by default
//        };
//    }

//    // Source-focused handler factory methods
//    private Func<Battle, int, Pokemon, Pokemon, IEffect, int?> CreateModifierSourceEffectHandler()
//    {
//        return (battle, relayVar, source, target, effect) =>
//        {
//            return null; // No modification by default
//        };
//    }

//    private Func<Battle, int, Pokemon, Pokemon, Move, int?> CreateModifierSourceMoveHandler()
//    {
//        return (battle, relayVar, source, target, move) =>
//        {
//            return null; // No modification by default
//        };
//    }

//    private Func<Battle, Pokemon, Pokemon, Move, bool?> CreateResultSourceMoveHandler()
//    {
//        return (battle, source, target, move) =>
//        {
//            return null; // No override by default
//        };
//    }

//    private Func<Battle, Pokemon, Pokemon, Move, IntBoolUnion?> CreateExtResultSourceMoveHandler()
//    {
//        return (battle, source, target, move) =>
//        {
//            return null; // No result by default
//        };
//    }

//    private Action<Battle, Pokemon, Pokemon, IEffect> CreateVoidSourceEffectHandler()
//    {
//        return (battle, source, target, effect) =>
//        {
//            // No action by default
//        };
//    }

//    private Action<Battle, Pokemon, Pokemon, Move> CreateVoidSourceMoveHandler()
//    {
//        return (battle, source, target, move) =>
//        {
//            // No action by default
//        };
//    }

//    private Dictionary<AbilityId, Ability> CreateAbilities()
//    {
//        return [];
//    }
//}

///// <summary>
///// Extension methods for easier access to common handlers
///// </summary>
//public static class MethodsExtensions
//{
//    /// <summary>
//    /// Gets a modifier effect handler from the methods registry
//    /// </summary>
//    public static Func<Battle, int, Pokemon, Pokemon, IEffect, int?> GetModifierEffectHandler(this Methods methods)
//    {
//        return methods.GetHandler<Func<Battle, int, Pokemon, Pokemon, IEffect, int?>>(CommonHandlerId.ModifierEffect);
//    }

//    /// <summary>
//    /// Gets a modifier move handler from the methods registry
//    /// </summary>
//    public static Func<Battle, int, Pokemon, Pokemon, Move, int?> GetModifierMoveHandler(this Methods methods)
//    {
//        return methods.GetHandler<Func<Battle, int, Pokemon, Pokemon, Move, int?>>(CommonHandlerId.ModifierMove);
//    }

//    /// <summary>
//    /// Gets a result move handler from the methods registry
//    /// </summary>
//    public static Func<Battle, Pokemon, Pokemon, Move, bool?> GetResultMoveHandler(this Methods methods)
//    {
//        return methods.GetHandler<Func<Battle, Pokemon, Pokemon, Move, bool?>>(CommonHandlerId.ResultMove);
//    }

//    // Add more extension methods as needed for other handler types...
//}