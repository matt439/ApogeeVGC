#pragma once

#include "global-types.h"
#include <unordered_map>
#include <string>
#include <optional>

template<typename T>
using DexTable = std::unordered_map<std::string, T>;

// forward declarations
struct AbilityData;
struct FormatData;
struct ItemData;
struct LearnsetData;
struct MoveData;
struct NatureData;
struct SpeciesData;
struct SpeciesFormatsData;
struct PokemonGoData;
struct ConditionData;
struct TypeData;

struct DexTableData
{
    DexTable<AbilityData> abilities;
    DexTable<FormatData> rulesets;
    DexTable<ItemData> items;
    DexTable<LearnsetData> learnsets;
    DexTable<MoveData> moves;
    DexTable<NatureData> natures;
    DexTable<SpeciesData> pokedex;
    DexTable<SpeciesFormatsData> formats_data;
    DexTable<PokemonGoData> pokemon_go_data;
    DexTable<AnyObject> scripts;
    DexTable<ConditionData> conditions;
    DexTable<TypeData> type_chart;
};

struct IModdedDex
{
	virtual ~IModdedDex() = default;
	virtual std::optional<std::string> get_alias(const std::string& id) = 0;
	virtual std::optional<DexTableData> get_data_cache() = 0;
    virtual DexTableData& data() = 0;
	virtual int get_gen() const = 0;
};