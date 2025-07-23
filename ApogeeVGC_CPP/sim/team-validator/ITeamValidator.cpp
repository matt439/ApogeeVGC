#include "ITeamValidator.h"


ITeamValidator::ITeamValidator(const std::string& format_name, IModdedDex* dex_instance) :
	dex(dex_instance)
{

}

ITeamValidator::ITeamValidator(const Format& format_instance, IModdedDex* dex_instance) :
	dex(dex_instance), format(std::make_unique<Format>(format_instance))
{

}