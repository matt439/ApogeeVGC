#include "Format.h"

Format::Format(
    std::string mod,
    FormatEffectType format_effect_type,
    bool debug,
    std::variant<bool, std::string> rated,
    GameType game_type,
    std::vector<std::string> ruleset,
    std::vector<std::string> base_ruleset,
    std::vector<std::string> banlist,
    std::vector<std::string> restricted,
    std::vector<std::string> unbanlist,
    std::unique_ptr<std::vector<std::string>> custom_rules,
    bool no_log,
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
    const std::string& source_effect) :

    BasicEffect(name, real_move, full_name, effect_type, exists,
        num, gen, short_desc, desc, is_nonstandard, no_copy,
        affects_fainted, source_effect),

    mod(std::move(mod)),
    format_effect_type(format_effect_type), debug(debug), rated(std::move(rated)),
    game_type(game_type), ruleset(std::move(ruleset)),
    base_ruleset(std::move(base_ruleset)), banlist(std::move(banlist)),
    restricted(std::move(restricted)), unbanlist(std::move(unbanlist)),
    custom_rules(std::move(custom_rules)), no_log(no_log)
{
    if (game_type == GameType::MULTI || game_type == GameType::FREEFORALL)
        player_count = 4;
    else
        player_count = 2;

}
