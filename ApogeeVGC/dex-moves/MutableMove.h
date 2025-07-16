#pragma once

#include "../dex-data/BasicEffect.h"
#include "MoveData.h"

struct MutableMove : BasicEffect, MoveData
{
	MutableMove() = default;
};
