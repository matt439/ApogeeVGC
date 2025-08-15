using ApogeeVGC.Sim;

namespace ApogeeVGC.Data;

public record Library
{
    private readonly Abilities _abilities = new();
    private readonly Conditions _conditions = new();
    private readonly Items _items = new();
    private readonly Learnsets _learnsets = new();
    private readonly Moves _moves = new();
    private readonly Natures _natures = new();
    private readonly Rulesets _rulesets = new();
    private readonly Species _species = new();
    private readonly SpeciesFormats _speciesFormats = new();
    private readonly Tags _tags = new();
    private readonly TypeChart _typeChart = new();

    public IReadOnlyDictionary<AbilityId, Ability> Abilities => _abilities.AbilitiesData;
    public IReadOnlyDictionary<ConditionId, Condition> Conditions => _conditions.ConditionsData;
    public IReadOnlyDictionary<ItemId, Item> Items => _items.ItemsData;
    public IReadOnlyDictionary<SpecieId, Learnset> Learnsets => _learnsets.LearnsetsData;
    public IReadOnlyDictionary<MoveId, Move> Moves => _moves.MovesData;
    public IReadOnlyDictionary<NatureType, Nature> Natures => _natures.NatureData;
    // rulesets
    public IReadOnlyDictionary<SpecieId, Specie> Species => _species.SpeciesData;
    public IReadOnlyDictionary<SpecieId, SpeciesFormat> SpeciesFormats => _speciesFormats.SpeciesFormatsData;
    // tags
    public IReadOnlyDictionary<PokemonType, TypeData> TypeData => _typeChart.TypeData;
    
}