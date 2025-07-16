#include "DexConditions.h"


DexConditions::DexConditions(IModdedDex* dex_ptr)
	: dex(dex_ptr)
{
}

DataType DexConditions::get_data_type() const
{
	return DataType::CONDITIONS;
}