#include "DexTypes.h"

#include "EMPTY_TYPE_INFO.h"
#include "../dex/DexTableData.h"

//TypeInfo EMPTY_TYPE_INFO(
//	ID(""),
//	"",
//	TypeInfoEffectType::EFFECT_TYPE,
//	false
//);

DexTypes::DexTypes(IModdedDex* dex_ptr) : dex(dex_ptr)
{
}

TypeInfo* DexTypes::get_type_info(const std::string& id)
{
	return nullptr; // TODO implement this properly
}

TypeInfo* DexTypes::get_type_info(const TypeInfo& type_info)
{
	return nullptr; // TODO implement this properly
}

TypeInfo* DexTypes::get_type_info_by_id(const ID& id)
{
	return nullptr; // TODO implement this properly
}

std::vector<std::string>* DexTypes::get_names()
{
	return nullptr; // TODO implement this properly
}

bool DexTypes::is_name(const std::string& name) const
{
	// TODO implement this properly
	return false; // Placeholder
}

std::vector<std::unique_ptr<TypeInfo>>* DexTypes::get_all_type_infos()
{
	return nullptr; // TODO implement this properly
}

DataType DexTypes::get_data_type() const
{
	return DataType::TYPE_CHART;
}