#pragma once

#include <variant>
#include <string>

class Pokemon;
class Effect;

using Source = std::variant<
	std::monostate,           // for null
	std::string,
	Pokemon*,
	Effect*,
	bool
>;                     // only 'false' is valid, document this
