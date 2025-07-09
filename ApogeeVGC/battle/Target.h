#pragma once

//#include "../pokemon/Pokemon.h"
//#include "../side/Side.h"
//#include "../field/Field.h"
//#include "Battle.h"
#include <variant>
#include <string>

// forward declarations
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