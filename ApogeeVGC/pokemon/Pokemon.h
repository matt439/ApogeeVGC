#pragma once

#include "../teams/PokemonSet.h"
#include "MoveSlot.h"

class Pokemon
{
public:
	Pokemon() = default;
private:
	// side
	// battle
	PokemonSet _set;
	std::vector<MoveSlot> _move_slots;
	std::string _hp_type;
	int _hp_power;
	int _position;
	std::string _details;
	//Species _species;
};