#pragma once

#include "../global-types/SparseStatsTable.h"
#include "../global-types/NonStandard.h"
#include <unordered_map>
#include <string>
#include <memory>

struct TypeData
{
	std::unordered_map<std::string, int> damage_taken = {}; // key (string) is attacking type name or effect ID
	std::unique_ptr<SparseStatsTable> hp_dvs = nullptr; // optional
	std::unique_ptr<SparseStatsTable> hp_ivs = nullptr; // optional
	std::unique_ptr<NonStandard> is_nonstandard = nullptr; // optional

	TypeData() = default;
	TypeData(const TypeData& other);
	TypeData(std::unordered_map<std::string, int> damage_taken,
		std::unique_ptr<SparseStatsTable> hp_dvs = nullptr,
		std::unique_ptr<SparseStatsTable> hp_ivs = nullptr,
		std::unique_ptr<NonStandard> is_nonstandard = nullptr);
};