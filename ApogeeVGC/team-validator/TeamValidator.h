#pragma once

#include "../dex-format/Format.h"
#include "../dex/ModdedDex.h"
#include "../dex-format/RuleTable.h"
#include "../dex-data/to_id.h"
#include "../dex/IDex.h"
#include <stdexcept>
#include <string>
#include <functional>

class TeamValidator
{
public:
    Format format;
    ModdedDex dex;
    int gen = 0;
    RuleTable rule_table;
    int min_source_gen = 0;

    TeamValidator(const std::string& format_name, IDex* dex_instance);
	TeamValidator(const Format& format_instance, IDex* dex_instance);
};
