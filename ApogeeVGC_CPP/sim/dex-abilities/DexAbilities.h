#pragma once

#include "../dex/IModdedDex.h"
#include "../dex/IDexDataManager.h"
#include "../global-types/ID.h"
#include "Ability.h"
#include <unordered_map>
#include <memory>

class DexAbilities : public IDexDataManager
{
public:
	IModdedDex* dex = nullptr;
	std::unordered_map<ID, std::unique_ptr<Ability>> abilities_cache = {};
	std::unique_ptr<std::vector<std::unique_ptr<Ability>>> all_cache = nullptr; // nullable

	DexAbilities() = default;
	DexAbilities(IModdedDex* dex_ptr);

	Ability* get_ability(const std::string& name = "");
	Ability* get_ability(const Ability& ability);
	Ability* get_ability_by_id(const ID& id);
	std::vector<std::unique_ptr<Ability>>* get_all_abilities();

	DataType get_data_type() const override;
};
