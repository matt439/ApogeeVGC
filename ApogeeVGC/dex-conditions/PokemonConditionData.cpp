#include "PokemonConditionData.h"

PokemonConditionData::PokemonConditionData(const std::string& name,
	const std::string& real_move,
	const std::string& full_name,
	bool exists,
	int num,
	int gen,
	const std::string& short_desc,
	const std::string& desc,
	NonStandard is_nonstandard,
	bool no_copy,
	bool affects_fainted,
	const std::string& source_effect) :
	Condition(name, real_move, full_name,
		exists, num, gen, short_desc, desc, is_nonstandard,
		no_copy, affects_fainted, source_effect)
{
}

PokemonConditionData::PokemonConditionData(const PokemonConditionData& other) :
	Condition(other),
	PokemonEventMethods()
{
}