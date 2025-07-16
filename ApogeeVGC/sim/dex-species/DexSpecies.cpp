#include "DexSpecies.h"

DexSpecies::DexSpecies(IModdedDex* dex_ptr)
	: dex(dex_ptr)
{
}

DataType DexSpecies::get_data_type() const
{
	return DataType::POKEDEX;
}