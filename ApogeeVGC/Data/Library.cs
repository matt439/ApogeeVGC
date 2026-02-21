using System.Collections.Frozen;
using ApogeeVGC.Sim.Moves;
using ApogeeVGC.Sim.Types;
using ApogeeVGC.Sim.PokemonClasses;
using ApogeeVGC.Sim.Conditions;
using ApogeeVGC.Sim.Abilities;
using ApogeeVGC.Sim.Items;
using ApogeeVGC.Sim.SpeciesClasses;
using ApogeeVGC.Sim.FormatClasses;

namespace ApogeeVGC.Data;

public record Library
{
    private readonly Abilities.Abilities _abilities;
    private readonly Conditions.Conditions _conditions;
    private readonly Formats _formats = new();
    private readonly Items.Items _items;
    private readonly Lazy<Learnsets> _learnsets = new(() => new Learnsets());
    private readonly Moves.Moves _moves;
    private readonly Natures _natures = new();
    private readonly Rulesets _rulesets = new();
    private readonly SpeciesData.SpeciesData _speciesData = new();
    private readonly SpeciesFormats _speciesFormats = new();
    private readonly Tags _tags = new();

    private FrozenDictionary<AbilityId, Ability> AbilitiesData => _abilities.AbilitiesData;
    private FrozenDictionary<ConditionId, Condition> ConditionsData => _conditions.ConditionsData;
    private FrozenDictionary<FormatId, Format> FormatsData => _formats.FormatData;
    private FrozenDictionary<ItemId, Item> ItemsData => _items.ItemsData;
    private FrozenDictionary<SpecieId, Learnset> LearnsetsData => _learnsets.Value.LearnsetsData;
    private FrozenDictionary<MoveId, Move> MovesData => _moves.MovesData;
    private FrozenDictionary<NatureId, Nature> NaturesData => _natures.NatureData;
    private FrozenDictionary<RuleId, Format> RulesetsData => _rulesets.RulesetData;
    private FrozenDictionary<SpecieId, Species> SpeciesData => _speciesData.SpeciesDataDictionary;
    private FrozenDictionary<SpecieId, SpeciesFormat> SpeciesFormatsData => _speciesFormats.SpeciesFormatsData;

    public FrozenDictionary<AbilityId, Ability> Abilities => AbilitiesData;

    public FrozenDictionary<ConditionId, Condition> Conditions => ConditionsData;

    public FrozenDictionary<FormatId, Format> Formats => FormatsData;

    public FrozenDictionary<ItemId, Item> Items => ItemsData;

    public FrozenDictionary<SpecieId, Learnset> Learnsets => LearnsetsData;

    public FrozenDictionary<MoveId, Move> Moves => MovesData;

    public FrozenDictionary<NatureId, Nature> Natures => NaturesData;

    public FrozenDictionary<RuleId, Format> Rulesets => RulesetsData;

    public FrozenDictionary<SpecieId, Species> Species => SpeciesData;

    public FrozenDictionary<SpecieId, SpeciesFormat> SpeciesFormats => SpeciesFormatsData;

    public FrozenDictionary<PokemonType, TypeData> TypeData => TypeChart.TypeData;
    public TypeChart TypeChart { get; } = new();

    public Library()
    {
        _conditions = new Conditions.Conditions(this);
        // Moves and items must be initialized last because they depend on other data
        _items = new Items.Items(this);
        _moves = new Moves.Moves(this);
        _abilities = new Abilities.Abilities(this);
    }
}