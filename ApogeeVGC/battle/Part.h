#pragma once

#include "../global-types/Effect.h"
#include <variant>
#include <string>

class Pokemon;
class Side;
class Move;

using Part = std::variant<
	std::monostate,           // for null/undefined
	std::string,
	int,                   // for number
	bool,
	Pokemon*,
	Side*,
	Effect*,
	Move*
>;