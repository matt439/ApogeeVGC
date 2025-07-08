#pragma once

#include "../global-types/function_type_aliases.h"
#include <optional>
#include <string>

struct FlingData
{
	int base_power;
	std::optional<std::string> status;
	std::optional<std::string> volatile_status;
	std::optional<ResultMoveFunc> effect;
};