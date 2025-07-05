#pragma once

#include "BasicEffect.h"
#include "MoveData.h"

struct MutableMove : public BasicEffect, public MoveData
{
	// Additional fields or methods can be added here if needed
};

struct ActiveMove : public MutableMove
{
};