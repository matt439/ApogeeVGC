#pragma once

#include <variant>
#include <string>

class Pokemon;
class Side;
class Effect;
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