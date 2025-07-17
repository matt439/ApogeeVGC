#include "ModdedDex.h"

#include "../../lib/utils.h"
#include "DATA_DIR.h"
#include "MODS_DIR.h"
#include "BASE_MOD.h"
#include "Dex.h"

#include <rapidjson/istreamwrapper.h>

ModdedDex::ModdedDex(Dex* dex_parent, const std::string& mod) :
    dex_parent(dex_parent),
	//dexes(dex_parent->get_dexes()),
	is_base(mod == "base"),
    current_mod(mod),
	formats(std::make_unique<DexFormats>(this)),
	abilities(std::make_unique<DexAbilities>(this)),
	items(std::make_unique<DexItems>(this)),
	moves(std::make_unique<DexMoves>(this)),
	species(std::make_unique<DexSpecies>(this)),
	conditions(std::make_unique<DexConditions>(this)),
	natures(std::make_unique<DexNatures>(this)),
	types(std::make_unique<DexTypes>()),
	stats(std::make_unique<DexStats>())
{
	if (is_base)
		data_dir = DATA_DIR;
	else
		data_dir = MODS_DIR / mod;
}

DexTableData* ModdedDex::get_data()
{
	return load_data();
}

std::unordered_map<std::string, std::unique_ptr<ModdedDex>>* ModdedDex::get_dexes()
{
    include_mods();
	return dex_parent->get_dexes();
}

ModdedDex* ModdedDex::get_modded_dex(const std::string& mod)
{
    if (!dex_parent->get_modded_dex("base")->mods_loaded)
        dex_parent->get_modded_dex("base")->include_mods();

    std::string mod_name;
	if (mod.empty() || mod == "base")
		mod_name = "base";
	else
		mod_name = mod;

    return dex_parent->get_modded_dex(mod_name)->include_data();
}

ModdedDex* ModdedDex::get_modded_dex_for_gen(int gen)
{
    if (gen == 0)
        return this;

    return get_modded_dex("gen" + std::to_string(gen));
}

ModdedDex* ModdedDex::get_modded_dex_for_format(const Format& format)
{
 //   if (!mods_loaded)
	//	include_mods();

 //   std::string mod = formats->get_format(format)->mod;
	//
	//if (mod.empty() || mod == "base")
	//	mod = BASE_MOD;

 //   return dex_parent->get_modded_dex(mod)->include_data();

	// TODO - implement this properly
    return this;
}

IDexData* ModdedDex::mod_data(DataType data_type, const std::string& id)
{
    if (is_base)
		return data_cache->get_data(data_type, id);

    IDexData* parent_data = get_dexes()->at(parent_mod)->data_cache->get_data(data_type, id);
    if (data_cache->get_data(data_type, id) != parent_data)
        return data_cache->get_data(data_type, id);

	data_cache->set_data(id, parent_data->clone());
    return data_cache->get_data(data_type, id);
}


const std::string& ModdedDex::effect_to_string() const
{
    return name;
}

#include <string>
#include <regex>
#include <algorithm>
#include <cctype>

std::string ModdedDex::trim(const std::string& s)
{
    auto start = s.find_first_not_of(" \t\n\r");
    if (start == std::string::npos) return "";
    auto end = s.find_last_not_of(" \t\n\r");
    return s.substr(start, end - start + 1);
}

std::string ModdedDex::get_name(const std::string& input)
{
    std::string name = input;

    // Replace [| \s[\],\u202e]+ with space
    std::regex re1(R"([|\s\[\],\u202e]+)");
    name = std::regex_replace(name, re1, " ");
    name = trim(name);

    // Truncate to 18 characters
    if (name.length() > 18) {
        name = trim(name.substr(0, 18));
    }

    // Remove Zalgo and certain Unicode ranges (approximate)
    // Remove combining marks (U+0300–U+036F, U+0483–U+0489, etc.)
    std::regex zalgo(R"([\u0300-\u036f\u0483-\u0489\u0610-\u0615\u064B-\u065F\u0670\u06D6-\u06DC\u06DF-\u06ED\u0E31\u0E34-\u0E3A\u0E47-\u0E4E]{3,})");
    name = std::regex_replace(name, zalgo, "");

    // Remove box drawing characters (U+239B–U+23B9)
    std::regex boxdraw(R"([\u239b-\u23b9])");
    name = std::regex_replace(name, boxdraw, "");

    return name;
}

bool ModdedDex::get_immunity(const std::string& source_type, const std::string& target_type) const
{
	//auto type_data = types->get_type_info(target_type);
	//if (!type_data || !type_data->exists)
	//	return true; // If type doesn't exist, assume no immunity

	//if (type_data->damage_taken.count(source_type) && type_data->damage_taken.at(source_type) == 3)
	//	return false; // Immune if damage taken is 3

    // TODO implement this properly
	return true; // Otherwise, not immune
}

//bool ModdedDex::get_immunity(const std::string& source_type,
//    const std::vector<const std::string&>& target_types) const
//{
//	for (const auto& t : target_types)
//    {
//		if (!get_immunity(source_type, t))
//            return false;
//	}
//	return true;
//}
//
//bool ModdedDex::get_immunity(const std::string& source_type, const std::string& target_type) const
//{
//    auto type_data = types.get(target_type);
//    if (type_data && type_data->damage_taken.count(source_type) && type_data->damage_taken.at(source_type) == 3) {
//        return false;
//    }
//    return true;
//}

bool ModdedDex::get_immunity(const std::string& source_type, const std::vector<std::string>& target_types) const
{
    for (const auto& t : target_types)
    {
        if (!get_immunity(source_type, t))
            return false;
    }
    return true;
}
//
//template<typename Source>
//bool ModdedDex::get_immunity(const Source& source, const std::vector<std::string>& target_types) const
//{
//    return get_immunity(source.type, target_types);
//}
//
//template<typename Source>
//bool ModdedDex::get_immunity(const Source& source, const std::string& target_type) const
//{
//    return get_immunity(source.type, target_type);
//}

int ModdedDex::get_effectiveness(const std::string& source_type, const std::string& target_type) const
{
    //auto type_data = types->get_type_info(target_type);

    //if (!type_data)
    //    return 0;

    //auto it = type_data->damage_taken.find(source_type);
    //if (it == type_data->damage_taken.end())
    //    return 0;

    //switch (it->second)
    //{
    //case 1: return 1;   // super-effective
    //case 2: return -1;  // resist
    //default: return 0;  // neutral or unknown
    //};


    // TODO implement this
    return 0;
}

int ModdedDex::get_effectiveness(const std::string& source_type,
    const std::vector<std::string>& target_types) const
{
    int total = 0;
    for (const auto& t : target_types)
    {
        total += get_effectiveness(source_type, t);
    }
    return total;
}

//template<typename Source>
//int ModdedDex::get_effectiveness(const Source& source, const std::string& target_type) const
//{
//    return get_effectiveness(source.type, target_type);
//}
//template<typename Source>
//int ModdedDex::get_effectiveness(const Source& source, const std::vector<std::string>& target_types) const
//{
//    return get_effectiveness(source.type, target_types);
//}

////std::string ModdedDex::data_type_to_key(DataType data_type)
////{
////    static const std::unordered_map<DataType, std::string> type_map =
////    {
////        {DataType::ABILITIES, "abilities"},
////        {DataType::RULESETS, "rulesets"},
////        {DataType::FORMATS_DATA, "formats-data"},
////        {DataType::ITEMS, "items"},
////        {DataType::LEARNSETS, "learnsets"},
////        {DataType::MOVES, "moves"},
////        {DataType::NATURES, "natures"},
////        {DataType::POKEDEX, "pokedex"},
////        {DataType::POKEMON_GO_DATA, "pokemongo"},
////        {DataType::SCRIPTS, "scripts"},
////        {DataType::CONDITIONS, "conditions"},
////        {DataType::TYPE_CHART, "typechart"}
////    };
////    return type_map.at(data_type);
////}

Descriptions ModdedDex::get_descriptions(const std::string& table, const std::string& id)
{
	//if (table.empty() || id.empty())
	//	return { "", "" }; // invalid input

 //   // look up the entry in the text table
 //   texttabledata& text_table = load_text_data();

	//return text_table.get_description_from_table(table, id);

    // TODO implement this
    return Descriptions();
}

ActiveMove ModdedDex::get_active_move(const std::string& id)
{
	// TODO implement this properly
    MoveFlags flags;
    return ActiveMove(0, id, MoveTarget::SELF, flags);
}

ActiveMove ModdedDex::get_active_move(const Move& move)
{
	// TODO implement this properly
    MoveFlags flags;
    return ActiveMove(0, "", MoveTarget::SELF, flags);
}

StatsTable ModdedDex::get_hidden_power()
{
	return StatsTable(); // TODO implement this properly
}

int ModdedDex::truncate_to_32_bit_int(int value) const
{
	// Truncate to 32-bit signed integer range
	if (value < INT32_MIN)
		return INT32_MIN;
	if (value > INT32_MAX)
		return INT32_MAX;
	return value;
}

std::vector<AnyObject> ModdedDex::data_search(const std::string& target,
    const std::vector<DataSearchOptions>& search_in,
    bool is_inexact)
{
	// TODO implement this properly
	std::vector<AnyObject> results;
	// Example search logic (pseudo-code):
	// for each data type in search_in:
	//     load the data for that type
	//     if is_inexact:
	//         use fuzzy matching to find matches
	//     else:
	//         use exact matching to find matches
	//     add matches to results
	return results;
}

AnyObject* ModdedDex::load_data_file(const std::string& base_path, DataType data_type)
{
	return nullptr; // TODO implement this properly
}

DexTable<ITextEntry>* ModdedDex::load_text_file(const std::string& name, const std::string& export_name)
{
    return nullptr; // TODO implement this properly
}

ModdedDex* ModdedDex::include_mods()
{
    if (!is_base)
        throw std::runtime_error("This must be called on the base Dex");

    if (mods_loaded)
        return this;

    for (const auto& entry : std::filesystem::directory_iterator(MODS_DIR))
    {
        if (entry.is_directory())
        {
            std::string mod = entry.path().filename().string();
			get_dexes()->emplace(mod, std::make_unique<ModdedDex>(dex_parent, mod));
        }
    }
    mods_loaded = true;
	return this;
}

ModdedDex* ModdedDex::include_mod_data()
{
    auto* dexes_map = get_dexes();
    for (auto& pair : *dexes_map)
    {
        pair.second->include_data();
    }
    return this;
}

ModdedDex* ModdedDex::include_data()
{
	load_data();
	return this;
}

TextTableData* ModdedDex::load_text_data()
{
    // Only cache in the base dex
    auto* base_dex = get_dexes()->at("base").get();
    if (base_dex->text_cache)
        return base_dex->text_cache.get();

    auto text_data = std::make_unique<TextTableData>();
	// TODO - implement loading text files properly
    //text_data->pokedex = *static_cast<DexTable<PokedexText>*>(load_text_file("pokedex", "PokedexText"));
    //text_data->moves = *static_cast<DexTable<MoveText>*>(load_text_file("moves", "MovesText"));
    //text_data->abilities = *static_cast<DexTable<AbilityText>*>(load_text_file("abilities", "AbilitiesText"));
    //text_data->items = *static_cast<DexTable<ItemText>*>(load_text_file("items", "ItemsText"));
    //text_data->default_text = *static_cast<DexTable<DefaultText>*>(load_text_file("default", "DefaultText"));

    base_dex->text_cache = std::move(text_data);
    return base_dex->text_cache.get();
}


std::string* ModdedDex::get_alias(const ID& id)
{
	// TODO - implement this properly
    return nullptr;
}

AliasesTable* ModdedDex::load_aliases()
{
	// TODO - implement this properly
    return aliases.get();
}

DexTableData* ModdedDex::load_data()
{
	// TODO - properly implement this
    return data_cache.get();
}

ModdedDex* ModdedDex::include_formats()
{
	// TODO - implement this properly
	return this;
}

//DexTableData* ModdedDex::get_data_cache()
//{
//	if (!data_cache)
//	{
//		data_cache = std::make_unique<DexTableData>();
//		// Load data into data_cache here, if needed
//	}
//	return data_cache.get();
//}

int ModdedDex::get_gen() const
{
    return gen;
}

//const std::string& ModdedDex::get_parent_mod() const
//{
//	return parent_mod;
//}

IDex* ModdedDex::get_idex_parent() const
{
    return dex_parent;
}

bool ModdedDex::get_is_base() const
{
    return is_base;
}

bool ModdedDex::has_mod(const std::string& mod)
{
	if (mod.empty() || mod == "base")
		return true; // Base mod always exists
	auto dexes_map = get_dexes();
	return dexes_map->find(mod) != dexes_map->end();
}

ModdedDex* ModdedDex::cast_to_modded_dex(IModdedDex* modded_dex) const
{
    return dynamic_cast<ModdedDex*>(modded_dex);
}

ModdedDex* ModdedDex::cast_to_modded_dex()
{
	return this;
}