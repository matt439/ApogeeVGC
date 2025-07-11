#pragma once

#include "../team-validator/TeamValidator.h"
#include "../dex/IDex.h"
#include "GameTimerSettings.h"
#include "ComplexBan.h"
#include "ComplexTeamBan.h"
#include <string>
#include <vector>
#include <map>
#include <utility> // for std::pair
#include <memory>

struct RuleTable
{
	IDex* dex = nullptr; // Pointer to the parent Dex instance
	std::map<std::string, std::string> rules = {};
	std::vector<ComplexBan> complex_bans = {};
	std::vector<ComplexTeamBan> complex_team_bans = {};
	std::unique_ptr<std::pair<TeamValidator("check_can_learn", dex), std::string>> check_can_learn = nullptr; // optional
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
};

