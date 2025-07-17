#include "DexFormats.h"

#include "../global-types/AnyObject.h"
#include "../dex-data/to_id.h"
#include "SectionInfo.h"
#include "FormatData.h"
#include "join.h"
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

FormatList DexFormats::load_formats_from_file(const std::string& file_path)
{
	FormatList list = {};

	// 1. Load the JSON file with rapidjson
	Document doc;
	std::ifstream ifs(file_path);
	if (!ifs.is_open())
		throw std::runtime_error("Could not open formats file: " + file_path);

	IStreamWrapper isw(ifs);
    doc.ParseStream(isw);
	if (doc.HasParseError())
		throw std::runtime_error("Error parsing formats file: " + file_path);

	// 2. Check if the root is an array
	if (!doc.IsArray())
		throw std::runtime_error("Formats file must contain an array at the root: " + file_path);

	// 3. Iterate through the array and convert to FormatList objects
	for (const auto& item : doc.GetArray()) {
		if (!item.IsObject())
			throw std::runtime_error("Each format must be an object in the array: " + file_path);

		// try to convert SectionInfo from JSON object
        try
        {
			std::unique_ptr<SectionInfo> section = std::make_unique<SectionInfo>(item);
			list.push_back(std::move(section));
		}
		catch (const std::exception&)
        {
			// try to convert FormatData from JSON object
            try
            {
				std::unique_ptr<FormatData> format = std::make_unique<FormatData>(item);
				list.push_back(std::move(format));
            }
            catch (const std::exception& e2)
            {
                throw std::runtime_error("Failed to parse format item: " + std::string(e2.what()));
            }
        }
	}
	return list;
}

FormatList DexFormats::merge_format_lists(const FormatList& list1, const FormatList& list2) const
{
	FormatList merged;
	for (const auto& item : list1)
	{
		if (item)
		{
			merged.push_back(item->clone_list_entry());
		}
	}
	for (const auto& item : list2)
	{
		if (item)
		{
			merged.push_back(item->clone_list_entry());
		}
	}
	return merged;
}

DexFormats* DexFormats::load()
{
    // 1. Check if this is the base mod
    if (!dex->get_is_base())
        throw std::runtime_error("This should only be run on the base mod");

    // 2. Include mods
    dex->include_mods();

    // 3. Check cache
    if (formats_list_cache) return this;

	std::unique_ptr<std::vector<std::unique_ptr<Format>>> formats_list =
        std::make_unique<std::vector<std::unique_ptr<Format>>>();

    // 4. Load formats from files (pseudo-code, use your JSON library)
    FormatList formats;
    FormatList custom_formats;
    try
    {
        custom_formats = load_formats_from_file("config/custom-formats.json");
    }
    catch (const std::exception& e) {
        // Ignore file not found, rethrow others
    }
    formats = load_formats_from_file("config/formats.json");

    // 5. Merge custom formats if present
    if (!custom_formats.empty())
    {
        formats = merge_format_lists(formats, custom_formats);
    }

    // 6. Process formats
    std::string section;
    int column = 1;
    for (size_t i = 0; i < formats.size(); ++i) {
        auto& format = formats[i];

		switch (format->get_type())
		{
		case FormatListEntryType::SECTION_INFO:
		{
			// If it's a SectionInfo, set section and column
			SectionInfo* section_info = dynamic_cast<SectionInfo*>(format.get());
			if (!section_info)
				throw std::runtime_error("Invalid section info at index " + std::to_string(i));

			section = section_info->section;
			column = *section_info->column;
			continue; // Skip to next iteration
		}
		case FormatListEntryType::FORMAT_DATA:
		{
			// If it's a FormatData, continue processing
			FormatData* format_data = dynamic_cast<FormatData*>(format.get());
			if (!format_data)
				throw std::runtime_error("Invalid format data at index " + std::to_string(i));

			std::string id = to_id(*format_data->name);
			if (ruleset_cache.count(id))
			{
				throw std::runtime_error("Duplicate format ID: " + id);
			}

			format_data->id = id;
			format_data->effect_type = std::make_unique<EffectType>(EffectType::FORMAT);
			format_data->base_ruleset = format_data->ruleset;
			if (!format_data->challenge_show) format_data->challenge_show = std::make_unique<bool>(true);
			if (!format_data->search_show) format_data->search_show = std::make_unique<bool>(true);
			if (!format_data->tournament_show) format_data->tournament_show = std::make_unique<bool>(true);
			if (!format_data->best_of_default) format_data->best_of_default = std::make_unique<bool>(false);
			if (!format_data->tera_preview_default) format_data->tera_preview_default = std::make_unique<bool>(false);
			if (format_data->mod.empty()) format_data->mod = "gen9";
			if (!dex->has_mod(format_data->mod))
				throw std::runtime_error("Format requires nonexistent mod: " + format_data->mod);

			// Make a Format object for the ruleset cache
			std::unique_ptr<Format> format_ptr = std::make_unique<Format>(*format_data);
			ruleset_cache.emplace(format_ptr->id, std::move(format_ptr));

			// Make another Format object for the formats list
			format_ptr = std::make_unique<Format>(*format_data);
			formats_list->push_back(std::move(format_ptr));
			break;
		}
		default:
			throw std::runtime_error("Invalid format type at index " + std::to_string(i));
		};
    }
    formats_list_cache = std::move(formats_list);
    return this;
}

std::string DexFormats::validate(const std::string& name)
{
    // 1. Split the name on "@@@"
    size_t split_pos = name.find("@@@");
    std::string format_name = name.substr(0, split_pos);
    std::string custom_rules_string = (split_pos != std::string::npos) ? name.substr(split_pos + 3) : "";

    // 2. Look up the format
    Format* format = this->get_format(format_name, false);
    if (!format || *format->effect_type != EffectType::FORMAT)
        throw std::runtime_error("Unrecognized format \"" + format_name + "\"");

    // 3. If no custom rules, return format id
    if (custom_rules_string.empty())
        return format->id;

    // 4. Get the rule table
    RuleTable rule_table = this->get_rule_table(const_cast<Format*>(format));

    bool has_custom_rules = false;
    bool has_pokemon_rule = false;
    std::vector<std::string> custom_rules;
    size_t start = 0, end;
    while ((end = custom_rules_string.find(',', start)) != std::string::npos) {
        std::string rule = custom_rules_string.substr(start, end - start);
        start = end + 1;
        // Clean up whitespace
        rule.erase(remove_if(rule.begin(), rule.end(), ::isspace), rule.end());

        // Validate the rule
        auto rule_spec = this->validate_rule(rule, const_cast<Format*>(format));
        if (std::holds_alternative<std::string>(rule_spec)) {
            const std::string& rule_str = std::get<std::string>(rule_spec);
            if (rule_str == "-pokemontag:allpokemon" || rule_str == "+pokemontag:allpokemon") {
                if (has_pokemon_rule)
                    throw std::runtime_error("You can't ban/unban pokemon before banning/unbanning all Pokemon.");
            }
            if (this->is_pokemon_rule(rule_str))
                has_pokemon_rule = true;
        }
        // If not a string or not in rule_table, mark as custom
        if (!std::holds_alternative<std::string>(rule_spec) ||
            rule_table.rules.find(std::get<std::string>(rule_spec)) == rule_table.rules.end())
            has_custom_rules = true;

        custom_rules.push_back(rule);
    }
    // Last rule (if any)
    if (start < custom_rules_string.size()) {
        std::string rule = custom_rules_string.substr(start);
        rule.erase(remove_if(rule.begin(), rule.end(), ::isspace), rule.end());
        auto rule_spec = this->validate_rule(rule, const_cast<Format*>(format));
        if (std::holds_alternative<std::string>(rule_spec)) {
            const std::string& rule_str = std::get<std::string>(rule_spec);
            if (rule_str == "-pokemontag:allpokemon" || rule_str == "+pokemontag:allpokemon") {
                if (has_pokemon_rule)
                    throw std::runtime_error("You can't ban/unban pokemon before banning/unbanning all Pokemon.");
            }
            if (this->is_pokemon_rule(rule_str))
                has_pokemon_rule = true;
        }
        if (!std::holds_alternative<std::string>(rule_spec) ||
            rule_table.rules.find(std::get<std::string>(rule_spec)) == rule_table.rules.end())
            has_custom_rules = true;

        custom_rules.push_back(rule);
    }

    if (!has_custom_rules)
        throw std::runtime_error("None of your custom rules change anything");

    // Build the validated format id
    std::string validated_format_id = format->id + "@@@" + join(custom_rules, ",");
    const Format* modded_format = this->get_format(validated_format_id, true);
    this->get_rule_table(const_cast<Format*>(modded_format));
    return validated_format_id;
}

Format* DexFormats::get_format(const Format& item, bool is_trusted)
{
	if (item.name->empty())
		return nullptr; // If name is empty, return nullptr

    return this->get_format(*item.name, is_trusted);
}

Format* DexFormats::get_format(const std::string& name, bool is_trusted)
{
    // 1. If name is empty, return nullptr
    if (name.empty()) return nullptr;

    // 2. Trim and normalize name to id
    std::string trimmed_name = name;
    // (Assume a trim function or use std::string_view if available)
    // For simplicity, just use as-is
    std::string id = to_id(trimmed_name);

    // 3. If no custom rules, check ruleset cache
    if (name.find("@@@") == std::string::npos) {
        auto it = ruleset_cache.find(id);
        if (it != ruleset_cache.end()) {
            return it->second.get();
        }
    }

    // 4. Resolve alias
    // (Assume dex->get_alias returns empty string if no alias)
    std::string alias = *dex->get_alias(id);
    if (!alias.empty()) {
        id = alias;
        trimmed_name = id;
    }

    // 5. Handle default mod prefix (assume DEFAULT_MOD is "gen9")
    const std::string default_mod = "gen9";
    if (dex->exists_in_dex_table_data(DataType::RULESETS, default_mod + id))
    {
        id = default_mod + id;
    }

   // // 6. Supplementary attributes for custom rules
   // AnyObject supplementary_attributes;
   // std::string base_name = trimmed_name;
   // std::string custom_rules_string;
   // size_t split_pos = name.find("@@@");
   // if (split_pos != std::string::npos) {
   //     base_name = name.substr(0, split_pos);
   //     custom_rules_string = name.substr(split_pos + 3);

   //     if (!is_trusted) {
   //         try {
   //             std::string validated = this->validate(name);
   //             // Recursively call with trusted = true
   //             return this->get_format(validated, true);
   //         }
   //         catch (...) {
   //             // Ignore validation errors
   //         }
   //     }
   //     if (is_trusted && !custom_rules_string.empty()) {
   //         // Parse custom rules
   //         std::vector<std::string> custom_rules;
   //         size_t start = 0, end;
   //         while ((end = custom_rules_string.find(',', start)) != std::string::npos) {
   //             custom_rules.push_back(custom_rules_string.substr(start, end - start));
   //             start = end + 1;
   //         }
   //         if (start < custom_rules_string.size()) {
   //             custom_rules.push_back(custom_rules_string.substr(start));
   //         }
			//// create an array in rapidjson format

			//rapidjson::Value custom_rules_array(rapidjson::kArrayType);
			//for (const auto& rule : custom_rules)
   //         {
			//	rapidjson::Value rule_value;
			//	rule_value.SetString(rule.c_str(), rule.size(), dex->get_allocator());
			//	custom_rules_array.PushBack(rule_value, dex->get_allocator());
			//}
   //         for (auto& rule : custom_rules)
   //         {
   //             supplementary_attributes["customRules"].data;
			//}
   //         supplementary_attributes["searchShow"].data = false;
   //     }
   // }

    // 7. Construct the Format object
    if (dex->exists_in_dex_table_data(DataType::RULESETS, id))
    {
        // Merge ruleset data and supplementary attributes
        IDexData* base_data = dex->get_from_dex_table_data(DataType::RULESETS, id);
		if (!base_data) {
			throw std::runtime_error("Format not found: " + id);
		}
		FormatData* format_data = dynamic_cast<FormatData*>(base_data);
        //// Apply supplementary attributes if any
        //if (!supplementary_attributes.empty()) {
        //    if (supplementary_attributes.count("customRules")) {
        //        base_data.custom_rules = std::make_unique<std::vector<std::string>>(
        //            std::any_cast<std::vector<std::string>>(supplementary_attributes["customRules"]));
        //    }
        //    if (supplementary_attributes.count("searchShow")) {
        //        base_data.search_show = std::make_unique<bool>(false);
        //    }
        //}
        // Construct and return a Format object
        return format_data;
    }
    else
    {
        return nullptr;
    }
}



std::vector<std::unique_ptr<Format>>* DexFormats::get_all_formats()
{
    load();
	return formats_list_cache.get();
}

bool DexFormats::is_pokemon_rule(const std::string& rule_spec) const
{
    if (rule_spec.size() <= 1)
        return false;

    std::string_view s = std::string_view(rule_spec).substr(1);
    return s.rfind("pokemontag:", 0) == 0 ||
        s.rfind("pokemon:", 0) == 0 ||
        s.rfind("basepokemon:", 0) == 0;
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