#include "TypeData.h"


TypeData::TypeData(std::unordered_map<std::string, int> damage_taken,
	std::unique_ptr<SparseStatsTable> hp_dvs,
	std::unique_ptr<SparseStatsTable> hp_ivs,
	std::unique_ptr<NonStandard> is_nonstandard) :
	damage_taken(std::move(damage_taken)),
	hp_dvs(std::move(hp_dvs)),
	hp_ivs(std::move(hp_ivs)),
	is_nonstandard(std::move(is_nonstandard))
{
}

TypeData::TypeData(const TypeData& other)
	: damage_taken(other.damage_taken),
	hp_dvs(other.hp_dvs ? std::make_unique<SparseStatsTable>(*other.hp_dvs) : nullptr),
	hp_ivs(other.hp_ivs ? std::make_unique<SparseStatsTable>(*other.hp_ivs) : nullptr),
	is_nonstandard(other.is_nonstandard ? std::make_unique<NonStandard>(*other.is_nonstandard) : nullptr)
{

}