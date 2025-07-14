#pragma once

//#include "PokemonConditionData.h"
//#include "SideConditionData.h"
//#include "FieldConditionData.h"
//#include <variant>

//using ConditionData = std::variant<PokemonConditionData, SideConditionData, FieldConditionData>;

struct ConditionData
{
	//std::unique_ptr<PokemonConditionData> pokemon_condition = nullptr; // optional
	//std::unique_ptr<SideConditionData> side_condition = nullptr; // optional
	//std::unique_ptr<FieldConditionData> field_condition = nullptr; // optional
	int x = 0;
};