#pragma once

#include "GlobalTypes.h"

const std::string DEFAULT_MOD = "gen9";

// RuleTable
// ModdedDex

//ModdedBattleScriptsData;
//ModdedBattlePokemon;
//ModdedBattleQueue;
//ModdedField;
//ModdedBattleActions;
//ModdedBattleSide;

//TeamValidator
// Species
// PokemonSources
// PokemonSet
// Set
// AnyObject

enum class FormatEffectType
{
	FORMAT,
	RULESET,
	RULE,
	VALIDATOR_RULE,
};

class Format
{
public:
	Format() = default;
	int x;
};