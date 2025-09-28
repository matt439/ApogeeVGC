using ApogeeVGC.Sim.Core;
using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Data;

public record Events
{
    private readonly Library _library;
    private readonly Dictionary<EventId, Dictionary<EffectIdUnion, Delegate>> _registeredDelegates = new();

    public Events(Library library)
    {
        _library = library;
        InitializeRegistryStructure();
    }

    private void InitializeRegistryStructure()
    {
        // Initialize the registry structure for each EventId
        foreach (EventId eventId in Enum.GetValues<EventId>())
        {
            _registeredDelegates[eventId] = new Dictionary<EffectIdUnion, Delegate>();
        }
    }
    
    /// <summary>
    /// Register a delegate implementation for a specific effect (move, ability, etc.)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The type of event handler</param>
    /// <param name="effectId">The ID of the effect (move ID, ability ID, etc.)</param>
    /// <param name="handler">The actual delegate implementation</param>
    public void RegisterDelegate<T>(EventId eventId, EffectIdUnion effectId, T handler) where T : Delegate
    {
        // Validate the delegate type matches the expected type for this EventId
        Type? expectedType = GetExpectedDelegateType(eventId);
        if (expectedType != null && !expectedType.IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException($"Handler type {typeof(T).Name} does not match expected type" +
                                        $"{expectedType.Name} for {eventId}");
        }
        
        _registeredDelegates[eventId][effectId] = handler;
    }
    
    /// <summary>
    /// Get a registered delegate for a specific effect
    /// </summary>
    /// <typeparam name="T">The delegate type to return</typeparam>
    /// <param name="eventId">The type of event handler</param>
    /// <param name="effectId">The ID of the effect</param>
    /// <returns>The delegate if found, null otherwise</returns>
    public T? GetDelegate<T>(EventId eventId, EffectIdUnion effectId) where T : Delegate
    {
        if (_registeredDelegates.TryGetValue(eventId, out var effectDict) &&
            effectDict.TryGetValue(effectId, out Delegate? handler) &&
            handler is T typedHandler)
        {
            return typedHandler;
        }
        return null;
    }
    
    /// <summary>
    /// Check if a delegate is registered for a specific effect
    /// </summary>
    /// <param name="eventId">The type of event handler</param>
    /// <param name="effectId">The ID of the effect</param>
    /// <returns>True if a delegate is registered</returns>
    public bool HasDelegate(EventId eventId, EffectIdUnion effectId)
    {
        return _registeredDelegates.TryGetValue(eventId, out var effectDict) &&
               effectDict.ContainsKey(effectId);
    }
    
    /// <summary>
    /// Get all registered effect IDs for a specific event type
    /// </summary>
    /// <param name="eventId">The type of event handler</param>
    /// <returns>Collection of effect IDs that have registered delegates</returns>
    public IReadOnlyCollection<EffectIdUnion> GetRegisteredEffects(EventId eventId)
    {
        if (_registeredDelegates.TryGetValue(eventId, out var effectDict))
        {
            return effectDict.Keys.ToList().AsReadOnly();
        }
        return [];
    }
    
    /// <summary>
    /// Remove a registered delegate
    /// </summary>
    /// <param name="eventId">The type of event handler</param>
    /// <param name="effectId">The ID of the effect</param>
    /// <returns>True if the delegate was removed</returns>
    public bool UnregisterDelegate(EventId eventId, EffectIdUnion effectId)
    {
        return _registeredDelegates.TryGetValue(eventId, out var effectDict) &&
               effectDict.Remove(effectId);
    }
    
    /// <summary>
    /// Get the expected delegate type for an EventId
    /// </summary>
    /// <param name="eventId">The event ID</param>
    /// <returns>The expected delegate type</returns>
    private static Type? GetExpectedDelegateType(EventId eventId)
    {
        return eventId switch
        {
            // CommonHandlers delegate types
            EventId.ModifierEffectHandler => typeof(ModifierEffectHandler),
            EventId.ModifierMoveHandler => typeof(ModifierMoveHandler),
            EventId.ResultMoveHandler => typeof(ResultMoveHandler),
            EventId.ExtResultMoveHandler => typeof(ExtResultMoveHandler),
            EventId.VoidEffectHandler => typeof(VoidEffectHandler),
            EventId.VoidMoveHandler => typeof(VoidMoveHandler),
            EventId.ModifierSourceEffectHandler => typeof(ModifierSourceEffectHandler),
            EventId.ModifierSourceMoveHandler => typeof(ModifierSourceMoveHandler),
            EventId.ResultSourceMoveHandler => typeof(ResultSourceMoveHandler),
            EventId.ExtResultSourceMoveHandler => typeof(ExtResultSourceMoveHandler),
            EventId.VoidSourceEffectHandler => typeof(VoidSourceEffectHandler),
            EventId.VoidSourceMoveHandler => typeof(VoidSourceMoveHandler),
            
            // Move-specific delegate types
            EventId.BasePowerCallback => typeof(BasePowerCallbackHandler),
            EventId.BeforeMoveCallback => typeof(BeforeMoveCallbackHandler),
            EventId.BeforeTurnCallback => typeof(BeforeTurnCallbackHandler),
            EventId.DamageCallback => typeof(DamageCallbackHandler),
            EventId.PriorityChargeCallback => typeof(PriorityChargeCallbackHandler),
            EventId.OnDisableMove => typeof(OnDisableMoveHandler),
            EventId.OnAfterSubDamage => typeof(OnAfterSubDamageHandler),
            EventId.OnDamage => typeof(OnDamageHandler),
            EventId.OnEffectiveness => typeof(OnEffectivenessHandler),
            EventId.OnHitSide => typeof(OnHitSideHandler),
            EventId.OnModifyMove => typeof(OnModifyMoveHandler),
            EventId.OnModifyType => typeof(OnModifyTypeHandler),
            EventId.OnModifyTarget => typeof(OnModifyTargetHandler),
            EventId.OnTryHitSide => typeof(OnTryHitSideHandler),
            EventId.OnMoveFail => typeof(OnMoveFailHandler),
            EventId.OnUseMoveMessage => typeof(OnUseMoveMessageHandler),
            EventId.OnTryHitField => typeof(OnTryHitFieldHandler),
            EventId.OnTryImmunity => typeof(OnTryImmunityHandler),
            EventId.OnTry => typeof(OnTryHandler),
            EventId.OnTryHit => typeof(OnTryHitHandler),
            EventId.OnPrepareHit => typeof(OnPrepareHitHandler),
            EventId.OnTryMove => typeof(OnTryMoveHandler),
            
            _ => null,
        };
    }
}

#region Common Handlers

// Common handler delegate types
public delegate int? ModifierEffectHandler(BattleContext battle, int relayVar, Pokemon target,
    Pokemon source, IEffect effect);

public delegate int? ModifierMoveHandler(BattleContext battle, int relayVar, Pokemon target,
    Pokemon source, Move move);

public delegate bool? ResultMoveHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate IntBoolUnion? ExtResultMoveHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate void VoidEffectHandler(BattleContext battle, Pokemon target, Pokemon source, IEffect effect);
public delegate void VoidMoveHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);

// Source-based variants (source comes first in parameters)
public delegate int? ModifierSourceEffectHandler(BattleContext battle, int relayVar, Pokemon source,
    Pokemon target, IEffect effect);

public delegate int? ModifierSourceMoveHandler(BattleContext battle, int relayVar, Pokemon source,
    Pokemon target, Move move);

public delegate bool? ResultSourceMoveHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);
public delegate IntBoolUnion? ExtResultSourceMoveHandler(BattleContext battle, Pokemon source,
    Pokemon target, Move move);

public delegate void VoidSourceEffectHandler(BattleContext battle, Pokemon source, Pokemon target, IEffect effect);
public delegate void VoidSourceMoveHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);

#endregion

#region Move-Specific Event Handlers

// Move callback delegates (from the original IMoveEventMethods interface)
public delegate IntFalseUnion? BasePowerCallbackHandler(BattleContext battle, Pokemon pokemon, Pokemon target, Move move);
public delegate bool? BeforeMoveCallbackHandler(BattleContext battle, Pokemon pokemon, Pokemon? target, Move move);
public delegate void BeforeTurnCallbackHandler(BattleContext battle, Pokemon pokemon, Pokemon target, Move move);
public delegate IntFalseUnion? DamageCallbackHandler(BattleContext battle, Pokemon pokemon, Pokemon target, Move move);
public delegate void PriorityChargeCallbackHandler(BattleContext battle, Pokemon pokemon);

// Move event delegates
public delegate void OnDisableMoveHandler(BattleContext battle, Pokemon pokemon);
public delegate void OnAfterSubDamageHandler(BattleContext battle, int damage, Pokemon target, Pokemon source, Move move);
public delegate IntBoolUnion? OnDamageHandler(BattleContext battle, int damage, Pokemon target, Pokemon source, IEffect effect);
public delegate int? OnEffectivenessHandler(BattleContext battle, int typeMod, Pokemon? target, string type, Move move);
public delegate bool? OnHitSideHandler(BattleContext battle, Side side, Pokemon source, Move move);
public delegate void OnModifyMoveHandler(BattleContext battle, Move move, Pokemon pokemon, Pokemon? target);
public delegate void OnModifyTypeHandler(BattleContext battle, Move move, Pokemon pokemon, Pokemon target);
public delegate void OnModifyTargetHandler(BattleContext battle, Pokemon relayTarget, Pokemon pokemon, Pokemon target, Move move);
public delegate bool? OnTryHitSideHandler(BattleContext battle, Side side, Pokemon source, Move move);

// Additional move event delegates
public delegate void OnMoveFailHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate void OnUseMoveMessageHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);
public delegate bool? OnTryHitFieldHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate bool? OnTryImmunityHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate bool? OnTryHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);
public delegate IntBoolUnion? OnTryHitHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);
public delegate bool? OnPrepareHitHandler(BattleContext battle, Pokemon target, Pokemon source, Move move);
public delegate bool? OnTryMoveHandler(BattleContext battle, Pokemon source, Pokemon target, Move move);

#endregion