#include "DexAbilities.h"

DexAbilities::DexAbilities(IModdedDex* dex_ptr) :
	dex(dex_ptr)
{
}

DataType DexAbilities::get_data_type() const
{
	return DataType::ABILITIES;
}