#pragma once

#include "../global-types/ID.h"
#include "../global-types/Nonstandard.h"
#include "TypeData.h"
#include "TypeInfoEffectType.h"

class TypeInfo : public TypeData
{
public:
	ID id = "";
	std::string name = "";
	TypeInfoEffectType effect_type = TypeInfoEffectType::TYPE;
	bool exists = false;
	int gen = 0;

	TypeInfo() = default;
	//TypeInfo(const TypeInfo&) = default;
	//
	//TypeInfo(TypeInfo&&) = default; // Add move constructor  
	//TypeInfo& operator=(const TypeInfo& other);
	//TypeInfo& operator=(TypeInfo&& other) = default; // Add move assignment operator

	TypeInfo(const ID& id,
		const std::string& name,
		TypeInfoEffectType effect_type,
		bool exists,
		int gen = 0,
		std::unordered_map<std::string, int> damage_taken = {},
		std::unique_ptr<SparseStatsTable> hp_dvs = nullptr,
		std::unique_ptr<SparseStatsTable> hp_ivs = nullptr,
		std::unique_ptr<NonStandard> is_nonstandard = nullptr)
		: TypeData(std::move(damage_taken), std::move(hp_dvs), std::move(hp_ivs), std::move(is_nonstandard)),
		id(id), name(name), effect_type(effect_type), exists(exists), gen(gen) {
	}

	const std::string& to_string() const;
};