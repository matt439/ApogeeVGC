#include "DexSpecies.h"

DexSpecies::DexSpecies(IModdedDex* dex_ptr)
	: dex(dex_ptr)
{
}

DataType DexSpecies::get_data_type() const
{
	return DataType::POKEDEX;
}

Species* DexSpecies::get_species(const std::string& name)
{
	return nullptr; // TODO implement this properly
}

Species* DexSpecies::get_species(const Species& species)
{
	return nullptr; // TODO implement this properly
}

Species* DexSpecies::get_species_by_id(const ID& id)
{
	return nullptr; // TODO implement this properly
}

std::vector<std::unique_ptr<Species>>* DexSpecies::get_all_species()
{
	return nullptr; // TODO implement this properly
}