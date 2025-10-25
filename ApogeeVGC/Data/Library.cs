using ApogeeVGC.Sim.Events;
using ApogeeVGC.Sim.GameObjects;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Types;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Utils;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;

namespace ApogeeVGC.Data;

public record Library
{
    private readonly Abilities _abilities;
    private readonly Conditions _conditions;
    private readonly EventIdInfoData _eventInfoData = new();
    private readonly Formats _formats = new();
    private readonly Items _items;
    private readonly Learnsets _learnsets = new();
    private readonly Moves _moves;
    private readonly Natures _natures = new();
    private readonly Rulesets _rulesets = new();
    private readonly SpeciesData _speciesData = new();
    private readonly SpeciesFormats _speciesFormats = new();
    private readonly Tags _tags = new();

    private IReadOnlyDictionary<AbilityId, Ability> AbilitiesData => _abilities.AbilitiesData;
    private IReadOnlyDictionary<ConditionId, Condition> ConditionsData => _conditions.ConditionsData;
    private IReadOnlyDictionary<EventId, EventIdInfo> EventData => _eventInfoData.EventData;
    private IReadOnlyDictionary<FormatId, Format> FormatsData => _formats.FormatData;
    private IReadOnlyDictionary<ItemId, Item> ItemsData => _items.ItemsData;
    private IReadOnlyDictionary<SpecieId, Learnset> LearnsetsData => _learnsets.LearnsetsData;
    private IReadOnlyDictionary<MoveId, Move> MovesData => _moves.MovesData;
    private IReadOnlyDictionary<NatureId, Nature> NaturesData => _natures.NatureData;
    private IReadOnlyDictionary<RuleId, Format> RulesetsData => _rulesets.RulesetData;
    private IReadOnlyDictionary<SpecieId, Species> SpeciesData => _speciesData.SpeciesDataDictionary;
    private IReadOnlyDictionary<SpecieId, SpeciesFormat> SpeciesFormatsData => _speciesFormats.SpeciesFormatsData;

    public IReadOnlyDictionary<AbilityId, Ability> Abilities => 
        new ReadOnlyDictionaryWrapper<AbilityId, Ability>(AbilitiesData);
    
    public IReadOnlyDictionary<ConditionId, Condition> Conditions => 
        new ReadOnlyDictionaryWrapper<ConditionId, Condition>(ConditionsData);

    public IReadOnlyDictionary<EventId, EventIdInfo> Events =>
        new ReadOnlyDictionaryWrapper<EventId, EventIdInfo>(EventData);

    public IReadOnlyDictionary<FormatId, Format> Formats => 
        new ReadOnlyDictionaryWrapper<FormatId, Format>(FormatsData);

    public IReadOnlyDictionary<ItemId, Item> Items => 
        new ReadOnlyDictionaryWrapper<ItemId, Item>(ItemsData);
    
    public IReadOnlyDictionary<SpecieId, Learnset> Learnsets => 
        new ReadOnlyDictionaryWrapper<SpecieId, Learnset>(LearnsetsData);
    
    public IReadOnlyDictionary<MoveId, Move> Moves => 
        new ReadOnlyDictionaryWrapper<MoveId, Move>(MovesData);
    
    public IReadOnlyDictionary<NatureId, Nature> Natures => 
        new ReadOnlyDictionaryWrapper<NatureId, Nature>(NaturesData);

    public IReadOnlyDictionary<RuleId, Format> Rulesets =>
        new ReadOnlyDictionaryWrapper<RuleId, Format>(RulesetsData);

    public IReadOnlyDictionary<SpecieId, Species> Species => 
        new ReadOnlyDictionaryWrapper<SpecieId, Species>(SpeciesData);
    
    public IReadOnlyDictionary<SpecieId, SpeciesFormat> SpeciesFormats => 
        new ReadOnlyDictionaryWrapper<SpecieId, SpeciesFormat>(SpeciesFormatsData);
    
    public IReadOnlyDictionary<PokemonType, TypeData> TypeData => TypeChart.TypeData;
    public TypeChart TypeChart { get; } = new();

    public Library()
    {
        _conditions = new Conditions(this);
        // Moves and items must be initialized last because they depend on other data
        _items = new Items(this);
        _moves = new Moves(this);
        _abilities = new Abilities(this);
    }
}