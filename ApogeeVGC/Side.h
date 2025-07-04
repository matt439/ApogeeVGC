#pragma once

#include "Team.h"
#include <vector>
#include <memory>

class Side
{
public:
	Side() = default;
private:
	Team _team;
	std::vector<std::unique_ptr<Pokemon>> _pokemon;
	std::vector<Pokemon*> _active_pokemon;
	unsigned int _id = 0;
	int _num_pokemon_left = 0;
	Pokemon* _fainted_last_turn;
	Pokemon* _fainted_this_turn;
	int _total_fainted = 0;
};