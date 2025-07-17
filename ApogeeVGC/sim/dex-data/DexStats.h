#pragma once

#include "../dex/IDexDataManager.h"
#include "../global-types/StatID.h"
#include "../dex/IModdedDex.h"
#include <unordered_map>
#include <string>

class DexStats : public IDexDataManager
{
public:
	IModdedDex* dex = nullptr;
	std::unordered_map<StatID, std::string> shortNames = {};
	std::unordered_map<StatID, std::string> mediumNames = {};
	std::unordered_map<StatID, std::string> names = {};

	DexStats() = default;
	DexStats(IModdedDex* dex_ptr);

	// returns nullptr if the stat is not found
	StatID* get_stat_id(const std::string& name);

	const std::vector<StatID>& get_ids_cache() const;

    DataType get_data_type() const override;
};