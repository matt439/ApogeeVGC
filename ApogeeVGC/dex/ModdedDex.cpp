#include "ModdedDex.h"

#include "constants.h"

#include <rapidjson/istreamwrapper.h>

ModdedDex::ModdedDex(IDex* dex_parent, const std::string& mod) :
    dex_parent(dex_parent),
	dexes(&dex_parent->get_dexes()),
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
    load_data();
	return data_cache.get();
}

std::unordered_map<std::string, ModdedDex>* ModdedDex::get_dexes()
{
    include_mods();
    return dexes;
}

ModdedDex* ModdedDex::get_modded_dex(const std::string& mod)
{
    if (!dexes->at("base").mods_loaded)
        dexes->at("base").include_mods();

    std::string mod_name;
	if (mod.empty() || mod == "base")
		mod_name = "base";
	else
		mod_name = mod;

    return dexes->at(mod_name).include_data();
}

ModdedDex* ModdedDex::get_modded_dex_for_gen(int gen)
{
    if (gen == 0)
        return this;

    return get_modded_dex("gen" + std::to_string(gen));
}

ModdedDex* ModdedDex::get_modded_dex_for_format(const Format& format)
{
    if (!mods_loaded)
		include_mods();

    std::string mod = formats->get_format(format).mod;
	
	if (mod.empty() || mod == "base")
		mod = BASE_MOD;

    return dexes->at(mod).include_data();
}

template<typename T>
T& ModdedDex::mod_data(DataType data_type, const std::string& id)
{
    // Access the correct DexTable for the data_type
    DexTable<T>& table = get_table<T>(data_type);
    if (is_base)
		return table.at(id);

    DexTable<T>& parent_table = dexes[parent_mod].get_table<T>(data_type);
    if (table.at(id) != parent_table.at(id))
		return table.at(id);

    // Deep clone logic (pseudo-code, implement as needed)
    table[id] = deep_clone(table.at(id));
    return table.at(id);
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
	auto type_data = types->get_type_info(target_type);
	if (!type_data || !type_data->exists)
		return true; // If type doesn't exist, assume no immunity

	if (type_data->damage_taken.count(source_type) && type_data->damage_taken.at(source_type) == 3)
		return false; // Immune if damage taken is 3

	return true; // Otherwise, not immune
}

bool ModdedDex::get_immunity(const std::string& source_type,
    const std::vector<const std::string&>& target_types) const
{
	for (const auto& t : target_types)
    {
		if (!get_immunity(source_type, t))
            return false;
	}
	return true;
}

//bool ModdedDex::get_immunity(const std::string& source_type, const std::string& target_type) const
//{
//    auto type_data = types.get(target_type);
//    if (type_data && type_data->damage_taken.count(source_type) && type_data->damage_taken.at(source_type) == 3) {
//        return false;
//    }
//    return true;
//}
//
//bool ModdedDex::get_immunity(const std::string& source_type, const std::vector<std::string>& target_types) const
//{
//    for (const auto& t : target_types) {
//        if (!get_immunity(source_type, t)) return false;
//    }
//    return true;
//}

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
    auto type_data = types->get_type_info(target_type);

    if (!type_data)
        return 0;

    auto it = type_data->damage_taken.find(source_type);
    if (it == type_data->damage_taken.end())
        return 0;

    switch (it->second)
    {
    case 1: return 1;   // super-effective
    case 2: return -1;  // resist
    default: return 0;  // neutral or unknown
    };
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
//
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
	if (table.empty() || id.empty())
		return { "", "" }; // Invalid input

    // Look up the entry in the text table
    TextTableData& text_table = load_text_data();

	return text_table.get_description_from_table(table, id);
}


//const rapidjson::Value* ModdedDex::load_data_file(const std::string& base_path, DataType data_type)
//{
//    std::string file_path = base_path + std::string(DATA_FILES.at(data_type)) + ".json";
//    try
//    {
//        std::ifstream ifs(file_path);
//        if (!ifs)
//        {
//            // File not found
//            return nullptr;
//        }
//
//        rapidjson::IStreamWrapper isw(ifs);
//        rapidjson::Document doc;
//        doc.ParseStream(isw);
//
//        if (!doc.IsObject())
//        {
//            throw std::runtime_error(file_path + " must export a non-null object");
//        }
//
//        std::string key = data_type_to_key(data_type);
//        if (!doc.HasMember(key.c_str()) || !doc[key.c_str()].IsObject())
//        {
//            throw std::runtime_error(file_path + " must export an object whose '" + key + "' property is an Object");
//        }
//
//        return &doc[key.c_str()];
//    }
//    catch (const std::exception& e)
//    {
//        // Only rethrow if not file-not-found
//        if (std::string(e.what()).find("No such file") == std::string::npos &&
//            std::string(e.what()).find("not found") == std::string::npos)
//        {
//            throw;
//        }
//        // Otherwise, return nullptr
//        return nullptr;
//    }
//}
//
//const rapidjson::Value* ModdedDex::load_text_file(const std::string& name, const std::string& export_name) {
//    std::filesystem::path file_path = DATA_DIR / "text" / (name + ".json");
//    std::ifstream ifs(file_path);
//    if (!ifs) {
//        // File not found
//        return nullptr;
//    }
//
//    rapidjson::IStreamWrapper isw(ifs);
//    static rapidjson::Document doc; // static to keep the data alive after return
//    doc.ParseStream(isw);
//
//    if (!doc.IsObject() || !doc.HasMember(export_name.c_str()) || !doc[export_name.c_str()].IsObject()) {
//        // Invalid file or missing export
//        return nullptr;
//    }
//
//    return &doc[export_name.c_str()];
//}
//
//void ModdedDex::include_mods()
//{
//    if (!is_base)
//    {
//        throw std::runtime_error("This must be called on the base Dex");
//    }
//    if (mods_loaded) return;
//
//    for (const auto& entry : std::filesystem::directory_iterator(MODS_DIR))
//    {
//        if (entry.is_directory())
//        {
//            std::string mod = entry.path().filename().string();
//            dexes[mod] = ModdedDex(mod);
//        }
//    }
//    mods_loaded = true;
//}
//
//TextTableData& ModdedDex::load_text_data()
//{
//    // Check if already cached
//    if (dexes["base"].text_cache.has_value())
//    {
//        return dexes["base"].text_cache.value();
//    }
//
//    TextTableData text_data;
//    // Each load_text_file returns a rapidjson::Value*; you need to convert/deserialize to DexTable<T>
//    // This is a placeholder for actual deserialization logic
//    // For real code, implement conversion from rapidjson::Value to DexTable<T>
//
//    // TODO
//    // Example (pseudo-code, you must implement the conversion):
//    // text_data.pokedex = parse_text_table<PokedexText>(load_text_file("pokedex", "PokedexText"));
//    // text_data.moves = parse_text_table<MoveText>(load_text_file("moves", "MovesText"));
//    // text_data.abilities = parse_text_table<AbilityText>(load_text_file("abilities", "AbilitiesText"));
//    // text_data.items = parse_text_table<ItemText>(load_text_file("items", "ItemsText"));
//    // text_data.default_text = parse_text_table<DefaultText>(load_text_file("default", "DefaultText"));
//
//    // Cache and return
//    dexes["base"].text_cache = text_data;
//    return dexes["base"].text_cache.value();
//}
//
//std::string* ModdedDex::get_alias(const ID& id)
//{
//    auto& aliases_map = load_aliases();
//    auto it = aliases_map.find(id);
//    if (it != aliases_map.end()) {
//        return &it->second;
//    }
//    return nullptr;
//}
//
//AliasesTable& ModdedDex::load_aliases()
//{
//    if (!is_base)
//    {
//        return dexes["base"].load_aliases();
//    }
//    if (aliases.has_value())
//    {
//        return aliases.value();
//    }
//
//    // Load aliases JSON file
//    std::filesystem::path alias_path = DATA_DIR / "aliases.json";
//    std::ifstream ifs(alias_path);
//    if (!ifs) throw std::runtime_error("Aliases file not found");
//    rapidjson::IStreamWrapper isw(ifs);
//    rapidjson::Document doc;
//    doc.ParseStream(isw);
//
//    AliasesTable aliases_map;
//    std::unordered_map<std::string, std::string> compound_names;
//    std::unordered_map<std::string, std::vector<std::string>> fuzzy_aliases;
//
//    // Fill aliases_map
//    const auto& aliases_json = doc["Aliases"];
//    for (auto it = aliases_json.MemberBegin(); it != aliases_json.MemberEnd(); ++it)
//    {
//        std::string alias = it->name.GetString();
//        std::string target = toID(it->value.GetString());
//        aliases_map[alias] = target;
//    }
//
//    // Fill compound_names
//    const auto& compound_json = doc["CompoundWordNames"];
//    for (auto& v : compound_json.GetArray())
//    {
//        std::string name = v.GetString();
//        compound_names[toID(name)] = name;
//    }
//
//    // Helper lambdas for fuzzy logic
//    auto addFuzzy = [&](const std::string& alias, const std::string& target)
//        {
//            if (alias == target || alias.length() < 2) return;
//            auto& vec = fuzzy_aliases[alias];
//            if (std::find(vec.begin(), vec.end(), target) == vec.end())
//            {
//                vec.push_back(target);
//            }
//        };
//
//    auto addFuzzyForme = [&](const std::string& alias, const std::string& target, const std::string& forme, const std::string& formeLetter)
//        {
//            addFuzzy(alias + forme, target);
//            if (forme.empty()) return;
//            addFuzzy(alias + formeLetter, target);
//            addFuzzy(formeLetter + alias, target);
//            if (forme == "alola") addFuzzy("alolan" + alias, target);
//            else if (forme == "galar") addFuzzy("galarian" + alias, target);
//            else if (forme == "hisui") addFuzzy("hisuian" + alias, target);
//            else if (forme == "paldea") addFuzzy("paldean" + alias, target);
//            else if (forme == "megax") addFuzzy("mega" + alias + "x", target);
//            else if (forme == "megay") addFuzzy("mega" + alias + "y", target);
//            else addFuzzy(forme + alias, target);
//
//            if (forme == "megax" || forme == "megay")
//            {
//                addFuzzy("mega" + alias, target);
//                addFuzzy(alias + "mega", target);
//                addFuzzy("m" + alias, target);
//                addFuzzy(alias + "m", target);
//            }
//        };
//
//    // TODO
//    // For each table: items, abilities, moves, pokedex
//    // You must implement the logic to iterate over each DexTable and process as in the TypeScript code.
//    // For brevity, this is left as a comment.
//
//    // Store results
//    this->aliases = aliases_map;
//    this->fuzzy_aliases = fuzzy_aliases;
//    return this->aliases.value();
//}
//
//std::optional<DexTableData> ModdedDex::get_data_cache()
//{
//    if (data_cache.has_value())
//    {
//        return data_cache;
//    }
//    return std::nullopt;
//}
//
//int ModdedDex::get_gen() const
//{
//    return gen;
//}

const std::string& ModdedDex::get_parent_mod() const
{
	return parent_mod;
}