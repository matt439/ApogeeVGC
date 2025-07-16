#include "DexFormats.h"

// #include "../dex/IModdedDex.h"

DexFormats::DexFormats(IModdedDex* dex_ptr)
	: dex(dex_ptr)
{
}

DataType DexFormats::get_data_type() const
{
	return DataType::FORMATS_DATA;
}

DexFormats* DexFormats::load()
{
	// TODO implement this
	return nullptr;
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