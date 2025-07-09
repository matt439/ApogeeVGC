#pragma once

#include "../global-types/function_type_aliases.h"
#include <string>

struct FlingData
{
	int base_power = 0;
	std::string* status = nullptr; // optional
	std::string* volatile_status = nullptr; // optional
	ResultMoveFunc* effect = nullptr; // optional
};