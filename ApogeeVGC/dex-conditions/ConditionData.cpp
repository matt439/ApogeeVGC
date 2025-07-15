#include "ConditionData.h"

ConditionData::ConditionData(const ConditionData& other)
{
	if (other.pokemon_condition)
		pokemon_condition = std::make_unique<PokemonConditionData>(*other.pokemon_condition);
	// if (other.side_condition)
	// 	side_condition = std::make_unique<SideConditionData>(*other.side_condition);
	// if (other.field_condition)
	// 	field_condition = std::make_unique<FieldConditionData>(*other.field_condition);
}