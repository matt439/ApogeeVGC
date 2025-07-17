#pragma once

#include "../sim/dex-format/FormatList.h"


const static std::vector<std::string> GEN9_OU_RULESET = {
	"Standard",
	"Evasion Abilities Clause", 
	"Sleep Moves Clause", 
	"!Sleep Clause Mod",
};

const static std::vector<std::string> GEN9_OU_BANLIST = {
	"Uber",
	"AG",
	"Arena Trap",
	"Moody",
	"Shadow Tag",
	"King\'s Rock",
	"Razor Fang",
	"Baton Pass",
	"Last Respects",
	"Shed Tail",
};

const static FormatList FORMATS = {
	SectionInfo("Gen 9"),
	FormatData("[Gen 9] OU", "gen9", GEN9_OU_RULESET, GEN9_OU_BANLIST)
};