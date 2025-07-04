#pragma once

#include "Field.h"
#include "Format.h"
#include "Side.h"

class Battle
{
public:
	Battle() = default;
private:
	unsigned int _id = 0;
	unsigned int _seed = 0;
	Field _field;
	Format _format;
	Side _side_a;
	Side _side_b;
};
