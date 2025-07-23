#include "DexNatures.h"

#include "to_id.h"
#include "EMPTY_NATURE.h"

DexNatures::DexNatures(IModdedDex* dex_ptr)
    : dex(dex_ptr)
{
}

DataType DexNatures::get_data_type() const
{
	return DataType::NATURES;
}

Nature* DexNatures::get_nature(const std::string& name)
{
	return nullptr; // TODO implement this properly
}

Nature* DexNatures::get_nature(const Nature& nature)
{
	return nullptr; // TODO implement this properly
}

Nature* DexNatures::get_nature_by_id(const ID& id)
{
	return nullptr; // TODO implement this properly
}

std::vector<std::unique_ptr<Nature>>* DexNatures::get_all_natures()
{
	return nullptr; // TODO implement this properly
}