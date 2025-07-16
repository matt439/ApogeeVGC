#pragma once

#include "../global-types/ID.h"
#include "../dex/IModdedDex.h"
#include "../dex/IDexDataManager.h"
#include "RuleTable.h"
#include "Format.h"
#include <unordered_map>
#include <memory>
#include <vector>
#include <string>

class DexFormats : public IDexDataManager
{
public:
	IModdedDex* dex = nullptr;
	std::unordered_map<ID, std::unique_ptr<Format>> ruleset_cache = {};
	std::unique_ptr<std::vector<std::unique_ptr<Format>>> formats_list_cache = nullptr; // nullable

	DexFormats() = default;
	DexFormats(IModdedDex* dex_ptr);

	DexFormats* load();

	/**
	 * Returns a sanitized format ID if valid, or throws if invalid.
	 */
	std::string validate(const std::string& name) const;

	/**
	 * The default mode is `isTrusted = false`, which is a bit of a
	 * footgun. PS will never do anything unsafe, but `isTrusted = true`
	 * will throw if the format string is invalid, while
	 * `isTrusted = false` will silently fall back to the original format.
	 */
	Format* get_format(const Format& item, bool is_trusted = false);
	Format* get_format(const std::string& name, bool is_trusted = false);

	////// Get by ID
	////const Format& get_format(const ID& id);

	// Get all formats
	std::vector<std::unique_ptr<Format>>* get_all_formats();

	bool is_pokemon_rule(const std::string& rule_spec) const;

	RuleTable get_rule_table(Format* format, int depth = 1,
		std::unordered_map<std::string, int>* repeals = nullptr);

	std::variant<std::string, std::vector<std::string>, int>
		validate_rule(const std::string& rule, Format* format = nullptr) const;

	bool valid_pokemon_tag(const ID& tag_id) const;

	std::string validate_rule_ban(const std::string& rule) const;

	DataType get_data_type() const override;
};