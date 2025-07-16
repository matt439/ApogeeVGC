#pragma once

#include "../dex/IDexData.h"
#include "../dex-conditions/EventMethods.h"
#include "Format.h"

struct FormatData : public Format, public EventMethods, public IDexData
{
	FormatData() = default;

    FormatData(
        // BasicEffect non-optional
        const std::string& name,
        const std::string& real_move = "",
        const std::string& full_name = "",
        EffectType effect_type = EffectType::CONDITION,
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
        FormatEffectType format_effect_type = FormatEffectType::CONDITION,
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

	DataType get_data_type() const override;
};