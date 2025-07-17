#pragma once

#include "../global-types/ID.h"
#include "../dex/IDexDataManager.h"
#include "../dex/IModdedDex.h"
#include "Condition.h"
#include <unordered_map>
#include <memory>

// class IModdedDex;

class DexConditions : public IDexDataManager
{
public:
	IModdedDex* dex = nullptr;
	std::unordered_map<ID, std::unique_ptr<Condition>> condition_cache = {};

	DexConditions() = default;
	DexConditions(IModdedDex* dex_ptr);

	Condition* get_condition(const std::string& name);
	Condition* get_condition(const Condition& condition);
	Condition* get_condition_by_id(const ID& id);

	DataType get_data_type() const override;
};