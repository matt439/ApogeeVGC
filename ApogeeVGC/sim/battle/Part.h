#pragma once

//#include "../pokemon/Pokemon.h"
//#include "../side/Side.h"
//#include "../dex-moves/Move.h"
//#include "../global-types/Effect.h"
#include <variant>
#include <string>

// forward declarations
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