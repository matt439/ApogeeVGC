#pragma once

#include "StatID.h"
// #include <unordered_map>

//using StatsTable = std::unordered_map<StatID, int>;

struct StatsTable
{
	int hp = 0;
	int atk = 0;
	int def = 0;
	int spa = 0;
	int spd = 0;
	int spe = 0;

	int get_stat(StatID id) const;
	void set_stat(StatID id, int value);
};