using ApogeeVGC.Sim.Effects;
using ApogeeVGC.Sim.FieldClasses;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Types;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;

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