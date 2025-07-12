#include "ItemData.h"

bool ItemData::operator==(const ItemData& other) const
{
	return name == other.name;
}