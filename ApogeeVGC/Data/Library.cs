using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Types;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.Utils;

namespace ApogeeVGC.Data;

public record Library
{
    private readonly Abilities _abilities;
    private readonly Conditions _conditions = new();
    private readonly Events _events;
    private readonly Items _items;
    private readonly Learnsets _learnsets = new();
    private readonly Moves _moves;
    private readonly Natures _natures = new();
    private readonly PseudoWeathers _pseudoWeathers;
    private readonly Rulesets _rulesets = new();
    private readonly SideConditions _sideConditions;
    private readonly Species _species = new();
    private readonly SpeciesFormats _speciesFormats = new();
    private readonly Tags _tags = new();
    private readonly Terrains _terrains;
    private readonly Weathers _weathers;

    // Keep the original read-only dictionaries for internal use
    private IReadOnlyDictionary<AbilityId, Ability> AbilitiesData => _abilities.AbilitiesData;
    private IReadOnlyDictionary<ConditionId, Condition> ConditionsData => _conditions.ConditionsData;
    private IReadOnlyDictionary<ItemId, Item> ItemsData => _items.ItemsData;
    private IReadOnlyDictionary<SpecieId, Learnset> LearnsetsData => _learnsets.LearnsetsData;
    private IReadOnlyDictionary<MoveId, Move> MovesData => _moves.MovesData;
    private IReadOnlyDictionary<NatureType, Nature> NaturesData => _natures.NatureData;
    private IReadOnlyDictionary<PseudoWeatherId, PseudoWeather> PseudoWeathersData => _pseudoWeathers.PseudoWeatherData;
    private IReadOnlyDictionary<SideConditionId, SideCondition> SideConditionsData => _sideConditions.SideConditionData;
    private IReadOnlyDictionary<SpecieId, Specie> SpeciesData => _species.SpeciesData;
    private IReadOnlyDictionary<SpecieId, SpeciesFormat> SpeciesFormatsData => _speciesFormats.SpeciesFormatsData;
    private IReadOnlyDictionary<TerrainId, Terrain> TerrainsData => _terrains.TerrainData;
    private IReadOnlyDictionary<WeatherId, Weather> WeathersData => _weathers.WeatherData;

    // Public accessor properties that return copies
    public IReadOnlyDictionary<AbilityId, Ability> Abilities => 
        new ReadOnlyDictionaryWrapper<AbilityId, Ability>(AbilitiesData);
    
    public IReadOnlyDictionary<ConditionId, Condition> Conditions => 
        new ReadOnlyDictionaryWrapper<ConditionId, Condition>(ConditionsData);
    
    public IReadOnlyDictionary<ItemId, Item> Items => 
        new ReadOnlyDictionaryWrapper<ItemId, Item>(ItemsData);
    
    public IReadOnlyDictionary<SpecieId, Learnset> Learnsets => 
        new ReadOnlyDictionaryWrapper<SpecieId, Learnset>(LearnsetsData);
    
    public IReadOnlyDictionary<MoveId, Move> Moves => 
        new ReadOnlyDictionaryWrapper<MoveId, Move>(MovesData);
    
    public IReadOnlyDictionary<NatureType, Nature> Natures => 
        new ReadOnlyDictionaryWrapper<NatureType, Nature>(NaturesData);
    
    public IReadOnlyDictionary<PseudoWeatherId, PseudoWeather> PseudoWeathers => 
        new ReadOnlyDictionaryWrapper<PseudoWeatherId, PseudoWeather>(PseudoWeathersData);
    
    public IReadOnlyDictionary<SideConditionId, SideCondition> SideConditions => 
        new ReadOnlyDictionaryWrapper<SideConditionId, SideCondition>(SideConditionsData);
    
    public IReadOnlyDictionary<SpecieId, Specie> Species => 
        new ReadOnlyDictionaryWrapper<SpecieId, Specie>(SpeciesData);
    
    public IReadOnlyDictionary<SpecieId, SpeciesFormat> SpeciesFormats => 
        new ReadOnlyDictionaryWrapper<SpecieId, SpeciesFormat>(SpeciesFormatsData);
    
    public IReadOnlyDictionary<TerrainId, Terrain> Terrains => 
        new ReadOnlyDictionaryWrapper<TerrainId, Terrain>(TerrainsData);
    
    public IReadOnlyDictionary<PokemonType, TypeData> TypeData => TypeChart.TypeData;
    public TypeChart TypeChart { get; } = new();
    
    public IReadOnlyDictionary<WeatherId, Weather> Weathers => 
        new ReadOnlyDictionaryWrapper<WeatherId, Weather>(WeathersData);

    public Library()
    {
        _events = new Events(this);
        _pseudoWeathers = new PseudoWeathers(this);
        _sideConditions = new SideConditions(this);
        _terrains = new Terrains(this);
        _weathers = new Weathers(this);

        // Moves and items must be initialized last because they depend on other data
        _items = new Items(this);
        _moves = new Moves(this);
        _abilities = new Abilities(this);
    }

    #region Type-Safe Delegate Management Methods (Primary API)
    
    /// <summary>
    /// Register a delegate for a specific effect using type-safe IEffect interface
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="effect">The effect (move, ability, item, etc.)</param>
    /// <param name="handler">The delegate implementation</param>
    public void RegisterDelegate<T>(EventId eventId, IEffect effect, T handler) where T : Delegate
    {
        EffectIdUnion effectId = GetEffectId(effect);
        _events.RegisterDelegate(eventId, effectId, handler);
    }
    
    /// <summary>
    /// Get a delegate for a specific effect using type-safe IEffect interface
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="effect">The effect</param>
    /// <returns>The delegate if found, null otherwise</returns>
    public T? GetDelegate<T>(EventId eventId, IEffect effect) where T : Delegate
    {
        EffectIdUnion effectId = GetEffectId(effect);
        return _events.GetDelegate<T>(eventId, effectId);
    }
    
    /// <summary>
    /// Check if a delegate exists for a specific effect using type-safe IEffect interface
    /// </summary>
    /// <param name="eventId">The event type</param>
    /// <param name="effect">The effect</param>
    /// <returns>True if delegate exists</returns>
    public bool HasDelegate(EventId eventId, IEffect effect)
    {
        EffectIdUnion effectId = GetEffectId(effect);
        return _events.HasDelegate(eventId, effectId);
    }
    
    /// <summary>
    /// Helper method to extract ID string from different effect types
    /// </summary>
    /// <param name="effect">The effect to get ID from</param>
    /// <returns>String representation of the effect's ID</returns>
    private static EffectIdUnion GetEffectId(IEffect effect)
    {
        return effect switch
        {
            Move move => move.Id,
            Ability ability => ability.Id,
            Item item => item.Id,
            Condition condition => condition.Id,
            Specie specie => specie.Id,
            Format => throw new NotImplementedException(),
            _ => throw new ArgumentException($"Unsupported effect type: {effect.GetType().Name}", nameof(effect)),
        };
    }
    
    #endregion
    
    #region Specific Type Convenience Methods (Type-Safe)
    
    /// <summary>
    /// Register a delegate for a move using Move object (type-safe)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="move">The move object</param>
    /// <param name="handler">The delegate implementation</param>
    public void RegisterDelegate<T>(EventId eventId, Move move, T handler) where T : Delegate
    {
        RegisterDelegate(eventId, (IEffect)move, handler);
    }
    
    /// <summary>
    /// Register a delegate for an ability using Ability object (type-safe)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="ability">The ability object</param>
    /// <param name="handler">The delegate implementation</param>
    public void RegisterDelegate<T>(EventId eventId, Ability ability, T handler) where T : Delegate
    {
        RegisterDelegate(eventId, (IEffect)ability, handler);
    }
    
    /// <summary>
    /// Register a delegate for an item using Item object (type-safe)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="item">The item object</param>
    /// <param name="handler">The delegate implementation</param>
    public void RegisterDelegate<T>(EventId eventId, Item item, T handler) where T : Delegate
    {
        RegisterDelegate(eventId, (IEffect)item, handler);
    }
    
    /// <summary>
    /// Register a delegate for a condition using Condition object (type-safe)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="condition">The condition object</param>
    /// <param name="handler">The delegate implementation</param>
    public void RegisterDelegate<T>(EventId eventId, Condition condition, T handler) where T : Delegate
    {
        RegisterDelegate(eventId, (IEffect)condition, handler);
    }
    
    #endregion
    
    #region Enum-Based Convenience Methods (Backward Compatible)
    
    /// <summary>
    /// Register a delegate for a move using MoveId (convenience method)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="moveId">The move ID</param>
    /// <param name="handler">The delegate implementation</param>
    public void RegisterMoveDelegate<T>(EventId eventId, MoveId moveId, T handler) where T : Delegate
    {
        Move move = Moves[moveId];
        RegisterDelegate(eventId, move, handler);
    }
    
    /// <summary>
    /// Get a delegate for a move using MoveId (convenience method)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="moveId">The move ID</param>
    /// <returns>The delegate if found, null otherwise</returns>
    public T? GetMoveDelegate<T>(EventId eventId, MoveId moveId) where T : Delegate
    {
        Move move = Moves[moveId];
        return GetDelegate<T>(eventId, move);
    }
    
    /// <summary>
    /// Register a delegate for an ability using AbilityId (convenience method)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="abilityId">The ability ID</param>
    /// <param name="handler">The delegate implementation</param>
    public void RegisterAbilityDelegate<T>(EventId eventId, AbilityId abilityId, T handler) where T : Delegate
    {
        Ability ability = Abilities[abilityId];
        RegisterDelegate(eventId, ability, handler);
    }
    
    /// <summary>
    /// Get a delegate for an ability using AbilityId (convenience method)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="abilityId">The ability ID</param>
    /// <returns>The delegate if found, null otherwise</returns>
    public T? GetAbilityDelegate<T>(EventId eventId, AbilityId abilityId) where T : Delegate
    {
        Ability ability = Abilities[abilityId];
        return GetDelegate<T>(eventId, ability);
    }
    
    /// <summary>
    /// Register a delegate for an item using ItemId (convenience method)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="itemId">The item ID</param>
    /// <param name="handler">The delegate implementation</param>
    public void RegisterItemDelegate<T>(EventId eventId, ItemId itemId, T handler) where T : Delegate
    {
        Item item = Items[itemId];
        RegisterDelegate(eventId, item, handler);
    }
    
    /// <summary>
    /// Get a delegate for an item using ItemId (convenience method)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="itemId">The item ID</param>
    /// <returns>The delegate if found, null otherwise</returns>
    public T? GetItemDelegate<T>(EventId eventId, ItemId itemId) where T : Delegate
    {
        Item item = Items[itemId];
        return GetDelegate<T>(eventId, item);
    }
    
    #endregion
    
    #region Legacy String-Based Methods (Deprecated but Kept for Compatibility)
    
    /// <summary>
    /// Register a delegate for a specific effect using string ID (DEPRECATED - use IEffect overloads)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="effectId">The effect ID (move, ability, etc.)</param>
    /// <param name="handler">The delegate implementation</param>
    [Obsolete("Use RegisterDelegate(EventId, IEffect, T) for type safety. This method will be removed in a future version.")]
    public void RegisterDelegate<T>(EventId eventId, EffectIdUnion effectId, T handler) where T : Delegate
    {
        _events.RegisterDelegate(eventId, effectId, handler);
    }
    
    /// <summary>
    /// Get a delegate for a specific effect using string ID (DEPRECATED - use IEffect overloads)
    /// </summary>
    /// <typeparam name="T">The delegate type</typeparam>
    /// <param name="eventId">The event type</param>
    /// <param name="effectId">The effect ID</param>
    /// <returns>The delegate if found, null otherwise</returns>
    [Obsolete("Use GetDelegate(EventId, IEffect) for type safety. This method will be removed in a future version.")]
    public T? GetDelegate<T>(EventId eventId, EffectIdUnion effectId) where T : Delegate
    {
        return _events.GetDelegate<T>(eventId, effectId);
    }
    
    /// <summary>
    /// Check if a delegate exists for a specific effect using string ID (DEPRECATED - use IEffect overloads)
    /// </summary>
    /// <param name="eventId">The event type</param>
    /// <param name="effectId">The effect ID</param>
    /// <returns>True if delegate exists</returns>
    [Obsolete("Use HasDelegate(EventId, IEffect) for type safety. This method will be removed in a future version.")]
    public bool HasDelegate(EventId eventId, EffectIdUnion effectId)
    {
        return _events.HasDelegate(eventId, effectId);
    }
    
    #endregion
}