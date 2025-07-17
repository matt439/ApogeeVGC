#include "DexStats.h"

#include "IDS_CACHE.h"

DexStats::DexStats(IModdedDex* dex_ptr) :
	dex(dex_ptr)
{
	int gen = dex->get_gen();

    if (gen != 1)
    {
        shortNames = {
            {StatID::HP, "HP"}, {StatID::ATK, "Atk"}, {StatID::DEF, "Def"},
            {StatID::SPA, "SpA"}, {StatID::SPD, "SpD"}, {StatID::SPE, "Spe"}
        };
        mediumNames = {
            {StatID::HP, "HP"}, {StatID::ATK, "Attack"}, {StatID::DEF, "Defense"},
            {StatID::SPA, "Sp. Atk"}, {StatID::SPD, "Sp. Def"}, {StatID::SPE, "Speed"}
        };
        names = {
            {StatID::HP, "HP"}, {StatID::ATK, "Attack"}, {StatID::DEF, "Defense"},
            {StatID::SPA, "Special Attack"}, {StatID::SPD, "Special Defense"}, {StatID::SPE, "Speed"}
        };
    }
    else
    {
        shortNames = {
            {StatID::HP, "HP"}, {StatID::ATK, "Atk"}, {StatID::DEF, "Def"},
            {StatID::SPA, "Spc"}, {StatID::SPD, "[SpD]"}, {StatID::SPE, "Spe"}
        };
        mediumNames = {
            {StatID::HP, "HP"}, {StatID::ATK, "Attack"}, {StatID::DEF, "Defense"},
            {StatID::SPA, "Special"}, {StatID::SPD, "[Sp. Def]"}, {StatID::SPE, "Speed"}
        };
        names = {
            {StatID::HP, "HP"}, {StatID::ATK, "Attack"}, {StatID::DEF, "Defense"},
            {StatID::SPA, "Special"}, {StatID::SPD, "[Special Defense]"}, {StatID::SPE, "Speed"}
        };
    }
}

StatID* DexStats::get_stat_id(const std::string& name)
{
	return nullptr; // TODO implement this properly
}

const std::vector<StatID>& DexStats::get_ids_cache() const 
{
	return IDS_CACHE;
}

DataType DexStats::get_data_type() const
{
	return DataType::STATS;
}