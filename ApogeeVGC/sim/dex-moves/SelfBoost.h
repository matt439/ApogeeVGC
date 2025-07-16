#pragma once

#include "../global-types/SparseBoostsTable.h"

struct SelfBoost
{
	// Boosts to apply to the user
	std::unique_ptr<SparseBoostsTable> boosts = nullptr; //optional
};