#include "TypeInfo.h"


TypeInfo::TypeInfo(const ID& id,
	const std::string& name,
	TypeInfoEffectType effect_type,
	bool exists,
	int gen,
	std::unordered_map<std::string, int> damage_taken,
	std::unique_ptr<SparseStatsTable> hp_dvs,
	std::unique_ptr<SparseStatsTable> hp_ivs,
	std::unique_ptr<NonStandard> is_nonstandard) :

	TypeData(std::move(damage_taken),
		std::move(hp_dvs),
		std::move(hp_ivs),
		std::move(is_nonstandard)),

	id(id), name(name), effect_type(effect_type), exists(exists), gen(gen)
{
}

TypeInfo::TypeInfo(const TypeInfo& other)
	: TypeData(*static_cast<const TypeData*>(&other)),
	id(other.id), name(other.name), effect_type(other.effect_type),
	exists(other.exists), gen(other.gen)
{
}

//TypeInfo& TypeInfo::operator=(const TypeInfo& other)
//{
//	id = other.id;
//	name = other.name;
//	effect_type = other.effect_type;
//	exists = other.exists;
//	gen = other.gen;
//
//	// Copy TypeData members
//	damage_taken = other.damage_taken;
//	if (other.hp_dvs)
//		hp_dvs = std::make_unique<SparseStatsTable>(*other.hp_dvs);
//	else
//		hp_dvs.reset();
//
//
//	if (other.hp_ivs)
//		hp_ivs = std::make_unique<SparseStatsTable>(*other.hp_ivs);
//	else
//		hp_ivs.reset();
//
//
//	if (other.is_nonstandard)
//		is_nonstandard = std::make_unique<NonStandard>(*other.is_nonstandard);
//	else
//		is_nonstandard.reset();
//}

const std::string& TypeInfo::to_string() const
{
	return id;
}