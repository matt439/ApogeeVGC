#include "AbilityData.h"

AbilityData::AbilityData(
	const std::string& name,
	int rating,
	bool suppress_weather,
	std::unique_ptr<AbilityFlags> flags,
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
	const std::string& source_effect)
	: Ability(name, rating, suppress_weather, std::move(flags),
		real_move, full_name, exists, num, gen, short_desc, desc,
		is_nonstandard, no_copy, affects_fainted, source_effect),
	AbilityEventMethods(),
	PokemonEventMethods()
{
}