#include "ConditionData.h"

ConditionData::ConditionData(const PokemonConditionData& v) : data(v)
{
}

ConditionData::ConditionData(const SideConditionData& v) : data(v)
{
}

ConditionData::ConditionData(const FieldConditionData& v) : data(v)
{
}


DataType ConditionData::get_data_type() const
{
	return DataType::CONDITIONS;
}