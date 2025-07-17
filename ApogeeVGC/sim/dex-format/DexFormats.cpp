#include "DexFormats.h"

#include "../global-types/AnyObject.h"
#include "../dex-data/to_id.h"
#include "SectionInfo.h"
#include "FormatData.h"
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