#include "DexAbilities.h"

DexAbilities::DexAbilities(IModdedDex* dex_ptr) :
	dex(dex_ptr)
{
}

DataType DexAbilities::get_data_type() const
{
	return DataType::ABILITIES;
}

Ability* DexAbilities::get_ability(const std::string& name)
{
	return nullptr; // TODO implement this properly
}

Ability* DexAbilities::get_ability(const Ability& ability)
{
	return nullptr; // TODO implement this properly
}

Ability* DexAbilities::get_ability_by_id(const ID& id)
{
	return nullptr; // TODO implement this properly
}

std::vector<std::unique_ptr<Ability>>* DexAbilities::get_all_abilities()
{
	return nullptr; // TODO implement this properly
}