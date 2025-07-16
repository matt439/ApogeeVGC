#pragma once

#include "../global-types/IDEntry.h"
#include "../global-types/SparseBoostsTable.h"
#include <memory>

struct ZMoveData
{
	std::unique_ptr<int> base_power = nullptr; // Base power of the Z-move, optional
	std::unique_ptr<IDEntry> effect = nullptr; // Effect ID for the Z-move, optional
	std::unique_ptr<SparseBoostsTable> boost = nullptr; // Boosts to apply when using the Z-move, optional
};