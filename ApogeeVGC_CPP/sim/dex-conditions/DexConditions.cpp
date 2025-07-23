#include "DexConditions.h"


DexConditions::DexConditions(IModdedDex* dex_ptr)
	: dex(dex_ptr)
{
}

DataType DexConditions::get_data_type() const
{
	return DataType::CONDITIONS;
}

Condition* DexConditions::get_condition(const std::string& name)
{
	return nullptr; // TODO implement this properly
}

Condition* DexConditions::get_condition(const Condition& condition)
{
	return nullptr; // TODO implement this properly
}

Condition* DexConditions::get_condition_by_id(const ID& id)
{
	return nullptr; // TODO implement this properly
}