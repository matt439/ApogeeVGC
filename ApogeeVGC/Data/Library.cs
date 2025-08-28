using ApogeeVGC.Sim;

namespace ApogeeVGC.Data;

public record Library
{
    private readonly Abilities _abilities = new();
    private readonly Conditions _conditions = new();
    private readonly Items _items = new();
    private readonly Learnsets _learnsets = new();
    private readonly Moves _moves;
    private readonly Natures _natures = new();
    private readonly PseudoWeathers _pseudoWeathers;
    private readonly Rulesets _rulesets = new();
    private readonly Species _species = new();
    private readonly SpeciesFormats _speciesFormats = new();
    private readonly Tags _tags = new();
    private readonly Terrains _terrains;
    private readonly Weathers _weathers;

    public IReadOnlyDictionary<AbilityId, Ability> Abilities => _abilities.AbilitiesData;
    public IReadOnlyDictionary<ConditionId, Condition> Conditions => _conditions.ConditionsData;
    public IReadOnlyDictionary<ItemId, Item> Items => _items.ItemsData;
    public IReadOnlyDictionary<SpecieId, Learnset> Learnsets => _learnsets.LearnsetsData;
    public IReadOnlyDictionary<MoveId, Move> Moves => _moves.MovesData;
    public IReadOnlyDictionary<NatureType, Nature> Natures => _natures.NatureData;
    public IReadOnlyDictionary<PseudoWeatherId, PseudoWeather> PseudoWeathers => _pseudoWeathers.PseudoWeatherData;
    // rulesets
    public IReadOnlyDictionary<SpecieId, Specie> Species => _species.SpeciesData;
    public IReadOnlyDictionary<SpecieId, SpeciesFormat> SpeciesFormats => _speciesFormats.SpeciesFormatsData;
    // tags
    public IReadOnlyDictionary<TerrainId, Terrain> Terrains => _terrains.TerrainData;
    public IReadOnlyDictionary<PokemonType, TypeData> TypeData => TypeChart.TypeData;
    public TypeChart TypeChart { get; } = new();
    public IReadOnlyDictionary<WeatherId, Weather> Weathers => _weathers.WeatherData;

    public Library()
    {
        _pseudoWeathers = new PseudoWeathers(this);
        _terrains = new Terrains(this);
        _weathers = new Weathers(this);

        // Moves must be initialized last because it depends on other data
        _moves = new Moves(this);
    }
}