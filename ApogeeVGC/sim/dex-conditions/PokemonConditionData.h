#pragma once

#include "Condition.h"
#include "PokemonEventMethods.h"

struct PokemonConditionData : public Condition, public PokemonEventMethods
{
	PokemonConditionData() = default;
	PokemonConditionData(const std::string& name,
		const std::string& real_move = "",
		const std::string& full_name = "",
		bool exists = true,
		int num = 0,
		int gen = 0,
		const std::string& short_desc = "",
		const std::string& desc = "",
		NonStandard is_nonstandard = NonStandard::NONE,
		bool no_copy = false,
		bool affects_fainted = false,
		const std::string& source_effect = "");

	PokemonConditionData(const PokemonConditionData& other);
};