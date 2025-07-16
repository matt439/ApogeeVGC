#include "FormatData.h"

FormatData::FormatData(
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

Format(name, real_move, full_name, effect_type, exists,
    num, gen, short_desc, desc, is_nonstandard, no_copy,
    affects_fainted, source_effect, mod,
    format_effect_type, debug, rated, game_type,
    ruleset, base_ruleset, banlist, restricted, unbanlist,
    custom_rules ? std::make_unique<std::vector<std::string>>(*custom_rules) : nullptr,
    no_log)
    , EventMethods()
{
}


DataType FormatData::get_data_type() const
{
	return DataType::FORMATS_DATA;
}