#pragma once

#include "../dex-moves/MoveTarget.h"
#include <string>

// custom struct
struct ZMoveOption
{
	std::string move = "";
	MoveTarget target = MoveTarget::ADJACENT_FOE;

	ZMoveOption() = default;
	ZMoveOption(const ZMoveOption&) = default;
};