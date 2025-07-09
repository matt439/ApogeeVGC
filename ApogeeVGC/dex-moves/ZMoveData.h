#pragma once

#include "../global-types/type_aliases.h"
#include <optional>

struct ZMoveData
{
	std::optional<int> base_power = std::nullopt; // Base power of the Z-move
	std::optional<IDEntry> effect = std::nullopt; // Effect ID for the Z-move
	std::optional<SparseBoostsTable> boost = std::nullopt; // Boosts to apply when using the Z-move
};