#pragma once

#include "../dex-format/Format.h"
#include "../dex/IModdedDex.h"
#include "../dex-format/RuleTable.h"
#include "ITeamValidator.h"
// #include "../dex-data/to_id.h"
// #include "../dex/IDex.h"
// #include <stdexcept>
#include <string>
#include <memory>
// #include <functional>

//class Format;
//class IModdedDex;
// class RuleTable;

class TeamValidator : public ITeamValidator
{
public:
	//std::unique_ptr<Format> format = nullptr;
	//IModdedDex* dex = nullptr;
    int gen = 0;
	std::unique_ptr<RuleTable> rule_table = nullptr;
    int min_source_gen = 0;

    TeamValidator(const std::string& format_name, IModdedDex* dex_instance);
	TeamValidator(const Format& format_instance, IModdedDex* dex_instance);
};
