#pragma once

#include "../pokemon/Pokemon.h"
#include "../global-types/Effect.h"
#include <variant>
#include <string>
#include <xutility>

using Source = std::variant<
	std::monostate,           // for null
	std::string,
	Pokemon*,
	Effect*,
	bool
>;                     // only 'false' is valid, document this
