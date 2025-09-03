using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Types;
using System.Collections;
using System.Reflection;
using ApogeeVGC.Sim.PokemonClasses;

namespace ApogeeVGC.Data;

public record Library
{
    private readonly Abilities _abilities;
    private readonly Conditions _conditions = new();
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
        _pseudoWeathers = new PseudoWeathers(this);
        _sideConditions = new SideConditions(this);
        _terrains = new Terrains(this);
        _weathers = new Weathers(this);

        // Moves and items must be initialized last because they depend on other data
        _items = new Items(this);
        _moves = new Moves(this);
        _abilities = new Abilities(this);
    }
}

/// <summary>
/// A wrapper around IReadOnlyDictionary that returns copies of objects when accessed.
/// This ensures that mutable fields in the objects cannot be accidentally shared between different users of the Library.
/// </summary>
/// <typeparam name="TKey">The type of the dictionary keys</typeparam>
/// <typeparam name="TValue">The type of the dictionary values, must have a Copy() method</typeparam>
internal class ReadOnlyDictionaryWrapper<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> innerDictionary)
    : IReadOnlyDictionary<TKey, TValue>
    where TValue : class
{
    public TValue this[TKey key] 
    { 
        get 
        { 
            TValue value = innerDictionary[key];
            return CopyValue(value);
        } 
    }

    public IEnumerable<TKey> Keys => innerDictionary.Keys;

    public IEnumerable<TValue> Values => innerDictionary.Values.Select(CopyValue);

    public int Count => innerDictionary.Count;

    public bool ContainsKey(TKey key) => innerDictionary.ContainsKey(key);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return innerDictionary.Select(kvp =>
            new KeyValuePair<TKey, TValue>(kvp.Key, CopyValue(kvp.Value))).GetEnumerator();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (innerDictionary.TryGetValue(key, out TValue? originalValue))
        {
            value = CopyValue(originalValue);
            return true;
        }
        value = null!;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static TValue CopyValue(TValue value)
    {
        // Use reflection to call the Copy method dynamically
        MethodInfo? copyMethod = typeof(TValue).GetMethod("Copy");
        if (copyMethod != null)
        {
            return (TValue)copyMethod.Invoke(value, null)!;
        }
        
        // If no Copy method exists, throw an exception with helpful information
        throw new InvalidOperationException($"Type {typeof(TValue).Name} does not have a Copy() method. " +
                                          "All types used in Library must implement a Copy() method for" +
                                          "proper isolation.");
    }
}