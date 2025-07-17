#include "StatsTable.h"

#include <stdexcept>

int StatsTable::get_stat(StatID id) const
{
	switch (id)
	{
	case StatID::HP: return hp;
	case StatID::ATK: return atk;
	case StatID::DEF: return def;
	case StatID::SPA: return spa;
	case StatID::SPD: return spd;
	case StatID::SPE: return spe;
	default:
		throw std::invalid_argument("Invalid StatID");
		break;
	}
}

void StatsTable::set_stat(StatID id, int value)
{
	switch (id)
	{
	case StatID::HP: hp = value; break;
	case StatID::ATK: atk = value; break;
	case StatID::DEF: def = value; break;
	case StatID::SPA: spa = value; break;
	case StatID::SPD: spd = value; break;
	case StatID::SPE: spe = value; break;
	default:
		throw std::invalid_argument("Invalid StatID");
		break;
	}
}