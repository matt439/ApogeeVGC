#pragma once

#include "../global-types/function_type_aliases.h"
#include <optional>
#include <string>

struct FlingData
{
	int base_power = 0;
	std::optional<std::string> status = std::nullopt;
	std::optional<std::string> volatile_status = std::nullopt;
	std::optional<ResultMoveFunc> effect = std::nullopt;
};