﻿using ApogeeVGC.Sim.Effects;
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
    private readonly Rulesets _rulesets = new();
    private readonly Species _species = new();
    private readonly SpeciesFormats _speciesFormats = new();
    private readonly Tags _tags = new();

    private IReadOnlyDictionary<AbilityId, Ability> AbilitiesData => _abilities.AbilitiesData;
    private IReadOnlyDictionary<ConditionId, Condition> ConditionsData => _conditions.ConditionsData;
    private IReadOnlyDictionary<ItemId, Item> ItemsData => _items.ItemsData;
    private IReadOnlyDictionary<SpecieId, Learnset> LearnsetsData => _learnsets.LearnsetsData;
    private IReadOnlyDictionary<MoveId, Move> MovesData => _moves.MovesData;
    private IReadOnlyDictionary<NatureType, Nature> NaturesData => _natures.NatureData;
    private IReadOnlyDictionary<SpecieId, Specie> SpeciesData => _species.SpeciesData;
    private IReadOnlyDictionary<SpecieId, SpeciesFormat> SpeciesFormatsData => _speciesFormats.SpeciesFormatsData;

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
    
    public IReadOnlyDictionary<SpecieId, Specie> Species => 
        new ReadOnlyDictionaryWrapper<SpecieId, Specie>(SpeciesData);
    
    public IReadOnlyDictionary<SpecieId, SpeciesFormat> SpeciesFormats => 
        new ReadOnlyDictionaryWrapper<SpecieId, SpeciesFormat>(SpeciesFormatsData);
    
    public IReadOnlyDictionary<PokemonType, TypeData> TypeData => TypeChart.TypeData;
    public TypeChart TypeChart { get; } = new();

    public Library()
    {
        // Moves and items must be initialized last because they depend on other data
        _items = new Items(this);
        _moves = new Moves(this);
        _abilities = new Abilities(this);
    }
}