#pragma once

#include <variant>
#include <string>

class Pokemon;
class Side;
class Field;
class Battle;

// For target
using Target = std::variant<
	std::monostate,           // for null
	std::string,
	Pokemon*,
	Side*,
	Field*,
	Battle*
>;