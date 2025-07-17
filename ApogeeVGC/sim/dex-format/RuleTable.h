#pragma once

#include "../team-validator/ITeamValidator.h"
#include "../dex/IDex.h"
#include "../dex-species/Species.h"
#include "GameTimerSettings.h"
#include "ComplexBan.h"
#include "ComplexTeamBan.h"
#include <string>
#include <vector>
#include <map>
#include <utility> // for std::pair
#include <memory>

// class TeamValidator; // Forward declaration

struct RuleTable
{
	IDex* dex = nullptr; // Pointer to the parent Dex instance
	std::map<std::string, std::string> rules = {};
	std::vector<ComplexBan> complex_bans = {};
	std::vector<ComplexTeamBan> complex_team_bans = {};

	//this is commented out to avoid circular dependency issues
	std::unique_ptr<std::pair<std::unique_ptr<ITeamValidator>, std::string>>
		check_can_learn = nullptr; // optional

	std::unique_ptr<std::pair<GameTimerSettings, std::string>> timer = nullptr; // optional
	std::vector<std::string> tag_rules = {};
	std::map<std::string, std::string> value_rules = {};
    int min_team_size = 0;
    int max_team_size = 0;
	std::unique_ptr<int> picked_team_size = nullptr;
	std::unique_ptr<int> max_total_level = nullptr;
    int max_move_count = 0;
    int min_source_gen = 0;
    int min_level = 0;
    int max_level = 0;
    int default_level = 0;
	std::unique_ptr<int> adjust_level = nullptr; // optional
	std::unique_ptr<int> adjust_level_down = nullptr; // optional
	std::unique_ptr<int> ev_limit = nullptr; // optional

	RuleTable() = default;

	bool is_banned(const std::string& thing) const;

	bool is_banned_species(const Species& species) const;

	bool is_restricted(const std::string& thing) const;

	bool is_restricted_species(const Species& species) const;

	std::vector<std::string> get_tag_rules() const;

	/**
	 * - non-empty string: banned, string is the reason
	 * - '': whitelisted
	 * - null: neither whitelisted nor banned
	 */
	std::string* check(const std::string& thing,
		const std::unordered_map<std::string, bool>* set_has = nullptr);

	std::string* get_reason(const std::string& key);

	std::string blame(const std::string& key) const;

	int get_complex_ban_index(const std::vector<ComplexBan>& complex_bans, const std::string& rule) const;

	void add_complex_ban(const std::string& rule, const std::string& source, int limit,
		const std::vector<std::string>& bans);

	void add_complex_team_ban(const std::string& rule, const std::string& source, int limit,
		const std::vector<std::string>& bans);

	/** After a RuleTable has been filled out, resolve its hardcoded numeric properties */
	void resolve_numbers(const Format& format, IModdedDex* dex);

	bool has_complex_bans() const;
};

