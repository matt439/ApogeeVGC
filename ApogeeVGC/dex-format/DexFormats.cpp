#include "DexFormats.h"

DexFormats::DexFormats(IModdedDex* dex_ptr)
	: dex(dex_ptr)
{
}

DataType DexFormats::get_data_type() const
{
	return DataType::FORMATS_DATA;
}