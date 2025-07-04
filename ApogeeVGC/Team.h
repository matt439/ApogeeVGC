#pragma once

#include <vector>
#include "Trainer.h"
#include "Pokemon.h"

class Team
{
public:
	Team() = default;

private:
	Trainer _trainer;
	std::vector<Pokemon> _pokemon;
};