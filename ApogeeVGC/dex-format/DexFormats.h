#pragma once

#include "../global-types/ID.h"
#include "../dex/IModdedDex.h"
#include "RuleTable.h"
#include "Format.h"
#include <unordered_map>
#include <memory>
#include <vector>
#include <string>

class DexFormats
{
public:
	IModdedDex* dex = nullptr;
	std::unordered_map<ID, std::unique_ptr<Format>> ruleset_cache = {};
	std::unique_ptr<std::vector<std::unique_ptr<Format>>> formats_list_cache = nullptr;

	DexFormats() = default;
	DexFormats(IModdedDex* dex_ptr);

	void load();

	std::string validate(const std::string& name) const;

	// Get by Format (returns reference)
	Format* get_format(const Format& item) const;

	// Get by name (string)
	Format* get_format(const std::string& name, bool is_trusted = false);

	//// Get by ID
	//const Format& get_format(const ID& id);

	// Get all formats
	std::vector<std::unique_ptr<Format>>* get_all_formats();

	bool is_pokemon_rule(const std::string& rule_spec) const;

	RuleTable get_rule_table(Format* format, int depth = 1, std::unordered_map<std::string, int>* repeals = nullptr);

	std::variant<std::string, std::vector<std::string>, int>
		validate_rule(const std::string& rule, Format* format = nullptr) const;

	bool valid_pokemon_tag(const ID& tag_id) const;

	bool validate_rule_ban(const std::string& rule) const;
};