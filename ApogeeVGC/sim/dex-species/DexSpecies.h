#pragma once

#include "../dex/IDexDataManager.h"
#include "../global-types/ID.h"
#include "Species.h"
#include "Learnset.h"
//#include "../dex/IModdedDex.h"
#include <unordered_map>
#include <memory>

class IModdedDex;

class DexSpecies : public IDexDataManager
{
public:
	IModdedDex* dex = nullptr;

	DexSpecies() = default;
	DexSpecies(IModdedDex* dex_ptr);

	std::unordered_map<ID, std::unique_ptr<Species>> species_cache = {};
	std::unordered_map<ID, std::unique_ptr<Learnset>> learnset_cache = {};
	std::unique_ptr<std::vector<std::unique_ptr<Species>>> all_cache = nullptr; // nullable

	Species* get_species(const std::string& name);
	Species* get_species(const Species& species);
	Species* get_species_by_id(const ID& id);
	
	// get_move_pool
	// get_full_learnset
	// learnset_parent
	// get_learnset_data
	// get_pokemon_go_data

	std::vector<std::unique_ptr<Species>>* get_all_species();

	// egg_moves_only

	DataType get_data_type() const override;
};