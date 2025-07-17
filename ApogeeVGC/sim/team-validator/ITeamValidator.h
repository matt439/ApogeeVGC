#pragma once

#include "../dex/IModdedDex.h"
#include "../dex-format/Format.h"
#include <string>

class ITeamValidator
{
public:
	virtual ~ITeamValidator() = default;
	ITeamValidator(const std::string& format_name, IModdedDex* dex_instance);
	ITeamValidator(const Format& format_instance, IModdedDex* dex_instance);
protected:
	std::unique_ptr<Format> format = nullptr;
	IModdedDex* dex = nullptr;
};