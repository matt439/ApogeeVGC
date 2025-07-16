#include "Format.h"

Format::Format(
    // BasicEffect non-optional
    const std::string& name,
    const std::string& real_move,
    const std::string& full_name,
    EffectType effect_type,
    bool exists,
    int num,
    int gen,
    const std::string& short_desc,
    const std::string& desc,
    NonStandard is_nonstandard,
    bool no_copy,
    bool affects_fainted,
    const std::string& source_effect,
	// Format-specific
    const std::string& mod,
    FormatEffectType format_effect_type,
    bool debug,
    std::variant<bool, std::string> rated,
    GameType game_type,
    const std::vector<std::string>& ruleset,
    const std::vector<std::string>& base_ruleset,
    const std::vector<std::string>& banlist,
    const std::vector<std::string>& restricted,
    const std::vector<std::string>& unbanlist,
    std::unique_ptr<std::vector<std::string>> custom_rules,
    bool no_log) :

    BasicEffect(name, real_move, full_name, effect_type, exists,
        num, gen, short_desc, desc, is_nonstandard, no_copy,
        affects_fainted, source_effect),

    mod(mod),
	format_effect_type(format_effect_type),
	debug(debug),
	rated(rated),
	game_type(game_type),
	ruleset(ruleset),
	base_ruleset(base_ruleset),
	banlist(banlist),
	restricted(restricted),
	unbanlist(unbanlist),
	custom_rules(custom_rules ? std::make_unique<std::vector<std::string>>(*custom_rules) : nullptr),
	no_log(no_log)

{
    if (game_type == GameType::MULTI || game_type == GameType::FREEFORALL)
        player_count = 4;
    else
        player_count = 2;
}


Format::Format(const Format& other) :
	BasicEffect(other),
	mod(other.mod),
	format_effect_type(other.format_effect_type),
	debug(other.debug),
	rated(other.rated),
	game_type(other.game_type),
	player_count(other.player_count),
	ruleset(other.ruleset),
	base_ruleset(other.base_ruleset),
	banlist(other.banlist),
	restricted(other.restricted),
	unbanlist(other.unbanlist),
	custom_rules(other.custom_rules ? std::make_unique<std::vector<std::string>>(*other.custom_rules) : nullptr),
	no_log(other.no_log)
{
}