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

std::set<ID> DexSpecies::get_move_pool(const ID& id, bool is_nat_dex)
{
	return std::set<ID>(); // TODO implement this properly
}

std::vector<Learnset*> DexSpecies::get_full_learnset(const ID& id)
{
	return std::vector<Learnset*>(); // TODO implement this properly
}

Species* DexSpecies::learnset_parent(const Species& species, bool checking_moves)
{
	return nullptr; // TODO implement this properly
}

Learnset* DexSpecies::get_learnset_data(const ID& id)
{
	return nullptr; // TODO implement this properly
}

PokemonGoData* DexSpecies::get_pokemon_go_data(const ID& id)
{
	return nullptr; // TODO implement this properly
}

bool DexSpecies::egg_moves_only(const Species& child, Species* father) const
{
	return false; // TODO implement this properly
}