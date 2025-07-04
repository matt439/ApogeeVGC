#pragma once

enum class StatType
{
	HP,
	ATK,
	DEF,
	SPA,
	SPD,
	SPE
};

#include <unordered_map>

using StatsTable = std::unordered_map<StatType, int>;