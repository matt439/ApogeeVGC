#pragma once

#include "../dex-data/BasicEffect.h"
#include "../global-types/GameType.h"
#include "../dex/IModdedDex.h"
#include "../global-types/ModdedBattleSide.h"
#include "../global-types/ModdedBattleActions.h"
#include "../global-types/ModdedBattleScriptsData.h"
#include "../global-types/ModdedBattlePokemon.h"
#include "../global-types/ModdedBattleQueue.h"
#include "../global-types/ModdedField.h"

#include "FormatEffectType.h"
// #include "RuleTable.h"
#include <string>
#include <vector>
#include <variant>
#include <functional>
#include <memory>
#include <set>

class Battle;
//struct ModdedBattleScriptsData;
//struct ModdedBattlePokemon;
//class ModdedBattleQueue;
//class ModdedField;
//struct ModdedBattleActions;
//struct ModdedBattleSide;
class Move;
struct PokemonSet;
class PokemonSources;
class TeamValidator;
// class RuleTable;

class Format : public BasicEffect
{
public:
    std::string mod = "";
    std::unique_ptr<std::string> team = nullptr; // optional
    FormatEffectType format_effect_type{};
    bool debug = false;
    std::variant<bool, std::string> rated = true; // can be bool or string
    GameType game_type{};
    int player_count = 2;
    std::vector<std::string> ruleset = {};
    std::vector<std::string> base_ruleset = {};
    std::vector<std::string> banlist = {};
    std::vector<std::string> restricted = {};
    std::vector<std::string> unbanlist = {};
    std::unique_ptr<std::vector<std::string>> custom_rules = nullptr;

	// commented out to avoid circular dependency issues
    //std::unique_ptr<RuleTable> rule_table = nullptr; // nullable
    std::unique_ptr<std::function<void(Battle*)>> on_begin = nullptr; // optional
    bool no_log = false;

    std::unique_ptr<std::variant<bool, std::string>> has_value = nullptr; // optional; bool, "integer", or "positive-integer"
    
    
	// commented out to avoid circular dependency issues
  //  std::unique_ptr<std::function<std::variant<std::string, void>
		//(const Format&, RuleTable*, IModdedDex*, const std::string&)>> on_validate_rule = nullptr; // optional

    std::unique_ptr<std::string> mutually_exclusive_with = nullptr; // optional

    std::unique_ptr<ModdedBattleScriptsData> battle = nullptr; // optional
    std::unique_ptr<ModdedBattlePokemon> pokemon = nullptr; // optional
    std::unique_ptr<ModdedBattleQueue> queue = nullptr; // optional
    std::unique_ptr<ModdedField> field = nullptr; // optional
    std::unique_ptr<ModdedBattleActions> actions = nullptr; // optional
    std::unique_ptr<ModdedBattleSide> side = nullptr; // optional

    std::unique_ptr<bool> challenge_show = nullptr; // optional
    std::unique_ptr<bool> search_show = nullptr; // optional
    std::unique_ptr<bool> best_of_default = nullptr; // optional
    std::unique_ptr<bool> tera_preview_default = nullptr; // optional
    std::unique_ptr<std::vector<std::string>> threads = nullptr; // optional
    std::unique_ptr<bool> tournament_show = nullptr; // optional

    // Optional function hooks
    std::unique_ptr<std::function<std::unique_ptr<std::string>(
        TeamValidator*, Move*, Species*, PokemonSources*, PokemonSet*)>> check_can_learn = nullptr; // optional

    std::unique_ptr<std::function<std::string(
        const Format*, const std::string*)>> get_evo_family = nullptr; // optional

    std::unique_ptr<std::function<std::set<std::string>(
        const Format*, Pokemon*)>> get_shared_power = nullptr; // optional

    std::unique_ptr<std::function<std::set<std::string>(
        const Format*, Pokemon*)>> get_shared_items = nullptr; // optional

    std::unique_ptr<std::function<std::variant<std::vector<std::string>, void>(
        TeamValidator*, PokemonSet*, Format*, void*, void*)>> on_change_set = nullptr; // optional

    std::unique_ptr<int> on_modify_species_priority = nullptr; // optional

    std::unique_ptr<std::function<std::variant<Species, void>(
        Battle*, Species*, Pokemon*, Pokemon*, Effect*)>> on_modify_species = nullptr; // optional

    std::unique_ptr<std::function<void(Battle*)>> on_battle_start = nullptr; // optional
    std::unique_ptr<std::function<void(Battle*)>> on_team_preview = nullptr; // optional

    std::unique_ptr<std::function<std::variant<std::vector<std::string>, void>(
        TeamValidator*, PokemonSet*, Format*, void*, void*)>> on_validate_set = nullptr; // optional

    std::unique_ptr<std::function<std::variant<std::vector<std::string>, void>(
        TeamValidator*, std::vector<PokemonSet>*, Format*, void*)>> on_validate_team = nullptr; // optional

    std::unique_ptr<std::function<std::unique_ptr<std::vector<std::string>>(
        TeamValidator*, PokemonSet*, void*)>> validate_set = nullptr; // optional

    std::unique_ptr<std::function<std::variant<std::vector<std::string>, void>(
        TeamValidator*, std::vector<PokemonSet>*, void*)>> validate_team = nullptr; // optional

    std::unique_ptr<std::string> section = nullptr; // optional
    std::unique_ptr<int> column = nullptr; // optional

    Format() = default;

    Format(
        // BasicEffect non-optional
        const std::string& name,
        const std::string& real_move = "",
        const std::string& full_name = "",
        //EffectType effect_type = EffectType::CONDITION,
        bool exists = true,
        int num = 0,
        int gen = 0,
        const std::string& short_desc = "",
        const std::string& desc = "",
        NonStandard is_nonstandard = NonStandard::NONE,
        bool no_copy = false,
        bool affects_fainted = false,
        const std::string& source_effect = "",
		// Format non-optional
        const std::string& mod = "gen9",
        FormatEffectType format_effect_type = FormatEffectType::FORMAT,
        bool debug = false,
        std::variant<bool, std::string> rated = true,
        GameType game_type = GameType::SINGLES,
        const std::vector<std::string>& ruleset = {},
        const std::vector<std::string>& base_ruleset = {},
        const std::vector<std::string>& banlist = {},
        const std::vector<std::string>& restricted = {},
        const std::vector<std::string>& unbanlist = {},
        std::unique_ptr<std::vector<std::string>> custom_rules = nullptr,
        bool no_log = false);

    Format(const Format& other);
        
};
