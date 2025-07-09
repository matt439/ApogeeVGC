#pragma once


#include "../global-types/Effect.h"
#include <variant>
#include <string>

class Pokemon;

using Source = std::variant<
	std::monostate,           // for null
	std::string,
	Pokemon*,
	Effect*,
	bool
>;                     // only 'false' is valid, document this
