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

AbilityData::AbilityData(const Ability& ability,
	const AbilityEventMethods& ability_events,
	const PokemonEventMethods& pokemon_events) :
	Ability(ability),
	AbilityEventMethods(ability_events),
	PokemonEventMethods(pokemon_events)
{
}

AbilityData::AbilityData(const AbilityData& other) :
	Ability(other),
	AbilityEventMethods(other),
	PokemonEventMethods(other)
{
}

DataType AbilityData::get_data_type() const
{
	return DataType::ABILITIES;
}