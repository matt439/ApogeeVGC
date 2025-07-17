#pragma once

#include "../dex/IDexDataManager.h"
#include "../global-types/ID.h"
#include "PokemonGoData.h"
#include "Species.h"
#include "Learnset.h"
//#include "../dex/IModdedDex.h"
#include <unordered_map>
#include <memory>
#include <set>

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
	
	/**
	 * @param id the ID of the species the move pool belongs to
	 * @param is_nat_dex
	 * @returns a Set of IDs of the full valid movepool of the given species for the current generation/mod.
	 * Note that inter-move incompatibilities, such as those from exclusive events, are not considered and all moves are
	 * lumped together. However, Necturna and Necturine's Sketchable moves are omitted from this pool, as their fundamental
	 * incompatibility with each other is essential to the nature of those species.
	 */
	std::set<ID> get_move_pool(const ID& id, bool is_nat_dex = false);

	std::vector<Learnset*> get_full_learnset(const ID& id);

	Species* learnset_parent(const Species& species, bool checking_moves = false);

	/**
	 * Gets the raw learnset data for the species.
	 *
	 * In practice, if you're trying to figure out what moves a pokemon learns,
	 * you probably want to `getFullLearnset` or `getMovePool` instead.
	 */
	Learnset* get_learnset_data(const ID& id);

	PokemonGoData* get_pokemon_go_data(const ID& id);

	std::vector<std::unique_ptr<Species>>* get_all_species();

	// Father can be nullptr, in which case it will be ignored.
	bool egg_moves_only(const Species& child, Species* father) const;

	DataType get_data_type() const override;
};