#include "DexFormats.h"

#include "../global-types/AnyObject.h"
#include <rapidjson/document.h>
#include <rapidjson/istreamwrapper.h>
#include <stdexcept>
#include <fstream>

using namespace rapidjson;


DexFormats::DexFormats(IModdedDex* dex_ptr)
	: dex(dex_ptr)
{
}

DataType DexFormats::get_data_type() const
{
	return DataType::FORMATS_DATA;
}

std::vector<FormatList> DexFormats::load_formats_from_file(const std::string& file_path)
{
	std::vector<FormatList> formats;

	//// 1. Load the JSON file with rapidjson
	//Document doc;
	//std::ifstream ifs(file_path);
	//if (!ifs.is_open())
	//	throw std::runtime_error("Could not open formats file: " + file_path);

	//IStreamWrapper isw(ifs);
 //   doc.ParseStream(isw);
	//if (doc.HasParseError())
	//	throw std::runtime_error("Error parsing formats file: " + file_path);

	//// 2. Check if the root is an array
	//if (!doc.IsArray())
	//	throw std::runtime_error("Formats file must contain an array at the root: " + file_path);

	//// 3. Iterate through the array and convert to FormatList objects
	//for (const auto& item : doc.GetArray()) {
	//	if (!item.IsObject())
	//		throw std::runtime_error("Each format must be an object in the array: " + file_path);

	//	FormatList format;
	//	if (item.HasMember("name") && item["name"].IsString()) {
	//		format.name = item["name"].GetString();
	//	}
	//	else {
	//		throw std::runtime_error("Format must have a 'name' string field: " + file_path);
	//	}
	//	if (item.HasMember("section") && item["section"].IsString()) {
	//		format.section = item["section"].GetString();
	//	}
	//	if (item.HasMember("column") && item["column"].IsInt()) {
	//		format.column = item["column"].GetInt();
	//	}
	//	if (item.HasMember("ruleset") && item["ruleset"].IsArray()) {
	//		for (const auto& rule : item["ruleset"].GetArray()) {
	//			if (rule.IsString()) {
	//				format.ruleset.push_back(rule.GetString());
	//			}
	//			else {
	//				throw std::runtime_error("Each rule in 'ruleset' must be a string: " + file_path);
	//			}
	//		}
	//	}
	//	formats.push_back(format);
	//}
	return formats;
}

DexFormats* DexFormats::load()
{
    //// 1. Check if this is the base mod
    //if (!dex->get_is_base())
    //    throw std::runtime_error("This should only be run on the base mod");

    //// 2. Include mods
    //dex->include_mods();

    //// 3. Check cache
    //if (formats_list_cache) return this;

    //// 4. Load formats from files (pseudo-code, use your JSON library)
    //std::vector<AnyObject> formats;
    //std::vector<AnyObject> customFormats;
    //try
    //{
    //    customFormats = load_formats_from_file("config/custom-formats.json");
    //}
    //catch (const std::exception& e) {
    //    // Ignore file not found, rethrow others
    //}
    //formats = load_formats_from_file("config/formats.json");

    //// 5. Merge custom formats if present
    //if (!customFormats.empty())
    //{
    //    formats = merge_format_lists(formats, customFormats);
    //}

    //// 6. Process formats
    //std::string section;
    //int column = 1;
    //for (size_t i = 0; i < formats.size(); ++i) {
    //    auto& format = formats[i];
    //    std::string id = to_id(format.name);
    //    if (!format.name.empty() && format.section.empty()) continue;
    //    if (id.empty()) {
    //        throw std::runtime_error("Format #" + std::to_string(i + 1) + " must have a valid name");
    //    }
    //    if (format.section.empty()) format.section = section;
    //    if (format.column == 0) format.column = column;
    //    if (rulesetCache.count(id)) {
    //        throw std::runtime_error("Duplicate format ID: " + id);
    //    }
    //    // Set defaults
    //    format.effectType = "Format";
    //    format.baseRuleset = format.ruleset;
    //    if (!format.challengeShow.has_value()) format.challengeShow = true;
    //    if (!format.searchShow.has_value()) format.searchShow = true;
    //    if (!format.tournamentShow.has_value()) format.tournamentShow = true;
    //    if (!format.bestOfDefault.has_value()) format.bestOfDefault = false;
    //    if (!format.teraPreviewDefault.has_value()) format.teraPreviewDefault = false;
    //    if (format.mod.empty()) format.mod = "gen9";
    //    if (!dex->has_mod(format.mod)) {
    //        throw std::runtime_error("Format requires nonexistent mod: " + format.mod);
    //    }
    //    // Create and cache the Format object
    //    auto ruleset = std::make_unique<Format>(format);
    //    rulesetCache[id] = ruleset.get();
    //    formatsList.push_back(std::move(ruleset));
    //}

    //formatsListCache = std::move(formatsList);
    return this;
}


std::string DexFormats::validate(const std::string& name) const
{
	// TODO implement this
	return name; // placeholder
}

Format* DexFormats::get_format(const Format& item, bool is_trusted)
{
	return nullptr; // TODO implement this
}

Format* DexFormats::get_format(const std::string& name, bool is_trusted)
{
	return nullptr; // TODO implement this
}

std::vector<std::unique_ptr<Format>>* DexFormats::get_all_formats()
{
	return nullptr; // TODO implement this
}

bool DexFormats::is_pokemon_rule(const std::string& rule_spec) const
{
	// TODO implement this
	return false; // placeholder
}

RuleTable DexFormats::get_rule_table(Format* format, int depth,
	std::unordered_map<std::string, int>* repeals)
{
	// TODO implement this
	return RuleTable(); // placeholder
}

std::variant<std::string, std::vector<std::string>, int>
	DexFormats::validate_rule(const std::string& rule, Format* format) const
{
	// TODO implement this
	return std::string(); // placeholder
}

bool DexFormats::valid_pokemon_tag(const ID& tag_id) const
{
	// TODO implement this
	return false; // placeholder
}

std::string DexFormats::validate_rule_ban(const std::string& rule) const
{
	// TODO implement this
	return ""; // placeholder
}